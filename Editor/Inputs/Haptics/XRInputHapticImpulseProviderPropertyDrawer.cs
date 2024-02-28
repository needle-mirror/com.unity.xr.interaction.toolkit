using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Haptics
{
    [CustomPropertyDrawer(typeof(XRInputHapticImpulseProvider), true)]
    class XRInputHapticImpulseProviderPropertyDrawer : XRBaseInputReaderPropertyDrawer<XRInputHapticImpulseProviderPropertyDrawer.SerializedPropertyFields>
    {
        public class SerializedPropertyFields : BaseSerializedPropertyFields
        {
            public SerializedProperty inputSourceMode;
            public SerializedProperty inputAction;
            public SerializedProperty inputActionReference;
            public SerializedProperty objectReferenceObject;

            /// <inheritdoc/>
            public override void FindProperties(SerializedProperty property)
            {
                inputSourceMode = property.FindPropertyRelative("m_InputSourceMode");
                inputAction = property.FindPropertyRelative("m_InputAction");
                inputActionReference = property.FindPropertyRelative("m_InputActionReference");
                objectReferenceObject = property.FindPropertyRelative("m_ObjectReferenceObject");
            }
        }

        static SerializedProperty GetEffectiveProperty(SerializedPropertyFields fields, out GUIContent content)
        {
            switch (fields.inputSourceMode.intValue)
            {
                // ReSharper disable once RedundantCaseLabel -- Explicit case labels for clarity.
                case (int)XRInputHapticImpulseProvider.InputSourceMode.Unused:
                default:
                    content = GUIContent.none;
                    return null;

                case (int)XRInputHapticImpulseProvider.InputSourceMode.InputAction:
                    content = Contents.inputAction;
                    return fields.inputAction;

                case (int)XRInputHapticImpulseProvider.InputSourceMode.InputActionReference:
                    content = Contents.inputActionReference;
                    return fields.inputActionReference;

                case (int)XRInputHapticImpulseProvider.InputSourceMode.ObjectReference:
                    content = Contents.objectReference;
                    return fields.objectReferenceObject;
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

                m_MultilinePropertyControl.propertiesContent.Add(Contents.inputAction);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.inputActionReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.objectReference);
            }

            m_MultilinePropertyControl.hasInfoHelpBox = ShouldCheckActionEnabled(m_Fields) && IsEffectiveActionNotNullAndDisabled(m_Fields);
            m_MultilinePropertyControl.infoHelpBoxMessage = Contents.actionIsDisabledText;
        }

        static bool ShouldCheckActionEnabled(SerializedPropertyFields fields)
        {
            return Application.isPlaying &&
                (fields.inputSourceMode.intValue == (int)XRInputHapticImpulseProvider.InputSourceMode.InputAction ||
                    fields.inputSourceMode.intValue == (int)XRInputHapticImpulseProvider.InputSourceMode.InputActionReference);
        }

        bool IsEffectiveActionNotNullAndDisabled(SerializedPropertyFields fields)
        {
            if (fields.inputSourceMode.intValue == (int)XRInputHapticImpulseProvider.InputSourceMode.InputAction)
                return IsActionNotNullAndDisabled(fields.inputAction);

            if (fields.inputSourceMode.intValue == (int)XRInputHapticImpulseProvider.InputSourceMode.InputActionReference)
                return IsActionReferenceNotNullAndDisabled(fields.inputActionReference);

            return false;
        }

        static class Contents
        {
            public static readonly GUIContent actionIsDisabledText = EditorGUIUtility.TrTextContent("Action is disabled.");
            public static readonly GUIContent inputAction = EditorGUIUtility.TrTextContent("Input Action");
            public static readonly GUIContent inputActionReference = EditorGUIUtility.TrTextContent("Input Action Reference");
            public static readonly GUIContent objectReference = EditorGUIUtility.TrTextContent("Object Reference");
            public static readonly GUIContent[] compactPopupOptions =
            {
                EditorGUIUtility.TrTextContent("Unused"),
                EditorGUIUtility.TrTextContent("Input Action"),
                EditorGUIUtility.TrTextContent("Input Action Reference"),
                EditorGUIUtility.TrTextContent("Object Reference"),
            };
        }
    }
}
