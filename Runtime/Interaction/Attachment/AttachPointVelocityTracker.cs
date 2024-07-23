using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Tracks the velocity and angular velocity of an attachment point in a XR interaction context.
    /// It uses a cache to calculate these velocities over a series of frames.
    /// </summary>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
    public class AttachPointVelocityTracker : IAttachPointVelocityTracker
    {
        // Stores the time when the velocity calculation started for the current frame.
        float m_DeltaTimeStart;

        // The number of frames over which the velocity will be calculated.
        const int k_VelocityUpdateInterval = 6;

        // The current frame counter.
        int m_FrameOn;

        // Caches for storing position and normal vectors over a series of frames.
        readonly Vector3[] m_VelocityPositionsCache = new Vector3[k_VelocityUpdateInterval];
        readonly Vector3[] m_VelocityNormalsCache = new Vector3[k_VelocityUpdateInterval];

        // Sums of position and normal vectors used for velocity calculation.
        Vector3 m_VelocityPositionsSum;
        Vector3 m_VelocityNormalsSum;

        // Calculated attach point linear and angular velocities.
        Vector3 m_AttachPointVelocity;
        Vector3 m_AttachPointAngularVelocity;

        /// <inheritdoc />
        public void UpdateAttachPointVelocityData(Transform attachTransform)
        {
            UpdateAttachPointVelocityData(attachTransform, false);
        }

        /// <inheritdoc />
        public void UpdateAttachPointVelocityData(Transform attachTransform, Transform xrOriginTransform)
        {
            UpdateAttachPointVelocityData(attachTransform, true, xrOriginTransform);
        }

        /// <summary>
        /// Update velocity data from the change in location over time of the attach point transform. Takes into account whether to use XR Origin Transform.
        /// </summary>
        /// <param name="attachTransformRef">The transform of the attachment point.</param>
        /// <param name="useXROriginTransform">Whether to use the XR Origin Transform.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform, if used.</param>
        void UpdateAttachPointVelocityData(Transform attachTransformRef, bool useXROriginTransform, Transform xrOriginTransform = null)
        {
            var currentAttachTransform = attachTransformRef;

            if (m_FrameOn < k_VelocityUpdateInterval)
            {
                m_VelocityPositionsCache[m_FrameOn] = useXROriginTransform ? xrOriginTransform!.InverseTransformPoint(currentAttachTransform.position) : currentAttachTransform.position;
                m_VelocityPositionsSum += m_VelocityPositionsCache[m_FrameOn];
                m_VelocityNormalsCache[m_FrameOn] = useXROriginTransform ? xrOriginTransform!.InverseTransformVector(currentAttachTransform.up) : currentAttachTransform.up;
                m_VelocityNormalsSum += m_VelocityNormalsCache[m_FrameOn];
            }
            else
            {
                var frameIndex = m_FrameOn % k_VelocityUpdateInterval;

                var deltaTime = Time.unscaledTime - m_DeltaTimeStart;

                var newPosition = useXROriginTransform ? xrOriginTransform!.InverseTransformPoint(currentAttachTransform.position) : currentAttachTransform.position;
                var newNormal = useXROriginTransform ? xrOriginTransform!.InverseTransformVector(currentAttachTransform.up) : currentAttachTransform.up;

                var newPositionsSum = m_VelocityPositionsSum - m_VelocityPositionsCache[frameIndex] + newPosition;
                var newNormalsSum = m_VelocityNormalsSum - m_VelocityNormalsCache[frameIndex] + newNormal;
                m_AttachPointVelocity = (newPositionsSum - m_VelocityPositionsSum) / deltaTime / k_VelocityUpdateInterval;

                var fromDirection = m_VelocityNormalsSum / k_VelocityUpdateInterval;
                var toDirection = newNormalsSum / k_VelocityUpdateInterval;

                var rotation = Quaternion.FromToRotation(fromDirection, toDirection);
                var rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                m_AttachPointAngularVelocity = rotationRate / deltaTime;

                m_VelocityPositionsCache[frameIndex] = newPosition;
                m_VelocityNormalsCache[frameIndex] = newNormal;
                m_VelocityPositionsSum = newPositionsSum;
                m_VelocityNormalsSum = newNormalsSum;
            }

            m_DeltaTimeStart = Time.unscaledTime;
            m_FrameOn++;
        }

        /// <summary>
        /// Resets the velocity tracking data.
        /// </summary>
        internal void ResetVelocityTracking()
        {
            m_FrameOn = 0;
            m_VelocityPositionsSum = Vector3.zero;
            m_VelocityNormalsSum = Vector3.zero;
            m_AttachPointVelocity = Vector3.zero;
            m_AttachPointAngularVelocity = Vector3.zero;
            Array.Clear(m_VelocityPositionsCache, 0, k_VelocityUpdateInterval);
            Array.Clear(m_VelocityNormalsCache, 0, k_VelocityUpdateInterval);
        }

        /// <inheritdoc />
        public Vector3 GetAttachPointVelocity()
        {
            return m_AttachPointVelocity;
        }

        /// <inheritdoc />
        public Vector3 GetAttachPointAngularVelocity()
        {
            return m_AttachPointAngularVelocity;
        }
    }
}