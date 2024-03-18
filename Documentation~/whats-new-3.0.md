# What's new in version 3.0

Summary of changes in XR Interaction Toolkit package version 3.0:

## Added

### Input Readers

The biggest change in XRI 3.0 comes in the form of a new [Input Reader](architecture.md#input-readers) architecture. The input readers allow a simplified, yet more sophisticated abstraction of input. Within this new architecture, it is possible to use legacy input, actions from the Input System package, manual manipulation (in Editor or via APIs), or custom scriptable objects for special cases or custom hardware platforms. Due to these changes, it became possible to simplify the input in such a way that the divergence in the old `XRBaseController` was no longer required and input could be embedded directly into the interactors, lowering code complexity and decreasing component count across GameObjects. To find out more about the new input readers, check out the `Demo Scene` in the [Starter Assets](samples-starter-assets.md#demo-scene). If you need to continue to use legacy input, there is a set of [Legacy Input Readers](samples-legacy-xr-input-readers.md) in the included sample packages as well.

> [!NOTE]
> Due to the nature of the changes introduced by the input readers, you may notice rolling changes to many of the other components throughout the XR Interaction Toolkit that reflect the usage of these new classes. You will likely want to upgrade your own project if you use Input Actions on any of your custom components.

### Near-Far Interactor and multi-casting

Another new feature of XRI 3.0 is the [Near-Far Interactor](near-far-interactor.md), which is an interactor capable of combining multiple types of physics casters. This allows a seamless transition when pulling objects closer to you from a distance or pushing them away, using the `SphereInteractionCaster` for near interaction and the `CurveInteractorCaster` for far interaction. Because casting occurs in both the near and far regions, this interactor replaces the need for using the XR Direct Interactor and the XR Ray Interactor. This modular new approach makes it easier to create and apply customized casters without having to extend or replace core components by simply implementing the new `IInteractionCaster` and `ICurveInteractionCaster` interfaces.

The [`InteractionAttachController`](interaction-attach-controller.md) is added by default along side the Near-Far Interactor to control the movement behavior of the interactable objects when selected at a distance. The default implementation allows for accelerated and smoothed motion by performing a pulling gesture towards the user, but custom movement behavior can be applied by implementing the IInteractionAttachController interface and replacing the component reference in the Near-Far Interactor.

The Near-Far Interactor also takes advantage of the new [Curve Visual Controller](curve-visual-controller.md), which is designed to provide a versatile and configurable controller for the Line Renderer component. It allows for more flexibility when displaying line-based visual feedback for interactions. The included [Starter Assets](samples-starter-assets.md) have been updated to utilize the new interactor and related components. For more information, please review the [Near Far Interactor](near-far-interactor.md) manual pages.

### XR Body Transformers and Locomotion Mediator

A new way to move and manipulate the XR Origin in response to input and interactions has been added. XR Body Transformers allow specific types of manipulation of the XR Origin and can be queued up for processing by the new `LocomotionMediator`. The original locomotion provider classes have been updated to use the new XR body transformations, simplifying the code and allowing for greater flexibility when extending the locomotion system as a whole. Along with the input changes, the original locomotion provider classes have also been updated or replaced to take full advantage of the new input architecture, streamlining the way locomotion is handled across the board. Read more about this in the [Locomotion](locomotion.md) documentation.

### Climb teleportation

On top of the locomotion mediator changes, climbing and teleportation has been enhanced to provide teleportation up and down ladders. In addition to simple endpoint-to-endpoint teleportation, [Teleportation multi-anchor volumes](teleportation-multi-anchor-volume.md) have been added. These allow multiple predefined teleport outlets per climbing surface which can be selected based on head or eye gaze (or any other method of determination). For more information about these new additions, please read the [Locomotion](locomotion.md) documentation and check out an example of how these updates are used in the `Demo Scene` of the [Starter Assets](samples-starter-assets.md#demo-scene) sample package.

## Changed

### Namespace changes

As a part of the move to XRI 3.0, a number of classes, structs, enums, and interfaces have been moved into namespaces that better reflect their use and the organizational structure they reside in. For a full list of namespace changes, please refer to our [changelog](../changelog/CHANGELOG.html).

## Deprecations

### Controller-based classes

With the new input changes outlined above, the controller-based classes are now obsolete and have been deprecated to reflect this. The functionality of the controller classes can be found in a combination of the standard `TrackedPoseDriver` as well the input properties on the individual interactors where that input makes sense.

### AR interactables and gesture interactor

With the introduction of `ARTransformer`, `ScreenSpaceRayInteractor`, and the associated input components for screen space interaction, the individual AR interactable classes and the `ARGestureInteractor` have been deprecated.

### Locomotion system and controller-based providers

The `LocomotionSystem` has been deprecated and replaced by the `LocomotionMediator` component. Along with this, and due to the input changes outlined above, the locomotion providers that depended on the previous input handling architecture have been deprecated and replaced with a simplified streamlined variant of each.

### Affordance system

The affordance system has also been deprecated in XRI 3.0. A new feedback system to handle visuals, audio, haptics and more will be coming soon in a future release of XRI.

For a full list of changes and updates in this version, see the [XR Interaction Toolkit package changelog](../changelog/CHANGELOG.html).