#if XR_HANDS_1_8_OR_NEWER
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Custom property drawer for <see cref="SimulatedHandExpression"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SimulatedHandExpression))]
    class SimulatedHandExpressionPropertyDrawer : PropertyDrawer
    {
        const string k_NamePath = "m_Name";
        const string k_ToggleInputPath = "m_ToggleInput";
        const string k_CaptureSequencePath = "m_CaptureSequence";
        const string k_SequenceTypePath = "m_SequenceType";
        const string k_SingleFrameIndexPath = "m_SingleFrameIndex";
        const string k_MultiFrameStartIndexPath = "m_MultiFrameStartIndex";
        const string k_MultiFrameEndIndexPath = "m_MultiFrameEndIndex";
        const string k_MultiFrameReverseStartIndexPath = "m_MultiFrameReverseStartIndex";
        const string k_MultiFrameReverseEndIndexPath = "m_MultiFrameReverseEndIndex";
        const string k_InBetweenKeyFrameIndexListPath = "m_InBetweenKeyFrameIndexList";
        const string k_IsQuickActionPath = "m_IsQuickAction";

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                var name = property.FindPropertyRelative(k_NamePath);
                var toggleInput = property.FindPropertyRelative(k_ToggleInputPath);
                var captureSequence = property.FindPropertyRelative(k_CaptureSequencePath);
                var sequenceType = property.FindPropertyRelative(k_SequenceTypePath);
                var isQuickAction = property.FindPropertyRelative(k_IsQuickActionPath);

                var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                DrawField(name);
                DrawField(toggleInput);
                DrawField(captureSequence);
                DrawField(sequenceType);

                EditorGUI.indentLevel++;

                var isSingleFrame = sequenceType.intValue == (int)SimulatedHandExpression.SequenceType.SingleFrame;
                if (isSingleFrame)
                {
                    var singleFrameIndex = property.FindPropertyRelative(k_SingleFrameIndexPath);

                    DrawField(singleFrameIndex);
                }
                else
                {
                    var multiFrameStartIndex = property.FindPropertyRelative(k_MultiFrameStartIndexPath);
                    var multiFrameEndIndex = property.FindPropertyRelative(k_MultiFrameEndIndexPath);
                    var multiFrameReverseStartIndex = property.FindPropertyRelative(k_MultiFrameReverseStartIndexPath);
                    var multiFrameReverseEndIndex = property.FindPropertyRelative(k_MultiFrameReverseEndIndexPath);
                    var inBetweenKeyFrameIndexList = property.FindPropertyRelative(k_InBetweenKeyFrameIndexListPath);

                    DrawField(multiFrameStartIndex);
                    DrawField(multiFrameEndIndex);
                    DrawField(multiFrameReverseStartIndex);
                    DrawField(multiFrameReverseEndIndex);
                    DrawField(inBetweenKeyFrameIndexList, true);
                }

                EditorGUI.indentLevel--;

                DrawField(isQuickAction);

                void DrawField(SerializedProperty prop, bool includeChildren = false)
                {
                    var height = EditorGUI.GetPropertyHeight(prop, includeChildren);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, includeChildren);
                    y += height + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            EditorGUI.EndProperty();
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Always reserve space for the foldout header row
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var total = EditorGUIUtility.singleLineHeight + spacing;

            if (!property.isExpanded)
                return total - spacing;

            var sequenceType = property.FindPropertyRelative(k_SequenceTypePath);
            var isSingleFrame = sequenceType.intValue == (int)SimulatedHandExpression.SequenceType.SingleFrame;

            AddHeight(property.FindPropertyRelative(k_NamePath));
            AddHeight(property.FindPropertyRelative(k_ToggleInputPath));
            AddHeight(property.FindPropertyRelative(k_CaptureSequencePath));
            AddHeight(property.FindPropertyRelative(k_SequenceTypePath));

            if (isSingleFrame)
            {
                AddHeight(property.FindPropertyRelative(k_SingleFrameIndexPath));
            }
            else
            {
                AddHeight(property.FindPropertyRelative(k_MultiFrameStartIndexPath));
                AddHeight(property.FindPropertyRelative(k_MultiFrameEndIndexPath));
                AddHeight(property.FindPropertyRelative(k_MultiFrameReverseStartIndexPath));
                AddHeight(property.FindPropertyRelative(k_MultiFrameReverseEndIndexPath));
                AddHeight(property.FindPropertyRelative(k_InBetweenKeyFrameIndexListPath), true);
            }

            AddHeight(property.FindPropertyRelative(k_IsQuickActionPath));

            // Remove the trailing spacing added after the last field
            return total - spacing;

            void AddHeight(SerializedProperty prop, bool includeChildren = false)
            {
                total += EditorGUI.GetPropertyHeight(prop, includeChildren) + spacing;
            }
        }
    }
}
#endif
