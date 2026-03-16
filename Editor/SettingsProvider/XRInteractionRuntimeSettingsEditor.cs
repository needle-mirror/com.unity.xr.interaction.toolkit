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

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionRuntimeSettings.interactionManagerSingletonMode"/>.</summary>
        SerializedProperty m_InteractionManagerSingletonMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionRuntimeSettings.interactionManagerRegistrationMode"/>.</summary>
        SerializedProperty m_InteractionManagerRegistrationMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRInteractionRuntimeSettings.uiModuleRegistrationMode"/>.</summary>
        SerializedProperty m_UIModuleRegistrationMode;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.managerCreationMode"/>.</summary>
            public static readonly GUIContent managerCreationMode =
                EditorGUIUtility.TrTextContent("Manager Creation Mode",
                    "Determines whether the manager component is automatically created. Applies to XR Interaction Manager and XR UI Input Module.\n\n" +
                    "\u2043 'Create Automatically' (Default) will create the manager component automatically as needed.\n" +
                    "\u2043 'Manual' will not automatically create the manager component. The component must be manually added to the scene or manually instantiated at runtime for interaction to function.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.interactionManagerSingletonMode"/>.</summary>
            public static readonly GUIContent interactionManagerSingletonMode =
                EditorGUIUtility.TrTextContent("Interaction Manager Singleton Mode",
                    "Determines whether multiple instances of the XR Interaction Manager component are allowed to exist or will instead be automatically destroyed to enforce a single component instance.\n\n" +
                    "\u2043 'Allow Multiple' (Default) will allow multiple instances of the manager component.\n" +
                    "\u2043 'Enforce Single' will enforce that only a single manager component can be active and enabled at one time. You can use this mode to help prevent a potentially undesirable situation where interaction components cannot interact with each other due to being unintentionally registered to different manager components.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.interactionManagerRegistrationMode"/>.</summary>
            public static readonly GUIContent interactionManagerRegistrationMode =
                EditorGUIUtility.TrTextContent("Interaction Manager Registration Mode",
                    "Determines whether interaction components are automatically registered with the XR Interaction Manager component when the manager reference is not set or the manager is destroyed.\n\n" +
                    "\u2043 'Find Automatically' (Default) will assign the manager component reference automatically at runtime and register with the manager component. Any registered interaction components to a manager being destroyed will automatically transfer to another manager.\n" +
                    "\u2043 'Manual' will not automatically find and register with the manager component when the manager reference is not set. The interaction manager reference must be set in the appropriate component or registered through scripting.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRInteractionRuntimeSettings.interactionManagerRegistrationMode"/>.</summary>
            public static readonly GUIContent uiModuleRegistrationMode =
                EditorGUIUtility.TrTextContent("XR UI Input Module Registration Mode",
                    "Determines whether interactor components are automatically registered with the XR UI Input Module component.\n\n" +
                    "\u2043 'Find Automatically' (Default) will find the component reference automatically at runtime and register with it. Any registered interactor components to a module being destroyed will automatically transfer to another module.\n" +
                    "\u2043 'Manual' will not automatically find and register with the module component. The interactor must be registered through scripting.");
        }

        void OnEnable()
        {
            m_ManagerCreationMode = serializedObject.FindProperty(nameof(m_ManagerCreationMode));
            m_InteractionManagerSingletonMode = serializedObject.FindProperty(nameof(m_InteractionManagerSingletonMode));
            m_InteractionManagerRegistrationMode = serializedObject.FindProperty(nameof(m_InteractionManagerRegistrationMode));
            m_UIModuleRegistrationMode = serializedObject.FindProperty(nameof(m_UIModuleRegistrationMode));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = k_LabelsWidth;
                EditorGUILayout.PropertyField(m_ManagerCreationMode, Contents.managerCreationMode);
                EditorGUILayout.PropertyField(m_InteractionManagerSingletonMode, Contents.interactionManagerSingletonMode);
                EditorGUILayout.PropertyField(m_InteractionManagerRegistrationMode, Contents.interactionManagerRegistrationMode);
                EditorGUILayout.PropertyField(m_UIModuleRegistrationMode, Contents.uiModuleRegistrationMode);
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
