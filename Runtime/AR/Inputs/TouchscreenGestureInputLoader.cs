// These are the guards in TouchscreenGestureInputController.cs
#if ((ENABLE_VR || UNITY_GAMECORE) && AR_FOUNDATION_PRESENT) || PACKAGE_DOCS_GENERATION
#define TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
#endif

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
    /// <summary>
    /// Component that manages the <see cref="TouchscreenGestureInputController"/> device in the Unity input system.
    /// Automatically adds and removes the device with this behavior's enabled state.
    /// </summary>
    /// <remarks>
    /// Requires AR Foundation (com.unity.xr.arfoundation) package to be installed in this project.
    /// </remarks>
    [AddComponentMenu("XR/Input/Touchscreen Gesture Input Loader", 11)]
    [HelpURL(XRHelpURLConstants.k_TouchscreenGestureInputLoader)]
    public class TouchscreenGestureInputLoader : MonoBehaviour
    {
        [Header("Gesture Configuration")]
        [SerializeField]
        [Tooltip("Time (in seconds) within (≤) which a touch and release has to occur for it to be registered as a tap.")]
        float m_TapDuration = 0.5f;

        /// <summary>
        /// Time (in seconds) within (≤) which a touch and release has to occur for it
        /// to be registered as a tap.
        /// </summary>
        /// <remarks>
        /// A touch and release that takes > this value causes the tap gesture to be canceled.
        /// </remarks>
        /// <seealso cref="TapGestureRecognizer.durationSeconds"/>
        /// <seealso cref="RefreshGestureRecognizersConfiguration"/>
        internal float tapDuration
        {
            get => m_TapDuration;
            set => m_TapDuration = value;
        }

#if TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
        TouchscreenGestureInputController m_GestureInputController;
#endif

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
#if !AR_FOUNDATION_PRESENT
            Debug.LogWarning("Script requires AR Foundation (com.unity.xr.arfoundation) package to add the TouchscreenGestureInputController device. Install using Window > Package Manager or click Fix on the related issue in Edit > Project Settings > XR Plug-in Management > Project Validation.", this);
            enabled = false;
#elif !TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
            Debug.LogWarning("The TouchscreenGestureInputController device cannot be added because it is not available on this platform.", this);
            enabled = false;
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            InitializeTouchscreenGestureController();
            RefreshGestureRecognizersConfiguration();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            RemoveTouchscreenGestureController();
        }

        /// <summary>
        /// Refresh the properties on the gesture recognizers, such as the tap duration threshold,
        /// based on the serialized properties of this component. This is done automatically when this
        /// component is enabled, however this method can be called to manually refresh the configuration
        /// after the property values are changed.
        /// </summary>
        internal void RefreshGestureRecognizersConfiguration()
        {
#if TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
            if (m_GestureInputController != null)
            {
                m_GestureInputController.tapGestureRecognizer.durationSeconds = m_TapDuration;
            }
#endif
        }

        void InitializeTouchscreenGestureController()
        {
#if TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
            if (m_GestureInputController == null)
            {
                m_GestureInputController = InputSystem.InputSystem.AddDevice<TouchscreenGestureInputController>();
                if (m_GestureInputController == null)
                {
                    Debug.LogError($"Failed to create {nameof(TouchscreenGestureInputController)}.", this);
                }
            }
            else
            {
                InputSystem.InputSystem.AddDevice(m_GestureInputController);
            }
#endif
        }

        void RemoveTouchscreenGestureController()
        {
#if TOUCHSCREEN_GESTURE_INPUT_CONTROLLER_AVAILABLE
            if (m_GestureInputController != null && m_GestureInputController.added)
            {
                InputSystem.InputSystem.RemoveDevice(m_GestureInputController);
            }
#endif
        }
    }
}
