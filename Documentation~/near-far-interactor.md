# Near-Far Interactor

Interactor that utilizes both near and far interaction casters, allowing seamless transitions between different
interaction types, while also using the far interaction caster's data to interact with UGUI.

This interactor is designed with modularity and extensibility in mind, allowing advanced developers to implement custom near and far casters that adhere to the `IInteractionCaster` and `ICurveInteractionCaster` interfaces, respectively.

The `IInteractionAttachController` interface allows developers to implement custom attach controllers that can be used to control the attach transform for translation and rotation.
The default implementation for this enables distance-based velocity scaling on motion to pull objects to the user from afar, with hand tracking and controller tracking support.

To complement this interactor, a visual controller component called `CurveVisualController` leverages the `ICurveInteractionDataProvider` interface to provide all the data required of the curve in a streamlined interface to allow for a flexible visual representation of the curve.

This interactor is designed to integrate the functionality of both `XRDirectInteractor` and `XRRayInteractor`, though there are two notable gaps to keep in mind.
- For mobile AR devices, the ray interactor is still required for raycasting AR data, as this `NearFarInteractor` does not yet support this use case. 
- The `NearFarInteractor` does not yet support using the joystick to control the attach transform for translation, rotation and scaling, as the `XRRayInteractor` does.



## Properties

| **Property**                              | **Description**                                                                                                                                                               |
|-------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Interaction Manager**                   | The XR Interaction Manager that this Interactor will communicate with. Will find one if None is set.                                                                          |
| **Interaction Layer Mask**                | Allows interaction with Interactables whose Interaction Layer Mask overlaps with any Layer in this Interaction Layer Mask.                                                    |
| **Select Action Trigger**                 | Choose how the select state is triggered from input, options include holding the button, pressing while hovered, toggling select on/off, or sticky toggle.                    |
| **Keep Selected Target Valid**            | Keeps selecting the target when not touching or pointing to it after initially selecting it. Recommended to be true for grabbing objects, false for teleportation.            |
| **Allow Hovered Activate**                | Send activate and deactivate events to interactables hovered over but not selected when there is no current selection.                                                        |
| **Interaction Attach Controller**         | Reference to the attach controller used to control the attach transform.                                                                                                      |
| **Enable Near Casting**                   | Determines if the near interaction caster is activated to find valid nearby targets. Default is `true`.                                                                       |
| **Near Caster**                           | Reference to the near interaction caster component used for near interactions. Must implement `IInteractionCaster`.                                                           |
| **Sorting Strategy**                      | Strategy for sorting targets detected by the near interaction caster. Options are `None`, `SquareDistance`, `InteractableBased`, and `ClosestPointOnCollider`.                |
| **Sort Near Targets After Target Filter** | Determines if the near targets should be sorted after the target filter is applied. Should be true only if filter does not sort targets. Not used if no target filter is set. |
| **Enable Far Casting**                    | Determines if the far interaction caster is activated to find valid distant targets. Default is `true`.                                                                       |
| **Far Caster**                            | Reference to the far interaction caster component used for far interactions. Must implement `ICurveInteractionCaster`.                                                        |
| **Far Attach Mode**                       | Determines how the attachment point is adjusted on far select. This typically results in whether the interactable stays distant at the far hit point or moves to the near hand. |
| **UI Interaction**                        | Allows the Interactor to interact with UI elements. Default is `true`.                                                                                                        |
| **Block UI On Interactable Selection**    | Blocks UI interaction when an interactable is selected. Default is `true`.                                                                                                    |
| **UI Press Input**                        | Defines the input used for pressing UI elements, functioning like a mouse button when pointing over UI. Implemented as an `XRInputButtonReader`.                              |
| **UI Scroll Input**                       | Defines the input used for scrolling UI elements, functioning like a mouse scroll wheel when pointing over UI. Implemented as an `XRInputValueReader<Vector2>`.               |
| **Select Input**                          | Input to use for selecting an interactable.                                                                                                                                   |
| **Activate Input**                        | Input to use for activating an interactable. This can trigger a secondary action on an interactable object.                                                                   |
| **Interactor Filters**                    | Add filters to extend this Interactor without needing to create a derived behavior.                                                                                           |
| **Starting Target Filter**                | The target filter that this Interactor will automatically link at startup (optional).                                                                                         |
| **Starting Hover Filters**                | The hover filters that this Interactor will automatically link at startup (optional). Used as additional hover validations.                                                   |
| **Starting Select Filters**               | The select filters that this Interactor will automatically link at startup (optional). Used as additional select validations.                                                 |
| **UI Hover Entered Event**                | Event triggered when a UI element is hovered over by this interactor.                                                                                                         |
| **UI Hover Exited Event**                 | Event triggered when a UI element is no longer hovered over by this interactor.                                                                                               |

## Enums

### Region

Used by the `selectionRegion` property to indicate whether the selection is currently occurring in the near-field or far-field region.

- **None**: No selection is occurring.
- **Near**: Selection is occurring with near-field interaction.
- **Far**: Selection is occurring with far-field interaction.

### NearCasterSortingStrategy

Determines the strategy for sorting valid targets discovered by the near interaction caster.

- **None**: No sorting is performed.
- **SquareDistance**: Sorting based on the square distance between the interactor and interactable attach transform world positions. Most efficient.
- **InteractableBased**: Sorting based on the interactable's defined sorting strategy, usually collider based.
- **ClosestPointOnCollider**: Sorting based on the square distance between the interactor's attach transform world position and the closest point on the interactable's collider. Used for high fidelity disambiguation.

### InteractorFarAttachMode

Controls the interactor's default behavior for how to adjust its attach transform on far select.

- **Near**: The interactor should reset its attach transform to the near point on far select. This will typically result in the interactable object moving to the hand.
- **Far**: The interactor should always move its attach transform to the far hit point on far select. This will typically result in the interactable object staying distant at the far hit point.

This behavior may be overridden by interactables, such as XR Grab Interactable, which implement the `IFarAttachProvider` interface. Those interactables default to using the interactor's preferred value, but the interactables can choose to force either Near or Far when selected as a result of being hit by the far interaction caster.

## Usage

This component should be attached to a GameObject that acts as an interactor in an interaction system. It requires components implementing `IInteractionAttachController`, `IInteractionCaster` for near casting, and `ICurveInteractionCaster` for far casting for full functionality. Default components implementing those interfaces will automatically be added at runtime if the Interaction Attach Controller, Near Caster, or Far Caster references are missing.
