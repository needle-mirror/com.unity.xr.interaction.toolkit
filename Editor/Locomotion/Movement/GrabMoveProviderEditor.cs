using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Movement
{
    /// <summary>
    /// Custom editor for a <see cref="GrabMoveProvider"/>.
    /// </summary>
    [CustomEditor(typeof(GrabMoveProvider), true), CanEditMultipleObjects]
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    public partial class GrabMoveProviderEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.mediator"/>.</summary>
        protected SerializedProperty m_Mediator;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.transformationPriority"/>.</summary>
        protected SerializedProperty m_TransformationPriority;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.enableFreeXMovement"/>.</summary>
        protected SerializedProperty m_EnableFreeXMovement;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.enableFreeYMovement"/>.</summary>
        protected SerializedProperty m_EnableFreeYMovement;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.enableFreeZMovement"/>.</summary>
        protected SerializedProperty m_EnableFreeZMovement;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabMoveProvider.controllerTransform"/>.</summary>
        protected SerializedProperty m_ControllerTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabMoveProvider.enableMoveWhileSelecting"/>.</summary>
        protected SerializedProperty m_EnableMoveWhileSelecting;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabMoveProvider.moveFactor"/>.</summary>
        protected SerializedProperty m_MoveFactor;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabMoveProvider.grabMoveInput"/>.</summary>
        protected SerializedProperty m_GrabMoveInput;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static partial class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.mediator"/>.</summary>
            public static readonly GUIContent mediator = EditorGUIUtility.TrTextContent("Mediator", "The locomotion mediator that the grab move provider will interface with.");
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.transformationPriority"/>.</summary>
            public static readonly GUIContent transformationPriority = EditorGUIUtility.TrTextContent("Transformation Priority", "The queue order of this provider's transformations of the XR Origin. The lower the value, the earlier the transformations are applied.");

            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.enableFreeXMovement"/>.</summary>
            public static readonly GUIContent enableFreeXMovement = EditorGUIUtility.TrTextContent("Enable Free X Movement", "Controls whether to enable unconstrained movement along the x-axis.");
            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.enableFreeYMovement"/>.</summary>
            public static readonly GUIContent enableFreeYMovement = EditorGUIUtility.TrTextContent("Enable Free Y Movement", "Controls whether to enable unconstrained movement along the y-axis.");
            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.enableFreeZMovement"/>.</summary>
            public static readonly GUIContent enableFreeZMovement = EditorGUIUtility.TrTextContent("Enable Free Z Movement", "Controls whether to enable unconstrained movement along the z-axis.");

            /// <summary><see cref="GUIContent"/> for <see cref="GrabMoveProvider.controllerTransform"/>.</summary>
            public static readonly GUIContent controllerTransform = EditorGUIUtility.TrTextContent("Controller Transform", "The controller Transform that will drive grab movement with its local position. Will use this GameObject's Transform if not set.");
            /// <summary><see cref="GUIContent"/> for <see cref="GrabMoveProvider.enableMoveWhileSelecting"/>.</summary>
            public static readonly GUIContent enableMoveWhileSelecting = EditorGUIUtility.TrTextContent("Enable Move While Selecting", "Controls whether to allow grab move locomotion while the controller is selecting an interactable.");
            /// <summary><see cref="GUIContent"/> for <see cref="GrabMoveProvider.moveFactor"/>.</summary>
            public static readonly GUIContent moveFactor = EditorGUIUtility.TrTextContent("Move Factor", "The ratio of actual movement distance to controller movement distance.");
            /// <summary><see cref="GUIContent"/> for <see cref="GrabMoveProvider.grabMoveInput"/>.</summary>
            public static readonly GUIContent grabMoveInput = EditorGUIUtility.TrTextContent("Grab Move Input", "Input data that will be used to perform grab movement while held.");
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_Mediator = serializedObject.FindProperty("m_Mediator");
            m_TransformationPriority = serializedObject.FindProperty("m_TransformationPriority");

            m_EnableFreeXMovement = serializedObject.FindProperty("m_EnableFreeXMovement");
            m_EnableFreeYMovement = serializedObject.FindProperty("m_EnableFreeYMovement");
            m_EnableFreeZMovement = serializedObject.FindProperty("m_EnableFreeZMovement");

            m_ControllerTransform = serializedObject.FindProperty("m_ControllerTransform");
            m_EnableMoveWhileSelecting = serializedObject.FindProperty("m_EnableMoveWhileSelecting");
            m_MoveFactor = serializedObject.FindProperty("m_MoveFactor");
            m_GrabMoveInput = serializedObject.FindProperty("m_GrabMoveInput");
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        protected override void DrawInspector()
        {
            DrawBeforeProperties();
            DrawProperties();
            DrawDerivedProperties();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the section of the custom inspector before <see cref="DrawProperties"/>.
        /// By default, this draws the read-only Script property.
        /// </summary>
        protected virtual void DrawBeforeProperties()
        {
            DrawScript();
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
            EditorGUILayout.PropertyField(m_Mediator, Contents.mediator);
            EditorGUILayout.PropertyField(m_TransformationPriority, Contents.transformationPriority);

            EditorGUILayout.PropertyField(m_EnableFreeXMovement, Contents.enableFreeXMovement);
            EditorGUILayout.PropertyField(m_EnableFreeYMovement, Contents.enableFreeYMovement);
            EditorGUILayout.PropertyField(m_EnableFreeZMovement, Contents.enableFreeZMovement);

            EditorGUILayout.PropertyField(m_ControllerTransform, Contents.controllerTransform);
            EditorGUILayout.PropertyField(m_EnableMoveWhileSelecting, Contents.enableMoveWhileSelecting);
            EditorGUILayout.PropertyField(m_MoveFactor, Contents.moveFactor);
            EditorGUILayout.PropertyField(m_GrabMoveInput, Contents.grabMoveInput);
        }
    }
}
