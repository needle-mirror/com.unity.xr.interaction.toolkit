using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    public partial class XRBaseInputInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.inputCompatibilityMode"/>.</summary>
        [Obsolete("m_InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
        protected SerializedProperty m_InputCompatibilityMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hideControllerOnSelect"/>.</summary>
        [Obsolete("m_HideControllerOnSelect has been deprecated in version 3.0.0.")]
        protected SerializedProperty m_HideControllerOnSelect;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.forceDeprecatedInput"/>.</summary>
        [Obsolete("m_ForceDeprecatedInput introduced in version 3.0.0-pre.1 has been removed in version 3.0.0-pre.2. Use m_InputCompatibilityMode instead.", true)]
        protected SerializedProperty m_ForceDeprecatedInput;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectEntered"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectEntered instead.")]
        protected SerializedProperty m_PlayAudioClipOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectEntered"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectEnteredClip instead.")]
        protected SerializedProperty m_AudioClipForOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectExited"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectExited instead.")]
        protected SerializedProperty m_PlayAudioClipOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectExited"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectExitedClip instead.")]
        protected SerializedProperty m_AudioClipForOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnSelectCanceled"/>.</summary>
        [Obsolete("m_PlayAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlaySelectCanceled instead.")]
        protected SerializedProperty m_PlayAudioClipOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnSelectCanceled"/>.</summary>
        [Obsolete("m_AudioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_SelectCanceledClip instead.")]
        protected SerializedProperty m_AudioClipForOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverEntered"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverEntered instead.")]
        protected SerializedProperty m_PlayAudioClipOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverEntered"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverEnteredClip instead.")]
        protected SerializedProperty m_AudioClipForOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverExited"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverExited instead.")]
        protected SerializedProperty m_PlayAudioClipOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverExited"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverExitedClip instead.")]
        protected SerializedProperty m_AudioClipForOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playAudioClipOnHoverCanceled"/>.</summary>
        [Obsolete("m_PlayAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_PlayHoverCanceled instead.")]
        protected SerializedProperty m_PlayAudioClipOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.audioClipForOnHoverCanceled"/>.</summary>
        [Obsolete("m_AudioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_HoverCanceledClip instead.")]
        protected SerializedProperty m_AudioClipForOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.allowHoverAudioWhileSelecting"/>.</summary>
        [Obsolete("m_AllowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.m_AllowHoverAudioWhileSelecting instead.")]
        protected SerializedProperty m_AllowHoverAudioWhileSelecting;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectEntered"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectEntered instead.")]
        protected SerializedProperty m_PlayHapticsOnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectEnterIntensity"/>.</summary>
        [Obsolete("m_HapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectEnteredDataAmplitude instead.")]
        protected SerializedProperty m_HapticSelectEnterIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectEnterDuration"/>.</summary>
        [Obsolete("m_HapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectEnteredDataDuration instead.")]
        protected SerializedProperty m_HapticSelectEnterDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverEntered"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverEntered instead.")]
        protected SerializedProperty m_PlayHapticsOnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverEnterIntensity"/>.</summary>
        [Obsolete("m_HapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverEnteredDataAmplitude instead.")]
        protected SerializedProperty m_HapticHoverEnterIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverEnterDuration"/>.</summary>
        [Obsolete("m_HapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverEnteredDataDuration instead.")]
        protected SerializedProperty m_HapticHoverEnterDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectExited"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectExited instead.")]
        protected SerializedProperty m_PlayHapticsOnSelectExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectExitIntensity"/>.</summary>
        [Obsolete("m_HapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectExitedDataAmplitude instead.")]
        protected SerializedProperty m_HapticSelectExitIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectExitDuration"/>.</summary>
        [Obsolete("m_HapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectExitedDataDuration instead.")]
        protected SerializedProperty m_HapticSelectExitDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnSelectCanceled"/>.</summary>
        [Obsolete("m_PlayHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlaySelectCanceled instead.")]
        protected SerializedProperty m_PlayHapticsOnSelectCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectCancelIntensity"/>.</summary>
        [Obsolete("m_HapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectCanceledDataAmplitude instead.")]
        protected SerializedProperty m_HapticSelectCancelIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticSelectCancelDuration"/>.</summary>
        [Obsolete("m_HapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_SelectCanceledDataDuration instead.")]
        protected SerializedProperty m_HapticSelectCancelDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverExited"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverExited instead.")]
        protected SerializedProperty m_PlayHapticsOnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverExitIntensity"/>.</summary>
        [Obsolete("m_HapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverExitedDataAmplitude instead.")]
        protected SerializedProperty m_HapticHoverExitIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverExitDuration"/>.</summary>
        [Obsolete("m_HapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverExitedDataDuration instead.")]
        protected SerializedProperty m_HapticHoverExitDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.playHapticsOnHoverCanceled"/>.</summary>
        [Obsolete("m_PlayHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_PlayHoverCanceled instead.")]
        protected SerializedProperty m_PlayHapticsOnHoverCanceled;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverCancelIntensity"/>.</summary>
        [Obsolete("m_HapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverCanceledDataAmplitude instead.")]
        protected SerializedProperty m_HapticHoverCancelIntensity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.hapticHoverCancelDuration"/>.</summary>
        [Obsolete("m_HapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_HoverCanceledDataDuration instead.")]
        protected SerializedProperty m_HapticHoverCancelDuration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInputInteractor.allowHoverHapticsWhileSelecting"/>.</summary>
        [Obsolete("m_AllowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.m_AllowHoverHapticsWhileSelecting instead.")]
        protected SerializedProperty m_AllowHoverHapticsWhileSelecting;

        protected static partial class BaseInputContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.inputCompatibilityMode"/>.</summary>
            [Obsolete("inputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
            public static readonly GUIContent inputCompatibilityMode = EditorGUIUtility.TrTextContent("Input Compatibility Mode", "Controls whether input is obtained through the deprecated legacy method where the XR Controller component is used. This is only used for backwards compatibility and will be eventually removed in a future version.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hideControllerOnSelect"/>.</summary>
            [Obsolete("hideControllerOnSelect has been deprecated in version 3.0.0.")]
            public static readonly GUIContent hideControllerOnSelect = EditorGUIUtility.TrTextContent("Hide Controller On Select", "(Deprecated) Hide the controller model on select. Requires an XR Controller component.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.forceDeprecatedInput"/>.</summary>
            [Obsolete("forceDeprecatedInput introduced in version 3.0.0-pre.1 has been removed in version 3.0.0-pre.2. Use m_InputCompatibilityMode instead.", true)]
            public static readonly GUIContent forceDeprecatedInput = EditorGUIUtility.TrTextContent("Force Deprecated Input", "Force the use of the deprecated input path where the input values are obtained through the XR Controller (Action-based) or XR Controller (Device-based). Not recommended.");

            /// <summary>The help box message when <see cref="XRBaseController"/> is missing.</summary>
            [Obsolete("missingRequiredController has been deprecated in version 3.0.0.")]
            public static readonly string missingRequiredController = "This component requires the GameObject or a parent GameObject to have an XR Controller component. Add one to ensure this component can respond to user input.";

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectEntered"/>.</summary>
            [Obsolete("playAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectEntered instead.")]
            public static readonly GUIContent playAudioClipOnSelectEntered = EditorGUIUtility.TrTextContent("On Select Entered", "Play an audio clip when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectEntered"/>.</summary>
            [Obsolete("audioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectEnteredClip instead.")]
            public static readonly GUIContent audioClipForOnSelectEntered = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectExited"/>.</summary>
            [Obsolete("playAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectExited instead.")]
            public static readonly GUIContent playAudioClipOnSelectExited = EditorGUIUtility.TrTextContent("On Select Exited", "Play an audio clip when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectExited"/>.</summary>
            [Obsolete("audioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectExitedClip instead.")]
            public static readonly GUIContent audioClipForOnSelectExited = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnSelectCanceled"/>.</summary>
            [Obsolete("playAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playSelectCanceled instead.")]
            public static readonly GUIContent playAudioClipOnSelectCanceled = EditorGUIUtility.TrTextContent("On Select Canceled", "Play an audio clip when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnSelectCanceled"/>.</summary>
            [Obsolete("audioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.selectCanceledClip instead.")]
            public static readonly GUIContent audioClipForOnSelectCanceled = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverEntered"/>.</summary>
            [Obsolete("playAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverEntered instead.")]
            public static readonly GUIContent playAudioClipOnHoverEntered = EditorGUIUtility.TrTextContent("On Hover Entered", "Play an audio clip when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverEntered"/>.</summary>
            [Obsolete("audioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverEnteredClip instead.")]
            public static readonly GUIContent audioClipForOnHoverEntered = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverExited"/>.</summary>
            [Obsolete("playAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverExited instead.")]
            public static readonly GUIContent playAudioClipOnHoverExited = EditorGUIUtility.TrTextContent("On Hover Exited", "Play an audio clip when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverExited"/>.</summary>
            [Obsolete("audioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverExitedClip instead.")]
            public static readonly GUIContent audioClipForOnHoverExited = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playAudioClipOnHoverCanceled"/>.</summary>
            [Obsolete("playAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.playHoverCanceled instead.")]
            public static readonly GUIContent playAudioClipOnHoverCanceled = EditorGUIUtility.TrTextContent("On Hover Canceled", "Play an audio clip when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.audioClipForOnHoverCanceled"/>.</summary>
            [Obsolete("audioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.hoverCanceledClip instead.")]
            public static readonly GUIContent audioClipForOnHoverCanceled = EditorGUIUtility.TrTextContent("AudioClip To Play", "The audio clip to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverAudioWhileSelecting"/>.</summary>
            [Obsolete("allowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor.Contents.allowHoverAudioWhileSelecting instead.")]
            public static readonly GUIContent allowHoverAudioWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Audio While Selecting", "Allow playing audio from Hover events if the Hovered Interactable is currently Selected by this Interactor.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectEntered"/>.</summary>
            [Obsolete("playHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectEntered instead.")]
            public static readonly GUIContent playHapticsOnSelectEntered = EditorGUIUtility.TrTextContent("On Select Entered", "Play haptics when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterIntensity"/>.</summary>
            [Obsolete("hapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticSelectEnterIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectEnterDuration"/>.</summary>
            [Obsolete("hapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticSelectEnterDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverEntered"/>.</summary>
            [Obsolete("playHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverEntered instead.")]
            public static readonly GUIContent playHapticsOnHoverEntered = EditorGUIUtility.TrTextContent("On Hover Entered", "Play haptics when the Hover State is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterIntensity"/>.</summary>
            [Obsolete("hapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticHoverEnterIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverEnterDuration"/>.</summary>
            [Obsolete("hapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticHoverEnterDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is entered.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectExited"/>.</summary>
            [Obsolete("playHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectExited instead.")]
            public static readonly GUIContent playHapticsOnSelectExited = EditorGUIUtility.TrTextContent("On Select Exited", "Play haptics when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitIntensity"/>.</summary>
            [Obsolete("hapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticSelectExitIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectExitDuration"/>.</summary>
            [Obsolete("hapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticSelectExitDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnSelectCanceled"/>.</summary>
            [Obsolete("playHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playSelectCanceled instead.")]
            public static readonly GUIContent playHapticsOnSelectCanceled = EditorGUIUtility.TrTextContent("On Select Canceled", "Play haptics when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelIntensity"/>.</summary>
            [Obsolete("hapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticSelectCancelIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticSelectCancelDuration"/>.</summary>
            [Obsolete("hapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticSelectCancelDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Select state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverExited"/>.</summary>
            [Obsolete("playHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverExited instead.")]
            public static readonly GUIContent playHapticsOnHoverExited = EditorGUIUtility.TrTextContent("On Hover Exited", "Play haptics when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitIntensity"/>.</summary>
            [Obsolete("hapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticHoverExitIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverExitDuration"/>.</summary>
            [Obsolete("hapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticHoverExitDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is exited without being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.playHapticsOnHoverCanceled"/>.</summary>
            [Obsolete("playHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.playHoverCanceled instead.")]
            public static readonly GUIContent playHapticsOnHoverCanceled = EditorGUIUtility.TrTextContent("On Hover Canceled", "Play haptics when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelIntensity"/>.</summary>
            [Obsolete("hapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.amplitude instead.")]
            public static readonly GUIContent hapticHoverCancelIntensity = EditorGUIUtility.TrTextContent("Haptic Intensity", "Haptics intensity to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.hapticHoverCancelDuration"/>.</summary>
            [Obsolete("hapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.duration instead.")]
            public static readonly GUIContent hapticHoverCancelDuration = EditorGUIUtility.TrTextContent("Duration", "Haptics duration (in seconds) to play when the Hover state is exited due to being canceled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInputInteractor.allowHoverHapticsWhileSelecting"/>.</summary>
            [Obsolete("allowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor.Contents.allowHoverHapticsWhileSelecting instead.")]
            public static readonly GUIContent allowHoverHapticsWhileSelecting = EditorGUIUtility.TrTextContent("Allow Hover Haptics While Selecting", "Allow playing haptics from Hover events if the Hovered Interactable is currently Selected by this Interactor.");
        }

        /// <summary>
        /// (Deprecated) The <c>BaseControllerContents</c> class was renamed to <see cref="BaseInputContents"/>.
        /// Use <see cref="BaseInputContents"/> instead. This class exists to assist with migration.
        /// </summary>
        [Obsolete("BaseControllerContents has been deprecated in version 3.0.0. It has been renamed to BaseInputContents.")]
        protected static class BaseControllerContents
        {
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
        [Obsolete("DrawAudioEvents has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor instead.")]
        protected virtual void DrawAudioEvents()
        {
            m_PlayAudioClipOnSelectEntered.isExpanded = EditorGUILayout.Foldout(m_PlayAudioClipOnSelectEntered.isExpanded, EditorGUIUtility.TrTempContent("(Deprecated) Audio Events"), true);
            if (m_PlayAudioClipOnSelectEntered.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawAudioEventsNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Audio Events foldout.
        /// </summary>
        /// <seealso cref="DrawAudioEvents"/>
        [Obsolete("DrawAudioEventsNested has been deprecated in version 3.0.0. Use SimpleAudioFeedbackEditor instead.")]
        protected virtual void DrawAudioEventsNested()
        {
            EditorGUILayout.HelpBox("Audio Events have been deprecated. Use the Simple Audio Feedback component instead.", MessageType.Warning);
            // Disable controls in Play mode since the fields are only pushed to the Simple Audio Feedback component at Awake.
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                EditorGUILayout.PropertyField(m_PlayAudioClipOnSelectEntered, BaseInputContents.playAudioClipOnSelectEntered);
                if (m_PlayAudioClipOnSelectEntered.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnSelectEntered, BaseInputContents.audioClipForOnSelectEntered);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayAudioClipOnSelectExited, BaseInputContents.playAudioClipOnSelectExited);
                if (m_PlayAudioClipOnSelectExited.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnSelectExited, BaseInputContents.audioClipForOnSelectExited);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayAudioClipOnSelectCanceled, BaseInputContents.playAudioClipOnSelectCanceled);
                if (m_PlayAudioClipOnSelectCanceled.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnSelectCanceled, BaseInputContents.audioClipForOnSelectCanceled);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayAudioClipOnHoverEntered, BaseInputContents.playAudioClipOnHoverEntered);
                if (m_PlayAudioClipOnHoverEntered.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnHoverEntered, BaseInputContents.audioClipForOnHoverEntered);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayAudioClipOnHoverExited, BaseInputContents.playAudioClipOnHoverExited);
                if (m_PlayAudioClipOnHoverExited.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnHoverExited, BaseInputContents.audioClipForOnHoverExited);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayAudioClipOnHoverCanceled, BaseInputContents.playAudioClipOnHoverCanceled);
                if (m_PlayAudioClipOnHoverCanceled.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_AudioClipForOnHoverCanceled, BaseInputContents.audioClipForOnHoverCanceled);
                    }
                }

                EditorGUILayout.PropertyField(m_AllowHoverAudioWhileSelecting, BaseInputContents.allowHoverAudioWhileSelecting);
            }
        }

        /// <summary>
        /// Draw the Haptic Events foldout.
        /// </summary>
        /// <seealso cref="DrawHapticEventsNested"/>
        [Obsolete("DrawHapticEvents has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor instead.")]
        protected virtual void DrawHapticEvents()
        {
            m_PlayHapticsOnSelectEntered.isExpanded = EditorGUILayout.Foldout(m_PlayHapticsOnSelectEntered.isExpanded, EditorGUIUtility.TrTempContent("(Deprecated) Haptic Events"), true);
            if (m_PlayHapticsOnSelectEntered.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawHapticEventsNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Haptic Events foldout.
        /// </summary>
        /// <seealso cref="DrawHapticEvents"/>
        [Obsolete("DrawHapticEventsNested has been deprecated in version 3.0.0. Use SimpleHapticFeedbackEditor instead.")]
        protected virtual void DrawHapticEventsNested()
        {
            EditorGUILayout.HelpBox("Haptic Events have been deprecated. Use the Simple Haptic Feedback component instead.", MessageType.Warning);
            // Disable controls in Play mode since the fields are only pushed to the Simple Haptic Feedback component at Awake.
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                EditorGUILayout.PropertyField(m_PlayHapticsOnSelectEntered, BaseInputContents.playHapticsOnSelectEntered);
                if (m_PlayHapticsOnSelectEntered.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticSelectEnterIntensity, BaseInputContents.hapticSelectEnterIntensity);
                        EditorGUILayout.PropertyField(m_HapticSelectEnterDuration, BaseInputContents.hapticSelectEnterDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayHapticsOnSelectExited, BaseInputContents.playHapticsOnSelectExited);
                if (m_PlayHapticsOnSelectExited.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticSelectExitIntensity, BaseInputContents.hapticSelectExitIntensity);
                        EditorGUILayout.PropertyField(m_HapticSelectExitDuration, BaseInputContents.hapticSelectExitDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayHapticsOnSelectCanceled, BaseInputContents.playHapticsOnSelectCanceled);
                if (m_PlayHapticsOnSelectCanceled.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticSelectCancelIntensity, BaseInputContents.hapticSelectCancelIntensity);
                        EditorGUILayout.PropertyField(m_HapticSelectCancelDuration, BaseInputContents.hapticSelectCancelDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayHapticsOnHoverEntered, BaseInputContents.playHapticsOnHoverEntered);
                if (m_PlayHapticsOnHoverEntered.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticHoverEnterIntensity, BaseInputContents.hapticHoverEnterIntensity);
                        EditorGUILayout.PropertyField(m_HapticHoverEnterDuration, BaseInputContents.hapticHoverEnterDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayHapticsOnHoverExited, BaseInputContents.playHapticsOnHoverExited);
                if (m_PlayHapticsOnHoverExited.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticHoverExitIntensity, BaseInputContents.hapticHoverExitIntensity);
                        EditorGUILayout.PropertyField(m_HapticHoverExitDuration, BaseInputContents.hapticHoverExitDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_PlayHapticsOnHoverCanceled, BaseInputContents.playHapticsOnHoverCanceled);
                if (m_PlayHapticsOnHoverCanceled.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_HapticHoverCancelIntensity, BaseInputContents.hapticHoverCancelIntensity);
                        EditorGUILayout.PropertyField(m_HapticHoverCancelDuration, BaseInputContents.hapticHoverCancelDuration);
                    }
                }

                EditorGUILayout.PropertyField(m_AllowHoverHapticsWhileSelecting, BaseInputContents.allowHoverHapticsWhileSelecting);
            }
        }

        /// <summary>
        /// Draw the properties related to the deprecated XR Controller component.
        /// </summary>
        [Obsolete("DrawDeprecatedControllerConfiguration has been deprecated in version 3.0.0.")]
        protected void DrawDeprecatedControllerConfiguration()
        {
            m_InputCompatibilityMode.isExpanded = EditorGUILayout.Foldout(m_InputCompatibilityMode.isExpanded, EditorGUIUtility.TrTempContent("(Deprecated) XR Controller Configuration"), true);
            if (m_InputCompatibilityMode.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_InputCompatibilityMode, BaseInputContents.inputCompatibilityMode);
                    VerifyControllerPresent();
                    EditorGUILayout.PropertyField(m_HideControllerOnSelect, BaseInputContents.hideControllerOnSelect);
                }
            }
        }

        /// <summary>
        /// Verify that the required <see cref="XRBaseController"/> component is present
        /// and display a warning box if missing.
        /// </summary>
        [Obsolete("VerifyControllerPresent has been deprecated in version 3.0.0.")]
        protected virtual void VerifyControllerPresent()
        {
            foreach (var targetObject in serializedObject.targetObjects)
            {
                var interactor = (XRBaseInputInteractor)targetObject;
                if (interactor.inputCompatibilityMode == XRBaseInputInteractor.InputCompatibilityMode.ForceDeprecatedInput &&
                    interactor.GetComponentInParent<XRBaseController>(true) == null)
                {
                    EditorGUILayout.HelpBox(BaseInputContents.missingRequiredController, MessageType.Warning, true);
                    break;
                }
            }
        }
    }
}
