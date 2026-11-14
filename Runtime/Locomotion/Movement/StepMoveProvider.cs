using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    /// <summary>
    /// Locomotion provider that allows the user to move their rig in fixed distance steps
    /// based on read input values, such as from hand microgesture swipes or the controller thumbstick.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="ContinuousMoveProvider"/>
    /// <seealso cref="Turning.SnapTurnProvider"/>
    [AddComponentMenu("XR/Locomotion/Step Move Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_StepMoveProvider)]
    public class StepMoveProvider : LocomotionProvider
    {
        [SerializeField]
        [Tooltip("The distance (in meters) to move the rig with each step.")]
        float m_StepDistance = 0.5f;
        /// <summary>
        /// The distance (in meters) Unity moves the rig with each step.
        /// </summary>
        public float stepDistance
        {
            get => m_StepDistance;
            set => m_StepDistance = value;
        }

        [SerializeField]
        [Tooltip("The amount of time that the system will wait before starting another step.")]
        float m_DebounceTime = 0.5f;
        /// <summary>
        /// The amount of time that Unity waits before starting another step.
        /// </summary>
        public float debounceTime
        {
            get => m_DebounceTime;
            set => m_DebounceTime = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable left & right steps.")]
        bool m_EnableStrafe = true;
        /// <summary>
        /// Controls whether to enable left and right steps.
        /// </summary>
        /// <seealso cref="enableStepBackward"/>
        public bool enableStrafe
        {
            get => m_EnableStrafe;
            set => m_EnableStrafe = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable backward steps.")]
        bool m_EnableStepBackward = true;
        /// <summary>
        /// Controls whether to enable backward steps.
        /// </summary>
        /// <seealso cref="enableStrafe"/>
        public bool enableStepBackward
        {
            get => m_EnableStepBackward;
            set => m_EnableStepBackward = value;
        }

        [SerializeField]
        [Tooltip("The time (in seconds) to delay the first step after receiving initial input to start stepping.")]
        float m_DelayTime;
        /// <summary>
        /// The time (in seconds) to delay the first step after receiving initial input to start stepping.
        /// Subsequent steps while holding down input are delayed by the <see cref="debounceTime"/>, not the delay time.
        /// This delay can be used, for example, as time to set a tunneling vignette effect as a VR comfort option.
        /// </summary>
        public float delayTime
        {
            get => m_DelayTime;
            set => m_DelayTime = value;
        }

        [SerializeField]
        [Tooltip("The source Transform to define the forward direction. If not set, the XR Origin camera is used.")]
        Transform m_ForwardSource;
        /// <summary>
        /// The source <see cref="Transform"/> that defines the forward direction.
        /// If not set, Unity uses the camera of the XR Origin.
        /// </summary>
        public Transform forwardSource
        {
            get => m_ForwardSource;
            set => m_ForwardSource = value;
        }

        /// <inheritdoc/>
        public override bool canStartMoving => m_DelayTime <= 0f || Time.time - m_DelayStartTime >= m_DelayTime;

        /// <summary>
        /// The transformation that is used by this component to apply step movement.
        /// </summary>
        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        [SerializeField]
        [Tooltip("Reads input data from the left hand. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_LeftHandStepInput = new XRInputValueReader<Vector2>("Left Hand Step");

        /// <summary>
        /// Reads input data from the left hand. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> leftHandStepInput
        {
            get => m_LeftHandStepInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_LeftHandStepInput, value, this);
        }

        [SerializeField]
        [Tooltip("Reads input data from the right hand. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_RightHandStepInput = new XRInputValueReader<Vector2>("Right Hand Step");

        /// <summary>
        /// Reads input data from the right hand. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> rightHandStepInput
        {
            get => m_RightHandStepInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_RightHandStepInput, value, this);
        }

        Vector2 m_CurrentStepDirection;
        float m_NextStepMinimumStartTime;
        float m_DelayStartTime;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_LeftHandStepInput.EnableDirectActionIfModeUsed();
            m_RightHandStepInput.EnableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            m_LeftHandStepInput.DisableDirectActionIfModeUsed();
            m_RightHandStepInput.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            // Wait for a certain amount of time before allowing another step.
            if (m_NextStepMinimumStartTime > 0f && m_NextStepMinimumStartTime < Time.time)
            {
                m_NextStepMinimumStartTime = 0f;
                return;
            }

            var direction = GetStepDirection(ReadInput());
            if (direction != Vector2.zero)
            {
                StartStep(direction);
            }
            else if (m_CurrentStepDirection == Vector2.zero && locomotionState == LocomotionState.Moving)
            {
                TryEndLocomotion();
            }

            if (locomotionState == LocomotionState.Moving && m_CurrentStepDirection != Vector2.zero)
            {
                m_NextStepMinimumStartTime = Time.time + m_DebounceTime;
                transformation.motion = ComputeStepMotion(m_CurrentStepDirection);
                TryQueueTransformation(transformation);
                m_CurrentStepDirection = Vector2.zero;

                if (direction == Vector2.zero)
                    TryEndLocomotion();
            }
        }

        Vector2 ReadInput()
        {
            var leftHandValue = m_LeftHandStepInput.ReadValue();
            var rightHandValue = m_RightHandStepInput.ReadValue();

            return leftHandValue + rightHandValue;
        }

        /// <summary>
        /// Determines the direction of the step based on the input vector.
        /// </summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns the step direction in rig space, where <c>x</c> is right and <c>y</c> is forward,
        /// or <see cref="Vector2.zero"/> when there is no step.</returns>
        protected virtual Vector2 GetStepDirection(Vector2 input)
        {
            if (input == Vector2.zero)
                return Vector2.zero;

            switch (CardinalUtility.GetNearestCardinal(input))
            {
                case Cardinal.North:
                    return Vector2.up;
                case Cardinal.South:
                    return m_EnableStepBackward ? Vector2.down : Vector2.zero;
                case Cardinal.East:
                    return m_EnableStrafe ? Vector2.right : Vector2.zero;
                case Cardinal.West:
                    return m_EnableStrafe ? Vector2.left : Vector2.zero;
                default:
                    return Vector2.zero;
            }
        }

        /// <summary>
        /// Determines the translation in world space for a single step.
        /// </summary>
        /// <param name="direction">The step direction in rig space, where <c>x</c> is right and <c>y</c> is forward.</param>
        /// <returns>Returns the translation amount in world space to move the rig.</returns>
        protected virtual Vector3 ComputeStepMotion(Vector2 direction)
        {
            var xrOrigin = mediator.xrOrigin;
            if (xrOrigin == null)
                return Vector3.zero;

            var forwardSourceTransform = m_ForwardSource == null ? xrOrigin.Camera.transform : m_ForwardSource;
            var originTransform = xrOrigin.Origin.transform;
            var originUp = originTransform.up;

            var forward = forwardSourceTransform.forward;
            if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(forward, originUp)), 1f))
            {
                // When the forward source is parallel with the rig up (looking straight up or down), the projection
                // is degenerate, so fall back to the opposite of the forward source's up direction, matching ContinuousMoveProvider.
                forward = -forwardSourceTransform.up;
            }

            forward = Vector3.ProjectOnPlane(forward, originUp).normalized;
            var right = Vector3.Cross(originUp, forward);

            return (forward * direction.y + right * direction.x) * (m_StepDistance * originTransform.localScale.x);
        }

        void StartStep(Vector2 direction)
        {
            if (m_NextStepMinimumStartTime > 0f)
                return;

            if (locomotionState == LocomotionState.Idle)
            {
                if (m_DelayTime > 0f)
                {
                    if (TryPrepareLocomotion())
                        m_DelayStartTime = Time.time;
                }
                else
                {
                    TryStartLocomotionImmediately();
                }
            }

            // Set the direction here so the step still happens
            // in the case where the input is released before the delay timeout happens.
            m_CurrentStepDirection = direction;
        }
    }
}
