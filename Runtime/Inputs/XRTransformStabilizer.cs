using Unity.Mathematics;
#if BURST_PRESENT
using Unity.Burst;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
    /// <summary>
    /// Provides low-latency stabilization for XR pose inputs, especially useful on rays.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    [AddComponentMenu("XR/XR Transform Stabilizer", 11)]
    [HelpURL(XRHelpURLConstants.k_XRTransformStabilizer)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_TransformStabilizer)]
    public class XRTransformStabilizer : MonoBehaviour
    {
        const float k_90FPS = 1f / 90f;

        [SerializeField]
        [Tooltip("The Transform component whose position and rotation will be matched and stabilized.")]
        Transform m_Target;

        /// <summary>
        /// The <see cref="Transform"/> component whose position and rotation will be matched and stabilized.
        /// </summary>
        public Transform targetTransform
        {
            get => m_Target;
            set => m_Target = value;
        }

        [SerializeField]
        [Tooltip("If enabled, will read the target and apply stabilization in local space. Otherwise, in world space.")]
        bool m_UseLocalSpace;

        /// <summary>
        /// If enabled, will read the target and apply stabilization in local space. Otherwise, in world space.
        /// </summary>
        public bool useLocalSpace
        {
            get => m_UseLocalSpace;
            set => m_UseLocalSpace = value;
        }

        [Header("Stabilization Parameters")]
        [SerializeField]
        [Tooltip("Maximum distance (in degrees) that stabilization will be applied.")]
        float m_AngleStabilization = 20f;

        /// <summary>
        /// Maximum distance (in degrees) that stabilization will be applied.
        /// </summary>
        public float angleStabilization
        {
            get => m_AngleStabilization;
            set => m_AngleStabilization = value;
        }

        [SerializeField]
        [Tooltip("Maximum distance (in meters) that stabilization will be applied.")]
        float m_PositionStabilization = 0.25f;

        /// <summary>
        /// Maximum distance (in meters) that stabilization will be applied.
        /// </summary>
        public float positionStabilization
        {
            get => m_PositionStabilization;
            set => m_PositionStabilization = value;
        }

        Transform m_ThisTransform;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_ThisTransform = transform;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            if (m_UseLocalSpace)
            {
                m_ThisTransform.localPosition = m_Target.localPosition;
                m_ThisTransform.localRotation = m_Target.localRotation;
            }
            else
            {
                m_ThisTransform.SetPositionAndRotation(m_Target.position, m_Target.rotation);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            var currentPosition = m_UseLocalSpace ? m_ThisTransform.localPosition : m_ThisTransform.position;
            var currentRotation = m_UseLocalSpace ? m_ThisTransform.localRotation : m_ThisTransform.rotation;
            var targetPosition = m_UseLocalSpace ? m_Target.localPosition : m_Target.position;
            var targetRotation = m_UseLocalSpace ? m_Target.localRotation : m_Target.rotation;

            StabilizeTransform(currentPosition, currentRotation, targetPosition, targetRotation, Time.deltaTime, m_PositionStabilization, m_AngleStabilization, 
                out var resultPosition, out var resultRotation);
            
            if (m_UseLocalSpace)
            {
                m_ThisTransform.localPosition = resultPosition;
                m_ThisTransform.localRotation = resultRotation;
            }
            else
            {
                m_ThisTransform.SetPositionAndRotation(resultPosition, resultRotation);
            }
        }
        
#if BURST_PRESENT
        [BurstCompile]
#endif
        static void StabilizeTransform(in Vector3 startPos, in Quaternion startRot, in Vector3 targetPos, in Quaternion targetRot, float deltaTime, float positionStabilization, float angleStabilization, out Vector3 resultPos, out Quaternion resultRot)
        {
            // Calculate the stabilized position
            var positionOffset = targetPos - startPos;
            var positionDistance = positionOffset.magnitude;
            var positionLerp = CalculateStabilizedLerp(positionDistance / positionStabilization, deltaTime);

            // Calculate the stabilized rotation
            var rotationOffset = Quaternion.Angle(targetRot, startRot);
            var rotationLerp = CalculateStabilizedLerp(rotationOffset / angleStabilization, deltaTime);

            resultPos = Vector3.Lerp(startPos, targetPos, positionLerp);
            resultRot = Quaternion.Slerp(startRot, targetRot, rotationLerp);
        }

        /// <summary>
        /// Calculates a lerp value for stabilizing between a historic and current value based on their distance.
        /// The historic value is weighted more heavily the closer the distance is to 0.
        /// At a distance greater than 1, the current value is used.
        /// This filters out jitter when input is trying to be held still or moved slowly while preserving low latency for large movement.
        /// </summary>
        /// <param name="distance">The distance between a historic and current value of motion or input.</param>
        /// <param name="timeSlice">How much time has passed between when these values were recorded.</param>
        /// <returns>Returns the stabilized lerp value.</returns>
#if BURST_PRESENT
        [BurstCompile]
#endif
        static float CalculateStabilizedLerp(float distance, float timeSlice)
        {
            // The original angle stabilization code just used distance directly
            // This feels great in VR but is frame-dependent on experiences running at 90 fps
            //return Mathf.Clamp01(distance);

            // We can estimate a time-independent analog
            var originalLerp = distance;

            // If the distance has moved far enough, just use the current value for low latency movement
            if (originalLerp >= 1f)
                return 1f;

            // If the values haven't changed, then it doesn't matter what the value is so we'll just use the historic one
            if (originalLerp <= 0f)
                return 0f;

            // For fps higher than 90 fps, we scale this value
            // For fps lower than 90 fps, we take advantage of the fact that each time this algorithm
            // runs with the same values, the remaining lerp distance squares itself
            // We estimate this up to 3 time slices.  At that point the numbers just get too small to be useful
            // (and any VR experience running at 30 fps is going to be pretty rough, even with re-projection)
            var doubleFrameLerp = originalLerp - originalLerp * originalLerp;
            var tripleFrameLerp = doubleFrameLerp * doubleFrameLerp;

            var localTimeSlice = timeSlice / k_90FPS;

            var firstSlice = math.clamp(localTimeSlice, 0f, 1f);
            var secondSlice = math.clamp(localTimeSlice - 1f, 0f, 1f);
            var thirdSlice = math.clamp(localTimeSlice - 2f, 0f, 1f);

            return originalLerp * firstSlice + doubleFrameLerp * secondSlice + tripleFrameLerp * thirdSlice;
        }
    }
}

