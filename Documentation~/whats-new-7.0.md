---
uid: xri-whats-new-7-0
---
# What's new in version 7.0

XR Interaction Toolkit 7.0 aligns the package version with Unity 7, adds gaze aim and hand tracking microgesture support, and makes the remaining deprecated APIs obsolete, including the XR Controller components, the legacy locomotion components, the AR gesture components, and the XR Device Simulator.

Discover new features and performance improvements in the latest update to the XR Interaction Toolkit. For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).

## Added

### Gaze aim

Gaze aim lets users target objects and UI by looking at them, then select with their hand or controller, such as with a pinch or a controller button. They can also scroll the UI they look at with microgesture swipes. Their hands can rest in their lap, and they don't need to aim a ray. It suits UI and far objects, and works alongside near interaction such as direct poking and grabbing.

Gaze aim is available in the [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample and is off by default. Refer to [Gaze aim](xref:xri-samples-hands-interaction-demo#gaze-aim) for how to turn it on and set up eye tracking.

### Microgesture support

You can now scroll UI and move around with hand tracking microgestures, such as thumb taps and swipes:

* **UI scrolling**: swipe to scroll the UI you point at. The new [Press Scroll Input](xref:xri-press-scroll-input) component and the **UI Scroll Momentum** interactor option make each swipe scroll a fixed distance and glide to a stop. Refer to [Scroll UI with XR input](xref:xri-ui-scrolling).
* **Locomotion**: tap to enter teleport mode, swipe to snap turn or step, and tap again to teleport. Stepping uses the new [Step Move Provider](xref:xri-step-move-provider). Refer to [Step movement](xref:xri-step-movement).

The [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample is set up for both. Refer to [Microgesture UI scrolling](xref:xri-samples-hands-interaction-demo#microgesture-ui-scrolling) and [Microgesture locomotion](xref:xri-samples-hands-interaction-demo#microgesture-locomotion) for how to use them.

### XR UI Toolkit Manager: PanelRenderer support

The XR UI Toolkit Manager now supports `PanelRenderer` panels in addition to `UIDocument` for world-space UI Toolkit interaction. Poke and ray interactions work with both panel types. A new `panelComponent` property on [`UIHoverEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.UI.UIHoverEventArgs) provides access to the panel host component during hover events. Refer to [UI Toolkit support](xref:xri-ui-world-space-ui-toolkit-support).

## Changed

### Package version alignment

The package version changed from 3.7 to 7.0 to align the package major version with Unity 7. The minimum supported Unity Editor version increased to 7000.0 from 6000.0, and the minimum uGUI version increased to 7.0.0 from 2.0.0.

### Recommended XR Hands version

The recommended version of the [XR Hands](https://docs.unity3d.com/Manual/com.unity.xr.hands.html) package is now 7.0.0-pre.1 or newer, which drives the aim pose input controls of `<XRHandDevice>` from Meta Hand Tracking Aim. The `XRI Default Input Actions` asset in the Starter Assets sample no longer binds the hand aim actions to `<MetaAimHand>`.

### Interactors read input only from input readers

Interactors now always read input from their input reader properties, such as `selectInput` and `activateInput`. Interactors no longer read input from an XR Controller component.

The interactor Inspectors no longer show the **Audio Events**, **Haptic Events**, or **XR Controller Configuration** sections, and interactors no longer convert these settings to feedback components at runtime. To play audio or haptics when interaction events occur, add the [Simple Audio Feedback](xref:xri-simple-audio-feedback) and [Simple Haptic Feedback](xref:xri-simple-haptic-feedback) components.

### Gravity requires a Gravity Provider

The Continuous Move Provider, Grab Move Provider, and Two-Handed Grab Move Provider no longer apply gravity themselves, and their **Use Gravity** and **Gravity Application Mode** settings are removed. To apply gravity during locomotion, add a [Gravity Provider](xref:gravity-provider) component to your XR Origin.

### Locomotion providers require a Locomotion Mediator

Locomotion providers no longer fall back to the obsolete Locomotion System. Each locomotion provider requires a `LocomotionMediator`. For more information, refer to [Locomotion overview](xref:xri-locomotion).

### AR gestures use Enhanced Touch input only

The AR gesture recognizers create gestures only from Input System Enhanced Touch input. Input Manager touch input is no longer supported.

## Obsolete APIs now cause compiler errors

All obsolete APIs that previously reported a compiler warning now report a compiler error, and their implementations have been removed. Where an obsolete API is still used, for example from a precompiled assembly, properties return default values and methods throw a `NotSupportedException`. The Affordance System and its related APIs are unchanged.

The following table lists the main obsolete components and APIs and their replacements:

| Obsolete components and APIs | Replacement |
| :--------------------------- | :---------- |
| XR Controller (Action-based), XR Controller (Device-based), `XRBaseController`, XR Screen Space Controller, and `XRBaseControllerInteractor` | Input reader properties on interactors. Refer to the [XRI 3.0 upgrade guide](xref:xri-upgrade-guide-3-0). |
| Locomotion System, Character Controller Driver, and the Action-based and Device-based Continuous Move, Continuous Turn, and Snap Turn Providers | `LocomotionMediator` and the Continuous Move, Continuous Turn, and Snap Turn Providers. Refer to [Locomotion overview](xref:xri-locomotion). |
| AR Gesture Interactor, the AR interactables, `ARBaseGestureInteractable`, and `GestureTransformationUtility` | Cross-platform AR components. Refer to [AR interaction](xref:xri-ar-interaction). |
| XR Device Simulator, `XRDeviceSimulatorLoader`, `SimulatedHandExpressionManager`, `HandExpressionCapture`, `HandExpressionDataExtractor`, and `SimulatedHandExpression.icon` | [XR Interaction Simulator](xref:xri-xr-interaction-simulator-overview) |
| `InputHelpers` | `XRInputDeviceButtonReader` or `XRInputDeviceValueReader`. Refer to [Input readers](xref:xri-input-readers). |

## Removed

The following features have been removed:

* The XR Device Simulator sample. Use the [XR Interaction Simulator](xref:xri-samples-xr-interaction-simulator) sample instead.
* The **Use Classic XR Device Simulator** setting from the XR Interaction Simulator project settings.
* Obsolete APIs that previously reported a compiler error, including `XRRig`, `XRLegacyGrabTransformer`, `XRInteractableEvent`, and `XRInteractorEvent`. For the full list, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).
