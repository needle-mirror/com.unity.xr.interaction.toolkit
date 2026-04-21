<!-- Ray, Direct, Gaze,

## Haptic Events (deprecated)

[!INCLUDE [interactor-haptic-events](snippets/interactor-haptic-events.md)]
-->


The **Haptic Events** section is deprecated and might be removed in a future release. Use the [Simple Haptic Feedback component](xref:xri-simple-haptic-feedback) instead.

![Haptic event options](../images/xr-ray-haptic-events.png)

| **Property** | **Description** |
| :--- | :--- |
| **On Select Entered** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor begins selecting an interactable. |
| **On Select Exited** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor successfully exits selection of an interactable. |
| **On Select Canceled** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor cancels selection of an interactable. |
| **On Hover Entered** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor begins hovering over an interactable. |
| **On Hover Exited** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor successfully ends hovering over an interactable. |
| **On Hover Canceled** | If enabled, the Unity editor will display UI for supplying the duration (in seconds) and intensity (normalized) to play in haptic feedback when this interactor cancels hovering over an interactable. |
| **Allow Hover Haptics While Selecting** | Whether to allow playing haptics from hover events if the hovered interactable is currently selected by this interactor. This is enabled by default. |
