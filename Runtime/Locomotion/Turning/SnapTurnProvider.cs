using System;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning
{
    /// <summary>
    /// Locomotion provider that allows the user to rotate their rig in fixed angle increments
    /// based on read input values, such as from the controller thumbstick.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="ContinuousTurnProvider"/>
    [AddComponentMenu("XR/Locomotion/Snap Turn Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_SnapTurnProvider)]
    public class SnapTurnProvider : LocomotionProvider
    {
        [SerializeField]
        [Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
        float m_TurnAmount = 45f;
        /// <summary>
        /// The number of degrees clockwise Unity rotates the rig when snap turning clockwise.
        /// </summary>
        public float turnAmount
        {
            get => m_TurnAmount;
            set => m_TurnAmount = value;
        }

        [SerializeField]
        [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
        float m_DebounceTime = 0.5f;
        /// <summary>
        /// The amount of time that Unity waits before starting another snap turn.
        /// </summary>
        public float debounceTime
        {
            get => m_DebounceTime;
            set => m_DebounceTime = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable left & right snap turns.")]
        bool m_EnableTurnLeftRight = true;
        /// <summary>
        /// Controls whether to enable left and right snap turns.
        /// </summary>
        /// <seealso cref="enableTurnAround"/>
        public bool enableTurnLeftRight
        {
            get => m_EnableTurnLeftRight;
            set => m_EnableTurnLeftRight = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable 180° snap turns.")]
        bool m_EnableTurnAround = true;
        /// <summary>
        /// Controls whether to enable 180° snap turns.
        /// </summary>
        /// <seealso cref="enableTurnLeftRight"/>
        public bool enableTurnAround
        {
            get => m_EnableTurnAround;
            set => m_EnableTurnAround = value;
        }

        [SerializeField]
        [Tooltip("The time (in seconds) to delay the first turn after receiving initial input for the turn.")]
        float m_DelayTime;

        /// <summary>
        /// The time (in seconds) to delay the first turn after receiving initial input for the turn.
        /// Subsequent turns while holding down input are delayed by the <see cref="debounceTime"/>, not the delay time.
        /// This delay can be used, for example, as time to set a tunneling vignette effect as a VR comfort option.
        /// </summary>
        public float delayTime
        {
            get => m_DelayTime;
            set => m_DelayTime = value;
        }

        /// <inheritdoc/>
        public override bool canStartMoving => m_DelayTime <= 0f || Time.time - m_DelayStartTime >= m_DelayTime;

        /// <summary>
        /// The transformation that is used by this component to apply turn movement.
        /// </summary>
        public XRBodyYawRotation transformation { get; set; } = new XRBodyYawRotation();

        [SerializeField]
        [Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_LeftHandTurnInput = new XRInputValueReader<Vector2>("Left Hand Snap Turn");

        /// <summary>
        /// Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> leftHandTurnInput
        {
            get => m_LeftHandTurnInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_LeftHandTurnInput, value, this);
        }

        [SerializeField]
        [Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_RightHandTurnInput = new XRInputValueReader<Vector2>("Right Hand Snap Turn");

        /// <summary>
        /// Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> rightHandTurnInput
        {
            get => m_RightHandTurnInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_RightHandTurnInput, value, this);
        }

        float m_CurrentTurnAmount;
        float m_TimeStarted;
        float m_DelayStartTime;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_LeftHandTurnInput.EnableDirectActionIfModeUsed();
            m_RightHandTurnInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_LeftHandTurnInput.DisableDirectActionIfModeUsed();
            m_RightHandTurnInput.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            // Wait for a certain amount of time before allowing another turn.
            if (m_TimeStarted > 0f && (m_TimeStarted + m_DebounceTime < Time.time))
            {
                m_TimeStarted = 0f;
                return;
            }

            var input = ReadInput();
            var amount = GetTurnAmount(input);
            if (Mathf.Abs(amount) > 0f)
            {
                StartTurn(amount);
            }
            else if (Mathf.Approximately(m_CurrentTurnAmount, 0f) && locomotionState == LocomotionState.Moving)
            {
                TryEndLocomotion();
            }

            if (locomotionState == LocomotionState.Moving && math.abs(m_CurrentTurnAmount) > 0f)
            {
                m_TimeStarted = Time.time;
                transformation.angleDelta = m_CurrentTurnAmount;
                TryQueueTransformation(transformation);
                m_CurrentTurnAmount = 0f;

                if (Mathf.Approximately(amount, 0f))
                    TryEndLocomotion();
            }
        }

        Vector2 ReadInput()
        {
            var leftHandValue = m_LeftHandTurnInput.ReadValue();
            var rightHandValue = m_RightHandTurnInput.ReadValue();

            return leftHandValue + rightHandValue;
        }

        /// <summary>
        /// Determines the turn amount in degrees for the given <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns the turn amount in degrees for the given <paramref name="input"/> vector.</returns>
        protected virtual float GetTurnAmount(Vector2 input)
        {
            if (input == Vector2.zero)
                return 0f;

            var cardinal = CardinalUtility.GetNearestCardinal(input);
            switch (cardinal)
            {
                case Cardinal.North:
                    break;
                case Cardinal.South:
                    if (m_EnableTurnAround)
                        return 180f;
                    break;
                case Cardinal.East:
                    if (m_EnableTurnLeftRight)
                        return m_TurnAmount;
                    break;
                case Cardinal.West:
                    if (m_EnableTurnLeftRight)
                        return -m_TurnAmount;
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(Cardinal)}={cardinal}");
                    break;
            }

            return 0f;
        }

        /// <summary>
        /// Begins turning locomotion.
        /// </summary>
        /// <param name="amount">Amount to turn.</param>
        protected void StartTurn(float amount)
        {
            if (m_TimeStarted > 0f)
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

            // We set the m_CurrentTurnAmount here so we can still trigger the turn
            // in the case where the input is released before the delay timeout happens.
            if (math.abs(amount) > 0f)
                m_CurrentTurnAmount = amount;
        }
    }
}
