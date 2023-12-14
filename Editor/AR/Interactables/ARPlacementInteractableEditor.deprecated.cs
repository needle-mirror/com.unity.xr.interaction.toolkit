using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARPlacementInteractable"/>.
    /// </summary>
    [Obsolete("ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.")]
    [CustomEditor(typeof(ARPlacementInteractable), true), CanEditMultipleObjects]
    public class ARPlacementInteractableEditor : ARBaseGestureInteractableEditor
    {
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
            base.OnEnable();

            m_PlacementPrefab = serializedObject.FindProperty("m_PlacementPrefab");
            m_FallbackLayerMask = serializedObject.FindProperty("m_FallbackLayerMask");
            m_ObjectPlaced = serializedObject.FindProperty("m_ObjectPlaced");

            m_OnObjectPlaced = serializedObject.FindProperty("m_OnObjectPlaced");
            m_OnObjectPlacedCalls = m_OnObjectPlaced.FindPropertyRelative("m_PersistentCalls.m_Calls");
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            base.DrawProperties();
            EditorGUILayout.PropertyField(m_PlacementPrefab);
            EditorGUILayout.PropertyField(m_FallbackLayerMask);
        }

        /// <inheritdoc />
        protected override void DrawInteractableEventsNested()
        {
            EditorGUILayout.PropertyField(m_ObjectPlaced);
            if (m_OnObjectPlacedCalls.arraySize > 0 || m_OnObjectPlacedCalls.hasMultipleDifferentValues)
                EditorGUILayout.PropertyField(m_OnObjectPlaced, Contents.onObjectPlaced);
            base.DrawInteractableEventsNested();
        }

        /// <inheritdoc />
        [Obsolete("ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.", true)]
        protected override bool IsDeprecatedEventsInUse()
        {
            return default;
        }

        /// <inheritdoc />
        [Obsolete("ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.", true)]
        protected override void MigrateEvents(SerializedObject serializedObject)
        {
            base.MigrateEvents(serializedObject);
        }
    }
}
