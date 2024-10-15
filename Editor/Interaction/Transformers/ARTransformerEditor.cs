#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace UnityEditor.XR.Interaction.Toolkit.Transformers
{
    /// <summary>
    /// Custom editor for an <see cref="ARTransformer"/>.
    /// </summary>
    [CustomEditor(typeof(ARTransformer), true), CanEditMultipleObjects]
    public class ARTransformerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.objectPlaneTranslationMode"/>.</summary>
        protected SerializedProperty m_ObjectPlaneTranslationMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.enablePlaneClassificationFilter"/>.</summary>
        protected SerializedProperty m_EnablePlaneClassificationFilter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.planeClassificationsList"/>.</summary>
        protected SerializedProperty m_PlaneClassificationsList;
#if AR_FOUNDATION_6_0_OR_NEWER
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.planeClassifications"/>.</summary>
        protected SerializedProperty m_PlaneClassifications;
#endif
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.useInteractorOrientation"/>.</summary>
        protected SerializedProperty m_UseInteractorOrientation;

        //Scaling Properties
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.minScale"/>.</summary>
        protected SerializedProperty m_MinScale;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.maxScale"/>.</summary>
        protected SerializedProperty m_MaxScale;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.scaleSensitivity"/>.</summary>
        protected SerializedProperty m_ScaleSensitivity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.elasticity"/>.</summary>
        protected SerializedProperty m_Elasticity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.enableElasticBreakLimit"/>.</summary>
        protected SerializedProperty m_EnableElasticBreakLimit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.elasticBreakLimit"/>.</summary>
        protected SerializedProperty m_ElasticBreakLimit;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.objectPlaneTranslationMode"/>.</summary>
            public static readonly GUIContent objectPlaneTranslationMode = EditorGUIUtility.TrTextContent("Object Plane Translation Mode", "Controls whether Unity constrains the object vertically, horizontally, or free to move in all axes.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.enablePlaneClassificationFilter"/>.</summary>
            public static readonly GUIContent enablePlaneClassificationFilter = EditorGUIUtility.TrTextContent("Plane Classification Filter", "Enabling this will filter interactable manipulation down to only planes that match any of the allowed plane classifications.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.planeClassificationsList"/>.</summary>
            public static readonly GUIContent planeClassificationsList = EditorGUIUtility.TrTextContent("Plane Classifications List", "The classifications a plane needs to match one of to allow interactable manipulation with.");
#if AR_FOUNDATION_6_0_OR_NEWER
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.planeClassifications"/>.</summary>
            public static readonly GUIContent planeClassifications = EditorGUIUtility.TrTextContent("Plane Classifications", "The classifications a plane needs to match one of to allow interactable manipulation with.");
#endif
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.m_UseInteractorOrientation"/>.</summary>
            public static readonly GUIContent useInteractorOrientation = EditorGUIUtility.TrTextContent("Use Interactor Orientation", "Controls whether the interactable will use the orientation of the interactor, or not.");

            //Scaling Properties
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.minScale"/>.</summary>
            public static readonly GUIContent minScale = EditorGUIUtility.TrTextContent("Min Scale", "The minimum scale of the object.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.maxScale"/>.</summary>
            public static readonly GUIContent maxScale = EditorGUIUtility.TrTextContent("Max Scale", "The maximum scale of the object.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.scaleSensitivity"/>.</summary>
            public static readonly GUIContent scaleSensitivity = EditorGUIUtility.TrTextContent("Scale Sensitivity", "Sensitivity to movement being translated into scale.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.elasticity"/>.</summary>
            public static readonly GUIContent elasticity = EditorGUIUtility.TrTextContent("Elasticity", "Amount of over scale allowed after hitting min/max of range.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.enableElasticBreakLimit"/>.</summary>
            public static readonly GUIContent enableElasticBreakLimit = EditorGUIUtility.TrTextContent("Enable Elastic Break Limit", "Whether to enable the elastic break limit when scaling the object beyond range.");
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.elasticBreakLimit"/>.</summary>
            public static readonly GUIContent elasticBreakLimit = EditorGUIUtility.TrTextContent("Elastic Break Limit", "The break limit of the elastic ratio used when scaling the object. Returns to min/max range over time after scaling beyond this limit.");

#if AR_FOUNDATION_6_0_OR_NEWER
            /// <summary>The help box message when a version of AR Foundation 6.0 or higher is present and the old PlaneClassification property was previously set.</summary>
            public static readonly GUIContent planeClassificationWarning = EditorGUIUtility.TrTextContent("The PlaneClassification enum was deprecated in AR Foundation 6.0. The old plane classification filter property for the ARTransformer contains prior settings. Please update your settings to the new PlaneClassifications enum type.");
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_ObjectPlaneTranslationMode = serializedObject.FindProperty("m_ObjectPlaneTranslationMode");
            m_EnablePlaneClassificationFilter = serializedObject.FindProperty("m_EnablePlaneClassificationFilter");
            m_PlaneClassificationsList = serializedObject.FindProperty("m_PlaneClassificationsList");
#if AR_FOUNDATION_6_0_OR_NEWER
            m_PlaneClassifications = serializedObject.FindProperty("m_PlaneClassifications");
#endif
            m_UseInteractorOrientation = serializedObject.FindProperty("m_UseInteractorOrientation");

            //Scaling Properties
            m_MinScale = serializedObject.FindProperty("m_MinScale");
            m_MaxScale = serializedObject.FindProperty("m_MaxScale");
            m_ScaleSensitivity = serializedObject.FindProperty("m_ScaleSensitivity");
            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_EnableElasticBreakLimit = serializedObject.FindProperty("m_EnableElasticBreakLimit");
            m_ElasticBreakLimit = serializedObject.FindProperty("m_ElasticBreakLimit");
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
            EditorGUILayout.PropertyField(m_ObjectPlaneTranslationMode, Contents.objectPlaneTranslationMode);
            EditorGUILayout.PropertyField(m_EnablePlaneClassificationFilter, Contents.enablePlaneClassificationFilter);

            if (m_EnablePlaneClassificationFilter.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
#if AR_FOUNDATION_6_0_OR_NEWER
                    if (m_PlaneClassificationsList.arraySize > 0)
                    {
                        EditorGUILayout.PropertyField(m_PlaneClassificationsList, Contents.planeClassificationsList);
                        if (GUILayout.Button("Migrate to Plane Classifications"))
                        {
                            Undo.RecordObjects(targets, "Migrate to Plane Classifications");
                            foreach (var obj in targets)
                            {
                                var arTransformer = (ARTransformer)obj;
                                arTransformer.MigratePlaneClassifications();
                            }
                        }

                        EditorGUILayout.HelpBox(Contents.planeClassificationWarning.text, MessageType.Warning);
                    }

                    EditorGUILayout.PropertyField(m_PlaneClassifications, Contents.planeClassifications);
#else
                    EditorGUILayout.PropertyField(m_PlaneClassificationsList, Contents.planeClassificationsList);
#endif
                }
            }

            EditorGUILayout.PropertyField(m_UseInteractorOrientation, Contents.useInteractorOrientation);

            // Scaling Properties
            EditorGUILayout.PropertyField(m_MinScale, Contents.minScale);
            EditorGUILayout.PropertyField(m_MaxScale, Contents.maxScale);
            EditorGUILayout.PropertyField(m_ScaleSensitivity, Contents.scaleSensitivity);
            EditorGUILayout.PropertyField(m_Elasticity, Contents.elasticity);
            EditorGUILayout.PropertyField(m_EnableElasticBreakLimit, Contents.enableElasticBreakLimit);
            if (m_EnableElasticBreakLimit.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_ElasticBreakLimit, Contents.elasticBreakLimit);
                }
            }
        }
    }
}
#endif
