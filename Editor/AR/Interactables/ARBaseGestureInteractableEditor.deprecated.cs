using System;
using UnityEditor.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARBaseGestureInteractable"/>.
    /// </summary>
    [Obsolete(k_ARBaseGestureInteractableEditorDeprecated, true)]
    [CustomEditor(typeof(ARBaseGestureInteractable), true), CanEditMultipleObjects]
    public class ARBaseGestureInteractableEditor : XRBaseInteractableEditor
    {
        const string k_ARBaseGestureInteractableEditorDeprecated = "ARBaseGestureInteractable has been deprecated.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARBaseGestureInteractable.xrOrigin"/>.</summary>
        protected SerializedProperty m_XROrigin;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARBaseGestureInteractable.arSessionOrigin"/>.</summary>
        protected SerializedProperty m_ARSessionOrigin;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <c>ARBaseGestureInteractable.m_ExcludeUITouches</c>.</summary>
        protected SerializedProperty m_ExcludeUITouches;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class BaseGestureContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="ARBaseGestureInteractable.xrOrigin"/>.</summary>
            public static readonly GUIContent xrOrigin = EditorGUIUtility.TrTextContent("XR Origin", "The XR Origin that this Interactable will use (such as to get the Camera or to transform from Session space). Will find one if None.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARBaseGestureInteractable.arSessionOrigin"/>.</summary>
            public static readonly GUIContent arSessionOrigin = EditorGUIUtility.TrTextContent("AR Session Origin", "(Deprecated) The AR Session Origin that this Interactable will use (such as to get the Camera or to transform from Session space). Will find one if None.");
            /// <summary><see cref="GUIContent"/> for <c>ARBaseGestureInteractable.excludeUITouches</c>.</summary>
            public static readonly GUIContent excludeUITouches = EditorGUIUtility.TrTextContent("Exclude UI Touches", "Enable to exclude touches that are over UI. Used to make screen space canvas elements block touches from hitting planes behind it.");

            /// <summary>The help box message when AR Session Origin is used.</summary>
            public static readonly GUIContent arSessionOriginDeprecated = EditorGUIUtility.TrTextContent("AR Session Origin has been deprecated. Use the XR Origin component instead.");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableEditorDeprecated);
        }
    }
}
