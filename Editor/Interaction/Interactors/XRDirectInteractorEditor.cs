using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="XRDirectInteractor"/>.
    /// </summary>
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    [CustomEditor(typeof(XRDirectInteractor), true), CanEditMultipleObjects]
    public class XRDirectInteractorEditor : XRBaseInputInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDirectInteractor.improveAccuracyWithSphereCollider"/>.</summary>
        protected SerializedProperty m_ImproveAccuracyWithSphereCollider;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDirectInteractor.physicsLayerMask"/>.</summary>
        protected SerializedProperty m_PhysicsLayerMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRDirectInteractor.physicsTriggerInteraction"/>.</summary>
        protected SerializedProperty m_PhysicsTriggerInteraction;

        /// <summary><see cref="GUIContent"/> for <see cref="XRDirectInteractor.improveAccuracyWithSphereCollider"/>.</summary>
        public static readonly GUIContent improveAccuracyWithSphereCollider = EditorGUIUtility.TrTextContent("Improve Accuracy With Sphere Collider", "Generates contacts using optimized sphere cast calls every frame instead of relying on contact events on Fixed Update. Disable to force the use of trigger events.");
        /// <summary><see cref="GUIContent"/> for <see cref="XRDirectInteractor.physicsLayerMask"/>.</summary>
        public static readonly GUIContent physicsLayerMask = EditorGUIUtility.TrTextContent("Physics Layer Mask", "Physics layer mask used for limiting direct interactor overlaps when using the Improve Accuracy With Sphere Collider option.");
        /// <summary><see cref="GUIContent"/> for <see cref="XRDirectInteractor.physicsTriggerInteraction"/>.</summary>
        public static readonly GUIContent physicsTriggerInteraction = EditorGUIUtility.TrTextContent("Physics Trigger Interaction", "Determines whether the direct interactor sphere overlap will hit triggers when using the Improve Accuracy With Sphere Collider option.");

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            m_ImproveAccuracyWithSphereCollider = serializedObject.FindProperty("m_ImproveAccuracyWithSphereCollider");
            m_PhysicsLayerMask = serializedObject.FindProperty("m_PhysicsLayerMask");
            m_PhysicsTriggerInteraction = serializedObject.FindProperty("m_PhysicsTriggerInteraction");
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            // Not calling base method to completely override drawn properties

            DrawInteractionManagement();
            DrawInteractionConfiguration();
            DrawSphereColliderConfiguration();

            EditorGUILayout.Space();

            DrawSelectionConfiguration();

            EditorGUILayout.Space();

            DrawInputConfiguration();
        }

        /// <summary>
        /// Draw the property fields related to interaction configuration.
        /// </summary>
        protected virtual void DrawInteractionConfiguration()
        {
            EditorGUILayout.PropertyField(m_Handedness, BaseContents.handedness);
            EditorGUILayout.PropertyField(m_AttachTransform, BaseContents.attachTransform);
            EditorGUILayout.PropertyField(m_DisableVisualsWhenBlockedInGroup, BaseContents.disableVisualsWhenBlockedInGroup);
        }

        /// <summary>
        /// Draw property fields related to Sphere Collider accuracy and casting configuration.
        /// </summary>
        protected virtual void DrawSphereColliderConfiguration()
        {
            EditorGUILayout.PropertyField(m_ImproveAccuracyWithSphereCollider, improveAccuracyWithSphereCollider);
            if (m_ImproveAccuracyWithSphereCollider.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_PhysicsLayerMask, physicsLayerMask);
                    EditorGUILayout.PropertyField(m_PhysicsTriggerInteraction, physicsTriggerInteraction);
                }
            }
        }

        /// <summary>
        /// Draw the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfigurationNested"/>
        protected virtual void DrawSelectionConfiguration()
        {
            m_SelectActionTrigger.isExpanded = EditorGUILayout.Foldout(m_SelectActionTrigger.isExpanded, EditorGUIUtility.TrTempContent("Selection Configuration"), true);
            if (m_SelectActionTrigger.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawSelectionConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfiguration"/>
        protected virtual void DrawSelectionConfigurationNested()
        {
            DrawSelectActionTrigger();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, BaseContents.keepSelectedTargetValid);
            EditorGUILayout.PropertyField(m_AllowHoveredActivate, BaseInputContents.allowHoveredActivate);
            EditorGUILayout.PropertyField(m_TargetPriorityMode, BaseInputContents.targetPriorityMode);
            EditorGUILayout.PropertyField(m_StartingSelectedInteractable, BaseContents.startingSelectedInteractable);
        }
    }
}
