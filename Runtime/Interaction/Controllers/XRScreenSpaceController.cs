
#if ((ENABLE_VR || UNITY_GAMECORE) && AR_FOUNDATION_PRESENT) || PACKAGE_DOCS_GENERATION
#define TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
#endif
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System;

#if TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
using UnityEngine.XR.Interaction.Toolkit.AR.Inputs;
#endif

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
    [Obsolete(k_XRScreenSpaceControllerDeprecated, true)]
    public class XRScreenSpaceController : XRBaseController
    {
        const string k_XRScreenSpaceControllerDeprecated = "XRScreenSpaceController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.";

        /// <summary>
        /// When enabled, a <see cref="TouchscreenGestureInputController"/> will be added to the Input System device list to detect touch gestures.
        /// This input controller drives the gesture values for the input actions for the screen space controller.
        /// </summary>
        public bool enableTouchscreenGestureInputController
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for reading screen Tap Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty tapStartPositionAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for reading the screen Drag Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="dragDeltaAction"/>
        public InputActionProperty dragCurrentPositionAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to read the delta Drag values for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="dragCurrentPositionAction"/>
        public InputActionProperty dragDeltaAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for reading the Pinch Start Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="pinchGapDeltaAction"/>
        public InputActionProperty pinchStartPositionAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to read the Pinch values for this GameObject. Must be an <see cref="AxisControl"/> Control.
        /// </summary>
        /// <seealso cref="pinchGapDeltaAction"/>
        public InputActionProperty pinchGapAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to read the delta Pinch values for this GameObject. Must be an <see cref="AxisControl"/> Control.
        /// </summary>
        /// <seealso cref="pinchStartPositionAction"/>
        public InputActionProperty pinchGapDeltaAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for reading the Twist Start Position for this GameObject. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="twistDeltaRotationAction"/>
        public InputActionProperty twistStartPositionAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to read the delta Twist values for this GameObject. Must be an <see cref="AxisControl"/> Control.
        /// </summary>
        /// <seealso cref="twistStartPositionAction"/>
        public InputActionProperty twistDeltaRotationAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The number of concurrent touches on the screen. Must be an <see cref="IntegerControl"/> Control.
        /// </summary>
        public InputActionProperty screenTouchCountAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The camera associated with the screen, and through which screen presses/touches will be interpreted.
        /// </summary>
        public Camera controllerCamera
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Tells the XR Screen Space Controller to ignore interactions when hitting a screen space canvas.
        /// </summary>
        /// <seealso cref="Canvas.renderMode"/>
        public bool blockInteractionsWithScreenSpaceUI
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Enables a rotation threshold that blocks pinch scale gestures when surpassed.
        /// </summary>
        /// <seealso cref="rotationThreshold"/>
        public bool useRotationThreshold
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The threshold at which a gestures will be interpreted only as rotation and not a pinch scale gesture.
        /// </summary>
        /// <seealso cref="useRotationThreshold"/>
        public float rotationThreshold
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// This value is the change in scale based on input from the <see cref="pinchGapDeltaAction"/> pinch gap delta action
        /// with <see cref="Screen.dpi"/> applied as a factor of the value read in. The delta refers to the change from the previous frame.
        /// </summary>
        /// <remarks>
        /// This value may come back as zero if the input action is not assigned or cannot be read.
        /// </remarks>
        public float scaleDelta { get; private set; }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRScreenSpaceControllerDeprecated);
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRScreenSpaceControllerDeprecated);
        }
    }
}
