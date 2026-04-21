---
uid: xri-architecture
---

# Interaction overview

Interactions in a scene are based on two key elements:

* **Interactors**: elements in the scene directly controlled by the user, typically through XR controller hardware, hand tracking, or touch screens on mobile AR devices. Interactors initiate interactions with interactable objects.

   The toolkit provides a variety of interactors, such as the [Near-Far Interactor](xref:xri-near-far-interactor) that lets users interact with both nearby and distant objects.

* **Interactables**: the objects in a scene that the user can interact with.

   The toolkit provides interactable components for general object manipulation, such as the [XR Grab Interactable](xref:xri-xr-grab-interactable), as well as specialized interactables for locomotion and UI.

Every frame, the active interactors identify which interactable objects they can interact with. For example, the user might place their hand near or point at an object. The user can then initiate selection of an interactable, the next phase of an interaction. For example, the user might press a specific button on a controller to select a hovered object. What happens next depends on the type of interactor and interactable and how you have configured the interaction. A grabbable object might be picked up by the user. A UI ScrollView might pan with movement of the user's finger.

Whenever the interaction state changes, both the interactor and the interactable objects dispatch events so that your code can react to the state change.

The following topics the technical details of how interaction work in the XR Interaction Toolkit:

| Topic | Description |
| :---- | :---------- |
| [Key interaction components](xref:xri-key-components) | Learn about the basic components involved in interactions.|
| [Interaction state and events](xref:xri-state-events) | Learn about interaction states and the events dispatched to signal state changes.|
| [Scene management considerations](xref:xri-scene-management) | Learn about managing interaction components across scene changes. |
| [Interaction update loop](xref:xri-update-loop) | Learn about how the interaction manager orchestrates the behavior of interactors and interactables. |
