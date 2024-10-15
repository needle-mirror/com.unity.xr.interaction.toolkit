---
uid: xri-grab-movement
---

# Grab movement

A grab movement translates the XR Origin counter to controller movement while a button input is held. This allows the user to move as if grabbing the whole world around them.

## Grab Move Providers
The package provides a [Grab Move Provider](grab-move-provider.md) and a [Two-Handed Grab Move Provider](two-handed-grab-move-provider.md).

If a [Character Controller](xref:class-CharacterController) is present on the XR Origin, the Grab Move Provider or Two-Handed Grab Move Provider will move the XR Origin using [`CharacterController.Move`](xref:UnityEngine.CharacterController.Move(UnityEngine.Vector3)) rather than directly translating the Transform of the XR Origin.
