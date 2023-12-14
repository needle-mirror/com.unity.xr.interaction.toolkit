using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Readers
{
    [CustomPropertyDrawer(typeof(InputFeatureUsageString<>), true)]
    public class InputFeatureUsageStringPropertyDrawer : PropertyDrawer
    {
        class DropdownHandler
        {
            public SerializedProperty name { get; set; }

            public void OnDropdownItemSelected(object userData)
            {
                if (name != null)
                {
                    name.stringValue = (string)userData;
                    name.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        bool m_ManualTextEditMode;
        readonly GUIContent m_TempContent = new GUIContent();
        GenericMenu m_UsagesMenu;
        readonly DropdownHandler m_DropdownHandler = new DropdownHandler();

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var name = property.FindPropertyRelative("m_Name");

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            label = EditorGUI.BeginProperty(position, label, property);

            //EditorGUI.PropertyField(position, name, label);

            var labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;

            var lineRect = position;
            lineRect.x += labelRect.width + 2f;
            lineRect.width -= labelRect.width + 2f;

            var nameTextRect = lineRect;
            nameTextRect.width -= 20;

            var editButtonRect = lineRect;
            editButtonRect.x += nameTextRect.width;
            editButtonRect.width = 20;
            editButtonRect.height = 15;

            EditorGUI.LabelField(labelRect, label);

            // Get the generic type parameter of the InputFeatureUsageString<T> field, e.g. Vector2
            // ReSharper disable PossibleNullReferenceException -- CustomPropertyDrawer already ensures that the base type exists
            var propertyValueType = property.serializedObject.targetObject
                .GetType().BaseType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Instance)
                .FieldType.GetGenericArguments()[0];

            var allSameType = true;
            if (property.serializedObject.isEditingMultipleObjects)
            {
                allSameType = property.serializedObject.targetObjects.All(target =>
                    target.GetType().BaseType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Instance)
                        .FieldType.GetGenericArguments()[0] == propertyValueType);
            }
            // ReSharper restore PossibleNullReferenceException

            if (m_ManualTextEditMode || !allSameType)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var newName = EditorGUI.TextField(nameTextRect, name.stringValue);
                    if (check.changed)
                        name.stringValue = newName;
                }
            }
            else
            {
                m_TempContent.text = name.stringValue;
                if (EditorGUI.DropdownButton(nameTextRect, m_TempContent, FocusType.Keyboard))
                {
                    m_UsagesMenu = new GenericMenu();
                    var commonUsages = typeof(CommonUsages).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    foreach (var info in commonUsages)
                    {
                        // Exclude fields that have the Obsolete attribute
                        if (info.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length > 0)
                            continue;

                        var inputFeatureUsage = info.GetValue(null);
                        var type = inputFeatureUsage.GetType();
                        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(InputFeatureUsage<>))
                            continue;

                        var genericArgument = type.GetGenericArguments()[0];
                        if (genericArgument == propertyValueType)
                        {
                            // Get the InputFeatureUsage<>.name property
                            var namePropertyInfo = type.GetProperty("name");
                            if (namePropertyInfo == null)
                                continue;

                            var usageName = (string)namePropertyInfo.GetValue(inputFeatureUsage);
                            m_UsagesMenu.AddItem(new GUIContent(usageName), name.stringValue == usageName, m_DropdownHandler.OnDropdownItemSelected, usageName);
                        }
                    }

                    m_DropdownHandler.name = name;
                    m_UsagesMenu.ShowAsContext();
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newTextEditMode = GUI.Toggle(editButtonRect, m_ManualTextEditMode, Contents.editButton, EditorStyles.miniButton);
                if (check.changed)
                    m_ManualTextEditMode = newTextEditMode;
            }

            EditorGUI.EndProperty();
        }

        static class Contents
        {
            public static readonly GUIContent editButton = EditorGUIUtility.TrTextContent("T", "Toggle between text editing and dropdown picker.");
        }
    }
}
