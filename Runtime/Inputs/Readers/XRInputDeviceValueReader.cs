namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// An abstract base <see cref="ScriptableObject"/> that provides a value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// or as part of an <see cref="XRInputDeviceButtonReader"/>. This class contains the common
    /// base fields of the typed value reader.
    /// </summary>
    /// <seealso cref="XRInputDeviceValueReader{TValue}"/>
    public abstract class XRInputDeviceValueReader : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Characteristics of the input device to read from. Controllers are either:" +
            "\nHeld In Hand, Tracked Device, Controller, Left" +
            "\nHeld In Hand, Tracked Device, Controller, Right")]
        private protected InputDeviceCharacteristics m_Characteristics;

        /// <summary>
        /// <para>Characteristics of the input device to read from. Controllers are either: </para>
        /// <para><see cref="InputDeviceCharacteristics.HeldInHand"/> | <see cref="InputDeviceCharacteristics.TrackedDevice"/> | <see cref="InputDeviceCharacteristics.Controller"/> | <see cref="InputDeviceCharacteristics.Left"/> or</para>
        /// <para><see cref="InputDeviceCharacteristics.HeldInHand"/> | <see cref="InputDeviceCharacteristics.TrackedDevice"/> | <see cref="InputDeviceCharacteristics.Controller"/> | <see cref="InputDeviceCharacteristics.Right"/>.</para>
        /// </summary>
        public InputDeviceCharacteristics characteristics
        {
            get => m_Characteristics;
            set => m_Characteristics = value;
        }
    }

    /// <summary>
    /// A <see cref="ScriptableObject"/> that provides a typed value from a device
    /// from the XR input subsystem as defined by its characteristics and feature usage string.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// or as part of an <see cref="XRInputDeviceButtonReader"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to read, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
    public abstract class XRInputDeviceValueReader<TValue> : XRInputDeviceValueReader, IXRInputValueReader<TValue> where TValue : struct
    {
        [SerializeField]
        [Tooltip("The name of the input feature to read.")]
        InputFeatureUsageString<TValue> m_Usage;

        /// <summary>
        /// The name of the input feature usage to read.
        /// </summary>
        public InputFeatureUsageString<TValue> usage
        {
            get => m_Usage;
            set => m_Usage = value;
        }

        InputDevice m_InputDevice;

        /// <inheritdoc />
        public abstract TValue ReadValue();

        /// <inheritdoc />
        public abstract bool TryReadValue(out TValue value);

        /// <summary>
        /// Read the value of the input as a <see langword="bool"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see langword="bool"/>.</returns>
        protected bool ReadBoolValue()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as an <see langword="uint"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see langword="uint"/>.</returns>
        protected uint ReadUIntValue()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<uint>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as a <see langword="float"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see langword="float"/>.</returns>
        protected float ReadFloatValue()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<float>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as a <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see cref="Vector2"/>.</returns>
        protected Vector2 ReadVector2Value()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as a <see cref="Vector3"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see cref="Vector3"/>.</returns>
        protected Vector3 ReadVector3Value()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as a <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see cref="Quaternion"/>.</returns>
        protected Quaternion ReadQuaternionValue()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Read the value of the input as an <see cref="InputTrackingState"/>.
        /// </summary>
        /// <returns>Returns the value of the input as a <see cref="InputTrackingState"/>.</returns>
        protected InputTrackingState ReadInputTrackingStateValue()
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<InputTrackingState>(m_Usage.name), out var value))
                return value;

            return default;
        }

        /// <summary>
        /// Try to read the value of the input as a <see langword="bool"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadBoolValue(out bool value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as an <see langword="uint"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadUIntValue(out uint value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<uint>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as a <see langword="float"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadFloatValue(out float value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<float>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadVector2Value(out Vector2 value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadVector3Value(out Vector3 value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadQuaternionValue(out Quaternion value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Try to read the value of the input as an <see cref="InputTrackingState"/>.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true"/>, the value read. Otherwise, the default for the type.</param>
        /// <returns>Returns <see langword="true"/> if the value was successfully read.</returns>
        protected bool TryReadInputTrackingStateValue(out InputTrackingState value)
        {
            if (RefreshInputDeviceIfNeeded() && m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<InputTrackingState>(m_Usage.name), out value))
                return true;

            value = default;
            return false;
        }

        /// <summary>
        /// Updates the found input device used to read input from if it isn't valid.
        /// This should be called before attempting to read a value from the input device.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the input device is valid or if a valid one with matching characteristics was found.</returns>
        protected bool RefreshInputDeviceIfNeeded()
        {
            return m_InputDevice.isValid || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(m_Characteristics, out m_InputDevice);
        }
    }
}
