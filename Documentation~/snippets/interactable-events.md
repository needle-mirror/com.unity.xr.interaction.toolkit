<!-- Note -- there is an existing page with this information (not an include).

All interactables

To include this file (adjust heading level and include file path as needed):

## Interactable Events {#interactable-events}

[!INCLUDE [interactable-events](snippets/interactable-events.md)]
-->

You can assign listener functions for interaction events in the [Inspector window](xref:um-unity-events) and in a C# class. These events apply to interactables - objects that interactors can interact with.

For detailed code examples showing how to subscribe to events and work with the event arguments, refer to [Handle interaction events](xref:xri-interaction-event-handling).

| **Property** | **Description** |
|---|---|
| **First Hover Entered** | The event that is called only when the first interactor begins hovering over this interactable as the sole hovering interactor. Subsequent interactors that begin hovering over this interactable will not cause this event to be invoked as long as any others are still hovering. |
| **Last Hover Exited** | The event that is called only when the last remaining hovering interactor ends hovering over this interactable. |
| **Hover Entered** | The event that is called when an interactor begins hovering over this interactable. |
| **Hover Exited** | The event that is called when an interactor ends hovering over this interactable. |
| **First Select Entered** | The event that is called only when the first interactor begins selecting this interactable as the sole selecting interactor. Subsequent interactors that begin selecting this interactable will not cause this event to be invoked as long as any others are still selecting. |
| **Last Select Exited** | The event that is called only when the last remaining selecting interactor ends selecting this interactable. |
| **Select Entered** | The event that is called when an interactor begins selecting this interactable. |
| **Select Exited** | The event that is called when an interactor ends selecting this interactable. |
| **Activated** | The event that is called when the selecting interactor activates this interactable.<br />Not to be confused with activating or deactivating a `GameObject` with `GameObject.SetActive`. This is a generic event when an interactor wants to activate an interactable, such as from a trigger pull on a controller. |
| **Deactivated** | The event that is called when an interactor deactivates this interactable.<br />Not to be confused with activating or deactivating a `GameObject` with `GameObject.SetActive`. This is a generic event when an interactor wants to deactivate an interactable, such as from a trigger release on a controller. |

> [!NOTE]
> Additional events might be available to listeners you assign in C# code. Refer to the [`XRBaseInteractable`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable) subclass documentation for specific information.
