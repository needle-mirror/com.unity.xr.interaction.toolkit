// ENABLE_VR is not defined on Game Core but the assembly is available with limited features when the XR module is enabled.
// These are the guards that Input System uses in GenericXRDevice.cs to define the XRController and XRHMD classes.
#if ENABLE_VR || UNITY_GAMECORE
#define XR_INPUT_DEVICES_AVAILABLE
#endif

using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Haptics utilities.
    /// Provides methods for a convenient way to send a haptic impulse to the left, right, or both controllers
    /// using a <see langword="static"/> method call.
    /// </summary>
    /// <remarks>
    /// If you need more control over how the haptic impulse is sent, you should use the <see cref="HapticImpulsePlayer"/>
    /// with an object reference to a custom <see cref="IXRHapticImpulseProvider"/>.
    /// </remarks>
    public static class HapticsUtility
    {
        /// <summary>
        /// Identify which controllers to send a haptic impulse to.
        /// </summary>
        public enum Controller
        {
            /// <summary>
            /// Left controller.
            /// </summary>
            Left,

            /// <summary>
            /// Right controller.
            /// </summary>
            Right,

            /// <summary>
            /// Both left and right controllers.
            /// </summary>
            Both,
        }

        static HapticImpulseCommandChannelGroup s_LeftChannelGroup;
        static HapticImpulseCommandChannelGroup s_RightChannelGroup;

        static XRInputDeviceHapticImpulseChannelGroup s_LegacyLeftChannelGroup;
        static XRInputDeviceHapticImpulseChannelGroup s_LegacyRightChannelGroup;
        static InputDevice s_LegacyLeftDevice;
        static InputDevice s_LegacyRightDevice;

        static HapticControlActionManager s_HapticControlManager;
        static InputAction s_LeftHapticAction;
        static InputAction s_RightHapticAction;

        /// <summary>
        /// Sends a haptic impulse on the controller device if it supports sending a haptic impulse.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <param name="controller">The controller to send the haptic impulse to.</param>
        /// <param name="frequency">The desired frequency of the impulse in Hz. A value of 0 means to use the default frequency of the device.</param>
        /// <param name="channel">The haptic channel of the device to send to, by index.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        public static bool SendHapticImpulse(float amplitude, float duration, Controller controller, float frequency = 0f, int channel = 0)
        {
            var leftSuccess = false;
            var rightSuccess = false;

            if (controller == Controller.Left || controller == Controller.Both)
            {
#if XR_INPUT_DEVICES_AVAILABLE
                var device = InputSystem.XR.XRController.leftHand;
#else
                InputSystem.InputDevice device = null;
#endif
                if (device != null)
                {
                    // Currently, the only code path that we can send a specific frequency is through OpenXR.
                    // However the only fixed HapticControl path common to all profiles is the "{Haptic}" usage, which is the default 0 channel.
                    // Quest Pro also has {HapticTrigger} (channel 1) and {HapticThumb} (channel 2), but these are not common to all profiles,
                    // so we can't make assumptions about the binding path to use for other channels.
                    if (frequency > 0f && channel == 0)
                        leftSuccess = SendHapticImpulseOpenXR(GetLeftHapticAction(), amplitude, duration, frequency);

                    if (!leftSuccess)
                        leftSuccess = SendHapticImpulse(ref s_LeftChannelGroup, device, channel, amplitude, duration, frequency);
                }
                else
                {
                    leftSuccess = SendHapticImpulseLegacy(ref s_LegacyLeftChannelGroup, ref s_LegacyLeftDevice,
                        XRInputTrackingAggregator.Characteristics.leftController, channel, amplitude, duration, frequency);
                }
            }

            if (controller == Controller.Right || controller == Controller.Both)
            {
#if XR_INPUT_DEVICES_AVAILABLE
                var device = InputSystem.XR.XRController.rightHand;
#else
                InputSystem.InputDevice device = null;
#endif
                if (device != null)
                {
                    if (frequency > 0f && channel == 0)
                        rightSuccess = SendHapticImpulseOpenXR(GetRightHapticAction(), amplitude, duration, frequency);

                    if (!rightSuccess)
                        rightSuccess = SendHapticImpulse(ref s_RightChannelGroup, device, channel, amplitude, duration, frequency);
                }
                else
                {
                    rightSuccess = SendHapticImpulseLegacy(ref s_LegacyRightChannelGroup, ref s_LegacyRightDevice,
                        XRInputTrackingAggregator.Characteristics.rightController, channel, amplitude, duration, frequency);
                }
            }

            if (controller == Controller.Both)
                return leftSuccess && rightSuccess;

            if (controller == Controller.Left)
                return leftSuccess;

            if (controller == Controller.Right)
                return rightSuccess;

            return false;
        }

        static bool SendHapticImpulseOpenXR(InputAction hapticAction, float amplitude, float duration, float frequency)
        {
            s_HapticControlManager ??= new HapticControlActionManager();
            return s_HapticControlManager.GetChannelGroup(hapticAction)?.GetChannel().SendHapticImpulse(amplitude, duration, frequency) ?? false;
        }

        static bool SendHapticImpulse(ref HapticImpulseCommandChannelGroup channelGroup, InputSystem.InputDevice device,
            int channel, float amplitude, float duration, float frequency)
        {
            channelGroup ??= new HapticImpulseCommandChannelGroup();
            channelGroup.Initialize(device);
            return channelGroup.GetChannel(channel)?.SendHapticImpulse(amplitude, duration, frequency) ?? false;
        }

        static bool SendHapticImpulseLegacy(ref XRInputDeviceHapticImpulseChannelGroup channelGroup, ref InputDevice device,
            InputDeviceCharacteristics characteristics, int channel, float amplitude, float duration, float frequency)
        {
            channelGroup ??= new XRInputDeviceHapticImpulseChannelGroup();
            if (device.isValid || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(characteristics, out device))
            {
                channelGroup.Initialize(device);
                return channelGroup.GetChannel(channel)?.SendHapticImpulse(amplitude, duration, frequency) ?? false;
            }

            return false;
        }

        static InputAction GetLeftHapticAction()
        {
            if (s_LeftHapticAction == null)
            {
                s_LeftHapticAction = new InputAction("Left Haptic", type: InputActionType.PassThrough, "<XRController>{LeftHand}/{Haptic}");
                s_LeftHapticAction.Enable();
            }

            return s_LeftHapticAction;
        }

        static InputAction GetRightHapticAction()
        {
            if (s_RightHapticAction == null)
            {
                s_RightHapticAction = new InputAction("Right Haptic", type: InputActionType.PassThrough, "<XRController>{RightHand}/{Haptic}");
                s_RightHapticAction.Enable();
            }

            return s_RightHapticAction;
        }
    }
}
