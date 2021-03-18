using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="XRRig"/>.
    /// </summary>
    [CustomEditor(typeof(XRRig), true), CanEditMultipleObjects]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class XRRigEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.rig"/>.</summary>
        protected SerializedProperty m_RigBaseGameObject;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.cameraFloorOffsetObject"/>.</summary>
        protected SerializedProperty m_CameraFloorOffsetObject;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.cameraGameObject"/>.</summary>
        protected SerializedProperty m_CameraGameObject;
#if UNITY_2019_3_OR_NEWER
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.trackingOriginMode"/>.</summary>
        protected SerializedProperty m_TrackingOriginMode;
#else
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.trackingSpace"/>.</summary>
        protected SerializedProperty m_TrackingSpace;
#endif
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.cameraYOffset"/>.</summary>
        protected SerializedProperty m_CameraYOffset;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.rig"/>.</summary>
            public static readonly GUIContent rig = EditorGUIUtility.TrTextContent("Rig Base GameObject", "The \"Rig\" GameObject is used to refer to the base of the XR Rig, by default it is this GameObject. This is the GameObject that will be manipulated via locomotion.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.cameraFloorOffsetObject"/>.</summary>
            public static readonly GUIContent cameraFloorOffsetObject = EditorGUIUtility.TrTextContent("Camera Floor Offset Object", "The GameObject to move to desired height off the floor (defaults to this object if none provided).");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.cameraGameObject"/>.</summary>
            public static readonly GUIContent cameraGameObject = EditorGUIUtility.TrTextContent("Camera GameObject", "The GameObject that contains the camera, this is usually the \"Head\" of XR rigs.");
#if UNITY_2019_3_OR_NEWER
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.trackingOriginMode"/>.</summary>
            public static readonly GUIContent trackingOriginMode = EditorGUIUtility.TrTextContent("Tracking Origin Mode", "The type of tracking origin to use for this Rig. Tracking origins identify where (0, 0, 0) is in the world of tracking.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.cameraYOffset"/>.</summary>
            public static readonly GUIContent cameraYOffset = EditorGUIUtility.TrTextContent("Camera Y Offset", "Camera Height to be used when in \"Device\" tracking origin mode to define the height of the user from the floor.");
#else
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.trackingSpace"/>.</summary>
            public static readonly GUIContent trackingSpace = EditorGUIUtility.TrTextContent("Tracking Space", "Set if the XR experience is Room Scale or Stationary.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.cameraYOffset"/>.</summary>
            public static readonly GUIContent cameraYOffset = EditorGUIUtility.TrTextContent("Camera Y Offset", "Camera Height to be used when in \"Stationary\" tracking space to define the height of the user from the floor.");
#endif
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        /// <seealso cref="MonoBehaviour"/>
        protected virtual void OnEnable()
        {
            m_RigBaseGameObject = serializedObject.FindProperty("m_RigBaseGameObject");
            m_CameraFloorOffsetObject = serializedObject.FindProperty("m_CameraFloorOffsetObject");
            m_CameraGameObject = serializedObject.FindProperty("m_CameraGameObject");
#if UNITY_2019_3_OR_NEWER
            m_TrackingOriginMode = serializedObject.FindProperty("m_TrackingOriginMode");
#else
            m_TrackingSpace = serializedObject.FindProperty("m_TrackingSpace");
#endif
            m_CameraYOffset = serializedObject.FindProperty("m_CameraYOffset");
        }

        /// <inheritdoc />
        protected override List<string> GetDerivedSerializedPropertyNames()
        {
            var propertyNames = base.GetDerivedSerializedPropertyNames();
            // Ignore m_TrackingSpace since it is deprecated and only kept around for data migration
            propertyNames.Add("m_TrackingSpace");
            return propertyNames;
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
            EditorGUILayout.PropertyField(m_RigBaseGameObject, Contents.rig);
            EditorGUILayout.PropertyField(m_CameraFloorOffsetObject, Contents.cameraFloorOffsetObject);
            EditorGUILayout.PropertyField(m_CameraGameObject, Contents.cameraGameObject);

#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.PropertyField(m_TrackingOriginMode, Contents.trackingOriginMode);
            var showCameraYOffset = m_TrackingOriginMode.enumValueIndex == (int)TrackingOriginModeFlags.Device;
#else
            EditorGUILayout.PropertyField(m_TrackingSpace, Contents.trackingSpace);
            var showCameraYOffset = m_TrackingSpace.enumValueIndex == (int)TrackingSpaceType.Stationary;
#endif
            if (showCameraYOffset)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_CameraYOffset, Contents.cameraYOffset);
                }
            }
        }
    }
}
