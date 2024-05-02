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
