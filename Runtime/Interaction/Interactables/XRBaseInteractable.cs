using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit.Utilities;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Abstract base class from which all interactable behaviours derive.
    /// This class hooks into the interaction system (via <see cref="XRInteractionManager"/>) and provides base virtual methods for handling
    /// hover and selection.
    /// </summary>
    [SelectionBase]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_Interactables)]
    public abstract partial class XRBaseInteractable : MonoBehaviour
    {
        /// <summary>
        /// Type of movement for an interactable.
        /// </summary>
        public enum MovementType
        {
            /// <summary>
            /// In VelocityTracking mode, the Rigidbody associated with the will have velocity and angular velocity added to it such that the interactable attach point will follow the interactor attach point
            /// as this is applying forces to the Rigidbody, this will appear to be a slight distance behind the visual representation of the Interactor / Controller.
            /// </summary>
            VelocityTracking,

            /// <summary>
            /// In Kinematic mode the Rigidbody associated with the interactable will be moved such that the interactable attach point will match the interactor attach point
            /// as this is updating the Rigidbody, this will appear a frame behind the visual representation of the Interactor / Controller.
            /// </summary>
            Kinematic,

            /// <summary>
            /// In Instantaneous Mode the interactable's transform is updated such that the interactable attach point will match the interactor's attach point.
            /// As this is updating the transform directly, any Rigidbody attached to the GameObject that the interactable component is on will be disabled while being interacted with so
            /// that any motion will not "judder" due to the Rigidbody interfering with motion.
            /// </summary>
            Instantaneous,
        }

        /// <summary>
        /// Calls the methods in its invocation list when this Interactable is registered with an Interaction Manager.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.interactableRegistered"/>
        public event Action<InteractableRegisteredEventArgs> registered;

        /// <summary>
        /// Calls the methods in its invocation list when this Interactable is unregistered from an Interaction Manager.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableUnregisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.interactableUnregistered"/>
        public event Action<InteractableUnregisteredEventArgs> unregistered;

        [SerializeField]
        XRInteractionManager m_InteractionManager;

        /// <summary>
        /// The <see cref="XRInteractionManager"/> that this Interactable will communicate with (will find one if <see langword="null"/>).
        /// </summary>
        public XRInteractionManager interactionManager
        {
            get => m_InteractionManager;
            set
            {
                m_InteractionManager = value;
                if (Application.isPlaying && isActiveAndEnabled)
                    RegisterWithInteractionManager();
            }
        }

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier -- readonly fields cannot be serialized by Unity
        List<Collider> m_Colliders = new List<Collider>();
#pragma warning restore IDE0044

        /// <summary>
        /// (Read Only) Colliders to use for interaction with this interactable (if empty, will use any child Colliders).
        /// </summary>
        public List<Collider> colliders => m_Colliders;

        [SerializeField]
        LayerMask m_InteractionLayerMask = -1;

        /// <summary>
        /// Allows interaction with Interactors whose Interaction Layer Mask overlaps with any layer in this Interaction Layer Mask.
        /// </summary>
        /// <seealso cref="XRBaseInteractor.interactionLayerMask"/>
        /// <seealso cref="IsHoverableBy"/>
        /// <seealso cref="IsSelectableBy"/>
        public LayerMask interactionLayerMask
        {
            get => m_InteractionLayerMask;
            set => m_InteractionLayerMask = value;
        }

        [SerializeField]
        GameObject m_CustomReticle;

        /// <summary>
        /// The reticle that will appear at the end of the line when it is valid.
        /// </summary>
        public GameObject customReticle
        {
            get => m_CustomReticle;
            set => m_CustomReticle = value;
        }

        [SerializeField]
        HoverEnterEvent m_FirstHoverEntered = new HoverEnterEvent();

        /// <summary>
        /// Gets or sets the event that is called only when the first Interactor begins hovering
        /// over this Interactable as the sole hovering Interactor. Subsequent Interactors that
        /// begin hovering over this Interactable will not cause this event to be invoked as
        /// long as any others are still hovering.
        /// </summary>
        /// <remarks>
        /// The <see cref="HoverEnterEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="lastHoverExited"/>
        /// <seealso cref="hoverEntered"/>
        public HoverEnterEvent firstHoverEntered
        {
            get => m_FirstHoverEntered;
            set => m_FirstHoverEntered = value;
        }

        [SerializeField]
        HoverExitEvent m_LastHoverExited = new HoverExitEvent();

        /// <summary>
        /// Gets or sets the event that is called only when the last remaining hovering Interactor
        /// ends hovering over this Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="HoverExitEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="firstHoverEntered"/>
        /// <seealso cref="hoverExited"/>
        public HoverExitEvent lastHoverExited
        {
            get => m_LastHoverExited;
            set => m_LastHoverExited = value;
        }

        [SerializeField]
        HoverEnterEvent m_HoverEntered = new HoverEnterEvent();

        /// <summary>
        /// Gets or sets the event that is called when an Interactor begins hovering over this Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="HoverEnterEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="hoverExited"/>
        public HoverEnterEvent hoverEntered
        {
            get => m_HoverEntered;
            set => m_HoverEntered = value;
        }

        [SerializeField]
        HoverExitEvent m_HoverExited = new HoverExitEvent();

        /// <summary>
        /// Gets or sets the event that is called when an Interactor ends hovering over this Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="HoverExitEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="hoverEntered"/>
        public HoverExitEvent hoverExited
        {
            get => m_HoverExited;
            set => m_HoverExited = value;
        }

        [SerializeField]
        SelectEnterEvent m_SelectEntered = new SelectEnterEvent();

        /// <summary>
        /// Gets or sets the event that is called when an Interactor begins selecting this Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="SelectEnterEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="selectExited"/>
        public SelectEnterEvent selectEntered
        {
            get => m_SelectEntered;
            set => m_SelectEntered = value;
        }

        [SerializeField]
        SelectExitEvent m_SelectExited = new SelectExitEvent();

        /// <summary>
        /// Gets or sets the event that is called when an Interactor ends selecting this Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="SelectExitEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="selectEntered"/>
        public SelectExitEvent selectExited
        {
            get => m_SelectExited;
            set => m_SelectExited = value;
        }

        [SerializeField]
        ActivateEvent m_Activated = new ActivateEvent();

        /// <summary>
        /// Gets or sets the event that is called when the selecting Interactor activates this Interactable.
        /// </summary>
        /// <remarks>
        /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
        /// This is a generic event when an Interactor wants to activate its selected Interactable,
        /// such as from a trigger pull on a controller.
        /// <br />
        /// The <see cref="ActivateEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="deactivated"/>
        public ActivateEvent activated
        {
            get => m_Activated;
            set => m_Activated = value;
        }

        [SerializeField]
        DeactivateEvent m_Deactivated = new DeactivateEvent();

        /// <summary>
        /// Gets or sets the event that is called when an Interactor deactivates this selected Interactable.
        /// </summary>
        /// <remarks>
        /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
        /// This is a generic event when an Interactor wants to deactivate its selected Interactable,
        /// such as from a trigger pull on a controller.
        /// <br />
        /// The <see cref="DeactivateEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="activated"/>
        public DeactivateEvent deactivated
        {
            get => m_Deactivated;
            set => m_Deactivated = value;
        }

        readonly List<XRBaseInteractor> m_HoveringInteractors = new List<XRBaseInteractor>();

        /// <summary>
        /// (Read Only) The list of interactors that are hovering on this interactable.
        /// </summary>
        /// <seealso cref="isHovered"/>
        /// <seealso cref="XRBaseInteractor.hoverTargets"/>
        public List<XRBaseInteractor> hoveringInteractors => m_HoveringInteractors;

        /// <summary>
        /// (Read Only) The interactor that is selecting this interactable (may be <see langword="null"/>).
        /// </summary>
        /// <seealso cref="isSelected"/>
        /// <seealso cref="XRBaseInteractor.selectTarget"/>
        public XRBaseInteractor selectingInteractor { get; private set; }

        /// <summary>
        /// (Read Only) Indicates whether this interactable is currently being hovered.
        /// </summary>
        /// <seealso cref="hoveringInteractors"/>
        public bool isHovered { get; private set; }

        /// <summary>
        /// (Read Only) Indicates whether this interactable is currently being selected.
        /// </summary>
        /// <seealso cref="selectingInteractor"/>
        public bool isSelected { get; private set; }

        readonly Dictionary<XRBaseInteractor, GameObject> m_ReticleCache = new Dictionary<XRBaseInteractor, GameObject>();

        XRInteractionManager m_RegisteredInteractionManager;

        /// <summary>
        /// Cached reference to an <see cref="XRInteractionManager"/> found with <see cref="Object.FindObjectOfType"/>.
        /// </summary>
        static XRInteractionManager s_InteractionManagerCache;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            m_InteractionManager = EditorComponentLocatorUtility.FindSceneComponentOfType<XRInteractionManager>(gameObject);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            // If no colliders were set, populate with children colliders
            if (m_Colliders.Count == 0)
                GetComponentsInChildren(m_Colliders);

            // Setup Interaction Manager
            FindCreateInteractionManager();

            // Warn about use of deprecated events
            if (m_OnFirstHoverEntered.GetPersistentEventCount() > 0 ||
                m_OnLastHoverExited.GetPersistentEventCount() > 0 ||
                m_OnHoverEntered.GetPersistentEventCount() > 0 ||
                m_OnHoverExited.GetPersistentEventCount() > 0 ||
                m_OnSelectEntered.GetPersistentEventCount() > 0 ||
                m_OnSelectExited.GetPersistentEventCount() > 0 ||
                m_OnSelectCanceled.GetPersistentEventCount() > 0 ||
                m_OnActivate.GetPersistentEventCount() > 0 ||
                m_OnDeactivate.GetPersistentEventCount() > 0)
            {
                Debug.LogWarning("Some deprecated Interactable Events are being used. These deprecated events will be removed in a future version." +
                    " Please convert these to use the newer events, and update script method signatures for Dynamic listeners.", this);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            FindCreateInteractionManager();
            RegisterWithInteractionManager();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            UnregisterWithInteractionManager();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            // Don't need to do anything; method kept for backwards compatibility.
        }

        void FindCreateInteractionManager()
        {
            if (m_InteractionManager != null)
                return;

            if (s_InteractionManagerCache == null)
                s_InteractionManagerCache = FindObjectOfType<XRInteractionManager>();

            if (s_InteractionManagerCache == null)
            {
                var interactionManagerGO = new GameObject("XR Interaction Manager", typeof(XRInteractionManager));
                s_InteractionManagerCache = interactionManagerGO.GetComponent<XRInteractionManager>();
            }

            m_InteractionManager = s_InteractionManagerCache;
        }

        void RegisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager == m_InteractionManager)
                return;

            UnregisterWithInteractionManager();

            if (m_InteractionManager != null)
            {
                m_InteractionManager.RegisterInteractable(this);
                m_RegisteredInteractionManager = m_InteractionManager;
            }
        }

        void UnregisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager == null)
                return;

            m_RegisteredInteractionManager.UnregisterInteractable(this);
            m_RegisteredInteractionManager = null;
        }

        /// <summary>
        /// Calculates distance squared to interactor (based on colliders).
        /// </summary>
        /// <param name="interactor">Interactor to calculate distance against.</param>
        /// <returns>Returns the minimum distance between the interactor and this interactable's colliders.</returns>
        public float GetDistanceSqrToInteractor(XRBaseInteractor interactor)
        {
            if (interactor == null)
                return float.MaxValue;

            var minDistanceSqr = float.MaxValue;
            foreach (var col in m_Colliders)
            {
                var offset = (interactor.attachTransform.position - col.transform.position);
                minDistanceSqr = Mathf.Min(offset.sqrMagnitude, minDistanceSqr);
            }
            return minDistanceSqr;
        }

        bool IsOnValidLayerMask(XRBaseInteractor interactor)
        {
            return (m_InteractionLayerMask & interactor.interactionLayerMask) != 0;
        }

        /// <summary>
        /// Determines if this interactable can be hovered by a given interactor.
        /// </summary>
        /// <param name="interactor">Interactor to check for a valid hover state with.</param>
        /// <returns>Returns <see langword="true"/> if hovering is valid this frame. Returns <see langword="false"/> if not.</returns>
        /// <seealso cref="XRBaseInteractor.CanHover"/>
        public virtual bool IsHoverableBy(XRBaseInteractor interactor) => IsOnValidLayerMask(interactor);

        /// <summary>
        /// Determines if this interactable can be selected by a given interactor.
        /// </summary>
        /// <param name="interactor">Interactor to check for a valid selection with.</param>
        /// <returns>Returns <see langword="true"/> if selection is valid this frame. Returns <see langword="false"/> if not.</returns>
        /// <seealso cref="XRBaseInteractor.CanSelect"/>
        public virtual bool IsSelectableBy(XRBaseInteractor interactor) => IsOnValidLayerMask(interactor);

        /// <summary>
        /// Attaches a custom reticle to the Interactable associated with an Interactor.
        /// </summary>
        /// <param name="interactor">Interactor that is interacting with the Interactable.</param>
        public virtual void AttachCustomReticle(XRBaseInteractor interactor)
        {
            if (interactor == null)
                return;

            // Try and find any attached reticle and swap it
            var reticleProvider = interactor.GetComponent<IXRCustomReticleProvider>();
            if (reticleProvider != null)
            {
                if (m_ReticleCache.TryGetValue(interactor, out var prevReticle))
                {
                    Destroy(prevReticle);
                    m_ReticleCache.Remove(interactor);
                }

                if (m_CustomReticle != null)
                {
                    var reticleInstance = Instantiate(m_CustomReticle);
                    m_ReticleCache.Add(interactor, reticleInstance);
                    reticleProvider.AttachCustomReticle(reticleInstance);
                }
            }
        }

        /// <summary>
        /// Removes a custom reticle so that it is no longer displayed on the Interactable.
        /// </summary>
        /// <param name="interactor">Interactor that is no longer interacting with the Interactable.</param>
        public virtual void RemoveCustomReticle(XRBaseInteractor interactor)
        {
            if (interactor == null)
                return;

            // Try and find any attached reticle and swap it
            var reticleProvider = interactor.transform.GetComponent<IXRCustomReticleProvider>();
            if (reticleProvider != null)
            {
                if (m_ReticleCache.TryGetValue(interactor, out var reticle))
                {
                    Destroy(reticle);
                    m_ReticleCache.Remove(interactor);
                    reticleProvider.RemoveCustomReticle();
                }
            }
        }

        /// <summary>
        /// This method is called by the Interaction Manager to update the Interactable.
        /// Please see the Interaction Manager documentation for more details on update order.
        /// </summary>
        /// <param name="updatePhase">The update phase this is called during.</param>
        public virtual void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when this Interactable is registered with it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that registered this Interactable.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.RegisterInteractable"/>
        protected internal virtual void OnRegistered(InteractableRegisteredEventArgs args)
        {
            if (args.manager != m_InteractionManager)
                Debug.LogWarning($"An Interactable was registered with an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_InteractionManager}\" but was registered with \"{args.manager}\".", this);

            registered?.Invoke(args);
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when this Interactable is unregistered from it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that unregistered this Interactable.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.UnregisterInteractable"/>
        protected internal virtual void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
            if (args.manager != m_RegisteredInteractionManager)
                Debug.LogWarning($"An Interactable was unregistered from an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_RegisteredInteractionManager}\" but was unregistered from \"{args.manager}\".", this);

            unregistered?.Invoke(args);
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is initiating the hover.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverEntered(HoverEnterEventArgs)"/>
        protected internal virtual void OnHoverEntering(HoverEnterEventArgs args)
        {
            if (m_CustomReticle != null)
                AttachCustomReticle(args.interactor);

            isHovered = true;
            m_HoveringInteractors.Add(args.interactor);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverEntering(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is initiating the hover.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverExited(HoverExitEventArgs)"/>
        protected internal virtual void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (m_HoveringInteractors.Count == 1)
                m_FirstHoverEntered?.Invoke(args);

            m_HoverEntered?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverEntered(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is ending the hover.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverExited(HoverExitEventArgs)"/>
        protected internal virtual void OnHoverExiting(HoverExitEventArgs args)
        {
            if (m_CustomReticle != null)
                RemoveCustomReticle(args.interactor);

            isHovered = false;
            m_HoveringInteractors.Remove(args.interactor);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverExiting(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is ending the hover.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverEntered(HoverEnterEventArgs)"/>
        protected internal virtual void OnHoverExited(HoverExitEventArgs args)
        {
            if (m_HoveringInteractors.Count == 0)
                m_LastHoverExited?.Invoke(args);

            m_HoverExited?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverExited(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is initiating the selection.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectEntered(SelectEnterEventArgs)"/>
        protected internal virtual void OnSelectEntering(SelectEnterEventArgs args)
        {
            isSelected = true;
            selectingInteractor = args.interactor;

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectEntering(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is initiating the selection.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectExited(SelectExitEventArgs)"/>
        protected internal virtual void OnSelectEntered(SelectEnterEventArgs args)
        {
            m_SelectEntered?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectEntered(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is ending the selection.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectExited(SelectExitEventArgs)"/>
        protected internal virtual void OnSelectExiting(SelectExitEventArgs args)
        {
            isSelected = false;
            selectingInteractor = null;

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            if (args.isCanceled)
                OnSelectCanceling(args.interactor);
            else
                OnSelectExiting(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is ending the selection.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectEntered(SelectEnterEventArgs)"/>
        protected internal virtual void OnSelectExited(SelectExitEventArgs args)
        {
            m_SelectExited?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            if (args.isCanceled)
                OnSelectCanceled(args.interactor);
            else
                OnSelectExited(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the <see cref="XRBaseControllerInteractor"/>
        /// when the Interactor begins an activation event on this selected Interactable.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is sending the activate event.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnDeactivated"/>
        protected internal virtual void OnActivated(ActivateEventArgs args)
        {
            m_Activated?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnActivate(args.interactor);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the <see cref="XRBaseControllerInteractor"/>
        /// when the Interactor ends an activation event on this selected Interactable.
        /// </summary>
        /// <param name="args">Event data containing the Interactor that is sending the deactivate event.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnActivated"/>
        protected internal virtual void OnDeactivated(DeactivateEventArgs args)
        {
            m_Deactivated?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnDeactivate(args.interactor);
#pragma warning restore 618
        }
    }
}
