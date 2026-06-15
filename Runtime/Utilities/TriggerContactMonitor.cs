using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
        class ReferenceCounter<T> where T : Collider
        {
            public int Count => m_Items.Count;

            public Dictionary<T, int>.KeyCollection items => m_Items.Keys;

            readonly Dictionary<T, int> m_Items = new Dictionary<T, int>();

            static readonly List<T> s_ItemsToRemove = new List<T>();

            /// <summary>
            /// Add the specified element to the set or increment the counter if already added.
            /// </summary>
            /// <param name="item">The element to add or increment.</param>
            /// <param name="counter">The new counter after incrementing or adding the item.</param>
            /// <returns>Returns <see langword="true"/> if the element is added to the set; <see langword="false"/> if the element is already present.</returns>
            public bool AddOrIncrement(T item, out int counter)
            {
                if (m_Items.TryGetValue(item, out var value))
                {
                    counter = ++value;
                    m_Items[item] = counter;
                    return false;
                }

                counter = 1;
                m_Items[item] = counter;
                return true;
            }

            /// <summary>
            /// Decrement the counter or remove the specified element from the set if decremented to zero.
            /// </summary>
            /// <param name="item">The element to decrement or remove.</param>
            /// <param name="counter">The new counter after decrementing or removing the item.</param>
            /// <returns>Returns <see langword="true"/> if the element is successfully found and decremented or removed.
            /// Otherwise, returns <see langword="false"/> such as if the element is not found in the set.</returns>
            public bool DecrementOrRemove(T item, out int counter)
            {
                if (m_Items.TryGetValue(item, out var value))
                {
                    if (value > 1)
                    {
                        counter = --value;
                        m_Items[item] = counter;
                    }
                    else
                    {
                        m_Items.Remove(item);
                        counter = 0;
                    }

                    return true;
                }

                counter = 0;
                return false;
            }

            public void Add(T item, int counter) => m_Items.Add(item, counter);

            public bool Remove(T item, out int counter) => m_Items.Remove(item, out counter);

            public int GetCounter(T item) => m_Items.GetValueOrDefault(item, 0);

            public void Clear() => m_Items.Clear();

            public bool Contains(T item) => m_Items.ContainsKey(item);

            public void Remove(Predicate<T> predicate)
            {
                if (m_Items.Count == 0)
                    return;

                foreach (var item in m_Items.Keys)
                {
                    if (predicate.Invoke(item))
                        s_ItemsToRemove.Add(item);
                }

                if (s_ItemsToRemove.Count > 0)
                {
                    foreach (var item in s_ItemsToRemove)
                    {
                        m_Items.Remove(item);
                    }

                    s_ItemsToRemove.Clear();
                }
            }
        }

        /// <summary>
        /// Calls the methods in its invocation list when an Interactable is being touched.
        /// </summary>
        /// <remarks>
        /// Will only be fired for an Interactable once when any of the colliders associated with it are touched.
        /// In other words, touching more of its colliders does not cause this to fire again until all of its colliders
        /// are no longer being touched.
        /// </remarks>
        public event Action<IXRInteractable> contactAdded;

        /// <summary>
        /// Calls the methods in its invocation list when an Interactable is no longer being touched.
        /// </summary>
        /// <remarks>
        /// Will only be fired for an Interactable once all the colliders associated with it are no longer touched.
        /// In other words, leaving just one of its colliders when another one of it is still being touched
        /// will not fire the event.
        /// </remarks>
        public event Action<IXRInteractable> contactRemoved;

        /// <summary>
        /// The Interaction Manager used to fetch the Interactable associated with a Collider.
        /// </summary>
        /// <seealso cref="XRInteractionManager.TryGetInteractableForCollider(Collider, out IXRInteractable)"/>
        public XRInteractionManager interactionManager { get; set; }

        public int enteredCollidersCount => m_EnteredAssociatedColliders.Count + m_EnteredUnassociatedColliders.Count;

        readonly Dictionary<Collider, IXRInteractable> m_EnteredInteractableColliders = new Dictionary<Collider, IXRInteractable>();

        readonly ReferenceCounter<Collider> m_EnteredAssociatedColliders = new ReferenceCounter<Collider>();
        readonly ReferenceCounter<Collider> m_EnteredUnassociatedColliders = new ReferenceCounter<Collider>();

        readonly HashSet<IXRInteractable> m_UnorderedInteractables = new HashSet<IXRInteractable>();

        // Preallocate delegates to avoid GC Alloc
        static readonly Predicate<Collider> s_IsDestroyed = IsDestroyed;
        static readonly Predicate<Collider> s_IsDestroyedOrDisabledInHierarchy = IsDestroyedOrDisabledInHierarchy;

        /// <summary>
        /// Reusable temporary list of Collider objects for resolving unassociated colliders.
        /// </summary>
        static readonly List<Collider> s_ScratchColliders = new List<Collider>();

        /// <summary>
        /// Reusable temporary list of Collider objects for removing colliders that did not stay during the frame
        /// but previously entered.
        /// </summary>
        static readonly List<Collider> s_ExitedColliders = new List<Collider>();

        /// <summary>
        /// Adds <paramref name="collider"/> to contact list.
        /// </summary>
        /// <param name="collider">The Collider to add.</param>
        /// <seealso cref="RemoveCollider"/>
        public void AddCollider(Collider collider)
        {
            if (interactionManager == null)
                return;

            if (!interactionManager.TryGetInteractableForCollider(collider, out var interactable))
            {
                m_EnteredUnassociatedColliders.AddOrIncrement(collider, out _);
                return;
            }

            if (m_EnteredAssociatedColliders.AddOrIncrement(collider, out _))
                m_EnteredInteractableColliders[collider] = interactable;

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
            if (m_EnteredUnassociatedColliders.DecrementOrRemove(collider, out _))
                return;

            if (m_EnteredAssociatedColliders.DecrementOrRemove(collider, out var counter) && counter == 0)
            {
                if (!m_EnteredInteractableColliders.Remove(collider, out var interactable) || interactable == null)
                    return;

                // Don't remove the Interactable if there are still
                // any of its other colliders touching this trigger.
                // Treat destroyed colliders as no longer touching.
                foreach (var kvp in m_EnteredInteractableColliders)
                {
                    if (kvp.Value == interactable && kvp.Key != null)
                        return;
                }

                if (m_UnorderedInteractables.Remove(interactable))
                    contactRemoved?.Invoke(interactable);
            }
        }

        /// <summary>
        /// Clear the state of all contacts. This can be done when the trigger Collider is disabled and is unable
        /// to receive exit events and thus must invalidate all touched colliders.
        /// </summary>
        public void RemoveAllCollidersWithoutNotify()
        {
            m_EnteredInteractableColliders.Clear();
            m_EnteredAssociatedColliders.Clear();
            m_EnteredUnassociatedColliders.Clear();
            m_UnorderedInteractables.Clear();
        }

        /// <summary>
        /// Get all interactables that are being touched.
        /// </summary>
        /// <param name="results">List to receive interactables.</param>
        public void GetContactInteractables(List<IXRInteractable> results)
        {
            results.Clear();
            results.AddRange(m_UnorderedInteractables);
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
            // Cull destroyed colliders from the set to keep it tidy
            // since there would be no reason to monitor it anymore.
            if (m_EnteredUnassociatedColliders.Count > 0)
                m_EnteredUnassociatedColliders.Remove(s_IsDestroyed);

            if (m_EnteredUnassociatedColliders.Count == 0 || interactionManager == null)
                return;

            s_ScratchColliders.Clear();
            foreach (var col in m_EnteredUnassociatedColliders.items)
            {
                if (interactionManager.TryGetInteractableForCollider(col, out var interactable))
                {
                    // Add to temporary list to remove in a second pass to avoid modifying
                    // the collection being iterated.
                    s_ScratchColliders.Add(col);

                    m_EnteredAssociatedColliders.Add(col, m_EnteredUnassociatedColliders.GetCounter(col));
                    m_EnteredInteractableColliders[col] = interactable;

                    if (m_UnorderedInteractables.Add(interactable))
                        contactAdded?.Invoke(interactable);
                }
            }

            s_ScratchColliders.ForEach(RemoveFromUnassociatedColliders);
            s_ScratchColliders.Clear();
        }

        void RemoveFromUnassociatedColliders(Collider col) => m_EnteredUnassociatedColliders.Remove(col, out _);

        /// <summary>
        /// Resolves the unassociated colliders to <paramref name="interactable"/> if they match.
        /// </summary>
        /// <param name="interactable">The Interactable to try to associate with the unassociated colliders.</param>
        /// <remarks>
        /// This process is done automatically when Colliders are added,
        /// but this method can be used to force a refresh.
        /// </remarks>
        public void ResolveUnassociatedColliders(IXRInteractable interactable)
        {
            // Cull destroyed colliders from the set to keep it tidy
            // since there would be no reason to monitor it anymore.
            if (m_EnteredUnassociatedColliders.Count > 0)
                m_EnteredUnassociatedColliders.Remove(s_IsDestroyed);

            if (m_EnteredUnassociatedColliders.Count == 0 || interactionManager == null)
                return;

            for (int index = 0, count = interactable.colliders.Count; index < count; ++index)
            {
                var col = interactable.colliders[index];
                if (col == null)
                    continue;

                if (m_EnteredUnassociatedColliders.Contains(col) &&
                    interactionManager.TryGetInteractableForCollider(col, out var associatedInteractable) &&
                    associatedInteractable == interactable)
                {
                    m_EnteredUnassociatedColliders.Remove(col, out var counter);
                    m_EnteredAssociatedColliders.Add(col, counter);
                    m_EnteredInteractableColliders[col] = interactable;

                    if (m_UnorderedInteractables.Add(interactable))
                        contactAdded?.Invoke(interactable);
                }
            }
        }

        /// <summary>
        /// Remove colliders that no longer stay during this frame but previously entered and
        /// adds stayed colliders that are not currently tracked.
        /// </summary>
        /// <param name="stayedColliders">Colliders that stayed during the fixed update.</param>
        /// <remarks>
        /// Can be called in the fixed update phase by interactors after <c>OnTriggerStay</c>.
        /// </remarks>
        public void UpdateStayedColliders(HashSet<Collider> stayedColliders)
        {
            // Compare against unassociated colliders
            if (m_EnteredUnassociatedColliders.Count > 0)
            {
                s_ExitedColliders.Clear();
                foreach (var collider in m_EnteredUnassociatedColliders.items)
                {
                    if (collider == null || !stayedColliders.Contains(collider))
                        // Add to temporary list to remove in a second pass to avoid modifying
                        // the collection being iterated.
                        s_ExitedColliders.Add(collider);
                    else
                        // Remove collider that is already synced
                        stayedColliders.Remove(collider);
                }

                if (s_ExitedColliders.Count > 0)
                {
                    foreach (var collider in s_ExitedColliders)
                    {
                        m_EnteredUnassociatedColliders.Remove(collider, out _);
                    }

                    s_ExitedColliders.Clear();
                }
            }

            // Compare against associated colliders
            if (m_EnteredAssociatedColliders.Count > 0)
            {
                s_ExitedColliders.Clear();
                foreach (var collider in m_EnteredAssociatedColliders.items)
                {
                    if (collider == null || !stayedColliders.Contains(collider))
                        // Add to temporary list to remove in a second pass to avoid modifying
                        // the collection being iterated.
                        s_ExitedColliders.Add(collider);
                    else
                        // Remove collider that is already synced
                        stayedColliders.Remove(collider);
                }
            }

            if (stayedColliders.Count > 0)
            {
                foreach (var collider in stayedColliders)
                {
                    if (collider != null)
                    {
                        // Add a stayed Collider to the entered colliders list if the
                        // Collider is not currently tracked
                        AddCollider(collider);
                    }
                }
            }

            if (s_ExitedColliders.Count > 0)
            {
                s_ExitedColliders.ForEach(RemoveCollider);
                s_ExitedColliders.Clear();
            }
        }

        /// <summary>
        /// Removes colliders from contact list which are no longer enabled or active in hierarchy.
        /// This should be used to cull colliders when <c>OnTriggerStay</c> is not used since otherwise
        /// colliders which are moved to no longer touch while disabled will not trigger <c>OnTriggerExit</c>.
        /// </summary>
        /// <remarks>
        /// Can be called in the fixed update phase by interactors after OnTrigger event methods are invoked.
        /// </remarks>
        /// <seealso cref="UpdateStayedColliders"/>
        public void RemoveDisabledOrDestroyedColliders()
        {
            if (m_EnteredUnassociatedColliders.Count > 0)
                m_EnteredUnassociatedColliders.Remove(s_IsDestroyedOrDisabledInHierarchy);

            if (m_EnteredAssociatedColliders.Count > 0)
            {
                s_ExitedColliders.Clear();
                foreach (var collider in m_EnteredAssociatedColliders.items)
                {
                    if (IsDestroyedOrDisabledInHierarchy(collider))
                        s_ExitedColliders.Add(collider);
                }
            }

            if (s_ExitedColliders.Count > 0)
            {
                s_ExitedColliders.ForEach(RemoveCollider);
                s_ExitedColliders.Clear();
            }
        }

        /// <summary>
        /// Checks whether the Interactable is being touched.
        /// </summary>
        /// <param name="interactable">The Interactable to check if touching.</param>
        /// <returns>Returns <see langword="true"/> if the Interactable is being touched. Otherwise, returns <see langword="false"/>.</returns>
        public bool IsContacting(IXRInteractable interactable)
        {
            return m_UnorderedInteractables.Contains(interactable);
        }

        static bool IsDestroyed(Collider col) => col == null;

        static bool IsDestroyedOrDisabledInHierarchy(Collider col) => col == null || !col.enabled || !col.gameObject.activeInHierarchy;
    }
}
