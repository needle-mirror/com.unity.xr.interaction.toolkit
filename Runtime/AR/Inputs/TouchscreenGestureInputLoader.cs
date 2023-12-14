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
#if AR_FOUNDATION_PRESENT
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
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            InitializeTouchscreenGestureController();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            RemoveTouchscreenGestureController();
        }

        void InitializeTouchscreenGestureController()
        {
#if AR_FOUNDATION_PRESENT
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
#if AR_FOUNDATION_PRESENT
            if (m_GestureInputController != null && m_GestureInputController.added)
            {
                InputSystem.InputSystem.RemoveDevice(m_GestureInputController);
            }
#endif
        }
    }
}
