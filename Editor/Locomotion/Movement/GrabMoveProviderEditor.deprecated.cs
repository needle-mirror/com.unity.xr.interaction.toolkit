using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Movement
{
    public partial class GrabMoveProviderEditor
    {
        const string k_IsDeprecatedInputInUseDeprecated = "IsDeprecatedInputInUse has been deprecated. The deprecated grabMoveAction input property has been removed.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="LocomotionProvider.system"/>.</summary>
        [Obsolete("Locomotion System has been removed in XRI 3.0.0 and will be removed in a future version.", true)]
        protected SerializedProperty m_System;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.gravityMode"/>.</summary>
        [Obsolete("Gravity Application Mode has been removed in XRI 3.0.0 and will be removed in a future version.", true)]
        protected SerializedProperty m_GravityApplicationMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ConstrainedMoveProvider.useGravity"/>.</summary>
        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.", true)]
        protected SerializedProperty m_UseGravity;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabMoveProvider.grabMoveAction"/>.</summary>
        [Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.", true)]
        protected SerializedProperty m_GrabMoveAction;

        /// <summary>
        /// Returns whether the deprecated input action property is in use.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the deprecated input action property is in use and needs to be migrated.</returns>
        [Obsolete(k_IsDeprecatedInputInUseDeprecated, true)]
        protected virtual bool IsDeprecatedInputInUse()
        {
            throw new NotSupportedException(k_IsDeprecatedInputInUseDeprecated);
        }

        protected static partial class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="LocomotionProvider.system"/>.</summary>
            [Obsolete("Locomotion System has been removed in XRI 3.0.0 and will be removed in a future version.", true)]
            public static readonly GUIContent system = EditorGUIUtility.TrTextContent("System", "The locomotion system that the snap turn provider will interface with.");

            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.gravityMode"/>.</summary>
            [Obsolete("Gravity Application Mode has been removed in XRI 3.0.0 and will be removed in a future version.", true)]
            public static readonly GUIContent gravityMode = EditorGUIUtility.TrTextContent("Gravity Application Mode", "Controls when gravity begins to take effect.");

            /// <summary><see cref="GUIContent"/> for <see cref="ConstrainedMoveProvider.useGravity"/>.</summary>
            [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.", true)]
            public static readonly GUIContent useGravity = EditorGUIUtility.TrTextContent("Use Gravity", "Controls whether gravity applies to constrained axes when a Character Controller is used. Ignored when a Gravity Provider component is found in the scene.");

            /// <summary><see cref="GUIContent"/> for <see cref="GrabMoveProvider.grabMoveAction"/>.</summary>
            [Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.", true)]
            public static readonly GUIContent grabMoveAction = EditorGUIUtility.TrTextContent("Grab Move Action", "(Deprecated) The Input System Action that will be used to perform grab movement while held. Must be a Button Control.");

            /// <summary><see cref="GUIContent"/> for the warning shown when the deprecated grab move action is in use.</summary>
            [Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.", true)]
            public static readonly GUIContent deprecatedInputInUse = EditorGUIUtility.TrTextContent("Deprecated input property is being used. This property will be removed in a future version. Please convert this to use the newer Grab Move Input.");
        }
    }
}
