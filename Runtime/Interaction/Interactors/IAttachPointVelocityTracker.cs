namespace UnityEngine.XR.Interaction.Toolkit.Interaction
{
    /// <summary>
    /// Interface defining the contract for trackers that can supply attach point velocity data
    /// and for updating that data.
    /// This includes both linear velocity and angular velocity.
    /// </summary>
    /// <seealso cref="AttachPointVelocityTracker"/>
    public interface IAttachPointVelocityTracker : IAttachPointVelocityProvider
    {
        /// <summary>
        /// Updates attach point velocity data using only the attachment transform.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        public void UpdateAttachPointVelocityData(Transform attachTransform);

        /// <summary>
        /// Updates attach point velocity data using the attachment transform and an XR Origin Transform.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform for relative calculations.</param>
        public void UpdateAttachPointVelocityData(Transform attachTransform, Transform xrOriginTransform);
    }
}
