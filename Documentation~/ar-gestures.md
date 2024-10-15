---
uid: xri-ar-gestures
---

# AR gestures

## Touchscreen gestures

Touchscreen gesture data is surfaced by the `TouchscreenGestureInputController` which translates touch events into gestures such as tap, drag, pinch and twist. This gesture data can be used as bindings for [Input System](https://docs.unity3d.com/Manual/com.unity.inputsystem.html) actions.

The XR Interaction Toolkit package comes with a number of pre-defined gestures, but you can always extend this package by defining your own gestures.

| Gesture | Triggered by input |
|---|---|
| **Tap** | User touches the screen |
| **Drag** | User drags finger across screen |
| **Pinch** | User moves two fingers toward or away from each other along a straight line |
| **Twist** | User rotates two fingers around a center point |
| **Two Finger Drag** | User drags with two fingers |
