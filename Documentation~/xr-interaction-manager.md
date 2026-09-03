---
uid: xri-xr-interaction-manager
---
# XR Interaction Manager

The Interaction Manager acts as an intermediary between Interactors and Interactables. It is possible to have multiple Interaction Managers, each with their own valid set of Interactors and Interactables. Upon being enabled, both Interactors and Interactables register themselves with a valid Interaction Manager (if a specific one has not already been assigned in the inspector). The loaded scenes must have at least one Interaction Manager for Interactors and Interactables to be able to communicate.

Many of the methods on the Interactors and Interactables are designed to be called by this Interaction Manager rather than being called directly in order to maintain consistency between both targets of an interaction event.

## Collider-to-interactable mapping

The Interaction Manager maintains an internal collider-to-interactable mapping that allows interactors to determine which interactable a collider belongs to. This mapping is built when an interactable registers with the manager during `OnEnable` and is used by interactors during hover, select, and poke evaluation.

By default, an interactable discovers its colliders during `Awake`. Colliders added via code after registration must be explicitly registered with the interactable for the mapping to be updated. Refer to [Managing colliders at runtime](xref:xri-interactable-components#managing-colliders-at-runtime) for details.

![XRInteractionManager component](images/xr-interaction-manager.png)
