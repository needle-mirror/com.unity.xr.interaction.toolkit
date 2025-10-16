---
uid: xri-whats-new-3-4
---
# What's new in version 3.4

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

Summary of changes in XR Interaction Toolkit package version 3.4:

## Added

### 10-key number pad keyboard sample

An additional keyboard layout was added to the [Spatial Keyboard](xref:xri-samples-spatial-keyboard) package sample. This new prefab can be used for numeric entry and is shown in the `KeyboardDemo` scene included in the sample.

### Minor XR Interaction Debugger window improvements

The [XR Interaction Debugger](xref:xri-debugger-window) window has added columns and a new tab for registered XR Interactable Snap Volumes. Interactors now display the [XR Interaction Group](xri-xr-interaction-group) they are a member of in a new column. There is also an additional column to display the full scene hierarchy path of a component, which can be revealed by right-clicking the column header and selecting **Hierarchy Path**. The XR Interaction Manager itself is now drawn greyed out when the component is no longer active and enabled instead of being removed from the window.

## Changed

### XR Interaction Manager creation and component registration

The automatic creation of the XR Interaction Manager and automatic registration of components to a default manager component can now be controlled with new project settings in **Edit** &gt; **Project Settings** under **XR Plug-in Management** &gt; **XR Interaction Toolkit**. The default behavior remains to automatically create the manager as needed and to find a default manager component when the Interaction Manager property reference is None or Missing. These changes to registration logic was done to allow for greater flexibility in projects with multiple scenes or advanced setups where users want better control over the timing of when components are registered with the manager. See [Project settings and validation](xref:xri-settings) for additional detail about these new runtime project settings.

Previously if there was no XR Interaction Manager or XR UI Input Module component, one would always be created automatically immediately during the `OnEnable` method of the interaction component (such as an interactor). This behavior was slightly altered to wait for all scenes to finish loading before automatically creating the default manager component to allow for a manager located in another scene to be found once loaded. Alternatively, set Manager Creation Mode to **Manual** to prevent Unity from creating one automatically.

New `static` methods were added to [`XRInteractionManager`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager) to register components to a global waitlist to allow those interaction components to be registered with an active and enabled `XRInteractionManager` component instance, either automatically when enabled in Project Settings or when triggered through scripting API. Enabled interaction components automatically add themselves to the waitlist when a manager component instance could not be found or when the manager component they were registered with is destroyed.

As part of these changes, when the XR Interaction Manager component is destroyed, all interactions (hover, select, and focus) managed by that manager are now canceled, indicated with the `isCanceled` property in the hover/select/focus exit event args, and the interaction components are notified in their `OnUnregistered` method, indicated with a new `managerDestroyed` property in the unregistered event args to let registered components know it was due to the XR Interaction Manager being destroyed.
