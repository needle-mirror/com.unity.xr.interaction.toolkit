using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="XRBaseInputInteractor"/>.
    /// </summary>
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    [CustomEditor(typeof(XRBaseInputInteractor), true), CanEditMultipleObjects]
    public partial class XRBaseInputInteractorEditor : XRBaseInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.selectInput"/>.</summary>
        protected SerializedProperty m_SelectInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.activateInput"/>.</summary>
        protected SerializedProperty m_ActivateInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.selectActionTrigger"/>.</summary>
        protected SerializedProperty m_SelectActionTrigger;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.allowHoveredActivate"/>.</summary>
        protected SerializedProperty m_AllowHoveredActivate;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.targetPriorityMode"/>.</summary>
        protected SerializedProperty m_TargetPriorityMode;

        /// <summary>
        /// Determines if deprecated properties should be shown.
        /// </summary>
        protected virtual bool showDeprecatedProperties => true;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static partial class BaseInputContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.selectInput"/>.</summary>
            public static readonly GUIContent selectInput = EditorGUIUtility.TrTextContent("Select Input", "Input to use for selecting an interactable.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.activateInput"/>.</summary>
            public static readonly GUIContent activateInput = EditorGUIUtility.TrTextContent("Activate Input", "Input to use for activating an interactable. This can be used to trigger a secondary action on an interactable object, such as pulling a trigger on a ball launcher after picking it up.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.selectActionTrigger"/>.</summary>
            public static readonly GUIContent selectActionTrigger = EditorGUIUtility.TrTextContent("Select Action Trigger", "Choose how the select state is triggered from input, either by: holding the button, pressed only while hovered, toggle select on/off when the button is pressed, or sticky toggle where it toggles off upon the second release.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoveredActivate"/>.</summary>
            public static readonly GUIContent allowHoveredActivate = EditorGUIUtility.TrTextContent("Allow Hovered Activate", "Send activate and deactivate events to interactables that this interactor is hovered over but not selected when there is no current selection.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.targetPriorityMode"/>.</summary>
            public static readonly GUIContent targetPriorityMode = EditorGUIUtility.TrTextContent("Target Priority Mode", "Specifies how many Interactables should be monitored in the Targets For Selection property, useful for custom feedback. The options are in order of best performance.");

            /// <summary><see cref="GUIContent"/> for the Input Configuration foldout.</summary>
            public static readonly GUIContent inputConfigurationFoldout = EditorGUIUtility.TrTextContent("Input Configuration", "Input configuration for this interactor.");

            /// <summary>The help box message when the <see cref="XRBaseInteractor.startingSelectedInteractable"/> will be instantly deselected due to the value of <see cref="XRBaseInputInteractor.selectActionTrigger"/>.</summary>
            public static readonly string selectActionTriggerWarning = "A Starting Selected Interactable will be instantly deselected if the Select Input is bound to a button that is not pressed while Select Action Trigger is State.";
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            m_SelectInput = serializedObject.FindProperty("m_SelectInput");
            m_ActivateInput = serializedObject.FindProperty("m_ActivateInput");
            m_SelectActionTrigger = serializedObject.FindProperty("m_SelectActionTrigger");
            m_AllowHoveredActivate = serializedObject.FindProperty("m_AllowHoveredActivate");
            m_TargetPriorityMode = serializedObject.FindProperty("m_TargetPriorityMode");

#pragma warning disable CS0618 // Type or member is obsolete
            m_InputCompatibilityMode = serializedObject.FindProperty("m_InputCompatibilityMode");
            m_HideControllerOnSelect = serializedObject.FindProperty("m_HideControllerOnSelect");

            m_PlayAudioClipOnSelectEntered = serializedObject.FindProperty("m_PlayAudioClipOnSelectEntered");
            m_AudioClipForOnSelectEntered = serializedObject.FindProperty("m_AudioClipForOnSelectEntered");
            m_PlayAudioClipOnSelectExited = serializedObject.FindProperty("m_PlayAudioClipOnSelectExited");
            m_AudioClipForOnSelectExited = serializedObject.FindProperty("m_AudioClipForOnSelectExited");
            m_PlayAudioClipOnSelectCanceled = serializedObject.FindProperty("m_PlayAudioClipOnSelectCanceled");
            m_AudioClipForOnSelectCanceled = serializedObject.FindProperty("m_AudioClipForOnSelectCanceled");
            m_PlayAudioClipOnHoverEntered = serializedObject.FindProperty("m_PlayAudioClipOnHoverEntered");
            m_AudioClipForOnHoverEntered = serializedObject.FindProperty("m_AudioClipForOnHoverEntered");
            m_PlayAudioClipOnHoverExited = serializedObject.FindProperty("m_PlayAudioClipOnHoverExited");
            m_AudioClipForOnHoverExited = serializedObject.FindProperty("m_AudioClipForOnHoverExited");
            m_PlayAudioClipOnHoverCanceled = serializedObject.FindProperty("m_PlayAudioClipOnHoverCanceled");
            m_AudioClipForOnHoverCanceled = serializedObject.FindProperty("m_AudioClipForOnHoverCanceled");
            m_AllowHoverAudioWhileSelecting = serializedObject.FindProperty("m_AllowHoverAudioWhileSelecting");

            m_PlayHapticsOnSelectEntered = serializedObject.FindProperty("m_PlayHapticsOnSelectEntered");
            m_HapticSelectEnterIntensity = serializedObject.FindProperty("m_HapticSelectEnterIntensity");
            m_HapticSelectEnterDuration = serializedObject.FindProperty("m_HapticSelectEnterDuration");
            m_PlayHapticsOnHoverEntered = serializedObject.FindProperty("m_PlayHapticsOnHoverEntered");
            m_HapticHoverEnterIntensity = serializedObject.FindProperty("m_HapticHoverEnterIntensity");
            m_HapticHoverEnterDuration = serializedObject.FindProperty("m_HapticHoverEnterDuration");
            m_PlayHapticsOnSelectExited = serializedObject.FindProperty("m_PlayHapticsOnSelectExited");
            m_HapticSelectExitIntensity = serializedObject.FindProperty("m_HapticSelectExitIntensity");
            m_HapticSelectExitDuration = serializedObject.FindProperty("m_HapticSelectExitDuration");
            m_PlayHapticsOnSelectCanceled = serializedObject.FindProperty("m_PlayHapticsOnSelectCanceled");
            m_HapticSelectCancelIntensity = serializedObject.FindProperty("m_HapticSelectCancelIntensity");
            m_HapticSelectCancelDuration = serializedObject.FindProperty("m_HapticSelectCancelDuration");
            m_PlayHapticsOnHoverExited = serializedObject.FindProperty("m_PlayHapticsOnHoverExited");
            m_HapticHoverExitIntensity = serializedObject.FindProperty("m_HapticHoverExitIntensity");
            m_HapticHoverExitDuration = serializedObject.FindProperty("m_HapticHoverExitDuration");
            m_PlayHapticsOnHoverCanceled = serializedObject.FindProperty("m_PlayHapticsOnHoverCanceled");
            m_HapticHoverCancelIntensity = serializedObject.FindProperty("m_HapticHoverCancelIntensity");
            m_HapticHoverCancelDuration = serializedObject.FindProperty("m_HapticHoverCancelDuration");
            m_AllowHoverHapticsWhileSelecting = serializedObject.FindProperty("m_AllowHoverHapticsWhileSelecting");
#pragma warning restore CS0618
        }

        /// <inheritdoc />
        protected override void DrawInspector()
        {
            base.DrawInspector();

            if (showDeprecatedProperties)
            {
                EditorGUILayout.Space();

                DrawDeprecatedProperties();
            }
        }

        /// <summary>
        /// Draw the deprecated properties.
        /// </summary>
        protected virtual void DrawDeprecatedProperties()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            DrawAudioEvents();
            DrawHapticEvents();
            DrawDeprecatedControllerConfiguration();
#pragma warning restore CS0618
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            // Not calling base method to completely override drawn properties

            DrawCoreConfiguration();
            DrawSelectActionTrigger();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, BaseContents.keepSelectedTargetValid);
            EditorGUILayout.PropertyField(m_AllowHoveredActivate, BaseInputContents.allowHoveredActivate);
            EditorGUILayout.PropertyField(m_TargetPriorityMode, BaseInputContents.targetPriorityMode);
            DrawInputConfiguration();
        }

        // ReSharper disable once RedundantOverriddenMember -- Method needed for API Updater Configuration Validation false positive
        /// <inheritdoc />
        protected override void DrawEvents()
        {
            base.DrawEvents();
        }

        /// <summary>
        /// Draw the Select Action Trigger property and display a warning box if misconfigured.
        /// </summary>
        protected virtual void DrawSelectActionTrigger()
        {
            EditorGUILayout.PropertyField(m_SelectActionTrigger, BaseInputContents.selectActionTrigger);
            if (m_StartingSelectedInteractable.objectReferenceValue != null &&
                m_SelectActionTrigger.intValue == (int)XRBaseInputInteractor.InputTriggerType.State)
            {
                EditorGUILayout.HelpBox(BaseInputContents.selectActionTriggerWarning, MessageType.Warning, true);
            }
        }

        /// <summary>
        /// Draw the Input Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawInputConfigurationNested"/>
        protected virtual void DrawInputConfiguration()
        {
            m_SelectInput.isExpanded = EditorGUILayout.Foldout(m_SelectInput.isExpanded, BaseInputContents.inputConfigurationFoldout, true);
            if (m_SelectInput.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawInputConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Input Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawInputConfiguration"/>
        protected virtual void DrawInputConfigurationNested()
        {
            EditorGUILayout.PropertyField(m_SelectInput, BaseInputContents.selectInput);
            EditorGUILayout.PropertyField(m_ActivateInput, BaseInputContents.activateInput);
        }
    }
}
