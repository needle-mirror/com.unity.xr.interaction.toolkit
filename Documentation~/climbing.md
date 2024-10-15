---
uid: xri-climbing
---

# Climbing

Climbing is facilitated by the Climb Provider and Climb Interactable components.

## Climb Locomotion

The package provides the ability to do climb locomotion, which allows the user to climb an Interactable such as a ladder. Climb locomotion translates the XR Origin counter to movement of whichever Interactor is selecting a Climb Interactable. If multiple Interactors are selecting a Climb Interactable, only the most recent selection will drive movement. This type of locomotion is similar to [grab movement](grab-movement.md) but uses Interactables to restrict locomotion.

Climb locomotion settings can be configured at the Provider level or overridden by the specific Climb Interactable being climbed. Settings can be configured to restrict movement along any of the local axes of the Interactable.

## Climb Provider

The package provides a [Climb Provider](climb-provider.md) component. This is the component that handles the actual movement of the XR Origin.

## Climb Interactable

The package provides a [Climb Interactable](climb-interactable.md) component that implements the `XRBaseInteractable` abstract class. This component allows you to make an object climbable by using the selection interaction to drive movement. You can optionally restrict movement so that the user is unable to move along certain local axes of the object.
