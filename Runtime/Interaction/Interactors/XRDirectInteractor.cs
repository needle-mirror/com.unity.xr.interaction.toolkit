using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

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

        readonly TriggerContactMonitor m_TriggerContactMonitor = new TriggerContactMonitor();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_TriggerContactMonitor.interactionManager = interactionManager;
            m_TriggerContactMonitor.contactAdded += OnContactAdded;
            m_TriggerContactMonitor.contactRemoved += OnContactRemoved;

            if (!GetComponents<Collider>().Any(x => x.isTrigger))
                Debug.LogWarning("Direct Interactor does not have required Collider set as a trigger.", this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerEnter(Collider other)
        {
            m_TriggerContactMonitor.AddCollider(other);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerExit(Collider other)
        {
            m_TriggerContactMonitor.RemoveCollider(other);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<XRBaseInteractable> targets)
        {
            SortingHelpers.SortByDistanceToInteractor(this, m_ValidTargets, targets);
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
            args.manager.interactableRegistered += OnInteractableRegistered;
            args.manager.interactableUnregistered += OnInteractableUnregistered;

            // Attempt to resolve any colliders that entered this trigger while this was not subscribed,
            // and filter out any targets that were unregistered while this was not subscribed.
            m_TriggerContactMonitor.interactionManager = args.manager;
            m_TriggerContactMonitor.ResolveUnassociatedColliders();
            XRInteractionManager.RemoveAllUnregistered(args.manager, m_ValidTargets);
        }

        /// <inheritdoc />
        protected internal override void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            base.OnUnregistered(args);
            args.manager.interactableRegistered -= OnInteractableRegistered;
            args.manager.interactableUnregistered -= OnInteractableUnregistered;
        }

        void OnInteractableRegistered(InteractableRegisteredEventArgs args)
        {
            m_TriggerContactMonitor.ResolveUnassociatedColliders(args.interactable);
        }

        void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            m_ValidTargets.Remove(args.interactable);
        }

        void OnContactAdded(XRBaseInteractable interactable)
        {
            if (!m_ValidTargets.Contains(interactable))
                m_ValidTargets.Add(interactable);
        }

        void OnContactRemoved(XRBaseInteractable interactable)
        {
            m_ValidTargets.Remove(interactable);
        }
    }
}
