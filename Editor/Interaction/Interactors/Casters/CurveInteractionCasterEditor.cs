using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// Custom editor for a <see cref="CurveInteractionCaster"/>.
    /// </summary>
    [CustomEditor(typeof(CurveInteractionCaster), true), CanEditMultipleObjects]
    class CurveInteractionCasterEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.castOrigin"/>.</summary>
        protected SerializedProperty m_CastOrigin;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.enableStabilization"/>.</summary>
        protected SerializedProperty m_EnableStabilization;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.positionStabilization"/>.</summary>
        protected SerializedProperty m_PositionStabilization;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.angleStabilization"/>.</summary>
        protected SerializedProperty m_AngleStabilization;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.aimTarget"/>.</summary>
        protected SerializedProperty m_AimTargetObject;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.raycastMask"/>.</summary>
        protected SerializedProperty m_RaycastMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.raycastTriggerInteraction"/>.</summary>
        protected SerializedProperty m_RaycastTriggerInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.raycastSnapVolumeInteraction"/>.</summary>
        protected SerializedProperty m_RaycastSnapVolumeInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.targetNumCurveSegments"/>.</summary>
        protected SerializedProperty m_TargetNumCurveSegments;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.hitDetectionType"/>.</summary>
        protected SerializedProperty m_HitDetectionType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.castDistance"/>.</summary>
        protected SerializedProperty m_CastDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.sphereCastRadius"/>.</summary>
        protected SerializedProperty m_SphereCastRadius;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.coneCastAngle"/>.</summary>
        protected SerializedProperty m_ConeCastAngle;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveInteractionCaster.liveConeCastDebugVisuals"/>.</summary>
        protected SerializedProperty m_LiveConeCastDebugVisuals;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.castOrigin"/>.</summary>
            public static readonly GUIContent castOrigin = EditorGUIUtility.TrTextContent("Cast Origin", "Source of origin and direction used when updating sample points.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.enableStabilization"/>.</summary>
            public static readonly GUIContent enableStabilization = EditorGUIUtility.TrTextContent("Enable Stabilization", "Determines whether to stabilize the cast origin.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.positionStabilization"/>.</summary>
            public static readonly GUIContent positionStabilization = EditorGUIUtility.TrTextContent("Position Stabilization", "Factor for stabilizing position. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.angleStabilization"/>.</summary>
            public static readonly GUIContent angleStabilization = EditorGUIUtility.TrTextContent("Angle Stabilization", "Factor for stabilizing angle. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.aimTarget"/>.</summary>
            public static readonly GUIContent aimTargetObject = EditorGUIUtility.TrTextContent("Aim Target Object", "Optional ray provider for calculating stable rotation.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.raycastMask"/>.</summary>
            public static readonly GUIContent raycastMask = EditorGUIUtility.TrTextContent("Raycast Mask", "Gets or sets layer mask used for limiting ray cast targets.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.raycastTriggerInteraction"/>.</summary>
            public static readonly GUIContent raycastTriggerInteraction = EditorGUIUtility.TrTextContent("Raycast Trigger Interaction", "Gets or sets type of interaction with trigger colliders via ray cast.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.raycastSnapVolumeInteraction"/>.</summary>
            public static readonly GUIContent raycastSnapVolumeInteraction = EditorGUIUtility.TrTextContent("Raycast Snap Volume Interaction", "Determines if ray casts include snap volume triggers: 'Collide' to include, 'Ignore' for performance optimization when not using specific XR components.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.targetNumCurveSegments"/>.</summary>
            public static readonly GUIContent targetNumCurveSegments = EditorGUIUtility.TrTextContent("Target Num Curve Segments", "Number of segments to sample along the curve.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.hitDetectionType"/>.</summary>
            public static readonly GUIContent hitDetectionType = EditorGUIUtility.TrTextContent("Hit Detection Type", "Type of hit detection to use for the ray cast");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.castDistance"/>.</summary>
            public static readonly GUIContent castDistance = EditorGUIUtility.TrTextContent("Cast Distance", "Maximum distance for all physics casts.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.sphereCastRadius"/>.</summary>
            public static readonly GUIContent sphereCastRadius = EditorGUIUtility.TrTextContent("Sphere Cast Radius", "Radius used for sphere casting");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.coneCastAngle"/>.</summary>
            public static readonly GUIContent coneCastAngle = EditorGUIUtility.TrTextContent("Cone Cast Angle", "Angle in degrees of the cone used for cone casting. Will use regular ray casting if set to 0.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveInteractionCaster.liveConeCastDebugVisuals"/>.</summary>
            public static readonly GUIContent liveConeCastDebugVisuals = EditorGUIUtility.TrTextContent("Live Cone Cast Debug Visuals", "If enabled, more detailed cone cast gizmos will be displayed in the editor. Only displayed in Play mode when GameObject is selected.");

            /// <summary><see cref="GUIContent"/> for the Stabilization Settings foldout.</summary>
            public static readonly GUIContent filteringSettingsFoldout = EditorGUIUtility.TrTextContent("Filtering Settings", "Filtering configuration for this caster.");
            /// <summary><see cref="GUIContent"/> for the Stabilization Settings foldout.</summary>
            public static readonly GUIContent curveCastingSettingsFoldout = EditorGUIUtility.TrTextContent("Curve Casting Settings", "Curve Casting configuration for this caster.");
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected void OnEnable()
        {
            m_CastOrigin = serializedObject.FindProperty("m_CastOrigin");
            m_EnableStabilization = serializedObject.FindProperty("m_EnableStabilization");
            m_PositionStabilization = serializedObject.FindProperty("m_PositionStabilization");
            m_AngleStabilization = serializedObject.FindProperty("m_AngleStabilization");
            m_AimTargetObject = serializedObject.FindProperty("m_AimTargetObject");
            m_RaycastMask = serializedObject.FindProperty("m_RaycastMask");
            m_RaycastTriggerInteraction = serializedObject.FindProperty("m_RaycastTriggerInteraction");
            m_RaycastSnapVolumeInteraction = serializedObject.FindProperty("m_RaycastSnapVolumeInteraction");
            m_TargetNumCurveSegments = serializedObject.FindProperty("m_TargetNumCurveSegments");
            m_HitDetectionType = serializedObject.FindProperty("m_HitDetectionType");
            m_CastDistance = serializedObject.FindProperty("m_CastDistance");
            m_SphereCastRadius = serializedObject.FindProperty("m_SphereCastRadius");
            m_ConeCastAngle = serializedObject.FindProperty("m_ConeCastAngle");
            m_LiveConeCastDebugVisuals = serializedObject.FindProperty("m_LiveConeCastDebugVisuals");
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
            EditorGUILayout.PropertyField(m_CastOrigin, Contents.castOrigin);

            DrawStabilizationProperties();

            EditorGUILayout.Space();

            DrawFilteringProperties();
            DrawCurveCastingProperties();
        }

        /// <summary>
        /// Draw the property fields related to stabilization.
        /// </summary>
        protected virtual void DrawStabilizationProperties()
        {
            EditorGUILayout.PropertyField(m_EnableStabilization, Contents.enableStabilization);
            EditorGUILayout.PropertyField(m_PositionStabilization, Contents.positionStabilization);
            EditorGUILayout.PropertyField(m_AngleStabilization, Contents.angleStabilization);
            EditorGUILayout.PropertyField(m_AimTargetObject, Contents.aimTargetObject);
        }

        /// <summary>
        /// Draw the property fields related to filtering.
        /// </summary>
        protected virtual void DrawFilteringProperties()
        {
            m_RaycastMask.isExpanded = EditorGUILayout.Foldout(m_RaycastMask.isExpanded, Contents.filteringSettingsFoldout, true);
            if (m_RaycastMask.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_RaycastMask, Contents.raycastMask);
                    EditorGUILayout.PropertyField(m_RaycastTriggerInteraction, Contents.raycastTriggerInteraction);
                    EditorGUILayout.PropertyField(m_RaycastSnapVolumeInteraction, Contents.raycastSnapVolumeInteraction);
                }
            }
        }

        /// <summary>
        /// Draw the property fields related to curve casting.
        /// </summary>
        protected virtual void DrawCurveCastingProperties()
        {
            m_TargetNumCurveSegments.isExpanded = EditorGUILayout.Foldout(m_TargetNumCurveSegments.isExpanded, Contents.curveCastingSettingsFoldout, true);
            if (m_TargetNumCurveSegments.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_TargetNumCurveSegments, Contents.targetNumCurveSegments);
                    EditorGUILayout.PropertyField(m_CastDistance, Contents.castDistance);
                    EditorGUILayout.PropertyField(m_HitDetectionType, Contents.hitDetectionType);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        switch (m_HitDetectionType.intValue)
                        {
                            case (int)CurveInteractionCaster.HitDetectionType.SphereCast:
                                EditorGUILayout.PropertyField(m_SphereCastRadius, Contents.sphereCastRadius);
                                break;
                            case (int)CurveInteractionCaster.HitDetectionType.ConeCast:
                                EditorGUILayout.PropertyField(m_ConeCastAngle, Contents.coneCastAngle);
                                EditorGUILayout.PropertyField(m_LiveConeCastDebugVisuals, Contents.liveConeCastDebugVisuals);
                                break;
                        }
                    }
                }
            }
        }
    }
}
