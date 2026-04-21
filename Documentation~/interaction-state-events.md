---
uid: xri-state-events
---

# Interaction state and events

An interaction between a user and a virtual object goes through four possible states, which can overlap. The interacting objects [dispatch events](#interaction-events) to report when the interaction enters and leaves a state. The interaction states include:

* [Hover](#hover): getting close to or pointing at it
* [Select](#select): grabbing it
* [Activate](#activate): using it
* [Focus](#focus): paying attention to it

**Hover**, **select**, and **focus** are similar to the traditional GUI concepts like mouse-over, mouse-down, and UI focus. **Activate** is a bit different and is entered and exited via a contextual command, such as the user pulling the trigger button of an XR controller.

The **hover** state usually precedes the **select** state. When a user **activates** an object it remains selected. **Focus** is maintained separately from other states -- a user must select an object for it to receive focus, but can then activate it, unselect it, and hover on it again without affecting the focus. Focus changes when the user selects a different object.

## States {#state}

The interaction states, **hover**, **select**, **activate**, and **focus**, can mean different things to different objects. The specific meaning is provided by the context of your application.

These interaction states always involve both an [Interactor](xref:xri-key-components#interactors) and [Interactable](xref:xri-key-components#interactables), and both can [dispatch events](#interaction-events) upon entering or exiting the state.

### Hover {#hover}

If an interactable becomes a valid target for an interactor its state changes to **hover**. Hovering on an object signifies the user's intention to interact with it, but doesn't typically change the behavior of that object. You can create a visual indicator for this change of state, such as changing the tint of the object or adding an outline, to provide a contextual clue to the user.

A hover state interaction ends when the interactor stops targeting the interactable.

More than one interactor can hover the same interactable at the same time. Entering the select state with one interactor does not end the hover state for other interactors.

### Select {#select}

Selection requires an action such as a button or trigger press from the user to enter the **select** state. You can use selection to begin a direct interaction, such as picking up a grabbable object, holding a lever, or pushing a UI button.

You can set an interactable's **Select Mode** property to specify whether it can be selected by more than one interactor at the same time.

### Focus {#focus}

An interactable becomes **focused** when it is selected by an interactor that is a member of an [interaction group](xref:xri-xr-interaction-group). This focus persists even after the interactable has exited the select and hover states.

You can use focus to let the user select an object and then continue the interaction with that object without needing to keep the select input activated. For example, you could let the user perform your select action to choose an object and then let them release selection while they change the object's properties on a separate palette window -- an operation that might be difficult if the user had to maintain selection the entire time.

You can set an interactable's **Focus Mode** property to specify whether it can be focused by more than one interaction group at the same time.

The focus state ends when:

* The interactor with focus selects a different interactable.
* The interactor with focus selects "nothing" -- for example, the user activates the select input when not hovering.
* An interactor in the same interaction group selects the focused interactable. In this case, the interactable retains focus, but the focusing interactor changes.
* An interactor in the same interaction group selects a different interactable.
* A different interactor selects a focused interactable whose **Focus Mode** is **Single**.

> [!IMPORTANT]
> An interactor must be part of an [Interaction Group](xref:xri-xr-interaction-group) in order to focus an interactable object. The interaction group maintains the focus state for its member interactors.

### Activate {#activate}

Activation is an extra action, typically mapped to a button or trigger that affects the currently selected object. You can use **activate** to let the user further use or interact with an object they've selected.

The activate behavior depends on the interactable. For example, you can use activate to toggle a grabbable flashlight on and off or to shoot a ball from a launcher.

Activate events are only dispatched by interactable objects. The object is in the active state while the selecting interactor's **Activate Input** is engaged.

## Interaction events {#interaction-events}

**Interaction events** are dispatched when an interaction state changes. Both the interactor and the interactable objects dispatch events for the same interaction, though interactables have additional events compared to interactors.

You can use interaction states and their related events to update your scene objects based on the user's actions. For example, you can use the hover state and events to highlight which interactable object will be selected when the user squeezes the grip button (or whichever button you have bound to the select action). Many of the concepts of good 2D UI design apply to 3D interaction design and the states and events provided by the XR Interaction Toolkit are intentionally similar to their 2D analogs.

**Hover**, **select**, and **focus** interaction events have the following variants:

* **First Event Entered**: signals when the first interactor hovers, selects, or focuses an interactable. Dispatched by interactable objects only. An Entered event is also dispatched.
* **Event Entered**: signals that an interactor has started an interaction. Dispatched by both the interactor and the interactable objects involved in the interaction.
* **Event Exited**: signals that an interactor has ended interaction. Dispatched by both the interactor and the interactable objects involved in the interaction.
* **Last Event Exited**: signal when the last interactor stops hovering, selecting, or focusing an interactable. Dispatched by interactable objects only. An Exited event is also dispatched.

For example, the following events can be dispatched for the hover interaction state: **First Hover Entered**, **Hover Entered**, **Hover Exited**, and **Last Hover Exited**. When there are multiple interactors hovering over the same interactable, the **Hover Entered** and **Hover Exited** events will be invoked as each interactor hovers over or away from the interactable, but **First Hover Entered** will only be invoked when the interactable wasn't being hovered by any interactor, and **Last Hover Exited** will only be invoked once the interactable is no longer hovered by any interactor.

The **activate** interaction events are only dispatched by interactables and have two variants:

* **Activate**: signals when the user triggers the activation input of an interactor that is currently selecting the interactable.
* **Deactivate**: signals when the user releases the activation input of an interactor that is currently activating the interactable.
