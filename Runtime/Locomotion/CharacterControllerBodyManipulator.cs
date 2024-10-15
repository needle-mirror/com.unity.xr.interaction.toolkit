using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Scriptable object that can perform constrained movement of an <see cref="XRMovableBody"/> by using a
    /// <see cref="CharacterController"/> that follows the user's body. Each time this object is about to call
    /// <see cref="CharacterController.Move"/> on the <see cref="characterController"/>, it first ensures that the
    /// <see cref="CharacterController.center"/> and <see cref="CharacterController.height"/> are set such that the
    /// bottom of the capsule matches the position determined by <see cref="XRMovableBody.GetBodyGroundLocalPosition"/>
    /// and the height of the capsule matches <see cref="XROrigin.CameraInOriginSpaceHeight"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterControllerBodyManipulator", menuName = "XR/Locomotion/Character Controller Body Manipulator")]
    [HelpURL(XRHelpURLConstants.k_CharacterControllerBodyManipulator)]
    public class CharacterControllerBodyManipulator : ScriptableConstrainedBodyManipulator
    {
        /// <inheritdoc/>
        public override CollisionFlags lastCollisionFlags => characterController != null ? characterController.collisionFlags : CollisionFlags.None;

        /// <inheritdoc/>
        public override bool isGrounded => characterController == null || characterController.isGrounded;

        /// <summary>
        /// The character controller attached to the <see cref="XRMovableBody.originTransform"/> of the
        /// <see cref="IConstrainedXRBodyManipulator.linkedBody"/>. This is <see langword="null"/> if
        /// <see cref="IConstrainedXRBodyManipulator.linkedBody"/> is <see langword="null"/>.
        /// </summary>
        public CharacterController characterController { get; private set; }

        /// <inheritdoc/>
        public override void OnLinkedToBody(XRMovableBody body)
        {
            base.OnLinkedToBody(body);

            var xrOrigin = body.xrOrigin;
            var origin = xrOrigin.Origin;

            // Try on the Origin GameObject first, and then fallback to the XR Origin GameObject (if different)
            if (!origin.TryGetComponent(out CharacterController foundController) && origin != xrOrigin.gameObject)
                xrOrigin.TryGetComponent(out foundController);

            if (foundController != null)
            {
                characterController = foundController;
                return;
            }

            Debug.LogWarning($"No CharacterController found. Adding one to Origin GameObject '{origin.name}'.", this);
            characterController = origin.AddComponent<CharacterController>();
        }

        /// <inheritdoc/>
        public override void OnUnlinkedFromBody()
        {
            base.OnUnlinkedFromBody();
            characterController = null;
        }

        /// <inheritdoc/>
        public override CollisionFlags MoveBody(Vector3 motion)
        {
            if (linkedBody == null || characterController == null)
                return CollisionFlags.None;

            var xrOrigin = linkedBody.xrOrigin;
            var bodyGroundPosition = linkedBody.GetBodyGroundLocalPosition();
            var capsuleHeight = xrOrigin.CameraInOriginSpaceHeight - bodyGroundPosition.y;
            characterController.height = capsuleHeight;
            characterController.center = new Vector3(bodyGroundPosition.x,
                bodyGroundPosition.y + capsuleHeight * 0.5f + characterController.skinWidth, bodyGroundPosition.z);

            return characterController.Move(motion);
        }
    }
}
