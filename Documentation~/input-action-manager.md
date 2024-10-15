---
uid: xri-input-action-manager
---

# Input Action Manager

This component automatically enables or disables all the inputs of type `InputAction` in a list of assets of type `InputActionAsset`.

The **Input Action Manager** is a convenient way to enable actions that you use in a scene. (Input actions must be enabled before they can be used.) You can also use this component to turn sets of actions on or off depending on context. Refer to [Enabling actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/Actions.html#enabling-actions) for more information about how to turn input actions on or off.

![InputActionManager component](images/input-action-manager.png)

| **Property** | **Description** |
|---|---|
| **Action Assets** | The `InputActionAsset`s this component automatically enables and disables in response to `OnEnable` and `OnDisable` from `MonoBehaviour`. |
