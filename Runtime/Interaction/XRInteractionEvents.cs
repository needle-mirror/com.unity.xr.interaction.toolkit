using System;
using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover, selection, and activation by this Interactable.
    /// </summary>
    [Serializable, Obsolete("XRInteractableEvent has been deprecated. Use events specific to each state change instead.")]
    public class XRInteractableEvent : UnityEvent<XRBaseInteractor>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover and selection by this Interactor.
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
        /// (Deprecated) The Interactor associated with the interaction event.
        /// </summary>
        /// <remarks>
        /// <c>interactor</c> has been deprecated. Use <see cref="interactorObject"/> instead.
        /// </remarks>
        [Obsolete("interactor has been deprecated. Use interactorObject instead.")]
        public XRBaseInteractor interactor
        {
            get => interactorObject as XRBaseInteractor;
            set => interactorObject = value;
        }

        /// <summary>
        /// (Deprecated) The Interactable associated with the interaction event.
        /// </summary>
        /// <remarks>
        /// <c>interactable</c> has been deprecated. Use <see cref="interactableObject"/> instead.
        /// </remarks>
        [Obsolete("interactable has been deprecated. Use interactableObject instead.")]
        public XRBaseInteractable interactable
        {
            get => interactableObject as XRBaseInteractable;
            set => interactableObject = value;
        }

        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    #region Teleport

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when queuing to teleport via
    /// a <see cref="TeleportationProvider"/>.
    /// </summary>
    [Serializable]
    public sealed class TeleportingEvent : UnityEvent<TeleportingEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event that Unity invokes during a selection or
    /// activation event between an Interactable and an Interactor, according to the
    /// timing defined by <see cref="BaseTeleportationInteractable.TeleportTrigger"/>.
    /// </summary>
    public class TeleportingEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The <see cref="TeleportRequest"/> that is being queued, but has not been acted on yet.
        /// </summary>
        public TeleportRequest teleportRequest { get; set; }
    }

    #endregion

    #region Hover

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor first initiates hovering over an Interactable.
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
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractor interactorObject
        {
            get => (IXRHoverInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractable interactableObject
        {
            get => (IXRHoverInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor ends hovering over an Interactable.
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
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractor interactorObject
        {
            get => (IXRHoverInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractable interactableObject
        {
            get => (IXRHoverInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }

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
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor initiates selecting an Interactable.
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
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractor interactorObject
        {
            get => (IXRSelectInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractable interactableObject
        {
            get => (IXRSelectInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor ends selecting an Interactable.
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
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractor interactorObject
        {
            get => (IXRSelectInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractable interactableObject
        {
            get => (IXRSelectInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }

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
    /// <see cref="UnityEvent"/> that Unity invokes when the selecting Interactor activates an Interactable.
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
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractor interactorObject
        {
            get => (IXRActivateInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractable interactableObject
        {
            get => (IXRActivateInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when the selecting Interactor deactivates an Interactable.
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
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractor interactorObject
        {
            get => (IXRActivateInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractable interactableObject
        {
            get => (IXRActivateInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }
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
        /// (Deprecated) The Interactor that was registered.
        /// </summary>
        /// <remarks>
        /// <c>interactor</c> has been deprecated. Use <see cref="interactorObject"/> instead.
        /// </remarks>
        [Obsolete("interactor has been deprecated. Use interactorObject instead.")]
        public XRBaseInteractor interactor
        {
            get => interactorObject as XRBaseInteractor;
            set => interactorObject = value;
        }

        /// <summary>
        /// The Interactor that was registered.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactable is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractableRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// (Deprecated) The Interactable that was registered.
        /// </summary>
        /// <remarks>
        /// <c>interactable</c> has been deprecated. Use <see cref="interactableObject"/> instead.
        /// </remarks>
        [Obsolete("interactable has been deprecated. Use interactableObject instead.")]
        public XRBaseInteractable interactable
        {
            get => interactableObject as XRBaseInteractable;
            set => interactableObject = value;
        }

        /// <summary>
        /// The Interactable that was registered.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactor is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractorUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// (Deprecated) The Interactor that was unregistered.
        /// </summary>
        /// <remarks>
        /// <c>interactor</c> has been deprecated. Use <see cref="interactorObject"/> instead.
        /// </remarks>
        [Obsolete("interactor has been deprecated. Use interactorObject instead.")]
        public XRBaseInteractor interactor
        {
            get => interactorObject as XRBaseInteractor;
            set => interactorObject = value;
        }

        /// <summary>
        /// The Interactor that was unregistered.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactable is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    public class InteractableUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// (Deprecated) The Interactable that was unregistered.
        /// </summary>
        /// <remarks>
        /// <c>interactable</c> has been deprecated. Use <see cref="interactableObject"/> instead.
        /// </remarks>
        [Obsolete("interactable has been deprecated. Use interactableObject instead.")]
        public XRBaseInteractable interactable
        {
            get => interactableObject as XRBaseInteractable;
            set => interactableObject = value;
        }

        /// <summary>
        /// The Interactable that was unregistered.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    #endregion
}