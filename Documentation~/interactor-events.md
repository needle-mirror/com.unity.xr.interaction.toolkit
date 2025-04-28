---
uid: xri-interactor-events
---
# Interactor Events

These are events that can be hooked into in the editor the same way you would respond to a UI button press. These apply to Interactors - objects that can interact with Interactables.

The UI Hover Entered and UI Hover Exited events are not available on all interactor components. Only those interactors that implement [`IUIHoverInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.UI.IUIHoverInteractor) and enable UI Interaction with the [XR UI Input Module](xref:xri-ui-input-module), such as [XR Poke Interactor](xref:xri-xr-poke-interactor), will have these events.

| **Property** | **Description** |
|---|---|
| **Hover Entered** | The event that is called when this Interactor begins hovering over an Interactable.<br />The `HoverEnterEventArgs` passed to each listener is only valid while the event is invoked, don't hold a reference to it. |
| **Hover Exited** | The event that is called when this Interactor ends hovering over an Interactable.<br />The `HoverExitEventArgs` passed to each listener is only valid while the event is invoked, don't hold a reference to it. |
| **Select Entered** | The event that is called when this Interactor begins selecting an Interactable.<br />The `SelectEnterEventArgs` passed to each listener is only valid while the event is invoked, don't hold a reference to it. |
| **Select Exited** | The event that is called when this Interactor ends selecting an Interactable.<br />The `SelectExitEventArgs` passed to each listener is only valid while the event is invoked, don't hold a reference to it. |
| **UI Hover Entered Event** | Event triggered when a UI element is hovered over by this interactor. |
| **UI Hover Exited Event** | Event triggered when a UI element is no longer hovered over by this interactor. |
