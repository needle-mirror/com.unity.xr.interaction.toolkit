using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// This class is an example script that highlights the various XR Interaction events and demonstrates how to create callbacks for each event.
/// </summary>
class XRInteractionEventsSample : MonoBehaviour
{
    // The interactor to listen to hover and select interaction events. Connect an XRBaseInteractor like NearFarInteractor or XRRayInteractor to this
    // property in the Editor.
    [SerializeField]
    XRBaseInteractor m_ExampleInteractor;

    // The interactable to listen to focus and activate events. Connect XRBaseInteractable like XRGrabInteractable to this property in the Editor
    [SerializeField]
    XRBaseInteractable m_ExampleInteractable;

    void OnEnable()
    {
        if (m_ExampleInteractor != null)
        {
            // Add listener to hover events
            m_ExampleInteractor.hoverEntered.AddListener(OnInteractorHoverEntered);
            m_ExampleInteractor.hoverExited.AddListener(OnInteractorHoverExited);

            // Add listener to select events
            m_ExampleInteractor.selectEntered.AddListener(OnInteractorSelectEntered);
            m_ExampleInteractor.selectExited.AddListener(OnInteractorSelectExited);
        }
        else
        {
            Debug.LogWarning("Example interactor is null. No hover or select event callbacks will be executed in this example. Ensure the ExampleInteractor field has a " + typeof(XRBaseInteractor) + " assigned.", this);
        }

        if (m_ExampleInteractable != null)
        {
            // Add listener to focus events
            m_ExampleInteractable.focusEntered.AddListener(OnInteractableFocusEntered);
            m_ExampleInteractable.focusExited.AddListener(OnInteractableFocusExited);

            // Add listener to activate events
            m_ExampleInteractable.activated.AddListener(OnInteractableActivated);
            m_ExampleInteractable.deactivated.AddListener(OnInteractableDeactivated);
        }
        else
        {
            Debug.LogWarning("Example interactable is null. No focus or activate event callbacks will be executed in this example. Ensure the ExampleInteractable field has a " + typeof(XRBaseInteractable) + " assigned.", this);
        }
    }

    void OnDisable()
    {
        if (m_ExampleInteractor != null)
        {
            // Remove listener to hover events
            m_ExampleInteractor.hoverEntered.RemoveListener(OnInteractorHoverEntered);
            m_ExampleInteractor.hoverExited.RemoveListener(OnInteractorHoverExited);

            // Remove listener to select events
            m_ExampleInteractor.selectEntered.RemoveListener(OnInteractorSelectEntered);
            m_ExampleInteractor.selectExited.RemoveListener(OnInteractorSelectExited);
        }

        if (m_ExampleInteractable != null)
        {
            // Remove listener to focus events
            m_ExampleInteractable.focusEntered.RemoveListener(OnInteractableFocusEntered);
            m_ExampleInteractable.focusExited.RemoveListener(OnInteractableFocusExited);

            // Remove listener to activate events
            m_ExampleInteractable.activated.RemoveListener(OnInteractableActivated);
            m_ExampleInteractable.deactivated.RemoveListener(OnInteractableDeactivated);
        }
    }

    void OnInteractorHoverEntered(HoverEnterEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for hover enter event goes here
        Debug.Log(args.interactorObject.transform.name + " began hovering " + args.interactableObject.transform.name, this);
    }

    void OnInteractorHoverExited(HoverExitEventArgs args)
    {
        // Example of ignoring event when the hover was canceled, such as due to the object being unregistered while hovering
        if (args.isCanceled)
            return;

        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for hover exit event goes here
        Debug.Log(args.interactorObject.transform.name + " stopped hovering " + args.interactableObject.transform.name, this);
    }

    void OnInteractorSelectEntered(SelectEnterEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for select enter event goes here
        Debug.Log(args.interactorObject.transform.name + " began selecting " + args.interactableObject.transform.name, this);
    }

    void OnInteractorSelectExited(SelectExitEventArgs args)
    {
        // Example of ignoring event when the select was canceled, such as due to the object being unregistered while selected
        if (args.isCanceled)
            return;

        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for select exit event goes here
        Debug.Log(args.interactorObject.transform.name + " stopped selecting " + args.interactableObject.transform.name, this);
    }

    void OnInteractableFocusEntered(FocusEnterEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for focus enter event goes here
        Debug.Log(args.interactorObject.transform.name + " began focusing " + args.interactableObject.transform.name, this);
    }

    void OnInteractableFocusExited(FocusExitEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for focus exit event goes here
        Debug.Log(args.interactorObject.transform.name + " stopped focusing " + args.interactableObject.transform.name, this);
    }

    void OnInteractableActivated(ActivateEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for activate event goes here
        Debug.Log(args.interactorObject.transform.name + " is activating " + args.interactableObject.transform.name, this);
    }

    void OnInteractableDeactivated(DeactivateEventArgs args)
    {
        // Example of casting args objects to interactor and interactable base classes
        var interactor = args.interactorObject as XRBaseInteractor;
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for deactivate event goes here
        Debug.Log(args.interactorObject.transform.name + " is deactivating " + args.interactableObject.transform.name, this);
    }
}
