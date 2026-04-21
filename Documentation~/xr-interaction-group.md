---
uid: xri-xr-interaction-group
---
# XR Interaction Group

An interaction group is a mediator for interactors. A group contains multiple member interactors, sorted by priority, and only allows one interactor in the group to interact (hover or select) at a time. Groups first prioritize continuous selection - so if a member interactor was interacting the previous frame and can start or continue selection in the current frame, then that interactor will be chosen for interaction even if a higher priority interactor tries to interact.

You can configure interaction overrides for each member. An override group member is able to ignore priority and block the interaction of the active member if it is able to select any of the interactables the active member is interacting with. When this happens, the override member is chosen as the single member for interaction. Priority is still respected when multiple members try to override at the same time. As an example of how overrides could be used, you might want Poke interaction to normally take priority so that Direct hover doesn't prevent it, but you might still want to let a user grab (Direct select) an interactable that is being poked.

An interaction group can also contain other Groups in its sorted list of members. A group within a group is treated like a single interactor when it comes to how the containing group prioritizes interactions. The interactor chosen for interaction within a Group is bubbled up to the next containing group, until it is ultimately either chosen for interaction in the top-level group or skipped over just like other interactors.

Interaction groups also support the [focus interaction state](xref:xri-state-events#focus). An interactor must be part of an interaction group in order to give focus to an interactable. The [IXRInteractionGroup](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractionGroup) instance keeps track of the focused interactable for the group. (Focus-related properties are not shown in the Inspector of the **XR Interaction Group** component.)

![XRInteractionGroup component](images/xr-interaction-group.png)

| **Property** | **Description** |
|---|---|
| **Interaction Manager** | The [XRInteractionManager](xr-interaction-manager.md) that this Interaction Group will communicate with (will find one if **None**). |
| **Starting Group Members** | Ordered list of interactors or Interaction Groups that are registered with the Group on `Awake`. |
| **Interaction Override Configuration** | Configuration for each Group Member of which other Members are able to override its interaction when they attempt to select any of its hovered or selected interactables, despite the difference in priority order. Each checkbox indicates whether the Member corresponding to its column can override the Member corresponding to its row. |

> [!TIP]
> When running in the Editor play mode, you cannot change the configuration of **Starting Group Members** or their overrides after the Group has had its `Awake` method called.
