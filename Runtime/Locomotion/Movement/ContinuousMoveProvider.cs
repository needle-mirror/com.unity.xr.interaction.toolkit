using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly move their rig continuously over time
    /// based on read input values, such as from the controller thumbstick.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="ContinuousTurnProvider"/>
    [AddComponentMenu("XR/Locomotion/Continuous Move Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_ContinuousMoveProvider)]
    public partial class ContinuousMoveProvider : LocomotionProvider, IGravityController
    {
        [SerializeField]
        [Tooltip("The speed, in units per second, to move forward.")]
        float m_MoveSpeed = 1f;
        /// <summary>
        /// The speed, in units per second, to move forward.
        /// </summary>
        public float moveSpeed
        {
            get => m_MoveSpeed;
            set => m_MoveSpeed = value;
        }

        [SerializeField]
        [Tooltip("Determines how much control the player has while in the air (0 = no control, 1 = full control).")]
        float m_InAirControlModifier = 0.5f;
        /// <summary>
        /// Determines how much control the player has while in the air (0 = no control, 1 = full control).
        /// </summary>
        public float inAirControlModifier
        {
            get => m_InAirControlModifier;
            set => m_InAirControlModifier = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable strafing (sideways movement).")]
        bool m_EnableStrafe = true;
        /// <summary>
        /// Controls whether to enable strafing (sideways movement).
        /// </summary>
        public bool enableStrafe
        {
            get => m_EnableStrafe;
            set => m_EnableStrafe = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to enable flying (unconstrained movement). This overrides the use of gravity.")]
        bool m_EnableFly;
        /// <summary>
        /// Controls whether to enable flying (unconstrained movement). This overrides the use of gravity.
        /// </summary>
        public bool enableFly
        {
            get => m_EnableFly;
            set => m_EnableFly = value;
        }

        [SerializeField]
        [Tooltip("The source Transform to define the forward direction.")]
        Transform m_ForwardSource;
        /// <summary>
        /// The source <see cref="Transform"/> that defines the forward direction.
        /// </summary>
        public Transform forwardSource
        {
            get => m_ForwardSource;
            set => m_ForwardSource = value;
        }

        /// <summary>
        /// The transformation that is used by this component to apply translation movement.
        /// </summary>
        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        [SerializeField]
        [Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_LeftHandMoveInput = new XRInputValueReader<Vector2>("Left Hand Move");

        /// <summary>
        /// Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> leftHandMoveInput
        {
            get => m_LeftHandMoveInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_LeftHandMoveInput, value, this);
        }

        [SerializeField]
        [Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
        XRInputValueReader<Vector2> m_RightHandMoveInput = new XRInputValueReader<Vector2>("Right Hand Move");

        /// <summary>
        /// Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).
        /// </summary>
        public XRInputValueReader<Vector2> rightHandMoveInput
        {
            get => m_RightHandMoveInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_RightHandMoveInput, value, this);
        }

        /// <inheritdoc />
        public bool canProcess => isActiveAndEnabled;

        /// <inheritdoc />
        public bool gravityPaused => m_EnableFly;

        GravityProvider m_GravityProvider;

        CharacterController m_CharacterController;

        bool m_AttemptedGetCharacterController;

        bool m_IsMovingXROrigin;

        Vector3 m_GravityDrivenVelocity;
        Vector3 m_InAirVelocity;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            if (ComponentLocatorUtility<GravityProvider>.TryFindComponent(out m_GravityProvider))
            {
#pragma warning disable CS0618 // Type or member is obsolete -- Assist with migration when this component wants gravity disabled
                if (!m_UseGravity)
                    MigrateUseGravityToGravityProvider();
#pragma warning restore CS0618
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_LeftHandMoveInput.EnableDirectActionIfModeUsed();
            m_RightHandMoveInput.EnableDirectActionIfModeUsed();

            m_GravityDrivenVelocity = Vector3.zero;
            m_InAirVelocity = Vector3.zero;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_LeftHandMoveInput.DisableDirectActionIfModeUsed();
            m_RightHandMoveInput.DisableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        protected override void OnLocomotionStateChanging(LocomotionState state)
        {
            base.OnLocomotionStateChanging(state);

            if (state == LocomotionState.Moving)
                TryLockGravity(m_EnableFly ? GravityOverride.ForcedOff : GravityOverride.ForcedOn);
            else if (state == LocomotionState.Ended)
                RemoveGravityLock();
        }

        /// <inheritdoc />
        protected override void OnLocomotionStarting()
        {
            base.OnLocomotionStarting();
        }

        /// <inheritdoc />
        protected override void OnLocomotionEnding()
        {
            base.OnLocomotionEnding();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            m_IsMovingXROrigin = false;

            var xrOrigin = mediator.xrOrigin?.Origin;
            if (xrOrigin == null)
                return;

            var input = ReadInput();
            var translationInWorldSpace = ComputeDesiredMove(input);

            if (input != Vector2.zero || m_GravityDrivenVelocity != Vector3.zero || m_InAirVelocity != Vector3.zero)
                MoveRig(translationInWorldSpace);

            if (!m_IsMovingXROrigin)
                TryEndLocomotion();
        }

        Vector2 ReadInput()
        {
            var leftHandValue = m_LeftHandMoveInput.ReadValue();
            var rightHandValue = m_RightHandMoveInput.ReadValue();

            return leftHandValue + rightHandValue;
        }

        /// <summary>
        /// Determines how much to slide the rig due to <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns the translation amount in world space to move the rig.</returns>
        protected virtual Vector3 ComputeDesiredMove(Vector2 input)
        {
            if (input == Vector2.zero && m_InAirVelocity == Vector3.zero)
                return Vector3.zero;

            var xrOrigin = mediator.xrOrigin;
            if (xrOrigin == null)
                return Vector3.zero;

            // Assumes that the input axes are in the range [-1, 1].
            // Clamps the magnitude of the input direction to prevent faster speed when moving diagonally,
            // while still allowing for analog input to move slower (which would be lost if simply normalizing).
            var inputMove = Vector3.ClampMagnitude(new Vector3(m_EnableStrafe ? input.x : 0f, 0f, input.y), 1f);

            var deltaTime = Time.deltaTime;

            // Check if the user is not in the air, and update the input velocity accordingly.
            // If the user is in the air, update the input velocity with the inAirControlModifier.
            if (m_GravityProvider == null || !m_GravityProvider.enabled || !m_GravityProvider.useGravity || m_GravityProvider.isGrounded)
                m_InAirVelocity = inputMove;
            else
                m_InAirVelocity += deltaTime * m_InAirControlModifier * 10 * (inputMove - m_InAirVelocity);

            // Determine frame of reference for what the input direction is relative to
            var forwardSourceTransform = m_ForwardSource == null ? xrOrigin.Camera.transform : m_ForwardSource;
            var inputForwardInWorldSpace = forwardSourceTransform.forward;

            var originTransform = xrOrigin.Origin.transform;
            var speedFactor = m_MoveSpeed * deltaTime * originTransform.localScale.x; // Adjust speed with user scale

            // If flying, just compute move directly from input and forward source
            if (m_EnableFly)
            {
                var inputRightInWorldSpace = forwardSourceTransform.right;
                var combinedMove = inputMove.x * inputRightInWorldSpace + inputMove.z * inputForwardInWorldSpace;
                return combinedMove * speedFactor;
            }

            var originUp = originTransform.up;

            if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(inputForwardInWorldSpace, originUp)), 1f))
            {
                // When the input forward direction is parallel with the rig normal,
                // it will probably feel better for the player to move along the same direction
                // as if they tilted forward or up some rather than moving in the rig forward direction.
                // It also will probably be a better experience to at least move in a direction
                // rather than stopping if the head/controller is oriented such that it is perpendicular with the rig.
                inputForwardInWorldSpace = -forwardSourceTransform.up;
            }

            var inputForwardProjectedInWorldSpace = Vector3.ProjectOnPlane(inputForwardInWorldSpace, originUp);
            var forwardRotation = Quaternion.FromToRotation(originTransform.forward, inputForwardProjectedInWorldSpace);

            var translationInRigSpace = forwardRotation * m_InAirVelocity * speedFactor;
            var translationInWorldSpace = originTransform.TransformDirection(translationInRigSpace);

            return translationInWorldSpace;
        }

        /// <summary>
        /// Creates a locomotion event to move the rig by <paramref name="translationInWorldSpace"/>,
        /// and optionally applies gravity.
        /// </summary>
        /// <param name="translationInWorldSpace">The translation amount in world space to move the rig (pre-gravity).</param>
        protected virtual void MoveRig(Vector3 translationInWorldSpace)
        {
            var xrOrigin = mediator.xrOrigin?.Origin;
            if (xrOrigin == null)
                return;

            FindCharacterController();

            var motion = translationInWorldSpace;

            // Step vertical velocity from gravity, but only if gravity is not being controlled by Gravity Provider
            if (m_GravityProvider == null && m_CharacterController != null && m_CharacterController.enabled)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (m_CharacterController.isGrounded || !m_UseGravity || m_EnableFly)
                    m_GravityDrivenVelocity = Vector3.zero;
                else
                    m_GravityDrivenVelocity += Physics.gravity * Time.deltaTime;

                motion += m_GravityDrivenVelocity * Time.deltaTime;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            TryStartLocomotionImmediately();
            if (locomotionState != LocomotionState.Moving)
                return;

            // Note that calling Move even with Vector3.zero will have an effect by causing isGrounded to update
            m_IsMovingXROrigin = true;
            transformation.motion = motion;
            TryQueueTransformation(transformation);
        }

        void FindCharacterController()
        {
            var xrOrigin = mediator.xrOrigin?.Origin;
            if (xrOrigin == null)
                return;

            // Save a reference to the optional CharacterController on the rig GameObject
            // that will be used to move instead of modifying the Transform directly.
            if (m_CharacterController == null && !m_AttemptedGetCharacterController)
            {
                // Try on the Origin GameObject first, and then fallback to the XR Origin GameObject (if different)
                if (!xrOrigin.TryGetComponent(out m_CharacterController) && xrOrigin != mediator.xrOrigin.gameObject)
                    mediator.xrOrigin.TryGetComponent(out m_CharacterController);

                m_AttemptedGetCharacterController = true;
            }
        }


        /// <inheritdoc/>
        public bool TryLockGravity(GravityOverride gravityOverride)
        {
            return m_GravityProvider != null && m_GravityProvider.TryLockGravity(this, gravityOverride);
        }

        /// <inheritdoc/>
        public void RemoveGravityLock()
        {
            if (m_GravityProvider != null)
                m_GravityProvider.UnlockGravity(this);
        }

        /// <inheritdoc />
        void IGravityController.OnGroundedChanged(bool isGrounded) => OnGroundedChanged(isGrounded);

        /// <inheritdoc />
        void IGravityController.OnGravityLockChanged(GravityOverride gravityOverride) => OnGravityLockChanged(gravityOverride);

        /// <summary>
        /// Called from <see cref="GravityProvider"/> when the grounded state changes.
        /// </summary>
        /// <param name="isGrounded">Whether the player is on the ground.</param>
        /// <seealso cref="GravityProvider.onGroundedChanged"/>
        protected virtual void OnGroundedChanged(bool isGrounded)
        {
        }

        /// <summary>
        /// Called from <see cref="GravityProvider.TryLockGravity"/> when gravity lock is changed.
        /// </summary>
        /// <param name="gravityOverride">The <see cref="GravityOverride"/> to apply.</param>
        /// <seealso cref="GravityProvider.onGravityLockChanged"/>
        protected virtual void OnGravityLockChanged(GravityOverride gravityOverride)
        {
        }
    }
}
