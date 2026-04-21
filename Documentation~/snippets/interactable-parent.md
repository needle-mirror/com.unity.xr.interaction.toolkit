<!--
Parent interactable options for Simple and Grab Interactables

To include this file (adjust heading level and include file path as needed):

## Parent interactable {#parent-interactable}

[!INCLUDE [interactable-parent](snippets/interactable-parent.md)]
-->

A parent interactable is processed before any of its children. When you have a specific order in which you need a set of interactables to be processed by the interaction manager, you can assign a hierarchy using the **Parent Interactable** fields of these interactables.

At the end of the [interaction update loop](xref:xri-update-loop), the interaction manager calls the [`ProcessInteractable`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.ProcessInteractable(UnityEngine.XR.Interaction.Toolkit.XRInteractionUpdateOrder.UpdatePhase)) method of the interactable objects in the scene. An interactable implementation can use this method to update its internal state. For example, the grab interactable implementation applies any needed transform changes while it is selected by an interactor in its `ProcessInteractable` method. The order in which interactables are processed by the manager defaults to the order in which the interactable objects register themselves with the manager. You can alter the processing order by establishing a parent-child relationship between interactables.

Set **Auto Find Parent Interactable** to `true` to use [`GetComponentInParent`](xref:UnityEngine.GameObject.GetComponentInParent(System.Boolean)) to find and assign an interactable parent. Inactive GameObjects are included in the search. Using this option has a small startup performance cost compared to explicitly assigning a parent.

> [!NOTE]
> You can also use the [`RegisterParentRelationship`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.RegisterParentRelationship(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable,UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable)) and [`UnregisterParentRelationship`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.UnregisterParentRelationship(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable,UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable)) methods on the [`XRInteractionManager`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager) in a script to establish a parent-child processing hierarchy.
