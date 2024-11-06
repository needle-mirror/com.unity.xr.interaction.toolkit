using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Tracks the velocity and angular velocity of an attachment point in an XR interaction context.
    /// It uses weighted linear regression to calculate velocities over a series of frames, providing smooth and accurate results.
    /// </summary>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
    public class AttachPointVelocityTracker : IAttachPointVelocityTracker
    {
        // The number of frames over which the velocity will be calculated.
        const int k_BufferSize = 20;

        // Minimum delta time to avoid division by zero.
        const float k_MinimumDeltaTime = 0.00001f;

        // Cache for storing position and time data over a series of frames.
        readonly CircularBuffer<(Vector3 position, float time)> m_PositionTimeBuffer = new CircularBuffer<(Vector3, float)>(k_BufferSize);

        // Cache for storing rotation and time data over a series of frames.
        readonly CircularBuffer<(Quaternion rotation, float time)> m_RotationTimeBuffer = new CircularBuffer<(Quaternion, float)>(k_BufferSize);

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
        /// Updates velocity data based on the attachment point's movement over time, considering XR Origin Transform if applicable.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        /// <param name="useXROriginTransform">Whether to use the XR Origin Transform.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform, if used.</param>
        void UpdateAttachPointVelocityData(Transform attachTransform, bool useXROriginTransform, Transform xrOriginTransform = null)
        {
            float currentTime = Time.unscaledTime;
            bool canUseOriginTransform = useXROriginTransform && xrOriginTransform != null;

            // Calculate position and rotation in the appropriate space
            var attachPose = attachTransform.GetWorldPose();
            Vector3 currentPosition = canUseOriginTransform
                ? xrOriginTransform.InverseTransformPoint(attachPose.position)
                : attachPose.position;
            Quaternion currentRotation = canUseOriginTransform
                ? Quaternion.Inverse(xrOriginTransform.rotation) * attachPose.rotation
                : attachPose.rotation;

            // Update position and time buffer
            m_PositionTimeBuffer.Add((currentPosition, currentTime));

            // Update rotation and time buffer
            m_RotationTimeBuffer.Add((currentRotation, currentTime));

            // Calculate linear velocity using weighted linear regression
            m_AttachPointVelocity = m_PositionTimeBuffer.count > 1 ? CalculateVelocityWithWeightedLinearRegression() : Vector3.zero;

            // Calculate angular velocity using weighted regression
            m_AttachPointAngularVelocity = m_RotationTimeBuffer.count > 1 ? CalculateAngularVelocityWithWeightedRegression() : Vector3.zero;
        }

        /// <summary>
        /// Calculates velocity using weighted linear regression on the buffered position and time data.
        /// This method gives more weight to recent samples, providing a more accurate and responsive velocity estimation.
        /// </summary>
        /// <returns>The calculated velocity vector based on recent movement history.</returns>
        Vector3 CalculateVelocityWithWeightedLinearRegression()
        {
            int n = m_PositionTimeBuffer.count;

            // Check if we have enough data points
            if (n < 2)
                return Vector3.zero;

            Vector3 sumPos = Vector3.zero;
            float sumTime = 0f;
            Vector3 sumPosTime = Vector3.zero;
            float sumTimeSquared = 0f;
            float sumWeights = 0f;

            float startTime = m_PositionTimeBuffer[0].time;
            float endTime = m_PositionTimeBuffer[n - 1].time;
            float timeRange = endTime - startTime;

            // Check for very small time range
            if (timeRange < k_MinimumDeltaTime)
                return (Mathf.Approximately(timeRange, 0f)) ? Vector3.zero : (m_PositionTimeBuffer[n - 1].position - m_PositionTimeBuffer[0].position) / timeRange;

            for (int i = 0; i < n; i++)
            {
                float t = m_PositionTimeBuffer[i].time - startTime;
                Vector3 pos = m_PositionTimeBuffer[i].position;

                // Calculate weight (more recent samples have higher weight).
                // Weight ranges from 1 to 2.
                float weight = 1f + (t / timeRange);

                sumPos += pos * weight;
                sumTime += t * weight;
                sumPosTime += pos * (t * weight);
                sumTimeSquared += t * t * weight;
                sumWeights += weight;
            }

            float denominator = sumWeights * sumTimeSquared - sumTime * sumTime;
            if (Mathf.Approximately(denominator, 0f))
                return Vector3.zero;

            Vector3 velocity = (sumWeights * sumPosTime - sumPos * sumTime) / denominator;
            return velocity;
        }

        /// <summary>
        /// Calculates angular velocity using weighted regression on the buffered rotation and time data.
        /// This method gives more weight to recent samples, providing a more accurate and responsive angular velocity estimation.
        /// </summary>
        /// <returns>The calculated angular velocity vector based on recent rotation history.</returns>
        Vector3 CalculateAngularVelocityWithWeightedRegression()
        {
            int n = m_RotationTimeBuffer.count;

            if (n < 2)
            {
                return Vector3.zero;
            }

            Vector3 sumAngularDisplacement = Vector3.zero;
            float sumTime = 0f;
            Vector3 sumAngularDisplacementTime = Vector3.zero;
            float sumTimeSquared = 0f;
            float sumWeights = 0f;

            float startTime = m_RotationTimeBuffer[0].time;
            float endTime = m_RotationTimeBuffer[n - 1].time;
            float timeRange = endTime - startTime;

            if (timeRange < k_MinimumDeltaTime)
                return Vector3.zero;

            Quaternion startRotation = m_RotationTimeBuffer[0].rotation;

            for (int i = 1; i < n; i++)
            {
                float t = m_RotationTimeBuffer[i].time - startTime;
                Quaternion rotation = m_RotationTimeBuffer[i].rotation;

                Quaternion deltaRotation = rotation * Quaternion.Inverse(startRotation);
                deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f)
                    angle -= 360f;

                Vector3 angularDisplacement = axis * (angle * Mathf.Deg2Rad);

                // Weight ranges from 1 to 2
                float weight = 1f + (t / timeRange);

                sumAngularDisplacement += angularDisplacement * weight;
                sumTime += t * weight;
                sumAngularDisplacementTime += angularDisplacement * (t * weight);
                sumTimeSquared += t * t * weight;
                sumWeights += weight;
            }

            float denominator = sumWeights * sumTimeSquared - sumTime * sumTime;
            if (Mathf.Approximately(denominator, 0f))
                return Vector3.zero;

            Vector3 angularVelocity = (sumWeights * sumAngularDisplacementTime - sumAngularDisplacement * sumTime) / denominator;
            return angularVelocity;
        }

        /// <summary>
        /// Resets the velocity tracking data.
        /// </summary>
        internal void ResetVelocityTracking()
        {
            m_PositionTimeBuffer.Clear();
            m_RotationTimeBuffer.Clear();
            m_AttachPointVelocity = Vector3.zero;
            m_AttachPointAngularVelocity = Vector3.zero;
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
