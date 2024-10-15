using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Custom editor for a <see cref="GazeTeleportationAnchorFilter"/>.
    /// </summary>
    [CustomEditor(typeof(GazeTeleportationAnchorFilter), true), CanEditMultipleObjects]
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    public class GazeTeleportationAnchorFilterEditor : Editor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GazeTeleportationAnchorFilter.maxGazeAngle"/>.</summary>
        protected SerializedProperty m_MaxGazeAngle;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GazeTeleportationAnchorFilter.gazeAngleScoreCurve"/>.</summary>
        protected SerializedProperty m_GazeAngleScoreCurve;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GazeTeleportationAnchorFilter.enableDistanceWeighting"/>.</summary>
        protected SerializedProperty m_EnableDistanceWeighting;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GazeTeleportationAnchorFilter.distanceWeightCurve"/>.</summary>
        protected SerializedProperty m_DistanceWeightCurve;

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_MaxGazeAngle = serializedObject.FindProperty("m_MaxGazeAngle");
            m_GazeAngleScoreCurve = serializedObject.FindProperty("m_GazeAngleScoreCurve");
            m_EnableDistanceWeighting = serializedObject.FindProperty("m_EnableDistanceWeighting");
            m_DistanceWeightCurve = serializedObject.FindProperty("m_DistanceWeightCurve");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_MaxGazeAngle);
            EditorGUILayout.PropertyField(m_GazeAngleScoreCurve);
            EditorGUILayout.PropertyField(m_EnableDistanceWeighting);
            if (m_EnableDistanceWeighting.boolValue)
                EditorGUILayout.PropertyField(m_DistanceWeightCurve);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
