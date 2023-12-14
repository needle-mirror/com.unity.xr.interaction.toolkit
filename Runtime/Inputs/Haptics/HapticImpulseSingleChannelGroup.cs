namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Haptic impulse channel group that wraps a single channel instance.
    /// This class provides a convenient way to create a channel group with a single channel known at time of construction.
    /// </summary>
    public class HapticImpulseSingleChannelGroup : IXRHapticImpulseChannelGroup
    {
        /// <inheritdoc />
        public int channelCount => 1;

        /// <summary>
        /// The single haptic impulse channel the group contains.
        /// </summary>
        public IXRHapticImpulseChannel impulseChannel { get; }

        /// <summary>
        /// Initializes and returns an instance of <see cref="HapticImpulseSingleChannelGroup"/>.
        /// </summary>
        /// <param name="channel">The single haptic impulse channel the group contains.</param>
        public HapticImpulseSingleChannelGroup(IXRHapticImpulseChannel channel)
        {
            impulseChannel = channel;
        }

        /// <inheritdoc />
        public IXRHapticImpulseChannel GetChannel(int channel = 0)
        {
            if (channel < 0)
            {
                Debug.LogError("Haptic channel can't be negative.");
                return null;
            }

            return impulseChannel;
        }
    }
}
