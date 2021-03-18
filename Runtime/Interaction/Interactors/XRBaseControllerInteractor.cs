using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Abstract base class from which all interactors that are controller driven derive.
    /// This class hooks into the interaction system (via <see cref="XRInteractionManager"/>) and provides base virtual methods for handling
    /// hover and selection. Additionally, this class provides functionality for checking the controller's selection status
    /// and hiding the controller on selection.
    /// </summary>
    public abstract partial class XRBaseControllerInteractor : XRBaseInteractor
    {
        /// <summary>
        /// This defines the type of input that triggers an interaction.
        /// </summary>
        public enum InputTriggerType
        {
            /// <summary>
            /// The interaction will start when the button is pressed.
            /// </summary>
            State,

            /// <summary>
            /// This interaction will start on the select button/key being activated or deactivated.
            /// </summary>
            /// <seealso cref="XRBaseController.selectInteractionState"/>
            StateChange,

            /// <summary>
            /// This interaction will start on the select button/key being activated
            /// and remain engaged until the second time the input is activated.
            /// </summary>
            Toggle,

            /// <summary>
            /// Start interaction on select enter, and wait until the second time the
            /// select key is depressed before exiting the interaction.  This is useful
            /// for grabbing and throwing without having to hold a button down.
            /// </summary>
            Sticky,
        }

        [SerializeField]
        InputTriggerType m_SelectActionTrigger = InputTriggerType.State;
        /// <summary>
        /// Controls whether this interactor toggles selection on button press (rather than selection on press).
        /// </summary>
        public InputTriggerType selectActionTrigger
        {
            get => m_SelectActionTrigger;
            set => m_SelectActionTrigger = value;
        }

        [SerializeField]
        bool m_HideControllerOnSelect;
        /// <summary>
        /// Controls whether this interactor should hide the controller on selection.
        /// </summary>
        public bool hideControllerOnSelect
        {
            get => m_HideControllerOnSelect;
            set
            {
                m_HideControllerOnSelect = value;
                if (!m_HideControllerOnSelect && m_Controller != null)
                    m_Controller.hideControllerModel = false;
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnSelectEnter")]
        bool m_PlayAudioClipOnSelectEntered;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Select Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectEntered"/>
        public bool playAudioClipOnSelectEntered
        {
            get => m_PlayAudioClipOnSelectEntered;
            set => m_PlayAudioClipOnSelectEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnSelectEnter")]
        AudioClip m_AudioClipForOnSelectEntered;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
        public AudioClip audioClipForOnSelectEntered
        {
            get => m_AudioClipForOnSelectEntered;
            set => m_AudioClipForOnSelectEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnSelectExit")]
        bool m_PlayAudioClipOnSelectExited;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Select Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectExited"/>
        public bool playAudioClipOnSelectExited
        {
            get => m_PlayAudioClipOnSelectExited;
            set => m_PlayAudioClipOnSelectExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnSelectExit")]
        AudioClip m_AudioClipForOnSelectExited;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
        public AudioClip audioClipForOnSelectExited
        {
            get => m_AudioClipForOnSelectExited;
            set => m_AudioClipForOnSelectExited = value;
        }

        [SerializeField]
        bool m_PlayAudioClipOnSelectCanceled;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Select Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectCanceled"/>
        public bool playAudioClipOnSelectCanceled
        {
            get => m_PlayAudioClipOnSelectCanceled;
            set => m_PlayAudioClipOnSelectCanceled = value;
        }

        [SerializeField]
        AudioClip m_AudioClipForOnSelectCanceled;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectCanceled"/>
        public AudioClip audioClipForOnSelectCanceled
        {
            get => m_AudioClipForOnSelectCanceled;
            set => m_AudioClipForOnSelectCanceled = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnHoverEnter")]
        bool m_PlayAudioClipOnHoverEntered;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Hover Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverEntered"/>
        public bool playAudioClipOnHoverEntered
        {
            get => m_PlayAudioClipOnHoverEntered;
            set => m_PlayAudioClipOnHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnHoverEnter")]
        AudioClip m_AudioClipForOnHoverEntered;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        public AudioClip audioClipForOnHoverEntered
        {
            get => m_AudioClipForOnHoverEntered;
            set => m_AudioClipForOnHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnHoverExit")]
        bool m_PlayAudioClipOnHoverExited;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Hover Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverExited"/>
        public bool playAudioClipOnHoverExited
        {
            get => m_PlayAudioClipOnHoverExited;
            set => m_PlayAudioClipOnHoverExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnHoverExit")]
        AudioClip m_AudioClipForOnHoverExited;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        public AudioClip audioClipForOnHoverExited
        {
            get => m_AudioClipForOnHoverExited;
            set => m_AudioClipForOnHoverExited = value;
        }

        [SerializeField]
        bool m_PlayAudioClipOnHoverCanceled;
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Hover Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverCanceled"/>
        public bool playAudioClipOnHoverCanceled
        {
            get => m_PlayAudioClipOnHoverCanceled;
            set => m_PlayAudioClipOnHoverCanceled = value;
        }

        [SerializeField]
        AudioClip m_AudioClipForOnHoverCanceled;
        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        public AudioClip audioClipForOnHoverCanceled
        {
            get => m_AudioClipForOnHoverCanceled;
            set => m_AudioClipForOnHoverCanceled = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnSelectEnter")]
        bool m_PlayHapticsOnSelectEntered;
        /// <summary>
        /// Controls whether to play haptics on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="hapticSelectEnterDuration"/>
        public bool playHapticsOnSelectEntered
        {
            get => m_PlayHapticsOnSelectEntered;
            set => m_PlayHapticsOnSelectEntered = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticSelectEnterIntensity;
        /// <summary>
        /// The Haptics intensity to play on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterDuration"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        public float hapticSelectEnterIntensity
        {
            get => m_HapticSelectEnterIntensity;
            set => m_HapticSelectEnterIntensity= value;
        }

        [SerializeField]
        float m_HapticSelectEnterDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        public float hapticSelectEnterDuration
        {
            get => m_HapticSelectEnterDuration;
            set => m_HapticSelectEnterDuration = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnSelectExit")]
        bool m_PlayHapticsOnSelectExited;
        /// <summary>
        /// Controls whether to play haptics on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="hapticSelectExitDuration"/>
        public bool playHapticsOnSelectExited
        {
            get => m_PlayHapticsOnSelectExited;
            set => m_PlayHapticsOnSelectExited = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticSelectExitIntensity;
        /// <summary>
        /// The Haptics intensity to play on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitDuration"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        public float hapticSelectExitIntensity
        {
            get => m_HapticSelectExitIntensity;
            set => m_HapticSelectExitIntensity= value;
        }

        [SerializeField]
        float m_HapticSelectExitDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        public float hapticSelectExitDuration
        {
            get => m_HapticSelectExitDuration;
            set => m_HapticSelectExitDuration = value;
        }

        [SerializeField]
        bool m_PlayHapticsOnSelectCanceled;
        /// <summary>
        /// Controls whether to play haptics on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="hapticSelectCancelDuration"/>
        public bool playHapticsOnSelectCanceled
        {
            get => m_PlayHapticsOnSelectCanceled;
            set => m_PlayHapticsOnSelectCanceled = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticSelectCancelIntensity;
        /// <summary>
        /// The Haptics intensity to play on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelDuration"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        public float hapticSelectCancelIntensity
        {
            get => m_HapticSelectCancelIntensity;
            set => m_HapticSelectCancelIntensity= value;
        }

        [SerializeField]
        float m_HapticSelectCancelDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        public float hapticSelectCancelDuration
        {
            get => m_HapticSelectCancelDuration;
            set => m_HapticSelectCancelDuration = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnHoverEnter")]
        bool m_PlayHapticsOnHoverEntered;
        /// <summary>
        /// Controls whether to play haptics on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="hapticHoverEnterDuration"/>
        public bool playHapticsOnHoverEntered
        {
            get => m_PlayHapticsOnHoverEntered;
            set => m_PlayHapticsOnHoverEntered = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticHoverEnterIntensity;
        /// <summary>
        /// The Haptics intensity to play on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterDuration"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        public float hapticHoverEnterIntensity
        {
            get => m_HapticHoverEnterIntensity;
            set => m_HapticHoverEnterIntensity = value;
        }

        [SerializeField]
        float m_HapticHoverEnterDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        public float hapticHoverEnterDuration
        {
            get => m_HapticHoverEnterDuration;
            set => m_HapticHoverEnterDuration = value;
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnHoverExit")]
        bool m_PlayHapticsOnHoverExited;
        /// <summary>
        /// Controls whether to play haptics on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="hapticHoverExitDuration"/>
        public bool playHapticsOnHoverExited
        {
            get => m_PlayHapticsOnHoverExited;
            set => m_PlayHapticsOnHoverExited = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticHoverExitIntensity;
        /// <summary>
        /// The Haptics intensity to play on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitDuration"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        public float hapticHoverExitIntensity
        {
            get => m_HapticHoverExitIntensity;
            set => m_HapticHoverExitIntensity = value;
        }

        [SerializeField]
        float m_HapticHoverExitDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        public float hapticHoverExitDuration
        {
            get => m_HapticHoverExitDuration;
            set => m_HapticHoverExitDuration = value;
        }

        [SerializeField]
        bool m_PlayHapticsOnHoverCanceled;
        /// <summary>
        /// Controls whether to play haptics on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="hapticHoverCancelDuration"/>
        public bool playHapticsOnHoverCanceled
        {
            get => m_PlayHapticsOnHoverCanceled;
            set => m_PlayHapticsOnHoverCanceled = value;
        }

        [SerializeField]
        [Range(0,1)]
        float m_HapticHoverCancelIntensity;
        /// <summary>
        /// The Haptics intensity to play on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelDuration"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        public float hapticHoverCancelIntensity
        {
            get => m_HapticHoverCancelIntensity;
            set => m_HapticHoverCancelIntensity= value;
        }

        [SerializeField]
        float m_HapticHoverCancelDuration;
        /// <summary>
        /// The Haptics duration (in seconds) to play on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        public float hapticHoverCancelDuration
        {
            get => m_HapticHoverCancelDuration;
            set => m_HapticHoverCancelDuration = value;
        }

        XRBaseController m_Controller;
        /// <summary>
        /// The controller instance that is queried for input.
        /// </summary>
        public XRBaseController xrController
        {
            get => m_Controller;
            set => m_Controller = value;
        }

        /// <summary>
        /// (Read Only) A list of Interactables that this Interactor could possibly interact with this frame.
        /// </summary>
        /// <seealso cref="XRBaseInteractor.GetValidTargets"/>
        protected abstract List<XRBaseInteractable> validTargets { get; }

        readonly ActivateEventArgs m_ActivateEventArgs = new ActivateEventArgs();
        readonly DeactivateEventArgs m_DeactivateEventArgs = new DeactivateEventArgs();

        bool m_ToggleSelectActive;
        bool m_WaitingForSelectDeactivate;
        AudioSource m_EffectsAudioSource;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            // Setup interaction controller (for sending down selection state and input)
            m_Controller = GetComponent<XRBaseController>();
            if (m_Controller == null)
                Debug.LogWarning($"Could not find {nameof(XRBaseController)} component on {gameObject}.", this);

            // If we are toggling selection and have a starting object, start out holding it
            if (m_SelectActionTrigger == InputTriggerType.Toggle && startingSelectedInteractable != null)
                m_ToggleSelectActive = true;

            if (m_PlayAudioClipOnSelectEntered || m_PlayAudioClipOnSelectExited || m_PlayAudioClipOnSelectCanceled ||
                m_PlayAudioClipOnHoverEntered || m_PlayAudioClipOnHoverExited || m_PlayAudioClipOnHoverCanceled)
            {
                CreateEffectsAudioSource();
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            // Perform toggling of selection state
            // and activation of selected object on activate.
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (m_Controller == null)
                    return;

                if (m_SelectActionTrigger == InputTriggerType.Toggle ||
                    m_SelectActionTrigger == InputTriggerType.Sticky)
                {
                    if (m_Controller.selectInteractionState.activatedThisFrame)
                    {
                        if (m_ToggleSelectActive)
                            m_WaitingForSelectDeactivate = true;

                        if (m_ToggleSelectActive || validTargets.Count > 0)
                            m_ToggleSelectActive = !m_ToggleSelectActive;
                    }

                    if (m_Controller.selectInteractionState.deactivatedThisFrame)
                        m_WaitingForSelectDeactivate = false;
                }

                if (selectTarget != null && m_Controller.activateInteractionState.activatedThisFrame)
                {
                    m_ActivateEventArgs.interactor = this;
                    m_ActivateEventArgs.interactable = selectTarget;
                    selectTarget.OnActivated(m_ActivateEventArgs);
                }

                if (selectTarget != null && m_Controller.activateInteractionState.deactivatedThisFrame)
                {
                    m_DeactivateEventArgs.interactor = this;
                    m_DeactivateEventArgs.interactable = selectTarget;
                    selectTarget.OnDeactivated(m_DeactivateEventArgs);
                }
            }
        }

        /// <summary>
        /// Gets whether the selection state is active for this interactor.  This will check if the controller has a valid selection
        /// state or whether toggle selection is currently on and active.
        /// </summary>
        public override bool isSelectActive
        {
            get
            {
                if (!base.isSelectActive)
                    return false;

                if (isPerformingManualInteraction)
                    return true;

                switch (m_SelectActionTrigger)
                {
                    case InputTriggerType.State:
                        return m_Controller != null && m_Controller.selectInteractionState.active;

                    case InputTriggerType.StateChange:
                        return (m_Controller != null && m_Controller.selectInteractionState.activatedThisFrame) ||
                            (selectTarget != null && m_Controller != null && !m_Controller.selectInteractionState.deactivatedThisFrame);

                    case InputTriggerType.Toggle:
                        return m_ToggleSelectActive;

                    case InputTriggerType.Sticky:
                        return m_ToggleSelectActive || m_WaitingForSelectDeactivate;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Whether or not the UI Press controller input is considered pressed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if active. Otherwise, returns <see langword="false"/>.</returns>
        protected bool isUISelectActive => m_Controller != null && m_Controller.uiPressInteractionState.active;

        /// <inheritdoc />
        protected internal override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            HandleSelecting();

            if (m_PlayHapticsOnSelectEntered)
                SendHapticImpulse(m_HapticSelectEnterIntensity, m_HapticSelectEnterDuration);

            if (m_PlayAudioClipOnSelectEntered)
                PlayAudio(m_AudioClipForOnSelectEntered);
        }

        /// <inheritdoc />
        protected internal override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            HandleDeselecting();

            if (args.isCanceled)
            {
                if (m_PlayHapticsOnSelectCanceled)
                    SendHapticImpulse(m_HapticSelectCancelIntensity, m_HapticSelectCancelDuration);

                if (m_PlayAudioClipOnSelectCanceled)
                    PlayAudio(m_AudioClipForOnSelectCanceled);
            }
            else
            {
                if (m_PlayHapticsOnSelectExited)
                    SendHapticImpulse(m_HapticSelectExitIntensity, m_HapticSelectExitDuration);

                if (m_PlayAudioClipOnSelectExited)
                    PlayAudio(m_AudioClipForOnSelectExited);
            }
        }

        /// <inheritdoc />
        protected internal override void OnHoverEntering(HoverEnterEventArgs args)
        {
            base.OnHoverEntering(args);

            if (m_PlayHapticsOnHoverEntered)
                SendHapticImpulse(m_HapticHoverEnterIntensity, m_HapticHoverEnterDuration);

            if (m_PlayAudioClipOnHoverEntered)
                PlayAudio(m_AudioClipForOnHoverEntered);
        }

        /// <inheritdoc />
        protected internal override void OnHoverExiting(HoverExitEventArgs args)
        {
            base.OnHoverExiting(args);

            if (args.isCanceled)
            {
                if (m_PlayHapticsOnHoverCanceled)
                    SendHapticImpulse(m_HapticHoverCancelIntensity, m_HapticHoverCancelDuration);

                if (m_PlayAudioClipOnHoverCanceled)
                    PlayAudio(m_AudioClipForOnHoverCanceled);
            }
            else
            {
                if (m_PlayHapticsOnHoverExited)
                    SendHapticImpulse(m_HapticHoverExitIntensity, m_HapticHoverExitDuration);

                if (m_PlayAudioClipOnHoverExited)
                    PlayAudio(m_AudioClipForOnHoverExited);
            }
        }

        /// <summary>
        /// Play a haptic impulse on the controller if one is available.
        /// </summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="XRBaseController.SendHapticImpulse"/>
        public bool SendHapticImpulse(float amplitude, float duration)
        {
            return m_Controller != null && m_Controller.SendHapticImpulse(amplitude, duration);
        }

        /// <summary>
        /// Play an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="audioClip">The clip to play.</param>
        protected virtual void PlayAudio(AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            if (m_EffectsAudioSource == null)
                CreateEffectsAudioSource();

            m_EffectsAudioSource.PlayOneShot(audioClip);
        }

        void CreateEffectsAudioSource()
        {
            m_EffectsAudioSource = gameObject.AddComponent<AudioSource>();
            m_EffectsAudioSource.loop = false;
            m_EffectsAudioSource.playOnAwake = false;
        }

        /// <summary>
        /// Called automatically to handle entering select.
        /// </summary>
        /// <seealso cref="OnSelectEntering"/>
        void HandleSelecting()
        {
            m_ToggleSelectActive = true;
            m_WaitingForSelectDeactivate = false;

            if (m_HideControllerOnSelect && m_Controller != null)
                m_Controller.hideControllerModel = true;
        }

        /// <summary>
        /// Called automatically to handle exiting select.
        /// </summary>
        /// <seealso cref="OnSelectExiting"/>
        void HandleDeselecting()
        {
            // Reset toggle values when no longer selecting
            // (can happen by another Interactor taking the Interactable or through method calls).
            m_ToggleSelectActive = false;
            m_WaitingForSelectDeactivate = false;

            if (m_Controller != null)
                m_Controller.hideControllerModel = false;
        }
    }
}
