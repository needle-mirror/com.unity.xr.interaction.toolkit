using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Send haptic impulses to a device.
    /// Use this component to manage the connection to haptic hardware. It acts as a bridge, allowing other components,
    /// such as <see cref="SimpleHapticFeedback"/>, to trigger vibrations on a specific device.
    /// </summary>
    [AddComponentMenu("XR/Haptics/Haptic Impulse Player", 11)]
    [HelpURL(XRHelpURLConstants.k_HapticImpulsePlayer)]
    public class HapticImpulsePlayer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Specifies the output haptic control or controller that haptic impulses are sent to.")]
        XRInputHapticImpulseProvider m_HapticOutput = new XRInputHapticImpulseProvider("Haptic");

        /// <summary>
        /// Specifies the output haptic control or controller that haptic impulses are sent to.
        /// </summary>
        public XRInputHapticImpulseProvider hapticOutput
        {
            get => m_HapticOutput;
            set => XRInputReaderUtility.SetInputProperty(ref m_HapticOutput, value, this);
        }

        [SerializeField, Range(0f, 1f)]
        [Tooltip("Sets a multiplier that dampens all haptic impulses this component sends. A value of 1 maintains the current amplitude.")]
        float m_AmplitudeMultiplier = 1f;

        /// <summary>
        /// Sets a multiplier that dampens all haptic impulses this component sends. A value of 1 maintains the current amplitude.
        /// </summary>
        public float amplitudeMultiplier
        {
            get => m_AmplitudeMultiplier;
            set => m_AmplitudeMultiplier = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_HapticOutput is { inputSourceMode: XRInputHapticImpulseProvider.InputSourceMode.InputActionReference } &&
                m_HapticOutput.inputActionReference == null)
            {
                var impulseProvider = gameObject.GetComponentInParent<IXRHapticImpulseProvider>(true);
                if (impulseProvider as Component != null)
                {
                    m_HapticOutput.SetObjectReference(impulseProvider);
                    m_HapticOutput.inputSourceMode = XRInputHapticImpulseProvider.InputSourceMode.ObjectReference;
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_HapticOutput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_HapticOutput.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// Sends a haptic impulse on the configured channel or default channel of the configured device.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// This method considers sending the haptic impulse a success (and thus returns <see langword="true"/>)
        /// if the haptic impulse was successfully sent to the device even if frequency is ignored or not supported by the device.
        /// <br />
        /// Uses the default frequency of the device.
        /// </remarks>
        public bool SendHapticImpulse(float amplitude, float duration)
            => SendHapticImpulse(amplitude, duration, 0f);

        /// <summary>
        /// Sends a haptic impulse on the configured channel or default channel of the configured device.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <param name="frequency">The desired frequency of the impulse in Hz. A value of 0 means to use the default frequency of the device.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// This method considers sending the haptic impulse a success (and thus returns <see langword="true"/>)
        /// if the haptic impulse was successfully sent to the device even if frequency is ignored or not supported by the device.
        /// <br />
        /// Frequency is currently only functional when the OpenXR Plugin (com.unity.xr.openxr) package is installed
        /// and the input action is using an input binding to a Haptic Control.
        /// </remarks>
        public bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
            if (!isActiveAndEnabled)
                return false;

            return m_HapticOutput.GetChannelGroup()?.GetChannel()?.SendHapticImpulse(amplitude * m_AmplitudeMultiplier, duration, frequency) ?? false;
        }

        internal static HapticImpulsePlayer GetOrCreateInHierarchy(GameObject gameObject)
        {
            var hapticImpulsePlayer = gameObject.GetComponentInParent<HapticImpulsePlayer>(true);
            if (hapticImpulsePlayer == null)
            {
                // Try to add the component in the hierarchy where it can be found and shared by other interactors.
                // Otherwise, just add it to this GameObject.
                var impulseProvider = gameObject.GetComponentInParent<IXRHapticImpulseProvider>(true);
                var impulseProviderComponent = impulseProvider as Component;
                hapticImpulsePlayer = impulseProviderComponent != null
                    ? impulseProviderComponent.gameObject.AddComponent<HapticImpulsePlayer>()
                    : gameObject.AddComponent<HapticImpulsePlayer>();
            }

            return hapticImpulsePlayer;
        }
    }
}
