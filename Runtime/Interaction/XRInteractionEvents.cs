using System;
using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover, selection, and activation by this interactable.
    /// </summary>
    [Serializable, Obsolete("XRInteractableEvent has been deprecated. Use events specific to each state change instead.")]
    public class XRInteractableEvent : UnityEvent<XRBaseInteractor>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover and selection by this interactor.
    /// </summary>
    [Serializable, Obsolete("XRInteractorEvent has been deprecated. Use events specific to each state change instead.")]
    public class XRInteractorEvent : UnityEvent<XRBaseInteractable>
    {
    }

    /// <summary>
    /// Event data associated with an interaction event between an Interactor and Interactable.
    /// </summary>
    public abstract class BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public XRBaseInteractor interactor { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public XRBaseInteractable interactable { get; set; }
    }

    #region Hover

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an Interactor first initiates hovering over an Interactable.
    /// </summary>
    [Serializable]
    public sealed class HoverEnterEvent : UnityEvent<HoverEnterEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor first initiates hovering over an Interactable.
    /// </summary>
    public class HoverEnterEventArgs : BaseInteractionEventArgs
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an Interactor ends hovering over an Interactable.
    /// </summary>
    [Serializable]
    public sealed class HoverExitEvent : UnityEvent<HoverExitEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor ends hovering over an Interactable.
    /// </summary>
    public class HoverExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// Whether the hover was ended due to being canceled, such as from
        /// either the Interactor or Interactable being unregistered due to being
        /// disabled or destroyed.
        /// </summary>
        public bool isCanceled { get; set; }
    }

    #endregion

    #region Select

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an Interactor initiates selecting an Interactable.
    /// </summary>
    [Serializable]
    public sealed class SelectEnterEvent : UnityEvent<SelectEnterEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor initiates selecting an Interactable.
    /// </summary>
    public class SelectEnterEventArgs : BaseInteractionEventArgs
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an Interactor ends selecting an Interactable.
    /// </summary>
    [Serializable]
    public sealed class SelectExitEvent : UnityEvent<SelectExitEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor ends selecting an Interactable.
    /// </summary>
    public class SelectExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// Whether the selection was ended due to being canceled, such as from
        /// either the Interactor or Interactable being unregistered due to being
        /// disabled or destroyed.
        /// </summary>
        public bool isCanceled { get; set; }
    }

    #endregion

    #region Activate

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when the selecting Interactor activates an Interactable.
    /// </summary>
    /// <remarks>
    /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
    /// This is a generic event when an Interactor wants to activate its selected Interactable,
    /// such as from a trigger pull on a controller.
    /// </remarks>
    [Serializable]
    public sealed class ActivateEvent : UnityEvent<ActivateEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when the selecting Interactor activates an Interactable.
    /// </summary>
    public class ActivateEventArgs : BaseInteractionEventArgs
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when the selecting Interactor deactivates an Interactable.
    /// </summary>
    /// <remarks>
    /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
    /// This is a generic event when an Interactor wants to deactivate its selected Interactable,
    /// such as from a trigger pull on a controller.
    /// </remarks>
    [Serializable]
    public sealed class DeactivateEvent : UnityEvent<DeactivateEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when the selecting Interactor deactivates an Interactable.
    /// </summary>
    public class DeactivateEventArgs : BaseInteractionEventArgs
    {
    }

    #endregion

    #region Registration

    /// <summary>
    /// Event data associated with a registration event with an <see cref="XRInteractionManager"/>.
    /// </summary>
    public abstract class BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interaction Manager associated with the registration event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactor is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractorRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactor that was registered.
        /// </summary>
        public XRBaseInteractor interactor { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactable is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractableRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactable that was registered.
        /// </summary>
        public XRBaseInteractable interactable { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactor is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractorUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactor that was unregistered.
        /// </summary>
        public XRBaseInteractor interactor { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactable is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractableUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactable that was unregistered.
        /// </summary>
        public XRBaseInteractable interactable { get; set; }
    }

    #endregion
}