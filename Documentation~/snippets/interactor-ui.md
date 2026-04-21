<!--
## UI Interaction properties {#ui-interaction}

[!INCLUDE [interactor-ui](snippets/interactor-ui.md)]
-->

To enable UI interaction, enable the **UI Interaction** checkbox. When enabled, additional options are added to the Inspector.

> [!NOTE]
> You must add the [XR UI Input Module](xref:xri-ui-input-module) to the [UI Event System](xref:input-system-ui-support) in the scene for any interactors to interact with UI elements.

Use the options in the **UI Interaction** section to configure how the interactor operates with UI elements on a canvas.

![UI interaction options](../images/xr-ray-ui-interaction.png)

| **Property** | **Description** |
| :--- | :--- |
| **Block Interactions With Screen Space UI** | Enable this to make the interactor ignore interactions when occluded by a screen space canvas. |
| **Block UI on Interactable Selection** | Enabling this option will block UI interaction when selecting interactables. |
| **UI Press Input** | The input used to press or select UI elements. Equivalent to a mouse button. |
| **UI Scroll Input** | The input to use to scroll UI elements. Equivalent to a mouse scroll wheel. |
