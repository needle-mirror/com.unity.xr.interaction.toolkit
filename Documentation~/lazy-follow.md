# Lazy Follow

Makes the GameObject this component is attached to follow a target with a delay and some other layout options.

| **Property** | **Description** |
|---|---|
| **Target** | (Optional) The object being followed. If not set, this will default to the main camera when this component is enabled. |
| **Target Offset** | The amount to offset the target's position when following. This position is relative/local to the target object. |
| **Movement Speed** | The laziness or smoothing that is applied to the follow movement. Higher values result in direct following, lower values will cause this object to follow more lazily. |
| **Snap On Enable** | Snap to target position when this component is enabled. |
| **Min Distance Allowed** | The min distance allowed within the time threshold which decides whether or not lazy follow capability is turned on. |
| **Max Distance Allowed** | The max distance allowed within the time threshold which decides whether or not lazy follow capability is turned on. |
| **Min Angle Allowed** | The min angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on. |
| **Max Angle Allowed** | The max angle offset allowed within the time threshold which decides whether or not lazy rotation capability is turned on. |
| **Time Until Threshold Reaches Max Distance** | The time threshold (in seconds) where if max distance is reached the lazy follow capability will not be turned off. |
| **Time Until Threshold Reaches Max Angle** | The time threshold (in seconds) where if max angle change is reached the lazy follow capability will not be turned off. |