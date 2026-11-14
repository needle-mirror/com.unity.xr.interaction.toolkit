using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARScaleInteractable"/>.
    /// </summary>
    [Obsolete(k_ARScaleInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARScaleInteractable), true), CanEditMultipleObjects]
    public class ARScaleInteractableEditor : ARBaseGestureInteractableEditor
    {
        const string k_ARScaleInteractableEditorDeprecated = "ARScaleInteractableEditor has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARScaleInteractable.minScale"/>.</summary>
        protected SerializedProperty m_MinScale;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARScaleInteractable.maxScale"/>.</summary>
        protected SerializedProperty m_MaxScale;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARScaleInteractable.elasticRatioLimit"/>.</summary>
        protected SerializedProperty m_ElasticRatioLimit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARScaleInteractable.sensitivity"/>.</summary>
        protected SerializedProperty m_Sensitivity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARScaleInteractable.elasticity"/>.</summary>
        protected SerializedProperty m_Elasticity;

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARScaleInteractableEditorDeprecated);
        }
    }
}
