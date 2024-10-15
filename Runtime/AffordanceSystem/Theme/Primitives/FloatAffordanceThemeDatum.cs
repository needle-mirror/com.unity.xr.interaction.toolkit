using System;
using Unity.XR.CoreUtils.Datums;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives
{
    /// <summary>
    /// Affordance state theme data structure for for float affordances.
    /// </summary>
    [Serializable]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class FloatAffordanceTheme : BaseAffordanceTheme<float>
    {
    }

    /// <summary>
    /// Serializable container class that holds a float affordance theme value or container asset reference.
    /// </summary>
    /// <seealso cref="FloatAffordanceThemeDatum"/>
    [Serializable]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class FloatAffordanceThemeDatumProperty : DatumProperty<FloatAffordanceTheme, FloatAffordanceThemeDatum>
    {
        /// <inheritdoc/>
        public FloatAffordanceThemeDatumProperty(FloatAffordanceTheme value) : base(value)
        {
        }

        /// <inheritdoc/>
        public FloatAffordanceThemeDatumProperty(FloatAffordanceThemeDatum datum) : base(datum)
        {
        }
    }

    /// <summary>
    /// <see cref="ScriptableObject"/> container class that holds a float affordance theme value.
    /// </summary>
    [CreateAssetMenu(fileName = "FloatAffordanceTheme", menuName = "Affordance Theme/Float Affordance Theme", order = 0)]
    [HelpURL(XRHelpURLConstants.k_FloatAffordanceThemeDatum)]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class FloatAffordanceThemeDatum : Datum<FloatAffordanceTheme>
    {
    }
}
