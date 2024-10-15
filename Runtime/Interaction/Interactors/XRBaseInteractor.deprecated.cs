using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    public abstract partial class XRBaseInteractor
    {
        const string k_InteractionLayerMaskDeprecated = "interactionLayerMask has been deprecated. Use interactionLayers instead.";
        const string k_EnableInteractionsDeprecated = "enableInteractions has been deprecated. Use allowHover and allowSelect instead.";
        const string k_OnHoverEnteringDeprecated = "OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.";
        const string k_OnHoverEnteredDeprecated = "OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.";
        const string k_OnHoverExitingDeprecated = "OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.";
        const string k_OnHoverExitedDeprecated = "OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.";
        const string k_OnSelectEnteringDeprecated = "OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.";
        const string k_OnSelectEnteredDeprecated = "OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.";
        const string k_OnSelectExitingDeprecated = "OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.";
        const string k_OnSelectExitedDeprecated = "OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.";
        const string k_SelectTargetDeprecated = "selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.";
        const string k_HoverTargetsDeprecated = "hoverTargets has been deprecated. Use interactablesHovered instead.";
        const string k_GetHoverTargetsDeprecated = "GetHoverTargets has been deprecated. Use interactablesHovered instead.";
        const string k_GetValidTargetsDeprecated = "GetValidTargets(List<XRBaseInteractable>) has been deprecated. Override GetValidTargets(List<IXRInteractable>) instead.";
        const string k_CanHoverDeprecated = "CanHover(XRBaseInteractable) has been deprecated. Use CanHover(IXRHoverInteractable) instead.";
        const string k_CanSelectDeprecated = "CanSelect(XRBaseInteractable) has been deprecated. Use CanSelect(IXRSelectInteractable) instead.";
        const string k_RequireSelectExclusiveDeprecated = "requireSelectExclusive has been deprecated. Put logic in CanSelect instead.";
        const string k_StartManualInteractionDeprecated = "StartManualInteraction(XRBaseInteractable) has been deprecated. Use StartManualInteraction(IXRSelectInteractable) instead.";

        /// <summary>
        /// (Deprecated) Allows interaction with Interactables whose Interaction Layer Mask overlaps with any Layer in this Interaction Layer Mask.
        /// </summary>
        /// <seealso cref="XRBaseInteractable.interactionLayerMask"/>
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
        /// (Deprecated) Defines whether interactions are enabled or not.
        /// </summary>
        /// <example>
        /// <c>enableInteractions = value;</c> is a convenience property for:
        /// <code>
        /// allowHover = value;
        /// allowSelect = value;
        /// </code>
        /// </example>
        /// <remarks>
        /// <c>enableInteractions</c> has been deprecated. Use <see cref="allowHover"/> and <see cref="allowSelect"/> instead.
        /// </remarks>
        [Obsolete(k_EnableInteractionsDeprecated, true)]
        public bool enableInteractions
        {
            get
            {
                Debug.LogError(k_EnableInteractionsDeprecated, this);
                throw new NotSupportedException(k_EnableInteractionsDeprecated);
            }
            set
            {
                _ = value;
                Debug.LogError(k_EnableInteractionsDeprecated, this);
                throw new NotSupportedException(k_EnableInteractionsDeprecated);
            }
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverEntered</c> has been deprecated. Use <see cref="hoverEntered"/> with updated signature instead.</remarks>
        [Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.", true)]
        public XRInteractorEvent onHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverExited</c> has been deprecated. Use <see cref="hoverExited"/> with updated signature instead.</remarks>
        [Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.", true)]
        public XRInteractorEvent onHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectEntered</c> has been deprecated. Use <see cref="selectEntered"/> with updated signature instead.</remarks>
        [Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.", true)]
        public XRInteractorEvent onSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectExited</c> has been deprecated. Use <see cref="selectExited"/> with updated signature instead.</remarks>
        [Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature instead.", true)]
        public XRInteractorEvent onSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverEnter</c> has been deprecated. Use <see cref="onHoverEntered"/> instead.</remarks>
        [Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered", true)]
        public XRInteractorEvent onHoverEnter => default;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverExit</c> has been deprecated. Use <see cref="onHoverExited"/> instead.</remarks>
        [Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited", true)]
        public XRInteractorEvent onHoverExit => default;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectEnter</c> has been deprecated. Use <see cref="onSelectEntered"/> instead.</remarks>
        [Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered", true)]
        public XRInteractorEvent onSelectEnter => default;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectExit></c> has been deprecated. Use <see cref="onSelectExited"/> instead.</remarks>
        [Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited", true)]
        public XRInteractorEvent onSelectExit => default;

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverEntering(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverEntering(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverEnteringDeprecated, true)]
        protected virtual void OnHoverEntering(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnHoverEnteringDeprecated, this);
            throw new NotSupportedException(k_OnHoverEnteringDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverEntered(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverEntered(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverEnteredDeprecated, true)]
        protected virtual void OnHoverEntered(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnHoverEnteredDeprecated, this);
            throw new NotSupportedException(k_OnHoverEnteredDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverExiting(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverExiting(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverExitingDeprecated, true)]
        protected virtual void OnHoverExiting(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnHoverExitingDeprecated, this);
            throw new NotSupportedException(k_OnHoverExitingDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverExited(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverExited(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnHoverExitedDeprecated, true)]
        protected virtual void OnHoverExited(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnHoverExitedDeprecated, this);
            throw new NotSupportedException(k_OnHoverExitedDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectEntering(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectEntering(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectEnteringDeprecated, true)]
        protected virtual void OnSelectEntering(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnSelectEnteringDeprecated, this);
            throw new NotSupportedException(k_OnSelectEnteringDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectEntered(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectEntered(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectEnteredDeprecated, true)]
        protected virtual void OnSelectEntered(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnSelectEnteredDeprecated, this);
            throw new NotSupportedException(k_OnSelectEnteredDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectExiting(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectExiting(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectExitingDeprecated, true)]
        protected virtual void OnSelectExiting(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnSelectExitingDeprecated, this);
            throw new NotSupportedException(k_OnSelectExitingDeprecated);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectExited(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectExited(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete(k_OnSelectExitedDeprecated, true)]
        protected virtual void OnSelectExited(XRBaseInteractable interactable)
        {
            Debug.LogError(k_OnSelectExitedDeprecated, this);
            throw new NotSupportedException(k_OnSelectExitedDeprecated);
        }

        /// <summary>
        /// Selected Interactable for this Interactor (may be <see langword="null"/>).
        /// </summary>
        /// <seealso cref="XRBaseInteractable.selectingInteractor"/>
        /// <remarks>
        /// <c>selectTarget</c> has been deprecated. Use <see cref="interactablesSelected"/>, <see cref="XRSelectInteractorExtensions.GetOldestInteractableSelected(IXRSelectInteractor)"/>, <see cref="hasSelection"/>, or <see cref="XRBaseInteractor.IsSelecting(IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectTargetDeprecated, true)]
        public XRBaseInteractable selectTarget
        {
            get
            {
                Debug.LogError(k_SelectTargetDeprecated, this);
                throw new NotSupportedException(k_SelectTargetDeprecated);
            }
            protected set
            {
                _ = value;
                Debug.LogError(k_SelectTargetDeprecated, this);
                throw new NotSupportedException(k_SelectTargetDeprecated);
            }
        }

        /// <summary>
        /// Target Interactables that are currently being hovered over (may by empty).
        /// </summary>
        /// <seealso cref="XRBaseInteractable.hoveringInteractors"/>
        /// <remarks>
        /// <c>hoverTargets</c> has been deprecated. Use <see cref="interactablesHovered"/> instead.
        /// </remarks>
        [Obsolete(k_HoverTargetsDeprecated, true)]
        protected List<XRBaseInteractable> hoverTargets
        {
            get
            {
                Debug.LogError(k_HoverTargetsDeprecated, this);
                throw new NotSupportedException(k_HoverTargetsDeprecated);
            }
        }

        /// <summary>
        /// Retrieves a copy of the list of target Interactables that are currently being hovered over.
        /// </summary>
        /// <param name="targets">The results list to store hover targets into.</param>
        /// <remarks>
        /// Clears <paramref name="targets"/> before adding to it.
        /// <br />
        /// <c>GetHoverTargets</c> has been deprecated. Use <see cref="interactablesHovered"/> instead.
        /// </remarks>
        [Obsolete(k_GetHoverTargetsDeprecated, true)]
        public void GetHoverTargets(List<XRBaseInteractable> targets)
        {
            Debug.LogError(k_GetHoverTargetsDeprecated, this);
            throw new NotSupportedException(k_GetHoverTargetsDeprecated);
        }

        /// <summary>
        /// Retrieve the list of Interactables that this Interactor could possibly interact with this frame.
        /// This list is sorted by priority (with highest priority first).
        /// </summary>
        /// <param name="targets">The results list to populate with Interactables that are valid for selection or hover.</param>
        /// <remarks>
        /// <c>GetValidTargets(List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="GetValidTargets(List{IXRInteractable})"/> instead.
        /// <see cref="XRInteractionManager.GetValidTargets(IXRInteractor, List{IXRInteractable})"/> will stitch the results together with <c>GetValidTargets(List&lt;IXRInteractable&gt;)</c>,
        /// but by default this method now does nothing.
        /// </remarks>
        [Obsolete(k_GetValidTargetsDeprecated, true)]
        public virtual void GetValidTargets(List<XRBaseInteractable> targets)
        {
            Debug.LogError(k_GetValidTargetsDeprecated, this);
            throw new NotSupportedException(k_GetValidTargetsDeprecated);
        }

        /// <summary>
        /// Determines if the Interactable is valid for hover this frame.
        /// </summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns>Returns <see langword="true"/> if the interactable can be hovered over this frame.</returns>
        /// <seealso cref="XRBaseInteractable.IsHoverableBy(XRBaseInteractor)"/>
        /// <remarks>
        /// <c>CanHover(XRBaseInteractable)</c> has been deprecated. Use <see cref="CanHover(IXRHoverInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_CanHoverDeprecated, true)]
        public virtual bool CanHover(XRBaseInteractable interactable)
        {
            Debug.LogError(k_CanHoverDeprecated, this);
            throw new NotSupportedException(k_CanHoverDeprecated);
        }

        /// <summary>
        /// Determines if the Interactable is valid for selection this frame.
        /// </summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns>Returns <see langword="true"/> if the Interactable can be selected this frame.</returns>
        /// <seealso cref="XRBaseInteractable.IsSelectableBy(XRBaseInteractor)"/>
        /// <remarks>
        /// <c>CanSelect(XRBaseInteractable)</c> has been deprecated. Use <see cref="CanSelect(IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_CanSelectDeprecated, true)]
        public virtual bool CanSelect(XRBaseInteractable interactable)
        {
            Debug.LogError(k_CanSelectDeprecated, this);
            throw new NotSupportedException(k_CanSelectDeprecated);
        }

        /// <summary>
        /// (Deprecated) (Read Only) Indicates whether this Interactor requires that an Interactable is not currently selected to begin selecting it.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/>, the Interaction Manager will only begin a selection when the Interactable is not currently selected.
        /// </remarks>
        /// <remarks>
        /// <c>requireSelectExclusive</c> has been deprecated. Put logic in <see cref="CanSelect(IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_RequireSelectExclusiveDeprecated, true)]
        public virtual bool requireSelectExclusive
        {
            get
            {
                Debug.LogError(k_RequireSelectExclusiveDeprecated, this);
                throw new NotSupportedException(k_RequireSelectExclusiveDeprecated);
            }
        }

        /// <summary>
        /// Manually initiate selection of an Interactable.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="EndManualInteraction"/>
        /// <remarks>
        /// <c>StartManualInteraction(XRBaseInteractable)</c> has been deprecated. Use <see cref="StartManualInteraction(IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_StartManualInteractionDeprecated, true)]
        public virtual void StartManualInteraction(XRBaseInteractable interactable)
        {
            Debug.LogError(k_StartManualInteractionDeprecated, this);
            throw new NotSupportedException(k_StartManualInteractionDeprecated);
        }
    }
}
