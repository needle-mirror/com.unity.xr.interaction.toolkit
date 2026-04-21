---
uid: xri-key-components
---

# Key interaction components

The XR Interaction Toolkit provides a set of [components](xref:xri-components) that you can place on GameObjects in a scene and which serve as the basis for implementing interactions.

> [!TIP]
> The [Starter Assets](xref:xri-samples-starter-assets) provided by the toolkit include an XR Origin prefab that is already set up with a complete set of interactors. You can use this prefab as a starting point for your projects. In addition, many of the XR templates available when you create a new project in the Unity Hub provide a similar XR Origin prefab, often customized to better serve the focus of a particular template.

## Interactors

Interactor components handle the actions of hovering and selecting interactable objects in the world. This component is responsible for creating a list of interactables that it could potentially hover or select each frame. The interactor sorts this target list by priority, and, by default, the closest interactables have highest priority. You can change the priority criteria with a [Target filter](target-filters.md).

Refer to [Interactor components](xref:xri-interactor-components) for a list of the available interactors.

## Interactables

Interactables are objects in a scene that an interactor can hover, select, focus, and/or activate. This component is responsible for defining the behavior of those interaction states. The same interactor might be able to pick up and throw a ball, shoot a gun, or press a 3D button on a keypad.

Refer to [Interactable components](xref:xri-interactable-components) for a list of the available interactables.

## Interaction Manager

The Interaction Manager acts as an intermediary between interactors and interactables. This component is responsible for actually causing the interaction state changes among its registered interactors and interactables. Only objects registered with the same Interaction Manager can interact with each other. For more detail about the exact mechanism the Interaction Manager uses to trigger the state changes, refer to [Update loop](xref:xri-update-loop).

An interactor or interactable component registers with an Interaction Manager during its [OnEnable](xref:MonoBehaviour.OnEnable) method. If you haven't assigned a manager to an object, it uses the first Interaction Manager it finds in the scene. In [OnDisable](xref:MonoBehaviour.OnDisable), an interactor or interactable unregisters from the Interaction Manager.

If a scene has a single Interaction Manager, all interactors can interact with all interactables. Alternately, you can have multiple Interaction Managers, each with their own set of interactors and interactables, and turn them on and off to enable or disable sets of interaction. You can assign a particular Interaction Manager to interactor and interactable components rather than relying on finding the default, when required.

The XR Interaction Manager component has a few properties that you can set in the Inspector for filtering hover and select interactions. Configure the starting set of filters that a manager uses to determine which interactables are eligible for interaction. All of these filter properties are optional. If you do not assign them, the manager uses default behavior. Refer to [Interaction filters](xref:xri-interaction-filters) for more information about implementing filters.

> [!TIP]
> Every scene needs at least one Interaction Manager component. By default, interactor and interactable components find the scene's Interaction Manager and assign it as their manager. If you add more than one manager to a scene, you should explicitly assign the correct one to every interactor and interactable to make sure the correct objects can interact with each other. If an Interaction Manager doesn't exist in a scene, the system creates one.

Refer to [Scene management considerations](xref:xri-scene-management) for more information about how to manage your scenes so that your interaction objects can find their correct manager.

## Interaction Groups {interaction-groups}

Interaction groups are mediators for interactors. A group contains multiple member interactors, sorted by priority, and only allows one interactor in the Group to interact (hover or select) at a time. A group also maintains the focus state for its member interactors, storing the last interactable a member of the group interacted with until focus is cleared.

Groups first prioritize continuous selection - so if a member interactor was interacting the previous frame and can start or continue selection in the current frame, then that interactor will be chosen for interaction even if a higher priority interactor tries to interact.

An interaction group can also contain other groups in its sorted list of members. A group within a group is treated like a single interactor when it comes to how the containing group prioritizes interactions. The interactor chosen for interaction within a group is bubbled up to the next containing group, until it is ultimately either chosen for interaction in the top-level group or skipped over just like other interactors.

Interaction groups also register with an Interaction Manager. A member interactor or group must be registered with the same Interaction Manager as its containing group for the member to function as part of the group. A group first registers with an Interaction Manager in [Awake](xref:MonoBehaviour.Awake). If you disable a group the group unregisters from the Interaction Manager (in [OnDisable](xref:MonoBehaviour.OnDisable)). If you enable the group again it again registers with an Interaction Manager (in [OnEnable](xref:MonoBehaviour.OnEnable)). If you add an interactor or group to a group at runtime, the new group member unregisters and then re-register with its Interaction Manager so that it can be treated as a group member in the update loop.
