<!-- Note -- there is an existing page with this information (not an include).

All interactors

To include this file (adjust heading level and include file path as needed):

## Interactor Events {#interactor-events}

[!INCLUDE [interactor-events](snippets/interactor-events.md)]
-->

You can assign listener functions for interaction events in the [Inspector window](xref:um-unity-events) and in a C# class. These events apply to interactors - objects that can interact with interactables.

The UI Hover Entered and UI Hover Exited events are not available on all interactor components. Only those interactors that implement [`IUIHoverInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.UI.IUIHoverInteractor) and enable UI Interaction with the [XR UI Input Module](xref:xri-ui-input-module), such as [XR Poke Interactor](xref:xri-xr-poke-interactor), will have these events.

For detailed code examples showing how to subscribe to events and work with the event arguments, refer to [Handle interaction events](xref:xri-interaction-event-handling).

| **Property** | **Description** |
|---|---|
| **Hover Entered** | The event that is called when this interactor begins hovering over an interactable. |
| **Hover Exited** | The event that is called when this interactor ends hovering over an interactable. |
| **Select Entered** | The event that is called when this interactor begins selecting an interactable.|
| **Select Exited** | The event that is called when this interactor ends selecting an interactable. |
| **UI Hover Entered Event** | Event triggered when a UI element is hovered over by this interactor. |
| **UI Hover Exited Event** | Event triggered when a UI element is no longer hovered over by this interactor. |

> [!NOTE]
> Additional events might be available to listeners you assign in C# code. Refer to the [`XRBaseInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor) subclass documentation for specific information.
