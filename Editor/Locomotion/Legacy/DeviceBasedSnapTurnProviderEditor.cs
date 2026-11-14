using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for a <see cref="DeviceBasedSnapTurnProvider"/>.
    /// </summary>
    [CustomEditor(typeof(DeviceBasedSnapTurnProvider), true), CanEditMultipleObjects]
    [Obsolete(k_DeviceBasedSnapTurnProviderEditorDeprecated, true)]
    public class DeviceBasedSnapTurnProviderEditor : BaseInteractionEditor
    {
        const string k_DeviceBasedSnapTurnProviderEditorDeprecated = "DeviceBasedSnapTurnProviderEditor has been deprecated in version 3.0.0.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <c>LocomotionProvider.system</c>.</summary>
        protected SerializedProperty m_System;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="SnapTurnProviderBase.turnAmount"/>.</summary>
        protected SerializedProperty m_TurnAmount;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="SnapTurnProviderBase.debounceTime"/>.</summary>
        protected SerializedProperty m_DebounceTime;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="SnapTurnProviderBase.enableTurnLeftRight"/>.</summary>
        protected SerializedProperty m_EnableTurnLeftRight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="SnapTurnProviderBase.enableTurnAround"/>.</summary>
        protected SerializedProperty m_EnableTurnAround;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="SnapTurnProviderBase.delayTime"/>.</summary>
        protected SerializedProperty m_DelayTime;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="DeviceBasedSnapTurnProvider.turnUsage"/>.</summary>
        protected SerializedProperty m_TurnUsage;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="DeviceBasedSnapTurnProvider.controllers"/>.</summary>
        protected SerializedProperty m_Controllers;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="DeviceBasedSnapTurnProvider.deadZone"/>.</summary>
        protected SerializedProperty m_DeadZone;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        [Obsolete(k_ContentsDeprecated, true)]
        protected static class Contents
        {
            const string k_ContentsDeprecated = "DeviceBasedSnapTurnProviderEditor.Contents has been deprecated in version 3.0.0.";

            /// <summary><see cref="GUIContent"/> for <c>LocomotionProvider.system</c>.</summary>
            public static readonly GUIContent system = EditorGUIUtility.TrTextContent("System", "The locomotion system that the snap turn provider will interface with.");

            /// <summary><see cref="GUIContent"/> for <see cref="SnapTurnProviderBase.turnAmount"/>.</summary>
            public static readonly GUIContent turnAmount = EditorGUIUtility.TrTextContent("Turn Amount", "The number of degrees to turn around the Y axis when performing a right handed snap turn. This will automatically be negated for left turns.");
            /// <summary><see cref="GUIContent"/> for <see cref="SnapTurnProviderBase.debounceTime"/>.</summary>
            public static readonly GUIContent debounceTime = EditorGUIUtility.TrTextContent("Activation Timeout", "How long between a successful snap turn does the user need to wait before being able to perform a subsequent snap turn.");
            /// <summary><see cref="GUIContent"/> for <see cref="SnapTurnProviderBase.enableTurnLeftRight"/>.</summary>
            public static readonly GUIContent enableTurnLeftRight = EditorGUIUtility.TrTextContent("Enable Turn Left & Right", "Controls whether to enable left & right snap turns.");
            /// <summary><see cref="GUIContent"/> for <see cref="SnapTurnProviderBase.enableTurnAround"/>.</summary>
            public static readonly GUIContent enableTurnAround = EditorGUIUtility.TrTextContent("Enable Turn Around", "Controls whether to enable 180° snap turns.");
            /// <summary><see cref="GUIContent"/> for <see cref="SnapTurnProviderBase.delayTime"/>.</summary>
            public static readonly GUIContent delayTime = EditorGUIUtility.TrTextContent("Delay Time", "The time (in seconds) to delay the first turn after receiving initial input for the turn.");

            /// <summary><see cref="GUIContent"/> for <see cref="DeviceBasedSnapTurnProvider.turnUsage"/>.</summary>
            public static readonly GUIContent turnUsage = EditorGUIUtility.TrTextContent("Turn Input Source", "The Input axis to use to begin a snap turn.");
            /// <summary><see cref="GUIContent"/> for <see cref="DeviceBasedSnapTurnProvider.controllers"/>.</summary>
            public static readonly GUIContent controllers = EditorGUIUtility.TrTextContent("Controllers", "XRControllers that allow for snap turning.");
            /// <summary><see cref="GUIContent"/> for <see cref="DeviceBasedSnapTurnProvider.deadZone"/>.</summary>
            public static readonly GUIContent deadZone = EditorGUIUtility.TrTextContent("Dead Zone", "Minimum distance of axis travel before performing a snap turn.");
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        protected override void DrawInspector()
        {
            throw new NotSupportedException(k_DeviceBasedSnapTurnProviderEditorDeprecated);
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the section of the custom inspector before <see cref="DrawProperties"/>.
        /// By default, this draws the read-only Script property.
        /// </summary>
        protected virtual void DrawBeforeProperties()
        {
            throw new NotSupportedException(k_DeviceBasedSnapTurnProviderEditorDeprecated);
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
            throw new NotSupportedException(k_DeviceBasedSnapTurnProviderEditorDeprecated);
        }
    }
}
