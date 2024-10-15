using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors.Visuals
{
    /// <summary>
    /// Custom editor for a <see cref="CurveVisualController"/>.
    /// </summary>
    [CustomEditor(typeof(CurveVisualController), true), CanEditMultipleObjects]
    public class CurveVisualControllerEditor : BaseInteractionEditor
    {
        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.lineRenderer"/>.
        /// </summary>
        protected SerializedProperty m_LineRenderer;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.curveInteractionDataProvider"/>.
        /// </summary>
        protected SerializedProperty m_CurveVisualObject;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.overrideLineOrigin"/>.
        /// </summary>
        protected SerializedProperty m_OverrideLineOrigin;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.lineOriginTransform"/>.
        /// </summary>
        protected SerializedProperty m_LineOriginTransform;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.visualPointCount"/>.
        /// </summary>
        protected SerializedProperty m_VisualPointCount;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.maxVisualCurveDistance"/>.
        /// </summary>
        protected SerializedProperty m_MaxVisualCurveDistance;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.restingVisualLineLength"/>.
        /// </summary>
        protected SerializedProperty m_RestingVisualLineLength;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.lineDynamicsMode"/>.
        /// </summary>
        protected SerializedProperty m_LineDynamicsMode;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.retractDelay"/>.
        /// </summary>
        protected SerializedProperty m_RetractDelay;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.retractDuration"/>.
        /// </summary>
        protected SerializedProperty m_RetractDuration;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.extendLineToEmptyHit"/>.
        /// </summary>
        protected SerializedProperty m_ExtendLineToEmptyHit;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.extensionRate"/>.
        /// </summary>
        protected SerializedProperty m_ExtensionRate;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.endPointExpansionRate"/>.
        /// </summary>
        protected SerializedProperty m_EndPointExpansionRate;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.computeMidPointWithComplexCurves"/>.
        /// </summary>
        protected SerializedProperty m_ComputeMidPointWithComplexCurves;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.snapToSelectedAttachIfAvailable"/>.
        /// </summary>
        protected SerializedProperty m_SnapToSelectedAttachIfAvailable;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.snapToSnapVolumeIfAvailable"/>.
        /// </summary>
        protected SerializedProperty m_SnapToSnapVolumeIfAvailable;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.curveStartOffset"/>.
        /// </summary>
        protected SerializedProperty m_CurveStartOffset;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.curveEndOffset"/>.
        /// </summary>
        protected SerializedProperty m_CurveEndOffset;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.customizeLinePropertiesForState"/>.
        /// </summary>
        protected SerializedProperty m_CustomizeLinePropertiesForState;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.linePropertyAnimationSpeed"/>.
        /// </summary>
        protected SerializedProperty m_LinePropertyAnimationSpeed;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.noValidHitProperties"/>.
        /// </summary>
        protected SerializedProperty m_NoValidHitProperties;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.uiHitProperties"/>.
        /// </summary>
        protected SerializedProperty m_UIHitProperties;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.uiPressHitProperties"/>.
        /// </summary>
        protected SerializedProperty m_UIPressHitProperties;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.selectHitProperties"/>.
        /// </summary>
        protected SerializedProperty m_SelectHitProperties;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.hoverHitProperties"/>.
        /// </summary>
        protected SerializedProperty m_HoverHitProperties;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.renderLineInWorldSpace"/>.
        /// </summary>
        protected SerializedProperty m_RenderLineInWorldSpace;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.swapMaterials"/>.
        /// </summary>
        protected SerializedProperty m_SwapMaterials;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.baseLineMaterial"/>.
        /// </summary>
        protected SerializedProperty m_BaseLineMaterial;

        /// <summary>
        /// <see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CurveVisualController.emptyHitMaterial"/>.
        /// </summary>
        protected SerializedProperty m_EmptyHitMaterial;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.lineRenderer"/>.</summary>
            public static readonly GUIContent lineRenderer = EditorGUIUtility.TrTextContent("Line Renderer", "Line renderer to control.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.curveInteractionDataProvider"/>.</summary>
            public static readonly GUIContent curveVisualObject = EditorGUIUtility.TrTextContent("Curve Visual Object", "Curve data source used to generate the visual curve.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.overrideLineOrigin"/>.</summary>
            public static readonly GUIContent overrideLineOrigin = EditorGUIUtility.TrTextContent("Override Line Origin", "Indicates whether to override the line origin with a custom transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.lineOriginTransform"/>.</summary>
            public static readonly GUIContent lineOriginTransform = EditorGUIUtility.TrTextContent("Line Origin Transform", "The transform that determines the origin position and direction of the line when overriding.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.visualPointCount"/>.</summary>
            public static readonly GUIContent visualPointCount = EditorGUIUtility.TrTextContent("Visual Point Count", "Number of points used to create the visual curve.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.maxVisualCurveDistance"/>.</summary>
            public static readonly GUIContent maxVisualCurveDistance = EditorGUIUtility.TrTextContent("Max Visual Curve Distance", "Maximum distance the visual curve can extend.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.restingVisualLineLength"/>.</summary>
            public static readonly GUIContent restingVisualLineLength = EditorGUIUtility.TrTextContent("Resting Visual Line Length", "Default length of the line when not extended or retracted.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.lineDynamicsMode"/>.</summary>
            public static readonly GUIContent lineDynamicsMode = EditorGUIUtility.TrTextContent("Line Dynamics Mode", "Specifies the dynamics mode of the line.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.retractDelay"/>.</summary>
            public static readonly GUIContent retractDelay = EditorGUIUtility.TrTextContent("Retract Delay", "Delay before the line starts retracting after extending.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.retractDuration"/>.</summary>
            public static readonly GUIContent retractDuration = EditorGUIUtility.TrTextContent("Retract Duration", "Duration it takes for the line to fully retract.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.extendLineToEmptyHit"/>.</summary>
            public static readonly GUIContent extendLineToEmptyHit = EditorGUIUtility.TrTextContent("Extend Line To Empty Hit", "Determines if the line should extend out to empty hits, if not, length will be maintained.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.extensionRate"/>.</summary>
            public static readonly GUIContent extensionRate = EditorGUIUtility.TrTextContent("Extension Rate", "Rate at which the line extends to meet hit point. Set to 0 for instant extension.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.endPointExpansionRate"/>.</summary>
            public static readonly GUIContent endPointExpansionRate = EditorGUIUtility.TrTextContent("End Point Expansion Rate", "Rate at which the end point expands and retracts to and from the end point.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.computeMidPointWithComplexCurves"/>.</summary>
            public static readonly GUIContent computeMidPointWithComplexCurves = EditorGUIUtility.TrTextContent("Compute Mid Point With Complex Curves", "Determines if the mid-point is computed for curves with more than 1 segment. Overwrites any line bend ratio settings when active.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.snapToSelectedAttachIfAvailable"/>.</summary>
            public static readonly GUIContent snapToSelectedAttachIfAvailable = EditorGUIUtility.TrTextContent("Snap To Selected Attach If Available", "Snaps the line to a selected attachment point if available.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.snapToSnapVolumeIfAvailable"/>.</summary>
            public static readonly GUIContent snapToSnapVolumeIfAvailable = EditorGUIUtility.TrTextContent("Snap To Snap Volume If Available", "Snaps the line to a snap volume if available.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.curveStartOffset"/>.</summary>
            public static readonly GUIContent curveStartOffset = EditorGUIUtility.TrTextContent("Curve Start Offset", "Offset at the start of the curve to avoid overlap with the origin.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.curveEndOffset"/>.</summary>
            public static readonly GUIContent curveEndOffset = EditorGUIUtility.TrTextContent("Curve End Offset", "Offset at the end of the curve in meters to avoid overlap with the target.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.customizeLinePropertiesForState"/>.</summary>
            public static readonly GUIContent customizeLinePropertiesForState = EditorGUIUtility.TrTextContent("Customize Line Properties For State", "Indicates whether to customize line properties for different endpoint type states.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.linePropertyAnimationSpeed"/>.</summary>
            public static readonly GUIContent linePropertyAnimationSpeed = EditorGUIUtility.TrTextContent("Line Property Animation Speed", "Speed at which the line width changes when transitioning between states.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.noValidHitProperties"/>.</summary>
            public static readonly GUIContent noValidHitProperties = EditorGUIUtility.TrTextContent("No Valid Hit Properties", "Line properties when no hit is detected or when over an object that cannot be interacted with.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.uiHitProperties"/>.</summary>
            public static readonly GUIContent uiHitProperties = EditorGUIUtility.TrTextContent("UI Hit Properties", "Line properties when a valid UI hit is detected.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.uiPressHitProperties"/>.</summary>
            public static readonly GUIContent uiPressHitProperties = EditorGUIUtility.TrTextContent("UI Press Hit Properties", "Line properties when a valid UI press is detected.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.selectHitProperties"/>.</summary>
            public static readonly GUIContent selectHitProperties = EditorGUIUtility.TrTextContent("Select Hit Properties", "Line properties when a valid selection is detected.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.hoverHitProperties"/>.</summary>
            public static readonly GUIContent hoverHitProperties = EditorGUIUtility.TrTextContent("Hover Hit Properties", "Line properties when a valid non-UI hit is detected.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.renderLineInWorldSpace"/>.</summary>
            public static readonly GUIContent renderLineInWorldSpace = EditorGUIUtility.TrTextContent("Render Line In World Space", "If true the line will be rendered in world space, otherwise it will be rendered in local space. Set this to false in the event that high speed locomotion causes some visual artifacts with the line renderer.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.swapMaterials"/>.</summary>
            public static readonly GUIContent swapMaterials = EditorGUIUtility.TrTextContent("Swap Materials", "Indicates whether to swap the Line Renderer component's material for different states.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.baseLineMaterial"/>.</summary>
            public static readonly GUIContent baseLineMaterial = EditorGUIUtility.TrTextContent("Base Line Material", "Material to use in all cases other than when over 3D geometry that is not a valid interactable target.");
            /// <summary><see cref="GUIContent"/> for <see cref="CurveVisualController.emptyHitMaterial"/>.</summary>
            public static readonly GUIContent emptyHitMaterial = EditorGUIUtility.TrTextContent("Empty Hit Material", "Material to use when over 3D geometry that is not a valid interactable target.");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_LineRenderer = serializedObject.FindProperty("m_LineRenderer");
            m_CurveVisualObject = serializedObject.FindProperty("m_CurveVisualObject");
            m_OverrideLineOrigin = serializedObject.FindProperty("m_OverrideLineOrigin");
            m_LineOriginTransform = serializedObject.FindProperty("m_LineOriginTransform");
            m_VisualPointCount = serializedObject.FindProperty("m_VisualPointCount");
            m_MaxVisualCurveDistance = serializedObject.FindProperty("m_MaxVisualCurveDistance");
            m_RestingVisualLineLength = serializedObject.FindProperty("m_RestingVisualLineLength");
            m_LineDynamicsMode = serializedObject.FindProperty("m_LineDynamicsMode");
            m_RetractDelay = serializedObject.FindProperty("m_RetractDelay");
            m_RetractDuration = serializedObject.FindProperty("m_RetractDuration");
            m_ExtendLineToEmptyHit = serializedObject.FindProperty("m_ExtendLineToEmptyHit");
            m_ExtensionRate = serializedObject.FindProperty("m_ExtensionRate");
            m_EndPointExpansionRate = serializedObject.FindProperty("m_EndPointExpansionRate");
            m_ComputeMidPointWithComplexCurves = serializedObject.FindProperty("m_ComputeMidPointWithComplexCurves");
            m_SnapToSelectedAttachIfAvailable = serializedObject.FindProperty("m_SnapToSelectedAttachIfAvailable");
            m_SnapToSnapVolumeIfAvailable = serializedObject.FindProperty("m_SnapToSnapVolumeIfAvailable");
            m_CurveStartOffset = serializedObject.FindProperty("m_CurveStartOffset");
            m_CurveEndOffset = serializedObject.FindProperty("m_CurveEndOffset");
            m_CustomizeLinePropertiesForState = serializedObject.FindProperty("m_CustomizeLinePropertiesForState");
            m_LinePropertyAnimationSpeed = serializedObject.FindProperty("m_LinePropertyAnimationSpeed");
            m_NoValidHitProperties = serializedObject.FindProperty("m_NoValidHitProperties");
            m_UIHitProperties = serializedObject.FindProperty("m_UIHitProperties");
            m_UIPressHitProperties = serializedObject.FindProperty("m_UIPressHitProperties");
            m_SelectHitProperties = serializedObject.FindProperty("m_SelectHitProperties");
            m_HoverHitProperties = serializedObject.FindProperty("m_HoverHitProperties");
            m_RenderLineInWorldSpace = serializedObject.FindProperty("m_RenderLineInWorldSpace");
            m_SwapMaterials = serializedObject.FindProperty("m_SwapMaterials");
            m_BaseLineMaterial = serializedObject.FindProperty("m_BaseLineMaterial");
            m_EmptyHitMaterial = serializedObject.FindProperty("m_EmptyHitMaterial");
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
            DrawObjectReferences();
            EditorGUILayout.Space();

            DrawEndPointTypeLineConfiguration();
            EditorGUILayout.Space();

            m_VisualPointCount.isExpanded = EditorGUILayout.Foldout(m_VisualPointCount.isExpanded, EditorGUIUtility.TrTempContent("Advanced Properties"), true);
            if (m_VisualPointCount.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawVisualSettings();
                    EditorGUILayout.Space();

                    DrawCurveAdjustments();
                    EditorGUILayout.Space();

                    DrawLineDynamics();
                    EditorGUILayout.Space();

                    DrawMaterialSettings();
                }
            }
        }

        /// <summary>
        /// Draws the object reference properties in the custom inspector.
        /// This includes fields like Line Renderer, Curve Visual Object, Override Line Origin,
        /// and Line Origin Transform.
        /// </summary>
        protected virtual void DrawObjectReferences()
        {
            EditorGUILayout.LabelField("Object References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_LineRenderer, Contents.lineRenderer);
            EditorGUILayout.PropertyField(m_CurveVisualObject, Contents.curveVisualObject);
            EditorGUILayout.PropertyField(m_OverrideLineOrigin, Contents.overrideLineOrigin);
            if (m_OverrideLineOrigin.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_LineOriginTransform, Contents.lineOriginTransform);
                }
            }
        }

        /// <summary>
        /// Draws the visual settings section of the custom inspector.
        /// This includes settings related to visual point count, maximum visual curve distance,
        /// and resting visual line length.
        /// </summary>
        protected virtual void DrawVisualSettings()
        {
            EditorGUILayout.LabelField("Visual Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_VisualPointCount, Contents.visualPointCount);
            EditorGUILayout.PropertyField(m_MaxVisualCurveDistance, Contents.maxVisualCurveDistance);
            EditorGUILayout.PropertyField(m_RestingVisualLineLength, Contents.restingVisualLineLength);
        }

        /// <summary>
        /// Draws the line dynamics section in the custom inspector.
        /// This includes settings for dynamics mode, retract delay, line retract duration,
        /// end point expansion rate, and extension rate.
        /// </summary>
        protected virtual void DrawLineDynamics()
        {
            EditorGUILayout.LabelField("Line Dynamics", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_RenderLineInWorldSpace, Contents.renderLineInWorldSpace);
            EditorGUILayout.PropertyField(m_ExtendLineToEmptyHit, Contents.extendLineToEmptyHit);
            EditorGUILayout.PropertyField(m_ExtensionRate, Contents.extensionRate);

            EditorGUILayout.PropertyField(m_LineDynamicsMode, Contents.lineDynamicsMode);
            if (m_LineDynamicsMode.intValue == (int)LineDynamicsMode.RetractOnHitLoss)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_RetractDelay, Contents.retractDelay);
                    EditorGUILayout.PropertyField(m_RetractDuration, Contents.retractDuration);
                }
            }

            if (m_LineDynamicsMode.intValue == (int)LineDynamicsMode.ExpandFromHitPoint)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_EndPointExpansionRate, Contents.endPointExpansionRate);
                }
            }
        }

        /// <summary>
        /// Draws the curve adjustments section in the custom inspector.
        /// This includes settings for line bend ratio, midpoint computation with complex curves,
        /// snapping options, and curve start/end offsets.
        /// </summary>
        protected virtual void DrawCurveAdjustments()
        {
            EditorGUILayout.LabelField("Curve Adjustments", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ComputeMidPointWithComplexCurves, Contents.computeMidPointWithComplexCurves);
            EditorGUILayout.PropertyField(m_SnapToSelectedAttachIfAvailable, Contents.snapToSelectedAttachIfAvailable);
            EditorGUILayout.PropertyField(m_SnapToSnapVolumeIfAvailable, Contents.snapToSnapVolumeIfAvailable);
            EditorGUILayout.PropertyField(m_CurveStartOffset, Contents.curveStartOffset);
            EditorGUILayout.PropertyField(m_CurveEndOffset, Contents.curveEndOffset);
        }

        /// <summary>
        /// Draws the line properties configuration section in the custom inspector.
        /// This includes settings for customizing line properties for different states and
        /// various line property configurations.
        /// </summary>
        protected virtual void DrawEndPointTypeLineConfiguration()
        {
            EditorGUILayout.LabelField("Line Properties Configuration", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_CustomizeLinePropertiesForState, Contents.customizeLinePropertiesForState);
            if (m_CustomizeLinePropertiesForState.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_LinePropertyAnimationSpeed, Contents.linePropertyAnimationSpeed);

                    EditorGUILayout.PropertyField(m_NoValidHitProperties, Contents.noValidHitProperties);
                    EditorGUILayout.PropertyField(m_UIHitProperties, Contents.uiHitProperties);
                    EditorGUILayout.PropertyField(m_UIPressHitProperties, Contents.uiPressHitProperties);
                    EditorGUILayout.PropertyField(m_HoverHitProperties, Contents.hoverHitProperties);
                    EditorGUILayout.PropertyField(m_SelectHitProperties, Contents.selectHitProperties);
                }
            }
        }

        /// <summary>
        /// Draws the material settings section in the custom inspector.
        /// This includes options for swapping materials and settings for normal line material
        /// and empty hit material.
        /// </summary>
        protected virtual void DrawMaterialSettings()
        {
            EditorGUILayout.LabelField("Material Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_SwapMaterials, Contents.swapMaterials);
            if (m_SwapMaterials.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_BaseLineMaterial, Contents.baseLineMaterial);
                    EditorGUILayout.PropertyField(m_EmptyHitMaterial, Contents.emptyHitMaterial);
                }
            }
        }
    }
}
