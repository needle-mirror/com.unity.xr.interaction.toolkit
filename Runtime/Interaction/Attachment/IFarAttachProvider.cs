using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Controls the interactor's default behavior for how to adjust its attach transform on far select.
    /// </summary>
    /// <seealso cref="NearFarInteractor.farAttachMode"/>
    /// <seealso cref="InteractableFarAttachMode"/>
    public enum InteractorFarAttachMode
    {
        /// <summary>
        /// The interactor should reset its attach transform to the near point on far select.
        /// This will typically result in the interactable object moving to the hand.
        /// </summary>
        Near,

        /// <summary>
        /// The interactor should always move its attach transform to the far hit point on far select.
        /// This will typically result in the interactable object staying distant at the far hit point.
        /// </summary>
        Far,
    }

    /// <summary>
    /// Controls how the interactor should adjust its attach transform on far select.
    /// </summary>
    /// <seealso cref="XRGrabInteractable.farAttachMode"/>
    /// <seealso cref="InteractorFarAttachMode"/>
    public enum InteractableFarAttachMode
    {
        /// <summary>
        /// Let the interactor decide the far attach mode. This is the default behavior.
        /// </summary>
        /// <seealso cref="NearFarInteractor.farAttachMode"/>
        /// <seealso cref="XRRayInteractor.useForceGrab"/>
        DeferToInteractor,

        /// <summary>
        /// The interactor should always reset its attach transform to the near point on far select.
        /// This will typically result in the interactable object moving to the hand.
        /// </summary>
        /// <remarks>
        /// This value will override the interactor's preference.
        /// </remarks>
        Near,

        /// <summary>
        /// The interactor should always move its attach transform to the far hit point on far select.
        /// This will typically result in the interactable object staying distant at the far hit point.
        /// </summary>
        /// <remarks>
        /// This value will override the interactor's preference.
        /// </remarks>
        Far,
    }

    /// <summary>
    /// Interface queried by an interactor when selecting an interactable to override how the interactor's
    /// attach transform should behave on far select.
    /// </summary>
    /// <seealso cref="XRGrabInteractable"/>
    public interface IFarAttachProvider
    {
        /// <summary>
        /// When selected because of a far interaction caster, controls how the interactor should attach to the interactable.
        /// Specifically, it controls whether the interactor's attach transform should move to the far hit point
        /// or whether it should stay near so the object can move to the hand.
        /// </summary>
        InteractableFarAttachMode farAttachMode { get; set; }
    }
}