<!-- Ray, Direct, Gaze,
## Audio Events (deprecated)

[!INCLUDE [interactor-audio-events](snippets/interactor-audio-events.md)]
-->


The **Audio Events** section is deprecated and might be removed in a future release. Use the [Simple Audio Feedback component](xref:xri-simple-audio-feedback) instead.

![Audio event options](../images/xr-ray-audio-events.png)

| **Property** | **Description** |
| :--- | :--- |
| **On Select Entered** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor begins selecting an interactable. |
| **On Select Exited** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor successfully exits selection of an interactable. |
| **On Select Canceled** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor cancels selection of an interactable. |
| **On Hover Entered** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor begins hovering over an interactable. |
| **On Hover Exited** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor successfully ends hovering over an interactable. |
| **On Hover Canceled** | If enabled, the Unity editor will display UI for supplying the audio clip to play when this interactor cancels hovering over an interactable. |
| **Allow Hover Audio While Selecting** | Whether to allow playing audio from hover events if the hovered interactable is currently selected by this interactor. This is enabled by default. |
