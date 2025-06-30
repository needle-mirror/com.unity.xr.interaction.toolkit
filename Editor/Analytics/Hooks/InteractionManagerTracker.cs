#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Runtime tracker for XR Interaction Manager registration events.
    /// This class is used to track the number of enabled interaction managers along with
    /// the number of registered interactors and interactables during a play mode session.
    /// </summary>
    /// <seealso cref="UpdateEventPayload"/>
    /// <seealso cref="XRInteractionManager"/>
    class InteractionManagerTracker
    {
        /// <summary>
        /// The maximum number of interactor or interactable instances to track in a <see cref="HashSet{T}"/>.
        /// This keeps the memory usage of the tracker bounded.
        /// </summary>
        /// <remarks>
        /// The peak count is allowed to report above this number since we are just tracking the int count,
        /// so no need to limit that since the purpose of this limit is to keep memory usage bounded.
        /// </remarks>
        const int k_MaxHashSetCount = 500;

        /// <summary>
        /// Duration in seconds of at least one enabled interaction manager during a play mode session.
        /// </summary>
        /// <remarks>
        /// This duration can range from 0 seconds to the total length of the play mode session,
        /// and does not differ based on multiple managers being enabled simultaneously.
        /// </remarks>
        public double sessionDuration { get; private set; }

        /// <summary>
        /// Peak number of enabled interaction managers during a play mode session.
        /// </summary>
        public int interactionManagersPeakCount { get; private set; }

        /// <summary>
        /// Total count of different interaction managers that were enabled during a play mode session.
        /// </summary>
        public int interactionManagersObjectCount => m_SubscribedManagers.Count;

        /// <summary>
        /// Peak number of registered interactors across all interaction managers during a play mode session.
        /// </summary>
        public int interactorsPeakRegisteredCount { get; private set; }

        /// <summary>
        /// Total count of different interactors that were registered during a play mode session.
        /// </summary>
        public int interactorsObjectRegisteredCount => m_Interactors.Count;

        /// <summary>
        /// Peak number of registered interactables across all interaction managers during a play mode session.
        /// </summary>
        public int interactablesPeakRegisteredCount { get; private set; }

        /// <summary>
        /// Total count of different interactables that were registered during a play mode session.
        /// </summary>
        public int interactablesObjectRegisteredCount => m_Interactables.Count;

        List<XRInteractionManager> m_EnabledManagers = new List<XRInteractionManager>();

        List<XRInteractionManager> m_SubscribedManagers = new List<XRInteractionManager>();

        /// <summary>
        /// All interactors that were registered at any point during a play mode session.
        /// </summary>
        HashSet<IXRInteractor> m_Interactors = new HashSet<IXRInteractor>();

        /// <summary>
        /// All interactables that were registered at any point during a play mode session.
        /// </summary>
        HashSet<IXRInteractable> m_Interactables = new HashSet<IXRInteractable>();

        double m_StartTimestamp;

        int m_CurrentInteractorsRegisteredCount;
        int m_CurrentInteractablesRegisteredCount;

        /// <summary>
        /// Reset the tracker to its initial state.
        /// Call this after generating the analytics payload to avoid accumulating data across
        /// multiple play mode sessions when domain reload is disabled.
        /// </summary>
        public void Cleanup()
        {
            foreach (var manager in m_SubscribedManagers)
            {
                if (manager != null)
                {
                    manager.interactorRegistered -= OnInteractorRegistered;
                    manager.interactorUnregistered -= OnInteractorUnregistered;
                    manager.interactableRegistered -= OnInteractableRegistered;
                    manager.interactableUnregistered -= OnInteractableUnregistered;
                }
            }

            sessionDuration = 0d;
            interactionManagersPeakCount = 0;
            interactorsPeakRegisteredCount = 0;
            interactablesPeakRegisteredCount = 0;

            m_EnabledManagers.Clear();
            m_SubscribedManagers.Clear();
            m_Interactors.Clear();
            m_Interactables.Clear();

            m_StartTimestamp = 0d;
            m_CurrentInteractorsRegisteredCount = 0;
            m_CurrentInteractablesRegisteredCount = 0;
        }

        /// <summary>
        /// Start tracking the manager component, invoked when its <c>OnEnable</c> is called.
        /// </summary>
        /// <param name="manager">The manager component to track.</param>
        /// <param name="now">Current timestamp.</param>
        public void StartSession(XRInteractionManager manager, double now)
        {
            if (m_EnabledManagers.Contains(manager))
                return;

            m_EnabledManagers.Add(manager);
            if (m_EnabledManagers.Count > interactionManagersPeakCount)
                interactionManagersPeakCount = m_EnabledManagers.Count;

            if (!m_SubscribedManagers.Contains(manager))
            {
                m_SubscribedManagers.Add(manager);

                // Developer reminder to update Cleanup() if new events are subscribed to here.
                manager.interactorRegistered += OnInteractorRegistered;
                manager.interactorUnregistered += OnInteractorUnregistered;
                manager.interactableRegistered += OnInteractableRegistered;
                manager.interactableUnregistered += OnInteractableUnregistered;

                // Add any existing interactors that are already registered before subscribing.
                using (ListPool<IXRInteractor>.Get(out var scratchInteractors))
                {
                    manager.GetRegisteredInteractors(scratchInteractors);
                    m_CurrentInteractorsRegisteredCount += scratchInteractors.Count;
                    foreach (var interactor in scratchInteractors)
                    {
                        AddToHashSet(interactor);
                    }
                }

                // Add any existing interactables that are already registered before subscribing.
                using (ListPool<IXRInteractable>.Get(out var scratchInteractables))
                {
                    manager.GetRegisteredInteractables(scratchInteractables);
                    m_CurrentInteractablesRegisteredCount += scratchInteractables.Count;
                    foreach (var interactable in scratchInteractables)
                    {
                        AddToHashSet(interactable);
                    }
                }

                UpdatePeakCount();
            }

            if (m_EnabledManagers.Count == 1)
                m_StartTimestamp = now;
        }

        /// <summary>
        /// End tracking the manager component, invoked when its <c>OnDisable</c> is called.
        /// </summary>
        /// <param name="manager">The manager component to track.</param>
        /// <param name="now">Current timestamp.</param>
        public void EndSession(XRInteractionManager manager, double now)
        {
            if (m_EnabledManagers.Remove(manager) && m_EnabledManagers.Count == 0)
                sessionDuration += now - m_StartTimestamp;
        }

        /// <summary>
        /// Update the analytics payload struct with the data from this tracker.
        /// </summary>
        /// <param name="payload">The analytics payload to write into.</param>
        public void UpdateEventPayload(ref XRIPlayModeEvent.Payload payload)
        {
            payload.interactionManagersPeakCount = interactionManagersPeakCount;
            payload.interactionManagersObjectCount = interactionManagersObjectCount;
            payload.interactorsPeakRegisteredCount = interactorsPeakRegisteredCount;
            payload.interactorsObjectRegisteredCount = interactorsObjectRegisteredCount;
            payload.interactablesPeakRegisteredCount = interactablesPeakRegisteredCount;
            payload.interactablesObjectRegisteredCount = interactablesObjectRegisteredCount;
            payload.interactionManagerDurationSeconds = (float)sessionDuration;
        }

        void UpdatePeakCount()
        {
            if (m_CurrentInteractorsRegisteredCount > interactorsPeakRegisteredCount)
                interactorsPeakRegisteredCount = m_CurrentInteractorsRegisteredCount;

            if (m_CurrentInteractablesRegisteredCount > interactablesPeakRegisteredCount)
                interactablesPeakRegisteredCount = m_CurrentInteractablesRegisteredCount;
        }

        void AddToHashSet(IXRInteractor interactor)
        {
            if (m_Interactors.Count < k_MaxHashSetCount)
                m_Interactors.Add(interactor);
        }

        void AddToHashSet(IXRInteractable interactable)
        {
            if (m_Interactables.Count < k_MaxHashSetCount)
                m_Interactables.Add(interactable);
        }

        void OnInteractorRegistered(InteractorRegisteredEventArgs args)
        {
            AddToHashSet(args.interactorObject);

            m_CurrentInteractorsRegisteredCount++;
            UpdatePeakCount();
        }

        void OnInteractorUnregistered(InteractorUnregisteredEventArgs args)
        {
            m_CurrentInteractorsRegisteredCount--;
        }

        void OnInteractableRegistered(InteractableRegisteredEventArgs args)
        {
            AddToHashSet(args.interactableObject);

            m_CurrentInteractablesRegisteredCount++;
            UpdatePeakCount();
        }

        void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            m_CurrentInteractablesRegisteredCount--;
        }
    }
}

#endif
