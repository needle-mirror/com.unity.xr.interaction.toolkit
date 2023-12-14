using UnityEngine.InputSystem.XR.Haptics;

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
            if (device == null)
                return false;

            // The command does not yet support specifying the frequency.
            var command = SendHapticImpulseCommand.Create(motorChannel, amplitude, duration);
            return device.ExecuteCommand(ref command) >= 0L;
        }
    }
}
