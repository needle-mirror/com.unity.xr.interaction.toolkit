namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="Quaternion"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceQuaternionValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceQuaternionValueReader", menuName = "XR/Input Value Reader/Quaternion")]
    public class XRInputDeviceQuaternionValueReader : XRInputDeviceValueReader<Quaternion>
    {
        /// <inheritdoc />
        public override Quaternion ReadValue() => ReadQuaternionValue();

        /// <inheritdoc />
        public override bool TryReadValue(out Quaternion value) => TryReadQuaternionValue(out value);
    }
}
