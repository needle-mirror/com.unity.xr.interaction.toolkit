---
uid: xri-continuous-movement
---

# Continuous movement

The package provides continuous movement and turning providers that allow the user to move and turn smoothly in the virtual environment.

## Continuous Turn Provider

The package provides a [Continuous Turn Provider](continuous-turn-provider.md) component. Continuous turning, as opposed to snap turning by discrete angles, smoothly rotates the XR Origin by an amount over time when the application receives a configured input (for example, a thumbstick is tilted to the left).

## Continuous Move Provider

The package provides a [Continuous Move Provider](continuous-move-provider.md) component. Continuous moving, as opposed to teleporting, smoothly translates the XR Origin by an amount over time when the application receives a configured input (for example, a thumbstick is tilted forward).

The **Forward Source** can be used to define which direction the XR Origin should move when, for example, pushing forward on a thumbstick. By default, it will use the Camera Object, meaning the user will move forward in the direction they are facing. An example of how this property can be used is to set it to a Transform that tracks the pose of a motion controller to allow the user to move forward in the direction they are holding the controller.

If a [Character Controller](xref:class-CharacterController) is present on the XR Origin, this Continuous Move Provider will move the XR Origin using [`CharacterController.Move`](xref:UnityEngine.CharacterController.Move(UnityEngine.Vector3)) rather than directly translating the Transform of the XR Origin.

## Dynamic Move Provider
The [Starter Assets](xref:xri-samples-starter-assets) sample package provides a [Dynamic Move Provider](xref:xri-samples-starter-assets#scripts) component. Dynamic moving extends the base Continuous Move Provider by automatically adjusting the frame of reference that determines the forward direction of movement based on user preference for each hand, using either the forward direction of the head or the forward direction of the hand or controller. This component is demonstrated in the [Starter Assets DemoScene](xref:xri-samples-starter-assets#demo-scene).
