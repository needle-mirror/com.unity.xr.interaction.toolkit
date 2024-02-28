// ENABLE_VR is not defined on Game Core but the assembly is available with limited features when the XR module is enabled.
// These are the guards that Input System uses to define the SendHapticImpulseCommand class.
#if ENABLE_VR || UNITY_GAMECORE
#define INPUT_HAPTICS_AVAILABLE
#endif

#if INPUT_HAPTICS_AVAILABLE
using UnityEngine.InputSystem.XR.Haptics;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Allows for sending haptic impulses to a channel on a device from the input system.
    /// </summary>
    /// <seealso cref="IXRHapticImpulseChannel"/>
    /// <seealso cref="SendHapticImpulseCommand"/>
    public class HapticImpulseCommandChannel : IXRHapticImpulseChannel
    {
        /// <summary>
        /// The channel to receive the impulse.
        /// </summary>
        public int motorChannel { get; set; }

        /// <summary>
        /// The input device to send the impulse to.
        /// </summary>
        public InputSystem.InputDevice device { get; set; }

        /// <inheritdoc />
        public bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
#if INPUT_HAPTICS_AVAILABLE
            if (device == null)
                return false;

            // The command does not yet support specifying the frequency.
            var command = SendHapticImpulseCommand.Create(motorChannel, amplitude, duration);
            return device.ExecuteCommand(ref command) >= 0L;
#else
            return false;
#endif
        }
    }
}
