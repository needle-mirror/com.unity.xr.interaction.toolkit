using System;
using Unity.XR.CoreUtils.Datums.Editor;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;

namespace UnityEditor.XR.Interaction.Toolkit.Datums.Editor
{
    /// <summary>
    /// Datum PropertyDrawer implementation for Vector4 Affordance Themes.
    /// </summary>
    [CustomPropertyDrawer(typeof(Vector4AffordanceThemeDatumProperty))]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class Vector4AffordanceThemeDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}