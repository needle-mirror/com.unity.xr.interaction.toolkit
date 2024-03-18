#if BURST_PRESENT
using Unity.Burst;
#endif
using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Manages and controls the anchor position for an XR interaction, handling how interactables snap and follow the interactor.
    /// It applies velocity-based scaling for anchor movements and supports stabilization options.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Interactors/Interaction Attach Controller", 22)]
    [HelpURL(XRHelpURLConstants.k_InteractionAttachController)]
    public class InteractionAttachController : MonoBehaviour, IInteractionAttachController
    {
        [SerializeField]
        [Tooltip("The transform that this anchor should follow.")]
        Transform m_TransformToFollow;

        /// <summary>
        /// Gets or sets the transform that the anchor should follow.
        /// </summary>
        public Transform transformToFollow
        {
            get => m_TransformToFollow;
            set => m_TransformToFollow = value;
        }

        [Header("Stabilization Parameters")]
        [SerializeField]
        [Tooltip("The stabilization mode for the motion of the anchor. Determines how the anchor's position and rotation are stabilized relative to the followed transform.")]
        MotionStabilizationMode m_MotionStabilizationMode = MotionStabilizationMode.WithPositionOffset;

        /// <summary>
        /// Gets or sets the stabilization mode for the motion of the anchor. Determines how the anchor's position and rotation are stabilized relative to the followed transform.
        /// </summary>
        public MotionStabilizationMode motionStabilizationMode
        {
            get => m_MotionStabilizationMode;
            set => m_MotionStabilizationMode = value;
        }

        [SerializeField]
        [Tooltip("Factor for stabilizing position. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.")]
        float m_PositionStabilization = 0.25f;

        /// <summary>
        /// Factor for stabilizing position. This value represents the maximum distance (in meters) over which position stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.
        /// </summary>
        public float positionStabilization
        {
            get => m_PositionStabilization;
            set => m_PositionStabilization = value;
        }

        [SerializeField]
        [Tooltip("Factor for stabilizing angle. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.")]
        float m_AngleStabilization = 20f;

        /// <summary>
        /// Factor for stabilizing angle. This value represents the maximum angle (in degrees) over which angle stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.
        /// </summary>
        public float angleStabilization
        {
            get => m_AngleStabilization;
            set => m_AngleStabilization = value;
        }

        [Header("Smoothing Settings")]
        [SerializeField]
        [Tooltip("If true offset will be smoothed over time in XR Origin space.")]
        bool m_SmoothOffset;

        /// <summary>
        /// If true offset will be smoothed over time in XR Origin space.
        /// May present some instability if smoothing is toggled during an interaction.
        /// </summary>
        public bool smoothOffset
        {
            get => m_SmoothOffset;
            set => m_SmoothOffset = value;
        }

        [SerializeField]
        [Tooltip("Smoothing speed for the offset anchor child.")]
        [Range(1f, 30f)]
        float m_SmoothingSpeed = 10f;

        /// <summary>
        /// Smoothing amount for the anchor's position and rotation. Higher values mean more smoothing occurs faster.
        /// </summary>
        public float smoothingSpeed
        {
            get => m_SmoothingSpeed;
            set => m_SmoothingSpeed = Mathf.Clamp(value, 1f, 30f);
        }

        [Header("Anchor Movement Parameters")]
        [SerializeField]
        [Tooltip("Flag to use distance-based velocity scaling for anchor movement.")]
        bool m_UseDistanceBasedVelocityScaling = true;

        /// <summary>
        /// Flag to use distance-based velocity scaling for anchor movement.
        /// </summary>
        public bool useDistanceBasedVelocityScaling
        {
            get => m_UseDistanceBasedVelocityScaling;
            set => m_UseDistanceBasedVelocityScaling = value;
        }
        
        [Space]
        [SerializeField]
        [Tooltip("Flag to determine if momentum is active when distance scaling is in effect.")]
        bool m_UseMomentum = true;

        /// <summary>
        /// Flag to determine if momentum is active when <see cref="useDistanceBasedVelocityScaling"/> is active.
        /// </summary>
        public bool useMomentum
        {
            get => m_UseMomentum;
            set => m_UseMomentum = value;
        }
        
        [SerializeField]
        [Tooltip("Decay scalar for momentum. Higher values will cause momentum to decay faster.")]
        [Range(0f, 10f)]
        float m_MomentumDecayScale = 1.25f;
        
        /// <summary>
        /// Decay scalar for momentum. Higher values will cause momentum to decay faster.
        /// </summary>
        public float momentumDecayScale
        {
            get => m_MomentumDecayScale;
            set => m_MomentumDecayScale = Mathf.Clamp(value, 0f, 10f);
        }
        
        [Space]
        [SerializeField]
        [Range(0f, 5f)]
        [Tooltip("Scales anchor velocity from 0 to 1 based on z-velocity's deviation below a threshold. 0 means no scaling.")]
        float m_ZVelocityRampThreshold = 0.3f;

        /// <summary>
        /// Scales anchor velocity from 0 to 1 based on z-velocity's deviation below a threshold. 0 means no scaling.
        /// </summary>
        public float zVelocityRampThreshold
        {
            get => m_ZVelocityRampThreshold;
            set => m_ZVelocityRampThreshold = Mathf.Clamp(value, 0f, 5f);
        }
        
        [SerializeField]
        [Tooltip("Adjusts the object's velocity calculation when moving towards the user. It modifies the distance-based calculation that determines the velocity scalar.")]
        [Range(0f, 2f)]
        float m_PullVelocityBias = 1f;

        /// <summary>
        /// Adjusts the object's velocity calculation when moving towards the user. 
        /// It modifies the distance-based calculation that determines the velocity scalar.
        /// <see cref="minAdditionalVelocityScalar"/>
        /// <see cref="maxAdditionalVelocityScalar"/>
        /// </summary>
        public float pullVelocityBias
        {
            get => m_PullVelocityBias;
            set => m_PullVelocityBias = Mathf.Clamp(value, 0f, 2f); 
        }

        [SerializeField]
        [Tooltip("Adjusts the object's velocity calculation when moving away from the user. It modifies the distance-based calculation that determines the velocity scalar.")]
        [Range(0f, 2f)]
        float m_PushVelocityBias = 1.25f;

        /// <summary>
        /// Adjusts the object's velocity calculation when moving away from the user. 
        /// It modifies the distance-based calculation that determines the velocity scalar.
        /// <see cref="minAdditionalVelocityScalar"/>
        /// <see cref="maxAdditionalVelocityScalar"/>
        /// </summary>
        public float pushVelocityBias
        {
            get => m_PushVelocityBias;
            set => m_PushVelocityBias = Mathf.Clamp(value, 0f, 2f); 
        }

        [SerializeField]
        [Tooltip("Minimum additional velocity scaling factor for movement, interpolated by a quad bezier curve.")]
        [Range(0f, 2f)]
        float m_MinAdditionalVelocityScalar = 0.05f;

        /// <summary>
        /// Minimum additional velocity scaling factor for movement, interpolated by a quad bezier curve.
        /// </summary>
        public float minAdditionalVelocityScalar
        {
            get => m_MinAdditionalVelocityScalar;
            set => m_MinAdditionalVelocityScalar = Mathf.Clamp(value, 0f, 2f);
        }

        [SerializeField]
        [Tooltip("Maximum additional velocity scaling factor for movement, interpolated by a quad bezier curve.")]
        [Range(0, 5f)]
        float m_MaxAdditionalVelocityScalar = 1.5f;

        /// <summary>
        /// Maximum additional velocity scaling factor for movement, interpolated by a quad bezier curve.
        /// </summary>
        public float maxAdditionalVelocityScalar
        {
            get => m_MaxAdditionalVelocityScalar;
            set => m_MaxAdditionalVelocityScalar = Mathf.Clamp(value, 0f, 5f);
        }

        /// <summary>
        /// Indicates whether the anchor currently has an offset applied.
        /// </summary>
        public bool hasOffset => m_HasOffset;

        /// <summary>
        /// Event callback used to notify when the attach controller has been updated.
        /// </summary>
        public event Action attachUpdated;

        bool m_FirstMovementFrame;
        bool m_HasOffset;
        Vector3 m_StartOffset;
        float m_Pivot;
        float m_Momentum;

        bool m_HasXROrigin;
        XROrigin m_XROrigin;
        Transform m_AnchorParent;
        Transform m_AnchorChild;

        Vector3 m_LastLocalTargetPosition;
        Vector3 m_LastChildOriginSpacePosition;

        readonly AttachPointVelocityTracker m_VelocityTracker = new AttachPointVelocityTracker();

        Transform GetXROriginTransform() => InitializeXROrigin() ? m_XROrigin.Origin.transform : null;

        bool InitializeXROrigin()
        {
            if (m_XROrigin == null)
                ComponentLocatorUtility<XROrigin>.TryFindComponent(out m_XROrigin);
            m_HasXROrigin = m_XROrigin != null;
            return m_HasXROrigin;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            var minVal = Mathf.Min(m_MinAdditionalVelocityScalar, m_MaxAdditionalVelocityScalar);
            var maxVal = Mathf.Max(m_MinAdditionalVelocityScalar, m_MaxAdditionalVelocityScalar);

            m_MinAdditionalVelocityScalar = minVal;
            m_MaxAdditionalVelocityScalar = maxVal;

            if (m_TransformToFollow == null)
                m_TransformToFollow = transform;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_TransformToFollow == null)
                m_TransformToFollow = transform;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (!InitializeXROrigin() && m_UseDistanceBasedVelocityScaling)
            {
                Debug.LogWarning($"Missing XR Origin. Disabling distance-based velocity scaling on this {this}.", this);
                m_UseDistanceBasedVelocityScaling = false;
            }
        }

        // ReSharper disable once Unity.RedundantEventFunction -- For consistent method override signature in derived classes
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        void SyncAnchor()
        {
            if (m_TransformToFollow == null)
                m_TransformToFollow = transform;
            m_AnchorParent.position = m_TransformToFollow.position;
            m_AnchorParent.rotation = m_TransformToFollow.rotation;
        }

        /// <inheritdoc />
        Transform IInteractionAttachController.GetOrCreateAnchorTransform(bool updateTransform)
        {
            if (m_AnchorParent == null)
            {
                var origin = GetXROriginTransform();
                var typeName = GetType().Name;

                // Capture hand name
                string handName = "";
                if (TryGetComponent(out IXRInteractor interactor))
                    handName = interactor.handedness.ToString();

                m_AnchorParent = new GameObject($"[{handName} {typeName}] Attach").transform;
                m_AnchorParent.SetParent(origin, false);
                m_AnchorParent.localPosition = Vector3.zero;
                m_AnchorParent.localRotation = Quaternion.identity;

                if (m_AnchorChild == null)
                {
                    m_AnchorChild = new GameObject($"[{handName} {typeName}] Attach Child").transform;
                    m_AnchorChild.SetParent(m_AnchorParent, false);
                    m_AnchorChild.localPosition = Vector3.zero;
                    m_AnchorChild.localRotation = Quaternion.identity;
                }
            }

            if (updateTransform)
                SyncAnchor();

            return m_AnchorChild;
        }

        /// <inheritdoc />
        void IInteractionAttachController.MoveTo(Vector3 targetWorldPosition)
        {
            SyncAnchor();
            m_AnchorChild.position = targetWorldPosition;
            var localPosition = m_AnchorChild.localPosition;
            if (localPosition.z < 0f)
            {
                localPosition.z = 0f;
                m_AnchorChild.localPosition = localPosition;
            }

            m_StartOffset = localPosition;
            m_LastLocalTargetPosition = localPosition;

            if (m_HasXROrigin)
                m_LastChildOriginSpacePosition = m_XROrigin.Origin.transform.InverseTransformPoint(m_AnchorChild.position);

            m_Pivot = localPosition.z;
            m_HasOffset = m_Pivot > 0f;
            m_Momentum = 0f;
            m_FirstMovementFrame = true;
        }

        /// <inheritdoc />
        void IInteractionAttachController.ApplyLocalPositionOffset(Vector3 offset)
        {
            var localPosition = m_AnchorChild.localPosition + offset;
            if (localPosition.z < 0f)
                localPosition.z = 0f;

            m_AnchorChild.localPosition = localPosition;
            m_LastLocalTargetPosition = localPosition;

            if (m_HasXROrigin)
                m_LastChildOriginSpacePosition = m_XROrigin.Origin.transform.InverseTransformPoint(m_AnchorChild.position);

            m_HasOffset = localPosition.z > 0f;
        }

        /// <inheritdoc />
        void IInteractionAttachController.ApplyLocalRotationOffset(Quaternion localRotation)
        {
            m_AnchorChild.localRotation *= localRotation;
        }

        /// <inheritdoc />
        public void ResetOffset()
        {
            m_FirstMovementFrame = true;
            m_HasOffset = false;
            m_Momentum = 0f;
            m_AnchorChild.SetLocalPose(Pose.identity);
            SyncAnchor();
        }

        /// <inheritdoc />
        void IInteractionAttachController.DoUpdate(float deltaTime)
        {
            if (!m_HasXROrigin)
                return;

            var originTransform = m_XROrigin.Origin.transform;
            var originUp = originTransform.up;

            // Check if we skip stabilization
            if (motionStabilizationMode == MotionStabilizationMode.Never || (motionStabilizationMode == MotionStabilizationMode.WithPositionOffset && !m_HasOffset))
                SyncAnchor();
            else
            {
                if (!hasOffset)
                {
                    XRTransformStabilizer.ApplyStabilization(ref m_AnchorParent, m_TransformToFollow, positionStabilization, angleStabilization, deltaTime);
                }
                else
                {
                    float childAnchorOffsetMagnitude = m_AnchorChild.localPosition.z;
                    float stabilizationMultiplier = (1f + childAnchorOffsetMagnitude);
                    float adjustedPositionStabilization = stabilizationMultiplier * positionStabilization;
                    float adjustedAngleStabilization = stabilizationMultiplier * angleStabilization;

                    var anchorParentWorldPos = m_AnchorParent.position;
                    var worldOffset = m_AnchorChild.position - anchorParentWorldPos;
                    var projectedWorldOffset = Vector3.ProjectOnPlane(worldOffset, originUp);
                    var stabilizationTarget = anchorParentWorldPos + projectedWorldOffset;

                    XRTransformStabilizer.ApplyStabilization(ref m_AnchorParent, m_TransformToFollow, stabilizationTarget, adjustedPositionStabilization, adjustedAngleStabilization, deltaTime);
                }
            }

            // Track attach point velocity
            if (m_UseDistanceBasedVelocityScaling)
                m_VelocityTracker.UpdateAttachPointVelocityData(transformToFollow, originTransform);

            if (!hasOffset)
            {
                attachUpdated?.Invoke();
                return;
            }

            if (!m_UseDistanceBasedVelocityScaling)
            {
                UpdatePosition(m_LastLocalTargetPosition, m_StartOffset, deltaTime);
                attachUpdated?.Invoke();
                return;
            }

            float3 offsetVector = m_SmoothOffset ? m_LastLocalTargetPosition : m_AnchorChild.localPosition;

            float3 velocityLocal;
            if (m_FirstMovementFrame)
            {
                velocityLocal = float3.zero;
                m_FirstMovementFrame = false;
            }
            else
            {
                var velocityWorld = m_VelocityTracker.GetAttachPointVelocity(originTransform);
                var projectedVelocityWorld = Vector3.ProjectOnPlane(velocityWorld, originUp);
                velocityLocal = m_AnchorParent.InverseTransformDirection(projectedVelocityWorld);
            }

            ComputeAmplifiedOffset(velocityLocal, m_StartOffset, offsetVector, m_MinAdditionalVelocityScalar, m_MaxAdditionalVelocityScalar, m_PushVelocityBias, m_PullVelocityBias, m_ZVelocityRampThreshold, m_UseMomentum, m_MomentumDecayScale, ref m_Momentum, ref m_Pivot, deltaTime, out var newOffset);

            // Check if the new offset's z-value is less than zero in local space 
            if (newOffset.z < 0f)
                ResetOffset();
            else
                UpdatePosition(m_LastChildOriginSpacePosition, newOffset, deltaTime);

            attachUpdated?.Invoke();
        }

        void UpdatePosition(Vector3 lastOriginSpacePosition, Vector3 targetLocalPosition, float deltaTime)
        {
            if (!m_SmoothOffset || !m_HasXROrigin)
            {
                m_AnchorChild.localPosition = targetLocalPosition;
                m_LastLocalTargetPosition = targetLocalPosition;
                return;
            }

            var previousWorldPosition = m_XROrigin.Origin.transform.TransformPoint(lastOriginSpacePosition);
            var newTargetWorldPosition = m_AnchorParent.TransformPoint(targetLocalPosition);

            var newWorldPosition = BurstLerpUtility.BezierLerp(previousWorldPosition, newTargetWorldPosition, m_SmoothingSpeed * deltaTime);

            m_AnchorChild.position = newWorldPosition;
            m_LastChildOriginSpacePosition = m_XROrigin.Origin.transform.InverseTransformPoint(newWorldPosition);

            m_LastLocalTargetPosition = targetLocalPosition;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void ComputeAmplifiedOffset(in float3 velocityLocal, in float3 startOffset, in float3 offsetVector, float minAdditionalVelocityScalar, float maxAdditionalVelocityScalar, float pushVelocityBias, float pullVelocityBias, float zVelocityRampThreshold, bool useMomentum, float momentumDecayScale, ref float momentum, ref float pivot, float deltaTime, out float3 newOffset)        
        {
            // Calculate the Bezier scale factor
            float distanceAdjustedMinVelocityScalar = minAdditionalVelocityScalar * pivot;
            float distanceAdjustedMaxVelocityScalar = maxAdditionalVelocityScalar * pivot;

            bool isMovingAway = velocityLocal.z > 0f;
            float distanceRatio = math.abs(offsetVector.z) / pivot;
            float t = math.clamp(distanceRatio * (isMovingAway ? pushVelocityBias : pullVelocityBias), 0f, 1f);
            float movementScale = BurstLerpUtility.BezierLerp(distanceAdjustedMinVelocityScalar, distanceAdjustedMaxVelocityScalar, t);

            float rampAmount = zVelocityRampThreshold > 0 ? math.clamp(math.abs(velocityLocal.z) / zVelocityRampThreshold, 0f, 1f) : 1f;
            bool isAboveRampThreshold = !(rampAmount < 1f);
            
            float zMovement = velocityLocal.z * rampAmount * (1f + movementScale) * deltaTime;

            // If zMovement changes direction and the change is above a threshold tolerance, reset momentum
            if (useMomentum)
            {
                const float tolerance = 0.001f; 
                float absMomentum = math.abs(momentum);
                float absZMovement = math.abs(zMovement);
                
                if ((int)math.sign(momentum) != (int)math.sign(zMovement)
                    && math.abs(absMomentum - absZMovement) > tolerance)
                {
                    if (isAboveRampThreshold)
                        momentum = zMovement / 2f;
                    else if (rampAmount > 0.25f)
                        momentum = 0f;
                }
                else if (isAboveRampThreshold)
                {
                    // Accumulate momentum in the direction of zMovement
                    momentum = math.max(absMomentum, absZMovement / 2f) * math.sign(zMovement);
                }
                
                // Cutoff momentum when value is too low
                if (math.abs(momentum) < tolerance)
                    momentum = 0f;
                // Decay momentum
                else
                    momentum *= 1f - momentumDecayScale * deltaTime;
            }
            else
            {
                momentum = 0f;
            }

            float newZOffset = offsetVector.z + zMovement + momentum;

            // Interpolate the offset to zero as the z-value approaches zero
            var offsetFactor = 1f - math.clamp(newZOffset / startOffset.z, 0f, 1f);
            var adjustedXOffset = math.lerp(startOffset.x, 0f, offsetFactor);
            var adjustedYOffset = math.lerp(startOffset.y, 0f, offsetFactor);

            newOffset = new float3(adjustedXOffset, adjustedYOffset, newZOffset);
            if (newZOffset > startOffset.z)
                pivot = newZOffset;
            else
                pivot = math.lerp(pivot, (startOffset.z + newZOffset) / 2f, deltaTime * movementScale);
        }
    }
}