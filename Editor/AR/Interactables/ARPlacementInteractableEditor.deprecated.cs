using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARPlacementInteractable"/>.
    /// </summary>
    [Obsolete(k_ARPlacementInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARPlacementInteractable), true), CanEditMultipleObjects]
    public class ARPlacementInteractableEditor : ARBaseGestureInteractableEditor
    {
        const string k_ARPlacementInteractableEditorDeprecated = "ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARPlacementInteractable.placementPrefab"/>.</summary>
        protected SerializedProperty m_PlacementPrefab;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARPlacementInteractable.fallbackLayerMask"/>.</summary>
        protected SerializedProperty m_FallbackLayerMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARPlacementInteractable.objectPlaced"/>.</summary>
        protected SerializedProperty m_ObjectPlaced;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARPlacementInteractable.onObjectPlaced"/>.</summary>
        protected SerializedProperty m_OnObjectPlaced;
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="ARPlacementInteractable.onObjectPlaced"/>.</summary>
        protected SerializedProperty m_OnObjectPlacedCalls;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="ARPlacementInteractable.onObjectPlaced"/>.</summary>
            public static readonly GUIContent onObjectPlaced = EditorGUIUtility.TrTextContent("(Deprecated) On Object Placed");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARPlacementInteractableEditorDeprecated);
        }

        /// <inheritdoc />
        protected override void DrawInteractableEventsNested()
        {
            throw new NotSupportedException(k_ARPlacementInteractableEditorDeprecated);
        }
    }
}
