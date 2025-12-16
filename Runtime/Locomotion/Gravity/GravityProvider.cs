using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity
{
    /// <summary>
    /// Locomotion provider that provides gravity to the player.
    /// This provider will also check if the player is grounded using a sphere cast from the player's head.
    /// Gravity will be applied to the player anytime <see cref="useGravity"/> is true and <see cref="isGrounded"/> is false.
    /// See <see cref="IGravityController"/> for external control over gravity.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Gravity Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_GravityProvider)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_GravityProvider)]
    public class GravityProvider : LocomotionProvider
    {
        [SerializeField]
        [Tooltip("Apply gravity to the XR Origin.")]
        bool m_UseGravity = true;

        /// <summary>
        /// Apply gravity to the XR Origin.
        /// </summary>
        public bool useGravity
        {
            get => m_UseGravity;
            set => m_UseGravity = value;
        }

        [SerializeField]
        [Tooltip("Apply gravity based on the current Up vector of the XR Origin.")]
        bool m_UseLocalSpaceGravity = true;

        /// <summary>
        /// Apply gravity based on the current Up vector of the XR Origin.
        /// </summary>
        public bool useLocalSpaceGravity
        {
            get => m_UseLocalSpaceGravity;
            set => m_UseLocalSpaceGravity = value;
        }

        [SerializeField]
        [Tooltip("Determines the maximum fall speed based on units per second.")]
        float m_TerminalVelocity = 90f;

        /// <summary>
        /// Determines the maximum fall speed based on units per second.
        /// </summary>
        public float terminalVelocity
        {
            get => m_TerminalVelocity;
            set => m_TerminalVelocity = value;
        }

        [SerializeField]
        [Tooltip("Determines the speed at which a player reaches max gravity velocity.")]
        float m_GravityAccelerationModifier = 1f;

        /// <summary>
        /// Determines the speed at which a player reaches max gravity velocity.
        /// </summary>
        public float gravityAccelerationModifier
        {
            get => m_GravityAccelerationModifier;
            set => m_GravityAccelerationModifier = value;
        }

        [SerializeField]
        [Tooltip("Sets the center of the character controller to match the local x and z positions of the player camera.")]
        bool m_UpdateCharacterControllerCenterEachFrame = true;

        /// <summary>
        /// Sets the center of the character controller to match the local x and z positions of the player camera.
        /// </summary>
        public bool updateCharacterControllerCenterEachFrame
        {
            get => m_UpdateCharacterControllerCenterEachFrame;
            set => m_UpdateCharacterControllerCenterEachFrame = value;
        }

        [SerializeField]
        [Tooltip("Buffer for the radius of the sphere cast used to check if the player is grounded.")]
        float m_SphereCastRadius = 0.09f;

        /// <summary>
        /// Buffer for the radius of the sphere cast used to check if the player is grounded.
        /// </summary>
        /// <remarks>
        /// Unity automatically multiplies this value by the XR Origin scale.
        /// </remarks>
        public float sphereCastRadius
        {
            get => m_SphereCastRadius;
            set => m_SphereCastRadius = value;
        }

        [SerializeField]
        [Tooltip("Buffer for the distance of the sphere cast used to check if the player is grounded.")]
        float m_SphereCastDistanceBuffer = -0.05f;

        /// <summary>
        /// Buffer for the distance of the sphere cast used to check if the player is grounded.
        /// </summary>
        /// <remarks>
        /// Unity automatically multiplies this value by the XR Origin scale.
        /// </remarks>
        public float sphereCastDistanceBuffer
        {
            get => m_SphereCastDistanceBuffer;
            set => m_SphereCastDistanceBuffer = value;
        }

        [SerializeField]
        [Tooltip("The layer mask used for the sphere cast to check if the player is grounded.")]
        LayerMask m_SphereCastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// The layer mask used for the sphere cast to check if the player is grounded.
        /// </summary>
        public LayerMask sphereCastLayerMask
        {
            get => m_SphereCastLayerMask;
            set => m_SphereCastLayerMask = value;
        }

        [SerializeField]
        [Tooltip("Whether trigger colliders are considered when using a sphere cast to determine if grounded. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.")]
        QueryTriggerInteraction m_SphereCastTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Whether trigger colliders are considered when using a sphere cast to determine if grounded.
        /// </summary>
        /// <remarks>
        /// When set to <see cref="QueryTriggerInteraction.UseGlobal"/>, the value of Queries Hit Triggers (<see cref="Physics.queriesHitTriggers"/>)
        /// in Edit &gt; Project Settings &gt; Physics will be used.
        /// </remarks>
        public QueryTriggerInteraction sphereCastTriggerInteraction
        {
            get => m_SphereCastTriggerInteraction;
            set => m_SphereCastTriggerInteraction = value;
        }

        [Tooltip("Event that is called when gravity lock is changed.")]
        [SerializeField]
        UnityEvent<GravityOverride> m_OnGravityLockChanged = new UnityEvent<GravityOverride>();

        /// <summary>
        /// Event that is called when gravity lock is changed.
        /// </summary>
        public UnityEvent<GravityOverride> onGravityLockChanged
        {
            get => m_OnGravityLockChanged;
            set => m_OnGravityLockChanged = value;
        }

        [Tooltip("Callback for anytime the grounded state changes.")]
        [SerializeField]
        UnityEvent<bool> m_OnGroundedChanged = new UnityEvent<bool>();

        /// <summary>
        /// Callback for anytime the grounded state changes.
        /// </summary>
        /// <seealso cref="isGrounded"/>
        public UnityEvent<bool> onGroundedChanged => m_OnGroundedChanged;

        bool m_IsGrounded;

        /// <summary>
        /// If the player is on the ground.
        /// </summary>
        /// <seealso cref="onGroundedChanged"/>
        public bool isGrounded => m_IsGrounded;

        /// <summary>
        /// The transformation that is used by this component to apply translation movement.
        /// </summary>
        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        readonly List<IGravityController> m_GravityControllers = new List<IGravityController>();

        /// <summary>
        /// (Read Only) The gravity controllers that can pause gravity.
        /// </summary>
        /// <remarks>
        /// Automatically populated during <see cref="Awake"/> with all components under the <see cref="LocomotionMediator"/>
        /// that implement the interface.
        /// </remarks>
        public List<IGravityController> gravityControllers => m_GravityControllers;

        /// <summary>
        /// The head of the local player.
        /// </summary>
        Transform m_HeadTransform;

        /// <summary>
        /// The allocation for SphereCastNonAlloc.
        /// </summary>
        readonly RaycastHit[] m_GroundedAllocHits = new RaycastHit[1];

        // Computed during CheckGrounded and stored as class fields to allow the gizmo to access the cast parameters
        Vector3 m_CastOrigin;
        float m_CastRadiusScaled;
        Vector3 m_CastDirection;
        float m_CastDistance;

        // Current calculated velocity of the player falling.
        Vector3 m_CurrentFallVelocity;

        PhysicsScene m_LocalPhysicsScene;

        CharacterController m_CharacterController;

        bool m_AttemptedGetCharacterController;

        XROrigin m_SearchedXROrigin;

        /// <summary>
        /// Providers that have gravity forced on.
        /// </summary>
        readonly List<IGravityController> m_GravityForcedOnProviders = new List<IGravityController>();

        /// <summary>
        /// Providers that have gravity forced off.
        /// </summary>
        readonly List<IGravityController> m_GravityForcedOffProviders = new List<IGravityController>();

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
            if (mediator != null)
                mediator.GetComponentsInChildren(true, m_GravityControllers);
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>
        /// </summary>
        protected virtual void Start()
        {
            // Only checking in OnEnable is insufficient since we will usually need to wait until Start to allow the
            // mediator to initialize references during its own Awake/OnEnable. This class drills down through those
            // properties to find additional component dependencies.
            FindDependencies();
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>
        /// </summary>
        protected virtual void Update()
        {
            CheckGrounded();

            if (m_IsGrounded && locomotionState == LocomotionState.Moving)
            {
                TryEndLocomotion();
                ResetFallForce();
            }

            if (TryProcessGravity(Time.deltaTime))
            {
                TryStartLocomotionImmediately();
                if (locomotionState == LocomotionState.Moving)
                {
                    transformation.motion = m_CurrentFallVelocity * Time.deltaTime;
                    TryQueueTransformation(transformation);
                }
            }

            if (m_UpdateCharacterControllerCenterEachFrame && m_HeadTransform != null && m_CharacterController != null)
            {
                var headLocalPosition = m_HeadTransform.localPosition;
                m_CharacterController.center = new Vector3(headLocalPosition.x, m_CharacterController.center.y, headLocalPosition.z);
            }
        }

        /// <summary>
        /// Updates current fall velocity if gravity is not currently blocked.
        /// </summary>
        /// <param name="time">By default, this uses <see cref="Time.deltaTime"/> as it's called from <see cref="Update"/>.</param>
        /// <returns>Returns <see langword="true"/> if <see cref="IsGravityBlocked"/> is <see langword="false"/>.</returns>
        protected virtual bool TryProcessGravity(float time)
        {
            if (IsGravityBlocked())
            {
                ResetFallForce();
                return false;
            }

            if (!Mathf.Approximately(m_CurrentFallVelocity.sqrMagnitude, m_TerminalVelocity * m_TerminalVelocity))
                m_CurrentFallVelocity = Vector3.ClampMagnitude(m_CurrentFallVelocity + (m_GravityAccelerationModifier * time * GetCurrentGravity()), m_TerminalVelocity);

            return true;
        }

        /// <summary>
        /// Gets the current up direction based on <see cref="useLocalSpaceGravity"/>. This value is normalized.
        /// </summary>
        /// <returns>Returns <c>-Physics.gravity.normalized</c> or the up of the XR Origin.</returns>
        public Vector3 GetCurrentUp()
        {
            return m_UseLocalSpaceGravity ? mediator.xrOrigin.Origin.transform.up : -Physics.gravity.normalized;
        }

        Vector3 GetCurrentGravity()
        {
            return m_UseLocalSpaceGravity ? -mediator.xrOrigin.Origin.transform.up * Physics.gravity.magnitude : Physics.gravity;
        }

        /// <summary>
        /// Checks if gravity is blocked by any of the following conditions:<br/>
        /// 1. If gravity is not being used.<br/>
        /// 2. If the player is grounded.<br/>
        /// 3. If any provider is forcing gravity off.<br/>
        /// 4. If any provider is pausing gravity while no provider is forcing gravity on.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if gravity is blocked for any reason.</returns>
        public bool IsGravityBlocked()
        {
            return !m_UseGravity || m_IsGrounded || !CanProcessGravity();
        }

        /// <summary>
        /// Resets the fall force to zero.
        /// </summary>
        public void ResetFallForce()
        {
            m_CurrentFallVelocity = Vector3.zero;
        }

        /// <summary>
        /// Checks if gravity can be processed based on the following priorities:<br/>
        /// 1. If any provider is forcing gravity off. Do not process gravity.<br/>
        /// 2. Else if any provider is forcing gravity on. Do process gravity.<br/>
        /// 3. Last if any provider is pausing gravity. Do not process gravity.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if gravity can be processed based on the priority.</returns>
        bool CanProcessGravity()
        {
            // If any provider is forcing gravity off, gravity is disabled
            if (m_GravityForcedOffProviders.Count > 0)
                return false;

            // If any provider is forcing gravity on, gravity is enabled
            if (m_GravityForcedOnProviders.Count > 0)
                return true;

            // If no controllers are being forced, check for pausing
            foreach (var controller in m_GravityControllers)
            {
                // If any controller is set to paused, gravity is disabled
                if (controller.canProcess && controller.gravityPaused)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Locks gravity for the <paramref name="provider"/> based on <paramref name="gravityOverride"/>.
        /// </summary>
        /// <remarks>
        /// This adds the provider to the internal lists keeping track of the locomotion providers that have forced gravity on or off.
        /// This method will report a warning if the provider is already locked and the previous <see cref="GravityOverride"/> mode does
        /// not match what is provided by the <paramref name="gravityOverride"/> parameter.
        /// </remarks>
        /// <param name="provider">The <paramref name="provider"/> requesting the lock.</param>
        /// <param name="gravityOverride">Determines the type of lock used.</param>
        /// <returns>Returns <see langword="true"/> if the provider is not already being locked.</returns>
        /// <seealso cref="UnlockGravity"/>
        public bool TryLockGravity(IGravityController provider, GravityOverride gravityOverride)
        {
            if (m_GravityForcedOffProviders.Contains(provider))
            {
                if (gravityOverride != GravityOverride.ForcedOff)
                    Debug.LogWarning($"Gravity Provider is already being locked (Forced Off) by {(provider is Object component ? component.name : provider)}. To force Gravity Override to on, unlock first before trying to lock again.", provider as Object ?? this);

                return false;
            }

            if (m_GravityForcedOnProviders.Contains(provider))
            {
                if (gravityOverride != GravityOverride.ForcedOn)
                    Debug.LogWarning($"Gravity Provider is already being locked (Forced On) by {(provider is Object component ? component.name : provider)}. To force Gravity Override to off, unlock first before trying to lock again.", provider as Object ?? this);

                return false;
            }

            if (gravityOverride == GravityOverride.ForcedOff)
                m_GravityForcedOffProviders.Add(provider);
            else if (gravityOverride == GravityOverride.ForcedOn)
                m_GravityForcedOnProviders.Add(provider);

            foreach (var controller in m_GravityControllers)
            {
                controller.OnGravityLockChanged(gravityOverride);
            }

            m_OnGravityLockChanged?.Invoke(gravityOverride);

            return true;
        }

        /// <summary>
        /// Unlock gravity for the provider.
        /// </summary>
        /// <param name="provider">The provider to unlock.</param>
        /// <remarks>
        /// Removes the provider from the internal lists keeping track of the locomotion providers that have forced gravity on or off.
        /// </remarks>
        /// <seealso cref="TryLockGravity"/>
        public void UnlockGravity(IGravityController provider)
        {
            m_GravityForcedOnProviders.Remove(provider);
            m_GravityForcedOffProviders.Remove(provider);
        }

        /// <summary>
        /// Checks if the player is grounded using a sphere cast from the player's head.
        /// </summary>
        void CheckGrounded()
        {
            var wasGrounded = m_IsGrounded;

            var scaleFactor = mediator.xrOrigin.Origin.transform.lossyScale.y;
            m_CastOrigin = GetBodyHeadPosition();
            m_CastRadiusScaled = m_SphereCastRadius * scaleFactor;
            m_CastDirection = -GetCurrentUp();
            m_CastDistance = (GetLocalHeadHeight() + m_SphereCastDistanceBuffer) * scaleFactor;

            m_IsGrounded = m_LocalPhysicsScene.SphereCast(m_CastOrigin, m_CastRadiusScaled,
                m_CastDirection, m_GroundedAllocHits, m_CastDistance, m_SphereCastLayerMask,
                m_SphereCastTriggerInteraction) > 0;

            // Check if grounded state changed
            if (wasGrounded != m_IsGrounded)
            {
                foreach (var controller in m_GravityControllers)
                {
                    controller.OnGroundedChanged(m_IsGrounded);
                }

                m_OnGroundedChanged?.Invoke(m_IsGrounded);
            }
        }

        /// <summary>
        /// Gets the local head height of the player. This is not scaled based on the XR Origin scale.
        /// </summary>
        /// <returns>Returns the local head height of the player.</returns>
        float GetLocalHeadHeight()
        {
            return mediator.xrOrigin.CameraInOriginSpaceHeight;
        }

        /// <summary>
        /// Gets the world space position of the player's head projected onto the body.
        /// </summary>
        /// <returns>Returns the world space position of the player's head projected onto the body.</returns>
        Vector3 GetBodyHeadPosition()
        {
            if (m_HeadTransform == null)
                return m_CharacterController != null ? m_CharacterController.bounds.center : transform.position;

            if (m_CharacterController == null)
                return m_HeadTransform.position;

            var center = m_CharacterController.bounds.center;
            return new Vector3(center.x, m_HeadTransform.position.y, center.z);
        }

        void FindDependencies()
        {
            // Only attempt to find components on an XR Origin once, especially since
            // the CharacterController is optional.
            var xrOrigin = mediator.xrOrigin;
            if (ReferenceEquals(xrOrigin, m_SearchedXROrigin))
                return;

            if (xrOrigin != null && xrOrigin.Origin != null)
            {
                // Save a reference to the optional CharacterController on the rig GameObject.
                // Try on the Origin GameObject first, and then fallback to the XR Origin GameObject (if different)
                if (!xrOrigin.Origin.TryGetComponent(out m_CharacterController) && xrOrigin.Origin != xrOrigin.gameObject)
                    xrOrigin.TryGetComponent(out m_CharacterController);

                m_HeadTransform = xrOrigin.Camera != null ? xrOrigin.Camera.transform : null;

                m_SearchedXROrigin = xrOrigin;
            }
        }

        /// <summary>
        /// Unity calls this when drawing gizmos while the object is selected.
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !isActiveAndEnabled)
                return;

            var color = m_IsGrounded ? Color.green : Color.red;
            Gizmos.color = color;

            // Draw a line spanning the entire cast
            var castMaxPoint = m_CastOrigin + (m_CastDirection * m_CastDistance);
            Debug.DrawLine(m_CastOrigin, castMaxPoint, color);

            var hit = m_GroundedAllocHits[0];

            // Draw a wire sphere at the end of the cast, either at the grounded hit
            // or the max possible range of the entire cast to help visualize the sphere cast.
            if (m_IsGrounded)
                Gizmos.DrawWireSphere(m_CastOrigin + (m_CastDirection * hit.distance), m_CastRadiusScaled);
            else
                Gizmos.DrawWireSphere(castMaxPoint, m_CastRadiusScaled);

            // Allow drawing the last valid hit point where the player was grounded even if not currently grounded
            if (hit.collider != null)
                Gizmos.DrawSphere(hit.point, 0.025f);
        }
    }
}
