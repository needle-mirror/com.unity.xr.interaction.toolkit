using System;
using System.Collections.Generic;
using UnityEditor.XR.Interaction.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs
{
    abstract class XRBaseInputReaderPropertyDrawer<T> : PropertyDrawer where T : XRBaseInputReaderPropertyDrawer<T>.BaseSerializedPropertyFields, new()
    {
        public abstract class BaseSerializedPropertyFields
        {
            public abstract void FindProperties(SerializedProperty property);
        }

        protected readonly T m_Fields = new T();

        protected readonly CompactPropertyControl m_CompactPropertyControl = new CompactPropertyControl();
        protected readonly MultilinePropertyControl m_MultilinePropertyControl = new MultilinePropertyControl();

        readonly List<InputAction> m_EnabledInputActions = new List<InputAction>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            m_Fields.FindProperties(property);

            var drawerMode = XRInteractionEditorSettings.Instance.inputReaderPropertyDrawerMode;
            switch (drawerMode)
            {
                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.Compact:
                default:
                    PushCompactContext();
                    return m_CompactPropertyControl.GetPropertyHeight();

                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineEffective:
                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineAll:
                    var showEffectiveOnly = drawerMode == XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineEffective;
                    PushMultilineContext(showEffectiveOnly);
                    return m_MultilinePropertyControl.GetPropertyHeight();
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            m_Fields.FindProperties(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            label = EditorGUI.BeginProperty(position, label, property);

            var drawerMode = XRInteractionEditorSettings.Instance.inputReaderPropertyDrawerMode;
            switch (drawerMode)
            {
                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.Compact:
                default:
                    PushCompactContext();
                    m_CompactPropertyControl.propertyChanged += OnPropertyChanged;
                    m_CompactPropertyControl.DrawCompactGUI(position, label);
                    m_CompactPropertyControl.propertyChanged -= OnPropertyChanged;
                    break;

                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineEffective:
                case XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineAll:
                    var showEffectiveOnly = drawerMode == XRInteractionEditorSettings.InputReaderPropertyDrawerMode.MultilineEffective;
                    PushMultilineContext(showEffectiveOnly);
                    m_MultilinePropertyControl.propertyChanged += OnPropertyChanged;
                    m_MultilinePropertyControl.DrawMultilineGUI(position, label);
                    m_MultilinePropertyControl.propertyChanged -= OnPropertyChanged;
                    break;
            }

            EditorGUI.EndProperty();
        }

        protected bool IsActionNotNullAndDisabled(SerializedProperty inputAction, bool useCachedActionsStatus = false)
        {
            var idString = inputAction.FindPropertyRelative("m_Id").stringValue;
            if (Guid.TryParse(idString, out var id))
            {
                if (!useCachedActionsStatus)
                {
                    m_EnabledInputActions.Clear();
                    InputSystem.ListEnabledActions(m_EnabledInputActions);
                }

                for (var i = 0; i < m_EnabledInputActions.Count; ++i)
                {
                    if (m_EnabledInputActions[i].id == id)
                        return false;
                }

                return true;
            }

            return false;
        }

        protected bool IsActionReferenceNotNullAndDisabled(SerializedProperty inputActionReference)
        {
            var actionReference = inputActionReference.objectReferenceValue as InputActionReference;
            if (actionReference != null)
            {
                var action = actionReference.action;
                if (action != null)
                    return !action.enabled;
            }

            return false;
        }

        protected abstract void PushCompactContext();

        protected abstract void PushMultilineContext(bool showEffectiveOnly);

        protected virtual void OnPropertyChanged(SerializedProperty property)
        {
        }
    }
}
