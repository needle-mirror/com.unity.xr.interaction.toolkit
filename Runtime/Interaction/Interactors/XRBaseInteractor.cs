using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit.Utilities;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Abstract base class from which all interactor behaviours derive.
    /// This class hooks into the interaction system (via <see cref="XRInteractionManager"/>) and provides base virtual methods for handling
    /// hover and selection.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_Interactors)]
    public abstract partial class XRBaseInteractor : MonoBehaviour
    {
        /// <summary>
        /// Calls the methods in its invocation list when this Interactor is registered with an Interaction Manager.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.interactorRegistered"/>
        public event Action<InteractorRegisteredEventArgs> registered;

        /// <summary>
        /// Calls the methods in its invocation list when this Interactor is unregistered from an Interaction Manager.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractorUnregisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.interactorUnregistered"/>
        public event Action<InteractorUnregisteredEventArgs> unregistered;
        [SerializeField]
        XRInteractionManager m_InteractionManager;

        /// <summary>
        /// The <see cref="XRInteractionManager"/> that this Interactor will communicate with (will find one if <see langword="null"/>).
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
        LayerMask m_InteractionLayerMask = -1;

        /// <summary>
        /// Allows interaction with Interactables whose Interaction Layer Mask overlaps with any layer in this Interaction Layer Mask.
        /// </summary>
        /// <seealso cref="XRBaseInteractable.interactionLayerMask"/>
        /// <seealso cref="CanHover"/>
        /// <seealso cref="CanSelect"/>
        public LayerMask interactionLayerMask
        {
            get => m_InteractionLayerMask;
            set => m_InteractionLayerMask = value;
        }

        [SerializeField]
        Transform m_AttachTransform;

        /// <summary>
        /// The <see cref="Transform"/> that is used as the attach point for Interactables.
        /// </summary>
        /// <remarks>
        /// Automatically instantiated and set in <see cref="Awake"/> if <see langword="null"/>.
        /// Setting this will not automatically destroy the previous object.
        /// </remarks>
        public Transform attachTransform
        {
            get => m_AttachTransform;
            set => m_AttachTransform = value;
        }

        [SerializeField]
        XRBaseInteractable m_StartingSelectedInteractable;

        /// <summary>
        /// The Interactable that this Interactor will automatically select at startup (optional, may be <see langword="null"/>).
        /// </summary>
        public XRBaseInteractable startingSelectedInteractable
        {
            get => m_StartingSelectedInteractable;
            set => m_StartingSelectedInteractable = value;
        }

        [SerializeField]
        HoverEnterEvent m_HoverEntered = new HoverEnterEvent();

        /// <summary>
        /// Gets or sets the event that is called when this Interactor begins hovering over an Interactable.
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
        /// Gets or sets the event that is called when this Interactor ends hovering over an Interactable.
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
        /// Gets or sets the event that is called when this Interactor begins selecting an Interactable.
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
        /// Gets or sets the event that is called when this Interactor ends selecting an Interactable.
        /// </summary>
        /// <remarks>
        /// The <see cref="SelectEnterEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="selectEntered"/>
        public SelectExitEvent selectExited
        {
            get => m_SelectExited;
            set => m_SelectExited = value;
        }

        bool m_AllowHover = true;

        /// <summary>
        /// Defines whether this interactor allows hover events.
        /// </summary>
        public bool allowHover
        {
            get => m_AllowHover;
            set => m_AllowHover = value;
        }

        bool m_AllowSelect = true;

        /// <summary>
        /// Defines whether this interactor allows select events.
        /// </summary>
        public bool allowSelect
        {
            get => m_AllowSelect;
            set => m_AllowSelect = value;
        }

        bool m_IsPerformingManualInteraction;

        /// <summary>
        /// Defines whether this interactor is performing a manual interaction or not.
        /// </summary>
        /// <seealso cref="StartManualInteraction"/>
        /// <seealso cref="EndManualInteraction"/>
        public bool isPerformingManualInteraction => m_IsPerformingManualInteraction;

        /// <summary>
        /// Selected interactable for this interactor (may be <see langword="null"/>).
        /// </summary>
        /// <seealso cref="XRBaseInteractable.selectingInteractor"/>
        public XRBaseInteractable selectTarget { get; protected set; }

        /// <summary>
        /// Target interactables that are currently being hovered over (may by empty).
        /// </summary>
        /// <seealso cref="XRBaseInteractable.hoveringInteractors"/>
        protected List<XRBaseInteractable> hoverTargets { get; } = new List<XRBaseInteractable>();

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
            // Create empty attach transform if none specified
            if (m_AttachTransform == null)
            {
                var attachGO = new GameObject($"[{gameObject.name}] Attach");
                m_AttachTransform = attachGO.transform;
                m_AttachTransform.SetParent(transform);
                m_AttachTransform.localPosition = Vector3.zero;
                m_AttachTransform.localRotation = Quaternion.identity;
            }

            // Setup Interaction Manager
            FindCreateInteractionManager();

            // Warn about use of deprecated events
            if (m_OnHoverEntered.GetPersistentEventCount() > 0 ||
                m_OnHoverExited.GetPersistentEventCount() > 0 ||
                m_OnSelectEntered.GetPersistentEventCount() > 0 ||
                m_OnSelectExited.GetPersistentEventCount() > 0)
            {
                Debug.LogWarning("Some deprecated Interactor Events are being used. These deprecated events will be removed in a future version." +
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
        protected virtual void Start()
        {
            if (m_InteractionManager != null && m_StartingSelectedInteractable != null)
                m_InteractionManager.ForceSelect(this, m_StartingSelectedInteractable);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            // Don't need to do anything; method kept for backwards compatibility.
        }

        /// <summary>
        /// Retrieves a copy of the list of target interactables that are currently being hovered over.
        /// </summary>
        /// <param name="targets">The results list to store hover targets into.</param>
        /// <remarks>
        /// Clears <paramref name="targets"/> before adding to it.
        /// </remarks>
        public void GetHoverTargets(List<XRBaseInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(hoverTargets);
        }

        /// <summary>
        /// Retrieve the list of Interactables that this Interactor could possibly interact with this frame.
        /// This list is sorted by priority (with highest priority first).
        /// </summary>
        /// <param name="targets">The results list to populate with Interactables that are valid for selection or hover.</param>
        public abstract void GetValidTargets(List<XRBaseInteractable> targets);

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
                m_InteractionManager.RegisterInteractor(this);
                m_RegisteredInteractionManager = m_InteractionManager;
            }
        }

        void UnregisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager == null)
                return;

            m_RegisteredInteractionManager.UnregisterInteractor(this);
            m_RegisteredInteractionManager = null;
        }

        bool IsOnValidLayerMask(XRBaseInteractable interactable)
        {
            return (m_InteractionLayerMask & interactable.interactionLayerMask) != 0;
        }

        /// <summary>
        /// (Read Only) Indicates whether this interactor is in a state where it could hover.
        /// </summary>
        public virtual bool isHoverActive => m_AllowHover;

        /// <summary>
        /// (Read Only) Indicates whether this interactor is in a state where it could select.
        /// </summary>
        public virtual bool isSelectActive => m_AllowSelect;

        /// <summary>
        /// Determines if the interactable is valid for hover this frame.
        /// </summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns>Returns <see langword="true"/> if the interactable can be hovered over this frame.</returns>
        /// <seealso cref="XRBaseInteractable.IsHoverableBy"/>
        public virtual bool CanHover(XRBaseInteractable interactable) => m_AllowHover && IsOnValidLayerMask(interactable);

        /// <summary>
        /// Determines if the interactable is valid for selection this frame.
        /// </summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns>Returns <see langword="true"/> if the interactable can be selected this frame.</returns>
        /// <seealso cref="XRBaseInteractable.IsSelectableBy"/>
        public virtual bool CanSelect(XRBaseInteractable interactable) => m_AllowSelect && IsOnValidLayerMask(interactable);

        /// <summary>
        /// (Read Only) Indicates whether this Interactor requires exclusive selection of an Interactable to select it.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/>, this Interactor will only select an Interactable when that Interactable is not currently selected.
        /// When <see langword="false"/>, a selected Interactable will first be deselected before being selected by this Interactor.
        /// </remarks>
        public virtual bool requireSelectExclusive => false;

        /// <summary>
        /// (Read Only) Overriding movement type of the selected interactable's movement.
        /// </summary>
        public virtual XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride => null;

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when this Interactor is registered with it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that registered this Interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.RegisterInteractor"/>
        protected internal virtual void OnRegistered(InteractorRegisteredEventArgs args)
        {
            if (args.manager != m_InteractionManager)
                Debug.LogWarning($"An Interactor was registered with an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_InteractionManager}\" but was registered with \"{args.manager}\".", this);

            registered?.Invoke(args);
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when this Interactor is unregistered from it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that unregistered this Interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.UnregisterInteractor"/>
        protected internal virtual void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            if (args.manager != m_RegisteredInteractionManager)
                Debug.LogWarning($"An Interactor was unregistered from an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_RegisteredInteractionManager}\" but was unregistered from \"{args.manager}\".", this);

            unregistered?.Invoke(args);
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is being hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverEntered(HoverEnterEventArgs)"/>
        protected internal virtual void OnHoverEntering(HoverEnterEventArgs args)
        {
            hoverTargets.Add(args.interactable);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverEntering(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is being hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverExited(HoverExitEventArgs)"/>
        protected internal virtual void OnHoverEntered(HoverEnterEventArgs args)
        {
            m_HoverEntered?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverEntered(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is no longer hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverExited(HoverExitEventArgs)"/>
        protected internal virtual void OnHoverExiting(HoverExitEventArgs args)
        {
            Debug.Assert(hoverTargets.Contains(args.interactable), this);
            hoverTargets.Remove(args.interactable);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverExiting(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is no longer hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnHoverEntered(HoverEnterEventArgs)"/>
        protected internal virtual void OnHoverExited(HoverExitEventArgs args)
        {
            m_HoverExited?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnHoverExited(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is being selected.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectEntered(SelectEnterEventArgs)"/>
        protected internal virtual void OnSelectEntering(SelectEnterEventArgs args)
        {
            selectTarget = args.interactable;

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectEntering(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is being selected.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectExited(SelectExitEventArgs)"/>
        protected internal virtual void OnSelectEntered(SelectEnterEventArgs args)
        {
            m_SelectEntered?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectEntered(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is no longer selected.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectExited(SelectExitEventArgs)"/>
        protected internal virtual void OnSelectExiting(SelectExitEventArgs args)
        {
            Debug.Assert(selectTarget == args.interactable, this);
            if (selectTarget == args.interactable)
                selectTarget = null;

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectExiting(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="args">Event data containing the Interactable that is no longer selected.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnSelectEntered(SelectEnterEventArgs)"/>
        protected internal virtual void OnSelectExited(SelectExitEventArgs args)
        {
            m_SelectExited?.Invoke(args);

#pragma warning disable 618 // Calling deprecated method to help with backwards compatibility with existing user code.
            OnSelectExited(args.interactable);
#pragma warning restore 618
        }

        /// <summary>
        /// This method is called by the Interaction Manager to update the Interactor.
        /// Please see the Interaction Manager documentation for more details on update order.
        /// </summary>
        /// <param name="updatePhase">The update phase this is called during.</param>
        public virtual void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }

        /// <summary>
        /// Manually initiate selection of an Interactable.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="EndManualInteraction"/>
        public virtual void StartManualInteraction(XRBaseInteractable interactable)
        {
            if (interactionManager == null)
            {
                Debug.LogWarning("Cannot start manual interaction without an Interaction Manager set.", this);
                return;
            }

            interactionManager.SelectEnter(this, interactable);
            m_IsPerformingManualInteraction = true;
        }

        /// <summary>
        /// Ends the manually initiated selection of an Interactable.
        /// </summary>
        /// <seealso cref="StartManualInteraction"/>
        public virtual void EndManualInteraction()
        {
            if (interactionManager == null)
            {
                Debug.LogWarning("Cannot end manual interaction without an Interaction Manager set.", this);
                return;
            }

            if (!m_IsPerformingManualInteraction)
            {
                Debug.LogWarning("Tried to end manual interaction but was not performing manual interaction. Ignoring request.", this);
                return;
            }

            interactionManager.SelectExit(this, selectTarget);
            m_IsPerformingManualInteraction = false;
        }
    }
}
