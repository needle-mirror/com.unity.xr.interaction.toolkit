---
uid: xri-whats-new-3-7
---
# What's new in version 3.7

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.7:

This release primarily focuses on support for interactables whose colliders change at runtime, such as UI Toolkit world space panels, along with reticle support for curve-based interactor visuals and various sample improvements and bug fixes.

## Added

### XR Base Interactable: Runtime collider registration

[`XRBaseInteractable`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable) now supports adding and removing colliders after the interactable has already been enabled and registered with the [XR Interaction Manager](xref:xri-xr-interaction-manager). The new [`RegisterCollider`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.RegisterCollider(UnityEngine.Collider)) and [`UnregisterCollider`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.UnregisterCollider(UnityEngine.Collider)) methods add or remove a single collider, [`RefreshColliders`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.RefreshColliders(System.Boolean)) rebuilds the list from the colliders currently found on the GameObject and its children, and the new `collidersChanged` event (with [`CollidersChangedEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.CollidersChangedEventArgs)) notifies other components when the list changes.

For [world space UI built with UI Toolkit](xref:xri-ui-world-space-ui-toolkit-support), colliders created dynamically by a `UIDocument` are registered automatically when an [`XRUIToolkitManager`](xref:UnityEngine.XR.Interaction.Toolkit.UI.XRUIToolkitManager) is present in the scene, so panels whose colliders are generated or replaced at runtime now work with interactors without any additional scripting.

Because the collider list is now managed at runtime, editing the **Colliders** list in the Inspector is blocked during Play mode while the interactable is active.

### Curve Visual Controller: Reticle support

The [Curve Visual Controller](xref:xri-curve-visual-controller) can now display a reticle at the end of the curve, and it implements [`IXRCustomReticleProvider`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.IXRCustomReticleProvider) so it can receive custom reticles supplied by interactables. This brings reticle support to interactors that use curve visuals, such as the [Near-Far Interactor](xref:xri-near-far-interactor), without needing a separate reticle component. This brings the Near-Far Interactor closer to parity with some of the features utilized by the `XRRayInteractor` and `XRInteractorLineVisual` components.

A new `BlockedReticleSwapper` component was also added to the [Starter Assets](xref:xri-samples-starter-assets) sample. It swaps the reticle on a Curve Visual Controller when the interactor is hovering an interactable that it cannot select, giving users a visual cue that selection is blocked.

### Samples: Hand support and teleport pad UI

The [Spatial Keyboard](xref:xri-samples-spatial-keyboard) sample now supports hands. It depends on the [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample, which contains the XR Origin prefab configured for hands, so both samples must be imported to use the hand-based scenes.

UI buttons were added to the teleport pads in the [Starter Assets](xref:xri-samples-starter-assets), [Spatial Keyboard](xref:xri-samples-spatial-keyboard), and [World Space UI](xref:xri-samples-world-space-ui) samples, making it easier to move between sample scenes and demonstration areas, especially when relying on only hand tracking for locomotion.

## Changed

### XR UI Toolkit Manager: Update method for dynamic colliders

[`XRUIToolkitManager`](xref:UnityEngine.XR.Interaction.Toolkit.UI.XRUIToolkitManager) now implements `MonoBehaviour.Update` to monitor dynamic collider registrations. The manager detects colliders destroyed externally, for example when a `UIDocument` is disabled and re-enabled, and re-registers them when they return. Panels with **Collider Update Mode** set to **Keep existing colliders (if any)** in their Panel Settings are skipped.

If you have a derived class that implements `Update`, you must call `base.Update()` for dynamic collider registration to function.


### XR Poke Filter: Runtime collider behavior updates

The [XR Poke Filter](xref:xri-xr-poke-filter) now automatically re-enables itself when a collider is added to an interactable that previously had none. Before this change, a filter that disabled itself at initialization due to a missing collider would stay disabled for the lifetime of the object. This was preventing poke interaction with interactables whose colliders are created later, such as UI Documents.

It also now stays subscribed to the interactable's `collidersChanged` event and updates its collider reference when colliders are replaced at runtime. It also recomputes the poke interaction depth at the start of each poke so that changes to collider bounds are taken into account. Related to this, the poke interaction axis length is now clamped to at least half of the collider's size along the poke direction, which ensures thin colliders with center offsets produce a valid poke depth. Colliders with zero depth along the poke direction still return a depth of `0`.
