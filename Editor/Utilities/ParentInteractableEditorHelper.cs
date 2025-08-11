using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Helper class for an Editor to draw properties related to <see cref="IXRParentInteractableLink"/>.
    /// Common code shared between interactor and interactable editors.
    /// </summary>
    class ParentInteractableEditorHelper
    {
        /// <summary>
        /// Container class for an individual component when single-object or multi-object editing.
        /// </summary>
        /// <remarks>
        /// Since the button shortcut for assigning a parent interactable to the field is a different field and
        /// hierarchy for each of the components when being multi-object edited, this class creates the <see cref="SerializedObject"/>
        /// and <see cref="SerializedProperty"/> for a component in the list when multi-object editing.
        /// </remarks>
        class ObjectFieldInfo
        {
            public Object targetObject { get; }
            public SerializedObject serializedObject { get; }
            // ReSharper disable once InconsistentNaming -- Prefer to name it after the field
            public SerializedProperty m_ParentInteractableObject { get; }

            public IXRInteractable hierarchyParentInteractable { get; set; }
            public bool canAssign { get; set; }
            public bool canReplace { get; set; }

            public ObjectFieldInfo(Object targetObject, SerializedObject serializedObject, SerializedProperty parentInteractableObject)
            {
                this.targetObject = targetObject;
                this.serializedObject = serializedObject;
                m_ParentInteractableObject = parentInteractableObject;
            }

            public ObjectFieldInfo(Object targetObject)
            {
                this.targetObject = targetObject;
                serializedObject = new SerializedObject(targetObject);
                m_ParentInteractableObject = serializedObject.FindProperty("m_ParentInteractableObject");
            }

            public void Assign(bool applyModifiedProperties)
            {
                if (canAssign)
                {
                    m_ParentInteractableObject.objectReferenceValue = hierarchyParentInteractable as Object;

                    if (applyModifiedProperties)
                        serializedObject.ApplyModifiedProperties();
                }
            }

            public void Replace(bool applyModifiedProperties)
            {
                if (canReplace)
                {
                    m_ParentInteractableObject.objectReferenceValue = hierarchyParentInteractable as Object;

                    if (applyModifiedProperties)
                        serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="IXRParentInteractableLink.autoFindParentInteractableInHierarchy"/>.</summary>
        SerializedProperty m_AutoFindParentInteractableInHierarchy;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="IXRParentInteractableLink.parentInteractable"/>.</summary>
        SerializedProperty m_ParentInteractableObject;

        bool m_IsMultiObjectEditing;
        readonly List<ObjectFieldInfo> m_ObjectFieldInfos = new List<ObjectFieldInfo>();

        /// <summary>
        /// Initialize the <see cref="SerializedProperty"/> fields. Call this method from <c>OnEnable</c> in the Editor class.
        /// </summary>
        public void InitializeSerializedProperties(
            SerializedObject serializedObject, SerializedProperty autoFindParentInteractableInHierarchy, SerializedProperty parentInteractableObject)
        {
            m_AutoFindParentInteractableInHierarchy = autoFindParentInteractableInHierarchy;
            m_ParentInteractableObject = parentInteractableObject;

            var targetObjects = serializedObject.targetObjects;
            m_IsMultiObjectEditing = targetObjects.Length > 1;

            // Since this method is called during Editor.OnEnable, we can do the one-time setup to avoid constant
            // allocations each editor update tick.
            m_ObjectFieldInfos.Clear();
            if (m_IsMultiObjectEditing)
            {
                // When multi-object editing, set up the SerializedObject and find fields for each target object.
                foreach (var targetObject in targetObjects)
                {
                    m_ObjectFieldInfos.Add(new ObjectFieldInfo(targetObject));
                }
            }
            else
            {
                // When single-object editing, use the SerializedObject and fields owned by the Editor
                m_ObjectFieldInfos.Add(new ObjectFieldInfo(targetObjects[0], serializedObject, parentInteractableObject));
            }
        }

        /// <summary>
        /// Draw the property fields related to parent interactable.
        /// </summary>
        /// <param name="serializedObject">A SerializedObject representing the object or objects being inspected.</param>
        public void DrawParentInteractableConfiguration(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(m_ParentInteractableObject, Contents.parentInteractable);

            var anyParentInteractableObjectNull = false;
            var numShowAssignHelpBox = 0;
            var numShowReplaceHelpBox = 0;
            foreach (var info in m_ObjectFieldInfos)
            {
                if (m_IsMultiObjectEditing)
                    info.serializedObject.Update();

                var parent = ((Component)info.targetObject).transform.parent;
                info.hierarchyParentInteractable = parent != null ? parent.GetComponentInParent<IXRInteractable>(true) : null;
                info.canAssign = info.hierarchyParentInteractable != null && info.m_ParentInteractableObject.objectReferenceValue == null;
                info.canReplace = info.hierarchyParentInteractable != null && info.m_ParentInteractableObject.objectReferenceValue != null &&
                    !ReferenceEquals(info.hierarchyParentInteractable, info.m_ParentInteractableObject.objectReferenceValue);

                anyParentInteractableObjectNull |= info.m_ParentInteractableObject.objectReferenceValue is null;

                if (info.canAssign)
                    numShowAssignHelpBox++;

                if (info.canReplace)
                    numShowReplaceHelpBox++;
            }

            // Assign
            if (numShowAssignHelpBox > 0)
            {
                var buttonContent = m_IsMultiObjectEditing
                    ? EditorGUIUtility.TrTextContent(
                        string.Format(Contents.parentInteractableAssignNumberedButton.text, numShowAssignHelpBox.ToString()),
                        string.Format(Contents.parentInteractableAssignNumberedButton.tooltip, numShowAssignHelpBox.ToString()))
                    : Contents.parentInteractableAssignButton;

                var messageContent = Contents.parentInteractableAssignPrompt;

                // Use the prompt with the GameObject name instead when there's just one applicable component.
                if (numShowAssignHelpBox == 1)
                {
                    foreach (var info in m_ObjectFieldInfos)
                    {
                        if (info.canAssign)
                        {
                            messageContent = EditorGUIUtility.TrTextContent(
                                string.Format(Contents.parentInteractableAssignPromptWithName.text, info.hierarchyParentInteractable.transform.name));
                            break;
                        }
                    }
                }

                if (PackageManagerEditorHelper.HelpBoxWithButton(messageContent, buttonContent, MessageType.Warning))
                {
                    foreach (var info in m_ObjectFieldInfos)
                    {
                        if (info.canAssign)
                            info.Assign(m_IsMultiObjectEditing);
                    }
                }
            }

            // Replace
            if (numShowReplaceHelpBox > 0)
            {
                var buttonContent = m_IsMultiObjectEditing
                    ? EditorGUIUtility.TrTextContent(
                        string.Format(Contents.parentInteractableReplaceNumberedButton.text, numShowReplaceHelpBox.ToString()),
                        string.Format(Contents.parentInteractableReplaceNumberedButton.tooltip, numShowReplaceHelpBox.ToString()))
                    : Contents.parentInteractableReplaceButton;

                var messageContent = Contents.parentInteractableMismatchPrompt;

                // Use the prompt with the GameObject name instead when there's just one applicable component.
                if (numShowReplaceHelpBox == 1)
                {
                    foreach (var info in m_ObjectFieldInfos)
                    {
                        if (info.canReplace)
                        {
                            messageContent = EditorGUIUtility.TrTextContent(
                                string.Format(Contents.parentInteractableMismatchPromptWithName.text, info.hierarchyParentInteractable.transform.name));
                            break;
                        }
                    }
                }

                if (PackageManagerEditorHelper.HelpBoxWithButton(messageContent, buttonContent, MessageType.Warning))
                {
                    foreach (var info in m_ObjectFieldInfos)
                    {
                        if (info.canReplace)
                            info.Replace(m_IsMultiObjectEditing);
                    }
                }
            }

            if (anyParentInteractableObjectNull || m_AutoFindParentInteractableInHierarchy.hasMultipleDifferentValues)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_AutoFindParentInteractableInHierarchy, Contents.autoFindParentInteractableInHierarchy);
                }
            }
        }

        static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.autoFindParentInteractableInHierarchy"/>.</summary>
            public static readonly GUIContent autoFindParentInteractableInHierarchy =
                EditorGUIUtility.TrTextContent("Auto Find Parent Interactable", "Automatically find a parent interactable up the GameObject hierarchy when registered with the interaction manager. Disable to manually set the object reference for improved startup performance.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.parentInteractable"/>.</summary>
            public static readonly GUIContent parentInteractable =
                EditorGUIUtility.TrTextContent("Parent Interactable", "An optional reference to a parent interactable dependency for determining processing order of interactables.");

            /// <summary><see cref="GUIContent"/> for the help box prompt for assigning the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableAssignPromptWithName =
                EditorGUIUtility.TrTextContent("A parent GameObject \"{0}\" has an interactable component added. Consider assigning Parent Interactable with an object reference to it.");

            /// <summary><see cref="GUIContent"/> for the help box prompt for assigning the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableAssignPrompt =
                EditorGUIUtility.TrTextContent("A parent GameObject has an interactable component added. Consider assigning Parent Interactable with an object reference to it.");

            /// <summary><see cref="GUIContent"/> for the help box prompt for replacing the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableMismatchPromptWithName =
                EditorGUIUtility.TrTextContent("A parent GameObject \"{0}\" has an interactable component added but Parent Interactable is set to another component. This is supported but may not be what you intended. Consider replacing Parent Interactable with an object reference to the interactable component up the GameObject hierarchy.");

            /// <summary><see cref="GUIContent"/> for the help box prompt for replacing the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableMismatchPrompt =
                EditorGUIUtility.TrTextContent("A parent GameObject has an interactable component added but Parent Interactable is set to another component. This is supported but may not be what you intended. Consider replacing Parent Interactable with an object reference to the interactable component up the GameObject hierarchy.");

            /// <summary><see cref="GUIContent"/> for the help box button for assigning the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableAssignButton =
                EditorGUIUtility.TrTextContent("Assign", "Set Parent Interactable to the nearest interactable up the GameObject hierarchy.");

            /// <summary><see cref="GUIContent"/> for the help box button for assigning the Parent Interactable for a numbered applicable components.</summary>
            public static readonly GUIContent parentInteractableAssignNumberedButton =
                EditorGUIUtility.TrTextContent("Assign ({0})", "Set Parent Interactable to the nearest interactable up the GameObject hierarchy on {0} applicable components.");

            /// <summary><see cref="GUIContent"/> for the help box button for replacing the Parent Interactable.</summary>
            public static readonly GUIContent parentInteractableReplaceButton =
                EditorGUIUtility.TrTextContent("Replace", "Replace Parent Interactable reference to the nearest interactable up the GameObject hierarchy.");

            /// <summary><see cref="GUIContent"/> for the help box button for replacing the Parent Interactable for a numbered applicable components.</summary>
            public static readonly GUIContent parentInteractableReplaceNumberedButton =
                EditorGUIUtility.TrTextContent("Replace ({0})", "Replace Parent Interactable reference to the nearest interactable up the GameObject hierarchy on {0} applicable components.");
        }
    }
}
