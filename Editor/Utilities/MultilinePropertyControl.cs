using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    class MultilinePropertyControl
    {
        const float k_Indent = 15f;

        /// <summary>
        /// Invoked when one of the <see cref="properties"/> has changed.
        /// Does not include <see cref="modeProperty"/>.
        /// </summary>
        public event Action<SerializedProperty> propertyChanged;

        /// <summary>
        /// The serialized property that determines which property to show.
        /// </summary>
        public SerializedProperty modeProperty { get; set; }

        /// <summary>
        /// The text options to show in the mode popup.
        /// </summary>
        public GUIContent[] modePopupOptions { get; set; }

        /// <summary>
        /// The subset of serialized properties to actively show. Should not include the mode property.
        /// </summary>
        public List<SerializedProperty> properties { get; } = new List<SerializedProperty>();

        /// <summary>
        /// The label to show for each property. Corresponds 1:1 with the <see cref="properties"/> list.
        /// </summary>
        public List<GUIContent> propertiesContent { get; } = new List<GUIContent>();

        /// <summary>
        /// The text message for a warning icon to show next to each property. Corresponds 1:1 with the <see cref="properties"/> list.
        /// May be less than the size of the properties list if remaining properties do not have warnings.
        /// </summary>
        public List<string> propertiesWarningMessage { get; } = new List<string>();

        /// <summary>
        /// Whether to show a warning help box below the properties.
        /// </summary>
        /// <seealso cref="infoHelpBoxMessage"/>
        public bool hasInfoHelpBox { get; set; }

        /// <summary>
        /// The text message to show in the warning help box.
        /// </summary>
        /// <seealso cref="hasInfoHelpBox"/>
        public GUIContent infoHelpBoxMessage { get; set; }

        readonly HelpBoxPopup m_HelpBoxPopup = new HelpBoxPopup { messageType = MessageType.Warning };
        readonly GUIContent m_TempContent = new GUIContent();

        public float GetPropertyHeight()
        {
            var height = 0f;

            // Field label.
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Mode dropdown.
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (var index = 0; index < properties.Count; ++index)
            {
                height += EditorGUI.GetPropertyHeight(properties[index]);
            }

            // Add spacing between properties
            if (properties.Count > 1)
                height += EditorGUIUtility.standardVerticalSpacing * (properties.Count - 1);

            if (hasInfoHelpBox)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        public void DrawMultilineGUI(Rect position, GUIContent label)
        {
            Debug.Assert(modeProperty != null);
            Debug.Assert(properties.Count == propertiesContent.Count);

            var titleRect = position;
            titleRect.height = EditorGUIUtility.singleLineHeight;

            var inputSourceModeRect = position;
            inputSourceModeRect.x += k_Indent;
            inputSourceModeRect.width -= k_Indent;
            inputSourceModeRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            inputSourceModeRect.height = EditorGUI.GetPropertyHeight(modeProperty);

            var firstRowRect = position;
            firstRowRect.x += k_Indent;
            firstRowRect.width -= k_Indent;
            firstRowRect.y += (titleRect.height + inputSourceModeRect.height) + (EditorGUIUtility.standardVerticalSpacing * 2f);

            EditorGUI.LabelField(titleRect, label);
            EditorGUI.PropertyField(inputSourceModeRect, modeProperty);

            if (properties.Count > 0)
            {
                // Rect for the first property, including the warning icon if applicable.
                firstRowRect.height = EditorGUI.GetPropertyHeight(properties[0]);

                if (propertiesWarningMessage.Count > 0)
                    DrawWarningIcon(firstRowRect, propertiesWarningMessage[0]);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(firstRowRect, properties[0], propertiesContent[0]);
                    if (check.changed)
                        propertyChanged?.Invoke(properties[0]);
                }

                var rect = firstRowRect;
                for (var index = 1; index < properties.Count; ++index)
                {
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = EditorGUI.GetPropertyHeight(properties[index]);

                    if (propertiesWarningMessage.Count > index)
                        DrawWarningIcon(rect, propertiesWarningMessage[index]);

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(rect, properties[index], propertiesContent[index]);
                        if (check.changed)
                            propertyChanged?.Invoke(properties[index]);
                    }
                }

                if (hasInfoHelpBox)
                {
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.HelpBox(rect, infoHelpBoxMessage.text, MessageType.Info);
                }
            }
        }

        void DrawWarningIcon(Rect rowRect, string warningMessage)
        {
            if (warningMessage == null)
                return;

            // Show the warning icon to the left of the property, and do not shift the property rect over.
            // This is different from the Compact drawer, which shows the warning icon next to the value
            // and shifts the value over to the right. This is because we are using PropertyField to draw the
            // property with a label, which does not allow us to shift the object field over without also shifting
            // the label. The right edge of the label should always line up vertically with all property fields.
            var warningRect = rowRect;
            var iconButtonStyle = EditorStyles.iconButton;
            warningRect.yMin += iconButtonStyle.margin.top + 1f;
            warningRect.width = iconButtonStyle.fixedWidth + iconButtonStyle.margin.right;
            warningRect.height = EditorGUIUtility.singleLineHeight;
            warningRect.x = warningRect.x - warningRect.width + (EditorGUI.indentLevel * k_Indent);

            m_TempContent.image = Contents.warningIcon.image;
            m_TempContent.tooltip = string.Empty;
            if (EditorGUI.DropdownButton(warningRect, m_TempContent, FocusType.Keyboard, EditorStyles.iconButton))
            {
                m_HelpBoxPopup.message = warningMessage;
                PopupWindow.Show(warningRect, m_HelpBoxPopup);
            }
        }

        static class Contents
        {
            public static readonly GUIContent warningIcon = EditorGUIUtility.TrIconContent("console.warnicon.sml");
        }
    }
}
