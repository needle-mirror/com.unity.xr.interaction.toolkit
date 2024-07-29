---
uid: xri-glossary
---
# Glossary

| Term | Meaning |
|---|---|
| **Camera** | The GameObject that contains a Camera component. This is usually the main camera that renders what the user sees. It is the head of XR rigs. |
| **Camera Floor Offset GameObject** | The GameObject to move the Camera to the desired height off the floor depending on the tracking origin mode. |
| **Climb** | A type of locomotion that moves the user counter to Interactor movement while the user is selecting a Climb Interactable. |
| **Continuous Move** | A type of locomotion that smoothly moves the user by an amount over time. |
| **Continuous Turn** | A type of locomotion that smoothly rotates the user by an amount over time. |
| **Device mode** | A device-relative tracking origin mode. Input devices will be tracked relative to the first known location. The Camera is moved to the height set by the Camera Y Offset value by moving the Camera Floor Offset GameObject. |
| **Floor mode** | A floor-relative tracking origin mode. Input devices will be tracked relative to a location on the user's floor. The offset provided by the Camera Floor Offset GameObject will be zeroed out. |
| **Gesture** | Sequences of movements that translate into an action that manipulates an interactable. |
| **Grab Move** | A type of locomotion that moves the user counter to controller movement, as if the user is grabbing the world around them. |
| **Haptic** | Sensory or visual stimuli that is sent to the user to give feedback for interaction. |
| **Hover** | The state where an Interactor is in a valid state to interact with an object. |
| **Interaction Manager** | A manager component that handles interaction between a set of Interactors and Interactables. |
| **Input Reader** | A serialized property that abstracts a source of input and allows components to read input from an XR device in a generic way. It supports different modes to select the method used to read input, including from a component or ScriptableObject reference that generates logical values. These are used by interactors to read input, such as a button press, to allow interaction events like select. |
| **Interactable** | An object in a scene that the user can interact with (for example, grab it, press it, or throw it). |
| **Interactor** | An object in a scene that can select or move another object in that scene. |
| **Locomotion Mediator** | Provides `LocomotionProvider` components with access to the `XRBodyTransformer` linked to the `LocomotionMediator` and manages the `LocomotionState` for each provider based on its request for the `XRBodyTransformer`. |
| **Locomotion Provider** | The base class for various locomotion implementations, such as teleportation and turning. |
| **Origin** | By default, the Origin is the GameObject that the XR Origin component is attached to, and the term is generally used interchangeably with XR Origin. This is the GameObject that the application will manipulate via locomotion. |
| **Select** | The state where an Interactor is currently interacting with an object. |
| **Snap Turn** | A type of locomotion that rotates the user by a fixed angle. |
| **Teleportation** | A type of locomotion that teleports the user from one position to another position. |
| **XR Body Transformer** | Manages user locomotion via transformation of an `XROrigin` and applies queued transformations during `Update`. |
| **XR Origin** | The component that implements the generic concept of a camera rig. It also provides options of tracking origin modes to configure the reference frame for positions reported by the XR device. It has properties to specify an Origin GameObject, a Camera Floor Offset GameObject, and a Camera. |
