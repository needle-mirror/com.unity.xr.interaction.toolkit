namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="bool"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// or as part of an <see cref="XRInputDeviceButtonReader"/>.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceBoolValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceBoolValueReader", menuName = "XR/Input Value Reader/bool")]
    public class XRInputDeviceBoolValueReader : XRInputDeviceValueReader<bool>
    {
        /// <inheritdoc />
        public override bool ReadValue() => ReadBoolValue();

        /// <inheritdoc />
        public override bool TryReadValue(out bool value) => TryReadBoolValue(out value);
    }
}
