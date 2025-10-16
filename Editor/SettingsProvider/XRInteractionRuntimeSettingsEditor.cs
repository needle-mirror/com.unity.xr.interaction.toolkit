using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Editor inspector for <see cref="XRInteractionRuntimeSettings"/>.
    /// </summary>
    [CustomEditor(typeof(XRInteractionRuntimeSettings))]
    class XRInteractionRuntimeSettingsEditor : Editor
    {
        const float k_LabelsWidth = 270f;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionRuntimeSettings.managerCreationMode"/>.</summary>
        SerializedProperty m_ManagerCreationMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionRuntimeSettings.managerRegistrationMode"/>.</summary>
        SerializedProperty m_ManagerRegistrationMode;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.managerCreationMode"/>.</summary>
            public static readonly GUIContent managerCreationMode =
                EditorGUIUtility.TrTextContent("Manager Creation Mode",
                    "Determines whether the manager component is automatically created.\n\n" +
                    "\u2043 'Create Automatically' (Default) will create the manager component automatically as needed.\n" +
                    "\u2043 'Manual' will not automatically create the manager component. The component must be manually added to the scene or manually instantiated at runtime for interaction to function.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.managerRegistrationMode"/>.</summary>
            public static readonly GUIContent managerRegistrationMode =
                EditorGUIUtility.TrTextContent("Manager Registration Mode",
                    "Determines whether interaction components are automatically registered with a manager component when the manager reference is not set or the manager is destroyed.\n\n" +
                    "\u2043 'Find Automatically' (Default) will assign the manager component reference automatically at runtime and register with the manager component. Any registered interaction components to a manager being destroyed will automatically transfer to another manager.\n" +
                    "\u2043 'Manual' will not automatically find and register with the manager component when the manager reference is not set. The interaction manager reference must be set in the appropriate component or registered through scripting.");
        }

        void OnEnable()
        {
            m_ManagerCreationMode = serializedObject.FindProperty(nameof(m_ManagerCreationMode));
            m_ManagerRegistrationMode = serializedObject.FindProperty(nameof(m_ManagerRegistrationMode));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = k_LabelsWidth;
                EditorGUILayout.PropertyField(m_ManagerCreationMode, Contents.managerCreationMode);
                EditorGUILayout.PropertyField(m_ManagerRegistrationMode, Contents.managerRegistrationMode);
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
