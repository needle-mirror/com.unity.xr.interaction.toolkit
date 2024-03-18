using System;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Serializable container class that holds a <see cref="TeleportVolumeDestinationSettings"/> value or a container asset reference.
    /// </summary>
    /// <seealso cref="TeleportVolumeDestinationSettingsDatum"/>
    [Serializable]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportVolumeDestinationSettingsDatumProperty : DatumProperty<TeleportVolumeDestinationSettings,
        TeleportVolumeDestinationSettingsDatum>
    {
        /// <inheritdoc/>
        public TeleportVolumeDestinationSettingsDatumProperty(TeleportVolumeDestinationSettings value) : base(value)
        {
        }

        /// <inheritdoc/>
        public TeleportVolumeDestinationSettingsDatumProperty(TeleportVolumeDestinationSettingsDatum datum) : base(datum)
        {
        }
    }
}