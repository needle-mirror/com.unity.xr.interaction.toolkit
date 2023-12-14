namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Interface for an object that can perform movement of an <see cref="XRMovableBody"/> that is constrained
    /// by collision based on where the user's body is.
    /// </summary>
    /// <seealso cref="CharacterControllerBodyManipulator"/>
    public interface IConstrainedXRBodyManipulator
    {
        /// <summary>
        /// The body whose <see cref="XRMovableBody.originTransform"/> to move.
        /// </summary>
        /// <seealso cref="OnLinkedToBody"/>
        /// <seealso cref="OnUnlinkedFromBody"/>
        XRMovableBody linkedBody { get; }

        /// <summary>
        /// Flags indicating the direction of the collision from the most recent call to <see cref="MoveBody"/>.
        /// </summary>
        CollisionFlags lastCollisionFlags { get; }

        /// <summary>
        /// Whether the <see cref="linkedBody"/> is touching the ground, as of the most recent call to <see cref="MoveBody"/>.
        /// </summary>
        bool isGrounded { get; }

        /// <summary>
        /// Called after the given body links this manipulator to it. The implementation should ensure that
        /// <see cref="linkedBody"/> points to <paramref name="body"/> after this method is called.
        /// </summary>
        /// <param name="body">The body linked with this manipulator.</param>
        /// <seealso cref="XRMovableBody.LinkConstrainedManipulator"/>
        /// <seealso cref="OnUnlinkedFromBody"/>
        void OnLinkedToBody(XRMovableBody body);

        /// <summary>
        /// Called when a body unlinks this manipulator from it. The implementation should ensure that
        /// <see cref="linkedBody"/> returns <see langword="null"/> after this method is called.
        /// </summary>
        /// <seealso cref="XRMovableBody.UnlinkConstrainedManipulator"/>
        /// <seealso cref="OnLinkedToBody"/>
        void OnUnlinkedFromBody();

        /// <summary>
        /// Applies the given motion to <see cref="linkedBody"/>. A collision can constrain the movement from taking place.
        /// </summary>
        /// <param name="motion">Amount of translation to apply.</param>
        /// <returns>Returns flags that indicate the direction of the collision, if there was one.</returns>
        CollisionFlags MoveBody(Vector3 motion);
    }
}