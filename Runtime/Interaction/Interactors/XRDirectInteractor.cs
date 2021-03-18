using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for directly interacting with interactables that are touching. This is handled via trigger volumes
    /// that update the current set of valid targets for this interactor. This component must have a collision volume that is
    /// set to be a trigger to work.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/XR Direct Interactor")]
    [HelpURL(XRHelpURLConstants.k_XRDirectInteractor)]
    public class XRDirectInteractor : XRBaseControllerInteractor
    {
        readonly List<XRBaseInteractable> m_ValidTargets = new List<XRBaseInteractable>();
        /// <inheritdoc />
        protected override List<XRBaseInteractable> validTargets => m_ValidTargets;

        /// <summary>
        /// Reusable map of interactables to their distance squared from this interactor (used for sort).
        /// </summary>
        readonly Dictionary<XRBaseInteractable, float> m_InteractableDistanceSqrMap = new Dictionary<XRBaseInteractable, float>();

        /// <summary>
        /// Sort comparison function used by <see cref="GetValidTargets"/>.
        /// </summary>
        Comparison<XRBaseInteractable> m_InteractableSortComparison;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_InteractableSortComparison = InteractableSortComparison;
            if (!GetComponents<Collider>().Any(x => x.isTrigger))
                Debug.LogWarning("Direct Interactor does not have required Collider set as a trigger.", this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerEnter(Collider other)
        {
            if (interactionManager == null)
                return;

            var interactable = interactionManager.GetInteractableForCollider(other);
            if (interactable != null && !m_ValidTargets.Contains(interactable))
                m_ValidTargets.Add(interactable);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerExit(Collider other)
        {
            if (interactionManager == null)
                return;

            var interactable = interactionManager.GetInteractableForCollider(other);
            if (interactable != null)
                m_ValidTargets.Remove(interactable);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<XRBaseInteractable> targets)
        {
            targets.Clear();
            m_InteractableDistanceSqrMap.Clear();

            // Calculate distance squared to interactor's attach transform and add to targets (which is sorted before returning)
            foreach (var interactable in m_ValidTargets)
            {
                m_InteractableDistanceSqrMap[interactable] = interactable.GetDistanceSqrToInteractor(this);
                targets.Add(interactable);
            }

            targets.Sort(m_InteractableSortComparison);
        }

        /// <inheritdoc />
        public override bool CanHover(XRBaseInteractable interactable)
        {
            return base.CanHover(interactable) && (selectTarget == null || selectTarget == interactable);
        }

        /// <inheritdoc />
        public override bool CanSelect(XRBaseInteractable interactable)
        {
            return base.CanSelect(interactable) && (selectTarget == null || selectTarget == interactable);
        }

        /// <inheritdoc />
        protected internal override void OnRegistered(InteractorRegisteredEventArgs args)
        {
            base.OnRegistered(args);
            args.manager.interactableUnregistered += OnInteractableUnregistered;
        }

        /// <inheritdoc />
        protected internal override void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            base.OnUnregistered(args);
            args.manager.interactableUnregistered -= OnInteractableUnregistered;
        }

        void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            m_ValidTargets.Remove(args.interactable);
        }

        int InteractableSortComparison(XRBaseInteractable x, XRBaseInteractable y)
        {
            var xDistance = m_InteractableDistanceSqrMap[x];
            var yDistance = m_InteractableDistanceSqrMap[y];
            if (xDistance > yDistance)
                return 1;
            if (xDistance < yDistance)
                return -1;

            return 0;
        }
    }
}
