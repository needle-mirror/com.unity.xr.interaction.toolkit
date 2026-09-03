using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.SampleContent
{
    /// <summary>
    /// Show/hide components and UI based on gaze alignment with those elements
    /// </summary>
    public class UIComponentToggler : CalloutGazeController
    {
        /// <summary>
        /// The CanvasGroup to activate/deactivate and fade in/out
        /// </summary>
        [Header("Component Toggling")]
        [SerializeField]
        CanvasGroup m_CanvasGroup;

        /// <summary>
        /// The duration to fade in/out UI content
        /// </summary>
        [SerializeField]
        float m_FadeDuration = 0.25f;

        /// <summary>
        /// The min/max distance that denotes this transform being in range of the gaze transform
        /// </summary>
        [SerializeField]
        Vector2 m_MinMaxThresholdDistance = new Vector2(2f, 5f);

        /// <summary>
        /// The degree to which this transform is facing the gaze transform
        /// </summary>
        [SerializeField]
        Vector2 m_MinMaxFacingThreshold = new Vector2(0.8f, 0.995f);

        /// <summary>
        /// Distance at which content will be faded in/out when crossing this threshold
        /// </summary>
        [SerializeField]
        float m_MaxRenderingDistance = 15f;

        /// <summary>
        /// Image and text elements to toggle on/off
        /// </summary>
        [SerializeField]
        List<MonoBehaviour> m_ComponentsToToggle;

        /// <summary>
        /// GameObjects to set active/inactive while toggling
        /// </summary>
        [SerializeField]
        GameObject[] m_ObjectsToToggle;

        /// <summary>
        /// Set content to hidden initially
        /// </summary>
        [SerializeField]
        bool m_StartHidden = true;

        /// <summary>
        /// Bool denoting that the CanvasGroup GameObject should be enabled/disabled when toggling
        /// </summary>
        [SerializeField]
        bool m_DisableCanvasGroupObject = false;

#pragma warning disable CS0618 // Type or member is obsolete
        FloatTweenableVariable m_FloatFadeTweenableVariable = new FloatTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete
        bool m_Hidden = false;
        bool m_InRange = false;
        Coroutine m_FadeRoutine;
        Transform m_Transform;

        void Start()
        {
            m_Transform = transform;

            if (m_GazeTransform == null)
                m_GazeTransform = Camera.main.transform;

            if (m_CanvasGroup == null)
                m_CanvasGroup = GetComponentInChildren<CanvasGroup>();

            m_FacingThreshold = 0.98f;
            m_FacingEntered.AddListener(delegate { ToggleFade(false); });
            m_FacingExited.AddListener(delegate { ToggleFade(true); });

            m_FloatFadeTweenableVariable.Subscribe(UpdateFade);

            if (m_StartHidden)
                ToggleFade(true);
        }

        protected override void Update()
        {
            base.Update();

            float currentDistance = Vector3.Distance(m_Transform.position, m_GazeTransform.position);

            if (m_InRange)
            {
                float perc = (Mathf.Clamp(currentDistance, m_MinMaxThresholdDistance.x, m_MinMaxThresholdDistance.y) - m_MinMaxThresholdDistance.x) / (m_MinMaxThresholdDistance.y - m_MinMaxThresholdDistance.x);
                m_FacingThreshold = Mathf.Lerp(m_MinMaxFacingThreshold.x, m_MinMaxFacingThreshold.y, perc);

                if (currentDistance > m_MaxRenderingDistance)
                {
                    m_InRange = false;
                    ToggleFade(true);
                }
            }
            else
            {
                if (currentDistance <= m_MaxRenderingDistance)
                    m_InRange = true;
            }
        }

        void OnDestroy()
        {
            m_FacingEntered.RemoveListener(delegate { ToggleFade(false); });
            m_FacingExited.RemoveListener(delegate { ToggleFade(true); });
        }

        [ContextMenu("Get References")]
        void FindRendererReferences()
        {
            m_ComponentsToToggle = new List<MonoBehaviour>();
            List<Image> images = new List<Image>(GetComponentsInChildren<Image>());
            List<TMP_Text> texts = new List<TMP_Text>(GetComponentsInChildren<TMP_Text>());

            foreach (Image image in images)
            {
                m_ComponentsToToggle.Add(image);
            }

            foreach (TMP_Text text in texts)
            {
                m_ComponentsToToggle.Add(text);
            }
        }

        [ContextMenu("Toggle Components")]
        void ToggleFade()
        {
            ToggleFade(!m_Hidden);
        }

        void ToggleFade(bool toggle)
        {
            m_Hidden = toggle;
            if (!m_Hidden)
                ToggleComponents(true);

            if (m_FadeRoutine != null)
                StopCoroutine(m_FadeRoutine);

            m_FadeRoutine = StartCoroutine(m_FloatFadeTweenableVariable.PlaySequence(m_FloatFadeTweenableVariable.Value, m_Hidden ? 0f : 1f, m_FadeDuration, CompleteFade));
        }

        void ToggleComponents(bool show)
        {
            foreach (var c in m_ComponentsToToggle)
            {
                if (c != null)
                    c.enabled = show;
            }

            foreach (GameObject go in m_ObjectsToToggle)
            {
                if (go != null)
                    go.SetActive(show);
            }

            if (m_DisableCanvasGroupObject)
            {
                if (m_CanvasGroup != null)
                    m_CanvasGroup.gameObject.SetActive(show);
            }
        }

        void UpdateFade(float fadeAmount)
        {
            if (m_CanvasGroup != null)
                m_CanvasGroup.alpha = fadeAmount;
        }

        void CompleteFade()
        {
            if (m_FloatFadeTweenableVariable.Value <= 0f)
                ToggleComponents(false);
        }

        /// <summary>
        /// Show or hide the visual elements assigned to this component
        /// </summary>
        /// <param name="show"></param>
        public void ToggleShow(bool show)
        {
            if (show)
                m_FacingEntered.Invoke();
            else
                CheckPointerExit();
        }
    }
}
