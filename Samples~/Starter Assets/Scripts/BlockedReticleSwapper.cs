using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Swaps the reticle on a <see cref="CurveVisualController"/> when the associated interactor
    /// is hovering an interactable but cannot select it (blocked state).
    /// </summary>
    public class BlockedReticleSwapper : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The interactor to monitor for blocked state.")]
        XRBaseInteractor m_Interactor;

        [SerializeField]
        [Tooltip("The curve visual controller whose reticle to swap when blocked.")]
        CurveVisualController m_CurveVisualController;

        [SerializeField]
        [Tooltip("The reticle prefab to show when the interactor is blocked. A prefab will be instantiated at runtime.")]
        GameObject m_BlockedReticle;

        GameObject m_OriginalReticle;
        GameObject m_BlockedReticleInstance;
        bool m_IsCurrentlyBlocked;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            if (m_Interactor == null || m_CurveVisualController == null || m_BlockedReticle == null)
            {
                enabled = false;
                return;
            }

            m_OriginalReticle = m_CurveVisualController.defaultReticle;

            if (m_BlockedReticleInstance == null)
            {
                m_BlockedReticleInstance = Instantiate(m_BlockedReticle);
                m_BlockedReticleInstance.SetActive(false);
            }

            m_IsCurrentlyBlocked = false;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            if (m_IsCurrentlyBlocked && m_CurveVisualController != null)
            {
                m_CurveVisualController.defaultReticle = m_OriginalReticle;
                m_IsCurrentlyBlocked = false;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDestroy()
        {
            if (m_BlockedReticleInstance != null)
                Destroy(m_BlockedReticleInstance);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            var blocked = IsInteractorBlocked();
            if (blocked == m_IsCurrentlyBlocked)
                return;

            m_IsCurrentlyBlocked = blocked;
            m_CurveVisualController.defaultReticle = blocked ? m_BlockedReticleInstance : m_OriginalReticle;
        }

        bool IsInteractorBlocked()
        {
            if (m_Interactor.hasSelection)
                return false;

            if (!m_Interactor.hasHover)
                return false;

            var interactionManager = m_Interactor.interactionManager;
            if (interactionManager == null)
                return false;

            foreach (var interactable in m_Interactor.interactablesHovered)
            {
                if (interactable is IXRSelectInteractable selectInteractable &&
                    interactionManager.IsSelectPossible(m_Interactor, selectInteractable))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
