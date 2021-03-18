using System;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactable component that allows basic "grab" functionality.
    /// Can attach to a selecting Interactor and follow it around while obeying physics (and inherit velocity when released).
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/XR Grab Interactable")]
    [HelpURL(XRHelpURLConstants.k_XRGrabInteractable)]
    public class XRGrabInteractable : XRBaseInteractable
    {
        const float k_DefaultTighteningAmount = 0.5f;
        const float k_DefaultSmoothingAmount = 5f;
        const float k_VelocityDamping = 0.4f;
        const float k_VelocityScale = 1f;
        const float k_AngularVelocityDamping = 0.4f;
        const float k_AngularVelocityScale = 0.95f;
        const int k_ThrowSmoothingFrameCount = 20;
        const float k_DefaultAttachEaseInTime = 0.15f;
        const float k_DefaultThrowSmoothingDuration = 0.25f;
        const float k_DefaultThrowVelocityScale = 1.5f;
        const float k_DefaultThrowAngularVelocityScale = 1f;

        [SerializeField]
        Transform m_AttachTransform;

        /// <summary>
        /// The attachment point to use on this Interactable (will use this object's position as center if none set).
        /// </summary>
        public Transform attachTransform
        {
            get => m_AttachTransform;
            set => m_AttachTransform = value;
        }

        [SerializeField]
        float m_AttachEaseInTime = k_DefaultAttachEaseInTime;

        /// <summary>
        /// Time in seconds to ease in the attach when selected (a value of 0 indicates no easing).
        /// </summary>
        public float attachEaseInTime
        {
            get => m_AttachEaseInTime;
            set => m_AttachEaseInTime = value;
        }

        [SerializeField]
        MovementType m_MovementType = MovementType.Kinematic;

        /// <summary>
        /// Specifies how this object is moved when selected, either through setting the velocity of the <see cref="Rigidbody"/>,
        /// moving the kinematic <see cref="Rigidbody"/> during Fixed Update, or by directly updating the <see cref="Transform"/>.
        /// </summary>
        /// <seealso cref="XRBaseInteractable.MovementType"/>
        public MovementType movementType
        {
            get => m_MovementType;
            set => m_MovementType = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_VelocityDamping = k_VelocityDamping;

        /// <summary>
        /// Scale factor of how much to dampen the existing velocity when tracking the position of the Interactor.
        /// The smaller the value, the longer it takes for the velocity to decay.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackPosition"/>
        public float velocityDamping
        {
            get => m_VelocityDamping;
            set => m_VelocityDamping = value;
        }

        [SerializeField]
        float m_VelocityScale = k_VelocityScale;

        /// <summary>
        /// Scale factor applied to the tracked velocity while updating the <see cref="Rigidbody"/>
        /// when tracking the position of the Interactor.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackPosition"/>
        public float velocityScale
        {
            get => m_VelocityScale;
            set => m_VelocityScale = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_AngularVelocityDamping = k_AngularVelocityDamping;

        /// <summary>
        /// Scale factor of how much to dampen the existing angular velocity when tracking the rotation of the Interactor.
        /// The smaller the value, the longer it takes for the angular velocity to decay.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackRotation"/>
        public float angularVelocityDamping
        {
            get => m_AngularVelocityDamping;
            set => m_AngularVelocityDamping = value;
        }

        [SerializeField]
        float m_AngularVelocityScale = k_AngularVelocityScale;

        /// <summary>
        /// Scale factor applied to the tracked angular velocity while updating the <see cref="Rigidbody"/>
        /// when tracking the rotation of the Interactor.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackRotation"/>
        public float angularVelocityScale
        {
            get => m_AngularVelocityScale;
            set => m_AngularVelocityScale = value;
        }

        [SerializeField]
        bool m_TrackPosition = true;

        /// <summary>
        /// Whether this object should follow the position of the Interactor when selected.
        /// </summary>
        /// <seealso cref="trackRotation"/>
        public bool trackPosition
        {
            get => m_TrackPosition;
            set => m_TrackPosition = value;
        }

        [SerializeField]
        bool m_SmoothPosition;

        /// <summary>
        /// Apply smoothing while following the position of the Interactor when selected.
        /// </summary>
        /// <seealso cref="smoothPositionAmount"/>
        /// <seealso cref="tightenPosition"/>
        public bool smoothPosition
        {
            get => m_SmoothPosition;
            set => m_SmoothPosition = value;
        }

        [SerializeField, Range(0f, 20f)]
        float m_SmoothPositionAmount = k_DefaultSmoothingAmount;

        /// <summary>
        /// Scale factor for how much smoothing is applied while following the position of the Interactor when selected.
        /// The larger the value, the closer this object will remain to the position of the Interactor.
        /// </summary>
        /// <seealso cref="smoothPosition"/>
        /// <seealso cref="tightenPosition"/>
        public float smoothPositionAmount
        {
            get => m_SmoothPositionAmount;
            set => m_SmoothPositionAmount = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_TightenPosition = k_DefaultTighteningAmount;

        /// <summary>
        /// Reduces the maximum follow position difference when using smoothing.
        /// </summary>
        /// <remarks>
        /// Fractional amount of how close the smoothed position should remain to the position of the Interactor when using smoothing.
        /// The value ranges from 0 meaning no bias in the smoothed follow distance,
        /// to 1 meaning effectively no smoothing at all.
        /// </remarks>
        /// <seealso cref="smoothPosition"/>
        /// <seealso cref="smoothPositionAmount"/>
        public float tightenPosition
        {
            get => m_TightenPosition;
            set => m_TightenPosition = value;
        }

        [SerializeField]
        bool m_TrackRotation = true;

        /// <summary>
        /// Whether this object should follow the rotation of the Interactor when selected.
        /// </summary>
        /// <seealso cref="trackPosition"/>
        public bool trackRotation
        {
            get => m_TrackRotation;
            set => m_TrackRotation = value;
        }

        [SerializeField]
        bool m_SmoothRotation;

        /// <summary>
        /// Apply smoothing while following the rotation of the Interactor when selected.
        /// </summary>
        /// <seealso cref="smoothRotationAmount"/>
        /// <seealso cref="tightenRotation"/>
        public bool smoothRotation
        {
            get => m_SmoothRotation;
            set => m_SmoothRotation = value;
        }

        [SerializeField, Range(0f, 20f)]
        float m_SmoothRotationAmount = k_DefaultSmoothingAmount;

        /// <summary>
        /// Scale factor for how much smoothing is applied while following the rotation of the Interactor when selected.
        /// The larger the value, the closer this object will remain to the rotation of the Interactor.
        /// </summary>
        /// <seealso cref="smoothRotation"/>
        /// <seealso cref="tightenRotation"/>
        public float smoothRotationAmount
        {
            get => m_SmoothRotationAmount;
            set => m_SmoothRotationAmount = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_TightenRotation = k_DefaultTighteningAmount;

        /// <summary>
        /// Reduces the maximum follow rotation difference when using smoothing.
        /// </summary>
        /// <remarks>
        /// Fractional amount of how close the smoothed rotation should remain to the rotation of the Interactor when using smoothing.
        /// The value ranges from 0 meaning no bias in the smoothed follow rotation,
        /// to 1 meaning effectively no smoothing at all.
        /// </remarks>
        /// <seealso cref="smoothRotation"/>
        /// <seealso cref="smoothRotationAmount"/>
        public float tightenRotation
        {
            get => m_TightenRotation;
            set => m_TightenRotation = value;
        }

        [SerializeField]
        bool m_ThrowOnDetach = true;

        /// <summary>
        /// Whether this object inherits the velocity of the Interactor when released.
        /// </summary>
        public bool throwOnDetach
        {
            get => m_ThrowOnDetach;
            set => m_ThrowOnDetach = value;
        }

        [SerializeField]
        float m_ThrowSmoothingDuration = k_DefaultThrowSmoothingDuration;

        /// <summary>
        /// Time period to average thrown velocity over.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwSmoothingDuration
        {
            get => m_ThrowSmoothingDuration;
            set => m_ThrowSmoothingDuration = value;
        }

        [SerializeField]
        AnimationCurve m_ThrowSmoothingCurve = AnimationCurve.Linear(1f, 1f, 1f, 0f);

        /// <summary>
        /// The curve to use to weight thrown velocity smoothing (most recent frames to the right).
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public AnimationCurve throwSmoothingCurve
        {
            get => m_ThrowSmoothingCurve;
            set => m_ThrowSmoothingCurve = value;
        }

        [SerializeField]
        float m_ThrowVelocityScale = k_DefaultThrowVelocityScale;

        /// <summary>
        /// Scale factor applied to this object's inherited velocity of the Interactor when released.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwVelocityScale
        {
            get => m_ThrowVelocityScale;
            set => m_ThrowVelocityScale = value;
        }

        [SerializeField]
        float m_ThrowAngularVelocityScale = k_DefaultThrowAngularVelocityScale;

        /// <summary>
        /// Scale factor applied to this object's inherited angular velocity of the Interactor when released.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwAngularVelocityScale
        {
            get => m_ThrowAngularVelocityScale;
            set => m_ThrowAngularVelocityScale = value;
        }

        [SerializeField, FormerlySerializedAs("m_GravityOnDetach")]
        bool m_ForceGravityOnDetach;

        /// <summary>
        /// Force this object to have gravity when released
        /// (will still use pre-grab value if this is <see langword="false"/>).
        /// </summary>
        public bool forceGravityOnDetach
        {
            get => m_ForceGravityOnDetach;
            set => m_ForceGravityOnDetach = value;
        }

        /// <summary>
        /// Force this object to have gravity when released
        /// (will still use pre-grab value if this is <see langword="false"/>).
        /// </summary>
        [Obsolete("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach")]
        public bool gravityOnDetach
        {
            get => forceGravityOnDetach;
            set => forceGravityOnDetach = value;
        }

        [SerializeField]
        bool m_RetainTransformParent = true;

        /// <summary>
        /// Whether to set the parent of this object back to its original parent this object was a child of after this object is dropped.
        /// </summary>
        public bool retainTransformParent
        {
            get => m_RetainTransformParent;
            set => m_RetainTransformParent = value;
        }

        // Point we are attaching to on this Interactable (in Interactor's attach coordinate space)
        Vector3 m_InteractorLocalPosition;
        Quaternion m_InteractorLocalRotation;

        // Point we are moving towards each frame (eventually will be at Interactor's attach point)
        Vector3 m_TargetWorldPosition;
        Quaternion m_TargetWorldRotation;

        float m_CurrentAttachEaseTime;
        MovementType m_CurrentMovementType;

        bool m_DetachInLateUpdate;
        Vector3 m_DetachVelocity;
        Vector3 m_DetachAngularVelocity;

        int m_ThrowSmoothingCurrentFrame;
        readonly float[] m_ThrowSmoothingFrameTimes = new float[k_ThrowSmoothingFrameCount];
        readonly Vector3[] m_ThrowSmoothingVelocityFrames = new Vector3[k_ThrowSmoothingFrameCount];
        readonly Vector3[] m_ThrowSmoothingAngularVelocityFrames = new Vector3[k_ThrowSmoothingFrameCount];

        Rigidbody m_Rigidbody;
        Vector3 m_LastPosition;
        Quaternion m_LastRotation;

        // Rigidbody's settings upon select, kept to restore these values when dropped
        bool m_WasKinematic;
        bool m_UsedGravity;
        float m_OldDrag;
        float m_OldAngularDrag;

        Transform m_OriginalSceneParent;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            m_CurrentMovementType = m_MovementType;
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
                Debug.LogError("Grab Interactable does not have a required Rigidbody.", this);
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            switch (updatePhase)
            {
                // During Fixed update we want to perform any physics based updates (e.g., Kinematic or VelocityTracking).
                // The call to MoveToTarget will perform the correct the type of update depending on the update phase.
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    if (isSelected)
                    {
                        if (m_CurrentMovementType == MovementType.Kinematic)
                        {
                            PerformKinematicUpdate(updatePhase);
                        }
                        else if (m_CurrentMovementType == MovementType.VelocityTracking)
                        {
                            PerformVelocityTrackingUpdate(Time.unscaledDeltaTime, updatePhase);
                        }
                    }

                    break;

                // During Dynamic update we want to perform any GameObject based manipulation (e.g., Instantaneous).
                // The call to MoveToTarget will perform the correct the type of update depending on the update phase.
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (isSelected)
                    {
                        UpdateTarget(Time.unscaledDeltaTime);
                        SmoothVelocityUpdate();
                        if (m_CurrentMovementType == MovementType.Instantaneous)
                        {
                            PerformInstantaneousUpdate(updatePhase);
                        }
                    }

                    break;

                // During OnBeforeUpdate we want to perform any last minute GameObject position changes before rendering (e.g., Instantaneous).
                // The call to MoveToTarget will perform the correct the type of update depending on the update phase.
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    if (isSelected)
                    {
                        UpdateTarget(Time.unscaledDeltaTime);
                        if (m_CurrentMovementType == MovementType.Instantaneous)
                        {
                            PerformInstantaneousUpdate(updatePhase);
                        }
                    }

                    break;

                // Late update is only used to handle detach as late as possible.
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    if (m_DetachInLateUpdate)
                    {
                        if (selectingInteractor == null)
                            Detach();
                        m_DetachInLateUpdate = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// Calculate the world position to place this object at when selected.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <returns>Returns the attach position in world space.</returns>
        Vector3 GetWorldAttachPosition(XRBaseInteractor interactor)
        {
            return interactor.attachTransform.position + interactor.attachTransform.rotation * m_InteractorLocalPosition;
        }

        /// <summary>
        /// Calculate the world rotation to place this object at when selected.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <returns>Returns the attach rotation in world space.</returns>
        Quaternion GetWorldAttachRotation(XRBaseInteractor interactor)
        {
            return interactor.attachTransform.rotation * m_InteractorLocalRotation;
        }

        void UpdateTarget(float timeDelta)
        {
            // Compute the unsmoothed target world position and rotation
            var rawTargetWorldPosition = GetWorldAttachPosition(selectingInteractor);
            var rawTargetWorldRotation = GetWorldAttachRotation(selectingInteractor);

            // Apply smoothing (if configured)
            if (m_AttachEaseInTime > 0f && m_CurrentAttachEaseTime <= m_AttachEaseInTime)
            {
                var currentAttachDelta = m_CurrentAttachEaseTime / m_AttachEaseInTime;
                m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, currentAttachDelta);
                m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, currentAttachDelta);
                m_CurrentAttachEaseTime += Time.unscaledDeltaTime;
            }
            else
            {
                if (m_SmoothPosition)
                {
                    m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, m_SmoothPositionAmount * timeDelta);
                    m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, m_TightenPosition);
                }
                else
                    m_TargetWorldPosition = rawTargetWorldPosition;

                if (m_SmoothRotation)
                {
                    m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, m_SmoothRotationAmount * timeDelta);
                    m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, m_TightenRotation);
                }
                else
                    m_TargetWorldRotation = rawTargetWorldRotation;
            }
        }

        void PerformInstantaneousUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic ||
                updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                if (m_TrackPosition)
                {
                    transform.position = m_TargetWorldPosition;
                }
                if (m_TrackRotation)
                {
                    transform.rotation = m_TargetWorldRotation;
                }
            }
        }

        void PerformKinematicUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (m_TrackPosition)
                {
                    var positionDelta = m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass;
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.MovePosition(m_Rigidbody.position + positionDelta);
                }
                if (m_TrackRotation)
                {
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    m_Rigidbody.MoveRotation(m_TargetWorldRotation);
                }
            }
        }

        void PerformVelocityTrackingUpdate(float timeDelta, XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                // Do velocity tracking
                if (m_TrackPosition)
                {
                    // Scale initialized velocity by prediction factor
                    m_Rigidbody.velocity *= (1f - m_VelocityDamping);
                    var posDelta = m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass;
                    var velocity = posDelta / timeDelta;

                    if (!float.IsNaN(velocity.x))
                        m_Rigidbody.velocity += (velocity * m_VelocityScale);
                }

                // Do angular velocity tracking
                if (m_TrackRotation)
                {
                    // Scale initialized velocity by prediction factor
                    m_Rigidbody.angularVelocity *= (1f - m_AngularVelocityDamping);
                    var rotationDelta = m_TargetWorldRotation * Quaternion.Inverse(m_Rigidbody.rotation);
                    rotationDelta.ToAngleAxis(out var angleInDegrees, out var rotationAxis);
                    if (angleInDegrees > 180f)
                        angleInDegrees -= 360f;

                    if (Mathf.Abs(angleInDegrees) > Mathf.Epsilon)
                    {
                        var angularVelocity = (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / timeDelta;
                        if (!float.IsNaN(angularVelocity.x))
                            m_Rigidbody.angularVelocity += (angularVelocity * m_AngularVelocityScale);
                    }
                }
            }
        }

        void UpdateInteractorLocalPose(XRBaseInteractor interactor)
        {
            // In order to move the Interactable to the Interactor we need to
            // calculate the Interactable attach point in the coordinate system of the
            // Interactor's attach point.
#pragma warning disable IDE0029 // Use coalesce expression -- Do not use for UnityEngine.Object types
            var attachTransform = m_AttachTransform != null ? m_AttachTransform : transform;
#pragma warning restore IDE0029
            var attachPosition = m_AttachTransform != null ? m_AttachTransform.position : transform.position;
            var attachOffset = m_Rigidbody.worldCenterOfMass - attachPosition;
            var localAttachOffset = attachTransform.InverseTransformDirection(attachOffset);

            var inverseLocalScale = interactor.attachTransform.lossyScale;
            inverseLocalScale = new Vector3(1f / inverseLocalScale.x, 1f / inverseLocalScale.y, 1f / inverseLocalScale.z);
            localAttachOffset.Scale(inverseLocalScale);

            m_InteractorLocalPosition = localAttachOffset;
            m_InteractorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(m_Rigidbody.rotation) * attachTransform.rotation);
        }

        /// <inheritdoc />
        protected internal override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            Grab();
        }

        /// <inheritdoc />
        protected internal override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            Drop();
        }

        /// <summary>
        /// Updates the state of the object due to being grabbed.
        /// Automatically called when entering the Select state.
        /// </summary>
        /// <seealso cref="Drop"/>
        protected virtual void Grab()
        {
            m_OriginalSceneParent = transform.parent;
            transform.SetParent(null);

            // Special case where the interactor will override this objects movement type (used for Sockets and other absolute interactors)
            m_CurrentMovementType = selectingInteractor.selectedInteractableMovementTypeOverride ?? m_MovementType;

            SetupRigidbodyGrab(m_Rigidbody);

            // Reset detach velocities
            m_DetachVelocity = Vector3.zero;
            m_DetachAngularVelocity = Vector3.zero;

            UpdateInteractorLocalPose(selectingInteractor);

            if (m_AttachEaseInTime > 0f)
            {
                m_TargetWorldPosition = m_Rigidbody.worldCenterOfMass;
                m_TargetWorldRotation = transform.rotation;
                m_CurrentAttachEaseTime = 0f;
            }

            SmoothVelocityStart();
        }

        /// <summary>
        /// Updates the state of the object due to being dropped and schedule to finish the detach during the end of the frame.
        /// Automatically called when exiting the Select state.
        /// </summary>
        /// <seealso cref="Detach"/>
        /// <seealso cref="Grab"/>
        protected virtual void Drop()
        {
            if (m_RetainTransformParent && m_OriginalSceneParent != null && !m_OriginalSceneParent.gameObject.activeInHierarchy)
            {
                bool exitingPlayMode;
#if UNITY_EDITOR
                // Suppress the warning when exiting Play mode to avoid confusing the user
                exitingPlayMode = UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
                exitingPlayMode = false;
#endif
                if (!exitingPlayMode)
                    Debug.LogWarning("Retain Transform Parent is set to true, and has a non-null Original Scene Parent. " +
                        "However, the old parent is deactivated so we are choosing not to re-parent upon dropping.", this);
            }
            else if (m_RetainTransformParent && gameObject.activeInHierarchy)
                transform.SetParent(m_OriginalSceneParent);

            SetupRigidbodyDrop(m_Rigidbody);

            m_CurrentMovementType = m_MovementType;
            m_DetachInLateUpdate = true;
            SmoothVelocityEnd();
        }

        /// <summary>
        /// Updates the state of the object to finish the detach after being dropped.
        /// Automatically called during the end of the frame after being dropped.
        /// </summary>
        /// <remarks>
        /// This method will update the velocity of the Rigidbody if configured to do so.
        /// </remarks>
        /// <seealso cref="Drop"/>
        protected virtual void Detach()
        {
            if (m_ThrowOnDetach)
            {
                m_Rigidbody.velocity = m_DetachVelocity;
                m_Rigidbody.angularVelocity = m_DetachAngularVelocity;
            }
        }

        /// <summary>
        /// Setup the <see cref="Rigidbody"/> on this object due to being grabbed.
        /// Automatically called when entering the Select state.
        /// </summary>
        /// <param name="rigidbody">The <see cref="Rigidbody"/> on this object.</param>
        /// <seealso cref="SetupRigidbodyDrop"/>
        // ReSharper disable once ParameterHidesMember
        protected virtual void SetupRigidbodyGrab(Rigidbody rigidbody)
        {
            // Remember Rigidbody settings and setup to move
            m_WasKinematic = rigidbody.isKinematic;
            m_UsedGravity = rigidbody.useGravity;
            m_OldDrag = rigidbody.drag;
            m_OldAngularDrag = rigidbody.angularDrag;
            rigidbody.isKinematic = m_CurrentMovementType == MovementType.Kinematic;
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        /// <summary>
        /// Setup the <see cref="Rigidbody"/> on this object due to being dropped.
        /// Automatically called when exiting the Select state.
        /// </summary>
        /// <param name="rigidbody">The <see cref="Rigidbody"/> on this object.</param>
        /// <seealso cref="SetupRigidbodyGrab"/>
        // ReSharper disable once ParameterHidesMember
        protected virtual void SetupRigidbodyDrop(Rigidbody rigidbody)
        {
            // Restore Rigidbody settings
            rigidbody.isKinematic = m_WasKinematic;
            rigidbody.useGravity = m_UsedGravity | m_ForceGravityOnDetach;
            rigidbody.drag = m_OldDrag;
            rigidbody.angularDrag = m_OldAngularDrag;
        }

        void SmoothVelocityStart()
        {
            if (selectingInteractor == null)
                return;
            m_LastPosition = selectingInteractor.attachTransform.position;
            m_LastRotation = selectingInteractor.attachTransform.rotation;
            Array.Clear(m_ThrowSmoothingFrameTimes, 0, m_ThrowSmoothingFrameTimes.Length);
            Array.Clear(m_ThrowSmoothingVelocityFrames, 0, m_ThrowSmoothingVelocityFrames.Length);
            Array.Clear(m_ThrowSmoothingAngularVelocityFrames, 0, m_ThrowSmoothingAngularVelocityFrames.Length);
            m_ThrowSmoothingCurrentFrame = 0;
        }

        void SmoothVelocityEnd()
        {
            if (m_ThrowOnDetach)
            {
                var smoothedVelocity = GetSmoothedVelocityValue(m_ThrowSmoothingVelocityFrames);
                var smoothedAngularVelocity = GetSmoothedVelocityValue(m_ThrowSmoothingAngularVelocityFrames);
                m_DetachVelocity = smoothedVelocity * m_ThrowVelocityScale;
                m_DetachAngularVelocity = smoothedAngularVelocity * m_ThrowAngularVelocityScale;
            }
        }

        void SmoothVelocityUpdate()
        {
            if (selectingInteractor == null)
                return;
            m_ThrowSmoothingFrameTimes[m_ThrowSmoothingCurrentFrame] = Time.time;
            m_ThrowSmoothingVelocityFrames[m_ThrowSmoothingCurrentFrame] = (selectingInteractor.attachTransform.position - m_LastPosition) / Time.deltaTime;

            var velocityDiff = (selectingInteractor.attachTransform.rotation * Quaternion.Inverse(m_LastRotation));
            m_ThrowSmoothingAngularVelocityFrames[m_ThrowSmoothingCurrentFrame] = (new Vector3(Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.x), Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.y), Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.z))
                / Time.deltaTime) * Mathf.Deg2Rad;

            m_ThrowSmoothingCurrentFrame = (m_ThrowSmoothingCurrentFrame + 1) % k_ThrowSmoothingFrameCount;
            m_LastPosition = selectingInteractor.attachTransform.position;
            m_LastRotation = selectingInteractor.attachTransform.rotation;
        }

        Vector3 GetSmoothedVelocityValue(Vector3[] velocityFrames)
        {
            var calcVelocity = new Vector3();
            var totalWeights = 0f;
            for (var frameCounter = 0; frameCounter < k_ThrowSmoothingFrameCount; ++frameCounter)
            {
                var frameIdx = (((m_ThrowSmoothingCurrentFrame - frameCounter - 1) % k_ThrowSmoothingFrameCount) + k_ThrowSmoothingFrameCount) % k_ThrowSmoothingFrameCount;
                if (m_ThrowSmoothingFrameTimes[frameIdx] == 0f)
                    break;

                var timeAlpha = (Time.time - m_ThrowSmoothingFrameTimes[frameIdx]) / m_ThrowSmoothingDuration;
                var velocityWeight = m_ThrowSmoothingCurve.Evaluate(Mathf.Clamp(1f - timeAlpha, 0f, 1f));
                calcVelocity += velocityFrames[frameIdx] * velocityWeight;
                totalWeights += velocityWeight;
                if (Time.time - m_ThrowSmoothingFrameTimes[frameIdx] > m_ThrowSmoothingDuration)
                    break;
            }

            if (totalWeights > 0f)
                return calcVelocity / totalWeights;

            return Vector3.zero;
        }
    }
}
