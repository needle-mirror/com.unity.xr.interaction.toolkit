using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

namespace UnityEditor.XR.Interaction.Toolkit.Feedback
{
    /// <summary>
    /// Custom editor for a <see cref="SimpleAudioFeedback"/>.
    /// </summary>
    [CustomEditor(typeof(SimpleAudioFeedback), true), CanEditMultipleObjects]
    public class SimpleAudioFeedbackEditor : BaseInteractionEditor
    {
        protected SerializedProperty m_InteractorSourceObject;
        protected SerializedProperty m_AudioSource;

        protected SerializedProperty m_PlaySelectEntered;
        protected SerializedProperty m_SelectEnteredClip;
        protected SerializedProperty m_PlaySelectExited;
        protected SerializedProperty m_SelectExitedClip;
        protected SerializedProperty m_PlaySelectCanceled;
        protected SerializedProperty m_SelectCanceledClip;

        protected SerializedProperty m_PlayHoverEntered;
        protected SerializedProperty m_HoverEnteredClip;
        protected SerializedProperty m_PlayHoverExited;
        protected SerializedProperty m_HoverExitedClip;
        protected SerializedProperty m_PlayHoverCanceled;
        protected SerializedProperty m_HoverCanceledClip;

        protected SerializedProperty m_AllowHoverAudioWhileSelecting;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent interactorSourceObject = EditorGUIUtility.TrTextContent("Interactor Source", "The interactor component to listen to for its interaction events.");
            public static readonly GUIContent audioSource = EditorGUIUtility.TrTextContent("Audio Source", "The Audio Source component to use to play audio clips.");

            public static readonly GUIContent playSelectEntered = EditorGUIUtility.TrTextContent("Play Select Entered", "Whether to play a sound when the interactor starts selecting an interactable.");
            public static readonly GUIContent selectEnteredClip = EditorGUIUtility.TrTextContent("Select Entered Clip", "The audio clip to play when the interactor starts selecting an interactable.");
            public static readonly GUIContent playSelectExited = EditorGUIUtility.TrTextContent("Play Select Exited", "Whether to play a sound when the interactor stops selecting an interactable without being canceled.");
            public static readonly GUIContent selectExitedClip = EditorGUIUtility.TrTextContent("Select Exited Clip", "The audio clip to play when the interactor stops selecting an interactable without being canceled.");
            public static readonly GUIContent playSelectCanceled = EditorGUIUtility.TrTextContent("Play Select Canceled", "Whether to play a sound when the interactor stops selecting an interactable due to being canceled.");
            public static readonly GUIContent selectCanceledClip = EditorGUIUtility.TrTextContent("Select Canceled Clip", "The audio clip to play when the interactor stops selecting an interactable due to being canceled.");

            public static readonly GUIContent playHoverEntered = EditorGUIUtility.TrTextContent("Play Hover Entered", "Whether to play a sound when the interactor starts hovering over an interactable.");
            public static readonly GUIContent hoverEnteredClip = EditorGUIUtility.TrTextContent("Hover Entered Clip", "The audio clip to play when the interactor starts hovering over an interactable.");
            public static readonly GUIContent playHoverExited = EditorGUIUtility.TrTextContent("Play Hover Exited", "Whether to play a sound when the interactor stops hovering over an interactable without being canceled.");
            public static readonly GUIContent hoverExitedClip = EditorGUIUtility.TrTextContent("Hover Exited Clip", "The audio clip to play when the interactor stops hovering over an interactable without being canceled.");
            public static readonly GUIContent playHoverCanceled = EditorGUIUtility.TrTextContent("Play Hover Canceled", "Whether to play a sound when the interactor stops hovering over an interactable due to being canceled.");
            public static readonly GUIContent hoverCanceledClip = EditorGUIUtility.TrTextContent("Hover Canceled Clip", "The audio clip to play when the interactor stops hovering over an interactable due to being canceled.");

            public static readonly GUIContent allowHoverAudioWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Audio While Selecting", "Whether to allow hover audio to play while the interactor is selecting an interactable.");

            public static readonly GUIContent selectHeader = EditorGUIUtility.TrTextContent("Select");
            public static readonly GUIContent hoverHeader = EditorGUIUtility.TrTextContent("Hover");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_InteractorSourceObject = serializedObject.FindProperty("m_InteractorSourceObject");
            m_AudioSource = serializedObject.FindProperty("m_AudioSource");

            m_PlaySelectEntered = serializedObject.FindProperty("m_PlaySelectEntered");
            m_SelectEnteredClip = serializedObject.FindProperty("m_SelectEnteredClip");
            m_PlaySelectExited = serializedObject.FindProperty("m_PlaySelectExited");
            m_SelectExitedClip = serializedObject.FindProperty("m_SelectExitedClip");
            m_PlaySelectCanceled = serializedObject.FindProperty("m_PlaySelectCanceled");
            m_SelectCanceledClip = serializedObject.FindProperty("m_SelectCanceledClip");

            m_PlayHoverEntered = serializedObject.FindProperty("m_PlayHoverEntered");
            m_HoverEnteredClip = serializedObject.FindProperty("m_HoverEnteredClip");
            m_PlayHoverExited = serializedObject.FindProperty("m_PlayHoverExited");
            m_HoverExitedClip = serializedObject.FindProperty("m_HoverExitedClip");
            m_PlayHoverCanceled = serializedObject.FindProperty("m_PlayHoverCanceled");
            m_HoverCanceledClip = serializedObject.FindProperty("m_HoverCanceledClip");

            m_AllowHoverAudioWhileSelecting = serializedObject.FindProperty("m_AllowHoverAudioWhileSelecting");
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
            EditorGUILayout.PropertyField(m_AudioSource, Contents.audioSource);

            // Select
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.selectHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PlaySelectEntered, Contents.playSelectEntered);
            using (new EditorGUI.DisabledScope(!m_PlaySelectEntered.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_SelectEnteredClip, Contents.selectEnteredClip);
            }

            EditorGUILayout.PropertyField(m_PlaySelectExited, Contents.playSelectExited);
            using (new EditorGUI.DisabledScope(!m_PlaySelectExited.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_SelectExitedClip, Contents.selectExitedClip);
            }

            EditorGUILayout.PropertyField(m_PlaySelectCanceled, Contents.playSelectCanceled);
            using (new EditorGUI.DisabledScope(!m_PlaySelectCanceled.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_SelectCanceledClip, Contents.selectCanceledClip);
            }

            // Hover
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.hoverHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PlayHoverEntered, Contents.playHoverEntered);
            using (new EditorGUI.DisabledScope(!m_PlayHoverEntered.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HoverEnteredClip, Contents.hoverEnteredClip);
            }

            EditorGUILayout.PropertyField(m_PlayHoverExited, Contents.playHoverExited);
            using (new EditorGUI.DisabledScope(!m_PlayHoverExited.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HoverExitedClip, Contents.hoverExitedClip);
            }

            EditorGUILayout.PropertyField(m_PlayHoverCanceled, Contents.playHoverCanceled);
            using (new EditorGUI.DisabledScope(!m_PlayHoverCanceled.boolValue))
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HoverCanceledClip, Contents.hoverCanceledClip);
            }

            EditorGUILayout.PropertyField(m_AllowHoverAudioWhileSelecting, Contents.allowHoverAudioWhileSelecting);
        }
    }
}
