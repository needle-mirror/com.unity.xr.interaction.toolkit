---
uid: xri-locomotion
---
# Locomotion overview

The XR Interaction Toolkit package provides a set of locomotion primitives that offer the means to move around in a scene during an XR experience. These components include:
- An XR Origin that represents the center of the tracking space for the user's headset, controllers, hands, and other trackable devices or objects in the XR space
- An XR Body Transformer manages user locomotion via transformation of an XR Origin and applies queued Body Transformations every `Update`
- A Locomotion Mediator mediates user locomotion by providing Locomotion Providers with access to the XR Body Transformer linked to this behavior
- A Teleportation Provider and Teleportation Interactables
- A Snap Turn Provider that rotates the user by fixed angles
- A Continuous Turn Provider that smoothly rotates the user over time
- A Continuous Move Provider that smoothly moves the user over time
- A Grab Move Provider that moves the user counter to controller movement
- A Two-Handed Grab Move Provider that can move, rotate, and scale the user counter to controller movement
- A Climb Provider that moves the user while they are selecting a Climb Interactable

This documentation outlines how to use and extend these components.

## Architecture

The main components of the locomotion architecture are the [Locomotion Mediator](#locomotion-mediator), the various [Locomotion Providers](locomotion-providers.md), the [XR Body Transformer](#xr-body-transformer), and the [Body Transformations](#ixrbodytransformation) which are applied to the XR Body Transformer by the Locomotion Providers.

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

The [XR Origin](https://docs.unity3d.com/Packages/com.unity.xr.core-utils@latest?subfolder=/manual/xr-origin-reference.html) is available for transformation in a container class called [`XRMovableBody`](#xr-movable-body). The XR Movable Body can be transformed using the user's body as a frame of reference.


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

[`IXRBodyTransformation`](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.IXRBodyTransformation) is an interface for a transformation that can be applied to an XR Origin using the user's body as a frame of reference. The following classes are the current implementations of `IXRBodyTransformation`:

| **Class** | **Description** |
|---|---|
| [Delegate XR Body Transformation](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.DelegateXRBodyTransformation) | Body transformation that invokes a delegate when applied. |
| [XR Body Ground Positioning](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XRBodyGroundPosition) | Transformation that moves the target's origin transform such that the world position of where the user's body is grounded matches the specified position. |
| [XR Body Scale](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XRBodyScale) | Transformation that sets the uniform local scale of the target's origin transform to the specified value, and then repositions the XR Origin such that the world position of where the user's body is grounded remains the same. |
| [XR Body Yaw Rotation](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XRBodyYawRotation) | Transformation that rotates the target's origin transform by the specified amount about the axis aligned with the XR Origin's up vector and passing through the world position of where the user's body is grounded.  |
| [XR Camera Forward XZ Alignment](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XRCameraForwardXZAlignment) | Transformation that rotates the target's origin transform about the axis aligned with the XR Origin's up vector and passing through the world position of the camera, such that the projection of the camera's forward vector onto the XR Origin's XZ plane matches the projection of the specified vector onto the XR Origin's XZ plane. |
| [XR Origin Movement](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XROriginMovement) | Transformation that translates the target's origin transform by the specified amount. |
| [XR Origin Up Alignment](xref:UnityEngine.XR.Interaction.Toolkit.Locomotion.XROriginUpAlignment) | Transformation that rotates the target's origin transform such that its up vector matches the specified vector. This does not maintain the world position of the user's body. |
