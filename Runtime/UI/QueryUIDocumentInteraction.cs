using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Sets whether physics queries hit Trigger colliders and include or ignore UI Toolkit panel (UIDocument or PanelRenderer) trigger colliders.
    /// </summary>
    /// <seealso cref="XRPokeInteractor.uiDocumentTriggerInteraction"/>
    /// <seealso cref="CurveInteractionCaster.raycastUIDocumentTriggerInteraction"/>
    /// <seealso cref="XRRayInteractor.raycastUIDocumentTriggerInteraction"/>
    public enum QueryUIDocumentInteraction
    {
        /// <summary>
        /// Queries never report Trigger hits that are associated with a UI Toolkit panel.
        /// </summary>
        Ignore,

        /// <summary>
        /// Queries always report Trigger hits that are associated with a UI Toolkit panel.
        /// </summary>
        Collide,
    }
}
