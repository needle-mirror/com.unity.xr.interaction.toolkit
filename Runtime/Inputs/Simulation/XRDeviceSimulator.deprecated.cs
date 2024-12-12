using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    public partial class XRDeviceSimulator
    {
        /// <summary>
        /// A hand expression that can be simulated by performing an input action.
        /// </summary>
        [Serializable]
        [Obsolete("XRDeviceSimulator.SimulatedHandExpression has been deprecated in XRI 3.1.0. Update the XR Device Simulator sample in Package Manager or use the unnested version of SimulatedHandExpression instead.")]
        public class SimulatedHandExpression : ISerializationCallbackReceiver
        {
            [SerializeField]
            [Tooltip("The unique name for the hand expression.")]
            [Delayed]
            string m_Name;

            /// <summary>
            /// The name of the hand expression to simulate when the input action is performed.
            /// </summary>
            public string name => m_ExpressionName.ToString();

            [SerializeField]
            [Tooltip("The input action to trigger the hand expression.")]
            InputActionReference m_ToggleAction;

            /// <summary>
            /// The input action reference to trigger this simulated hand expression.
            /// </summary>
            public InputActionReference toggleAction => m_ToggleAction;

            [SerializeField]
            [Tooltip("The captured hand expression to simulate when the input action is performed.")]
            HandExpressionCapture m_Capture;

            /// <summary>
            /// The captured expression to simulate when the input action is performed.
            /// </summary>
            internal HandExpressionCapture capture
            {
                get => m_Capture;
                set => m_Capture = value;
            }

            HandExpressionName m_ExpressionName;

            /// <summary>
            /// The name of the hand expression to simulate when the input action is performed.
            /// Use this for a faster name identifier than comparing by <see cref="string"/> name.
            /// </summary>
            internal HandExpressionName expressionName
            {
                get => m_ExpressionName;
                set => m_ExpressionName = value;
            }

            /// <summary>
            /// Sprite icon for the simulated hand expression.
            /// </summary>
            public Sprite icon => m_Capture.icon;

            Action<SimulatedHandExpression, InputAction.CallbackContext> m_Performed;

            /// <summary>
            /// Event that is called when the input action for the simulated hand expression is performed.
            /// </summary>
            /// <remarks>
            /// Wraps the performed action of the <see cref="toggleAction"/> in order to add a reference
            /// to this class in the callback method signature.
            /// </remarks>
            public event Action<SimulatedHandExpression, InputAction.CallbackContext> performed
            {
                add
                {
                    m_Performed += value;
                    if (!m_Subscribed)
                    {
                        m_Subscribed = true;
                        m_ToggleAction.action.performed += OnActionPerformed;
                    }
                }
                remove
                {
                    m_Performed -= value;
                    if (m_Performed == null)
                    {
                        m_Subscribed = false;
                        m_ToggleAction.action.performed -= OnActionPerformed;
                    }
                }
            }

            bool m_Subscribed;

            /// <inheritdoc/>
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                m_Name = m_ExpressionName.ToString();
            }

            /// <inheritdoc/>
            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                m_ExpressionName = new HandExpressionName(m_Name);
            }

            void OnActionPerformed(InputAction.CallbackContext context)
            {
                m_Performed?.Invoke(this, context);
            }
        }

        /// <summary>
        /// The device mode of the left and right device.
        /// </summary>
        /// <seealso cref="deviceMode"/>
        [Obsolete("DeviceMode has been deprecated in XRI 3.1.0 due to being moved out XR Device Simulator. Use DeviceMode in the SimulatedDeviceLifecycleManager instead.")]
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
        }

        [SerializeField]
        [Obsolete("m_RestingHandExpressionCapture has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
        HandExpressionCapture m_RestingHandExpressionCapture;

        [SerializeField]
        [Tooltip("The list of hand expressions to simulate.")]
        [Obsolete("m_SimulatedHandExpressions has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
        List<SimulatedHandExpression> m_SimulatedHandExpressions = new List<SimulatedHandExpression>();

        /// <summary>
        /// The list of simulated hand expressions for the device simulator.
        /// </summary>
        [Obsolete("simulatedHandExpressions has been deprecated in XRI 3.1.0. Update the XR Device Simulator sample in Package Manager or use simulatedHandExpressions in the SimulatedHandExpressionManager instead.")]
        public List<SimulatedHandExpression> simulatedHandExpressions => m_SimulatedHandExpressions;

        /// <summary>
        /// This boolean value indicates whether we remove other <see cref="XRHMD"/> devices in this session so that they don't conflict with the <see cref="XRDeviceSimulator"/>.
        /// A <see langword="true"/> value (default) means we remove all other <see cref="XRHMD"/> devices except the <see cref="XRSimulatedHMD"/> generated by the <see cref="XRDeviceSimulator"/>.
        /// A <see langword="false"/> value means we do not remove any other <see cref="XRHMD"/> devices.
        /// </summary>
        [Obsolete("removeOtherHMDDevices has been deprecated in XRI 3.1.0. Use removeOtherHMDDevices in the SimulatedDeviceLifecycleManager instead.")]
        public bool removeOtherHMDDevices
        {
            get => m_DeviceLifecycleManager != null ? m_DeviceLifecycleManager.removeOtherHMDDevices : default;
            set
            {
                if (m_DeviceLifecycleManager != null)
                    m_DeviceLifecycleManager.removeOtherHMDDevices = value;
                else
                    _ = value;
            }
        }

        /// <summary>
        /// Whether to create a simulated Hand Tracking Subsystem and provider on startup. Requires the XR Hands package.
        /// </summary>
        [Obsolete("handTrackingCapability has been deprecated in XRI 3.1.0. Use handTrackingCapability in the SimulatedDeviceLifecycleManager instead.")]
        public bool handTrackingCapability
        {
            get => m_DeviceLifecycleManager != null ? m_DeviceLifecycleManager.handTrackingCapability : default;
            set
            {
                if (m_DeviceLifecycleManager != null)
                    m_DeviceLifecycleManager.handTrackingCapability = value;
                else
                    _ = value;
            }
        }

        /// <summary>
        /// Whether the simulator is in controller mode or tracked hand mode.
        /// </summary>
        /// <seealso cref="DeviceMode"/>
        [Obsolete("deviceMode has been deprecated in XRI 3.1.0 due to being moved out XR Device Simulator. Use deviceMode in the SimulatedDeviceLifecycleManager instead.")]
        public DeviceMode deviceMode => m_DeviceLifecycleManager != null ? (DeviceMode)m_DeviceLifecycleManager.deviceMode : default;

        /// <summary>
        /// Add simulated XR devices to the Input System.
        /// </summary>
        [Obsolete("AddDevices has been deprecated in XRI 3.1.0 and will be removed in a future release. It has instead been moved to the SimulatedDeviceLifecycleManager.", false)]
        protected virtual void AddDevices()
        {
            if (m_DeviceLifecycleManager != null)
                m_DeviceLifecycleManager.AddDevices();
            else
                Debug.LogError("No Simulated Device Lifecycle Manager has been found so AddDevices() will not be called.", this);
        }

        /// <summary>
        /// Remove simulated XR devices from the Input System.
        /// </summary>
        [Obsolete("RemoveDevices has been deprecated in XRI 3.1.0 and will be removed in a future release. It has instead been moved to the SimulatedDeviceLifecycleManager.", false)]
        protected virtual void RemoveDevices()
        {
            if (m_DeviceLifecycleManager != null)
                m_DeviceLifecycleManager.RemoveDevices();
            else
                Debug.LogError("No Simulated Device Lifecycle Manager has been found so RemoveDevices() will not be called.", this);
        }

        [Obsolete("InitializeHandExpressions has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
        void InitializeHandExpressions()
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceLifecycleManager == null || m_DeviceLifecycleManager.simHandSubsystem == null || m_RestingHandExpressionCapture == null)
                return;

            // Pass the hand expression captures to the simulated hand subsystem
            m_DeviceLifecycleManager.simHandSubsystem.SetCapturedExpression(HandExpressionName.Default, m_RestingHandExpressionCapture);
            for (var index = 0; index < m_SimulatedHandExpressions.Count; ++index)
            {
                var simulatedExpression = m_SimulatedHandExpressions[index];

                if (simulatedExpression.capture != null)
                    m_DeviceLifecycleManager.simHandSubsystem.SetCapturedExpression(simulatedExpression.expressionName, simulatedExpression.capture);
                else
                    Debug.LogError($"Missing Capture reference for Simulated Hand Expression: {simulatedExpression.expressionName}", this);
            }
#endif
        }

#if XR_HANDS_1_1_OR_NEWER
        [Obsolete("SubscribeHandExpressionActionsDeprecated has been deprecated in XRI 3.1.0 and replaced with SubscribeHandExpressionActions.")]
        void SubscribeHandExpressionActionsDeprecated()
        {
            foreach (var simulatedExpression in m_SimulatedHandExpressions)
            {
                simulatedExpression.performed += OnHandExpressionPerformedDeprecated;
            }
        }

        [Obsolete("UnsubscribeHandExpressionActionsDeprecated has been deprecated in XRI 3.1.0 and replaced with UnsubscribeHandExpressionActions.")]
        void UnsubscribeHandExpressionActionsDeprecated()
        {
            foreach (var simulatedExpression in m_SimulatedHandExpressions)
            {
                simulatedExpression.performed -= OnHandExpressionPerformedDeprecated;
            }
        }

        [Obsolete("OnHandExpressionPerformedDeprecated has been deprecated in XRI 3.1.0 and replaced with OnHandExpressionPerformed.")]
        void OnHandExpressionPerformedDeprecated(SimulatedHandExpression simulatedExpression, InputAction.CallbackContext context)
        {
            ToggleHandExpressionDeprecated(simulatedExpression);
        }
#endif

        [Obsolete("ToggleHandExpressionDeprecated has been deprecated in XRI 3.1.0 and replaced with ToggleHandExpression.")]
        void ToggleHandExpressionDeprecated(SimulatedHandExpression simulatedExpression)
        {
#if XR_HANDS_1_1_OR_NEWER
            if (m_DeviceLifecycleManager == null || m_DeviceLifecycleManager.simHandSubsystem == null)
                return;

            // When toggling off, change back to the default resting hand. Otherwise, change to the expression pressed.
            if (manipulatingLeftHand)
            {
                m_LeftHandState.expressionName = m_LeftHandState.expressionName == simulatedExpression.expressionName
                    ? HandExpressionName.Default
                    : simulatedExpression.expressionName;
                m_DeviceLifecycleManager.simHandSubsystem.SetHandExpression(Handedness.Left, m_LeftHandState.expressionName);
            }

            if (manipulatingRightHand)
            {
                m_RightHandState.expressionName = m_RightHandState.expressionName == simulatedExpression.expressionName
                    ? HandExpressionName.Default
                    : simulatedExpression.expressionName;
                m_DeviceLifecycleManager.simHandSubsystem.SetHandExpression(Handedness.Right, m_RightHandState.expressionName);
            }
#endif
        }
    }
}
