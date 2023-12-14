using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Editor inspector for <see cref="XRInteractionEditorSettings"/>.
    /// </summary>
    [CustomEditor(typeof(XRInteractionEditorSettings))]
    class XRInteractionEditorSettingsEditor : Editor
    {
        const float k_LabelsWidth = 270f;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionEditorSettings.inputReaderPropertyDrawerMode"/>.</summary>
        SerializedProperty m_InputReaderPropertyDrawerMode;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionEditorSettings.inputReaderPropertyDrawerMode"/>.</summary>
            public static readonly GUIContent inputReaderPropertyDrawerMode =
                EditorGUIUtility.TrTextContent("Input Reader Property Drawer Mode",
                    "Determines how the Inspector window displays input reader fields.\n\n" +
                    "'Compact' displays the property in a compact format, using a minimal number of lines.\n" +
                    "'Multiline Effective' displays the effective input source underlying the property, using multiple lines.\n" +
                    "'Multiline All' displays all the input sources underlying the property.");
        }

        void OnEnable()
        {
            m_InputReaderPropertyDrawerMode = serializedObject.FindProperty(nameof(m_InputReaderPropertyDrawerMode));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = k_LabelsWidth;
                EditorGUILayout.PropertyField(m_InputReaderPropertyDrawerMode, Contents.inputReaderPropertyDrawerMode);
                EditorGUIUtility.labelWidth = labelWidth;

                if (check.changed)
                {
                    Repaint();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
