using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles the lifecycle of simulated HMDs, controllers, and hands. It deals with the adding and removing of devices,
    /// keeping track of the current device mode, and initializing subsystems for simulation.
    /// </summary>
    /// <seealso cref="XRDeviceSimulator"/>
    /// <seealso cref="XRSimulatedController"/>
    /// <seealso cref="XRSimulatedHMD"/>
    [AddComponentMenu("XR/Debug/Simulated Device Lifecycle Manager", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_SimulatedDeviceLifecycleManager)]
    [HelpURL(XRHelpURLConstants.k_SimulatedDeviceLifecycleManager)]
    public class SimulatedDeviceLifecycleManager : MonoBehaviour
    {
        [SerializeField]
        bool m_RemoveOtherHMDDevices = true;
        /// <summary>
        /// This boolean value indicates whether we remove other <see cref="XRHMD"/> devices in this session so that they don't conflict with the simulated devices.
        /// A <see langword="true"/> value (default) means we remove all other <see cref="XRHMD"/> devices except the <see cref="XRSimulatedHMD"/> generated by the simulator.
        /// A <see langword="false"/> value means we do not remove any other <see cref="XRHMD"/> devices.
        /// </summary>
        public bool removeOtherHMDDevices
        {
            get => m_RemoveOtherHMDDevices;
            set => m_RemoveOtherHMDDevices = value;
        }

        [SerializeField]
        bool m_HandTrackingCapability = true;
        /// <summary>
        /// Whether to create a simulated Hand Tracking Subsystem and provider on startup. Requires the XR Hands package.
        /// </summary>
        public bool handTrackingCapability
        {
            get => m_HandTrackingCapability;
            set => m_HandTrackingCapability = value;
        }

        /// <summary>
        /// The device mode of the left and right device.
        /// </summary>
        /// <seealso cref="deviceMode"/>
        public enum DeviceMode
        {
            /// <summary>
            /// Motion controller mode.
            /// </summary>
            Controller,

            /// <summary>
            /// Tracked hand mode.
            /// </summary>
            Hand,

            /// <summary>
            /// No device mode.
            /// </summary>
            None,
        }

        DeviceMode m_DeviceMode = DeviceMode.Controller;

        /// <summary>
        /// Whether the simulator is in controller mode or tracked hand mode.
        /// </summary>
        /// <seealso cref="DeviceMode"/>
        public DeviceMode deviceMode => m_DeviceMode;

        /// <summary>
        /// The runtime instance of the Simulated Device Lifecycle Manager.
        /// </summary>
        public static SimulatedDeviceLifecycleManager instance { get; private set; }

#if ENABLE_VR || UNITY_GAMECORE
        XRSimulatedHMD m_HMDDevice;
        XRSimulatedController m_LeftControllerDevice;
        XRSimulatedController m_RightControllerDevice;

        bool m_OnInputDeviceChangeSubscribed;
#endif

#if XR_HANDS_1_1_OR_NEWER
        XRDeviceSimulatorHandsSubsystem m_SimHandSubsystem;

        /// <summary>
        /// The instance of the simulated hand subsystem.
        /// </summary>
        internal XRDeviceSimulatorHandsSubsystem simHandSubsystem => m_SimHandSubsystem;

        XRHandProviderUtility.SubsystemUpdater m_SubsystemUpdater;
        XRInputModalityManager m_InputModalityManager;
#endif

        bool m_DeviceModeDirty;
        bool m_StartedDeviceModeChange;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning($"Another instance of Simulated Device Lifecycle Manager already exists ({instance}), destroying {gameObject}.", this);
                Destroy(gameObject);
                return;
            }

            InitializeHandSubsystem();

#if XR_HANDS_1_1_OR_NEWER
            ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out m_InputModalityManager);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
#if ENABLE_VR || UNITY_GAMECORE

#if XR_HANDS_1_1_OR_NEWER
            m_SimHandSubsystem?.Start();
            m_SubsystemUpdater?.Start();
#endif

            if (m_RemoveOtherHMDDevices)
            {
                // Operate on a copy of the devices array since we are removing from it
                foreach (var device in InputSystem.InputSystem.devices.ToArray())
                {
                    if (device is XRHMD && !(device is XRSimulatedHMD))
                    {
                        InputSystem.InputSystem.RemoveDevice(device);
                    }
                }

                InputSystem.InputSystem.onDeviceChange += OnInputDeviceChange;
                m_OnInputDeviceChangeSubscribed = true;
            }
#endif
            AddDevices();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_OnInputDeviceChangeSubscribed)
            {
                InputSystem.InputSystem.onDeviceChange -= OnInputDeviceChange;
                m_OnInputDeviceChangeSubscribed = false;
            }
#endif
            RemoveDevices();

#if XR_HANDS_1_1_OR_NEWER
            m_SubsystemUpdater?.Stop();
            m_SimHandSubsystem?.Stop();
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
#if XR_HANDS_1_1_OR_NEWER
            m_SimHandSubsystem?.Destroy();
            m_SubsystemUpdater?.Destroy();
            m_SimHandSubsystem = null;
            m_SubsystemUpdater = null;
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceModeDirty)
            {
                switch (m_DeviceMode)
                {
                    // Changing from hands to controllers over multiple frames.
                    // Step 1: Simulate hand tracking lost
                    // Step 2: Add controller input devices.
                    case DeviceMode.Controller when !m_StartedDeviceModeChange:
                        // Step 1
                        m_SimHandSubsystem?.SetUpdateHandsAllowed(false);
                        m_StartedDeviceModeChange = true;
                        break;
                    case DeviceMode.Controller:
                        // Step 2
                        AddControllerDevices();
                        m_DeviceModeDirty = false;
                        m_StartedDeviceModeChange = false;
                        break;
                    // Changing from controllers to hands over multiple frames.
                    // Step 1: Remove controller devices.
                    // Step 2: Simulate hand tracking reacquired.
                    case DeviceMode.Hand when !m_StartedDeviceModeChange:
                        // Step 1
                        RemoveControllerDevices();
                        m_StartedDeviceModeChange = true;
                        break;
                    case DeviceMode.Hand:
                        // Step 2
                        m_SimHandSubsystem?.SetUpdateHandsAllowed(true);
                        m_DeviceModeDirty = false;
                        m_StartedDeviceModeChange = false;
                        break;
                }
            }
#endif
        }

#if ENABLE_VR || UNITY_GAMECORE
        internal void ApplyHMDState(XRSimulatedHMDState state)
        {
            if (m_HMDDevice != null && m_HMDDevice.added)
            {
                InputState.Change(m_HMDDevice, state);
            }
        }

        internal void ApplyControllerState(XRSimulatedControllerState leftControllerState, XRSimulatedControllerState rightControllerState)
        {
            if (m_LeftControllerDevice != null && m_LeftControllerDevice.added)
            {
                InputState.Change(m_LeftControllerDevice, leftControllerState);
            }

            if (m_RightControllerDevice != null && m_RightControllerDevice.added)
            {
                InputState.Change(m_RightControllerDevice, rightControllerState);
            }
        }
#endif

        internal void ApplyHandState(XRSimulatedHandState leftHandState, XRSimulatedHandState rightHandState)
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceMode != DeviceMode.Hand)
                return;

            if (m_SimHandSubsystem == null)
                return;

            m_SimHandSubsystem.SetIsTracked(Handedness.Left, leftHandState.isTracked);
            m_SimHandSubsystem.SetIsTracked(Handedness.Right, rightHandState.isTracked);

            m_SimHandSubsystem.SetHandExpression(Handedness.Left, leftHandState.expressionName);
            m_SimHandSubsystem.SetRootHandPose(Handedness.Left, new Pose(leftHandState.position, leftHandState.rotation));

            m_SimHandSubsystem.SetHandExpression(Handedness.Right, rightHandState.expressionName);
            m_SimHandSubsystem.SetRootHandPose(Handedness.Right, new Pose(rightHandState.position, rightHandState.rotation));
#endif
        }

        internal void SwitchDeviceMode()
        {
#if XR_HANDS_1_1_OR_NEWER
            // Fully changing between controller and hand mode takes multiple frames.
            // Don't allow changing the mode again before it has finished.
            if (m_DeviceModeDirty)
                return;

            // Disallow switching device mode if the modality manager is being used
            // and the opposite set of GameObjects are not assigned.
            if (m_InputModalityManager != null)
            {
                if (m_DeviceMode == DeviceMode.Controller && m_InputModalityManager.leftHand == null && m_InputModalityManager.rightHand == null)
                    return;

                if (m_DeviceMode == DeviceMode.Hand && m_InputModalityManager.leftController == null && m_InputModalityManager.rightController == null)
                    return;
            }

            m_DeviceMode = Negate(m_DeviceMode);
            m_DeviceModeDirty = true;
#endif
        }

        /// <summary>
        /// Add simulated XR devices to the Input System.
        /// </summary>
        internal virtual void AddDevices()
        {
#if ENABLE_VR || UNITY_GAMECORE
            // Simulated HMD
            if (m_HMDDevice == null)
            {
                var descHMD = new InputDeviceDescription
                {
                    product = nameof(XRSimulatedHMD),
                    capabilities = new XRDeviceDescriptor
                    {
                        characteristics = XRInputTrackingAggregator.Characteristics.hmd,
                    }.ToJson(),
                };

                m_HMDDevice = InputSystem.InputSystem.AddDevice(descHMD) as XRSimulatedHMD;
                if (m_HMDDevice == null)
                    Debug.LogError($"Failed to create {nameof(XRSimulatedHMD)}.", this);
            }
            else
            {
                InputSystem.InputSystem.AddDevice(m_HMDDevice);
            }

            if (m_DeviceMode == DeviceMode.Controller)
                AddControllerDevices();
#endif
        }

        /// <summary>
        /// Remove simulated XR devices from the Input System.
        /// </summary>
        internal virtual void RemoveDevices()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_HMDDevice != null && m_HMDDevice.added)
                InputSystem.InputSystem.RemoveDevice(m_HMDDevice);

            RemoveControllerDevices();
#endif
        }

        void AddControllerDevices()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_LeftControllerDevice == null)
            {
                var descLeftHand = new InputDeviceDescription
                {
                    product = nameof(XRSimulatedController),
                    capabilities = new XRDeviceDescriptor
                    {
                        deviceName = $"{nameof(XRSimulatedController)} - {InputSystem.CommonUsages.LeftHand}",
                        characteristics = XRInputTrackingAggregator.Characteristics.leftController,
                    }.ToJson(),
                };

                m_LeftControllerDevice = InputSystem.InputSystem.AddDevice(descLeftHand) as XRSimulatedController;
                if (m_LeftControllerDevice != null)
                    InputSystem.InputSystem.SetDeviceUsage(m_LeftControllerDevice, InputSystem.CommonUsages.LeftHand);
                else
                    Debug.LogError($"Failed to create {nameof(XRSimulatedController)} for {InputSystem.CommonUsages.LeftHand}.", this);
            }
            else
            {
                InputSystem.InputSystem.AddDevice(m_LeftControllerDevice);
            }

            if (m_RightControllerDevice == null)
            {
                var descRightHand = new InputDeviceDescription
                {
                    product = nameof(XRSimulatedController),
                    capabilities = new XRDeviceDescriptor
                    {
                        deviceName = $"{nameof(XRSimulatedController)} - {InputSystem.CommonUsages.RightHand}",
                        characteristics = XRInputTrackingAggregator.Characteristics.rightController,
                    }.ToJson(),
                };

                m_RightControllerDevice = InputSystem.InputSystem.AddDevice(descRightHand) as XRSimulatedController;
                if (m_RightControllerDevice != null)
                    InputSystem.InputSystem.SetDeviceUsage(m_RightControllerDevice, InputSystem.CommonUsages.RightHand);
                else
                    Debug.LogError($"Failed to create {nameof(XRSimulatedController)} for {InputSystem.CommonUsages.RightHand}.", this);
            }
            else
            {
                InputSystem.InputSystem.AddDevice(m_RightControllerDevice);
            }
#endif
        }

        void RemoveControllerDevices()
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (m_LeftControllerDevice != null && m_LeftControllerDevice.added)
            {
                InputSystem.InputSystem.RemoveDevice(m_LeftControllerDevice);
            }

            if (m_RightControllerDevice != null && m_RightControllerDevice.added)
            {
                InputSystem.InputSystem.RemoveDevice(m_RightControllerDevice);
            }
#endif
        }

        void OnInputDeviceChange(InputSystem.InputDevice device, InputDeviceChange change)
        {
#if ENABLE_VR || UNITY_GAMECORE
            if (!m_RemoveOtherHMDDevices)
                return;

            switch (change)
            {
                case InputDeviceChange.Added:
                    if (device is XRHMD && !(device is XRSimulatedHMD))
                        InputSystem.InputSystem.RemoveDevice(device);
                    break;
            }
#endif
        }

        void InitializeHandSubsystem()
        {
#if XR_HANDS_1_1_OR_NEWER
            if (!m_HandTrackingCapability)
                return;

            if (m_RemoveOtherHMDDevices)
            {
                var currentHandSubsystems = new List<XRHandSubsystem>();
                SubsystemManager.GetSubsystems(currentHandSubsystems);
                foreach (var handSubsystem in currentHandSubsystems)
                {
                    if (handSubsystem.running)
                        handSubsystem.Stop();
                }
            }

            var descriptors = new List<XRHandSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(descriptors);
            for (var i = 0; i < descriptors.Count; ++i)
            {
                var descriptor = descriptors[i];
                if (descriptor.id == XRDeviceSimulatorHandsProvider.id)
                {
                    m_SimHandSubsystem = descriptor.Create() as XRDeviceSimulatorHandsSubsystem;
                    break;
                }
            }

            if (m_SimHandSubsystem == null)
            {
                Debug.LogError("Couldn't find simulated hands subsystem.", this);
                return;
            }

            m_SimHandSubsystem.SetUpdateHandsAllowed(false);

            m_SubsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(m_SimHandSubsystem);
#endif
        }

        static DeviceMode Negate(DeviceMode mode)
        {
            switch (mode)
            {
                case DeviceMode.Controller:
                    return DeviceMode.Hand;
                case DeviceMode.Hand:
                    return DeviceMode.Controller;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(mode)}={mode}.");
                    return DeviceMode.Controller;
            }
        }
    }
}
