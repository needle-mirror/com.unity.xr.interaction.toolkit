using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// An anchor is a teleportation destination which teleports the user to a pre-determined
    /// specific position and/or rotation.
    /// </summary>
    /// <seealso cref="TeleportationArea"/>
    /// <seealso cref="TeleportationMultiAnchorVolume"/>
    [AddComponentMenu("XR/Teleportation Anchor", 11)]
    [HelpURL(XRHelpURLConstants.k_TeleportationAnchor)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportationAnchor : BaseTeleportationInteractable
    {
        [SerializeField]
        [Tooltip("The Transform that represents the teleportation destination.")]
        Transform m_TeleportAnchorTransform;

        /// <summary>
        /// The <see cref="Transform"/> that represents the teleportation destination.
        /// </summary>
        public Transform teleportAnchorTransform
        {
            get => m_TeleportAnchorTransform;
            set => m_TeleportAnchorTransform = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
            if (m_TeleportAnchorTransform == null)
                m_TeleportAnchorTransform = transform;
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            m_TeleportAnchorTransform = transform;
        }

        /// <summary>
        /// Unity calls this when drawing gizmos.
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (m_TeleportAnchorTransform == null)
                return;

            Gizmos.color = Color.blue;
            GizmoHelpers.DrawWireCubeOriented(m_TeleportAnchorTransform.position, m_TeleportAnchorTransform.rotation, 1f);

            GizmoHelpers.DrawAxisArrows(m_TeleportAnchorTransform, 1f);
        }

        /// <inheritdoc />
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            return m_TeleportAnchorTransform;
        }

        /// <summary>
        /// Attempts to queue a request to teleport to this anchor. This method can be called from script to initiate teleportation
        /// manually rather than relying on the interaction system to initiate teleportation. May not succeed in teleporting
        /// if the anchor Transform is destroyed or there is no <see cref="TeleportationProvider"/>.
        /// </summary>
        /// <remarks>
        /// Due to script execution order of the <seealso cref="TeleportationProvider"/>, depending on when this method is called,
        /// teleportation of the XR Origin may not occur until the next frame.
        /// </remarks>
        // void return type to allow the method to be called from a UnityEvent
        public void RequestTeleport() => SendTeleportRequest(null);

        /// <inheritdoc />
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (m_TeleportAnchorTransform == null)
                return false;

            teleportRequest.destinationPosition = m_TeleportAnchorTransform.position;
            teleportRequest.destinationRotation = m_TeleportAnchorTransform.rotation;
            return true;
        }

        [ContextMenu("Teleport to anchor", false)]
        void RequestTeleportFromEditor() => RequestTeleport();

        [ContextMenu("Teleport to anchor", true)]
        bool RequestTeleportFromEditorValidate() => Application.isPlaying;
    }
}
