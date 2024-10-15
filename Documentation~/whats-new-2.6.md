---
uid: xri-whats-new-2-6
---
# What's new in version 2.6

Summary of changes in XR Interaction Toolkit package version 2.6:

## Added

These new features were backported from version 3.0.0.

### Spatial Keyboard sample

The [Spatial Keyboard sample](samples-spatial-keyboard.md) provides base prefabs and scripts for implementation and customization of a virtual keyboard.

#### Global keyboard

The `GlobalNonNativeKeyboard` supports the spawning and positioning of a keyboard prefab, which can be reused as a global spatial keyboard.

#### Key functions

The spatial keyboard sample `XRKeyboardKey` leverages `KeyFunctions`, which are scriptable objects which can be customized to enable the key commands to support custom keyboard functionality.

#### Keyboard Display component

The `XRKeyboardDisplay` component works in conjunction with input fields and can be configured to support the global keyboard or an instanced keyboard in the scene. Additionally, it can be configured to update or clear text on submit and enable the keyboard to monitor character limits. The `XRKeyboardDisplay` also contains its own set of events that respond to keyboard events which can be useful when using a global keyboard where the context can change frequently.

#### Keyboard layouts

Multiple keyboard layouts can be supported for a subset of keys on the keyboard. This is demonstrated by the alpha-numeric and symbols layouts in the `XRI Keyboard` prefab. See [keyboard layouts](samples-spatial-keyboard.md#keyboard-layout) for more detailed information.

### Climb teleportation

Climbing and teleportation has been enhanced to provide teleportation up and down ladders. In addition to simple endpoint-to-endpoint teleportation, [Teleportation multi-anchor volumes](teleportation-multi-anchor-volume.md) have been added. These allow multiple predefined teleport outlets per climbing surface which can be selected based on head or eye gaze (or any other method of determination). For more information about these new additions, please read the [Locomotion](locomotion.md) documentation and check out an example of how these updates are used in the `Demo Scene` of the [Starter Assets](samples-starter-assets.md#demo-scene) sample.

For a full list of changes and updates in this version, refer to the [XR Interaction Toolkit package changelog](xref:xri-changelog).
