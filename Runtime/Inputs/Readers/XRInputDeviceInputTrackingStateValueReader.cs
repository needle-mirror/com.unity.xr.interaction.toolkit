namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a <see cref="InputTrackingState"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceInputTrackingStateValueReader)]
    [CreateAssetMenu(fileName = "XRInputDeviceInputTrackingStateValueReader", menuName = "XR/Input Value Reader/InputTrackingState")]
    public class XRInputDeviceInputTrackingStateValueReader : XRInputDeviceValueReader<InputTrackingState>
    {
        /// <inheritdoc />
        public override InputTrackingState ReadValue() => ReadInputTrackingStateValue();

        /// <inheritdoc />
        public override bool TryReadValue(out InputTrackingState value) => TryReadInputTrackingStateValue(out value);
    }
}
