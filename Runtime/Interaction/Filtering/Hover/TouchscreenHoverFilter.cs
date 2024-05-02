#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION
using UnityEngine.XR.Interaction.Toolkit.AR.Inputs;
#endif
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
    /// <summary>
    /// Hover filter that checks if the screen is being touched and doing a selecting gesture.
    /// Can be used with the ray interactor to prevent hover interactions when the screen is not being touched.
    /// </summary>
    [AddComponentMenu("XR/AR/Touchscreen Hover Filter", 11)]
    [HelpURL(XRHelpURLConstants.k_TouchscreenHoverFilter)]
    public class TouchscreenHoverFilter : MonoBehaviour, IXRHoverFilter
    {
        [SerializeField]
        XRInputValueReader<int> m_ScreenTouchCountInput = new XRInputValueReader<int>("Screen Touch Count");

        /// <summary>
        /// The input used to read the screen touch count value.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.fingerCount"/>
        public XRInputValueReader<int> screenTouchCountInput
        {
            get => m_ScreenTouchCountInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ScreenTouchCountInput, value, this);
        }

        /// <inheritdoc />
        public bool canProcess => isActiveAndEnabled;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            if (interactor is XRBaseInputInteractor inputInteractor)
                return inputInteractor.selectInput.ReadIsPerformed() && m_ScreenTouchCountInput.ReadValue() <= 1;

            return m_ScreenTouchCountInput.ReadValue() > 0;
        }
    }
}
