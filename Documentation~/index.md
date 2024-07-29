---
uid: xri-index
---
# XR Interaction Toolkit

The XR Interaction Toolkit package is a high-level, component-based, interaction system for creating VR and AR experiences. It provides a framework that makes 3D and UI interactions available from Unity input events. The core of this system is a set of base Interactor and Interactable components, and an Interaction Manager that ties these two types of components together. It also contains components that you can use for locomotion and drawing visuals.

XR Interaction Toolkit contains a set of components that support the following Interaction tasks:
- Cross-platform XR controller input: Meta Quest (Oculus), OpenXR, Windows Mixed Reality, and more.
- Basic object hover, select and grab
- Haptic feedback through XR controllers
- Visual feedback (tint/line rendering) to indicate possible and active interactions
- Basic canvas UI interaction with XR controllers
- Utility for interacting with XR Origin, a VR camera rig for handling stationary and room-scale VR experiences

To use the AR interaction components in the package, you must have the [AR Foundation](https://docs.unity3d.com/Manual/com.unity.xr.arfoundation.html) package in your Project. The AR functionality provided by the XR Interaction Toolkit includes:
- AR gesture system to map screen touches to gesture events in the Input System via the `TouchscreenGestureInputController`.
- Various Screen Space input components that feeds screen-space interaction data into an `XRRayInteractor` that works with AR.
- `ARTransformer` that translates gestures such as place, select, translate, rotate, and scale into object manipulation.

Finally, its possible to simulate all of your interactions with the [XR Device Simulator](xr-device-simulator.md) in case you don't have the hardware for the project you are working on, or just want to test interactions without entering the headset. For more information, see [XR Device Simulator overview](xr-device-simulator-overview.md).

## Technical details

### Requirements

This version of the XR Interaction Toolkit is compatible with the following versions of the Unity Editor:

* 2021.3 and later

### Dependencies

The XR Interaction Toolkit package has several dependencies which are automatically added to your project when installing:

* [Input System (com.unity.inputsystem)](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/index.html)
* [Mathematics (com.unity.mathematics)](https://docs.unity3d.com/Packages/com.unity.mathematics@1.2/manual/index.html)
* [Unity UI (com.unity.ugui)](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/index.html)
* [XR Core Utilities (com.unity.xr.core-utils)](https://docs.unity3d.com/Packages/com.unity.xr.core-utils@2.2/manual/index.html)
* Built-in modules
  * [Audio](https://docs.unity3d.com/Manual/com.unity.modules.audio.html)
  * [IMGUI](https://docs.unity3d.com/Manual/com.unity.modules.imgui.html)
  * [Physics](https://docs.unity3d.com/Manual/com.unity.modules.physics.html)
  * [XR](https://docs.unity3d.com/Manual/com.unity.modules.xr.html)

#### Optional dependencies

To enable additional AR interaction components included in the package, [AR Foundation (com.unity.xr.arfoundation)](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest/) must be added to your project using Package Manager.

To enable additional properties in some behaviors, the [Animation](https://docs.unity3d.com/Manual/com.unity.modules.animation.html) module must be added to your project using Package Manager.

### Known limitations

* When using multiple interactor support on XR Grab Interactables and transferring between a socket interactor and direct/ray interactor, if **Attach Ease In Time** is set to 0, there can be a 1 frame visual skip that can occur. To mitigate this visual disturbance, set the **Attach Ease In Time** to a minimum of 0.15. You can also resolve this issue by loading the scene containing the socket interactors after the controller interactors are registered with the XR Interaction Manager if your project does not enable or disable the direct/ray interactors at runtime in order to make the sockets registered last.

* Mouse inputs don't interact with world space UIs when an XR Plug-in Provider in **Edit &gt; Project Settings &gt; XR Plug-in Management** is enabled and running. For more information, please follow the issue tracker. ([1400186](https://issuetracker.unity3d.com/product/unity/issues/guid/1400186/))

* The Poke Point visual in the Poke Interactor prefab in the Starter Assets sample does not hide with the controller model when the **Hide Controller On Select** property is enabled on the direct/ray interactor.

* Single-pass instanced rendering is not supported when using Shader Graph shaders on the Built-in Render Pipeline. This affects shaders included in the sample assets, including the Hand Interaction Demo. (A symptom of the issue is that one eye renders incorrectly.) If your project uses the Built-in Render Pipeline, you can avoid the problem by taking one of the following actions:

   * Switch to [multi-pass rendering](xref:SinglePassStereoRendering). (Performance of multi-pass rendering is typically much worse than single-pass.)
   * Change the sample Materials to use non-Shader Graph shaders.
   * Update the project to use the Universal Render Pipeline (URP).


### Helpful links

If you have a question after reading the documentation, you can:

* Join our [support forum](https://forum.unity.com/forums/xr-interaction-toolkit-and-input.519/).
* Search the [issue tracker](https://issuetracker.unity3d.com/product/unity/issues?project=192&status=1&unity_version=&view=newest) for active issues.
* View our [public roadmap](https://portal.productboard.com/brs5gbymuktquzeomnargn2u) and submit feature requests.
* Download [example projects](https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples) that demonstrates functionality.
