using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
    /// <summary>
    /// Grab transformer used when <see cref="XRGrabInteractable.attachPointCompatibilityMode"/> is
    /// set to <see cref="XRGrabInteractable.AttachPointCompatibilityMode.Legacy"/>.
    /// This is for backwards compatibility purpose for old projects.
    /// Marked for deprecation, this component will be removed in a future version.
    /// </summary>
    /// <seealso cref="XRSingleGrabFreeTransformer"/>
    [AddComponentMenu("")]
    [HelpURL(XRHelpURLConstants.k_XRLegacyGrabTransformer)]
    [Obsolete("XRLegacyGrabTransformer has been deprecated, use XRSingleFreeGrabTransformer instead.", true)]
    public sealed class XRLegacyGrabTransformer : XRBaseGrabTransformer
    {
        /// <inheritdoc />
        public override void OnLink(XRGrabInteractable grabInteractable)
        {
            base.OnLink(grabInteractable);
        }

        /// <inheritdoc />
        public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
        }

        /// <inheritdoc />
        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
        }
    }
}
