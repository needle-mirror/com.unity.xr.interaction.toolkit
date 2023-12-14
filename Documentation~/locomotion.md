# Locomotion

The XR Interaction Toolkit package provides a set of locomotion primitives that offer the means to move around in a scene during an XR experience. These components include:
- An XR Origin that represents the center of the tracking space for the user's headset, controllers, hands, and other trackable devices or objects in the XR space
- An XR Body Transformer manages user locomotion via transformation of an XR Origin and applies queued Body Transformations every `Update`
- A Locomotion Mediator mediates user locomotion by providing Locomotion Providers with access to the XR Body Transformer linked to this behavior
- A Teleportation Provider and Teleportation Interactables
- A Snap Turn Provider that rotates the user by fixed angles
- A Continuous Turn Provider that smoothly rotates the user over time
- A Continuous Move Provider that smoothly moves the user over time
- A Grab Move Provider that moves the user counter to controller movement
- A Two Handed Grab Move Provider that can move, rotate, and scale the user counter to controller movement
- A Climb Provider that moves the user while they are selecting a Climb Interactable

This documentation outlines how to use and extend these components.

## Glossary

| **Term** | **Meaning** |
|---|---|
| **XR Origin** | The component that implements the generic concept of a camera rig. It also provides options of tracking origin modes to configure the reference frame for positions reported by the XR device. It has properties to specify an Origin GameObject, a Camera Floor Offset GameObject, and a Camera. |
| **Origin** | By default, the Origin is the GameObject that the XR Origin component is attached to, and the term is generally used interchangeably with XR Origin. This is the GameObject that the application will manipulate via locomotion. |
| **Camera Floor Offset GameObject** | The GameObject to move the Camera to the desired height off the floor depending on the tracking origin mode. |
| **Camera** | The GameObject that contains a Camera component. This is usually the main camera that renders what the user sees. It is the head of XR rigs. |
| **Floor mode** | A floor-relative tracking origin mode. Input devices will be tracked relative to a location on the user's floor. |
| **Device mode** | A device-relative tracking origin mode. Input devices will be tracked relative to the first known location. The Camera is moved to the height set by the Camera Y Offset value by moving the Camera Floor Offset GameObject. |
| **Locomotion Mediator** | Provides `LocomotionProvider`s with access to the `XRBodyTransformer` linked to the `LocomotionMediator` and manages the `LocomotionState` for each provider based on its request for the `XRBodyTransformer`. |
| **XR Body Transformer** | Manages user locomotion via transformation of an `XROrigin` and applies queued transformations during `Update`. |
| **Locomotion Provider** | The base class for various locomotion implementations, such as teleportation and turning. |
| **Teleportation** | A type of locomotion that teleports the user from one position to another position. |
| **Snap Turn** | A type of locomotion that rotates the user by a fixed angle. |
| **Continuous Turn** | A type of locomotion that smoothly rotates the user by an amount over time. |
| **Continuous Move** | A type of locomotion that smoothly moves the user by an amount over time. |
| **Grab Move** | A type of locomotion that moves the user counter to controller movement, as if the user is grabbing the world around them. |
| **Climb** | A type of locomotion that moves the user counter to Interactor movement while the user is selecting a Climb Interactable. |

## Set up a basic scene for snap turn and teleportation

Before you follow the steps below, to streamline setup of components, it is recommended that you install the [Starter Assets](samples-starter-assets.md) sample and follow the steps for [Configuring Preset Manager defaults](samples-starter-assets.md#configuring-preset-manager-defaults) to reduce the burden of configuring the input actions. Additionally the Starter Assets contain preconfigured XR Origin, teleportation area, and teleportation anchor prefabs.

### 1. Set up the XR Origin and input actions

Follow the steps in General setup to [Create the XR Origin camera rig for tracked devices](general-setup.md#create-the-xr-origin-camera-rig-for-tracked-devices).

For setups that use input actions for input, make sure the **Input Action Manager** component is added to a GameObject (such as the XR Origin GameObject). This will enable all input actions in the referenced asset so that the inputs can be read correctly by the locomotion components.

If you installed the Starter Assets sample as recommended, you can add the **XRI Default Input Actions** from the Samples folder to the **Action Assets** field of Input Action Manager. Alternatively, you can create and use your customized Input Actions and controller bindings.

![input-action-manager](images/input-action-manager.png)

### 2. Add snap turn and teleportation capabilities

On the **XR Origin** GameObject, add a **Locomotion Mediator**, a **Snap Turn Provider**, and a **Teleportation Provider**.

To set up snap turn, you need to configure the [Snap Turn Provider](#snap-turn-provider) in the Inspector.

Set **Left Hand Snap Turn Input** and/or **Right Hand Snap Turn Input** to Vector 2 Control Type Actions with bindings for your desired inputs. The Actions that you assign should use either the **XR Controller (LeftHand)** or **XR Controller (RightHand)** binding paths.

![locomotion-setup-xr-rig-components](images/locomotion-setup-xr-origin-components.png)

### 3. Create teleportation interactables

From Unity's main menu, click **GameObject &gt; XR &gt; Teleportation Area** or **GameObject &gt; XR &gt; Teleportation Anchor** to create a plane that can be teleported to. a Teleportation Area teleports users to their pointed location on its child collider, whereas a Teleportation Anchor specifies a pre-determined position and/or rotation in addition to the Teleportation Area.

If you followed steps 1-3, you should have a basic scene with the ability to perform snap turn and teleportation with your controllers. The following steps provide additional details on changing the visuals of the **XR Ray Interactor**.

### 4. Configure line type

The [XR Ray Interactor](xr-ray-interactor.md) was added by default to the **Left Controller** and **Right Controller** GameObjects when creating the XR Origin from the menu. Under its **Raycast Configuration** includes three default options of **Line Type** that can be used to select interactables:

* **Straight Line**
* **Projectile Curve**
* **Bezier Curve**

These options are described below.

#### Straight Line

![raycast-configuration-straight-line](images/raycast-configuration-straight-line.png)

If you select the **Straight Line** option, the XR Ray Interactor performs a single ray cast into the scene with a ray length set by the **Max Raycast Distance** property. The image above shows the configuration options.

| **Property** | **Description** |
|---|---|
| **Max Raycast Distance** | The distance to be ray cast into the scene. |

#### Projectile Curve

If you select the **Projectile Curve** option, the XR Ray Interactor samples the trajectory of a projectile to generate a projectile curve. You can use the angle of the Controller to control the distance of the landing point. When you lift your Controller, the landing point first goes further away, then comes closer if you keep lifting the Controller.

The **Projectile Curve** option is recommended for use in teleportation scenarios.

![raycast-configuration-projectile-curve](images/raycast-configuration-projectile-curve.png)

| **Property** | **Description** |
|---|---|
| **Reference Frame** | The reference frame of the projectile. If you don't set this, the XR Ray Interactor attempts to use the local XR Origin, which makes the curve always go up then down in the tracking space. If the XR Origin doesn't exist, the curve rotates with the Controller. |
| **Velocity** | Initial velocity of the projectile. Increase this value to make the curve reach further. |
| **Acceleration** | Gravity of the projectile in the reference frame. |
| **Additional FlightTime** | Additional flight time after the projectile lands. Increase this value to make the endpoint drop lower in height. |
| **Sample Frequency** | The number of sample points of the curve. Higher numbers offer better quality. |

#### Bezier Curve

![raycast-configuration-bezier-curve](images/raycast-configuration-bezier-curve.png)

In addition to its start point, the **Bezier Curve** uses a control point and an end point. The start point is the position of the **Attach Transform** of the **XR Ray Interactor**. Both the **Projectile Curve** and the **Bezier Curve** use the reference frame of the Origin unless otherwise set by the user.

| **Property** | **Description** |
|---|---|
| **End Point Distance** | Define how far away the end point is from the start point. Increase the value to increase the distance. |
| **End Point Height** | Define how high the end point is in relation to the start point. Increase this value to increase the height. |
| **Control Point Distance** | Define how far away the peak of the curve is from the start point. Increase this value to increase the distance. |
| **Control Point Height** | Define how high the peak of the curve is in relation to the start point. Increase this value to increase the height. |
| **Sample Frequency** | Define the number of sample points the curve has. Higher numbers offer better quality. |

### 5. Set line visual

The XR Interactor Line Visual provides additional options to customize the appearance of the XR Ray Interactor for teleportation and other interactions. It requires the [Line Renderer](https://docs.unity3d.com/Manual/class-LineRenderer.html) component and uses line points from the XR Ray Interactor.

![xr-interactor-line-visual](images/xr-interactor-line-visual.png)

| **Property** | **Description** |
|---|---|
| **Line Width** | The width of the line, in centimeters. |
| **Width Curve** | The relative width of the line from start to end. |
| **Valid Color Gradient** | When the line hits any collider of a valid target, it changes to this color gradient. |
| **Invalid Color Gradient** | When the line hits an invalid target, it changes to this color gradient. |
| **Override Line Length** | If you enable this option, the line visual can have a different length from the underlying ray cast. |
| **Line Length** | If the **Override Line Length** option is enabled, this field sets the rendered length of the line. This length can't be longer than the ray cast distance. |
| **Smooth Movement** | If enabled, the rendered line is delayed from and smoothly follows the ray cast line. |
| **Reticle** | The GameObject to visualize the destination of Teleportation. |

## Architecture

The main components of the locomotion architecture are the [Locomotion Mediator](#locomotion-mediator), the various [Locomotion Providers](#locomotion-providers), the [XR Body Transformer](#xr-body-transformer), and the [Body Transformations](#ixrbodytransformation) which are applied to the XR Body Transformer by the Locomotion Providers. 

Locomotion Providers contain a reference to the Locomotion Mediator, and the Locomotion Mediator in turn provides the Locomotion Providers with access to the XR Body Transformer. The Locomotion Mediator also maintains the [Locomotion State](#locomotion-state) for the Locomotion Providers making transformation requests.

The Locomotion Providers calculate the appropriate transformation and upon receiving access to the XR Body Transformer, the provider attempts to queue the Body Transformation with the XR Body Transformer.

The XR Body Transformer queues the transformation and applies each transformation to the [XR Movable Body](#xr-movable-body) on `Update`.

The overall flow of a Locomotion request is as follows:

1. The Locomotion Provider computes the desired transformation.
2. The Locomotion Provider requests to try to prepare or start locomotion from the Locomotion Mediator.
2. The Locomotion Mediator checks to see if the locomotion request is possible at the current time.
3. Upon success, the Locomotion Mediator updates the Locomotion Provider with the necessary XR Body Transformer.
4. The Locomotion Provider tries and queues the desired transformation with the XR Body Transformer. 
5. The XR Body Transformer adds the transformation to the queue based on the priority of the transformation.
6. On `Update`, the XR Body Transformer applies each transformation that is queued.

### XR Origin

The XR Origin is available for transformation in a container class called [`XRMovableBody`](#xr-movable-body). The XR Movable Body can be transformed using the user's body as a frame of reference.

The image below shows the XR Origin component.

![xr-origin](images/xr-origin.png)

| **Property** | **Description** |
|---|---|
|**Origin Base Game Object**|Indicates which GameObject acts as the Transform from tracking space into world space. In the recommended hierarchy, this is the XR Origin GameObject.|
|**Camera Floor Offset GameObject**|Sets which GameObject has a vertical offset applied if the device tracking origin doesn't contain the user's height.|
|**Camera**|Indicates which GameObject holds the user's Camera. This is important because the user's Camera might not be at the origin of the tracking volume. In the suggested hierarchy, this is the Main Camera GameObject.|
|**Tracking Origin Mode**|Sets the desired tracking origin used by the application.|
|**Camera Y Offset**| The number of world space units by which the GameObject specified by the **Camera Floor Offset GameObject** is moved up vertically if the device tracking origin doesn't contain the user's height.|

### XR Movable Body

The XR Movable Body is a container for the XR Origin that can be transformed using the user's body as a frame of reference. The XR Movable Body contains a reference to an XR Origin and the XR Origin transform and is instantiated by the XR Body Transformer.

Additionally, the XR Movable body utilizes a Body Position Evaluator, which determines the position of the user's body, and a Constrain Manipulator, which can be used to perform movement that is constrained by collision.

### Locomotion Mediator

The [Locomotion Mediator](locomotion-mediator.md) component is a key part of locomotion, mediating transformation requests from Locomotion Providers, giving Locomotion Providers access to the XR Body Transformer, and managing the Locomotion State of the Locomotion Providers.

The Locomotion Mediator gets the XR Body Transformer component on `Awake` to prepare for Locomotion Provider requests. 

When Locomotion Providers call `LocomotionMediator.TryPrepareLocomotion`, the Locomotion Mediator will add the Locomotion Provider to a provider data map for future processing and update the Locomotion Provider's Locomotion State to `LocomotionState.Preparing` in the data map. Once the provider is in `LocomotionState.Preparing` the Locomotion Mediator will transition the provider to `LocomotionState.Moving` during the next `LocomotionMediator.Update` where `LocomotionProvider.canStartMoving` is true.

Similarly, when Locomotion Providers call `LocomotionMediator.TryStartLocomotion`, the Locomotion Mediator will add the Locomotion Provider to a provider data map for future processing and update the Locomotion Provider's Locomotion State to `Locomotion.Moving` in the data map. The Locomotion Mediator will then call `LocomotionProvider.OnLocomotionStart(IXRBodyTransformer)` to grant that Provider access to the XR Body Transformer. Note, `LocomotionMediator.TryStartLocomotion` is usually called from `LocomotionMediator.TryStartLocomotionImmediately`, which will bypass the `LocomotionState.Preparing` state and not check `LocomotionProvider.canStartMoving`.

Lastly, when the locomotion is complete, the Locomotion Providers call `LocomotionMediator.TryEndLocomotion`. The Locomotion Mediator will check if the Locomotion State of that provider is still active. If it is no longer active, the Locomotion Mediator will update the Locomotion Provider's Locomotion State to `LocomotionState.Ended` and calls `LocomotionProvider.OnLocomotionEnd()` on the locomotion provider.

#### Locomotion State

Locomotion State is a replacement for the deprecated Locomotion Phase. It represents the state of locomotion for any given Locomotion Provider.

| **Property** | **Description** |
|---|---|
| **Idle** | Locomotion state where the Locomotion Provider is idle, before locomotion input occurs. |
| **Preparing** | Locomotion state where the Locomotion Provider is getting ready to move, when locomotion start input occurs. |
| **Moving** | Locomotion state where the Locomotion Provider is queuing XR Body Transformations with the XR Body Transformer. |
| **Ended** | Locomotion state where the Locomotion Provider is no longer moving, after locomotion end input has completed. |

### XR Body Transformer

The [XR Body Transformer](xr-body-transformer.md) component manages user locomotion via transformations of an XR Origin. 

Locomotion Providers that have gained access to the XR Body Transformer via the Locomotion Mediator can call `XRBodyTransformer.QueueTransformation(IXRBodyTransformation)` to queue the transformation to be applied next `Update`. Transformations are applied sequentially based on ascending priority and transformations with the same priority are applied in the order they were queued. Each transformation is removed from the queue after it is applied.

### IXRBodyTransformation

`IXRBodyTransformation` is an interface for a transformation that can be applied to an XR Origin using the user's body as a frame of reference. The following are the current implementations of of `IXRBodyTransformation`

| **Class** | **Description** |
|---|---|
| **Delegate XR Body Transformation** | Body transformation that invokes a delegate when applied. |
| **XR Body Ground Positioning** | Transformation that moves the target's origin transform such that the world position of where the user's body is grounded matches the specified position. |
| **XR Body Scaling** | Transformation that sets the uniform local scale of the target's origin transform to the specified value, and then repositions the Origin such that the world position of where the user's body is grounded remains the same. |
| **XR Body Yaw Rotation** | Transformation that rotates the target's origin transform by the specified amount about the axis aligned with the Origin's up vector and passing through the world position of where the user's body is grounded.  |
| **XR Camera Forward XZ Alignment** | Transformation that rotates the target's origin transform about the axis aligned with the Origin's up vector and passing through the world position of the camera, such that the projection of the camera's forward vector onto the Origin's XZ plane matches the projection of the specified vector onto the Origin's XZ plane. |
| **XR Origin Movement** | Transformation that translates the target's origin transform by the specified amount. |
| **XR Origin Up Alignment** | Transformation that rotates the target's origin transform such that its up vector matches the specified vector. This does not maintain the world position of the user's body. |

### Locomotion Providers

Locomotion Providers implement different types of locomotion. The package supplies multiple Locomotion Providers: the [Teleportation Provider](#teleportation-provider), the [Snap Turn Provider](#snap-turn-provider), the [Continuous Turn Provider](#continuous-turn-provider), the [Continuous Move Provider](#continuous-move-provider), [Grab Move Providers](#grab-move-providers), and the [Climb Provider](#climb-provider), all of which implement the `LocomotionProvider` abstract class. These are discussed in more detail in the sections below.

Locomotion Providers contain a reference to [Locomotion Mediator](#locomotion-mediator), which gives the Locomotion Provider access to the [XR Body Transformer](#xr-body-transformer). If the Locomotion Mediator is `null`, the Locomotion Provider will attempt to find one.

Once the Locomotion Provider has calculated the transformation, the provider is ready to begin locomotion.

Use `TryPrepareLocomotion` to communicate with the Locomotion Mediator and attempt to transition the provider into the `LocomotionState.Preparing` state. If this succeeds, then the provider can enter the `LocomotionState.Moving` by waiting for the mediator's next `Update` in which the provider's `canStartMoving` is `true`. This will provide the Locomotion Provider with access to the XR Body Transformer.

Alternatively, use `TryStartLocomotionImmediately` to attempt to transition the provider into the `LocomotionState.Moving` state and to gain access to the required XR Body Transformer immediately. Note, `TryStartLocomotionImmediately` will bypass `LocomotionState.Preparing` state and the `LocomotionProvider.canStartMoving` check.

When the Locomotion Provider receives to the XR Body Transformer, the provider can try to queue the desired transformation with the XR Body Transformer. See [XR Body Transformer](#xr-body-transformer) for a more thorough explanation of the queueing process.

Upon completion of the locomotion, use `TryEndLocomotion` to have the Locomotion Mediator attempt to transition the Locomotion Provider to the `LocomotionState.Ended`. The Locomotion Mediator will call `LocomotionProvider.OnLocomotionEnd()`.

The `LocomotionProvider` abstract class also providers two events:
* `locomotionStarted` is invoked once the provider successfully enters the `LocomotionState.Moving` state.
* `locomotionEnded` is invoked once the provider successfully enters the `LocomotionState.Ended` state.

### Teleportation

The package provides a simple implementation of teleportation that also demonstrates how to implement complex locomotion scenarios using the `LocomotionProvider`.

The Teleportation Provider inherits from the `LocomotionProvider` abstract class. The Teleportation Provider is responsible for moving the Origin to the desired location on the user's request.

This implementation has several types of teleportation destinations: a Teleportation Area, a Teleportation Anchor, and a Teleportation Multi-Anchor Volume. These are discussed in more detail below. In short:

- Teleportation Areas allow the user to choose a location on a surface that they wish to teleport to.

- Teleportation Anchors teleport the user to a pre-determined specific position and/or rotation that they specify. Technically, it functions like the Teleportation Area but has the additional anchor functionality.

- Teleportation Multi-Anchor Volumes allow the user to target a volume of space to teleport to one of several anchor locations, based on custom filtering logic.

All types of teleportation destinations are implemented on top of the XR Interaction system using the `BaseTeleportationInteractable` as the starting point for shared code.

The XR Interaction system also provides various line rendering options. For more information, see documentation for the [XR Interactor Line Visual](xr-interactor-line-visual.md) and the [XR Interactor Reticle Visual](xr-interactor-reticle-visual.md).

#### Teleportation Provider

The [Teleportation Provider](teleportation-provider.md) component implements the `LocomotionProvider` abstract class. You can have as many instances of the Teleportation Provider component in your scene as you need. However, in most cases, a single instance is enough.

The **Mediator** field should reference the Locomotion Mediator MonoBehaviour that you want the teleportation provider to interact with. If you don't specify a Locomotion Mediator, the provider attempts to find one in the current scene.

#### Teleportation Area Interactable

The [Teleportation Area](teleportation-area.md) component is a specialization of the `BaseTeleportationInteractable` class. It allows the user to select any location on the surface as their destination.

The Teleportation Area Interactable is intended to be used by the XR Ray Interactor or any of its specializations. It uses the intersection point of the ray and the area's collision volume to determine the location that the user wants to teleport to. It can also optionally match the user's rotation to the forward direction of the attach transform of the selecting Interactor. The Teleportation Area Interactable has a specialized implementation of the `GenerateTeleportRequest` method, which generates a teleportation request that is queued with the Teleportation Provider.

**Match Orientation** is used to specify how the rotation of the XR Origin changes when teleporting.
- If your application does not rotate the Origin in any way, and you always want the Origin's up vector to match World Space's Up vector, use the **World Space Up** option.
- If you want the user to be able to stand on a ceiling, wall, or other tilted surface, and have them rotate to match so that the ceiling or wall feels like their new floor, select **Target Up** instead. The Origin will match the up vector of the Transform that the Teleportation Area component is attached to.
- If you want to point the user in a very specific direction when they arrive at a target, select **Target Up And Forward**. This will match the Origin's rotation to the exact rotation of the Transform that a Teleportation Area is attached to.
- If you do not want a teleport to change the rotation in any way, and you want the user to retain the same rotation before and after a teleport, select **None**.  If your entire application is oriented at a 45 degree angle, for instance, you can rotate the Origin's root Transform and set all teleport targets to `MatchOrientation.None`.

#### Teleportation Anchor Interactable

The [Teleportation Anchor](teleportation-anchor.md) component is a specialization of the `BaseTeleportationInteractable` class that allows the user to teleport to an anchor location by selecting the anchor or an area around it.

The Teleportation Anchor Interactable is intended to be used by the XR Ray Interactor or any of its specializations. It uses the intersection point of the ray and the area's collision volume to determine the location that the user wants to teleport to. It can also optionally match the user's rotation to the forward direction of the attach transform of the selecting Interactor. The Teleportation Anchor Interactable has a specialized implementation of the `GenerateTeleportRequest` method, which generates a teleportation request that is queued with the Teleportation Provider.

The **Teleport Anchor Transform** field defines the transform that the Origin teleports to when the user teleports to this anchor. It uses both the position and the rotation of the anchor, depending on which **Match Orientation** is selected.

#### Teleportation Multi-Anchor Volume Interactable

The [Teleportation Multi-Anchor Volume](teleportation-multi-anchor-volume.md) component is a specialization of the `BaseTeleportationInteractable` class that allows the user to teleport to one of several anchor locations by selecting the entire volume.

The volume uses custom filtering logic to choose the best destination anchor. By default it uses whichever anchor is furthest from the user, which is useful for easily getting to either end of a climbable object like a ladder. To change the filtering logic, you can assign any instance of an implementation of the `ITeleportationVolumeAnchorFilter` interface to the volume.

Note that Teleportation Multi-Anchor Volume uses pure transforms to define its anchors. It does not use Teleportation Anchor interactables at all.

### Snap Turn Provider

The package provides a [Snap Turn Provider](snap-turn-provider.md) component. A snap turn means the Origin rotates by a fixed amount when the application receives a configured input (for example, a thumbstick is tilted to the left).

### Continuous Turn Provider

The package provides a [Continuous Turn Provider](continuous-turn-provider.md) component. Continuous turning, as opposed to snap turning by discrete angles, smoothly rotates the Origin by an amount over time when the application receives a configured input (for example, a thumbstick is tilted to the left).

### Continuous Move Provider

The package provides a [Continuous Move Provider](continuous-move-provider.md) component. Continuous moving, as opposed to teleporting, smoothly translates the Origin by an amount over time when the application receives a configured input (for example, a thumbstick is tilted forward).

The **Forward Source** can be used to define which direction the Origin should move when, for example, pushing forward on a thumbstick. By default, it will use the Camera Object, meaning the user will move forward in the direction they are facing. An example of how this property can be used is to set it to a Transform that tracks the pose of a motion controller to allow the user to move forward in the direction they are holding the controller.

If a [Character Controller](https://docs.unity3d.com/Manual/class-CharacterController.html) is present on the Origin, this Continuous Move Provider will move the Origin using [`CharacterController.Move`](https://docs.unity3d.com/ScriptReference/CharacterController.Move.html) rather than directly translating the Transform of the Origin.

### Grab Move Providers

The package provides a [Grab Move Provider](grab-move-provider.md) and a [Two-Handed Grab Move Provider](two-handed-grab-move-provider.md). A grab movement translates the Origin counter to controller movement while a button input is held. This allows the user to move as if grabbing the whole world around them.

If a [Character Controller](https://docs.unity3d.com/Manual/class-CharacterController.html) is present on the Origin, the Grab Move Provider or Two Handed Grab Move Provider will move the Origin using [`CharacterController.Move`](https://docs.unity3d.com/ScriptReference/CharacterController.Move.html) rather than directly translating the Transform of the Origin.

### Climb Locomotion

The package provides the ability to do climb locomotion, which allows the user to climb an Interactable such as a ladder. Climb locomotion translates the Origin counter to movement of whichever Interactor is selecting a Climb Interactable. If multiple Interactors are selecting a Climb Interactable, only the most recent selection will drive movement. This type of locomotion is similar to [grab movement](#grab-move-providers) but uses Interactables to restrict locomotion.

Climb locomotion settings can be configured at the Provider level or overridden by the specific Climb Interactable being climbed. Settings can be configured to restrict movement along any of the local axes of the Interactable.

#### Climb Provider

The package provides a [Climb Provider](climb-provider.md) component. This is the component that handles the actual movement of the XR Origin.

#### Climb Interactable

The package provides a [Climb Interactable](climb-interactable.md) component that implements the `XRBaseInteractable` abstract class. This component allows you to make an object climbable by using the selection interaction to drive movement. You can optionally restrict movement so that the user is unable to move along certain local axes of the object.

## Document revision history

|Date|Reason|
|---|---|
|**December 7, 2023**| Documentation updated to reflect deprecation of Locomotion System and introduction of Locomotion Mediator and XR Body Transformer in version 3.0.0.|
|**May 19, 2023**| Documentation updated to match changes made in `com.unity.xr.core-utils` version 2.2.1.|
|**February 14, 2022**| Documentation updated to match package version 2.0.0.|
|**May 12, 2021**|Documentation updated for changes to tracking origin mode on the XR Origin. Matches package version 1.0.0-pre.4.|
|**January 10, 2020**|Documentation fixes, adds revision history.|
|**October 20, 2020**|Added continuous locomotion and updated for Inspector changes. Matches package version 0.10.0.|
