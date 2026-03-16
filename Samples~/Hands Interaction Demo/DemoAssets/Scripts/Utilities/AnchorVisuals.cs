using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Helper script used to control the Teleport Anchor visuals animations.
    /// </summary>
    public class AnchorVisuals : MonoBehaviour
    {
        /// <summary>
        /// The animation for the vertical glow element on the platform.
        /// </summary>
        [SerializeField]
        [Tooltip("The animation for the vertical glow element on the platform.")]
        Animation m_FadeAnimation;

        /// <summary>
        /// The arrow transform, at the center of the platform.
        /// </summary>
        [SerializeField]
        [Tooltip("The arrow transform, at the center of the platform.")]
        Transform m_Arrow;

        /// <summary>
        /// Height of the arrow transform when teleport ray hovers the teleport pad.
        /// </summary>
        [SerializeField]
        [Tooltip("Height of the arrow transform when teleport ray hovers the teleport pad.")]
        float m_TargetArrowHeight = 1f;

        /// <summary>
        /// Animation duration of the arrow transform to and from the target arrow height.
        /// </summary>
        [SerializeField]
        [Tooltip("Animation duration of the arrow transform to and from the target arrow height.")]
        float m_ArrowAnimationDuration = 0.2f;

        /// <summary>
        /// Animation curve of hte arrow transform to and from the target arrow height.
        /// </summary>
        [SerializeField]
        [Tooltip("Animation curve of hte arrow transform to and from the target arrow height.")]
        AnimationCurve m_AnimationCurve;

        Coroutine m_ArrowCoroutine;

#pragma warning disable CS0618 // Type or member is obsolete
        Vector3TweenableVariable m_ArrowHeight;
#pragma warning restore CS0618

        Vector3 m_InitialArrowScale;

        void Start()
        {
            if (m_FadeAnimation != null)
            {
                var fadeAnim = m_FadeAnimation;
                var clipName = m_FadeAnimation.clip.name;
                fadeAnim[clipName].normalizedTime = 1f;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            m_ArrowHeight = new Vector3TweenableVariable { animationCurve = m_AnimationCurve, };
#pragma warning restore CS0618
            m_InitialArrowScale = m_Arrow.localScale;
        }

        void Update()
        {
            m_Arrow.localPosition = m_ArrowHeight.Value;
        }

        /// <summary>
        /// Performs animations when teleport interactor enters the teleport anchor selection.
        /// </summary>
        public void OnAnchorEnter()
        {
            m_Arrow.localScale = m_InitialArrowScale;

            if (m_FadeAnimation != null)
            {
                var fadeAnim = m_FadeAnimation;
                var clipName = m_FadeAnimation.clip.name;
                fadeAnim[clipName].normalizedTime = 0f;
                fadeAnim[clipName].speed = 1f;
                fadeAnim.Play();
            }

            if (m_ArrowCoroutine != null)
                StopCoroutine(m_ArrowCoroutine);

            var arrowPosition = m_Arrow.localPosition;
            m_ArrowCoroutine = StartCoroutine(m_ArrowHeight.PlaySequence(arrowPosition, new float3(arrowPosition.x, m_TargetArrowHeight, arrowPosition.z), m_ArrowAnimationDuration));
        }

        /// <summary>
        /// Performs animations when teleport interactor exits the teleport anchor selection.
        /// </summary>
        public void OnAnchorExit()
        {
            if (m_FadeAnimation != null)
            {
                // Set time to 1, at the end of the animation, play at 1.5x speed
                var fadeAnim = m_FadeAnimation;
                var clipName = m_FadeAnimation.clip.name;
                fadeAnim[clipName].normalizedTime = 1f;
                fadeAnim[clipName].speed = -1.5f;
                fadeAnim.Play();
            }

            if (m_ArrowCoroutine != null)
                StopCoroutine(m_ArrowCoroutine);

            var arrowPosition = m_Arrow.localPosition;
            m_ArrowCoroutine = StartCoroutine(m_ArrowHeight.PlaySequence(arrowPosition, new float3(arrowPosition.x, 0f, arrowPosition.z), m_ArrowAnimationDuration));
        }

        /// <summary>
        /// Hides the arrow visual when teleporting.
        /// </summary>
        public void HideArrowOnTeleport()
        {
            m_Arrow.localScale = Vector3.zero;
        }
    }
}
