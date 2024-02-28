using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    class CompactPropertyControl
    {
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
        /// The text message for a warning icon to show next to each property. Corresponds 1:1 with the <see cref="properties"/> list.
        /// May be less than the size of the properties list if remaining properties do not have warnings.
        /// </summary>
        public List<string> propertiesWarningMessage { get; } = new List<string>();

        /// <summary>
        /// Whether to show an info help box below the properties.
        /// </summary>
        /// <seealso cref="infoHelpBoxMessage"/>
        public bool hasInfoHelpBox { get; set; }

        /// <summary>
        /// The text message to show in the info help box.
        /// </summary>
        /// <seealso cref="hasInfoHelpBox"/>
        public GUIContent infoHelpBoxMessage { get; set; }

        /// <summary>
        /// Whether to show a help icon with mouseover tooltip next to the second property.
        /// Does not show up if there is only one property.
        /// </summary>
        /// <seealso cref="helpTooltip"/>
        public bool hasHelpTooltip { get; set; }

        /// <summary>
        /// The tooltip to show when hovering over the help icon.
        /// </summary>
        /// <seealso cref="hasHelpTooltip"/>
        public string helpTooltip { get; set; }

        readonly HelpBoxPopup m_HelpBoxPopup = new HelpBoxPopup { messageType = MessageType.Warning };
        readonly GUIContent m_TempContent = new GUIContent();

        public float GetPropertyHeight()
        {
            var height = 0f;
            if (properties.Count == 0)
                height = EditorGUI.GetPropertyHeight(modeProperty);
            else
            {
                for (var index = 0; index < properties.Count; ++index)
                {
                    height += EditorGUI.GetPropertyHeight(properties[index]);
                }

                // Add spacing between properties
                if (properties.Count > 1)
                    height += EditorGUIUtility.standardVerticalSpacing * (properties.Count - 1);
            }

            if (hasInfoHelpBox)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        public void DrawCompactGUI(Rect position, GUIContent label)
        {
            Debug.Assert(modeProperty != null);

            position = EditorGUI.PrefixLabel(position, label);

            // Calculate rect for configuration button
            var buttonRect = position;
            var popupStyle = Styles.popup;
            buttonRect.yMin += popupStyle.margin.top + 1f;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            position.xMin = buttonRect.xMax;

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Using BeginProperty / EndProperty on the popup button allows the user to
            // revert prefab overrides to mode by right-clicking the configuration button.
            EditorGUI.BeginProperty(buttonRect, GUIContent.none, modeProperty);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newPopupIndex = EditorGUI.Popup(buttonRect, modeProperty.intValue, modePopupOptions, popupStyle);
                if (check.changed)
                    modeProperty.intValue = newPopupIndex;
            }
            EditorGUI.EndProperty();

            if (properties.Count > 0)
            {
                // Rect for the first property, including the warning icon if applicable.
                var rowRect = position;
                rowRect.height = EditorGUI.GetPropertyHeight(properties[0]);

                // Rect for the property itself, which may be shifted over if there is a warning icon.
                var rect = rowRect;
                if (propertiesWarningMessage.Count > 0)
                    DrawWarningIcon(ref rect, propertiesWarningMessage[0]);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(rect, properties[0], GUIContent.none);
                    if (check.changed)
                        propertyChanged?.Invoke(properties[0]);
                }

                if (properties.Count > 1 && hasHelpTooltip)
                {
                    var helpRect = buttonRect;
                    helpRect.y += rowRect.height + EditorGUIUtility.standardVerticalSpacing;
                    m_TempContent.image = Contents.helpIcon.image;
                    m_TempContent.tooltip = helpTooltip;
                    EditorGUI.LabelField(helpRect, m_TempContent, EditorStyles.iconButton);
                }

                for (var index = 1; index < properties.Count; ++index)
                {
                    rowRect.y += rowRect.height + EditorGUIUtility.standardVerticalSpacing;
                    rowRect.height = EditorGUI.GetPropertyHeight(properties[index]);

                    rect = rowRect;
                    if (propertiesWarningMessage.Count > index)
                        DrawWarningIcon(ref rect, propertiesWarningMessage[index]);

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(rect, properties[index], GUIContent.none);
                        if (check.changed)
                            propertyChanged?.Invoke(properties[index]);
                    }
                }

                if (hasInfoHelpBox)
                {
                    rowRect.y += rowRect.height + EditorGUIUtility.standardVerticalSpacing;
                    rowRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.HelpBox(rowRect, infoHelpBoxMessage.text, MessageType.Info);
                }
            }
            else
            {
                EditorGUI.LabelField(position, Contents.unusedLabelText, EditorStyles.miniLabel);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        }

        void DrawWarningIcon(ref Rect rowRect, string warningMessage)
        {
            if (warningMessage == null)
                return;

            // Create space for the warning icon and shift the property rect over.
            var warningRect = rowRect;
            var iconButtonStyle = EditorStyles.iconButton;
            warningRect.yMin += iconButtonStyle.margin.top + 1f;
            warningRect.width = iconButtonStyle.fixedWidth + iconButtonStyle.margin.right;
            warningRect.height = EditorGUIUtility.singleLineHeight;
            rowRect.xMin = warningRect.xMax;

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
            public static readonly GUIContent unusedLabelText = EditorGUIUtility.TrTextContent("Unused");
            public static readonly GUIContent helpIcon = EditorGUIUtility.TrIconContent("_Help");
            public static readonly GUIContent warningIcon = EditorGUIUtility.TrIconContent("console.warnicon.sml");
        }

        static class Styles
        {
            public static readonly GUIStyle popup = new GUIStyle("PaneOptions") { imagePosition = ImagePosition.ImageOnly };
        }
    }
}
