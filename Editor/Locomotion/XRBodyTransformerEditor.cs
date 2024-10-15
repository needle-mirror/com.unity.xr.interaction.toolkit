using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Custom editor for an <see cref="XRBodyTransformer"/>.
    /// </summary>
    [CustomEditor(typeof(XRBodyTransformer), true)]
    public class XRBodyTransformerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBodyTransformer.xrOrigin"/>.</summary>
        protected SerializedProperty m_XROrigin;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBodyTransformer.bodyPositionEvaluator"/>.</summary>
        protected SerializedProperty m_BodyPositionEvaluatorObject;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBodyTransformer.constrainedBodyManipulator"/>.</summary>
        protected SerializedProperty m_ConstrainedBodyManipulatorObject;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBodyTransformer.useCharacterControllerIfExists"/>.</summary>
        protected SerializedProperty m_UseCharacterControllerIfExists;

        CharacterController m_CharacterController;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for settings related to <see cref="XRBodyTransformer.constrainedBodyManipulator"/>.</summary>
            public static readonly GUIContent constrainedMovementSettings = EditorGUIUtility.TrTextContent("Constrained Movement Settings");
            /// <summary><see cref="GUIContent"/> for optional <see cref="CharacterController"/> reference (Editor-only).</summary>
            public static readonly GUIContent characterController = EditorGUIUtility.TrTextContent("Character Controller", "The optional Character Controller component on the XR Origin's base GameObject. This is a read-only reference.");
            /// <summary><see cref="GUIContent"/> for the button to add a <see cref="CharacterController"/> to <see cref="XROrigin.Origin"/>.</summary>
            public static readonly GUIContent addCharacterController = EditorGUIUtility.TrTextContent("Add Character Controller", "Add a Character Controller component to the XR Origin's base GameObject. This is only enabled if a Constrained Manipulator is not already assigned.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBodyTransformer.useCharacterControllerIfExists"/>.</summary>
            public static readonly GUIContent useCharacterControllerIfExists = EditorGUIUtility.TrTextContent("Use Character Controller");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_XROrigin = serializedObject.FindProperty("m_XROrigin");
            m_BodyPositionEvaluatorObject = serializedObject.FindProperty("m_BodyPositionEvaluatorObject");
            m_ConstrainedBodyManipulatorObject = serializedObject.FindProperty("m_ConstrainedBodyManipulatorObject");
            m_UseCharacterControllerIfExists = serializedObject.FindProperty("m_UseCharacterControllerIfExists");

            UpdateCharacterControllerReference();
            Contents.useCharacterControllerIfExists.tooltip = m_UseCharacterControllerIfExists.tooltip;
        }

        /// <inheritdoc/>
        protected override void DrawInspector()
        {
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_XROrigin);
                if (changeCheck.changed)
                    UpdateCharacterControllerReference();
            }

            EditorGUILayout.PropertyField(m_BodyPositionEvaluatorObject);

            EditorGUILayout.LabelField(Contents.constrainedMovementSettings, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_ConstrainedBodyManipulatorObject);
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField(Contents.characterController, m_CharacterController, typeof(CharacterController), true);

            using (new EditorGUI.DisabledScope(m_ConstrainedBodyManipulatorObject.objectReferenceValue != null))
            {
                if (m_CharacterController == null)
                {
                    var xrOrigin = m_XROrigin.objectReferenceValue as XROrigin;
                    using (new EditorGUI.DisabledScope(xrOrigin == null))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(EditorGUI.indentLevel * 15);
                            if (GUILayout.Button(Contents.addCharacterController, EditorStyles.miniButton))
                            {
                                m_CharacterController = xrOrigin.Origin.AddComponent<CharacterController>();
                                Undo.RegisterCreatedObjectUndo(m_CharacterController, "Add Character Controller");
                                EditorGUIUtility.PingObject(m_CharacterController);
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(m_UseCharacterControllerIfExists, Contents.useCharacterControllerIfExists);
                }
            }

            EditorGUI.indentLevel--;
        }

        void UpdateCharacterControllerReference()
        {
            var xrOrigin = m_XROrigin.objectReferenceValue as XROrigin;
            m_CharacterController = xrOrigin != null ? xrOrigin.Origin.GetComponent<CharacterController>() : null;
        }
    }
}
