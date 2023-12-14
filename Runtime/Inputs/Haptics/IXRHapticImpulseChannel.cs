namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// An interface that allows for sending haptic impulses to a channel on a device.
    /// </summary>
    /// <seealso cref="HapticImpulseCommandChannel"/>
    /// <seealso cref="IXRHapticImpulseChannelGroup"/>
    public interface IXRHapticImpulseChannel
    {
        /// <summary>
        /// Sends a haptic impulse on the device if it supports sending a haptic impulse.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <param name="frequency">The desired frequency of the impulse in Hz. A value of 0 means to use the default frequency of the device.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// This method considers sending the haptic impulse a success (and thus returns <see langword="true"/>)
        /// if the haptic impulse was successfully sent to the device even if frequency is ignored or not supported by the device.
        /// <br />
        /// Frequency is currently only functional when the OpenXR Plugin (com.unity.xr.openxr) package is installed
        /// and the input action is using an input binding to a Haptic Control.
        /// </remarks>
        bool SendHapticImpulse(float amplitude, float duration, float frequency = 0f);
    }
}
