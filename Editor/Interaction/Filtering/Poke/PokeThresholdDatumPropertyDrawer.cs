using Unity.XR.CoreUtils.Datums.Editor;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEditor.XR.Interaction.Toolkit.Filtering
{
    /// <summary>
    /// Property drawer for the serializable container class that holds a poke threshold value or container asset reference.
    /// In this case, it is used to draw a <see cref="PokeThresholdDatumProperty"/>.
    /// </summary>
    /// <seealso cref="PokeThresholdDatumProperty"/>
    /// <seealso cref="DatumPropertyDrawer"/>
    [CustomPropertyDrawer(typeof(PokeThresholdDatumProperty))]
    public class PokeThresholdDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}
