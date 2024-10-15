---
uid: xri-interactor-components
---

# Interactor components

The interactor components provided by the toolkit.


| **Topic**             | **Description**         |
| :-------------------- | :----------------------- |
| [Near-Far Interactor](near-far-interactor.md)                          | Interacts with both close and distant interactables when the user either touches or points at them. |
| [XR Direct Interactor](xr-direct-interactor.md)                        | Interacts with nearby interactables when the user touches them. |
| [XR Poke Interactor](xr-poke-interactor.md)                            | Interacts with nearby interactables when the user performs a specific poking motion.|
| [XR Ray Interactor](xr-ray-interactor.md)                              | Interacts with interactables at a distance when the user points at them. |
| [XR Gaze Interactor](xr-gaze-interactor.md) | Interacts with gaze-enabled interactables via eye tracking. (Can fallback to use head tracking for devices that don't support eye tracking.)|
| [XR Socket Interactor](xr-socket-interactor.md)                        | A specialized interactor that only interacts with interactables close to it. |
| [AR Gesture Interactor](ar-gesture-interactor.md)                      | (Deprecated) Interacts with `ARGestureInteractable` objects through mobile device touchscreen gesture input.|

Helper components that modify interactor visuals or behavior.

| **Topic**             | **Description**         |
| :-------------------- | :----------------------- |
| [Interaction Attach Controller](interaction-attach-controller.md)      | Controls the motion of an object that the user picks up. |
| [Curve Visual Controller](curve-visual-controller.md)                  | Controls the line drawn from a compatible interactor. |
| [XR Hand Skeleton Poke Displacer](xr-hand-skeleton-poke-displacer.md)  | Helps prevent a hand mesh from penetrating buttons or other UI elements during a poke interaction.|
| [XR Interactor Line Visual](xr-interactor-line-visual.md)              | Controls the line drawn from a compatible interactor. |
| [XR Interactor Reticle Visual](xr-interactor-reticle-visual.md)        | Draws a reticle Prefab where the user is pointing. |
| [XR Gaze Assistance](xr-gaze-assistance.md) | Enables ray interactors to use gaze-based interaction when the ray is pointing offscreen. Also provides aim assistance for thrown objects or other projectiles. |

## Additional resources

* [Climb Teleport Interactor](climb-teleport-interactor.md)
* [Component index](components.md)
