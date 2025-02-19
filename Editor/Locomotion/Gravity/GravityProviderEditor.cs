using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Gravity
{
    /// <summary>
    /// Custom editor for a <see cref="GravityProvider"/>.
    /// </summary>
    [CustomEditor(typeof(GravityProvider), true), CanEditMultipleObjects]
    public class GravityProviderEditor : BaseInteractionEditor
    {
        const string k_GravityExpandedKey = "XRI." + nameof(GravityProviderEditor) + ".GravityExpanded";
        const string k_SphereCastExpandedKey = "XRI." + nameof(GravityProviderEditor) + ".SphereCastExpanded";
        const string k_EventsExpandedKey = "XRI." + nameof(GravityProviderEditor) + ".EventsExpanded";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.mediator"/>.</summary>
        protected SerializedProperty m_Mediator;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.transformationPriority"/>.</summary>
        protected SerializedProperty m_TransformationPriority;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.useGravity"/>.</summary>
        protected SerializedProperty m_UseGravity;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.useLocalSpaceGravity"/>.</summary>
        protected SerializedProperty m_UseLocalSpaceGravity;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.terminalVelocity"/>.</summary>
        protected SerializedProperty m_TerminalVelocity;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.gravityAccelerationModifier"/>.</summary>
        protected SerializedProperty m_GravityAccelerationModifier;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.updateCharacterControllerCenterEachFrame"/>.</summary>
        protected SerializedProperty m_UpdateCharacterControllerCenterEachFrame;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.sphereCastRadius"/>.</summary>
        protected SerializedProperty m_SphereCastRadius;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.sphereCastDistanceBuffer"/>.</summary>
        protected SerializedProperty m_SphereCastDistanceBuffer;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.sphereCastLayerMask"/>.</summary>
        protected SerializedProperty m_SphereCastLayerMask;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.sphereCastTriggerInteraction"/>.</summary>
        protected SerializedProperty m_SphereCastTriggerInteraction;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.onGravityLockChanged"/>.</summary>
        protected SerializedProperty m_OnGravityLockChanged;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GravityProvider.onGroundedChanged"/>.</summary>
        protected SerializedProperty m_OnGroundedChanged;

        /// <summary> Whether the Gravity settings are expanded.</summary>
        bool m_GravityExpanded;

        /// <summary> Whether the Sphere Cast settings are expanded.</summary>
        bool m_SphereCastExpanded;

        /// <summary> Whether the Events settings are expanded.</summary>
        bool m_EventsExpanded;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.mediator"/>.</summary>
            public static readonly GUIContent mediator = EditorGUIUtility.TrTextContent("Mediator", "The locomotion mediator that the grab move provider will interface with.");

            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.transformationPriority"/>.</summary>
            public static readonly GUIContent transformationPriority = EditorGUIUtility.TrTextContent("Transformation Priority", "The queue order of this provider's transformations of the XR Origin. The lower the value, the earlier the transformations are applied.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.useGravity"/>.</summary>
            public static readonly GUIContent useGravity = EditorGUIUtility.TrTextContent("Use Gravity", "Apply gravity to the XR Origin.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.useLocalSpaceGravity"/>.</summary>
            public static readonly GUIContent useLocalSpaceGravity = EditorGUIUtility.TrTextContent("Use Local Space Gravity", "Apply gravity based on the current Up vector of the XR Origin.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.terminalVelocity"/>.</summary>
            public static readonly GUIContent terminalVelocity = EditorGUIUtility.TrTextContent("Terminal Velocity", "Determines the maximum fall speed based on units per second.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.gravityAccelerationModifier"/>.</summary>
            public static readonly GUIContent gravityAccelerationModifier = EditorGUIUtility.TrTextContent("Gravity Acceleration Modifier", "Determines the speed at which a player reaches max gravity velocity.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.updateCharacterControllerCenterEachFrame"/>.</summary>
            public static readonly GUIContent updateCharacterControllerCenterEachFrame = EditorGUIUtility.TrTextContent("Update Character Controller Center Each Frame", "Sets the center of the character controller to match the local x and z positions of the player camera.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.sphereCastRadius"/>.</summary>
            public static readonly GUIContent sphereCastRadius = EditorGUIUtility.TrTextContent("Sphere Cast Radius Buffer", "Buffer for the radius of the sphere cast used to check if the player is grounded.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.sphereCastDistanceBuffer"/>.</summary>
            public static readonly GUIContent sphereCastDistanceBuffer = EditorGUIUtility.TrTextContent("Sphere Cast Distance Buffer", "Buffer for the distance of the sphere cast used to check if the player is grounded.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.sphereCastLayerMask"/>.</summary>
            public static readonly GUIContent sphereCastLayerMask = EditorGUIUtility.TrTextContent("Sphere Cast Layer Mask", "The layer mask used for the sphere cast to check if the player is grounded.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.sphereCastTriggerInteraction"/>.</summary>
            public static readonly GUIContent sphereCastTriggerInteraction = EditorGUIUtility.TrTextContent("Sphere Cast Trigger Interaction", "Whether trigger colliders are considered when using a sphere cast to determine if grounded.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.onGravityLockChanged"/>.</summary>
            public static readonly GUIContent onGravityLockChanged = EditorGUIUtility.TrTextContent("On Gravity Lock Changed", "Event triggered when gravity lock changes.");

            /// <summary><see cref="GUIContent"/> for <see cref="GravityProvider.onGroundedChanged"/>.</summary>
            public static readonly GUIContent onGroundedChanged = EditorGUIUtility.TrTextContent("On Grounded Changed", "Event triggered when grounded state changes.");

            /// <summary><see cref="GUIContent"/> for Gravity Settings header.</summary>
            public static readonly GUIContent gravitySettingsHeader = EditorGUIUtility.TrTextContent("Gravity Settings");
            /// <summary><see cref="GUIContent"/> for Sphere Cast Settings header.</summary>
            public static readonly GUIContent sphereCastSettingsHeader = EditorGUIUtility.TrTextContent("Sphere Cast Settings");
            /// <summary><see cref="GUIContent"/> for Events header.</summary>
            public static readonly GUIContent eventsHeader = EditorGUIUtility.TrTextContent("Events");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_Mediator = serializedObject.FindProperty(nameof(m_Mediator));
            m_TransformationPriority = serializedObject.FindProperty(nameof(m_TransformationPriority));
            m_UseGravity = serializedObject.FindProperty(nameof(m_UseGravity));
            m_UseLocalSpaceGravity = serializedObject.FindProperty(nameof(m_UseLocalSpaceGravity));
            m_TerminalVelocity = serializedObject.FindProperty(nameof(m_TerminalVelocity));
            m_GravityAccelerationModifier = serializedObject.FindProperty(nameof(m_GravityAccelerationModifier));
            m_UpdateCharacterControllerCenterEachFrame = serializedObject.FindProperty(nameof(m_UpdateCharacterControllerCenterEachFrame));
            m_SphereCastRadius = serializedObject.FindProperty(nameof(m_SphereCastRadius));
            m_SphereCastDistanceBuffer = serializedObject.FindProperty(nameof(m_SphereCastDistanceBuffer));
            m_SphereCastLayerMask = serializedObject.FindProperty(nameof(m_SphereCastLayerMask));
            m_SphereCastTriggerInteraction = serializedObject.FindProperty(nameof(m_SphereCastTriggerInteraction));
            m_OnGravityLockChanged = serializedObject.FindProperty(nameof(m_OnGravityLockChanged));
            m_OnGroundedChanged = serializedObject.FindProperty(nameof(m_OnGroundedChanged));

            m_GravityExpanded = SessionState.GetBool(k_GravityExpandedKey, true);
            m_SphereCastExpanded = SessionState.GetBool(k_SphereCastExpandedKey, true);
            m_EventsExpanded = SessionState.GetBool(k_EventsExpandedKey, true);
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
            EditorGUILayout.PropertyField(m_Mediator, Contents.mediator);
            EditorGUILayout.PropertyField(m_TransformationPriority, Contents.transformationPriority);
            EditorGUILayout.PropertyField(m_UpdateCharacterControllerCenterEachFrame, Contents.updateCharacterControllerCenterEachFrame);
            DrawDerivedProperties();

            EditorGUILayout.Space();

            DrawGravityProperties();

            EditorGUILayout.Space();

            DrawSphereCastProperties();

            EditorGUILayout.Space();

            DrawEvents();
        }

        /// <summary>
        /// Draw the property fields related to Gravity.
        /// </summary>
        protected virtual void DrawGravityProperties()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_GravityExpanded = EditorGUILayout.Foldout(m_GravityExpanded, Contents.gravitySettingsHeader, true, EditorStyles.foldoutHeader);
                if (check.changed)
                    SessionState.SetBool(k_GravityExpandedKey, m_GravityExpanded);
            }

            if (!m_GravityExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_UseGravity, Contents.useGravity);
                if (m_UseGravity.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_UseLocalSpaceGravity, Contents.useLocalSpaceGravity);
                        EditorGUILayout.PropertyField(m_TerminalVelocity, Contents.terminalVelocity);
                        EditorGUILayout.PropertyField(m_GravityAccelerationModifier, Contents.gravityAccelerationModifier);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the property fields related to Sphere Cast.
        /// </summary>
        protected virtual void DrawSphereCastProperties()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_SphereCastExpanded = EditorGUILayout.Foldout(m_SphereCastExpanded, Contents.sphereCastSettingsHeader, true, EditorStyles.foldoutHeader);
                if (check.changed)
                    SessionState.SetBool(k_SphereCastExpandedKey, m_SphereCastExpanded);
            }

            if (!m_SphereCastExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_SphereCastRadius, Contents.sphereCastRadius);
                EditorGUILayout.PropertyField(m_SphereCastDistanceBuffer, Contents.sphereCastDistanceBuffer);
                EditorGUILayout.PropertyField(m_SphereCastLayerMask, Contents.sphereCastLayerMask);
                EditorGUILayout.PropertyField(m_SphereCastTriggerInteraction, Contents.sphereCastTriggerInteraction);
            }
        }

        /// <summary>
        /// Draw the property fields related to Events.
        /// </summary>
        protected virtual void DrawEvents()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_EventsExpanded = EditorGUILayout.Foldout(m_EventsExpanded, Contents.eventsHeader, true, EditorStyles.foldoutHeader);
                if (check.changed)
                    SessionState.SetBool(k_EventsExpandedKey, m_EventsExpanded);
            }

            if (!m_EventsExpanded)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_OnGroundedChanged, Contents.onGroundedChanged);
                EditorGUILayout.PropertyField(m_OnGravityLockChanged, Contents.onGravityLockChanged);
            }
        }
    }
}
