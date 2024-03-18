using System;
using System.Diagnostics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Feedback
{
    /// <summary>
    /// Data needed to describe a haptic impulse.
    /// </summary>
    [Serializable]
    public class HapticImpulseData
    {
        [SerializeField, Range(0f, 1f)]
        float m_Amplitude;

        /// <summary>
        /// The desired motor amplitude that should be within a [0-1] range.
        /// </summary>
        public float amplitude
        {
            get => m_Amplitude;
            set => m_Amplitude = value;
        }

        [SerializeField]
        float m_Duration;

        /// <summary>
        /// The desired duration of the impulse in seconds.
        /// </summary>
        public float duration
        {
            get => m_Duration;
            set => m_Duration = value;
        }

        [SerializeField]
        float m_Frequency;

        /// <summary>
        /// The desired frequency of the impulse in Hz.
        /// The default value of 0 means to use the default frequency of the device.
        /// Not all devices or XR Plug-ins support specifying a frequency.
        /// </summary>
        public float frequency
        {
            get => m_Frequency;
            set => m_Frequency = value;
        }
    }

    /// <summary>
    /// Component that responds to select and hover events by playing haptic impulses
    /// (in other words, vibrating the controller).
    /// </summary>
    /// <seealso cref="SimpleAudioFeedback"/>
    [AddComponentMenu("XR/Feedback/Simple Haptic Feedback", 11)]
    [HelpURL(XRHelpURLConstants.k_SimpleHapticFeedback)]
    public class SimpleHapticFeedback : MonoBehaviour
    {
        [SerializeField]
        [RequireInterface(typeof(IXRInteractor))]
        Object m_InteractorSourceObject;

        [SerializeField]
        HapticImpulsePlayer m_HapticImpulsePlayer;

        /// <summary>
        /// The Haptic Impulse Player component to use to play haptic impulses.
        /// </summary>
        public HapticImpulsePlayer hapticImpulsePlayer
        {
            get => m_HapticImpulsePlayer;
            set => m_HapticImpulsePlayer = value;
        }

        [SerializeField]
        bool m_PlaySelectEntered;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor starts selecting an interactable.
        /// </summary>
        public bool playSelectEntered
        {
            get => m_PlaySelectEntered;
            set => m_PlaySelectEntered = value;
        }

        [SerializeField]
        HapticImpulseData m_SelectEnteredData = new HapticImpulseData { amplitude = 0.5f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor starts selecting an interactable.
        /// </summary>
        public HapticImpulseData selectEnteredData
        {
            get => m_SelectEnteredData;
            set => m_SelectEnteredData = value;
        }

        [SerializeField]
        bool m_PlaySelectExited;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor stops selecting an interactable without being canceled.
        /// </summary>
        public bool playSelectExited
        {
            get => m_PlaySelectExited;
            set => m_PlaySelectExited = value;
        }

        [SerializeField]
        HapticImpulseData m_SelectExitedData = new HapticImpulseData { amplitude = 0.5f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor stops selecting an interactable without being canceled.
        /// </summary>
        public HapticImpulseData selectExitedData
        {
            get => m_SelectExitedData;
            set => m_SelectExitedData = value;
        }

        [SerializeField]
        bool m_PlaySelectCanceled;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor stops selecting an interactable due to being canceled.
        /// </summary>
        public bool playSelectCanceled
        {
            get => m_PlaySelectCanceled;
            set => m_PlaySelectCanceled = value;
        }

        [SerializeField]
        HapticImpulseData m_SelectCanceledData = new HapticImpulseData { amplitude = 0.5f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor stops selecting an interactable due to being canceled.
        /// </summary>
        public HapticImpulseData selectCanceledData
        {
            get => m_SelectCanceledData;
            set => m_SelectCanceledData = value;
        }

        [SerializeField]
        bool m_PlayHoverEntered;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor starts hovering over an interactable.
        /// </summary>
        public bool playHoverEntered
        {
            get => m_PlayHoverEntered;
            set => m_PlayHoverEntered = value;
        }

        [SerializeField]
        HapticImpulseData m_HoverEnteredData = new HapticImpulseData { amplitude = 0.25f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor starts hovering over an interactable.
        /// </summary>
        public HapticImpulseData hoverEnteredData
        {
            get => m_HoverEnteredData;
            set => m_HoverEnteredData = value;
        }

        [SerializeField]
        bool m_PlayHoverExited;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor stops hovering over an interactable without being canceled.
        /// </summary>
        public bool playHoverExited
        {
            get => m_PlayHoverExited;
            set => m_PlayHoverExited = value;
        }

        [SerializeField]
        HapticImpulseData m_HoverExitedData = new HapticImpulseData { amplitude = 0.25f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor stops hovering over an interactable without being canceled.
        /// </summary>
        public HapticImpulseData hoverExitedData
        {
            get => m_HoverExitedData;
            set => m_HoverExitedData = value;
        }

        [SerializeField]
        bool m_PlayHoverCanceled;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor stops hovering over an interactable due to being canceled.
        /// </summary>
        public bool playHoverCanceled
        {
            get => m_PlayHoverCanceled;
            set => m_PlayHoverCanceled = value;
        }

        [SerializeField]
        HapticImpulseData m_HoverCanceledData = new HapticImpulseData { amplitude = 0.25f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor stops hovering over an interactable due to being canceled.
        /// </summary>
        public HapticImpulseData hoverCanceledData
        {
            get => m_HoverCanceledData;
            set => m_HoverCanceledData = value;
        }

        [SerializeField]
        bool m_AllowHoverHapticsWhileSelecting;

        /// <summary>
        /// Whether to allow hover haptics to play while the interactor is selecting an interactable.
        /// </summary>
        public bool allowHoverHapticsWhileSelecting
        {
            get => m_AllowHoverHapticsWhileSelecting;
            set => m_AllowHoverHapticsWhileSelecting = value;
        }

        readonly UnityObjectReferenceCache<IXRInteractor, Object> m_InteractorSource = new UnityObjectReferenceCache<IXRInteractor, Object>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected void Reset()
        {
#if UNITY_EDITOR
            m_InteractorSourceObject = GetComponentInParent<IXRInteractor>(true) as Object;
            m_HapticImpulsePlayer = GetComponentInParent<HapticImpulsePlayer>(true);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_InteractorSourceObject == null)
                m_InteractorSourceObject = GetComponentInParent<IXRInteractor>(true) as Object;

            if (m_PlaySelectEntered || m_PlaySelectExited || m_PlaySelectCanceled ||
                m_PlayHoverEntered || m_PlayHoverExited || m_PlayHoverCanceled)
            {
                CreateHapticImpulsePlayer();
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            Subscribe(GetInteractorSource());
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            Unsubscribe(GetInteractorSource());
        }

        /// <summary>
        /// Gets the interactor this behavior should subscribe to for events.
        /// </summary>
        /// <returns>Returns the interactor this behavior should subscribe to for events.</returns>
        /// <seealso cref="SetInteractorSource"/>
        public IXRInteractor GetInteractorSource()
        {
            return m_InteractorSource.Get(m_InteractorSourceObject);
        }

        /// <summary>
        /// Sets the interactor this behavior should subscribe to for events.
        /// </summary>
        /// <param name="interactor">The interactor this behavior should subscribe to for events.</param>
        /// <remarks>
        /// This also sets the serialized field to the given interactor as a Unity Object.
        /// </remarks>
        /// <seealso cref="GetInteractorSource"/>
        public void SetInteractorSource(IXRInteractor interactor)
        {
            if (Application.isPlaying && isActiveAndEnabled)
                Unsubscribe(m_InteractorSource.Get(m_InteractorSourceObject));

            m_InteractorSource.Set(ref m_InteractorSourceObject, interactor);

            if (Application.isPlaying && isActiveAndEnabled)
                Subscribe(interactor);
        }

        /// <summary>
        /// Sends a haptic impulse to the referenced haptic impulse player component.
        /// </summary>
        /// <param name="data">The parameters of the haptic impulse.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="SendHapticImpulse(float,float,float)"/>
        protected bool SendHapticImpulse(HapticImpulseData data)
        {
            return data != null && SendHapticImpulse(data.amplitude, data.duration, data.frequency);
        }

        /// <summary>
        /// Sends a haptic impulse to the referenced haptic impulse player component.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <param name="frequency">The desired frequency of the impulse in Hz. A value of 0 means to use the default frequency of the device.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="HapticImpulsePlayer.SendHapticImpulse(float,float,float)"/>
        protected bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
            if (m_HapticImpulsePlayer == null)
                CreateHapticImpulsePlayer();

            return m_HapticImpulsePlayer.SendHapticImpulse(amplitude, duration, frequency);
        }

        void CreateHapticImpulsePlayer()
        {
            m_HapticImpulsePlayer = HapticImpulsePlayer.GetOrCreateInHierarchy(gameObject);
        }

        void Subscribe(IXRInteractor interactor)
        {
            if (interactor == null || (interactor is Object interactorObject && interactorObject == null))
                return;

            if (interactor is IXRSelectInteractor selectInteractor)
            {
                selectInteractor.selectEntered.AddListener(OnSelectEntered);
                selectInteractor.selectExited.AddListener(OnSelectExited);
            }

            if (interactor is IXRHoverInteractor hoverInteractor)
            {
                hoverInteractor.hoverEntered.AddListener(OnHoverEntered);
                hoverInteractor.hoverExited.AddListener(OnHoverExited);
            }
        }

        void Unsubscribe(IXRInteractor interactor)
        {
            if (interactor == null || (interactor is Object interactorObject && interactorObject == null))
                return;

            if (interactor is IXRSelectInteractor selectInteractor)
            {
                selectInteractor.selectEntered.RemoveListener(OnSelectEntered);
                selectInteractor.selectExited.RemoveListener(OnSelectExited);
            }

            if (interactor is IXRHoverInteractor hoverInteractor)
            {
                hoverInteractor.hoverEntered.RemoveListener(OnHoverEntered);
                hoverInteractor.hoverExited.RemoveListener(OnHoverExited);
            }
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (m_PlaySelectEntered)
                SendHapticImpulse(m_SelectEnteredData);
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            if (m_PlaySelectCanceled && args.isCanceled)
                SendHapticImpulse(m_SelectCanceledData);

            if (m_PlaySelectExited && !args.isCanceled)
                SendHapticImpulse(m_SelectExitedData);
        }

        void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (m_PlayHoverEntered && IsHoverHapticsAllowed(args.interactorObject, args.interactableObject))
                SendHapticImpulse(m_HoverEnteredData);
        }

        void OnHoverExited(HoverExitEventArgs args)
        {
            if (!IsHoverHapticsAllowed(args.interactorObject, args.interactableObject))
                return;

            if (m_PlayHoverCanceled && args.isCanceled)
                SendHapticImpulse(m_HoverCanceledData);

            if (m_PlayHoverExited && !args.isCanceled)
                SendHapticImpulse(m_HoverExitedData);
        }

        bool IsHoverHapticsAllowed(IXRInteractor interactor, IXRInteractable interactable)
        {
            return m_AllowHoverHapticsWhileSelecting || !IsSelecting(interactor, interactable);
        }

        static bool IsSelecting(IXRInteractor interactor, IXRInteractable interactable)
        {
            return interactor is IXRSelectInteractor selectInteractor &&
                interactable is IXRSelectInteractable selectable &&
                selectInteractor.IsSelecting(selectable);
        }
    }
}
