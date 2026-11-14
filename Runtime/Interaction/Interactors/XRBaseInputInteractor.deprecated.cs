using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    public abstract partial class XRBaseInputInteractor
    {
        const string k_CreateEffectsAudioSourceDeprecated = "CreateEffectsAudioSource has been deprecated in version 3.0.0.";
        const string k_CanPlayHoverAudioDeprecated = "CanPlayHoverAudio has been deprecated in version 3.0.0.";
        const string k_CanPlayHoverHapticsDeprecated = "CanPlayHoverHaptics has been deprecated in version 3.0.0.";
        const string k_HandleSelectingDeprecated = "HandleSelecting has been deprecated in version 3.0.0.";
        const string k_HandleDeselectingDeprecated = "HandleDeselecting has been deprecated in version 3.0.0.";

        /// <summary>
        /// Controls whether input can be obtained through the deprecated legacy method for backwards compatibility.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        /// <seealso cref="inputCompatibilityMode"/>
        [Obsolete("InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.", true)]
        public enum InputCompatibilityMode
        {
            /// <summary>
            /// Automatically determine whether to use the deprecated legacy input based on whether the interactor has an XR Controller component.
            /// If the <see cref="xrController"/> is set, then the deprecated legacy input will be used.
            /// </summary>
            Automatic,

            /// <summary>
            /// Force the interactor to read inputs from the deprecated legacy XR Controller component.
            /// This is how the interactor used to read inputs prior to version 3.0.0.
            /// </summary>
            ForceDeprecatedInput,

            /// <summary>
            /// Force the interactor to read inputs from the input reader properties on this interactor.
            /// This is how the interactor is recommended to read inputs starting in version 3.0.0.
            /// </summary>
            ForceInputReaders,
        }

        public partial class LogicalInputState
        {
            /// <summary>
            /// (Deprecated) Read whether the button stopped performing this frame, which typically means whether the button stopped being pressed during this frame.
            /// This is typically only true for one single frame.
            /// </summary>
            [Obsolete("wasUnperformedThisFrame has been deprecated in version 3.0.0-pre.2. It has been renamed to wasCompletedThisFrame. (UnityUpgradable) -> wasCompletedThisFrame", true)]
            public bool wasUnperformedThisFrame => default;
        }

        /// <summary>
        /// Controls whether this Interactor should hide the controller model on selection.
        /// </summary>
        /// <seealso cref="XRBaseController.hideControllerModel"/>
        [Obsolete("hideControllerOnSelect has been deprecated in version 3.0.0.", true)]
        public bool hideControllerOnSelect
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether input is obtained through the deprecated legacy method where the XR Controller component is used.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        /// <seealso cref="InputCompatibilityMode"/>
        /// <seealso cref="forceDeprecatedInput"/>
        [Obsolete("inputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.", true)]
        public InputCompatibilityMode inputCompatibilityMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether this interactor is being forced to use the deprecated input path where the input values are obtained through the <see cref="xrController"/>.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        [Obsolete("forceDeprecatedInput introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.", true)]
        public bool forceDeprecatedInput
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The controller instance that is queried for input.
        /// </summary>
        [Obsolete("xrController has been deprecated in version 3.0.0.", true)]
        public XRBaseController xrController
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) (Read Only) Whether or not Unity considers the UI Press controller input pressed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if active. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use a serialized XRInputButtonReader to read button input instead. Some derived interactors have a uiPressInput property that can be used instead.", true)]
        protected virtual bool isUISelectActive => default;

        /// <summary>
        /// (Deprecated) (Read Only) The current scroll value Unity would apply to the UI.
        /// </summary>
        /// <returns>Returns a Vector2 with scroll strength for each axis. </returns>
        [Obsolete("uiScrollValue has been deprecated in version 3.0.0. Use a serialized XRInputValueReader<Vector2> to read scroll input instead. Some derived interactors have a uiScrollInput property that can be used instead.", true)]
        protected Vector2 uiScrollValue => default;

        #region Audio Events

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectEntered"/>
        [Obsolete("playAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectEntered instead.", true)]
        public bool playAudioClipOnSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
        [Obsolete("audioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectEnteredClip instead.", true)]
        public AudioClip audioClipForOnSelectEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectExited"/>
        [Obsolete("playAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectExited instead.", true)]
        public bool playAudioClipOnSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
        [Obsolete("audioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectExitedClip instead.", true)]
        public AudioClip audioClipForOnSelectExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectCanceled"/>
        [Obsolete("playAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectCanceled instead.", true)]
        public bool playAudioClipOnSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectCanceled"/>
        [Obsolete("audioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectCanceledClip instead.", true)]
        public AudioClip audioClipForOnSelectCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverEntered"/>
        [Obsolete("playAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverEntered instead.", true)]
        public bool playAudioClipOnHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        [Obsolete("audioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverEnteredClip instead.", true)]
        public AudioClip audioClipForOnHoverEntered
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverExited"/>
        [Obsolete("playAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverExited instead.", true)]
        public bool playAudioClipOnHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        [Obsolete("audioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverExitedClip instead.", true)]
        public AudioClip audioClipForOnHoverExited
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverCanceled"/>
        [Obsolete("playAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverCanceled instead.", true)]
        public bool playAudioClipOnHoverCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        [Obsolete("audioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverCanceledClip instead.", true)]
        public AudioClip audioClipForOnHoverCanceled
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Controls whether Unity allows playing an <see cref="AudioClip"/> from a Hover event if the Hovered Interactable is currently Selected by this Interactor.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        [Obsolete("allowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedback.allowHoverAudioWhileSelecting instead.", true)]
        public bool allowHoverAudioWhileSelecting
        {
            get => default;
            set => _ = value;
        }

        #endregion

        #region Haptic Events

        /// <summary>
        /// Controls whether Unity plays haptics on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="hapticSelectEnterDuration"/>
        [Obsolete("playHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectEntered instead.", true)]
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
        [Obsolete("hapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.amplitude instead.", true)]
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
        [Obsolete("hapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.duration instead.", true)]
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
        [Obsolete("playHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectExited instead.", true)]
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
        [Obsolete("hapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.amplitude instead.", true)]
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
        [Obsolete("hapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.duration instead.", true)]
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
        [Obsolete("playHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectCanceled instead.", true)]
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
        [Obsolete("hapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.amplitude instead.", true)]
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
        [Obsolete("hapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.duration instead.", true)]
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
        [Obsolete("playHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverEntered instead.", true)]
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
        [Obsolete("hapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.amplitude instead.", true)]
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
        [Obsolete("hapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.duration instead.", true)]
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
        [Obsolete("playHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverExited instead.", true)]
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
        [Obsolete("hapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.amplitude instead.", true)]
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
        [Obsolete("hapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.duration instead.", true)]
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
        [Obsolete("playHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverCanceled instead.", true)]
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
        [Obsolete("hapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.amplitude instead.", true)]
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
        [Obsolete("hapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.duration instead.", true)]
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
        [Obsolete("allowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedback.allowHoverHapticsWhileSelecting instead.", true)]
        public bool allowHoverHapticsWhileSelecting
        {
            get => default;
            set => _ = value;
        }

        #endregion

        #region API Updater Configuration Validation false positive failure suppression

        [Obsolete(k_CreateEffectsAudioSourceDeprecated, true)]
        void CreateEffectsAudioSource()
        {
            throw new NotSupportedException(k_CreateEffectsAudioSourceDeprecated);
        }

        [Obsolete(k_CanPlayHoverAudioDeprecated, true)]
        bool CanPlayHoverAudio(IXRHoverInteractable hoveredInteractable)
        {
            throw new NotSupportedException(k_CanPlayHoverAudioDeprecated);
        }

        [Obsolete(k_CanPlayHoverHapticsDeprecated, true)]
        bool CanPlayHoverHaptics(IXRHoverInteractable hoveredInteractable)
        {
            throw new NotSupportedException(k_CanPlayHoverHapticsDeprecated);
        }

        [Obsolete(k_HandleSelectingDeprecated, true)]
        void HandleSelecting()
        {
            throw new NotSupportedException(k_HandleSelectingDeprecated);
        }

        [Obsolete(k_HandleDeselectingDeprecated, true)]
        void HandleDeselecting()
        {
            throw new NotSupportedException(k_HandleDeselectingDeprecated);
        }

        // ReSharper disable once RedundantOverriddenMember -- Method needed for API Updater Configuration Validation false positive
        /// <inheritdoc />
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            base.OnHoverEntering(args);
        }

        // ReSharper disable once RedundantOverriddenMember -- Method needed for API Updater Configuration Validation false positive
        /// <inheritdoc />
        protected override void OnHoverExiting(HoverExitEventArgs args)
        {
            base.OnHoverExiting(args);
        }

        // ReSharper disable UnusedMember.Local -- Method needed for API Updater Configuration Validation false positive
        static ActivateEventArgs CreateActivateEventArgs() => new ActivateEventArgs();
        static DeactivateEventArgs CreateDeactivateEventArgs() => new DeactivateEventArgs();
        // ReSharper restore UnusedMember.Local

        #endregion
    }
}
