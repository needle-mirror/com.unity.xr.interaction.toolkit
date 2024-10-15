using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    public partial class XRRayInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.manipulateAttachTransform"/>.</summary>
        [Obsolete("m_AllowAnchorControl has been renamed in version 3.0.0. Use m_ManipulateAttachTransform instead. (UnityUpgradable) -> m_ManipulateAttachTransform")]
        protected SerializedProperty m_AllowAnchorControl;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateReferenceFrame"/>.</summary>
        [Obsolete("m_AnchorRotateReferenceFrame has been renamed in version 3.0.0. Use m_RotateReferenceFrame instead. (UnityUpgradable) -> m_RotateReferenceFrame")]
        protected SerializedProperty m_AnchorRotateReferenceFrame;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateMode"/>.</summary>
        [Obsolete("m_AnchorRotationMode has been renamed in version 3.0.0. Use m_RotateMode instead. (UnityUpgradable) -> m_RotateMode")]
        protected SerializedProperty m_AnchorRotationMode;

        protected static partial class Contents
        {
            /// <summary>The help box message when the hit detection type is cone cast but the line type is not straight line.</summary>
            [Obsolete("coneCastRequiresStraightLineWarning has been deprecated in version 3.0.6. The warning message is no longer necessary since the configuration is now supported.")]
            public static readonly GUIContent coneCastRequiresStraightLineWarning = EditorGUIUtility.TrTextContent("Cone Cast requires Straight Line.");
        }
    }
}
