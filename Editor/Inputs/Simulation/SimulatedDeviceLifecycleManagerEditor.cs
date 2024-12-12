using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Custom editor for <see cref="SimulatedDeviceLifecycleManager"/>.
    /// </summary>
    [CustomEditor(typeof(SimulatedDeviceLifecycleManager), true), CanEditMultipleObjects]
    class SimulatedDeviceLifecycleManagerEditor : BaseInteractionEditor
    {
        protected SerializedProperty m_RemoveOtherHMDDevices;
        protected SerializedProperty m_HandTrackingCapability;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent removeOtherHMDDevices = EditorGUIUtility.TrTextContent("Remove Other HMD Devices", "Whether to remove other XR HMD devices in this session so that they don't conflict with the simulated devices.");
            public static readonly GUIContent handTrackingCapability = EditorGUIUtility.TrTextContent("Hand Tracking Capability", "Whether to create a simulated Hand Tracking Subsystem and provider on startup. Requires the XR Hands package.");

            public static readonly GUIContent handPropertiesHeader = EditorGUIUtility.TrTextContent("Hand Properties");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_RemoveOtherHMDDevices = serializedObject.FindProperty("m_RemoveOtherHMDDevices");
            m_HandTrackingCapability = serializedObject.FindProperty("m_HandTrackingCapability");
        }

        /// <inheritdoc />
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
            EditorGUILayout.PropertyField(m_RemoveOtherHMDDevices, Contents.removeOtherHMDDevices);
            EditorGUILayout.Space();
            DrawHandProperties();
        }

        /// <summary>
        /// Draw the property fields related to hands.
        /// </summary>
        protected virtual void DrawHandProperties()
        {
            EditorGUILayout.LabelField(Contents.handPropertiesHeader, EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HandTrackingCapability, Contents.handTrackingCapability);
            }
        }
    }
}
