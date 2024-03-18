using System;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
    /// <summary>
    /// Serializable container class that holds a <see cref="ClimbSettings"/> value or a container asset reference.
    /// </summary>
    /// <seealso cref="ClimbSettingsDatum"/>
    [Serializable]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class ClimbSettingsDatumProperty : DatumProperty<ClimbSettings, ClimbSettingsDatum>
    {
        /// <inheritdoc/>
        public ClimbSettingsDatumProperty(ClimbSettings value) : base(value)
        {
        }

        /// <inheritdoc/>
        public ClimbSettingsDatumProperty(ClimbSettingsDatum datum) : base(datum)
        {
        }
    }
}