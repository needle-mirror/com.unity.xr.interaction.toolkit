#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Interactor used for interacting with interactables through poking.
    /// </summary>
    /// <seealso cref="XRPokeFilter"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    [AddComponentMenu("XR/Interactors/XR Poke Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_XRPokeInteractor)]
    public class XRPokeInteractor : XRBaseInteractor, IUIHoverInteractor, IUIInteractorRegistrationHandler, IPokeStateDataProvider, IAttachPointVelocityProvider
    {
        /// <summary>
        /// Sets whether physics queries hit Trigger colliders and include or ignore snap volume trigger colliders.
        /// </summary>
        public enum QuerySnapVolumeInteraction
        {
            /// <summary>
            /// Queries never report Trigger hits that are registered with a snap volume.
            /// </summary>
            Ignore,

            /// <summary>
            /// Queries always report Trigger hits that are registered with a snap volume.
            /// </summary>
            Collide,
        }

        /// <summary>
        /// Reusable list of interactables (used to process the valid targets when this interactor has a filter).
        /// </summary>
        static readonly List<IXRInteractable> s_Results = new List<IXRInteractable>();

        [SerializeField]
        float m_PokeDepth = 0.1f;

        /// <summary>
        /// The depth threshold within which an interaction can begin to be evaluated as a poke.
        /// </summary>
        public float pokeDepth
        {
            get => m_PokeDepth;
            set => m_PokeDepth = value;
        }

        [SerializeField]
        float m_PokeWidth = 0.0075f;

        /// <summary>
        /// The width threshold within which an interaction can begin to be evaluated as a poke.
        /// </summary>
        public float pokeWidth
        {
            get => m_PokeWidth;
            set => m_PokeWidth = value;
        }

        [SerializeField]
        float m_PokeSelectWidth = 0.015f;

        /// <summary>
        /// The width threshold within which an interaction can be evaluated as a poke select.
        /// </summary>
        public float pokeSelectWidth
        {
            get => m_PokeSelectWidth;
            set => m_PokeSelectWidth = value;
        }

        [SerializeField]
        float m_PokeHoverRadius = 0.015f;

        /// <summary>
        /// The radius threshold within which an interaction can be evaluated as a poke hover.
        /// </summary>
        public float pokeHoverRadius
        {
            get => m_PokeHoverRadius;
            set => m_PokeHoverRadius = value;
        }

        [SerializeField]
        float m_PokeInteractionOffset = 0.005f;

        /// <summary>
        /// Distance along the poke interactable interaction axis that allows for a poke to be triggered sooner/with less precision.
        /// </summary>
        public float pokeInteractionOffset
        {
            get => m_PokeInteractionOffset;
            set => m_PokeInteractionOffset = value;
        }

        [SerializeField]
        LayerMask m_PhysicsLayerMask = Physics.AllLayers;

        /// <summary>
        /// Physics layer mask used for limiting poke sphere overlap.
        /// </summary>
        public LayerMask physicsLayerMask
        {
            get => m_PhysicsLayerMask;
            set => m_PhysicsLayerMask = value;
        }

        [SerializeField]
        QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Determines whether the poke sphere overlap will hit triggers.
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

        [SerializeField]
        QuerySnapVolumeInteraction m_SnapVolumeInteraction = QuerySnapVolumeInteraction.Ignore;

        /// <summary>
        /// Whether physics queries should include or ignore hits on trigger colliders that are snap volume colliders,
        /// even if the physics query is set to ignore triggers.
        /// </summary>
        /// <seealso cref="physicsTriggerInteraction"/>
        /// <seealso cref="XRInteractableSnapVolume.snapCollider"/>
        public QuerySnapVolumeInteraction snapVolumeInteraction
        {
            get => m_SnapVolumeInteraction;
            set => m_SnapVolumeInteraction = value;
        }

        [SerializeField]
        QueryUIDocumentInteraction m_UIDocumentTriggerInteraction = QueryUIDocumentInteraction.Collide;

        /// <summary>
        /// Whether physics casts should include or ignore hits on trigger colliders that are UI Toolkit UI Document colliders,
        /// even if the physics cast is set to ignore triggers.
        /// If you are not using UI Toolkit, you should set this property
        /// to <see cref="QueryUIDocumentInteraction.Ignore"/> to avoid the performance cost.
        /// </summary>
        /// <remarks>
        /// When set to <see cref="QueryUIDocumentInteraction.Collide"/> when <see cref="physicsTriggerInteraction"/> is set to ignore trigger colliders
        /// (when set to <see cref="QueryTriggerInteraction.Ignore"/> or when set to <see cref="QueryTriggerInteraction.UseGlobal"/>
        /// while <see cref="Physics.queriesHitTriggers"/> is <see langword="false"/>),
        /// the physics cast query will be modified to include trigger colliders, but then this behavior will ignore any trigger collider
        /// hits that are not UI Toolkit UI Documents.
        /// <br />
        /// When set to <see cref="QueryUIDocumentInteraction.Ignore"/> when <see cref="physicsTriggerInteraction"/> is set to hit trigger colliders
        /// (when set to <see cref="QueryTriggerInteraction.Collide"/> or when set to <see cref="QueryTriggerInteraction.UseGlobal"/>
        /// while <see cref="Physics.queriesHitTriggers"/> is <see langword="true"/>),
        /// this behavior will ignore any trigger collider hits that are UI Toolkit UI Documents.
        /// </remarks>
        /// <seealso cref="physicsTriggerInteraction"/>
        public QueryUIDocumentInteraction uiDocumentTriggerInteraction
        {
            get => m_UIDocumentTriggerInteraction;
            set => m_UIDocumentTriggerInteraction = value;
        }

        [SerializeField]
        bool m_RequirePokeFilter = true;

        /// <summary>
        /// Denotes whether or not valid targets will only include objects with a poke filter.
        /// </summary>
        public bool requirePokeFilter
        {
            get => m_RequirePokeFilter;
            set => m_RequirePokeFilter = value;
        }

        [SerializeField]
        bool m_EnableUIInteraction = true;

        /// <summary>
        /// Gets or sets whether this Interactor is able to affect UI.
        /// Requires the XR UI Input Module on the Event System.
        /// </summary>
        public bool enableUIInteraction
        {
            get => m_EnableUIInteraction;
            set
            {
                if (m_EnableUIInteraction != value)
                {
                    m_EnableUIInteraction = value;
                    if (Application.isPlaying && isActiveAndEnabled)
                        UpdateUIRegistration();
                }
            }
        }

        [SerializeField]
        bool m_ClickUIOnDown;

        /// <summary>
        /// When enabled, this will invoke click events on press down instead of on release.
        /// Otherwise, click is invoked as normal on release.
        /// </summary>
        public bool clickUIOnDown
        {
            get => m_ClickUIOnDown;
            set => m_ClickUIOnDown = value;
        }

        [SerializeField]
        bool m_DebugVisualizationsEnabled;

        /// <summary>
        /// Denotes whether or not debug visuals are enabled for this poke interactor.
        /// Debug visuals include two spheres to display whether or not hover and select colliders have collided.
        /// </summary>
        public bool debugVisualizationsEnabled
        {
            get => m_DebugVisualizationsEnabled;
            set => m_DebugVisualizationsEnabled = value;
        }

        [SerializeField]
        UIHoverEnterEvent m_UIHoverEntered = new UIHoverEnterEvent();

        /// <inheritdoc />
        public UIHoverEnterEvent uiHoverEntered
        {
            get => m_UIHoverEntered;
            set => m_UIHoverEntered = value;
        }

        [SerializeField]
        UIHoverExitEvent m_UIHoverExited = new UIHoverExitEvent();

        /// <inheritdoc />
        public UIHoverExitEvent uiHoverExited
        {
            get => m_UIHoverExited;
            set => m_UIHoverExited = value;
        }

        BindableVariable<PokeStateData> m_PokeStateData = new BindableVariable<PokeStateData>();

        /// <inheritdoc />
        public IReadOnlyBindableVariable<PokeStateData> pokeStateData => m_PokeStateData;

        /// <summary>
        /// The tracker used to compute the velocity of the attach point.
        /// This behavior automatically updates this velocity tracker each frame during <see cref="PreprocessInteractor"/>.
        /// </summary>
        /// <seealso cref="GetAttachPointVelocity"/>
        /// <seealso cref="GetAttachPointAngularVelocity"/>
        protected IAttachPointVelocityTracker attachPointVelocityTracker { get; set; } = new AttachPointVelocityTracker();

        GameObject m_HoverDebugSphere;
        MeshRenderer m_HoverDebugRenderer;

        // Motion continuity state for swept-sphere discovery (internal, non-serialized)
        // Used to validate that the inter-frame sweep path is physically meaningful
        // before issuing a SphereCast along it.
        Vector3 m_PrevTipWorld;
        Vector3 m_CurrTipWorld;
        bool m_HasValidPrev;
        int m_LastTipUpdateFrame = -1;

        /// <summary>
        /// Squared motion threshold below which OverlapSphere is used instead of SphereCast.
        /// 0.000025f = (0.005m)² = 5mm. Calibrated to sit above hand-tracking jitter (~1-3mm)
        /// while catching moderate-speed pokes through thin UI surfaces (1-5mm thick).
        /// </summary>
        const float k_MinSweepDistanceSqr = 0.000025f;

        /// <summary>
        /// Maximum plausible squared inter-frame delta. Anything above this is treated as a teleport
        /// and the sweep is suppressed to avoid phantom hits along a physically meaningless path.
        /// 4.0f = (2.0m)².
        /// </summary>
        const float k_MaxReasonableDeltaSqr = 4.0f;

        IXRSelectInteractable m_CurrentPokeTarget;
        IXRPokeFilter m_CurrentPokeFilter;

        readonly RaycastHit[] m_SphereCastHits = new RaycastHit[25];
        readonly Collider[] m_OverlapSphereHits = new Collider[25];
        readonly List<PokeCollision> m_PokeTargets = new List<PokeCollision>();
        readonly List<IXRSelectFilter> m_InteractableSelectFilters = new List<IXRSelectFilter>();

        readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();
        static readonly Dictionary<IXRInteractable, IXRPokeFilter> s_ValidTargetsScratchMap = new Dictionary<IXRInteractable, IXRPokeFilter>();

        RegisteredUIInteractorCache m_RegisteredUIInteractorCache;
        PhysicsScene m_LocalPhysicsScene;

        // Used to avoid GC Alloc each frame in UpdateUIModel
        Func<Vector3> m_PositionProvider;

        /// <summary>
        /// (Read Only) Whether UI interaction with UI Toolkit is enabled.
        /// </summary>
        bool canProcessUIToolkit
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            get => m_EnableUIInteraction && XRUIToolkitHandler.uiToolkitSupportEnabled;
#else
            get => false;
#endif
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        /// <summary>
        /// The handler responsible for UI Toolkit poke interaction and state management.
        /// </summary>
        /// <seealso cref="XRUIToolkitPokeHandler"/>
        XRUIToolkitPokeHandler m_UIToolkitPokeHandler;
#endif

        // TODO Hidden for now - will evaluate its use later.
        [HideInInspector]
        [SerializeField]
        [Tooltip("When enabled, multi-point sampling is used for more forgiving UI element detection. Off by default for performance.")]
        bool m_EnableMultiPick;

        /// <summary>
        /// Whether to use multi-point sampling for more forgiving UI element detection.
        /// When enabled, additional sampling points are used to improve interaction with small UI elements,
        /// at the cost of some performance overhead.
        /// </summary>
        /// <seealso cref="XRUIToolkitPokeHandler"/>
        internal bool enableMultiPick
        {
            get => m_EnableMultiPick;
            set => m_EnableMultiPick = value;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
            m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);
            m_PositionProvider = GetPokePosition;
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDebugObjectVisibility(true);
            ResetMotionContinuityState();

            if (m_EnableUIInteraction)
            {
                m_RegisteredUIInteractorCache?.HandleOnEnable();
#if UITOOLKIT_WORLDSPACE_ENABLED
                XRUIToolkitHandler.Register(this);

                // Update visualizer state
                m_UIToolkitPokeHandler ??= new XRUIToolkitPokeHandler(this);
                m_UIToolkitPokeHandler.UpdateVisualizersState();
#endif
            }

            if (attachPointVelocityTracker is AttachPointVelocityTracker velocityTracker)
                velocityTracker.ResetVelocityTracking();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            SetDebugObjectVisibility(false);

            m_RegisteredUIInteractorCache?.HandleOnDisable();

#if UITOOLKIT_WORLDSPACE_ENABLED
            if (canProcessUIToolkit)
            {
                // Reset handler state
                m_UIToolkitPokeHandler?.ResetPointerState();
            }

            XRUIToolkitHandler.Unregister(this);
#endif
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
#if UITOOLKIT_WORLDSPACE_ENABLED
            m_UIToolkitPokeHandler?.Dispose();
#endif
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (TryGetXROrigin(out var origin))
                    attachPointVelocityTracker.UpdateAttachPointVelocityData(GetAttachTransform(null), origin);
                else
                    attachPointVelocityTracker.UpdateAttachPointVelocityData(GetAttachTransform(null));

                UpdateMotionContinuityState();

                RegisterValidTargets(out m_CurrentPokeTarget, out m_CurrentPokeFilter);
                ProcessPokeStateData();
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);


            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                UpdateDebugVisuals();
        }

        bool RegisterValidTargets(out IXRSelectInteractable currentTarget, out IXRPokeFilter pokeFilter)
        {
            int sphereOverlapCount = EvaluateSphereOverlap();
            bool hasOverlap = sphereOverlapCount > 0;

            m_ValidTargets.Clear();
            s_ValidTargetsScratchMap.Clear();

            if (hasOverlap)
            {
                int pokeTargetsCount = m_PokeTargets.Count;
                for (var i = 0; i < pokeTargetsCount; ++i)
                {
                    var interactable = m_PokeTargets[i].interactable;
                    if (m_ValidTargets.Contains(interactable) || interactable is not (IXRSelectInteractable and IXRHoverInteractable hoverable) || !hoverable.IsHoverableBy(this))
                        continue;
                    m_ValidTargets.Add(m_PokeTargets[i].interactable);
                    s_ValidTargetsScratchMap.Add(m_PokeTargets[i].interactable, m_PokeTargets[i].filter);
                }

                // Sort before target filter
                if (m_ValidTargets.Count > 1)
                    SortingHelpers.SortByDistanceToInteractor(this, m_ValidTargets, SortingHelpers.squareDistanceAttachPointEvaluator);

                var filter = targetFilter;
                if (filter != null && filter.canProcess)
                {
                    filter.Process(this, m_ValidTargets, s_Results);

                    // Copy results elements to targets
                    m_ValidTargets.Clear();
                    m_ValidTargets.AddRange(s_Results);
                }

                if (m_ValidTargets.Count == 0)
                    hasOverlap = false;
            }

            currentTarget = hasOverlap ? (IXRSelectInteractable)m_ValidTargets[0] : null;
            pokeFilter = hasOverlap ? s_ValidTargetsScratchMap[currentTarget] : null;
            return hasOverlap;
        }

        void ProcessPokeStateData()
        {
            if (TrackedDeviceGraphicRaycaster.TryGetPokeStateDataForInteractor(this, out var uiData))
                m_PokeStateData.Value = uiData;
            else if (m_CurrentPokeFilter is IPokeStateDataProvider pokeStateDataProvider)
                m_PokeStateData.Value = pokeStateDataProvider.pokeStateData.Value;
            else
                m_PokeStateData.Value = default;
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();

            if (!isActiveAndEnabled)
                return;

            if (m_ValidTargets.Count > 0)
                targets.Add(m_ValidTargets[0]);
        }

        int EvaluateSphereOverlap()
        {
            m_PokeTargets.Clear();

            // m_CurrTipWorld is already set by UpdateMotionContinuityState() earlier in PreprocessInteractor,
            // so we reuse it rather than calling GetAttachTransform(null).position a second time this frame.
            Vector3 pokeInteractionPoint = m_CurrTipWorld;

            // Determine whether we have valid motion continuity for a swept query.
            var continuityDelta = m_CurrTipWorld - m_PrevTipWorld;
            float continuityDeltaSqr = continuityDelta.sqrMagnitude;

            // m_HasValidPrev already accounts for first-frame suppression (UpdateMotionContinuityState
            // keeps it false when m_LastTipUpdateFrame < 0), frame gaps, and teleport-like deltas
            bool canSweep =
                m_HasValidPrev &&
                continuityDeltaSqr >= k_MinSweepDistanceSqr &&
                continuityDeltaSqr <= k_MaxReasonableDeltaSqr;

            var manager = interactionManager;

            // Resolve trigger interaction: widen physics query to include triggers when
            // UI document colliders need to be detected, then post-filter non-UI triggers.
            var baseQueryHitsTriggers = m_PhysicsTriggerInteraction == QueryTriggerInteraction.Collide ||
                (m_PhysicsTriggerInteraction == QueryTriggerInteraction.UseGlobal && Physics.queriesHitTriggers);

            var allowSnapVolumes = m_SnapVolumeInteraction == QuerySnapVolumeInteraction.Collide;
            var allowUIDocuments = m_UIDocumentTriggerInteraction == QueryUIDocumentInteraction.Collide;

            var queryTriggerInteraction = (baseQueryHitsTriggers || allowSnapVolumes || allowUIDocuments)
                ? QueryTriggerInteraction.Collide
                : QueryTriggerInteraction.Ignore;

            // If motion continuity is invalid or below sweep threshold, use static overlap at current position.
            // Otherwise, sweep a sphere along the inter-frame path to catch thin colliders.
            if (!canSweep)
            {
                var numberOfOverlaps = m_LocalPhysicsScene.OverlapSphere(pokeInteractionPoint, m_PokeHoverRadius, m_OverlapSphereHits,
                    m_PhysicsLayerMask, queryTriggerInteraction);

                if (numberOfOverlaps > 0)
                {
                    numberOfOverlaps = TriggerColliderFilterUtility.FilterTriggerColliders(
                        manager, m_OverlapSphereHits, numberOfOverlaps,
                        baseQueryHitsTriggers, allowSnapVolumes, allowUIDocuments);
                }

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    var hitCollider = m_OverlapSphereHits[i];
                    if (FindPokeTarget(hitCollider, out var newPokeCollision))
                        m_PokeTargets.Add(newPokeCollision);
                }
            }
            else
            {
                float sweepDistance = Mathf.Sqrt(continuityDeltaSqr);
                var sweepDirection = continuityDelta / sweepDistance;

                var numberOfOverlaps = m_LocalPhysicsScene.SphereCast(
                    m_PrevTipWorld,
                    m_PokeHoverRadius,
                    sweepDirection,
                    m_SphereCastHits,
                    sweepDistance,
                    m_PhysicsLayerMask,
                    queryTriggerInteraction);

                if (numberOfOverlaps > 0)
                {
                    numberOfOverlaps = TriggerColliderFilterUtility.FilterTriggerColliders(
                        manager, m_SphereCastHits, numberOfOverlaps,
                        baseQueryHitsTriggers, allowSnapVolumes, allowUIDocuments);
                }

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    var hitCollider = m_SphereCastHits[i].collider;
                    if (FindPokeTarget(hitCollider, out var newPokeCollision))
                        m_PokeTargets.Add(newPokeCollision);
                }
            }

            return m_PokeTargets.Count;
        }

        /// <summary>
        /// Resets motion continuity state. Called on enable/disable transitions to prevent
        /// stale prev-frame positions from producing ghost sweep hits.
        /// </summary>
        void ResetMotionContinuityState()
        {
            m_HasValidPrev = false;
            m_LastTipUpdateFrame = -1;
            m_PrevTipWorld = Vector3.zero;
            m_CurrTipWorld = Vector3.zero;
        }

        /// <summary>
        /// Captures per-frame tip motion state in PreprocessInteractor to validate that inter-frame
        /// sweep paths are physically meaningful. Intentionally conservative about continuity: any
        /// frame gap, teleport, or first-frame condition suppresses sweeping for that frame.
        /// </summary>
        void UpdateMotionContinuityState()
        {
            var curr = GetAttachTransform(null).position;

            // First update after reset, initialize both endpoints, suppress sweep.
            // Suppress sweeping on frame gaps (skipped frames, disabled interactor, tracking discontinuities).
            if (m_LastTipUpdateFrame < 0 || Time.frameCount != m_LastTipUpdateFrame + 1)
            {
                m_PrevTipWorld = curr;
                m_CurrTipWorld = curr;
                m_HasValidPrev = false;
                m_LastTipUpdateFrame = Time.frameCount;
                return;
            }

            m_PrevTipWorld = m_CurrTipWorld;
            m_CurrTipWorld = curr;
            m_LastTipUpdateFrame = Time.frameCount;

            // Suppress sweeping on teleport-like deltas to avoid ghost hits.
            var delta = m_CurrTipWorld - m_PrevTipWorld;
            if (delta.sqrMagnitude > k_MaxReasonableDeltaSqr)
            {
                m_PrevTipWorld = m_CurrTipWorld;
                m_HasValidPrev = false;
                return;
            }

            m_HasValidPrev = true;
        }

        bool FindPokeTarget(Collider hitCollider, out PokeCollision newPokeCollision)
        {
            newPokeCollision = default;

            if (interactionManager.TryGetInteractableForCollider(hitCollider, out var interactable))
            {
                if (TryGetPokeFilter(interactable, out var pokeFilter))
                {
                    newPokeCollision = new PokeCollision(interactable, pokeFilter);
                    ProcessValidInteraction(hitCollider, interactable, pokeFilter);
                    return true;
                }
                else if (!m_RequirePokeFilter)
                {
                    newPokeCollision = new PokeCollision(interactable, null);
                    ProcessValidInteraction(hitCollider, interactable, null);
                    return true;
                }
            }

            return false;
        }

        bool TryGetPokeFilter(IXRInteractable interactable, out IXRPokeFilter pokeFilter)
        {
            pokeFilter = null;

            if (interactable is XRBaseInteractable baseInteractable)
            {
                baseInteractable.selectFilters.GetAll(m_InteractableSelectFilters);
                foreach (var filter in m_InteractableSelectFilters)
                {
                    if (filter is IXRPokeFilter tempPokeFilter && filter.canProcess)
                    {
                        pokeFilter = tempPokeFilter;
                        return true;
                    }
                }
            }

            return false;
        }

        // Replace UIPokeLogicImplementation with:
        void ProcessValidInteraction(Collider hitCollider, IXRInteractable interactable, IXRPokeFilter pokeFilter)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (canProcessUIToolkit)
                m_UIToolkitPokeHandler?.ProcessPokeInteraction(hitCollider, interactable.transform, interactable, m_EnableMultiPick, pokeFilter);
#endif
        }

        void SetDebugObjectVisibility(bool isVisible)
        {
            if (m_DebugVisualizationsEnabled)
            {
                if (m_HoverDebugSphere == null)
                {
                    m_HoverDebugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    m_HoverDebugSphere.name = "[Debug] Poke - HoverVisual: " + this;
                    m_HoverDebugSphere.transform.SetParent(GetAttachTransform(null), false);
                    m_HoverDebugSphere.transform.localScale = new Vector3(m_PokeHoverRadius, m_PokeHoverRadius, m_PokeHoverRadius);

                    if (m_HoverDebugSphere.TryGetComponent<Collider>(out var debugCollider))
                        Destroy(debugCollider);

                    m_HoverDebugRenderer = GetOrAddComponent<MeshRenderer>(m_HoverDebugSphere);
                }
            }

            var visibility = m_DebugVisualizationsEnabled && isVisible;

            if (m_HoverDebugSphere != null && m_HoverDebugSphere.activeSelf != visibility)
                m_HoverDebugSphere.SetActive(visibility);
        }

        void UpdateDebugVisuals()
        {
            SetDebugObjectVisibility(m_CurrentPokeTarget != null);

            if (!m_DebugVisualizationsEnabled)
                return;
            m_HoverDebugRenderer.material.color = m_PokeTargets.Count > 0 ? new Color(0f, 0.8f, 0f, 0.1f) : new Color(0.8f, 0f, 0f, 0.1f);
        }

        static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            return go.TryGetComponent<T>(out var component) ? component : go.AddComponent<T>();
        }

        readonly struct PokeCollision
        {
            public readonly IXRInteractable interactable;
            public readonly IXRPokeFilter filter;

            public PokeCollision(IXRInteractable interactable, IXRPokeFilter filter)
            {
                this.interactable = interactable;
                this.filter = filter;
            }
        }

        /// <inheritdoc />
        public virtual void UpdateUIModel(ref TrackedDeviceModel model)
        {
            if (!isActiveAndEnabled || this.IsBlockedByInteractionWithinGroup())
            {
                model.Reset(false);
                return;
            }

            var pokeInteractionTransform = GetAttachTransform(null);
            var pokePose = pokeInteractionTransform.GetWorldPose();
            Vector3 startPoint = pokePose.position;
            Vector3 penetrationDirection = pokePose.rotation * Vector3.forward;
            Vector3 endPoint = startPoint + (penetrationDirection * m_PokeDepth);

            model.position = pokePose.position;
            model.orientation = pokePose.rotation;
            model.positionProvider = m_PositionProvider;
            model.raycastLayerMask = m_PhysicsLayerMask;
            model.pokeDepth = m_PokeDepth;
            model.interactionType = UIInteractionType.Poke;
            model.clickOnDown = m_ClickUIOnDown;
            model.UpdatePokeSelectState();

            var raycastPoints = model.raycastPoints;
            raycastPoints.Clear();
            raycastPoints.Add(startPoint);
            raycastPoints.Add(endPoint);
        }

        Vector3 GetPokePosition()
        {
            return GetAttachTransform(null).position;
        }

        /// <inheritdoc />
        public bool TryGetUIModel(out TrackedDeviceModel model)
        {
            if (m_RegisteredUIInteractorCache == null)
            {
                model = TrackedDeviceModel.invalid;
                return false;
            }

            return m_RegisteredUIInteractorCache.TryGetUIModel(out model);
        }

        /// <summary>
        /// Updates the registration with the appropriate UI system based on current settings.
        /// Handles both UGUI (via XRUIInputModule) and UI Toolkit (via XRUIToolkitHandler) registration.
        /// </summary>
        void UpdateUIRegistration()
        {
            m_RegisteredUIInteractorCache?.UpdateEnableUIInteraction(m_EnableUIInteraction);

#if UITOOLKIT_WORLDSPACE_ENABLED
            if (m_EnableUIInteraction)
            {
                XRUIToolkitHandler.Register(this);

                // Update visualizer state
                m_UIToolkitPokeHandler ??= new XRUIToolkitPokeHandler(this);
                m_UIToolkitPokeHandler.UpdateVisualizersState();
            }
            else
            {
                XRUIToolkitHandler.Unregister(this);
            }
#endif
        }

        /// <inheritdoc />
        void IUIInteractorRegistrationHandler.OnRegistered(UIInteractorRegisteredEventArgs args) => OnRegistered(args);

        /// <inheritdoc />
        void IUIInteractorRegistrationHandler.OnUnregistered(UIInteractorUnregisteredEventArgs args) => OnUnregistered(args);

        /// <inheritdoc />
        void IUIHoverInteractor.OnUIHoverEntered(UIHoverEventArgs args) => OnUIHoverEntered(args);

        /// <inheritdoc />
        void IUIHoverInteractor.OnUIHoverExited(UIHoverEventArgs args) => OnUIHoverExited(args);

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method
        /// when this UI interactor is registered with it.
        /// </summary>
        /// <param name="args">Event data containing the XR UI Input Module that registered this UI interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRUIInputModule.RegisterInteractor"/>
        protected virtual void OnRegistered(UIInteractorRegisteredEventArgs args)
        {
            m_RegisteredUIInteractorCache ??= new RegisteredUIInteractorCache(this);
            m_RegisteredUIInteractorCache.HandleOnRegistered(args);
        }

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method
        /// when this UI interactor is unregistered from it.
        /// </summary>
        /// <param name="args">Event data containing the XR UI Input Module that unregistered this UI interactor.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRUIInputModule.UnregisterInteractor"/>
        protected virtual void OnUnregistered(UIInteractorUnregisteredEventArgs args)
        {
            m_RegisteredUIInteractorCache?.HandleOnUnregistered(args);
        }

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method when the Interactor begins hovering over a UI element.
        /// </summary>
        /// <param name="args">Event data containing the UI element that is being hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnUIHoverExited(UIHoverEventArgs)"/>
        protected virtual void OnUIHoverEntered(UIHoverEventArgs args)
        {
            m_UIHoverEntered?.Invoke(args);
        }

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method when the Interactor ends hovering over a UI element.
        /// </summary>
        /// <param name="args">Event data containing the UI element that is no longer hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnUIHoverEntered(UIHoverEventArgs)"/>
        protected virtual void OnUIHoverExited(UIHoverEventArgs args)
        {
            m_UIHoverExited?.Invoke(args);
        }

        /// <inheritdoc />
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

#if UITOOLKIT_WORLDSPACE_ENABLED
            if (args.interactableObject != null && canProcessUIToolkit && XRUIToolkitHandler.IsValidUIToolkitInteraction(args.interactableObject.colliders))
            {
                m_UIToolkitPokeHandler?.ResetPointerState();
            }
#endif
        }

        /// <inheritdoc />
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            base.OnHoverEntering(args);
        }

        /// <summary>
        /// Last computed default attach point velocity, based on multi-frame sampling of the pose in world space.
        /// </summary>
        /// <returns>Returns the transformed attach point linear velocity.</returns>
        /// <seealso cref="GetAttachPointAngularVelocity"/>
        public Vector3 GetAttachPointVelocity()
        {
            if (TryGetXROrigin(out var origin))
            {
                return attachPointVelocityTracker.GetAttachPointVelocity(origin);
            }

            return attachPointVelocityTracker.GetAttachPointVelocity();
        }

        /// <summary>
        /// Last computed default attach point angular velocity, based on multi-frame sampling of the pose in world space.
        /// </summary>
        /// <returns>Returns the transformed attach point angular velocity.</returns>
        /// <seealso cref="GetAttachPointVelocity"/>
        public Vector3 GetAttachPointAngularVelocity()
        {
            if (TryGetXROrigin(out var origin))
            {
                return attachPointVelocityTracker.GetAttachPointAngularVelocity(origin);
            }

            return attachPointVelocityTracker.GetAttachPointAngularVelocity();
        }
    }
}
