namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Allows for sending haptic impulses to a channel on a device from the XR input subsystem.
    /// </summary>
    /// <seealso cref="IXRHapticImpulseChannel"/>
    /// <seealso cref="InputDevice.SendHapticImpulse"/>
    public class XRInputDeviceHapticImpulseChannel : IXRHapticImpulseChannel
    {
        /// <summary>
        /// The channel to receive the impulse.
        /// </summary>
        public int motorChannel { get; set; }

        /// <summary>
        /// The input device to send the impulse to.
        /// </summary>
        public InputDevice device { get; set; }

        /// <inheritdoc />
        public bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
            // InputDevice does not support sending frequency.
            return device.SendHapticImpulse((uint)motorChannel, amplitude, duration);
        }
    }
}
