using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Sets whether physics queries hit Trigger colliders and include or ignore UI Toolkit UI Document trigger colliders.
    /// </summary>
    /// <seealso cref="XRPokeInteractor.uiDocumentTriggerInteraction"/>
    /// <seealso cref="CurveInteractionCaster.raycastUIDocumentTriggerInteraction"/>
    public enum QueryUIDocumentInteraction
    {
        /// <summary>
        /// Queries never report Trigger hits that are associated with a UI Toolkit UI Document.
        /// </summary>
        Ignore,

        /// <summary>
        /// Queries always report Trigger hits that are associated with a UI Toolkit UI Document.
        /// </summary>
        Collide,
    }
}
