using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The Interaction Manager acts as an intermediary between Interactors and Interactables.
    /// It is possible to have multiple Interaction Managers, each with their own valid set of Interactors and Interactables.
    /// Upon Awake both Interactors and Interactables register themselves with a valid Interaction Manager
    /// (if a specific one has not already been assigned in the inspector). The loaded scenes must have at least one Interaction Manager
    /// for Interactors and Interactables to be able to communicate.
    /// </summary>
    /// <remarks>
    /// Many of the methods on this class are designed to be internal such that they can be called by the abstract
    /// base classes of the Interaction system (but are not called directly).
    /// </remarks>
    [AddComponentMenu("XR/XR Interaction Manager")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_InteractionManager)]
    [HelpURL(XRHelpURLConstants.k_XRInteractionManager)]
    public class XRInteractionManager : MonoBehaviour
    {
        /// <summary>
        /// Calls the methods in its invocation list when an <see cref="XRBaseInteractor"/> is registered.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="RegisterInteractor"/>
        /// <seealso cref="XRBaseInteractor.registered"/>
        public event Action<InteractorRegisteredEventArgs> interactorRegistered;

        /// <summary>
        /// Calls the methods in its invocation list when an <see cref="XRBaseInteractor"/> is unregistered.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorUnregisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="UnregisterInteractor"/>
        /// <seealso cref="XRBaseInteractor.unregistered"/>
        public event Action<InteractorUnregisteredEventArgs> interactorUnregistered;

        /// <summary>
        /// Calls the methods in its invocation list when an <see cref="XRBaseInteractable"/> is registered.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="RegisterInteractable"/>
        /// <seealso cref="XRBaseInteractable.registered"/>
        public event Action<InteractableRegisteredEventArgs> interactableRegistered;

        /// <summary>
        /// Calls the methods in its invocation list when an <see cref="XRBaseInteractable"/> is unregistered.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableUnregisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="UnregisterInteractable"/>
        /// <seealso cref="XRBaseInteractable.unregistered"/>
        public event Action<InteractableUnregisteredEventArgs> interactableUnregistered;

        // TODO Expose as a read-only wrapper without using ReadOnlyCollection since that class causes allocations when enumerating
        /// <summary>
        /// (Read Only) List of registered interactors.
        /// </summary>
        /// <remarks>
        /// Intended to be used by XR Interaction Debugger and tests.
        /// </remarks>
        internal List<XRBaseInteractor> interactors => m_Interactors;

        // TODO Expose as a read-only wrapper without using ReadOnlyCollection since that class causes allocations when enumerating
        /// <summary>
        /// (Read Only) List of registered interactables.
        /// </summary>
        /// <remarks>
        /// Intended to be used by XR Interaction Debugger and tests.
        /// </remarks>
        internal List<XRBaseInteractable> interactables => m_Interactables;

        /// <summary>
        /// Map of all registered objects to test for colliding.
        /// </summary>
        readonly Dictionary<Collider, XRBaseInteractable> m_ColliderToInteractableMap = new Dictionary<Collider, XRBaseInteractable>();

        /// <summary>
        /// List of registered interactors.
        /// </summary>
        readonly List<XRBaseInteractor> m_Interactors = new List<XRBaseInteractor>();

        /// <summary>
        /// List of registered interactables.
        /// </summary>
        readonly List<XRBaseInteractable> m_Interactables = new List<XRBaseInteractable>();

        /// <summary>
        /// Reusable list of interactables for retrieving hover targets.
        /// </summary>
        readonly List<XRBaseInteractable> m_HoverTargetList = new List<XRBaseInteractable>();

        /// <summary>
        /// Reusable list of valid targets for an Interactor.
        /// </summary>
        readonly List<XRBaseInteractable> m_InteractorValidTargets = new List<XRBaseInteractable>();

        // Reusable event args
        readonly SelectEnterEventArgs m_SelectEnterEventArgs = new SelectEnterEventArgs();
        readonly SelectExitEventArgs m_SelectExitEventArgs = new SelectExitEventArgs();
        readonly HoverEnterEventArgs m_HoverEnterEventArgs = new HoverEnterEventArgs();
        readonly HoverExitEventArgs m_HoverExitEventArgs = new HoverExitEventArgs();
        readonly InteractorRegisteredEventArgs m_InteractorRegisteredEventArgs = new InteractorRegisteredEventArgs();
        readonly InteractorUnregisteredEventArgs m_InteractorUnregisteredEventArgs = new InteractorUnregisteredEventArgs();
        readonly InteractableRegisteredEventArgs m_InteractableRegisteredEventArgs = new InteractableRegisteredEventArgs();
        readonly InteractableUnregisteredEventArgs m_InteractableUnregisteredEventArgs = new InteractableUnregisteredEventArgs();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Dynamic);

            foreach (var interactor in m_Interactors)
            {
                GetValidTargets(interactor, m_InteractorValidTargets);

                ClearInteractorSelection(interactor);
                ClearInteractorHover(interactor, m_InteractorValidTargets);
                InteractorSelectValidTargets(interactor, m_InteractorValidTargets);
                InteractorHoverValidTargets(interactor, m_InteractorValidTargets);
            }

            ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void LateUpdate()
        {
            ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Late);
            ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Late);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Fixed);
            ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Fixed);
        }

        /// <summary>
        /// Delegate method used to register for "Just Before Render" input updates for VR devices.
        /// </summary>
        /// <seealso cref="Application"/>
        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderOrder)]
        protected virtual void OnBeforeRender()
        {
            ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
            ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
        }

        /// <summary>
        ///  Process all interactors in a scene.
        /// </summary>
        /// <param name="updatePhase">The update phase.</param>
        protected virtual void ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            foreach (var interactor in m_Interactors)
            {
                interactor.ProcessInteractor(updatePhase);
            }
        }

        /// <summary>
        /// Process all interactables in a scene.
        /// </summary>
        /// <param name="updatePhase">The update phase.</param>
        protected virtual void ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            foreach (var interactable in m_Interactables)
            {
                interactable.ProcessInteractable(updatePhase);
            }
        }

        /// <summary>
        /// Register a new Interactor to be processed.
        /// </summary>
        /// <param name="interactor">The Interactor to be registered.</param>
        public virtual void RegisterInteractor(XRBaseInteractor interactor)
        {
            if (m_Interactors.Contains(interactor))
                return;

            m_Interactors.Add(interactor);

            m_InteractorRegisteredEventArgs.manager = this;
            m_InteractorRegisteredEventArgs.interactor = interactor;
            interactor.OnRegistered(m_InteractorRegisteredEventArgs);
            interactorRegistered?.Invoke(m_InteractorRegisteredEventArgs);
        }

        /// <summary>
        /// Unregister an Interactor so it is no longer processed.
        /// </summary>
        /// <param name="interactor">The Interactor to be unregistered.</param>
        public virtual void UnregisterInteractor(XRBaseInteractor interactor)
        {
            if (!m_Interactors.Contains(interactor))
                return;

            CancelInteractorSelection(interactor);
            CancelInteractorHover(interactor);

            m_Interactors.Remove(interactor);

            m_InteractorUnregisteredEventArgs.manager = this;
            m_InteractorUnregisteredEventArgs.interactor = interactor;
            interactor.OnUnregistered(m_InteractorUnregisteredEventArgs);
            interactorUnregistered?.Invoke(m_InteractorUnregisteredEventArgs);
        }

        /// <summary>
        /// Register a new Interactable to be processed.
        /// </summary>
        /// <param name="interactable">The Interactable to be registered.</param>
        public virtual void RegisterInteractable(XRBaseInteractable interactable)
        {
            if (m_Interactables.Contains(interactable))
                return;

            m_Interactables.Add(interactable);

            foreach (var interactableCollider in interactable.colliders)
            {
                if (interactableCollider != null && !m_ColliderToInteractableMap.ContainsKey(interactableCollider))
                    m_ColliderToInteractableMap.Add(interactableCollider, interactable);
            }

            m_InteractableRegisteredEventArgs.manager = this;
            m_InteractableRegisteredEventArgs.interactable = interactable;
            interactable.OnRegistered(m_InteractableRegisteredEventArgs);
            interactableRegistered?.Invoke(m_InteractableRegisteredEventArgs);
        }

        /// <summary>
        /// Unregister an Interactable so it is no longer processed.
        /// </summary>
        /// <param name="interactable">The Interactable to be unregistered.</param>
        public virtual void UnregisterInteractable(XRBaseInteractable interactable)
        {
            if (!m_Interactables.Contains(interactable))
                return;

            CancelInteractableSelection(interactable);
            CancelInteractableHover(interactable);

            m_Interactables.Remove(interactable);

            foreach (var interactableCollider in interactable.colliders)
            {
                if (interactableCollider != null)
                    m_ColliderToInteractableMap.Remove(interactableCollider);
            }

            m_InteractableUnregisteredEventArgs.manager = this;
            m_InteractableUnregisteredEventArgs.interactable = interactable;
            interactable.OnUnregistered(m_InteractableUnregisteredEventArgs);
            interactableUnregistered?.Invoke(m_InteractableUnregisteredEventArgs);
        }

        /// <summary>
        /// Return all registered Interactors into List <paramref name="results"/>.
        /// </summary>
        /// <param name="results">List to receive registered Interactors.</param>
        /// <remarks>
        /// This method populates the list with the registered Interactors at the time the
        /// method is called. It is not a live view, meaning Interactors
        /// registered or unregistered afterward will not be reflected in the
        /// results of this method.
        /// </remarks>
        /// <seealso cref="GetRegisteredInteractables"/>
        public void GetRegisteredInteractors(List<XRBaseInteractor> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            results.Clear();
            foreach (var interactor in m_Interactors)
                results.Add(interactor);
        }

        /// <summary>
        /// Return all registered Interactables into List <paramref name="results"/>.
        /// </summary>
        /// <param name="results">List to receive registered Interactables.</param>
        /// <remarks>
        /// This method populates the list with the registered Interactables at the time the
        /// method is called. It is not a live view, meaning Interactables
        /// registered or unregistered afterward will not be reflected in the
        /// results of this method.
        /// </remarks>
        /// <seealso cref="GetRegisteredInteractors"/>
        public void GetRegisteredInteractables(List<XRBaseInteractable> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            results.Clear();
            foreach (var interactable in m_Interactables)
                results.Add(interactable);
        }

        /// <inheritdoc cref="GetInteractableForCollider"/>
        [Obsolete("TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)")]
        public XRBaseInteractable TryGetInteractableForCollider(Collider interactableCollider)
        {
            return GetInteractableForCollider(interactableCollider);
        }

        /// <summary>
        /// Gets the Interactable a specific collider is attached to.
        /// </summary>
        /// <param name="interactableCollider">The collider of the Interactable to retrieve.</param>
        /// <returns>The Interactable that the collider is attached to. Otherwise returns <see langword="null"/> if no such Interactable exists.</returns>
        public XRBaseInteractable GetInteractableForCollider(Collider interactableCollider)
        {
            if (interactableCollider != null && m_ColliderToInteractableMap.TryGetValue(interactableCollider, out var interactable))
                return interactable;

            return null;
        }


        // TODO Expose the dictionary as a read-only wrapper if necessary rather than providing a reference this way
        /// <summary>
        /// Gets the dictionary that has all the registered colliders and their associated Interactable.
        /// </summary>
        /// <param name="map">When this method returns, contains the dictionary that has all the registered colliders and their associated Interactable.</param>
        public void GetColliderToInteractableMap(ref Dictionary<Collider, XRBaseInteractable> map)
        {
            if (map != null)
            {
                map.Clear();
                map = m_ColliderToInteractableMap;
            }
        }

        /// <summary>
        /// For the provided <paramref name="interactor"/>, return a list of the valid interactables that can be hovered over or selected.
        /// </summary>
        /// <param name="interactor">The Interactor whose valid targets we want to find.</param>
        /// <param name="validTargets">List to be filled with valid targets of the Interactor.</param>
        /// <returns>The list of valid targets of the Interactor.</returns>
        public List<XRBaseInteractable> GetValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            interactor.GetValidTargets(validTargets);

            // Remove interactables that are not being handled by this manager.
            for (var i = validTargets.Count - 1; i >= 0; --i)
            {
                if (!m_Interactables.Contains(validTargets[i]))
                    validTargets.RemoveAt(i);
            }

            return validTargets;
        }

        /// <summary>
        /// Force selects an Interactable.
        /// </summary>
        /// <param name="interactor">The Interactor that will force select the Interactable.</param>
        /// <param name="interactable">The Interactable to be forced selected.</param>
        public void ForceSelect(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            SelectEnter(interactor, interactable);
        }

        /// <summary>
        /// Automatically called each frame during Update to clear the selection of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its selection state.</param>
        public virtual void ClearInteractorSelection(XRBaseInteractor interactor)
        {
            if (interactor.selectTarget != null &&
                (!interactor.isSelectActive || !interactor.CanSelect(interactor.selectTarget) || !interactor.selectTarget.IsSelectableBy(interactor)))
            {
                SelectExit(interactor, interactor.selectTarget);
            }
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered to cancel the selection of the Interactor if necessary.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its selection state due to cancellation.</param>
        public virtual void CancelInteractorSelection(XRBaseInteractor interactor)
        {
            if (interactor.selectTarget != null)
                SelectCancel(interactor, interactor.selectTarget);
        }

        /// <summary>
        /// Automatically called when an Interactable is unregistered to cancel the selection of the Interactable if necessary.
        /// </summary>
        /// <param name="interactable">The Interactable to potentially exit its selection state due to cancellation.</param>
        public virtual void CancelInteractableSelection(XRBaseInteractable interactable)
        {
            if (interactable.selectingInteractor != null)
                SelectCancel(interactable.selectingInteractor, interactable);
        }

        /// <summary>
        /// Automatically called each frame during Update to clear the hover state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its hover state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        public virtual void ClearInteractorHover(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            interactor.GetHoverTargets(m_HoverTargetList);
            for (var i = m_HoverTargetList.Count - 1; i >= 0; --i)
            {
                var target = m_HoverTargetList[i];
                if (!interactor.isHoverActive || !interactor.CanHover(target) || !target.IsHoverableBy(interactor) || validTargets == null || !validTargets.Contains(target))
                    HoverExit(interactor, target);
            }
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered to cancel the hover state of the Interactor if necessary.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its hover state due to cancellation.</param>
        public virtual void CancelInteractorHover(XRBaseInteractor interactor)
        {
            interactor.GetHoverTargets(m_HoverTargetList);
            for (var i = m_HoverTargetList.Count - 1; i >= 0; --i)
            {
                var target = m_HoverTargetList[i];
                HoverCancel(interactor, target);
            }
        }

        /// <summary>
        /// Automatically called when an Interactable is unregistered to cancel the hover state of the Interactable if necessary.
        /// </summary>
        /// <param name="interactable">The Interactable to potentially exit its hover state due to cancellation.</param>
        public virtual void CancelInteractableHover(XRBaseInteractable interactable)
        {
            for (var i = interactable.hoveringInteractors.Count - 1; i >= 0; --i)
                HoverCancel(interactable.hoveringInteractors[i], interactable);
        }

        /// <summary>
        /// Initiates selection of an Interactable by an Interactor. This method may cause the Interactable to first exit being selected.
        /// </summary>
        /// <param name="interactor">The Interactor that is selecting.</param>
        /// <param name="interactable">The Interactable being selected.</param>
        /// <remarks>
        /// This attempt will be ignored if the Interactor requires exclusive selection of an Interactable and that
        /// Interactable is already selected by another Interactor.
        /// </remarks>
        public virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            // If Exclusive Selection, is this the only Interactor trying to interact?
            if (interactor.requireSelectExclusive)
            {
                for (var i = 0; i < m_Interactors.Count; ++i)
                {
                    if (m_Interactors[i] != interactor
                        && m_Interactors[i].selectTarget == interactable)
                    {
                        return;
                    }
                }
            }
            else
            {
                for (var i = 0; i < m_Interactors.Count; ++i)
                {
                    if (m_Interactors[i].selectTarget == interactable)
                        SelectExit(m_Interactors[i], interactable);
                }
            }

            m_SelectEnterEventArgs.interactor = interactor;
            m_SelectEnterEventArgs.interactable = interactable;
            SelectEnter(interactor, interactable, m_SelectEnterEventArgs);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        public virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            m_SelectExitEventArgs.interactor = interactor;
            m_SelectExitEventArgs.interactable = interactable;
            m_SelectExitEventArgs.isCanceled = false;
            SelectExit(interactor, interactable, m_SelectExitEventArgs);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor due to cancellation,
        /// such as from either being unregistered due to being disabled or destroyed.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        public virtual void SelectCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            m_SelectExitEventArgs.interactor = interactor;
            m_SelectExitEventArgs.interactable = interactable;
            m_SelectExitEventArgs.isCanceled = true;
            SelectExit(interactor, interactable, m_SelectExitEventArgs);
        }

        /// <summary>
        /// Initiates hovering of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is hovering.</param>
        /// <param name="interactable">The Interactable being hovered over.</param>
        public virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            m_HoverEnterEventArgs.interactor = interactor;
            m_HoverEnterEventArgs.interactable = interactable;
            HoverEnter(interactor, interactable, m_HoverEnterEventArgs);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        public virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            m_HoverExitEventArgs.interactor = interactor;
            m_HoverExitEventArgs.interactable = interactable;
            m_HoverExitEventArgs.isCanceled = false;
            HoverExit(interactor, interactable, m_HoverExitEventArgs);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor due to cancellation,
        /// such as from either being unregistered due to being disabled or destroyed.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        public virtual void HoverCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            m_HoverExitEventArgs.interactor = interactor;
            m_HoverExitEventArgs.interactable = interactable;
            m_HoverExitEventArgs.isCanceled = true;
            HoverExit(interactor, interactable, m_HoverExitEventArgs);
        }

        /// <summary>
        /// Initiates selection of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is selecting.</param>
        /// <param name="interactable">The Interactable being selected.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        protected virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectEnterEventArgs args)
        {
            Debug.Assert(args.interactor == interactor, this);
            Debug.Assert(args.interactable == interactable, this);

            interactor.OnSelectEntering(args);
            interactable.OnSelectEntering(args);
            interactor.OnSelectEntered(args);
            interactable.OnSelectEntered(args);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        protected virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectExitEventArgs args)
        {
            Debug.Assert(args.interactor == interactor, this);
            Debug.Assert(args.interactable == interactable, this);

            interactor.OnSelectExiting(args);
            interactable.OnSelectExiting(args);
            interactor.OnSelectExited(args);
            interactable.OnSelectExited(args);
        }

        /// <summary>
        /// Initiates hovering of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is hovering.</param>
        /// <param name="interactable">The Interactable being hovered over.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        protected virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverEnterEventArgs args)
        {
            Debug.Assert(args.interactor == interactor, this);
            Debug.Assert(args.interactable == interactable, this);

            interactor.OnHoverEntering(args);
            interactable.OnHoverEntering(args);
            interactor.OnHoverEntered(args);
            interactable.OnHoverEntered(args);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        protected virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverExitEventArgs args)
        {
            Debug.Assert(args.interactor == interactor, this);
            Debug.Assert(args.interactable == interactable, this);

            interactor.OnHoverExiting(args);
            interactable.OnHoverExiting(args);
            interactor.OnHoverExited(args);
            interactable.OnHoverExited(args);
        }

        /// <summary>
        /// Automatically called each frame during Update to enter the selection state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially enter its selection state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        protected virtual void InteractorSelectValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            if (interactor.isSelectActive)
            {
                for (var i = 0; i < validTargets.Count && interactor.isSelectActive; ++i)
                {
                    if (interactor.CanSelect(validTargets[i]) && validTargets[i].IsSelectableBy(interactor) &&
                        interactor.selectTarget != validTargets[i])
                    {
                        SelectEnter(interactor, validTargets[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Automatically called each frame during Update to enter the hover state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially enter its hover state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        protected virtual void InteractorHoverValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            if (interactor.isHoverActive)
            {
                for (var i = 0; i < validTargets.Count && interactor.isHoverActive; ++i)
                {
                    interactor.GetHoverTargets(m_HoverTargetList);
                    if (interactor.CanHover(validTargets[i]) && validTargets[i].IsHoverableBy(interactor) &&
                        !m_HoverTargetList.Contains(validTargets[i]))
                    {
                        HoverEnter(interactor, validTargets[i]);
                    }
                }
            }
        }
    }
}
