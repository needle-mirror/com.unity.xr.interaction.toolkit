using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARRotationInteractable"/>.
    /// </summary>
    [Obsolete(k_ARRotationInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARRotationInteractable), true), CanEditMultipleObjects]
    public class ARRotationInteractableEditor : ARBaseGestureInteractableEditor
    {
        const string k_ARRotationInteractableEditorDeprecated = "ARRotationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARRotationInteractable.rotationRateDegreesDrag"/>.</summary>
        protected SerializedProperty m_RotationRateDegreesDrag;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARRotationInteractable.rotationRateDegreesTwist"/>.</summary>
        protected SerializedProperty m_RotationRateDegreesTwist;

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARRotationInteractableEditorDeprecated);
        }
    }
}
