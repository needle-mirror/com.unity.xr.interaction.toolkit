---
uid: xri-general-setup
---
# General setup

This section will walk you through the steps to create an XR Origin camera rig for a head-mounted device and create the basic building blocks of XR interactivity.

These steps will guide you through setup to use [Input Actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/Actions.html) to indirectly read input from one or more controls, which is the recommended path.

## Import starter assets

See the [Starter Assets](samples-starter-assets.md) sample for steps to import assets to streamline setup of behaviors. That sample contains a default set of input actions and presets which will be used in this guide.

## Create the XR Interaction Manager

Create the XR Interaction Manager using **GameObject &gt; XR &gt; Interaction Manager**. The component on this GameObject will allow the interactors and interactables in your scenes to interact with each other.

## Create the XR Origin camera rig for tracked devices

Create the XR Origin camera rig using **GameObject &gt; XR &gt; XR Origin (VR)**. This will also automatically create a new Main Camera GameObject tagged as "MainCamera" as a child of a new Camera Offset GameObject. The GameObject with the Camera is assigned as the value of **Camera GameObject** on XR Origin. If you already had a Camera tagged "MainCamera" in your scene, Unity may warn about there being another Camera tagged "MainCamera" in your scene. You will typically only need one Main Camera, so you should delete the original Camera at this time. For more information about the Main Camera, see [`Camera.main`](https://docs.unity3d.com/ScriptReference/Camera-main.html).

> [!TIP]
> If you already have components that are referencing the original Camera GameObject, you may want to keep the original Camera instead. Drag the original Camera in the Hierarchy window to be a child GameObject of Camera Offset then reset the Transform by clicking the **More menu (&#8942;)** in the Inspector window next to Transform and select **Reset**. Then use the **More menu (&#8942;)** to **Copy Component** and **Paste Component As New** to move each additional component from the new Camera GameObject to your old Camera GameObject. Finally delete the new Camera GameObject and update **Camera GameObject** on XR Origin to your Camera.

The [XR Origin](https://docs.unity3d.com/Packages/com.unity.xr.core-utils@2.2/manual/xr-origin.html) component on this GameObject transforms trackable devices like the head-mounted display and controllers to their final position in the Unity scene. This is the GameObject that is moved around the environment to achieve locomotion rather than applying movement directly to the Main Camera itself.

The Camera Offset child GameObject that is created is automatically assigned as the value of **Camera Floor Offset Object** on XR Origin. This GameObject's position is updated automatically by Unity depending on the **Tracking Origin Mode** value on XR Origin.

For this guide, leave the **Tracking Origin Mode** set to **Not Specified**.

> [!NOTE]
> When the mode is **Device**, the XR runtime will generally report the position of tracked devices relative to a fixed position in space, such as the initial position of the HMD when started. Set the **Camera Y Offset** on XR Origin to the height you want the Main Camera to be above ground when in that mode. When the mode is **Floor**, the XR runtime will generally report the position of tracked devices relative to the player's real floor. Unity will automatically clear the height of the Camera Offset when in this mode since it is not necessary to artificially raise the tracking origin up. Set the mode to **Not Specified** to use the default mode of the XR runtime.

To have the position and rotation of the XR HMD update the Main Camera Transform, a Tracked Pose Driver (Input System) component is added. This component is configured to set the **Position Input** binding to `<XRHMD>/centerEyePosition`, the **Rotation Input** binding to `<XRHMD>/centerEyeRotation`, and the **Tracking State Input** binding to `<XRHMD>/trackingState`.

## Configure Interactor

An Interactor component controls how a GameObject interacts with Interactable components in the scene. There are multiple types of Interactors, one of which is an [XR Ray Interactor](xr-ray-interactor.md), a component that uses [ray casting](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html) in order to find valid Interactable objects in the scene.

To read input from an XR input device, the Interactor has various input properties that allow you to define the source of input. One example is the **Select Input** which allows you to set the input action that triggers selection. Refer to the property table in the documentation for [Interactor components](components.md#interactors) or refer to the tooltips in the Inspector window for more information.

To enable haptic vibration feedback for an interactor, add a [Simple Haptic Feedback](simple-haptic-feedback.md) component to the GameObject to trigger haptics in response to select and hover events. You can then specify intensities and durations of haptic feedback to play back on select and hover events in the Inspector window. That component references a [Haptic Impulse Player](haptic-impulse-player.md) component (which can be shared by different interactors) that is used to actually play the haptics to a device. On the Haptic Impulse Player, set **Haptic Output** to an input action with a binding path to any active control, such as `<XRController>{LeftHand}/*`, in order to identify the device. If you are using OpenXR, you can instead set the binding path to a `haptic` control.

The **UI Interaction** option controls whether this XR Ray Interactor can interact with Unity UI elements in a world space canvas in the scene. See [UI Setup](ui-setup.md) for more information and steps for enabling UI interactivity.

To have the position and rotation of a motion controller update the Transform component, a Tracked Pose Driver (Input System) component should be added. As an example for the left controller, this component should be configured to set the **Position Input** binding to `<XRController>{LeftHand}/devicePosition`, the **Rotation Input** binding to `<XRController>{LeftHand}/deviceRotation`, and the **Tracking State Input** binding to `<XRController>{LeftHand}/trackingState`.

## Enable input actions used for input

Actions must be enabled before they react to input. See [Enabling actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/Actions.html#enabling-actions) in the Input System documentation for details about this process. Most behaviors in this package have input properties which can either store an Input Action directly, or indirectly by referencing an input action contained in an Input Action Asset. When directly defined actions are used (in other words, the mode is set to **Input Action**), behaviors automatically enable and disable the actions that are directly defined during their own `OnEnable` and `OnDisable` events. This can be useful when doing rapid prototyping. However when indirect action references are used (in other words, the mode is set to **Input Action Reference**), behaviors intentionally don't automatically enable or disable the Input Actions that are indirectly defined to allow the enabled state to be managed externally.

The Input Action Manager component can be used to automatically enable or disable the Actions defined in an Input Action Asset during its own `OnEnable` and `OnDisable` events.

If you created the XR Origin using **GameObject &gt; XR &gt; XR Origin (VR)**, you will already have an Input Action Manager attached to the **XR Origin**, if not, use **GameObject &gt; Create Empty** and rename the GameObject Input Action Manager. Use **Component &gt; Input &gt; Input Action Manager** to add the component to the GameObject you created.

If you have imported the **Starter Assets** sample package, the **XRI Default Input Actions** should already be set in the **Action Assets** configuration for the component. If **Starter Assets** are not available or you are creating this object manually, select **Add (+)** or set **Size** to **1** in the Inspector window to add an element to the **Action Assets** list. Select the element's object picker (circle icon) and choose **XRI Default Input Actions**.

![input-action-manager](images/input-action-manager.png)

If you later create additional Input Action Assets, add them to the **Action Assets** list to enable all its actions also.

> [!NOTE]
> For Input Actions to read from input devices correctly while running in the Unity Editor, the Game view may need to have focus depending on the current project settings. If you find that your input, such as button presses on the controllers, are not working, ensure the Game view has focus by clicking it with your mouse. See [Background and focus change behavior](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/Devices.html#background-and-focus-change-behavior) to learn how to adjust settings to not require focus in the Game view.

<a id="create-grab-interactable"></a>
## Create an Interactable for the player to grab

Interactable components define how the user can interact with objects in a scene. To create a basic 3D cube that can be grabbed, use **GameObject &gt; XR &gt; Grab Interactable**.

For this example, create a plane for the cube to rest on so it does not fall out of reach. Use **GameObject &gt; 3D Object &gt; Plane**, then click on the Cube and click and drag the Transform gizmo to position it above the Plane GameObject.

In the screenshot below, the GameObject with the XR Grab Interactable supports grabbing, moving, dropping, and throwing with smoothed tracking.

![interactable-setup](images/interactable-setup.png)

> [!TIP]
> Interactables added through the **GameObject &gt; XR** menu use a Box Collider to detect interaction, but other types of Collider components such as a convex Mesh Collider can provide better hit detection at the cost of performance.

To configure an existing GameObject to make it an interactable object to allow the user to grab it, select it in your scene and add these components:
- Add **Component &gt; XR &gt; XR Grab Interactable**
- Add **Component &gt; Physics &gt; Box Collider**

## Enable an XR provider

Open **Edit &gt; Project Settings &gt; XR Plug-in Management**. If you do not yet have the package installed, click **Install XR Plugin Management** in that window. Select one or more of the plug-in providers for the device(s) you wish to target, such as [Oculus](https://docs.unity3d.com/Packages/com.unity.xr.oculus@latest/) or [Open XR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest/). Open XR will require additional configuration, see its package documentation for those steps.
