using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
    /// <summary>
    /// Takes a single press, such as a hand tracking microgesture swipe or a button press, and converts it into one UI scroll
    /// of <see cref="scrollAmount"/> in the pressed direction. Holding a press does not repeat the scroll.
    /// </summary>
    /// <remarks>
    /// <para>Assign this component to an interactor's UI scroll input using <see cref="XRInputValueReader.InputSourceMode.ObjectReference"/>
    /// mode, and enable <see cref="NearFarInteractor.uiScrollMomentum"/> so each press keeps scrolling with momentum until it stops on the scroll view
    /// it was aimed at. Use one instance per hand. The Hands Interaction Demo sample's XR Origin is set up this way.</para>
    /// <para>It also works without momentum, but a uGUI <c>ScrollRect</c> then jumps the full distance in a single frame, and a UI Toolkit <c>ScrollView</c>
    /// scrolls much farther than <see cref="scrollAmount"/> because its own wheel step applies.</para>
    /// </remarks>
    /// <seealso cref="NearFarInteractor.uiScrollInput"/>
    /// <seealso cref="NearFarInteractor.uiScrollMomentum"/>
    [AddComponentMenu("XR/Input/Press Scroll Input", 11)]
    [HelpURL(XRHelpURLConstants.k_PressScrollInput)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_PressScrollInput)]
    public class PressScrollInput : MonoBehaviour, IXRInputValueReader<Vector2>
    {
        [SerializeField]
        [Tooltip("The directional input whose presses are converted into scrolls, such as a 2DVector composite of microgesture swipes or buttons. Each new press scrolls once in its direction, and the value is normalized so every press scrolls the full Scroll Amount.")]
        XRInputValueReader<Vector2> m_ScrollInput = new XRInputValueReader<Vector2>("Scroll");

        /// <summary>
        /// The directional input whose presses are converted into scrolls, such as a <c>2DVector</c> composite of
        /// microgesture swipes or buttons. Each new press scrolls once in its direction.
        /// </summary>
        /// <remarks>
        /// The input value is normalized, so only its direction is used and every press scrolls the full <see cref="scrollAmount"/>.
        /// </remarks>
        public XRInputValueReader<Vector2> scrollInput
        {
            get => m_ScrollInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ScrollInput, value, this);
        }

        [SerializeField]
        [Tooltip("The distance each press scrolls, in pixels, on a Scroll Rect with default scroll sensitivity, or on a UI Toolkit Scroll View when the interactor uses UI Scroll Momentum.")]
        float m_ScrollAmount = 300f;

        /// <summary>
        /// The distance each press scrolls, in pixels, on a scroll view with default scroll sensitivity.
        /// </summary>
        /// <remarks>
        /// Each scroll view scales this distance: a uGUI <c>ScrollRect</c> by its <c>scrollSensitivity</c>, and a UI Toolkit
        /// <c>ScrollView</c> by its <c>mouseWheelScrollSize</c> relative to the default. On UI Toolkit the distance is only
        /// exact when the interactor uses <see cref="NearFarInteractor.uiScrollMomentum"/>; without it, the scroll view's own
        /// wheel step applies.
        /// </remarks>
        public float scrollAmount
        {
            get => m_ScrollAmount;
            set => m_ScrollAmount = value;
        }

        Vector2 m_ScrollDelta;
        bool m_WasPressed;

        /// <inheritdoc />
        public Vector2 ReadValue()
        {
            return m_ScrollDelta;
        }

        /// <inheritdoc />
        public bool TryReadValue(out Vector2 value)
        {
            value = m_ScrollDelta;
            return value != Vector2.zero;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_ScrollInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_ScrollInput.DisableDirectActionIfModeUsed();
            m_ScrollDelta = Vector2.zero;
            m_WasPressed = false;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            ProcessScroll(UIInputModule.GetActiveTrackedScrollDeltaMultiplier());
        }

        /// <summary>
        /// Converts a new press into <see cref="scrollAmount"/> in the pressed direction for the frame the press starts,
        /// and outputs zero on every other frame.
        /// </summary>
        /// <param name="trackedScrollDeltaMultiplier">The multiplier the UI input module applies to the scroll delta.</param>
        internal void ProcessScroll(float trackedScrollDeltaMultiplier)
        {
            m_ScrollInput.TryReadValue(out var direction);
            var pressed = direction != Vector2.zero;
            var newPress = pressed && !m_WasPressed;
            m_WasPressed = pressed;

            // Use its magnitude so a negative multiplier still inverts the scroll direction.
            var multiplierMagnitude = Mathf.Abs(trackedScrollDeltaMultiplier);
            if (!newPress || multiplierMagnitude <= 0f)
            {
                m_ScrollDelta = Vector2.zero;
                return;
            }

            // The UI layer multiplies scroll input by the tracked scroll delta multiplier, so divide it out to land on scrollAmount pixels.
            // Normalize so every press scrolls the full scrollAmount, whatever the input's magnitude.
            m_ScrollDelta = direction.normalized * (m_ScrollAmount / multiplierMagnitude);
        }
    }
}
