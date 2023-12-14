using System;
using Unity.XR.CoreUtils.Datums.Editor;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio;

namespace UnityEditor.XR.Interaction.Toolkit.Datums.Editor
{
    /// <summary>
    /// Datum PropertyDrawer implementation for Audio Clip Affordance Themes.
    /// </summary>
    [CustomPropertyDrawer(typeof(AudioAffordanceThemeDatumProperty))]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class AudioAffordanceThemeDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}