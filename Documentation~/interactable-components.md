---
uid: xri-interactable-components
---

# Interactable components

The interactable components provided by the toolkit.

| **Topic**             | **Description**         |
| :-------------------- | :----------------------- |
| [XR Grab Interactable](xr-grab-interactable.md)               | An interactable that is picked up when selected.|
| [XR Simple Interactable](xr-simple-interactable.md)           | An interactable that changes state when interacted with, but does not have any built-in behavior. |

Helper components that modify interactable object visuals or behavior.

| **Topic**             | **Description**         |
| :-------------------- | :----------------------- |
| [XR Interactable Snap Volume](xr-interactable-snap-volume.md) | Allows the line from an XR Ray Interactor to snap to an interactable.|
| [XR Tint Interactable Visual](xr-tint-interactable-visual.md) | Changes the tint of an interactable based on the current hover or select state. |

## Managing colliders at runtime

By default, an interactable discovers its colliders during `Awake` using `GetComponentsInChildren`. Trigger colliders are excluded from this automatic discovery since they are typically associated with snap volumes. If you need a trigger collider registered with the interactable, assign it directly in the **Colliders** list in the Inspector or use `RegisterCollider`.

When colliders are added to or removed from an interactable via code after it has registered with the [Interaction Manager](xref:xri-xr-interaction-manager), the collider-to-interactable mapping must be updated for interaction to work correctly. `XRBaseInteractable` provides the following methods for this purpose:

| **Method** | **Description** |
|---|---|
| `RegisterCollider(Collider)` | Adds a collider to the interactable's colliders list and registers it with the Interaction Manager's collider-to-interactable mapping. Use this when you have a reference to the collider to add. |
| `UnregisterCollider(Collider)` | Removes a collider from the interactable's colliders list and removes it from the Interaction Manager's mapping. Active hover and select interactions are re-evaluated on the next frame. |
| `RefreshColliders(bool)` | Re-scans the interactable's GameObject and its children, clears and repopulates the colliders list, and updates the Interaction Manager's mapping. Colliders not on the GameObject or its children will not be included. Colliders already registered with a different interactable are skipped. Use this when you don't have a reference to the specific collider that changed. |

These methods fire the `collidersChanged` event with `CollidersChangedEventArgs`, which allows other components (such as [XR Poke Filter](xref:xri-xr-poke-filter)) to react when colliders change. The event args include a reference to the interactable, a `ColliderUpdateType` indicating whether a collider was added, removed, or the full list was refreshed, and the specific collider involved (for add and remove operations).

### Example: Adding a collider at runtime

```csharp
// Adding a collider at runtime
var newCollider = interactable.gameObject.AddComponent<BoxCollider>();
newCollider.size = new Vector3(0.5f, 0.5f, 0.5f);

// Register the collider so the interaction system recognizes it.
interactable.RegisterCollider(newCollider);

// Later, unregister before destroying so the mapping is cleaned up.
interactable.UnregisterCollider(newCollider);
Destroy(newCollider);
```

> [!IMPORTANT]
> Adding a collider via `AddComponent` without calling `RegisterCollider` will not update the collider-to-interactable mapping. The collider will exist on the GameObject but will not be recognized by the interaction system. Modifying the `colliders` list directly also does not update the mapping.

> [!NOTE]
> The **Colliders** list in the Inspector is not editable during Play mode while the interactable is active. To modify colliders during Play mode, use the methods above in code, or disable the component first to edit the list in the Inspector. Re-enabling the component will re-register the updated colliders with the Interaction Manager.

For UI Toolkit world space panels that create colliders dynamically, refer to the [UI Toolkit support page](xref:xri-ui-world-space-ui-toolkit-support) for details on how this is handled automatically.

## Additional resources

* [Climb Interactable](climb-interactable.md)
* [AR Interactables](ar-interactable-components.md)
* [Component index](components.md)
