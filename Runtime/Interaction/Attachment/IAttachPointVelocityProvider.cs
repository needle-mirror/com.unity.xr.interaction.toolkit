using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Interface defining the contract for providers that can supply attach point velocity data.
    /// This includes both linear velocity and angular velocity.
    /// </summary>
    /// <seealso cref="AttachPointVelocityTracker"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
    public interface IAttachPointVelocityProvider
    {
        /// <summary>
        /// Retrieves the current linear velocity of the attachment point.
        /// </summary>
        /// <returns>The current linear velocity of the attachment point as a <see cref="Vector3"/>.</returns>
        Vector3 GetAttachPointVelocity();

        /// <summary>
        /// Retrieves the current angular velocity of the attachment point.
        /// </summary>
        /// <returns>The current angular velocity of the attachment point as a <see cref="Vector3"/>.</returns>
        Vector3 GetAttachPointAngularVelocity();
    }

    /// <summary>
    /// Extension methods for <see cref="IAttachPointVelocityProvider"/>.
    /// </summary>
    /// <seealso cref="IAttachPointVelocityProvider"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
    public static class AttachPointVelocityProviderExtensions
    {
        /// <summary>
        /// Retrieves the current linear velocity of the attachment point, transformed by the XR Origin.
        /// </summary>
        /// <param name="provider">The <see cref="IAttachPointVelocityProvider"/> to retrieve the velocity from.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform for relative calculation.</param>
        /// <returns>Returns the transformed current linear velocity of the attachment point as a <see cref="Vector3"/>.</returns>
        public static Vector3 GetAttachPointVelocity(this IAttachPointVelocityProvider provider, Transform xrOriginTransform)
        {
            return xrOriginTransform.TransformDirection(provider.GetAttachPointVelocity());
        }

        /// <summary>
        /// Retrieves the current angular velocity of the attachment point, transformed by the XR Origin.
        /// </summary>
        /// <param name="provider">The <see cref="IAttachPointVelocityProvider"/> to retrieve the velocity from.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform for relative calculation.</param>
        /// <returns>Returns the transformed current angular velocity of the attachment point as a <see cref="Vector3"/>.</returns>
        public static Vector3 GetAttachPointAngularVelocity(this IAttachPointVelocityProvider provider, Transform xrOriginTransform)
        {
            return xrOriginTransform.TransformDirection(provider.GetAttachPointAngularVelocity());
        }
    }
}