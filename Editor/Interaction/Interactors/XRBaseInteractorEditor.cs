using System.Collections.Generic;
using UnityEditor.XR.Interaction.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="XRBaseInteractor"/>.
    /// </summary>
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    [CustomEditor(typeof(XRBaseInteractor), true), CanEditMultipleObjects]
    public partial class XRBaseInteractorEditor : BaseInteractionEditor
    {
        const string k_FiltersExpandedKey = "XRI." + nameof(XRBaseInteractorEditor) + ".FiltersExpanded";
        const string k_HoverFiltersExpandedKey = "XRI." + nameof(XRBaseInteractorEditor) + ".HoverFiltersExpanded";
        const string k_SelectFiltersExpandedKey = "XRI." + nameof(XRBaseInteractorEditor) + ".SelectFiltersExpanded";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.interactionManager"/>.</summary>
        protected SerializedProperty m_InteractionManager;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.interactionLayers"/>.</summary>
        protected SerializedProperty m_InteractionLayers;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.handedness"/>.</summary>
        protected SerializedProperty m_Handedness;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.attachTransform"/>.</summary>
        protected SerializedProperty m_AttachTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.keepSelectedTargetValid"/>.</summary>
        protected SerializedProperty m_KeepSelectedTargetValid;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.disableVisualsWhenBlockedInGroup"/>.</summary>
        protected SerializedProperty m_DisableVisualsWhenBlockedInGroup;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.startingSelectedInteractable"/>.</summary>
        protected SerializedProperty m_StartingSelectedInteractable;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.startingTargetFilter"/>.</summary>
        protected SerializedProperty m_StartingTargetFilter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.startingHoverFilters"/>.</summary>
        protected SerializedProperty m_StartingHoverFilters;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.startingSelectFilters"/>.</summary>
        protected SerializedProperty m_StartingSelectFilters;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.hoverEntered"/>.</summary>
        protected SerializedProperty m_HoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.hoverExited"/>.</summary>
        protected SerializedProperty m_HoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.selectEntered"/>.</summary>
        protected SerializedProperty m_SelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.selectExited"/>.</summary>
        protected SerializedProperty m_SelectExited;

        bool m_FiltersExpanded;
        ReadOnlyReorderableList<IXRHoverFilter> m_HoverFilters;
        ReadOnlyReorderableList<IXRSelectFilter> m_SelectFilters;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static partial class BaseContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.interactionManager"/>.</summary>
            public static readonly GUIContent interactionManager = EditorGUIUtility.TrTextContent("Interaction Manager", "The XR Interaction Manager that this Interactor will communicate with (will find one if None).");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.interactionLayers"/>.</summary>
            public static readonly GUIContent interactionLayers = EditorGUIUtility.TrTextContent("Interaction Layer Mask", "Allows interaction with Interactables whose Interaction Layer Mask overlaps with any Layer in this Interaction Layer Mask.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.handedness"/>.</summary>
            public static readonly GUIContent handedness = EditorGUIUtility.TrTextContent("Handedness", "Represents which hand or controller the interactor is associated with.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.attachTransform"/>.</summary>
            public static readonly GUIContent attachTransform = EditorGUIUtility.TrTextContent("Attach Transform", "The Transform that is used as the attach point for Interactables. Will create an empty GameObject if None.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.keepSelectedTargetValid"/>.</summary>
            public static readonly GUIContent keepSelectedTargetValid = EditorGUIUtility.TrTextContent("Keep Selected Target Valid", "Keep selecting the target when not touching or pointing to it after initially selecting it. It is recommended to set this value to true for grabbing objects, false for teleportation.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.disableVisualsWhenBlockedInGroup"/>.</summary>
            public static readonly GUIContent disableVisualsWhenBlockedInGroup = EditorGUIUtility.TrTextContent("Disable Visuals When Blocked In Group", "Whether to disable visuals when this Interactor is part of an Interaction Group and is incapable of interacting due to active interaction by another Interactor in the Group.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.startingSelectedInteractable"/>.</summary>
            public static readonly GUIContent startingSelectedInteractable = EditorGUIUtility.TrTextContent("Starting Selected Interactable", "The Interactable that this Interactor will automatically select at startup (optional, may be None).");
            /// <summary><see cref="GUIContent"/> for the header label of Hover events.</summary>
            public static readonly GUIContent hoverEventsHeader = EditorGUIUtility.TrTextContent("Hover", "Called when this Interactor begins hovering over an Interactable (Entered), or ends hovering (Exited).");
            /// <summary><see cref="GUIContent"/> for the header label of Select events.</summary>
            public static readonly GUIContent selectEventsHeader = EditorGUIUtility.TrTextContent("Select", "Called when this Interactor begins selecting an Interactable (Entered), or ends selecting (Exited).");

            /// <summary>The Interactor filters foldout.</summary>
            public static readonly GUIContent interactorFilters = EditorGUIUtility.TrTextContent("Interactor Filters", "Add filters to extend this Interactor without needing to create a derived behavior.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.startingSelectedInteractable"/>.</summary>
            public static readonly GUIContent startingTargetFilter = EditorGUIUtility.TrTextContent("Starting Target Filter", "The target filter that this Interactor will automatically link at startup (optional, may be None).");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.startingSelectFilters"/>.</summary>
            public static readonly GUIContent startingHoverFilters = EditorGUIUtility.TrTextContent("Starting Hover Filters", "The hover filters that this Interactor will automatically link at startup (optional, may be empty). Used as additional hover validations for this Interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.startingSelectFilters"/>.</summary>
            public static readonly GUIContent startingSelectFilters = EditorGUIUtility.TrTextContent("Starting Select Filters", "The select filters that this Interactor will automatically link at startup (optional, may be empty). Used as additional select validations for this Interactor.");
            /// <summary>The list of runtime hover filters.</summary>
            public static readonly GUIContent hoverFiltersHeader = EditorGUIUtility.TrTextContent("Hover Filters", "This list is populated in Awake, during Play mode.");
            /// <summary>The list of runtime select filters.</summary>
            public static readonly GUIContent selectFiltersHeader = EditorGUIUtility.TrTextContent("Select Filters", "This list is populated in Awake, during Play mode.");
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        /// <seealso cref="MonoBehaviour"/>
        protected virtual void OnEnable()
        {
            m_InteractionManager = serializedObject.FindProperty("m_InteractionManager");
            m_InteractionLayers = serializedObject.FindProperty("m_InteractionLayers");
            m_Handedness = serializedObject.FindProperty("m_Handedness");
            m_AttachTransform = serializedObject.FindProperty("m_AttachTransform");
            m_KeepSelectedTargetValid = serializedObject.FindProperty("m_KeepSelectedTargetValid");
            m_DisableVisualsWhenBlockedInGroup = serializedObject.FindProperty("m_DisableVisualsWhenBlockedInGroup");
            m_StartingSelectedInteractable = serializedObject.FindProperty("m_StartingSelectedInteractable");

            m_HoverEntered = serializedObject.FindProperty("m_HoverEntered");
            m_HoverExited = serializedObject.FindProperty("m_HoverExited");
            m_SelectEntered = serializedObject.FindProperty("m_SelectEntered");
            m_SelectExited = serializedObject.FindProperty("m_SelectExited");

            m_FiltersExpanded = SessionState.GetBool(k_FiltersExpandedKey, false);

            m_StartingHoverFilters = serializedObject.FindProperty("m_StartingHoverFilters");
            m_StartingSelectFilters = serializedObject.FindProperty("m_StartingSelectFilters");

            m_StartingTargetFilter = serializedObject.FindProperty("m_StartingTargetFilter");

            var interactor = (XRBaseInteractor)target;
            m_HoverFilters = new ReadOnlyReorderableList<IXRHoverFilter>(new List<IXRHoverFilter>(), BaseContents.hoverFiltersHeader, k_HoverFiltersExpandedKey)
            {
                isExpanded = SessionState.GetBool(k_HoverFiltersExpandedKey, true),
                updateElements = list => interactor.hoverFilters.GetAll(list),
                onListReordered = (element, newIndex) => interactor.hoverFilters.MoveTo(element, newIndex),
            };

            m_SelectFilters = new ReadOnlyReorderableList<IXRSelectFilter>(new List<IXRSelectFilter>(), BaseContents.selectFiltersHeader, k_SelectFiltersExpandedKey)
            {
                isExpanded = SessionState.GetBool(k_SelectFiltersExpandedKey, true),
                updateElements = list => interactor.selectFilters.GetAll(list),
                onListReordered = (element, newIndex) => interactor.selectFilters.MoveTo(element, newIndex),
            };
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        /// <seealso cref="DrawFilters"/>
        /// <seealso cref="DrawEvents"/>
        protected override void DrawInspector()
        {
            DrawBeforeProperties();
            DrawProperties();
            DrawDerivedProperties();

            DrawFilters();

            EditorGUILayout.Space();

            DrawEvents();
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
        /// when a derived behavior adds additional serialized properties that should
        /// be displayed in the Inspector.
        /// </summary>
        protected virtual void DrawProperties()
        {
            DrawCoreConfiguration();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, BaseContents.keepSelectedTargetValid);
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the event properties. Override this method to customize the
        /// events shown in the Inspector. This is typically the method overridden
        /// when a derived behavior adds additional serialized event properties
        /// that should be displayed in the Inspector.
        /// </summary>
        protected virtual void DrawEvents()
        {
            DrawInteractorEvents();
        }

        /// <summary>
        /// Draw the core group of property fields. These are the main properties
        /// that appear before any other spaced section in the inspector.
        /// </summary>
        protected virtual void DrawCoreConfiguration()
        {
            DrawInteractionManagement();
            EditorGUILayout.PropertyField(m_Handedness, BaseContents.handedness);
            EditorGUILayout.PropertyField(m_AttachTransform, BaseContents.attachTransform);
            EditorGUILayout.PropertyField(m_DisableVisualsWhenBlockedInGroup, BaseContents.disableVisualsWhenBlockedInGroup);
            EditorGUILayout.PropertyField(m_StartingSelectedInteractable, BaseContents.startingSelectedInteractable);
        }

        /// <summary>
        /// Draw the property fields related to interaction management.
        /// </summary>
        protected virtual void DrawInteractionManagement()
        {
            EditorGUILayout.PropertyField(m_InteractionManager, BaseContents.interactionManager);
            EditorGUILayout.PropertyField(m_InteractionLayers, BaseContents.interactionLayers);
        }

        /// <summary>
        /// Draw the Interactor Filter foldout.
        /// </summary>
        protected virtual void DrawFilters()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_FiltersExpanded = EditorGUILayout.Foldout(m_FiltersExpanded, BaseContents.interactorFilters, true);
                if (check.changed)
                    SessionState.SetBool(k_FiltersExpandedKey, m_FiltersExpanded);
            }

            if (!m_FiltersExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                DrawFiltersNested();
            }
        }

        /// <summary>
        /// Draw the property fields related to the hover and select filters.
        /// </summary>
        protected virtual void DrawFiltersNested()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                EditorGUILayout.PropertyField(m_StartingTargetFilter, BaseContents.startingTargetFilter);
                EditorGUILayout.PropertyField(m_StartingHoverFilters, BaseContents.startingHoverFilters);
                EditorGUILayout.PropertyField(m_StartingSelectFilters, BaseContents.startingSelectFilters);
            }

            if (!Application.isPlaying)
                return;

            if (serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.HelpBox("Filters cannot be multi-edited.", MessageType.None);
                return;
            }

            m_HoverFilters.DoLayoutList();
            m_SelectFilters.DoLayoutList();
        }

        /// <summary>
        /// Draw the Interactor Events foldout.
        /// </summary>
        /// <seealso cref="DrawInteractorEventsNested"/>
        protected virtual void DrawInteractorEvents()
        {
            m_HoverEntered.isExpanded = EditorGUILayout.Foldout(m_HoverEntered.isExpanded, EditorGUIUtility.TrTempContent("Interactor Events"), true);
            if (m_HoverEntered.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawInteractorEventsNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Interactor Events foldout.
        /// </summary>
        /// <seealso cref="DrawInteractorEvents"/>
        protected virtual void DrawInteractorEventsNested()
        {
            EditorGUILayout.LabelField(BaseContents.hoverEventsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_HoverEntered);
            EditorGUILayout.PropertyField(m_HoverExited);

            EditorGUILayout.LabelField(BaseContents.selectEventsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_SelectEntered);
            EditorGUILayout.PropertyField(m_SelectExited);
        }
    }
}
