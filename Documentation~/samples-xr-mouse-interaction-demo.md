---
uid: xri-samples-xr-mouse-interaction-demo
---
# XR Mouse Interaction Demo

This sample demonstrates Android Mouse input support using the Android Mouse OpenXR Extension with the XR Interaction Toolkit. It contains a demo scene and the assets required to integrate a physical mouse connected to an Android XR device.

This sample is installed into the default location for package samples, in the `Assets\Samples\XR Interaction Toolkit\[version]\XR Mouse Interaction Demo` folder. You can move these Assets to a different location.

|**Asset**|**Description**|
|---|---|
|**`Editor\Scripts\`**|Asset folder containing Unity Editor scripts which adds Project Validation rules for the sample.|
|**`Materials\`**|Asset folder containing materials for the mouse cursor visual.|
|**`Prefabs\`**|Asset folder containing the `XRMouse` prefab.|
|**`Scripts\`**|Asset folder containing the `XRMousePointer` and `XRMouseCursorVisual` scripts.|
|**`Textures\`**|Asset folder containing textures for the mouse cursor visual.|
|**`XRMouseDemoScene`**|Scene based on the Hands Interaction Demo scene that demonstrates mouse input on Android XR devices.|

## Prerequisites and setup

> [!IMPORTANT]
> This sample requires the [Hands Interaction Demo](samples-hands-interaction-demo.md) sample to be imported first, as the demo scene and prefabs build on top of the hand tracking rig it provides.

In order for this sample to function properly, the following packages and samples are required. Install packages by clicking **Fix** in **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **Project Validation**.

* [Hands Interaction Demo](samples-hands-interaction-demo.md) sample - imported from Package Manager under XR Interaction Toolkit in the Samples area (including all of its own prerequisites)
* **Unity 6 (6000.0) or newer** - Required for Android XR mouse interaction profile support
* [OpenXR Plugin (com.unity.xr.openxr)](https://docs.unity3d.com/Manual/com.unity.xr.openxr.html) - 1.17.0 or newer
* [Android XR (OpenXR) (com.unity.xr.androidxr-openxr)](https://docs.unity3d.com/Packages/com.unity.xr.androidxr-openxr@latest/) - Required for Android XR platform support

### Enabling the Android Mouse Interaction Profile

After installing the required packages, you must enable the **Android Mouse Interaction Profile** in OpenXR settings:

1. Open **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **OpenXR**.
2. Select the **Android** tab.
3. Under **Interaction Profiles**, add **Android Mouse Interaction Profile**.

The `XRMousePointer` will not function without this profile enabled.

## Scripts

### XRMousePointer

Reads input from the Android Mouse Interaction Profile via `XRInputValueReader` and `XRInputButtonReader`. Each frame it reads the aim position and rotation, scroll delta, and click button states, then forwards the processed data to `XRMouseCursorVisual`.

The component adjusts the cursor radius in response to scroll wheel input when the `NearFarInteractor` is not hovering over a scrollable UI element. When over a scrollable UI element, scroll input passes through to the UI instead.

The component requires Unity 6 and OpenXR Plugin 1.17.0 or newer with the Android Mouse Interaction Profile enabled. It will deactivate itself at startup if those requirements are not met.

### XRMouseCursorVisual

Manages the visual cursor GameObject. Each frame it receives the aim center, direction, rotation, and radius from `XRMousePointer` and places the cursor at the raycast endpoint of the `NearFarInteractor`. It handles a small depth offset toward the camera to prevent z-fighting on surfaces, aligns the cursor to surface normals on 3D hits, and drives hover and select scale animations.

All cursor positioning uses local-space transforms on the XR Origin so the cursor remains correct after teleportation.

## Prefabs

### XRMouse

The top-level prefab that wires up `XRMousePointer` and `XRMouseCursorVisual` together with the cursor GameObjects (center dot and ring). Add this prefab as a child of the **Camera Offset** GameObject in an `XR Origin Hands (XR Rig)` to enable mouse support.

## Troubleshooting

If you encounter validation errors, open **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **Project Validation**. The following requirements are automatically validated when the `XR Mouse Interaction Demo` sample is installed:

* **Hands Interaction Demo sample** - Must be imported from XR Interaction Toolkit. Clicking **Fix** will import it automatically.
* **Unity Version** - Unity 6 or newer is required. If you're using an older version, clicking **Fix** will automatically disable the component.
* **OpenXR Package Version** - OpenXR 1.17.0 or newer is required. Clicking **Fix** will automatically update the package.
* **Android XR Package** - The Android XR (OpenXR) package must be installed. Clicking **Fix** will automatically install the package.
* **Android Mouse Interaction Profile** - The Android Mouse Interaction Profile must be enabled in OpenXR settings. Clicking **Fix** will open the OpenXR settings window where you can manually add the profile to the Interaction Profiles list (Android tab).
