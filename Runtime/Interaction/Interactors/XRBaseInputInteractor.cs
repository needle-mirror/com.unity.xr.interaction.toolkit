using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Abstract base class from which all interactors that use inputs to drive interaction state derive from.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public abstract partial class XRBaseInputInteractor : XRBaseInteractor, IXRActivateInteractor
    {
        /// <summary>
        /// This defines the type of input that triggers an interaction.
        /// </summary>
        /// <seealso cref="selectActionTrigger"/>
        public enum InputTriggerType
        {
            /// <summary>
            /// Unity will consider the input active while the button is pressed.
            /// A user can hold the button before the interaction is possible
            /// and still trigger the interaction when it is possible.
            /// </summary>
            /// <remarks>
            /// When multiple interactors select an interactable at the same time and that interactable's
            /// <see cref="InteractableSelectMode"/> is set to <see cref="InteractableSelectMode.Single"/>, you may
            /// experience undesired behavior of selection repeatedly passing between the interactors and the select
            /// interaction events firing each frame. State Change is the recommended and default option.
            /// </remarks>
            /// <seealso cref="InteractionState.active"/>
            /// <seealso cref="InteractableSelectMode"/>
            State,

            /// <summary>
            /// Unity will consider the input active only on the frame the button is pressed,
            /// and if successful remain engaged until the input is released.
            /// A user must press the button while the interaction is possible to trigger the interaction.
            /// They will not trigger the interaction if they started pressing the button before the interaction was possible.
            /// </summary>
            /// <seealso cref="InteractionState.activatedThisFrame"/>
            StateChange,

            /// <summary>
            /// The interaction starts on the frame the input is pressed
            /// and remains engaged until the second time the input is pressed.
            /// </summary>
            Toggle,

            /// <summary>
            /// The interaction starts on the frame the input is pressed
            /// and remains engaged until the second time the input is released.
            /// </summary>
            Sticky,
        }

        /// <summary>
        /// Interpreted input from an input reader. Represents the logical state of an interaction input,
        /// such as the select input, which may not be the same as the physical state of the input.
        /// </summary>
        /// <seealso cref="InputTriggerType"/>
        public partial class LogicalInputState
        {
            /// <summary>
            /// Whether the logical input state is currently active.
            /// </summary>
            public bool active { get; private set; }

            InputTriggerType m_Mode;

            /// <summary>
            /// The type of input
            /// </summary>
            public InputTriggerType mode
            {
                get => m_Mode;
                set
                {
                    if (m_Mode != value)
                    {
                        m_Mode = value;
                        Refresh();
                    }
                }
            }

            /// <summary>
            /// Read whether the button is currently performed, which typically means whether the button is being pressed.
            /// This is typically true for multiple frames.
            /// </summary>
            /// <seealso cref="IXRInputButtonReader.ReadIsPerformed"/>
            public bool isPerformed { get; private set; }

            /// <summary>
            /// Read whether the button performed this frame, which typically means whether the button started being pressed during this frame.
            /// This is typically only true for one single frame.
            /// </summary>
            /// <seealso cref="IXRInputButtonReader.ReadWasPerformedThisFrame"/>
            public bool wasPerformedThisFrame { get; private set; }

            /// <summary>
            /// Read whether the button stopped performing this frame, which typically means whether the button stopped being pressed during this frame.
            /// This is typically only true for one single frame.
            /// </summary>
            public bool wasCompletedThisFrame { get; private set; }

            bool m_HasSelection;

            float m_TimeAtPerformed;
            float m_TimeAtCompleted;

            bool m_ToggleActive;
            bool m_ToggleDeactivatedThisFrame;
            bool m_WaitingForDeactivate;

            internal void UpdateInput(bool performed, bool performedThisFrame, bool completedThisFrame, bool hasSelection) =>
                UpdateInput(performed, performedThisFrame, completedThisFrame, hasSelection, Time.realtimeSinceStartup);

            void UpdateInput(bool performed, bool performedThisFrame, bool completedThisFrame, bool hasSelection, float realtime)
            {
                isPerformed = performed;
                wasPerformedThisFrame = performedThisFrame;
                wasCompletedThisFrame = completedThisFrame;
                m_HasSelection = hasSelection;

                if (wasPerformedThisFrame)
                    m_TimeAtPerformed = realtime;

                if (wasCompletedThisFrame)
                    m_TimeAtCompleted = realtime;

                m_ToggleDeactivatedThisFrame = false;
                if (mode == InputTriggerType.Toggle || mode == InputTriggerType.Sticky)
                {
                    if (m_ToggleActive && performedThisFrame)
                    {
                        m_ToggleActive = false;
                        m_ToggleDeactivatedThisFrame = true;
                        m_WaitingForDeactivate = true;
                    }

                    if (wasCompletedThisFrame)
                        m_WaitingForDeactivate = false;
                }

                Refresh();
            }

            internal void UpdateHasSelection(bool hasSelection)
            {
                if (m_HasSelection == hasSelection)
                    return;

                // Reset toggle values when no longer selecting
                // (can happen by another Interactor taking the Interactable or through method calls).
                m_HasSelection = hasSelection;
                m_ToggleActive = hasSelection;
                m_WaitingForDeactivate = false;

                Refresh();
            }

            void Refresh()
            {
                switch (mode)
                {
                    case InputTriggerType.State:
                        active = isPerformed;
                        break;

                    case InputTriggerType.StateChange:
                        active = wasPerformedThisFrame || (m_HasSelection && !wasCompletedThisFrame);
                        break;

                    case InputTriggerType.Toggle:
                        active = m_ToggleActive || (wasPerformedThisFrame && !m_ToggleDeactivatedThisFrame);
                        break;

                    case InputTriggerType.Sticky:
                        active = m_ToggleActive || m_WaitingForDeactivate || wasPerformedThisFrame;
                        break;

                    default:
                        Assert.IsTrue(false, $"Unhandled {nameof(InputTriggerType)}={mode}");
                        break;
                }
            }
        }

        [SerializeField]
        XRInputButtonReader m_SelectInput = new XRInputButtonReader("Select");

        /// <summary>
        /// Input to use for selecting an interactable.
        /// </summary>
        public XRInputButtonReader selectInput
        {
            get => m_SelectInput;
            set => SetInputProperty(ref m_SelectInput, value);
        }

        [SerializeField]
        XRInputButtonReader m_ActivateInput = new XRInputButtonReader("Activate");

        /// <summary>
        /// Input to use for activating an interactable.
        /// This can be used to trigger a secondary action on an interactable object,
        /// such as pulling a trigger on a ball launcher after picking it up.
        /// </summary>
        public XRInputButtonReader activateInput
        {
            get => m_ActivateInput;
            set => SetInputProperty(ref m_ActivateInput, value);
        }

        [SerializeField]
        InputTriggerType m_SelectActionTrigger = InputTriggerType.StateChange;

        /// <summary>
        /// Choose how Unity interprets the select input.
        /// Controls between different input styles for determining if this interactor can select,
        /// such as whether the button is currently held or whether the button toggles select upon being pressed.
        /// </summary>
        /// <seealso cref="InputTriggerType"/>
        /// <seealso cref="logicalSelectState"/>
        public InputTriggerType selectActionTrigger
        {
            get => m_SelectActionTrigger;
            set => m_SelectActionTrigger = value;
        }

        [SerializeField]
        bool m_AllowHoveredActivate;

        /// <summary>
        /// Controls whether to send activate and deactivate events to interactables
        /// that this interactor is hovered over but not selected when there is no current selection.
        /// By default, the interactor will only send activate and deactivate events to interactables that it's selected.
        /// </summary>
        /// <seealso cref="allowActivate"/>
        /// <seealso cref="GetActivateTargets"/>
        public bool allowHoveredActivate
        {
            get => m_AllowHoveredActivate;
            set => m_AllowHoveredActivate = value;
        }

        [SerializeField]
        TargetPriorityMode m_TargetPriorityMode;

        /// <inheritdoc />
        public override TargetPriorityMode targetPriorityMode
        {
            get => m_TargetPriorityMode;
            set => m_TargetPriorityMode = value;
        }

        bool m_AllowActivate = true;

        /// <summary>
        /// Defines whether this interactor allows sending activate and deactivate events.
        /// </summary>
        /// <seealso cref="allowHoveredActivate"/>
        /// <seealso cref="shouldActivate"/>
        /// <seealso cref="shouldDeactivate"/>
        public bool allowActivate
        {
            get => m_AllowActivate;
            set => m_AllowActivate = value;
        }

        /// <inheritdoc />
        public override bool isSelectActive
        {
            get
            {
                if (!base.isSelectActive)
                    return false;

                if (isPerformingManualInteraction)
                    return true;

                m_LogicalSelectState.mode = m_SelectActionTrigger;
                return m_LogicalSelectState.active;
            }
        }

        /// <inheritdoc />
        public virtual bool shouldActivate
        {
            get
            {
                if (m_AllowActivate && (hasSelection || m_AllowHoveredActivate && hasHover))
                {
                    return m_LogicalActivateState.wasPerformedThisFrame;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public virtual bool shouldDeactivate
        {
            get
            {
                if (m_AllowActivate && (hasSelection || m_AllowHoveredActivate && hasHover))
                {
                    return m_LogicalActivateState.wasCompletedThisFrame;
                }

                return false;
            }
        }

        /// <summary>
        /// The logical state of the select input.
        /// </summary>
        /// <seealso cref="selectInput"/>
        public LogicalInputState logicalSelectState => m_LogicalSelectState;

        /// <summary>
        /// The logical state of the activate input.
        /// </summary>
        /// <seealso cref="activateInput"/>
        public LogicalInputState logicalActivateState => m_LogicalActivateState;

        /// <summary>
        /// The list of button input readers used by this interactor. This interactor will automatically enable or disable direct actions
        /// if that mode is used during <see cref="OnEnable"/> and <see cref="OnDisable"/>.
        /// </summary>
        /// <seealso cref="XRInputButtonReader.EnableDirectActionIfModeUsed"/>
        /// <seealso cref="XRInputButtonReader.DisableDirectActionIfModeUsed"/>
        protected List<XRInputButtonReader> buttonReaders { get; } = new List<XRInputButtonReader>();

        /// <summary>
        /// The list of value input readers used by this interactor. This interactor will automatically enable or disable direct actions
        /// if that mode is used during <see cref="OnEnable"/> and <see cref="OnDisable"/>.
        /// </summary>
        /// <seealso cref="XRInputButtonReader.EnableDirectActionIfModeUsed"/>
        /// <seealso cref="XRInputButtonReader.DisableDirectActionIfModeUsed"/>
        protected List<XRInputValueReader> valueReaders { get; } = new List<XRInputValueReader>();

        readonly LinkedPool<ActivateEventArgs> m_ActivateEventArgs = new LinkedPool<ActivateEventArgs>(() => new ActivateEventArgs(), collectionCheck: false);
        readonly LinkedPool<DeactivateEventArgs> m_DeactivateEventArgs = new LinkedPool<DeactivateEventArgs>(() => new DeactivateEventArgs(), collectionCheck: false);

        static readonly List<IXRActivateInteractable> s_ActivateTargets = new List<IXRActivateInteractable>();

        readonly LogicalInputState m_LogicalSelectState = new LogicalInputState();
        readonly LogicalInputState m_LogicalActivateState = new LogicalInputState();

        AudioSource m_AudioSource;
        HapticImpulsePlayer m_HapticImpulsePlayer;

        /// <inheritdoc />
        protected override void Awake()
        {
            targetsForSelection = new List<IXRSelectInteractable>();

            base.Awake();

            buttonReaders.Add(m_SelectInput);
            buttonReaders.Add(m_ActivateInput);
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonReaders.ForEach(reader => reader?.EnableDirectActionIfModeUsed());
            valueReaders.ForEach(reader => reader?.EnableDirectActionIfModeUsed());
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            buttonReaders.ForEach(reader => reader?.DisableDirectActionIfModeUsed());
            valueReaders.ForEach(reader => reader?.DisableDirectActionIfModeUsed());
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                m_LogicalSelectState.UpdateInput(m_SelectInput.ReadIsPerformed(), m_SelectInput.ReadWasPerformedThisFrame(), m_SelectInput.ReadWasCompletedThisFrame(), hasSelection);
                m_LogicalActivateState.UpdateInput(m_ActivateInput.ReadIsPerformed(), m_ActivateInput.ReadWasPerformedThisFrame(), m_ActivateInput.ReadWasCompletedThisFrame(), hasSelection);
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // Send activate/deactivate events as necessary.
                if (m_AllowActivate)
                {
                    var sendActivate = shouldActivate;
                    var sendDeactivate = shouldDeactivate;
                    if (sendActivate || sendDeactivate)
                    {
                        GetActivateTargets(s_ActivateTargets);

                        if (sendActivate)
                            SendActivateEvent(s_ActivateTargets);

                        // Note that this makes it possible for an interactable to receive an OnDeactivated event
                        // but not the earlier OnActivated event if it was selected afterward.
                        if (sendDeactivate)
                            SendDeactivateEvent(s_ActivateTargets);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method for setting an input property.
        /// </summary>
        /// <param name="property">The <see langword="ref"/> to the field.</param>
        /// <param name="value">The new value being set.</param>
        /// <remarks>
        /// If the application is playing, this method will also enable or disable directly embedded input actions
        /// serialized by the input if that mode is used. It will also add or remove the input from the list of button inputs
        /// to automatically manage enabling and disabling direct actions with this behavior.
        /// </remarks>
        /// <seealso cref="buttonReaders"/>
        protected void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value)
        {
            XRInputReaderUtility.SetInputProperty(ref property, value, this, buttonReaders);
        }

        /// <summary>
        /// Helper method for setting an input property.
        /// </summary>
        /// <param name="property">The <see langword="ref"/> to the field.</param>
        /// <param name="value">The new value being set.</param>
        /// <typeparam name="TValue">Type of the value read by the property, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
        /// <remarks>
        /// If the application is playing, this method will also enable or disable directly embedded input actions
        /// serialized by the input if that mode is used. It will also add or remove the input from the list of value inputs
        /// to automatically manage enabling and disabling direct actions with this behavior.
        /// </remarks>
        /// <seealso cref="valueReaders"/>
        protected void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value) where TValue : struct
        {
            XRInputReaderUtility.SetInputProperty(ref property, value, this, valueReaders);
        }

        void SendActivateEvent(List<IXRActivateInteractable> targets)
        {
            foreach (var interactable in targets)
            {
                if (interactable == null || interactable as Object == null)
                    continue;

                using (m_ActivateEventArgs.Get(out var args))
                {
                    args.interactorObject = this;
                    args.interactableObject = interactable;
                    interactable.OnActivated(args);
                }
            }
        }

        void SendDeactivateEvent(List<IXRActivateInteractable> targets)
        {
            foreach (var interactable in targets)
            {
                if (interactable == null || interactable as Object == null)
                    continue;

                using (m_DeactivateEventArgs.Get(out var args))
                {
                    args.interactorObject = this;
                    args.interactableObject = interactable;
                    interactable.OnDeactivated(args);
                }
            }
        }

        /// <inheritdoc />
        public virtual void GetActivateTargets(List<IXRActivateInteractable> targets)
        {
            targets.Clear();
            if (hasSelection)
            {
                foreach (var interactable in interactablesSelected)
                {
                    if (interactable is IXRActivateInteractable activateInteractable)
                    {
                        targets.Add(activateInteractable);
                    }
                }
            }
            else if (m_AllowHoveredActivate && hasHover)
            {
                foreach (var interactable in interactablesHovered)
                {
                    if (interactable is IXRActivateInteractable activateInteractable)
                    {
                        targets.Add(activateInteractable);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            m_LogicalSelectState.UpdateHasSelection(true);
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            // Wait until all selections have been exited in case multiple selections are allowed.
            if (hasSelection)
                return;

            m_LogicalSelectState.UpdateHasSelection(false);
        }

        /// <summary>
        /// Play a haptic impulse on the controller if one is available.
        /// </summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        public bool SendHapticImpulse(float amplitude, float duration)
        {
            if (m_HapticImpulsePlayer == null)
                GetOrCreateHapticImpulsePlayer();

            return m_HapticImpulsePlayer.SendHapticImpulse(amplitude, duration);
        }

        /// <summary>
        /// Play an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="audioClip">The clip to play.</param>
        protected virtual void PlayAudio(AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            if (m_AudioSource == null)
                GetOrCreateAudioSource();

            m_AudioSource.PlayOneShot(audioClip);
        }

        void GetOrCreateAudioSource()
        {
            if (!TryGetComponent(out m_AudioSource))
                m_AudioSource = gameObject.AddComponent<AudioSource>();

            m_AudioSource.loop = false;
            m_AudioSource.playOnAwake = false;
        }

        void GetOrCreateHapticImpulsePlayer()
        {
            m_HapticImpulsePlayer = HapticImpulsePlayer.GetOrCreateInHierarchy(gameObject);
        }
    }
}
