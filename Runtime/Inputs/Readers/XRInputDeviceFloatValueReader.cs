namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="float"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// or as part of an <see cref="XRInputDeviceButtonReader"/>.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceFloatValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceFloatValueReader", menuName = "XR/Input Value Reader/float")]
    public class XRInputDeviceFloatValueReader : XRInputDeviceValueReader<float>
    {
        /// <inheritdoc />
        public override float ReadValue() => ReadFloatValue();

        /// <inheritdoc />
        public override bool TryReadValue(out float value) => TryReadFloatValue(out value);
    }
}
