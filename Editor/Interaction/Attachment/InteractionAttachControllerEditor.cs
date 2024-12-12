using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;

namespace UnityEditor.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Custom editor for an <see cref="InteractionAttachController"/>.
    /// </summary>
    [CustomEditor(typeof(InteractionAttachController), true), CanEditMultipleObjects]
    public class InteractionAttachControllerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.transformToFollow"/>.</summary>
        protected SerializedProperty m_TransformToFollow;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.motionStabilizationMode"/>.</summary>
        protected SerializedProperty m_MotionStabilizationMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.positionStabilization"/>.</summary>
        protected SerializedProperty m_PositionStabilization;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.angleStabilization"/>.</summary>
        protected SerializedProperty m_AngleStabilization;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.smoothOffset"/>.</summary>
        protected SerializedProperty m_SmoothOffset;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.smoothingSpeed"/>.</summary>
        protected SerializedProperty m_SmoothingSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.useDistanceBasedVelocityScaling"/>.</summary>
        protected SerializedProperty m_UseDistanceBasedVelocityScaling;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.useMomentum"/>.</summary>
        protected SerializedProperty m_UseMomentum;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.momentumDecayScale"/>.</summary>
        protected SerializedProperty m_MomentumDecayScale;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.momentumDecayScaleFromInput"/>.</summary>
        protected SerializedProperty m_MomentumDecayScaleFromInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.zVelocityRampThreshold"/>.</summary>
        protected SerializedProperty m_ZVelocityRampThreshold;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.pullVelocityBias"/>.</summary>
        protected SerializedProperty m_PullVelocityBias;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.pushVelocityBias"/>.</summary>
        protected SerializedProperty m_PushVelocityBias;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.minAdditionalVelocityScalar"/>.</summary>
        protected SerializedProperty m_MinAdditionalVelocityScalar;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.maxAdditionalVelocityScalar"/>.</summary>
        protected SerializedProperty m_MaxAdditionalVelocityScalar;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.useManipulationInput"/>.</summary>
        protected SerializedProperty m_UseManipulationInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationInput"/>.</summary>
        protected SerializedProperty m_ManipulationInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationXAxisMode"/>.</summary>
        protected SerializedProperty m_ManipulationXAxisMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationYAxisMode"/>.</summary>
        protected SerializedProperty m_ManipulationYAxisMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.combineManipulationAxes"/>.</summary>
        protected SerializedProperty m_CombineManipulationAxes;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationTranslateSpeed"/>.</summary>
        protected SerializedProperty m_ManipulationTranslateSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationRotateSpeed"/>.</summary>
        protected SerializedProperty m_ManipulationRotateSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.manipulationRotateReferenceFrame"/>.</summary>
        protected SerializedProperty m_ManipulationRotateReferenceFrame;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="InteractionAttachController.enableDebugLines"/>.</summary>
        protected SerializedProperty m_EnableDebugLines;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.transformToFollow"/>.</summary>
            public static readonly GUIContent transformToFollow = EditorGUIUtility.TrTextContent("Transform To Follow", "The transform that this anchor should follow.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.motionStabilizationMode"/>.</summary>
            public static readonly GUIContent motionStabilizationMode = EditorGUIUtility.TrTextContent("Motion Stabilization Mode", "The stabilization mode for the motion of the anchor. Determines how the anchor's position and rotation are stabilized relative to the followed transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.positionStabilization"/>.</summary>
            public static readonly GUIContent positionStabilization = EditorGUIUtility.TrTextContent("Position Stabilization", "Factor for stabilizing position. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.angleStabilization"/>.</summary>
            public static readonly GUIContent angleStabilization = EditorGUIUtility.TrTextContent("Angle Stabilization", "Factor for stabilizing angle. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.smoothOffset"/>.</summary>
            public static readonly GUIContent smoothOffset = EditorGUIUtility.TrTextContent("Smooth Offset", "If true offset will be smoothed over time in XR Origin space.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.smoothingSpeed"/>.</summary>
            public static readonly GUIContent smoothingSpeed = EditorGUIUtility.TrTextContent("Smoothing Speed", "Smoothing speed for the offset anchor child.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.useDistanceBasedVelocityScaling"/>.</summary>
            public static readonly GUIContent useDistanceBasedVelocityScaling = EditorGUIUtility.TrTextContent("Use Distance Based Velocity Scaling", "Whether to use distance-based velocity scaling for anchor movement.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.useMomentum"/>.</summary>
            public static readonly GUIContent useMomentum = EditorGUIUtility.TrTextContent("Use Momentum", "Whether momentum is used when distance scaling is in effect.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.momentumDecayScale"/>.</summary>
            public static readonly GUIContent momentumDecayScale = EditorGUIUtility.TrTextContent("Decay: Physical Movement", "Decay scalar for momentum when triggered with push/pull gesture. Higher values will cause momentum to decay faster.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.momentumDecayScaleFromInput"/>.</summary>
            public static readonly GUIContent momentumDecayScaleFromInput = EditorGUIUtility.TrTextContent("Decay: Input", "Decay scalar for momentum when triggered with manipulation input. Higher values will cause momentum to decay faster.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.zVelocityRampThreshold"/>.</summary>
            public static readonly GUIContent zVelocityRampThreshold = EditorGUIUtility.TrTextContent("Z Velocity Ramp Threshold", "Scales anchor velocity from 0 to 1 based on z-velocity's deviation below a threshold. 0 means no scaling.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.pullVelocityBias"/>.</summary>
            public static readonly GUIContent pullVelocityBias = EditorGUIUtility.TrTextContent("Pull Velocity Bias", "Adjusts the object's velocity calculation when moving towards the user. It modifies the distance-based calculation that determines the velocity scalar.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.pushVelocityBias"/>.</summary>
            public static readonly GUIContent pushVelocityBias = EditorGUIUtility.TrTextContent("Push Velocity Bias", "Adjusts the object's velocity calculation when moving away from the user. It modifies the distance-based calculation that determines the velocity scalar.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.minAdditionalVelocityScalar"/>.</summary>
            public static readonly GUIContent minAdditionalVelocityScalar = EditorGUIUtility.TrTextContent("Min Additional Velocity Scalar", "Minimum additional velocity scaling factor for movement, interpolated by a quad bezier curve.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.maxAdditionalVelocityScalar"/>.</summary>
            public static readonly GUIContent maxAdditionalVelocityScalar = EditorGUIUtility.TrTextContent("Max Additional Velocity Scalar", "Maximum additional velocity scaling factor for movement, interpolated by a quad bezier curve.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.useManipulationInput"/>.</summary>
            public static readonly GUIContent useManipulationInput = EditorGUIUtility.TrTextContent("Use Manipulation Input", "Whether to use manipulation input to move the anchor.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationInput"/>.</summary>
            public static readonly GUIContent manipulationInput = EditorGUIUtility.TrTextContent("Manipulation Input", "The input used to manipulate the anchor.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationXAxisMode"/>.</summary>
            public static readonly GUIContent manipulationXAxisMode = EditorGUIUtility.TrTextContent("X-axis Mode", "What the x-axis (left/right) of the manipulation input does when controlling the anchor.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationYAxisMode"/>.</summary>
            public static readonly GUIContent manipulationYAxisMode = EditorGUIUtility.TrTextContent("Y-axis Mode", "What the y-axis (up/down) of the manipulation input does when controlling the anchor.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.combineManipulationAxes"/>.</summary>
            public static readonly GUIContent combineManipulationAxes = EditorGUIUtility.TrTextContent("Combine Manipulation Axes", "Whether to allow simultaneous manipulation of both axes. Disable to allow only one axis of manipulation input at a time based on which axis is most actuated.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationTranslateSpeed"/>.</summary>
            public static readonly GUIContent manipulationTranslateSpeed = EditorGUIUtility.TrTextContent("Translate Speed", "Speed at which the anchor is translated when using manipulation input.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationRotateSpeed"/>.</summary>
            public static readonly GUIContent manipulationRotateSpeed = EditorGUIUtility.TrTextContent("Rotate Speed", "Speed at which the anchor is rotated when using manipulation input.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.manipulationRotateReferenceFrame"/>.</summary>
            public static readonly GUIContent manipulationRotateReferenceFrame = EditorGUIUtility.TrTextContent("Rotate Reference Frame", "The optional reference frame to define the rotation axes when the anchor is rotated when using manipulation input.");
            /// <summary><see cref="GUIContent"/> for <see cref="InteractionAttachController.enableDebugLines"/>.</summary>
            public static readonly GUIContent enableDebugLines = EditorGUIUtility.TrTextContent("Enable Debug Lines", "Enable debug lines for the attach transform offset and velocity vector.");

            /// <summary><see cref="GUIContent"/> for the header of the stabilization parameters section.</summary>
            public static readonly GUIContent stabilizationParametersHeader = EditorGUIUtility.TrTextContent("Stabilization Parameters");
            /// <summary><see cref="GUIContent"/> for the header of the smoothing settings section.</summary>
            public static readonly GUIContent smoothingSettingsHeader = EditorGUIUtility.TrTextContent("Smoothing Settings");
            /// <summary><see cref="GUIContent"/> for the header of the anchor movement section.</summary>
            public static readonly GUIContent anchorMovementHeader = EditorGUIUtility.TrTextContent("Anchor Movement");
            /// <summary><see cref="GUIContent"/> for the header of the anchor movement parameters section.</summary>
            public static readonly GUIContent anchorMovementParametersHeader = EditorGUIUtility.TrTextContent("Anchor Movement Parameters");
            /// <summary><see cref="GUIContent"/> for the header of the debug section.</summary>
            public static readonly GUIContent debugConfigurationHeader = EditorGUIUtility.TrTextContent("Debug Configuration");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_TransformToFollow = serializedObject.FindProperty("m_TransformToFollow");
            m_MotionStabilizationMode = serializedObject.FindProperty("m_MotionStabilizationMode");
            m_PositionStabilization = serializedObject.FindProperty("m_PositionStabilization");
            m_AngleStabilization = serializedObject.FindProperty("m_AngleStabilization");
            m_SmoothOffset = serializedObject.FindProperty("m_SmoothOffset");
            m_SmoothingSpeed = serializedObject.FindProperty("m_SmoothingSpeed");
            m_UseDistanceBasedVelocityScaling = serializedObject.FindProperty("m_UseDistanceBasedVelocityScaling");
            m_UseMomentum = serializedObject.FindProperty("m_UseMomentum");
            m_MomentumDecayScale = serializedObject.FindProperty("m_MomentumDecayScale");
            m_MomentumDecayScaleFromInput = serializedObject.FindProperty("m_MomentumDecayScaleFromInput");
            m_ZVelocityRampThreshold = serializedObject.FindProperty("m_ZVelocityRampThreshold");
            m_PullVelocityBias = serializedObject.FindProperty("m_PullVelocityBias");
            m_PushVelocityBias = serializedObject.FindProperty("m_PushVelocityBias");
            m_MinAdditionalVelocityScalar = serializedObject.FindProperty("m_MinAdditionalVelocityScalar");
            m_MaxAdditionalVelocityScalar = serializedObject.FindProperty("m_MaxAdditionalVelocityScalar");
            m_UseManipulationInput = serializedObject.FindProperty("m_UseManipulationInput");
            m_ManipulationInput = serializedObject.FindProperty("m_ManipulationInput");
            m_ManipulationXAxisMode = serializedObject.FindProperty("m_ManipulationXAxisMode");
            m_ManipulationYAxisMode = serializedObject.FindProperty("m_ManipulationYAxisMode");
            m_CombineManipulationAxes = serializedObject.FindProperty("m_CombineManipulationAxes");
            m_ManipulationTranslateSpeed = serializedObject.FindProperty("m_ManipulationTranslateSpeed");
            m_ManipulationRotateSpeed = serializedObject.FindProperty("m_ManipulationRotateSpeed");
            m_ManipulationRotateReferenceFrame = serializedObject.FindProperty("m_ManipulationRotateReferenceFrame");
            m_EnableDebugLines = serializedObject.FindProperty("m_EnableDebugLines");
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
            EditorGUILayout.PropertyField(m_TransformToFollow, Contents.transformToFollow);

            // Stabilization Parameters
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.stabilizationParametersHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_MotionStabilizationMode, Contents.motionStabilizationMode);
            if (m_MotionStabilizationMode.intValue != (int)MotionStabilizationMode.Never)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_PositionStabilization, Contents.positionStabilization);
                    EditorGUILayout.PropertyField(m_AngleStabilization, Contents.angleStabilization);
                }
            }

            // Smoothing Settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.smoothingSettingsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_SmoothOffset, Contents.smoothOffset);
            if (m_SmoothOffset.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_SmoothingSpeed, Contents.smoothingSpeed);
                }
            }

            // Anchor Movement
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.anchorMovementHeader, EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_UseDistanceBasedVelocityScaling, Contents.useDistanceBasedVelocityScaling);
                EditorGUILayout.PropertyField(m_UseManipulationInput, Contents.useManipulationInput);
                if (m_UseManipulationInput.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_ManipulationInput, Contents.manipulationInput);
                        EditorGUILayout.PropertyField(m_ManipulationXAxisMode, Contents.manipulationXAxisMode);
                        EditorGUILayout.PropertyField(m_ManipulationYAxisMode, Contents.manipulationYAxisMode);
                        EditorGUILayout.PropertyField(m_CombineManipulationAxes, Contents.combineManipulationAxes);

                        if ((InteractionAttachController.ManipulationYAxisMode)m_ManipulationYAxisMode.intValue == InteractionAttachController.ManipulationYAxisMode.Translate)
                        {
                            EditorGUILayout.PropertyField(m_ManipulationTranslateSpeed, Contents.manipulationTranslateSpeed);
                        }

                        if ((InteractionAttachController.ManipulationXAxisMode)m_ManipulationXAxisMode.intValue == InteractionAttachController.ManipulationXAxisMode.HorizontalRotation ||
                            (InteractionAttachController.ManipulationYAxisMode)m_ManipulationYAxisMode.intValue == InteractionAttachController.ManipulationYAxisMode.VerticalRotation)
                        {
                            EditorGUILayout.PropertyField(m_ManipulationRotateSpeed, Contents.manipulationRotateSpeed);
                            EditorGUILayout.PropertyField(m_ManipulationRotateReferenceFrame, Contents.manipulationRotateReferenceFrame);
                        }
                    }
                }

                // Anchor Movement Parameters
                if (m_UseDistanceBasedVelocityScaling.boolValue || m_UseManipulationInput.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(Contents.anchorMovementParametersHeader, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_UseMomentum, Contents.useMomentum);
                    if (m_UseMomentum.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            using (new EditorGUI.DisabledScope(!m_UseDistanceBasedVelocityScaling.boolValue))
                            {
                                EditorGUILayout.PropertyField(m_MomentumDecayScale, Contents.momentumDecayScale);
                            }

                            using (new EditorGUI.DisabledScope(!m_UseManipulationInput.boolValue))
                            {
                                EditorGUILayout.PropertyField(m_MomentumDecayScaleFromInput, Contents.momentumDecayScaleFromInput);
                            }
                        }
                    }

                    EditorGUILayout.PropertyField(m_ZVelocityRampThreshold, Contents.zVelocityRampThreshold);
                    EditorGUILayout.PropertyField(m_PullVelocityBias, Contents.pullVelocityBias);
                    EditorGUILayout.PropertyField(m_PushVelocityBias, Contents.pushVelocityBias);
                    EditorGUILayout.PropertyField(m_MinAdditionalVelocityScalar, Contents.minAdditionalVelocityScalar);
                    EditorGUILayout.PropertyField(m_MaxAdditionalVelocityScalar, Contents.maxAdditionalVelocityScalar);
                }
            }

            // Debug Configuration
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.debugConfigurationHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_EnableDebugLines, Contents.enableDebugLines);
        }
    }
}
