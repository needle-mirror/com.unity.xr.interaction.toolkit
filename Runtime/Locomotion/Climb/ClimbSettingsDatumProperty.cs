using System;
using Unity.XR.CoreUtils.Datums;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Serializable container class that holds a <see cref="ClimbSettings"/> value or a container asset reference.
    /// </summary>
    /// <seealso cref="ClimbSettingsDatum"/>
    [Serializable]
    public class ClimbSettingsDatumProperty : DatumProperty<ClimbSettings, ClimbSettingsDatum>
    {
        /// <inheritdoc cref="ClimbSettings"/>
        public ClimbSettingsDatumProperty(ClimbSettings value) : base(value)
        {
        }

        /// <inheritdoc cref="ClimbSettingsDatum"/>
        public ClimbSettingsDatumProperty(ClimbSettingsDatum datum) : base(datum)
        {
        }
    }
}
