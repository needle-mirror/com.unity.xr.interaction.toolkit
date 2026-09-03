#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
#if UITOOLKIT_WORLDSPACE_ENABLED
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// This component controls whether UI Toolkit support is enabled for
    /// compatible <see cref="IXRInteractor"/> components in the scene.
    /// </summary>
    [AddComponentMenu("XR/XR UI Toolkit Manager", 11)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRUIToolkitManager)]
    [HelpURL(XRHelpURLConstants.k_XRUIToolkitManager)]
    public class XRUIToolkitManager : MonoBehaviour
    {
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            XRUIToolkitHandler.uiToolkitSupportEnabled = true;

            XRInteractionManager.activeInteractionManagersChanged += OnActiveManagerChanged;

            foreach (var manager in XRInteractionManager.activeInteractionManagers)
            {
                SubscribeToManager(manager);
            }
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            XRUIToolkitHandler.uiToolkitSupportEnabled = false;

            XRInteractionManager.activeInteractionManagersChanged -= OnActiveManagerChanged;

            foreach (var manager in XRInteractionManager.activeInteractionManagers)
            {
                UnsubscribeFromManager(manager);
            }

            // Clear the monitored interactables only.
            // The m_UIDocumentInteractables is kept for re-registration without TryGetComponent.
            m_MonitoredInteractables.Clear();
#endif
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        /// <summary>
        /// Flag to indicate the cache should be pruned so that it doesn't infinitely grow in size.
        /// Set upon certain events rather than polling for destruction every frame.
        /// </summary>
        bool m_Prune;

        /// <summary>
        /// Permanent cache of confirmed UITK panels with dynamic colliders. Survives
        /// unregister and manager disable. Only removed on interactable destroy.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_UIDocumentInteractables = new HashSet<XRBaseInteractable>();

        /// <summary>
        /// Currently registered UITK panels. Polled each frame for collider changes.
        /// Stays through collider create/destroy cycles. Removed on interactable unregister.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_MonitoredInteractables = new HashSet<XRBaseInteractable>();

        // Scratch list for getting registered interactables.
        readonly List<IXRInteractable> m_ScratchInteractables = new List<IXRInteractable>();

        // Scratch list for safe removal during iteration.
        readonly List<XRBaseInteractable> m_PendingToRemove = new List<XRBaseInteractable>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            if (m_MonitoredInteractables.Count > 0)
                ValidateMonitoredInteractables();

            if (m_Prune)
            {
                PruneDestroyedCacheEntries();
                m_Prune = false;
            }
        }

        /// <summary>
        /// Polls monitored interactables for collider changes. Cleans up destroyed
        /// interactables from both sets.
        /// </summary>
        void ValidateMonitoredInteractables()
        {
            m_PendingToRemove.Clear();
            foreach (var interactable in m_MonitoredInteractables)
            {
                if (interactable == null)
                {
                    m_PendingToRemove.Add(interactable);
                    continue;
                }

                if (!HasValidCollider(interactable))
                {
                    if (!TryRegisterCollider(interactable) && interactable.colliders.Count > 0)
                        interactable.RefreshColliders();
                }
            }

            foreach (var interactable in m_PendingToRemove)
            {
                m_MonitoredInteractables.Remove(interactable);
                m_UIDocumentInteractables.Remove(interactable);
            }
        }

        /// <summary>
        /// Removes destroyed interactables from the cache to prevent accumulating
        /// stale references in scenes that spawn and destroy UI panels.
        /// </summary>
        void PruneDestroyedCacheEntries()
        {
            if (m_UIDocumentInteractables.Count > 0)
                m_UIDocumentInteractables.RemoveWhere(i => i == null);
        }

        void OnActiveManagerChanged(XRInteractionManager manager, ComponentLifecyclePhase phase)
        {
            if (phase == ComponentLifecyclePhase.Enable)
                SubscribeToManager(manager);
            else if (phase == ComponentLifecyclePhase.Disable)
                UnsubscribeFromManager(manager);
            else if (phase == ComponentLifecyclePhase.Destroy)
                m_Prune = true;
        }

        void SubscribeToManager(XRInteractionManager manager)
        {
            manager.interactableRegistered += OnInteractableRegistered;
            manager.interactableUnregistered += OnInteractableUnregistered;

            // Initial scan — check all interactables without HasValidCollider pre-filter.
            manager.GetRegisteredInteractables(m_ScratchInteractables);
            foreach (var interactable in m_ScratchInteractables)
            {
                if (interactable is XRBaseInteractable baseInteractable)
                    RegisterInteractableForMonitoring(baseInteractable);
            }

            m_ScratchInteractables.Clear();
        }

        void UnsubscribeFromManager(XRInteractionManager manager)
        {
            manager.interactableRegistered -= OnInteractableRegistered;
            manager.interactableUnregistered -= OnInteractableUnregistered;
        }

        void OnInteractableRegistered(InteractableRegisteredEventArgs args)
        {
            if (args.interactableObject is XRBaseInteractable interactable)
            {
                // Fast path: already confirmed as a UITK panel from a previous registration.
                // Re-add to monitoring without TryGetComponent.
                if (m_UIDocumentInteractables.Contains(interactable))
                {
                    m_MonitoredInteractables.Add(interactable);
                    TryRegisterCollider(interactable);
                    return;
                }

                // Per-registration callback: skip the expensive TryGetComponent for interactables
                // that already have valid colliders (most non-UITK interactables).
                if (HasValidCollider(interactable))
                    return;

                RegisterInteractableForMonitoring(interactable);
            }
        }

        /// <summary>
        /// Removes the interactable from active monitoring on unregister.
        /// Kept in cache for fast re-registration. Destroyed entries are
        /// pruned by <see cref="PruneDestroyedCacheEntries"/>.
        /// </summary>
        void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            if (args.interactableObject is XRBaseInteractable interactable)
                m_MonitoredInteractables.Remove(interactable);
        }

        /// <summary>
        /// Checks if an interactable has a UIDocument with dynamic collider creation.
        /// If confirmed, adds to the permanent cache and active monitoring set.
        /// </summary>
        void RegisterInteractableForMonitoring(XRBaseInteractable interactable)
        {
            if (!interactable.TryGetComponent<UIDocument>(out var uiDocument))
                return;

            if (uiDocument.panelSettings != null && uiDocument.panelSettings.colliderUpdateMode == ColliderUpdateMode.Keep)
                return;

            m_UIDocumentInteractables.Add(interactable);
            m_MonitoredInteractables.Add(interactable);
            TryRegisterCollider(interactable);

            m_Prune = true;
        }

        /// <summary>
        /// Checks if a collider exists on the interactable's hierarchy and registers it.
        /// Uses a lightweight GetComponentInChildren check before calling RefreshColliders
        /// to avoid unnecessary overhead when no collider has been created yet.
        /// </summary>
        static bool TryRegisterCollider(XRBaseInteractable interactable)
        {
            if (interactable.GetComponentInChildren<Collider>() == null)
                return false;

            interactable.RefreshColliders();
            return HasValidCollider(interactable);
        }

        static bool HasValidCollider(IXRInteractable interactable)
        {
            foreach (var col in interactable.colliders)
            {
                if (col != null)
                    return true;
            }

            return false;
        }
#endif
    }
}
