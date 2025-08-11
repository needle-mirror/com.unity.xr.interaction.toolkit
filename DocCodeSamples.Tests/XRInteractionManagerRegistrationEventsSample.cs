using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// This class is an example script that registration events and demonstrates how to create callbacks for each event.
/// </summary>
class XRInteractionManagerRegistrationEventsSample : MonoBehaviour
{
    [SerializeField]
    XRInteractionManager m_InteractionManager;

    XRInteractionGroup m_ExampleInteractionGroup;
    NearFarInteractor m_ExampleInteractor;
    XRGrabInteractable m_ExampleInteractable;

    void Awake()
    {
        // Try to find an interaction manager if necessary.
        // In Unity versions older than 2021.3.18f1, replace FindFirstObjectByType with FindObjectOfType.
        if (m_InteractionManager == null)
            m_InteractionManager = FindFirstObjectByType<XRInteractionManager>();

        // Note, the code below is creating the XRInteractionGroup, NearFarInteractor, and XRGrabInteractable before subscribing
        // to the interaction manager register/unregister events. Therefore, the initial register callback will not be logged.
        // To demonstrate these callbacks, enable/disable each example game object in the hierarchy to receive the register/unregister callbacks.

        // XRInteractionGroup automatically registers with the interaction manager on Awake
        var interactionGroupGO = new GameObject("Example Interaction Group");
        m_ExampleInteractionGroup = interactionGroupGO.AddComponent<XRInteractionGroup>();

        // XRBaseInteractor automatically registers with the interaction manager on Enable
        var interactorGO = new GameObject("Example Interactor");
        m_ExampleInteractor = interactorGO.AddComponent<NearFarInteractor>();

        // XRBaseInteractable automatically registers with the interaction manager on Enable
        var interactableGO = new GameObject("Example Interactable");
        m_ExampleInteractable = interactableGO.AddComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (m_InteractionManager == null)
        {
            Debug.LogWarning("Interaction manager is null. No registration events will be executed in this example. Ensure there is an " + typeof(XRInteractionManager) + " in the scene.", this);
            return;
        }

        // Subscribe to the interaction manager registration events
        m_InteractionManager.interactionGroupRegistered += OnInteractionGroupRegistered;
        m_InteractionManager.interactorRegistered += OnInteractorRegistered;
        m_InteractionManager.interactableRegistered += OnInteractableRegistered;

        m_InteractionManager.interactionGroupUnregistered += OnInteractionGroupUnregistered;
        m_InteractionManager.interactorUnregistered += OnInteractorUnregistered;
        m_InteractionManager.interactableUnregistered += OnInteractableUnregistered;
    }

    void OnDisable()
    {
        if (m_InteractionManager == null)
            return;

        // Unsubscribe to the interaction manager registration events
        m_InteractionManager.interactionGroupRegistered -= OnInteractionGroupRegistered;
        m_InteractionManager.interactorRegistered -= OnInteractorRegistered;
        m_InteractionManager.interactableRegistered -= OnInteractableRegistered;

        m_InteractionManager.interactionGroupUnregistered -= OnInteractionGroupUnregistered;
        m_InteractionManager.interactorUnregistered -= OnInteractorUnregistered;
        m_InteractionManager.interactableUnregistered -= OnInteractableUnregistered;
    }

    void OnInteractionGroupRegistered(InteractionGroupRegisteredEventArgs args)
    {
        // Example of casting args object to interaction group base class
        var interactionGroup = args.interactionGroupObject as XRInteractionGroup;

        // Logic for interaction group registered event goes here
        if (interactionGroup != null && interactionGroup == m_ExampleInteractionGroup)
            Debug.Log(interactionGroup.transform.name + " is registered with " + args.manager.transform.name, this);
    }

    void OnInteractionGroupUnregistered(InteractionGroupUnregisteredEventArgs args)
    {
        // Example of casting args object to interaction group base class
        var interactionGroup = args.interactionGroupObject as XRInteractionGroup;

        // Logic for interaction group unregistered event goes here
        if (interactionGroup != null && interactionGroup == m_ExampleInteractionGroup)
            Debug.Log(interactionGroup.transform.name + " is unregistered with " + args.manager.transform.name, this);
    }

    void OnInteractorRegistered(InteractorRegisteredEventArgs args)
    {
        // Example of casting args objects to interactor base class
        var interactor = args.interactorObject as XRBaseInteractor;

        // Logic for interactor registered event goes here
        if (interactor != null && interactor == m_ExampleInteractor)
            Debug.Log(args.interactorObject.transform.name + " is registered with " + args.manager.transform.name, this);
    }

    void OnInteractorUnregistered(InteractorUnregisteredEventArgs args)
    {
        // Example of casting args objects to interactor base class
        var interactor = args.interactorObject as XRBaseInteractor;

        // Logic for interactor unregistered event goes here
        if (interactor != null && interactor == m_ExampleInteractor)
            Debug.Log(args.interactorObject.transform.name + " is unregistered with " + args.manager.transform.name, this);
    }

    void OnInteractableRegistered(InteractableRegisteredEventArgs args)
    {
        // Example of casting args objects to interactable base class
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for interactable registered event goes here
        if (interactable != null && interactable == m_ExampleInteractable)
            Debug.Log(args.interactableObject.transform.name + " is registered with " + args.manager.transform.name, this);
    }

    void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
    {
        // Example of casting args objects to interactable base class
        var interactable = args.interactableObject as XRBaseInteractable;

        // Logic for interactable unregistered event goes here
        if (interactable != null && interactable == m_ExampleInteractable)
            Debug.Log(args.interactableObject.transform.name + " is unregistered with " + args.manager.transform.name, this);
    }
}
