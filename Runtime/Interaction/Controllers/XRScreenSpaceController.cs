using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets screen presses and gestures by using actions from the  Input System and converting them
    /// into XR Interaction states, such as Select. It applies the current press position on the screen to 
    /// move the transform of the GameObject.
    /// </summary>
    /// <remarks>
    /// This behavior requires that the Input System is enabled in the <b>Active Input Handling</b>
    /// setting in <b>Edit &gt; Project Settings &gt; Player</b> for input values to be read.
    /// Each input action must also be enabled to read the current value of the action. Referenced
    /// input actions in an Input Action Asset are not enabled by default.
    /// </remarks>
    /// <seealso cref="XRBaseController"/>
    /// <seealso cref="TouchscreenGestureInputController"/>
    [AddComponentMenu("XR/XR Screen Space Controller", 11)]
    [HelpURL(XRHelpURLConstants.k_XRScreenSpaceController)]
    public class XRScreenSpaceController : XRBaseController
    {
        [SerializeField]
        [Tooltip("The action to use for the screen tap position. (Vector 2 Control).")]
        InputActionProperty m_TapStartPositionAction = new InputActionProperty(new InputAction("Tap Start Position", expectedControlType: "Vector2"));
        /// <summary>
        /// The Input System action to use for reading screen Tap Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty tapStartPositionAction
        {
            get => m_TapStartPositionAction;
            set => SetInputActionProperty(ref m_TapStartPositionAction, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the current screen drag position. (Vector 2 Control).")]
        InputActionProperty m_DragCurrentPositionAction = new InputActionProperty(new InputAction("Drag Current Position", expectedControlType: "Vector2"));
        /// <summary>
        /// The Input System action to use for reading the screen Drag Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="dragDeltaAction"/>
        public InputActionProperty dragCurrentPositionAction
        {
            get => m_DragCurrentPositionAction;
            set => SetInputActionProperty(ref m_DragCurrentPositionAction, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the delta of the screen drag. (Vector 2 Control).")]
        InputActionProperty m_DragDeltaAction = new InputActionProperty(new InputAction("Drag Delta", expectedControlType: "Vector2"));
        /// <summary>
        /// The Input System action used to read the delta Drag values for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="dragCurrentPositionAction"/>
        public InputActionProperty dragDeltaAction
        {
            get => m_DragDeltaAction;
            set => SetInputActionProperty(ref m_DragDeltaAction, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the screen pinch gesture start position. (Vector 2 Control).")]
        InputActionProperty m_PinchStartPosition = new InputActionProperty(new InputAction("Pinch Start Position", expectedControlType: "Vector2"));
        /// <summary>
        /// The Input System action to use for reading the Pinch Start Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="pinchGapDelta"/>
        public InputActionProperty pinchStartPosition
        {
            get => m_PinchStartPosition;
            set => SetInputActionProperty(ref m_PinchStartPosition, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the delta of the screen pinch gesture. (Axis Control).")]
        InputActionProperty m_PinchGapDeltaAction = new InputActionProperty(new InputAction("Pinch Gap Delta", expectedControlType: "Axis"));
        /// <summary>
        /// The Input System action used to read the delta Pinch values for this GameObject. Must be a <see cref="AxisControl"/> Control.
        /// </summary>
        /// <seealso cref="pinchStartPosition"/>
        public InputActionProperty pinchGapDelta
        {
            get => m_PinchGapDeltaAction;
            set => SetInputActionProperty(ref m_PinchGapDeltaAction, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the screen twist gesture start position. (Vector 2 Control).")]
        InputActionProperty m_TwistStartPosition = new InputActionProperty(new InputAction("Twist Start Position", expectedControlType: "Vector2"));
        /// <summary>
        /// The Input System action to use for reading the Twist Start Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="twistRotationDeltaAction"/>
        public InputActionProperty twistStartPosition
        {
            get => m_TwistStartPosition;
            set => SetInputActionProperty(ref m_TwistStartPosition, value);
        }

        [SerializeField]
        [Tooltip("The action to use for the delta of the screen twist gesture. (Axis Control).")]
        InputActionProperty m_TwistRotationDeltaAction = new InputActionProperty(new InputAction("Twist Delta Rotation", expectedControlType: "Axis"));
        /// <summary>
        /// The Input System action used to read the delta Twist values for this GameObject. Must be a <see cref="AxisControl"/> Control.
        /// </summary>
        /// <seealso cref="twistStartPosition"/>
        public InputActionProperty twistRotationDeltaAction
        {
            get => m_TwistRotationDeltaAction;
            set => SetInputActionProperty(ref m_TwistRotationDeltaAction, value);
        }

        [SerializeField]
        [Tooltip("The number of concurrent touches on the screen. (Integer Control).")]
        InputActionProperty m_ScreenTouchCount = new InputActionProperty(new InputAction("Screen Touch Count", expectedControlType: "Integer"));
        /// <summary>
        /// The number of concurrent touches on the screen.
        /// </summary>
        public InputActionProperty screenTouchCount
        {
            get => m_ScreenTouchCount;
            set => SetInputActionProperty(ref m_ScreenTouchCount, value);
        }

        [SerializeField]
        [Tooltip("The camera associated with the screen, and through which screen presses/touches will be interpreted.")]
        Camera m_ControllerCamera;
        /// <summary>
        /// The camera associated with the screen, and through which screen presses/touches will be interpreted.
        /// </summary>
        public Camera controllerCamera
        {
            get => m_ControllerCamera;
            set => m_ControllerCamera = value;
        }

        bool m_TrackingInputActionsEnabled;
        bool m_InputActionsEnabled;

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);
            if (controllerState == null)
                return;
   
            // Warn the user if using referenced actions and they are disabled
            if (!m_TrackingInputActionsEnabled)
            {
                if ((m_DragCurrentPositionAction != null && IsDisabledReferenceAction(m_DragCurrentPositionAction)) ||
                    (m_TapStartPositionAction != null && IsDisabledReferenceAction(m_TapStartPositionAction)) ||
                    (m_TwistStartPosition != null && IsDisabledReferenceAction(m_TwistStartPosition)))
                {
                    Debug.LogWarning("'Enable Input Tracking' is enabled, but the Tap, Drag, Pinch and/or Twist Action is disabled." +
                        " The pose of the controller will not be updated correctly until the Input Actions are enabled." +
                        " Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action." +
                        " The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.",
                        this);
                }
                else
                {
                    m_TrackingInputActionsEnabled = true;
                }
            }

            controllerState.isTracked = true;
            controllerState.inputTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

            if (m_TrackingInputActionsEnabled && TryGetCurrentPositionAction(out var posAction))
            {
                var screenPos =  posAction.ReadValue<Vector2>();                
                Vector3 screenToWorldPoint = m_ControllerCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, m_ControllerCamera.nearClipPlane));
                Vector3 directionVector = (screenToWorldPoint - m_ControllerCamera.transform.position).normalized;
                controllerState.position = transform.parent? transform.parent.InverseTransformPoint(screenToWorldPoint): screenToWorldPoint;
                controllerState.rotation = Quaternion.LookRotation(directionVector);           
            }
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);
            if (controllerState == null)
                return;

            // Warn the user if using referenced actions and they are disabled
            if (!m_InputActionsEnabled)
            {
                if ((m_DragCurrentPositionAction != null && IsDisabledReferenceAction(m_DragCurrentPositionAction)) ||
                    (m_TapStartPositionAction != null && IsDisabledReferenceAction(m_TapStartPositionAction)) ||
                    (m_TwistRotationDeltaAction != null && IsDisabledReferenceAction(m_TwistRotationDeltaAction)))
                {
                    Debug.LogWarning("'Enable Input Actions' is enabled, but the Tap, Drag, Pinch and/or Twist Action is disabled." +
                        " The controller input will not be handled correctly until the Input Actions are enabled." +
                        " Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action." +
                        " The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.",
                        this);
                }
                else
                {
                    m_InputActionsEnabled = true;
                }
            }

            controllerState.ResetFrameDependentStates();

            if(m_InputActionsEnabled && TryGetCurrentTwoInputSelectAction(out InputAction twoInputSelectAction))
            {
                controllerState.selectInteractionState.SetFrameState((twoInputSelectAction.phase == InputActionPhase.Started || twoInputSelectAction.phase == InputActionPhase.Performed), twoInputSelectAction.ReadValue<float>());
            }
            else if(m_InputActionsEnabled && TryGetCurrentOneInputSelectAction(out InputAction oneInputSelectAction))
            {   
                controllerState.selectInteractionState.SetFrameState(oneInputSelectAction.phase == InputActionPhase.Started, oneInputSelectAction.ReadValue<Vector2>().magnitude);
            }
            else
            {
                controllerState.selectInteractionState.SetFrameState(false, 0);
            }
        }

        bool TryGetCurrentPositionAction(out InputAction action)
        {   
            int touchCount = m_ScreenTouchCount.action.ReadValue<int>();
            if(touchCount <= 1)
            {
                if (m_DragCurrentPositionAction.action != null && m_DragCurrentPositionAction.action.phase == InputActionPhase.Started)
                {
                    action = m_DragCurrentPositionAction.action;
                    return true;
                }
                else if (m_TapStartPositionAction.action != null && m_TapStartPositionAction.action.phase == InputActionPhase.Started)
                {
                    action = m_TapStartPositionAction.action;
                    return true;
                }
            }
            else if (m_TwistStartPosition.action != null && (m_TwistStartPosition.action.phase == InputActionPhase.Started || m_TwistStartPosition.action.phase == InputActionPhase.Performed))
            {
                action = m_TwistStartPosition.action;
                return true;
            }

            action = null;
            return false;
        }

        bool TryGetCurrentOneInputSelectAction(out InputAction action)
        {   
            if (m_DragCurrentPositionAction.action != null && m_DragCurrentPositionAction.action.phase == InputActionPhase.Started)
            {
                action = m_DragCurrentPositionAction.action;
                return true;
            }
            else if (m_TapStartPositionAction.action != null && m_TapStartPositionAction.action.phase == InputActionPhase.Started)
            {
                action = m_TapStartPositionAction.action;
                return true;
            }

            action = null;
            return false;
        }

        bool TryGetCurrentTwoInputSelectAction(out InputAction action)
        {
            if (m_TwistRotationDeltaAction.action != null && (m_TwistRotationDeltaAction.action.phase == InputActionPhase.Started || m_TwistRotationDeltaAction.action.phase == InputActionPhase.Performed))
            {
                action = m_TwistRotationDeltaAction.action;
                return true;
            }

            action = null;
            return false;
        }

        void Start()
        {
            if (m_ControllerCamera == null)
            {
                m_ControllerCamera = Camera.main;
                if (m_ControllerCamera == null)
                {
                    Debug.LogWarning($"Could not find associated {nameof(Camera)} in scene." +
                        $"This {nameof(XRScreenSpaceController)} will be disabled.", this);
                    enabled = false;
                }
            }
        }

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

        void EnableAllDirectActions()
        {
            m_TapStartPositionAction.EnableDirectAction();
            m_DragCurrentPositionAction.EnableDirectAction();
            m_DragDeltaAction.EnableDirectAction();
            m_PinchStartPosition.EnableDirectAction();
            m_PinchGapDeltaAction.EnableDirectAction();
            m_TwistStartPosition.EnableDirectAction();
            m_TwistRotationDeltaAction.EnableDirectAction();
            m_ScreenTouchCount.EnableDirectAction();
        }

        void DisableAllDirectActions()
        {
            m_TapStartPositionAction.DisableDirectAction();
            m_DragCurrentPositionAction.DisableDirectAction();
            m_DragDeltaAction.DisableDirectAction();
            m_PinchStartPosition.DisableDirectAction();
            m_PinchGapDeltaAction.DisableDirectAction();
            m_TwistStartPosition.DisableDirectAction();
            m_TwistRotationDeltaAction.DisableDirectAction();
            m_ScreenTouchCount.DisableDirectAction();
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