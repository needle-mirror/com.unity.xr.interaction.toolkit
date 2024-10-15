using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARAnnotationInteractable"/>.
    /// </summary>
    [Obsolete("ARAnnotationInteractable has been deprecated. Instead, it is suggested to use LazyFollow and the Hover and Select events from the current interaction system to set the visibility of UGUI canvas objects.")]
    [CustomEditor(typeof(ARAnnotationInteractable), true), CanEditMultipleObjects]
    public class ARAnnotationInteractableEditor : ARBaseGestureInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARAnnotationInteractable.annotations"/>.</summary>
        protected SerializedProperty m_Annotations;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            m_Annotations = serializedObject.FindProperty("m_Annotations");
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            base.DrawProperties();
            EditorGUILayout.PropertyField(m_Annotations);
        }
    }
}
