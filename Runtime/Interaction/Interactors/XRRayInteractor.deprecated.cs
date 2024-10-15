using System;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    public partial class XRRayInteractor
    {
        /// <summary>
        /// (Deprecated) Initial velocity of the projectile. Increasing this value will make the curve reach further.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        /// <remarks>
        /// <c>Velocity</c> has been deprecated. Use <see cref="velocity"/> instead.
        /// </remarks>
        [Obsolete("Velocity has been deprecated. Use velocity instead. (UnityUpgradable) -> velocity", true)]
        public float Velocity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gravity of the projectile in the reference frame.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        /// <remarks>
        /// <c>Acceleration</c> has been deprecated. Use <see cref="acceleration"/> instead.
        /// </remarks>
        [Obsolete("Acceleration has been deprecated. Use acceleration instead. (UnityUpgradable) -> acceleration", true)]
        public float Acceleration
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc cref="additionalFlightTime"/>
        /// <remarks>
        /// <c>AdditionalFlightTime</c> has been deprecated. Use <see cref="additionalFlightTime"/> instead.
        /// </remarks>
        [Obsolete("AdditionalFlightTime has been deprecated. Use additionalFlightTime instead. (UnityUpgradable) -> additionalFlightTime", true)]
        public float AdditionalFlightTime
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc cref="angle"/>
        /// <remarks>
        /// <c>Angle</c> has been deprecated. Use <see cref="angle"/> instead.
        /// </remarks>
        [Obsolete("Angle has been deprecated. Use angle instead. (UnityUpgradable) -> angle", true)]
        public float Angle => default;

        /// <summary>
        /// The <see cref="Transform"/> that upon entering selection
        /// (when this Interactor first initiates selection of an Interactable),
        /// this Interactor will copy the pose of the attach <see cref="Transform"/> values into.
        /// </summary>
        /// <remarks>
        /// Automatically instantiated and set in <see cref="Awake"/>.
        /// Setting this will not automatically destroy the previous object.
        /// <br />
        /// <c>originalAttachTransform</c> has been deprecated. Use <see cref="rayOriginTransform"/> instead.
        /// </remarks>
        /// <seealso cref="XRBaseInteractor.attachTransform"/>
        [Obsolete("originalAttachTransform has been deprecated. Use rayOriginTransform instead. (UnityUpgradable) -> rayOriginTransform", true)]
        protected Transform originalAttachTransform
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Obsolete) Use <see cref="ILineRenderable.GetLinePoints"/> instead.
        /// </summary>
        /// <param name="linePoints">Obsolete.</param>
        /// <param name="numPoints">Obsolete.</param>
        /// <param name="_">Dummy value to support old function signature.</param>
        /// <returns>Obsolete.</returns>
        /// <remarks>
        /// <c>GetLinePoints</c> with <c>ref int</c> parameter has been deprecated. Use signature with <c>out int</c> parameter instead.
        /// </remarks>
        [Obsolete("GetLinePoints with ref int parameter has been deprecated. Use signature with out int parameter instead.", true)]
        // ReSharper disable RedundantAssignment
        public bool GetLinePoints(ref Vector3[] linePoints, ref int numPoints, int _ = default)
        // ReSharper restore RedundantAssignment
        {
            return default;
        }

        /// <summary>
        /// (Obsolete) Use <see cref="ILineRenderable.TryGetHitInfo"/> instead.
        /// </summary>
        /// <param name="position">Obsolete.</param>
        /// <param name="normal">Obsolete.</param>
        /// <param name="positionInLine">Obsolete.</param>
        /// <param name="isValidTarget">Obsolete.</param>
        /// <param name="_">Dummy value to support old function signature.</param>
        /// <returns>Obsolete.</returns>
        /// <remarks>
        /// <c>TryGetHitInfo</c> with <c>ref</c> parameters has been deprecated. Use signature with <c>out</c> parameters instead.
        /// </remarks>
        [Obsolete("TryGetHitInfo with ref parameters has been deprecated. Use signature with out parameters instead.", true)]
        // ReSharper disable RedundantAssignment
        public bool TryGetHitInfo(ref Vector3 position, ref Vector3 normal, ref int positionInLine, ref bool isValidTarget, int _ = default)
        // ReSharper restore RedundantAssignment
        {
            return default;
        }

        /// <inheritdoc cref="TryGetCurrent3DRaycastHit(out RaycastHit)"/>
        /// <remarks>
        /// <c>GetCurrentRaycastHit</c> has been deprecated. Use <see cref="TryGetCurrent3DRaycastHit(out RaycastHit)"/> instead.
        /// </remarks>
        [Obsolete("GetCurrentRaycastHit has been deprecated. Use TryGetCurrent3DRaycastHit instead. (UnityUpgradable) -> TryGetCurrent3DRaycastHit(*)", true)]
        public bool GetCurrentRaycastHit(out RaycastHit raycastHit)
        {
            raycastHit = default;
            return default;
        }

        /// <summary>
        /// (Deprecated) Sets how Attach Transform rotation is controlled.
        /// </summary>
        /// <seealso cref="anchorRotationMode"/>
        [Obsolete("AnchorRotationMode has been deprecated in version 3.0.0. Use RotateMode instead.")]
        public enum AnchorRotationMode
        {
            /// <summary>
            /// The Attach Transform rotates over time while rotation input is active.
            /// </summary>
            RotateOverTime,

            /// <summary>
            /// The Attach Transform rotates to match the direction of the 2-dimensional rotation input.
            /// </summary>
            MatchDirection,
        }

        /// <summary>
        /// (Deprecated) Allows the user to move the Attach Transform using the thumbstick.
        /// </summary>
        /// <seealso cref="rotateSpeed"/>
        /// <seealso cref="translateSpeed"/>
        /// <seealso cref="anchorRotateReferenceFrame"/>
        /// <seealso cref="anchorRotationMode"/>
        [Obsolete("allowAnchorControl has been renamed in version 3.0.0. Use manipulateAttachTransform instead. (UnityUpgradable) -> manipulateAttachTransform")]
        public bool allowAnchorControl
        {
            get => manipulateAttachTransform;
            set => manipulateAttachTransform = value;
        }

        /// <summary>
        /// (Deprecated) The optional reference frame to define the up axis when rotating the attach anchor point.
        /// When not set, rotates about the local up axis of the attach transform.
        /// </summary>
        /// <seealso cref="manipulateAttachTransform"/>
        /// <seealso cref="RotateAnchor(Transform, float)"/>
        /// <seealso cref="RotateAnchor(Transform, Vector2, Quaternion)"/>
        [Obsolete("anchorRotateReferenceFrame has been renamed in version 3.0.0. Use rotateReferenceFrame instead. (UnityUpgradable) -> rotateReferenceFrame")]
        public Transform anchorRotateReferenceFrame
        {
            get => rotateReferenceFrame;
            set => rotateReferenceFrame = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets how the attach rotation manipulation is controlled.
        /// </summary>
        /// <seealso cref="manipulateAttachTransform"/>
        /// <seealso cref="AnchorRotationMode"/>
        [Obsolete("anchorRotationMode has been deprecated in version 3.0.0. Use rotateMode instead.")]
        public AnchorRotationMode anchorRotationMode
        {
            get => (AnchorRotationMode)(int)rotateMode;
            set => rotateMode = (RotateMode)(int)value;
        }

        /// <inheritdoc />
        [Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use uiPressInput to read button input instead.")]
        protected override bool isUISelectActive
        {
            get
            {
                if (m_HoverToSelect && m_HoverUISelectActive)
                    return allowSelect;

                return base.isUISelectActive;
            }
        }

        [Obsolete("m_ActionBasedController has been deprecated in version 3.0.0.")]
        ActionBasedController m_ActionBasedController;
        [Obsolete("m_DeviceBasedController has been deprecated in version 3.0.0.")]
        XRController m_DeviceBasedController;
        [Obsolete("m_ScreenSpaceController has been deprecated in version 3.0.0.")]
        XRScreenSpaceController m_ScreenSpaceController;
        [Obsolete("m_IsActionBasedController has been deprecated in version 3.0.0.")]
        bool m_IsActionBasedController;
        [Obsolete("m_IsDeviceBasedController has been deprecated in version 3.0.0.")]
        bool m_IsDeviceBasedController;
        [Obsolete("m_IsScreenSpaceController has been deprecated in version 3.0.0.")]
        bool m_IsScreenSpaceController;

        [Obsolete("ProcessManipulationInputDeviceBasedController has been deprecated in version 3.0.0.")]
        void ProcessManipulationInputDeviceBasedController()
        {
            if (!m_IsDeviceBasedController || !m_DeviceBasedController.inputDevice.isValid)
                return;

            m_DeviceBasedController.inputDevice.IsPressed(m_DeviceBasedController.moveObjectIn, out var inPressed, m_DeviceBasedController.axisToPressThreshold);
            m_DeviceBasedController.inputDevice.IsPressed(m_DeviceBasedController.moveObjectOut, out var outPressed, m_DeviceBasedController.axisToPressThreshold);

            if (inPressed || outPressed)
            {
                var directionAmount = inPressed ? 1f : -1f;
                TranslateAnchor(effectiveRayOrigin, attachTransform, directionAmount);
            }

            switch (m_RotateMode)
            {
                case RotateMode.RotateOverTime:
                    m_DeviceBasedController.inputDevice.IsPressed(m_DeviceBasedController.rotateObjectLeft, out var leftPressed, m_DeviceBasedController.axisToPressThreshold);
                    m_DeviceBasedController.inputDevice.IsPressed(m_DeviceBasedController.rotateObjectRight, out var rightPressed, m_DeviceBasedController.axisToPressThreshold);
                    if (leftPressed || rightPressed)
                    {
                        var directionAmount = leftPressed ? -1f : 1f;
                        RotateAnchor(attachTransform, directionAmount);
                    }
                    break;

                case RotateMode.MatchDirection:
                    if (m_DeviceBasedController.inputDevice.TryReadAxis2DValue(m_DeviceBasedController.directionalAnchorRotation, out var directionalValue))
                    {
                        var referenceRotation = m_RotateReferenceFrame != null ? m_RotateReferenceFrame.rotation : effectiveRayOrigin.rotation;
                        RotateAnchor(attachTransform, directionalValue, referenceRotation);
                    }
                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(RotateMode)}={m_RotateMode}.");
                    break;
            }
        }

        [Obsolete("ProcessManipulationInputActionBasedController has been deprecated in version 3.0.0.")]
        void ProcessManipulationInputActionBasedController()
        {
            if (!m_IsActionBasedController)
                return;

            // Check if the scaling toggle was performed this frame.
            if (TryReadButton(m_ActionBasedController.scaleToggleAction.action))
            {
                m_ScaleInputActive = !m_ScaleInputActive;
            }

            // If not scaling, we can translate and rotate
            if (!m_ScaleInputActive)
            {
                switch (m_RotateMode)
                {
                    case RotateMode.RotateOverTime:
                        if (TryRead2DAxis(m_ActionBasedController.rotateAnchorAction.action, out var rotateAmt))
                            RotateAnchor(attachTransform, rotateAmt.x);
                        break;

                    case RotateMode.MatchDirection:
                        if (TryRead2DAxis(m_ActionBasedController.directionalAnchorRotationAction.action, out var directionAmt))
                        {
                            var referenceRotation = m_RotateReferenceFrame != null ? m_RotateReferenceFrame.rotation : effectiveRayOrigin.rotation;
                            RotateAnchor(attachTransform, directionAmt, referenceRotation);
                        }
                        break;

                    default:
                        Assert.IsTrue(false, $"Unhandled {nameof(RotateMode)}={m_RotateMode}.");
                        break;
                }

                if (TryRead2DAxis(m_ActionBasedController.translateAnchorAction.action, out var translateAmt))
                {
                    TranslateAnchor(effectiveRayOrigin, attachTransform, translateAmt.y);
                }
            }
            else if (m_ScaleMode == ScaleMode.ScaleOverTime && TryRead2DAxis(m_ActionBasedController.scaleDeltaAction.action, out var scaleAmt))
            {
                scaleValue = scaleAmt.y;
            }
        }

        [Obsolete("ProcessManipulationInputScreenSpaceController has been deprecated in version 3.0.0.")]
        void ProcessManipulationInputScreenSpaceController()
        {
            if (!m_IsScreenSpaceController)
                return;

            switch (m_RotateMode)
            {
                case RotateMode.RotateOverTime:
                    // Not a valid value for a screen space controller, don't do anything.
                    // Warning logged in OnXRControllerChanged.
                    break;

                case RotateMode.MatchDirection:
                    if (m_ScreenSpaceController.twistDeltaRotationAction.action != null &&
                        m_ScreenSpaceController.twistDeltaRotationAction.action.phase.IsInProgress())
                    {
                        var deltaRotation = m_ScreenSpaceController.twistDeltaRotationAction.action.ReadValue<float>();
                        var rotationAmount = -deltaRotation;
                        RotateAnchor(attachTransform, rotationAmount);
                    }
                    else if (m_ScreenSpaceController.dragDeltaAction.action != null &&
                             m_ScreenSpaceController.dragDeltaAction.action.phase.IsInProgress() &&
                             m_ScreenSpaceController.screenTouchCountAction.action?.ReadValue<int>() > 1)
                    {
                        var deltaRotation = m_ScreenSpaceController.dragDeltaAction.action.ReadValue<Vector2>();
                        var worldToVerticalOrientedDevice = Quaternion.Inverse(Quaternion.LookRotation(attachTransform.forward, Vector3.up));
                        var rotatedDelta = worldToVerticalOrientedDevice * attachTransform.rotation * deltaRotation;

                        var rotationAmount = (rotatedDelta.x / Screen.dpi) * -50f;
                        RotateAnchor(attachTransform, rotationAmount);
                    }

                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(RotateMode)}={m_RotateMode}.");
                    break;
            }

            if (m_ScaleMode == ScaleMode.DistanceDelta)
            {
                scaleValue = m_ScreenSpaceController.scaleDelta;
            }
        }

        /// <summary>
        /// Rotates the attach anchor for this interactor. This can be useful to rotate a held object.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The rotation amount.</param>
        [Obsolete("RotateAnchor has been renamed in version 3.0.0. Use RotateAttachTransform instead.")]
        protected virtual void RotateAnchor(Transform anchor, float directionAmount)
        {
            RotateAttachTransform(anchor, directionAmount);
        }

        /// <summary>
        /// Rotates the attach anchor for this interactor to match a given direction. This can be useful to compute a direction angle for teleportation.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="direction">The directional input.</param>
        /// <param name="referenceRotation">The reference rotation to define the up axis for rotation.</param>
        [Obsolete("RotateAnchor has been renamed in version 3.0.0. Use RotateAttachTransform instead.")]
        protected virtual void RotateAnchor(Transform anchor, Vector2 direction, Quaternion referenceRotation)
        {
            RotateAttachTransform(anchor, direction, referenceRotation);
        }

        /// <summary>
        /// Translates the attach anchor for this interactor. This can be useful to move a held object closer or further away from the interactor.
        /// </summary>
        /// <param name="rayOrigin">The starting position and direction of any ray casts.</param>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The translation amount.</param>
        [Obsolete("TranslateAnchor has been renamed in version 3.0.0. Use TranslateAttachTransform instead.")]
        protected virtual void TranslateAnchor(Transform rayOrigin, Transform anchor, float directionAmount)
        {
            TranslateAttachTransform(rayOrigin, anchor, directionAmount);
        }

        [Obsolete("TryRead2DAxis has been deprecated in version 3.0.0.")]
        static bool TryRead2DAxis(InputAction action, out Vector2 output)
        {
            if (action != null)
            {
                output = action.ReadValue<Vector2>();
                return true;
            }
            output = default;
            return false;
        }

        [Obsolete("TryReadButton has been deprecated in version 3.0.0.")]
        static bool TryReadButton(InputAction action)
        {
            return action != null && action.WasPerformedThisFrame();
        }

        /// <inheritdoc />
#pragma warning disable CS0672 // Member overrides obsolete member
#pragma warning disable CS0618 // Type or member is obsolete
        private protected override void OnXRControllerChanged()
        {
            base.OnXRControllerChanged();
            var value = xrController;

            m_ActionBasedController = value as ActionBasedController;
            m_IsActionBasedController = m_ActionBasedController != null;

            m_DeviceBasedController = value as XRController;
            m_IsDeviceBasedController = m_DeviceBasedController != null;

            m_ScreenSpaceController = value as XRScreenSpaceController;
            m_IsScreenSpaceController = m_ScreenSpaceController != null;

            if (forceDeprecatedInput && m_IsScreenSpaceController && m_ManipulateAttachTransform && m_RotateMode == RotateMode.RotateOverTime)
            {
                Debug.LogWarning("Rotate Over Time is not a valid value for Rotation Mode when using XR Screen Space Controller." +
                    " This XR Ray Interactor will not be able to rotate the anchor using screen touches.", this);
            }
        }
#pragma warning restore CS0618
#pragma warning restore CS0672
    }
}
