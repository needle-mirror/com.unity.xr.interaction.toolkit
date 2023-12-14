using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    class MultilinePropertyControl
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
        /// The label to show for each property. Corresponds 1:1 with the <see cref="properties"/> list.
        /// </summary>
        public List<GUIContent> propertiesContent { get; } = new List<GUIContent>();

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

            if (hasWarningHelpBox)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        public void DrawMultilineGUI(Rect position, GUIContent label)
        {
            Debug.Assert(modeProperty != null);
            Debug.Assert(properties.Count == propertiesContent.Count);

            const float kIndent = 15f;

            var titleRect = position;
            titleRect.height = EditorGUIUtility.singleLineHeight;

            var inputSourceModeRect = position;
            inputSourceModeRect.x += kIndent;
            inputSourceModeRect.width -= kIndent;
            inputSourceModeRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            inputSourceModeRect.height = EditorGUI.GetPropertyHeight(modeProperty);

            var firstRect = position;
            firstRect.x += kIndent;
            firstRect.width -= kIndent;
            firstRect.y += (titleRect.height + inputSourceModeRect.height) + (EditorGUIUtility.standardVerticalSpacing * 2f);

            EditorGUI.LabelField(titleRect, label);
            EditorGUI.PropertyField(inputSourceModeRect, modeProperty);

            if (properties.Count > 0)
            {
                firstRect.height = EditorGUI.GetPropertyHeight(properties[0]);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(firstRect, properties[0], propertiesContent[0]);
                    if (check.changed)
                        propertyChanged?.Invoke(properties[0]);
                }

                var rect = firstRect;
                for (var index = 1; index < properties.Count; ++index)
                {
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = EditorGUI.GetPropertyHeight(properties[index]);
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(rect, properties[index], propertiesContent[index]);
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
        }
    }
}
