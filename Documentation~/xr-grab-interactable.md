---
uid: xri-xr-grab-interactable
---
# XR Grab Interactable

Interactable component that allows for basic grab functionality. When this behavior is selected (grabbed) by an Interactor, this behavior will follow it around and inherit velocity when released.

![XRGrabInteractable component](images/xr-grab-interactable.png)

| **Property** | **Description** |
|---|---|
| **Interaction Manager** | The [XRInteractionManager](xr-interaction-manager.md) that this Interactable will communicate with (will find one if **None**). |
| **Interaction Layer Mask** | Allows interaction with Interactors whose [Interaction Layer Mask](interaction-layers.md) overlaps with any Layer in this Interaction Layer Mask. |
| **Colliders** | Colliders to use for interaction with this Interactable (if empty, will use any child Colliders). |
| **Distance Calculation Mode** | Specifies how distance is calculated to Interactors, from fastest to most accurate. If using Mesh Colliders, Collider Volume only works if the mesh is convex. |
| &emsp;Transform Position | Calculates the distance using the Interactable's transform position. This option has low performance cost, but it may have low distance calculation accuracy for some objects. |
| &emsp;Collider Position | Calculates the distance using the Interactable's Colliders list using the shortest distance to each. This option has moderate performance cost and should have moderate distance calculation accuracy for most objects. |
| &emsp;Collider Volume | Calculates the distance using the Interactable's Colliders list using the shortest distance to the closest point of each (either on the surface or inside the Collider). This option has high performance cost but high distance calculation accuracy. |
| **Custom Reticle** | The reticle that appears at the end of the line when valid. |
| **Select Mode** | Indicates the selection policy of an Interactable. This controls how many Interactors can select this Interactable.<br />The value is only read by the Interaction Manager when a selection attempt is made, so changing this value from **Multiple** to **Single** will not cause selections to be exited. |
| &emsp;Single | Set **Select Mode** to **Single** to prevent additional simultaneous selections from more than one Interactor at a time. |
| &emsp;Multiple | Set **Select Mode** to **Multiple** to allow simultaneous selections on the Interactable from multiple Interactors. |
| **Focus Mode** | Specifies the [focus](architecture.md#states) policy of this interactable. |
| &emsp;None | Set **Focus Mode** to **None** to disable the foucs state for an interactable. |
| &emsp;Single | Set **Focus Mode** to **Single** to allow interactors of a single [interaction group](architecture.md#interaction-groups) to focus this interactable. |
| &emsp;Multiple | Set **Focus Mode** to **Multiple** to allow interactors of multiple [interaction groups](architecture.md#interaction-groups) to focus this interactable. |
| **Allow Gaze Interaction** | Enable for basic interaction events from an [XRGazeInteractor](xr-gaze-interactor.md) and other gaze features. |
| **Allow Gaze Select** | Enable selection from an [XRGazeInteractor](xr-gaze-interactor.md). |
| **Override Gaze Time To Select** | Enables this Interactable to override the hover to select time on an [XRGazeInteractor](xr-gaze-interactor.md). |
| **Gaze Time To Select** | Number of seconds an [XRGazeInteractor](xr-gaze-interactor.md) must hover this interactable to select it if **Hover To Select** is enabled on the gaze Interactor. |
| **Override Time To Auto Deselect** | Enables this Interactable to override the auto deselect time on an [XRGazeInteractor](xr-gaze-interactor.md). |
| **Time To Auto Deselect** | Number of seconds this Interactable will be selected by an [XRGazeInteractor](xr-gaze-interactor.md) before being automatically deselected if **Auto Deselect** is enabled on the gaze Interactor. |
| **Allow Gaze Assistance** | If enabled, an [XRGazeInteractor](xr-gaze-interactor.md) will place an [XRInteractableSnapVolume](xr-interactable-snap-volume.md) at this interactable to allow a properly configured [XRRayInteractor](xr-ray-interactor.md) to snap to this interactable. See the [XR Interactable Snap Volume](xr-interactable-snap-volume.md) or [XR Ray Interactor](xr-ray-interactor.md) pages for further information about correctly configuring an `XRRayInteractor` to support an `XRInteractableSnapVolume`. |
| **Movement Type** | Specifies how this object moves when selected, either through setting the velocity of the `Rigidbody`, moving the kinematic `Rigidbody` during Fixed Update, or by directly updating the `Transform` each frame. |
| &emsp;Velocity Tracking | Set **Movement Type** to Velocity Tracking to move the Interactable object by setting the velocity and angular velocity of the Rigidbody. Use this if you don't want the object to be able to move through other Colliders without a Rigidbody as it follows the Interactor, however with the tradeoff that it can appear to lag behind and not move as smoothly as Instantaneous. |
| &emsp;Kinematic | Set **Movement Type** to Kinematic to move the Interactable object by moving the kinematic Rigidbody towards the target position and orientation. Use this if you want to keep the visual representation synchronized to match its Physics state, and if you want to allow the object to be able to move through other Colliders without a Rigidbody as it follows the Interactor. |
| &emsp;Instantaneous | Set **Movement Type** to Instantaneous to move the Interactable object by setting the position and rotation of the Transform every frame. Use this if you want the visual representation to be updated each frame, minimizing latency, however with the tradeoff that it will be able to move through other Colliders without a Rigidbody as it follows the Interactor. |
| **Predicted Visuals Transform** | Optional child GameObject for this component to drive the visuals of this Interactable between physics updates. Not used with Instantaneous. |
| **Limit Linear Velocity** | Whether to limit the linear velocity applied to the Rigidbody. |
| **Max Linear Velocity Delta** | The maximum linear velocity that Unity will apply to the Rigidbody each physics frame (and the optional predicted visuals if used). |
| **Limit Angular Velocity** | Whether to limit the angular velocity applied to the Rigidbody. |
| **Max Angular Velocity Delta** | The maximum angular velocity in radians per second that Unity will apply to the Rigidbody each physics frame (and the optional predicted visuals if used). |
| **Retain Transform Parent** | Enable to have Unity set the parent of this object back to its original parent this object was a child of after this object is dropped. |
| **Track Position** | Enable to have this object follow the position of the Interactor when selected. |
| **Smooth Position** | Enable to have Unity apply smoothing while following the position of the Interactor when selected. |
| **Smooth Position Amount** | Scale factor for how much smoothing is applied while following the position of the Interactor when selected. The larger the value, the closer this object will remain to the position of the Interactor. |
| **Tighten Position** | Reduces the maximum follow position difference when using smoothing.<br />Fractional amount of how close the smoothed position should remain to the position of the Interactor when using smoothing. The value ranges from 0 meaning no bias in the smoothed follow distance, to 1 meaning effectively no smoothing at all. |
| **Velocity Damping** | Scale factor of how much to dampen the existing linear velocity when tracking the position of the Interactor. The smaller the value, the longer it takes for the velocity to decay.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Velocity Scale** | Scale factor Unity applies to the tracked linear velocity while updating the `Rigidbody` when tracking the position of the Interactor.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Track Rotation** | Enable to have this object follow the rotation of the Interactor when selected. |
| **Smooth Rotation** | Apply smoothing while following the rotation of the Interactor when selected. |
| **Smooth Rotation Amount** | Scale factor for how much smoothing is applied while following the rotation of the Interactor when selected. The larger the value, the closer this object will remain to the rotation of the Interactor. |
| **Tighten Rotation** | Reduces the maximum follow rotation difference when using smoothing.<br />Fractional amount of how close the smoothed rotation should remain to the rotation of the Interactor when using smoothing. The value ranges from 0 meaning no bias in the smoothed follow rotation, to 1 meaning effectively no smoothing at all. |
| **Track Scale** | Enable to have this object follow the scale of the Interactor when selected. |
| **Smooth Scale** | Apply smoothing while following the scale of the Interactor when selected. |
| **Smooth Scale Amount** | Scale factor for how much smoothing is applied while following the scale of the interactable when selected. The larger the value, the closer this object will remain to the scale of the Interactor. |
| **Tighten Scale** | Reduces the maximum follow scale difference when using smoothing.<br />Fractional amount of how close the smoothed scale should remain to the scale of the Interactor when using smoothing. The value ranges from 0 meaning no bias in the smoothed follow scale, to 1 meaning effectively no smoothing at all. |
| **Angular Velocity Damping** | Scale factor of how much Unity dampens the existing angular velocity when tracking the rotation of the Interactor. The smaller the value, the longer it takes for the angular velocity to decay.<br />Only applies when **Movement Type** is in _VelocityTracking_ mode. |
| **Angular Velocity Scale** | Scale factor Unity applies to the tracked angular velocity while updating the `Rigidbody` when tracking the rotation of the Interactor.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Throw On Detach** | Enable to have this object inherit the velocity of the Interactor when released. This is not supported for a kinematic Rigidbody. |
| **Throw Smoothing Duration** | This value represents the time over which collected samples are used for velocity calculation (up to a max of 20 previous frames, which is dependent on both Smoothing Duration and framerate). As an example, if this value is set to 0.25, position and velocity values will be averaged over the past 0.25 seconds. Each of those values is weighted (multiplied) by the Throw Smoothing Curve as well. |
| **Throw Smoothing Curve** | The curve used to weight velocity smoothing upon throwing (most recent frames to the right). By default this curve is flat with a 1.0 value so all smoothing values are treated equally across the smoothing duration. |
| **Throw Velocity Scale** | Scale factor Unity applies to this object's linear velocity inherited from the Interactor when released. |
| **Throw Angular Velocity Scale** | Scale factor Unity applies to this object's angular velocity inherited from the Interactor when released. |
| **Force Gravity On Detach** | Forces this object to have gravity when released (will still use pre-grab value if this is `false` / unchecked). |
| **Attach Transform** | The attachment point Unity uses on this Interactable (will use this object's position if none set). |
| **Secondary Attach Transform** | A second attachment point to use on this Interactable for two-handed interaction. (Unity uses the second interactor's attach transform if you don't set this property). |
| **Far Attach Mode** | Determines how the interactor's attachment point is adjusted on far select. This typically results in whether the interactable stays distant at the far hit point or moves to the near hand. |
| **Use Dynamic Attach** | Enable to make the effective attachment point based on the pose of the Interactor when the selection is made. |
| **Match Position** | Match the position of the Interactor's attachment point when initializing the grab. This will override the position of Attach Transform. |
| **Match Rotation** | Match the rotation of the Interactor's attachment point when initializing the grab. This will override the rotation of Attach Transform. |
| **Snap To Collider Volume** | Adjust the dynamic attachment point to keep it on or inside the Colliders that make up this object. |
| **Reinitialize Every Single Grab** | Re-initialize the dynamic attachment pose when changing from multiple grabs back to a single grab. Use this if you want to keep the current pose of the object after releasing a second hand rather than reverting back to the attach pose from the original grab. |
| **Attach Ease In Time** | Time in seconds Unity eases in the attach when selected (a value of 0 indicates no easing). |
| **Add Default Grab Transformers** | Whether Unity will add the default set of grab transformers if either the Single or Multiple Grab Transformers lists are empty. |
| **Starting Multiple Grab Transformers** | The grab transformers that this Interactable automatically links at startup (optional, may be empty). Used for multi-interactor selection. After startup, this property is not used. Useful when there is more than one Grab Transformer that should be processed and you need to specify the order. |
| **Starting Single Grab Transformers** | The grab transformers that this Interactable automatically links at startup (optional, may be empty). Used for single-interactor selection. After startup, this property is not used. Useful when there is more than one Grab Transformer that should be processed and you need to specify the order. |
| **Multiple Grab Transformers** | (Play mode only) The grab transformers used when there are multiple interactors selecting this object. |
| **Single Grab Transformers** | (Play mode only) The grab transformers used when there is a single interactor selecting this object. |
| **Interactable Filter** | Assign [IXRHoverFilter(xref:UnityEngine.XR.Interaction.Toolkit.Filtering.IXRHoverFilter), [IXRSelectFilter(xref:UnityEngine.XR.Interaction.Toolkit.Filtering.IXRSelectFilter), and [IXRInteractionStrengthFilter(xref:UnityEngine.XR.Interaction.Toolkit.Filtering.IXRInteractionStrengthFilter) objects to an Interactable.|
| **Interactable Events** | Assign event handlers to events dispatched by an Interactable. Refer to the [Interactable Events](interactable-events.md) page for more information. |

## Grab transformers

This XR Grab Interactable behavior is responsible for applying the position, rotation, and local scale calculated by one or more [IXRGrabTransformer](xref:UnityEngine.XR.Interaction.Toolkit.Transformers.IXRGrabTransformer) implementations. The (xref:UnityEngine.XR.Interaction.Toolkit.Transformers.XRGeneralTransformer) grab transformer is automatically added by Unity (when **Add Default Grab Transformers** is enabled), but this functionality can be disabled to manually set the grab transformers used by this behavior, allowing you to customize how this component determines where the object should move and rotate to. This default grab transformer also comes with a set of configurable options to allow axis constraints for translation, two-handed rotation, and two-handed scaling (which is disabled by default).

Grab transformer components can be added to the GameObject to link them with the XR Grab Interactable. They can be found in the **Component** &gt; **XR** &gt; **Transformers** menu. You can then add references to those components explicitly to **Starting Single Grab Transformers** or **Starting Multiple Grab Transformers** if you have more than one and need to specify the order in which they execute, or if you need to override which list the grab transformer is automatically added to.

<a id="stutter"></a>
## Reducing stutter from physics update rate

When the Movement Type is set to Instantaneous, the GameObject is moved by directly moving the Transform every frame to the target pose returned by the [grab transformers](#grab-transformers). However, the other Movement Types will cause the GameObject to only move after `FixedUpdate` which can cause the motion of the object to be much less smooth than when the object is set to Instantaneous.

Unity provides a way for the Rigidbody component to smooth the appearance of movement stutter due to the mismatch between the physics update rate and the application's frame rate, which you can read more about in the Unity manual page [Apply interpolation to a Rigidbody](xref:um-rigidbody-interpolation). However, setting the Rigidbody component to Interpolate or Extrapolate comes with drawbacks when an object is grabbed in an XR application. Interpolation will smooth out motion to eliminate stutter, but it will appear to move slightly behind where it should be, which can be detrimental to immersion. This latency is amplified even more when the user is moving or turning with locomotion while grabbing the object.

### Predicted Visuals Transform

To reduce the detrimental effects of interpolation, the XR Grab Interactable supports assigning a **Predicted Visuals Transform** property which allows you to separate the Rigidbody and colliders from the visual representation of the interactable object. When this property is set and the Movement Type is set to Kinematic or Velocity Tracking the component will automatically update the visuals every frame while the Rigidbody is not colliding with anything. This is required because when Movement Type is set to Kinematic or Velocity Tracking, the computed target pose is only applied to the Rigidbody every `FixedUpdate`, which is typically lower than the application frame rate. By updating the visuals and physics separately in this way, the object appears to move smoothly every frame while still maintaining accurate and expected physics collisions.

A typical GameObject hierarchy of a prefab to make use of this feature:
* **Grab Interactable** (components: `Rigidbody`, `XRGrabInteractable`, `XRGeneralGrabTransformer`)
  * **Visuals** (one or more components of: `MeshFilter`, `MeshRenderer`, etc.)
    * Even more visual GameObjects...
  * **Collider** (one or more components of: `MeshCollider`, `BoxCollider`, etc.)
    * Even more collider GameObjects...
  * **Visual feedback or affordance** (components: `XRInteractableAffordanceStateProvider`, `MaterialPropertyBlockHelper`, `ColorMaterialPropertyAffordanceReceiver`, etc.)

In this example, note that the colliders are separated from the visuals. With this GameObject setup, the **Predicted Visuals Transform** can be set to the **Visuals** GameObject to make use of the smoother visuals while minimizing latency. The result is a physics-based Movement Type interactable that can have the same appearance as Instantaneous.

This can be combined with the Interpolation setting of the Rigidbody component to reduce stutter while the object is colliding or when dropped. The XR Grab Interactable component will automatically force it to None while driving the visual GameObject, and automatically revert it to the old Interpolation value when possible.

#### Limitations

The Predicted Visuals Transform does not replace physics. While the Rigidbody is colliding with something, the visuals stops being driven by the component and the movement of the object reverts to the same as if the property was not set. Thus there may still be stutter or the object may lag behind depending on the Interpolation setting of the Rigidbody even with this system.

There are several additional constraints/limitations of this system:
  * Predicted Visuals Transform must be a child GameObject of the XR Grab Interactable.
  * Predicted Visuals Transform must not have a collider component on that GameObject or any of its child GameObjects so that the colliders correctly updates in step with the Rigidbody and physics.
  * Predicted Visuals Transform must not have any intermediate parent GameObject between it and the XR Grab Interactable itself with a Transform offset. In other words, while there can be a GameObject between the Grab Interactable and the Visuals, it must have Position and Rotation set to all 0.
  * The XR Grab Interactable will only capture the initial Transform position and rotation offset of the Predicted Visuals Transform when the object is grabbed. It will overwrite the Transform values every frame while grabbed, and restore the original offset when dropped.
  * The XR Grab Interactable will automatically force the Interpolation setting of the Rigidbody to None while the Predicted Visuals Transform is being driven by the component.
  * When the Rigidbody is colliding or sleeping while grabbed, and upon being dropped, the XR Grab Interactable will automatically revert to the original Interpolation setting of the Rigidbody.
  * Target scale returned by the grab transformers is currently still only applied during `FixedUpdate` for Kinematic and Velocity Tracking movement types. This may change in a future version so scale changes can be applied every frame to the visuals between physics steps.
