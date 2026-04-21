---
uid: xri-interaction-event-handling
---

# Handle interaction events

Interaction events extend [UnityEvent](xref:UnityEngine.Events.UnityEvent) and are shown in the Inspector window. You can assign listener functions for interaction events in the [Inspector window](xref:um-unity-events) and in a method of a custom C# class.

## Event arguments

All interaction event arguments inherit from [`BaseInteractionEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.BaseInteractionEventArgs) and include the following properties:

- [`interactorObject`](xref:UnityEngine.XR.Interaction.Toolkit.BaseInteractionEventArgs.interactorObject): The [`IXRInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor) that is involved in the interaction.
- [`interactableObject`](xref:UnityEngine.XR.Interaction.Toolkit.BaseInteractionEventArgs.interactableObject): The [`IXRInteractable`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable) that is involved in the interaction.

Many interaction event subclasses implement a [`manager`](xref:UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs.manager) property, which references the [`XRInteractionManager`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager) that is mediating the interaction.

Exit event arguments (like [`SelectExitEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs), [`HoverExitEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.HoverExitEventArgs)) also include an [`isCanceled`](xref:UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs.isCanceled) property, which indicates whether the interaction was canceled rather than completed normally.

> [!IMPORTANT]
> The event arguments class passed to listeners is only valid during the event invocation. Do not store a reference to it.

## Assign a listener in the inspector

In the [Inspector window](xref:um-unity-events), you can assign a listener function you have defined in a script to events dispatched by an interactor or interactable object.

The arguments you include in your function declaration determine how much information about the event your function receives:

* No arguments: the function has no information about the event. You can assign this listener to any [UnityEvent](xref:um-unity-events).
* [`BaseInteractionEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.BaseInteractionEventArgs): the function gets the base event properties, such as the interactor and interactable involved in the event, but not those defined for specific events. You can assign this listener to any interaction event.
* The specific event argument, such as [`SelectEnterEventArgs`](xref:UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs): the function gets all available information for the event. You can only assign this listener to events that use the same event argument type.

The following code example illustrates how to define listener functions that you can assign to the interaction event properties in the Inspector for interactor and interactable objects:

``` csharp
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleInteractionEvent : MonoBehaviour
{
    // Has no information about the event itself
    public void OnEventWithoutParameter()
    {
        Debug.Log("An interaction event happened.");
    }

    // Doesn't know what kind of event happened, but has
    // base event properties
    public void OnAnyInteractionEvent(BaseInteractionEventArgs args)
    {
        Debug.Log($"{args.interactorObject.transform.name} " +
                    "interacted with " +
                    $"{args.interactableObject.transform.name}.");
    }

    // Knows about the select enter event
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log($"{args.interactableObject.transform.name} " +
                    "selected by " +
                    $"{args.interactorObject.transform.name} " +
                    "mediated by " +
                    $"{args.manager.transform.name}.");
    }
}
```

## Assign a listener in a script

You can assign listener functions directly from a script without using the Inspector.

The following example illustrates how to listen to events associated with an interactor and an interactable:

``` csharp
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InteractionEventHandler : MonoBehaviour
{
    [SerializeField]
    XRBaseInteractable interactable;

    [SerializeField]
    XRBaseInteractor interactor;

    void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.activated.AddListener(OnActivated);
        }

        if (interactor != null)
        {
            interactor.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.activated.RemoveListener(OnActivated);
        }

        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"{args.interactableObject.transform.name} selected by {args.interactorObject.transform.name}");
    }

    void OnActivated(ActivateEventArgs args)
    {
        Debug.Log($"{args.interactableObject.transform.name} activated by {args.interactorObject.transform.name}");
    }
}
```
