using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="XRPokeInteractor"/>.
    /// </summary>
    [CustomEditor(typeof(XRPokeInteractor), true), CanEditMultipleObjects]
    public class XRPokeInteractorEditor : XRBaseInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.pokeDepth"/>.</summary>
        protected SerializedProperty m_PokeDepth;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.pokeWidth"/>.</summary>
        protected SerializedProperty m_PokeWidth;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.pokeSelectWidth"/>.</summary>
        protected SerializedProperty m_PokeSelectWidth;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.pokeHoverRadius"/>.</summary>
        protected SerializedProperty m_PokeHoverRadius;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.pokeInteractionOffset"/>.</summary>
        protected SerializedProperty m_PokeInteractionOffset;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.physicsLayerMask"/>.</summary>
        protected SerializedProperty m_PhysicsLayerMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.physicsTriggerInteraction"/>.</summary>
        protected SerializedProperty m_PhysicsTriggerInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.uiDocumentTriggerInteraction"/>.</summary>
        protected SerializedProperty m_UIDocumentTriggerInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.requirePokeFilter"/>.</summary>
        protected SerializedProperty m_RequirePokeFilter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.enableUIInteraction"/>.</summary>
        protected SerializedProperty m_EnableUIInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.clickUIOnDown"/>.</summary>
        protected SerializedProperty m_ClickUIOnDown;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.debugVisualizationsEnabled"/>.</summary>
        protected SerializedProperty m_DebugVisualizationsEnabled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.uiHoverEntered"/>.</summary>
        protected SerializedProperty m_UIHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRPokeInteractor.uiHoverExited"/>.</summary>
        protected SerializedProperty m_UIHoverExited;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.pokeDepth"/>.</summary>
            public static readonly GUIContent pokeDepth = EditorGUIUtility.TrTextContent("Poke Depth", "The depth threshold within which an interaction can begin to be evaluated as a poke.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.pokeWidth"/>.</summary>
            public static readonly GUIContent pokeWidth = EditorGUIUtility.TrTextContent("Poke Width", "The width threshold within which an interaction can begin to be evaluated as a poke.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.pokeSelectWidth"/>.</summary>
            public static readonly GUIContent pokeSelectWidth = EditorGUIUtility.TrTextContent("Poke Select Width", "The width threshold within which an interaction can be evaluated as a poke select.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.pokeHoverRadius"/>.</summary>
            public static readonly GUIContent pokeHoverRadius = EditorGUIUtility.TrTextContent("Poke Hover Radius", "The radius threshold within which an interaction can be evaluated as a poke hover.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.pokeInteractionOffset"/>.</summary>
            public static readonly GUIContent pokeInteractionOffset = EditorGUIUtility.TrTextContent("Poke Interaction Offset", "Distance along the poke interactable interaction axis that allows for a poke to be triggered sooner/with less precision.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.physicsLayerMask"/>.</summary>
            public static readonly GUIContent physicsLayerMask = EditorGUIUtility.TrTextContent("Physics Layer Mask", "Physics layer mask used for limiting poke sphere overlap.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.physicsTriggerInteraction"/>.</summary>
            public static readonly GUIContent physicsTriggerInteraction = EditorGUIUtility.TrTextContent("Physics Trigger Interaction", "Determines whether the poke sphere overlap and cast will hit triggers. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.uiDocumentTriggerInteraction"/>.</summary>
            public static readonly GUIContent uiDocumentTriggerInteraction = EditorGUIUtility.TrTextContent("UI Document Trigger Interaction", "Determines if poke sphere overlap and cast include UI Document triggers: 'Collide' to include, 'Ignore' for performance optimization when not using UI Toolkit.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.requirePokeFilter"/>.</summary>
            public static readonly GUIContent requirePokeFilter = EditorGUIUtility.TrTextContent("Require Poke Filter", "Denotes whether or not valid targets will only include objects with a poke filter.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.enableUIInteraction"/>.</summary>
            public static readonly GUIContent enableUIInteraction = EditorGUIUtility.TrTextContent("UI Interaction", "When enabled, this allows the poke interactor to hover and select UI elements.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.clickUIOnDown"/>.</summary>
            public static readonly GUIContent clickUIOnDown = EditorGUIUtility.TrTextContent("Click UI On Down", "When enabled, this will invoke click events on press down instead of on release for buttons, toggles, input fields, and dropdowns.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRPokeInteractor.debugVisualizationsEnabled"/>.</summary>
            public static readonly GUIContent debugVisualizationsEnabled = EditorGUIUtility.TrTextContent("Debug Visualizations Enabled", "Denotes whether or not debug visuals are enabled for this poke interactor.");

            /// <summary><see cref="GUIContent"/> for the header label of UI events.</summary>
            public static readonly GUIContent uiEventsHeader = EditorGUIUtility.TrTextContent("UI", "Called when this Interactor begins hovering over UI (Entered), or ends hovering (Exited).");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            m_PokeDepth = serializedObject.FindProperty("m_PokeDepth");
            m_PokeWidth = serializedObject.FindProperty("m_PokeWidth");
            m_PokeSelectWidth = serializedObject.FindProperty("m_PokeSelectWidth");
            m_PokeHoverRadius = serializedObject.FindProperty("m_PokeHoverRadius");
            m_PokeInteractionOffset = serializedObject.FindProperty("m_PokeInteractionOffset");
            m_PhysicsLayerMask = serializedObject.FindProperty("m_PhysicsLayerMask");
            m_PhysicsTriggerInteraction = serializedObject.FindProperty("m_PhysicsTriggerInteraction");
            m_UIDocumentTriggerInteraction = serializedObject.FindProperty("m_UIDocumentTriggerInteraction");
            m_RequirePokeFilter = serializedObject.FindProperty("m_RequirePokeFilter");
            m_EnableUIInteraction = serializedObject.FindProperty("m_EnableUIInteraction");
            m_ClickUIOnDown = serializedObject.FindProperty("m_ClickUIOnDown");
            m_DebugVisualizationsEnabled = serializedObject.FindProperty("m_DebugVisualizationsEnabled");
            m_UIHoverEntered = serializedObject.FindProperty("m_UIHoverEntered");
            m_UIHoverExited = serializedObject.FindProperty("m_UIHoverExited");
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            base.DrawProperties();

            EditorGUILayout.Space();

            DrawPokeProperties();
            DrawUIProperties();
            DrawDebugProperties();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draw the property fields related to poke settings.
        /// </summary>
        protected virtual void DrawPokeProperties()
        {
            EditorGUILayout.PropertyField(m_PokeDepth, Contents.pokeDepth);
            EditorGUILayout.PropertyField(m_PokeWidth, Contents.pokeWidth);
            EditorGUILayout.PropertyField(m_PokeSelectWidth, Contents.pokeSelectWidth);
            EditorGUILayout.PropertyField(m_PokeHoverRadius, Contents.pokeHoverRadius);
            EditorGUILayout.PropertyField(m_PokeInteractionOffset, Contents.pokeInteractionOffset);
            EditorGUILayout.PropertyField(m_PhysicsLayerMask, Contents.physicsLayerMask);
            EditorGUILayout.PropertyField(m_PhysicsTriggerInteraction, Contents.physicsTriggerInteraction);
            // TODO for 3.2.0
            //EditorGUILayout.PropertyField(m_UIDocumentTriggerInteraction, Contents.uiDocumentTriggerInteraction);
            EditorGUILayout.PropertyField(m_RequirePokeFilter, Contents.requirePokeFilter);
        }

        /// <summary>
        /// Draw the property fields related to the UI interaction.
        /// </summary>
        protected virtual void DrawUIProperties()
        {
            EditorGUILayout.PropertyField(m_EnableUIInteraction, Contents.enableUIInteraction);
            if (m_EnableUIInteraction.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_ClickUIOnDown, Contents.clickUIOnDown);
                }
            }
        }

        /// <summary>
        /// Draw debug property fields.
        /// </summary>
        protected virtual void DrawDebugProperties()
        {
            EditorGUILayout.PropertyField(m_DebugVisualizationsEnabled, Contents.debugVisualizationsEnabled);
        }

        /// <inheritdoc />
        protected override void DrawInteractorEventsNested()
        {
            base.DrawInteractorEventsNested();

            EditorGUILayout.LabelField(Contents.uiEventsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_UIHoverEntered);
            EditorGUILayout.PropertyField(m_UIHoverExited);
        }
    }
}
