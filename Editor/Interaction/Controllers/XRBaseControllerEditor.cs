using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="XRBaseController"/>.
    /// </summary>
    [CustomEditor(typeof(XRBaseController), true), CanEditMultipleObjects]
    [Obsolete(k_XRBaseControllerEditorDeprecated, true)]
    public class XRBaseControllerEditor : BaseInteractionEditor
    {
        const string k_XRBaseControllerEditorDeprecated = "XRBaseControllerEditor has been deprecated in version 3.0.0.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.updateTrackingType"/>.</summary>
        protected SerializedProperty m_UpdateTrackingType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.enableInputTracking"/>.</summary>
        protected SerializedProperty m_EnableInputTracking;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.enableInputActions"/>.</summary>
        protected SerializedProperty m_EnableInputActions;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelPrefab"/>.</summary>
        protected SerializedProperty m_ModelPrefab;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelParent"/>.</summary>
        protected SerializedProperty m_ModelParent;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.model"/>.</summary>
        protected SerializedProperty m_Model;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.animateModel"/>.</summary>
        protected SerializedProperty m_AnimateModel;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelSelectTransition"/>.</summary>
        protected SerializedProperty m_ModelSelectTransition;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseController.modelDeSelectTransition"/>.</summary>
        protected SerializedProperty m_ModelDeSelectTransition;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        protected static class BaseContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.updateTrackingType"/>.</summary>
            public static GUIContent updateTrackingType = EditorGUIUtility.TrTextContent("Update Tracking Type", "The time within the frame that the controller will sample tracking input.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.enableInputTracking"/>.</summary>
            public static GUIContent enableInputTracking = EditorGUIUtility.TrTextContent("Enable Input Tracking", "Whether input pose tracking is enabled for this controller. When enabled, the current tracking pose input of the controller device will be read each frame.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.enableInputActions"/>.</summary>
            public static GUIContent enableInputActions = EditorGUIUtility.TrTextContent("Enable Input Actions", "Whether input for XR Interaction events is enabled for this controller. When enabled, the current input of the controller device will be read each frame.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelPrefab"/>.</summary>
            public static GUIContent modelPrefab = EditorGUIUtility.TrTextContent("Model Prefab", "The prefab of a controller model to show for this controller that will be automatically instantiated by this behavior.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelParent"/>.</summary>
            public static GUIContent modelParent = EditorGUIUtility.TrTextContent("Model Parent", "The transform that is used as the parent for the model prefab when it is instantiated. Will be set to a new child GameObject if None.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.model"/>.</summary>
            public static GUIContent model = EditorGUIUtility.TrTextContent("Model", "The instance of the controller model in the scene. This can be set to an existing object instead of using Model Prefab.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.animateModel"/>.</summary>
            public static GUIContent animateModel = EditorGUIUtility.TrTextContent("Animate Model", "Whether to animate the model in response to interaction events. When enabled, activates a named animation trigger upon selecting or deselecting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelSelectTransition"/>.</summary>
            public static GUIContent modelSelectTransition = EditorGUIUtility.TrTextContent("Model Select Transition", "The animation trigger name to activate upon selecting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseController.modelDeSelectTransition"/>.</summary>
            public static GUIContent modelDeSelectTransition = EditorGUIUtility.TrTextContent("Model Deselect Transition", "The animation trigger name to activate upon deselecting.");

            /// <summary><see cref="GUIContent"/> for the Tracking header label.</summary>
            public static readonly GUIContent trackingHeader = EditorGUIUtility.TrTextContent("Tracking");
            /// <summary><see cref="GUIContent"/> for the Input header label.</summary>
            public static readonly GUIContent inputHeader = EditorGUIUtility.TrTextContent("Input");
            /// <summary><see cref="GUIContent"/> for the Model header label.</summary>
            public static readonly GUIContent modelHeader = EditorGUIUtility.TrTextContent("Model");

            /// <summary>The help box message when Model Prefab and Model are both set.</summary>
            public static readonly GUIContent modelPrefabIgnored = EditorGUIUtility.TrTextContent("Model Prefab will be ignored and not instantiated since Model is already set.");
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        /// <seealso cref="MonoBehaviour"/>
        protected virtual void OnEnable()
        {
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        protected override void DrawInspector()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the section of the custom inspector before <see cref="DrawProperties"/>.
        /// By default, this draws the read-only Script property.
        /// </summary>
        protected virtual void DrawBeforeProperties()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
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
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }

        /// <summary>
        /// Draw property fields related to tracking.
        /// These are related to <see cref="XRBaseController.enableInputTracking"/>.
        /// </summary>
        protected virtual void DrawTrackingConfiguration()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }

        /// <summary>
        /// Draw property fields related to interaction input.
        /// These are related to <see cref="XRBaseController.enableInputActions"/>.
        /// </summary>
        protected virtual void DrawInputConfiguration()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }

        /// <summary>
        /// Draw property fields related to other, specialized input actions and haptic output.
        /// </summary>
        protected virtual void DrawOtherActions()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }

        /// <summary>
        /// Draw property fields related to the controller model.
        /// </summary>
        protected virtual void DrawModelProperties()
        {
            throw new NotSupportedException(k_XRBaseControllerEditorDeprecated);
        }
    }
}
