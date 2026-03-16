using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Controller handling the playback of video tutorial content
    /// </summary>
    public class TutorialVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// The header text object drawn on this panel
        /// </summary>
        [SerializeField]
        TMP_Text m_HeaderText;

        /// <summary>
        /// The CanvasGroup that handles the fading of the tutorial UI content
        /// </summary>
        [SerializeField]
        CanvasGroup m_VideoFadeGroup;

        /// <summary>
        /// The image that fades in/out when showing/hiding the tutorial video content
        /// </summary>
        [SerializeField]
        RawImage m_VideoImage;

        /// <summary>
        /// The VideoPlayer that displays the tutorial video content
        /// </summary>
        [SerializeField]
        VideoPlayer m_VideoPlayer;

        /// <summary>
        /// The parent game object that houses the tutorial UI content
        /// </summary>
        [SerializeField]
        GameObject m_UIContainer;

        /// <summary>
        /// The speed at which the canvas content fades in/out
        /// </summary>
        [SerializeField]
        float m_FadeSpeedVideo = 1.0f;

        IEnumerator m_FadeEnumerator;

        void Start()
        {
            m_VideoFadeGroup.alpha = 0f;
            m_VideoImage.material.color = new Color(1f, 1f, 1f, 0f);

            StartCoroutine(DelayedStart());
        }

        IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(1.5f);

            StartCoroutine(FadeInVideo());
        }

        /// <summary>
        /// Called in the "Select" event on the Hand Teleport Interactor
        /// </summary>
        public void HideTutorial()
        {
            if (m_FadeEnumerator != null)
                StopCoroutine(m_FadeEnumerator);

            if (!gameObject.activeInHierarchy)
                return;

            m_FadeEnumerator = FadeOutVideo();
            StartCoroutine(m_FadeEnumerator);
        }

        IEnumerator FadeInVideo()
        {
            while (m_VideoFadeGroup.alpha < 1f)
            {
                m_VideoFadeGroup.alpha += Time.deltaTime * m_FadeSpeedVideo;
                m_VideoImage.material.color = new Color(1f, 1f, 1f, m_VideoFadeGroup.alpha);
                yield return null;
            }
        }

        IEnumerator FadeOutVideo()
        {
            while (m_VideoFadeGroup.alpha > 0f)
            {
                float fadeAmount = Time.deltaTime * m_FadeSpeedVideo;
                m_VideoFadeGroup.alpha -= fadeAmount;
                m_VideoImage.material.color = new Color(1f, 1f, 1f, m_VideoFadeGroup.alpha);

                yield return null;
            }

            m_VideoPlayer.targetTexture.Release();

            m_UIContainer.SetActive(false);
        }
    }
}
