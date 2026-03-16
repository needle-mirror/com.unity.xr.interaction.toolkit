namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Perform smoothed periodic rotation oscillation of GameObject transform
    /// </summary>
    public class RotationOscillator : MonoBehaviour
    {
        /// <summary>
        /// Speed of oscillation
        /// </summary>
        [SerializeField]
        float m_Speed = 1f;

        /// <summary>
        /// Rotation angle (positive = clockwise, negative = counterclockwise)
        /// </summary>
        [SerializeField]
        float m_Angle = 45f;

        float m_StartRotation;
        float m_TimeOffset;
        Transform m_Transform;

        void Start()
        {
            m_Transform = transform;
            m_StartRotation = m_Transform.eulerAngles.y;
            m_TimeOffset = Time.time;
        }

        void Update()
        {
            // Calculate center point of rotation
            float centerRotation = m_StartRotation + (m_Angle);

            // Calculate oscillation around the center
            float oscillation = Mathf.Sin((Time.time - m_TimeOffset) * m_Speed) * Mathf.Abs(m_Angle);

            // Apply rotation only to Y axis
            m_Transform.eulerAngles = new Vector3(m_Transform.eulerAngles.x, centerRotation + oscillation, m_Transform.eulerAngles.z);
        }
    }
}
