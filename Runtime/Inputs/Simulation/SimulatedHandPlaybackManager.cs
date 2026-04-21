using System.Collections.Generic;

#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif

#if XR_HANDS_1_8_OR_NEWER
using UnityEngine.XR.Hands.Capture;
using UnityEngine.XR.Hands.Capture.Playback;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles the playback of simulated hand expressions.
    /// </summary>
    /// <seealso cref="XRInteractionSimulator"/>
    /// <seealso cref="SimulatedDeviceLifecycleManager"/>
    /// <seealso cref="SimulatedHandExpression"/>
    [AddComponentMenu("XR/Debug/Simulated Hand Playback Manager", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_SimulatedHandPlaybackManager)]
    [HelpURL(XRHelpURLConstants.k_SimulatedHandPlaybackManager)]
    public class SimulatedHandPlaybackManager : MonoBehaviour
    {
        /// <summary>
        /// Hand playback state data for a <see cref="SimulatedHandExpression"/> in the <see cref="SimulatedHandPlaybackManager"/>.
        /// </summary>
        internal struct HandPlaybackData
        {
#if XR_HANDS_1_8_OR_NEWER
            /// <summary>
            /// The handedness of the playback.
            /// </summary>
            public Handedness handedness;

            /// <summary>
            /// Whether or not a segment of the <see cref="XRHandCaptureSequence"/> is currently being played through.
            /// </summary>
            public bool multiFrameSequenceSegmentIsPlaying;

            /// <summary>
            /// Whether or not we reset to the starting frame for the multi-frame <see cref="SimulatedHandExpression"/>.
            /// </summary>
            public bool resetToStartFrame;

            /// <summary>
            /// The current index for <see cref="SimulatedHandExpression.inBetweenKeyFrameIndexList"/> in playback.
            /// </summary>
            public int inBetweenPlaybackIndex;

            /// <summary>
            /// The next keyframe that playback will pause at.
            /// </summary>
            public int nextStoppingFrameKey;

            /// <summary>
            /// The simulated hand expression associated with this playback.
            /// </summary>
            public SimulatedHandExpression handExpression;

            public HandPlaybackData(Handedness leftOrRight)
            {
                handedness = leftOrRight;
                multiFrameSequenceSegmentIsPlaying = false;
                resetToStartFrame = false;
                inBetweenPlaybackIndex = 0;
                nextStoppingFrameKey = 0;
                handExpression = default;
            }
#endif
        }

        [SerializeField]
        [Tooltip("The list of hand expressions to simulate.")]
        List<SimulatedHandExpression> m_SimulatedHandExpressions = new List<SimulatedHandExpression>();

        /// <summary>
        /// The list of simulated hand expressions.
        /// </summary>
        public List<SimulatedHandExpression> simulatedHandExpressions => m_SimulatedHandExpressions;

        [SerializeField]
        [Tooltip("The resting hand expression to use when no other hand expression is active.")]
        SimulatedHandExpression m_RestingHandExpression;

        /// <summary>
        /// The resting hand expression to use when no other hand expression is active.
        /// </summary>
        public SimulatedHandExpression restingHandExpression
        {
            get => m_RestingHandExpression;
            set => m_RestingHandExpression = value;
        }

        SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;
        HandPlaybackData m_LeftPlaybackData;
        HandPlaybackData m_RightPlaybackData;


        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
#if XR_HANDS_1_8_OR_NEWER
            m_LeftPlaybackData = new HandPlaybackData(Handedness.Left);
            m_RightPlaybackData = new HandPlaybackData(Handedness.Right);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Start()
        {
#if XR_HANDS_1_1_OR_NEWER
            m_DeviceLifecycleManager = XRSimulatorUtility.FindCreateSimulatedDeviceLifecycleManager(gameObject);
#endif
        }

#if XR_HANDS_1_8_OR_NEWER
        /// <summary>
        /// Handles the pausing and resetting to start of a sequence by querying the state of the playback every frame.
        /// </summary>
        internal void ProcessHandSequencePlayback(Handedness leftOrRight)
        {
            if (leftOrRight == Handedness.Left)
                ProcessHandSequencePlayback(ref m_LeftPlaybackData);
            else if (leftOrRight == Handedness.Right)
                ProcessHandSequencePlayback(ref m_RightPlaybackData);
        }

        /// <summary>
        /// Handles state management of the hand playback based on input from the <see cref="XRInteractionSimulator"/>.
        /// </summary>
        /// <param name="skipToStartOrEnd">
        /// If set to true, it will ignore any in-between keyframes and skip to the sequence end or reset to the sequence start
        /// based on the current position in the sequence. This is normally used for hotkey input from the <see cref="XRInteractionSimulator"/>.
        /// </param>
        internal void ToggleHandPlayback(SimulatedHandExpression handExpression, bool skipToStartOrEnd, Handedness leftOrRight)
        {
            if (leftOrRight == Handedness.Left)
                ToggleHandPlayback(handExpression, skipToStartOrEnd, ref m_LeftPlaybackData);
            else if (leftOrRight == Handedness.Right)
                ToggleHandPlayback(handExpression, skipToStartOrEnd, ref m_RightPlaybackData);
        }

        void ProcessHandSequencePlayback(ref HandPlaybackData playbackData)
        {
            if (m_DeviceLifecycleManager == null || !playbackData.multiFrameSequenceSegmentIsPlaying)
                return;

            var playback = playbackData.handedness == Handedness.Left ? m_DeviceLifecycleManager.leftHandPlayback : m_DeviceLifecycleManager.rightHandPlayback;

            if (playback != null && playback.frameIndex == playbackData.nextStoppingFrameKey)
            {
                if (playbackData.resetToStartFrame)
                {
                    playback.Stop();
                    playback.frameIndex = playbackData.handExpression.multiFrameStartIndex;
                    playbackData.nextStoppingFrameKey = playbackData.handExpression.multiFrameStartIndex;
                    playbackData.inBetweenPlaybackIndex = 0;
                    playbackData.resetToStartFrame = false;
                }
                else
                {
                    playback.Pause();
                }

                playbackData.multiFrameSequenceSegmentIsPlaying = false;
            }
        }

        void ToggleHandPlayback(SimulatedHandExpression handExpression, bool skipToStartOrEnd, ref HandPlaybackData playbackData)
        {
            if (m_DeviceLifecycleManager == null)
                return;

            var playback = m_DeviceLifecycleManager.handSubsystem?.GetPlayback(playbackData.handedness);
            if (playback != null)
            {
                if (playback.sourceCaptureSequence != handExpression.captureSequence)
                {
                    playback.sourceCaptureSequence = handExpression.captureSequence;
                    playback.frameIndex = handExpression.sequenceType == SimulatedHandExpression.SequenceType.SingleFrame ?
                        handExpression.singleFrameIndex : handExpression.multiFrameStartIndex;
                    playbackData.nextStoppingFrameKey = playback.frameIndex;
                    playbackData.inBetweenPlaybackIndex = 0;
                    playbackData.handExpression = handExpression;
                }
                else if (handExpression.sequenceType == SimulatedHandExpression.SequenceType.MultiFrame)
                {
                    if (!playback.isPlaying)
                        playback.Play();

                    if (skipToStartOrEnd)
                    {
                        if (playbackData.nextStoppingFrameKey == handExpression.multiFrameStartIndex
                            || (playbackData.nextStoppingFrameKey > handExpression.multiFrameStartIndex
                            && playbackData.nextStoppingFrameKey < handExpression.multiFrameEndIndex))
                        {
                            //If the next stopping frame is start or an in-between frame, skip to the end.
                            playbackData.nextStoppingFrameKey = handExpression.multiFrameEndIndex;
                            playbackData.multiFrameSequenceSegmentIsPlaying = true;
                        }
                        else if (playbackData.nextStoppingFrameKey == handExpression.multiFrameEndIndex)
                        {
                            //If the next stopping frame is end, begin reverse segment playback.
                            playback.frameIndex = handExpression.multiFrameReverseStartIndex;
                            playbackData.nextStoppingFrameKey = handExpression.multiFrameReverseEndIndex;
                            playbackData.multiFrameSequenceSegmentIsPlaying = true;
                            playbackData.resetToStartFrame = true;
                        }
                        else
                        {
                            //If we are in the reverse segment, just skip to the end of the reverse segment and prepare for reset to start.
                            playback.frameIndex = handExpression.multiFrameReverseEndIndex;
                            playbackData.resetToStartFrame = true;
                        }
                    }
                    else
                    {
                        if (playbackData.nextStoppingFrameKey == handExpression.multiFrameReverseEndIndex)
                            return;

                        //If key is pressed before playback complete, skip directly to the next stopping frame.
                        if (playbackData.multiFrameSequenceSegmentIsPlaying)
                            playback.frameIndex = playbackData.nextStoppingFrameKey;

                        if (playbackData.nextStoppingFrameKey == handExpression.multiFrameStartIndex && handExpression.inBetweenKeyFrameIndexList.Count == 0)
                        {
                            //If next stopping frame is start, and there are no in-between frames, play all the way to the end.
                            playbackData.nextStoppingFrameKey = handExpression.multiFrameEndIndex;
                        }
                        else if (playbackData.nextStoppingFrameKey == handExpression.multiFrameEndIndex)
                        {
                           //If the next stopping frame is end, begin reverse segment playback.
                            playback.frameIndex = handExpression.multiFrameReverseStartIndex;
                            playbackData.nextStoppingFrameKey = handExpression.multiFrameReverseEndIndex;
                            playbackData.resetToStartFrame = true;
                        }
                        else if (playbackData.inBetweenPlaybackIndex == handExpression.inBetweenKeyFrameIndexList.Count)
                        {
                            //If we have reached the end of the in-between frames, play to the end key frame.
                            playbackData.nextStoppingFrameKey = handExpression.multiFrameEndIndex;
                            playbackData.inBetweenPlaybackIndex = 0;
                        }
                        else
                        {
                            //Iterating through the inbetween key frames.
                            playbackData.nextStoppingFrameKey = handExpression.inBetweenKeyFrameIndexList[playbackData.inBetweenPlaybackIndex];
                            playbackData.inBetweenPlaybackIndex++;
                        }

                        playbackData.multiFrameSequenceSegmentIsPlaying = true;
                    }
                }
            }
        }
#endif
    }
}
