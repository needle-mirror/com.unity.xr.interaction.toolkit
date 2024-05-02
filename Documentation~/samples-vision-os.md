# visionOS Sample

The visionOS sample provides a working demo of core interactors working with visionOS input. While some work has been done to accommodate the discontinuous nature of pose tracking on the platform, the interaction paradigm demonstrated on other platforms is intact here.

Importantly, a simple swap of the XR Origin (XR Rig) prefab and the introduction of a volume camera used in a scene that is already designed around hand tracking should be all that's required to get XRI content to work in a mixed reality (Bounded or Unbounded) context on visionOS.

## Prerequisites and setup

This sample and its dependencies are configured to run with the latest [Unity 2022.3 LTS](https://unity.com/releases/editor/qa/lts-releases) or newer (minimum 2022.3.19f1). A Unity Pro license is also [required](https://unity.com/campaign/spatial) to access the PolySpatial packages.

In order for this sample to function properly, a few additional packages are required. Install these by clicking **Fix** in **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **Project Validation** or by using the **Window** &gt; **Package Manager** window.
  * [Shader Graph](https://docs.unity3d.com/Manual/com.unity.shadergraph.html) - For the materials used. PolySpatial requires the use of Shader Graph for any custom materials.
  * [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/index.html) - For mixed reality support.
  * [PolySpatial XR](https://docs.unity3d.com/Packages/com.unity.polyspatial.xr@1.1/manual/index.html) - For PolySpatial XR functionality.
  * [PolySpatial visionOS](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@1.1/manual/index.html) - For visionOS support with PolySpatial.

### Key interactions supported
- Touching objects with your index finger tip will allow interactables to receive poke interaction events, leveraging the [poke interactor](xr-poke-interactor.md).
- Gazing at objects will highlight them using the [`VisionOSHoverEffect`](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@1.1/manual/HoverEffect.html) component, and on pinch [`VisionOSFarCaster`](#visionosfarcaster) will provide that object as a valid target to the [`NearFarInteractor`](near-far-interactor.md).
- Direct pinching objects will leverage the [`PointNearCaster`](#pointnearcaster) and prioritize close proximity targets over the object surfaced by the platform input.

Given the modular paradigm of the Near-Far Interactor, it is possible to selectively disable either near or far interaction at authoring or runtime to control how interaction behaves.

This sample is installed into the default location for package samples, in the `Assets\Samples\XR Interaction Toolkit\[version]\visionOS` folder. You can move these Assets to a different location.

|**Asset**|**Description**|
|---|---|
|**`Editor\`**|Contains project validation logic for the Unity Editor.|
|**`Input\`**|Contains action map and generated C# bindings used by [`SpatialTouchInputReader.cs`](#spatialtouchinputreader).|
|**`Materials\`**|Contains SpatialUI materials added in this sample. Other materials are located in the [Starter Assets sample](samples-starter-assets.md).|
|**`Models\`**|Contains 3D models used in this sample, namely the Spatial UI .fbx model, as well as rounded cube and platform meshes.|
|**`Prefabs\`**|Contains numerous starter prefab and example object prefabs. Importantly look for the `XRI_SimpleRig` prefab to get started.|
|**`Resources\`**|Contains volume config scriptable object. This is required to configure mixed reality PolySpatial applications.|
|**`Scripts\`**|Contains the sample's scripts. See the [`Input Bridge`](#scripts--input-bridge) folder for the core scripts required to make the XRI Rig work with visionOS.|
|**`Shaders\`**|Asset folder containing Shader Graph shaders used in the scene.|
|**`Themes\`**|Contains [Affordance System](affordance-system.md) themes created for this sample. Grab interaction's rely on themes from the [Starter Assets sample](samples-starter-assets.md).|
|**`VolumeDemo`**|Scene that demonstrates XRI interaction utilizing PolySpatial input events for direct, indirect, and poke-based input in a shared space volume.<br />See [Demo scene](#demo-scene) below.|

## Demo scene

The `VolumeDemo` scene shows a minimal bounded volume setup designed to demo 3D interaction using XRI. It features two-handed interactions supported on cubes, a floating cylinder, and tapered cylinder. 

The cubes and tapered cylinder are set up using velocity tracked mode on the [`XRGrabInteractable`](xr-grab-interactable.md), to showcase how physics interaction is meant to behave. The floating cylinder uses the lower latency instantaneous mode, which ignores physics collisions when evaluating motion.

There is also a reset button in the scene that is meant to demo a tactile poke interactable using an [`XRPokeFollowAffordance`](samples-starter-assets.md#scripts) component to animate the button position as the poke depth changes. When fully pressed it will trigger the reset function on all interactables in the scene with a `Resetable` component on them, to re-initialize them to the original location.

Finally, there is an instance of the [`SpatialPanel_UI`](#spatialpanel_ui) prefab meant to show how common UI components can be interacted with using XRI interaction events.

## Prefabs
- **Interactables**
  - `GrabAffordance`
  - `XRI_Cube`
- **Interactors**
  - `Primary Interaction Group`
  - `Secondary Interaction Group Variant`
- **SpatialUI**
  - `ButtonAffordance`
  - `SpatialPanel_UI`
- `XRI_SimpleRig`

The most important prefab in this folder is the `XRI_SimpleRig`, which uses the prefabs in the `Interactors` folder to create a functional rig that can be dropped into an XRI scene and allow interaction to occur on visionOS, assuming the project is well configured.

The `Primary Interaction Group`, and its secondary variant, are configured to handle poke, direct, and indirect pinch for the corresponding spatial pointer device input.

`GrabAffordance` shows a minimal setup used to enable select rim shader effects to pair with an [XRI interactable](xr-simple-interactable.md). `ButtonAffordance` does the same for the `SpatialUI` components.

`XRI_Cube` is a simple grab interactable that supports two-handed interaction by enabling Multiple on Select Mode property from [`XRGrabInteractable`](xr-grab-interactable.md). While `XRGrabInteractable` automatically adds a `GeneralGrabTransformer` component at runtime if needed, the component has been added to the GameObject to allow two-handed scaling by enabling the Allow Two Handed Scaling property on that component.

`SpatialPanel_UI` is a 3D UI panel showing common interaction patterns in UI, implemented using XRI interaction events. This panel should be compatible with projects that use XRI on other supported platforms.

## Scripts / Input Bridge
The input bridge folder represents the core of the work done to make XRI interactors compatible with visionOS.

### SpatialTouchInputReader
[XRI 3.0](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/whats-new-3.0.html) introduced [Input Readers](architecture.md#input-readers), which are a core part of managing input events that interactors rely on to correctly handle state transitions. Because interactors poll input from an abstracted Input Reader rather than input actions directly, it is possible to write a custom implementation of input logic that can process input before they're exposed to interactors.

To handle an interactor's select state transition, we need to implement [`IXRInputButtonReader`](xref:UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.IXRInputButtonReader) which exposes a bool through `ReadIsPerformed` to tell the interactor when select is active.

`SpatialTouchInputReader` implements `IXRInputButtonReader` and is set up to bind to `performed` calls on the generated `SpatialPointerInput` input class, which exposes the Primary `SpatialPointer0` and Secondary `SpatialPointer1` spatial pointers. visionOS does not provide handedness information for these pointers, and the pointer `kind` can change throughout the lifecycle of an interaction. If no input is active, the first hand to pinch or poke something will be the primary pointer, and the second will be the secondary.

Note: This input reader will not function in fully immersive VR mode because different input action binding are required to source input events in that mode.

#### Frame behavior of pointer events
Because of the way PolySpatial is set up to work with visionOS, input performed calls may not arrive at regular intervals. In one frame, you might receive 3 callbacks, while in the next, you'll receive 0. This erratic input timing can lead to issues if your input stack expects only one state transition per frame, which is indeed the case here. Consider a scenario where you poll `ReadValue` once per frame to get the last passed value. If the previous frame started an interaction with a `Begin` phase event, and the current frame ended that interaction and started a new one, the last state would still be `Begin`. While two different interactions have occurred, the polling input code would have failed to transition between them correctly.

To address this issue, `SpatialTouchInputReader` implements a queue to buffer input events, capturing every one that passes through. Then, in the component's update loop, we can work our way through the queue and ensure that we only change the input `ReadIsPerformed` state once per frame, assuring the integrity of `WasPerformedThisFrame` and `WasCompletedThisFrame`. This queue also allows us to safely exit interactions when an incompatible `kind` is passed through. Our XRI rig is configured with a separate input reader for poke (Touch) and for Near-Far interaction (Direct & Indirect Pinch), and visionOS' fluidity in change `kind`s requires careful consideration.

### VisionOSFarCaster
[XRI 3.0](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/whats-new-3.0.html) introduced the [Near-Far Interactor](near-far-interactor.md), which modularized the components responsible for sourcing the list of valid targets considered by the interactor. One of the key motivations for this change was to make it possible for valid targets to be determined by other means than physics casting.

The visionOS Far Caster is a prime example of the power of this system, as it sources its valid targets directly from the [`SpatialPointerState`](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@1.1/manual/PolySpatialInput.html) struct exposed by the [`SpatialTouchInputReader`](#spatialtouchinputreader).

Note: This caster will only work in bounded and unbounded mixed reality modes, and not in fully immersive VR mode. This is because of the limitation of the [`SpatialTouchInputReader`](#spatialtouchinputreader), but also because visionOS only sources an entity id (valid target) when ARKit is doing the rendering.

### PointNearCaster
Much like the [`VisionOSFarCaster`](#visionosfarcaster) is in charge of handling far casting for the [Near-Far Interactor](near-far-interactor.md), we need to have a near interaction caster that is well suited to the discontinuous input of visionOS.

The `PointNearCaster` functions very similarly to the [`SphereInteractionCaster`](near-far-interactor.md) in XRI, except that instead of sweeping the distance covered between frames for valid targets, a simple overlap sphere check is performed when [`SpatialTouchInputReader`](#spatialtouchinputreader) reports valid positions. 

While in theory the entity id returned by the [`SpatialTouchInputReader`](#spatialtouchinputreader) during `DirectPinch` input events should be sufficient for near interaction, in practice, visionOS fails to prioritize direct pinch over indirect when the user's hand is directly over objects. For this reason, the sphere overlap check done by this component at the pinch device position takes precedence over the target returned by [`VisionOSFarCaster`](#visionosfarcaster), simply because the Near-Far Interactor prioritizes near interactors over far by default.

### Poke Interaction Toggler
Unlike the [Near-Far Interactor](near-far-interactor.md) which allows the valid targets to be determined by caster components, the [XR Poke Interactor](xr-poke-interactor.md) does not support custom casters as of this release.

As a result, the Poke Interactor is ill-suited for discontinuous input and does not support custom entity id target sources. The `PokeInteractorToggler` component simply listens for changes in the [`SpatialTouchInputReader`](#spatialtouchinputreader) and toggles the `XRPokeInteractor` on and off as touch input comes and goes. This ensures that sudden jumps in position of the poke interactor do not cause accidental touches to occur.

## Scripts / UI
The following components are ported from PolySpatial samples to leverage XRI interaction events.

### Drop-Down Component
Simple interactable drop down component that activates a pop-up of `DropDownElement` components.

### Drop-Down Element
Drop-Down Element component pairs with the `DropDownComponent` to handle selecting an element and to close the drop-down pop-up.

### Slider Component
Slider component that uses a material property to animate the fill amount of a slider. Poking the slider will move the slider end to the poke position. Indirect pinching will move the slider using a motion delta as the user's hand moves left and right.

A float [UnityEvent](https://docs.unity3d.com/Manual/UnityEvents.html) is exposed to the Unity Editor to allow scripts to bind to the fill amount of the slider.

### Toggle Component
Simple toggle interactable that animates the toggle visual according to the current toggle state.

A bool [UnityEvent](https://docs.unity3d.com/Manual/UnityEvents.html) is exposed to the Unity Editor to allow scripts to bind to the toggle state.

## Input
Folder holding the input action map used by this sample. A generated C# script also exposes the input actions to be bound through code.

See [`SpatialTouchInputReader`](#spatialtouchinputreader) for details on how this is used.

## Known limitations
- Because poke relies on continuous motion to determine interaction, and volumes do not have access to the hand skeleton data, this sample relies on the touch data returned by the spatial pointer API, which can lead to some late starts on poke or incorrect angle threshold calculations in some situations. Work is ongoing to improve the feel of poke with visionOS.
- At the time of this release, PolySpatial requires that all canvases have a PolySpatial Graphics Raycaster component on them, which is incompatible with the [`TrackedDeviceGraphicsRaycaster`](ui-setup.md#tracked-device-graphics-raycaster) in the XR Interaction Toolkit. Because of this, UGUI interaction is not well supported on visionOS in projects that use XRI. We are investigating ways of improving support for this for future releases of PolySpatial and XRI.

## Helpful links

If you have a question after reading the documentation, you can:
* Join our XRI [support forum](https://forum.unity.com/forums/xr-interaction-toolkit-and-input.519/).
* Visit the visionOS [support forum](https://discussions.unity.com/c/visionos).
* Search the [issue tracker](https://issuetracker.unity3d.com/product/unity/issues?project=192&status=1&unity_version=&view=newest) for active issues.
* View our [public roadmap](https://portal.productboard.com/brs5gbymuktquzeomnargn2u) and submit feature requests.
