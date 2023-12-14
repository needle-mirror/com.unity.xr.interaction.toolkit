namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="Vector3"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceVector3ValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceVector3ValueReader", menuName = "XR/Input Value Reader/Vector3")]
    public class XRInputDeviceVector3ValueReader : XRInputDeviceValueReader<Vector3>
    {
        /// <inheritdoc />
        public override Vector3 ReadValue() => ReadVector3Value();

        /// <inheritdoc />
        public override bool TryReadValue(out Vector3 value) => TryReadVector3Value(out value);
    }
}
