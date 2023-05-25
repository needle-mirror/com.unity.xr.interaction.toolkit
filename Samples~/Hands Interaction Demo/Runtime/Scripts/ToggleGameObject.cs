using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.Samples
{
    public class ToggleGameObject : MonoBehaviour
    {
        [SerializeField]
        bool m_RestoreActiveStateOnEnable = false;

        public bool restoreActiveStateOnEnable
        {
            get => m_RestoreActiveStateOnEnable;
            set => m_RestoreActiveStateOnEnable = value;
        }

        [SerializeField]
        GameObject m_ActivationGameObject;

        public GameObject activationGameObject
        {
            get => m_ActivationGameObject;
            set => m_ActivationGameObject = value;
        }

        bool m_CurrentlyActive = false;

        void OnEnable()
        {
            if (m_RestoreActiveStateOnEnable && m_CurrentlyActive)
            {
                activationGameObject.SetActive(true);
            }
        }

        void OnDisable()
        {
            activationGameObject.SetActive(false);

            if (!m_RestoreActiveStateOnEnable)
            {
                m_CurrentlyActive = false;
            }
        }

        public void ToggleActiveState()
        {
            m_CurrentlyActive = !m_CurrentlyActive;
            activationGameObject.SetActive(m_CurrentlyActive);
            if (m_CurrentlyActive)
                Debug.Log("Turning menu ON");            
        }
    }
}
