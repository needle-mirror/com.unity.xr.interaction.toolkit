namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Perform smoothed periodic position oscillation of GameObject transform
    /// </summary>
    public class PositionOscillator : MonoBehaviour
    {
        /// <summary>
        /// Speed of oscillation
        /// </summary>
        [SerializeField]
        float m_Speed = 1f;

        /// <summary>
        /// Distance and direction to move (positive = up, negative = down)
        /// </summary>
        [SerializeField]
        float m_Distance = 1f;

        Vector3 m_StartPosition;
        float m_TimeOffset;
        Transform m_Transform;

        void Start()
        {
            m_Transform = transform;
            m_StartPosition = m_Transform.position;
            m_TimeOffset = Time.time;
        }

        void Update()
        {
            // Oscillate around startPosition + offsetDirection
            var offsetDirection = Vector3.up * (m_Distance);
            var oscillation = Mathf.Sin((Time.time - m_TimeOffset) * m_Speed) * Mathf.Abs(m_Distance);
            m_Transform.position = m_StartPosition + offsetDirection + (Vector3.up * oscillation);
        }
    }
}
