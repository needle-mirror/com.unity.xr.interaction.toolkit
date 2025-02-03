---
uid: xri-haptic-impulse-player
---
# Haptic Impulse Player

Send haptic impulses to a device.

Use this component to manage the connection to haptic hardware. It acts as a bridge, allowing other components, such as [Simple Haptic Feedback](xref:xri-simple-haptic-feedback), to trigger vibrations on a specific device.

![HapticImpulsePlayer component](images/haptic-impulse-player.png)

## Haptic Impulse Player properties

The Haptic Impulse Player component contains the following properties:

| **Property** | **Description** |
| :--- | :--- |
| **Haptic Output** | Specifies the output haptic control or controller that haptic impulses are sent to. |
| **Amplitude Multiplier** | Sets a multiplier that dampens all haptic impulses this component sends. A value of 1 maintains the current amplitude. |

## Additional resources

* [`HapticImpulsePlayer` class](xref:UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticImpulsePlayer)
* [`IXRHapticImpulseProvider`](xref:UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.IXRHapticImpulseProvider)
* [Component index](xref:xri-components)
