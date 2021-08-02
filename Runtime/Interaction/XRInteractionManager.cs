using System;
using System.Collections.Generic;
using Unity.Profiling;
#if AR_FOUNDATION_PRESENT
using UnityEngine.XR.Interaction.Toolkit.AR;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The Interaction Manager acts as an intermediary between Interactors and Interactables.
    /// It is possible to have multiple Interaction Managers, each with their own valid set of Interactors and Interactables.
    /// Upon being enabled, both Interactors and Interactables register themselves with a valid Interaction Manager
    /// (if a specific one has not already been assigned in the inspector). The loaded scenes must have at least one Interaction Manager
    /// for Interactors and Interactables to be able to communicate.
    /// </summary>
    /// <remarks>
    /// Many of the methods on the abstract base classes of Interactors and Interactables are designed to be internal
    /// such that they can be called by this Interaction Manager rather than being called directly in order to maintain
    /// consistency between both targets of an interaction event.
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

        /// <summary>
        /// (Read Only) List of enabled Interaction Manager instances.
        /// </summary>
        /// <remarks>
        /// Intended to be used by XR Interaction Debugger.
        /// </remarks>
        internal static List<XRInteractionManager> activeInteractionManagers { get; } = new List<XRInteractionManager>();

        /// <summary>
        /// Map of all registered objects to test for colliding.
        /// </summary>
        readonly Dictionary<Collider, XRBaseInteractable> m_ColliderToInteractableMap = new Dictionary<Collider, XRBaseInteractable>();

        /// <summary>
        /// List of registered Interactors.
        /// </summary>
        readonly RegistrationList<XRBaseInteractor> m_Interactors = new RegistrationList<XRBaseInteractor>();

        /// <summary>
        /// List of registered Interactables.
        /// </summary>
        readonly RegistrationList<XRBaseInteractable> m_Interactables = new RegistrationList<XRBaseInteractable>();

        /// <summary>
        /// Reusable list of Interactables for retrieving hover targets of an Interactor.
        /// </summary>
        readonly List<XRBaseInteractable> m_HoverTargets = new List<XRBaseInteractable>();

        /// <summary>
        /// Reusable set of Interactables for retrieving hover targets of an Interactor.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_UnorderedHoverTargets = new HashSet<XRBaseInteractable>();

        /// <summary>
        /// Reusable list of valid targets for an Interactor.
        /// </summary>
        readonly List<XRBaseInteractable> m_ValidTargets = new List<XRBaseInteractable>();

        /// <summary>
        /// Reusable set of valid targets for an Interactor.
        /// </summary>
        readonly HashSet<XRBaseInteractable> m_UnorderedValidTargets = new HashSet<XRBaseInteractable>();

        // Reusable event args
        readonly SelectEnterEventArgs m_SelectEnterEventArgs = new SelectEnterEventArgs();
        readonly SelectExitEventArgs m_SelectExitEventArgs = new SelectExitEventArgs();
        readonly HoverEnterEventArgs m_HoverEnterEventArgs = new HoverEnterEventArgs();
        readonly HoverExitEventArgs m_HoverExitEventArgs = new HoverExitEventArgs();
        readonly InteractorRegisteredEventArgs m_InteractorRegisteredEventArgs = new InteractorRegisteredEventArgs();
        readonly InteractorUnregisteredEventArgs m_InteractorUnregisteredEventArgs = new InteractorUnregisteredEventArgs();
        readonly InteractableRegisteredEventArgs m_InteractableRegisteredEventArgs = new InteractableRegisteredEventArgs();
        readonly InteractableUnregisteredEventArgs m_InteractableUnregisteredEventArgs = new InteractableUnregisteredEventArgs();

        static readonly ProfilerMarker s_ProcessInteractorsMarker = new ProfilerMarker("XRI.ProcessInteractors");
        static readonly ProfilerMarker s_ProcessInteractablesMarker = new ProfilerMarker("XRI.ProcessInteractables");
        static readonly ProfilerMarker s_GetValidTargetsMarker = new ProfilerMarker("XRI.GetValidTargets");
        static readonly ProfilerMarker s_FilterRegisteredValidTargetsMarker = new ProfilerMarker("XRI.FilterRegisteredValidTargets");
        static readonly ProfilerMarker s_EvaluateInvalidSelectionsMarker = new ProfilerMarker("XRI.EvaluateInvalidSelections");
        static readonly ProfilerMarker s_EvaluateInvalidHoversMarker = new ProfilerMarker("XRI.EvaluateInvalidHovers");
        static readonly ProfilerMarker s_EvaluateValidSelectionsMarker = new ProfilerMarker("XRI.EvaluateValidSelections");
        static readonly ProfilerMarker s_EvaluateValidHoversMarker = new ProfilerMarker("XRI.EvaluateValidHovers");
        static readonly ProfilerMarker s_SelectEnterMarker = new ProfilerMarker("XRI.SelectEnter");
        static readonly ProfilerMarker s_SelectExitMarker = new ProfilerMarker("XRI.SelectExit");
        static readonly ProfilerMarker s_HoverEnterMarker = new ProfilerMarker("XRI.HoverEnter");
        static readonly ProfilerMarker s_HoverExitMarker = new ProfilerMarker("XRI.HoverExit");

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            activeInteractionManagers.Add(this);
            Application.onBeforeRender += OnBeforeRender;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
            activeInteractionManagers.Remove(this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        // ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable -- ProfilerMarker.Begin with context object does not have Pure attribute
        protected virtual void Update()
        {
            FlushRegistration();

            using (s_ProcessInteractorsMarker.Auto())
                ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Dynamic);

            foreach (var interactor in m_Interactors.registeredSnapshot)
            {
                if (!m_Interactors.IsStillRegistered(interactor))
                    continue;

                using (s_GetValidTargetsMarker.Auto())
                    GetValidTargets(interactor, m_ValidTargets);

                using (s_EvaluateInvalidSelectionsMarker.Auto())
                    ClearInteractorSelection(interactor);
                using (s_EvaluateInvalidHoversMarker.Auto())
                    ClearInteractorHover(interactor, m_ValidTargets);
                using (s_EvaluateValidSelectionsMarker.Auto())
                    InteractorSelectValidTargets(interactor, m_ValidTargets);
                using (s_EvaluateValidHoversMarker.Auto())
                    InteractorHoverValidTargets(interactor, m_ValidTargets);
            }

            using (s_ProcessInteractablesMarker.Auto())
                ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void LateUpdate()
        {
            FlushRegistration();

            using (s_ProcessInteractorsMarker.Auto())
                ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Late);
            using (s_ProcessInteractablesMarker.Auto())
                ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Late);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            FlushRegistration();

            using (s_ProcessInteractorsMarker.Auto())
                ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Fixed);
            using (s_ProcessInteractablesMarker.Auto())
                ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Fixed);
        }

        /// <summary>
        /// Delegate method used to register for "Just Before Render" input updates for VR devices.
        /// </summary>
        /// <seealso cref="Application"/>
        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderOrder)]
        protected virtual void OnBeforeRender()
        {
            FlushRegistration();

            using (s_ProcessInteractorsMarker.Auto())
                ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
            using (s_ProcessInteractablesMarker.Auto())
                ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
        }
        // ReSharper restore PossiblyImpureMethodCallOnReadonlyVariable

        /// <summary>
        ///  Process all interactors in a scene.
        /// </summary>
        /// <param name="updatePhase">The update phase.</param>
        protected virtual void ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            foreach (var interactor in m_Interactors.registeredSnapshot)
            {
                if (!m_Interactors.IsStillRegistered(interactor))
                    continue;

                interactor.ProcessInteractor(updatePhase);
            }
        }

        /// <summary>
        /// Process all interactables in a scene.
        /// </summary>
        /// <param name="updatePhase">The update phase.</param>
        protected virtual void ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            foreach (var interactable in m_Interactables.registeredSnapshot)
            {
                if (!m_Interactables.IsStillRegistered(interactable))
                    continue;

                interactable.ProcessInteractable(updatePhase);
            }
        }

        /// <summary>
        /// Register a new Interactor to be processed.
        /// </summary>
        /// <param name="interactor">The Interactor to be registered.</param>
        public virtual void RegisterInteractor(XRBaseInteractor interactor)
        {
            if (m_Interactors.Register(interactor))
            {
                m_InteractorRegisteredEventArgs.manager = this;
                m_InteractorRegisteredEventArgs.interactor = interactor;
                OnRegistered(m_InteractorRegisteredEventArgs);
            }
        }

        /// <summary>
        /// Automatically called when an Interactor is registered with this Interaction Manager.
        /// Notifies the Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Event data containing the registered Interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="RegisterInteractor"/>
        protected virtual void OnRegistered(InteractorRegisteredEventArgs args)
        {
            Debug.Assert(args.manager == this, this);

            args.interactor.OnRegistered(args);
            interactorRegistered?.Invoke(args);
        }

        /// <summary>
        /// Unregister an Interactor so it is no longer processed.
        /// </summary>
        /// <param name="interactor">The Interactor to be unregistered.</param>
        public virtual void UnregisterInteractor(XRBaseInteractor interactor)
        {
            if (!IsRegistered(interactor))
                return;

            CancelInteractorSelection(interactor);
            CancelInteractorHover(interactor);

            if (m_Interactors.Unregister(interactor))
            {
                m_InteractorUnregisteredEventArgs.manager = this;
                m_InteractorUnregisteredEventArgs.interactor = interactor;
                OnUnregistered(m_InteractorUnregisteredEventArgs);
            }
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered from this Interaction Manager.
        /// Notifies the Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Event data containing the unregistered Interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="UnregisterInteractor"/>
        protected virtual void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            Debug.Assert(args.manager == this, this);

            args.interactor.OnUnregistered(args);
            interactorUnregistered?.Invoke(args);
        }

        /// <summary>
        /// Register a new Interactable to be processed.
        /// </summary>
        /// <param name="interactable">The Interactable to be registered.</param>
        public virtual void RegisterInteractable(XRBaseInteractable interactable)
        {
            if (m_Interactables.Register(interactable))
            {
                foreach (var interactableCollider in interactable.colliders)
                {
                    if (interactableCollider == null)
                        continue;

                    // Add the association for a fast lookup which maps from Collider to Interactable.
                    // Warn if the same Collider is already used by another registered Interactable
                    // since the lookup will only return the earliest registered rather than a list of all.
                    // The warning is suppressed in the case of gesture interactables since it's common
                    // to compose multiple on the same GameObject.
                    if (!m_ColliderToInteractableMap.TryGetValue(interactableCollider, out var associatedInteractable))
                    {
                        m_ColliderToInteractableMap.Add(interactableCollider, interactable);
                    }
#if AR_FOUNDATION_PRESENT
                    else if (!(interactable is ARBaseGestureInteractable && associatedInteractable is ARBaseGestureInteractable))
#else
                    else
#endif
                    {
                        Debug.LogWarning("A Collider used by an Interactable object is already registered with another Interactable object." +
                            $" The {interactableCollider} will remain associated with {associatedInteractable}, which was registered before {interactable}." +
                            $" The value returned by {nameof(XRInteractionManager)}.{nameof(GetInteractableForCollider)} will be the first association.",
                            interactable);
                    }
                }

                m_InteractableRegisteredEventArgs.manager = this;
                m_InteractableRegisteredEventArgs.interactable = interactable;
                OnRegistered(m_InteractableRegisteredEventArgs);
            }
        }

        /// <summary>
        /// Automatically called when an Interactable is registered with this Interaction Manager.
        /// Notifies the Interactable, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Event data containing the registered Interactable.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="RegisterInteractable"/>
        protected virtual void OnRegistered(InteractableRegisteredEventArgs args)
        {
            Debug.Assert(args.manager == this, this);

            args.interactable.OnRegistered(args);
            interactableRegistered?.Invoke(args);
        }

        /// <summary>
        /// Unregister an Interactable so it is no longer processed.
        /// </summary>
        /// <param name="interactable">The Interactable to be unregistered.</param>
        public virtual void UnregisterInteractable(XRBaseInteractable interactable)
        {
            if (!IsRegistered(interactable))
                return;

            CancelInteractableSelection(interactable);
            CancelInteractableHover(interactable);

            if (m_Interactables.Unregister(interactable))
            {
                // This makes the assumption that the list of Colliders has not been changed after
                // the Interactable is registered. If any were removed afterward, those would remain
                // in the dictionary.
                foreach (var interactableCollider in interactable.colliders)
                {
                    if (interactableCollider == null)
                        continue;

                    if (m_ColliderToInteractableMap.TryGetValue(interactableCollider, out var associatedInteractable) && associatedInteractable == interactable)
                        m_ColliderToInteractableMap.Remove(interactableCollider);
                }

                m_InteractableUnregisteredEventArgs.manager = this;
                m_InteractableUnregisteredEventArgs.interactable = interactable;
                OnUnregistered(m_InteractableUnregisteredEventArgs);
            }
        }

        /// <summary>
        /// Automatically called when an Interactable is unregistered from this Interaction Manager.
        /// Notifies the Interactable, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Event data containing the unregistered Interactable.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="UnregisterInteractable"/>
        protected virtual void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
            Debug.Assert(args.manager == this, this);

            args.interactable.OnUnregistered(args);
            interactableUnregistered?.Invoke(args);
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

            m_Interactors.GetRegisteredItems(results);
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

            m_Interactables.GetRegisteredItems(results);
        }

        /// <summary>
        /// Checks whether the <paramref name="interactor"/> is registered with this Interaction Manager.
        /// </summary>
        /// <param name="interactor">The Interactor to check.</param>
        /// <returns>Returns <see langword="true"/> if registered. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="RegisterInteractor"/>
        public bool IsRegistered(XRBaseInteractor interactor)
        {
            return m_Interactors.IsRegistered(interactor);
        }

        /// <summary>
        /// Checks whether the <paramref name="interactable"/> is registered with this Interaction Manager.
        /// </summary>
        /// <param name="interactable">The Interactable to check.</param>
        /// <returns>Returns <see langword="true"/> if registered. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="RegisterInteractable"/>
        public bool IsRegistered(XRBaseInteractable interactable)
        {
            return m_Interactables.IsRegistered(interactable);
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
        /// For the provided <paramref name="interactor"/>, return a list of the valid Interactables that can be hovered over or selected.
        /// </summary>
        /// <param name="interactor">The Interactor whose valid targets we want to find.</param>
        /// <param name="validTargets">List to be filled with valid targets of the Interactor.</param>
        /// <returns>The list of valid targets of the Interactor.</returns>
        /// <seealso cref="XRBaseInteractor.GetValidTargets"/>
        public List<XRBaseInteractable> GetValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            interactor.GetValidTargets(validTargets);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable -- ProfilerMarker.Begin with context object does not have Pure attribute
            using (s_FilterRegisteredValidTargetsMarker.Auto())
                RemoveAllUnregistered(this, validTargets);

            return validTargets;
        }

        /// <summary>
        /// Removes all the Interactables from the given list that are not being handled by the manager.
        /// </summary>
        /// <param name="manager">The Interaction Manager to check registration against.</param>
        /// <param name="interactables">List of elements that will be filtered to exclude those not registered.</param>
        /// <returns>Returns the number of elements removed from the list.</returns>
        /// <remarks>
        /// Does not modify the manager at all, just the list.
        /// </remarks>
        internal static int RemoveAllUnregistered(XRInteractionManager manager, List<XRBaseInteractable> interactables)
        {
            var numRemoved = 0;
            for (var i = interactables.Count - 1; i >= 0; --i)
            {
                if (!manager.m_Interactables.IsRegistered(interactables[i]))
                {
                    interactables.RemoveAt(i);
                    ++numRemoved;
                }
            }

            return numRemoved;
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
            interactor.GetHoverTargets(m_HoverTargets);
            if (m_HoverTargets.Count == 0)
                return;

            // Performance optimization of the Contains checks by putting the valid targets into a HashSet.
            // Some Interactors like ARGestureInteractor can have hundreds of valid Interactables
            // since they will add most ARBaseGestureInteractable instances.
            m_UnorderedValidTargets.Clear();
            if (validTargets.Count > 0)
            {
                foreach (var target in validTargets)
                {
                    m_UnorderedValidTargets.Add(target);
                }
            }

            for (var i = m_HoverTargets.Count - 1; i >= 0; --i)
            {
                var target = m_HoverTargets[i];
                if (!interactor.isHoverActive || !interactor.CanHover(target) || !target.IsHoverableBy(interactor) || !m_UnorderedValidTargets.Contains(target))
                    HoverExit(interactor, target);
            }
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered to cancel the hover state of the Interactor if necessary.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its hover state due to cancellation.</param>
        public virtual void CancelInteractorHover(XRBaseInteractor interactor)
        {
            interactor.GetHoverTargets(m_HoverTargets);
            for (var i = m_HoverTargets.Count - 1; i >= 0; --i)
            {
                var target = m_HoverTargets[i];
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
            if (interactable.isSelected && interactable.selectingInteractor != interactor)
            {
                if (interactor.requireSelectExclusive)
                    return;

                SelectExit(interactable.selectingInteractor, interactable);
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
        // ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable -- ProfilerMarker.Begin with context object does not have Pure attribute
        protected virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectEnterEventArgs args)
        {
            Debug.Assert(args.interactor == interactor, this);
            Debug.Assert(args.interactable == interactable, this);

            using (s_SelectEnterMarker.Auto())
            {
                interactor.OnSelectEntering(args);
                interactable.OnSelectEntering(args);
                interactor.OnSelectEntered(args);
                interactable.OnSelectEntered(args);
            }
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

            using (s_SelectExitMarker.Auto())
            {
                interactor.OnSelectExiting(args);
                interactable.OnSelectExiting(args);
                interactor.OnSelectExited(args);
                interactable.OnSelectExited(args);
            }
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

            using (s_HoverEnterMarker.Auto())
            {
                interactor.OnHoverEntering(args);
                interactable.OnHoverEntering(args);
                interactor.OnHoverEntered(args);
                interactable.OnHoverEntered(args);
            }
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

            using (s_HoverExitMarker.Auto())
            {
                interactor.OnHoverExiting(args);
                interactable.OnHoverExiting(args);
                interactor.OnHoverExited(args);
                interactable.OnHoverExited(args);
            }
        }
        // ReSharper restore PossiblyImpureMethodCallOnReadonlyVariable

        /// <summary>
        /// Automatically called each frame during Update to enter the selection state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially enter its selection state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        protected virtual void InteractorSelectValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            if (!interactor.isSelectActive)
                return;

            for (var i = 0; i < validTargets.Count && interactor.isSelectActive; ++i)
            {
                var interactable = validTargets[i];
                if (interactor.CanSelect(interactable) && interactable.IsSelectableBy(interactor) &&
                    interactor.selectTarget != interactable)
                {
                    SelectEnter(interactor, interactable);
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
            if (!interactor.isHoverActive)
                return;

            // For performance optimization, this is done once instead of within each iteration.
            // Makes the assumption that the process of entering the hover state does not
            // cause other hover targets to be cleared or canceled, which could affect this list.
            interactor.GetHoverTargets(m_HoverTargets);
            m_UnorderedHoverTargets.Clear();
            if (m_HoverTargets.Count > 0)
            {
                foreach (var target in m_HoverTargets)
                {
                    m_UnorderedHoverTargets.Add(target);
                }
            }

            for (var i = 0; i < validTargets.Count && interactor.isHoverActive; ++i)
            {
                var interactable = validTargets[i];
                if (interactor.CanHover(interactable) && interactable.IsHoverableBy(interactor))
                {
                    if (!m_UnorderedHoverTargets.Contains(interactable))
                    {
                        HoverEnter(interactor, interactable);

                        // Add to the set to protect against the valid targets list containing duplicates
                        // which could cause the hover state to be entered again.
                        m_UnorderedHoverTargets.Add(interactable);
                    }
                }
            }
        }

        void FlushRegistration()
        {
            m_Interactors.Flush();
            m_Interactables.Flush();
        }

        /// <summary>
        /// Use this class to maintain a registration of Interactors or Interactables. This maintains
        /// a synchronized list that stays constant until buffered registration status changes are
        /// explicitly committed.
        /// </summary>
        /// <typeparam name="T">The type of object to register, i.e. <see cref="XRBaseInteractor"/> or <see cref="XRBaseInteractable"/>.</typeparam>
        /// <remarks>
        /// Objects may be registered or unregistered from an Interaction Manager
        /// at any time, including when processing objects.
        /// For consistency with the functionality of Unity components which do not have
        /// Update called the same frame in which they are enabled, disabled, or destroyed,
        /// this class will maintain multiple lists to achieve that desired result with processing
        /// Interactors and Interactables.
        /// </remarks>
        internal class RegistrationList<T>
        {
            /// <summary>
            /// A snapshot of registered items that should potentially be processed this update phase of the current frame.
            /// The count of items shall only change upon a call to <see cref="Flush"/>.
            /// </summary>
            /// <remarks>
            /// Items being in this collection does not imply that the item is currently registered.
            /// <br />
            /// Logically this should be a <see cref="IReadOnlyList{T}"/> but is kept as a <see cref="List{T}"/>
            /// to avoid allocations when iterating. Use <see cref="Register"/> and <see cref="Unregister"/>
            /// instead of directly changing this list.
            /// </remarks>
            public List<T> registeredSnapshot { get; } = new List<T>();

            readonly List<T> m_BufferedAdd = new List<T>();
            readonly List<T> m_BufferedRemove = new List<T>();

            readonly HashSet<T> m_UnorderedBufferedAdd = new HashSet<T>();
            readonly HashSet<T> m_UnorderedBufferedRemove = new HashSet<T>();
            readonly HashSet<T> m_UnorderedRegisteredSnapshot = new HashSet<T>();
            readonly HashSet<T> m_UnorderedRegisteredItems = new HashSet<T>();

            /// <summary>
            /// Checks the registration status of <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The item to query.</param>
            /// <returns>Returns <see langword="true"/> if registered. Otherwise, returns <see langword="false"/>.</returns>
            /// <remarks>
            /// This includes pending changes that have not yet been pushed to <see cref="registeredSnapshot"/>.
            /// </remarks>
            /// <seealso cref="IsStillRegistered"/>

            public bool IsRegistered(T item) => m_UnorderedRegisteredItems.Contains(item);

            /// <summary>
            /// Faster variant of <see cref="IsRegistered"/> that assumes that the <paramref name="item"/> is in the snapshot.
            /// It short circuits the check when there are no pending changes to unregister, which is usually the case.
            /// </summary>
            /// <param name="item">The item to query.</param>
            /// <returns>Returns <see langword="true"/> if registered</returns>
            /// <remarks>
            /// This includes pending changes that have not yet been pushed to <see cref="registeredSnapshot"/>.
            /// Use this method instead of <see cref="IsRegistered"/> when iterating over <see cref="registeredSnapshot"/>
            /// for improved performance.
            /// </remarks>
            /// <seealso cref="IsRegistered"/>
            public bool IsStillRegistered(T item) => m_UnorderedBufferedRemove.Count == 0 || !m_UnorderedBufferedRemove.Contains(item);

            /// <summary>
            /// Register <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The item to register.</param>
            /// <returns>Returns <see langword="true"/> if a change in registration status occurred. Otherwise, returns <see langword="false"/>.</returns>
            public bool Register(T item)
            {
                if (m_UnorderedBufferedAdd.Count > 0 && m_UnorderedBufferedAdd.Contains(item))
                    return false;

                if ((m_UnorderedBufferedRemove.Count > 0 && m_UnorderedBufferedRemove.Remove(item)) || !m_UnorderedRegisteredSnapshot.Contains(item))
                {
                    m_BufferedRemove.Remove(item);
                    m_BufferedAdd.Add(item);
                    m_UnorderedBufferedAdd.Add(item);
                    m_UnorderedRegisteredItems.Add(item);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Unregister <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The item to unregister.</param>
            /// <returns>Returns <see langword="true"/> if a change in registration status occurred. Otherwise, returns <see langword="false"/>.</returns>
            public bool Unregister(T item)
            {
                if (m_UnorderedBufferedRemove.Count > 0 && m_BufferedRemove.Contains(item))
                    return false;

                if ((m_UnorderedBufferedAdd.Count > 0 && m_UnorderedBufferedAdd.Remove(item)) || m_UnorderedRegisteredSnapshot.Contains(item))
                {
                    m_BufferedAdd.Remove(item);
                    m_BufferedRemove.Add(item);
                    m_UnorderedBufferedRemove.Add(item);
                    m_UnorderedRegisteredItems.Remove(item);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Flush pending registration changes into <see cref="registeredSnapshot"/>.
            /// </summary>
            public void Flush()
            {
                // This method is called multiple times each frame,
                // so additional explicit Count checks are done for
                // performance.
                if (m_BufferedRemove.Count > 0)
                {
                    foreach (var item in m_BufferedRemove)
                    {
                        registeredSnapshot.Remove(item);
                        m_UnorderedRegisteredSnapshot.Remove(item);
                    }

                    m_BufferedRemove.Clear();
                    m_UnorderedBufferedRemove.Clear();
                }

                if (m_BufferedAdd.Count > 0)
                {
                    foreach (var item in m_BufferedAdd)
                    {
                        if (!m_UnorderedRegisteredSnapshot.Contains(item))
                        {
                            registeredSnapshot.Add(item);
                            m_UnorderedRegisteredSnapshot.Add(item);
                        }
                    }

                    m_BufferedAdd.Clear();
                    m_UnorderedBufferedAdd.Clear();
                }
            }

            /// <summary>
            /// Return all registered items into List <paramref name="results"/> in the order they were registered.
            /// </summary>
            /// <param name="results">List to receive registered items.</param>
            /// <remarks>
            /// Clears <paramref name="results"/> before adding to it.
            /// </remarks>
            public void GetRegisteredItems(List<T> results)
            {
                if (results == null)
                    throw new ArgumentNullException(nameof(results));

                results.Clear();
                EnsureCapacity(results, registeredSnapshot.Count - m_BufferedRemove.Count + m_BufferedAdd.Count);
                foreach (var item in registeredSnapshot)
                {
                    if (m_UnorderedBufferedRemove.Count > 0 && m_UnorderedBufferedRemove.Contains(item))
                        continue;

                    results.Add(item);
                }

                results.AddRange(m_BufferedAdd);
            }

            static void EnsureCapacity(List<T> list, int capacity)
            {
                if (list.Capacity < capacity)
                    list.Capacity = capacity;
            }
        }
    }
}
