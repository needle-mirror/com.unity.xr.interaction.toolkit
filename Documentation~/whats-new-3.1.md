---
uid: xri-whats-new-3-1
---
# What's new in version 3.1

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.1:

## Added

### Near-Far Interactor controller manipulation

The [Near-Far Interactor](near-far-interactor.md) added in 3.0 combined the interactivity of the XR Direct Interactor and the XR Ray Interactor, however it did not have all the functionality previously available with the XR Ray Interactor for manipulating objects at a distance. With version 3.1, the [Interaction Attach Controller](interaction-attach-controller.md) component that sits alongside the Near-Far Interactor allows you to configure anchor movement for both physical push and pull gestures (suitable for a hand tracking experience) in addition to a `Vector2` input control (such as a thumbstick) for fine precision when **Use Manipulation Input** is enabled. It allows you to configure for horizontal rotation, vertical rotation, and translation.

### Gravity and Jump

Two new ways to move and manipulate the XR Origin have been added. A Jump Provider enables an easy way to add basic jumping when pressing a button. It is configurable for different use cases, such as varying heights based on duration held or whether to allow jumping while in the air. A new Gravity Provider component consolidates gravity logic, which was previously handled separately by each individual locomotion provider. It also exposes events for when the player rig becomes grounded. A new `IGravityController` interface can be implemented by locomotion providers to influence the Gravity Provider.

The `ClimbProvider` and `ContinuousMoveProvider` locomotion provider classes have been updated to take full advantage of the new gravity locomotion architecture, allowing gravity to be configured in a common way across different ways of locomotion. It also allows for better handling and coordination with other locomotion providers when pausing gravity, such as preventing the move provider while climbing up a wall. In addition, the [Continuous Move Provider](continuous-movement.md#continuous-move-provider) has a new **In Air Control Modifier** property which determines how much control the player has while in the air. Read more about these new locomotion changes in the [Locomotion](locomotion.md) documentation.

### XR Interaction Simulator

The usability of the XR Device Simulator has been improved with a new replacement component and prefab, the XR Interaction Simulator. Update or import the [XR Device Simulator](samples-xr-device-simulator.md) sample in the **Window** &gt; **Package Manager** window to add this new functionality to your project. For those developers already making use of the XR Device Simulator, see the note below in [Simulator project settings](#simulator-project-settings).

The goal was to simplify spatial interaction and to make the controls more intuitive, along with a cohesive UI to better represent the state of the simulator. Further improvements are planned for future versions of the package to make the simulator even easier to use.

## Changed

### Simulator project settings

The default simulator prefab has been changed from the older `XR Device Simulator` to the new `XR Interaction Simulator`. To revert back to the classic control scheme and functionality of the XR Device Simulator, you can enable **Use Classic** in **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **XR Interaction Toolkit**.

> [!NOTE]
> Existing projects that already were configured in Project Settings to use the simulator will need to disable and re-enable **Use XR Interaction Simulator in scenes** in order to use the new simulator.

## Deprecations

### Gravity in locomotion

With the new locomotion changes outlined above, the gravity properties in move and grab move locomotion providers are now obsolete and have been deprecated to reflect this. The recommended way of configuring gravity is with the added Gravity Provider component instead, which has been added to the `XR Origin (XR Rig)` prefab from the [Starter Assets](samples-starter-assets.md) sample on the `Gravity` GameObject. The functionality of gravity in `ContinuousMoveProvider` and `ConstrainedMoveProvider` has been retained for backwards compatibility, however the gravity is now calculated with the `GravityProvider` component when that is added to the scene.
