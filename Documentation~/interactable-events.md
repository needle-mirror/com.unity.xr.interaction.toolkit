---
uid: xri-interactable-events
---
# Interactable Events

You can assign listener functions for interaction events in the [Inspector window](xref:um-unity-events) and in a C# class. These apply events to interactables - objects that interactors can interact with.

For detailed code examples showing how to subscribe to these events and work with the event arguments, refer to [Handle interaction events](interaction-event-usage-examples.md).

| **Property** | **Description** |
|---|---|
| **First Hover Entered** | The event that is called only when the first interactor begins hovering over this interactable as the sole hovering interactor. Subsequent interactors that begin hovering over this interactable will not cause this event to be invoked as long as any others are still hovering. |
| **Last Hover Exited** | The event that is called only when the last remaining hovering interactor ends hovering over this interactable. |
| **Hover Entered** | The event that is called when an interactor begins hovering over this interactable. |
| **Hover Exited** | The event that is called when an interactor ends hovering over this interactable. |
| **First Select Entered** | The event that is called only when the first interactor begins selecting this interactable as the sole selecting interactor. Subsequent interactors that begin selecting this interactable will not cause this event to be invoked as long as any others are still selecting. |
| **Last Select Exited** | The event that is called only when the last remaining selecting interactor ends selecting this interactable. |
| **Select Entered** | The event that is called when an interactor begins selecting this interactable. |
| **Select Exited** | The event that is called when an interactor ends selecting this interactable.|
| **First Focus Entered** | The event that is called only when the first interactor gives focus to this interactable as the sole focusing interactor. Subsequent interactors that give focus to this interactable will not cause this event to be invoked as long as any others are still holding focus. |
| **Last Focus Exited** | The event that is called only when the last remaining focusing interactor releases focus of this interactable. |
| **Focus Entered** | The event that is called when an interactor gives focus to this interactable. |
| **Focus Exited** | The event that is called when an interactor releases focus of this interactable. |
| **Activated** | The event that is called when the selecting interactor activates this interactable.<br />Not to be confused with activating or deactivating a `GameObject` with `GameObject.SetActive`. This is a generic event when an interactor wants to activate an Interactable, such as from a trigger pull on a controller.|
| **Deactivated** | The event that is called when an interactor deactivates this interactable.<br />Not to be confused with activating or deactivating a `GameObject` with `GameObject.SetActive`. This is a generic event when an interactor wants to deactivate an interactable, such as from a trigger release on a controller. |
