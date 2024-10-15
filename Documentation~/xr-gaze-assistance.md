---
uid: xri-xr-gaze-assistance
---
# XR Gaze Assistance

Allow specified ray interactors to fallback to eye-gaze when they are off screen or pointing off screen. This component enables split interaction functionality to allow the user to aim with eye gaze and select with a controller.

The component also enables aim assistance to help thrown objects move toward the interactable object at which the user is looking. On being thrown, a GrabInteractable object receives a velocity boost aiming it toward the target. (If the user is not looking at the target, the object is directed at the endpoint of the gaze ray.)

To use gaze assistance, add an **XR Gaze Assitance** component to a GameObject in the scene. (The component is already present on the root GameObject of the **XR Origin (XR Rig)** prefab included in the [Starter assets](xref:xri-samples-starter-assets).) You must set the **Gaze Interactor** property or the component deactivates itself at runtime.

Add any ray interactors that you want to assist with eye gaze, to the **Ray Interactors** list in the component.

The **Aim Assist** properties are used by any [XR Grab Interactable](xref:xri-xr-grab-interactable) that has its **Throw On Detach** option enabled. The current endpoint of the gaze interactor ray is used as the target when computing the velocity boost.

![XRGazeAssistance component](images/xr-gaze-assistance.png)

| **Property** | **Description** |
|---|---|
| **Gaze Interactor** | Eye data source - used as fallback data and to determine if fallback is necessary. |
| **Fallback Divergence** | The angle at which fallback data will be used instead of the original interactor ray. |
| **Hide Cursor With No Active Rays** | If the eye reticle should be hidden when all interactors are using their original data. |
| **Ray Interactors** | Interactors that can fall back to gaze data. |
| &emsp;**Interactor** | Instance of a Ray Interactor that can fallback to gaze data. |
| &emsp;**Teleport Ray** | Set to true if the Ray Interactor is used for teleportation. |
| **Aim Assist Required Angle** | How far thrown objects can aim outside of eye gaze and still be considered for aim assist. |
| **Aim Assist Required Speed** | The minimum speed in meters per second at which a thrown object must be moving to be considered for aim assist. |
| **Aim Assist Percent** | How much of the corrected aim velocity to use, as a value between 0 and 1. A value of 0 provides no assistance, a value of 0.5 mixes the original and boost velocities equally, and a value of 1.0 uses only the aim assitance boost (which auto-hits the target, if possible). |
| **Aim Assist Max Speed Percent** | The maximum boost a projectile can receive from aim assistance, as a multiple of the original speed. A value of 0.25 would mean that the added boost would be no more than one-quarter of the object's original speed. A value of 10 would mean that the boost could be up to ten times the original speed.|

> [!TIP]
> To provide your own aim assist algorithm, create a component that implements the [IXRAimAssist](xref:UnityEngine.XR.Interaction.Toolkit.Gaze.IXRAimAssist) interface. Place this component on the same GameObject as the interactor object that throws the object. (A grab interactable finds the closest `IXRAimAssist` object on or above the interactor's parent GameObject.)
