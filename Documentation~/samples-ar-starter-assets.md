---
uid: xri-samples-ar-starter-assets
---
# AR Starter Assets

This sample is installed into the default location for package samples, in the `Assets\Samples\XR Interaction Toolkit\[version]\AR Starter Assets` folder. You can move these Assets to a different location after importing it.

The XR Interaction Toolkit package provides an example implementation for mobile AR development. This includes the a demo scene and some prefabs to get you started with your own projects.

|**Asset**|**Description**|
|---|---|
|**`DemoAssets\`**|Asset folder containing assets only used for the `ARDemoScene`.|
|**`Editor\Scripts\`**|Asset folder containing Unity Editor scripts which adds Project Validation rules for the sample.|
|**`Materials\FeatheredPlaneMaterial`**|This material provides a visual representation used for AR plane detection in the XR Simulator and in builds.|
|**`Prefabs\AR Feathered Plane`**|This prefab uses the `FeatheredPlaneMaterial` to show where the AR plane is detected or simulated.|
|**`Prefabs\Screen Space Ray Interactor`**|This prefab contains the component for adding the input device to the input system that the Touchscreen Gestures actions are bound to and the XR Ray Interactor setup to work with mobile AR devices.|
|**`Prefabs\XR Origin (AR Rig)`**|This contains the basic configuration to use interactions with a mobile device, containing components from AR Foundation and leveraging the Screen Space Ray Interactor prefab and the AR settings on the XR Ray Interactor.|
|**`Scripts\ARContactSpawnTrigger`**|Component that spawns an object from the designated list at the physical contact point with an AR plane.|
|**`Scripts\ARFeatheredPlaneMeshVisualizer`**|Component that supports a feathering effect at the edge of a detected plane, which reduces the visual impression of a hard edge.|
|**`Scripts\ARInteractorSpawnTrigger`**|Component that spawns an object from the designated list at the point where the Ray Interactor hits the AR plane when the specific `Spawn Object` action is triggered from the Input System.|
|**`ARDemoScene`**|Scene that illustrates a basic AR rig setup that supports screen space touch gestures along with example interactable objects that can be spawned and UI.|

## Prerequisites and setup

In order for this sample to function properly, a few additional packages are required. Install these by clicking **Fix** in **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **Project Validation** or by using the **Window** &gt; **Package Manager** window.
  * [Starter Assets](samples-starter-assets.md) sample - imported from Package Manager under XR Interaction Toolkit in the Samples area
  * [AR Foundation (com.unity.xr.arfoundation)](https://docs.unity3d.com/Manual/com.unity.xr.arfoundation.html) - 4.2.8 or newer
  * [Apple ARKit XR Plugin (com.unity.xr.arkit)](https://docs.unity3d.com/Packages/com.unity.xr.arkit@latest/index.html) - 4.2.8 or newer for iOS
  * [Google ARCore XR Plugin (com.unity.xr.arcore)](https://docs.unity3d.com/Packages/com.unity.xr.arcore@latest/index.html) - 4.2.8 or newer for Android

### Build settings:

You will need to set your target platform as follows:

1. Open **File** &gt; **Build Profiles**

2. Set the active build target to Android or iOS.

3. Specify `Assets\Samples\XR Interaction Toolkit\[version]\AR Starter Assets\ARDemoScene` in Scene List.

### Project settings:

Your project will need to enable the appropriate XR Plug-in for your build target.

1. Open **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management**.
   * Select Google ARCore for Android or Apple ARKit for iOS.

#### Renderer features:

If your project was created using the Universal 3D Template, then there are a few more steps to set up `ARDemoScene` to use the Universal Render Pipeline:

1. Open **Edit** &gt; **Project Settings** &gt; **Graphics**.

   * Set **Default Render Pipeline** to **Mobile_RPAsset**.

2. Open **Edit** &gt; **Project Settings** &gt; **Quality**.

   * Set **Quality Level** to **Mobile**.

3. Browse to `Assets\Settings\Mobile_Renderer` in the Project window.

   * Click **Add Renderer Feature**, and add **AR Background Renderer Feature**.

   * Click **Add Renderer Feature**, and add **AR Command Buffer Support Renderer Feature**.
