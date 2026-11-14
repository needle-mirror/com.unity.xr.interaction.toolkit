using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARSelectionInteractable"/>.
    /// </summary>
    [Obsolete(k_ARSelectionInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARSelectionInteractable), true), CanEditMultipleObjects]
    public class ARSelectionInteractableEditor : ARBaseGestureInteractableEditor
    {
        const string k_ARSelectionInteractableEditorDeprecated = "ARSelectionInteractable has been deprecated. To achieve the same results use the interactable's focus state instead.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARSelectionInteractable.selectionVisualization"/>.</summary>
        protected SerializedProperty m_SelectionVisualization;

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARSelectionInteractableEditorDeprecated);
        }
    }
}
