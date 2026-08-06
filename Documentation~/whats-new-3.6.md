---
uid: xri-whats-new-3-6
---
# What's new in version 3.6

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.6:

This release primarily focuses on under-the-hood improvements to the XR Interaction Simulator and various bug fixes.

## Added

### XR Interaction Simulator: State class and Device Aim Camera Radius

A new [`XRInteractionSimulatorState`](xref:UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRInteractionSimulatorState) class has been added that consolidates the tracking of the current state of the `XRInteractionSimulator`. This includes which device is being manipulated, the current input mode, and the current hand expression for each hand. Previously, this information was spread across several individual properties on the simulator itself. Additionally, a new **Device Aim Camera Radius** property (in meters) was added to control how closely a manipulated device can face the camera, helping prevent erratic rotation behavior when interacting with objects whose colliders intersect with the camera.

### ICurveInteractionCaster: Raycast Mask property

The [`ICurveInteractionCaster`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.Casters.ICurveInteractionCaster) interface now exposes a [`raycastMask`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.Casters.ICurveInteractionCaster.raycastMask) property, allowing you to get and set the raycast layer mask without having to cast to the concrete [`CurveInteractionCaster`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.Casters.CurveInteractionCaster) type. This is implemented as a [default interface member](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface#default-interface-members), so custom implementations of this interface may need to implement this property if their `LayerMask` property name differs.

## Changed

### XR Interaction Simulator: State property deprecations

Several individual state properties on `XRInteractionSimulator` have been deprecated in favor of reading from the new `currentState` property. These properties continue to function but will be removed in a future version. Refer to the [changelog](xref:xri-changelog) for the full list of deprecated properties.

### UIInputModule: Event camera prioritization

The [`UIInputModule`](xref:UnityEngine.XR.Interaction.Toolkit.UI.UIInputModule) now prioritizes the event camera from the canvas's raycaster over the main camera (`Camera.main`) when resolving screen-space coordinate conversions. This ensures correct behavior for world-space canvases that use a camera other than the main camera, such as composition layer canvases.

## Fixed

### XR Direct Interactor and XR Socket Interactor: OnTriggerStay support

Fixed [XR Direct Interactor](xref:xri-xr-direct-interactor) and [XR Socket Interactor](xref:xri-xr-socket-interactor) so they can be used when the **Generate On Trigger Stay Events** option is disabled in **Edit** > **Project Settings** > **Physics** > **Settings** for improved performance. Previously, these interactors relied exclusively on `OnTriggerStay` to track overlapping colliders. The interactors can now solely use `OnTriggerEnter` and `OnTriggerExit` to maintain a list of active trigger collisions in most cases, ensuring correct behavior regardless of the project setting. For more information about minor limitations with disabling this setting, refer to the section "Limitations with Physics settings" in the manual pages for each interactor.

### XR Interaction Simulator stability improvements

Several fixes were made to improve the stability and reliability of the `XRInteractionSimulator`. The simulator no longer erratically spins controller objects when grabbing interactables whose colliders intersect with the camera while mouse-looking, and no longer causes persistent scroll behavior when releasing the right mouse button while scrolling. Scene lifecycle handling was also improved so the simulator correctly refreshes its references to the camera, controllers, hands, and XR Origin after scene changes when persisting via `DontDestroyOnLoad`. Additionally, switching to hand mode is now properly blocked when XR Hands 1.8.0+ is not installed, preventing controllers from disappearing without hands appearing.
