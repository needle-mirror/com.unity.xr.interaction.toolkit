# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

<!-- Headers should be listed in this order: Added, Changed, Deprecated, Removed, Fixed, Security -->

## [1.0.0-pre.3] - 2021-03-18

### Added
- Added ability for serialized fields added in derived behaviors to automatically appear in the Inspector. Users will no longer need to create a custom [Editor](https://docs.unity3d.com/ScriptReference/Editor.html) to be able to see those fields in the Inspector. See [Extending the XR Interaction Toolkit](../manual/index.html#extending-the-xr-interaction-toolkit) in the manual for details about customizing how they are drawn.
- Added support for `EnhancedTouch` from the Input System for AR gesture classes. This means AR interaction is functional when the Active Input Handling project setting is set to Input System Package (New).
- Added registration events to `XRBaseInteractable` and `XRBaseInteractor` which work like those in `XRInteractionManager` but for just that object.
- Added new methods in `ARPlacementInteractable` to divide the logic in `OnEndManipulation` into `TryGetPlacementPose`, `PlaceObject`, and `OnObjectPlaced`.
- Added `XRRayInteractor.hitClosestOnly` property to limit the number of valid targets. Enable this to make only the closest Interactable receive hover events rather than all Interactables in the full length of the raycast.
- Added new methods in `XRRayInteractor` for getting information about UI hits, and made more methods `virtual` or `public`.
- Added several properties to Grab Interactable (Damping and Scale) to allow for tweaking the velocity and angular velocity when the Movement Type is Velocity Tracking. These values can be adjusted to reduce oscillation and latency from the Interactor.

### Changed
- Changed script execution order so `LocomotionProvider` occurs before Interactors are processed, fixing Ray Interactor from casting with stale controller poses when moving or turning the rig and causing visual flicker of the line.
- Changed script execution order so `XRUIInputModule` processing occurs after `LocomotionProvider` and before Interactors are processed to fix the frame delay with UI hits due to using stale raycast rays. `XRUIInputModule.Process` now does nothing, override `XRUIInputModule.DoProcess` which is called directly from `Update`.
- Changed `XRUIInputModule.DoProcess` from `abstract` to `virtual`. Overriding methods in derived classes should call `base.DoProcess` to ensure `IUpdateSelectedHandler` event sending occurs as before.
- Changed Ray Interactor's Reference Frame property to use global up as a fallback when not set instead of the Interactor's up.
- Changed Ray Interactor Projectile Curve to end at ground height rather than controller height. Additional Ground Height and Additional Flight Time properties can be adjusted to control how long the curve travels, but this change means the curve will be longer than it was in previous versions.
- Changed `TrackedDeviceGraphicRaycaster` to ignore Trigger Colliders by default when checking for 3D occlusion. Added `raycastTriggerInteraction` property to control this.
- Changed `XRBaseInteractor.allowHover` and `XRBaseInteractor.allowSelect` to retain their value instead of getting changed to `true` during `OnEnable`. Their initial values are unchanged, remaining `true`.
- Changed some AR behaviors to be more configurable rather than using some hardcoded values or requiring using MainCamera. AR Placement Interactable and AR Translation Interactable must now specify a Fallback Layer Mask to support non-trackables instead of always using Layer 9.
- Changed `IUIInteractor` to not inherit from `ILineRenderable`.

### Deprecated
- Deprecated `XRBaseInteractor.enableInteractions`, use `XRBaseInteractor.allowHover` and `XRBaseInteractor.allowSelect` instead.

### Removed
- Removed several MonoBehaviour message functions in AR behaviors to use `ProcessInteractable` and `ProcessInteractor` instead.

### Fixed
- Fixed issue where the end of a Projectile or Bezier Curve lags behind and appears bent when the controller is moved too fast. ([1291060](https://issuetracker.unity3d.com/product/unity/issues/guid/1291060))
- Fixed Ray Interactor interacting with Interactables that are behind UI. ([1312217](https://issuetracker.unity3d.com/product/unity/issues/guid/1312217))
- Fixed `XRRayInteractor.hoverToSelect` not being functional. ([1301630](https://issuetracker.unity3d.com/product/unity/issues/guid/1301630))
- Fixed Ray Interactor not allowing for valid targets behind an Interactable with multiple Collider objects when the ray hits more than one of those Colliders.
- Fixed Ray Interactor performance to only perform raycasts once per frame instead of each time `GetValidTargets` is called by doing it during `ProcessInteractor` instead.
- Fixed exception in `XRInteractorLineVisual` when changing the Sample Frequency or Line Type of a Ray Interactor.
- Fixed Ray Interactor anchor control rotation when the Rig plane was not up. Added a property `anchorRotateReferenceFrame` to control the rotation axis.
- Fixed Reference Frame missing from the Ray Interactor Inspector when the Line Type was Bezier Curve.
- Fixed mouse scroll amount being too large in `XRUIInputModule` when using Input System.
- Fixed Scrollbar initially scrolling to incorrect position at XR pointer down when using `TrackedDeviceGraphicRaycaster`, which was caused by `RaycastResult.screenPosition` never being set.
- Fixed `GestureRecognizer` skipping updating some gestures during the same frame when another gesture finished.
- Fixed namespace of several Editor classes to be in `UnityEditor.XR.Interaction.Toolkit` instead of `UnityEngine.XR.Interaction.Toolkit`.
- Fixed default value of Blocking Mask on Tracked Device Graphic Raycaster to be Everything (was skipping Layer 31).

## [1.0.0-pre.2] - 2021-01-20

### Added
- Added registration events to `XRInteractionManager` and `OnRegistered`/`OnUnregistered` methods to `XRBaseInteractable` and `XRBaseInteractor`.
- Added and improved XML documentation comments and tooltips.
- Added warnings to XR Controller (Action-based) when referenced Input Actions have not been enabled.
- Added warning to Tracked Device Graphic Raycaster when the Event Camera is not set on the World Space Canvas.

### Changed
- Changed `XRBaseInteractable` and `XRBaseInteractor` to no longer register with `XRInteractionManager` in `Awake` and instead register and unregister in `OnEnable` and `OnDisable`, respectively.
- Changed the signature of all interaction event methods (e.g. `OnSelectEntering`) to take event data through a class argument rather than being passed the `XRBaseInteractable` or `XRBaseInteractor` directly. This was done to allow for additional related data to be provided by the Interaction Manager without requiring users to handle additional methods. This also makes it easier to handle the case when the selection or hover is canceled (due to either the Interactor or Interactable being unregistered as a result of being disabled or destroyed) without needing to duplicate code in an `OnSelectCanceling` and `OnSelectCanceled`.
  |Old Signature|New Signature|
  |---|---|
  |`OnHoverEnter*(XRBaseInteractor interactor)`<br/>-and-<br/>`OnHoverEnter*(XRBaseInteractable interactable)`|`OnHoverEnter*(HoverEnterEventArgs args)`|
  |`OnHoverExit*(XRBaseInteractor interactor)`<br/>-and-<br/>`OnHoverExit*(XRBaseInteractable interactable)`|`OnHoverExit*(HoverExitEventArgs args)`|
  |`OnSelectEnter*(XRBaseInteractor interactor)`<br/>-and-<br/>`OnSelectEnter*(XRBaseInteractable interactable)`|`OnSelectEnter*(SelectEnterEventArgs args)`|
  |`OnSelectExit*(XRBaseInteractor interactor)`<br/>-and-<br/>`OnSelectExit*(XRBaseInteractable interactable)`|`OnSelectExit*(SelectExitEventArgs args)` and using `!args.isCanceled`|
  |`OnSelectCancel*(XRBaseInteractor interactor)`|`OnSelectExit*(SelectExitEventArgs args)` and using `args.isCanceled`|
  |`OnActivate(XRBaseInteractor interactor)`|`OnActivated(ActivateEventArgs args)`|
  |`OnDeactivate(XRBaseInteractor interactor)`|`OnDeactivated(DeactivateEventArgs args)`|
  ```csharp
  // Example Interactable that overrides an interaction event method.
  public class ExampleInteractable : XRBaseInteractable
  {
      // Old code -- delete after migrating to new method signature
      protected override void OnSelectEntering(XRBaseInteractor interactor)
      {
          base.OnSelectEntering(interactor);
          // Do something with interactor
      }

      // New code
      protected override void OnSelectEntering(SelectEnterEventArgs args)
      {
          base.OnSelectEntering(args);
          var interactor = args.interactor;
          // Do something with interactor
      }
  }

  // Example behavior that is the target of an Interactable Event set in the Inspector with a Dynamic binding.
  public class ExampleListener : MonoBehaviour
  {
      // Old code -- delete after migrating to new method signature and fixing reference in Inspector
      public void OnSelectEntered(XRBaseInteractor interactor)
      {
          // Do something with interactor
      }

      // New code
      public void OnSelectEntered(SelectEnterEventArgs args)
      {
          var interactor = args.interactor;
          // Do something with interactor
      }
  }
  ```
- Changed which methods are called by the Interaction Manager when either the Interactor or Interactable is unregistered. Previously `XRBaseInteractable` had `OnSelectCanceling` and `OnSelectCanceled` called on select cancel, and `OnSelectExiting` and `OnSelectExited` called when not canceled. This has been combined into `OnSelectExiting(SelectExitEventArgs)` and `OnSelectExited(SelectExitEventArgs)` and the `isCanceled` property is used to distinguish as needed. The **Select Exited** event in the Inspector is invoked in either case.
  ```csharp
  public class ExampleInteractable : XRBaseInteractable
  {
      protected override void OnSelectExiting(SelectExitEventArgs args)
      {
          base.OnSelectExiting(args);
          // Do something common to both.
          if (args.isCanceled)
              // Do something when canceled only.
          else
              // Do something when not canceled.
      }

  }
  ```
- Changed many custom Editors to also apply to child classes so they inherit the custom layout of the Inspector. If your derived class adds a `SerializeField` or public field, you will need to create a custom [Editor](https://docs.unity3d.com/ScriptReference/Editor.html) to be able to see those fields in the Inspector. For Interactor and Interactable classes, you will typically only need to override the `DrawProperties` method in `XRBaseInteractorEditor` or `XRBaseInteractableEditor` rather than the entire `OnInspectorGUI`. See [Extending the XR Interaction Toolkit](../manual/index.html#extending-the-xr-interaction-toolkit) in the manual for a code example.
- Changed `XRInteractionManager.SelectCancel` to call `OnSelectExiting` and `OnSelectExited` on both the `XRBaseInteractable` and `XRBaseInteractor` in a similar interleaved order to other interaction state changes and when either is unregistered.
- Changed order of `XRInteractionManager.UnregisterInteractor` to first cancel the select state before canceling hover state for consistency with the normal update loop which exits select before exiting hover.
- Changed `XRBaseInteractor.StartManualInteraction` and `XRBaseInteractor.EndManualInteraction` to go through `XRInteractionManager` rather than bypassing constraints and events on the Interactable.
- Changed the **GameObject > XR > Grab Interactable** menu item to create a visible cube and use a Box Collider so that it is easier to use.
- Renamed `LocomotionProvider.startLocomotion` to `LocomotionProvider.beginLocomotion` for consistency with method name.

### Fixed
- Fixed Direct Interactor and Socket Interactor causing exceptions when a valid target was unregistered, such as from being destroyed.
- Fixed Ray Interactor clearing custom direction when initializing (fixed initialization of the Original Attach Transform so it copies values from the Attach Transform instead of setting position and rotation values to defaults). ([1291523](https://issuetracker.unity3d.com/product/unity/issues/guid/1291523))
- Fixed Socket Interactor so only an enabled Renderer is drawn while drawing meshes for hovered Interactables.
- Fixed Grab Interactable to respect Interaction Layer Mask for whether it can be hovered by an Interactor instead of always allowing it.
- Fixed Grab Interactable so it restores the Rigidbody's drag and angular drag values on drop.
- Fixed mouse input not working with Unity UI when Active Input Handling was set to Input System Package.
- Fixed issue where Interactables in AR were translated at the height of the highest plane regardless of where the ray is cast.
- Fixed so steps to setup camera in `XRRig` only occurs in Play mode in the Editor.
- Fixed file names of .asmdef files to match assembly name.
- Fixed broken links for the help button (?) in the Inspector so it opens Scripting API documentation for each behavior in the package. ([1291475](https://issuetracker.unity3d.com/product/unity/issues/guid/1291475))
- Fixed XR Rig so it handles the Tracking Origin Mode changing on the device.
- Fixed XR Controller so it only sets position and rotation while the controller device is being tracked instead of resetting to the origin (such as from the device disconnecting or opening a system menu).

## [1.0.0-pre.1] - 2020-11-14

### Removed
- Removed anchor control deadzone properties from XR Controller (Action-based) used by Ray Interactor, it should now be configured on the Actions themselves

## [0.10.0-preview.7] - 2020-11-03

### Added
- Added multi-object editing support to all Editors

### Fixed
- Fixed Inspector foldouts to keep expanded state when clicking between GameObjects

## [0.10.0-preview.6] - 2020-10-30

### Added
- Added support for haptic impulses in XR Controller (Action-based)

### Fixed
- Fixed issue with actions not being considered pressed the frame after triggered
- Fixed issue where an AR test would fail due to the size of the Game view
- Fixed exception when adding an Input Action Manager while playing

## [0.10.0-preview.5] - 2020-10-23

### Added
- Added sample containing default set of input actions and presets

### Fixed
- Fixed issue with PrimaryAxis2D input from mouse not moving the scrollbars on UI as expected. ([1278162](https://issuetracker.unity3d.com/product/unity/issues/guid/1278162))
- Fixed issue where Bezier Curve did not take into account controller tilt. ([1245614](https://issuetracker.unity3d.com/product/unity/issues/guid/1245614))
- Fixed issue where a socket's hover mesh was offset. ([1285693](https://issuetracker.unity3d.com/product/unity/issues/guid/1285693))
- Fixed issue where disabling parent before `XRGrabInteractable` child was causing an error in `OnSelectCanceling`

## [0.10.0-preview.4] - 2020-10-14

### Fixed
- Fixed migration of a renamed field in interactors

## [0.10.0-preview.3] - 2020-10-14

### Added
- Added ability to control whether the line will always be cut short at the first raycast hit, even when invalid, to the Interactor Line Visual ([1252532](https://issuetracker.unity3d.com/product/unity/issues/guid/1252532))

### Changed
- Renamed `OnSelectEnter`, `OnSelectExit`, `OnSelectCancel`, `OnHoverEnter`, `OnHoverExit`, `OnFirstHoverEnter`, and `OnLastHoverExit` to `OnSelectEntered`, `OnSelectExited`, `OnSelectCanceled`, `OnHoverEntered`, `OnHoverExited`, `OnFirstHoverEntered`, and `OnLastHoverExited` respectively.
- Replaced some `ref` parameters with `out` parameters in `ILineRenderable`; callers should replace `ref` with `out`

### Fixed
- Fixed Tracked Device Graphic Raycaster not respecting the Raycast Target property of UGUI Graphic when unchecked ([1221300](https://issuetracker.unity3d.com/product/unity/issues/guid/1221300))
- Fixed XR Ray Interactor flooding the console with assertion errors when sphere cast is used ([1259554](https://issuetracker.unity3d.com/product/unity/issues/guid/1259554), [1266781](https://issuetracker.unity3d.com/product/unity/issues/guid/1266781))
- Fixed foldouts in the Inspector to expand or collapse when clicking the label, not just the icon ([1259683](https://issuetracker.unity3d.com/product/unity/issues/guid/1259683))
- Fixed created objects having a duplicate name of a sibling ([1259702](https://issuetracker.unity3d.com/product/unity/issues/guid/1259702))
- Fixed created objects not being selected automatically ([1259682](https://issuetracker.unity3d.com/product/unity/issues/guid/1259682))
- Fixed XRUI Input Module component being duplicated in EventSystem GameObject after creating it from UI Canvas menu option ([1218216](https://issuetracker.unity3d.com/product/unity/issues/guid/1218216))
- Fixed missing AudioListener on created XR Rig Camera ([1241970](https://issuetracker.unity3d.com/product/unity/issues/guid/1241970))
- Fixed several issues related to creating objects from the GameObject menu, such as broken undo/redo and proper use of context object
- Fixed issue where GameObjects parented under an `XRGrabInteractable` did not retain their local position and rotation when drawn as a Socket Interactor Hover Mesh ([1256693](https://issuetracker.unity3d.com/product/unity/issues/guid/1256693))
- Fixed issue where Interaction callbacks (`OnSelectEnter`, `OnSelectExit`, `OnHoverEnter`, and `OnHoverExit`) are triggered before interactor and interactable objects are updated. ([1231662](https://issuetracker.unity3d.com/product/unity/issues/guid/1231662), [1228907](https://issuetracker.unity3d.com/product/unity/issues/guid/1228907), [1231482](https://issuetracker.unity3d.com/product/unity/issues/guid/1231482))

## [0.10.0-preview.2] - 2020-08-26

### Added
- Added XR Device Simulator and sample assets for simulating an XR HMD and controllers using keyboard & mouse

## [0.10.0-preview.1] - 2020-08-10

### Added
- Added continuous move and turn locomotion

### Changed
- Changed accesibility levels to avoid `protected` fields, instead exposed through properties
- Components that use Input System actions no longer automatically enable or disable them. Add the `InputActionManager` component to a GameObject in a scene and use the Inspector to reference the `InputActionAsset` you want to automatically enable at startup.
- Some properties have been renamed from PascalCase to camelCase to conform with coding standard; the API Updator should update usage automatically in most cases

### Fixed
- Fixed compilation issue when AR Foundation package is also installed
- Fixed the Interactor Line Visual lagging behind the controller ([1264748](https://issuetracker.unity3d.com/product/unity/issues/guid/1264748))
- Fixed Socket Interactor not creating default hover materials, and backwards usage of the materials ([1225734](https://issuetracker.unity3d.com/product/unity/issues/guid/1225734))
- Fixed Tint Interactable Visual to allow it to work with objects that have multiple materials
- Improved Tint Interactable Visual to not create a material instance when Emission is enabled on the material

## [0.9.9-preview.3] - 2020-06-24

### Changed
- In progress changes to visibilty

## [0.9.9-preview.2] - 2020-06-22

### Changed
- Hack week version push.

## [0.9.9-preview.1] - 2020-06-04

### Changed
- Swaps axis for feature API anchor manipulation

### Fixed
- Fixed controller recording not working
- Start controller recording at 0 time so you dont have to wait for the recording to start playing.

## [0.9.9-preview] - 2020-06-04

### Added
- Added Input System support
- Added abiltiy to query the controller from the interactor

### Changed
- Changed a number of members and properties to be `protected` rather than `private`
- Changed to remove `sealed` from a number of classes.

## [0.9.4-preview] - 2020-04-01

### Fixed
- Fixed to allow 1.3.X or 2.X versions of legacy input helpers to work with the XR Interaction Toolkit.

## [0.9.3-preview] - 2020-01-23

### Added
- Added pose provider support to XR Controller
- Added abiilty to put objects back to their original hierarchy position when dropping them
- Made teleport configurable to use either activate or select
- Removed need for box colliders behind UI to stop line visuals from drawing through them

### Fixed
- Fixed minor documentation issues
- Fixed passing from hand to hand of objects using direct interactors
- Fixed null ref in controller states clear
- Fixed no "OnRelease" even for Activate on Grabbable

## [0.9.2-preview] - 2019-12-17

### Changed
- Rolled LIH version back until 1.3.9 is on production.

## [0.9.1-preview] - 2019-12-12

### Fixed
- Documentation image fix

## [0.9.0-preview] - 2019-12-06

### Changed
- Release candidate

## [0.0.9-preview] - 2019-12-06

### Changed
- Further release prep

## [0.0.8-preview] - 2019-12-05

### Changed
- Pre-release release.

## [0.0.6-preview] - 2019-10-15

### Changed
- Changes to README.md file

### Fixed
- Further CI/CD fixes.

## [0.0.5-preview] - 2019-10-03

### Changed
- Renamed everything to com.unity.xr.interaction.toolkit / XR Interaction Toolkit

### Fixed
- Setup CI correctly.

## [0.0.4-preview] - 2019-05-08

### Changed
- Bump package version for CI tests.

## [0.0.3-preview] - 2019-05-07

### Added
- Initial preview release of the XR Interaction framework.
