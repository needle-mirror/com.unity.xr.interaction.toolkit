using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

#if XR_HANDS_1_8_OR_NEWER
using UnityEngine.XR.Hands.Capture;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A hand expression that can be simulated by performing an input action.
    /// </summary>
    [Serializable]
    public class SimulatedHandExpression : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The sequence type of the capture sequence. A single-frame sequence type denotes a hand expression with a
        /// singular frame while a multi-frame type is a hand expression spanning multiple frames.
        /// </summary>
        /// <seealso cref="captureSequence"/>
        public enum SequenceType
        {
            /// <summary>
            /// A hand expression represented by a singular capture sequence frame.
            /// </summary>
            SingleFrame,

            /// <summary>
            /// A hand expression represented by a capture sequence spanning multiple frames.
            /// </summary>
            MultiFrame,
        }

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

#if XR_HANDS_1_8_OR_NEWER
        [SerializeField]
        [Tooltip("The captured sequence of poses and common gesture data that represent the hand expression" +
        "to be simulated when toggled by an input action.")]
        XRHandCaptureSequence m_CaptureSequence;

        /// <summary>
        /// The captured sequence of poses and common gesture data that represent
        /// the hand expression to be simulated when toggled by an input action.
        /// </summary>
        /// <seealso cref="toggleInput"/>
        public XRHandCaptureSequence captureSequence
        {
            get => m_CaptureSequence;
            set => m_CaptureSequence = value;
        }

#else
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("The sequence type whether single-frame or multi-frame.")]
        SequenceType m_SequenceType;

        /// <summary>
        /// The sequence type whether single-frame or multi-frame.
        /// </summary>
        /// <seealso cref="SequenceType"/>
        public SequenceType sequenceType
        {
            get => m_SequenceType;
            set => m_SequenceType = value;
        }

#if XR_HANDS_1_8_OR_NEWER
        [SerializeField]
        [Tooltip("The singular frame to be played in the case of a single-frame sequence type.")]
        int m_SingleFrameIndex;

        /// <summary>
        /// The singular frame to be played in the case of a single-frame sequence type.
        /// </summary>
        /// <seealso cref="sequenceType"/>
        internal int singleFrameIndex
        {
            get => m_SingleFrameIndex;
            set => m_SingleFrameIndex = value;
        }

        [SerializeField]
        [Tooltip("The frame used as the starting frame when replaying a multi-frame capture sequence.")]
        int m_MultiFrameStartIndex;

        /// <summary>
        /// The frame used as the starting frame when replaying a multi-frame capture sequence.
        /// </summary>
        internal int multiFrameStartIndex
        {
            get => m_MultiFrameStartIndex;
            set => m_MultiFrameStartIndex = value;
        }

        [SerializeField]
        [Tooltip("The frame used as the ending frame when replaying a multi-frame capture sequence.")]
        int m_MultiFrameEndIndex;

        /// <summary>
        ///  The frame used as the ending frame when replaying a multi-frame capture sequence.
        /// </summary>
        internal int multiFrameEndIndex
        {
            get => m_MultiFrameEndIndex;
            set => m_MultiFrameEndIndex = value;
        }

        [SerializeField]
        [Tooltip("The frame used as the starting frame when replaying the reverse sequence in a multi-frame capture sequence.")]
        int m_MultiFrameReverseStartIndex;

        /// <summary>
        /// The frame used as the starting frame when replaying the reverse sequence in a multi-frame capture sequence.
        /// </summary>
        internal int multiFrameReverseStartIndex
        {
            get => m_MultiFrameReverseStartIndex;
            set => m_MultiFrameReverseStartIndex = value;
        }

        [SerializeField]
        [Tooltip("The frame used as the ending frame when replaying the reverse sequence in a multi-frame capture sequence.")]
        int m_MultiFrameReverseEndIndex;

        /// <summary>
        /// The frame used as the ending frame when replaying the reverse sequence in a multi-frame capture sequence.
        /// </summary>
        internal int multiFrameReverseEndIndex
        {
            get => m_MultiFrameReverseEndIndex;
            set => m_MultiFrameReverseEndIndex = value;
        }

        [SerializeField]
        [Tooltip("The list of simulation key frame indices that represent the phases in which the capture sequence will be played back.")]
        List<int> m_InBetweenKeyFrameIndexList = new List<int>();

        /// <summary>
        /// The list of key frame indices that represent the phases in which the capture sequence will be played back.
        /// </summary>
        /// <remarks>
        /// These indices represent points between <see cref="multiFrameStartIndex"/> and <see cref="multiFrameEndIndex"/>.
        /// </remarks>
        internal List<int> inBetweenKeyFrameIndexList
        {
            get => m_InBetweenKeyFrameIndexList;
            set => m_InBetweenKeyFrameIndexList = value;
        }
#endif

#pragma warning disable CS0618
#if XR_HANDS_1_8_OR_NEWER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("The captured hand expression to simulate when the input action is performed.")]
        HandExpressionCapture m_Capture;
#pragma warning restore CS0618

        /// <summary>
        /// The captured expression to simulate when the input action is performed.
        /// </summary>
        [Obsolete("HandExpressionCapture has been marked for deprecation and will be functionally replaced by XRHandCaptureSequence in the XR Hands package.")]
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
