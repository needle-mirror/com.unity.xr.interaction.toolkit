using Unity.Mathematics;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Makes the GameObject this component is attached to follow a target with a delay and some other layout options.
    /// </summary>
    [AddComponentMenu("XR/Lazy Follow", 22)]
    [HelpURL(XRHelpURLConstants.k_LazyFollow)]
    public class LazyFollow : MonoBehaviour
    {
        [SerializeField, Tooltip("(Optional) The object being followed. If not set, this will default to the main camera when this component is enabled.")]
        Transform m_Target;

        /// <summary>
        /// The object being followed. If not set, this will default to the main camera when this component is enabled.
        /// </summary>
        public Transform target
        {
            get => m_Target;
            set => m_Target = value;
        }

        [SerializeField, Tooltip("The amount to offset the target's position when following. This position is relative/local to the target object.")]
        Vector3 m_TargetOffset = Vector3.forward;

        /// <summary>
        /// The amount to offset the target's position when following. This position is relative/local to the target object.
        /// </summary>
        public Vector3 targetOffset
        {
            get => m_TargetOffset;
            set => m_TargetOffset = value;
        }

        [SerializeField, Tooltip("The laziness or smoothing that is applied to the follow movement. Higher values result in direct following, lower values will cause this object to follow more lazily.")]
        float m_MovementSpeed = 7f;

        /// <summary>
        /// The laziness or smoothing that is applied to the follow movement. Higher values result in direct following, lower values will cause this object to follow more lazily.
        /// </summary>
        public float movementSpeed
        {
            get => m_MovementSpeed;
            set => m_MovementSpeed = value;
        }

        [SerializeField, Tooltip("Snap to target position when this component is enabled.")]
        bool m_SnapOnEnable = true;

        /// <summary>
        /// Snap to target position when this component is enabled.
        /// </summary>
        public bool snapOnEnable
        {
            get => m_SnapOnEnable;
            set => m_SnapOnEnable = value;
        }

        [SerializeField, Tooltip("The min distance allowed within the time threshold which decides whether or not lazy follow capability is turned on.")]
        float m_MinDistanceAllowed = 0.01f;

        /// <summary>
        /// The min distance allowed within the time threshold which decides whether or not lazy follow capability is turned on.
        /// </summary>
        public float minDistanceAllowed
        {
            get => m_MinDistanceAllowed;
            set => m_MinDistanceAllowed = value;
        }

        [SerializeField, Tooltip("The max distance allowed within the time threshold which decides whether or not lazy follow capability is turned on.")]
        float m_MaxDistanceAllowed = 0.3f;

        /// <summary>
        /// The max distance allowed within the time threshold which decides whether or not lazy follow capability is turned on.
        /// </summary>
        public float maxDistanceAllowed
        {
            get => m_MaxDistanceAllowed;
            set => m_MaxDistanceAllowed = value;
        }

         [SerializeField, Tooltip("The min angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on.")]
        float m_MinAngleAllowed = 0.01f;

        /// <summary>
        /// The min angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on.
        /// </summary>
        public float minAngleAllowed
        {
            get => m_MinAngleAllowed;
            set => m_MinAngleAllowed = value;
        }

        [SerializeField, Tooltip("The max angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on.")]
        float m_MaxAngleAllowed = 0.3f;

        /// <summary>
        /// The max angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on.
        /// </summary>
        public float maxAngleAllowed
        {
            get => m_MaxAngleAllowed;
            set => m_MaxAngleAllowed = value;
        }

        [SerializeField, Tooltip("The time threshold (in seconds) where if max distance is reached the lazy follow capability will not be turned off.")]
        float m_TimeUntilThresholdReachesMaxDistance = 3f;

        /// <summary>
        /// The time threshold (in seconds) where if max distance is reached the lazy follow capability will not be turned off.
        /// </summary>
        public float timeUntilThresholdReachesMaxDistance
        {
            get => m_TimeUntilThresholdReachesMaxDistance;
            set => m_TimeUntilThresholdReachesMaxDistance = value;
        }

        [SerializeField, Tooltip("The time threshold (in seconds) where if max angle change is reached the lazy follow capability will not be turned off.")]
        float m_TimeUntilThresholdReachesMaxAngle = 3f;

        /// <summary>
        /// The time threshold (in seconds) where if max angle change is reached the lazy follow capability will not be turned off.
        /// </summary>
        public float timeUntilThresholdReachesMaxAngle
        {
            get => m_TimeUntilThresholdReachesMaxAngle;
            set => m_TimeUntilThresholdReachesMaxAngle = value;
        }

        Vector3 m_TargetPosition => m_Target.position + m_Target.TransformVector(m_TargetOffset);

        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

        Vector3 m_LastTargetPosition = Vector3.zero;

        Vector3TweenableVariable m_Vector3TweenableVariable;
        QuaternionTweenableVariable m_QuaternionTweenableVariable;

        float m_LastUpdateTime;

        /// <summary>
        /// This method will only return <see langword="true"/> if the new value is within the threshold target
        /// set by the time since the last successful update.
        /// </summary>
        /// <param name="newTarget">The new target position.</param>
        /// <returns>Returns <see langword="true"/> if the new value is within the threshold target.
        /// Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool TryGetThresholdTargetPosition(out Vector3 newTarget)
        {
            newTarget = m_TargetPosition;
            var newSqrTargetOffset = Vector3.Distance(m_Vector3TweenableVariable.target, newTarget);
            var timeSinceLastUpdate = Time.unscaledTime - m_LastUpdateTime;

            // Widen tolerance zone over time
            var allowedTargetDistanceOffset = Mathf.Lerp(m_MinDistanceAllowed, m_MaxDistanceAllowed, Mathf.Clamp01(timeSinceLastUpdate / m_TimeUntilThresholdReachesMaxDistance));
            return newSqrTargetOffset > (allowedTargetDistanceOffset * allowedTargetDistanceOffset);
        }

        /// <summary>
        /// This method will only return <see langword="true"/> if the new value is within the threshold target
        /// set by the time since the last successful update.
        /// </summary>
        /// <param name="newTarget">The new target rotation.</param>
        /// <returns>Returns <see langword="true"/> if the new value is within the threshold target.
        /// Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool TryGetThresholdTargetRotation(out Quaternion newTarget)
        {
            var forward = gameObject.transform.position - m_Target.position;
            var right = Vector3.Cross(forward, Vector3.up);
            var up = Vector3.Cross(forward, right);
            newTarget = Quaternion.LookRotation(forward, up);

            var angle = Quaternion.Angle(m_QuaternionTweenableVariable.target, newTarget);
            var timeSinceLastUpdate = Time.unscaledTime - m_LastUpdateTime;

            // Widen tolerance zone over time
            var allowedTargetAngleOffset = Mathf.Lerp(m_MinAngleAllowed, m_MaxAngleAllowed, Mathf.Clamp01(timeSinceLastUpdate / m_TimeUntilThresholdReachesMaxAngle));
            return angle > (allowedTargetAngleOffset * allowedTargetAngleOffset);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_Vector3TweenableVariable = new Vector3TweenableVariable();
            m_QuaternionTweenableVariable = new QuaternionTweenableVariable();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            // Default to main camera
            if (m_Target == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    m_Target = mainCamera.transform;
            }

            var thisTransform = transform;
            if (m_SnapOnEnable && m_Target != null)
            {
                thisTransform.position = m_TargetPosition;
            }

            var currentPosition = thisTransform.position;
            var currentRotation = thisTransform.rotation;

            m_Vector3TweenableVariable.Value = currentPosition;
            m_Vector3TweenableVariable.target = currentPosition;

            m_QuaternionTweenableVariable.Value = currentRotation;
            m_QuaternionTweenableVariable.target = currentRotation;

            m_BindingsGroup.AddBinding(m_Vector3TweenableVariable.SubscribeAndUpdate(UpdatePosition));
            m_BindingsGroup.AddBinding(m_QuaternionTweenableVariable.SubscribeAndUpdate(UpdateRotation));
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_BindingsGroup.Clear();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            m_Vector3TweenableVariable?.Dispose();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void LateUpdate()
        {
            if (m_Target == null)
                return;

            if (m_TargetPosition != m_LastTargetPosition)
            {
                m_LastUpdateTime = Time.unscaledTime;
                m_LastTargetPosition = m_TargetPosition;
            }

            var targetWithinUpdateThreshold = TryGetThresholdTargetPosition(out var newTarget);
            if (targetWithinUpdateThreshold)
            {
                m_Vector3TweenableVariable.target = newTarget;
            }

            var rotationWithinUpdateThreshold = TryGetThresholdTargetRotation(out var newRotation);
            if (rotationWithinUpdateThreshold)
            {
                m_QuaternionTweenableVariable.target = newRotation;
            }

            var tweenTarget = Time.unscaledDeltaTime * m_MovementSpeed;
            m_Vector3TweenableVariable.HandleTween(tweenTarget);
            m_QuaternionTweenableVariable.HandleTween(tweenTarget);
        }

        void UpdatePosition(float3 position)
        {
            transform.position = position;
        }

        void UpdateRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}