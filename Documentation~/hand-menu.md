---
uid: xri-hand-menu
---
# Hand Menu

Makes a GameObject follow a tracked hand or motion controller with logic for setting visibility of the menu based on the palm orientation. This can be used, for example, to show a preferences menu when the user is looking at their palm.

|**Property**|**Description**|
|---|---|
|**Hand Menu UI GameObject** | Child GameObject used to hold the hand menu UI. This is the transform that moves each frame.|
|**Menu Handedness** | Which hand should the menu anchor to.<br/><ul><li>**None** to make the menu not follow either hand. Effectively disables the hand menu.</li><li>**Left** to make the menu follow the left hand.</li><li>**Right** to make the menu follow the right hand.</li><li>**Either** to make the menu follow either hand, choosing the first hand that satisfies requirements.</li></ul>|
|**Hand Menu Up Direction**|Determines the up direction of the menu when the hand menu is looking at the camera.<br/><ul><li>**World Up** to use the global world up direction (`Vector3.up`).</li><li>**Transform Up** to use this GameObject's world up direction (`Transform.up`). Useful if this component is on a child GameObject of the XR Origin and the user can teleport to walls.</li><li>**Camera Up** to use the main camera up direction. The menu will stay oriented with the head when the user tilts their head left or right.</li></ul>|
|**Left Palm Anchor** | Anchor associated with the left palm pose for the hand.|
|**Left Offset Child Anchor** | Offset from the left palm anchor where the UI should sit.|
|**Right Palm Anchor** | Anchor associated with the right palm pose for the hand.|
|**Right Offset Child Anchor** | Offset from the right palm anchor where the UI should sit.|
|**Follow Speed Multiplier** | Multiplier for delta time used when computing position and rotation tweens for this hand menu.|
|**Min Follow Distance** | Minimum distance in meters from target before which tween starts.|
|**Max Follow Distance** | Maximum distance in meters from target before tween targets, when time threshold is reached.|
|**Min To Max Delay Seconds** | Time required to elapse before the max distance allowed goes from the min distance to the max.|
|**Hide Menu When Gaze Diverges** | If true, menu will hide when gaze to menu origin's divergence angle is above the threshold. In other words, the menu will only show if looking roughly in it's direction.|
|**Menu Visible Gaze Divergence Threshold** | Only show menu if gaze to menu origin's divergence angle is below this value.|
|**Menu Hide Gaze Divergence Threshold Buffer** | An angle, in degrees, that is subtracted from the gaze divergence angle to create the threshold used to trigger hiding the menu.|
|**Animate Menu Hide And Reveal** | Should the menu animate when it is revealed or hidden.|
|**Reveal Hide Animation Duration** | Duration of the reveal/hide animation in seconds.|
|**Hide Menu On Select** | Should the menu hide when a selection is made with the hand for which the menu is anchored to.|
|**Interaction Manager** | XR Interaction Manager used to determine if a hand is selecting. Will find one if not set. Used for Hide Menu On Select.|
|**Hand Tracking Follow Preset** | The Follow Preset Datum used to define how the hand menu performs while hands are being tracked.|
|**Controller Follow Preset** | The Follow Preset Datum used to define how the hand menu performs while controllers are being tracked.|

## Follow Preset Datum
Defines the configuration of a following behaviour for a hand or object. It determines how an object should follow the hand and includes specifications about local position and rotation, angle constraints, gaze snapping, and smoothing settings.

|**Property**|**Description**|
|---|---|
|**Right Hand Local Position** | Local space anchor position for the right hand.|
|**Left Hand Local Position** | Local space anchor position for the left hand.|
|**Right Hand Local Rotation** | Local space anchor rotation for the right hand.|
|**Left Hand Local Rotation** | Local space anchor rotation for the left hand.|
|**Palm Reference Axis** | Reference axis equivalent used for comparisons with the user's gaze direction and the world up direction.|
|**Invert Axis For Right Hand** | Given that the default reference hand for menus is the left hand, it may be required to mirror the reference axis for the right hand.|
|**Require Palm Facing User** | Whether or not to check if the palm reference axis is facing the user.|
|**Palm Facing User Degree Angle Threshold** | The angle threshold in degrees to check if the palm reference axis is facing the user.|
|**Require Palm Facing Up** | Whether or not to check if the palm reference axis is facing up.|
|**Palm Facing Up Degree Angle Threshold** | The angle threshold in degrees to check if the palm reference axis is facing up.|
|**Hide Delay Seconds** | The amount of time in seconds to wait before hiding the following element after the hand is no longer tracked.|
|**Palm Facing User Hide Menu Angle Threshold Delta** | The additional threshold angle, in degrees, used as a buffer to hide the menu when the palm is facing the user.|
|**Palm Facing Up Hide Menu Angle Threshold Delta** | The additional threshold angle, in degrees, used as a buffer to hide the menu when the palm is facing up.|
|**Snap To Gaze** | Whether to snap the following element to the gaze direction.|
|**Snap To Gaze Angle Threshold** | The angle threshold in degrees to snap the following element to the gaze direction.|
|**Allow Smoothing** | Whether to allow smoothing of the following element position and rotation.|
|**Follow Lower Smoothing Value** | The lower bound of smoothing to apply.|
|**Follow Upper Smoothing Value** | The upper bound of smoothing to apply.|
