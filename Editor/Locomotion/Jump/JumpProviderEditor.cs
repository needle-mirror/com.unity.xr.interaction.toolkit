using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Jump
{
    /// <summary>
    /// Custom editor for a <see cref="JumpProvider"/>.
    /// </summary>
    [CustomEditor(typeof(JumpProvider), true), CanEditMultipleObjects]
    public class JumpProviderEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.mediator"/>.</summary>
        protected SerializedProperty m_Mediator;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.transformationPriority"/>.</summary>
        protected SerializedProperty m_TransformationPriority;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.disableGravityDuringJump"/>.</summary>
        protected SerializedProperty m_DisableGravityDuringJump;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.unlimitedInAirJumps"/>.</summary>
        protected SerializedProperty m_UnlimitedInAirJumps;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.inAirJumpCount"/>.</summary>
        protected SerializedProperty m_InAirJumpCount;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.jumpForgivenessWindow"/>.</summary>
        protected SerializedProperty m_JumpForgivenessWindow;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.jumpHeight"/>.</summary>
        protected SerializedProperty m_JumpHeight;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.variableHeightJump"/>.</summary>
        protected SerializedProperty m_VariableHeightJump;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.minJumpHoldTime"/>.</summary>
        protected SerializedProperty m_MinJumpHoldTime;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.maxJumpHoldTime"/>.</summary>
        protected SerializedProperty m_MaxJumpHoldTime;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.earlyOutDecelerationSpeed"/>.</summary>
        protected SerializedProperty m_EarlyOutDecelerationSpeed;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="JumpProvider.jumpInput"/>.</summary>
        protected SerializedProperty m_JumpInput;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.mediator"/>.</summary>
            public static readonly GUIContent mediator = EditorGUIUtility.TrTextContent("Mediator", "The locomotion mediator that the grab move provider will interface with.");
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.transformationPriority"/>.</summary>
            public static readonly GUIContent transformationPriority = EditorGUIUtility.TrTextContent("Transformation Priority", "The queue order of this provider's transformations of the XR Origin. The lower the value, the earlier the transformations are applied.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.disableGravityDuringJump"/>.</summary>
            public static readonly GUIContent disableGravityDuringJump = EditorGUIUtility.TrTextContent("Disable Gravity During Jump", "Disable gravity during the jump. This will result in a more floaty jump.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.unlimitedInAirJumps"/>.</summary>
            public static readonly GUIContent unlimitedInAirJumps = EditorGUIUtility.TrTextContent("Unlimited In Air Jumps", "Allow player to jump without being grounded.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.inAirJumpCount"/>.</summary>
            public static readonly GUIContent inAirJumpCount = EditorGUIUtility.TrTextContent("In Air Jump Count", "The number of times a player can jump before landing.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.jumpForgivenessWindow"/>.</summary>
            public static readonly GUIContent jumpForgivenessWindow = EditorGUIUtility.TrTextContent("Jump Forgiveness Window", "The time window after leaving the ground that a jump can still be performed. Sometimes known as coyote time.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.jumpHeight"/>.</summary>
            public static readonly GUIContent jumpHeight = EditorGUIUtility.TrTextContent("Jump Height", "The height (approximately in meters) the player will be when reaching the apex of the jump.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.variableHeightJump"/>.</summary>
            public static readonly GUIContent variableHeightJump = EditorGUIUtility.TrTextContent("Variable Height Jump", "Allow the player to stop their jump early when input is released before reaching the maximum jump height.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.minJumpHoldTime"/>.</summary>
            public static readonly GUIContent minJumpHoldTime = EditorGUIUtility.TrTextContent("Min Jump Hold Time", "The minimum amount of time the jump will execute for.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.maxJumpHoldTime"/>.</summary>
            public static readonly GUIContent maxJumpHoldTime = EditorGUIUtility.TrTextContent("Max Jump Hold Time", "The maximum time the jump button can be held to reach the maximum jump height.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.earlyOutDecelerationSpeed"/>.</summary>
            public static readonly GUIContent earlyOutDecelerationSpeed = EditorGUIUtility.TrTextContent("Early Out Deceleration Speed", "The speed at which the player will decelerate when the jump button is released early.");

            /// <summary><see cref="GUIContent"/> for <see cref="JumpProvider.jumpInput"/>.</summary>
            public static readonly GUIContent jumpInput = EditorGUIUtility.TrTextContent("Jump Input", "The input used to trigger a jump.");

            /// <summary><see cref="GUIContent"/> for Jump Settings header.</summary>
            public static readonly GUIContent jumpSettingsHeader = EditorGUIUtility.TrTextContent("Jump Settings");
            /// <summary><see cref="GUIContent"/> for Air Jump Settings header.</summary>
            public static readonly GUIContent airJumpSettingsHeader = EditorGUIUtility.TrTextContent("Air Jump Settings");
            /// <summary><see cref="GUIContent"/> for Variable Height Jump Settings header.</summary>
            public static readonly GUIContent variableHeightJumpSettingsHeader = EditorGUIUtility.TrTextContent("Variable Height Jump Settings");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_Mediator = serializedObject.FindProperty(nameof(m_Mediator));
            m_TransformationPriority = serializedObject.FindProperty(nameof(m_TransformationPriority));
            m_DisableGravityDuringJump = serializedObject.FindProperty(nameof(m_DisableGravityDuringJump));
            m_UnlimitedInAirJumps = serializedObject.FindProperty(nameof(m_UnlimitedInAirJumps));
            m_InAirJumpCount = serializedObject.FindProperty(nameof(m_InAirJumpCount));
            m_JumpForgivenessWindow = serializedObject.FindProperty(nameof(m_JumpForgivenessWindow));
            m_JumpHeight = serializedObject.FindProperty(nameof(m_JumpHeight));
            m_VariableHeightJump = serializedObject.FindProperty(nameof(m_VariableHeightJump));
            m_MinJumpHoldTime = serializedObject.FindProperty(nameof(m_MinJumpHoldTime));
            m_MaxJumpHoldTime = serializedObject.FindProperty(nameof(m_MaxJumpHoldTime));
            m_EarlyOutDecelerationSpeed = serializedObject.FindProperty(nameof(m_EarlyOutDecelerationSpeed));
            m_JumpInput = serializedObject.FindProperty(nameof(m_JumpInput));
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

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.jumpSettingsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_JumpHeight, Contents.jumpHeight);
            EditorGUILayout.PropertyField(m_JumpForgivenessWindow, Contents.jumpForgivenessWindow);
            EditorGUILayout.PropertyField(m_DisableGravityDuringJump, Contents.disableGravityDuringJump);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.airJumpSettingsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_UnlimitedInAirJumps, Contents.unlimitedInAirJumps);
            if (!m_UnlimitedInAirJumps.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_InAirJumpCount, Contents.inAirJumpCount);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.variableHeightJumpSettingsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_VariableHeightJump, Contents.variableHeightJump);
            if (m_VariableHeightJump.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_MinJumpHoldTime, Contents.minJumpHoldTime);
                    EditorGUILayout.PropertyField(m_MaxJumpHoldTime, Contents.maxJumpHoldTime);
                    EditorGUILayout.PropertyField(m_EarlyOutDecelerationSpeed, Contents.earlyOutDecelerationSpeed);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_JumpInput, Contents.jumpInput);
        }
    }
}
