using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles setting up and keeping track of simulated hand expressions.
    /// </summary>
    /// <seealso cref="XRDeviceSimulator"/>
    /// <seealso cref="SimulatedDeviceLifecycleManager"/>
    /// <seealso cref="SimulatedHandExpression"/>
    [AddComponentMenu("XR/Debug/Simulated Hand Expression Manager", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_SimulatedHandExpressionManager)]
    [HelpURL(XRHelpURLConstants.k_SimulatedHandExpressionManager)]
    public class SimulatedHandExpressionManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of hand expressions to simulate.")]
        List<SimulatedHandExpression> m_SimulatedHandExpressions = new List<SimulatedHandExpression>();

        /// <summary>
        /// The list of simulated hand expressions.
        /// </summary>
        public List<SimulatedHandExpression> simulatedHandExpressions => m_SimulatedHandExpressions;

        [SerializeField]
        [Tooltip("The resting hand expression to use when no other hand expression is active.")]
        HandExpressionCapture m_RestingHandExpressionCapture;

        /// <summary>
        /// The resting hand expression to use when no other hand expression is active.
        /// </summary>
        internal HandExpressionCapture restingHandExpressionCapture
        {
            get => m_RestingHandExpressionCapture;
            set => m_RestingHandExpressionCapture = value;
        }

        SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Start()
        {
            m_DeviceLifecycleManager = XRSimulatorUtility.FindCreateSimulatedDeviceLifecycleManager(gameObject);
            InitializeHandExpressions();
        }

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
    }
}
