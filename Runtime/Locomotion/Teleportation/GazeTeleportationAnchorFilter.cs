using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Filter for a <see cref="TeleportationMultiAnchorVolume"/> that designates the anchor most aligned with the
    /// camera forward direction, optionally weighted by distance from the user, as the teleportation destination.
    /// Distance calculation uses the camera position projected onto the XZ plane of the XR Origin.
    /// </summary>
    [CreateAssetMenu(fileName = "GazeTeleportationAnchorFilter", menuName = "XR/Locomotion/Gaze Teleportation Anchor Filter")]
    [HelpURL(XRHelpURLConstants.k_GazeTeleportationAnchorFilter)]
    public class GazeTeleportationAnchorFilter : ScriptableObject, ITeleportationVolumeAnchorFilter
    {
        [SerializeField]
        [Range(0f, 180f)]
        [Tooltip("The maximum angle (in degrees) between the camera forward and the direction from the camera to an " +
            "anchor for the anchor to be considered a valid destination.")]
        float m_MaxGazeAngle = 90f;

        /// <summary>
        /// The maximum angle (in degrees) between the camera forward and the direction from the camera to an anchor for
        /// the anchor to be considered a valid destination.
        /// </summary>
        public float maxGazeAngle
        {
            get => m_MaxGazeAngle;
            set => m_MaxGazeAngle = value;
        }

        [SerializeField]
        [Tooltip("The curve used to score an anchor by its angle from the camera forward. The X axis is the normalized " +
            "angle, where 0 is 0 degrees and 1 is the Max Gaze Angle. The Y axis is the score, where a higher value " +
            "means a better destination.")]
        AnimationCurve m_GazeAngleScoreCurve;

        /// <summary>
        /// The curve used to score an anchor by its alignment with the camera forward. The X axis is the normalized
        /// angle between the camera forward and the anchor direction, where 0 is 0 degrees and 1 is
        /// <see cref="maxGazeAngle"/> degrees. The Y axis is the score. The anchor with the highest score is chosen
        /// as the destination.
        /// </summary>
        public AnimationCurve gazeAngleScoreCurve
        {
            get => m_GazeAngleScoreCurve;
            set => m_GazeAngleScoreCurve = value;
        }

        [SerializeField]
        [Tooltip("Whether to weight an anchor's score by its distance from the user.")]
        bool m_EnableDistanceWeighting = true;

        /// <summary>
        /// Whether to weight an anchor's score by its distance from the user.
        /// </summary>
        public bool enableDistanceWeighting
        {
            get => m_EnableDistanceWeighting;
            set => m_EnableDistanceWeighting = value;
        }

        [SerializeField]
        [Tooltip("The curve used to weight an anchor's score by its distance from the user. The X axis is the normalized " +
            "distance, where 0 is the closest anchor and 1 is the furthest anchor. The Y axis is the weight.")]
        AnimationCurve m_DistanceWeightCurve;

        /// <summary>
        /// The curve used to weight an anchor's score by its distance from the user. The X axis is the normalized
        /// distance, where 0 is the distance of the closest anchor and 1 is the distance of the furthest anchor.
        /// The Y axis is the weight.
        /// </summary>
        public AnimationCurve distanceWeightCurve
        {
            get => m_DistanceWeightCurve;
            set => m_DistanceWeightCurve = value;
        }

        float[] m_AnchorWeights;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Reset()
        {
            m_GazeAngleScoreCurve = new AnimationCurve(
                new Keyframe(0, 1, 0, 0),
                new Keyframe(1, 0, -2, -2));

            m_DistanceWeightCurve = new AnimationCurve(
                new Keyframe(0, 0.1f, 0, 0),
                new Keyframe(0.01f, 0.1f, 0, 0),
                new Keyframe(0.05f, 1, 0, 0),
                new Keyframe(1, 1, 0, 0));
        }

        /// <inheritdoc/>
        public int GetDestinationAnchorIndex(TeleportationMultiAnchorVolume teleportationVolume)
        {
            var anchors = teleportationVolume.anchorTransforms;
            if (m_AnchorWeights == null || m_AnchorWeights.Length != anchors.Count)
                m_AnchorWeights = new float[anchors.Count];

            var xrOrigin = teleportationVolume.teleportationProvider.system.xrOrigin;

            if (m_EnableDistanceWeighting)
            {
                var userPosition = xrOrigin.GetCameraFloorWorldPosition();
                var maxSqDistance = -1f;
                var minSqDistance = float.MaxValue;
                for (var i = 0; i < anchors.Count; ++i)
                {
                    var sqDistance = (anchors[i].position - userPosition).sqrMagnitude;
                    m_AnchorWeights[i] = sqDistance;
                    if (sqDistance > maxSqDistance)
                        maxSqDistance = sqDistance;

                    if (sqDistance < minSqDistance)
                        minSqDistance = sqDistance;
                }

                for (var i = 0; i < anchors.Count; ++i)
                {
                    m_AnchorWeights[i] = m_DistanceWeightCurve.Evaluate(
                        (m_AnchorWeights[i] - minSqDistance) / (maxSqDistance - minSqDistance));
                }
            }
            else
            {
                for (var i = 0; i < anchors.Count; ++i)
                    m_AnchorWeights[i] = 1f;
            }

            // Score anchors based on angle between camera forward and anchor direction, weighted by distance.
            var destinationAnchorIndex = -1;
            var cameraTrans = xrOrigin.Camera.transform;
            var cameraPosition = cameraTrans.position;
            var cameraForward = cameraTrans.forward;
            var maxScore = 0f; // Anything scored 0 or below is not a valid destination.
            for (var i = 0; i < anchors.Count; ++i)
            {
                var anchorPosition = anchors[i].position;
                var anchorDirection = Vector3.Normalize(anchorPosition - cameraPosition);
                BurstMathUtility.Angle(cameraForward, anchorDirection, out var angle);
                if (angle > m_MaxGazeAngle)
                    continue;

                var angleT = angle / m_MaxGazeAngle;
                var score = m_GazeAngleScoreCurve.Evaluate(angleT) * m_AnchorWeights[i];
                if (score > maxScore)
                {
                    destinationAnchorIndex = i;
                    maxScore = score;
                }
            }

            return destinationAnchorIndex;
        }
    }
}