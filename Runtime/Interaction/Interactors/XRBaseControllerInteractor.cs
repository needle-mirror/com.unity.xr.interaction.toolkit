using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Abstract base class from which all interactors that are controller-driven derive.
    /// This class hooks into the interaction system (via <see cref="XRInteractionManager"/>) and provides base virtual methods for handling
    /// hover and selection. Additionally, this class provides functionality for checking the controller's selection status
    /// and hiding the controller on selection.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [Obsolete(k_XRBaseControllerInteractorDeprecated, true)]
    public abstract class XRBaseControllerInteractor : XRBaseInteractor, IXRActivateInteractor
    {
        const string k_XRBaseControllerInteractorDeprecated = "XRBaseControllerInteractor has been deprecated in version 3.0.0. It has been renamed to XRBaseInputInteractor. (UnityUpgradable) -> UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor";

        /// <summary>
        /// This defines the type of input that triggers an interaction.
        /// </summary>
        /// <seealso cref="selectActionTrigger"/>
        [Obsolete("XRBaseControllerInteractor.InputTriggerType has been deprecated in version 3.0.0. It has been moved to XRBaseInputInteractor.InputTriggerType.", true)]
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
        /// Choose how Unity interprets the select input action from the controller.
        /// Controls between different input styles for determining if this Interactor can select,
        /// such as whether the button is currently pressed or just toggles the active state.
        /// </summary>
        /// <seealso cref="InputTriggerType"/>
        /// <seealso cref="isSelectActive"/>
        public InputTriggerType selectActionTrigger
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether this Interactor should hide the controller model on selection.
        /// </summary>
        /// <seealso cref="XRBaseController.hideControllerModel"/>
        public bool hideControllerOnSelect
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether to send activate and deactivate events to interactables
        /// that this interactor is hovered over but not selected when there is no current selection.
        /// By default, the interactor will only send activate and deactivate events to interactables that it's selected.
        /// </summary>
        /// <seealso cref="allowActivate"/>
        /// <seealso cref="GetActivateTargets"/>
        public bool allowHoveredActivate
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        public override TargetPriorityMode targetPriorityMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Select Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectEntered"/>
        public bool playAudioClipOnSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
        public AudioClip audioClipForOnSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Select Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectExited"/>
        public bool playAudioClipOnSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
        public AudioClip audioClipForOnSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Select Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectCanceled"/>
        public bool playAudioClipOnSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectCanceled"/>
        public AudioClip audioClipForOnSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Hover Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverEntered"/>
        public bool playAudioClipOnHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        public AudioClip audioClipForOnHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Hover Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverExited"/>
        public bool playAudioClipOnHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        public AudioClip audioClipForOnHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays an <see cref="AudioClip"/> on Hover Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverCanceled"/>
        public bool playAudioClipOnHoverCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="AudioClip"/> Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        public AudioClip audioClipForOnHoverCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity allows playing an <see cref="AudioClip"/> from a Hover event if the Hovered Interactable is currently Selected by this Interactor.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        public bool allowHoverAudioWhileSelecting
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="hapticSelectEnterDuration"/>
        public bool playHapticsOnSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterDuration"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        public float hapticSelectEnterIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        public float hapticSelectEnterDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="hapticSelectExitDuration"/>
        public bool playHapticsOnSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitDuration"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        public float hapticSelectExitIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        public float hapticSelectExitDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="hapticSelectCancelDuration"/>
        public bool playHapticsOnSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelDuration"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        public float hapticSelectCancelIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        public float hapticSelectCancelDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="hapticHoverEnterDuration"/>
        public bool playHapticsOnHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterDuration"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        public float hapticHoverEnterIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        public float hapticHoverEnterDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="hapticHoverExitDuration"/>
        public bool playHapticsOnHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitDuration"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        public float hapticHoverExitIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        public float hapticHoverExitDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity plays haptics on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="hapticHoverCancelDuration"/>
        public bool playHapticsOnHoverCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics intensity Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelDuration"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        public float hapticHoverCancelIntensity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        public float hapticHoverCancelDuration
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether Unity allows playing haptics from a Hover event if the Hovered Interactable is currently Selected by this Interactor.
        /// </summary>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        public bool allowHoverHapticsWhileSelecting
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Defines whether this interactor allows sending activate and deactivate events.
        /// </summary>
        /// <seealso cref="allowHoveredActivate"/>
        /// <seealso cref="shouldActivate"/>
        /// <seealso cref="shouldDeactivate"/>
        public bool allowActivate
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The controller instance that is queried for input.
        /// </summary>
        public XRBaseController xrController
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        public override bool isSelectActive => default;

        /// <summary>
        /// (Read Only) Whether or not Unity considers the UI Press controller input pressed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if active. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool isUISelectActive => default;

        /// <summary>
        /// (Read Only) The current scroll value Unity would apply to the UI.
        /// </summary>
        /// <returns>Returns a Vector2 with scroll strength for each axis. </returns>
        protected Vector2 uiScrollValue => default;

        /// <inheritdoc />
        public virtual bool shouldActivate => default;

        /// <inheritdoc />
        public virtual bool shouldDeactivate => default;

        /// <inheritdoc />
        public virtual void GetActivateTargets(List<IXRActivateInteractable> targets)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnHoverExiting(HoverExitEventArgs args)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
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
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }

        /// <summary>
        /// Play an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="audioClip">The clip to play.</param>
        protected virtual void PlayAudio(AudioClip audioClip)
        {
            throw new NotSupportedException(k_XRBaseControllerInteractorDeprecated);
        }
    }
}
