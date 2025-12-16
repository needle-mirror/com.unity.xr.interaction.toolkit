using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump
{
    /// <summary>
    /// Jump Provider allows the player to jump in the scene.
    /// This uses a jump force to drive the value of the <see cref="jumpHeight"/> over time
    /// to allow the player to control how floaty the jump feels.
    /// The player can hold down the jump button to increase the altitude of the jump.
    /// This provider handles coyote time to allow jumping to feel more consistent.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Jump Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_JumpProvider)]
    public class JumpProvider : LocomotionProvider, IGravityController
    {
        [SerializeField]
        [Tooltip("Disable gravity during the jump. This will result in a more floaty jump.")]
        bool m_DisableGravityDuringJump;

        /// <summary>
        /// Disable gravity during the jump. This will result in a more floaty jump.
        /// </summary>
        public bool disableGravityDuringJump
        {
            get => m_DisableGravityDuringJump;
            set => m_DisableGravityDuringJump = value;
        }

        [SerializeField]
        [Tooltip("Allow player to jump without being grounded.")]
        bool m_UnlimitedInAirJumps;

        /// <summary>
        /// Allow player to jump without being grounded.
        /// </summary>
        public bool unlimitedInAirJumps
        {
            get => m_UnlimitedInAirJumps;
            set => m_UnlimitedInAirJumps = value;
        }

        [SerializeField]
        [Tooltip("The number of times a player can jump before landing.")]
        int m_InAirJumpCount = 1;

        /// <summary>
        /// The number of times a player can jump before landing.
        /// </summary>
        public int inAirJumpCount
        {
            get => m_InAirJumpCount;
            set
            {
                m_InAirJumpCount = Mathf.Max(0, value);
                m_CurrentInAirJumpCount = m_InAirJumpCount;
            }
        }

        [SerializeField]
        [Tooltip("The time window after leaving the ground that a jump can still be performed. Sometimes known as coyote time.")]
        float m_JumpForgivenessWindow = 0.25f;

        /// <summary>
        /// The time window after leaving the ground that a jump can still be performed. Sometimes known as coyote time.
        /// </summary>
        public float jumpForgivenessWindow
        {
            get => m_JumpForgivenessWindow;
            set
            {
                m_JumpForgivenessWindow = value;
                m_CurrentJumpForgivenessWindowTime = m_JumpForgivenessWindow;
            }
        }

        [SerializeField]
        [Tooltip("The height (approximately in meters) the player will be when reaching the apex of the jump.")]
        float m_JumpHeight = 1.25f;

        /// <summary>
        /// The height (approximately in meters) the player will be when reaching the apex of the jump.
        /// </summary>
        public float jumpHeight
        {
            get => m_JumpHeight;
            set => m_JumpHeight = value;
        }

        [SerializeField]
        [Tooltip("Allow the player to stop their jump early when input is released before reaching the maximum jump height.")]
        bool m_VariableHeightJump = true;

        /// <summary>
        /// Whether the jump height is based on how long the player continues to hold the jump button.
        /// Enable to allow the player to stop their jump early when input is released before reaching the maximum jump height.
        /// Disable to jump a fixed height.
        /// </summary>
        public bool variableHeightJump
        {
            get => m_VariableHeightJump;
            set => m_VariableHeightJump = value;
        }

        [SerializeField]
        [Tooltip("The minimum amount of time the jump will execute for.")]
        float m_MinJumpHoldTime = 0.1f;

        /// <summary>
        /// The minimum amount of time the jump will execute for.
        /// </summary>
        public float minJumpHoldTime
        {
            get => m_MinJumpHoldTime;
            set => m_MinJumpHoldTime = value;
        }

        [SerializeField]
        [Tooltip("The maximum time a player can hold down the jump button to increase altitude.")]
        float m_MaxJumpHoldTime = 0.5f;

        /// <summary>
        /// The maximum time a player can hold down the jump button to increase altitude.
        /// </summary>
        public float maxJumpHoldTime
        {
            get => m_MaxJumpHoldTime;
            set => m_MaxJumpHoldTime = value;
        }

        [SerializeField]
        [Tooltip("The speed at which the jump will decelerate when the player releases the jump button early.")]
        float m_EarlyOutDecelerationSpeed = .1f;

        /// <summary>
        /// The speed at which the jump will decelerate when the player releases the jump button early.
        /// </summary>
        public float earlyOutDecelerationSpeed
        {
            get => m_EarlyOutDecelerationSpeed;
            set => m_EarlyOutDecelerationSpeed = value;
        }

        [SerializeField]
        [Tooltip("Input data that will be used to perform a jump.")]
        XRInputButtonReader m_JumpInput = new XRInputButtonReader("Jump");

        /// <summary>
        /// Input data that will be used to perform a jump.
        /// If the source is an Input Action, it must have a button-like interaction where phase equals performed when pressed.
        /// Typically a <see cref="ButtonControl"/> Control or a Value type action with a Press interaction.
        /// </summary>
        public XRInputButtonReader jumpInput
        {
            get => m_JumpInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_JumpInput, value, this);
        }

        /// <summary>
        /// The transformation that is used by this component to apply translation movement.
        /// </summary>
        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        bool m_IsJumping;

        /// <summary>
        /// Returns whether the player is currently jumping.
        /// </summary>
        /// <remarks>
        /// This is only <see langword="true"/> during jump ascent, it is not <see langword="true"/> during the descent.
        /// </remarks>
        public bool isJumping => m_IsJumping;

        /// <inheritdoc />
        public bool canProcess => isActiveAndEnabled;

        /// <inheritdoc/>
        public bool gravityPaused { get; protected set; }

        /// <summary>
        /// Flag to make sure the player has released the jump button before allowing another jump.
        /// </summary>
        bool m_HasJumped;

        /// <summary>
        /// The current jump forgiveness time.
        /// </summary>
        float m_CurrentJumpForgivenessWindowTime;

        /// <summary>
        /// The time the player will stop at. Usually it's <see cref="maxJumpHoldTime"/>, but can change when using <see cref="variableHeightJump"/>.
        /// </summary>
        float m_StoppingJumpTime;

        /// <summary>
        /// The current jump force being applied this frame.
        /// </summary>
        float m_CurrentJumpForceThisFrame;

        /// <summary>
        /// Current jump vector being applied to the player.
        /// </summary>
        Vector3 m_JumpVector;

        /// <summary>
        /// Reference to the gravity provider.
        /// </summary>
        GravityProvider m_GravityProvider;

        bool m_HasGravityProvider;

        float m_CurrentJumpTimer;

        int m_CurrentInAirJumpCount;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            m_InAirJumpCount = Mathf.Max(0, m_InAirJumpCount);
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            m_HasGravityProvider = ComponentLocatorUtility<GravityProvider>.TryFindComponent(out m_GravityProvider);
            if (!m_HasGravityProvider)
            {
                Debug.LogError("Could not find Gravity Provider component which is required by the Jump Provider component. Disabling component.", this);
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_JumpInput.EnableDirectActionIfModeUsed();

            m_CurrentInAirJumpCount = m_InAirJumpCount;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            m_JumpInput.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>
        /// </summary>
        protected virtual void Update()
        {
            CheckJump();
        }

        /// <summary>
        /// Checks if the player can jump and updates the jump routine.
        /// </summary>
        void CheckJump()
        {
            if (!m_HasGravityProvider)
                return;

            if (m_CurrentJumpForgivenessWindowTime > 0f)
                m_CurrentJumpForgivenessWindowTime -= Time.deltaTime;

            // If the player has jumped and the jump input is no longer being pressed, reset the jump flag.
            if (m_HasJumped && m_JumpInput.ReadWasCompletedThisFrame())
                m_HasJumped = false;

            if (!m_HasJumped && m_JumpInput.ReadIsPerformed())
                Jump();

            if (m_IsJumping)
                UpdateJump();
        }

        /// <summary>
        /// Initiates the jump routine.
        /// </summary>
        /// <seealso cref="CanJump"/>
        public void Jump()
        {
            if (!CanJump())
                return;

            if (!m_GravityProvider.isGrounded)
                m_CurrentInAirJumpCount--;

            m_HasJumped = true;
            m_IsJumping = true;
            m_CurrentJumpTimer = 0f;
            m_StoppingJumpTime = m_MaxJumpHoldTime;
            m_CurrentJumpForgivenessWindowTime = 0f;
            m_CurrentJumpForceThisFrame = m_JumpHeight;

            if (m_DisableGravityDuringJump)
                TryLockGravity(GravityOverride.ForcedOff);

            m_GravityProvider.ResetFallForce();
        }

        /// <summary>
        /// Determines if the player can jump.
        /// </summary>
        /// <returns>Returns whether the player can jump. The <see cref="Jump"/> method will not do anything while this method returns <see langword="false"/>.</returns>
        /// <remarks>
        /// Returns <see langword="true"/> when any of these conditions are met:<br/>
        /// - The player is grounded or within the jump forgiveness time (coyote time).<br/>
        /// - The player has remaining in-air jumps.<br/>
        /// </remarks>
        /// <seealso cref="Jump"/>
        public bool CanJump()
        {
            return m_UnlimitedInAirJumps || m_CurrentInAirJumpCount > 0 || m_GravityProvider.isGrounded || m_CurrentJumpForgivenessWindowTime > 0f;
        }

        /// <summary>
        /// Called every frame while <see cref="isJumping"/> is true.
        /// </summary>
        void UpdateJump()
        {
            var dt = Time.deltaTime;
            ProcessJumpForce(dt);

            // Calculate the jump vector based on the current gravity mode.
            if (m_GravityProvider.useLocalSpaceGravity)
                m_JumpVector = m_CurrentJumpForceThisFrame * dt * m_GravityProvider.GetCurrentUp();
            else
                m_JumpVector.y = m_CurrentJumpForceThisFrame * dt;

            if (m_IsJumping)
                TryStartLocomotionImmediately();

            if (locomotionState != LocomotionState.Moving)
                return;

            transformation.motion = m_JumpVector;
            TryQueueTransformation(transformation);
        }

        void ProcessJumpForce(float dt)
        {
            m_CurrentJumpTimer += dt;

            if (m_StoppingJumpTime == m_MaxJumpHoldTime &&
                (m_MaxJumpHoldTime <= 0 ||
                (m_VariableHeightJump && m_CurrentJumpTimer > m_MinJumpHoldTime && !m_JumpInput.ReadIsPerformed())))
            {
                m_StoppingJumpTime = Mathf.Min(m_CurrentJumpTimer + m_EarlyOutDecelerationSpeed, m_MaxJumpHoldTime);
            }

            // Calculate the jump force based on the normalized time (0 to 1) of the jump.
            m_CurrentJumpForceThisFrame = CalculateJumpForceForFrame(Mathf.Clamp01(m_CurrentJumpTimer / m_StoppingJumpTime));

            // If the player has reached the maximum jump time, stop the jump.
            if (m_CurrentJumpTimer >= m_StoppingJumpTime)
                StopJump();
        }


        /// <summary>
        /// Calculates the jump force for the current frame based on the normalized time of the current jump.
        /// This function uses an approximation to convert the jump force to meters for a better UX.
        /// </summary>
        /// <param name="normalizedJumpTime">Normalized value between <see cref="m_CurrentJumpTimer"/> and <see cref="m_StoppingJumpTime"/></param>
        /// <returns>The current force to be applied for a jump to reach an approximate height based <see cref="m_JumpHeight"/></returns>
        float CalculateJumpForceForFrame(float normalizedJumpTime)
        {
            // Start and end jump forces for the jump height. This is used to interpolate the jump force based on the current time of the jump.
            var startJumpForce = 7f;
            var endJumpForce = 5f;

            // Any jump height greater than this clamp will use the endJumpForce value.
            var jumpHeightClamp = 4f;

            // Approximate the force to meters conversion based on the jump height.
            var approximateForceToMetersConversion = Mathf.Lerp(startJumpForce, endJumpForce, Mathf.Clamp01(m_JumpHeight / jumpHeightClamp));

            // If gravity is disabled during the jump, reduce the force to meters conversion to keep the approximation within an acceptable threshold.
            if (m_DisableGravityDuringJump)
                approximateForceToMetersConversion /= 1.5f;

            // Calculate the jump force based on the normalized time of the jump.
            return (1 - normalizedJumpTime) * m_JumpHeight * approximateForceToMetersConversion;
        }

        /// <summary>
        /// Stops the jump routine.
        /// </summary>
        void StopJump()
        {
            m_IsJumping = false;
            TryEndLocomotion();
            if (m_DisableGravityDuringJump)
                RemoveGravityLock();
        }

        /// <summary>
        /// Starts the coyote timer.
        /// </summary>
        void StartCoyoteTimer()
        {
            m_CurrentJumpForgivenessWindowTime = m_JumpForgivenessWindow;
        }

        /// <summary>
        /// Whether this <see cref="JumpProvider"/> is currently pausing gravity.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if <see cref="isJumping"/> and <see cref="disableGravityDuringJump"/>
        /// are currently <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </returns>
        public bool IsPausingGravity()
        {
            return m_IsJumping && m_DisableGravityDuringJump;
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
            gravityPaused = false;

            if (!isActiveAndEnabled)
                return;

            if (!isGrounded)
            {
                // If not jumping, enable coyote time.
                if (!m_IsJumping)
                    StartCoyoteTimer();
            }
            else
            {
                m_CurrentJumpForgivenessWindowTime = 0f;

                // Reset the jump vector when the player is grounded.
                m_JumpVector = Vector3.zero;

                m_CurrentInAirJumpCount = m_InAirJumpCount;
                if (m_IsJumping)
                    StopJump();
            }
        }

        /// <summary>
        /// Called from <see cref="GravityProvider.TryLockGravity"/> when gravity lock is changed.
        /// </summary>
        /// <param name="gravityOverride">The <see cref="GravityOverride"/> to apply.</param>
        /// <seealso cref="GravityProvider.onGravityLockChanged"/>
        protected virtual void OnGravityLockChanged(GravityOverride gravityOverride)
        {
            if (gravityOverride == GravityOverride.ForcedOn)
                gravityPaused = false;
        }
    }
}
