using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARTranslationInteractable"/>.
    /// </summary>
    [Obsolete(k_ARTranslationInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARTranslationInteractable), true), CanEditMultipleObjects]
    public class ARTranslationInteractableEditor : ARBaseGestureInteractableEditor
    {
        const string k_ARTranslationInteractableEditorDeprecated = "ARTranslationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTranslationInteractable.objectGestureTranslationMode"/>.</summary>
        protected SerializedProperty m_ObjectGestureTranslationMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTranslationInteractable.maxTranslationDistance"/>.</summary>
        protected SerializedProperty m_MaxTranslationDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTranslationInteractable.fallbackLayerMask"/>.</summary>
        protected SerializedProperty m_FallbackLayerMask;

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARTranslationInteractableEditorDeprecated);
        }
    }
}
