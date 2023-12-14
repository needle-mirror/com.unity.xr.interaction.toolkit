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
        /// Whether to show a warning help box below the properties.
        /// </summary>
        /// <seealso cref="warningHelpBoxMessage"/>
        public bool hasWarningHelpBox { get; set; }

        /// <summary>
        /// The text message to show in the warning help box.
        /// </summary>
        /// <seealso cref="hasWarningHelpBox"/>
        public GUIContent warningHelpBoxMessage { get; set; }

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

        readonly GUIContent m_HelpContent = new GUIContent();

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

            if (hasWarningHelpBox)
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
                var firstRect = position;
                firstRect.height = EditorGUI.GetPropertyHeight(properties[0]);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(firstRect, properties[0], GUIContent.none);
                    if (check.changed)
                        propertyChanged?.Invoke(properties[0]);
                }

                if (properties.Count > 1 && hasHelpTooltip)
                {
                    var helpRect = buttonRect;
                    helpRect.y += firstRect.height + EditorGUIUtility.standardVerticalSpacing;
                    m_HelpContent.image = Contents.helpIcon.image;
                    m_HelpContent.tooltip = helpTooltip;
                    EditorGUI.LabelField(helpRect, m_HelpContent, EditorStyles.iconButton);
                }

                var rect = firstRect;
                for (var index = 1; index < properties.Count; ++index)
                {
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = EditorGUI.GetPropertyHeight(properties[index]);
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(rect, properties[index], GUIContent.none);
                        if (check.changed)
                            propertyChanged?.Invoke(properties[index]);
                    }
                }

                if (hasWarningHelpBox)
                {
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.HelpBox(rect, warningHelpBoxMessage.text, MessageType.Info);
                }
            }
            else
            {
                EditorGUI.LabelField(position, Contents.unusedLabelText, EditorStyles.miniLabel);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        }

        static class Contents
        {
            public static readonly GUIContent unusedLabelText = EditorGUIUtility.TrTextContent("Unused");
            public static readonly GUIContent helpIcon = EditorGUIUtility.TrIconContent("_Help");
        }

        static class Styles
        {
            public static readonly GUIStyle popup = new GUIStyle("PaneOptions") { imagePosition = ImagePosition.ImageOnly };
        }
    }
}
