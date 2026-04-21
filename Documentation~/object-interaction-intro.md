---
uid: xri-3d-interaction-overview
---

# Introduction to 3D interaction

3D interaction lets users reach out and manipulate objects in a virtual scene. At its core, this involves two types of components working together: **interactors** attached to the user and **interactables** attached to the objects that the user can interact with.

## How interaction works

An interaction goes through several phases:

1. **Hover**: The user gets close to or points at an object. Both the interactor and interactable enter the hover state. You can use this state to show visual feedback, such as highlighting the object.
2. **Select**: The user performs an action, like pressing a button or squeezing a grip, to grab or select the object. The interactable enters the select state.
3. **Activate**: While holding a selected object, the user can trigger a secondary action. For example, the user might pull a trigger to fire a grabbed weapon or press a button to toggle a flashlight.

For a full description of all interaction states, refer to [Interaction state and events](xref:xri-state-events).

## Key components

To set up a basic 3D interaction, you need three things:

* **An [XR Interaction Manager](xref:xri-xr-interaction-manager)**: coordinates interaction between interactors and interactables. You need at least one for interaction to function. If you don't add one, the system creates one automatically by default.
* **An [Interactor](xref:xri-interactor-components)**: a component on a user-controlled object, such as a hand or controller, that detects which objects the user can interact with. Common interactors include:
  * [Near-Far Interactor](xref:xri-near-far-interactor): handles both close-range and distant interaction.
  * [XR Poke Interactor](xref:xri-xr-poke-interactor): handles poking, typically with a point in front of a handheld XR controller or a fingertip.
  * [XR Ray Interactor](xref:xri-xr-ray-interactor): interacts with objects the user points at from a distance.
* **An [Interactable](xref:xri-interactable-components)**: a component on a scene object that defines how it responds to interactions. For example, [XR Grab Interactable](xref:xri-xr-grab-interactable) lets the user pick up, move, and throw an object.

Only interactors and interactables registered with the same XR Interaction Manager can interact with each other.

## Visual feedback

You can give users visual hints about which objects they can interact with and what state those objects are in:

* [XR Tint Interactable Visual](xref:xri-xr-tint-interactable-visual) changes an object's color when the user hovers over or selects it.
* [XR Interactor Line Visual](xref:xri-xr-interactor-line-visual) draws a line from the interactor to the point where it hits an interactable, useful for ray-based interaction.

## Events

Both interactors and interactables dispatch events whenever the interaction state changes. You can use these events to trigger interaction feedback and gameplay logic, for example, playing a sound when the user picks up an object or updating a score when an object is placed in a target zone.

For more information about events and their variants, refer to [Interaction events](xref:xri-interaction-events-landing).

## Prefabs and Examples

The fastest way to set up 3D interaction is to use the prefabs included in the [Starter Assets](xref:xri-samples-starter-assets) sample. The XR Origin prefab in that sample comes with interactors already configured, so you only need to:

1. Add the XR Origin prefab to your scene.
2. Add an interactable component and a Collider to the objects you want the user to interact with.

For step-by-step instructions, refer to these tutorials:

* [Create a basic scene](xref:xri-create-basic-scene): set up a scene with the required XR components.
* [Create a basic interaction](xref:xri-create-basic-interaction): make a GameObject that the user can grab.

You can also explore the [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample for a more complete example of hand-based 3D interaction, or the [XRI Examples](xref:xri-examples) project for a variety of interaction scenarios.

## Additional resources

* [Interaction overview](xref:xri-architecture)
* [Samples](xref:xri-samples)
* [Tutorials](xref:xri-tutorials)
