using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// <see cref="MonoBehaviour"/> that controls interaction recording and playback (via <see cref="XRControllerRecording"/> assets).
    /// </summary>
    /// <seealso cref="XRControllerRecording"/>
    [AddComponentMenu("XR/Debug/XR Controller Recorder", 11)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_ControllerRecorder)]
    [HelpURL(XRHelpURLConstants.k_XRControllerRecorder)]
    public partial class XRControllerRecorder : MonoBehaviour
    {
        class ButtonBypass : IXRInputButtonReader
        {
            public InteractionState state { get; set; }

            /// <inheritdoc />
            public bool ReadIsPerformed()
            {
                return state.active;
            }

            /// <inheritdoc />
            public bool ReadWasPerformedThisFrame()
            {
                return state.activatedThisFrame;
            }

            /// <inheritdoc />
            public bool ReadWasCompletedThisFrame()
            {
                return state.deactivatedThisFrame;
            }

            /// <inheritdoc />
            public float ReadValue()
            {
                return state.value;
            }

            /// <inheritdoc />
            public bool TryReadValue(out float value)
            {
                value = state.value;
                return true;
            }
        }

        class ValueBypass<TValue> : IXRInputValueReader<TValue> where TValue : struct
        {
            public TValue state { get; set; }

            /// <inheritdoc />
            public TValue ReadValue()
            {
                return state;
            }

            /// <inheritdoc />
            public bool TryReadValue(out TValue value)
            {
                value = state;
                return true;
            }
        }

        [Header("Input Recording/Playback")]

        [SerializeField, Tooltip("Controls whether this recording will start playing when the component's Awake() method is called.")]
        bool m_PlayOnStart;

        /// <summary>
        /// Controls whether this recording will start playing when the component's <see cref="Awake"/> method is called.
        /// </summary>
        public bool playOnStart
        {
            get => m_PlayOnStart;
            set => m_PlayOnStart = value;
        }

        [SerializeField, Tooltip("Controller Recording asset for recording and playback of controller events.")]
        XRControllerRecording m_Recording;

        /// <summary>
        /// Controller Recording asset for recording and playback of controller events.
        /// </summary>
        public XRControllerRecording recording
        {
            get => m_Recording;
            set => m_Recording = value;
        }

        [SerializeField, Tooltip("Interactor whose input will be recorded and played back.")]
        [RequireInterface(typeof(IXRInteractor))]
        Object m_InteractorObject;

        [SerializeField, Tooltip("If true, every frame of the recording must be visited even if a larger time period has passed.")]
        bool m_VisitEachFrame;

        /// <summary>
        /// If <see langword="true"/>, every frame of the recording must be visited even if a larger time period has passed.
        /// </summary>
        public bool visitEachFrame
        {
            get => m_VisitEachFrame;
            set => m_VisitEachFrame = value;
        }

        /// <summary>
        /// Whether the <see cref="XRControllerRecorder"/> is currently recording interaction state.
        /// </summary>
        public bool isRecording
        {
            get => m_IsRecording;
            set
            {
                if (m_IsRecording != value)
                {
                    recordingStartTime = Time.time;
                    isPlaying = false;
                    m_CurrentTime = 0d;
                    if (m_Recording)
                    {
                        if (value)
                            m_Recording.InitRecording();
                        else
                            m_Recording.SaveRecording();
                    }
                    m_IsRecording = value;
                }
            }
        }

        /// <summary>
        /// Whether the XRControllerRecorder is currently playing back interaction state.
        /// </summary>
        public bool isPlaying
        {
            get => m_IsPlaying;
            set
            {
                if (m_IsPlaying != value)
                {
                    isRecording = false;
                    if (m_Recording)
                        ResetPlayback();
                    m_CurrentTime = 0d;
                    m_IsPlaying = value;

                    if (value)
                        StartPlaying();
                    else
                        StopPlaying();
                }
            }
        }

        double m_CurrentTime;

        /// <summary>
        /// (Read Only) The current recording/playback time.
        /// </summary>
        public double currentTime => m_CurrentTime;

        /// <summary>
        /// (Read Only) The total playback time (or 0 if no recording).
        /// </summary>
        public double duration => m_Recording != null ? m_Recording.duration : 0d;

        /// <summary>
        /// The <see cref="Time.time"/> when recording was last started.
        /// </summary>
        protected float recordingStartTime { get; set; }

        readonly UnityObjectReferenceCache<IXRInteractor, Object> m_Interactor = new UnityObjectReferenceCache<IXRInteractor, Object>();

        bool m_IsRecording;
        bool m_IsPlaying;
        double m_LastPlaybackTime;
        int m_LastFrameIdx;

        bool m_PrevEnableInputActions;
        bool m_PrevEnableInputTracking;

        IXRInputButtonReader m_PrevSelectBypass;
        IXRInputButtonReader m_PrevActivateBypass;
        IXRInputButtonReader m_PrevUIPressBypass;
        IXRInputValueReader<Vector2> m_PrevUIScrollBypass;

        readonly ButtonBypass m_SelectBypass = new ButtonBypass();
        readonly ButtonBypass m_ActivateBypass = new ButtonBypass();
        readonly ButtonBypass m_UIPressBypass = new ButtonBypass();
        readonly ValueBypass<Vector2> m_UIScrollBypass = new ValueBypass<Vector2>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
#pragma warning disable CS0618 // Type or member is obsolete -- Find reference for backwards compatibility
            if (m_XRController == null)
                m_XRController = GetComponentInParent<XRBaseController>(true);
#pragma warning restore CS0618

            if (m_InteractorObject == null)
                m_InteractorObject = GetComponentInParent<IXRInteractor>(true) as Object;

            m_CurrentTime = 0d;

            if (m_PlayOnStart)
                isPlaying = true;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            if (isRecording)
            {
                var interactor = GetInteractor();

                XRControllerState controllerState;
                var time = Time.time - recordingStartTime;
#pragma warning disable CS0618 // Type or member is obsolete -- If the controller component is added, use its state for backwards compatibility
                if (m_XRController != null)
                {
                    controllerState = new XRControllerState(m_XRController.currentControllerState);
                }
#pragma warning restore CS0618
                else if (interactor != null)
                {
                    var localPose = interactor.transform.GetLocalPose();
                    controllerState = new XRControllerState
                    {
                        inputTrackingState = InputTrackingState.All,
                        isTracked = true,
                        position = localPose.position,
                        rotation = localPose.rotation,
                    };
                }
                else
                {
                    controllerState = new XRControllerState();
                }

                controllerState.time = time;

                if (interactor != null)
                {
                    if (interactor is XRBaseInputInteractor inputInteractor)
                    {
                        // Select
                        var selectInput = inputInteractor.selectInput;
                        controllerState.selectInteractionState = new InteractionState
                        {
                            value = selectInput.ReadValue(),
                            active = selectInput.ReadIsPerformed(),
                            activatedThisFrame = selectInput.ReadWasPerformedThisFrame(),
                            deactivatedThisFrame = selectInput.ReadWasCompletedThisFrame(),
                        };

                        // Activate
                        var activateInput = inputInteractor.activateInput;
                        controllerState.activateInteractionState = new InteractionState
                        {
                            value = activateInput.ReadValue(),
                            active = activateInput.ReadIsPerformed(),
                            activatedThisFrame = activateInput.ReadWasPerformedThisFrame(),
                            deactivatedThisFrame = activateInput.ReadWasCompletedThisFrame(),
                        };
                    }
                    else
                    {
                        controllerState.selectInteractionState = new InteractionState();
                        controllerState.activateInteractionState = new InteractionState();
                    }

                    if (interactor is XRRayInteractor rayInteractor)
                    {
                        // UI Press
                        var uiPressInput = rayInteractor.uiPressInput;
                        controllerState.uiPressInteractionState = new InteractionState
                        {
                            value = uiPressInput.ReadValue(),
                            active = uiPressInput.ReadIsPerformed(),
                            activatedThisFrame = uiPressInput.ReadWasPerformedThisFrame(),
                            deactivatedThisFrame = uiPressInput.ReadWasCompletedThisFrame(),
                        };

                        // UI Scroll
                        controllerState.uiScrollValue = rayInteractor.uiScrollInput.ReadValue();
                    }
                    else
                    {
                        controllerState.uiPressInteractionState = new InteractionState();
                        controllerState.uiScrollValue = Vector2.zero;
                    }
                }

                m_Recording.AddRecordingFrameNonAlloc(controllerState);
            }
            else if (isPlaying)
            {
                UpdatePlaybackTime(m_CurrentTime);
            }

            if (isRecording || isPlaying)
                m_CurrentTime += Time.deltaTime;
            if (isPlaying && m_CurrentTime > m_Recording.duration && (!m_VisitEachFrame || m_LastFrameIdx >= (m_Recording.frames.Count - 1)))
                isPlaying = false;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            isRecording = false;
            isPlaying = false;
        }

        /// <summary>
        /// Gets the interactor whose input will be recorded and played back.
        /// </summary>
        /// <returns>Returns the interactor whose input will be recorded and played back.</returns>
        /// <seealso cref="SetInteractor"/>
        public IXRInteractor GetInteractor()
        {
            return m_Interactor.Get(m_InteractorObject);
        }

        /// <summary>
        /// Sets the interactor whose input will be recorded and played back.
        /// </summary>
        /// <param name="interactor">The interactor whose input will be recorded and played back.</param>
        /// <remarks>
        /// This also sets the serialized field to the given interactor as a Unity Object.
        /// </remarks>
        /// <seealso cref="GetInteractor"/>
        public void SetInteractor(IXRInteractor interactor)
        {
            m_Interactor.Set(ref m_InteractorObject, interactor);
        }

        /// <summary>
        /// Resets the recorder to the start of the clip.
        /// </summary>
        public void ResetPlayback()
        {
            m_LastPlaybackTime = 0d;
            m_LastFrameIdx = 0;
        }

        /// <summary>
        /// Store the previous state of the object being tracked and setup input bypass.
        /// </summary>
        void StartPlaying()
        {
#pragma warning disable CS0618 // Type or member is obsolete -- Done for backwards compatibility
            if (m_XRController != null)
            {
                m_PrevEnableInputActions = m_XRController.enableInputActions;
                m_PrevEnableInputTracking = m_XRController.enableInputTracking;
                m_XRController.enableInputActions = false;
                m_XRController.enableInputTracking = false;
            }
#pragma warning restore CS0618

            var interactor = GetInteractor();
            if (interactor != null)
            {
                if (interactor is XRBaseInputInteractor inputInteractor)
                {
                    m_PrevSelectBypass = inputInteractor.selectInput.bypass;
                    m_PrevActivateBypass = inputInteractor.activateInput.bypass;

                    inputInteractor.selectInput.bypass = m_SelectBypass;
                    inputInteractor.activateInput.bypass = m_ActivateBypass;
                }
                else
                {
                    m_PrevSelectBypass = null;
                    m_PrevActivateBypass = null;
                }

                if (interactor is XRRayInteractor rayInteractor)
                {
                    m_PrevUIPressBypass = rayInteractor != null ? rayInteractor.uiPressInput.bypass : null;
                    m_PrevUIScrollBypass = rayInteractor != null ? rayInteractor.uiScrollInput.bypass : null;

                    rayInteractor.uiPressInput.bypass = m_UIPressBypass;
                    rayInteractor.uiScrollInput.bypass = m_UIScrollBypass;
                }
                else
                {
                    m_PrevUIPressBypass = null;
                    m_PrevUIScrollBypass = null;
                }
            }
        }

        /// <summary>
        /// Restore the previous state of the object being tracked from before we started playing.
        /// </summary>
        void StopPlaying()
        {
#pragma warning disable CS0618 // Type or member is obsolete -- Done for backwards compatibility
            if (m_XRController != null)
            {
                m_XRController.enableInputActions = m_PrevEnableInputActions;
                m_XRController.enableInputTracking = m_PrevEnableInputTracking;
            }
#pragma warning restore CS0618

            var interactor = GetInteractor();
            if (m_Interactor != null)
            {
                if (interactor is XRBaseInputInteractor inputInteractor)
                {
                    inputInteractor.selectInput.bypass = m_PrevSelectBypass;
                    inputInteractor.activateInput.bypass = m_PrevActivateBypass;
                }

                if (interactor is XRRayInteractor rayInteractor)
                {
                    rayInteractor.uiPressInput.bypass = m_PrevUIPressBypass;
                    rayInteractor.uiScrollInput.bypass = m_PrevUIScrollBypass;
                }
            }
        }

        void UpdatePlaybackTime(double playbackTime)
        {
            if (!m_Recording || m_Recording == null || m_Recording.frames.Count == 0 || m_LastFrameIdx >= m_Recording.frames.Count  )
                return;

            // Look for next frame in order (binary search would be faster but we are only searching from last cached frame index)
            var prevFrame = m_Recording.frames[m_LastFrameIdx];
            var frameIdx = m_LastFrameIdx;
            if (prevFrame.time < playbackTime)
            {
                while (frameIdx < m_Recording.frames.Count &&
                       m_Recording.frames[frameIdx].time >= m_LastPlaybackTime &&
                       m_Recording.frames[frameIdx].time <= playbackTime)
                {
                    ++frameIdx;
                    if (m_VisitEachFrame)
                    {
                        if (frameIdx < m_Recording.frames.Count)
                            playbackTime = m_Recording.frames[frameIdx].time;

                        break;
                    }

                }
            }

            // Past last frame or on the same frame, don't do anything
            if (frameIdx >= m_Recording.frames.Count)
                return;

            var recordingFrame = m_Recording.frames[frameIdx];

#pragma warning disable CS0618 // Type or member is obsolete -- If the controller component is added, set its state for backwards compatibility
            if (m_XRController != null)
                m_XRController.currentControllerState = recordingFrame;
#pragma warning restore CS0618

            var interactor = GetInteractor();
            if (interactor != null)
            {
                m_SelectBypass.state = recordingFrame.selectInteractionState;
                m_ActivateBypass.state = recordingFrame.activateInteractionState;
                m_UIPressBypass.state = recordingFrame.uiPressInteractionState;
                m_UIScrollBypass.state = recordingFrame.uiScrollValue;

                // Drive the Transform directly if we're not using the deprecated XR Controller
#pragma warning disable CS0618 // Type or member is obsolete
                if (m_XRController == null)
#pragma warning restore CS0618
                {
                    var interactorTransform = interactor.transform;
                    var hasPosition = (recordingFrame.inputTrackingState & InputTrackingState.Position) != 0;
                    var hasRotation = (recordingFrame.inputTrackingState & InputTrackingState.Rotation) != 0;
                    if (hasPosition && hasRotation)
                        interactorTransform.SetLocalPose(new Pose(recordingFrame.position, recordingFrame.rotation));
                    else if (hasPosition)
                        interactorTransform.localPosition = recordingFrame.position;
                    else if (hasRotation)
                        interactorTransform.localRotation = recordingFrame.rotation;
                }
            }

            m_LastFrameIdx = frameIdx;
            m_LastPlaybackTime = playbackTime;
        }

        /// <summary>
        /// Gets the state of the controller while playing or recording.
        /// </summary>
        /// <param name="controllerState">When this method returns, contains the <see cref="XRControllerState"/> object representing the state of the controller.</param>
        /// <returns>Returns <see langword="true"/> when playing or recording. Otherwise, returns <see langword="false"/>.</returns>
        public virtual bool GetControllerState(out XRControllerState controllerState)
        {
            if (isPlaying)
            {
                // Return the current frame we're playing back
                if (m_Recording.frames.Count > m_LastFrameIdx)
                {
                    controllerState = m_Recording.frames[m_LastFrameIdx];
                    return true;
                }
            }
            else if (isRecording)
            {
                // Relay the last frame we got
                if (m_Recording.frames.Count > 0)
                {
                    controllerState = m_Recording.frames[m_Recording.frames.Count - 1];
                    return true;
                }
            }

            controllerState = null;
            return false;
        }
    }
}
