using System;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when queuing to teleport via
    /// a <see cref="TeleportationProvider"/>.
    /// </summary>
    [Serializable]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public sealed class TeleportingEvent : UnityEvent<TeleportingEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event that Unity invokes during a selection or
    /// activation event between an Interactable and an Interactor, according to the
    /// timing defined by <see cref="BaseTeleportationInteractable.TeleportTrigger"/>.
    /// </summary>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportingEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The <see cref="TeleportRequest"/> that is being queued, but has not been acted on yet.
        /// </summary>
        public TeleportRequest teleportRequest { get; set; }
    }
}
