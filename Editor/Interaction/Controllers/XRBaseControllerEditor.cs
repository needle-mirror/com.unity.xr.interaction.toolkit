using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="XRBaseController"/>.
    /// </summary>
    [CustomEditor(typeof(XRBaseController), true), CanEditMultipleObjects]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class XRBaseControllerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.updateTrackingType"/>.</summary>
        protected SerializedProperty m_UpdateTrackingType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.enableInputTracking"/>.</summary>
        protected SerializedProperty m_EnableInputTracking;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.enableInputActions"/>.</summary>
        protected SerializedProperty m_EnableInputActions;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelPrefab"/>.</summary>
        protected SerializedProperty m_ModelPrefab;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelTransform"/>.</summary>
        protected SerializedProperty m_ModelTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.animateModel"/>.</summary>
        protected SerializedProperty m_AnimateModel;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelSelectTransition"/>.</summary>
        protected SerializedProperty m_ModelSelectTransition;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelDeSelectTransition"/>.</summary>
        protected SerializedProperty m_ModelDeSelectTransition;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class BaseContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.updateTrackingType"/>.</summary>
            public static GUIContent updateTrackingType = EditorGUIUtility.TrTextContent("Update Tracking Type", "The time within the frame that the controller will sample input.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.enableInputTracking"/>.</summary>
            public static GUIContent enableInputTracking = EditorGUIUtility.TrTextContent("Enable Input Tracking", "Whether input tracking is enabled for this controller.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.enableInputActions"/>.</summary>
            public static GUIContent enableInputActions = EditorGUIUtility.TrTextContent("Enable Input Actions", "Used to disable an input state changing in the interactor. Useful for swapping to a different interactor on the same object.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelPrefab"/>.</summary>
            public static GUIContent modelPrefab = EditorGUIUtility.TrTextContent("Model Prefab", "The model prefab to show for this controller.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelTransform"/>.</summary>
            public static GUIContent modelTransform = EditorGUIUtility.TrTextContent("Model Transform", "The model transform that is used as the parent for the controller model.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.animateModel"/>.</summary>
            public static GUIContent animateModel = EditorGUIUtility.TrTextContent("Animate Model", "Whether this model animates in response to interaction events.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelSelectTransition"/>.</summary>
            public static GUIContent modelSelectTransition = EditorGUIUtility.TrTextContent("Model Select Transition", "The animation transition to enable when selecting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelDeSelectTransition"/>.</summary>
            public static GUIContent modelDeSelectTransition = EditorGUIUtility.TrTextContent("Model Deselect Transition", "The animation transition to enable when de-selecting.");

            /// <summary><see cref="GUIContent"/> for the Tracking header label.</summary>
            public static readonly GUIContent trackingHeader = EditorGUIUtility.TrTextContent("Tracking");
            /// <summary><see cref="GUIContent"/> for the Tracking header label.</summary>
            public static readonly GUIContent inputHeader = EditorGUIUtility.TrTextContent("Input");
            /// <summary><see cref="GUIContent"/> for the Model header label.</summary>
            public static readonly GUIContent modelHeader = EditorGUIUtility.TrTextContent("Model");
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        /// <seealso cref="MonoBehaviour"/>
        protected virtual void OnEnable()
        {
            m_UpdateTrackingType = serializedObject.FindProperty("m_UpdateTrackingType");
            m_EnableInputTracking = serializedObject.FindProperty("m_EnableInputTracking");
            m_EnableInputActions = serializedObject.FindProperty("m_EnableInputActions");
            m_ModelPrefab = serializedObject.FindProperty("m_ModelPrefab");
            m_ModelTransform = serializedObject.FindProperty("m_ModelTransform");
            m_AnimateModel = serializedObject.FindProperty("m_AnimateModel");
            m_ModelSelectTransition = serializedObject.FindProperty("m_ModelSelectTransition");
            m_ModelDeSelectTransition = serializedObject.FindProperty("m_ModelDeSelectTransition");
        }

        /// <inheritdoc />
        protected override List<string> GetDerivedSerializedPropertyNames()
        {
            var propertyNames = base.GetDerivedSerializedPropertyNames();
            // Ignore m_ButtonPressPoint since it is deprecated and planned to be removed when Input System 1.1 is released.
            // The expectation is if a user needs to modify it, they can do so through setting the property.
            propertyNames.Add("m_ButtonPressPoint");
            return propertyNames;
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
            DrawTrackingConfiguration();

            EditorGUILayout.Space();

            DrawInputConfiguration();

            EditorGUILayout.Space();

            DrawOtherActions();

            EditorGUILayout.Space();

            DrawModelProperties();
        }

        /// <summary>
        /// Draw property fields related to tracking.
        /// These are related to <see cref="XRBaseController.enableInputTracking"/>.
        /// </summary>
        protected virtual void DrawTrackingConfiguration()
        {
            EditorGUILayout.LabelField(BaseContents.trackingHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_UpdateTrackingType, BaseContents.updateTrackingType);
            EditorGUILayout.PropertyField(m_EnableInputTracking, BaseContents.enableInputTracking);
        }

        /// <summary>
        /// Draw property fields related to interaction input.
        /// These are related to <see cref="XRBaseController.enableInputActions"/>.
        /// </summary>
        protected virtual void DrawInputConfiguration()
        {
            EditorGUILayout.LabelField(BaseContents.inputHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_EnableInputActions, BaseContents.enableInputActions);
        }

        /// <summary>
        /// Draw property fields related to other, specialized input actions and haptic output.
        /// </summary>
        protected virtual void DrawOtherActions()
        {
        }

        /// <summary>
        /// Draw property fields related to the controller model.
        /// </summary>
        protected virtual void DrawModelProperties()
        {
            EditorGUILayout.LabelField(BaseContents.modelHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ModelPrefab, BaseContents.modelPrefab);
            EditorGUILayout.PropertyField(m_ModelTransform, BaseContents.modelTransform);
            EditorGUILayout.PropertyField(m_AnimateModel, BaseContents.animateModel);

            if (m_AnimateModel.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_ModelSelectTransition, BaseContents.modelSelectTransition);
                    EditorGUILayout.PropertyField(m_ModelDeSelectTransition, BaseContents.modelDeSelectTransition);
                }
            }
        }
    }
}
