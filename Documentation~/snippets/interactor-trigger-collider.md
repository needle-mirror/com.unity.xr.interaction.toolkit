<!-- direct interactor, socket interactor

## Supporting components

[!INCLUDE [interactor-trigger-collider](snippets/interactor-trigger-collider.md)]
-->

* [Collider](xref:UnityEngine.Collider): a Collider instance set to [isTrigger](xref:UnityEngine.Collider.isTrigger) must be present on the same GameObject.

Refer to the Unity Manual for an [introduction to collision](xref:um-colliders-overview) for trigger colliders and how to [configure trigger collisions](xref:um-collider-interactions-create-trigger).

### Limitations with Physics settings

Unity 6.0 LTS and newer supports setting Physics settings to disable **Generate On Trigger Stay Events** in the [Physics Project Settings](xref:um-class-physics-manager). Disabling that option may improve performance since Unity will not generate [`OnTriggerStay`](xref:um-collider-interactions-other-events) events and is the recommended setting. However, doing so will introduce some minor limitations with this component. Unity does not trigger an `OnTriggerExit` event when a collider is disabled or its GameObject is deactivated. When enabled in Physics Project Settings, this component uses `OnTriggerStay` to detect when this occurs and properly update the list of colliders the interactor is touching each physics frame. But if this method is disabled in the Physics settings, the interactor can only rely on the `OnTriggerEnter` and `OnTriggerExit` events along with polling the enabled state of colliders.

If any of the trigger colliders on this interactor becomes disabled after touching another collider, such as by disabling it or deactivating the GameObject, the internal state maintaining the list of touching colliders will be fully cleared by this component. This is done to prevent interactables from being stuck as a valid target even when they are no longer touching the interactor's triggers. This means that if the interactor has multiple trigger colliders, disabling one of them will clear the valid targets even if the interactable is still touching an enabled trigger collider. Depending on which remaining enabled colliders are touching, the user would need to remove from the bounds of the trigger colliders and enter again for the `OnTriggerEnter` to fire to make it a valid target for hover and select. If the interactor only has a single trigger collider, you can avoid that limitation when Generate On Trigger Stay Events is disabled.

> [!NOTE]
> The list of trigger colliders is captured during `OnEnable` of the interactor. If you add colliders to the interactor during runtime after that method, you will need to toggle the enabled state of the component to retrigger `OnEnable` for the list to be updated to support that behavior.

This component will similarly check if any of the touching interactable colliders have been disabled and treat it like an exit event with that collider. Unlike disabling the interactor's trigger colliders, this will only remove that collider rather than fully clearing the valid targets. Both of these checks of the enabled state of colliders are done during the `yield WaitForFixedUpdate` execution phase. Refer to [Event function execution order](xref:um-execution-order) for more information.
