using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// An interface that allows for getting the haptic impulse channel(s) on a device.
    /// </summary>
    /// <seealso cref="IXRHapticImpulseChannel"/>
    public interface IXRHapticImpulseChannelGroup
    {
        /// <summary>
        /// The number of haptic channels on the device.
        /// </summary>
        int channelCount { get; }

        /// <summary>
        /// Gets the haptic channel on the device by index.
        /// </summary>
        /// <param name="channel">The haptic channel index.</param>
        /// <returns>Returns the haptic channel, or <see langword="null"/> if the haptic channel does not exist.</returns>
        IXRHapticImpulseChannel GetChannel(int channel = 0);
    }
}
