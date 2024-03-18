using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    public abstract partial class BaseTeleportationInteractable
    {
        const string k_GenerateTeleportRequestDeprecated = "GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest) has been deprecated. Use GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest) instead.";

        /// <summary>
        /// Automatically called upon the teleport trigger when a teleport request should be generated.
        /// </summary>
        /// <param name="interactor">The interactor that initiated the teleport trigger.</param>
        /// <param name="raycastHit">The ray cast hit information from the interactor.</param>
        /// <param name="teleportRequest">The teleport request that should be filled out during this method call.</param>
        /// <returns>Returns <see langword="true"/> if the teleport request was successfully updated and should be queued. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="TeleportationProvider.QueueTeleportRequest"/>
        /// <remarks>
        /// <c>GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest)</c> has been deprecated. Use <see cref="GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest)"/> instead.
        /// </remarks>
        [Obsolete(k_GenerateTeleportRequestDeprecated, true)]
        protected virtual bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            Debug.LogError(k_GenerateTeleportRequestDeprecated, this);
            throw new NotSupportedException(k_GenerateTeleportRequestDeprecated);
        }
    }
}
