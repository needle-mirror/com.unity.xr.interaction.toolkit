using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// An interface that represents an interactable component that has an optional reference to a
    /// parent interactable for determining processing order of interactables.
    /// </summary>
    /// <remarks>
    /// Interactor or interactable components that implement this interface enable the <see cref="XRInteractionManager"/>
    /// to automatically register the parent relationship when the component is registered. If the interactable
    /// component does not implement this interface, any parent relationship the component has must be manually
    /// registered.
    /// </remarks>
    /// <seealso cref="IXRInteractable.ProcessInteractable"/>
    /// <seealso cref="XRInteractionManager.RegisterInteractor(IXRInteractor)"/>
    /// <seealso cref="XRInteractionManager.RegisterInteractable(IXRInteractable)"/>
    /// <seealso cref="XRInteractionManager.RegisterParentRelationship(IXRInteractor,IXRInteractable)"/>
    /// <seealso cref="XRInteractionManager.RegisterParentRelationship(IXRInteractable,IXRInteractable)"/>
    public interface IXRParentInteractableLink
    {
        /// <summary>
        /// Whether the XR Interaction Manager will automatically find a parent interactable up the GameObject
        /// hierarchy when this object is first registered with it. Set to disabled if you
        /// want to avoid the runtime performance expense of an automatic <c>GetComponentInParent</c> call.
        /// </summary>
        /// <remarks>
        /// Ignored if the <see cref="parentInteractable"/> reference is assigned.
        /// </remarks>
        /// <seealso cref="parentInteractable"/>
        bool autoFindParentInteractableInHierarchy { get; set; }

        /// <summary>
        /// An optional reference to a parent interactable for determining processing order of interactables.
        /// </summary>
        /// <remarks>
        /// For interactables that implement this, the referenced parent interactable will be processed before
        /// this interactable.
        /// <br/>
        /// For interactors that implement this, selected interactables will inherit this referenced parent interactable
        /// for determining the process order of interactables such that parents are processed before their children
        /// interactables.
        /// <br/>
        /// The interactables must be registered with the same XR Interaction Manager for this to have an effect
        /// on processing order.
        /// </remarks>
        IXRInteractable parentInteractable { get; set; }
    }
}
