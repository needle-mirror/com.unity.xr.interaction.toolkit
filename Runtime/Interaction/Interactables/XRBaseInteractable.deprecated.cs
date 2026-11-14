
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
    }
}
