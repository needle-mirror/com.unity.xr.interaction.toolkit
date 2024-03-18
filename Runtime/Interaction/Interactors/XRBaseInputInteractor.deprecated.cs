using System;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    public abstract partial class XRBaseInputInteractor
    {
        /// <summary>
        /// Controls whether input can be obtained through the deprecated legacy method for backwards compatibility.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        /// <seealso cref="inputCompatibilityMode"/>
        [Obsolete("InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
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
            [Obsolete("wasUnperformedThisFrame has been deprecated in version 3.0.0-pre.2. It has been renamed to wasCompletedThisFrame. (UnityUpgradable) -> wasCompletedThisFrame")]
            public bool wasUnperformedThisFrame => wasCompletedThisFrame;
        }

        [SerializeField]
        bool m_HideControllerOnSelect;

        /// <summary>
        /// Controls whether this Interactor should hide the controller model on selection.
        /// </summary>
        /// <seealso cref="XRBaseController.hideControllerModel"/>
        [Obsolete("hideControllerOnSelect has been deprecated in version 3.0.0.")]
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

        [SerializeField]
        [Obsolete("m_InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
        InputCompatibilityMode m_InputCompatibilityMode = InputCompatibilityMode.Automatic;


        /// <summary>
        /// Controls whether input is obtained through the deprecated legacy method where the XR Controller component is used.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        /// <seealso cref="InputCompatibilityMode"/>
        /// <seealso cref="forceDeprecatedInput"/>
        [Obsolete("inputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
        public InputCompatibilityMode inputCompatibilityMode
        {
            get => m_InputCompatibilityMode;
            set => m_InputCompatibilityMode = value;
        }

        /// <summary>
        /// Controls whether this interactor is being forced to use the deprecated input path where the input values are obtained through the <see cref="xrController"/>.
        /// This is only used for backwards compatibility and will be eventually removed in a future version.
        /// </summary>
        [Obsolete("forceDeprecatedInput introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
        public bool forceDeprecatedInput
        {
            get => (m_HasXRController && m_InputCompatibilityMode == InputCompatibilityMode.Automatic) || m_InputCompatibilityMode == InputCompatibilityMode.ForceDeprecatedInput;
            set => m_InputCompatibilityMode = value ? InputCompatibilityMode.ForceDeprecatedInput : InputCompatibilityMode.ForceInputReaders;
        }

        [Obsolete("m_Controller has been deprecated in version 3.0.0.")]
        XRBaseController m_Controller;

        /// <summary>
        /// (Deprecated) The controller instance that is queried for input.
        /// </summary>
        [Obsolete("xrController has been deprecated in version 3.0.0.")]
        public XRBaseController xrController
        {
            get => m_Controller;
            set
            {
                if (m_Controller != value)
                {
                    m_Controller = value;
                    OnXRControllerChanged();
                }
            }
        }

        bool m_HasXRController;

        /// <summary>
        /// (Deprecated) (Read Only) Whether or not Unity considers the UI Press controller input pressed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if active. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use a serialized XRInputButtonProvider to read button input instead.")]
        protected virtual bool isUISelectActive => m_Controller != null && m_Controller.uiPressInteractionState.active;

        /// <summary>
        /// (Deprecated) (Read Only) The current scroll value Unity would apply to the UI.
        /// </summary>
        /// <returns>Returns a Vector2 with scroll strength for each axis. </returns>
        [Obsolete("uiScrollValue has been deprecated in version 3.0.0. Use a serialized XRInputValueProvider<Vector2> to read scroll input instead.")]
        protected Vector2 uiScrollValue => m_Controller != null ? m_Controller.uiScrollValue : Vector2.zero;

        #region Audio Events

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnSelectEnter")]
        bool m_PlayAudioClipOnSelectEntered;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectEntered"/>
        [Obsolete("playAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectEntered instead.")]
        public bool playAudioClipOnSelectEntered
        {
            get => m_PlayAudioClipOnSelectEntered;
            set
            {
                m_PlayAudioClipOnSelectEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playSelectEntered = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnSelectEnter")]
        AudioClip m_AudioClipForOnSelectEntered;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
        [Obsolete("audioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectEnteredClip instead.")]
        public AudioClip audioClipForOnSelectEntered
        {
            get => m_AudioClipForOnSelectEntered;
            set
            {
                m_AudioClipForOnSelectEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.selectEnteredClip = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnSelectExit")]
        bool m_PlayAudioClipOnSelectExited;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectExited"/>
        [Obsolete("playAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectExited instead.")]
        public bool playAudioClipOnSelectExited
        {
            get => m_PlayAudioClipOnSelectExited;
            set
            {
                m_PlayAudioClipOnSelectExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playSelectExited = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnSelectExit")]
        AudioClip m_AudioClipForOnSelectExited;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
        [Obsolete("audioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectExitedClip instead.")]
        public AudioClip audioClipForOnSelectExited
        {
            get => m_AudioClipForOnSelectExited;
            set
            {
                m_AudioClipForOnSelectExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.selectExitedClip = value;
                }
            }
        }

        [SerializeField]
        bool m_PlayAudioClipOnSelectCanceled;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Select Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectCanceled"/>
        [Obsolete("playAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectCanceled instead.")]
        public bool playAudioClipOnSelectCanceled
        {
            get => m_PlayAudioClipOnSelectCanceled;
            set
            {
                m_PlayAudioClipOnSelectCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playSelectCanceled = value;
                }
            }
        }

        [SerializeField]
        AudioClip m_AudioClipForOnSelectCanceled;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectCanceled"/>
        [Obsolete("audioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectCanceledClip instead.")]
        public AudioClip audioClipForOnSelectCanceled
        {
            get => m_AudioClipForOnSelectCanceled;
            set
            {
                m_AudioClipForOnSelectCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.selectCanceledClip = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnHoverEnter")]
        bool m_PlayAudioClipOnHoverEntered;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverEntered"/>
        [Obsolete("playAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverEntered instead.")]
        public bool playAudioClipOnHoverEntered
        {
            get => m_PlayAudioClipOnHoverEntered;
            set
            {
                m_PlayAudioClipOnHoverEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playHoverEntered = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnHoverEnter")]
        AudioClip m_AudioClipForOnHoverEntered;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        [Obsolete("audioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverEnteredClip instead.")]
        public AudioClip audioClipForOnHoverEntered
        {
            get => m_AudioClipForOnHoverEntered;
            set
            {
                m_AudioClipForOnHoverEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.hoverEnteredClip = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayAudioClipOnHoverExit")]
        bool m_PlayAudioClipOnHoverExited;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverExited"/>
        [Obsolete("playAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverExited instead.")]
        public bool playAudioClipOnHoverExited
        {
            get => m_PlayAudioClipOnHoverExited;
            set
            {
                m_PlayAudioClipOnHoverExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playHoverExited = value;
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_AudioClipForOnHoverExit")]
        AudioClip m_AudioClipForOnHoverExited;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        [Obsolete("audioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverExitedClip instead.")]
        public AudioClip audioClipForOnHoverExited
        {
            get => m_AudioClipForOnHoverExited;
            set
            {
                m_AudioClipForOnHoverExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.hoverExitedClip = value;
                }
            }
        }

        [SerializeField]
        bool m_PlayAudioClipOnHoverCanceled;
        /// <summary>
        /// (Deprecated) Controls whether Unity plays an <see cref="AudioClip"/> on Hover Canceled.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverCanceled"/>
        [Obsolete("playAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverCanceled instead.")]
        public bool playAudioClipOnHoverCanceled
        {
            get => m_PlayAudioClipOnHoverCanceled;
            set
            {
                m_PlayAudioClipOnHoverCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.playHoverCanceled = value;
                }
            }
        }

        [SerializeField]
        AudioClip m_AudioClipForOnHoverCanceled;
        /// <summary>
        /// (Deprecated) The <see cref="AudioClip"/> Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        [Obsolete("audioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverCanceledClip instead.")]
        public AudioClip audioClipForOnHoverCanceled
        {
            get => m_AudioClipForOnHoverCanceled;
            set
            {
                m_AudioClipForOnHoverCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.hoverCanceledClip = value;
                }
            }
        }

        [SerializeField]
        bool m_AllowHoverAudioWhileSelecting = true;
        /// <summary>
        /// (Deprecated) Controls whether Unity allows playing an <see cref="AudioClip"/> from a Hover event if the Hovered Interactable is currently Selected by this Interactor.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        /// <seealso cref="playAudioClipOnHoverCanceled"/>
        [Obsolete("allowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedback.allowHoverAudioWhileSelecting instead.")]
        public bool allowHoverAudioWhileSelecting
        {
            get => m_AllowHoverAudioWhileSelecting;
            set
            {
                m_AllowHoverAudioWhileSelecting = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateAudioFeedback();
                    m_AudioFeedback.allowHoverAudioWhileSelecting = value;
                }
            }
        }

        #endregion

        #region Haptic Events

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnSelectEnter")]
        bool m_PlayHapticsOnSelectEntered;
        /// <summary>
        /// Controls whether Unity plays haptics on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="hapticSelectEnterDuration"/>
        [Obsolete("playHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectEntered instead.")]
        public bool playHapticsOnSelectEntered
        {
            get => m_PlayHapticsOnSelectEntered;
            set
            {
                m_PlayHapticsOnSelectEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playSelectEntered = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticSelectEnterIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterDuration"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        [Obsolete("hapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.amplitude instead.")]
        public float hapticSelectEnterIntensity
        {
            get => m_HapticSelectEnterIntensity;
            set
            {
                m_HapticSelectEnterIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectEnteredData != null)
                        m_HapticFeedback.selectEnteredData.amplitude = value;
                    else
                        m_HapticFeedback.selectEnteredData = new HapticImpulseData { amplitude = value, duration = m_HapticSelectEnterDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticSelectEnterDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="playHapticsOnSelectEntered"/>
        [Obsolete("hapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.duration instead.")]
        public float hapticSelectEnterDuration
        {
            get => m_HapticSelectEnterDuration;
            set
            {
                m_HapticSelectEnterDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectEnteredData != null)
                        m_HapticFeedback.selectEnteredData.duration = value;
                    else
                        m_HapticFeedback.selectEnteredData = new HapticImpulseData { amplitude = m_HapticSelectEnterIntensity, duration = value, };
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnSelectExit")]
        bool m_PlayHapticsOnSelectExited;
        /// <summary>
        /// Controls whether Unity plays haptics on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="hapticSelectExitDuration"/>
        [Obsolete("playHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectExited instead.")]
        public bool playHapticsOnSelectExited
        {
            get => m_PlayHapticsOnSelectExited;
            set
            {
                m_PlayHapticsOnSelectExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playSelectExited = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticSelectExitIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitDuration"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        [Obsolete("hapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.amplitude instead.")]
        public float hapticSelectExitIntensity
        {
            get => m_HapticSelectExitIntensity;
            set
            {
                m_HapticSelectExitIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectExitedData != null)
                        m_HapticFeedback.selectExitedData.amplitude = value;
                    else
                        m_HapticFeedback.selectExitedData = new HapticImpulseData { amplitude = value, duration = m_HapticSelectExitDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticSelectExitDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="playHapticsOnSelectExited"/>
        [Obsolete("hapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.duration instead.")]
        public float hapticSelectExitDuration
        {
            get => m_HapticSelectExitDuration;
            set
            {
                m_HapticSelectExitDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectExitedData != null)
                        m_HapticFeedback.selectExitedData.duration = value;
                    else
                        m_HapticFeedback.selectExitedData = new HapticImpulseData { amplitude = m_HapticSelectExitIntensity, duration = value, };
                }
            }
        }

        [SerializeField]
        bool m_PlayHapticsOnSelectCanceled;
        /// <summary>
        /// Controls whether Unity plays haptics on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="hapticSelectCancelDuration"/>
        [Obsolete("playHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectCanceled instead.")]
        public bool playHapticsOnSelectCanceled
        {
            get => m_PlayHapticsOnSelectCanceled;
            set
            {
                m_PlayHapticsOnSelectCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playSelectCanceled = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticSelectCancelIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelDuration"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        [Obsolete("hapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.amplitude instead.")]
        public float hapticSelectCancelIntensity
        {
            get => m_HapticSelectCancelIntensity;
            set
            {
                m_HapticSelectCancelIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectCanceledData != null)
                        m_HapticFeedback.selectCanceledData.amplitude = value;
                    else
                        m_HapticFeedback.selectCanceledData = new HapticImpulseData { amplitude = value, duration = m_HapticSelectCancelDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticSelectCancelDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Select Canceled.
        /// </summary>
        /// <seealso cref="hapticSelectCancelIntensity"/>
        /// <seealso cref="playHapticsOnSelectCanceled"/>
        [Obsolete("hapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.duration instead.")]
        public float hapticSelectCancelDuration
        {
            get => m_HapticSelectCancelDuration;
            set
            {
                m_HapticSelectCancelDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.selectCanceledData != null)
                        m_HapticFeedback.selectCanceledData.duration = value;
                    else
                        m_HapticFeedback.selectCanceledData = new HapticImpulseData { amplitude = m_HapticSelectCancelIntensity, duration = value, };
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnHoverEnter")]
        bool m_PlayHapticsOnHoverEntered;
        /// <summary>
        /// Controls whether Unity plays haptics on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="hapticHoverEnterDuration"/>
        [Obsolete("playHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverEntered instead.")]
        public bool playHapticsOnHoverEntered
        {
            get => m_PlayHapticsOnHoverEntered;
            set
            {
                m_PlayHapticsOnHoverEntered = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playHoverEntered = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticHoverEnterIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterDuration"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        [Obsolete("hapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.amplitude instead.")]
        public float hapticHoverEnterIntensity
        {
            get => m_HapticHoverEnterIntensity;
            set
            {
                m_HapticHoverEnterIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverEnteredData != null)
                        m_HapticFeedback.hoverEnteredData.amplitude = value;
                    else
                        m_HapticFeedback.hoverEnteredData = new HapticImpulseData { amplitude = value, duration = m_HapticHoverEnterDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticHoverEnterDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        [Obsolete("hapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.duration instead.")]
        public float hapticHoverEnterDuration
        {
            get => m_HapticHoverEnterDuration;
            set
            {
                m_HapticHoverEnterDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverEnteredData != null)
                        m_HapticFeedback.hoverEnteredData.duration = value;
                    else
                        m_HapticFeedback.hoverEnteredData = new HapticImpulseData { amplitude = m_HapticHoverEnterIntensity, duration = value, };
                }
            }
        }

        [SerializeField, FormerlySerializedAs("m_PlayHapticsOnHoverExit")]
        bool m_PlayHapticsOnHoverExited;
        /// <summary>
        /// Controls whether Unity plays haptics on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="hapticHoverExitDuration"/>
        [Obsolete("playHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverExited instead.")]
        public bool playHapticsOnHoverExited
        {
            get => m_PlayHapticsOnHoverExited;
            set
            {
                m_PlayHapticsOnHoverExited = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playHoverExited = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticHoverExitIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitDuration"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        [Obsolete("hapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.amplitude instead.")]
        public float hapticHoverExitIntensity
        {
            get => m_HapticHoverExitIntensity;
            set
            {
                m_HapticHoverExitIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverExitedData != null)
                        m_HapticFeedback.hoverExitedData.amplitude = value;
                    else
                        m_HapticFeedback.hoverExitedData = new HapticImpulseData { amplitude = value, duration = m_HapticHoverExitDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticHoverExitDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Exited.
        /// </summary>
        /// <seealso cref="hapticHoverExitIntensity"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        [Obsolete("hapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.duration instead.")]
        public float hapticHoverExitDuration
        {
            get => m_HapticHoverExitDuration;
            set
            {
                m_HapticHoverExitDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverExitedData != null)
                        m_HapticFeedback.hoverExitedData.duration = value;
                    else
                        m_HapticFeedback.hoverExitedData = new HapticImpulseData { amplitude = m_HapticHoverExitIntensity, duration = value, };
                }
            }
        }

        [SerializeField]
        bool m_PlayHapticsOnHoverCanceled;
        /// <summary>
        /// Controls whether Unity plays haptics on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="hapticHoverCancelDuration"/>
        [Obsolete("playHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverCanceled instead.")]
        public bool playHapticsOnHoverCanceled
        {
            get => m_PlayHapticsOnHoverCanceled;
            set
            {
                m_PlayHapticsOnHoverCanceled = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.playHoverCanceled = value;
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float m_HapticHoverCancelIntensity;
        /// <summary>
        /// The Haptics intensity Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelDuration"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        [Obsolete("hapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.amplitude instead.")]
        public float hapticHoverCancelIntensity
        {
            get => m_HapticHoverCancelIntensity;
            set
            {
                m_HapticHoverCancelIntensity = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverCanceledData != null)
                        m_HapticFeedback.hoverCanceledData.amplitude = value;
                    else
                        m_HapticFeedback.hoverCanceledData = new HapticImpulseData { amplitude = value, duration = m_HapticHoverCancelDuration, };
                }
            }
        }

        [SerializeField]
        float m_HapticHoverCancelDuration;
        /// <summary>
        /// The Haptics duration (in seconds) Unity plays on Hover Canceled.
        /// </summary>
        /// <seealso cref="hapticHoverCancelIntensity"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        [Obsolete("hapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.duration instead.")]
        public float hapticHoverCancelDuration
        {
            get => m_HapticHoverCancelDuration;
            set
            {
                m_HapticHoverCancelDuration = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    if (m_HapticFeedback.hoverCanceledData != null)
                        m_HapticFeedback.hoverCanceledData.duration = value;
                    else
                        m_HapticFeedback.hoverCanceledData = new HapticImpulseData { amplitude = m_HapticHoverCancelIntensity, duration = value, };
                }
            }
        }

        [SerializeField]
        bool m_AllowHoverHapticsWhileSelecting = true;
        /// <summary>
        /// Controls whether Unity allows playing haptics from a Hover event if the Hovered Interactable is currently Selected by this Interactor.
        /// </summary>
        /// <seealso cref="playHapticsOnHoverEntered"/>
        /// <seealso cref="playHapticsOnHoverExited"/>
        /// <seealso cref="playHapticsOnHoverCanceled"/>
        [Obsolete("allowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedback.allowHoverHapticsWhileSelecting instead.")]
        public bool allowHoverHapticsWhileSelecting
        {
            get => m_AllowHoverHapticsWhileSelecting;
            set
            {
                m_AllowHoverHapticsWhileSelecting = value;
                if (Application.isPlaying)
                {
                    GetOrCreateAndMigrateHapticFeedback();
                    m_HapticFeedback.allowHoverHapticsWhileSelecting = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// (Deprecated) Override this method to handle internal changes when the <see cref="xrController"/> property value
        /// changes.
        /// </summary>
        [Obsolete("OnXRControllerChanged has been deprecated in version 3.0.0.")]
        private protected virtual void OnXRControllerChanged()
        {
            m_HasXRController = m_Controller != null;
        }

        void WarnMixedInputConfiguration()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (forceDeprecatedInput)
#pragma warning restore CS0618
            {
                const string warning = "The interactor has input properties configured to be used but the interactor is set to read input through the deprecated XR Controller component instead." +
                    " If you want to force the input readers to be used even when an XR Controller component is present, set Input Compatibility Mode to Force Input Readers.";
                foreach (var reader in buttonReaders)
                {
                    if ((reader.inputSourceMode == XRInputButtonReader.InputSourceMode.InputActionReference && (reader.inputActionReferencePerformed != null || reader.inputActionReferenceValue != null)) ||
                        (reader.inputSourceMode != XRInputButtonReader.InputSourceMode.InputActionReference && reader.inputSourceMode != XRInputButtonReader.InputSourceMode.Unused))
                    {
                        Debug.LogWarning(warning, this);
                        return;
                    }
                }

                foreach (var reader in valueReaders)
                {
                    if ((reader.inputSourceMode == XRInputValueReader.InputSourceMode.InputActionReference && reader.inputActionReference != null) ||
                        (reader.inputSourceMode != XRInputValueReader.InputSourceMode.InputActionReference && reader.inputSourceMode != XRInputValueReader.InputSourceMode.Unused))
                    {
                        Debug.LogWarning(warning, this);
                        return;
                    }
                }
            }
        }

        #region API Updater Configuration Validation false positive failure suppression

        [Obsolete("CreateEffectsAudioSource has been deprecated in version 3.0.0.")]
        void CreateEffectsAudioSource()
        {
        }

        [Obsolete("CanPlayHoverAudio has been deprecated in version 3.0.0.")]
        bool CanPlayHoverAudio(IXRHoverInteractable hoveredInteractable)
        {
            return m_AllowHoverAudioWhileSelecting || !IsSelecting(hoveredInteractable);
        }

        [Obsolete("CanPlayHoverHaptics has been deprecated in version 3.0.0.")]
        bool CanPlayHoverHaptics(IXRHoverInteractable hoveredInteractable)
        {
            return m_AllowHoverHapticsWhileSelecting || !IsSelecting(hoveredInteractable);
        }

        [Obsolete("HandleSelecting has been deprecated in version 3.0.0.")]
        void HandleSelecting()
        {
        }

        [Obsolete("HandleDeselecting has been deprecated in version 3.0.0.")]
        void HandleDeselecting()
        {
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