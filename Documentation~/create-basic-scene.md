---
uid: xri-create-basic-scene
---

# Create a basic scene

This section will walk you through the steps to create an XR Origin camera rig for a head-mounted device and create the basic building blocks of XR interactivity.

<a name="import-starter-assets"></a>
## Import starter assets

See the [Starter Assets](samples-starter-assets.md) sample for steps to import assets to streamline setup of behaviors. That sample contains a default set of input actions and presets which will be used in this guide.

## Add the XR Interaction Manager

Add the XR Interaction Manager using **GameObject &gt; XR &gt; Interaction Manager**. The component on this GameObject allows the interactors and interactables in your scenes to interact with each other.

<a name="add-xr-origin"></a>
## Add the XR Origin camera rig for tracked devices

If you have [imported the starter assets](#import-starter-assets), you can add the **XR Origin (XR Rig)** prefab to the scene. This prefab contains the XR Origin required for any XR scene with interactor components already added, including:

* XR Origin
   * Camera
   * Gaze Interactor
   * Controller objects for right and left hand
      * Poke Interactor
      * Near-Far Interactor
      * Teleport Interactor
      * Controller visual (using a generic controller model)
   * Teleport stabilizer
   * Locomotion setup
      * Turn
      * Move
      * Grab Move (disabled by default)
      * Teleportation
      * Climb

You can find the Prefab in your project's Assets folder: `Assets/Samples/XR Interaction Toolkit/<version>/Starter Assets/Prefabs`.

If you prefer to set up the XR Origin yourself, you can add a minimal one from the **GameObject** menu: **GameObject &gt; XR &gt; XR Origin (VR)**. This also automatically creates a new Main Camera GameObject tagged as "MainCamera" as a child of a new Camera Offset GameObject. The GameObject with the Camera is assigned as the value of **Camera GameObject** on XR Origin. If you already had a Camera tagged "MainCamera" in your scene, Unity may warn about there being another Camera tagged "MainCamera" in your scene. You will typically only need one Main Camera, so you should delete the original Camera at this time. For more information about the Main Camera, see [`Camera.main`](https://docs.unity3d.com/ScriptReference/Camera-main.html).

> [!TIP]
> If you already have components that are referencing the original Camera GameObject, you may want to keep the original Camera instead. Drag the original Camera in the Hierarchy window to be a child GameObject of Camera Offset then reset the Transform by clicking the **More menu (&#8942;)** in the Inspector window next to Transform and select **Reset**. Then use the **More menu (&#8942;)** to **Copy Component** and **Paste Component As New** to move each additional component from the new Camera GameObject to your old Camera GameObject. Finally delete the new Camera GameObject and update **Camera GameObject** on XR Origin to your Camera.

The [XR Origin](https://docs.unity3d.com/Packages/com.unity.xr.core-utils@2.2/manual/xr-origin.html) component on this GameObject transforms trackable devices like the head-mounted display and controllers to their final position in the Unity scene. This is the GameObject that is moved around the environment to achieve locomotion rather than applying movement directly to the Main Camera itself.

The Camera Offset child GameObject that is created is automatically assigned as the value of **Camera Floor Offset Object** on XR Origin. This GameObject's position is updated automatically by Unity depending on the **Tracking Origin Mode** value on XR Origin.

For this guide, leave the **Tracking Origin Mode** set to **Not Specified**.

> [!NOTE]
> When the mode is **Device**, the XR runtime will generally report the position of tracked devices relative to a fixed position in space, such as the initial position of the HMD when started. Set the **Camera Y Offset** on XR Origin to the height you want the Main Camera to be above ground when in that mode. When the mode is **Floor**, the XR runtime will generally report the position of tracked devices relative to the player's real floor. Unity will automatically clear the height of the Camera Offset when in this mode since it is not necessary to artificially raise the tracking origin up. Set the mode to **Not Specified** to use the default mode of the XR runtime.

To have the position and rotation of the XR HMD update the Main Camera Transform, a Tracked Pose Driver (Input System) component is added. This component is configured to set the **Position Input** binding to `<XRHMD>/centerEyePosition`, the **Rotation Input** binding to `<XRHMD>/centerEyeRotation`, and the **Tracking State Input** binding to `<XRHMD>/trackingState`.

## Configure Interactor

An Interactor component controls how a GameObject interacts with Interactable components in the scene. There are multiple types of Interactors, one of which is an [Near-Far Interactor](xref:xri-near-far-interactor), a component that uses both [sphere casting](xref:UnityEngine.Physics.SphereCastNonAlloc(UnityEngine.Vector3,System.Single,UnityEngine.Vector3,UnityEngine.RaycastHit[],System.Single,System.Int32,UnityEngine.QueryTriggerInteraction)) and [ray casting](xref:UnityEngine.Physics.RaycastNonAlloc(UnityEngine.Ray,UnityEngine.RaycastHit[],System.Single,System.Int32,UnityEngine.QueryTriggerInteraction)) in order to find both close and distant Interactable objects in the scene.

To read input from an XR input device, the Interactor has various input properties that allow you to define the source of input. One example is the **Select Input** which allows you to set the input action that triggers selection. Refer to the property table in the documentation for [Interactor components](xref:xri-interactor-components) or refer to the tooltips in the Inspector window for more information.

To enable haptic vibration feedback for an interactor, add a [Simple Haptic Feedback](simple-haptic-feedback.md) component to the GameObject to trigger haptics in response to select and hover events. You can then specify intensities and durations of haptic feedback to play back on select and hover events in the Inspector window. That component references a [Haptic Impulse Player](haptic-impulse-player.md) component (which can be shared by different interactors) that is used to actually play the haptics to a device. On the Haptic Impulse Player, set **Haptic Output** to an input action with a binding path to any active control, such as `<XRController>{LeftHand}/*`, in order to identify the device. If you are using OpenXR, you can instead set the binding path to a `haptic` control.

The **UI Interaction** option controls whether this Interactor can interact with Unity UI elements in a world space canvas in the scene. See [UI Setup](ui-setup.md) for more information and steps for enabling UI interactivity.

To have the position and rotation of a motion controller update the Transform component, a Tracked Pose Driver (Input System) component should be added. As an example for the left controller, this component should be configured to set the **Position Input** binding to `<XRController>{LeftHand}/devicePosition`, the **Rotation Input** binding to `<XRController>{LeftHand}/deviceRotation`, and the **Tracking State Input** binding to `<XRController>{LeftHand}/trackingState`.

## Enable an XR provider

Open **Edit &gt; Project Settings &gt; XR Plug-in Management**. If you do not yet have the package installed, click **Install XR Plugin Management** in that window. Select one or more of the plug-in providers for the device(s) you wish to target, such as [Oculus](https://docs.unity3d.com/Packages/com.unity.xr.oculus@latest/) or [Open XR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest/). Open XR will require additional configuration, see its package documentation for those steps.
