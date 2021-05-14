using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.trackingOriginMode"/>.</summary>
        [Obsolete("m_TrackingOriginMode has been deprecated. Use m_RequestedTrackingOriginMode instead.")]
        protected SerializedProperty m_TrackingOriginMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.requestedTrackingOriginMode"/>.</summary>
        protected SerializedProperty m_RequestedTrackingOriginMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRig.cameraYOffset"/>.</summary>
        protected SerializedProperty m_CameraYOffset;

        List<XRRig> m_Rigs;

        readonly GUIContent[] m_MixedValuesOptions = { Contents.mixedValues };

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
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.requestedTrackingOriginMode"/>.</summary>
            public static readonly GUIContent trackingOriginMode = EditorGUIUtility.TrTextContent("Tracking Origin Mode", "The type of tracking origin to use for this Rig. Tracking origins identify where (0, 0, 0) is in the world of tracking.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.currentTrackingOriginMode"/>.</summary>
            public static readonly GUIContent currentTrackingOriginMode = EditorGUIUtility.TrTextContent("Current Tracking Origin Mode", "The Tracking Origin Mode that this Rig is in.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRig.cameraYOffset"/>.</summary>
            public static readonly GUIContent cameraYOffset = EditorGUIUtility.TrTextContent("Camera Y Offset", "Camera height to be used when in \"Device\" Tracking Origin Mode to define the height of the user from the floor.");
            /// <summary><see cref="GUIContent"/> to indicate mixed values when multi-object editing.</summary>
            public static readonly GUIContent mixedValues = EditorGUIUtility.TrTextContent("\u2014", "Mixed Values");
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
#pragma warning disable 618 // Setting deprecated field to help with backwards compatibility with existing user code.
            m_TrackingOriginMode = serializedObject.FindProperty("m_TrackingOriginMode");
#pragma warning restore 618
            m_RequestedTrackingOriginMode = serializedObject.FindProperty("m_RequestedTrackingOriginMode");
            m_CameraYOffset = serializedObject.FindProperty("m_CameraYOffset");

            m_Rigs = targets.Cast<XRRig>().ToList();
        }

        /// <inheritdoc />
        protected override List<string> GetDerivedSerializedPropertyNames()
        {
            var propertyNames = base.GetDerivedSerializedPropertyNames();
            // Ignore these fields since they are deprecated and only kept around for data migration
            propertyNames.Add("m_TrackingSpace");
            propertyNames.Add("m_TrackingOriginMode");
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

            EditorGUILayout.PropertyField(m_RequestedTrackingOriginMode, Contents.trackingOriginMode);

            var showCameraYOffset =
                m_RequestedTrackingOriginMode.enumValueIndex == (int)XRRig.TrackingOriginMode.NotSpecified ||
                m_RequestedTrackingOriginMode.enumValueIndex == (int)XRRig.TrackingOriginMode.Device ||
                m_RequestedTrackingOriginMode.hasMultipleDifferentValues;
            if (showCameraYOffset)
            {
                // The property should be enabled when not playing since the default for the XR device
                // may be Device, so the property should be editable to define the offset.
                // When playing, disable the property to convey that it isn't having an effect,
                // which is when the current mode is Floor.
                var currentTrackingOriginMode = ((XRRig)target).currentTrackingOriginMode;
                var allCurrentlyFloor = (m_Rigs.Count == 1 && currentTrackingOriginMode == TrackingOriginModeFlags.Floor) ||
                    m_Rigs.All(rig => rig.currentTrackingOriginMode == TrackingOriginModeFlags.Floor);
                var disabled = Application.isPlaying &&
                    !m_RequestedTrackingOriginMode.hasMultipleDifferentValues &&
                    m_RequestedTrackingOriginMode.enumValueIndex == (int)XRRig.TrackingOriginMode.NotSpecified &&
                    allCurrentlyFloor;
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledScope(disabled))
                {
                    EditorGUILayout.PropertyField(m_CameraYOffset, Contents.cameraYOffset);
                }
            }

            DrawCurrentTrackingOriginMode();
        }

        /// <summary>
        /// Draw the current Tracking Origin Mode while the application is playing.
        /// </summary>
        /// <seealso cref="XRRig.currentTrackingOriginMode"/>
        protected void DrawCurrentTrackingOriginMode()
        {
            if (!Application.isPlaying)
                return;

            using (new EditorGUI.DisabledScope(true))
            {
                var currentTrackingOriginMode = ((XRRig)target).currentTrackingOriginMode;
                if (m_Rigs.Count == 1 || m_Rigs.All(rig => rig.currentTrackingOriginMode == currentTrackingOriginMode))
                    EditorGUILayout.EnumPopup(Contents.currentTrackingOriginMode, currentTrackingOriginMode);
                else
                    EditorGUILayout.Popup(Contents.currentTrackingOriginMode, 0, m_MixedValuesOptions);
            }
        }
    }
}
