using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="BaseTeleportationInteractable"/>.
    /// </summary>
    [CustomEditor(typeof(BaseTeleportationInteractable), true), CanEditMultipleObjects]
    public class BaseTeleportationInteractableEditor : XRBaseInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="BaseTeleportationInteractable.teleportationProvider"/>.</summary>
        protected SerializedProperty m_TeleportationProvider;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="BaseTeleportationInteractable.matchOrientation"/>.</summary>
        protected SerializedProperty m_MatchOrientation;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="BaseTeleportationInteractable.teleportTrigger"/>.</summary>
        protected SerializedProperty m_TeleportTrigger;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            m_TeleportationProvider = serializedObject.FindProperty("m_TeleportationProvider");
            m_MatchOrientation = serializedObject.FindProperty("m_MatchOrientation");
            m_TeleportTrigger = serializedObject.FindProperty("m_TeleportTrigger");

            // Set default expanded for some foldouts
            const string initializedKey = "XRI." + nameof(BaseTeleportationInteractableEditor) + ".Initialized";
            if (!SessionState.GetBool(initializedKey, false))
            {
                SessionState.SetBool(initializedKey, true);
                m_MatchOrientation.isExpanded = true;
            }
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            base.DrawProperties();

            EditorGUILayout.Space();

            DrawTeleportationConfiguration();
        }

        /// <summary>
        /// Draw the Teleportation Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawTeleportationConfigurationNested"/>
        protected virtual void DrawTeleportationConfiguration()
        {
            m_MatchOrientation.isExpanded = EditorGUILayout.Foldout(m_MatchOrientation.isExpanded, EditorGUIUtility.TrTempContent("Teleportation Configuration"), true);
            if (m_MatchOrientation.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawTeleportationConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Teleportation Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawTeleportationConfiguration"/>
        protected virtual void DrawTeleportationConfigurationNested()
        {
            EditorGUILayout.PropertyField(m_MatchOrientation);
            EditorGUILayout.PropertyField(m_TeleportTrigger);
            EditorGUILayout.PropertyField(m_TeleportationProvider);
        }
    }
}
