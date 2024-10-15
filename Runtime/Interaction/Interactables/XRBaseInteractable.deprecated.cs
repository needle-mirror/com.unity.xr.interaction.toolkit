using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    public abstract partial class XRBaseInteractable
    {
        const string k_InteractionLayerMaskDeprecated = "interactionLayerMask has been deprecated. Use interactionLayers instead.";
        const string k_OnHoverEnteringDeprecated = "OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.";
        const string k_OnHoverEnteredDeprecated = "OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.";
        const string k_OnHoverExitingDeprecated = "OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.";
        const string k_OnHoverExitedDeprecated = "OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.";
        const string k_OnSelectEnteringDeprecated = "OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.";
        const string k_OnSelectEnteredDeprecated = "OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.";
        const string k_OnSelectExitingDeprecated = "OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.";
        const string k_OnSelectExitedDeprecated = "OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.";
        const string k_OnSelectCancelingDeprecated = "OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.";
        const string k_OnSelectCanceledDeprecated = "OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.";
        const string k_OnActivateDeprecated = "OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.";
        const string k_OnDeactivateDeprecated = "OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.";
        const string k_GetDistanceSqrToInteractorDeprecated = "GetDistanceSqrToInteractor(XRBaseInteractor) has been deprecated. Use GetDistanceSqrToInteractor(IXRInteractor) instead.";
        const string k_AttachCustomReticleDeprecated = "AttachCustomReticle(XRBaseInteractor) has been deprecated. Use AttachCustomReticle(IXRInteractor) instead.";
        const string k_RemoveCustomReticleDeprecated = "RemoveCustomReticle(XRBaseInteractor) has been deprecated. Use RemoveCustomReticle(IXRInteractor) instead.";
        const string k_HoveringInteractorsDeprecated = "hoveringInteractors has been deprecated. Use interactorsHovering instead.";
        const string k_SelectingInteractorDeprecated = "selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.";
        const string k_IsHoverableByDeprecated = "IsHoverableBy(XRBaseInteractor) has been deprecated. Use IsHoverableBy(IXRHoverInteractor) instead.";
        const string k_IsSelectableByDeprecated = "IsSelectableBy(XRBaseInteractor) has been deprecated. Use IsSelectableBy(IXRSelectInteractor) instead.";

        /// <summary>
        /// (Deprecated) Allows interaction with Interactors whose Interaction Layer Mask overlaps with any Layer in this Interaction Layer Mask.
        /// </summary>
        /// <seealso cref="XRBaseInteractor.interactionLayerMask"/>
        /// <remarks><c>interactionLayerMask</c> has been deprecated. Use <see cref="interactionLayers"/> instead.</remarks>
        [Obsolete(k_InteractionLayerMaskDeprecated, true)]
        public LayerMask interactionLayerMask
        {
            get
            {
                Debug.LogError(k_InteractionLayerMaskDeprecated, this);
                throw new NotSupportedException(k_InteractionLayerMaskDeprecated);
            }
            set
            {
                _ = value;
                Debug.LogError(k_InteractionLayerMaskDeprecated, this);
                throw new NotSupportedException(k_InteractionLayerMaskDeprecated);
            }
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls only when the first Interactor begins hovering
        /// over this Interactable as the sole hovering Interactor. Subsequent Interactors that
        /// begin hovering over this Interactable will not cause this event to be invoked as
        /// long as any others are still hovering.
        /// </summary>
        /// <remarks><c>onFirstHoverEntered</c> has been deprecated. Use <see cref="firstHoverEntered"/> with updated signature instead.</remarks>
        [Obsolete("onFirstHoverEntered has been deprecated. Use firstHoverEntered with updated signature instead.", true)]
        public XRInteractableEvent onFirstHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls only when the last remaining hovering Interactor
        /// ends hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onLastHoverExited</c> has been deprecated. Use <see cref="lastHoverExited"/> with updated signature instead.</remarks>
        [Obsolete("onLastHoverExited has been deprecated. Use lastHoverExited with updated signature instead.", true)]
        public XRInteractableEvent onLastHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor begins hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onHoverEntered</c> has been deprecated. Use <see cref="hoverEntered"/> instead.</remarks>
        [Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.", true)]
        public XRInteractableEvent onHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor ends hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onHoverExited</c> has been deprecated. Use <see cref="hoverExited"/> hoverExited with updated signature instead.</remarks>
        [Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.", true)]
        public XRInteractableEvent onHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor begins selecting this Interactable.
        /// </summary>
        /// <remarks><c>onSelectEntered</c> has been deprecated. Use <see cref="selectEntered"/> with updated signature instead.</remarks>
        [Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.", true)]
        public XRInteractableEvent onSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor ends selecting this Interactable.
        /// </summary>
        /// <remarks><c>onSelectExited</c> has been deprecated. Use <see cref="selectExited"/> with updated signature and check for <c>!</c><see cref="SelectExitEventArgs.isCanceled"/> instead.</remarks>
        [Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature and check for !args.isCanceled instead.", true)]
        public XRInteractableEvent onSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactable is selected by an Interactor and either is unregistered
        /// (such as from being disabled or destroyed).
        /// </summary>
        /// <remarks><c>onSelectCanceled</c> has been deprecated. Use <see cref="selectExited"/> with updated signature and check for <see cref="SelectExitEventArgs.isCanceled"/> instead.</remarks>
        [Obsolete("onSelectCanceled has been deprecated. Use selectExited with updated signature and check for args.isCanceled instead.", true)]
        public XRInteractableEvent onSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor activates this selected Interactable.
        /// </summary>
        /// <remarks><c>onActivate</c> has been deprecated. Use <see cref="activated"/> instead.</remarks>
        [Obsolete("onActivate has been deprecated. Use activated with updated signature instead.", true)]
        public XRInteractableEvent onActivate
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when an Interactor deactivates this selected Interactable.
        /// </summary>
        /// <remarks><c>onDeactivate</c> has been deprecated. Use <see cref="deactivated"/> instead.</remarks>
        [Obsolete("onDeactivate has been deprecated. Use deactivated with updated signature instead.", true)]
        public XRInteractableEvent onDeactivate
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Unity calls this only when the first Interactor begins hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onFirstHoverEnter</c> has been deprecated. Use <see cref="onFirstHoverEntered"/> instead.</remarks>
        [Obsolete("onFirstHoverEnter has been deprecated. Use onFirstHoverEntered instead. (UnityUpgradable) -> onFirstHoverEntered", true)]
        public XRInteractableEvent onFirstHoverEnter => default;

        /// <summary>
        /// (Deprecated) Unity calls this every time an Interactor begins hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onHoverEnter</c> has been deprecated. Use <see cref="onHoverEntered"/> instead.</remarks>
        [Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered", true)]
        public XRInteractableEvent onHoverEnter => default;

        /// <summary>
        /// (Deprecated) Unity calls this every time an Interactor ends hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onHoverExit</c> has been deprecated. Use <see cref="onHoverExited"/> instead.</remarks>
        [Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited", true)]
        public XRInteractableEvent onHoverExit => default;

        /// <summary>
        /// (Deprecated) Unity calls this only when the last Interactor ends hovering over this Interactable.
        /// </summary>
        /// <remarks><c>onLastHoverExit</c> has been deprecated. Use <see cref="onLastHoverExited"/> instead.</remarks>
        [Obsolete("onLastHoverExit has been deprecated. Use onLastHoverExited instead. (UnityUpgradable) -> onLastHoverExited", true)]
        public XRInteractableEvent onLastHoverExit => default;

        /// <summary>
        /// (Deprecated) Unity calls this when an Interactor begins selecting this Interactable.
        /// </summary>
        /// <remarks><c>onSelectEnter</c> has been deprecated. Use <see cref="onSelectEntered"/> instead.</remarks>
        [Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered", true)]
        public XRInteractableEvent onSelectEnter => default;

        /// <summary>
        /// (Deprecated) Unity calls this when an Interactor ends selecting this Interactable.
        /// </summary>
        /// <remarks><c>onSelectExit</c> has been deprecated. Use <see cref="onSelectExited"/> instead.</remarks>
        [Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited", true)]
        public XRInteractableEvent onSelectExit => default;

        /// <summary>
        /// (Deprecated) Unity calls this when the Interactor selecting this Interactable is disabled or destroyed.
        /// </summary>
        /// <remarks><c>onSelectCancel</c> has been deprecated. Use <see cref="onSelectCanceled"/> instead.</remarks>
        [Obsolete("onSelectCancel has been deprecated. Use onSelectCanceled instead. (UnityUpgradable) -> onSelectCanceled", true)]
        public XRInteractableEvent onSelectCancel => default;

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the hover.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractor)"/>
        /// <remarks><c>OnHoverEntering(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnHoverEntering(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverEnteringDeprecated, true)]
        protected virtual void OnHoverEntering(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnHoverEnteringDeprecated, this);
            throw new NotSupportedException(k_OnHoverEnteringDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the hover.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractor)"/>
        /// <remarks><c>OnHoverEntered(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnHoverEntered(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverEnteredDeprecated, true)]
        protected virtual void OnHoverEntered(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnHoverEnteredDeprecated, this);
            throw new NotSupportedException(k_OnHoverEnteredDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the hover.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractor)"/>
        /// <remarks><c>OnHoverExiting(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnHoverExiting(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverExitingDeprecated, true)]
        protected virtual void OnHoverExiting(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnHoverExitingDeprecated, this);
            throw new NotSupportedException(k_OnHoverExitingDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the hover.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractor)"/>
        /// <remarks><c>OnHoverExited(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnHoverExited(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverExitedDeprecated, true)]
        protected virtual void OnHoverExited(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnHoverExitedDeprecated, this);
            throw new NotSupportedException(k_OnHoverExitedDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <remarks><c>OnSelectEntering(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectEntering(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectEnteringDeprecated, true)]
        protected virtual void OnSelectEntering(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectEnteringDeprecated, this);
            throw new NotSupportedException(k_OnSelectEnteringDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is initiating the selection.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        /// <remarks><c>OnSelectEntered(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectEntered(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectEnteredDeprecated, true)]
        protected virtual void OnSelectEntered(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectEnteredDeprecated, this);
            throw new NotSupportedException(k_OnSelectEnteredDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <remarks><c>OnSelectExiting(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectExiting(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectExitingDeprecated, true)]
        protected virtual void OnSelectExiting(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectExitingDeprecated, this);
            throw new NotSupportedException(k_OnSelectExitingDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        /// <remarks><c>OnSelectExited(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectExited(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectExitedDeprecated, true)]
        protected virtual void OnSelectExited(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectExitedDeprecated, this);
            throw new NotSupportedException(k_OnSelectExitedDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// while this Interactable is selected by an Interactor
        /// right before either is unregistered (such as from being disabled or destroyed)
        /// in a first pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceled"/>
        /// <remarks><c>OnSelectCanceling(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectExiting(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectCancelingDeprecated, true)]
        protected virtual void OnSelectCanceling(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectCancelingDeprecated, this);
            throw new NotSupportedException(k_OnSelectCancelingDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// while an Interactor selects this Interactable when either
        /// is unregistered (such as from being disabled or destroyed)
        /// in a second pass.
        /// </summary>
        /// <param name="interactor">Interactor that is ending the selection.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectExited(XRBaseInteractor)"/>
        /// <seealso cref="OnSelectCanceling"/>
        /// <remarks><c>OnSelectCanceled(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnSelectExited(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectCanceledDeprecated, true)]
        protected virtual void OnSelectCanceled(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnSelectCanceledDeprecated, this);
            throw new NotSupportedException(k_OnSelectCanceledDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRBaseControllerInteractor"/> calls this method
        /// when the Interactor begins an activation event on this Interactable.
        /// </summary>
        /// <param name="interactor">Interactor that is sending the activate event.</param>
        /// <seealso cref="OnDeactivate"/>
        /// <remarks><c>OnActivate(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnActivated(ActivateEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnActivateDeprecated, true)]
        protected virtual void OnActivate(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnActivateDeprecated, this);
            throw new NotSupportedException(k_OnActivateDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRBaseControllerInteractor"/> calls this method
        /// when the Interactor ends an activation event on this Interactable.
        /// </summary>
        /// <param name="interactor">Interactor that is sending the deactivate event.</param>
        /// <seealso cref="OnActivate"/>
        /// <remarks><c>OnDeactivate(XRBaseInteractor)</c> has been deprecated. Use <see cref="OnDeactivated(DeactivateEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnDeactivateDeprecated, true)]
        protected virtual void OnDeactivate(XRBaseInteractor interactor)
        {
            Debug.LogError(k_OnDeactivateDeprecated, this);
            throw new NotSupportedException(k_OnDeactivateDeprecated);
        }

        /// <summary>
        /// (Deprecated) Calculates distance squared to interactor (based on colliders).
        /// </summary>
        /// <param name="interactor">Interactor to calculate distance against.</param>
        /// <returns>Returns the minimum distance between the interactor and this interactable's colliders.</returns>
        /// <remarks><c>GetDistanceSqrToInteractor(XRBaseInteractor)</c> has been deprecated. Use <see cref="GetDistanceSqrToInteractor(IXRInteractor)"/> instead.</remarks>
        [Obsolete(k_GetDistanceSqrToInteractorDeprecated, true)]
        public virtual float GetDistanceSqrToInteractor(XRBaseInteractor interactor)
        {
            Debug.LogError(k_GetDistanceSqrToInteractorDeprecated, this);
            throw new NotSupportedException(k_GetDistanceSqrToInteractorDeprecated);
        }

        /// <summary>
        /// (Deprecated) Attaches the custom reticle to the Interactor.
        /// </summary>
        /// <param name="interactor">Interactor that is interacting with this Interactable.</param>
        /// <remarks><c>AttachCustomReticle(XRBaseInteractor)</c> has been deprecated. Use <see cref="AttachCustomReticle(IXRInteractor)"/> instead.</remarks>
        [Obsolete(k_AttachCustomReticleDeprecated, true)]
        public virtual void AttachCustomReticle(XRBaseInteractor interactor)
        {
            Debug.LogError(k_AttachCustomReticleDeprecated, this);
            throw new NotSupportedException(k_AttachCustomReticleDeprecated);
        }

        /// <summary>
        /// (Deprecated) Removes the custom reticle from the Interactor.
        /// </summary>
        /// <param name="interactor">Interactor that is no longer interacting with this Interactable.</param>
        /// <remarks><c>RemoveCustomReticle(XRBaseInteractor)</c> has been deprecated. Use <see cref="RemoveCustomReticle(IXRInteractor)"/> instead.</remarks>
        [Obsolete(k_RemoveCustomReticleDeprecated, true)]
        public virtual void RemoveCustomReticle(XRBaseInteractor interactor)
        {
            Debug.LogError(k_RemoveCustomReticleDeprecated, this);
            throw new NotSupportedException(k_RemoveCustomReticleDeprecated);
        }

        /// <summary>
        /// (Deprecated) (Read Only) The list of interactors that are hovering on this interactable.
        /// </summary>
        /// <seealso cref="isHovered"/>
        /// <seealso cref="XRBaseInteractor.hoverTargets"/>
        [Obsolete(k_HoveringInteractorsDeprecated, true)]
        public List<XRBaseInteractor> hoveringInteractors
        {
            get
            {
                Debug.LogError(k_HoveringInteractorsDeprecated, this);
                throw new NotSupportedException(k_HoveringInteractorsDeprecated);
            }
        }

        /// <summary>
        /// (Deprecated) The Interactor selecting this Interactable (may be <see langword="null"/>).
        /// </summary>
        /// <remarks>
        /// Unity automatically sets this value during <see cref="OnSelectEntering(SelectEnterEventArgs)"/>
        /// and <see cref="OnSelectExiting(SelectExitEventArgs)"/> and should not typically need to be set
        /// by a user. The setter is <see langword="protected"/> to allow for rare scenarios where a derived
        /// class needs to control this value. Changing this value does not invoke select events.
        /// <br />
        /// <c>selectingInteractor</c> has been deprecated. Use <see cref="interactorsSelecting"/> or <see cref="isSelected"/> instead.
        /// </remarks>
        /// <seealso cref="isSelected"/>
        /// <seealso cref="XRBaseInteractor.selectTarget"/>
        [Obsolete(k_SelectingInteractorDeprecated, true)]
        public XRBaseInteractor selectingInteractor
        {
            get
            {
                Debug.LogError(k_SelectingInteractorDeprecated, this);
                throw new NotSupportedException(k_SelectingInteractorDeprecated);
            }
            protected set
            {
                _ = value;
                Debug.LogError(k_SelectingInteractorDeprecated, this);
                throw new NotSupportedException(k_SelectingInteractorDeprecated);
            }
        }

        /// <summary>
        /// (Deprecated) Determines if this interactable can be hovered by a given interactor.
        /// </summary>
        /// <param name="interactor">Interactor to check for a valid hover state with.</param>
        /// <returns>Returns <see langword="true"/> if hovering is valid this frame. Returns <see langword="false"/> if not.</returns>
        /// <seealso cref="XRBaseInteractor.CanHover(XRBaseInteractable)"/>
        /// <remarks>
        /// <c>IsHoverableBy(XRBaseInteractor)</c> has been deprecated. Use <see cref="IsHoverableBy(IXRHoverInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_IsHoverableByDeprecated, true)]
        public virtual bool IsHoverableBy(XRBaseInteractor interactor)
        {
            Debug.LogError(k_IsHoverableByDeprecated, this);
            throw new NotSupportedException(k_IsHoverableByDeprecated);
        }

        /// <summary>
        /// (Deprecated) Determines if a given Interactor can select this Interactable.
        /// </summary>
        /// <param name="interactor">Interactor to check for a valid selection with.</param>
        /// <returns>Returns <see langword="true"/> if selection is valid this frame. Returns <see langword="false"/> if not.</returns>
        /// <seealso cref="XRBaseInteractor.CanSelect(XRBaseInteractable)"/>
        /// <remarks>
        /// <c>IsSelectableBy(XRBaseInteractor)</c> has been deprecated. Use <see cref="IsSelectableBy(IXRSelectInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_IsSelectableByDeprecated, true)]
        public virtual bool IsSelectableBy(XRBaseInteractor interactor)
        {
            Debug.LogError(k_IsSelectableByDeprecated, this);
            throw new NotSupportedException(k_IsSelectableByDeprecated);
        }
    }
}
