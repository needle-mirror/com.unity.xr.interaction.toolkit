using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Readers
{
    [CustomPropertyDrawer(typeof(XRInputButtonReader), true)]
    class XRInputButtonReaderPropertyDrawer : XRBaseInputReaderPropertyDrawer<XRInputButtonReaderPropertyDrawer.SerializedPropertyFields>
    {
        // Corresponds to the values in InputBindings.Flags, which is internal.
        const int k_CompositeBindingFlags = 1 << 2;
        const int k_PartOfCompositeBindingFlags = 1 << 3;

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
            public SerializedProperty manualQueueWasCompletedThisFrame;
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
                manualQueueWasCompletedThisFrame = property.FindPropertyRelative("m_ManualQueueWasCompletedThisFrame");
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
            m_CompactPropertyControl.propertiesWarningMessage.Clear();

            var numProperties = GetEffectiveProperties(m_Fields, out var performed, out _,
                out var value, out _);

            if (numProperties > 0)
                m_CompactPropertyControl.properties.Add(performed);

            if (numProperties > 1)
                m_CompactPropertyControl.properties.Add(value);

            GetEffectivePropertyWarningState(m_Fields, out var hasPropertyWarning, out var propertyWarningMessage);
            if (hasPropertyWarning)
                m_CompactPropertyControl.propertiesWarningMessage.Add(propertyWarningMessage);

            GetInfoHelpBoxState(m_Fields, out var hasWarningHelpBox, out var warningHelpBoxMessage);
            m_CompactPropertyControl.hasInfoHelpBox = hasWarningHelpBox;
            m_CompactPropertyControl.infoHelpBoxMessage = warningHelpBoxMessage;

            GetHelpState(m_Fields, out var hasHelpTooltip, out var helpTooltip);
            m_CompactPropertyControl.hasHelpTooltip = hasHelpTooltip;
            m_CompactPropertyControl.helpTooltip = helpTooltip;
        }

        /// <inheritdoc/>
        protected override void PushMultilineContext(bool showEffectiveOnly)
        {
            m_MultilinePropertyControl.modeProperty = m_Fields.inputSourceMode;
            m_CompactPropertyControl.modePopupOptions = Contents.compactPopupOptions;
            m_MultilinePropertyControl.properties.Clear();
            m_MultilinePropertyControl.propertiesContent.Clear();
            m_MultilinePropertyControl.propertiesWarningMessage.Clear();

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

                GetEffectivePropertyWarningState(m_Fields, out var hasPropertyWarning, out var propertyWarningMessage);
                if (hasPropertyWarning)
                    m_MultilinePropertyControl.propertiesWarningMessage.Add(propertyWarningMessage);
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

                GetDirectActionPropertyWarningState(m_Fields.inputActionPerformed, out var hasPropertyWarning, out var propertyWarningMessage);
                m_MultilinePropertyControl.propertiesWarningMessage.Add(hasPropertyWarning ? propertyWarningMessage : null);
                m_MultilinePropertyControl.propertiesWarningMessage.Add(null);
                GetPropertyWarningState(m_Fields.inputActionReferencePerformed, out hasPropertyWarning, out propertyWarningMessage);
                m_MultilinePropertyControl.propertiesWarningMessage.Add(hasPropertyWarning ? propertyWarningMessage : null);
            }

            GetInfoHelpBoxState(m_Fields, out var hasInfoHelpBox, out var infoHelpBoxMessage);
            m_MultilinePropertyControl.hasInfoHelpBox = hasInfoHelpBox;
            m_MultilinePropertyControl.infoHelpBoxMessage = infoHelpBoxMessage;
        }

        static void GetEffectivePropertyWarningState(SerializedPropertyFields fields, out bool hasPropertyWarning, out string propertyWarningMessage)
        {
            if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputAction)
            {
                GetDirectActionPropertyWarningState(fields.inputActionPerformed, out hasPropertyWarning, out propertyWarningMessage);
                return;
            }

            if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputActionReference)
            {
                GetPropertyWarningState(fields.inputActionReferencePerformed, out hasPropertyWarning, out propertyWarningMessage);
                return;
            }

            hasPropertyWarning = false;
            propertyWarningMessage = null;
        }

        static void GetPropertyWarningState(SerializedProperty inputActionReferencePerformed, out bool hasPropertyWarning, out string propertyWarningMessage)
        {
            var actionReference = inputActionReferencePerformed.objectReferenceValue as InputActionReference;
            if (actionReference != null && actionReference.asset != null)
            {
                // This is an alternative to getting the action using actionReference.action.
                // When the user follows the warning message and adds the Press interaction to the action/binding
                // and saves the asset, the actionReference.action will return a stale value of the Input Action
                // until Input System invalidates the reference, such as upon entering play mode. The stale action
                // will have the old state of the action, which will not have the Press interaction added.
                // So in order to avoid confusion where the warning icon would still show up until some time later,
                // and to ensure we are validating against the up to date version of the action, we have to get
                // the action from the asset directly. We use FindProperty to get the ID of the action since it
                // isn't available as a public property.
                var actionReferenceSerializedObject = new SerializedObject(actionReference);
                actionReferenceSerializedObject.UpdateIfRequiredOrScript();
                var actionIdProperty = actionReferenceSerializedObject.FindProperty("m_ActionId");
                var action = actionReference.asset.FindAction(new Guid(actionIdProperty.stringValue));

                GetPropertyWarningState(action, out hasPropertyWarning, out propertyWarningMessage);
                return;
            }

            hasPropertyWarning = false;
            propertyWarningMessage = null;
        }

        static void GetDirectActionPropertyWarningState(SerializedProperty inputActionPerformed, out bool hasPropertyWarning, out string propertyWarningMessage)
        {
            var actionType = (InputActionType)inputActionPerformed.FindPropertyRelative("m_Type").intValue;
            if (actionType == InputActionType.Value || actionType == InputActionType.PassThrough)
            {
                // Determine if any of the bindings has the default interaction.
                if (string.IsNullOrEmpty(inputActionPerformed.FindPropertyRelative("m_Interactions").stringValue))
                {
                    var singletonActionBindings = inputActionPerformed.FindPropertyRelative("m_SingletonActionBindings");
                    for (var i = 0; i < singletonActionBindings.arraySize; ++i)
                    {
                        var binding = CreateInputBinding(singletonActionBindings.GetArrayElementAtIndex(i));
                        if (binding.isPartOfComposite)
                            continue;

                        if (string.IsNullOrEmpty(binding.interactions))
                        {
                            hasPropertyWarning = true;
                            propertyWarningMessage = string.Format(Contents.performedActionIsNotButtonLike.text, GetDisplayString(binding), actionType);
                            return;
                        }
                    }
                }
            }

            hasPropertyWarning = false;
            propertyWarningMessage = null;
        }

        static void GetPropertyWarningState(InputAction action, out bool hasPropertyWarning, out string propertyWarningMessage)
        {
            if (action != null && (action.type == InputActionType.Value || action.type == InputActionType.PassThrough))
            {
                // Determine if any of the bindings has the default interaction.
                if (string.IsNullOrEmpty(action.interactions))
                {
                    var bindings = action.bindings;
                    for (var i = 0; i < bindings.Count; ++i)
                    {
                        var binding = bindings[i];
                        if (binding.isPartOfComposite)
                            continue;

                        if (string.IsNullOrEmpty(binding.interactions))
                        {
                            hasPropertyWarning = true;
                            propertyWarningMessage = string.Format(Contents.performedActionIsNotButtonLike.text, GetDisplayString(binding), action.type);
                            return;
                        }
                    }
                }
            }

            hasPropertyWarning = false;
            propertyWarningMessage = null;
        }

        static string GetDisplayString(InputBinding binding)
        {
            return binding.ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames |
                InputBinding.DisplayStringOptions.DontOmitDevice |
                InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }

        static InputBinding CreateInputBinding(SerializedProperty bindingProperty)
        {
            if (bindingProperty == null)
                return default;

            var flagsProperty = bindingProperty.FindPropertyRelative("m_Flags");
            var flags = flagsProperty.intValue;
            var binding = new InputBinding(path: bindingProperty.FindPropertyRelative("m_Path").stringValue,
                action: bindingProperty.FindPropertyRelative("m_Action").stringValue,
                groups: bindingProperty.FindPropertyRelative("m_Groups").stringValue,
                processors: bindingProperty.FindPropertyRelative("m_Processors").stringValue,
                interactions: bindingProperty.FindPropertyRelative("m_Interactions").stringValue,
                name: bindingProperty.FindPropertyRelative("m_Name").stringValue)
            {
                isComposite = (flags & k_CompositeBindingFlags) == k_CompositeBindingFlags,
                isPartOfComposite = (flags & k_PartOfCompositeBindingFlags) == k_PartOfCompositeBindingFlags,
                id = new Guid(bindingProperty.FindPropertyRelative("m_Id").stringValue),
            };


            return binding;
        }

        void GetInfoHelpBoxState(SerializedPropertyFields fields, out bool hasInfoHelpBox, out GUIContent infoHelpBoxMessage)
        {
            var performedDisabled = false;
            var valueDisabled = false;
            hasInfoHelpBox = ShouldCheckActionEnabled(fields) && IsEffectiveActionNotNullAndDisabled(fields, out performedDisabled, out valueDisabled);
            infoHelpBoxMessage = Contents.GetActionDisabledText(performedDisabled, valueDisabled);
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

        static void GetHelpState(SerializedPropertyFields fields, out bool hasHelpTooltip, out string helpTooltip)
        {
            if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputAction ||
                fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.InputActionReference)
            {
                hasHelpTooltip = true;
                helpTooltip = Contents.actionsHelpTooltip.text;
                return;
            }

            if (fields.inputSourceMode.intValue == (int)XRInputButtonReader.InputSourceMode.ManualValue)
            {
                hasHelpTooltip = true;
                helpTooltip = Contents.manualHelpTooltip.text;
                return;
            }

            hasHelpTooltip = false;
            helpTooltip = null;
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
                m_Fields.manualQueueWasCompletedThisFrame.boolValue = !requestedPerformed;
                m_Fields.manualQueueValue.floatValue = m_Fields.manualValue.floatValue;
                m_Fields.manualQueueTargetFrame.intValue = Time.frameCount + 1;
            }
        }

        static class Contents
        {
            public static readonly GUIContent performedActionIsNotButtonLike = EditorGUIUtility.TrTextContent("{0} is using the default interaction on a {1} type Input Action. You should consider either adding a Press interaction to the action/binding or change the type of the Input Action to Button to get button-like interaction.");
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
