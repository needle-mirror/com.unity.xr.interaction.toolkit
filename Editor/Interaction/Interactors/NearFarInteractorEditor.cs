using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="NearFarInteractor"/>.
    /// </summary>
    [CustomEditor(typeof(NearFarInteractor), true), CanEditMultipleObjects]
    public class NearFarInteractorEditor : XRBaseInputInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.interactionAttachController"/>.</summary>
        protected SerializedProperty m_InteractionAttachController;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.enableNearCasting"/>.</summary>
        protected SerializedProperty m_EnableNearCasting;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.nearInteractionCaster"/>.</summary>
        protected SerializedProperty m_NearInteractionCaster;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.nearCasterSortingStrategy"/>.</summary>
        protected SerializedProperty m_NearCasterSortingStrategy;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.sortNearTargetsAfterTargetFilter"/>.</summary>
        protected SerializedProperty m_SortNearTargetsAfterTargetFilter;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.enableFarCasting"/>.</summary>
        protected SerializedProperty m_EnableFarCasting;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.farInteractionCaster"/>.</summary>
        protected SerializedProperty m_FarInteractionCaster;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.farAttachMode"/>.</summary>
        protected SerializedProperty m_FarAttachMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.enableUIInteraction"/>.</summary>
        protected SerializedProperty m_EnableUIInteraction;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.blockUIOnInteractableSelection"/>.</summary>
        protected SerializedProperty m_BlockUIOnInteractableSelection;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.uiHoverEntered"/>.</summary>
        protected SerializedProperty m_UIHoverEntered;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.uiHoverExited"/>.</summary>
        protected SerializedProperty m_UIHoverExited;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.uiPressInput"/>.</summary>
        protected SerializedProperty m_UIPressInput;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="NearFarInteractor.uiScrollInput"/>.</summary>
        protected SerializedProperty m_UIScrollInput;

        /// <inheritdoc />
        protected override bool showDeprecatedProperties => false;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.interactionAttachController"/>.</summary>
            public static readonly GUIContent interactionAttachController = EditorGUIUtility.TrTextContent("Interaction Attach Controller", "Reference to the attach controller used to control the attach transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.enableNearCasting"/>.</summary>
            public static readonly GUIContent enableNearCasting = EditorGUIUtility.TrTextContent("Enable Near Casting", "Determines if the near interaction caster will be used to find valid targets for this interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.nearInteractionCaster"/>.</summary>
            public static readonly GUIContent nearInteractionCaster = EditorGUIUtility.TrTextContent("Near Caster", "Reference to the near interaction caster used to find valid targets for this interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.nearCasterSortingStrategy"/>.</summary>
            public static readonly GUIContent nearCasterSortingStrategy = EditorGUIUtility.TrTextContent("Sorting Strategy", "Strategy used to compute the distance used to sort valid targets discovered by the near interaction caster.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.sortNearTargetsAfterTargetFilter"/>.</summary>
            public static readonly GUIContent sortNearTargetsAfterTargetFilter = EditorGUIUtility.TrTextContent("Sort Near Targets After Target Filter", "Determines if the near targets should be sorted after the target filter is applied. Should be true only if filter does not sort targets. Not used if no target filter is set.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.enableFarCasting"/>.</summary>
            public static readonly GUIContent enableFarCasting = EditorGUIUtility.TrTextContent("Enable Far Casting", "Determines if the far interaction caster will be used to find valid targets for this interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.farInteractionCaster"/>.</summary>
            public static readonly GUIContent farInteractionCaster = EditorGUIUtility.TrTextContent("Far Caster", "Reference to the far interaction caster used to find valid targets for this interactor.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.farAttachMode"/>.</summary>
            public static readonly GUIContent farAttachMode = EditorGUIUtility.TrTextContent("Far Attach Mode", "Determines how the attachment point is adjusted on far select. This typically results in whether the interactable stays distant at the far hit point or moves to the near hand.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.enableUIInteraction"/>.</summary>
            public static readonly GUIContent enableUIInteraction = EditorGUIUtility.TrTextContent("UI Interaction", "Enable to affect Unity UI GameObjects in a way that is similar to a mouse pointer. Requires the XR UI Input Module on the Event System.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.blockUIOnInteractableSelection"/>.</summary>
            public static readonly GUIContent blockUIOnInteractableSelection = EditorGUIUtility.TrTextContent("Block UI on Interactable Selection", "Enabling this option will block UI interaction when selecting interactables.");

            /// <summary><see cref="GUIContent"/> for the header label of UI events.</summary>
            public static readonly GUIContent uiEventsHeader = EditorGUIUtility.TrTextContent("UI", "Called when this Interactor begins hovering over UI (Entered), or ends hovering (Exited).");

            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.uiPressInput"/>.</summary>
            public static readonly GUIContent uiPressInput = EditorGUIUtility.TrTextContent("UI Press Input", "Input to use for pressing UI elements. Functions like a mouse button when pointing over UI.");
            /// <summary><see cref="GUIContent"/> for <see cref="NearFarInteractor.uiScrollInput"/>.</summary>
            public static readonly GUIContent uiScrollInput = EditorGUIUtility.TrTextContent("UI Scroll Input", "Input to use for scrolling UI elements. Functions like a mouse scroll wheel when pointing over UI.");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            m_InteractionAttachController = serializedObject.FindProperty("m_InteractionAttachController");
            m_EnableNearCasting = serializedObject.FindProperty("m_EnableNearCasting");
            m_NearInteractionCaster = serializedObject.FindProperty("m_NearInteractionCaster");
            m_NearCasterSortingStrategy = serializedObject.FindProperty("m_NearCasterSortingStrategy");
            m_SortNearTargetsAfterTargetFilter = serializedObject.FindProperty("m_SortNearTargetsAfterTargetFilter");
            m_EnableFarCasting = serializedObject.FindProperty("m_EnableFarCasting");
            m_FarInteractionCaster = serializedObject.FindProperty("m_FarInteractionCaster");
            m_FarAttachMode = serializedObject.FindProperty("m_FarAttachMode");
            m_EnableUIInteraction = serializedObject.FindProperty("m_EnableUIInteraction");
            m_BlockUIOnInteractableSelection = serializedObject.FindProperty("m_BlockUIOnInteractableSelection");
            m_UIHoverEntered = serializedObject.FindProperty("m_UIHoverEntered");
            m_UIHoverExited = serializedObject.FindProperty("m_UIHoverExited");
            m_UIPressInput = serializedObject.FindProperty("m_UIPressInput");
            m_UIScrollInput = serializedObject.FindProperty("m_UIScrollInput");
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            // Not calling base method to completely override drawn properties
            DrawInteractionManagement();
            // Handedness needs to be manually drawn since we don't want the other properties it's grouped with.
            EditorGUILayout.PropertyField(m_Handedness, BaseContents.handedness);
            DrawSelectionConfiguration();
            EditorGUILayout.Space();
            DrawAttachTransformController();
            EditorGUILayout.Space();
            DrawNearInteractionCaster();
            EditorGUILayout.Space();
            DrawFarInteractionCaster();
            EditorGUILayout.Space();
            DrawInputConfiguration();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draw the property fields related to attach controller.
        /// </summary>
        protected virtual void DrawAttachTransformController()
        {
            EditorGUILayout.PropertyField(m_InteractionAttachController, Contents.interactionAttachController);
        }

        /// <summary>
        /// Draw the property fields related to near casting.
        /// </summary>
        protected virtual void DrawNearInteractionCaster()
        {
            EditorGUILayout.PropertyField(m_EnableNearCasting, Contents.enableNearCasting);
            if (m_EnableNearCasting.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_NearInteractionCaster, Contents.nearInteractionCaster);
                    EditorGUILayout.PropertyField(m_NearCasterSortingStrategy, Contents.nearCasterSortingStrategy);
                    EditorGUILayout.PropertyField(m_SortNearTargetsAfterTargetFilter, Contents.sortNearTargetsAfterTargetFilter);
                }
            }
        }

        /// <summary>
        /// Draw the property fields related to far casting.
        /// </summary>
        protected virtual void DrawFarInteractionCaster()
        {
            EditorGUILayout.PropertyField(m_EnableFarCasting, Contents.enableFarCasting);
            if (m_EnableFarCasting.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_FarInteractionCaster, Contents.farInteractionCaster);
                    EditorGUILayout.PropertyField(m_FarAttachMode, Contents.farAttachMode);
                    DrawUIInteraction();
                }
            }
        }

        /// <inheritdoc />
        protected override void DrawInteractorEventsNested()
        {
            base.DrawInteractorEventsNested();

            EditorGUILayout.LabelField(Contents.uiEventsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_UIHoverEntered);
            EditorGUILayout.PropertyField(m_UIHoverExited);
        }

        /// <summary>
        /// Draw the property fields related to UI interactions.
        /// </summary>
        protected virtual void DrawUIInteraction()
        {
            EditorGUILayout.PropertyField(m_EnableUIInteraction, Contents.enableUIInteraction);
            if (m_EnableUIInteraction.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_BlockUIOnInteractableSelection, Contents.blockUIOnInteractableSelection);
                    DrawUIInteractionInputConfiguration();
                }
            }
        }

        /// <summary>
        /// Draw the input configuration for UI interactions.
        /// </summary>
        protected virtual void DrawUIInteractionInputConfiguration()
        {
            EditorGUILayout.PropertyField(m_UIPressInput, Contents.uiPressInput);
            EditorGUILayout.PropertyField(m_UIScrollInput, Contents.uiScrollInput);
        }

        /// <summary>
        /// Draw the property fields related to selection configuration.
        /// </summary>
        protected virtual void DrawSelectionConfiguration()
        {
            DrawSelectActionTrigger();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, BaseContents.keepSelectedTargetValid);
            EditorGUILayout.PropertyField(m_AllowHoveredActivate, BaseInputContents.allowHoveredActivate);
        }
    }
}
