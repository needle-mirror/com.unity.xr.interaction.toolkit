using System;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public abstract partial class XRBaseInteractable
    {
#pragma warning disable 618
        [SerializeField, FormerlySerializedAs("m_OnFirstHoverEnter")]
        XRInteractableEvent m_OnFirstHoverEntered = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called only when the first Interactor begins hovering
        /// over this Interactable as the sole hovering Interactor. Subsequent Interactors that
        /// begin hovering over this Interactable will not cause this event to be invoked as
        /// long as any others are still hovering.
        /// </summary>
        [Obsolete("onFirstHoverEntered has been deprecated. Use firstHoverEntered with updated signature instead.")]
        public XRInteractableEvent onFirstHoverEntered
        {
            get => m_OnFirstHoverEntered;
            set => m_OnFirstHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnLastHoverExit")]
        XRInteractableEvent m_OnLastHoverExited = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called only when the last remaining hovering Interactor
        /// ends hovering over this Interactable.
        /// </summary>
        [Obsolete("onLastHoverExited has been deprecated. Use lastHoverExited with updated signature instead.")]
        public XRInteractableEvent onLastHoverExited
        {
            get => m_OnLastHoverExited;
            set => m_OnLastHoverExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnHoverEnter")]
        XRInteractableEvent m_OnHoverEntered = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor begins hovering over this Interactable.
        /// </summary>
        [Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.")]
        public XRInteractableEvent onHoverEntered
        {
            get => m_OnHoverEntered;
            set => m_OnHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnHoverExit")]
        XRInteractableEvent m_OnHoverExited = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor ends hovering over this Interactable.
        /// </summary>
        [Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.")]
        public XRInteractableEvent onHoverExited
        {
            get => m_OnHoverExited;
            set => m_OnHoverExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectEnter")]
        XRInteractableEvent m_OnSelectEntered = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor begins selecting this Interactable.
        /// </summary>
        [Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.")]
        public XRInteractableEvent onSelectEntered
        {
            get => m_OnSelectEntered;
            set => m_OnSelectEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectExit")]
        XRInteractableEvent m_OnSelectExited = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor ends selecting this Interactable.
        /// </summary>
        [Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature and check for !args.isCanceled instead.")]
        public XRInteractableEvent onSelectExited
        {
            get => m_OnSelectExited;
            set => m_OnSelectExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectCancel")]
        XRInteractableEvent m_OnSelectCanceled = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactable is selected by an Interactor and either is unregistered
        /// (such as from being disabled or destroyed).
        /// </summary>
        [Obsolete("onSelectCanceled has been deprecated. Use selectExited with updated signature and check for args.isCanceled instead.")]
        public XRInteractableEvent onSelectCanceled
        {
            get => m_OnSelectCanceled;
            set => m_OnSelectCanceled = value;
        }

        [SerializeField]
        XRInteractableEvent m_OnActivate = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor activates this selected Interactable.
        /// </summary>
        [Obsolete("onActivate has been deprecated. Use activated with updated signature instead.")]
        public XRInteractableEvent onActivate
        {
            get => m_OnActivate;
            set => m_OnActivate = value;
        }

        [SerializeField]
        XRInteractableEvent m_OnDeactivate = new XRInteractableEvent();

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when an Interactor deactivates this selected Interactable.
        /// </summary>
        [Obsolete("onDeactivate has been deprecated. Use deactivated with updated signature instead.")]
        public XRInteractableEvent onDeactivate
        {
            get => m_OnDeactivate;
            set => m_OnDeactivate = value;
        }

        /// <summary>
        /// (Deprecated) Called only when the first Interactor begins hovering over this Interactable.
        /// </summary>
        [Obsolete("onFirstHoverEnter has been deprecated. Use onFirstHoverEntered instead. (UnityUpgradable) -> onFirstHoverEntered")]
        public XRInteractableEvent onFirstHoverEnter => onFirstHoverEntered;

        /// <summary>
        /// (Deprecated) Called every time when an Interactor begins hovering over this Interactable.
        /// </summary>
        [Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered")]
        public XRInteractableEvent onHoverEnter => onHoverEntered;

        /// <summary>
        /// (Deprecated) Called every time when an Interactor ends hovering over this Interactable.
        /// </summary>
        [Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited")]
        public XRInteractableEvent onHoverExit => onHoverExited;

        /// <summary>
        /// (Deprecated) Called only when the last Interactor ends hovering over this Interactable.
        /// </summary>
        [Obsolete("onLastHoverExit has been deprecated. Use onLastHoverExited instead. (UnityUpgradable) -> onLastHoverExited")]
        public XRInteractableEvent onLastHoverExit => onLastHoverExited;

        /// <summary>
        /// (Deprecated) Called when an Interactor begins selecting this Interactable.
        /// </summary>
        [Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered")]
        public XRInteractableEvent onSelectEnter => onSelectEntered;

        /// <summary>
        /// (Deprecated) Called when an Interactor ends selecting this Interactable.
        /// </summary>
        [Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited")]
        public XRInteractableEvent onSelectExit => onSelectExited;

        /// <summary>
        /// (Deprecated) Called when the Interactor selecting this Interactable is disabled or destroyed.
        /// </summary>
        [Obsolete("onSelectCancel has been deprecated. Use onSelectCanceled instead. (UnityUpgradable) -> onSelectCanceled")]
        public XRInteractableEvent onSelectCancel => onSelectCanceled;

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the hover.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractor)"/>
        [Obsolete("OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.")]
        protected internal virtual void OnHoverEntering(XRBaseInteractor interactor)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the hover.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractor)"/>
        [Obsolete("OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.")]
        protected internal virtual void OnHoverEntered(XRBaseInteractor interactor)
        {
            if (m_HoveringInteractors.Count == 1)
                m_OnFirstHoverEntered?.Invoke(interactor);

            m_OnHoverEntered?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the hover.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractor)"/>
        [Obsolete("OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.")]
        protected internal virtual void OnHoverExiting(XRBaseInteractor interactor)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the hover.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractor)"/>
        [Obsolete("OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.")]
        protected internal virtual void OnHoverExited(XRBaseInteractor interactor)
        {
            if (m_HoveringInteractors.Count == 0)
                m_OnLastHoverExited?.Invoke(interactor);

            m_OnHoverExited?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        [Obsolete("OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.")]
        protected internal virtual void OnSelectEntering(XRBaseInteractor interactor)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        [Obsolete("OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.")]
        protected internal virtual void OnSelectEntered(XRBaseInteractor interactor)
        {
            m_OnSelectEntered?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        [Obsolete("OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.")]
        protected internal virtual void OnSelectExiting(XRBaseInteractor interactor)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        [Obsolete("OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.")]
        protected internal virtual void OnSelectExited(XRBaseInteractor interactor)
        {
            m_OnSelectExited?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// while this Interactable is selected by an Interactor
        /// right before either is unregistered (such as from being disabled or destroyed)
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        [Obsolete("OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.")]
        protected internal virtual void OnSelectCanceling(XRBaseInteractor interactor)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// while this Interactable is selected by an Interactor
        /// when either is unregistered (such as from being disabled or destroyed)
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceling"/>
        [Obsolete("OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.")]
        protected internal virtual void OnSelectCanceled(XRBaseInteractor interactor)
        {
            m_OnSelectCanceled?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the <see cref="XRBaseControllerInteractor"/>
        /// when the Interactor begins an activation event on this selected Interactable.
        /// </summary>
        /// <param name="interactor">Interactor that is sending the activate event.</param>
        /// <seealso cref="OnDeactivate"/>
        [Obsolete("OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.")]
        protected internal virtual void OnActivate(XRBaseInteractor interactor)
        {
            m_OnActivate?.Invoke(interactor);
        }

        /// <summary>
        /// (Deprecated) This method is called by the <see cref="XRBaseControllerInteractor"/>
        /// when the Interactor ends an activation event on this selected Interactable.
        /// </summary>
        /// <param name="interactor">Interactor that is sending the deactivate event.</param>
        /// <seealso cref="OnActivate"/>
        [Obsolete("OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.")]
        protected internal virtual void OnDeactivate(XRBaseInteractor interactor)
        {
            m_OnDeactivate?.Invoke(interactor);
        }
#pragma warning restore 618
    }
}
