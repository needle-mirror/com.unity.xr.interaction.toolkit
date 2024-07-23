using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Interface defining the contract for trackers that can supply attach point velocity data
    /// and for updating that data.
    /// This includes both linear velocity and angular velocity.
    /// </summary>
    /// <seealso cref="AttachPointVelocityTracker"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
    public interface IAttachPointVelocityTracker : IAttachPointVelocityProvider
    {
        /// <summary>
        /// Updates attach point velocity data using only the attachment transform.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        void UpdateAttachPointVelocityData(Transform attachTransform);

        /// <summary>
        /// Updates attach point velocity data using the attachment transform and an XR Origin Transform.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform for relative calculations.</param>
        void UpdateAttachPointVelocityData(Transform attachTransform, Transform xrOriginTransform);
    }
}
