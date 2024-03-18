using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial class XRInteractionManager
    {
        const string k_RegisterInteractorDeprecated = "RegisterInteractor(XRBaseInteractor) has been deprecated. Use RegisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractor((IXRInteractor)this)` instead.";
        const string k_UnregisterInteractorDeprecated = "UnregisterInteractor(XRBaseInteractor) has been deprecated. Use UnregisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractor((IXRInteractor)this)` instead.";
        const string k_RegisterInteractableDeprecated = "RegisterInteractable(XRBaseInteractable) has been deprecated. Use RegisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractable((IXRInteractable)this)` instead.";
        const string k_UnregisterInteractableDeprecated = "UnregisterInteractable(XRBaseInteractable) has been deprecated. Use UnregisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractable((IXRInteractable)this)` instead.";
        const string k_GetRegisteredInteractorsDeprecated = "GetRegisteredInteractors(List<XRBaseInteractor>) has been deprecated. Use GetRegisteredInteractors(List<IXRInteractor>) instead.";
        const string k_GetRegisteredInteractablesDeprecated = "GetRegisteredInteractables(List<XRBaseInteractable>) has been deprecated. Use GetRegisteredInteractables(List<IXRInteractable>) instead.";
        const string k_IsRegisteredInteractorDeprecated = "IsRegistered(XRBaseInteractor) has been deprecated. Use IsRegistered(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractor)this)` instead.";
        const string k_IsRegisteredInteractableDeprecated = "IsRegistered(XRBaseInteractable) has been deprecated. Use IsRegistered(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractable)this)` instead.";
        const string k_TryGetInteractableForColliderDeprecated = "TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)";
        const string k_GetInteractableForColliderDeprecated = "GetInteractableForCollider has been deprecated. Use TryGetInteractableForCollider(Collider, out IXRInteractable) instead.";
        const string k_GetColliderToInteractableMapDeprecated = "GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.";
        const string k_GetValidTargetsDeprecated = "GetValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use GetValidTargets(IXRInteractor, List<IXRInteractable>) instead.";
        const string k_ForceSelectDeprecated = "ForceSelect(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead.";
        const string k_ClearInteractorSelectionDeprecated = "ClearInteractorSelection(XRBaseInteractor) has been deprecated. Use ClearInteractorSelection(IXRSelectInteractor, List<IXRInteractable>) instead.";
        const string k_CancelInteractorSelectionDeprecated = "CancelInteractorSelection(XRBaseInteractor) has been deprecated. Use CancelInteractorSelection(IXRSelectInteractor) instead.";
        const string k_CancelInteractableSelectionDeprecated = "CancelInteractableSelection(XRBaseInteractable) has been deprecated. Use CancelInteractableSelection(IXRSelectInteractable) instead.";
        const string k_ClearInteractorHoverDeprecated = "ClearInteractorHover(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use ClearInteractorHover(IXRHoverInteractor, List<IXRInteractable>) instead.";
        const string k_CancelInteractorHoverDeprecated = "CancelInteractorHover(XRBaseInteractor) has been deprecated. Use CancelInteractorHover(IXRHoverInteractor) instead.";
        const string k_CancelInteractableHoverDeprecated = "CancelInteractableHover(XRBaseInteractable) has been deprecated. Use CancelInteractableHover(IXRHoverInteractable) instead.";
        const string k_SelectEnterDeprecated = "SelectEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";
        const string k_SelectExitDeprecated = "SelectExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";
        const string k_SelectCancelDeprecated = "SelectCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectCancel(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectCancel((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";
        const string k_HoverEnterDeprecated = "HoverEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverEnter((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";
        const string k_HoverExitDeprecated = "HoverExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverExit((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";
        const string k_HoverCancelDeprecated = "HoverCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverCancel(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverCancel((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";
        const string k_SelectEnterProtectedDeprecated = "SelectEnter(XRBaseInteractor, XRBaseInteractable, SelectEnterEventArgs) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable, SelectEnterEventArgs) instead.";
        const string k_SelectExitProtectedDeprecated = "SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs) instead.";
        const string k_HoverEnterProtectedDeprecated = "HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs) instead.";
        const string k_HoverExitProtectedDeprecated = "HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs) instead.";
        const string k_InteractorSelectValidTargetsDeprecated = "InteractorSelectValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorSelectValidTargets(IXRSelectInteractor, List<IXRInteractable>) instead.";
        const string k_InteractorHoverValidTargetsDeprecated = "InteractorHoverValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorHoverValidTargets(IXRHoverInteractor, List<IXRInteractable>) instead.";

        /// <inheritdoc cref="RegisterInteractor(IXRInteractor)"/>
        /// <remarks>
        /// <c>RegisterInteractor(XRBaseInteractor)</c> has been deprecated. Use <see cref="RegisterInteractor(IXRInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_RegisterInteractorDeprecated, true)]
        public virtual void RegisterInteractor(XRBaseInteractor interactor)
        {
            Debug.LogError(k_RegisterInteractorDeprecated, this);
            throw new NotSupportedException(k_RegisterInteractorDeprecated);
        }

        /// <inheritdoc cref="UnregisterInteractor(IXRInteractor)"/>
        /// <remarks>
        /// <c>UnregisterInteractor(XRBaseInteractor)</c> has been deprecated. Use <see cref="UnregisterInteractor(IXRInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_UnregisterInteractorDeprecated, true)]
        public virtual void UnregisterInteractor(XRBaseInteractor interactor)
        {
            Debug.LogError(k_UnregisterInteractorDeprecated, this);
            throw new NotSupportedException(k_UnregisterInteractorDeprecated);
        }

        /// <inheritdoc cref="RegisterInteractable(IXRInteractable)"/>
        /// <remarks>
        /// <c>RegisterInteractable(XRBaseInteractable)</c> has been deprecated. Use <see cref="RegisterInteractable(IXRInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_RegisterInteractableDeprecated, true)]
        public virtual void RegisterInteractable(XRBaseInteractable interactable)
        {
            Debug.LogError(k_RegisterInteractableDeprecated, this);
            throw new NotSupportedException(k_RegisterInteractableDeprecated);
        }

        /// <inheritdoc cref="UnregisterInteractable(IXRInteractable)"/>
        /// <remarks>
        /// <c>UnregisterInteractable(XRBaseInteractable)</c> has been deprecated. Use <see cref="UnregisterInteractable(IXRInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_UnregisterInteractableDeprecated, true)]
        public virtual void UnregisterInteractable(XRBaseInteractable interactable)
        {
            Debug.LogError(k_UnregisterInteractableDeprecated, this);
            throw new NotSupportedException(k_UnregisterInteractableDeprecated);
        }

        /// <inheritdoc cref="GetRegisteredInteractors(List{IXRInteractor})"/>
        /// <remarks>
        /// <c>GetRegisteredInteractors(List&lt;XRBaseInteractor&gt;)</c> has been deprecated. Use <see cref="GetRegisteredInteractors(List{IXRInteractor})"/> instead.
        /// </remarks>
        [Obsolete(k_GetRegisteredInteractorsDeprecated, true)]
        public void GetRegisteredInteractors(List<XRBaseInteractor> results)
        {
            Debug.LogError(k_GetRegisteredInteractorsDeprecated, this);
            throw new NotSupportedException(k_GetRegisteredInteractorsDeprecated);
        }

        /// <inheritdoc cref="GetRegisteredInteractables(List{IXRInteractable})"/>
        /// <remarks>
        /// <c>GetRegisteredInteractables(List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="GetRegisteredInteractables(List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_GetRegisteredInteractablesDeprecated, true)]
        public void GetRegisteredInteractables(List<XRBaseInteractable> results)
        {
            Debug.LogError(k_GetRegisteredInteractablesDeprecated, this);
            throw new NotSupportedException(k_GetRegisteredInteractablesDeprecated);
        }

        /// <inheritdoc cref="IsRegistered(IXRInteractor)"/>
        /// <remarks>
        /// <c>IsRegistered(XRBaseInteractor)</c> has been deprecated. Use <see cref="IsRegistered(IXRInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_IsRegisteredInteractorDeprecated, true)]
        public bool IsRegistered(XRBaseInteractor interactor)
        {
            Debug.LogError(k_IsRegisteredInteractorDeprecated, this);
            throw new NotSupportedException(k_IsRegisteredInteractorDeprecated);
        }

        /// <inheritdoc cref="IsRegistered(IXRInteractable)"/>
        /// <remarks>
        /// <c>IsRegistered(XRBaseInteractable)</c> has been deprecated. Use <see cref="IsRegistered(IXRInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_IsRegisteredInteractableDeprecated, true)]
        public bool IsRegistered(XRBaseInteractable interactable)
        {
            Debug.LogError(k_IsRegisteredInteractableDeprecated, this);
            throw new NotSupportedException(k_IsRegisteredInteractableDeprecated);
        }

        /// <inheritdoc cref="GetInteractableForCollider"/>
        /// <remarks>
        /// <c>TryGetInteractableForCollider</c> has been deprecated. Use <see cref="GetInteractableForCollider"/> instead.
        /// </remarks>
        [Obsolete(k_TryGetInteractableForColliderDeprecated, true)]
        public XRBaseInteractable TryGetInteractableForCollider(Collider interactableCollider)
        {
            Debug.LogError(k_TryGetInteractableForColliderDeprecated, this);
            throw new NotSupportedException(k_TryGetInteractableForColliderDeprecated);
        }

        /// <summary>
        /// Gets the Interactable a specific collider is attached to.
        /// </summary>
        /// <param name="interactableCollider">The collider of the Interactable to retrieve.</param>
        /// <returns>Returns the Interactable that the collider is attached to. Otherwise returns <see langword="null"/> if no such Interactable exists.</returns>
        /// <remarks>
        /// <c>GetInteractableForCollider</c> has been deprecated. Use <see cref="TryGetInteractableForCollider(Collider, out IXRInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_GetInteractableForColliderDeprecated, true)]
        public XRBaseInteractable GetInteractableForCollider(Collider interactableCollider)
        {
            Debug.LogError(k_GetInteractableForColliderDeprecated, this);
            throw new NotSupportedException(k_GetInteractableForColliderDeprecated);
        }

        /// <summary>
        /// Gets the dictionary that has all the registered colliders and their associated Interactable.
        /// </summary>
        /// <param name="map">When this method returns, contains the dictionary that has all the registered colliders and their associated Interactable.</param>
        /// <remarks>
        /// Clears <paramref name="map"/> before adding to it.
        /// <br />
        /// <c>GetColliderToInteractableMap</c> has been deprecated. GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.
        /// </remarks>
        [Obsolete(k_GetColliderToInteractableMapDeprecated, true)]
        public void GetColliderToInteractableMap(ref Dictionary<Collider, XRBaseInteractable> map)
        {
            Debug.LogError(k_GetColliderToInteractableMapDeprecated, this);
            throw new NotSupportedException(k_GetColliderToInteractableMapDeprecated);
        }

        /// <summary>
        /// For the provided <paramref name="interactor"/>, returns a list of the valid Interactables that can be hovered over or selected.
        /// </summary>
        /// <param name="interactor">The Interactor whose valid targets we want to find.</param>
        /// <param name="validTargets">List to be filled with valid targets of the Interactor.</param>
        /// <returns>The list of valid targets of the Interactor.</returns>
        /// <seealso cref="IXRInteractor.GetValidTargets"/>
        /// <remarks>
        /// <c>GetValidTargets(XRBaseInteractor, List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="GetValidTargets(IXRInteractor, List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_GetValidTargetsDeprecated, true)]
        public List<XRBaseInteractable> GetValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            Debug.LogError(k_GetValidTargetsDeprecated, this);
            throw new NotSupportedException(k_GetValidTargetsDeprecated);
        }

        /// <summary>
        /// Manually forces selection of an Interactable. This is different than starting manual interaction.
        /// </summary>
        /// <param name="interactor">The Interactor that will select the Interactable.</param>
        /// <param name="interactable">The Interactable to be selected.</param>
        /// <remarks>
        /// <c>ForceSelect(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="SelectEnter(IXRSelectInteractor, IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_ForceSelectDeprecated, true)]
        public void ForceSelect(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_ForceSelectDeprecated, this);
            throw new NotSupportedException(k_ForceSelectDeprecated);
        }

        /// <summary>
        /// Automatically called each frame during Update to clear the selection of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its selection state.</param>
        /// <remarks>
        /// <c>ClearInteractorSelection(XRBaseInteractor)</c> has been deprecated. Use <see cref="ClearInteractorSelection(IXRSelectInteractor, List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_ClearInteractorSelectionDeprecated, true)]
        public virtual void ClearInteractorSelection(XRBaseInteractor interactor)
        {
            Debug.LogError(k_ClearInteractorSelectionDeprecated, this);
            throw new NotSupportedException(k_ClearInteractorSelectionDeprecated);
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered to cancel the selection of the Interactor if necessary.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its selection state due to cancellation.</param>
        /// <remarks>
        /// <c>CancelInteractorSelection(XRBaseInteractor)</c> has been deprecated. Use <see cref="CancelInteractorSelection(IXRSelectInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_CancelInteractorSelectionDeprecated, true)]
        public virtual void CancelInteractorSelection(XRBaseInteractor interactor)
        {
            Debug.LogError(k_CancelInteractorSelectionDeprecated, this);
            throw new NotSupportedException(k_CancelInteractorSelectionDeprecated);
        }

        /// <summary>
        /// Automatically called when an Interactable is unregistered to cancel the selection of the Interactable if necessary.
        /// </summary>
        /// <param name="interactable">The Interactable to potentially exit its selection state due to cancellation.</param>
        /// <remarks>
        /// <c>CancelInteractableSelection(XRBaseInteractable)</c> has been deprecated. Use <see cref="CancelInteractableSelection(IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_CancelInteractableSelectionDeprecated, true)]
        public virtual void CancelInteractableSelection(XRBaseInteractable interactable)
        {
            Debug.LogError(k_CancelInteractableSelectionDeprecated, this);
            throw new NotSupportedException(k_CancelInteractableSelectionDeprecated);
        }

        /// <summary>
        /// Automatically called each frame during Update to clear the hover state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its hover state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        /// <remarks>
        /// <c>ClearInteractorHover(XRBaseInteractor, List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="ClearInteractorHover(IXRHoverInteractor, List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_ClearInteractorHoverDeprecated, true)]
        public virtual void ClearInteractorHover(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            Debug.LogError(k_ClearInteractorHoverDeprecated, this);
            throw new NotSupportedException(k_ClearInteractorHoverDeprecated);
        }

        /// <summary>
        /// Automatically called when an Interactor is unregistered to cancel the hover state of the Interactor if necessary.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially exit its hover state due to cancellation.</param>
        /// <remarks>
        /// <c>CancelInteractorHover(XRBaseInteractor)</c> has been deprecated. Use <see cref="CancelInteractorHover(IXRHoverInteractor)"/> instead.
        /// </remarks>
        [Obsolete(k_CancelInteractorHoverDeprecated, true)]
        public virtual void CancelInteractorHover(XRBaseInteractor interactor)
        {
            Debug.LogError(k_CancelInteractorHoverDeprecated, this);
            throw new NotSupportedException(k_CancelInteractorHoverDeprecated);
        }

        /// <summary>
        /// Automatically called when an Interactable is unregistered to cancel the hover state of the Interactable if necessary.
        /// </summary>
        /// <param name="interactable">The Interactable to potentially exit its hover state due to cancellation.</param>
        /// <remarks>
        /// <c>CancelInteractableHover(XRBaseInteractable)</c> has been deprecated. Use <see cref="CancelInteractableHover(IXRHoverInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_CancelInteractableHoverDeprecated, true)]
        public virtual void CancelInteractableHover(XRBaseInteractable interactable)
        {
            Debug.LogError(k_CancelInteractableHoverDeprecated, this);
            throw new NotSupportedException(k_CancelInteractableHoverDeprecated);
        }

        /// <summary>
        /// Initiates selection of an Interactable by an Interactor. This method may first result in other interaction events
        /// such as causing the Interactable to first exit being selected.
        /// </summary>
        /// <param name="interactor">The Interactor that is selecting.</param>
        /// <param name="interactable">The Interactable being selected.</param>
        /// <remarks>
        /// This attempt may be ignored depending on the selection policy of the Interactor and/or the Interactable.
        /// <br />
        /// <c>SelectEnter(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="SelectEnter(IXRSelectInteractor, IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectEnterDeprecated, true)]
        public virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_SelectEnterDeprecated, this);
            throw new NotSupportedException(k_SelectEnterDeprecated);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        /// <remarks>
        /// <c>SelectExit(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="SelectExit(IXRSelectInteractor, IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectExitDeprecated, true)]
        public virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_SelectExitDeprecated, this);
            throw new NotSupportedException(k_SelectExitDeprecated);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor due to cancellation,
        /// such as from either being unregistered due to being disabled or destroyed.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        /// <remarks>
        /// <c>SelectCancel(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="SelectCancel(IXRSelectInteractor, IXRSelectInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectCancelDeprecated, true)]
        public virtual void SelectCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_SelectCancelDeprecated, this);
            throw new NotSupportedException(k_SelectCancelDeprecated);
        }

        /// <summary>
        /// Initiates hovering of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is hovering.</param>
        /// <param name="interactable">The Interactable being hovered over.</param>
        /// <remarks>
        /// <c>HoverEnter(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="HoverEnter(IXRHoverInteractor, IXRHoverInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_HoverEnterDeprecated, true)]
        public virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_HoverEnterDeprecated, this);
            throw new NotSupportedException(k_HoverEnterDeprecated);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        /// <remarks>
        /// <c>HoverExit(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="HoverExit(IXRHoverInteractor, IXRHoverInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_HoverExitDeprecated, true)]
        public virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_HoverExitDeprecated, this);
            throw new NotSupportedException(k_HoverExitDeprecated);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor due to cancellation,
        /// such as from either being unregistered due to being disabled or destroyed.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        /// <remarks>
        /// <c>HoverCancel(XRBaseInteractor, XRBaseInteractable)</c> has been deprecated. Use <see cref="HoverCancel(IXRHoverInteractor, IXRHoverInteractable)"/> instead.
        /// </remarks>
        [Obsolete(k_HoverCancelDeprecated, true)]
        public virtual void HoverCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            Debug.LogError(k_HoverCancelDeprecated, this);
            throw new NotSupportedException(k_HoverCancelDeprecated);
        }

        /// <summary>
        /// Initiates selection of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is selecting.</param>
        /// <param name="interactable">The Interactable being selected.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        /// <remarks>
        /// <c>SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs)</c> has been deprecated. Use <see cref="SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectEnterProtectedDeprecated, true)]
        protected virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectEnterEventArgs args)
        {
            Debug.LogError(k_SelectEnterProtectedDeprecated, this);
            throw new NotSupportedException(k_SelectEnterProtectedDeprecated);
        }

        /// <summary>
        /// Initiates ending selection of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer selecting.</param>
        /// <param name="interactable">The Interactable that is no longer being selected.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        /// <remarks>
        /// <c>SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs)</c> has been deprecated. Use <see cref="SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs)"/> instead.
        /// </remarks>
        [Obsolete(k_SelectExitProtectedDeprecated, true)]
        protected virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectExitEventArgs args)
        {
            Debug.LogError(k_SelectExitProtectedDeprecated, this);
            throw new NotSupportedException(k_SelectExitProtectedDeprecated);
        }

        /// <summary>
        /// Initiates hovering of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is hovering.</param>
        /// <param name="interactable">The Interactable being hovered over.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        /// <remarks>
        /// <c>HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs)</c> has been deprecated. Use <see cref="HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs)"/> instead.
        /// </remarks>
        [Obsolete(k_HoverEnterProtectedDeprecated, true)]
        protected virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverEnterEventArgs args)
        {
            Debug.LogError(k_HoverEnterProtectedDeprecated, this);
            throw new NotSupportedException(k_HoverEnterProtectedDeprecated);
        }

        /// <summary>
        /// Initiates ending hovering of an Interactable by an Interactor, passing the given <paramref name="args"/>.
        /// </summary>
        /// <param name="interactor">The Interactor that is no longer hovering.</param>
        /// <param name="interactable">The Interactable that is no longer being hovered over.</param>
        /// <param name="args">Event data containing the Interactor and Interactable involved in the event.</param>
        /// <remarks>
        /// <c>HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs)</c> has been deprecated. Use <see cref="HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs)"/> instead.
        /// </remarks>
        [Obsolete(k_HoverExitProtectedDeprecated, true)]
        protected virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverExitEventArgs args)
        {
            Debug.LogError(k_HoverExitProtectedDeprecated, this);
            throw new NotSupportedException(k_HoverExitProtectedDeprecated);
        }

        /// <summary>
        /// Automatically called each frame during Update to enter the selection state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially enter its selection state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        /// <remarks>
        /// <c>InteractorSelectValidTargets(XRBaseInteractor, List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="InteractorSelectValidTargets(IXRSelectInteractor, List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_InteractorSelectValidTargetsDeprecated, true)]
        protected virtual void InteractorSelectValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            Debug.LogError(k_InteractorSelectValidTargetsDeprecated, this);
            throw new NotSupportedException(k_InteractorSelectValidTargetsDeprecated);
        }

        /// <summary>
        /// Automatically called each frame during Update to enter the hover state of the Interactor if necessary due to current conditions.
        /// </summary>
        /// <param name="interactor">The Interactor to potentially enter its hover state.</param>
        /// <param name="validTargets">The list of interactables that this Interactor could possibly interact with this frame.</param>
        /// <remarks>
        /// <c>InteractorHoverValidTargets(XRBaseInteractor, List&lt;XRBaseInteractable&gt;)</c> has been deprecated. Use <see cref="InteractorHoverValidTargets(IXRHoverInteractor, List{IXRInteractable})"/> instead.
        /// </remarks>
        [Obsolete(k_InteractorHoverValidTargetsDeprecated, true)]
        protected virtual void InteractorHoverValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
        {
            Debug.LogError(k_InteractorHoverValidTargetsDeprecated, this);
            throw new NotSupportedException(k_InteractorHoverValidTargetsDeprecated);
        }
    }
}
