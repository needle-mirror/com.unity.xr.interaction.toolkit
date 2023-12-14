using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Readers
{
    [CustomPropertyDrawer(typeof(XRInputButtonReader), true)]
    class XRInputButtonReaderPropertyDrawer : XRBaseInputReaderPropertyDrawer<XRInputButtonReaderPropertyDrawer.SerializedPropertyFields>
    {
        public class SerializedPropertyFields : BaseSerializedPropertyFields
        {
            public SerializedProperty inputSourceMode;
            public SerializedProperty inputActionPerformed;
            public SerializedProperty inputActionValue;
            public SerializedProperty inputActionReferencePerformed;
            public SerializedProperty inputActionReferenceValue;
            public SerializedProperty objectReferenceObject;
            public SerializedProperty manualPerformed;
            public SerializedProperty manualValue;

            public SerializedProperty manualQueuePerformed;
            public SerializedProperty manualQueueWasPerformedThisFrame;
            public SerializedProperty manualQueueValue;
            public SerializedProperty manualQueueTargetFrame;

            /// <inheritdoc/>
            public override void FindProperties(SerializedProperty property)
            {
                inputSourceMode = property.FindPropertyRelative("m_InputSourceMode");
                inputActionPerformed = property.FindPropertyRelative("m_InputActionPerformed");
                inputActionValue = property.FindPropertyRelative("m_InputActionValue");
                inputActionReferencePerformed = property.FindPropertyRelative("m_InputActionReferencePerformed");
                inputActionReferenceValue = property.FindPropertyRelative("m_InputActionReferenceValue");
                objectReferenceObject = property.FindPropertyRelative("m_ObjectReferenceObject");
                manualPerformed = property.FindPropertyRelative("m_ManualPerformed");
                manualValue = property.FindPropertyRelative("m_ManualValue");

                manualQueuePerformed = property.FindPropertyRelative("m_ManualQueuePerformed");
                manualQueueWasPerformedThisFrame = property.FindPropertyRelative("m_ManualQueueWasPerformedThisFrame");
                manualQueueValue = property.FindPropertyRelative("m_ManualQueueValue");
                manualQueueTargetFrame = property.FindPropertyRelative("m_ManualQueueTargetFrame");
            }
        }

        static int GetEffectiveProperties(SerializedPropertyFields fields,
            out SerializedProperty performed, out GUIContent performedContent,
            out SerializedProperty value, out GUIContent valueContent)
        {
            switch (fields.inputSourceMode.intValue)
            {
                // ReSharper disable once RedundantCaseLabel -- Explicit case labels for clarity.
                case (int)XRInputButtonReader.InputSourceMode.Unused:
                default:
                    performed = null;
                    performedContent = GUIContent.none;
                    value = null;
                    valueContent = GUIContent.none;
                    return 0;

                case (int)XRInputButtonReader.InputSourceMode.InputAction:
                    performed = fields.inputActionPerformed;
                    performedContent = Contents.performedInputAction;
                    value = fields.inputActionValue;
                    valueContent = Contents.valueInputAction;
                    return 2;

                case (int)XRInputButtonReader.InputSourceMode.InputActionReference:
                    performed = fields.inputActionReferencePerformed;
                    performedContent = Contents.performedInputActionReference;
                    value = fields.inputActionReferenceValue;
                    valueContent = Contents.valueInputActionReference;
                    return 2;

                case (int)XRInputButtonReader.InputSourceMode.ObjectReference:
                    performed = fields.objectReferenceObject;
                    performedContent = Contents.objectReference;
                    value = fields.objectReferenceObject;
                    valueContent = Contents.objectReference;
                    return 1;

                case (int)XRInputButtonReader.InputSourceMode.ManualValue:
                    performed = fields.manualPerformed;
                    performedContent = Contents.manualPerformed;
                    value = fields.manualValue;
                    valueContent = Contents.manualValue;
                    return 2;
            }
        }

        /// <inheritdoc/>
        protected override void PushCompactContext()
        {
            m_CompactPropertyControl.modeProperty = m_Fields.inputSourceMode;
            m_CompactPropertyControl.modePopupOptions = Contents.compactPopupOptions;
            m_CompactPropertyControl.properties.Clear();

            var numProperties = GetEffectiveProperties(m_Fields, out var performed, out _,
                out var value, out _);

            if (numProperties > 0)
                m_CompactPropertyControl.properties.Add(performed);

            if (numProperties > 1)
                m_CompactPropertyControl.properties.Add(value);

            GetWarningState(out var hasWarningHelpBox, out var warningHelpBoxMessage);
            m_CompactPropertyControl.hasWarningHelpBox = hasWarningHelpBox;
            m_CompactPropertyControl.warningHelpBoxMessage = warningHelpBoxMessage;

            if (m_Fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputAction ||
                m_Fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputActionReference)
            {
                m_CompactPropertyControl.hasHelpTooltip = true;
                m_CompactPropertyControl.helpTooltip = Contents.actionsHelpTooltip.text;
            }
            else if (m_Fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.ManualValue)
            {
                m_CompactPropertyControl.hasHelpTooltip = true;
                m_CompactPropertyControl.helpTooltip = Contents.manualHelpTooltip.text;
            }
            else
            {
                m_CompactPropertyControl.hasHelpTooltip = false;
                m_CompactPropertyControl.helpTooltip = null;
            }
        }

        /// <inheritdoc/>
        protected override void PushMultilineContext(bool showEffectiveOnly)
        {
            m_MultilinePropertyControl.modeProperty = m_Fields.inputSourceMode;
            m_CompactPropertyControl.modePopupOptions = Contents.compactPopupOptions;
            m_MultilinePropertyControl.properties.Clear();
            m_MultilinePropertyControl.propertiesContent.Clear();

            if (showEffectiveOnly)
            {
                var numProperties = GetEffectiveProperties(m_Fields, out var performed, out var performedContent,
                    out var value, out var valueContent);

                if (numProperties > 0)
                {
                    m_MultilinePropertyControl.properties.Add(performed);
                    m_MultilinePropertyControl.propertiesContent.Add(performedContent);
                }

                if (numProperties > 1)
                {
                    m_MultilinePropertyControl.properties.Add(value);
                    m_MultilinePropertyControl.propertiesContent.Add(valueContent);
                }
            }
            else
            {
                m_MultilinePropertyControl.properties.Add(m_Fields.inputActionPerformed);
                m_MultilinePropertyControl.properties.Add(m_Fields.inputActionValue);
                m_MultilinePropertyControl.properties.Add(m_Fields.inputActionReferencePerformed);
                m_MultilinePropertyControl.properties.Add(m_Fields.inputActionReferenceValue);
                m_MultilinePropertyControl.properties.Add(m_Fields.objectReferenceObject);
                m_MultilinePropertyControl.properties.Add(m_Fields.manualPerformed);
                m_MultilinePropertyControl.properties.Add(m_Fields.manualValue);

                m_MultilinePropertyControl.propertiesContent.Add(Contents.performedInputAction);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.valueInputAction);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.performedInputActionReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.valueInputActionReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.objectReference);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.manualPerformed);
                m_MultilinePropertyControl.propertiesContent.Add(Contents.manualValue);
            }

            GetWarningState(out var hasWarningHelpBox, out var warningHelpBoxMessage);
            m_MultilinePropertyControl.hasWarningHelpBox = hasWarningHelpBox;
            m_MultilinePropertyControl.warningHelpBoxMessage = warningHelpBoxMessage;
        }

        void GetWarningState(out bool hasWarningHelpBox, out GUIContent warningHelpBoxMessage)
        {
            var performedDisabled = false;
            var valueDisabled = false;
            hasWarningHelpBox = ShouldCheckActionEnabled(m_Fields) && IsEffectiveActionNotNullAndDisabled(m_Fields, out performedDisabled, out valueDisabled);
            warningHelpBoxMessage = Contents.GetActionDisabledText(performedDisabled, valueDisabled);
        }

        static bool ShouldCheckActionEnabled(SerializedPropertyFields fields)
        {
            return Application.isPlaying &&
                (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputAction ||
                    fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputActionReference);
        }

        bool IsEffectiveActionNotNullAndDisabled(SerializedPropertyFields fields, out bool performedDisabled, out bool valueDisabled)
        {
            performedDisabled = false;
            valueDisabled = false;

            if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputAction)
            {
                performedDisabled = IsActionNotNullAndDisabled(fields.inputActionPerformed);
                valueDisabled = IsActionNotNullAndDisabled(fields.inputActionValue, true);
            }
            else if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputActionReference)
            {
                performedDisabled = IsActionReferenceNotNullAndDisabled(fields.inputActionReferencePerformed);
                valueDisabled = IsActionReferenceNotNullAndDisabled(fields.inputActionReferenceValue);
            }

            return performedDisabled || valueDisabled;
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(SerializedProperty property)
        {
            // While playing, set the frame performed to the next frame so the value will be considered performed when it is read next frame.
            if (Application.isPlaying &&
                m_Fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.ManualValue &&
                m_Fields.manualPerformed == property)
            {
                var requestedPerformed = m_Fields.manualPerformed.boolValue;

                // Revert the performed value so it can be set again next frame.
                m_Fields.manualPerformed.boolValue = !m_Fields.manualPerformed.boolValue;

                // Duplicate logic from QueueManualState to set the performed value next frame.
                m_Fields.manualQueuePerformed.boolValue = requestedPerformed;
                m_Fields.manualQueueWasPerformedThisFrame.boolValue = requestedPerformed;
                m_Fields.manualQueueValue.floatValue = m_Fields.manualValue.floatValue;
                m_Fields.manualQueueTargetFrame.intValue = Time.frameCount + 1;
            }
        }

        static class Contents
        {
            public static readonly GUIContent performedActionIsDisabledText = EditorGUIUtility.TrTextContent("Performed action is disabled.");
            public static readonly GUIContent valueActionIsDisabledText = EditorGUIUtility.TrTextContent("Value action is disabled.");
            public static readonly GUIContent actionsAreDisabledText = EditorGUIUtility.TrTextContent("Actions are disabled.");
            public static readonly GUIContent performedInputAction = EditorGUIUtility.TrTextContent("Performed Input Action");
            public static readonly GUIContent valueInputAction = EditorGUIUtility.TrTextContent("Value Input Action");
            public static readonly GUIContent performedInputActionReference = EditorGUIUtility.TrTextContent("Performed Input Action Reference");
            public static readonly GUIContent valueInputActionReference = EditorGUIUtility.TrTextContent("Value Input Action Reference");
            public static readonly GUIContent objectReference = EditorGUIUtility.TrTextContent("Object Reference");
            public static readonly GUIContent manualPerformed = EditorGUIUtility.TrTextContent("Manual Performed");
            public static readonly GUIContent manualValue = EditorGUIUtility.TrTextContent("Manual Value");
            public static readonly GUIContent actionsHelpTooltip = EditorGUIUtility.TrTextContent("The first action is whether the button is down. The second action is the scalar value that varies from 0 to 1. Can be the same input action.");
            public static readonly GUIContent manualHelpTooltip = EditorGUIUtility.TrTextContent("The first row is whether the button is down. The second row is the scalar value that varies from 0 to 1.");
            public static readonly GUIContent[] compactPopupOptions =
            {
                EditorGUIUtility.TrTextContent("Unused"),
                EditorGUIUtility.TrTextContent("Input Action"),
                EditorGUIUtility.TrTextContent("Input Action Reference"),
                EditorGUIUtility.TrTextContent("Object Reference"),
                EditorGUIUtility.TrTextContent("Manual Value"),
            };

            public static GUIContent GetActionDisabledText(bool performedDisabled, bool valueDisabled)
            {
                if (performedDisabled && valueDisabled)
                    return actionsAreDisabledText;

                if (performedDisabled)
                    return performedActionIsDisabledText;

                if (valueDisabled)
                    return valueActionIsDisabledText;

                return GUIContent.none;
            }
        }
    }
}
