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

# Architecture notes
`TrackedDeviceGraphicRaycaster` uses two distinct result collections during a UI raycast pass:

- `allRaycastResults` (shared, aggregated output)

- `allRaycastResults` is the (`RaycastResult`)List (whose parameter in the `Raycast` function is named `resultAppendList`) provided by the Unity EventSystem during EventSystem.RaycastAll. This list is shared across all active raycasters for the current frame/event, and each `raycaster` is expected to append its own RaycastResult entries into it. The EventSystem later uses the aggregated results to determine hover, press, drag, click, and other UI event routing.

Due to `allRaycastResults` being shared and appended-to by multiple raycasters, its contents and ordering must not be assumed to belong to a single raycaster (e.g., `allRaycastResults`[0] may be a hit produced by a different raycaster).

- `m_RaycasterLocalHitDataBuffer` per-raycaster authoritative hit data

- `m_RaycasterLocalHitDataBuffer` is a per-instance working buffer containing `RaycastHitData` entries computed by this specific `TrackedDeviceGraphicRaycaster`. It is cleared and repopulated each cast (ray or spherecast) and used to sort candidate Graphic hits for this raycaster’s Canvas.

For poke interaction, `m_RaycasterLocalHitDataBuffer`[0] represents this raycaster’s top-ranked hit and is used as the authoritative target when evaluating poke state and selection via `XRPokeLogic` (passing the correct pokedTransform into `MeetsRequirementsForSelectAction`). This avoids relying on the shared `allRaycastResults` list for determining the “first hit,” which can be incorrect when multiple canvases/raycasters are active.
