---
uid: xri-architecture
---

# Interaction overview

This section describes the relationship between the core components of the interaction system and the states that make up the lifecycle of an interaction.

<a id="states"></a>
## States

The Interaction system has four common states: Hover, Select, Focus, and Activate. These states can mean different things to different objects. Hover, Select, and Focus are loosely related to the traditional GUI concepts of mouse-over, mouse-down, and UI focus. Activate is specific to XR and is entered and exited via a contextual command.

These interaction states always involve both an [Interactor](#interactors) and [Interactable](#interactables), and both are notified upon entering or exiting the state.

|State|Function|
|---|---|
|**Hover**|If an Interactable is a valid target for the Interactor its state changes to Hover. Hovering on an object signifies an intention to interact with it, but doesn't typically change the behavior of that object, though it might create a visual indicator for this change of state, like how a hovered button changes tint.|
|**Select**|Selection requires an action such as a button or trigger press from the user to enable the Select state. When an Interactable is in the Select state, Unity considers the selecting Interactor to be interacting with it. For example, Selection can simulate picking up a grabbable object, holding a lever, or preparing to push a door that has focus via hovering.|
|**Focus**|An Interactable is focused when it is selected by an Interactor. This focus persists until another Interactable is selected or the Interactor explicitly attempts to select nothing. This state is useful for performing actions on an object. For example - gaining focus of an object and then manipulating its color in a menu.|
|**Activate**|Activation is an extra action, typically mapped to a button or trigger that affects the currently selected object. This lets the user further interact with an object they've selected. The Activate action depends on the Interactable. For example, you can use Activate to toggle a grabbable flashlight on/off or shoot a ball launcher. You can hook the component to process Activate into an action without any additional code by hooking an existing callback using the Inspector window under **Interactable Events** and then add to **Activated** via UnityEvents.|

## Components

### Interactors
Interactor components handle the actions of hovering and selecting Interactable objects in the world. This component is responsible for creating a list of Interactables (called Valid Target) that it could potentially hover or select each frame. The Valid Target list is sorted by priority, and by default the closest Interactables have highest priority. This priority criteria can be changed or extended using [Target filters](target-filters.md).

### Interactables
Interactables are objects in a scene that an Interactor can hover, select, focus, and/or activate. This component is responsible for defining the behavior of those interaction states. The same Interactor might be able to pick up and throw a ball, shoot a gun, or press a 3D button on a keypad.

### Interaction Manager
The Interaction Manager acts as an intermediary between Interactors and Interactables. This component is responsible for actually causing the interaction state changes among its group of registered Interactors and Interactables.

The typical setup is to have a single Interaction Manager, where all Interactables can potentially be affected by all Interactors. You can have multiple complementary Interaction Managers, each with their own set of Interactors and Interactables, and turn them on and off to enable or disable sets of interaction. The collection of loaded scenes needs to have at least one Interaction Manager for interactions to work.

Upon being enabled (during the behavior's `OnEnable`), both Interactors and Interactables register with an Interaction Manager automatically. They will use the first found Interaction Manager if you don't specify one in the Inspector window. Upon being disabled (during the behavior's `OnDisable`), both Interactors and Interactables unregister from the Interaction Manager they are registered with.

For more detail about the exact mechanism the Interaction Manager uses to trigger the state changes, see [Update loop](#update-loop).

<a id="interaction-groups"></a>
### Interaction Groups
Interaction Groups are mediators for Interactors. A Group contains multiple member Interactors, sorted by priority, and only allows one Interactor in the Group to interact (hover or select) at a time. Groups first prioritize continuous selection - so if a member Interactor was interacting the previous frame and can start or continue selection in the current frame, then that Interactor will be chosen for interaction even if a higher priority Interactor tries to interact.

An Interaction Group can also contain other Groups in its sorted list of members. A Group within a Group is treated like a single Interactor when it comes to how the containing Group prioritizes interactions. The Interactor chosen for interaction within a Group is bubbled up to the next containing Group, until it is ultimately either chosen for interaction in the top-level Group or skipped over just like other Interactors.

Interaction Groups also register with an Interaction Manager. A member Interactor or Group must be registered with the same Interaction Manager as its containing Group for the member to function as part of the Group. A Group first registers with an Interaction Manager during the behavior's `Awake`. Upon being disabled (during the behavior's `OnDisable`) the Group will unregister from the Interaction Manager. If the Group is then re-enabled (during the behavior's `OnEnable`) it again registers with an Interaction Manager. If an Interactor or Group is added to a Group at runtime, it will unregister from and then re-registers with its Interaction Manager so that it can be treated as a Group member in the update loop.

### Input Readers
The input properties on interactor components and locomotion provider components provides a way to abstract the source of input data, which Interactors then use to translate into interaction states, notably for selection and activation. The input actions are bound to a tracked device's controls and are read from each frame they are needed. Interactor like the XR Ray Interactor that depend on controller or hand input then query to determine if they should select or activate.

You may want different inputs to be used for a ray-based teleportation interactor and a ray-based interactor for grabbing, and both Interactor components can reference different input actions to read different input values from the same tracked controller device.

### Tracked Pose Driver
This component in the Input System package is responsible for reading the position and rotation of the tracked device, and applying it to the Transform component.

## Update loop

The update loop of the Interaction Manager queries Interactors and Interactables, and handles the hover, focus and selection states. First, it asks Interactors for a valid list of targets (used for both hover and selection). It then checks both Interactors and Interactables to see if their existing hover, focus and selection objects are still valid. After invalid previous states have been cleared (exited via `OnSelectExiting` and `OnSelectExited`/`OnHoverExiting` and `OnHoverExited`/`OnFocusExiting` and `OnFocusExited`), it queries both objects for valid selection, focus and hover states, and the objects enter a new state via `OnSelectEntering` and `OnSelectEntered`/`OnHoverEntering` and `OnHoverEntered`/`OnFocusEntering` and `OnFocusEntered`.

![interaction-update](images/interaction-update.svg)

All registered Interactables and Interactors are updated before and after interaction state changes by the Interaction Manager explicitly using `PreprocessInteractor`, `ProcessInteractor`, and `ProcessInteractable`. Interactors are always notified before Interactables for both processing and state changes, and Interactors contained within Interaction Groups are always notified before Interactors that are not contained within Groups. Interactables and Interactors are not limited from using the normal `MonoBehaviour` `Update` call, but per-frame logic should typically be done in one of the process methods instead so that Interactors are able to update before Interactables. Using the `ProcessInteractable` method also ensures that Interactables process first if they are a virtual parent of another Interactable. The XR Interaction Manager will maintain and dynamically sort the list of Interactables based on registered dependencies when using virtual parenting.

### Processing interactables

Interactables register with the XR Interaction Manager during the component's own `OnEnable` and unregister with the component's `OnDisable` method. The XR Interaction Manager will process all registered interactables in the order they were most recently registered. Due to the nature of how Unity executes different script components and interactables sharing the same script execution order, the order cannot be relied upon if you have a dependency. See [Script execution order](https://docs.unity3d.com/Manual/script-execution-order.html) in the Unity manual for more information.

If an interactable component needs to be processed after another interactable component is processed, this dependency relationship can be registered with the XR Interaction Manager through scripting using the `RegisterParentRelationship`/`UnregisterParentRelationship` methods on the `XRInteractionManager`. Alternatively, you can assign the Parent Interactable property in the Inspector window of an interactable component to register the dependency relationship when the component is registered with the XR Interaction Manager.

A parent interactable will be processed, meaning `ProcessInteractable` will be called by the XR Interaction Manager, before its child interactables are processed. This functions recursively, meaning if the parent interactable itself has its own parent interactable, that dependency will also be respected.

Interactors can also register a parent interactable to indirectly change the processing order of interactables that they select. Similarly to interactables, you can assign the Parent Interactable property in the Inspector window of an interactor component to register the dependency relationship that selected interactables will inherit from the interactor. It can also be registered through scripting using the `RegisterParentRelationship`/`UnregisterParentRelationship`.

> [!NOTE]
> The Parent Interactable property will only be read at the time when the component is registered with the manager. The `RegisterParentRelationship` method can be called at any time to set parent dependencies, including when selected.

This virtual parenting system can be useful in the case where you have a [XR Socket Interactor](xref:xri-xr-socket-interactor) component which has a parent GameObject with an [XR Grab Interactable](xref:xri-xr-grab-interactable) component. Setting the Parent Interactable property on the socket to the Grab Interactable will ensure that other interactables placed into the socket will process after the socket's parent interactable. This ordering allows the target pose of the grab interactable placed in the socket to be updated correctly without a frame delay that would occur if the processing order was done in reverse if left to chance.

### Interaction strength

Interactors and Interactables can report a variable (that is, analog) selection interaction strength, which is a normalized `[0.0, 1.0]` amount of selection that the interactor is performing. For interactors that use motion controller input, this is typically the analog trigger or grip press amount. For interactables that can be poked, it can be based on the depth of a poke interactor.

Interaction strength values are updated after all interaction state changes have occurred by the Interaction Manager explicitly using `ProcessInteractionStrength`. In this case, Interactables are notified before Interactors to allow a poke depth to be computed before gathering the overall interaction strength in the interactor.
