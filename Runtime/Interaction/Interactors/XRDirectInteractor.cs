using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Interactor used for directly interacting with interactables that are touching. This is handled via trigger volumes
    /// that update the current set of valid targets for this interactor. This component must have a collision volume that is
    /// set to be a trigger to work.
    /// </summary>
    /// <remarks>
    /// For more information about the interaction system, refer to
    /// <a href="xref:xri-architecture">Interaction overview</a>.
    /// </remarks>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Interactors/XR Direct Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_XRDirectInteractor)]
    public class XRDirectInteractor : XRBaseInputInteractor
    {
        [SerializeField]
        bool m_ImproveAccuracyWithSphereCollider;

        /// <summary>
        /// When a Sphere Collider component is the only collider on this interactor, and no Rigidbody component is attached,
        /// the interactor will use Burst compiler optimizations and sphere casts instead of relying on physics trigger events
        /// to evaluate direct interactions when this property is enabled. This also improves inter-frame accuracy and reliability.
        /// </summary>
        /// <remarks>
        /// Cannot change this value at runtime after <c>Awake</c>.
        /// Enabling this property can improve inter-frame reliability during fast motion when the requirements for optimization are met
        /// by running on each Update instead of Fixed Update and using a sphere cast to determine valid targets.
        /// Disable to force the use of <a href="https://docs.unity3d.com/Manual/collider-interactions-ontrigger.html">OnTrigger events</a>,
        /// such as <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html"><c>OnTriggerEnter</c></a>
        /// and <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html"><c>OnTriggerExit</c></a> methods.
        /// </remarks>
        /// <seealso cref="usingSphereColliderAccuracyImprovement"/>
        public bool improveAccuracyWithSphereCollider
        {
            get => m_ImproveAccuracyWithSphereCollider;
            set => m_ImproveAccuracyWithSphereCollider = value;
        }

        /// <summary>
        /// Whether the requirements were successfully met to use the alternate improved collider accuracy code path.
        /// </summary>
        /// <remarks>
        /// The requirements are a single Sphere Collider component and no Rigidbody component on this GameObject.
        /// </remarks>
        /// <seealso cref="improveAccuracyWithSphereCollider"/>
        public bool usingSphereColliderAccuracyImprovement => m_UsingSphereColliderAccuracyImprovement;

        [SerializeField]
        LayerMask m_PhysicsLayerMask = 1; // Default

        /// <summary>
        /// Physics layer mask used for limiting direct interactor overlaps when using the <see cref="improveAccuracyWithSphereCollider"/> option.
        /// </summary>
        public LayerMask physicsLayerMask
        {
            get => m_PhysicsLayerMask;
            set => m_PhysicsLayerMask = value;
        }

        [SerializeField]
        QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Determines whether the direct interactor sphere overlap will hit triggers when using the <see cref="improveAccuracyWithSphereCollider"/> option.
        /// </summary>
        /// <remarks>
        /// When set to <see cref="QueryTriggerInteraction.UseGlobal"/>, the value of Queries Hit Triggers (<see cref="Physics.queriesHitTriggers"/>)
        /// in Edit &gt; Project Settings &gt; Physics will be used.
        /// </remarks>
        public QueryTriggerInteraction physicsTriggerInteraction
        {
            get => m_PhysicsTriggerInteraction;
            set => m_PhysicsTriggerInteraction = value;
        }

        /// <summary>
        /// The set of Interactables that this Interactor could possibly interact with this frame.
        /// This list is not sorted by priority.
        /// </summary>
        /// <seealso cref="IXRInteractor.GetValidTargets"/>
        protected List<IXRInteractable> unsortedValidTargets { get; } = new List<IXRInteractable>();

        /// <summary>
        /// The set of Colliders that stayed in touch with this Interactor.
        /// This list will be populated either during <see cref="OnTriggerStay"/> or while
        /// evaluating sphere overlap depending on the <see cref="improveAccuracyWithSphereCollider"/> option.
        /// </summary>
        readonly HashSet<Collider> m_StayedColliders = new HashSet<Collider>();

        readonly TriggerContactMonitor m_TriggerContactMonitor = new TriggerContactMonitor();
        readonly TriggerColliderMonitor m_TriggerColliderMonitor = new TriggerColliderMonitor();

        /// <summary>
        /// Reusable value of <see cref="WaitForFixedUpdate"/> to reduce allocations.
        /// </summary>
        static readonly WaitForFixedUpdate s_WaitForFixedUpdate = new WaitForFixedUpdate();

        /// <summary>
        /// Reference to Coroutine that updates the trigger contact monitor after all OnTrigger events
        /// have been invoked.
        /// </summary>
        IEnumerator m_UpdateCollidersAfterTriggerEvents;

        bool m_UsingSphereColliderAccuracyImprovement;
        SphereCollider m_SphereCollider;
        PhysicsScene m_LocalPhysicsScene;
        Vector3 m_LastSphereCastOrigin = Vector3.zero;
        readonly Collider[] m_OverlapSphereHits = new Collider[25];
        readonly RaycastHit[] m_SphereCastHits = new RaycastHit[25];
        bool m_FirstFrame = true;
        bool m_ContactsSortedThisFrame;
        readonly List<IXRInteractable> m_SortedValidTargets = new List<IXRInteractable>();

        bool m_HasOnTriggerStayEverInvoked;
        bool m_PauseOnTriggerStay = true;
        bool m_WaitForNextFixedUpdate;
        bool m_ReinitializeValidTargetsFromContacts;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
            m_TriggerContactMonitor.interactionManager = interactionManager;
            m_UpdateCollidersAfterTriggerEvents = UpdateCollidersAfterOnTriggerEvents();
            ValidateColliderConfiguration();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            m_TriggerContactMonitor.contactAdded += OnContactAdded;
            m_TriggerContactMonitor.contactRemoved += OnContactRemoved;
            ResetCollidersAndValidTargets();
            m_PauseOnTriggerStay = false;
            m_TriggerColliderMonitor.FindColliders(gameObject);
            m_TriggerColliderMonitor.CaptureEnabledState();

            // Schedule to initialize the valid targets list from the contacts. We would have missed
            // collider-to-interactable association updates while not subscribed to the monitor,
            // and we need to keep the valid targets list empty until the next process (either physics tick or sphere overlap).
            m_ReinitializeValidTargetsFromContacts = true;

            if (!m_UsingSphereColliderAccuracyImprovement)
                StartCoroutine(m_UpdateCollidersAfterTriggerEvents);
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            m_TriggerContactMonitor.contactAdded -= OnContactAdded;
            m_TriggerContactMonitor.contactRemoved -= OnContactRemoved;
            ResetCollidersAndValidTargets();
            // The `m_StayedColliders` list is only consumed in the coroutine. Since the coroutine is stopped
            // when this component is disabled, pause writing into that list until re-enabled and the coroutine
            // can be started again.
            m_PauseOnTriggerStay = true;

            // If the GameObject is deactivated, the Collider will no longer receive OnTriggerExit calls.
            // If we detect that Generate On Trigger Stay Events is disabled, we must invalidate all
            // touched colliders since we will miss the exit events.
            // Note: Ideally we would only do this if the Collider is disabled (either the component or from the GameObject
            // being deactivated), but there is no signal for that so we can only apply this if the GameObject is
            // deactivated.
            if (!m_HasOnTriggerStayEverInvoked && !gameObject.activeInHierarchy)
                m_TriggerContactMonitor.RemoveAllCollidersWithoutNotify();

            if (!m_UsingSphereColliderAccuracyImprovement)
            {
                StopCoroutine(m_UpdateCollidersAfterTriggerEvents);

                // Starting the coroutine when re-enabled will resume it immediately. Set a flag to avoid processing
                // the stayed collider list until the next physics tick and keep the valid targets cleared until then.
                m_WaitForNextFixedUpdate = true;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerEnter(Collider other)
        {
            if (m_UsingSphereColliderAccuracyImprovement)
                return;

            // Capture the enabled state of the trigger colliders once this begins touching another collider.
            // This allows users the freedom to enable or disable colliders until we need to start caring.
            if (!m_HasOnTriggerStayEverInvoked && m_TriggerContactMonitor.enteredCollidersCount == 0)
                m_TriggerColliderMonitor.CaptureEnabledState();

            m_TriggerContactMonitor.AddCollider(other);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerStay(Collider other)
        {
            m_HasOnTriggerStayEverInvoked = true;

            if (m_PauseOnTriggerStay || m_UsingSphereColliderAccuracyImprovement)
                return;

            m_StayedColliders.Add(other);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected void OnTriggerExit(Collider other)
        {
            if (m_UsingSphereColliderAccuracyImprovement)
                return;

            m_TriggerContactMonitor.RemoveCollider(other);
        }

        /// <summary>
        /// This coroutine functions like a LateFixedUpdate method that executes after OnTriggerXXX.
        /// </summary>
        /// <returns>Returns enumerator for coroutine.</returns>
        IEnumerator UpdateCollidersAfterOnTriggerEvents()
        {
            while (true)
            {
                // Wait until the end of the physics cycle so that OnTriggerXXX can get called.
                // See https://docs.unity3d.com/Manual/ExecutionOrder.html
                yield return s_WaitForFixedUpdate;

                if (m_WaitForNextFixedUpdate)
                {
                    m_WaitForNextFixedUpdate = false;
                    continue;
                }

                if (m_HasOnTriggerStayEverInvoked)
                    m_TriggerContactMonitor.UpdateStayedColliders(m_StayedColliders);
                else if (m_TriggerContactMonitor.enteredCollidersCount > 0)
                {
                    if (m_TriggerColliderMonitor.HasTriggerableColliderBeenDisabled())
                    {
                        if (m_TriggerColliderMonitor.triggerableColliders.Count > 1)
                        {
                            Debug.LogWarning("A trigger collider on this interactor was disabled." +
                                " Need to invalidate all touched colliders to guarantee no colliders are stuck hovering" +
                                " since trigger exit events are not invoked for disabled colliders.", this);
                        }

                        ResetCollidersAndValidTargets();
                        m_TriggerContactMonitor.RemoveAllCollidersWithoutNotify();
                        m_TriggerColliderMonitor.CaptureEnabledState();

                    }
                    else
                    {
                        m_TriggerContactMonitor.RemoveDisabledOrDestroyedColliders();
                    }
                }

                m_StayedColliders.Clear();

                if (m_ReinitializeValidTargetsFromContacts)
                    ReinitializeValidTargetsFromContacts();
            }
            // ReSharper disable once IteratorNeverReturns -- stopped when behavior is destroyed.
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);
            if (m_UsingSphereColliderAccuracyImprovement && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                EvaluateSphereOverlap();
        }

        void EvaluateSphereOverlap()
        {
            m_ContactsSortedThisFrame = false;
            m_StayedColliders.Clear();

            Transform directAttachTransform = GetAttachTransform(null);
            // Hover Check
            Vector3 interactorPosition = directAttachTransform.TransformPoint(m_SphereCollider.center);
            Vector3 overlapStart = m_LastSphereCastOrigin;
            Vector3 interFrameEnd = interactorPosition;
            float grabRadius = m_SphereCollider.radius * m_SphereCollider.transform.lossyScale.x;

            BurstPhysicsUtils.GetSphereOverlapParameters(overlapStart, interFrameEnd, out var normalizedOverlapVector, out var overlapSqrMagnitude, out var overlapDistance);

            // If no movement is recorded.
            // Check if sphere cast size is sufficient for proper cast, or if first frame since last frame poke position will be invalid.
            if (m_FirstFrame || overlapSqrMagnitude < 0.001f)
            {
                var numberOfOverlaps = m_LocalPhysicsScene.OverlapSphere(interFrameEnd, grabRadius, m_OverlapSphereHits,
                    m_PhysicsLayerMask, m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    m_StayedColliders.Add(m_OverlapSphereHits[i]);
                }
            }
            else
            {
                var numberOfOverlaps = m_LocalPhysicsScene.SphereCast(
                    overlapStart,
                    grabRadius,
                    normalizedOverlapVector,
                    m_SphereCastHits,
                    overlapDistance,
                    m_PhysicsLayerMask,
                    m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    m_StayedColliders.Add(m_SphereCastHits[i].collider);
                }
            }

            m_TriggerContactMonitor.UpdateStayedColliders(m_StayedColliders);
            m_StayedColliders.Clear();

            if (m_ReinitializeValidTargetsFromContacts)
                ReinitializeValidTargetsFromContacts();

            m_LastSphereCastOrigin = interactorPosition;
            m_FirstFrame = false;
        }

        void ValidateColliderConfiguration()
        {
            // If there isn't a Rigidbody on the same GameObject, a Trigger Collider has to be on this GameObject
            // for OnTriggerEnter, OnTriggerStay, and OnTriggerExit to be called by Unity. When this has a Rigidbody, Colliders can be
            // on child GameObjects and they don't necessarily have to be Trigger Colliders.
            // See Collision action matrix https://docs.unity3d.com/Manual/CollidersOverview.html
            if (!TryGetComponent(out Rigidbody _))
            {
                var colliders = GetComponents<Collider>();

                // If we don't have a Rigidbody and we only have 1 collider that is a Sphere Collider, we can use that to optimize the direct interactor.
                if (m_ImproveAccuracyWithSphereCollider &&
                    colliders.Length == 1 && colliders[0] is SphereCollider sphereCollider)
                {
                    m_SphereCollider = sphereCollider;

                    // Disable collider as only its properties are used to drive parameters to the sphere overlap.
                    m_SphereCollider.enabled = false;
                    m_UsingSphereColliderAccuracyImprovement = true;
                    return;
                }

                var hasTriggerCollider = false;
                foreach (var col in colliders)
                {
                    if (col.isTrigger)
                    {
                        hasTriggerCollider = true;
                        break;
                    }
                }

                if (!hasTriggerCollider)
                    Debug.LogWarning("Direct Interactor does not have required Collider set as a trigger.", this);
            }
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();

            if (!isActiveAndEnabled)
                return;

            var filter = targetFilter;
            if (filter != null && filter.canProcess)
                filter.Process(this, unsortedValidTargets, targets);
            else
            {
                // If not using the filter, we can cache the sorting of valid targets until the next time PreprocessInteractor is executed.
                if (m_ContactsSortedThisFrame)
                {
                    targets.AddRange(m_SortedValidTargets);
                    return;
                }

                // Sort valid targets
                SortingHelpers.SortByDistanceToInteractor(this, unsortedValidTargets, m_SortedValidTargets);

                targets.AddRange(m_SortedValidTargets);
                m_ContactsSortedThisFrame = true;
            }
        }

        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && (!hasSelection || IsSelecting(interactable));
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && (!hasSelection || IsSelecting(interactable));
        }

        /// <inheritdoc />
        protected override void OnRegistered(InteractorRegisteredEventArgs args)
        {
            base.OnRegistered(args);
            args.manager.interactableRegistered += OnInteractableRegistered;
            args.manager.interactableUnregistered += OnInteractableUnregistered;
            m_TriggerContactMonitor.interactionManager = args.manager;

            if (!m_UsingSphereColliderAccuracyImprovement)
            {
                // Attempt to resolve any colliders that entered this trigger while this was not subscribed,
                // and filter out any targets that were unregistered while this was not subscribed.
                m_TriggerContactMonitor.ResolveUnassociatedColliders();
                XRInteractionManager.RemoveAllUnregistered(args.manager, unsortedValidTargets);
            }
        }

        /// <inheritdoc />
        protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            base.OnUnregistered(args);
            args.manager.interactableRegistered -= OnInteractableRegistered;
            args.manager.interactableUnregistered -= OnInteractableUnregistered;
        }

        void OnInteractableRegistered(InteractableRegisteredEventArgs args)
        {
            var interactable = args.interactableObject;
            m_TriggerContactMonitor.ResolveUnassociatedColliders(interactable);
            if (m_TriggerContactMonitor.IsContacting(interactable))
                OnContactAdded(interactable);
        }

        void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            OnContactRemoved(args.interactableObject);
        }

        void OnContactAdded(IXRInteractable interactable)
        {
            if (unsortedValidTargets.Contains(interactable))
                return;

            unsortedValidTargets.Add(interactable);
            m_ContactsSortedThisFrame = false;
        }

        void OnContactRemoved(IXRInteractable interactable)
        {
            if (unsortedValidTargets.Remove(interactable))
                m_ContactsSortedThisFrame = false;
        }

        /// <summary>
        /// Clears current valid targets and stayed colliders.
        /// </summary>
        void ResetCollidersAndValidTargets()
        {
            unsortedValidTargets.Clear();
            m_SortedValidTargets.Clear();
            m_ContactsSortedThisFrame = false;
            m_FirstFrame = true;
            m_StayedColliders.Clear();
        }

        void ReinitializeValidTargetsFromContacts()
        {
            m_ReinitializeValidTargetsFromContacts = false;
            m_TriggerContactMonitor.GetContactInteractables(unsortedValidTargets);
            XRInteractionManager.RemoveAllUnregistered(m_TriggerContactMonitor.interactionManager, unsortedValidTargets);
            m_ContactsSortedThisFrame = false;
        }
    }
}
