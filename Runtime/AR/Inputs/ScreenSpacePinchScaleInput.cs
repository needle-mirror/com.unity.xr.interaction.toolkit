using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
    /// <summary>
    /// Component that provides the pinch gap delta scaling value from a touchscreen for mobile AR.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// to provide a scale value.
    /// </summary>
    /// <seealso cref="XRRayInteractor.scaleDistanceDeltaInput"/>
    /// <seealso cref="Interactors.ScaleMode.DistanceDelta"/>
    [AddComponentMenu("XR/Input/Screen Space Pinch Scale Input", 11)]
    [HelpURL(XRHelpURLConstants.k_ScreenSpacePinchScaleInput)]
    public class ScreenSpacePinchScaleInput : MonoBehaviour, IXRInputValueReader<float>
    {
        [SerializeField]
        [Tooltip("Enables a rotation threshold that blocks pinch scale gestures when surpassed.")]
        bool m_UseRotationThreshold = true;

        /// <summary>
        /// Enables a rotation threshold that blocks pinch scale gestures when surpassed.
        /// </summary>
        /// <seealso cref="rotationThreshold"/>
        public bool useRotationThreshold
        {
            get => m_UseRotationThreshold;
            set => m_UseRotationThreshold = value;
        }

        [SerializeField]
        [Tooltip("The threshold at which a gestures will be interpreted only as rotation and not a pinch scale gesture.")]
        float m_RotationThreshold = 0.02f;

        /// <summary>
        /// The threshold at which a gestures will be interpreted only as rotation and not a pinch scale gesture.
        /// </summary>
        /// <seealso cref="useRotationThreshold"/>
        public float rotationThreshold
        {
            get => m_RotationThreshold;
            set => m_RotationThreshold = value;
        }

        [SerializeField]
        [Tooltip("The input used to read the pinch gap delta value.")]
        XRInputValueReader<float> m_PinchGapDeltaInput = new XRInputValueReader<float>("Pinch Gap Delta");

        /// <summary>
        /// The input used to read the pinch gap delta value.
        /// </summary>
        public XRInputValueReader<float> pinchGapDeltaInput
        {
            get => m_PinchGapDeltaInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_PinchGapDeltaInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to read the twist delta rotation value.")]
        XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation");

        /// <summary>
        /// The input used to read the twist delta rotation value.
        /// </summary>
        public XRInputValueReader<float> twistDeltaRotationInput
        {
            get => m_TwistDeltaRotationInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TwistDeltaRotationInput, value, this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_PinchGapDeltaInput.EnableDirectActionIfModeUsed();
            m_TwistDeltaRotationInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_PinchGapDeltaInput.DisableDirectActionIfModeUsed();
            m_TwistDeltaRotationInput.DisableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        public float ReadValue()
        {
            TryReadValue(out var value);
            return value;
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            if (m_UseRotationThreshold &&
                m_TwistDeltaRotationInput.TryReadValue(out var twistDeltaRotation) &&
                Mathf.Abs(twistDeltaRotation) >= m_RotationThreshold)
            {
                value = 0f;
                return true;
            }

            if (m_PinchGapDeltaInput.TryReadValue(out var pinchGapDelta))
            {
                value = pinchGapDelta / Screen.dpi;
                return true;
            }

            value = 0f;
            return false;
        }
    }
}
