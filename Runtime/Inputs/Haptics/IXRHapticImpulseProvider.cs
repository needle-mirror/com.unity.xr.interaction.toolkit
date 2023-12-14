namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Interface which provides a group of haptic impulse channels.
    /// </summary>
    public interface IXRHapticImpulseProvider
    {
        /// <summary>
        /// Gets the group of haptic impulse channels.
        /// </summary>
        /// <returns>Returns the haptic impulse channel group, which may be <see langword="null"/>.</returns>
        IXRHapticImpulseChannelGroup GetChannelGroup();
    }
}
