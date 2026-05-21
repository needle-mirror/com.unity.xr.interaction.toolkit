using System.Collections.Generic;
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

        // The threshold of the attach point must exceed to take into account camera velocity when applicable
        const float k_AttachPointVelocityThreshold = 0.05f;

        // Minimum delta time to avoid division by zero.
        const float k_MinimumDeltaTime = 0.00001f;

        // Cache for storing position and time data over a series of frames.
        readonly CircularBuffer<(Vector3 position, float time)> m_PositionTimeBuffer = new CircularBuffer<(Vector3, float)>(k_BufferSize);

        // Cache for storing camera position and time data over a series of frames.
        readonly CircularBuffer<(Vector3 position, float time)> m_CameraPositionTimeBuffer = new CircularBuffer<(Vector3, float)>(k_BufferSize);

        // Cache for storing rotation and time data over a series of frames.
        readonly CircularBuffer<(Quaternion rotation, float time)> m_RotationTimeBuffer = new CircularBuffer<(Quaternion, float)>(k_BufferSize);

        // Cache for storing reference to XR Origin
        static readonly Dictionary<Transform, XROrigin> s_XROriginCache = new Dictionary<Transform, XROrigin>();

        // Calculated attach point linear and angular velocities.
        Vector3 m_AttachPointVelocity;
        Vector3 m_AttachPointAngularVelocity;

        /// <inheritdoc />
        public void UpdateAttachPointVelocityData(Transform attachTransform)
        {
            CalculateAndUpdateAttachPointVelocity(attachTransform);
        }

        /// <inheritdoc />
        public void UpdateAttachPointVelocityData(Transform attachTransform, Transform xrOriginTransform)
        {
            CalculateAndUpdateAttachPointVelocity(attachTransform, xrOriginTransform);
        }

        /// <summary>
        /// Updates velocity data based on the attachment point's movement over time, considering XR Origin Transform and XR Origin camera transform if applicable.
        /// </summary>
        /// <param name="attachTransform">The transform of the attachment point.</param>
        /// <param name="xrOriginTransform">The XR Origin Transform, if used.</param>
        /// <remarks>When useXROriginTransform is true, this function will attempt to get the <see cref="XROrigin"/> camera to use when calculating attach point velocity.</remarks>
        void CalculateAndUpdateAttachPointVelocity(Transform attachTransform, Transform xrOriginTransform = null)
        {
            bool canUseXROriginTransform = xrOriginTransform != null;
            Transform cameraTransform = null;
            if (canUseXROriginTransform)
            {
                // Cache XR Origin component to access camera for velocity calculations, if possible
                if (!s_XROriginCache.TryGetValue(xrOriginTransform, out var xrOrigin) && xrOriginTransform.TryGetComponent(out xrOrigin))
                    s_XROriginCache.Add(xrOriginTransform, xrOrigin);
                cameraTransform = xrOrigin != null ? xrOrigin.Camera.transform : null;
            }

            float currentTime = Time.unscaledTime;
            bool canUseCameraTransform = cameraTransform != null;

            // Calculate position and rotation in the appropriate space
            var attachPose = attachTransform.GetWorldPose();
            Vector3 currentPosition = canUseXROriginTransform
                ? xrOriginTransform.InverseTransformPoint(attachPose.position)
                : attachPose.position;
            Quaternion currentRotation = canUseXROriginTransform
                ? Quaternion.Inverse(xrOriginTransform.rotation) * attachPose.rotation
                : attachPose.rotation;

            // Update position and time buffer
            m_PositionTimeBuffer.Add((currentPosition, currentTime));

            // Update rotation and time buffer
            m_RotationTimeBuffer.Add((currentRotation, currentTime));

            // Calculate linear velocity using weighted linear regression
            m_AttachPointVelocity = m_PositionTimeBuffer.count > 1 ? CalculateVelocityWithWeightedLinearRegression(m_PositionTimeBuffer) : Vector3.zero;

            // Calculate angular velocity using weighted regression
            m_AttachPointAngularVelocity = m_RotationTimeBuffer.count > 1 ? CalculateAngularVelocityWithWeightedRegression(m_RotationTimeBuffer) : Vector3.zero;

            // Calculate the camera velocity to account for physical body movement if the camera transform is provided,
            // If the camera and the attach transform are both moving, this could be physical walking movement.
            // The velocity of the attach transform should subtract the camera velocity to account for the offset.
            if (canUseCameraTransform)
            {
                Vector3 cameraPositionWithRespectToOrigin = xrOriginTransform.InverseTransformPoint(cameraTransform.position);

                // Update position and time buffer of camera with respect to the origin
                m_CameraPositionTimeBuffer.Add((cameraPositionWithRespectToOrigin, currentTime));

                // Check attach transform velocity is over threshold to prevent camera velocity from being subtracted when only head is moving.
                if (Mathf.Abs(m_AttachPointVelocity.sqrMagnitude) > k_AttachPointVelocityThreshold)
                {
                    // Calculate camera linear velocity using weighted linear regression
                    var cameraVelocity = m_CameraPositionTimeBuffer.count > 1 ? CalculateVelocityWithWeightedLinearRegression(m_CameraPositionTimeBuffer) : Vector3.zero;
                    m_AttachPointVelocity -= cameraVelocity;
                }
            }
        }

        /// <summary>
        /// Calculates velocity using weighted linear regression on the buffered position and time data.
        /// This method gives more weight to recent samples, providing a more accurate and responsive velocity estimation.
        /// </summary>
        /// <param name="positionTimeBuffer">Position and time buffer containing data for weighted linear regression calculation.</param>
        /// <returns>The calculated velocity vector based on recent movement history.</returns>
        static Vector3 CalculateVelocityWithWeightedLinearRegression(CircularBuffer<(Vector3 position, float time)> positionTimeBuffer)
        {
            int n = positionTimeBuffer.count;

            // Check if we have enough data points
            if (n < 2)
                return Vector3.zero;

            Vector3 sumPos = Vector3.zero;
            float sumTime = 0f;
            Vector3 sumPosTime = Vector3.zero;
            float sumTimeSquared = 0f;
            float sumWeights = 0f;

            float startTime = positionTimeBuffer[0].time;
            float endTime = positionTimeBuffer[n - 1].time;
            float timeRange = endTime - startTime;

            // Check for very small time range
            if (timeRange < k_MinimumDeltaTime)
                return (Mathf.Approximately(timeRange, 0f)) ? Vector3.zero : (positionTimeBuffer[n - 1].position - positionTimeBuffer[0].position) / timeRange;

            for (int i = 0; i < n; i++)
            {
                float t = positionTimeBuffer[i].time - startTime;
                Vector3 pos = positionTimeBuffer[i].position;

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
        /// <param name="rotationTimeBuffer">Rotation and time buffer containing data for weighted linear regression calculation.</param>
        /// <returns>The calculated angular velocity vector based on recent rotation history.</returns>
        static Vector3 CalculateAngularVelocityWithWeightedRegression(CircularBuffer<(Quaternion rotation, float time)> rotationTimeBuffer)
        {
            int n = rotationTimeBuffer.count;

            if (n < 2)
            {
                return Vector3.zero;
            }

            Vector3 sumAngularDisplacement = Vector3.zero;
            float sumTime = 0f;
            Vector3 sumAngularDisplacementTime = Vector3.zero;
            float sumTimeSquared = 0f;
            float sumWeights = 0f;

            float startTime = rotationTimeBuffer[0].time;
            float endTime = rotationTimeBuffer[n - 1].time;
            float timeRange = endTime - startTime;

            if (timeRange < k_MinimumDeltaTime)
                return Vector3.zero;

            Quaternion startRotation = rotationTimeBuffer[0].rotation;

            for (int i = 1; i < n; i++)
            {
                float t = rotationTimeBuffer[i].time - startTime;
                Quaternion rotation = rotationTimeBuffer[i].rotation;

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
        public void ResetVelocityTracking()
        {
            m_PositionTimeBuffer.Clear();
            m_CameraPositionTimeBuffer.Clear();
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
