using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="XRController"/>.
    /// </summary>
    [CustomEditor(typeof(XRController), true), CanEditMultipleObjects]
    [Obsolete(k_XRControllerEditorDeprecated, true)]
    public class XRControllerEditor : XRBaseControllerEditor
    {
        const string k_XRControllerEditorDeprecated = "XRControllerEditor has been deprecated in version 3.0.0.";

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.controllerNode"/>.</summary>
            public static GUIContent controllerNode = EditorGUIUtility.TrTextContent("Controller Node", "The XR Node for this controller.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.selectUsage"/>.</summary>
            public static GUIContent selectUsage = EditorGUIUtility.TrTextContent("Select Usage", "The input to use for detecting a select.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.activateUsage"/>.</summary>
            public static GUIContent activateUsage = EditorGUIUtility.TrTextContent("Activate Usage", "The input to use for detecting activation.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.uiPressUsage"/>.</summary>
            public static GUIContent uiPressUsage = EditorGUIUtility.TrTextContent("UI Press Usage", "The input to use for detecting a UI press.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.axisToPressThreshold"/>.</summary>
            public static GUIContent axisToPressThreshold = EditorGUIUtility.TrTextContent("Axis To Press Threshold", "The amount an axis needs to be pressed to trigger an interaction event.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.rotateObjectLeft"/>.</summary>
            public static GUIContent rotateAnchorLeft = EditorGUIUtility.TrTextContent("Rotate Object Left", "The input to use to rotate an anchor to the left.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.rotateObjectRight"/>.</summary>
            public static GUIContent rotateAnchorRight = EditorGUIUtility.TrTextContent("Rotate Object Right", "The input to use to rotate an anchor to the right.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.moveObjectIn"/>.</summary>
            public static GUIContent moveObjectIn = EditorGUIUtility.TrTextContent("Move Object In", "The input that will be used to translate the anchor away from the interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.moveObjectOut"/>.</summary>
            public static GUIContent moveObjectOut = EditorGUIUtility.TrTextContent("Move Object Out", "The input that will be used to translate the anchor towards the interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.directionalAnchorRotation"/>.</summary>
            public static GUIContent directionalAnchorRotation = EditorGUIUtility.TrTextContent("Directional Anchor Rotation", "The input to use to compute a directional angle to rotate the interactor's attach point to match it.");

#if XR_LEGACY_INPUT_HELPERS_2_1_OR_NEWER || PACKAGE_DOCS_GENERATION
            /// <summary><see cref="GUIContent"/> for <see cref="XRController.poseProvider"/>.</summary>
            public static GUIContent poseProvider = EditorGUIUtility.TrTextContent("Pose Provider", "Pose provider used to provide tracking data separate from the XR Node.");
            /// <summary>The help box message when Pose Provider is being used.</summary>
            public static GUIContent poseProviderWarning = EditorGUIUtility.TrTextContent("This XR Controller is using an external pose provider for tracking.  This takes priority over the Controller Node setting.");
#endif
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawInputConfiguration()
        {
            throw new NotSupportedException(k_XRControllerEditorDeprecated);
        }

        /// <inheritdoc />
        protected override void DrawOtherActions()
        {
            throw new NotSupportedException(k_XRControllerEditorDeprecated);
        }
    }
}
