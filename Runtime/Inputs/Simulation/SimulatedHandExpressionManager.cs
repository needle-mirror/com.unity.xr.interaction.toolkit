using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles setting up and keeping track of simulated hand expressions.
    /// </summary>
    /// <seealso cref="XRDeviceSimulator"/>
    /// <seealso cref="SimulatedDeviceLifecycleManager"/>
    /// <seealso cref="SimulatedHandExpression"/>
    [AddComponentMenu("XR/Debug/Simulated Hand Expression Manager (Deprecated)", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_SimulatedHandExpressionManager)]
    [HelpURL(XRHelpURLConstants.k_SimulatedHandExpressionManager)]
    [Obsolete(k_SimulatedHandExpressionManagerDeprecated, true)]
    public class SimulatedHandExpressionManager : MonoBehaviour
    {
        const string k_SimulatedHandExpressionManagerDeprecated = "SimulatedHandExpressionManager has been marked for deprecation and will be replaced by SimulatedHandPlaybackManager in tandem with the XRInteractionSimulator in future versions.";

        /// <summary>
        /// The list of simulated hand expressions.
        /// </summary>
        public List<SimulatedHandExpression> simulatedHandExpressions => default;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Start()
        {
        }
    }
}
