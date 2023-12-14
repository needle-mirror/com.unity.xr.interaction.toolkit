namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a group of haptic impulse channels for a device
    /// from the XR input subsystem as defined by its characteristics. Intended to be used with
    /// an <see cref="XRInputHapticImpulseProvider"/> as its object reference.
    /// </summary>
    /// <seealso cref="XRInputHapticImpulseProvider"/>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceHapticImpulseProvider)]
    [CreateAssetMenu(fileName = "XRInputDeviceHapticImpulseProvider", menuName = "XR/Input Device Haptic Impulse Provider")]
    public class XRInputDeviceHapticImpulseProvider : ScriptableObject, IXRHapticImpulseProvider
    {
        [SerializeField]
        InputDeviceCharacteristics m_Characteristics;

        XRInputDeviceHapticImpulseChannelGroup m_ChannelGroup;

        InputDevice m_InputDevice;

        /// <inheritdoc />
        public IXRHapticImpulseChannelGroup GetChannelGroup()
        {
            RefreshInputDeviceIfNeeded();
            m_ChannelGroup ??= new XRInputDeviceHapticImpulseChannelGroup();
            m_ChannelGroup.Initialize(m_InputDevice);

            return m_ChannelGroup;
        }

        void RefreshInputDeviceIfNeeded()
        {
            if (!m_InputDevice.isValid)
                XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(m_Characteristics, out m_InputDevice);
        }
    }
}
