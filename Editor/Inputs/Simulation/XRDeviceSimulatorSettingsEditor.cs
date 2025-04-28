using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Custom editor for an <see cref="XRDeviceSimulatorSettings"/>.
    /// </summary>
    [CustomEditor(typeof(XRDeviceSimulatorSettings))]
    class XRDeviceSimulatorSettingsEditor : Editor
    {
        const string k_PackageName = "com.unity.xr.interaction.toolkit";
        const string k_PackageDisplayName = "XR Interaction Toolkit";
        const string k_SampleDisplayName = "XR Device Simulator";
        const string k_InteractionSimulatorSampleDisplayName = "XR Interaction Simulator";

        const string k_XRInteractionSimulatorPrefabName = "XR Interaction Simulator";
        const string k_XRDeviceSimulatorPrefabName = "XR Device Simulator";

        const float k_LabelsWidth = 270f;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDeviceSimulatorSettings.automaticallyInstantiateSimulatorPrefab"/>.</summary>
        SerializedProperty m_AutomaticallyInstantiateSimulatorPrefab;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDeviceSimulatorSettings.automaticallyInstantiateInEditorOnly"/>.</summary>
        SerializedProperty m_AutomaticallyInstantiateInEditorOnly;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDeviceSimulatorSettings.useClassic"/>.</summary>
        SerializedProperty m_UseClassic;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDeviceSimulatorSettings.simulatorPrefab"/>.</summary>
        SerializedProperty m_SimulatorPrefab;

        /// <summary>
        /// Class that holds GUI content values used by this editor.
        /// </summary>
        static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRDeviceSimulatorSettings.automaticallyInstantiateSimulatorPrefab"/>.</summary>
            public static readonly GUIContent automaticallyInstantiateSimulatorPrefab =
                EditorGUIUtility.TrTextContent("Use XR Interaction Simulator in scenes",
                    "When enabled, the XR Interaction Simulator will be automatically created on play mode in your scenes.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRDeviceSimulatorSettings.automaticallyInstantiateInEditorOnly"/>.</summary>
            public static readonly GUIContent automaticallyInstantiateInEditorOnly =
                EditorGUIUtility.TrTextContent("Instantiate In Editor Only",
                    "Enable to only automatically create the simulator prefab when running inside the Unity Editor." +
                    " Disable to allow the simulator prefab to also be created in standalone builds.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRDeviceSimulatorSettings.useClassic"/>. </summary>
            public static readonly GUIContent useClassic =
                EditorGUIUtility.TrTextContent("Use Classic XR Device Simulator", "Enable to automatically use the legacy XR Device Simulator prefab instead.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRDeviceSimulatorSettings.simulatorPrefab"/>.</summary>
            public static readonly GUIContent simulatorPrefab =
                EditorGUIUtility.TrTextContent("XR Interaction Simulator prefab",
                    "Reference to the XR Interaction Simulator prefab that will be instantiated at runtime.");
        }

        void OnEnable()
        {
            m_AutomaticallyInstantiateSimulatorPrefab = serializedObject.FindProperty("m_AutomaticallyInstantiateSimulatorPrefab");
            m_AutomaticallyInstantiateInEditorOnly = serializedObject.FindProperty("m_AutomaticallyInstantiateInEditorOnly");
            m_UseClassic = serializedObject.FindProperty("m_UseClassic");
            m_SimulatorPrefab = serializedObject.FindProperty("m_SimulatorPrefab");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = k_LabelsWidth;
                EditorGUILayout.PropertyField(m_AutomaticallyInstantiateSimulatorPrefab, Contents.automaticallyInstantiateSimulatorPrefab);
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledScope(!m_AutomaticallyInstantiateSimulatorPrefab.boolValue))
                {
                    EditorGUILayout.PropertyField(m_AutomaticallyInstantiateInEditorOnly, Contents.automaticallyInstantiateInEditorOnly);
                    EditorGUILayout.PropertyField(m_UseClassic, Contents.useClassic);
                    EditorGUILayout.PropertyField(m_SimulatorPrefab, Contents.simulatorPrefab);
                }

                EditorGUIUtility.labelWidth = labelWidth;

                if (check.changed)
                {
                    if (m_AutomaticallyInstantiateSimulatorPrefab.boolValue)
                    {
                        if (m_UseClassic.boolValue)
                            LoadXRDeviceSimulatorSampleAsset<XRDeviceSimulator>(k_XRDeviceSimulatorPrefabName, k_SampleDisplayName);
                        else
                            LoadXRDeviceSimulatorSampleAsset<XRInteractionSimulator>(k_XRInteractionSimulatorPrefabName, k_InteractionSimulatorSampleDisplayName);
                    }
                    else
                        m_SimulatorPrefab.objectReferenceValue = null;

                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                }
            }
        }

        void LoadXRDeviceSimulatorSampleAsset<T>(string simulatorPrefabName, string sampleDisplayName)
        {
            var packageSamples = Sample.FindByPackage(k_PackageName, string.Empty);
            if (packageSamples == null)
            {
                Debug.LogError($"Couldn't find samples of the {k_PackageName} package for importing the {sampleDisplayName} sample; aborting.", this);
                return;
            }

            var foundXRDeviceSimulatorSample = false;

            foreach (var packageSample in packageSamples)
            {
                if (packageSample.displayName != sampleDisplayName)
                    continue;

                if (!packageSample.isImported)
                {
                    string importSampleTitle = "Importing " + sampleDisplayName + " sample.";
                    string importSampleMessage = "The " + sampleDisplayName + " sample is going to be imported from the " + k_PackageDisplayName + " package, press \"Ok\" to continue.";

                    if (EditorUtility.DisplayDialog(importSampleTitle, importSampleMessage, "Ok", "Cancel"))
                    {
                        packageSample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                    else
                    {
                        m_AutomaticallyInstantiateSimulatorPrefab.boolValue = false;
                        return;
                    }
                }

                foundXRDeviceSimulatorSample = true;
                break;
            }

            if (!foundXRDeviceSimulatorSample)
            {
                Debug.LogError($"Couldn't find {sampleDisplayName} sample in the {k_PackageDisplayName} package; aborting.", this);
                return;
            }

            var searchFilter = "\"" + simulatorPrefabName + "\"";
            var foundXRDeviceSimulatorAsset = false;
            foreach (var guid in AssetDatabase.FindAssets(searchFilter))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var simulatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (simulatorPrefab != null && simulatorPrefab.TryGetComponent<T>(out _))
                {
                    m_SimulatorPrefab.objectReferenceValue = simulatorPrefab;
                    foundXRDeviceSimulatorAsset = true;
                }
            }

            if (!foundXRDeviceSimulatorAsset)
            {
                Debug.LogError($"Couldn't find the {simulatorPrefabName} prefab; has the asset been renamed?", this);
            }
        }
    }
}
