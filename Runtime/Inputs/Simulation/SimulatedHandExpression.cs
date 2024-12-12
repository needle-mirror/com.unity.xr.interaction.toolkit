using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A hand expression that can be simulated by performing an input action.
    /// </summary>
    [Serializable]
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
        [Tooltip("The input to trigger the simulated hand expression.")]
        XRInputButtonReader m_ToggleInput;

        /// <summary>
        /// The input to trigger the simulated hand expression.
        /// </summary>
        public XRInputButtonReader toggleInput
        {
            get => m_ToggleInput;
            set => m_ToggleInput = value;
        }

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

        [SerializeField]
        [Tooltip("Whether or not this expression appears in the quick action list in the simulator.")]
        bool m_IsQuickAction;

        /// <summary>
        /// Whether or not this expression appears in the quick action list in the simulator.
        /// </summary>
        public bool isQuickAction
        {
            get => m_IsQuickAction;
            set => m_IsQuickAction = value;
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
    }
}
