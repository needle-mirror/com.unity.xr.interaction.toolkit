namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Translate transform to face the main camera position
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// Process billboard logic according to Y-axis-up or Y-axis-down
        /// </summary>
        [SerializeField] bool m_WorldUp;

        /// <summary>
        /// Reverse and flip the forward facing billboard direction
        /// </summary>
        [SerializeField] bool m_FlipForward;

        Camera m_Camera;
        Transform m_Transform;

        void Awake()
        {
            m_Transform = transform;
            m_Camera = Camera.main;
        }

        void Update()
        {
            Quaternion lookRotation = Quaternion.LookRotation(m_Camera.transform.position - m_Transform.position);

            if (m_WorldUp)
            {
                Vector3 offset = lookRotation.eulerAngles;
                offset.x = 0;
                offset.z = 0;

                if (m_FlipForward)
                    offset.y += 180;

                lookRotation = Quaternion.Euler(offset);
            }

            m_Transform.rotation = lookRotation;
        }
    }
}
