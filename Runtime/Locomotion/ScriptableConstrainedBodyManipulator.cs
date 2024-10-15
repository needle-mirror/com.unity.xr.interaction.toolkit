namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Base for a scriptable object that can perform movement of an <see cref="XRMovableBody"/> that is constrained
    /// by collision based on where the user's body is.
    /// </summary>
    public abstract class ScriptableConstrainedBodyManipulator : ScriptableObject, IConstrainedXRBodyManipulator
    {
        /// <inheritdoc/>
        public XRMovableBody linkedBody { get; private set; }

        /// <inheritdoc/>
        public abstract CollisionFlags lastCollisionFlags { get; }

        /// <inheritdoc/>
        public abstract bool isGrounded { get; }

        /// <inheritdoc/>
        public virtual void OnLinkedToBody(XRMovableBody body)
        {
            linkedBody = body;
        }

        /// <inheritdoc/>
        public virtual void OnUnlinkedFromBody()
        {
            linkedBody = null;
        }

        /// <inheritdoc/>
        public abstract CollisionFlags MoveBody(Vector3 motion);
    }
}
