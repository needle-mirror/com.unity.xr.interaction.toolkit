#if AR_FOUNDATION_5_2_OR_NEWER && (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX)
#define XR_SIMULATION_AVAILABLE
#endif

using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;

#endif

#if !(ENABLE_VR || UNITY_GAMECORE)
// Disable warnings about unused fields. This component is not functional when the simulated devices cannot be created,
// but the class signature and SerializeField fields are kept to avoid losing data.
#pragma warning disable 414 // The field 'field' is assigned but its value is never used
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles mouse and keyboard input from the user and uses it to
    /// drive simulated XR controllers and an XR head mounted display (HMD).
    /// </summary>
    /// <remarks>
    /// This class does not directly manipulate the camera or controllers which are part of
    /// the XR Origin, but rather drives them indirectly through simulated input devices.
    /// <br /><br />
    /// Use the Package Manager window to install the <i>XR Device Simulator</i> sample into
    /// your project to get sample mouse and keyboard bindings for Input System actions that
    /// this component expects. The sample also includes a prefab of a <see cref="GameObject"/>
    /// with this component attached that has references to those sample actions already set.
    /// To make use of this simulator, add the prefab to your scene (the prefab makes use
    /// of <see cref="InputActionManager"/> to ensure the Input System actions are enabled).
    /// <br /><br />
    /// Note that the XR Origin must read the position and rotation of the HMD and controllers
    /// by using Input System actions (such as by using <see cref="ActionBasedController"/>
    /// and <see cref="TrackedPoseDriver"/>) for this simulator to work as expected.
    /// Attempting to use XR input subsystem device methods (such as by using <see cref="XRController"/>
    /// and <see cref="SpatialTracking.TrackedPoseDriver"/>) will not work as expected
    /// since this simulator depends on the Input System to drive the simulated devices.
    /// </remarks>
    /// <seealso cref="XRSimulatedController"/>
    /// <seealso cref="XRSimulatedHMD"/>
    /// <seealso cref="SimulatedInputLayoutLoader"/>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [AddComponentMenu("XR/Debug/XR Device Simulator (Deprecated)", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_DeviceSimulator)]
    [HelpURL(XRHelpURLConstants.k_XRDeviceSimulator)]
    [Obsolete(k_XRDeviceSimulatorDeprecated, true)]
    public partial class XRDeviceSimulator : MonoBehaviour
    {
        const string k_XRDeviceSimulatorDeprecated = "XRDeviceSimulator has been marked for deprecation and will be replaced by the XRInteractionSimulator in future versions.";

        /// <summary>
        /// The coordinate space in which to operate.
        /// </summary>
        /// <seealso cref="keyboardTranslateSpace"/>
        /// <seealso cref="mouseTranslateSpace"/>
        public enum Space
        {
            /// <summary>
            /// Applies translations of a controller or HMD relative to its own coordinate space, considering its own rotations.
            /// Will translate a controller relative to itself, independent of the camera.
            /// </summary>
            Local,

            /// <summary>
            /// Applies translations of a controller or HMD relative to its parent. If the object does not have a parent, meaning
            /// it is a root object, the parent coordinate space is the same as the world coordinate space. This is the same
            /// as <see cref="Local"/> but without considering its own rotations.
            /// </summary>
            Parent,

            /// <summary>
            /// Applies translations of a controller or HMD relative to the screen.
            /// Will translate a controller relative to the camera, independent of the controller's orientation.
            /// </summary>
            Screen,
        }

        /// <summary>
        /// The transformation mode in which to operate.
        /// </summary>
        /// <seealso cref="mouseTransformationMode"/>
        public enum TransformationMode
        {
            /// <summary>
            /// Applies translations from input.
            /// </summary>
            Translate,

            /// <summary>
            /// Applies rotations from input.
            /// </summary>
            Rotate,
        }

        /// <summary>
        /// The target device control(s) to update from input.
        /// </summary>
        /// <remarks>
        /// <see cref="FlagsAttribute"/> to support updating multiple controls from input
        /// (e.g. to drive the primary and secondary 2D axis on a controller from the same input).
        /// </remarks>
        /// <seealso cref="axis2DTargets"/>
        [Flags]
        public enum Axis2DTargets
        {
            /// <summary>
            /// Do not update device state from input.
            /// </summary>
            None = 0,

            /// <summary>
            /// Update device position from input.
            /// </summary>
            Position = 1 << 0,

            /// <summary>
            /// Update the primary touchpad or joystick on a controller device from input.
            /// </summary>
            Primary2DAxis = 1 << 1,

            /// <summary>
            /// Update the secondary touchpad or joystick on a controller device from input.
            /// </summary>
            Secondary2DAxis = 1 << 2,
        }

        /// <summary>
        /// Input Action asset containing controls for the simulator itself. Unity will automatically enable and disable it with this component.
        /// </summary>
        public InputActionAsset deviceSimulatorActionAsset
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Input Action asset containing controls for the simulated controllers. Unity will automatically enable and disable it as needed.
        /// </summary>
        public InputActionAsset controllerActionAsset
        {
            get => default;
            set => _ = value;
        }

#region Device Simulator Actions
        /// <summary>
        /// The Input System Action used to translate in the x-axis (left/right) while held.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="AxisControl"/>.
        /// </summary>
        public InputActionReference keyboardXTranslateAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to translate in the y-axis (up/down) while held.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="AxisControl"/>.
        /// </summary>
        public InputActionReference keyboardYTranslateAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to translate in the z-axis (forward/back) while held.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="AxisControl"/>.
        /// </summary>
        public InputActionReference keyboardZTranslateAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to enable manipulation of the left-hand controller while held.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <remarks>
        /// Note that if controls on the left-hand controller are actuated when this action is released,
        /// those controls will continue to remain actuated. This is to allow for multi-hand interactions
        /// without needing to have dedicated bindings for manipulating each controller separately and concurrently.
        /// </remarks>
        /// <seealso cref="manipulateRightAction"/>
        /// <seealso cref="toggleManipulateLeftAction"/>
        public InputActionReference manipulateLeftAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to enable manipulation of the right-hand controller while held.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <remarks>
        /// Note that if controls on the right-hand controller are actuated when this action is released,
        /// those controls will continue to remain actuated. This is to allow for multi-hand interactions
        /// without needing to have dedicated bindings for manipulating each controller separately and concurrently.
        /// </remarks>
        /// <seealso cref="manipulateLeftAction"/>
        public InputActionReference manipulateRightAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle enable manipulation of the left-hand controller when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="manipulateLeftAction"/>
        /// <seealso cref="toggleManipulateRightAction"/>
        public InputActionReference toggleManipulateLeftAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle enable manipulation of the right-hand controller when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="manipulateRightAction"/>
        /// <seealso cref="toggleManipulateLeftAction"/>
        public InputActionReference toggleManipulateRightAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle enable looking around with the HMD and controllers.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference toggleManipulateBodyAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to enable manipulation of the HMD while held.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference manipulateHeadAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to change between hand and controller mode.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference handControllerModeAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to cycle between the different available devices.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="manipulateHeadAction"/>
        /// <seealso cref="manipulateLeftAction"/>
        /// <seealso cref="manipulateRightAction"/>
        public InputActionReference cycleDevicesAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to stop all manipulation.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference stopManipulationAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to translate or rotate by a scaled amount along or about the x- and y-axes.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/>.
        /// </summary>
        /// <remarks>
        /// Typically bound to the screen-space motion delta of the mouse in pixels.
        /// </remarks>
        /// <seealso cref="mouseScrollAction"/>
        public InputActionReference mouseDeltaAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to translate or rotate by a scaled amount along or about the z-axis.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/>.
        /// </summary>
        /// <remarks>
        /// Typically bound to the horizontal and vertical scroll wheels, though only the vertical is used.
        /// </remarks>
        /// <seealso cref="mouseDeltaAction"/>
        public InputActionReference mouseScrollAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to cause the manipulated device(s) to rotate when moving the mouse when held.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <remarks>
        /// Forces rotation mode when held, no matter what the current mouse transformation mode is.
        /// </remarks>
        /// <seealso cref="negateModeAction"/>
        public InputActionReference rotateModeOverrideAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle between translating or rotating the manipulated device(s)
        /// when moving the mouse when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference toggleMouseTransformationModeAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to cause the manipulated device(s) to rotate when moving the mouse
        /// while held when it would normally translate, and vice-versa.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <remarks>
        /// Can be used to temporarily change the mouse transformation mode to the other mode while held
        /// for making quick adjustments.
        /// </remarks>
        /// <seealso cref="toggleMouseTransformationModeAction"/>
        public InputActionReference negateModeAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to constrain the translation or rotation to the x-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="yConstraintAction"/>
        /// <seealso cref="zConstraintAction"/>
        public InputActionReference xConstraintAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to constrain the translation or rotation to the y-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="xConstraintAction"/>
        /// <seealso cref="zConstraintAction"/>
        public InputActionReference yConstraintAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to constrain the translation or rotation to the z-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="xConstraintAction"/>
        /// <seealso cref="yConstraintAction"/>
        public InputActionReference zConstraintAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to cause the manipulated device(s) to reset position or rotation
        /// (depending on the effective manipulation mode).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <remarks>
        /// Resets position to <see cref="Vector3.zero"/> and rotation to <see cref="Quaternion.identity"/>.
        /// May be combined with axis constraints (<see cref="xConstraintAction"/>, <see cref="yConstraintAction"/>, and <see cref="zConstraintAction"/>).
        /// </remarks>
        public InputActionReference resetAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle the cursor lock mode for the game window when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="Cursor.lockState"/>
        /// <seealso cref="desiredCursorLockMode"/>
        public InputActionReference toggleCursorLockAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to toggle enable translation from keyboard inputs when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="keyboardXTranslateAction"/>
        /// <seealso cref="keyboardYTranslateAction"/>
        /// <seealso cref="keyboardZTranslateAction"/>
        public InputActionReference toggleDevicePositionTargetAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to toggle enable manipulation of the <see cref="Axis2DTargets.Primary2DAxis"/> of the controllers when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="toggleSecondary2DAxisTargetAction"/>
        /// <seealso cref="toggleDevicePositionTargetAction"/>
        /// <seealso cref="axis2DAction"/>
        public InputActionReference togglePrimary2DAxisTargetAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action used to toggle enable manipulation of the <see cref="Axis2DTargets.Secondary2DAxis"/> of the controllers when pressed.
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        /// <seealso cref="togglePrimary2DAxisTargetAction"/>
        /// <seealso cref="toggleDevicePositionTargetAction"/>
        /// <seealso cref="axis2DAction"/>
        public InputActionReference toggleSecondary2DAxisTargetAction
        {
            get => default;
            set => _ = value;
        }

#endregion
#region Controller Actions
        /// <summary>
        /// The Input System Action used to control the value of one or more 2D Axis controls on the manipulated controller device(s).
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="togglePrimary2DAxisTargetAction"/> and <see cref="toggleSecondary2DAxisTargetAction"/> toggle enables
        /// the ability to manipulate 2D Axis controls on the simulated controllers, and this <see cref="axis2DAction"/>
        /// actually controls the value of them while those controller devices are being manipulated.
        /// <br />
        /// Typically bound to WASD on a keyboard, and controls the primary and/or secondary 2D Axis controls on them.
        /// </remarks>
        public InputActionReference axis2DAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control one or more 2D Axis controls on the opposite hand
        /// of the exclusively manipulated controller device.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/>.
        /// </summary>
        /// <remarks>
        /// Typically bound to Q and E on a keyboard for the horizontal component, and controls the opposite hand's
        /// 2D Axis controls when manipulating one (and only one) controller. Can be used to quickly and simultaneously
        /// control the 2D Axis on the other hand's controller. In a typical setup of continuous movement bound on the left-hand
        /// controller stick, and turning bound on the right-hand controller stick, while exclusively manipulating the left-hand
        /// controller to move, this action can be used to trigger turning.
        /// </remarks>
        /// <seealso cref="axis2DAction"/>
        public InputActionReference restingHandAxis2DAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Grip control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference gripAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Trigger control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference triggerAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the PrimaryButton control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference primaryButtonAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the SecondaryButton control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference secondaryButtonAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Menu control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference menuAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Primary2DAxisClick control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference primary2DAxisClickAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Secondary2DAxisClick control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference secondary2DAxisClickAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Primary2DAxisTouch control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference primary2DAxisTouchAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the Secondary2DAxisTouch control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference secondary2DAxisTouchAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the PrimaryTouch control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference primaryTouchAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action used to control the SecondaryTouch control of the manipulated controller device(s).
        /// Must be a <see cref="ButtonControl"/>.
        /// </summary>
        public InputActionReference secondaryTouchAction
        {
            get => default;
            set => _ = value;
        }

#endregion
        /// <summary>
        /// Input Action asset containing controls for the simulated hands. Unity will automatically enable and disable it as needed.
        /// </summary>
        public InputActionAsset handActionAsset
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="Transform"/> that contains the <see cref="Camera"/>. This is usually the "Head" of XR Origins.
        /// Automatically set to <see cref="Camera.main"/> if unset.
        /// </summary>
        public Transform cameraTransform
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The coordinate space in which keyboard translation should operate.
        /// </summary>
        /// <seealso cref="Space"/>
        /// <seealso cref="mouseTranslateSpace"/>
        /// <seealso cref="keyboardXTranslateAction"/>
        /// <seealso cref="keyboardYTranslateAction"/>
        /// <seealso cref="keyboardZTranslateAction"/>
        public Space keyboardTranslateSpace
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The coordinate space in which mouse translation should operate.
        /// </summary>
        /// <seealso cref="Space"/>
        /// <seealso cref="keyboardTranslateSpace"/>
        public Space mouseTranslateSpace
        {
            get => default;
            set => _ = value;
        }

#region Sensitivity
        /// <summary>
        /// Speed of translation in the x-axis (left/right) when triggered by keyboard input.
        /// </summary>
        /// <seealso cref="keyboardXTranslateAction"/>
        /// <seealso cref="keyboardYTranslateSpeed"/>
        /// <seealso cref="keyboardZTranslateSpeed"/>
        public float keyboardXTranslateSpeed
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Speed of translation in the y-axis (up/down) when triggered by keyboard input.
        /// </summary>
        /// <seealso cref="keyboardYTranslateAction"/>
        /// <seealso cref="keyboardXTranslateSpeed"/>
        /// <seealso cref="keyboardZTranslateSpeed"/>
        public float keyboardYTranslateSpeed
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Speed of translation in the z-axis (forward/back) when triggered by keyboard input.
        /// </summary>
        /// <seealso cref="keyboardZTranslateAction"/>
        /// <seealso cref="keyboardXTranslateSpeed"/>
        /// <seealso cref="keyboardYTranslateSpeed"/>
        public float keyboardZTranslateSpeed
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Speed multiplier applied for body translation when triggered by keyboard input.
        /// </summary>
        /// <seealso cref="keyboardXTranslateSpeed"/>
        /// <seealso cref="keyboardYTranslateSpeed"/>
        /// <seealso cref="keyboardZTranslateSpeed"/>
        public float keyboardBodyTranslateMultiplier
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of translation in the x-axis (left/right) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseYTranslateSensitivity"/>
        /// <seealso cref="mouseScrollTranslateSensitivity"/>
        public float mouseXTranslateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of translation in the y-axis (up/down) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseXTranslateSensitivity"/>
        /// <seealso cref="mouseScrollTranslateSensitivity"/>
        public float mouseYTranslateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of translation in the z-axis (forward/back) when triggered by mouse scroll input.
        /// </summary>
        /// <seealso cref="mouseScrollAction"/>
        /// <seealso cref="mouseXTranslateSensitivity"/>
        /// <seealso cref="mouseYTranslateSensitivity"/>
        public float mouseScrollTranslateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of rotation along the x-axis (pitch) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseYRotateSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float mouseXRotateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of rotation along the y-axis (yaw) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseXRotateSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float mouseYRotateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.
        /// </summary>
        /// <seealso cref="mouseScrollAction"/>
        /// <seealso cref="mouseXRotateSensitivity"/>
        /// <seealso cref="mouseYRotateSensitivity"/>
        public float mouseScrollRotateSensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input.
        /// A <see langword="false"/> value (default) means typical FPS style where moving the mouse up/down pitches up/down.
        /// A <see langword="true"/> value means flight control style where moving the mouse up/down pitches down/up.
        /// </summary>
        public bool mouseYRotateInvert
        {
            get => default;
            set => _ = value;
        }

#endregion
        /// <summary>
        /// The desired cursor lock mode to toggle to from <see cref="CursorLockMode.None"/>
        /// (either <see cref="CursorLockMode.Locked"/> or <see cref="CursorLockMode.Confined"/>).
        /// </summary>
        /// <seealso cref="toggleCursorLockAction"/>
        public CursorLockMode desiredCursorLockMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The optional Device Simulator UI prefab to use along with the XR Device Simulator.
        /// </summary>
        public GameObject deviceSimulatorUI
        {
            get => default;
            set => _ = value;
        }

#region Analog Configuration
        /// <summary>
        /// The amount of the simulated grip on the controller when the Grip control is pressed.
        /// </summary>
        /// <seealso cref="gripAction"/>
        public float gripAmount
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The amount of the simulated trigger pull on the controller when the Trigger control is pressed.
        /// </summary>
        /// <seealso cref="triggerAction"/>
        public float triggerAmount
        {
            get => default;
            set => _ = value;
        }

#endregion
#region Tracking State
        /// <summary>
        /// Whether the HMD should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool hmdIsTracked
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Which tracking values the HMD should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState hmdTrackingState
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether the left-hand controller should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool leftControllerIsTracked
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Which tracking values the left-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState leftControllerTrackingState
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether the right-hand controller should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool rightControllerIsTracked
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Which tracking values the right-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState rightControllerTrackingState
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether the left hand should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool leftHandIsTracked
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether the right hand should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool rightHandIsTracked
        {
            get => default;
            set => _ = value;
        }

        #endregion

        /// <summary>
        /// The transformation mode in which the mouse should operate.
        /// </summary>
        /// <seealso cref="TransformationMode"/>
        public TransformationMode mouseTransformationMode { get; set; }

        /// <summary>
        /// Is the user currently using negate mode.
        /// </summary>
        /// <seealso cref="mouseTransformationMode"/>
        public bool negateMode { get; private set; }

        /// <summary>
        /// One or more 2D Axis controls that keyboard input should apply to (or none).
        /// </summary>
        /// <remarks>
        /// Used to control a combination of the position (<see cref="Axis2DTargets.Position"/>),
        /// primary 2D axis (<see cref="Axis2DTargets.Primary2DAxis"/>), or
        /// secondary 2D axis (<see cref="Axis2DTargets.Secondary2DAxis"/>) of manipulated device(s).
        /// </remarks>
        /// <seealso cref="keyboardXTranslateAction"/>
        /// <seealso cref="keyboardYTranslateAction"/>
        /// <seealso cref="keyboardZTranslateAction"/>
        /// <seealso cref="axis2DAction"/>
        /// <seealso cref="restingHandAxis2DAction"/>
        public Axis2DTargets axis2DTargets { get; set; }

        /// <summary>
        /// Whether the simulator is manipulating the Left device (controller or hand).
        /// </summary>
        public bool manipulatingLeftDevice => default;

        /// <summary>
        /// Whether the simulator is manipulating the Right device (controller or hand).
        /// </summary>
        public bool manipulatingRightDevice => default;

        /// <summary>
        /// Whether the simulator is manipulating the Left Controller.
        /// </summary>
        public bool manipulatingLeftController => default;

        /// <summary>
        /// Whether the simulator is manipulating the Right Controller.
        /// </summary>
        public bool manipulatingRightController => default;

        /// <summary>
        /// Whether the simulator is manipulating the Left Hand.
        /// </summary>
        public bool manipulatingLeftHand => default;

        /// <summary>
        /// Whether the simulator is manipulating the Right Hand.
        /// </summary>
        public bool manipulatingRightHand => default;

        /// <summary>
        /// Whether the simulator is manipulating the HMD, Left Controller, and Right Controller as if the whole player was turning their torso,
        /// similar to a typical FPS style.
        /// </summary>
        public bool manipulatingFPS => default;

        /// <summary>
        /// The runtime instance of the XR Device Simulator.
        /// </summary>
        public static XRDeviceSimulator instance { get; private set; }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated device(s)
        /// related to position and rotation.
        /// </summary>
        protected virtual void ProcessPoseInput()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to input controls.
        /// </summary>
        protected virtual void ProcessControlInput()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to 2D Axis input controls.
        /// </summary>
        protected virtual void ProcessAxis2DControlInput()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to button input controls.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

        /// <summary>
        /// Update the state of manipulated controller device related to analog values only.
        /// This is used to adjust the grip and trigger values when the user adjusts the slider
        /// when not manipulating the device.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessAnalogButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }
#endif

        /// <summary>
        /// Gets a <see cref="Vector3"/> that can be multiplied component-wise with another <see cref="Vector3"/>
        /// to reset components of the <see cref="Vector3"/>, based on axis constraint inputs.
        /// </summary>
        /// <returns>Returns a <see cref="Vector3"/> mask used to reset the components of another <see cref="Vector3"/>.</returns>
        /// <seealso cref="resetAction"/>
        /// <seealso cref="xConstraintAction"/>
        /// <seealso cref="yConstraintAction"/>
        /// <seealso cref="zConstraintAction"/>
        protected Vector3 GetResetScale()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }

        /// <summary>
        /// Returns the negated <see cref="TransformationMode"/> of the given <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">The <see cref="TransformationMode"/> to get the negated mode of.</param>
        /// <returns>Returns <see cref="TransformationMode.Translate"/> if given <see cref="TransformationMode.Rotate"/>, and vice versa.</returns>
        public static TransformationMode Negate(TransformationMode mode)
        {
            throw new NotSupportedException(k_XRDeviceSimulatorDeprecated);
        }
    }
}
