using System;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly rotate their rig continuously over time
    /// based on read input values, such as from the controller thumbstick.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="ContinuousMoveProvider"/>
    /// <seealso cref="SnapTurnProvider"/>
    [AddComponentMenu("XR/Locomotion/Continuous Turn Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_ContinuousTurnProvider)]
    public class ContinuousTurnProvider : LocomotionProvider
    {
        [SerializeField]
        [Tooltip("The number of degrees/second clockwise to rotate when turning clockwise.")]
        float m_TurnSpeed = 60f;
        /// <summary>
        /// The number of degrees/second clockwise to rotate when turning clockwise.
        /// </summary>
        public float turnSpeed
        {
            get => m_TurnSpeed;
            set => m_TurnSpeed = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable left & right continuous turns.")]
        bool m_EnableTurnLeftRight = true;
        /// <summary>
        /// Controls whether to enable left and right continuous turns.
        /// </summary>
        public bool enableTurnLeftRight
        {
            get => m_EnableTurnLeftRight;
            set => m_EnableTurnLeftRight = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable 180° snap turns on the South direction.")]
        bool m_EnableTurnAround;
        /// <summary>
        /// Controls whether to enable 180° snap turns on the South direction.
        /// </summary>
        public bool enableTurnAround
        {
            get => m_EnableTurnAround;
            set => m_EnableTurnAround = value;
        }

        [SerializeField]
        [Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_LeftHandTurnInput = new XRInputValueReader<Vector2>("Left Hand Turn");

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
        XRInputValueReader<Vector2> m_RightHandTurnInput = new XRInputValueReader<Vector2>("Right Hand Turn");

        /// <summary>
        /// Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> rightHandTurnInput
        {
            get => m_RightHandTurnInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_RightHandTurnInput, value, this);
        }

        /// <summary>
        /// The transformation that is used by this component to apply turn movement.
        /// </summary>
        public XRBodyYawRotation transformation { get; set; } = new XRBodyYawRotation();

        bool m_IsTurningXROrigin;
        bool m_TurnAroundActivated;

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
            m_IsTurningXROrigin = false;

            // Use the input amount to scale the turn speed.
            var input = ReadInput();
            var turnAmount = GetTurnAmount(input);

            TurnRig(turnAmount);

            if (!m_IsTurningXROrigin)
                TryEndLocomotion();

            if (input == Vector2.zero)
                m_TurnAroundActivated = false;
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
            var cardinal = CardinalUtility.GetNearestCardinal(input);
            switch (cardinal)
            {
                case Cardinal.North:
                    break;
                case Cardinal.South:
                    if (m_EnableTurnAround && !m_TurnAroundActivated)
                        return 180f;
                    break;
                case Cardinal.East:
                case Cardinal.West:
                    if (m_EnableTurnLeftRight)
                        return input.magnitude * (Mathf.Sign(input.x) * m_TurnSpeed * Time.deltaTime);
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(Cardinal)}={cardinal}");
                    break;
            }

            return 0f;
        }

        /// <summary>
        /// Rotates the rig by <paramref name="turnAmount"/> degrees.
        /// </summary>
        /// <param name="turnAmount">The amount of rotation in degrees.</param>
        protected void TurnRig(float turnAmount)
        {
            if (Mathf.Approximately(turnAmount, 0f))
                return;

            if (Mathf.Approximately(turnAmount, 180f))
                m_TurnAroundActivated = true;

            TryStartLocomotionImmediately();

            if (locomotionState != LocomotionState.Moving)
                return;

            m_IsTurningXROrigin = true;
            transformation.angleDelta = turnAmount;
            TryQueueTransformation(transformation);
        }
    }
}
