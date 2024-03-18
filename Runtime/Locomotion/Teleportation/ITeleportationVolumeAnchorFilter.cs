using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Interface for filtering a list of anchors for a <see cref="TeleportationMultiAnchorVolume"/> to designate one
    /// as the destination anchor.
    /// </summary>
    /// <seealso cref="TeleportationMultiAnchorVolume"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public interface ITeleportationVolumeAnchorFilter
    {
        /// <summary>
        /// Called by the <paramref name="teleportationVolume"/> to designate an anchor as the teleportation destination.
        /// </summary>
        /// <param name="teleportationVolume">The volume that is designating a destination anchor.</param>
        /// <returns>
        /// Returns the index of the transform in the <paramref name="teleportationVolume"/>'s anchors list to use as
        /// the teleportation destination. If this value is outside the range of the list, the volume will not
        /// designate a destination anchor.
        /// </returns>
        int GetDestinationAnchorIndex(TeleportationMultiAnchorVolume teleportationVolume);
    }
}