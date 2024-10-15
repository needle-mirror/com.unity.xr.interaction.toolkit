using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Movement
{
    /// <summary>
    /// Custom editor for a <see cref="GrabMoveProvider"/>.
    /// </summary>
    public partial class GrabMoveProviderEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.system"/>.</summary>
        [Obsolete("Locomotion System has been removed in XRI 3.0.0 and will be removed in a future version.")]
        protected SerializedProperty m_System;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.gravityMode"/>.</summary>
        [Obsolete("Gravity Application Mode has been removed in XRI 3.0.0 and will be removed in a future version.")]
        protected SerializedProperty m_GravityApplicationMode;

        protected static partial class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.system"/>.</summary>
            [Obsolete("Locomotion System has been removed in XRI 3.0.0 and will be removed in a future version.")]
            public static readonly GUIContent system = EditorGUIUtility.TrTextContent("System", "The locomotion system that the snap turn provider will interface with.");

            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.gravityMode"/>.</summary>
            [Obsolete("Gravity Application Mode has been removed in XRI 3.0.0 and will be removed in a future version.")]
            public static readonly GUIContent gravityMode = EditorGUIUtility.TrTextContent("Gravity Application Mode", "Controls when gravity begins to take effect.");

        }
    }
}
