---
uid: xri-teleportation
---

# Teleportation

The package provides a simple implementation of teleportation that also demonstrates how to implement complex locomotion scenarios using the `LocomotionProvider`.

The Teleportation Provider inherits from the `LocomotionProvider` abstract class. The Teleportation Provider is responsible for moving the XR Origin to the desired location on the user's request.

This implementation has several types of teleportation destinations: a Teleportation Area, a Teleportation Anchor, and a Teleportation Multi-Anchor Volume. These are discussed in more detail below. In short:

- Teleportation Areas allow the user to choose a location on a surface that they wish to teleport to.

- Teleportation Anchors teleport the user to a pre-determined specific position and/or rotation that they specify. Technically, it functions like the Teleportation Area but has the additional anchor functionality.

- Teleportation Multi-Anchor Volumes allow the user to target a volume of space to teleport to one of several anchor locations, based on custom filtering logic.

All types of teleportation destinations are implemented on top of the XR Interaction system using the `BaseTeleportationInteractable` as the starting point for shared code.

The XR Interaction system also provides various line rendering options. For more information, see documentation for the [XR Interactor Line Visual](xr-interactor-line-visual.md) and the [XR Interactor Reticle Visual](xr-interactor-reticle-visual.md).

## Teleportation Provider

The [Teleportation Provider](teleportation-provider.md) component implements the `LocomotionProvider` abstract class. You can have as many instances of the Teleportation Provider component in your scene as you need. However, in most cases, a single instance is enough.

The **Mediator** field should reference the Locomotion Mediator MonoBehaviour that you want the teleportation provider to interact with. If you don't specify a Locomotion Mediator, the provider attempts to find one in the current scene.

## Teleportation Area Interactable

The [Teleportation Area](teleportation-area.md) component is a specialization of the `BaseTeleportationInteractable` class. It allows the user to select any location on the surface as their destination.

The Teleportation Area Interactable is intended to be used by the XR Ray Interactor or any of its specializations. It uses the intersection point of the ray and the area's collision volume to determine the location that the user wants to teleport to. It can also optionally match the user's rotation to the forward direction of the attach transform of the selecting Interactor. The Teleportation Area Interactable has a specialized implementation of the `GenerateTeleportRequest` method, which generates a teleportation request that is queued with the Teleportation Provider.

**Match Orientation** is used to specify how the rotation of the XR Origin changes when teleporting.
- If your application does not rotate the XR Origin in any way, and you always want the XR Origin's up vector to match World Space's Up vector, use the **World Space Up** option.
- If you want the user to be able to stand on a ceiling, wall, or other tilted surface, and have them rotate to match so that the ceiling or wall feels like their new floor, select **Target Up** instead. The XR Origin will match the up vector of the Transform that the Teleportation Area component is attached to.
- If you want to point the user in a very specific direction when they arrive at a target, select **Target Up And Forward**. This will match the XR Origin's rotation to the exact rotation of the Transform that a Teleportation Area is attached to.
- If you do not want a teleport to change the rotation in any way, and you want the user to retain the same rotation before and after a teleport, select **None**.  If your entire application is oriented at a 45 degree angle, for instance, you can rotate the XR Origin's root Transform and set all teleport targets to `MatchOrientation.None`.

## Teleportation Anchor Interactable

The [Teleportation Anchor](teleportation-anchor.md) component is a specialization of the `BaseTeleportationInteractable` class that allows the user to teleport to an anchor location by selecting the anchor or an area around it.

The Teleportation Anchor Interactable is intended to be used by the XR Ray Interactor or any of its specializations. It uses the intersection point of the ray and the area's collision volume to determine the location that the user wants to teleport to. It can also optionally match the user's rotation to the forward direction of the attach transform of the selecting Interactor. The Teleportation Anchor Interactable has a specialized implementation of the `GenerateTeleportRequest` method, which generates a teleportation request that is queued with the Teleportation Provider.

The **Teleport Anchor Transform** field defines the transform that the XR Origin teleports to when the user teleports to this anchor. It uses both the position and the rotation of the anchor, depending on which **Match Orientation** is selected.

## Teleportation Multi-Anchor Volume Interactable

The [Teleportation Multi-Anchor Volume](teleportation-multi-anchor-volume.md) component is a specialization of the `BaseTeleportationInteractable` class that allows the user to teleport to one of several anchor locations by selecting the entire volume.

The volume uses custom filtering logic to choose the best destination anchor. By default it uses whichever anchor is furthest from the user, which is useful for easily getting to either end of a climbable object like a ladder. To change the filtering logic, you can assign any instance of an implementation of the `ITeleportationVolumeAnchorFilter` interface to the volume.

Note that Teleportation Multi-Anchor Volume uses pure transforms to define its anchors. It does not use Teleportation Anchor interactables at all.
