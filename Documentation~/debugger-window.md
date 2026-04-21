---
uid: xri-debugger-window
---
# Debugger window

The XR Interaction Toolkit Debugger window displays a top-down view of all the Input Devices, Interactables, and Interactors in the loaded scenes. It also displays their relationship to each other and their registered Interaction Managers. To open this window, go to **Window** &gt; **Analysis** &gt; **XR Interaction Debugger** from Unity's main menu.

You must be in Play mode to use this window.

![interaction-debugger](images/interaction-debugger.png)

## Input devices

The **Input Devices** tab displays all valid input devices that are registered by an [`XRInputSubsystem`](xref:UnityEngine.XR.XRInputSubsystem). Refer to [Unity XR Input](xref:um-xr-input) in the Unity Manual for more information about the `XRInputSubsystem` and [`InputDevice`](xref:UnityEngine.XR.InputDevice) association. To see a list of all input devices in the Unity [Input System package](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest), including the values of their input controls and buttons, open the **Window** &gt; **Analysis** &gt; **Input Debugger** window from Unity's main menu and double-click any device under the **Devices** foldout. For more information about debugging Input System devices, see [Debugging](xref:input-system-debugging) in the Unity Input System manual.

This tab can be used to verify that the input controls that are bound to the actions set in an Interactor's Inspector window under Input Configuration, such as the Select Input, are actuating to expected values. To verify that the Input Action you have assigned is enabled and actually resolving to an input control on an input device, open the **Window** &gt; **Analysis** &gt; **Input Debugger** window and expand the **Actions** foldout to search for your input action and its resolved control bindings. See [Debugging Actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest?subfolder=/manual/Debugging.html#debugging-actions) in the Unity Input System manual for more information on these steps.

### Troubleshooting missing input devices

This **Input Devices** tab doesn't display input devices that are only registered with the [Input System package](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest), such as the simulated devices created by the [XR Interaction Simulator](xref:xri-xr-interaction-simulator-overview) or the [XR Device Simulator](xref:xri-xr-device-simulator-overview). Additionally, the tracked hand devices created by the [XR Hands package](xref:xrhands-manual) are only registered with the Input System package and do not currently appear in this window.

If you do not see any XR devices in either the **Input Devices** tab or within the **Input Debugger** window, your project may not be configured for XR. Check for any warnings or errors that could be preventing expected behavior on the **Project Validation** page under **XR Plug-in Management** in your **Project Settings** (menu: **Edit** &gt; **Project Settings**).

See [Configure your project](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest?subfolder=/manual/project-configuration.html) in the Unity OpenXR Plugin manual for steps to configure your project, which includes steps like enabling **Initialize XR on Startup** and enabling a Plug-in Provider within the **Edit** &gt; **Project Settings** window under **XR Plug-in Management**, and enabling an Interaction Profile under **OpenXR**. Note that Editor Play mode uses Desktop Platform Settings regardless of Active Build Target.

For AR projects that use AR Foundation, see [Provider project settings](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest?subfolder=/manual/project-setup/install-arfoundation.html#provider-project-settings) in the Unity AR Foundation manual for steps to configure your project.

## Interactors and interactables

The Interactors and Interactables tabs displays all active and enabled interactor and interactable components in the loaded scenes that are registered with an [XR Interaction Manager](xref:xri-xr-interaction-manager). Each row displays information about the component, grouped under the XR Interaction Manager that the component is registered with. The name of the interaction manager can be greyed out, which indicates that the manager component is not active and enabled.

The order of interactors and interactables indicates the order that the components are processed by the interaction manager. Refer to [Processing interactables](xref:xri-update-loop#processing-interactables) for more information.

## Snap volumes

The Snap Volumes tab displays all active and enabled [XR Interactable Snap Volume](xref:xri-xr-interactable-snap-volume) components in the loaded scenes that are registered with an XR Interaction Manager. Each row displays information about the snap volume, including the interactable that the snap volume is associated with, grouped under the XR Interaction Manager that the snap volume is registered with. The name of the interaction manager and the associated interactable can be greyed out, which indicates that those components are not active and enabled.

## Target filters

The Target Filters tab displays all active and enabled [XR Target Filter](xref:xri-target-filters#xr-target-filter) components in the loaded scenes. It displays the names of the interactors that each XR Target Filter is linked with.

You can select an `XRTargetFilter` in the Target Filters tree to inspect its Evaluators' scores in the Score tree. The Score tree displays the final and weighted scores for each interactable in the interactor's Valid Target list. The interactors are shown as the parent of their respective Valid Target list.

![xr-target-filter-debugger](images/xr-target-filter-debugger.png)
