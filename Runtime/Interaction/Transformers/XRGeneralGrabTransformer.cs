using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
    /// <summary>
    /// Grab transformer which supports moving and rotating unconstrained with one or two interactors.
    /// Also allows clamped or unclamped scaling when using two interactors.
    /// Allows axis constraints on translation.
    /// </summary>
    /// <seealso cref="XRGrabInteractable"/>
    [AddComponentMenu("XR/Transformers/XR General Grab Transformer", 11)]
    [HelpURL(XRHelpURLConstants.k_XRGeneralGrabTransformer)]
    public class XRGeneralGrabTransformer : XRBaseGrabTransformer
    {
        /// <summary>
        /// Axis constraint enum.
        /// </summary>
        /// <seealso cref="permittedDisplacementAxes"/>
        [Flags]
        public enum ManipulationAxes
        {
            /// <summary>
            /// X-axis movement is permitted.
            /// </summary>
            X = 1 << 0,

            /// <summary>
            /// Y-axis movement is permitted.
            /// </summary>
            Y = 1 << 1,

            /// <summary>
            /// Z-axis movement is permitted.
            /// </summary>
            Z = 1 << 2,

            /// <summary>
            /// All axes movement is permitted.
            /// Shortcut for <c>ManipulationAxes.X | ManipulationAxes.Y | ManipulationAxes.Z</c>.
            /// </summary>
            All = X | Y | Z,
        }

        /// <summary>
        /// Constrained Axis Displacement Mode
        /// </summary>
        /// <seealso cref="constrainedAxisDisplacementMode"/>
        public enum ConstrainedAxisDisplacementMode
        {
            /// <summary>
            /// Determines the permitted axes based on the initial object rotation in world space.
            /// </summary>
            ObjectRelative,

            /// <summary>
            /// Determines the permitted axes based on the initial object rotation in world space, but also locks the up axis to be the world up.
            /// </summary>
            ObjectRelativeWithLockedWorldUp,

            /// <summary>
            /// Uses the world axes to project all displacement against.
            /// </summary>
            WorldAxisRelative,
        }

        /// <summary>
        /// Two handed rotation mode.
        /// </summary>
        public enum TwoHandedRotationMode
        {
            /// <summary>
            /// Determines rotation using only first hand.
            /// </summary>
            FirstHandOnly,

            /// <summary>
            /// Determines two handed rotation using first hand and then directing the object towards the second one.
            /// </summary>
            FirstHandDirectedTowardsSecondHand,

            /// <summary>
            /// Directs first hand towards second hand, but uses the two handed average to determine the base rotation.
            /// </summary>
            TwoHandedAverage,
        }

        [Header("Translation Constraints")]
        [SerializeField]
        [Tooltip("Permitted axes for translation displacement relative to the object's initial rotation.")]
        ManipulationAxes m_PermittedDisplacementAxes = ManipulationAxes.All;

        /// <summary>
        /// Permitted axes for translation displacement relative to the object's initial rotation.
        /// </summary>
        /// <seealso cref="ManipulationAxes"/>
        public ManipulationAxes permittedDisplacementAxes
        {
            get => m_PermittedDisplacementAxes;
            set => m_PermittedDisplacementAxes = value;
        }

        [SerializeField]
        [Tooltip("Determines how the constrained axis displacement mode is computed.")]
        ConstrainedAxisDisplacementMode m_ConstrainedAxisDisplacementMode = ConstrainedAxisDisplacementMode.ObjectRelativeWithLockedWorldUp;

        /// <summary>
        /// Determines how the constrained axis displacement mode is computed.
        /// </summary>
        /// <seealso cref="ConstrainedAxisDisplacementMode"/>
        public ConstrainedAxisDisplacementMode constrainedAxisDisplacementMode
        {
            get => m_ConstrainedAxisDisplacementMode;
            set => m_ConstrainedAxisDisplacementMode = value;
        }

        [Header("Rotation Constraints")]
        [SerializeField]
        [Tooltip("Determines how rotation is calculated when using two hands for the grab interaction.")]
        TwoHandedRotationMode m_TwoHandedRotationMode = TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand;

        /// <summary>
        /// Determines how rotation is calculated when using two hands for the grab interaction.
        /// </summary>
        /// <seealso cref="TwoHandedRotationMode"/>
        public TwoHandedRotationMode allowTwoHandedRotation
        {
            get => m_TwoHandedRotationMode;
            set => m_TwoHandedRotationMode = value;
        }

        [Header("Scaling Constraints")]
        [SerializeField]
        [Tooltip("Allow scaling when using multi-grab interaction.")]
        bool m_AllowTwoHandedScaling;

        /// <summary>
        /// Allow scaling when using multi-grab interaction.
        /// </summary>
        public bool allowTwoHandedScaling
        {
            get => m_AllowTwoHandedScaling;
            set => m_AllowTwoHandedScaling = value;
        }

        [SerializeField]
        [Tooltip("Percentage as a measure of 0 to 1 of scaled relative hand displacement required to trigger scale operation." +
                 "\nIf this value is 0f, scaling happens the moment both grab interactors move closer or further away from each other." +
                 "\nOtherwise, this percentage is used as a threshold before any scaling happens.")]
        [Range(0f, 1f)]
        float m_ThresholdMoveRatioForScale = 0.1f;

        /// <summary>
        /// Percentage as a measure of 0 to 1 of scaled relative hand displacement required to trigger scale operation.
        /// If this value is 0f, scaling happens the moment both grab interactors move closer or further away from each other.
        /// Otherwise, this percentage is used as a threshold before any scaling happens.
        /// </summary>
        public float thresholdMoveRatioForScale
        {
            get => m_ThresholdMoveRatioForScale;
            set => m_ThresholdMoveRatioForScale = value;
        }

        [Space]
        [SerializeField]
        [Tooltip("If enabled, scaling will abide by ratio ranges defined below.")]
        bool m_ClampScaling = true;

        /// <summary>
        /// If enabled, scaling will abide by ratio ranges defined by <see cref="minimumScaleRatio"/> and <see cref="maximumScaleRatio"/>.
        /// </summary>
        public bool clampScaling
        {
            get => m_ClampScaling;
            set => m_ClampScaling = value;
        }

        [SerializeField]
        [Tooltip("Minimum scale multiplier applied to the initial scale captured on start.")]
        [Range(0.01f, 1f)]
        float m_MinimumScaleRatio = 0.25f;

        /// <summary>
        /// Minimum scale multiplier applied to the initial scale captured on start.
        /// </summary>
        public float minimumScaleRatio
        {
            get => m_MinimumScaleRatio;
            set
            {
                m_MinimumScaleRatio = Mathf.Min(1f, value);
                m_MinimumScale = m_InitialScale * m_MinimumScaleRatio;
            }
        }

        [SerializeField]
        [Tooltip("Maximum scale multiplier applied to the initial scale captured on start.")]
        [Range(1f, 10f)]
        float m_MaximumScaleRatio = 2f;

        /// <summary>
        /// Maximum scale multiplier applied to the initial scale captured on start.
        /// </summary>
        public float maximumScaleRatio
        {
            get => m_MaximumScaleRatio;
            set
            {
                m_MaximumScaleRatio = Mathf.Max(1f, value);
                m_MaximumScale = m_InitialScale * m_MaximumScaleRatio;
            }
        }

        [Space]
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("Scales the distance of displacement between interactors needed to modify the scale interactable.")]
        float m_ScaleMultiplier = 0.25f;

        /// <summary>
        /// Scales the distance of displacement between interactors needed to modify the scale interactable.
        /// </summary>
        public float scaleMultiplier
        {
            get => m_ScaleMultiplier;
            set => m_ScaleMultiplier = value;
        }

        /// <inheritdoc />
        protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

        Pose m_OriginalObjectPose;
        Pose m_OffsetPose;
        Pose m_OriginalInteractorPose;
        Vector3 m_InteractorLocalGrabPoint;
        Vector3 m_ObjectLocalGrabPoint;
        IXRInteractor m_OriginalInteractor;
        
        // Two handed grab start cached values
        int m_LastGrabCount;
        Vector3 m_StartHandleBar;
        Vector3 m_ScaleAtGrabStart;

        bool m_FirstFrameSinceTwoHandedGrab;
        Vector3 m_LastTwoHandedUp;

        Vector3 m_InitialScale;
        Vector3 m_MinimumScale;
        Vector3 m_MaximumScale;

        ConstrainedAxisDisplacementMode m_ConstrainedAxisDisplacementModeOnGrab;
        ManipulationAxes m_PermittedDisplacementAxesOnGrab;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_InitialScale = transform.localScale;
        }

        /// <inheritdoc />
        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                {
                    UpdateTarget(grabInteractable, ref targetPose, ref localScale);
                    break;
                }
            }
        }

        /// <inheritdoc />
        public override void OnGrab(XRGrabInteractable grabInteractable)
        {
            base.OnGrab(grabInteractable);

            var interactor = grabInteractable.interactorsSelecting[0];
            var grabInteractableTransform = grabInteractable.transform;
            var grabAttachTransform = grabInteractable.GetAttachTransform(interactor);

            m_OriginalObjectPose = grabInteractableTransform.GetWorldPose();
            m_OriginalInteractorPose = interactor.GetAttachTransform(grabInteractable).GetWorldPose();
            m_OriginalInteractor = interactor;
            m_LastGrabCount = 1;

            Vector3 offsetTargetPosition = Vector3.zero;
            Quaternion offsetTargetRotation = Quaternion.identity;

            Quaternion capturedRotation = m_OriginalObjectPose.rotation;
            if (grabInteractable.trackRotation)
            {
                capturedRotation = m_OriginalInteractorPose.rotation;

                offsetTargetRotation = Quaternion.Inverse(Quaternion.Inverse(m_OriginalObjectPose.rotation) * grabAttachTransform.rotation);
            }

            Vector3 capturedPosition = m_OriginalObjectPose.position;
            if (grabInteractable.trackPosition)
            {
                capturedPosition = m_OriginalInteractorPose.position;

                // Calculate offset of the grab interactable's position relative to its attach transform
                var attachOffset = m_OriginalObjectPose.position - grabAttachTransform.position;

                offsetTargetPosition = grabInteractable.trackRotation ? grabAttachTransform.InverseTransformDirection(attachOffset) : attachOffset;
            }

            // Cache axis settings on grab because changing them while grab is in progress can lead to undesired results.
            m_ConstrainedAxisDisplacementModeOnGrab = m_ConstrainedAxisDisplacementMode;
            m_PermittedDisplacementAxesOnGrab = m_PermittedDisplacementAxes;

            // Adjust capture position according to permitted axes
            capturedPosition = AdjustPositionForPermittedAxes(capturedPosition, m_OriginalObjectPose, m_PermittedDisplacementAxesOnGrab, m_ConstrainedAxisDisplacementModeOnGrab);

            // Store adjusted transform pose
            m_OriginalObjectPose = new Pose(capturedPosition, capturedRotation);

            Vector3 localScale = grabInteractableTransform.localScale;
            TranslateSetup(m_OriginalInteractorPose, m_OriginalInteractorPose.position, m_OriginalObjectPose, localScale);

            Quaternion worldToGripRotation = offsetTargetRotation * Quaternion.Inverse(m_OriginalInteractorPose.rotation);
            Quaternion relativeCaptureRotation = worldToGripRotation * m_OriginalObjectPose.rotation;

            // Scale offset target position to match new local scale
            Vector3 scaledOffsetTargetPosition = offsetTargetPosition.Divide(localScale);

            m_OffsetPose = new Pose(scaledOffsetTargetPosition, relativeCaptureRotation);
        }

        /// <inheritdoc />
        public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
            base.OnGrabCountChanged(grabInteractable, targetPose, localScale);

            var newGrabCount = grabInteractable.interactorsSelecting.Count;

            if (newGrabCount == 1)
            {
                // If the initial grab interactor changes, or we reduce the grab count, we need to recompute initial grab parameters. 
                var interactor0 = grabInteractable.interactorsSelecting[0];
                if (interactor0 != m_OriginalInteractor || newGrabCount < m_LastGrabCount)
                {
                    OnGrab(grabInteractable);
                }
            }
            else if (newGrabCount > 1)
            {
                var interactor0 = grabInteractable.interactorsSelecting[0];
                var interactor1 = grabInteractable.interactorsSelecting[1];

                var interactor0Transform = interactor0.GetAttachTransform(grabInteractable);
                var grabAttachTransform1 = grabInteractable.GetAttachTransform(interactor1);

                m_StartHandleBar = interactor0Transform.InverseTransformPoint(grabAttachTransform1.position);
                m_ScaleAtGrabStart = localScale;
                
                // Precompute scale range to support modifying the values in the Inspector window without needing to multiply every frame
                m_MinimumScale = m_InitialScale * m_MinimumScaleRatio;
                m_MaximumScale = m_InitialScale * m_MaximumScaleRatio;
                
                m_FirstFrameSinceTwoHandedGrab = true;
            }

            m_LastGrabCount = newGrabCount;
        }

        Pose ComputeAdjustedInteractorPose(XRGrabInteractable grabInteractable, out Vector3 newHandleBar)
        {
            if (grabInteractable.interactorsSelecting.Count == 1 || m_TwoHandedRotationMode == TwoHandedRotationMode.FirstHandOnly)
            {
                newHandleBar = m_StartHandleBar;
                return grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable).GetWorldPose();
            }

            if (grabInteractable.interactorsSelecting.Count > 1)
            {
                var interactor0 = grabInteractable.interactorsSelecting[0];
                var interactor1 = grabInteractable.interactorsSelecting[1];

                var interactor0Transform = interactor0.GetAttachTransform(grabInteractable);
                var interactor1Transform = interactor1.GetAttachTransform(grabInteractable);

                newHandleBar = interactor0Transform.InverseTransformPoint(interactor1Transform.position);

                Quaternion newRotation;
                if (m_TwoHandedRotationMode == TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand)
                {
                    newRotation = interactor0Transform.rotation * Quaternion.FromToRotation(m_StartHandleBar, newHandleBar);
                }
                else if (m_TwoHandedRotationMode == TwoHandedRotationMode.TwoHandedAverage)
                {
                    var forward = (interactor1Transform.position - interactor0Transform.position).normalized;
                    
                    var averageRight = Vector3.Slerp(interactor0Transform.right, interactor1Transform.right, 0.5f);
                    var up = Vector3.Slerp(interactor0Transform.up, interactor1Transform.up, 0.5f);
                    
                    var crossUp = Vector3.Cross(forward, averageRight);
                    var angleDiff = Mathf.PingPong(Vector3.Angle(up, forward), 90f);
                    up = Vector3.Slerp(crossUp, up, angleDiff / 90f);
                    
                    var crossRight = Vector3.Cross(up, forward);
                    up = Vector3.Cross(forward, crossRight);

                    if (m_FirstFrameSinceTwoHandedGrab)
                    {
                        m_FirstFrameSinceTwoHandedGrab = false;
                    }
                    else
                    {
                        // We also keep track of whether the up vector was pointing up or down previously, to allow for objects to be flipped through a series of rotations
                        // Such as a 180 degree rotation on the y, followed by a 180 degree rotation on the x
                        if (Vector3.Dot(up, m_LastTwoHandedUp) <= 0f)
                        {
                            up = -up;
                        }
                    }

                    m_LastTwoHandedUp = up;

                    var twoHandedRotation = Quaternion.LookRotation(forward, up);
                    
                    // Given that this rotation method doesn't really consider the first interactor's start rotation, we have to remove the offset pose computed on grab. 
                    newRotation = twoHandedRotation * Quaternion.Inverse(m_OffsetPose.rotation);
                }
                else
                {
                    newRotation = interactor0Transform.rotation;
                }

                return new Pose(interactor0Transform.position, newRotation);
            }

            newHandleBar = m_StartHandleBar;
            return Pose.identity;
        }

        void TranslateSetup(Pose interactorCentroidPose, Vector3 grabCentroid, Pose objectPose, Vector3 objectScale)
        {
            Quaternion worldToInteractorRotation = Quaternion.Inverse(interactorCentroidPose.rotation);
            m_InteractorLocalGrabPoint = worldToInteractorRotation * (grabCentroid - interactorCentroidPose.position);

            m_ObjectLocalGrabPoint = Quaternion.Inverse(objectPose.rotation) * (grabCentroid - objectPose.position);
            m_ObjectLocalGrabPoint = m_ObjectLocalGrabPoint.Divide(objectScale);
        }

        Vector3 ComputeNewObjectPosition(Pose newInteractionPose, Quaternion objectRotation, Vector3 objectScale, bool trackRotation)
        {
            // Scale up offset pose with new object scale
            Vector3 scaledOffsetPose = Vector3.Scale(m_OffsetPose.position, objectScale);

            // Adjust computed offset with current source rotation
            Vector3 rotationAdjustedTargetOffset = trackRotation ? newInteractionPose.rotation * scaledOffsetPose : scaledOffsetPose;
            Vector3 newTargetPosition = newInteractionPose.position + rotationAdjustedTargetOffset;

            Vector3 scaledGrabToObject = Vector3.Scale(m_ObjectLocalGrabPoint, objectScale);
            Vector3 adjustedPointerToGrab = m_InteractorLocalGrabPoint;
            adjustedPointerToGrab = newInteractionPose.rotation * adjustedPointerToGrab;

            return adjustedPointerToGrab - objectRotation * scaledGrabToObject + newTargetPosition;
        }

        Quaternion ComputeNewObjectRotation(Pose newInteractionPose, bool trackRotation)
        {
            if (!trackRotation)
                return m_OriginalObjectPose.rotation;
            return newInteractionPose.rotation * m_OffsetPose.rotation;
        }

        static Vector3 AdjustPositionForPermittedAxes(Vector3 targetPosition, Pose originalObjectPose, ManipulationAxes permittedAxes, ConstrainedAxisDisplacementMode axisDisplacementMode)
        {
            bool hasX = (permittedAxes & ManipulationAxes.X) != 0;
            bool hasY = (permittedAxes & ManipulationAxes.Y) != 0;
            bool hasZ = (permittedAxes & ManipulationAxes.Z) != 0;

            if (hasX && hasY && hasZ)
                return targetPosition;

            if (!hasX && !hasY && !hasZ)
                return originalObjectPose.position;

            Vector3 xComponent = Vector3.zero;
            Vector3 yComponent = Vector3.zero;
            Vector3 zComponent = Vector3.zero;

            Vector3 translationVector = targetPosition - originalObjectPose.position;
            Vector3 sumTranslationVector = Vector3.zero;

            if (axisDisplacementMode == ConstrainedAxisDisplacementMode.WorldAxisRelative)
            {
                if (hasX)
                    xComponent = Vector3.Project(translationVector, Vector3.right);

                if (hasY)
                    yComponent = Vector3.Project(translationVector, Vector3.up);

                if (hasZ)
                    zComponent = Vector3.Project(translationVector, Vector3.forward);

                sumTranslationVector = (xComponent + yComponent + zComponent);
            }
            else if (axisDisplacementMode == ConstrainedAxisDisplacementMode.ObjectRelative)
            {
                if (hasX)
                    xComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.right);

                if (hasY)
                    yComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.up);

                if (hasZ)
                    zComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.forward);

                sumTranslationVector = (xComponent + yComponent + zComponent);
            }
            else if (axisDisplacementMode == ConstrainedAxisDisplacementMode.ObjectRelativeWithLockedWorldUp)
            {
                if (hasX && hasZ)
                {
                    sumTranslationVector = Vector3.ProjectOnPlane(translationVector, Vector3.up);
                }
                else
                {
                    Vector3 upComponent = Vector3.zero;

                    if (hasX)
                    {
                        xComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.right);
                    }

                    if (hasY)
                    {
                        yComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.up);
                        upComponent = Vector3.Project(translationVector, Vector3.up);
                    }

                    if (hasZ)
                    {
                        zComponent = Vector3.Project(translationVector, originalObjectPose.rotation * Vector3.forward);
                    }

                    sumTranslationVector = Vector3.ProjectOnPlane(xComponent + yComponent + zComponent, Vector3.up) + upComponent;
                }
            }

            return originalObjectPose.position + sumTranslationVector;
        }

        Vector3 ComputeNewScale(XRGrabInteractable grabInteractable, Vector3 startScale, Vector3 currentScale, Vector3 startHandleBar, Vector3 newHandleBar)
        {
            if (!m_AllowTwoHandedScaling || grabInteractable.interactorsSelecting.Count < 2)
            {
                return currentScale;
            }

            var scaleRatio = Vector3.SqrMagnitude(newHandleBar) / Vector3.SqrMagnitude(startHandleBar);
            if (scaleRatio > 1)
            {
                var amountOver1 = (scaleRatio - 1f);
                var multipliedAmountOver1 = amountOver1 * m_ScaleMultiplier;

                var multipliedAmountOver1WithoutThreshold = multipliedAmountOver1 - m_ThresholdMoveRatioForScale;
                if (multipliedAmountOver1WithoutThreshold < 0f)
                {
                    return currentScale;
                }

                var targetScaleRatio = 1f + multipliedAmountOver1WithoutThreshold;
                var targetScale = targetScaleRatio * startScale;

                bool isOverMaximum = targetScale.x > m_MaximumScale.x || targetScale.y > m_MaximumScale.y || targetScale.z > m_MaximumScale.z;
                return isOverMaximum && m_ClampScaling ? m_MaximumScale : targetScale;
            }

            if (scaleRatio < 1f)
            {
                var invertedScaleRatio = 1f / scaleRatio;
                var amountOver1 = invertedScaleRatio - 1f;
                var multipliedAmountOver1 = amountOver1 * m_ScaleMultiplier;

                var multipliedAmountOver1WithoutThreshold = multipliedAmountOver1 - m_ThresholdMoveRatioForScale;
                if (multipliedAmountOver1WithoutThreshold < 0f)
                {
                    return currentScale;
                }

                var invertedTargetScaleRatio = 1f + multipliedAmountOver1WithoutThreshold;
                var targetScale = 1f / invertedTargetScaleRatio * startScale;

                bool isUnderMinimum = targetScale.x < m_MinimumScale.x || targetScale.y < m_MinimumScale.y || startScale.z < m_MinimumScale.z;
                return isUnderMinimum && m_ClampScaling ? m_MinimumScale : targetScale;
            }

            return currentScale;
        }

        void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose, ref Vector3 localScale)
        {
            var interactorAttachPose = ComputeAdjustedInteractorPose(grabInteractable, out Vector3 newHandleBar);

            localScale = ComputeNewScale(grabInteractable, m_ScaleAtGrabStart, localScale, m_StartHandleBar, newHandleBar);

            targetPose.rotation = ComputeNewObjectRotation(interactorAttachPose, grabInteractable.trackRotation);

            var targetObjectPosition = ComputeNewObjectPosition(interactorAttachPose, targetPose.rotation, localScale, grabInteractable.trackRotation);
            targetPose.position = AdjustPositionForPermittedAxes(targetObjectPosition, m_OriginalObjectPose, m_PermittedDisplacementAxesOnGrab, m_ConstrainedAxisDisplacementModeOnGrab);
        }
    }
}