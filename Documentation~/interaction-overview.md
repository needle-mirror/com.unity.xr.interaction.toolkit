---
uid: xri-overview
---

# XR Interaction Toolkit overview

Interactions in a scene are based on two key elements:

* **Interactors**: elements in the scene directly controlled by the user, typically through XR controller hardware, hand tracking, or touch screens on mobile AR devices. Interactors initiate interactions with interactable objects.
* **Interactables**: the objects in a scene that the user can interact with. Interactable objects have states that indicate how they are being interacted with.

Every frame, the active interactors identify which interactable objects they can interact with. These interactable objects enter the hover state and dispatch hover entered events, which you can use to provide feedback to the user that interaction is possible. The user can then initiate selection of an interactable, the next phase of an interaction. For example, the user might press a specific button on a controller to select a "hovered" object. When selected an interactable object enters the selected state and dispatches select entered events. What happens next, depends on the type of interactor and interactable and how you have configured the interaction. A grabbable object might be picked up by the user. A UI ScrollView might pan with movement of the user's finger.

The XR Interaction Toolkit provides three broad categories of interaction:

* **3D object interaction**: the ability to pick up, move, trigger some behavior, or otherwise interact with GameObjects in the scene.
* **UI interaction**: the ability to interact with UI controls on a Canvas.
* **Locomotion**: the ability of the user to move around in the scene. Some modes of locomotion, like teleportation and climbing, use interactors and interactables. Other modes of locomotion, like continuous movement and grab locomotion, rely directly on user input, such as controller joysticks, and don't need interactors or interactables.

For information about setting up your project and scenes to use the XR Interaction Toolkit, as well as more information about the topics discussed in brief here, refer to:

| **Topic**             | **Description**         |
| :-------------------- | :----------------------- |
| [Get started](xref:xri-get-started)       | How to install the toolkit and set up your project and individual scenes. |
| [Input](xref:xri-input)                   | How to set up, configure, and customize input. |
| [3D Interaction](xref:xri-3d-interaction) | How to set up interaction with GameObjects. |
| [Locomotion](xref:xri-locomotion)         | How to set up the different modes of locomotion. |
| [UI Interaction](xref:xri-ui-interaction) | How to set up interaction with UI components. |

For examples and tutorials, refer to the [Samples](xref:xri-samples) and [Tutorials](xref:xri-tutorials) sections.
