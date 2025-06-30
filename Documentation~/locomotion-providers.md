---
uid: xri-locomotion-providers
---

# Locomotion Providers overview

Locomotion Providers implement different types of locomotion.

The toolkit supplies [multiple Locomotion Providers](locomotion-providers-landing.md), which implement the `LocomotionProvider` abstract class.

Locomotion Providers contain a reference to the [Locomotion Mediator](locomotion-mediator.md) component, which gives the Locomotion Provider access to the [XR Body Transformer](xr-body-transformer.md) component. If the Locomotion Mediator is `null`, the Locomotion Provider will attempt to find one.

Once the Locomotion Provider has calculated the transformation, the provider is ready to begin locomotion.

Use `TryPrepareLocomotion` to communicate with the Locomotion Mediator and attempt to transition the provider into the `LocomotionState.Preparing` state. If this succeeds, then the provider can enter the `LocomotionState.Moving` by waiting for the mediator's next `Update` in which the provider's `canStartMoving` is `true`. This will provide the Locomotion Provider with access to the XR Body Transformer.

Alternatively, use `TryStartLocomotionImmediately` to attempt to transition the provider into the `LocomotionState.Moving` state and to gain access to the required XR Body Transformer immediately. Note, `TryStartLocomotionImmediately` will bypass `LocomotionState.Preparing` state and the `LocomotionProvider.canStartMoving` check.

When the Locomotion Provider transitions into the `LocomotionState.Moving` state, the provider can begin trying to queue the desired transformation with the XR Body Transformer. See the [Locomotion overview](locomotion.md#xr-body-transformer) XR Body Transformer section for a more thorough explanation of the queueing process.

Upon completion of the locomotion, use `TryEndLocomotion` to have the Locomotion Mediator attempt to transition the Locomotion Provider to the `LocomotionState.Ended`. The Locomotion Mediator will call `LocomotionProvider.OnLocomotionStateChanging` if successful.

The `LocomotionProvider` abstract class also provides public events:
* `locomotionStarted` is invoked once the provider successfully enters the `LocomotionState.Moving` state.
* `locomotionEnded` is invoked once the provider successfully enters the `LocomotionState.Ended` state and all of the provider's queued transformations have been applied, if any.
* `locomotionStateChanged` is invoked immediately upon the provider transitioning `LocomotionState`.
* `beforeStepLocomotion` is invoked just before the provider's queued transformations are applied.
* `afterStepLocomotion` is invoked just after the provider's queued transformations are applied.
