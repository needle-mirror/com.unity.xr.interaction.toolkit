---
uid: xri-whats-new-3-5
---
# What's new in version 3.5

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.5:

## Added

### Hand Teleportation Sample

The [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) package sample has been updated to include a new prefab and scripts to demonstrate one way to trigger teleportation using the XR Hands package gesture detection. The Demo Scene for the Hands Interaction Demo sample has also been updated with a more interactive scene, which includes multiple surfaces and areas to teleport to using the hand gesture-based teleportation.

## Changed

### XR Interaction Simulator: Point & Click Interaction

The XR Interaction Simulator has been updated to support a more-intuitive way to control interactions with a mouse and keyboard. The default interaction mode will now use the screen-to-world point to automatically aim the interactors at the object hovered in the scene. Select input is mapped to the mouse and will auto-detect when the interactor is hovered over an XRInteractable or uGUI elements and send the appropriate select action through the Input System.

### XR UI Input Module component registration

The XR UI Input Module was updated to support some of the registration features recently added to the XR Interaction Manager (See [What's new in version 3.4](xref:xri-whats-new-3-4) for a summary of changes made to the XR Interaction Manager creation and component registration behavior). The default behavior remains to automatically create the XR UI Input Module component as needed on the Event System when interactor components enable support for UI interaction during the `OnEnable` method of the interactor. The XR UI Input Module won't be automatically created until all scenes finish loading, allowing for one to be found in a scene that has just finished loading. To match the functionality of XR Interaction Manager, you can now control whether the UI interactors are immediately registered with the input module component or instead get added to a global waitlist for more control over when the interactor is registered. See [Project settings and validation](xref:xri-settings) for additional detail about the runtime project settings that control creation and registration modes.

New `static` methods were added to [`XRUIInputModule`](xref:UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule) to register UI interactors with a global waitlist to allow those interactor components to be registered with an active and enabled `XRUIInputModule` component instance, either automatically when enabled in Project Settings or when triggered through a scripting API. The built-in interactor components automatically add themselves to the waitlist when the input module component they are registered with is destroyed.

An additional interface [`IUIInteractorRegistrationHandler`](xref:UnityEngine.XR.Interaction.Toolkit.UI.IUIInteractorRegistrationHandler) should generally be implemented by custom UI interactor components instead of [`IUIInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.UI.IUIInteractor) to allow them to be notified of registration events. The `XRUIInputModule` will now invoke `OnRegistered` and `OnUnregistered` methods on those UI interactors when registration changes occur, including when the input module is destroyed.
