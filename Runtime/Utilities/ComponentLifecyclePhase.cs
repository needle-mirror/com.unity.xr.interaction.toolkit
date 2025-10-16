using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Represents a lifecycle phase of a <see cref="MonoBehaviour"/>.
    /// </summary>
    /// <seealso cref="XRInteractionManager.activeInteractionManagersChanged"/>
    /// <seealso cref="XRInputModalityManager.activeModalityManagersChanged"/>
    enum ComponentLifecyclePhase
    {
        /// <summary>
        /// <c>Awake</c> method.
        /// </summary>
        Awake,

        /// <summary>
        /// <c>OnEnable</c> method.
        /// </summary>
        Enable,

        /// <summary>
        /// <c>OnDisable</c> method.
        /// </summary>
        Disable,

        /// <summary>
        /// <c>OnDestroy</c> method.
        /// </summary>
        Destroy,
    }
}
