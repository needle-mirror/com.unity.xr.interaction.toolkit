using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Readers
{
    [CustomPropertyDrawer(typeof(XRInputValueReader<>), true)]
    class XRInputValueReaderPropertyDrawer : XRBaseInputReaderPropertyDrawer<XRInputValueReaderPropertyDrawer.SerializedPropertyFields>
    {
        public class SerializedPropertyFields : BaseSerializedPropertyFields
        {
            public SerializedProperty inputSourceMode;
            public SerializedProperty inputAction;
            public SerializedProperty inputActionReference;
            public SerializedProperty objectReferenceObject;
            public SerializedProperty manualValue;

            /// <inheritdoc/>
            public override void FindProperties(SerializedProperty property)
            {
                inputSourceMode = property.FindPropertyRelative("m_InputSourceMode");
                inputAction = property.FindPropertyRelative("m_InputAction");
                inputActionReference = property.FindPropertyRelative("m_InputActionReference");
                objectReferenceObject = property.FindPropertyRelative("m_ObjectReferenceObject");
                manualValue = property.FindPropertyRelative("m_ManualValue");
            }
        }

        static SerializedProperty GetEffectiveProperty(SerializedPropertyFields fields, out GUIContent content)
        {
            switch (fields.inputSourceMode.intValue)
            {
                // ReSharper disable once RedundantCaseLabel -- Explicit case labels for clarity.
                case (int)XRInputValueReader.InputSourceMode.Unused:
                default:
                    content = GUIContent.none;
                    return null;

                case (int)XRInputValueReader.InputSourceMode.InputAction:
                    content = Contents.inputAction;
                    return fields.inputAction;

                case (int)XRInputValueReader.InputSourceMode.InputActionReference:
                    content = Contents.inputActionReference;
                    return fields.inputActionReference;

                case (int)XRInputValueReader.InputSourceMode.ObjectReference:
                    content = Contents.objectReference;
                    return fields.objectReferenceObject;

                case (int)XRInputValueReader.InputSourceMode.ManualValue:
                    content = Contents.manualValue;
                    return fields.manualValue;
            }
        }

        /// <inheritdoc/>
        protected override void PushCompactContext()
        {
            m_CompactPropertyControl.modeProperty = m_Fields.inputSourceMode;
            m_CompactPropertyControl.modePopupOptions = Contents.compactPopupOptions;
            m_CompactPropertyControl.properties.Clear();

            var effectiveProperty = GetEffectiveProperty(m_Fields, out _);
            if (effectiveProperty != null)
                m_CompactPropertyControl.properties.Add(effectiveProperty);

            m_CompactPropertyControl.hasInfoHelpBox = ShouldCheckActionEnabled(m_Fields) && IsEffectiveActionNotNullAndDisabled(m_Fields);
            m_CompactPropertyControl.infoHelpBoxMessage = Contents.actionIsDisabledText;
        }

        /// <inheritdoc/>
        protected override void PushMultilineContext(bool showEffectiveOnly)
        {
            m_MultilinePropertyControl.modeProperty = m_Fields.inputSourceMode;
            m_MultilinePropertyControl.modePopupOptions = Contents.compactPopupOptions;
            m_MultilinePropertyControl.properties.Clear();
            m_MultilinePropertyControl.propertiesContent.Clear();

            if (showEffectiveOnly)
            {
                var effectiveProperty = GetEffectiveProperty(m_Fields, out var effectivePropertyContent);
                if (effectiveProperty != null)
                {
                    m_MultilinePropertyControl.properties.Add(effectiveProperty);
                    m_MultilinePropertyControl.propertiesContent.Add(effectivePropertyContent);
                }
            }
            else
            {
                m_MultilinePropertyControl.properties.Add(m_Fields.inputAction);
                m_MultilinePropertyControl.properties.Add(m_Fields.inputActionReference);
                m_MultilinePropertyControl.properties.Add(m_Fields.objectReferenceObject);
                m_MultilinePropertyControl.properties.Add(m_Fields.manualValue);

                m_MultilinePropertyControl.propertiesContent.Add(Contents.inputAction);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.inputActionReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.objectReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.manualValue);
            }

            m_MultilinePropertyControl.hasInfoHelpBox = ShouldCheckActionEnabled(m_Fields) && IsEffectiveActionNotNullAndDisabled(m_Fields);
            m_MultilinePropertyControl.infoHelpBoxMessage = Contents.actionIsDisabledText;
        }

        static bool ShouldCheckActionEnabled(SerializedPropertyFields fields)
        {
            return Application.isPlaying &&
                (fields.inputSourceMode.intValue == (int)XRInputValueReader.InputSourceMode.InputAction ||
                    fields.inputSourceMode.intValue == (int)XRInputValueReader.InputSourceMode.InputActionReference);
        }

        bool IsEffectiveActionNotNullAndDisabled(SerializedPropertyFields fields)
        {
            if (fields.inputSourceMode.intValue == (int)XRInputValueReader.InputSourceMode.InputAction)
                return IsActionNotNullAndDisabled(fields.inputAction);

            if (fields.inputSourceMode.intValue == (int)XRInputValueReader.InputSourceMode.InputActionReference)
                return IsActionReferenceNotNullAndDisabled(fields.inputActionReference);

            return false;
        }

        static class Contents
        {
            public static readonly GUIContent actionIsDisabledText = EditorGUIUtility.TrTextContent("Action is disabled.");
            public static readonly GUIContent inputAction = EditorGUIUtility.TrTextContent("Input Action");
            public static readonly GUIContent inputActionReference = EditorGUIUtility.TrTextContent("Input Action Reference");
            public static readonly GUIContent objectReference = EditorGUIUtility.TrTextContent("Object Reference");
            public static readonly GUIContent manualValue = EditorGUIUtility.TrTextContent("Manual Value");
            public static readonly GUIContent[] compactPopupOptions =
            {
                EditorGUIUtility.TrTextContent("Unused"),
                EditorGUIUtility.TrTextContent("Input Action"),
                EditorGUIUtility.TrTextContent("Input Action Reference"),
                EditorGUIUtility.TrTextContent("Object Reference"),
                EditorGUIUtility.TrTextContent("Manual Value"),
            };
        }
    }
}
