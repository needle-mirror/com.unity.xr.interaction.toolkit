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
    }
}
