using System;
using Unity.XR.CoreUtils.Datums;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
    /// <summary>
    /// Serializable container class that holds a poke threshold value or container asset reference.
    /// </summary>
    /// <seealso cref="PokeThresholdDatum"/>
    [Serializable]
    public class PokeThresholdDatumProperty : DatumProperty<PokeThresholdData, PokeThresholdDatum>
    {
        /// <inheritdoc cref="DatumProperty{PokeThresholdData, PokeThresholdDatum}"/>
        public PokeThresholdDatumProperty(PokeThresholdData value) : base(value)
        {
        }

        /// <inheritdoc cref="DatumProperty{PokeThresholdData, PokeThresholdDatum}"/>
        public PokeThresholdDatumProperty(PokeThresholdDatum datum) : base(datum)
        {
        }
    }
}
