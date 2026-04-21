<!-- ray interactor, gaze interactor

## Manipulate Attach Transform options {#attach-transform}

[!INCLUDE [interactor-attach-manipulators](snippets/interactor-attach-manipulators.md)]
-->

Interactors that operate at a distance, like Ray and Gaze interactors, provide additional options to control how the user can manipulate a selected interactable object. These options define whether the user can move, rotate, or scale an object and which controller input to use for each type of manipulation.

To enable the additional manipulation options, set **Manipulate Attach Transform** to `true`.

![Attachment manipulation options](../images/xr-ray-manipulate-attach-transform.png)

Refer to the following sections for information on the manipulation properties:

* [Translation properties](#translation-properties)
* [Rotation properties](#rotation-properties)
   * [Rotation Mode options](#rotate-mode)
* [Scale properties](#scale-properties)
   * [Scale Mode Options](#scale-mode)

### Translation properties

Use the Translation properties to adjust how the attached object moves along the ray cast by the interactor.

| **Property** | **Description** |
| :--- | :--- |
| **Translate Speed** | The maximum speed at which the interactable object can be translated in meters per second.|
| **Translate Input**| The input used to control translation. This must be a 2D axis input, such as a joystick. The standard input for translation is `x` axis of the primary 2D axis of the left controller, which is a thumbstick. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Left Interaction/Manipulation`.|

### Rotation properties

Use the Rotation properties to adjust how the attached object turns while selected.

| **Property** | **Description** |
| :--- | :--- |
| [Rotate Mode](#rotate-mode) | Specifies how input is applied when rotating interactable objects. |
| **Rotate Speed** | The maximum speed at which the interactable object can be rotated, specified in degrees per second. This property is only applicable when **Rotate Mode** is set to **Rotate Over Time**. |
| **Rotate Reference Frame** | The reference frame used for rotation. This can be set to either **World** or **Local**. |
| **Rotate Input** | The input used to control rotation. This must be a 2D axis input, such as a joystick. The standard input for rotation is `y` axis of the primary 2D axis of the left controller, which is a thumbstick. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Left Interaction/Manipulation`. |
| **Directional Input** | The input used to control the direction of rotation. This must be a 2D axis input, such as a joystick. The standard input for teleportation direction is the primary 2D axis of the right controller, which is a thumbstick. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Right Interaction/Teleportation`. |

#### Rotate Mode {#rotate-mode}

Choose the way that a selected object rotates.

| **Option** | **Description** |
| :--- | :--- |
| **Rotate Over Time**| The interactable object rotates continuously based on the input from the **Rotate Input** property. The speed of rotation is determined by the value of the **Rotate Speed** property. In the standard toolkit interaction setup, **Rotate Over Time** mode is used for manipulating selected interactable objects with the left controller thumbstick.|
| **Match Direction**| The interactable object rotates to match the direction of the input from the **Directional Input** property. In the standard toolkit interaction setup, **Match Direction** mode is used for choosing the destination direction for teleportation with the right controller thumbstick.|

### Scale properties

Use the Scale properties to adjust how the attached object turns while selected.

In the standard toolkit interaction setup, the left controller is used for both scaling and for translation/rotation. The user toggles between scaling and translation/rotation with a button press (as specified with the **Scale Toggle Input** property).

| **Property** | **Description** |
| :--- | :--- |
| [Scale Mode](#scale-mode) | Specifies how input is applied when scaling objects. You can change the scale mode at runtime to enable or disable scaling.
| **Scale Speed** | The maximum speed at which the interactable object can be scaled, specified in meters per second. This property is only applicable when **Scale Mode** is set to **Scale Over Time**. |
| **Scale Input** | The input used to control scaling. This must be a 2D axis input, such as a joystick. The standard input for scaling is the `z` axis of the primary 2D axis of the left controller, which is a thumbstick. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Left Interaction/Manipulation`. |
| **Scale Toggle Input** | The input used to toggle between scaling and translation/rotation manipulation. This can be a button press or a float value. The standard input for toggling scaling is the primary button on the left controller, which is typically the X button. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Left Interaction/Select`. |
| **Scale Over Time Input** | The input used to control scaling over time. This must be a 2D axis input, such as a joystick. The standard input for scaling over time is the `z` axis of the primary 2D axis of the left controller, which is a thumbstick. In the [XRI Default Input Actions](xref:xri-samples-starter-assets#input-actions-asset) asset, this is mapped to the action named `XRI Left Interaction/Manipulation`. |
| **Scale Distance Delta Input** | The input used for scaling. This must be a float value representing the change from the previous frame |

#### Scale Mode {#scale-mode}

Choose the way that a selected object scales.

| **Option** | **Description** |
| :--- | :--- |
| **None** | Scaling is disabled.|
| **Scale Over Time** | The interactable object scales continuously based on the input from the **Scale Input** input reader property. The speed of scaling is determined by the value of the **Scale Speed** property. |
| **Distance Delta** | The interactable object scales based on a delta value from the previous frame. The delta value in a frame is read from  the **Scale Distance Delta Input** input reader property. |
