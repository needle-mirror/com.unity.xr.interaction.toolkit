using System;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Helper utility class for Inspector <see cref="Editor"/> classes to warn about a package dependency.
    /// </summary>
    public class PackageManagerEditorHelper
    {
        static class Contents
        {
            public static GUIContent installNow { get; } = EditorGUIUtility.TrTextContent("Install Now");
            public static GUIContent installationInProgress { get; } = EditorGUIUtility.TrTextContent("Installation in progress...");

            public static GUIContent infoIcon { get; } = EditorGUIUtility.TrIconContent("console.infoicon.sml");
            public static GUIContent warningIcon { get; } = EditorGUIUtility.TrIconContent("console.warnicon.sml");
            public static GUIContent errorIcon { get; } = EditorGUIUtility.TrIconContent("console.erroricon.sml");

            public static readonly GUIStyle helpBoxWithButtonStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.UpperLeft,
            };
        }

        static PackageManagerEditorHelper s_ARFoundationHelper;
        /// <summary>
        /// Shared helper for the <c>com.unity.xr.arfoundation</c> package.
        /// </summary>
        public static PackageManagerEditorHelper inputSystemHelper =>
            s_ARFoundationHelper ?? (s_ARFoundationHelper = new PackageManagerEditorHelper("com.unity.xr.arfoundation"));

        readonly string m_PackageIdentifier;

        readonly GUIContent m_DependencyMessage;

        AddRequest m_AddRequest;

        static GUIContent s_HelpBoxMessageContent;

        /// <summary>
        /// Creates a new <see cref="PackageManagerEditorHelper"/> to use for a package.
        /// </summary>
        /// <param name="packageIdentifier">A string representing the package to be added.</param>
        public PackageManagerEditorHelper(string packageIdentifier)
        {
            if (string.IsNullOrEmpty(packageIdentifier))
                throw new ArgumentException($"Package identifier cannot be null or empty.", nameof(packageIdentifier));

            m_PackageIdentifier = packageIdentifier;
            m_DependencyMessage = EditorGUIUtility.TrTextContent($"This component has a dependency on {m_PackageIdentifier}");
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        public void Reset()
        {
            m_AddRequest = null;
        }

        /// <summary>
        /// Draws a help box with a warning that the component has a dependency,
        /// and a button to install the package dependency.
        /// </summary>
        public void DrawDependencyHelpBox()
        {
            EditorGUI.BeginDisabledGroup(m_AddRequest != null && !m_AddRequest.IsCompleted);
            if (HelpBoxWithButton(m_DependencyMessage, Contents.installNow, MessageType.None))
            {
                m_AddRequest = Client.Add(m_PackageIdentifier);
            }
            EditorGUI.EndDisabledGroup();

            if (m_AddRequest != null)
            {
                if (m_AddRequest.Error != null)
                {
                    EditorGUILayout.LabelField(EditorGUIUtility.TrTextContent($"Installation error: {m_AddRequest.Error.errorCode}: {m_AddRequest.Error.message}"), EditorStyles.miniLabel);
                }
                else if (!m_AddRequest.IsCompleted)
                {
                    EditorGUILayout.LabelField(Contents.installationInProgress, EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField(EditorGUIUtility.TrTextContent($"Installation status: {m_AddRequest.Status}"), EditorStyles.miniLabel);
                }
            }
        }

        /// <summary>
        /// Make a help box with a message and button.
        /// </summary>
        /// <param name="messageContent">The message text.</param>
        /// <param name="buttonContent">The button text.</param>
        /// <param name="type">The type of message, such as Warning or Error.</param>
        /// <returns>Returns <see langword="true"/> if button was pressed. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="MaterialEditor.HelpBoxWithButton"/>
        internal static bool HelpBoxWithButton(GUIContent messageContent, GUIContent buttonContent, MessageType type)
        {
            // The vertical spacing is chosen to ensure the button does not overlap the text
            // when the text is long enough to extend to the end of the help box.
            const float kButtonWidth = 90f;
            float kSingleVerticalSpacing = EditorGUIUtility.standardVerticalSpacing; // 2
            float kDoubleVerticalSpacing = EditorGUIUtility.standardVerticalSpacing * 2f; // 4
            const float kButtonHeight = 20f;

            // Set icon for the help box message
            s_HelpBoxMessageContent ??= new GUIContent();
            s_HelpBoxMessageContent.text = messageContent.text;
            s_HelpBoxMessageContent.tooltip = messageContent.tooltip;
            s_HelpBoxMessageContent.image = GetHelpIcon(type);

            // Reserve size of wrapped text
            var contentRect = GUILayoutUtility.GetRect(s_HelpBoxMessageContent, EditorStyles.helpBox);
            // Reserve size of button
            GUILayoutUtility.GetRect(1f, kButtonHeight + kSingleVerticalSpacing);

            // Render background box with text at full height
            contentRect.height += kButtonHeight + kDoubleVerticalSpacing;
            GUI.Label(contentRect, s_HelpBoxMessageContent, Contents.helpBoxWithButtonStyle);

            // Button (align lower right)
            var buttonRect = new Rect(contentRect.xMax - kButtonWidth - 4f, contentRect.yMax - kButtonHeight - 4f, kButtonWidth, kButtonHeight);
            return GUI.Button(buttonRect, buttonContent);
        }

        static Texture GetHelpIcon(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return Contents.infoIcon.image;
                case MessageType.Warning:
                    return Contents.warningIcon.image;
                case MessageType.Error:
                    return Contents.errorIcon.image;
                default:
                    return null;
            }
        }
    }
}
