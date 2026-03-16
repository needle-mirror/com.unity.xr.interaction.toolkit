#if AR_FOUNDATION_5_2_OR_NEWER && (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX)
#define XR_SIMULATION_AVAILABLE
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif

#if XR_HANDS_1_8_OR_NEWER
using UnityEngine.XR.Hands.Capture.Playback;
#endif


#if XR_SIMULATION_AVAILABLE
using Unity.XR.CoreUtils;
using UnityEngine.XR.Simulation;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles mouse and keyboard input from the user and uses it to
    /// simulate on-device interaction in XR Interaction Toolkit.
    /// </summary>
    /// <remarks>
    /// This class does not directly manipulate the camera or controllers which are part of
    /// the XR Origin, but rather drives them indirectly through simulated input devices.
    /// <br /><br />
    /// Use the Package Manager window to install the <i>XR Interaction Simulator</i> sample into
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
    [AddComponentMenu("XR/Debug/XR Interaction Simulator", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_InteractionSimulator)]
    [HelpURL(XRHelpURLConstants.k_XRInteractionSimulator)]
    public class XRInteractionSimulator : MonoBehaviour
    {
        enum HitType
        {
            None,
            Object,
            WorldUI,
            ScreenUI,
        }

        /// <summary>
        /// Additional multiplier of the normalized scroll input.
        /// </summary>
        /// <remarks>
        /// The serialized mouse scroll sensitivity values were originally created when the input system always reported
        /// the values in a platform specific input range, which on Windows typically meant each notch of the wheel would
        /// report a multiple of 120. Now that the <see cref="m_MouseScrollValue"/> is stored normalized,
        /// this multiplier causes the original intended effective sensitivity to be kept.
        /// </remarks>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        const float k_MouseScrollSensitivity = ScrollUtility.windowsPlatformSpecificDivisor;

        const float k_DeviceLeftRightOffsetAmount = 0.1f;
        const float k_DeviceForwardOffsetAmount = 0.3f;
        const float k_DeviceDownOffsetAmount = 0.045f;
        const float k_DefaultRaycastDistance = 10f;
        const int k_MaxRaycastHits = 10;

        [SerializeField]
        [Tooltip("The Transform that contains the Camera. This is usually the \"Head\" of XR Origins. Automatically set to the first enabled camera tagged MainCamera if unset.")]
        Transform m_CameraTransform;

        /// <summary>
        /// The <see cref="Transform"/> that contains the <see cref="Camera"/>. This is usually the "Head" of XR Origins.
        /// Automatically set to <see cref="Camera.main"/> if unset.
        /// </summary>
        public Transform cameraTransform
        {
            get => m_CameraTransform;
            set => m_CameraTransform = value;
        }

        [SerializeField]
        [Tooltip("The Transform that contains the left controller. Automatically set to the Left Controller reference in the XR Input Modality Manager if unset.")]
        Transform m_LeftControllerTransform;

        /// <summary>
        /// The <see cref="Transform"/> that contains the left controller.
        /// Automatically set to the reference to the left controller in the <see cref="XRInputModalityManager"/> if unset.
        /// If it is unset and there is no modality manager then it will be set to the <see cref="cameraTransform"/>.
        /// </summary>
        public Transform leftControllerTransform
        {
            get => m_LeftControllerTransform;
            set => m_LeftControllerTransform = value;
        }

        [SerializeField]
        [Tooltip("The Transform that contains the right controller. Automatically set to the Right Controller reference in the XR Input Modality Manager if unset.")]
        Transform m_RightControllerTransform;

        /// <summary>
        /// The <see cref="Transform"/> that contains the right controller.
        /// Automatically set to the reference to the right controller in the <see cref="XRInputModalityManager"/> if unset.
        /// If it is unset and there is no modality manager then it will be set to the <see cref="cameraTransform"/>.
        /// </summary>
        public Transform rightControllerTransform
        {
            get => m_RightControllerTransform;
            set => m_RightControllerTransform = value;
        }

        [SerializeField]
        [Tooltip("The corresponding manager for this simulator that handles the lifecycle of the simulated devices.")]
        SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;

        /// <summary>
        /// The corresponding manager for this simulator that handles the lifecycle of the simulated devices.
        /// </summary>
        /// <remarks>
        /// If this value is not set, the simulator will either find a lifecycle manager in the scene or create one.
        /// </remarks>
        public SimulatedDeviceLifecycleManager deviceLifecycleManager
        {
            get => m_DeviceLifecycleManager;
            set => m_DeviceLifecycleManager = value;
        }

#pragma warning disable CS0618
        SimulatedHandExpressionManager m_HandExpressionManager;
#pragma warning restore CS0618

        /// <summary>
        /// The corresponding manager for this simulator that handles the hand expressions.
        /// </summary>
        /// <remarks>
        /// If this value is not set, the simulator will find a hand expression manager in the scene.
        /// </remarks>
        [Obsolete("SimulatedHandExpressionManager has been marked for deprecation and will be replaced with SimulatedHandPlaybackManager in future versions.")]
        public SimulatedHandExpressionManager handExpressionManager
        {
            get => m_HandExpressionManager;
            set => m_HandExpressionManager = value;
        }

        [SerializeField]
        [Tooltip("The corresponding manager for this simulator that handles the simulated hand playback.")]
        SimulatedHandPlaybackManager m_HandPlaybackManager;

        /// <summary>
        /// The corresponding manager for this simulator that handles the simulated hand playback.
        /// </summary>
        /// <remarks>
        /// If this value is not set, the simulator will either find a hand playback manager in the scene or create one.
        /// </remarks>
        public SimulatedHandPlaybackManager handPlaybackManager
        {
            get => m_HandPlaybackManager;
            set => m_HandPlaybackManager = value;
        }

        [SerializeField]
        [Tooltip("The optional Interaction Simulator UI prefab to use along with the XR Interaction Simulator.")]
        GameObject m_InteractionSimulatorUI;

        /// <summary>
        /// The optional Interaction Simulator UI prefab to use along with the XR Interaction Simulator.
        /// </summary>
        public GameObject interactionSimulatorUI
        {
            get => m_InteractionSimulatorUI;
            set => m_InteractionSimulatorUI = value;
        }

        [SerializeField]
        [Tooltip("Whether the HMD should report the pose as fully tracked or unavailable/inferred.")]
        bool m_HMDIsTracked = true;

        /// <summary>
        /// Whether the HMD should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool hmdIsTracked
        {
            get => m_HMDIsTracked;
            set => m_HMDIsTracked = value;
        }

        [SerializeField]
        [Tooltip("Which tracking values the HMD should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
        InputTrackingState m_HMDTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

        /// <summary>
        /// Which tracking values the HMD should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState hmdTrackingState
        {
            get => m_HMDTrackingState;
            set => m_HMDTrackingState = value;
        }

        [SerializeField]
        [Tooltip("Whether the left-hand controller should report the pose as fully tracked or unavailable/inferred.")]
        bool m_LeftControllerIsTracked = true;

        /// <summary>
        /// Whether the left-hand controller should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool leftControllerIsTracked
        {
            get => m_LeftControllerIsTracked;
            set => m_LeftControllerIsTracked = value;
        }

        [SerializeField]
        [Tooltip("Which tracking values the left-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
        InputTrackingState m_LeftControllerTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

        /// <summary>
        /// Which tracking values the left-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState leftControllerTrackingState
        {
            get => m_LeftControllerTrackingState;
            set => m_LeftControllerTrackingState = value;
        }

        [SerializeField]
        [Tooltip("Whether the right-hand controller should report the pose as fully tracked or unavailable/inferred.")]
        bool m_RightControllerIsTracked = true;

        /// <summary>
        /// Whether the right-hand controller should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool rightControllerIsTracked
        {
            get => m_RightControllerIsTracked;
            set => m_RightControllerIsTracked = value;
        }

        [SerializeField]
        [Tooltip("Which tracking values the right-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
        InputTrackingState m_RightControllerTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

        /// <summary>
        /// Which tracking values the right-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.
        /// </summary>
        public InputTrackingState rightControllerTrackingState
        {
            get => m_RightControllerTrackingState;
            set => m_RightControllerTrackingState = value;
        }

        [SerializeField]
        [Tooltip("Whether the left hand should report the pose as fully tracked or unavailable/inferred.")]
        bool m_LeftHandIsTracked = true;

        /// <summary>
        /// Whether the left hand should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool leftHandIsTracked
        {
            get => m_LeftHandIsTracked;
            set => m_LeftHandIsTracked = value;
        }

        [SerializeField]
        [Tooltip("Whether the right hand should report the pose as fully tracked or unavailable/inferred.")]
        bool m_RightHandIsTracked = true;

        /// <summary>
        /// Whether the right hand should report the pose as fully tracked or unavailable/inferred.
        /// </summary>
        public bool rightHandIsTracked
        {
            get => m_RightHandIsTracked;
            set => m_RightHandIsTracked = value;
        }

        [SerializeField]
        [Tooltip("The input used to translate in the x-axis (left/right) while held.")]
        XRInputValueReader<float> m_TranslateXInput = new XRInputValueReader<float>("Translate X Input");

        /// <summary>
        /// The input used to translate in the x-axis (left/right) while held.
        /// </summary>
        public XRInputValueReader<float> translateXInput
        {
            get => m_TranslateXInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TranslateXInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to translate in the y-axis (up/down) while held.")]
        XRInputValueReader<float> m_TranslateYInput = new XRInputValueReader<float>("Translate Y Input");

        /// <summary>
        /// The input used to translate in the y-axis (up/down) while held.
        /// </summary>
        public XRInputValueReader<float> translateYInput
        {
            get => m_TranslateYInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TranslateYInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to translate in the z-axis (forward/back) while held.")]
        XRInputValueReader<float> m_TranslateZInput = new XRInputValueReader<float>("Translate Z Input");

        /// <summary>
        /// The input used to translate in the z-axis (forward/back) while held.
        /// </summary>
        public XRInputValueReader<float> translateZInput
        {
            get => m_TranslateZInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TranslateZInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle enable manipulation of the left-hand controller when pressed.")]
        XRInputButtonReader m_ToggleManipulateLeftInput;

        /// <summary>
        /// The input used to toggle enable manipulation of the left-hand controller when pressed.
        /// </summary>
        /// <seealso cref="toggleManipulateRightInput"/>
        public XRInputButtonReader toggleManipulateLeftInput
        {
            get => m_ToggleManipulateLeftInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ToggleManipulateLeftInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle enable manipulation of the right-hand controller when pressed")]
        XRInputButtonReader m_ToggleManipulateRightInput;

        /// <summary>
        /// The input used to toggle enable manipulation of the right-hand controller when pressed.
        /// </summary>
        /// <seealso cref="toggleManipulateLeftInput"/>
        public XRInputButtonReader toggleManipulateRightInput
        {
            get => m_ToggleManipulateRightInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ToggleManipulateRightInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used for controlling the left-hand device's actions for buttons or hand expressions.")]
        XRInputButtonReader m_LeftDeviceActionsInput;

        /// <summary>
        /// The input used for controlling the left-hand device's actions for buttons or hand expressions.
        /// </summary>
        public XRInputButtonReader leftDeviceActionsInput
        {
            get => m_LeftDeviceActionsInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_LeftDeviceActionsInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to cycle between the different available devices.")]
        XRInputButtonReader m_CycleDevicesInput;

        /// <summary>
        /// The input used to cycle between the different available devices.
        /// </summary>
        public XRInputButtonReader cycleDevicesInput
        {
            get => m_CycleDevicesInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_CycleDevicesInput, value, this);
        }

        [SerializeField]
        [Tooltip("The keyboard input used to rotate by a scaled amount along or about the x- and y-axes.")]
        XRInputValueReader<Vector2> m_KeyboardRotationDeltaInput = new XRInputValueReader<Vector2>("Keyboard Rotation Delta Input");

        /// <summary>
        /// The keyboard input used to rotate by a scaled amount along or about the x- and y-axes.
        /// </summary>
        public XRInputValueReader<Vector2> keyboardRotationDeltaInput
        {
            get => m_KeyboardRotationDeltaInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_KeyboardRotationDeltaInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle associated inputs from a mouse device.")]
        XRInputButtonReader m_ToggleMouseInput;

        /// <summary>
        /// The input used to toggle associated inputs from a mouse device.
        /// </summary>
        /// <seealso cref="mouseRotationDeltaInput"/>
        /// <seealso cref="mouseScrollInput"/>
        public XRInputButtonReader toggleMouseInput
        {
            get => m_ToggleMouseInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ToggleMouseInput, value, this);
        }

        [SerializeField]
        [Tooltip("The mouse input used to rotate by a scaled amount along or about the x- and y-axes.")]
        XRInputValueReader<Vector2> m_MouseRotationDeltaInput = new XRInputValueReader<Vector2>("Mouse Rotation Delta Input");

        /// <summary>
        /// The mouse input used to rotate by a scaled amount along or about the x- and y-axes.
        /// </summary>
        /// <remarks>
        /// Typically bound to the screen-space motion delta of the mouse in pixels.
        /// </remarks>
        /// <seealso cref="keyboardRotationDeltaInput"/>
        public XRInputValueReader<Vector2> mouseRotationDeltaInput
        {
            get => m_MouseRotationDeltaInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MouseRotationDeltaInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to translate or rotate by a scaled amount along or about the z-axis.")]
        XRInputValueReader<Vector2> m_MouseScrollInput;

        /// <summary>
        /// The input used to translate or rotate by a scaled amount along or about the z-axis.
        /// </summary>
        /// <remarks>
        /// Typically bound to the horizontal and vertical scroll wheels, though only the vertical is used.
        /// </remarks>
        public XRInputValueReader<Vector2> mouseScrollInput
        {
            get => m_MouseScrollInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MouseScrollInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Grip control of the manipulated controller device(s).")]
        XRInputButtonReader m_GripInput;

        /// <summary>
        /// The input used to control the Grip control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader gripInput
        {
            get => m_GripInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_GripInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Trigger control of the manipulated controller device(s).")]
        XRInputButtonReader m_TriggerInput;

        /// <summary>
        /// The input used to control the Trigger control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader triggerInput
        {
            get => m_TriggerInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TriggerInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the PrimaryButton control of the manipulated controller device(s).")]
        XRInputButtonReader m_PrimaryButtonInput;

        /// <summary>
        /// The input used to control the PrimaryButton control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader primaryButtonInput
        {
            get => m_PrimaryButtonInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_PrimaryButtonInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the SecondaryButton control of the manipulated controller device(s).")]
        XRInputButtonReader m_SecondaryButtonInput;

        /// <summary>
        /// The input used to control the SecondaryButton control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader secondaryButtonInput
        {
            get => m_SecondaryButtonInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SecondaryButtonInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Menu control of the manipulated controller device(s).")]
        XRInputButtonReader m_MenuInput;

        /// <summary>
        /// The input used to control the Menu control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader menuInput
        {
            get => m_MenuInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MenuInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Primary2DAxisClick control of the manipulated controller device(s).")]
        XRInputButtonReader m_Primary2DAxisClickInput;

        /// <summary>
        /// The input used to control the Primary2DAxisClick control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader primary2DAxisClickInput
        {
            get => m_Primary2DAxisClickInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_Primary2DAxisClickInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Secondary2DAxisClick control of the manipulated controller device(s).")]
        XRInputButtonReader m_Secondary2DAxisClickInput;

        /// <summary>
        /// The input used to control the Secondary2DAxisClick control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader secondary2DAxisClickInput
        {
            get => m_Secondary2DAxisClickInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_Secondary2DAxisClickInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Primary2DAxisTouch control of the manipulated controller device(s).")]
        XRInputButtonReader m_Primary2DAxisTouchInput;

        /// <summary>
        /// The input used to control the Primary2DAxisTouch control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader primary2DAxisTouchInput
        {
            get => m_Primary2DAxisTouchInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_Primary2DAxisTouchInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the Secondary2DAxisTouch control of the manipulated controller device(s).")]
        XRInputButtonReader m_Secondary2DAxisTouchInput;

        /// <summary>
        /// The input used to control the Secondary2DAxisTouch control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader secondary2DAxisTouchInput
        {
            get => m_Secondary2DAxisTouchInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_Secondary2DAxisTouchInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the PrimaryTouch control of the manipulated controller device(s).")]
        XRInputButtonReader m_PrimaryTouchInput;

        /// <summary>
        /// The input used to control the PrimaryTouch control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader primaryTouchInput
        {
            get => m_PrimaryTouchInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_PrimaryTouchInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the SecondaryTouch control of the manipulated controller device(s).")]
        XRInputButtonReader m_SecondaryTouchInput;

        /// <summary>
        /// The input used to control the SecondaryTouch control of the manipulated controller device(s).
        /// </summary>
        public XRInputButtonReader secondaryTouchInput
        {
            get => m_SecondaryTouchInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SecondaryTouchInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to constrain the translation or rotation to the x-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
        XRInputButtonReader m_XConstraintInput;

        /// <summary>
        /// The input used to constrain the translation or rotation to the x-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// </summary>
        /// <seealso cref="yConstraintInput"/>
        /// <seealso cref="zConstraintInput"/>
        public XRInputButtonReader xConstraintInput
        {
            get => m_XConstraintInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_XConstraintInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to constrain the translation or rotation to the y-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
        XRInputButtonReader m_YConstraintInput;

        /// <summary>
        /// The input used to constrain the translation or rotation to the y-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// </summary>
        /// <seealso cref="xConstraintInput"/>
        /// <seealso cref="zConstraintInput"/>
        public XRInputButtonReader yConstraintInput
        {
            get => m_YConstraintInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_YConstraintInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to constrain the translation or rotation to the z-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
        XRInputButtonReader m_ZConstraintInput;

        /// <summary>
        /// The input used to constrain the translation or rotation to the z-axis when moving the mouse or resetting.
        /// May be combined with another axis constraint to constrain to a plane.
        /// </summary>
        /// <seealso cref="xConstraintInput"/>
        /// <seealso cref="yConstraintInput"/>
        public XRInputButtonReader zConstraintInput
        {
            get => m_ZConstraintInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ZConstraintInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to cause the manipulated device(s) to reset position or rotation (depending on the effective manipulation mode).")]
        XRInputButtonReader m_ResetInput;

        /// <summary>
        /// The input used to cause the manipulated device(s) to reset position or rotation
        /// </summary>
        /// <remarks>
        /// Resets position to <see cref="Vector3.zero"/> and rotation to <see cref="Quaternion.identity"/>.
        /// May be combined with axis constraints (<see cref="xConstraintInput"/>, <see cref="yConstraintInput"/>, and <see cref="zConstraintInput"/>).
        /// </remarks>
        public XRInputButtonReader resetInput
        {
            get => m_ResetInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ResetInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to control the value of one or more 2D Axis controls on the manipulated controller device(s).")]
        XRInputValueReader<Vector2> m_Axis2DInput;

        /// <summary>
        /// The input used to control the value of one or more 2D Axis controls on the manipulated controller device(s).
        /// </summary>
        /// <remarks>
        /// Typically bound to IJKL on a keyboard, and controls the primary and/or secondary 2D Axis controls on them.
        /// </remarks>
        public XRInputValueReader<Vector2> axis2DInput
        {
            get => m_Axis2DInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_Axis2DInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle enable manipulation of the Primary2DAxis of the controllers when pressed.")]
        XRInputButtonReader m_TogglePrimary2DAxisTargetInput;

        /// <summary>
        /// The input used to toggle enable manipulation of the <see cref="Axis2DTargets.Primary2DAxis"/> of the controllers when pressed.
        /// </summary>
        /// <seealso cref="toggleSecondary2DAxisTargetInput"/>
        /// <seealso cref="axis2DInput"/>
        public XRInputButtonReader togglePrimary2DAxisTargetInput
        {
            get => m_TogglePrimary2DAxisTargetInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TogglePrimary2DAxisTargetInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle enable manipulation of the Secondary2DAxis of the controllers when pressed.")]
        XRInputButtonReader m_ToggleSecondary2DAxisTargetInput;

        /// <summary>
        /// The input used to toggle enable manipulation of the <see cref="Axis2DTargets.Secondary2DAxis"/> of the controllers when pressed.
        /// </summary>
        /// <seealso cref="togglePrimary2DAxisTargetInput"/>
        public XRInputButtonReader toggleSecondary2DAxisTargetInput
        {
            get => m_ToggleSecondary2DAxisTargetInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ToggleSecondary2DAxisTargetInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to cycle the quick-action for controller inputs or hand expressions.")]
        XRInputButtonReader m_CycleQuickActionInput;

        /// <summary>
        /// The input used to cycle the quick-action for controller inputs or hand expressions.
        /// </summary>
        /// <seealso cref="togglePerformQuickActionInput"/>
        public XRInputButtonReader cycleQuickActionInput
        {
            get => m_CycleQuickActionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_CycleQuickActionInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to perform the currently active quick-action controller input or hand expression.")]
        XRInputButtonReader m_TogglePerformQuickActionInput;

        /// <summary>
        /// The input used to perform the currently active quick-action controller input or hand expression.
        /// </summary>
        /// <seealso cref="cycleQuickActionInput"/>
        public XRInputButtonReader togglePerformQuickActionInput
        {
            get => m_TogglePerformQuickActionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TogglePerformQuickActionInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle manipulation of only the head pose.")]
        XRInputButtonReader m_ToggleManipulateHeadInput;

        /// <summary>
        /// The input used to toggle manipulation of only the head pose.
        /// </summary>
        public XRInputButtonReader toggleManipulateHeadInput
        {
            get => m_ToggleManipulateHeadInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ToggleManipulateHeadInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to toggle point-and-click select. This input is interpreted as a trigger press when over UI objects, and otherwise a grip press.")]
        XRInputButtonReader m_MouseClickInput;

        /// <summary>
        /// The input used to toggle point-and-click select.
        /// </summary>
        /// <remarks>
        /// This input is interpreted as a trigger press when over UI objects, otherwise it is
        /// interpreted as a grip press.
        /// </remarks>
        public XRInputButtonReader mouseClickInput
        {
            get => m_MouseClickInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MouseClickInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input that represents the current screen-space position of the mouse in pixels.")]
        XRInputValueReader<Vector2> m_MousePointInput;

        /// <summary>
        /// The input that represents the current screen-space position of the mouse in pixels.
        /// </summary>
        public XRInputValueReader<Vector2> mousePointInput
        {
            get => m_MousePointInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MousePointInput, value, this);
        }

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of the simulated grip on the controller when the Grip control is pressed.")]
        float m_GripAmount = 1f;

        /// <summary>
        /// The amount of the simulated grip on the controller when the Grip control is pressed.
        /// </summary>
        /// <seealso cref="gripInput"/>
        public float gripAmount
        {
            get => m_GripAmount;
            set => m_GripAmount = value;
        }

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of the simulated trigger pull on the controller when the Trigger control is pressed.")]
        float m_TriggerAmount = 1f;

        /// <summary>
        /// The amount of the simulated trigger pull on the controller when the Trigger control is pressed.
        /// </summary>
        /// <seealso cref="triggerInput"/>
        public float triggerAmount
        {
            get => m_TriggerAmount;
            set => m_TriggerAmount = value;
        }

        [SerializeField]
        [Tooltip("Speed of translation in the x-axis (left/right) when triggered by input.")]
        float m_TranslateXSpeed = 0.2f;

        /// <summary>
        /// Speed of translation in the x-axis (left/right) when triggered by input.
        /// </summary>
        /// <seealso cref="translateXInput"/>
        /// <seealso cref="translateYSpeed"/>
        /// <seealso cref="translateZSpeed"/>
        public float translateXSpeed
        {
            get => m_TranslateXSpeed;
            set => m_TranslateXSpeed = value;
        }

        [SerializeField]
        [Tooltip("Speed of translation in the y-axis (up/down) when triggered by input.")]
        float m_TranslateYSpeed = 0.2f;

        /// <summary>
        /// Speed of translation in the y-axis (up/down) when triggered by input.
        /// </summary>
        /// <seealso cref="translateYInput"/>
        /// <seealso cref="translateXSpeed"/>
        /// <seealso cref="translateZSpeed"/>
        public float translateYSpeed
        {
            get => m_TranslateYSpeed;
            set => m_TranslateYSpeed = value;
        }

        [SerializeField]
        [Tooltip("Speed of translation in the z-axis (forward/back) when triggered by input.")]
        float m_TranslateZSpeed = 0.2f;

        /// <summary>
        /// Speed of translation in the z-axis (forward/back) when triggered by input.
        /// </summary>
        /// <seealso cref="translateZInput"/>
        /// <seealso cref="translateXSpeed"/>
        /// <seealso cref="translateYSpeed"/>
        public float translateZSpeed
        {
            get => m_TranslateZSpeed;
            set => m_TranslateZSpeed = value;
        }

        [SerializeField]
        [Tooltip("Speed multiplier applied for body translation when triggered by input.")]
        float m_BodyTranslateMultiplier = 5f;

        /// <summary>
        /// Speed multiplier applied for body translation when triggered by input.
        /// </summary>
        /// <seealso cref="translateXSpeed"/>
        /// <seealso cref="translateYSpeed"/>
        /// <seealso cref="translateZSpeed"/>
        public float bodyTranslateMultiplier
        {
            get => m_BodyTranslateMultiplier;
            set => m_BodyTranslateMultiplier = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of rotation along the x-axis (pitch) when triggered by input.")]
        float m_RotateXSensitivity = 0.2f;

        /// <summary>
        /// Sensitivity of rotation along the x-axis (pitch) when triggered by input.
        /// </summary>
        /// <seealso cref="mouseRotationDeltaInput"/>
        /// <seealso cref="keyboardRotationDeltaInput"/>
        /// <seealso cref="rotateYSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float rotateXSensitivity
        {
            get => m_RotateXSensitivity;
            set => m_RotateXSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of rotation along the y-axis (yaw) when triggered by input.")]
        float m_RotateYSensitivity = 0.2f;

        /// <summary>
        /// Sensitivity of rotation along the y-axis (yaw) when triggered by input.
        /// </summary>
        /// <seealso cref="mouseRotationDeltaInput"/>
        /// <seealso cref="keyboardRotationDeltaInput"/>
        /// <seealso cref="rotateXSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float rotateYSensitivity
        {
            get => m_RotateYSensitivity;
            set => m_RotateYSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.")]
        float m_MouseScrollRotateSensitivity = 0.05f;

        /// <summary>
        /// Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.
        /// </summary>
        /// <seealso cref="mouseScrollInput"/>
        /// <seealso cref="rotateXSensitivity"/>
        /// <seealso cref="rotateYSensitivity"/>
        public float mouseScrollRotateSensitivity
        {
            get => m_MouseScrollRotateSensitivity;
            set => m_MouseScrollRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("A boolean value of whether to invert the y-axis when rotating." +
            "\nA false value (default) means typical FPS style where moving up/down pitches up/down." +
            "\nA true value means flight control style where moving up/down pitches down/up.")]
        bool m_RotateYInvert;

        /// <summary>
        /// A boolean value of whether to invert the y-axis of mouse input when rotating.
        /// A <see langword="false"/> value (default) means typical FPS style where moving up/down pitches up/down.
        /// A <see langword="true"/> value means flight control style where moving up/down pitches down/up.
        /// </summary>
        public bool rotateYInvert
        {
            get => m_RotateYInvert;
            set => m_RotateYInvert = value;
        }

        [SerializeField]
        [Tooltip("The coordinate space in which translation should operate.")]
        Space m_TranslateSpace = Space.Screen;

        /// <summary>
        /// The coordinate space in which translation should operate.
        /// </summary>
        public Space translateSpace
        {
            get => m_TranslateSpace;
            set => m_TranslateSpace = value;
        }

        [SerializeField]
        [Tooltip("The subset of quick-action controller buttons/inputs that a user can shift through in the simulator.")]
        List<ControllerInputMode> m_QuickActionControllerInputModes = new List<ControllerInputMode>();

        /// <summary>
        /// The subset of quick-action controller buttons/inputs that a user can shift through in the simulator.
        /// </summary>
        /// <seealso cref="cycleQuickActionInput"/>
        /// <seealso cref="togglePerformQuickActionInput"/>
        public List<ControllerInputMode> quickActionControllerInputModes
        {
            get => m_QuickActionControllerInputModes;
            set => m_QuickActionControllerInputModes = value;
        }

        [SerializeField]
        [Tooltip("Enables point-and-click mode which is active by default, otherwise falls back to mouse input for controller rotation.")]
        bool m_UsePointAndClick = true;

        /// <summary>
        /// Enables point-and-click mode which is active by default, otherwise falls back to mouse input for controller rotation.
        /// </summary>
        public bool usePointAndClick
        {
            get => m_UsePointAndClick;
            set => m_UsePointAndClick = value;
        }

#if XR_HANDS_1_1_OR_NEWER
        [SerializeField]
        [Tooltip("The default handedness when starting the simulator and using point-and-click in FPS mode. Note: Changing this property at runtime will not change the currently active handedness.")]
        Handedness m_DefaultPointAndClickHandedness = Handedness.Right;

        /// <summary>
        /// The default handedness when starting the simulator and using point-and-click in FPS mode.
        /// Note: Changing this property at runtime will not change the currently active handedness.
        /// </summary>
        public Handedness defaultPointAndClickHandedness
        {
            get => m_DefaultPointAndClickHandedness;
            set => m_DefaultPointAndClickHandedness = value;
        }
#endif

        TargetedDevices m_TargetedDeviceInput;

        /// <summary>
        /// The currently active/targeted devices in the interaction simulator.
        /// </summary>
        public TargetedDevices targetedDeviceInput
        {
            get => m_TargetedDeviceInput;
            set => m_TargetedDeviceInput = value;
        }

        /// <summary>
        /// The controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode controllerInputMode => manipulatingLeftController ? m_LeftControllerInputMode : m_RightControllerInputMode;

        ControllerInputMode m_LeftControllerInputMode = ControllerInputMode.Trigger;

        /// <summary>
        /// The left controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode leftControllerInputMode => m_LeftControllerInputMode;

        ControllerInputMode m_RightControllerInputMode = ControllerInputMode.Trigger;

        /// <summary>
        /// The right controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode rightControllerInputMode => m_RightControllerInputMode;

        /// <summary>
        /// The hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression currentHandExpression => manipulatingLeftDevice ? m_LeftCurrentHandExpression : m_RightCurrentHandExpression;

        SimulatedHandExpression m_LeftCurrentHandExpression = new SimulatedHandExpression();

        /// <summary>
        /// The left hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression leftCurrentHandExpression => m_LeftCurrentHandExpression;

        SimulatedHandExpression m_RightCurrentHandExpression = new SimulatedHandExpression();

        /// <summary>
        /// The right hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression rightCurrentHandExpression => m_RightCurrentHandExpression;

        bool m_PointAndClickActive;

        /// <summary>
        /// Whether or not point-and-click mode is currently active in the simulator.
        /// </summary>
        /// <remarks>
        /// This property only refers to whether or not the currently manipulated device supports point-and-click.
        /// For a way to turn off the point-and-click feature completely, set <see cref="usePointAndClick"/> to false.
        /// </remarks>
        public bool pointAndClickActive => m_PointAndClickActive;

        /// <summary>
        /// One or more 2D Axis controls that keyboard input should apply to (or none).
        /// </summary>
        /// <remarks>
        /// Used to control a combination of the position (<see cref="Axis2DTargets.Position"/>),
        /// primary 2D axis (<see cref="Axis2DTargets.Primary2DAxis"/>), or
        /// secondary 2D axis (<see cref="Axis2DTargets.Secondary2DAxis"/>) of manipulated device(s).
        /// </remarks>
        /// <seealso cref="translateXInput"/>
        /// <seealso cref="translateYInput"/>
        /// <seealso cref="translateZInput"/>
        /// <seealso cref="axis2DInput"/>
        public Axis2DTargets axis2DTargets { get; set; } = Axis2DTargets.Primary2DAxis;

        /// <summary>
        /// Whether the simulator is manipulating the Left device (controller or hand).
        /// </summary>
        public bool manipulatingLeftDevice => m_TargetedDeviceInput.HasDevice(TargetedDevices.LeftDevice);

        /// <summary>
        /// Whether the simulator is manipulating the Right device (controller or hand).
        /// </summary>
        public bool manipulatingRightDevice => m_TargetedDeviceInput.HasDevice(TargetedDevices.RightDevice);

        /// <summary>
        /// Whether the simulator is manipulating the Left Controller.
        /// </summary>
        public bool manipulatingLeftController => m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && manipulatingLeftDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Right Controller.
        /// </summary>
        public bool manipulatingRightController => m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && manipulatingRightDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Left Hand.
        /// </summary>
        public bool manipulatingLeftHand => m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && manipulatingLeftDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Right Hand.
        /// </summary>
        public bool manipulatingRightHand => m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && manipulatingRightDevice;

        /// <summary>
        /// Whether the simulator is manipulating the HMD.
        /// </summary>
        public bool manipulatingHMD => m_TargetedDeviceInput == TargetedDevices.HMD;

        /// <summary>
        /// Whether the simulator is manipulating the HMD, Left Controller, and Right Controller as if the whole player was turning their torso,
        /// similar to a typical FPS style.
        /// </summary>
        public bool manipulatingFPS => m_TargetedDeviceInput.HasDevice(TargetedDevices.FPS);

        /// <summary>
        /// The runtime instance of the XR Interaction Simulator.
        /// </summary>
        public static XRInteractionSimulator instance { get; private set; }

        /// <summary>
        /// Calls the methods in its invocation list when the singleton component instance is set during Awake
        /// or when the instance is destroyed during OnDestroy. The event argument is <see langword="true"/> when the instance is set,
        /// and <see langword="false"/> when the instance is destroyed.
        /// </summary>
        /// <remarks>
        /// Intended to be used by analytics.
        /// </remarks>
        /// <seealso cref="instance"/>
        internal static Action<bool> instanceChanged;

        (Transform transform, Camera camera) m_CachedCamera;

        /// <summary>
        /// Current value of the x-axis when using translate.
        /// </summary>
        float m_TranslateXValue;

        /// <summary>
        /// Current value of the y-axis when using translate.
        /// </summary>
        float m_TranslateYValue;

        /// <summary>
        /// Current value of the z-axis when using translate.
        /// </summary>
        float m_TranslateZValue;

        Vector2 m_RotationDeltaValue;
        Vector2 m_MouseScrollValue;

        bool m_XConstraintValue;
        bool m_YConstraintValue;
        bool m_ZConstraintValue;
        bool m_ResetValue;

        Vector2 m_Axis2DValue;

        int m_LeftControllerInputModeIndex;
        int m_RightControllerInputModeIndex;
        int m_LeftHandExpressionIndex;
        int m_RightHandExpressionIndex;

        bool m_ToggleManipulateWaitingForReleaseBoth;
#if XR_HANDS_1_8_OR_NEWER
        bool m_LeftHandHotkeyToggled;
        bool m_RightHandHotkeyToggled;
#endif

        Vector3 m_LeftControllerEuler;
        Vector3 m_RightControllerEuler;
        Vector3 m_CenterEyeEuler;

        Vector3 m_ScreenToWorldPoint;
        Vector3 m_RaycastDirVector;
        Vector3 m_HitEndPoint;

#if ENABLE_VR || UNITY_GAMECORE
        XRSimulatedHMDState m_HMDState;
        XRSimulatedControllerState m_LeftControllerState;
        XRSimulatedControllerState m_RightControllerState;
#endif

        XRSimulatedHandState m_LeftHandState;
        XRSimulatedHandState m_RightHandState;

        TargetedDevices m_PreviousTargetedDevices;

        PhysicsScene m_LocalPhysicsScene;
        readonly RaycastHit[] m_RaycastHits = new RaycastHit[k_MaxRaycastHits];
        XRUIInputModule m_UIInputModule;
        PointerEventData m_UIPointerEventData;
        bool m_PerformingLeftPointAndClickGripInteraction;
        bool m_PerformingRightPointAndClickGripInteraction;
        bool m_PerformingLeftPointAndClickTriggerInteraction;
        bool m_PerformingRightPointAndClickTriggerInteraction;
        bool m_OriginalInputModuleMouseValue;
        bool m_OriginalInputModuleTouchValue;
        bool m_OriginalInputModuleValuesCaptured;
        bool m_CanUsePointAndClick;
        float m_PreviousRaycastHitDistance;

#if XR_SIMULATION_AVAILABLE
        XROrigin m_XROrigin;
        SimulationCameraPoseProvider m_SimulationCameraPoseProvider;
        Vector3 m_OriginalCameraOffsetObjectPosition;
        float m_OriginalCameraYOffset;
#endif

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this;
                instanceChanged?.Invoke(true);
            }
            else if (instance != this)
            {
                Debug.LogWarning($"Another instance of XR Interaction Simulator already exists ({instance}), destroying {gameObject}.", this);
                Destroy(gameObject);
                return;
            }

            if (m_DeviceLifecycleManager == null)
                m_DeviceLifecycleManager = XRSimulatorUtility.FindCreateSimulatedDeviceLifecycleManager(gameObject);

#pragma warning disable CS0618 // Type or member is obsolete
            if (ComponentLocatorUtility<SimulatedHandExpressionManager>.TryFindComponent(out m_HandExpressionManager) &&
                !ComponentLocatorUtility<SimulatedHandPlaybackManager>.TryFindComponent(out m_HandPlaybackManager))
            {
                Debug.LogWarning("There seems to be a SimulatedHandExpressionManager in the scene but no SimulatedHandPlaybackManager." +
                    "Ensure that the XR Interaction Simulator sample package is up to date to support hand playback.");
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if (m_HandPlaybackManager == null)
                m_HandPlaybackManager = XRSimulatorUtility.FindCreateSimulatedHandPlaybackManager(gameObject);

            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();

#if ENABLE_VR || UNITY_GAMECORE
            m_HMDState.Reset();
            m_LeftControllerState.Reset();
            m_RightControllerState.Reset();
            m_LeftHandState.Reset();
            m_RightHandState.Reset();

            // Adding offset to the controller/hand when starting simulation to move them away from the Camera position
            m_LeftControllerState.devicePosition = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
            m_RightControllerState.devicePosition = XRSimulatorUtility.rightDeviceDefaultInitialPosition;
            m_LeftHandState.position = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
            m_RightHandState.position = XRSimulatorUtility.rightDeviceDefaultInitialPosition;

#if XR_HANDS_1_1_OR_NEWER
            if(m_DefaultPointAndClickHandedness == Handedness.Left)
                m_TargetedDeviceInput = TargetedDevices.FPS | TargetedDevices.LeftDevice;
            else
                m_TargetedDeviceInput = TargetedDevices.FPS | TargetedDevices.RightDevice;
#else
            m_TargetedDeviceInput = TargetedDevices.FPS | TargetedDevices.RightDevice;
#endif

            if (m_InteractionSimulatorUI != null)
                Instantiate(m_InteractionSimulatorUI, transform);
#else
            Debug.LogWarning("XR Interaction Simulator is not functional on platforms where ENABLE_VR is not defined.", this);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            XRSimulatorUtility.FindCameraTransform(ref m_CachedCamera, ref m_CameraTransform);

            if (m_DeviceLifecycleManager != null)
                m_DeviceLifecycleManager.deviceModeChanged += OnDeviceModeChanged;

            if (!FindLeftRightControllers())
            {
                Debug.LogWarning("XR Interaction Simulator could not detect controllers. Point-and-click functionality will be disabled.", this);
                m_CanUsePointAndClick = false;
            }
            else
                m_CanUsePointAndClick = true;

#if ENABLE_VR || UNITY_GAMECORE
#if XR_SIMULATION_AVAILABLE && XR_MANAGEMENT_4_0_OR_NEWER
            if (XRSimulatorUtility.XRSimulationLoaderEnabledForEditorPlayMode())
            {
                if (m_XROrigin != null || ComponentLocatorUtility<XROrigin>.TryFindComponent(out m_XROrigin))
                {
                    if (m_XROrigin.CameraYOffset != 0)
                    {
                        var offset = new Vector3(0f, m_XROrigin.CameraYOffset, 0f);
                        m_HMDState.centerEyePosition += offset;
                        m_LeftControllerState.devicePosition += offset;
                        m_RightControllerState.devicePosition += offset;

                        m_LeftHandState.position += offset;
                        m_RightHandState.position += offset;

                        m_OriginalCameraYOffset = m_XROrigin.CameraYOffset;
                        m_XROrigin.CameraYOffset = 0f;
                    }

                    if (m_XROrigin.CameraFloorOffsetObject != null && m_XROrigin.CameraFloorOffsetObject.transform.position != Vector3.zero)
                    {
                        m_OriginalCameraOffsetObjectPosition = m_XROrigin.CameraFloorOffsetObject.transform.position;
                        m_XROrigin.CameraFloorOffsetObject.transform.position = Vector3.zero;
                    }

                    Debug.LogWarning("Override XR Simulation Input is enabled and either the XR Origin's Camera Y Offset or the XR Origin's" +
                        " Camera Floor Offset Object's position is set to a non-zero value. Due to the way XR Simulation applies its transformations," +
                        " the offsets will be set to zero and the Camera Y Offset will be applied directly to the simulated camera and devices during Play mode.", this);
                }

                if (m_SimulationCameraPoseProvider != null || ComponentLocatorUtility<SimulationCameraPoseProvider>.TryFindComponent(out m_SimulationCameraPoseProvider))
                    m_SimulationCameraPoseProvider.enabled = false;
            }
#endif
#endif
            InitializeControllerHandActions();

            if (m_HandPlaybackManager.simulatedHandExpressions.Count > 0)
                CycleQuickActionHandExpression();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (m_DeviceLifecycleManager != null)
                m_DeviceLifecycleManager.deviceModeChanged -= OnDeviceModeChanged;

#if ENABLE_VR || UNITY_GAMECORE
#if XR_SIMULATION_AVAILABLE
            if (m_SimulationCameraPoseProvider != null)
                m_SimulationCameraPoseProvider.enabled = true;

            if (m_XROrigin != null)
            {
                if (m_OriginalCameraYOffset != 0f)
                {
                    var offset = new Vector3(0f, m_OriginalCameraYOffset, 0f);
                    m_HMDState.centerEyePosition -= offset;
                    m_LeftControllerState.devicePosition -= offset;
                    m_RightControllerState.devicePosition -= offset;

                    m_LeftHandState.position -= offset;
                    m_RightHandState.position -= offset;
                }

                if (m_XROrigin.CameraFloorOffsetObject != null)
                    m_XROrigin.CameraFloorOffsetObject.transform.position = m_OriginalCameraOffsetObjectPosition;

                m_XROrigin.CameraYOffset = m_OriginalCameraYOffset;
            }
#endif
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
                instanceChanged?.Invoke(false);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            ReadInputValues();

            HandleLeftOrRightDeviceToggle();

            if (m_CycleDevicesInput.ReadWasPerformedThisFrame())
                CycleTargetDevices();

            if (m_CycleQuickActionInput.ReadWasPerformedThisFrame() && !manipulatingFPS && !manipulatingHMD)
                CycleQuickAction();

            if (m_ToggleManipulateHeadInput.ReadWasPerformedThisFrame())
                HandleHMDToggle();

            if (m_TogglePerformQuickActionInput.ReadWasPerformedThisFrame())
                PerformQuickAction();

            HandlePointAndClickActive();

            ProcessPoseInput();
            ProcessPointAndClick();
            ProcessControlInput();
            ProcessHandExpressionInput();
            ProcessHandExpressionPlayback();

#if ENABLE_VR || UNITY_GAMECORE
            m_DeviceLifecycleManager.ApplyHandState(m_LeftHandState, m_RightHandState);
            m_DeviceLifecycleManager.ApplyHMDState(m_HMDState);
            m_DeviceLifecycleManager.ApplyControllerState(m_LeftControllerState, m_RightControllerState);
#endif

#if XR_SIMULATION_AVAILABLE
            if (m_SimulationCameraPoseProvider != null)
                m_SimulationCameraPoseProvider.transform.SetWorldPose(m_CameraTransform.GetWorldPose());
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_ScreenToWorldPoint, m_HitEndPoint);
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated device(s)
        /// related to position and rotation.
        /// </summary>
        protected virtual void ProcessPoseInput()
        {
#if ENABLE_VR || UNITY_GAMECORE
            SetTrackedStates();

            if (m_TargetedDeviceInput == TargetedDevices.None)
                return;

            if (!XRSimulatorUtility.FindCameraTransform(ref m_CachedCamera, ref m_CameraTransform))
                return;

            var cameraParent = m_CameraTransform.parent;
            var cameraParentRotation = cameraParent != null ? cameraParent.rotation : Quaternion.identity;
            var inverseCameraParentRotation = Quaternion.Inverse(cameraParentRotation);

            // If we are not manipulating any input, manipulate the devices as an FPS controller.
            // It allows the player to translate along the ground and rotate while keeping the controllers in front,
            // essentially rotating the HMD and both controllers around a common pivot rather than local to each.
            // Time delay as a workaround to avoid large mouse deltas on the first frame.
            if (manipulatingFPS && Time.time > 1f)
            {
                // Translation, with scroll contribution to the forward direction
                var xTranslateInput = m_TranslateXValue * m_TranslateXSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var yTranslateInput = m_TranslateYValue * m_TranslateYSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var zTranslateInput = (m_TranslateZValue + m_MouseScrollValue.y) * m_TranslateZSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var translationInDeviceSpace = XRSimulatorUtility.GetTranslationInDeviceSpace(xTranslateInput, yTranslateInput, zTranslateInput,
                    m_CameraTransform, cameraParentRotation, inverseCameraParentRotation);

                // Modify both controllers and hands in FPS mode no matter the device mode of the simulator
                // because we want to keep the devices in front. If we only updated one set, switching the mode
                // to the other would have the other devices no longer in front in the same relative position,
                // which is probably not what the user wants.
                m_LeftControllerState.devicePosition += translationInDeviceSpace;
                m_RightControllerState.devicePosition += translationInDeviceSpace;
                m_LeftHandState.position += translationInDeviceSpace;
                m_RightHandState.position += translationInDeviceSpace;

                m_HMDState.centerEyePosition += translationInDeviceSpace;
                m_HMDState.devicePosition = m_HMDState.centerEyePosition;

                // Rotation
                var scaledRotationDeltaInput =
                    new Vector2(m_RotationDeltaValue.x * m_RotateXSensitivity,
                        m_RotationDeltaValue.y * m_RotateYSensitivity * (m_RotateYInvert ? 1f : -1f));

                Vector3 anglesDelta;
                if (m_XConstraintValue && !m_YConstraintValue && !m_ZConstraintValue) // X
                    anglesDelta = new Vector3(-scaledRotationDeltaInput.x + scaledRotationDeltaInput.y, 0f, 0f);
                else if (!m_XConstraintValue && m_YConstraintValue && !m_ZConstraintValue) // Y
                    anglesDelta = new Vector3(0f, scaledRotationDeltaInput.x + -scaledRotationDeltaInput.y, 0f);
                else
                    anglesDelta = new Vector3(scaledRotationDeltaInput.y, scaledRotationDeltaInput.x, 0f);

                m_CenterEyeEuler += anglesDelta;
                // Avoid awkward pitch angles
                m_CenterEyeEuler.x = Mathf.Clamp(m_CenterEyeEuler.x, -XRSimulatorUtility.cameraMaxXAngle, XRSimulatorUtility.cameraMaxXAngle);
                m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                m_HMDState.deviceRotation = m_HMDState.centerEyeRotation;

                var controllerRotationDelta = Quaternion.AngleAxis(anglesDelta.y, Quaternion.Euler(0f, m_CenterEyeEuler.y, 0f) * Vector3.up);
                var pivotPoint = m_HMDState.centerEyePosition;

                // Controllers
                m_LeftControllerState.devicePosition = controllerRotationDelta * (m_LeftControllerState.devicePosition - pivotPoint) + pivotPoint;
                m_LeftControllerState.deviceRotation = controllerRotationDelta * m_LeftControllerState.deviceRotation;
                m_RightControllerState.devicePosition = controllerRotationDelta * (m_RightControllerState.devicePosition - pivotPoint) + pivotPoint;
                m_RightControllerState.deviceRotation = controllerRotationDelta * m_RightControllerState.deviceRotation;

                // Replace euler angle representation with the updated value to make sure
                // the rotation of the controller doesn't jump when manipulating them not in FPS mode.
                m_LeftControllerEuler = m_LeftControllerState.deviceRotation.eulerAngles;
                m_RightControllerEuler = m_RightControllerState.deviceRotation.eulerAngles;

                // Hands
                m_LeftHandState.position = controllerRotationDelta * (m_LeftHandState.position - pivotPoint) + pivotPoint;
                m_LeftHandState.rotation = controllerRotationDelta * m_LeftHandState.rotation;
                m_RightHandState.position = controllerRotationDelta * (m_RightHandState.position - pivotPoint) + pivotPoint;
                m_RightHandState.rotation = controllerRotationDelta * m_RightHandState.rotation;

                m_LeftHandState.euler = m_LeftHandState.rotation.eulerAngles;
                m_RightHandState.euler = m_RightHandState.rotation.eulerAngles;
            }
            else if (!manipulatingFPS)
            {
                var xTranslateInput = m_TranslateXValue * m_TranslateXSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var yTranslateInput = m_TranslateYValue * m_TranslateYSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var zTranslateInput = m_TranslateZValue * m_TranslateZSpeed * m_BodyTranslateMultiplier * Time.deltaTime;
                var deltaPosition = XRSimulatorUtility.GetTranslationInDeviceSpace(xTranslateInput, yTranslateInput, zTranslateInput, m_CameraTransform, cameraParentRotation, inverseCameraParentRotation);

                var scaledRotationDeltaInput =
                    new Vector2(m_RotationDeltaValue.x * m_RotateXSensitivity,
                        m_RotationDeltaValue.y * m_RotateYSensitivity * (m_RotateYInvert ? 1f : -1f));

                Vector3 anglesDelta;
                if (m_XConstraintValue && !m_YConstraintValue && m_ZConstraintValue) // XZ
                    anglesDelta = new Vector3(scaledRotationDeltaInput.y, 0f, -scaledRotationDeltaInput.x);
                else if (!m_XConstraintValue && m_YConstraintValue && m_ZConstraintValue) // YZ
                    anglesDelta = new Vector3(0f, scaledRotationDeltaInput.x, -scaledRotationDeltaInput.y);
                else if (m_XConstraintValue && !m_YConstraintValue && !m_ZConstraintValue) // X
                    anglesDelta = new Vector3(-scaledRotationDeltaInput.x + scaledRotationDeltaInput.y, 0f, 0f);
                else if (!m_XConstraintValue && m_YConstraintValue && !m_ZConstraintValue) // Y
                    anglesDelta = new Vector3(0f, scaledRotationDeltaInput.x + -scaledRotationDeltaInput.y, 0f);
                else if (!m_XConstraintValue && !m_YConstraintValue && m_ZConstraintValue) // Z
                    anglesDelta = new Vector3(0f, 0f, -scaledRotationDeltaInput.x + -scaledRotationDeltaInput.y);
                else
                    anglesDelta = new Vector3(scaledRotationDeltaInput.y, scaledRotationDeltaInput.x, 0f);

                // Scroll contribution
                anglesDelta += new Vector3(0f, 0f, m_MouseScrollValue.y * k_MouseScrollSensitivity * m_MouseScrollRotateSensitivity);

                if (manipulatingLeftController)
                {
                    var deltaRotation = XRSimulatorUtility.GetDeltaRotation(m_TranslateSpace, m_LeftControllerState, inverseCameraParentRotation);
                    m_LeftControllerState.devicePosition += deltaRotation * deltaPosition;

                    if (!m_PointAndClickActive)
                    {
                        m_LeftControllerEuler += anglesDelta;
                        m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);
                    }
                }

                if (manipulatingRightController)
                {
                    var deltaRotation = XRSimulatorUtility.GetDeltaRotation(m_TranslateSpace, m_RightControllerState, inverseCameraParentRotation);
                    m_RightControllerState.devicePosition += deltaRotation * deltaPosition;

                    if (!m_PointAndClickActive)
                    {
                        m_RightControllerEuler += anglesDelta;
                        m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);
                    }
                }

                if (manipulatingLeftHand)
                {
                    var deltaRotation = XRSimulatorUtility.GetDeltaRotation(m_TranslateSpace, m_LeftHandState, inverseCameraParentRotation);
                    m_LeftHandState.position += deltaRotation * deltaPosition;

                    m_LeftHandState.euler += anglesDelta;
                    m_LeftHandState.rotation = Quaternion.Euler(m_LeftHandState.euler);
                }

                if (manipulatingRightHand)
                {
                    var deltaRotation = XRSimulatorUtility.GetDeltaRotation(m_TranslateSpace, m_RightHandState, inverseCameraParentRotation);
                    m_RightHandState.position += deltaRotation * deltaPosition;

                    m_RightHandState.euler += anglesDelta;
                    m_RightHandState.rotation = Quaternion.Euler(m_RightHandState.euler);
                }

                if (m_TargetedDeviceInput.HasDevice(TargetedDevices.HMD))
                {
                    var deltaRotation = XRSimulatorUtility.GetDeltaRotation(m_TranslateSpace, m_HMDState, inverseCameraParentRotation);
                    m_HMDState.centerEyePosition += deltaRotation * deltaPosition;
                    m_HMDState.devicePosition = m_HMDState.centerEyePosition;

                    if (!m_PointAndClickActive)
                    {
                        m_CenterEyeEuler += anglesDelta;
                        m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                        m_HMDState.deviceRotation = m_HMDState.centerEyeRotation;
                    }
                }

                if (m_PointAndClickActive && (m_TargetedDeviceInput.HasDevice(TargetedDevices.HMD) || manipulatingLeftController || manipulatingRightController))
                {
                    m_CenterEyeEuler += anglesDelta;
                    m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                    m_HMDState.deviceRotation = m_HMDState.centerEyeRotation;
                }
            }

            // Reset
            if (m_ResetValue)
            {
                var headForward = (m_HMDState.deviceRotation * Vector3.forward) * k_DeviceForwardOffsetAmount;
                var downOffset = (m_HMDState.deviceRotation * Vector3.down) * k_DeviceDownOffsetAmount;
                var leftControllerOffset = (m_HMDState.deviceRotation * Vector3.left) * k_DeviceLeftRightOffsetAmount;
                var rightControllerOffset = (m_HMDState.deviceRotation * Vector3.right) * k_DeviceLeftRightOffsetAmount;

                // Controllers
                // We reset both position and rotation, so axis constraint is ignored
                m_LeftControllerState.devicePosition = m_HMDState.devicePosition + headForward + downOffset + leftControllerOffset;
                m_RightControllerState.devicePosition = m_HMDState.devicePosition + headForward + downOffset + rightControllerOffset;

                m_LeftControllerEuler = m_HMDState.deviceRotation.eulerAngles;
                m_LeftControllerState.deviceRotation = m_HMDState.deviceRotation;

                m_RightControllerEuler = m_HMDState.deviceRotation.eulerAngles;
                m_RightControllerState.deviceRotation = m_HMDState.deviceRotation;

                // Hands
                m_LeftHandState.position = m_HMDState.devicePosition + headForward + downOffset + leftControllerOffset;
                m_RightHandState.position = m_HMDState.devicePosition + headForward + downOffset + rightControllerOffset;

                m_LeftHandState.euler = m_HMDState.deviceRotation.eulerAngles;
                m_LeftHandState.rotation = m_HMDState.deviceRotation;

                m_RightHandState.euler = m_HMDState.deviceRotation.eulerAngles;
                m_RightHandState.rotation = m_HMDState.deviceRotation;
            }
#endif
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to input controls.
        /// </summary>
        protected virtual void ProcessControlInput()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_DeviceLifecycleManager.deviceMode != SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                return;

            if (m_LeftDeviceActionsInput.ReadIsPerformed())
            {
                ProcessButtonControlInput(ref m_LeftControllerState);
                ProcessAxis2DControlInput(ref m_LeftControllerState);
            }
            else
            {
                ProcessButtonControlInput(ref m_RightControllerState);
                ProcessAxis2DControlInput(ref m_RightControllerState);
            }

            if (!manipulatingLeftController)
                ProcessAnalogButtonControlInput(ref m_LeftControllerState);

            if (!manipulatingRightController)
                ProcessAnalogButtonControlInput(ref m_RightControllerState);
#endif
        }

        void ProcessHandExpressionInput()
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceLifecycleManager == null || m_DeviceLifecycleManager.deviceMode != SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                return;

            if (m_HandPlaybackManager.restingHandExpression != null && m_HandPlaybackManager.restingHandExpression.toggleInput != null &&
                m_HandPlaybackManager.restingHandExpression.toggleInput.ReadWasPerformedThisFrame())
            {
                ProcessToggledHandExpressionInput(m_HandPlaybackManager.restingHandExpression, 0);
                return;
            }

            for (var index = 0; index < m_HandPlaybackManager.simulatedHandExpressions.Count; ++index)
            {
                var simulatedExpression = m_HandPlaybackManager.simulatedHandExpressions[index];
                if (simulatedExpression.toggleInput.ReadWasPerformedThisFrame())
                    ProcessToggledHandExpressionInput(simulatedExpression, index);
            }
#endif
        }

        void ProcessToggledHandExpressionInput(SimulatedHandExpression handExpression, int index)
        {
            if (m_LeftDeviceActionsInput.ReadIsPerformed())
            {
#if XR_HANDS_1_8_OR_NEWER
                m_LeftHandHotkeyToggled = true;
#endif
                m_LeftCurrentHandExpression = handExpression;
                m_LeftHandExpressionIndex = index;
                ToggleHandExpression(true, false);
            }
            else
            {
#if XR_HANDS_1_8_OR_NEWER
                m_RightHandHotkeyToggled = true;
#endif
                m_RightCurrentHandExpression = handExpression;
                m_RightHandExpressionIndex = index;
                ToggleHandExpression(false, true);
            }

        }

        void ProcessHandExpressionPlayback()
        {
#if XR_HANDS_1_8_OR_NEWER
            if (m_HandPlaybackManager == null)
                return;

            m_HandPlaybackManager.ProcessHandSequencePlayback(Handedness.Left);
            m_HandPlaybackManager.ProcessHandSequencePlayback(Handedness.Right);
#endif
        }

        void ToggleHandExpression(bool leftHand, bool rightHand)
        {
#if XR_HANDS_1_8_OR_NEWER
            if (m_DeviceLifecycleManager == null || m_DeviceLifecycleManager.handSubsystem == null)
                return;

            if (leftHand)
            {
                m_HandPlaybackManager.ToggleHandPlayback(m_LeftCurrentHandExpression, m_LeftHandHotkeyToggled, Handedness.Left);
                m_LeftHandHotkeyToggled = false;
            }

            if (rightHand)
            {
                m_HandPlaybackManager.ToggleHandPlayback(m_RightCurrentHandExpression, m_RightHandHotkeyToggled, Handedness.Right);
                m_RightHandHotkeyToggled = false;
            }
#endif
        }

#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to 2D Axis input controls.
        /// </summary>
        protected virtual void ProcessAxis2DControlInput(ref XRSimulatedControllerState controllerState)
        {
            if ((axis2DTargets & Axis2DTargets.Primary2DAxis) != 0)
            {
                controllerState.primary2DAxis = m_Axis2DValue;
            }

            if ((axis2DTargets & Axis2DTargets.Secondary2DAxis) != 0)
            {
                controllerState.secondary2DAxis = m_Axis2DValue;
            }
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to button input controls.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            if (m_GripInput.ReadIsPerformed())
            {
                controllerState.grip = m_GripAmount;
                controllerState.WithButton(ControllerButton.GripButton);
            }
            else if (m_GripInput.ReadWasCompletedThisFrame())
            {
                controllerState.grip = 0f;
                controllerState.WithButton(ControllerButton.GripButton, false);
            }

            if (m_TriggerInput.ReadIsPerformed())
            {
                controllerState.trigger = m_TriggerAmount;
                controllerState.WithButton(ControllerButton.TriggerButton);
            }
            else if (m_TriggerInput.ReadWasCompletedThisFrame())
            {
                controllerState.trigger = 0f;
                controllerState.WithButton(ControllerButton.TriggerButton, false);
            }

            if (m_PrimaryButtonInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.PrimaryButton);
            else if (m_PrimaryButtonInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.PrimaryButton, false);

            if (m_SecondaryButtonInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.SecondaryButton);
            else if (m_SecondaryButtonInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.SecondaryButton, false);

            if (m_MenuInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.MenuButton);
            else if (m_MenuInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.MenuButton, false);

            if (m_Primary2DAxisClickInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.Primary2DAxisClick);
            else if (m_Primary2DAxisClickInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.Primary2DAxisClick, false);

            if (m_Secondary2DAxisClickInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.Secondary2DAxisClick);
            else if (m_Secondary2DAxisClickInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.Secondary2DAxisClick, false);

            if (m_Primary2DAxisTouchInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.Primary2DAxisTouch);
            else if (m_Primary2DAxisTouchInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.Primary2DAxisTouch, false);

            if (m_Secondary2DAxisTouchInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.Secondary2DAxisTouch);
            else if (m_Secondary2DAxisTouchInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.Secondary2DAxisTouch, false);

            if (m_PrimaryTouchInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.PrimaryTouch);
            else if (m_PrimaryTouchInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.PrimaryTouch, false);

            if (m_SecondaryTouchInput.ReadIsPerformed())
                controllerState.WithButton(ControllerButton.SecondaryTouch);
            else if (m_SecondaryTouchInput.ReadWasCompletedThisFrame())
                controllerState.WithButton(ControllerButton.SecondaryTouch, false);
        }

        /// <summary>
        /// Update the state of manipulated controller device related to analog values only.
        /// This is used to adjust the grip and trigger values when the user adjusts the slider
        /// when not manipulating the device.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessAnalogButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            if (controllerState.HasButton(ControllerButton.GripButton))
                controllerState.grip = m_GripAmount;

            if (controllerState.HasButton(ControllerButton.TriggerButton))
                controllerState.trigger = m_TriggerAmount;
        }
#endif

        /// <summary>
        /// Gets a <see cref="Vector3"/> that can be multiplied component-wise with another <see cref="Vector3"/>
        /// to reset components of the <see cref="Vector3"/>, based on axis constraint inputs.
        /// </summary>
        /// <returns>Returns a <see cref="Vector3"/> mask used to reset the components of another <see cref="Vector3"/>.</returns>
        /// <seealso cref="resetInput"/>
        /// <seealso cref="xConstraintInput"/>
        /// <seealso cref="yConstraintInput"/>
        /// <seealso cref="zConstraintInput"/>
        protected Vector3 GetResetScale()
        {
            return m_XConstraintValue || m_YConstraintValue || m_ZConstraintValue
                ? new Vector3(m_XConstraintValue ? 0f : 1f, m_YConstraintValue ? 0f : 1f, m_ZConstraintValue ? 0f : 1f)
                : Vector3.zero;
        }

        /// <summary>
        /// Reads any new values from the input readers and applies it to the corresponding value or state properties
        /// for further processing.
        /// </summary>
        protected virtual void ReadInputValues()
        {
#if ENABLE_VR || UNITY_GAMECORE
            // Translation & Rotation
            m_TranslateXValue = m_TranslateXInput.ReadValue();
            m_TranslateYValue = m_TranslateYInput.ReadValue();
            m_TranslateZValue = m_TranslateZInput.ReadValue();
            m_RotationDeltaValue = m_KeyboardRotationDeltaInput.ReadValue();

            if (m_ToggleMouseInput.ReadIsPerformed())
            {
                Vector2 mouseRotationValue = m_MouseRotationDeltaInput.ReadValue();

                if (mouseRotationValue != Vector2.zero)
                    m_RotationDeltaValue = mouseRotationValue;

                m_MouseScrollValue = ScrollUtility.GetNormalized(m_MouseScrollInput.ReadValue());
            }

            m_XConstraintValue = m_XConstraintInput.ReadIsPerformed();
            m_YConstraintValue = m_YConstraintInput.ReadIsPerformed();
            m_ZConstraintValue = m_ZConstraintInput.ReadIsPerformed();
            m_ResetValue = m_ResetInput.ReadWasPerformedThisFrame();

            m_Axis2DValue = Vector2.ClampMagnitude(m_Axis2DInput.ReadValue(), 1f);

            if (m_TogglePrimary2DAxisTargetInput.ReadWasPerformedThisFrame())
                axis2DTargets = Axis2DTargets.Primary2DAxis;

            if (m_ToggleSecondary2DAxisTargetInput.ReadWasPerformedThisFrame())
                axis2DTargets = Axis2DTargets.Secondary2DAxis;
#endif
        }

        void InitializeControllerHandActions()
        {
            if (m_QuickActionControllerInputModes.Count > 0)
            {
                m_LeftControllerInputMode = m_QuickActionControllerInputModes[0];
                m_RightControllerInputMode = m_QuickActionControllerInputModes[0];
            }

            m_LeftCurrentHandExpression = m_HandPlaybackManager.restingHandExpression;
            m_RightCurrentHandExpression = m_HandPlaybackManager.restingHandExpression;
        }

        void CycleQuickAction()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
            {
                if (m_QuickActionControllerInputModes.Count == 0)
                {
                    Debug.LogWarning("The key to switch between controller inputs has been pressed," +
                        " but there doesn't seem to be any inputs set in the quick-action controller input modes.", this);
                    return;
                }

                if (manipulatingLeftController)
                {
                    m_LeftControllerInputModeIndex = (m_LeftControllerInputModeIndex < (m_QuickActionControllerInputModes.Count - 1)) ? (m_LeftControllerInputModeIndex + 1) : 0;
                    m_LeftControllerInputMode = m_QuickActionControllerInputModes[m_LeftControllerInputModeIndex];
                }

                if (manipulatingRightController)
                {
                    m_RightControllerInputModeIndex = (m_RightControllerInputModeIndex < (m_QuickActionControllerInputModes.Count - 1)) ? (m_RightControllerInputModeIndex + 1) : 0;
                    m_RightControllerInputMode = m_QuickActionControllerInputModes[m_RightControllerInputModeIndex];
                }
            }
            else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
            {
                CycleQuickActionHandExpression();
            }
#endif
        }

        void PerformQuickAction()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
            {
                if (manipulatingLeftController)
                    ToggleControllerButtonInput(ref m_LeftControllerState, m_LeftControllerInputMode);
                if (manipulatingRightController)
                    ToggleControllerButtonInput(ref m_RightControllerState, m_RightControllerInputMode);
            }
            else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
            {
                ToggleHandExpression(manipulatingLeftHand, manipulatingRightHand);
            }
#endif
        }

#if ENABLE_VR || UNITY_GAMECORE
        void ToggleControllerButtonInput(ref XRSimulatedControllerState controllerState, ControllerInputMode inputMode)
        {
            switch (inputMode)
            {
                case ControllerInputMode.None:
                    break;
                case ControllerInputMode.Trigger:
                    controllerState.ToggleButton(ControllerButton.TriggerButton);
                    controllerState.trigger = controllerState.HasButton(ControllerButton.TriggerButton) ? m_TriggerAmount : 0f;
                    break;
                case ControllerInputMode.Grip:
                    controllerState.ToggleButton(ControllerButton.GripButton);
                    controllerState.grip = controllerState.HasButton(ControllerButton.GripButton) ? m_GripAmount : 0f;
                    break;
                case ControllerInputMode.PrimaryButton:
                    controllerState.ToggleButton(ControllerButton.PrimaryButton);
                    break;
                case ControllerInputMode.SecondaryButton:
                    controllerState.ToggleButton(ControllerButton.SecondaryButton);
                    break;
                case ControllerInputMode.Menu:
                    controllerState.ToggleButton(ControllerButton.MenuButton);
                    break;
                case ControllerInputMode.Primary2DAxisClick:
                    controllerState.ToggleButton(ControllerButton.Primary2DAxisClick);
                    break;
                case ControllerInputMode.Secondary2DAxisClick:
                    controllerState.ToggleButton(ControllerButton.Secondary2DAxisClick);
                    break;
                case ControllerInputMode.Primary2DAxisTouch:
                    controllerState.ToggleButton(ControllerButton.Primary2DAxisTouch);
                    break;
                case ControllerInputMode.Secondary2DAxisTouch:
                    controllerState.ToggleButton(ControllerButton.Secondary2DAxisTouch);
                    break;
                case ControllerInputMode.PrimaryTouch:
                    controllerState.ToggleButton(ControllerButton.PrimaryTouch);
                    break;
                case ControllerInputMode.SecondaryTouch:
                    controllerState.ToggleButton(ControllerButton.SecondaryTouch);
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(inputMode)}={inputMode}.");
                    break;
            }
        }

        static void ClearControllerButtonInput(ref XRSimulatedControllerState controllerState)
        {
            controllerState.trigger = 0f;
            controllerState.grip = 0f;
            controllerState.buttons = 0;
        }

        void SetTrackedStates()
        {
            m_LeftControllerState.isTracked = m_LeftControllerIsTracked;
            m_RightControllerState.isTracked = m_RightControllerIsTracked;
            m_LeftHandState.isTracked = m_LeftHandIsTracked;
            m_RightHandState.isTracked = m_RightHandIsTracked;
            m_HMDState.isTracked = m_HMDIsTracked;
            m_LeftControllerState.trackingState = (int)m_LeftControllerTrackingState;
            m_RightControllerState.trackingState = (int)m_RightControllerTrackingState;
            m_HMDState.trackingState = (int)m_HMDTrackingState;
        }
#endif

        void CycleTargetDevices()
        {
            if (targetedDeviceInput.HasDevice(TargetedDevices.HMD))
                targetedDeviceInput = targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);

            if (targetedDeviceInput == TargetedDevices.None)
                targetedDeviceInput = TargetedDevices.FPS | TargetedDevices.RightDevice;
            else if (targetedDeviceInput.HasDevice(TargetedDevices.FPS))
            {
                targetedDeviceInput = targetedDeviceInput.WithoutDevice(TargetedDevices.FPS);

                if (!targetedDeviceInput.HasDevice(TargetedDevices.LeftDevice) && !targetedDeviceInput.HasDevice(TargetedDevices.RightDevice))
                    targetedDeviceInput = TargetedDevices.LeftDevice | TargetedDevices.RightDevice;
            }
            else if (targetedDeviceInput.HasDevice(TargetedDevices.LeftDevice) || targetedDeviceInput.HasDevice(TargetedDevices.RightDevice))
                targetedDeviceInput = targetedDeviceInput.WithDevice(TargetedDevices.FPS);
        }

        void HandleLeftOrRightDeviceToggle()
        {
            if (m_ToggleManipulateWaitingForReleaseBoth)
            {
                m_ToggleManipulateWaitingForReleaseBoth = m_ToggleManipulateLeftInput.ReadIsPerformed() || m_ToggleManipulateRightInput.ReadIsPerformed();
                return;
            }

            // If both buttons are pressed simultaneously, activate both devices.
            // We don't wait until release in that case in order to have immediate feedback that the gesture was accepted.
            // Waiting until button release for individual presses makes it easier to avoid unintended changes
            // when the user wants to activate the simultaneous gesture.
            if (m_ToggleManipulateLeftInput.ReadIsPerformed() && m_ToggleManipulateRightInput.ReadIsPerformed())
            {
                if (targetedDeviceInput.HasDevice(TargetedDevices.HMD))
                    targetedDeviceInput = targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);

                // Once both are pressed simultaneously,
                // prevent further toggling until both buttons are released.
                m_ToggleManipulateWaitingForReleaseBoth = true;

                if (targetedDeviceInput == (TargetedDevices.LeftDevice | TargetedDevices.RightDevice))
                    m_DeviceLifecycleManager.SwitchDeviceMode();
                else
                    targetedDeviceInput = targetedDeviceInput.WithDevice(TargetedDevices.LeftDevice).WithDevice(TargetedDevices.RightDevice)
                        .WithoutDevice(TargetedDevices.FPS);
            }
            else if (m_ToggleManipulateLeftInput.ReadWasCompletedThisFrame())
            {
                if (targetedDeviceInput.HasDevice(TargetedDevices.HMD))
                    targetedDeviceInput = targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);

                if (targetedDeviceInput == TargetedDevices.LeftDevice)
                    m_DeviceLifecycleManager.SwitchDeviceMode();
                else
                    targetedDeviceInput = targetedDeviceInput.WithDevice(TargetedDevices.LeftDevice)
                        .WithoutDevice(TargetedDevices.RightDevice).WithoutDevice(TargetedDevices.FPS);
            }
            else if (m_ToggleManipulateRightInput.ReadWasCompletedThisFrame())
            {
                if (targetedDeviceInput.HasDevice(TargetedDevices.HMD))
                    targetedDeviceInput = targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);

                if (targetedDeviceInput == TargetedDevices.RightDevice)
                    m_DeviceLifecycleManager.SwitchDeviceMode();
                else
                    targetedDeviceInput = targetedDeviceInput.WithDevice(TargetedDevices.RightDevice)
                        .WithoutDevice(TargetedDevices.LeftDevice).WithoutDevice(TargetedDevices.FPS);
            }
        }

        void HandleHMDToggle()
        {
            if (targetedDeviceInput != TargetedDevices.HMD)
            {
                m_PreviousTargetedDevices = targetedDeviceInput;
                targetedDeviceInput = TargetedDevices.HMD;
            }
            else
            {
                targetedDeviceInput = m_PreviousTargetedDevices;
            }
        }

        void CycleQuickActionHandExpression()
        {
#if XR_HANDS_1_8_OR_NEWER
            if (m_DeviceLifecycleManager == null)
                return;

            var handExpressions = m_HandPlaybackManager.simulatedHandExpressions;

            if (manipulatingLeftHand)
            {
                int index = (m_LeftCurrentHandExpression == m_HandPlaybackManager.restingHandExpression) ? -1 : m_LeftHandExpressionIndex;
                for (var i = 0; i < handExpressions.Count; ++i)
                {
                    index = (index < (handExpressions.Count - 1)) ? (index + 1) : 0;
                    if (handExpressions[index].isQuickAction)
                    {
                        m_LeftHandExpressionIndex = index;
                        m_LeftCurrentHandExpression = handExpressions[index];
                        m_LeftHandHotkeyToggled = false;
                        m_HandPlaybackManager.ToggleHandPlayback(m_LeftCurrentHandExpression, false, Handedness.Left);
                        break;
                    }
                }
            }

            if (manipulatingRightHand)
            {
                int index = (m_RightCurrentHandExpression == m_HandPlaybackManager.restingHandExpression) ? -1 : m_RightHandExpressionIndex;
                for (var i = 0; i < handExpressions.Count; ++i)
                {
                    index = (index < (handExpressions.Count - 1)) ? (index + 1) : 0;
                    if (handExpressions[index].isQuickAction)
                    {
                        m_RightHandExpressionIndex = index;
                        m_RightCurrentHandExpression = handExpressions[index];
                        m_RightHandHotkeyToggled = false;
                        m_HandPlaybackManager.ToggleHandPlayback(m_RightCurrentHandExpression, false, Handedness.Right);
                        break;
                    }
                }
            }
#endif
        }

        void OnDeviceModeChanged(SimulatedDeviceLifecycleManager.DeviceMode mode)
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceLifecycleManager == null || m_DeviceLifecycleManager.handSubsystem == null || m_HandPlaybackManager.restingHandExpression == null)
                return;

            if (mode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
            {
#if ENABLE_VR || UNITY_GAMECORE
                ClearControllerButtonInput(ref m_LeftControllerState);
                ClearControllerButtonInput(ref m_RightControllerState);
#endif
#if XR_HANDS_1_8_OR_NEWER
                if (m_HandPlaybackManager.restingHandExpression.captureSequence != null)
                {
                    m_HandPlaybackManager.ToggleHandPlayback(m_HandPlaybackManager.restingHandExpression, false, Handedness.Left);
                    m_HandPlaybackManager.ToggleHandPlayback(m_HandPlaybackManager.restingHandExpression, false, Handedness.Right);
                    m_LeftCurrentHandExpression = m_HandPlaybackManager.restingHandExpression;
                    m_RightCurrentHandExpression = m_HandPlaybackManager.restingHandExpression;
                }
#endif
            }
            else
            {
#if XR_HANDS_1_8_OR_NEWER
                var playbackLeft = m_DeviceLifecycleManager.leftHandPlayback;
                if (playbackLeft != null)
                    playbackLeft.sourceCaptureSequence = null;

                var playbackRight = m_DeviceLifecycleManager.rightHandPlayback;
                if (playbackRight != null)
                    playbackRight.sourceCaptureSequence = null;
#endif
            }
#endif
        }

        void ProcessPointAndClick()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_PointAndClickActive && !RotationInputIsPerformed())
            {
                if (!XRSimulatorUtility.FindCameraTransform(ref m_CachedCamera, ref m_CameraTransform))
                    return;

                Vector2 screenPosition = m_MousePointInput.ReadValue();
                m_ScreenToWorldPoint = m_CachedCamera.camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, m_CachedCamera.camera.nearClipPlane));
                m_RaycastDirVector = (m_ScreenToWorldPoint - m_CameraTransform.position).normalized;

                if (TryGetClosestRaycastHitPoint(screenPosition, m_ScreenToWorldPoint, m_RaycastDirVector, out var hitPosition, out var hitType))
                {
                    m_HitEndPoint = hitPosition;

                    if (manipulatingLeftController && m_LeftControllerTransform != null)
                        ConvertWorldPointToLocalAimRotation(m_HitEndPoint, m_LeftControllerTransform, ref m_LeftControllerState);

                    if (manipulatingRightController && m_RightControllerTransform != null)
                        ConvertWorldPointToLocalAimRotation(m_HitEndPoint, m_RightControllerTransform, ref m_RightControllerState);
                }
                else
                {
                    // If we don't have a previous hit distance, use a default distance instead. This prevents the controllers from locking up
                    // when there is no previous hit (e.g. when starting point-and-click in empty space).
                    float distance = m_PreviousRaycastHitDistance > 0f ? m_PreviousRaycastHitDistance : k_DefaultRaycastDistance;
                    Ray screenPointToRay = m_CachedCamera.camera.ScreenPointToRay(screenPosition);
                    Vector3 cachedDistancePoint = screenPointToRay.GetPoint(distance);

                    if (manipulatingLeftController && m_LeftControllerTransform != null)
                        ConvertWorldPointToLocalAimRotation(cachedDistancePoint, m_LeftControllerTransform, ref m_LeftControllerState);

                    if (manipulatingRightController && m_RightControllerTransform != null)
                         ConvertWorldPointToLocalAimRotation(cachedDistancePoint, m_RightControllerTransform, ref m_RightControllerState);
                }

                if (m_MouseClickInput.ReadWasPerformedThisFrame())
                    HandlePointAndClickSelect(hitType);

                if (m_MouseClickInput.ReadWasCompletedThisFrame() ||
                    (!m_MouseClickInput.ReadIsPerformed() &&
                        (m_PerformingLeftPointAndClickGripInteraction || m_PerformingLeftPointAndClickTriggerInteraction ||
                            m_PerformingRightPointAndClickGripInteraction || m_PerformingRightPointAndClickTriggerInteraction)))
                {
                    ClearPointAndClickSelect();
                }
            }
#endif
        }

#if ENABLE_VR || UNITY_GAMECORE
        static void ConvertWorldPointToLocalAimRotation(Vector3 point, Transform referenceTransform, ref XRSimulatedControllerState controllerState)
        {
            var localHitPoint = referenceTransform.parent.InverseTransformPoint(point);
            var controllerDirVector = (localHitPoint - controllerState.devicePosition).normalized;
            controllerState.deviceRotation = Quaternion.LookRotation(controllerDirVector);
        }
#endif

        /// <summary>
        /// Checks whether rotation input is currently being performed from keyboard or mouse.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if rotation input is currently being performed.</returns>
        internal bool RotationInputIsPerformed() => m_ToggleMouseInput.ReadIsPerformed() || m_RotationDeltaValue != Vector2.zero;

        void HandlePointAndClickSelect(HitType hitType)
        {
#if ENABLE_VR || UNITY_GAMECORE
            // Set controller's input. Default to grip, but do trigger if world-space UI hit.
            // We also do grip when hitting nothing since it's still possible the interactor is hitting an interactable
            // object since sphere/cone casting may have hit something that the ray cast done by this simulator did not.
            // Users should be able to at least try selecting the object, especially when there is a valid hover.
            // Do nothing if hitting screen-space UI.
            if (hitType == HitType.Object || hitType == HitType.None)
            {
                if (manipulatingLeftController)
                {
                    m_LeftControllerState.grip = m_GripAmount;
                    m_LeftControllerState.WithButton(ControllerButton.GripButton);
                    m_PerformingLeftPointAndClickGripInteraction = true;
                }

                if (manipulatingRightController)
                {
                    m_RightControllerState.grip = m_GripAmount;
                    m_RightControllerState.WithButton(ControllerButton.GripButton);
                    m_PerformingRightPointAndClickGripInteraction = true;
                }
            }
            else if (hitType == HitType.WorldUI)
            {
                if (manipulatingLeftController)
                {
                    m_LeftControllerState.trigger = m_TriggerAmount;
                    m_LeftControllerState.WithButton(ControllerButton.TriggerButton);
                    m_PerformingLeftPointAndClickTriggerInteraction = true;
                }

                if (manipulatingRightController)
                {
                    m_RightControllerState.trigger = m_TriggerAmount;
                    m_RightControllerState.WithButton(ControllerButton.TriggerButton);
                    m_PerformingRightPointAndClickTriggerInteraction = true;
                }
            }
#endif
        }

        void ClearPointAndClickSelect()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_PerformingLeftPointAndClickGripInteraction)
            {
                m_LeftControllerState.grip = 0f;
                m_LeftControllerState.WithButton(ControllerButton.GripButton, false);
                m_PerformingLeftPointAndClickGripInteraction = false;
            }

            if (m_PerformingLeftPointAndClickTriggerInteraction)
            {
                m_LeftControllerState.trigger = 0f;
                m_LeftControllerState.WithButton(ControllerButton.TriggerButton, false);
                m_PerformingLeftPointAndClickTriggerInteraction = false;
            }

            if (m_PerformingRightPointAndClickGripInteraction)
            {
                m_RightControllerState.grip = 0f;
                m_RightControllerState.WithButton(ControllerButton.GripButton, false);
                m_PerformingRightPointAndClickGripInteraction = false;
            }

            if (m_PerformingRightPointAndClickTriggerInteraction)
            {
                m_RightControllerState.trigger = 0f;
                m_RightControllerState.WithButton(ControllerButton.TriggerButton, false);
                m_PerformingRightPointAndClickTriggerInteraction = false;
            }
#endif
        }

        bool TryGetClosestRaycastHitPoint(Vector2 screenPosition, Vector3 origin, Vector3 direction, out Vector3 hitPosition, out HitType hitType)
        {
            //CONSIDER: #XRA-663
            var raycastHitCount = m_LocalPhysicsScene.Raycast(origin, direction, m_RaycastHits);
            var uiRaycastResult = (m_UIInputModule != null || FindUIInputModule()) ? m_UIInputModule.PerformRaycast(ref m_UIPointerEventData, screenPosition) : default;

            if (raycastHitCount > 0)
            {
                SortingHelpers.SortRaycastHitsByDistance(m_RaycastHits, raycastHitCount);
                var raycastHit = m_RaycastHits[0];

                if (!uiRaycastResult.isValid || (raycastHit.distance < uiRaycastResult.distance))
                {
                    hitPosition = raycastHit.point;
                    hitType = HitType.Object;
                    m_PreviousRaycastHitDistance = raycastHit.distance;
                    return true;
                }
            }

            if (uiRaycastResult.isValid)
            {
                // Screen-space UI hits will have a distance of 0
                hitPosition = uiRaycastResult.worldPosition;
                if (uiRaycastResult.distance > 0f)
                {
                    hitType = HitType.WorldUI;
                    m_PreviousRaycastHitDistance = uiRaycastResult.distance;
                    return true;
                }

                hitType = HitType.ScreenUI;
                return false;
            }

            hitPosition = default;
            hitType = HitType.None;
            return false;
        }

        bool FindUIInputModule()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem != null && eventSystem.currentInputModule != null)
            {
                m_UIInputModule = eventSystem.currentInputModule as XRUIInputModule;
            }
            return m_UIInputModule != null;
        }

        bool FindLeftRightControllers()
        {
            if (ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out var inputModalityManager))
            {
                if (m_LeftControllerTransform == null && inputModalityManager.leftController != null)
                {
                    var trackedPoseDriver = inputModalityManager.leftController.transform.GetComponentInChildren<TrackedPoseDriver>();
                    if (trackedPoseDriver != null)
                        m_LeftControllerTransform = trackedPoseDriver.transform;
                }

                if (m_RightControllerTransform == null && inputModalityManager.rightController != null)
                {
                    var trackedPoseDriver = inputModalityManager.rightController.transform.GetComponentInChildren<TrackedPoseDriver>();
                    if (trackedPoseDriver != null)
                        m_RightControllerTransform = trackedPoseDriver.transform;
                }
            }

            if (m_LeftControllerTransform == null)
                m_LeftControllerTransform = m_CameraTransform;

            if (m_RightControllerTransform == null)
                m_RightControllerTransform = m_CameraTransform;

            return m_LeftControllerTransform != null || m_RightControllerTransform != null;
        }

        void DisableInputModuleInput()
        {
            // Skip capturing and setting values if already disabled.
            if ((m_UIInputModule != null || FindUIInputModule()) && (m_UIInputModule.enableMouseInput || m_UIInputModule.enableTouchInput))
            {
                if (!m_OriginalInputModuleValuesCaptured)
                {
                    Debug.Log("To avoid interfering with point and click interactions, XRUIInputModule mouse and touch input will be disabled while the simulated controllers are active.", this);

                    m_OriginalInputModuleMouseValue = m_UIInputModule.enableMouseInput;
                    m_OriginalInputModuleTouchValue = m_UIInputModule.enableTouchInput;
                    m_OriginalInputModuleValuesCaptured = true;
                }

                m_UIInputModule.enableMouseInput = false;
                m_UIInputModule.enableTouchInput = false;
            }
        }

        void RestoreInputModuleInput()
        {
            if (m_OriginalInputModuleValuesCaptured && (m_UIInputModule != null || FindUIInputModule()))
            {
                m_UIInputModule.enableMouseInput = m_OriginalInputModuleMouseValue;
                m_UIInputModule.enableTouchInput = m_OriginalInputModuleTouchValue;
                m_OriginalInputModuleValuesCaptured = false;
            }
        }

        void HandlePointAndClickActive()
        {
            if (m_UsePointAndClick && m_CanUsePointAndClick)
            {
                var manipulatingController = manipulatingLeftController || manipulatingRightController;
                if (!m_PointAndClickActive && manipulatingController)
                {
                    m_PointAndClickActive = true;
                    DisableInputModuleInput();
                }
                else if (m_PointAndClickActive && (!manipulatingController || !usePointAndClick || !m_CanUsePointAndClick))
                {
                    m_PointAndClickActive = false;
                    RestoreInputModuleInput();
                }
            }

            //This check covers the case in which the UIInputModule was not available in the first call in this method due to the execution order.
            if (m_PointAndClickActive)
                DisableInputModuleInput();
        }
    }
}
