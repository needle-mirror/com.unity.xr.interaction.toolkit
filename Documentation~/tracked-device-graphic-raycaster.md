---
uid: xri-tracked-device-graphic-raycaster
---

# Tracked Device Graphic Raycaster component

The **Tracked Device Graphic Raycaster** component lets you use 3D tracked devices to highlight and select UI elements on a canvas.

Add a **Tracked Device Graphic Raycaster** component to the same GameObject containing the [Canvas](https://docs.unity3d.com/Packages/com.unity.ugui@latest?subfolder=/manual/UICanvas.html) component.

| **Property** | **Description** |
|---|---|
| Ignore Reversed Graphics | When enabled, raycasts ignore graphics facing away from the source of the raycast. |
| Check for 2D Occlusion | Enable if raycasts should be blocked by 2D objects occluding the canvas. Objects must have a collider to block the raycast. Enabling this option incurs a performance cost. |
| Check for 3D Occlusion | Enable if raycasts should be blocked by 3D objects occluding the canvas. Objects must have a collider to block the raycast. Enabling this option incurs a performance cost.  |
| Blocking Mask | A [LayerMask](xref:layers-and-layermasks) identifying the layers containing objects that are allowed to occlude the canvas. |
| Raycast Trigger Interaction | Specifies whether the ray cast should be blocked by trigger colliders when checking for 3D occlusion. Refer to [QueryTriggerInteraction](xref:UnityEngine.QueryTriggerInteraction) for more information. |
