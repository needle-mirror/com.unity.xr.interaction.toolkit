using UnityEngine.Events;
#if UIELEMENTS_MODULE_AVAILABLE
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.WorldSpaceUI
{
    /// <summary>
    /// Sample class that demonstrates how to bind to a UI Toolkit button click event
    /// on a <c>UIDocument</c> or <c>PanelRenderer</c> panel.
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

#if UIELEMENTS_MODULE_AVAILABLE
        UIDocument m_UIDocument;
        bool m_HasUIDocument;
#if UNITY_6000_5_OR_NEWER
        PanelRenderer m_PanelRenderer;
#endif
        VisualElement m_Root;
        Button m_Button;
        Label m_Label;

        void Awake()
        {
            m_HasUIDocument = TryGetComponent(out m_UIDocument);
#if UNITY_6000_5_OR_NEWER
            m_PanelRenderer = GetComponent<PanelRenderer>();
#endif
        }

        void OnEnable()
        {
#if UNITY_6000_5_OR_NEWER
            // A PanelRenderer calls back now if its UI is loaded, and again every time its UI reloads.
            if (m_PanelRenderer != null)
                m_PanelRenderer.RegisterUIReloadCallback(OnUIReload);
#endif
        }

        void OnDisable()
        {
#if UNITY_6000_5_OR_NEWER
            if (m_PanelRenderer != null)
                m_PanelRenderer.UnregisterUIReloadCallback(OnUIReload);
#endif

            Bind(null);
        }

        void Update()
        {
            // A UIDocument has no callback, and recreates its UI when re-enabled, so bind whenever its root changes.
            if (m_HasUIDocument && m_UIDocument != null && m_UIDocument.rootVisualElement != m_Root)
                Bind(m_UIDocument.rootVisualElement);
        }

#if UNITY_6000_5_OR_NEWER
        void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version) => Bind(root);
#endif

        void Bind(VisualElement root)
        {
            if (m_Button != null)
                m_Button.clicked -= OnButtonClicked;

            m_Root = root;
            m_Button = root?.Q<Button>("EventButton");
            m_Label = root?.Q<Label>("DebugLabel");

            if (m_Button != null)
                m_Button.clicked += OnButtonClicked;
        }

        void OnButtonClicked()
        {
            m_OnButtonClicked.Invoke();
            if (m_Label != null)
                m_Label.text = "Button clicked at: " + Time.time;
        }
#endif
    }
}
