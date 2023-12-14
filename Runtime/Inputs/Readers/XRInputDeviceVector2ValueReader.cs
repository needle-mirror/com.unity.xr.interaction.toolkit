namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="Vector2"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// or as part of an <see cref="XRInputDeviceButtonReader"/>.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceVector2ValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceVector2ValueReader", menuName = "XR/Input Value Reader/Vector2")]
    public class XRInputDeviceVector2ValueReader : XRInputDeviceValueReader<Vector2>
    {
        /// <inheritdoc />
        public override Vector2 ReadValue() => ReadVector2Value();

        /// <inheritdoc />
        public override bool TryReadValue(out Vector2 value) => TryReadVector2Value(out value);
    }
}
