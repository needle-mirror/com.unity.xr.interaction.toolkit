#if UIELEMENTS_MODULE_PRESENT
#if UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
#if UNITY_6000_5_OR_NEWER
#define UITOOLKIT_PANEL_RENDERER_ENABLED
#endif
#if UNITY_7000_0_a8_OR_NEWER
#define UITOOLKIT_COLLIDER_EVENTS
#endif
#endif // UIELEMENTS_MODULE_PRESENT
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
            XRUIToolkitHandler.activeScrollMomentum = m_ScrollMomentum;

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
            if (XRUIToolkitHandler.activeScrollMomentum == m_ScrollMomentum)
                XRUIToolkitHandler.activeScrollMomentum = null;

            m_ScrollMomentum.Clear();

            XRInteractionManager.activeInteractionManagersChanged -= OnActiveManagerChanged;

            foreach (var manager in XRInteractionManager.activeInteractionManagers)
            {
                UnsubscribeFromManager(manager);
            }

            // Clear the monitored interactables only.
            // The m_PanelInteractables is kept for re-registration without TryGetComponent.
            m_MonitoredInteractables.Clear();
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
#if UITOOLKIT_COLLIDER_EVENTS
            foreach (var interactable in m_PanelInteractables)
            {
                if (interactable != null && interactable.TryGetComponent<IPanelComponent>(out var panelComponent))
                    UnsubscribeFromColliderEvents(panelComponent);
            }
#endif
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        /// <summary>
        /// The momentum scrolls of the UI Toolkit scroll views that interactors scrolled with UI scroll momentum.
        /// </summary>
        readonly ScrollMomentum m_ScrollMomentum = new ScrollMomentum();

        /// <summary>
        /// Flag to indicate the cache should be pruned so that it doesn't infinitely grow in size.
        /// Set upon certain events rather than polling for destruction every frame.
        /// </summary>
        bool m_Prune;

        /// <summary>
        /// Permanent cache of confirmed UITK panel interactables (<see cref="UIDocument"/> or <see cref="PanelRenderer"/>)
        /// with dynamic colliders. Survives unregister and manager disable. Only removed on interactable destroy.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_PanelInteractables = new HashSet<XRBaseInteractable>();

        /// <summary>
        /// Currently registered UITK panels. Polled each frame for collider changes.
        /// Stays through collider create/destroy cycles. Removed on interactable unregister.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_MonitoredInteractables = new HashSet<XRBaseInteractable>();

        // Scratch list for safe removal during iteration.
        readonly List<XRBaseInteractable> m_PendingToRemove = new List<XRBaseInteractable>();

        // Scratch list for getting registered interactables.
        readonly List<IXRInteractable> m_ScratchInteractables = new List<IXRInteractable>();

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

            m_ScrollMomentum.UpdateMomentum(Time.unscaledDeltaTime);
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
                m_PanelInteractables.Remove(interactable);
            }
        }

        /// <summary>
        /// Removes destroyed interactables from the cache to prevent accumulating
        /// stale references in scenes that spawn and destroy UI panels.
        /// </summary>
        void PruneDestroyedCacheEntries()
        {
            if (m_PanelInteractables.Count > 0)
                m_PanelInteractables.RemoveWhere(i => i == null);
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

            // Scan all registered interactables. Cached panels that should be monitored are re-added to monitoring
            // since the monitored set was cleared on disable or by interactable unregistration.
            manager.GetRegisteredInteractables(m_ScratchInteractables);
            foreach (var interactable in m_ScratchInteractables)
            {
                if (interactable is XRBaseInteractable baseInteractable)
                {
                    if (m_PanelInteractables.Contains(baseInteractable))
                    {
                        if (ShouldMonitor(baseInteractable))
                            m_MonitoredInteractables.Add(baseInteractable);

                        TryRegisterCollider(baseInteractable);
                    }
                    else
                    {
                        RegisterInteractableForMonitoring(baseInteractable);
                    }
                }
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
                if (m_PanelInteractables.Contains(interactable))
                {
                    if (ShouldMonitor(interactable))
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
        /// Checks if an interactable has a panel component (<see cref="UIDocument"/> or <see cref="PanelRenderer"/>)
        /// with dynamic collider creation. If confirmed, adds to the permanent cache and active monitoring set.
        /// </summary>
        void RegisterInteractableForMonitoring(XRBaseInteractable interactable)
        {
#if UITOOLKIT_PANEL_RENDERER_ENABLED
            if (!interactable.TryGetComponent<IPanelComponent>(out var panelComponent))
                return;

            if (panelComponent.panelSettings != null && panelComponent.panelSettings.colliderUpdateMode == ColliderUpdateMode.Keep)
                return;
#else
            if (!interactable.TryGetComponent<UIDocument>(out var uiDocument))
                return;

            if (uiDocument.panelSettings != null && uiDocument.panelSettings.colliderUpdateMode == ColliderUpdateMode.Keep)
                return;
#endif

            m_PanelInteractables.Add(interactable);
#if UITOOLKIT_COLLIDER_EVENTS
            if (!TrySubscribeToColliderEvents(panelComponent))
                m_MonitoredInteractables.Add(interactable);
#else
            m_MonitoredInteractables.Add(interactable);
#endif
            TryRegisterCollider(interactable);

            m_Prune = true;
        }

#if UITOOLKIT_COLLIDER_EVENTS
        /// <summary>
        /// Whether the panel component type raises <c>worldSpaceColliderChanged</c>.
        /// Keep in sync with the types handled by <see cref="TrySubscribeToColliderEvents"/>.
        /// </summary>
        static bool HasColliderEvents(IPanelComponent panelComponent) =>
            panelComponent is UIDocument || panelComponent is PanelRenderer;

        /// <summary>
        /// Attempts to subscribe to <c>worldSpaceColliderChanged</c> on the concrete panel component type.
        /// </summary>
        /// <returns><see langword="true"/> if the event subscription was successful, <see langword="false"/> if the panel component type is not recognized.</returns>
        bool TrySubscribeToColliderEvents(IPanelComponent panelComponent)
        {
            if (panelComponent is UIDocument uiDocument)
                uiDocument.worldSpaceColliderChanged += OnWorldSpaceColliderChanged;
            else if (panelComponent is PanelRenderer panelRenderer)
                panelRenderer.worldSpaceColliderChanged += OnWorldSpaceColliderChanged;
            else
                return false;

            return true;
        }

        /// <summary>
        /// Unsubscribes from <c>worldSpaceColliderChanged</c> on the concrete panel component type.
        /// </summary>
        void UnsubscribeFromColliderEvents(IPanelComponent panelComponent)
        {
            if (panelComponent is UIDocument uiDocument)
                uiDocument.worldSpaceColliderChanged -= OnWorldSpaceColliderChanged;
            else if (panelComponent is PanelRenderer panelRenderer)
                panelRenderer.worldSpaceColliderChanged -= OnWorldSpaceColliderChanged;
        }

        /// <summary>
        /// Handles panel component collider creation and destruction events.
        /// </summary>
        void OnWorldSpaceColliderChanged(Collider col, bool created)
        {
            if (!isActiveAndEnabled || col == null)
                return;

            var interactable = col.GetComponentInParent<XRBaseInteractable>();
            if (interactable == null)
                return;

            interactable.RefreshColliders();
        }
#endif

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

        /// <summary>
        /// Whether a cached UITK panel should be monitored (polled each frame for collider changes)
        /// rather than relying on collider events.
        /// </summary>
        static bool ShouldMonitor(XRBaseInteractable interactable)
        {
#if UITOOLKIT_COLLIDER_EVENTS
            return interactable.TryGetComponent<IPanelComponent>(out var panelComponent) && !HasColliderEvents(panelComponent);
#else
            return true;
#endif
        }

        /// <summary>
        /// Whether the interactable is currently polled each frame for collider changes. Used by tests.
        /// </summary>
        internal bool IsMonitored(XRBaseInteractable interactable) => m_MonitoredInteractables.Contains(interactable);
#endif
    }
}
