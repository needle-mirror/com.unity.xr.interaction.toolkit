using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Filter for a <see cref="TeleportationMultiAnchorVolume"/> that designates the anchor furthest from the user
    /// as the teleportation destination. Distance calculation uses the camera position projected onto the XZ plane of
    /// the XR Origin.
    /// </summary>
    [CreateAssetMenu(fileName = "FurthestTeleportationAnchorFilter", menuName = "XR/Locomotion/Furthest Teleportation Anchor Filter")]
    [HelpURL(XRHelpURLConstants.k_FurthestTeleportationAnchorFilter)]
    public class FurthestTeleportationAnchorFilter : ScriptableObject, ITeleportationVolumeAnchorFilter
    {
        /// <inheritdoc/>
        public int GetDestinationAnchorIndex(TeleportationMultiAnchorVolume teleportationVolume)
        {
            var anchorIndex = -1;
            var furthestSqDistance = -1f;
            var userPosition = teleportationVolume.teleportationProvider.system.xrOrigin.GetCameraFloorWorldPosition();
            var anchors = teleportationVolume.anchorTransforms;
            for (var i = 0; i < anchors.Count; ++i)
            {
                var sqrDistance = (anchors[i].position - userPosition).sqrMagnitude;
                if (sqrDistance > furthestSqDistance)
                {
                    anchorIndex = i;
                    furthestSqDistance = sqrDistance;
                }
            }

            return anchorIndex;
        }
    }
}