---
uid: xri-whats-new-3-2
---
# What's new in version 3.2

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.2:

## Added

### Support for reduced visual stutter in XR Grab Interactable

The [XR Grab Interactable](xref:xri-xr-grab-interactable) component now has support for specifying a separate visual hierarchy that can be updated by the component at a more frequent rate than the physics system to allow for Kinematic and Velocity Tracking movement types to have the same smoothness and latency as the Instantaneous movement type when the Rigidbody is not colliding with anything. This allows the grabbed object to have a much better feel in hand, especially when using continuous locomotion.

Additionally, new properties were added to optionally constrain the linear velocity and angular velocity of the object when the Movement Type is set to Velocity Tracking. This lets you keep the object from moving too fast and stabilizes the object, especially when a user tries to push it through static geometry.

For more information about how to configure the component to smooth movement, see [Reducing stutter from physics update rate](xref:xri-xr-grab-interactable#stutter) in the component documentation.

### UI Toolkit support

Initial support for UI Toolkit in Unity 6.2 is now available. In addition to support for UI Toolkit, a [World Space UI Sample](xref:xri-samples-world-space-ui) package has been included so that you can learn how to integrate UI Toolkit support into your own XR projects. Please see documentation for [World Space UI with UI Toolkit](xref:xri-ui-world-space-ui-toolkit-support).

> [!NOTE]
> This feature is currently considered pre-release and is expected to undergo improvements, both from a performance and feature-parity perspective. Currently only the **XR Poke Interactor** and **Near-Far Interactor** are compatible with UI Toolkit, with plans to bring support to the **XR Ray Interactor** in a future release. For more information about UI Toolkit, please see the [UI Toolkit](https://docs.unity3d.com/6000.2/Documentation/Manual/UIElements.html) Unity Documentation.

#### Current Known Issues
* **XR Ray Interactor** does not interaction with UI Toolkit documents.
* Interaction with **Scroll View** elements only works by interacting with the scroll bars.
* **Colliders** added to the **UI Documents** must not be set as **Trigger** or poke interactions will not work.
* When using mouse or touch input in the Unity Editor, you may see this error: `ArgumentOutOfRangeException: Unsupported EventSource for pointer event`. To avoid this error, you will need to assign a Project-wide Action in **Project Settings** > **Input System Package**. If you do not have an **Input Action Asset** assigned, you can click the **Create and assign a default project-wide Action Asset**. Once created or assigned, navigate to the **UI** Action Map, expand **Click** and delete the binding for **trigger [XR Controller]**.

### Support for triggering UI clicks on poke down

XR Poke Interactor now supports triggering clicks in the Unity UI (UGUI) system on press down instead of only on release for buttons, toggles, input fields, and dropdowns. This makes it much easier to interact with many UI elements when poking. To enable this functionality, enable Click UI On Down on the component. The package samples have also been updated to enable this by default on the XR Poke Interactor.
