using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    public partial class XRBaseInputInteractorEditor
    {
        const string k_DrawAudioEventsDeprecated = "DrawAudioEvents has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor instead.";
        const string k_DrawAudioEventsNestedDeprecated = "DrawAudioEventsNested has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor instead.";
        const string k_DrawHapticEventsDeprecated = "DrawHapticEvents has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor instead.";
        const string k_DrawHapticEventsNestedDeprecated = "DrawHapticEventsNested has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor instead.";
        const string k_DrawDeprecatedControllerConfigurationDeprecated = "DrawDeprecatedControllerConfiguration has been deprecated in version 3.0.0.";
        const string k_VerifyControllerPresentDeprecated = "VerifyControllerPresent has been deprecated in version 3.0.0.";

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.inputCompatibilityMode"/>.</summary>
        [Obsolete("m_InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.", true)]
        protected SerializedProperty m_InputCompatibilityMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hideControllerOnSelect"/>.</summary>
        [Obsolete("m_HideControllerOnSelect has been deprecated in version 3.0.0.", true)]
        protected SerializedProperty m_HideControllerOnSelect;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectEntered"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectEntered instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectEntered"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectEnteredClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectExited"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectExited instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectExited"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectExitedClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectCanceled"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectCanceled instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectCanceled"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectCanceledClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverEntered"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverEntered instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverEntered"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverEnteredClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverExited"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverExited instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverExited"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverExitedClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverCanceled"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverCanceled instead.", true)]
        protected SerializedProperty m_PlayAudioClipOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverCanceled"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverCanceledClip instead.", true)]
        protected SerializedProperty m_AudioClipForOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.allowHoverAudioWhileSelecting"/>.</summary>
        [Obsolete("m_AllowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_AllowHoverAudioWhileSelecting instead.", true)]
        protected SerializedProperty m_AllowHoverAudioWhileSelecting;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectEntered"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectEntered instead.", true)]
        protected SerializedProperty m_PlayHapticsOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectEnterIntensity"/>.</summary>
        [Obsolete("m_HapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectEnteredDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticSelectEnterIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectEnterDuration"/>.</summary>
        [Obsolete("m_HapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectEnteredDataDuration instead.", true)]
        protected SerializedProperty m_HapticSelectEnterDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverEntered"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverEntered instead.", true)]
        protected SerializedProperty m_PlayHapticsOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverEnterIntensity"/>.</summary>
        [Obsolete("m_HapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverEnteredDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticHoverEnterIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverEnterDuration"/>.</summary>
        [Obsolete("m_HapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverEnteredDataDuration instead.", true)]
        protected SerializedProperty m_HapticHoverEnterDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectExited"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectExited instead.", true)]
        protected SerializedProperty m_PlayHapticsOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectExitIntensity"/>.</summary>
        [Obsolete("m_HapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectExitedDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticSelectExitIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectExitDuration"/>.</summary>
        [Obsolete("m_HapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectExitedDataDuration instead.", true)]
        protected SerializedProperty m_HapticSelectExitDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectCanceled"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectCanceled instead.", true)]
        protected SerializedProperty m_PlayHapticsOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectCancelIntensity"/>.</summary>
        [Obsolete("m_HapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectCanceledDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticSelectCancelIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectCancelDuration"/>.</summary>
        [Obsolete("m_HapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectCanceledDataDuration instead.", true)]
        protected SerializedProperty m_HapticSelectCancelDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverExited"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverExited instead.", true)]
        protected SerializedProperty m_PlayHapticsOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverExitIntensity"/>.</summary>
        [Obsolete("m_HapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverExitedDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticHoverExitIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverExitDuration"/>.</summary>
        [Obsolete("m_HapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverExitedDataDuration instead.", true)]
        protected SerializedProperty m_HapticHoverExitDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverCanceled"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverCanceled instead.", true)]
        protected SerializedProperty m_PlayHapticsOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverCancelIntensity"/>.</summary>
        [Obsolete("m_HapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverCanceledDataAmplitude instead.", true)]
        protected SerializedProperty m_HapticHoverCancelIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverCancelDuration"/>.</summary>
        [Obsolete("m_HapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverCanceledDataDuration instead.", true)]
        protected SerializedProperty m_HapticHoverCancelDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.allowHoverHapticsWhileSelecting"/>.</summary>
        [Obsolete("m_AllowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_AllowHoverHapticsWhileSelecting instead.", true)]
        protected SerializedProperty m_AllowHoverHapticsWhileSelecting;

        protected static partial class BaseInputContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.inputCompatibilityMode"/>.</summary>
            [Obsolete("inputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.", true)]
            public static readonly GUIContent inputCompatibilityMode = EditorGUIUtility.TrTextContent("Input Compatibility Mode", "Controls whether input is obtained through the deprecated legacy method where the XR Controller component is used. This is only used for backwards compatibility and will be eventually removed in a future version.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hideControllerOnSelect"/>.</summary>
            [Obsolete("hideControllerOnSelect has been deprecated in version 3.0.0.", true)]
            public static readonly GUIContent hideControllerOnSelect = EditorGUIUtility.TrTextContent("Hide Controller On Select", "(Deprecated) Hide the controller model on select. Requires an XR Controller component.");

            /// <summary>The help box message when <see cref="XRBaseController"/> is missing.</summary>
            [Obsolete("missingRequiredController has been deprecated in version 3.0.0.", true)]
            public static readonly string missingRequiredController = "This component requires the GameObject or a parent GameObject to have an XR Controller component. Add one to ensure this component can respond to user input.";

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectEntered"/>.</summary>
            [Obsolete("playAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectEntered instead.", true)]
            public static readonly GUIContent playAudioClipOnSelectEntered = EditorGUIUtility.TrTextContent("On Select Entered", "Play an audio clip when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectEntered"/>.</summary>
            [Obsolete("audioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectEnteredClip instead.", true)]
            public static readonly GUIContent audioClipForOnSelectEntered = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectExited"/>.</summary>
            [Obsolete("playAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectExited instead.", true)]
            public static readonly GUIContent playAudioClipOnSelectExited = EditorGUIUtility.TrTextContent("On Select Exited", "Play an audio clip when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectExited"/>.</summary>
            [Obsolete("audioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectExitedClip instead.", true)]
            public static readonly GUIContent audioClipForOnSelectExited = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectCanceled"/>.</summary>
            [Obsolete("playAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectCanceled instead.", true)]
            public static readonly GUIContent playAudioClipOnSelectCanceled = EditorGUIUtility.TrTextContent("On Select Canceled", "Play an audio clip when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectCanceled"/>.</summary>
            [Obsolete("audioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectCanceledClip instead.", true)]
            public static readonly GUIContent audioClipForOnSelectCanceled = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverEntered"/>.</summary>
            [Obsolete("playAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverEntered instead.", true)]
            public static readonly GUIContent playAudioClipOnHoverEntered = EditorGUIUtility.TrTextContent("On Hover Entered", "Play an audio clip when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverEntered"/>.</summary>
            [Obsolete("audioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverEnteredClip instead.", true)]
            public static readonly GUIContent audioClipForOnHoverEntered = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverExited"/>.</summary>
            [Obsolete("playAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverExited instead.", true)]
            public static readonly GUIContent playAudioClipOnHoverExited = EditorGUIUtility.TrTextContent("On Hover Exited", "Play an audio clip when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverExited"/>.</summary>
            [Obsolete("audioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverExitedClip instead.", true)]
            public static readonly GUIContent audioClipForOnHoverExited = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverCanceled"/>.</summary>
            [Obsolete("playAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverCanceled instead.", true)]
            public static readonly GUIContent playAudioClipOnHoverCanceled = EditorGUIUtility.TrTextContent("On Hover Canceled", "Play an audio clip when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverCanceled"/>.</summary>
            [Obsolete("audioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverCanceledClip instead.", true)]
            public static readonly GUIContent audioClipForOnHoverCanceled = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverAudioWhileSelecting"/>.</summary>
            [Obsolete("allowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.allowHoverAudioWhileSelecting instead.", true)]
            public static readonly GUIContent allowHoverAudioWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Audio While Selecting", "Allow playing audio from Hover events if the Hovered Interactable is currently Selected by this Interactor.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectEntered"/>.</summary>
            [Obsolete("playHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectEntered instead.", true)]
            public static readonly GUIContent playHapticsOnSelectEntered = EditorGUIUtility.TrTextContent("On Select Entered", "Play haptics when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterIntensity"/>.</summary>
            [Obsolete("hapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticSelectEnterIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterDuration"/>.</summary>
            [Obsolete("hapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticSelectEnterDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverEntered"/>.</summary>
            [Obsolete("playHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverEntered instead.", true)]
            public static readonly GUIContent playHapticsOnHoverEntered = EditorGUIUtility.TrTextContent("On Hover Entered", "Play haptics when the Hover State is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterIntensity"/>.</summary>
            [Obsolete("hapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticHoverEnterIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterDuration"/>.</summary>
            [Obsolete("hapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticHoverEnterDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectExited"/>.</summary>
            [Obsolete("playHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectExited instead.", true)]
            public static readonly GUIContent playHapticsOnSelectExited = EditorGUIUtility.TrTextContent("On Select Exited", "Play haptics when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitIntensity"/>.</summary>
            [Obsolete("hapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticSelectExitIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitDuration"/>.</summary>
            [Obsolete("hapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticSelectExitDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectCanceled"/>.</summary>
            [Obsolete("playHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectCanceled instead.", true)]
            public static readonly GUIContent playHapticsOnSelectCanceled = EditorGUIUtility.TrTextContent("On Select Canceled", "Play haptics when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelIntensity"/>.</summary>
            [Obsolete("hapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticSelectCancelIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelDuration"/>.</summary>
            [Obsolete("hapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticSelectCancelDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverExited"/>.</summary>
            [Obsolete("playHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverExited instead.", true)]
            public static readonly GUIContent playHapticsOnHoverExited = EditorGUIUtility.TrTextContent("On Hover Exited", "Play haptics when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitIntensity"/>.</summary>
            [Obsolete("hapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticHoverExitIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitDuration"/>.</summary>
            [Obsolete("hapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticHoverExitDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverCanceled"/>.</summary>
            [Obsolete("playHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverCanceled instead.", true)]
            public static readonly GUIContent playHapticsOnHoverCanceled = EditorGUIUtility.TrTextContent("On Hover Canceled", "Play haptics when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelIntensity"/>.</summary>
            [Obsolete("hapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.", true)]
            public static readonly GUIContent hapticHoverCancelIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelDuration"/>.</summary>
            [Obsolete("hapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.", true)]
            public static readonly GUIContent hapticHoverCancelDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverHapticsWhileSelecting"/>.</summary>
            [Obsolete("allowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.allowHoverHapticsWhileSelecting instead.", true)]
            public static readonly GUIContent allowHoverHapticsWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Haptics While Selecting", "Allow playing haptics from Hover events if the Hovered Interactable is currently Selected by this Interactor.");
        }

        /// <summary>
        /// (Deprecated) The <c>BaseControllerContents</c> class was renamed to <see cref="BaseInputContents"/>.
        /// Use <see cref="BaseInputContents"/> instead. This class exists to assist with migration.
        /// </summary>
        [Obsolete(k_BaseControllerContentsDeprecated, true)]
        protected static class BaseControllerContents
        {
            const string k_BaseControllerContentsDeprecated = "BaseControllerContents has been deprecated in version 3.0.0. It has been renamed to BaseInputContents.";

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.selectActionTrigger"/>.</summary>
            public static readonly GUIContent selectActionTrigger = BaseInputContents.selectActionTrigger;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoveredActivate"/>.</summary>
            public static readonly GUIContent allowHoveredActivate = BaseInputContents.allowHoveredActivate;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.targetPriorityMode"/>.</summary>
            public static readonly GUIContent targetPriorityMode = BaseInputContents.targetPriorityMode;

            /// <summary>The help box message when the <see cref="XRBaseInteractor.startingSelectedInteractable"/> will be instantly deselected due to the value of <see cref="XRBaseInputInteractor.selectActionTrigger"/>.</summary>
            public static readonly string selectActionTriggerWarning = BaseInputContents.selectActionTriggerWarning;

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hideControllerOnSelect"/>.</summary>
            public static readonly GUIContent hideControllerOnSelect = BaseInputContents.hideControllerOnSelect;

            /// <summary>The help box message when <see cref="XRBaseController"/> is missing.</summary>
            public static readonly string missingRequiredController = BaseInputContents.missingRequiredController;

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectEntered"/>.</summary>
            public static readonly GUIContent playAudioClipOnSelectEntered = BaseInputContents.playAudioClipOnSelectEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectEntered"/>.</summary>
            public static readonly GUIContent audioClipForOnSelectEntered = BaseInputContents.audioClipForOnSelectEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectExited"/>.</summary>
            public static readonly GUIContent playAudioClipOnSelectExited = BaseInputContents.playAudioClipOnSelectExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectExited"/>.</summary>
            public static readonly GUIContent audioClipForOnSelectExited = BaseInputContents.audioClipForOnSelectExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectCanceled"/>.</summary>
            public static readonly GUIContent playAudioClipOnSelectCanceled = BaseInputContents.playAudioClipOnSelectCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectCanceled"/>.</summary>
            public static readonly GUIContent audioClipForOnSelectCanceled = BaseInputContents.audioClipForOnSelectCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverEntered"/>.</summary>
            public static readonly GUIContent playAudioClipOnHoverEntered = BaseInputContents.playAudioClipOnHoverEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverEntered"/>.</summary>
            public static readonly GUIContent audioClipForOnHoverEntered = BaseInputContents.audioClipForOnHoverEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverExited"/>.</summary>
            public static readonly GUIContent playAudioClipOnHoverExited = BaseInputContents.playAudioClipOnHoverExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverExited"/>.</summary>
            public static readonly GUIContent audioClipForOnHoverExited = BaseInputContents.audioClipForOnHoverExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverCanceled"/>.</summary>
            public static readonly GUIContent playAudioClipOnHoverCanceled = BaseInputContents.playAudioClipOnHoverCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverCanceled"/>.</summary>
            public static readonly GUIContent audioClipForOnHoverCanceled = BaseInputContents.audioClipForOnHoverCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverAudioWhileSelecting"/>.</summary>
            public static readonly GUIContent allowHoverAudioWhileSelecting = BaseInputContents.allowHoverAudioWhileSelecting;

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectEntered"/>.</summary>
            public static readonly GUIContent playHapticsOnSelectEntered = BaseInputContents.playHapticsOnSelectEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterIntensity"/>.</summary>
            public static readonly GUIContent hapticSelectEnterIntensity = BaseInputContents.hapticSelectEnterIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterDuration"/>.</summary>
            public static readonly GUIContent hapticSelectEnterDuration = BaseInputContents.hapticSelectEnterDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverEntered"/>.</summary>
            public static readonly GUIContent playHapticsOnHoverEntered = BaseInputContents.playHapticsOnHoverEntered;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterIntensity"/>.</summary>
            public static readonly GUIContent hapticHoverEnterIntensity = BaseInputContents.hapticHoverEnterIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterDuration"/>.</summary>
            public static readonly GUIContent hapticHoverEnterDuration = BaseInputContents.hapticHoverEnterDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectExited"/>.</summary>
            public static readonly GUIContent playHapticsOnSelectExited = BaseInputContents.playHapticsOnSelectExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitIntensity"/>.</summary>
            public static readonly GUIContent hapticSelectExitIntensity = BaseInputContents.hapticSelectExitIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitDuration"/>.</summary>
            public static readonly GUIContent hapticSelectExitDuration = BaseInputContents.hapticSelectExitDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectCanceled"/>.</summary>
            public static readonly GUIContent playHapticsOnSelectCanceled = BaseInputContents.playHapticsOnSelectCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelIntensity"/>.</summary>
            public static readonly GUIContent hapticSelectCancelIntensity = BaseInputContents.hapticSelectCancelIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelDuration"/>.</summary>
            public static readonly GUIContent hapticSelectCancelDuration = BaseInputContents.hapticSelectCancelDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverExited"/>.</summary>
            public static readonly GUIContent playHapticsOnHoverExited = BaseInputContents.playHapticsOnHoverExited;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitIntensity"/>.</summary>
            public static readonly GUIContent hapticHoverExitIntensity = BaseInputContents.hapticHoverExitIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitDuration"/>.</summary>
            public static readonly GUIContent hapticHoverExitDuration = BaseInputContents.hapticHoverExitDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverCanceled"/>.</summary>
            public static readonly GUIContent playHapticsOnHoverCanceled = BaseInputContents.playHapticsOnHoverCanceled;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelIntensity"/>.</summary>
            public static readonly GUIContent hapticHoverCancelIntensity = BaseInputContents.hapticHoverCancelIntensity;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelDuration"/>.</summary>
            public static readonly GUIContent hapticHoverCancelDuration = BaseInputContents.hapticHoverCancelDuration;
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverHapticsWhileSelecting"/>.</summary>
            public static readonly GUIContent allowHoverHapticsWhileSelecting = BaseInputContents.allowHoverHapticsWhileSelecting;
        }

        // ReSharper disable once RedundantOverriddenMember -- Method needed for API Updater Configuration Validation test false positive
        /// <inheritdoc />
        protected override void DrawBeforeProperties()
        {
            base.DrawBeforeProperties();
        }

        /// <summary>
        /// Draw the Audio Events foldout.
        /// </summary>
        /// <seealso cref="DrawAudioEventsNested"/>
        [Obsolete(k_DrawAudioEventsDeprecated, true)]
        protected virtual void DrawAudioEvents()
        {
            throw new NotSupportedException(k_DrawAudioEventsDeprecated);
        }

        /// <summary>
        /// Draw the nested contents of the Audio Events foldout.
        /// </summary>
        /// <seealso cref="DrawAudioEvents"/>
        [Obsolete(k_DrawAudioEventsNestedDeprecated, true)]
        protected virtual void DrawAudioEventsNested()
        {
            throw new NotSupportedException(k_DrawAudioEventsNestedDeprecated);
        }

        /// <summary>
        /// Draw the Haptic Events foldout.
        /// </summary>
        /// <seealso cref="DrawHapticEventsNested"/>
        [Obsolete(k_DrawHapticEventsDeprecated, true)]
        protected virtual void DrawHapticEvents()
        {
            throw new NotSupportedException(k_DrawHapticEventsDeprecated);
        }

        /// <summary>
        /// Draw the nested contents of the Haptic Events foldout.
        /// </summary>
        /// <seealso cref="DrawHapticEvents"/>
        [Obsolete(k_DrawHapticEventsNestedDeprecated, true)]
        protected virtual void DrawHapticEventsNested()
        {
            throw new NotSupportedException(k_DrawHapticEventsNestedDeprecated);
        }

        /// <summary>
        /// Draw the properties related to the deprecated XR Controller component.
        /// </summary>
        [Obsolete(k_DrawDeprecatedControllerConfigurationDeprecated, true)]
        protected void DrawDeprecatedControllerConfiguration()
        {
            throw new NotSupportedException(k_DrawDeprecatedControllerConfigurationDeprecated);
        }

        /// <summary>
        /// Verify that the required <see cref="XRBaseController"/> component is present
        /// and display a warning box if missing.
        /// </summary>
        [Obsolete(k_VerifyControllerPresentDeprecated, true)]
        protected virtual void VerifyControllerPresent()
        {
            throw new NotSupportedException(k_VerifyControllerPresentDeprecated);
        }
    }
}
