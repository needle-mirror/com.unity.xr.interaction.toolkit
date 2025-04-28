using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// This component controls whether UI Toolkit support is enabled for
    /// compatible <see cref="IXRInteractor"/> components in the scene.
    /// </summary>
    [AddComponentMenu("XR/XR UI Toolkit Manager", 11)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRUIToolkitManager)]
    [HelpURL(XRHelpURLConstants.k_XRUIToolkitManager)]
    public class XRUIToolkitManager : MonoBehaviour
    {
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            XRUIToolkitHandler.uiToolkitSupportEnabled = true;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            XRUIToolkitHandler.uiToolkitSupportEnabled = false;
        }
    }
}
