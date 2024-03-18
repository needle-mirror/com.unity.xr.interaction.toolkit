using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// An adapter component that provides a <see cref="bool"/> and <see cref="float"/> value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputButtonReader"/> as its object reference.
    /// </summary>
    [AddComponentMenu("XR/Input/XR Input Device Button Reader", 11)]
    [HelpURL(XRHelpURLConstants.k_XRInputDeviceButtonReader)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
    public sealed class XRInputDeviceButtonReader : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField]
        [Tooltip("The value that is read to determine whether the button is down.")]
        XRInputDeviceBoolValueReader m_BoolValueReader;

        /// <summary>
        /// The value that is read to determine whether the button is down.
        /// </summary>
        public XRInputDeviceBoolValueReader boolValueReader
        {
            get => m_BoolValueReader;
            set => m_BoolValueReader = value;
        }

        [SerializeField]
        [Tooltip("The value that is read to determine the scalar value that varies from 0 to 1.")]
        XRInputDeviceFloatValueReader m_FloatValueReader;

        /// <summary>
        /// The value that is read to determine the scalar value that varies from 0 to 1.
        /// </summary>
        public XRInputDeviceFloatValueReader floatValueReader
        {
            get => m_FloatValueReader;
            set => m_FloatValueReader = value;
        }

        bool m_IsPerformed;
        bool m_WasPerformedThisFrame;
        bool m_WasCompletedThisFrame;

        readonly UnityObjectReferenceCache<XRInputDeviceBoolValueReader> m_BoolValueReaderCache = new UnityObjectReferenceCache<XRInputDeviceBoolValueReader>();
        readonly UnityObjectReferenceCache<XRInputDeviceFloatValueReader> m_FloatValueReaderCache = new UnityObjectReferenceCache<XRInputDeviceFloatValueReader>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            if (m_BoolValueReader == null)
                Debug.LogError("No bool value reader set for XRInputDeviceButtonReader.", this);

            if (m_FloatValueReader == null)
                Debug.LogError("No float value reader set for XRInputDeviceButtonReader.", this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            var prevPerformed = m_IsPerformed;
            m_IsPerformed = TryGetBoolValueReader(out var reference) && reference.ReadValue();
            m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;
            m_WasCompletedThisFrame = prevPerformed && !m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadIsPerformed()
        {
            return m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadWasPerformedThisFrame()
        {
            return m_WasPerformedThisFrame;
        }

        /// <inheritdoc />
        public bool ReadWasCompletedThisFrame()
        {
            return m_WasCompletedThisFrame;
        }

        /// <inheritdoc />
        public float ReadValue()
        {
            if (TryGetFloatValueReader(out var reference))
                return reference.ReadValue();

            return default;
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            if (TryGetFloatValueReader(out var reference))
                return reference.TryReadValue(out value);

            value = default;
            return false;
        }

        bool TryGetBoolValueReader(out XRInputDeviceBoolValueReader reference) =>
            m_BoolValueReaderCache.TryGet(m_BoolValueReader, out reference);

        bool TryGetFloatValueReader(out XRInputDeviceFloatValueReader reference) =>
            m_FloatValueReaderCache.TryGet(m_FloatValueReader, out reference);
    }
}
