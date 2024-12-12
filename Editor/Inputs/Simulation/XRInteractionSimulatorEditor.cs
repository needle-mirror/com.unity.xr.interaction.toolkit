using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Custom editor for <see cref="XRInteractionSimulator"/>.
    /// </summary>
    [CustomEditor(typeof(XRInteractionSimulator), true), CanEditMultipleObjects]
    class XRInteractionSimulatorEditor : BaseInteractionEditor
    {
        const string k_SimulatorInputsExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".SimulatorInputsExpanded";
        const string k_ControllerInputsExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".ControllerInputsExpanded";
        const string k_SimulatorSettingsExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".SimulatorSettingsExpanded";
        const string k_SensitivityExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".SensitivityExpanded";
        const string k_AnalogConfigurationExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".AnalogConfigurationExpanded";
        const string k_TrackingStateExpandedKey = "XRI." + nameof(XRInteractionSimulatorEditor) + ".TrackingStateExpanded";

        // Simulator Inputs
        bool m_SimulatorInputsExpanded;
        SerializedProperty m_TranslateXInput;
        SerializedProperty m_TranslateYInput;
        SerializedProperty m_TranslateZInput;
        SerializedProperty m_ToggleManipulateLeftInput;
        SerializedProperty m_ToggleManipulateRightInput;
        SerializedProperty m_ToggleManipulateHeadInput;
        SerializedProperty m_CycleDevicesInput;
        SerializedProperty m_KeyboardRotationDeltaInput;
        SerializedProperty m_ToggleMouseInput;
        SerializedProperty m_MouseRotationDeltaInput;
        SerializedProperty m_MouseScrollInput;
        SerializedProperty m_XConstraintInput;
        SerializedProperty m_YConstraintInput;
        SerializedProperty m_ZConstraintInput;
        SerializedProperty m_ResetInput;
        SerializedProperty m_CycleQuickActionInput;
        SerializedProperty m_TogglePerformQuickActionInput;
        SerializedProperty m_LeftDeviceActionsInput;
        SerializedProperty m_TogglePrimary2DAxisTargetInput;
        SerializedProperty m_ToggleSecondary2DAxisTargetInput;

        // Controller Inputs
        bool m_ControllerInputsExpanded;
        SerializedProperty m_Axis2DInput;
        SerializedProperty m_GripInput;
        SerializedProperty m_TriggerInput;
        SerializedProperty m_PrimaryButtonInput;
        SerializedProperty m_SecondaryButtonInput;
        SerializedProperty m_MenuInput;
        SerializedProperty m_Primary2DAxisClickInput;
        SerializedProperty m_Secondary2DAxisClickInput;
        SerializedProperty m_Primary2DAxisTouchInput;
        SerializedProperty m_Secondary2DAxisTouchInput;
        SerializedProperty m_PrimaryTouchInput;
        SerializedProperty m_SecondaryTouchInput;
        SerializedProperty m_QuickActionControllerInputModes;

        // Simulator Settings
        bool m_SimulatorSettingsExpanded;
        SerializedProperty m_CameraTransform;
        SerializedProperty m_DeviceLifecycleManager;
        SerializedProperty m_HandExpressionManager;
        SerializedProperty m_TranslateSpace;
        SerializedProperty m_InteractionSimulatorUI;

        // Sensitivity
        bool m_SensitivityExpanded;
        SerializedProperty m_TranslateXSpeed;
        SerializedProperty m_TranslateYSpeed;
        SerializedProperty m_TranslateZSpeed;
        SerializedProperty m_BodyTranslateMultiplier;
        SerializedProperty m_RotateXSensitivity;
        SerializedProperty m_RotateYSensitivity;
        SerializedProperty m_MouseScrollRotateSensitivity;
        SerializedProperty m_RotateYInvert;

        // Analog Configuration
        bool m_AnalogConfigurationExpanded;
        SerializedProperty m_GripAmount;
        SerializedProperty m_TriggerAmount;

        // Tracking State
        bool m_TrackingStateExpanded;
        SerializedProperty m_HMDIsTracked;
        SerializedProperty m_HMDTrackingState;
        SerializedProperty m_LeftControllerIsTracked;
        SerializedProperty m_LeftControllerTrackingState;
        SerializedProperty m_RightControllerIsTracked;
        SerializedProperty m_RightControllerTrackingState;
        SerializedProperty m_LeftHandIsTracked;
        SerializedProperty m_RightHandIsTracked;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIStyle foldoutTitleStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
            };
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        /// <seealso cref="MonoBehaviour"/>
        protected virtual void OnEnable()
        {
            m_TranslateXInput = serializedObject.FindProperty("m_TranslateXInput");
            m_TranslateYInput = serializedObject.FindProperty("m_TranslateYInput");
            m_TranslateZInput = serializedObject.FindProperty("m_TranslateZInput");
            m_ToggleManipulateLeftInput = serializedObject.FindProperty("m_ToggleManipulateLeftInput");
            m_ToggleManipulateRightInput = serializedObject.FindProperty("m_ToggleManipulateRightInput");
            m_ToggleManipulateHeadInput = serializedObject.FindProperty("m_ToggleManipulateHeadInput");
            m_CycleDevicesInput = serializedObject.FindProperty("m_CycleDevicesInput");
            m_KeyboardRotationDeltaInput = serializedObject.FindProperty("m_KeyboardRotationDeltaInput");
            m_ToggleMouseInput = serializedObject.FindProperty("m_ToggleMouseInput");
            m_MouseRotationDeltaInput = serializedObject.FindProperty("m_MouseRotationDeltaInput");
            m_MouseScrollInput = serializedObject.FindProperty("m_MouseScrollInput");
            m_XConstraintInput = serializedObject.FindProperty("m_XConstraintInput");
            m_YConstraintInput = serializedObject.FindProperty("m_YConstraintInput");
            m_ZConstraintInput = serializedObject.FindProperty("m_ZConstraintInput");
            m_ResetInput = serializedObject.FindProperty("m_ResetInput");
            m_CycleQuickActionInput = serializedObject.FindProperty("m_CycleQuickActionInput");
            m_TogglePerformQuickActionInput = serializedObject.FindProperty("m_TogglePerformQuickActionInput");
            m_LeftDeviceActionsInput = serializedObject.FindProperty("m_LeftDeviceActionsInput");
            m_TogglePrimary2DAxisTargetInput = serializedObject.FindProperty("m_TogglePrimary2DAxisTargetInput");
            m_ToggleSecondary2DAxisTargetInput = serializedObject.FindProperty("m_ToggleSecondary2DAxisTargetInput");

            m_Axis2DInput = serializedObject.FindProperty("m_Axis2DInput");
            m_GripInput = serializedObject.FindProperty("m_GripInput");
            m_TriggerInput = serializedObject.FindProperty("m_TriggerInput");
            m_PrimaryButtonInput = serializedObject.FindProperty("m_PrimaryButtonInput");
            m_SecondaryButtonInput = serializedObject.FindProperty("m_SecondaryButtonInput");
            m_MenuInput = serializedObject.FindProperty("m_MenuInput");
            m_Primary2DAxisClickInput = serializedObject.FindProperty("m_Primary2DAxisClickInput");
            m_Secondary2DAxisClickInput = serializedObject.FindProperty("m_Secondary2DAxisClickInput");
            m_Primary2DAxisTouchInput = serializedObject.FindProperty("m_Primary2DAxisTouchInput");
            m_Secondary2DAxisTouchInput = serializedObject.FindProperty("m_Secondary2DAxisTouchInput");
            m_PrimaryTouchInput = serializedObject.FindProperty("m_PrimaryTouchInput");
            m_SecondaryTouchInput = serializedObject.FindProperty("m_SecondaryTouchInput");
            m_QuickActionControllerInputModes = serializedObject.FindProperty("m_QuickActionControllerInputModes");

            m_CameraTransform = serializedObject.FindProperty("m_CameraTransform");
            m_DeviceLifecycleManager = serializedObject.FindProperty("m_DeviceLifecycleManager");
            m_HandExpressionManager = serializedObject.FindProperty("m_HandExpressionManager");
            m_TranslateSpace = serializedObject.FindProperty("m_TranslateSpace");
            m_InteractionSimulatorUI = serializedObject.FindProperty("m_InteractionSimulatorUI");

            m_TranslateXSpeed = serializedObject.FindProperty("m_TranslateXSpeed");
            m_TranslateYSpeed = serializedObject.FindProperty("m_TranslateYSpeed");
            m_TranslateZSpeed = serializedObject.FindProperty("m_TranslateZSpeed");
            m_BodyTranslateMultiplier = serializedObject.FindProperty("m_BodyTranslateMultiplier");
            m_RotateXSensitivity = serializedObject.FindProperty("m_RotateXSensitivity");
            m_RotateYSensitivity = serializedObject.FindProperty("m_RotateYSensitivity");
            m_MouseScrollRotateSensitivity = serializedObject.FindProperty("m_MouseScrollRotateSensitivity");
            m_RotateYInvert = serializedObject.FindProperty("m_RotateYInvert");

            m_GripAmount = serializedObject.FindProperty("m_GripAmount");
            m_TriggerAmount = serializedObject.FindProperty("m_TriggerAmount");

            m_HMDIsTracked = serializedObject.FindProperty("m_HMDIsTracked");
            m_HMDTrackingState = serializedObject.FindProperty("m_HMDTrackingState");
            m_LeftControllerIsTracked = serializedObject.FindProperty("m_LeftControllerIsTracked");
            m_LeftControllerTrackingState = serializedObject.FindProperty("m_LeftControllerTrackingState");
            m_RightControllerIsTracked = serializedObject.FindProperty("m_RightControllerIsTracked");
            m_RightControllerTrackingState = serializedObject.FindProperty("m_RightControllerTrackingState");
            m_LeftHandIsTracked = serializedObject.FindProperty("m_LeftHandIsTracked");
            m_RightHandIsTracked = serializedObject.FindProperty("m_RightHandIsTracked");

            m_SimulatorInputsExpanded = SessionState.GetBool(k_SimulatorInputsExpandedKey, false);
            m_ControllerInputsExpanded = SessionState.GetBool(k_ControllerInputsExpandedKey, false);
            m_SimulatorSettingsExpanded = SessionState.GetBool(k_SimulatorSettingsExpandedKey, false);
            m_SensitivityExpanded = SessionState.GetBool(k_SensitivityExpandedKey, false);
            m_AnalogConfigurationExpanded = SessionState.GetBool(k_AnalogConfigurationExpandedKey, false);
            m_TrackingStateExpanded = SessionState.GetBool(k_TrackingStateExpandedKey, false);
        }

        /// <inheritdoc />
        protected override void DrawInspector()
        {
            DrawScript();
            DrawInputs();
            EditorGUILayout.Space();
            DrawGeneralSimulatorSettings();
            EditorGUILayout.Space();
            DrawSensitivitySettings();
            EditorGUILayout.Space();
            DrawAnalogConfigurationSettings();
            EditorGUILayout.Space();
            DrawTrackingStateSettings();
            EditorGUILayout.Space();
            DrawDerivedProperties();
        }

        /// <summary>
        /// Draw the property fields related to input readers.
        /// </summary>
        protected virtual void DrawInputs()
        {
            EditorGUILayout.LabelField("Inputs", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                DrawSimulatorInputs();
                DrawControllerInputs();
                EditorGUILayout.PropertyField(m_QuickActionControllerInputModes);
            }
        }

        /// <summary>
        /// Draw the property fields related to simulator input readers.
        /// </summary>
        protected virtual void DrawSimulatorInputs()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_SimulatorInputsExpanded = EditorGUILayout.Foldout(m_SimulatorInputsExpanded, EditorGUIUtility.TrTempContent("Simulator Inputs"), true);
                if (check.changed)
                    SessionState.SetBool(k_SimulatorInputsExpandedKey, m_SimulatorInputsExpanded);
            }

            if (!m_SimulatorInputsExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_TranslateXInput);
                EditorGUILayout.PropertyField(m_TranslateYInput);
                EditorGUILayout.PropertyField(m_TranslateZInput);
                EditorGUILayout.PropertyField(m_ToggleManipulateLeftInput);
                EditorGUILayout.PropertyField(m_ToggleManipulateRightInput);
                EditorGUILayout.PropertyField(m_ToggleManipulateHeadInput);
                EditorGUILayout.PropertyField(m_CycleDevicesInput);
                EditorGUILayout.PropertyField(m_KeyboardRotationDeltaInput);
                EditorGUILayout.PropertyField(m_ToggleMouseInput);
                EditorGUILayout.PropertyField(m_MouseRotationDeltaInput);
                EditorGUILayout.PropertyField(m_MouseScrollInput);
                EditorGUILayout.PropertyField(m_XConstraintInput);
                EditorGUILayout.PropertyField(m_YConstraintInput);
                EditorGUILayout.PropertyField(m_ZConstraintInput);
                EditorGUILayout.PropertyField(m_ResetInput);
                EditorGUILayout.PropertyField(m_CycleQuickActionInput);
                EditorGUILayout.PropertyField(m_TogglePerformQuickActionInput);
                EditorGUILayout.PropertyField(m_LeftDeviceActionsInput);
                EditorGUILayout.PropertyField(m_TogglePrimary2DAxisTargetInput);
                EditorGUILayout.PropertyField(m_ToggleSecondary2DAxisTargetInput);
            }
        }

        /// <summary>
        /// Draw the property fields related to controller input readers.
        /// </summary>
        protected virtual void DrawControllerInputs()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_ControllerInputsExpanded = EditorGUILayout.Foldout(m_ControllerInputsExpanded, EditorGUIUtility.TrTempContent("Controller Inputs"), true);
                if (check.changed)
                    SessionState.SetBool(k_ControllerInputsExpandedKey, m_ControllerInputsExpanded);
            }

            if (!m_ControllerInputsExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_Axis2DInput);
                EditorGUILayout.PropertyField(m_GripInput);
                EditorGUILayout.PropertyField(m_TriggerInput);
                EditorGUILayout.PropertyField(m_PrimaryButtonInput);
                EditorGUILayout.PropertyField(m_SecondaryButtonInput);
                EditorGUILayout.PropertyField(m_MenuInput);
                EditorGUILayout.PropertyField(m_Primary2DAxisClickInput);
                EditorGUILayout.PropertyField(m_Secondary2DAxisClickInput);
                EditorGUILayout.PropertyField(m_Primary2DAxisTouchInput);
                EditorGUILayout.PropertyField(m_Secondary2DAxisTouchInput);
                EditorGUILayout.PropertyField(m_PrimaryTouchInput);
                EditorGUILayout.PropertyField(m_SecondaryTouchInput);
            }
        }

        /// <summary>
        /// Draw the property fields related to general simulator settings.
        /// </summary>
        protected virtual void DrawGeneralSimulatorSettings()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_SimulatorSettingsExpanded = EditorGUILayout.Foldout(m_SimulatorSettingsExpanded, EditorGUIUtility.TrTempContent("Simulator Settings"), true, Contents.foldoutTitleStyle);
                if (check.changed)
                    SessionState.SetBool(k_SimulatorSettingsExpandedKey, m_SimulatorSettingsExpanded);
            }

            if (!m_SimulatorSettingsExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_CameraTransform);
                EditorGUILayout.PropertyField(m_DeviceLifecycleManager);
                EditorGUILayout.PropertyField(m_HandExpressionManager);
                EditorGUILayout.PropertyField(m_TranslateSpace);
                EditorGUILayout.PropertyField(m_InteractionSimulatorUI);
            }
        }

        /// <summary>
        /// Draw the property fields related to sensitivity settings.
        /// </summary>
        protected virtual void DrawSensitivitySettings()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_SensitivityExpanded = EditorGUILayout.Foldout(m_SensitivityExpanded, EditorGUIUtility.TrTempContent("Sensitivity"), true, Contents.foldoutTitleStyle);
                if (check.changed)
                    SessionState.SetBool(k_SensitivityExpandedKey, m_SensitivityExpanded);
            }

            if (!m_SensitivityExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_TranslateXSpeed);
                EditorGUILayout.PropertyField(m_TranslateYSpeed);
                EditorGUILayout.PropertyField(m_TranslateZSpeed);
                EditorGUILayout.PropertyField(m_BodyTranslateMultiplier);
                EditorGUILayout.PropertyField(m_RotateXSensitivity);
                EditorGUILayout.PropertyField(m_RotateYSensitivity);
                EditorGUILayout.PropertyField(m_MouseScrollRotateSensitivity);
                EditorGUILayout.PropertyField(m_RotateYInvert);
            }
        }

        /// <summary>
        /// Draw the property fields related to analog configuration.
        /// </summary>
        protected virtual void DrawAnalogConfigurationSettings()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_AnalogConfigurationExpanded = EditorGUILayout.Foldout(m_AnalogConfigurationExpanded, EditorGUIUtility.TrTempContent("Analog Configuration"), true, Contents.foldoutTitleStyle);
                if (check.changed)
                    SessionState.SetBool(k_AnalogConfigurationExpandedKey, m_AnalogConfigurationExpanded);
            }

            if (!m_AnalogConfigurationExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_GripAmount);
                EditorGUILayout.PropertyField(m_TriggerAmount);
            }
        }

        /// <summary>
        /// Draw the property fields related to tracking state.
        /// </summary>
        protected virtual void DrawTrackingStateSettings()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_TrackingStateExpanded = EditorGUILayout.Foldout(m_TrackingStateExpanded, EditorGUIUtility.TrTempContent("Tracking State"), true, Contents.foldoutTitleStyle);
                if (check.changed)
                    SessionState.SetBool(k_TrackingStateExpandedKey, m_TrackingStateExpanded);
            }

            if (!m_TrackingStateExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HMDIsTracked);
                EditorGUILayout.PropertyField(m_HMDTrackingState);
                EditorGUILayout.PropertyField(m_LeftControllerIsTracked);
                EditorGUILayout.PropertyField(m_LeftControllerTrackingState);
                EditorGUILayout.PropertyField(m_RightControllerIsTracked);
                EditorGUILayout.PropertyField(m_RightControllerTrackingState);
                EditorGUILayout.PropertyField(m_LeftHandIsTracked);
                EditorGUILayout.PropertyField(m_RightHandIsTracked);
            }
        }
    }
}
