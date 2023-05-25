# What's new in version 2.4.0

Summary of changes in XR Interaction Toolkit package version 2.4.0:

## Added

### XR Ray Stabilization and Visual Improvements

The XR Interactor Line Visual has been updated so that it bends to the selected interactable by default. This helps improve the natural feeling of the Ray Interactor along with easier interactions for users. Along with the visual changes, the performance has also been improved for the line visual and ray interactor by optimizing most of the line computation math for the Burst compiler. The [Burst package](https://docs.unity3d.com/Manual/com.unity.burst.html) must be installed in your project to take advantage of the optimizations. In addition to the visual and performance updates, ray stabilization has been added by the use of the [XR Transform Stabilizer](xr-transform-stabilizer.md) component. This applies optimized stabilization techniques to remove pose jitter and makes aiming and selecting with rays easier for users. These new updates have been added to the [Starter Assets](samples.md#starter-assets) prefabs and set up for immediate use.

### XR Gaze and Aim Assistance

Building upon gaze interaction support from XRI 2.3, [XR Gaze Assistance](xr-gaze-assistance.md) allows specified controller-based ray interactors to fallback to eye-gaze for primary aiming and selection when they are off screen or pointing off screen. This component enables split interaction functionality to allow the user to aim with eye gaze and select with a controller. The XR Gaze Assistance component also has an aim-assist feature. This works by auto adjusting trajectories for thrown objects or projectiles to help them hit the gazed-at targets. 

### Hand Interaction Additions & Updates

#### XR Input Modality Manager

The new [XR Input Modality Manager](xr-input-modality-manager.md) manages swapping between hands and controllers at runtime based on whether hands and controllers are tracked. Updated prefabs in the package samples to make use of this component.

#### Reactive Hand Visuals

Updated the [Hands Interaction Demo](samples.md#hands-interaction-demo) with new interaction-reactive visuals which respond to interaction strength visually for each finger. The `XR Origin Hands (XR Rig)` prefab in the Hands Interaction Demo was updated to use prefabs for each hand visual with affordances to highlight the fingers during interaction.

#### Hand Tracking for Device Simulator

The [XR Device Simulator](xr-device-simulator-overview.md) has received another big update with the added support for simulating Hand Tracking. This comes with a number of standard, pre-defined poses that can be used to test hand-based interactions from inside the editor. 

### Climb Locomotion Provider

A new [Climb Locomotion Provider](climb-provider.md) and [Climb Interactables](climb-interactable.md) allow users to grab and pull themselves along a set of climbable objects. This works in any direction to create ladders, climbing walls or even monkey bars. A Climb Provider instance has been added to `XR Origin Preconfigured` in the [Starter Assets](samples.md#starter-assets) sample along with a `Climb Sample` prefab that can be tested out in the `DemoScene`. This prefab includes preconfigured Climb Interactables.

## Changes and Fixes

For a full list of changes and updates in this version, see the [XR Interaction Toolkit package changelog](../changelog/CHANGELOG.html).