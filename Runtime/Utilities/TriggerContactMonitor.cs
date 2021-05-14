using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Use this class to maintain a list of Colliders being touched in order to determine the set of
    /// Interactables that are being touched.
    /// </summary>
    /// <remarks>
    /// This class is useful for Interactors that utilize a trigger Collider to determine which objects
    /// it is coming in contact with. For Interactables with multiple Colliders, this will help handle the
    /// bookkeeping to know if any of the colliders are still being touched.
    /// </remarks>
    class TriggerContactMonitor
    {
        /// <summary>
        /// Calls the methods in its invocation list when an Interactable is being touched.
        /// </summary>
        /// <remarks>
        /// Will only be fired for an Interactable once when any of the colliders associated with it are touched.
        /// In other words, touching more of its colliders does not cause this to fire again until all of its colliders
        /// are no longer being touched.
        /// </remarks>
        public event Action<XRBaseInteractable> contactAdded;

        /// <summary>
        /// Calls the methods in its invocation list when an Interactable is no longer being touched.
        /// </summary>
        /// <remarks>
        /// Will only be fired for an Interactable once all of the colliders associated with it are no longer touched.
        /// In other words, leaving just one of its colliders when another one of it is still being touched
        /// will not fire the event.
        /// </remarks>
        public event Action<XRBaseInteractable> contactRemoved;

        /// <summary>
        /// The Interaction Manager used to fetch the Interactable associated with a Collider.
        /// </summary>
        /// <seealso cref="XRInteractionManager.GetInteractableForCollider"/>
        public XRInteractionManager interactionManager { get; set; }

        readonly Dictionary<Collider, XRBaseInteractable> m_EnteredColliders = new Dictionary<Collider, XRBaseInteractable>();
        readonly HashSet<XRBaseInteractable> m_UnorderedInteractables = new HashSet<XRBaseInteractable>();
        readonly HashSet<Collider> m_EnteredUnassociatedColliders = new HashSet<Collider>();

        /// <summary>
        /// Reusable temporary list of Collider objects.
        /// </summary>
        static readonly List<Collider> s_ScratchColliders = new List<Collider>();

        /// <summary>
        /// Adds <paramref name="collider"/> to contact list.
        /// </summary>
        /// <param name="collider">The Collider to add.</param>
        /// <seealso cref="RemoveCollider"/>
        public void AddCollider(Collider collider)
        {
            var interactable = interactionManager != null ? interactionManager.GetInteractableForCollider(collider) : null;
            if (interactable == null)
            {
                m_EnteredUnassociatedColliders.Add(collider);
                return;
            }

            m_EnteredColliders[collider] = interactable;

            if (m_UnorderedInteractables.Add(interactable))
                contactAdded?.Invoke(interactable);
        }

        /// <summary>
        /// Removes <paramref name="collider"/> from contact list.
        /// </summary>
        /// <param name="collider">The Collider to remove.</param>
        /// <seealso cref="AddCollider"/>
        public void RemoveCollider(Collider collider)
        {
            if (m_EnteredUnassociatedColliders.Remove(collider))
                return;

            if (m_EnteredColliders.TryGetValue(collider, out var interactable))
            {
                m_EnteredColliders.Remove(collider);

                if (interactable == null)
                    return;

                // Don't remove the Interactable if there are still
                // any of its colliders touching this trigger.
                foreach (var kvp in m_EnteredColliders)
                {
                    if (kvp.Value == interactable)
                        return;
                }

                if (m_UnorderedInteractables.Remove(interactable))
                    contactRemoved?.Invoke(interactable);
            }
        }

        /// <summary>
        /// Resolves all unassociated colliders to Interactables if possible.
        /// </summary>
        /// <remarks>
        /// This process is done automatically when Colliders are added,
        /// but this method can be used to force a refresh.
        /// </remarks>
        public void ResolveUnassociatedColliders()
        {
            if (m_EnteredUnassociatedColliders.Count == 0 || interactionManager == null)
                return;

            s_ScratchColliders.Clear();
            foreach (var col in m_EnteredUnassociatedColliders)
            {
                var interactable = interactionManager.GetInteractableForCollider(col);
                if (interactable != null)
                {
                    // Add to temporary list to remove in a second pass to avoid modifying
                    // the collection being iterated.
                    s_ScratchColliders.Add(col);
                    m_EnteredColliders[col] = interactable;

                    if (m_UnorderedInteractables.Add(interactable))
                        contactAdded?.Invoke(interactable);
                }
            }

            foreach (var col in s_ScratchColliders)
            {
                m_EnteredUnassociatedColliders.Remove(col);
            }

            s_ScratchColliders.Clear();
        }

        /// <summary>
        /// Resolves the unassociated colliders to <paramref name="interactable"/> if they match.
        /// This process
        /// </summary>
        /// <param name="interactable">The Interactable to try to associate with the unassociated colliders.</param>
        /// <remarks>
        /// This process is done automatically when Colliders are added,
        /// but this method can be used to force a refresh.
        /// </remarks>
        public void ResolveUnassociatedColliders(XRBaseInteractable interactable)
        {
            if (m_EnteredUnassociatedColliders.Count == 0 || interactionManager == null)
                return;

            foreach (var col in interactable.colliders)
            {
                if (m_EnteredUnassociatedColliders.Contains(col) && interactionManager.GetInteractableForCollider(col) == interactable)
                {
                    m_EnteredUnassociatedColliders.Remove(col);
                    m_EnteredColliders[col] = interactable;

                    if (m_UnorderedInteractables.Add(interactable))
                        contactAdded?.Invoke(interactable);
                }
            }
        }
    }
}