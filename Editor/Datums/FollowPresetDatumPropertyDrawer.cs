using System;
using Unity.XR.CoreUtils.Datums.Editor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI.BodyUI;

namespace UnityEditor.XR.Interaction.Toolkit.Datums.Editor
{
    /// <summary>
    /// Datum PropertyDrawer implementation for Follow Preset Datum Property Drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(FollowPresetDatumProperty))]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class FollowPresetDatumPropertyDrawer : DatumPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty selectedValue = GetSelectedProperty(property);
            if (selectedValue.hasVisibleChildren)
            {
                return EditorGUI.GetPropertyHeight(selectedValue, true);
            }
            return base.GetPropertyHeight(property, label);
        }
    }
}
