---
uid: xri-whats-new-2-2
---
# What's new in version 2.2

Summary of changes in XR Interaction Toolkit package version 2.2:

With the XR Interaction Toolkit version 2.2.0 comes even more highly-requested features and updates. The main features in this release include multi-grab support, new locomotion methods and improvements, and a collection of completely ready-to-go prefabs in our Starter Assets sample package.

## Multi-grab Support
XRI now supports grabbing objects with multiple hands/controllers. XR Grab Interactables have been upgraded with our new system of [Grab transformers](xr-grab-interactable.md#grab-transformers). This behavior is enabled automatically by default and you don't even have to update your existing scenes, as default grab transformers are applied at runtime if none are provided. The solving method for grabbing with one or more hands/controllers is also fully configurable. We have made the [`IXRGrabTransformer`](xref:UnityEngine.XR.Interaction.Toolkit.Transformers.IXRGrabTransformer) interface fully extensible so that you can create your own custom movement transformers to fit your gameplay or application requirements. More information for this new feature can be found in the [XR Grab Interactable](xr-grab-interactable.md#grab-transformers) documentation.

## Starter Prefabs
Within the [Starter Assets](samples-starter-assets.md) sample that ships with XRI, you will now find a collection of prefabs and a demo scene to get you started more easily in your own projects. Included is a `Complete XR Origin Set Up` prefab which enables several modes of locomotion, teleportation, and interaction via Ray and Direct interactors. This prefab uses Action-based components included with XRI along with the `XRI Default Input Actions` asset. Also included is a simple demo scene that illustrates some basic setup using XRI to accelerate your learning and development with the toolkit. In addition to the demo scene and complete set up prefab, there are other prefabs for grab interactable objects, teleportation areas, and UI interaction using UGUI. These prefabs are supported by some sample scripts to get you started with controller management and input mediation. Please review the [Samples](samples-starter-assets.md#prefabs) documentation for more information on installation and configuration.

> [!NOTE]
> As of version 2.4.0, some of the Starter Assets prefabs have been renamed. Notably the `Complete XR Origin Set Up` prefab, mentioned above, has been renamed to `XR Interaction Setup`. Refer to the [Starter Assets](samples-starter-assets.md) sample documentation for the latest information on the sample and the prefabs included.

## Locomotion Improvements
Added support for teleportation directionality so that users can specify the direction they will face when teleportation finishes  by rotating the thumbstick on a controller. This includes the addition of a `Teleport Direction` input action for each hand in the `XRI Default Input Actions` asset in the `Starter Assets` sample. If you already have a teleport controller setup in your scene, you will need to find the XR Ray Interactor component associated with one or both hands where the teleport controller lives and enable the checkbox for `Anchor Control`, then set the `Rotation Mode` to `Match Direction`. To enable teleport interactables in the scene to use directional input from the controllers, you will need to check the box for `Match Directional Input` on the **Teleportation Anchor** or **Teleportation Area**. Note that this option will only appear in the Inspector when the `Teleportation Configuration` option for `Match Orientation` is set to either `World Space Up` or `Target Up`.

> [!NOTE]
> As of version 3.0.0, Anchor Control has been renamed to Manipulate Attach Transform. As of version 3.1.0, Teleport Direction input action has been renamed to Manipulation.

We have also added a new **Grab Move Provider**. This method of locomotion can best be described as grabbing the world itself and pulling yourself through it. Each hand provides the means to translate yourself through the world space, but also included is a **Two-Handed Grab Move Provider**, which combines the **Grab Move Provider** on each hand and allows you to rotate and scale yourself in the world. The `XRI Default Input Actions` in the `Starter Assets` have been updated to include the appropriate actions to support this new mode of locomotion. For more information, read the documentation on [Locomotion](xref:xri-grab-movement), the [Grab Move Provider](grab-move-provider.md) and the [Two-Handed Grab Move Provider](two-handed-grab-move-provider.md).

## Changes and Fixes

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).
