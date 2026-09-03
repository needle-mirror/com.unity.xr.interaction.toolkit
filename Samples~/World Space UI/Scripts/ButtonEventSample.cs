using UnityEngine.Events;
#if UIELEMENTS_MODULE_AVAILABLE
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.WorldSpaceUI
{
    /// <summary>
    /// Sample class that demonstrates how to bind to a UI Toolkit button click event.
    /// </summary>
    public class ButtonEventSample : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_OnButtonClicked = new UnityEvent();

        /// <summary>
        /// Event to be invoked when the UI Toolkit button is clicked.
        /// </summary>
        public UnityEvent onButtonClicked
        {
            get => m_OnButtonClicked;
            set => m_OnButtonClicked = value;
        }

        const string k_LabelName = "DebugLabel";
        const string k_EventButtonName = "EventButton";
#if UIELEMENTS_MODULE_AVAILABLE
        UIDocument m_UIDocument;
        Button m_Button;
        Label m_Label;
        VisualElement m_Root;
#endif

        private void OnEnable()
        {
#if UIELEMENTS_MODULE_AVAILABLE
            m_UIDocument = GetComponent<UIDocument>();
            BindElements();
#endif
        }

        private void OnDisable()
        {
#if UIELEMENTS_MODULE_AVAILABLE
            UnbindElements();
#endif
        }

#if UIELEMENTS_MODULE_AVAILABLE
        private void Update()
        {
            // When the UIDocument component is disabled and re-enabled independently of
            // this GameObject, the visual tree is recreated but OnEnable does not fire on
            // this script. Detect the change by checking if rootVisualElement has changed.
            if (m_UIDocument != null && m_UIDocument.enabled &&
                m_UIDocument.rootVisualElement != null && m_UIDocument.rootVisualElement != m_Root)
                BindElements();
        }

        void BindElements()
        {
            UnbindElements();

            if (m_UIDocument == null)
                return;

            m_Root = m_UIDocument.rootVisualElement;
            if (m_Root == null)
                return;

            m_Button = m_Root.Q<Button>(k_EventButtonName);
            m_Label = m_Root.Q<Label>(k_LabelName);

            if (m_Button != null)
                m_Button.clicked += HandleButtonClicked;
        }

        void UnbindElements()
        {
            if (m_Button != null)
                m_Button.clicked -= HandleButtonClicked;

            m_Button = null;
            m_Label = null;
            m_Root = null;
        }
#endif

        private void HandleButtonClicked()
        {
            if (m_OnButtonClicked != null)
                m_OnButtonClicked.Invoke();

#if UIELEMENTS_MODULE_AVAILABLE
            if (m_Label != null)
                m_Label.text = "Button clicked at: " + Time.time;
#endif
        }
    }
}
