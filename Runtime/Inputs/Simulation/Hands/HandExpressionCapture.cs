using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands
{
    /// <summary>
    /// Stores the joints for a hand expression for both a left and right hand. The pose data is used to simulate a specific hand expression
    /// in the XR Interaction Simulator.
    /// </summary>
    [Obsolete(k_HandExpressionCaptureDeprecated, true)]
    class HandExpressionCapture : ScriptableObject
    {
        const string k_HandExpressionCaptureDeprecated = "HandExpressionCapture has been marked for deprecation and will be functionally replaced by XRHandCaptureSequence in the XR Hands package.";

        /// <summary>
        /// The icon to represent the hand expression.
        /// </summary>
        public Sprite icon
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The captured poses of the left hand that is serialized to the <see cref="ScriptableObject"/> asset.
        /// </summary>
        public Pose[] leftHandCapturedPoses
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The captured poses of the right hand that is serialized to the <see cref="ScriptableObject"/> asset.
        /// </summary>
        public Pose[] rightHandCapturedPoses
        {
            get => default;
            set => _ = value;
        }
    }
}
