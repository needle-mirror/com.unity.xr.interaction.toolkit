### Common Interactor properties

The following properties are common to most Interactor components:

| **Property**                              | **Description**       |
| :---------------------------------------- | :-------------------- |
| **Interaction Manager**                   | The XR Interaction Manager that the interactor communicates with. The component tries to find one in the scene if you don't assign one. When your scene contains more than one manager, you should assign the correct manager for each interactor. |
| **Interaction Layer Mask**                | The interactor can only interact with interactables in one of the layers specified here. |
| **Handedness** | Associates this interactor with a specific hand. Use **None** for interactors not associated with a hand, such as gaze or socket interactors. |

> [!NOTE]
> The **Handedness** property is used by other toolkit code that need to identify which hand or controller an interactor is associated with. This property does not affect the behavior of the interactor itself. The functions that rely on this property include:
> * [XRInteractionManager.IsHandSelecting](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.IsHandSelecting(UnityEngine.XR.Interaction.Toolkit.Interactors.InteractorHandedness))
> * [XRHoverInteractableExtensions.ISHoveredByLeft](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRHoverInteractableExtensions.IsHoveredByLeft(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable)) and [IsHoveredByRight](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRHoverInteractableExtensions.IsHoveredByRight(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable))
> * [XRSelectInteractableExtensions.IsSelectedByLeft](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRSelectInteractableExtensions.IsSelectedByLeft(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)) and [IsSelectedByRight](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRSelectInteractableExtensions.IsSelectedByRight(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable))

### Attachments

For interactions that involve moving an object, interactor components provide attachment properties to control how the user moves the object.

#### Attach Transform

Most interactors have an **Attach Transform** property that lets you designate a temporary parent transform for a selected object.

If you do not assign a value, an appropriate GameObject is automatically instantiated and set in `Awake`.

Interactors that operate at a distance, like Ray and Gaze interactors, move the attachment GameObject to the raycast contact point when the user selects an interactable object.

Setting this property at runtime does not automatically destroy the previous object.

> [!NOTE]
> The [Near-Far Interactor](xref:xri-near-far-interactor) component does not have its own **Attach Transform** property. Instead it supports the [Interaction Attach Controller](xref:xri-interaction-attach-controller) component, which defines attachment behavior for the Near-Far interactor.

#### Additional information

* [XR Interaction Manager](xref:xri-xr-interaction-manager)
* [Interaction layers](xref:xri-interaction-layers)
* [XRBaseInteractor](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor)
