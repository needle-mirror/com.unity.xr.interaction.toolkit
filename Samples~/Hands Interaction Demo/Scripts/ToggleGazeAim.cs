using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Toggles whether gaze is used to target objects. While <see cref="gazeAimEnabled"/> is <see langword="true"/>,
    /// users target objects and UI by looking at them, for example looking at an object and pinching to select it.
    /// Otherwise, users target objects using their hands or controllers.
    /// </summary>
    public class ToggleGazeAim : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Whether users target objects by looking at them, instead of with their hands or controllers.")]
        bool m_GazeAimEnabled;

        /// <summary>
        /// Whether users target objects by looking at them, instead of with their hands or controllers.
        /// </summary>
        public bool gazeAimEnabled
        {
            get => m_GazeAimEnabled;
            set
            {
                m_GazeAimEnabled = value;
                if (isActiveAndEnabled)
                    UpdateGazeAim();
            }
        }

        [SerializeField]
        [Tooltip("The drivers on the Near-Far Interactors that aim their rays with gaze. They are enabled while gaze is used to target objects.")]
        List<GazePinchRayDriver> m_Drivers = new List<GazePinchRayDriver>();

        /// <summary>
        /// The drivers on the Near-Far Interactors that aim their rays with gaze. They are enabled while gaze is used to target objects.
        /// </summary>
        public List<GazePinchRayDriver> drivers
        {
            get => m_Drivers;
            set => m_Drivers = value;
        }

        [SerializeField]
        [Tooltip("The GameObjects to activate while gaze is used to target objects, such as the Gaze Interactor and Gaze Stabilized objects of the XR Origin.")]
        List<GameObject> m_GazeObjectsToToggle = new List<GameObject>();

        /// <summary>
        /// The GameObjects to activate while gaze is used to target objects, such as the Gaze Interactor and Gaze Stabilized objects of the XR Origin.
        /// </summary>
        public List<GameObject> gazeObjectsToToggle
        {
            get => m_GazeObjectsToToggle;
            set => m_GazeObjectsToToggle = value;
        }

        void OnEnable()
        {
            UpdateGazeAim();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            // Activating the gaze objects can add components, which logs a warning during OnValidate, so update on the next Editor frame.
            UnityEditor.EditorApplication.delayCall -= UpdateGazeAimIfActive;
            UnityEditor.EditorApplication.delayCall += UpdateGazeAimIfActive;
        }

        void UpdateGazeAimIfActive()
        {
            if (this != null && isActiveAndEnabled)
                UpdateGazeAim();
        }
#endif

        void UpdateGazeAim()
        {
            // Activate the gaze objects before enabling the drivers, and disable the drivers before deactivating them,
            // so a driver never runs without its gaze pose.
            if (m_GazeAimEnabled)
            {
                SetGazeObjectsActive(true);
                SetDriversEnabled(true);
            }
            else
            {
                SetDriversEnabled(false);
                SetGazeObjectsActive(false);
            }
        }

        void SetDriversEnabled(bool enable)
        {
            foreach (var driver in m_Drivers)
            {
                if (driver != null)
                    driver.enabled = enable;
            }
        }

        void SetGazeObjectsActive(bool active)
        {
            foreach (var gazeObject in m_GazeObjectsToToggle)
            {
                if (gazeObject != null)
                    gazeObject.SetActive(active);
            }
        }
    }
}
