using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Performs events when this object is within the field of view of the gaze transform. This is currently used to
    /// hide and show UI within the field of view.
    /// </summary>
    public class CalloutGazeController : MonoBehaviour
    {
        /// <summary>
        /// The transform which the forward direction will be used to evaluate as the gaze direction.
        /// </summary>
        [SerializeField]
        [Tooltip("The transform which the forward direction will be used to evaluate as the gaze direction.")]
        protected Transform m_GazeTransform;

        /// <summary>
        /// Threshold for the dot product when determining if the Gaze Transform is facing this object. The lower the threshold, the wider the field of view.
        /// </summary>
        [SerializeField]
        [Tooltip("Threshold for the dot product when determining if the Gaze Transform is facing this object. The lower the threshold, the wider the field of view.")]
        [Range(0f, 1f)]
        protected float m_FacingThreshold = 0.85f;

        /// <summary>
        /// Events fired when the Gaze Transform begins facing this game object.
        /// </summary>
        [SerializeField]
        [Tooltip("Events fired when the Gaze Transform begins facing this game object.")]
        protected UnityEvent m_FacingEntered;

        /// <summary>
        /// Events fired when the Gaze Transform stops facing this game object.
        /// </summary>
        [SerializeField]
        [Tooltip("Events fired when the Gaze Transform stops facing this game object.")]
        protected UnityEvent m_FacingExited;

        /// <summary>
        /// Distance threshold for movement in a single frame that determines a large movement that will trigger Facing Exited events.
        /// </summary>
        [SerializeField]
        [Tooltip("Distance threshold for movement in a single frame that determines a large movement that will trigger Facing Exited events.")]
        float m_LargeMovementDistanceThreshold = 0.05f;

        /// <summary>
        /// Cool down time after a large movement for Facing Entered events to fire again.
        /// </summary>
        [SerializeField]
        [Tooltip("Cool down time after a large movement for Facing Entered events to fire again.")]
        float m_LargeMovementCoolDownTime = 0.25f;

        /// <summary>
        /// Use Distance Threshold.
        /// </summary>
        [SerializeField]
        [Tooltip("Use Distance Threshold.")]
        bool m_UseDistanceThreshold = false;

        /// <summary>
        /// Distance threshold to stop applying Facing Events.
        /// </summary>
        [SerializeField]
        [Tooltip("Distance threshold to stop applying Facing Events.")]
        float m_MaxDistanceThreshold = 10f;

        bool m_IsFacing;
        float m_LargeMovementCoolDown;
        Vector3 m_LastPosition;

        void Start()
        {
            if (!m_GazeTransform)
                m_GazeTransform = Camera.main.transform;
        }

        protected virtual void Update()
        {
            CheckLargeMovement();

            if (m_LargeMovementCoolDown < m_LargeMovementCoolDownTime)
                return;

            CheckFacing();
        }

        void CheckFacing()
        {
            if (!m_GazeTransform)
                return;

            if (m_UseDistanceThreshold)
            {
                float currentDistance = Vector3.Distance(m_GazeTransform.position, transform.position);
                if (currentDistance > m_MaxDistanceThreshold)
                {
                    if (m_IsFacing)
                        FacingExited();

                    return;
                }
            }

            var dotProduct = Vector3.Dot(m_GazeTransform.forward, (transform.position - m_GazeTransform.position).normalized);
            if (dotProduct > m_FacingThreshold && !m_IsFacing)
                FacingEntered();
            else if (dotProduct < m_FacingThreshold && m_IsFacing)
                FacingExited();
        }

        void CheckLargeMovement()
        {
            // Check if there is large movement
            var currentPosition = transform.position;
            var positionDelta = Mathf.Abs(Vector3.Distance(m_LastPosition, currentPosition));
            if (positionDelta > m_LargeMovementDistanceThreshold)
            {
                m_LargeMovementCoolDown = 0f;
                FacingExited();
            }

            m_LargeMovementCoolDown += Time.deltaTime;
            m_LastPosition = currentPosition;
        }

        void FacingEntered()
        {
            m_IsFacing = true;
            m_FacingEntered.Invoke();
        }

        void FacingExited()
        {
            m_IsFacing = false;
            m_FacingExited.Invoke();
        }

        /// <summary>
        /// Verify if the gaze vector has diverged beyond the allowed range
        /// </summary>
        public void CheckPointerExit()
        {
            var dotProduct = Vector3.Dot(m_GazeTransform.forward, (transform.position - m_GazeTransform.position).normalized);
            float currentDistance = Vector3.Distance(m_GazeTransform.position, transform.position);
            if (dotProduct < m_FacingThreshold || (m_UseDistanceThreshold && currentDistance > m_MaxDistanceThreshold))
                FacingExited();
        }
    }
}
