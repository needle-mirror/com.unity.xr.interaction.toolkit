namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// State class for the <see cref="XRInteractionSimulator"/>.
    /// </summary>
    /// <remarks>
    /// Settable properties are assigned each frame in <see cref="XRInteractionSimulator"/>.
    /// </remarks>
    /// <seealso cref="XRInteractionSimulator.currentState"/>
    /// <seealso cref="XRInteractionSimulator.previousState"/>
    public class XRInteractionSimulatorState
    {
        /// <summary>
        /// Whether the simulator is in controller mode or tracked hand mode.
        /// </summary>
        public SimulatedDeviceLifecycleManager.DeviceMode deviceMode { get; internal set; }

        /// <summary>
        /// The currently active/targeted devices in the interaction simulator.
        /// </summary>
        public TargetedDevices targetedDeviceInput { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating right.
        /// </summary>
        public bool isTranslatingRight { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating left.
        /// </summary>
        public bool isTranslatingLeft { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating up.
        /// </summary>
        public bool isTranslatingUp { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating down.
        /// </summary>
        public bool isTranslatingDown { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating forward.
        /// </summary>
        public bool isTranslatingForward { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is translating backward.
        /// </summary>
        public bool isTranslatingBackward { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is rotating right.
        /// </summary>
        public bool isRotatingRight { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is rotating left.
        /// </summary>
        public bool isRotatingLeft { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is rotating up.
        /// </summary>
        public bool isRotatingUp { get; internal set; }

        /// <summary>
        /// Whether or not the active device in the simulator is rotating down.
        /// </summary>
        public bool isRotatingDown { get; internal set; }

        /// <summary>
        /// Whether the simulator is currently performing a quick-action on the left-handed device.
        /// </summary>
        public bool performingLeftQuickAction { get; internal set; }

        /// <summary>
        /// Whether the simulator is currently performing a quick-action on the right-handed device.
        /// </summary>
        public bool performingRightQuickAction { get; internal set; }

        /// <summary>
        /// Whether the modifier for left-handed hotkeys is actively pressed.
        /// </summary>
        public bool leftDeviceHotkeyModifierPressed { get; internal set; }

        /// <summary>
        /// The left controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode leftControllerInputMode { get; internal set; }

        /// <summary>
        /// The right controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode rightControllerInputMode { get; internal set; }

        /// <summary>
        /// The currently active hotkey buttons for the controller device.
        /// </summary>
        public HeldHotkeyButtons activeControllerHotkeyButtons { get; internal set; }

        /// <summary>
        /// The left hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression leftHandExpression { get; internal set; }

        /// <summary>
        /// The right hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression rightHandExpression { get; internal set; }

        /// <summary>
        /// Whether a hand expression toggle input is currently held.
        /// </summary>
        /// <seealso cref="SimulatedHandExpression.toggleInput"/>
        public bool handExpressionToggleHeld { get; internal set; }

        /// <summary>
        /// Whether the simulator is manipulating the Left device (controller or hand).
        /// </summary>
        public bool manipulatingLeftDevice => targetedDeviceInput.HasDevice(TargetedDevices.LeftDevice);

        /// <summary>
        /// Whether the simulator is manipulating the Right device (controller or hand).
        /// </summary>
        public bool manipulatingRightDevice => targetedDeviceInput.HasDevice(TargetedDevices.RightDevice);

        /// <summary>
        /// Whether the simulator is manipulating the Left Controller.
        /// </summary>
        public bool manipulatingLeftController => deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && manipulatingLeftDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Right Controller.
        /// </summary>
        public bool manipulatingRightController => deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && manipulatingRightDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Left Hand.
        /// </summary>
        public bool manipulatingLeftHand => deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && manipulatingLeftDevice;

        /// <summary>
        /// Whether the simulator is manipulating the Right Hand.
        /// </summary>
        public bool manipulatingRightHand => deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && manipulatingRightDevice;

        /// <summary>
        /// Whether the simulator is manipulating the HMD.
        /// </summary>
        public bool manipulatingHMD => targetedDeviceInput == TargetedDevices.HMD;

        /// <summary>
        /// Whether the simulator is manipulating the HMD, Left Controller, and Right Controller as if the whole player was turning their torso,
        /// similar to a typical FPS style.
        /// </summary>
        public bool manipulatingFPS => targetedDeviceInput.HasDevice(TargetedDevices.FPS);

        /// <summary>
        /// The controller input mode which the controller should currently simulate.
        /// </summary>
        public ControllerInputMode currentControllerInputMode => manipulatingLeftController ? leftControllerInputMode : rightControllerInputMode;

        /// <summary>
        /// The hand expression which the simulated hands should currently simulate.
        /// </summary>
        public SimulatedHandExpression currentHandExpression => manipulatingLeftDevice ? leftHandExpression : rightHandExpression;
    }
}
