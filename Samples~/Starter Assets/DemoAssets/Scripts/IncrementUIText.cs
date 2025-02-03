#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Add this component to a GameObject and call the <see cref="IncrementText"/> method
    /// in response to a Unity Event to update a text display to count up with each event.
    /// </summary>
    public class IncrementUIText : MonoBehaviour
    {
#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
        [SerializeField]
        [Tooltip("The TextMeshProUGUI component this behavior uses to display the incremented value.")]
        TextMeshProUGUI m_Text;

        /// <summary>
        /// The TextMeshProUGUI component this behavior uses to display the incremented value.
        /// </summary>
        public TextMeshProUGUI text
        {
            get => m_Text;
            set => m_Text = value;
        }
#else
        // Fallback field to keep the component functional without TMP.
        // Uses UnityEngine.Object so it can still hold a reference if TMP later becomes available.
        [SerializeField]
        [Tooltip("The TextMeshProUGUI component this behavior uses to display the incremented value.")]
        Object m_Text;

        /// <summary>
        /// The TextMeshProUGUI component this behavior uses to display the incremented value.
        /// </summary>
        public Object text
        {
            get => m_Text;
            set => m_Text = value;
        }
#endif

        int m_Count;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_Text == null)
                Debug.LogWarning("Missing required TextMeshProUGUI component reference. Use the Inspector window to assign which TMP component to increment.", this);
        }

        /// <summary>
        /// Increment the string message of the TextMeshProUGUI component.
        /// </summary>
        public void IncrementText()
        {
            m_Count += 1;
#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
            if (m_Text != null)
                m_Text.text = m_Count.ToString();
#else
            if (m_Text != null)
                Debug.LogWarning("TextMeshPro is not installed; cannot update TMP text.", this);
#endif
        }
    }
}
