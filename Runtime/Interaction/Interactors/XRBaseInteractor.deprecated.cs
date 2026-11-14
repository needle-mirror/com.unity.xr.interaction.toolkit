
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
    }
}
