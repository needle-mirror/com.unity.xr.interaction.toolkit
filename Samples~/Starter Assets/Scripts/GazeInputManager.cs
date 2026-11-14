using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Manages input fallback for <see cref="XRGazeInteractor"/> when eye tracking is not available.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    public class GazeInputManager : MonoBehaviour
    {
        // This is the name of the layout that is registered by EyeGazeInteraction in the OpenXR Plugin package
        const string k_EyeGazeLayoutName = "EyeGaze";

        static readonly List<InputDevice> s_InputDevicesReuse = new List<InputDevice>();

        [SerializeField]
        [Tooltip("Enable fallback to head tracking if eye tracking is unavailable.")]
        bool m_FallbackIfEyeTrackingUnavailable = true;

        /// <summary>
        /// Enable fallback to head tracking if eye tracking is unavailable.
        /// </summary>
        public bool fallbackIfEyeTrackingUnavailable
        {
            get => m_FallbackIfEyeTrackingUnavailable;
            set => m_FallbackIfEyeTrackingUnavailable = value;
        }

        bool m_EyeTrackingDeviceFound;

        /// <summary>
        /// Whether a device that supports eye tracking has been found.
        /// </summary>
        /// <seealso cref="eyeTrackingDeviceFoundChanged"/>
        public bool eyeTrackingDeviceFound => m_EyeTrackingDeviceFound;

        /// <summary>
        /// Checks whether the gaze is tracked this frame. If an eye tracking device has been found, the gaze comes from the eyes.
        /// Otherwise, if <see cref="fallbackIfEyeTrackingUnavailable"/> is on, the gaze follows the head.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the eyes are tracked, or if the gaze falls back to the head and the head is tracked.
        /// Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="eyeTrackingDeviceFound"/>
        public bool IsGazeTracked()
        {
            if (m_EyeTrackingDeviceFound)
                return XRInputTrackingAggregator.GetEyeGazeStatus().isTracked;

            return m_FallbackIfEyeTrackingUnavailable && XRInputTrackingAggregator.GetHMDStatus().isTracked;
        }

        /// <summary>
        /// Invoked when <see cref="eyeTrackingDeviceFound"/> changes, when an eye tracking device connects or disconnects after <c>Awake</c>.
        /// </summary>
        public event Action<GazeInputManager> eyeTrackingDeviceFoundChanged;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
            InputSystem.InputSystem.onDeviceChange += OnDeviceChange;

            m_EyeTrackingDeviceFound = HasEyeTrackingDevice();
            if (m_EyeTrackingDeviceFound)
                Debug.Log("Eye tracking device found!", this);
            else
                Debug.LogWarning($"Could not find a device that supports eye tracking on Awake. {this} has subscribed to device connected events and will activate the GameObject when an eye tracking device is connected.", this);

            UpdateActive();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
            InputSystem.InputSystem.onDeviceChange -= OnDeviceChange;
        }

        void OnDeviceConnected(InputDevice inputDevice)
        {
            if ((inputDevice.characteristics & InputDeviceCharacteristics.EyeTracking) == InputDeviceCharacteristics.EyeTracking)
                UpdateEyeTrackingDeviceFound();
        }

        void OnDeviceDisconnected(InputDevice inputDevice)
        {
            if ((inputDevice.characteristics & InputDeviceCharacteristics.EyeTracking) == InputDeviceCharacteristics.EyeTracking)
                UpdateEyeTrackingDeviceFound();
        }

        void OnDeviceChange(InputSystem.InputDevice device, InputDeviceChange change)
        {
            if (device.layout != k_EyeGazeLayoutName)
                return;

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected ||
                change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
            {
                UpdateEyeTrackingDeviceFound();
            }
        }

        // An eye tracker can be reported by both the XR input devices and the Input System,
        // so check both rather than trusting the one device that changed.
        static bool HasEyeTrackingDevice()
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, s_InputDevicesReuse);
            foreach (var inputDevice in s_InputDevicesReuse)
            {
                if (inputDevice.isValid)
                    return true;
            }

            foreach (var device in InputSystem.InputSystem.devices)
            {
                if (device.layout == k_EyeGazeLayoutName)
                    return true;
            }

            return false;
        }

        void UpdateEyeTrackingDeviceFound()
        {
            var found = HasEyeTrackingDevice();
            if (found == m_EyeTrackingDeviceFound)
                return;

            Debug.Log(found ? "Eye tracking device found!" : "Eye tracking device lost!", this);
            m_EyeTrackingDeviceFound = found;
            UpdateActive();
            eyeTrackingDeviceFoundChanged?.Invoke(this);
        }

        void UpdateActive()
        {
            gameObject.SetActive(m_EyeTrackingDeviceFound || m_FallbackIfEyeTrackingUnavailable);
        }
    }
}
