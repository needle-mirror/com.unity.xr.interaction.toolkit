using UnityEngine.Video;

namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// This script Toggles on / off the video player after each loop to fix a bug where the video player freezes after finishing a loop.
    /// </summary>
    public class ResetVideoOnLoop : MonoBehaviour
    {
        /// <summary>
        /// The VideoPlayer to control
        /// </summary>
        [SerializeField]
        VideoPlayer m_VideoPlayer;

        void Start()
        {
            m_VideoPlayer.loopPointReached += OnLoopPointReached;
        }

        void OnDestroy()
        {
            m_VideoPlayer.loopPointReached -= OnLoopPointReached;
        }

        /// <summary>
        /// Function called when the VideoPlayer's loop point has been reached
        /// </summary>
        /// <param name="source">The video player that is looping</param>
        void OnLoopPointReached(VideoPlayer source)
        {
            source.enabled = false;
            source.enabled = true;
        }
    }
}
