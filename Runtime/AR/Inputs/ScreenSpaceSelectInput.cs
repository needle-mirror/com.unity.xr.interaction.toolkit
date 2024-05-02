using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
    /// <summary>
    /// Component that provides the select input for the ray interactor when using touchscreen with mobile AR.
    /// Intended to be used with an <see cref="XRInputButtonReader"/> as its object reference
    /// to provide the select input.
    /// </summary>
    /// <seealso cref="XRBaseInputInteractor.selectInput"/>
    [AddComponentMenu("XR/Input/Screen Space Select Input", 11)]
    [HelpURL(XRHelpURLConstants.k_ScreenSpaceSelectInput)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_ScreenSpaceSelectInput)]
    public class ScreenSpaceSelectInput : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField]
        XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

        /// <summary>
        /// Input to use for the screen tap start position.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.tapStartPosition"/>
        public XRInputValueReader<Vector2> tapStartPositionInput
        {
            get => m_TapStartPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_DragCurrentPositionInput = new XRInputValueReader<Vector2>("Drag Current Position");

        /// <summary>
        /// Input to use for the screen drag current position.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.dragCurrentPosition"/>
        public XRInputValueReader<Vector2> dragCurrentPositionInput
        {
            get => m_DragCurrentPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_DragCurrentPositionInput, value, this);
        }

        [SerializeField]
        XRInputValueReader<float> m_PinchGapDeltaInput = new XRInputValueReader<float>("Pinch Gap Delta");

        /// <summary>
        /// Input to use for the screen pinch gap delta relative to the previous frame.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.pinchGapDelta"/>
        public XRInputValueReader<float> pinchGapDeltaInput
        {
            get => m_PinchGapDeltaInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_PinchGapDeltaInput, value, this);
        }

        [SerializeField]
        XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation");

        /// <summary>
        /// Input to use for the screen twist delta relative to the previous frame.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.twistDeltaRotation"/>
        public XRInputValueReader<float> twistDeltaRotationInput
        {
            get => m_TwistDeltaRotationInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TwistDeltaRotationInput, value, this);
        }

        bool m_IsPerformed;
        bool m_WasPerformedThisFrame;
        bool m_WasCompletedThisFrame;

        Vector2 m_TapStartPosition;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            var prevPerformed = m_IsPerformed;

            var prevTapStartPosition = m_TapStartPosition;
            var tapPerformedThisFrame = m_TapStartPositionInput.TryReadValue(out m_TapStartPosition) && prevTapStartPosition != m_TapStartPosition;

            m_IsPerformed = m_PinchGapDeltaInput.TryReadValue(out _) ||
                m_TwistDeltaRotationInput.TryReadValue(out _) ||
                m_DragCurrentPositionInput.TryReadValue(out _) ||
                tapPerformedThisFrame;
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
            return m_IsPerformed ? 1f : 0f;
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            value = m_IsPerformed ? 1f : 0f;
            return m_IsPerformed;
        }
    }
}
