using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

namespace UnityEditor.XR.Interaction.Toolkit.Feedback
{
    /// <summary>
    /// Custom editor for a <see cref="SimpleHapticFeedback"/>.
    /// </summary>
    [CustomEditor(typeof(SimpleHapticFeedback), true), CanEditMultipleObjects]
    public class SimpleHapticFeedbackEditor : BaseInteractionEditor
    {
        protected SerializedProperty m_InteractorSourceObject;
        protected SerializedProperty m_HapticImpulsePlayer;

        protected SerializedProperty m_PlaySelectEntered;
        protected SerializedProperty m_SelectEnteredData;
        protected SerializedProperty m_SelectEnteredDataAmplitude;
        protected SerializedProperty m_SelectEnteredDataDuration;
        protected SerializedProperty m_SelectEnteredDataFrequency;
        protected SerializedProperty m_PlaySelectExited;
        protected SerializedProperty m_SelectExitedData;
        protected SerializedProperty m_SelectExitedDataAmplitude;
        protected SerializedProperty m_SelectExitedDataDuration;
        protected SerializedProperty m_SelectExitedDataFrequency;
        protected SerializedProperty m_PlaySelectCanceled;
        protected SerializedProperty m_SelectCanceledData;
        protected SerializedProperty m_SelectCanceledDataAmplitude;
        protected SerializedProperty m_SelectCanceledDataDuration;
        protected SerializedProperty m_SelectCanceledDataFrequency;

        protected SerializedProperty m_PlayHoverEntered;
        protected SerializedProperty m_HoverEnteredData;
        protected SerializedProperty m_HoverEnteredDataAmplitude;
        protected SerializedProperty m_HoverEnteredDataDuration;
        protected SerializedProperty m_HoverEnteredDataFrequency;
        protected SerializedProperty m_PlayHoverExited;
        protected SerializedProperty m_HoverExitedData;
        protected SerializedProperty m_HoverExitedDataAmplitude;
        protected SerializedProperty m_HoverExitedDataDuration;
        protected SerializedProperty m_HoverExitedDataFrequency;
        protected SerializedProperty m_PlayHoverCanceled;
        protected SerializedProperty m_HoverCanceledData;
        protected SerializedProperty m_HoverCanceledDataAmplitude;
        protected SerializedProperty m_HoverCanceledDataDuration;
        protected SerializedProperty m_HoverCanceledDataFrequency;

        protected SerializedProperty m_AllowHoverHapticsWhileSelecting;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent interactorSourceObject = EditorGUIUtility.TrTextContent("Interactor Source", "The interactor component to listen to for its interaction events.");
            public static readonly GUIContent hapticImpulsePlayer = EditorGUIUtility.TrTextContent("Haptic Impulse Player", "The Haptic Impulse Player component to use to play haptic impulses.");

            public static readonly GUIContent playSelectEntered = EditorGUIUtility.TrTextContent("Play Select Entered", "Whether to play a haptic impulse when the interactor starts selecting an interactable.");
            public static readonly GUIContent selectEnteredData = EditorGUIUtility.TrTextContent("Select Entered Data", "The haptic impulse to play when the interactor starts selecting an interactable.");
            public static readonly GUIContent playSelectExited = EditorGUIUtility.TrTextContent("Play Select Exited", "Whether to play a haptic impulse when the interactor stops selecting an interactable without being canceled.");
            public static readonly GUIContent selectExitedData = EditorGUIUtility.TrTextContent("Select Exited Data", "The haptic impulse to play when the interactor stops selecting an interactable without being canceled.");
            public static readonly GUIContent playSelectCanceled = EditorGUIUtility.TrTextContent("Play Select Canceled", "Whether to play a haptic impulse when the interactor stops selecting an interactable due to being canceled.");
            public static readonly GUIContent selectCanceledData = EditorGUIUtility.TrTextContent("Select Canceled Data", "The haptic impulse to play when the interactor stops selecting an interactable due to being canceled.");

            public static readonly GUIContent playHoverEntered = EditorGUIUtility.TrTextContent("Play Hover Entered", "Whether to play a haptic impulse when the interactor starts hovering over an interactable.");
            public static readonly GUIContent hoverEnteredData = EditorGUIUtility.TrTextContent("Hover Entered Data", "The haptic impulse to play when the interactor starts hovering over an interactable.");
            public static readonly GUIContent playHoverExited = EditorGUIUtility.TrTextContent("Play Hover Exited", "Whether to play a haptic impulse when the interactor stops hovering over an interactable without being canceled.");
            public static readonly GUIContent hoverExitedData = EditorGUIUtility.TrTextContent("Hover Exited Data", "The haptic impulse to play when the interactor stops hovering over an interactable without being canceled.");
            public static readonly GUIContent playHoverCanceled = EditorGUIUtility.TrTextContent("Play Hover Canceled", "Whether to play a haptic impulse when the interactor stops hovering over an interactable due to being canceled.");
            public static readonly GUIContent hoverCanceledData = EditorGUIUtility.TrTextContent("Hover Canceled Data", "The haptic impulse to play when the interactor stops hovering over an interactable due to being canceled.");

            public static readonly GUIContent allowHoverHapticsWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Haptics While Selecting", "Whether to allow hover haptics to play while the interactor is selecting an interactable.");

            public static readonly GUIContent amplitude = EditorGUIUtility.TrTextContent("Amplitude", "The desired motor amplitude.");
            public static readonly GUIContent duration = EditorGUIUtility.TrTextContent("Duration", "The desired duration of the impulse in seconds.");
            public static readonly GUIContent frequency = EditorGUIUtility.TrTextContent("Frequency", "The desired frequency of the impulse in Hz. The default value of 0 means to use the default frequency of the device.");

            public static readonly GUIContent selectHeader = EditorGUIUtility.TrTextContent("Select");
            public static readonly GUIContent hoverHeader = EditorGUIUtility.TrTextContent("Hover");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_InteractorSourceObject = serializedObject.FindProperty("m_InteractorSourceObject");
            m_HapticImpulsePlayer = serializedObject.FindProperty("m_HapticImpulsePlayer");

            m_PlaySelectEntered = serializedObject.FindProperty("m_PlaySelectEntered");
            m_SelectEnteredData = serializedObject.FindProperty("m_SelectEnteredData");
            m_SelectEnteredDataAmplitude = m_SelectEnteredData.FindPropertyRelative("m_Amplitude");
            m_SelectEnteredDataDuration = m_SelectEnteredData.FindPropertyRelative("m_Duration");
            m_SelectEnteredDataFrequency = m_SelectEnteredData.FindPropertyRelative("m_Frequency");
            m_PlaySelectExited = serializedObject.FindProperty("m_PlaySelectExited");
            m_SelectExitedData = serializedObject.FindProperty("m_SelectExitedData");
            m_SelectExitedDataAmplitude = m_SelectExitedData.FindPropertyRelative("m_Amplitude");
            m_SelectExitedDataDuration = m_SelectExitedData.FindPropertyRelative("m_Duration");
            m_SelectExitedDataFrequency = m_SelectExitedData.FindPropertyRelative("m_Frequency");
            m_PlaySelectCanceled = serializedObject.FindProperty("m_PlaySelectCanceled");
            m_SelectCanceledData = serializedObject.FindProperty("m_SelectCanceledData");
            m_SelectCanceledDataAmplitude = m_SelectCanceledData.FindPropertyRelative("m_Amplitude");
            m_SelectCanceledDataDuration = m_SelectCanceledData.FindPropertyRelative("m_Duration");
            m_SelectCanceledDataFrequency = m_SelectCanceledData.FindPropertyRelative("m_Frequency");

            m_PlayHoverEntered = serializedObject.FindProperty("m_PlayHoverEntered");
            m_HoverEnteredData = serializedObject.FindProperty("m_HoverEnteredData");
            m_HoverEnteredDataAmplitude = m_HoverEnteredData.FindPropertyRelative("m_Amplitude");
            m_HoverEnteredDataDuration = m_HoverEnteredData.FindPropertyRelative("m_Duration");
            m_HoverEnteredDataFrequency = m_HoverEnteredData.FindPropertyRelative("m_Frequency");
            m_PlayHoverExited = serializedObject.FindProperty("m_PlayHoverExited");
            m_HoverExitedData = serializedObject.FindProperty("m_HoverExitedData");
            m_HoverExitedDataAmplitude = m_HoverExitedData.FindPropertyRelative("m_Amplitude");
            m_HoverExitedDataDuration = m_HoverExitedData.FindPropertyRelative("m_Duration");
            m_HoverExitedDataFrequency = m_HoverExitedData.FindPropertyRelative("m_Frequency");
            m_PlayHoverCanceled = serializedObject.FindProperty("m_PlayHoverCanceled");
            m_HoverCanceledData = serializedObject.FindProperty("m_HoverCanceledData");
            m_HoverCanceledDataAmplitude = m_HoverCanceledData.FindPropertyRelative("m_Amplitude");
            m_HoverCanceledDataDuration = m_HoverCanceledData.FindPropertyRelative("m_Duration");
            m_HoverCanceledDataFrequency = m_HoverCanceledData.FindPropertyRelative("m_Frequency");

            m_AllowHoverHapticsWhileSelecting = serializedObject.FindProperty("m_AllowHoverHapticsWhileSelecting");
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        protected override void DrawInspector()
        {
            DrawBeforeProperties();
            DrawProperties();
            DrawDerivedProperties();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the section of the custom inspector before <see cref="DrawProperties"/>.
        /// By default, this draws the read-only Script property.
        /// </summary>
        protected virtual void DrawBeforeProperties()
        {
            DrawScript();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the property fields. Override this method to customize the
        /// properties shown in the Inspector. This is typically the method overridden
        /// when a derived behavior adds additional serialized properties
        /// that should be displayed in the Inspector.
        /// </summary>
        protected virtual void DrawProperties()
        {
            EditorGUILayout.PropertyField(m_InteractorSourceObject, Contents.interactorSourceObject);
            EditorGUILayout.PropertyField(m_HapticImpulsePlayer, Contents.hapticImpulsePlayer);

            // Select
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.selectHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PlaySelectEntered, Contents.playSelectEntered);
            DrawHapticImpulseData(m_PlaySelectEntered, m_SelectEnteredDataAmplitude, m_SelectEnteredDataDuration, m_SelectEnteredDataFrequency);

            EditorGUILayout.PropertyField(m_PlaySelectExited, Contents.playSelectExited);
            DrawHapticImpulseData(m_PlaySelectExited, m_SelectExitedDataAmplitude, m_SelectExitedDataDuration, m_SelectExitedDataFrequency);

            EditorGUILayout.PropertyField(m_PlaySelectCanceled, Contents.playSelectCanceled);
            DrawHapticImpulseData(m_PlaySelectCanceled, m_SelectCanceledDataAmplitude, m_SelectCanceledDataDuration, m_SelectCanceledDataFrequency);

            // Hover
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.hoverHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PlayHoverEntered, Contents.playHoverEntered);
            DrawHapticImpulseData(m_PlayHoverEntered, m_HoverEnteredDataAmplitude, m_HoverEnteredDataDuration, m_HoverEnteredDataFrequency);

            EditorGUILayout.PropertyField(m_PlayHoverExited, Contents.playHoverExited);
            DrawHapticImpulseData(m_PlayHoverExited, m_HoverExitedDataAmplitude, m_HoverExitedDataDuration, m_HoverExitedDataFrequency);

            EditorGUILayout.PropertyField(m_PlayHoverCanceled, Contents.playHoverCanceled);
            DrawHapticImpulseData(m_PlayHoverCanceled, m_HoverCanceledDataAmplitude, m_HoverCanceledDataDuration, m_HoverCanceledDataFrequency);

            EditorGUILayout.PropertyField(m_AllowHoverHapticsWhileSelecting, Contents.allowHoverHapticsWhileSelecting);
        }

        static void DrawHapticImpulseData(SerializedProperty boolProperty, SerializedProperty amplitude, SerializedProperty duration, SerializedProperty frequency)
        {
            using (new EditorGUI.DisabledScope(!boolProperty.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(amplitude, Contents.amplitude);
                EditorGUILayout.PropertyField(duration, Contents.duration);
                EditorGUILayout.PropertyField(frequency, Contents.frequency);
            }
        }
    }
}
