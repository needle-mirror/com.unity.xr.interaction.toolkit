---
uid: xri-step-movement
---

# Step movement

The package provides a [Step Move Provider](step-move-provider.md) component. A step means the XR Origin moves by a fixed distance when the application receives a configured input (for example, a hand microgesture swipe forward). Step movement is to continuous movement what snap turning is to continuous turning: the user moves in discrete increments rather than smoothly over time.

Each input moves the XR Origin in one of four directions: forward, backward, left, or right. The provider picks the direction nearest to the input, so a diagonal input steps in a single direction rather than diagonally. Turn off **Enable Strafe** or **Enable Step Backward** to ignore left and right input or backward input.

The **Forward Source** defines which direction is forward. By default, it uses the camera of the XR Origin, so the user steps in the direction they are facing. Unity flattens the forward direction onto the XR Origin's horizontal plane, so looking up or down doesn't move the user up or down.

Holding the input steps repeatedly, once every **Debounce Time**. Use **Delay Time** to wait before the first step, for example to fade in a [tunneling vignette](tunneling-vignette-controller.md) as a comfort option.

The [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample uses a Step Move Provider to step with hand microgesture swipes while in teleport mode.
