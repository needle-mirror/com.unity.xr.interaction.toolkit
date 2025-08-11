---
uid: xri-whats-new-3-3
---
# What's new in version 3.3

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.3:

## Added

### Support for declaring interactable dependencies for processing order

The XR Interaction Manager component can now handle dependency relationships between interactables to control the order in which interactables are processed (`IXRInteractable.ProcessInteractable`). Interactables can have a Parent Interactable which ensures that the parent is processed before it. This can be used for compound interactables to be updated as you would expect. For example, you can have an [XR Grab Interactable](xref:xri-xr-grab-interactable) that has an [XR Socket Interactor](xref:xri-xr-socket-interactor) on a child GameObject. A second XR Grab Interactable that is placed in the socket should be updated after the XR Grab Interactable being held and moved. By using the Parent Interactable system, the interactable in the socket will process after, eliminating a potential one frame delay in computing the desired pose.

New properties were added to interactors and interactables to explicitly set a Parent Interactable for when that component is registered with the XR Interaction Manager. You can enable the Auto Find Parent Interactable property if you want to automatically search up the GameObject hierarchy at runtime instead of assigning the Parent Interactable property for ease of setup. You can also use new `public` methods on the [`XRInteractionManager`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager) to set parent interactable relationships through scripting API.

The [XR Interaction Debugger](xref:xri-debugger-window) window has a new column to show the parent interactable(s) for all interactors and interactables. It also now lists the interactors and interactables in the order that they are processed rather than alphabetically.

For more information about how interactables are processed and how to customize the ordering, see [Processing interactables](xref:xri-architecture#processing-interactables) in the Interaction overview manual page.

## Changed

### Interactions triggered through scripting API now checks conditions

Changed the `public` `SelectEnter`, `HoverEnter`, and `FocusEnter` methods in `XRInteractionManager` to check conditions before allowing the state change. Previously, those methods could bypass conditions like Interaction Layer Mask overlap checks and whether the objects are registered with that manager. Previously, this could cause confusion when manually triggering a selection since during the next frame, the manager would cause a deselect anyway since the objects fail those checks.

Additional methods were added to `XRInteractionManager` to unconditionally enter those interaction states.
