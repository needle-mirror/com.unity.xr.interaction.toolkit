---
uid: xri-interaction-attach-controller
---
# Interaction Attach Controller

This is the default implementation of `IInteractionAttachController` that can be used to control the attach transform for translation and rotation.
It is used in conjunction with the `NearFarInteractor` to handle interaction at a distance and stabilization of selected objects, while also implementing logic to pull objects at an accelerated speed towards the user.

This class also features a smoothing option that occurs relative to the `XROrigin`, enabling smooth motion for objects grabbed at a distance that remains consistent during locomotion.

Importantly the stabilization and smoothing options are configured by default to only affect objects that are grabbed at a distance (ie. objects that have an offset), which ensures that when transitioning to near interaction, objects move with minimal latency relative to the hand or controller visual representations.


| **Property**                    | **Description** |
|----------------------------------------|-----------------|
| **Transform To Follow**                | Gets or sets the transform that the anchor should follow. |
| **Motion Stabilization Mode**          | Gets or sets the stabilization mode for the motion of the anchor. Determines how the anchor's position and rotation are stabilized relative to the followed transform. |
| **Position Stabilization**             | Factor for stabilizing position. This value represents the maximum distance (in meters) over which position stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance. |
| **Angle Stabilization**                | Factor for stabilizing angle. This value represents the maximum angle (in degrees) over which angle stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle. |
| **Smooth Offset**                      | If true offset will be smoothed over time in XR Origin space. May present some instability if smoothing is toggled during an interaction. |
| **Smoothing Speed**                    | Smoothing amount for the anchor's position and rotation. Higher values mean more smoothing occurs faster. |
| **Use Distance Based Velocity Scaling**| Whether to use distance-based velocity scaling for anchor movement. |
| **Use Momentum**                       | Whether momentum is used when distance scaling is in effect. |
| **Momentum Decay Scale**               | Decay scalar for momentum. Higher values will cause momentum to decay faster. |
| **Z Velocity Ramp Threshold**          | Scales anchor velocity from 0 to 1 based on z-velocity's deviation below a threshold. 0 means no scaling. |
| **Pull Velocity Bias**                 | Adjusts the object's velocity calculation when moving towards the user. It modifies the distance-based calculation that determines the velocity scalar. |
| **Push Velocity Bias**                 | Adjusts the object's velocity calculation when moving away from the user. It modifies the distance-based calculation that determines the velocity scalar. |
| **Min Additional Velocity Scalar**     | Minimum additional velocity scaling factor for movement, interpolated by a quad bezier curve. |
| **Max Additional Velocity Scalar**     | Maximum additional velocity scaling factor for movement, interpolated by a quad bezier curve. |
