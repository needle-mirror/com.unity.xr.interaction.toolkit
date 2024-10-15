using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Custom property drawer for a <see cref="TeleportVolumeDestinationSettings"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TeleportVolumeDestinationSettings))]
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    public class TeleportVolumeDestinationSettingsPropertyDrawer : PropertyDrawer
    {
        const string k_EnableDestinationEvaluationDelayPath = "m_EnableDestinationEvaluationDelay";
        const string k_DestinationEvaluationDelayTimePath = "m_DestinationEvaluationDelayTime";
        const string k_PollForDestinationChangePath = "m_PollForDestinationChange";
        const string k_DestinationPollFrequencyPath = "m_DestinationPollFrequency";
        const string k_DestinationFilterObjectPath = "m_DestinationFilterObject";

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var enableDestinationEvaluationDelay = property.FindPropertyRelative(k_EnableDestinationEvaluationDelayPath);
            var destinationEvaluationDelayTime = property.FindPropertyRelative(k_DestinationEvaluationDelayTimePath);
            var pollForDestinationChange = property.FindPropertyRelative(k_PollForDestinationChangePath);
            var destinationPollFrequency = property.FindPropertyRelative(k_DestinationPollFrequencyPath);
            var destinationFilterObject = property.FindPropertyRelative(k_DestinationFilterObjectPath);

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, enableDestinationEvaluationDelay);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (enableDestinationEvaluationDelay.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, destinationEvaluationDelayTime);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.indentLevel--;
            }

            EditorGUI.PropertyField(position, pollForDestinationChange);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (pollForDestinationChange.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, destinationPollFrequency);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.indentLevel--;
            }

            EditorGUI.PropertyField(position, destinationFilterObject);

            EditorGUI.EndProperty();
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enableDestinationEvaluationDelay = property.FindPropertyRelative(k_EnableDestinationEvaluationDelayPath);
            var pollForDestinationChange = property.FindPropertyRelative(k_PollForDestinationChangePath);

            var visiblePropertyCount = 3;
            if (enableDestinationEvaluationDelay.boolValue)
                visiblePropertyCount++;
            if (pollForDestinationChange.boolValue)
                visiblePropertyCount++;

            return visiblePropertyCount * EditorGUIUtility.singleLineHeight + (visiblePropertyCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
