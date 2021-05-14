using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets feature values on a tracked input controller device using actions from the Input System
    /// into XR Interaction states, such as Select. Additionally, it applies the current Pose value
    /// of a tracked device to the transform of the GameObject.
    /// </summary>
    /// <remarks>
    /// This behavior requires that the Input System is enabled in the <b>Active Input Handling</b>
    /// setting in <b>Edit &gt; Project Settings &gt; Player</b> for input values to be read.
    /// Each input action must also be enabled to read the current value of the action. Referenced
    /// input actions in an Input Action Asset are not enabled by default.
    /// </remarks>
    /// <seealso cref="XRBaseController"/>
    [AddComponentMenu("XR/XR Controller (Action-based)")]
    [HelpURL(XRHelpURLConstants.k_ActionBasedController)]
    public class ActionBasedController : XRBaseController
    {
        [SerializeField]
        InputActionProperty m_PositionAction;
        /// <summary>
        /// The Input System action to use for Position Tracking for this GameObject. Must be a <see cref="Vector3Control"/> Control.
        /// </summary>
        public InputActionProperty positionAction
        {
            get => m_PositionAction;
            set => SetInputActionProperty(ref m_PositionAction, value);
        }

        [SerializeField]
        InputActionProperty m_RotationAction;
        /// <summary>
        /// The Input System action to use for Rotation Tracking for this GameObject. Must be a <see cref="QuaternionControl"/> Control.
        /// </summary>
        public InputActionProperty rotationAction
        {
            get => m_RotationAction;
            set => SetInputActionProperty(ref m_RotationAction, value);
        }

        [SerializeField]
        InputActionProperty m_SelectAction;
        /// <summary>
        /// The Input System action to use for Selecting an Interactable. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionProperty selectAction
        {
            get => m_SelectAction;
            set => SetInputActionProperty(ref m_SelectAction, value);
        }

        [SerializeField]
        InputActionProperty m_ActivateAction;
        /// <summary>
        /// The Input System action to use for Activating a selected Interactable. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionProperty activateAction
        {
            get => m_ActivateAction;
            set => SetInputActionProperty(ref m_ActivateAction, value);
        }

        [SerializeField]
        InputActionProperty m_UIPressAction;
        /// <summary>
        /// The Input System action to use for UI interaction. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionProperty uiPressAction
        {
            get => m_UIPressAction;
            set => SetInputActionProperty(ref m_UIPressAction, value);
        }

        [SerializeField]
        InputActionProperty m_HapticDeviceAction;
        /// <summary>
        /// The Input System action to use for identifying the device to send haptic impulses to.
        /// Can be any control type that will have an active control driving the action.
        /// </summary>
        public InputActionProperty hapticDeviceAction
        {
            get => m_HapticDeviceAction;
            set => SetInputActionProperty(ref m_HapticDeviceAction, value);
        }

        [SerializeField]
        InputActionProperty m_RotateAnchorAction;
        /// <summary>
        /// The Input System action to use for rotating the interactor's attach point.
        /// Must be a <see cref="Vector2Control"/> Control. Will use the X-axis as the rotation input.
        /// </summary>
        public InputActionProperty rotateAnchorAction
        {
            get => m_RotateAnchorAction;
            set => SetInputActionProperty(ref m_RotateAnchorAction, value);
        }

        [SerializeField]
        InputActionProperty m_TranslateAnchorAction;
        /// <summary>
        /// The Input System action to use for translating the interactor's attach point closer or further away from the interactor.
        /// Must be a <see cref="Vector2Control"/> Control. Will use the Y-axis as the translation input.
        /// </summary>
        public InputActionProperty translateAnchorAction
        {
            get => m_TranslateAnchorAction;
            set => SetInputActionProperty(ref m_TranslateAnchorAction, value);
        }

        [SerializeField]
        float m_ButtonPressPoint = 0.5f;

        /// <summary>
        /// The value threshold for when a button is considered pressed to trigger an interaction event.
        /// If a button has a value equal to or greater than this value, it is considered pressed.
        /// </summary>
#if INPUT_SYSTEM_1_1_OR_NEWER
        [Obsolete("Deprecated, this obsolete property is not used when Input System version is 1.1.0 or higher. Configure press point on the action or binding instead.", true)]
#else
        [Obsolete("Marked for deprecation, this property will be removed when Input System dependency version is bumped to 1.1.0.")]
#endif
        public float buttonPressPoint
        {
            get => m_ButtonPressPoint;
            set => m_ButtonPressPoint = value;
        }

        bool m_HasCheckedDisabledTrackingInputReferenceActions;
        bool m_HasCheckedDisabledInputReferenceActions;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            EnableAllDirectActions();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            DisableAllDirectActions();
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            if (controllerState == null)
                return;

            // Warn the user if using referenced actions and they are disabled
            if (!m_HasCheckedDisabledTrackingInputReferenceActions &&
                (m_PositionAction.action != null || m_RotationAction.action != null))
            {
                if (IsDisabledReferenceAction(m_PositionAction) || IsDisabledReferenceAction(m_RotationAction))
                {
                    Debug.LogWarning("'Enable Input Tracking' is enabled, but Position and/or Rotation Action is disabled." +
                        " The pose of the controller will not be updated correctly until the Input Actions are enabled." +
                        " Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action." +
                        " The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.",
                        this);
                }

                m_HasCheckedDisabledTrackingInputReferenceActions = true;
            }

            controllerState.poseDataFlags = PoseDataFlags.NoData;

            if (m_PositionAction.action?.activeControl?.device is TrackedDevice positionTrackedDevice)
            {
                var trackingState = (InputTrackingState)positionTrackedDevice.trackingState.ReadValue();
                if ((trackingState & InputTrackingState.Position) != 0)
                {
                    var pos = m_PositionAction.action.ReadValue<Vector3>();
                    controllerState.position = pos;
                    controllerState.poseDataFlags |= PoseDataFlags.Position;
                }
            }

            if (m_RotationAction.action?.activeControl?.device is TrackedDevice rotationTrackedDevice)
            {
                var trackingState = (InputTrackingState)rotationTrackedDevice.trackingState.ReadValue();
                if ((trackingState & InputTrackingState.Rotation) != 0)
                {
                    var rot = m_RotationAction.action.ReadValue<Quaternion>();
                    controllerState.rotation = rot;
                    controllerState.poseDataFlags |= PoseDataFlags.Rotation;
                }
            }
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);
            if (controllerState == null)
                return;

            // Warn the user if using referenced actions and they are disabled
            if (!m_HasCheckedDisabledInputReferenceActions &&
                (m_SelectAction.action != null || m_ActivateAction.action != null || m_UIPressAction.action != null))
            {
                if (IsDisabledReferenceAction(m_SelectAction) || IsDisabledReferenceAction(m_ActivateAction) || IsDisabledReferenceAction(m_UIPressAction))
                {
                    Debug.LogWarning("'Enable Input Actions' is enabled, but Select, Activate, and/or UI Press Action is disabled." +
                        " The controller input will not be handled correctly until the Input Actions are enabled." +
                        " Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action." +
                        " The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.",
                        this);
                }

                m_HasCheckedDisabledInputReferenceActions = true;
            }

            controllerState.ResetFrameDependentStates();
            controllerState.selectInteractionState.SetFrameState(IsPressed(m_SelectAction.action));
            controllerState.activateInteractionState.SetFrameState(IsPressed(m_ActivateAction.action));
            controllerState.uiPressInteractionState.SetFrameState(IsPressed(m_UIPressAction.action));
        }

        /// <summary>
        /// Evaluates whether the given input action is considered pressed.
        /// </summary>
        /// <param name="action">The input action to check.</param>
        /// <returns>Returns <see langword="true"/> when the input action is considered pressed. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool IsPressed(InputAction action)
        {
            if (action == null)
                return false;

#if INPUT_SYSTEM_1_1_OR_NEWER
                return action.phase == InputActionPhase.Performed;
#else
            if (action.activeControl is ButtonControl buttonControl)
                return buttonControl.isPressed;

            if (action.activeControl is AxisControl)
                return action.ReadValue<float>() >= m_ButtonPressPoint;

            return action.triggered || action.phase == InputActionPhase.Performed;
#endif
        }

        /// <inheritdoc />
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            if (m_HapticDeviceAction.action?.activeControl?.device is XRControllerWithRumble rumbleController)
            {
                rumbleController.SendImpulse(amplitude, duration);
                return true;
            }

            return false;
        }

        void EnableAllDirectActions()
        {
            m_PositionAction.EnableDirectAction();
            m_RotationAction.EnableDirectAction();
            m_SelectAction.EnableDirectAction();
            m_ActivateAction.EnableDirectAction();
            m_UIPressAction.EnableDirectAction();
            m_HapticDeviceAction.EnableDirectAction();
            m_RotateAnchorAction.EnableDirectAction();
            m_TranslateAnchorAction.EnableDirectAction();
        }

        void DisableAllDirectActions()
        {
            m_PositionAction.DisableDirectAction();
            m_RotationAction.DisableDirectAction();
            m_SelectAction.DisableDirectAction();
            m_ActivateAction.DisableDirectAction();
            m_UIPressAction.DisableDirectAction();
            m_HapticDeviceAction.DisableDirectAction();
            m_RotateAnchorAction.DisableDirectAction();
            m_TranslateAnchorAction.DisableDirectAction();
        }

        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }

        static bool IsDisabledReferenceAction(InputActionProperty property) =>
            property.reference != null && property.reference.action != null && !property.reference.action.enabled;
    }
}
