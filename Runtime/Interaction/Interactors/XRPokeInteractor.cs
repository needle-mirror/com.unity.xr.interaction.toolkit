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
    public class XRPokeInteractor : XRBaseInteractor, IUIHoverInteractor, IPokeStateDataProvider, IAttachPointVelocityProvider
    {
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
        /// </summary>
        public bool enableUIInteraction
        {
            get => m_EnableUIInteraction;
            set
            {
                if (m_EnableUIInteraction != value)
                {
                    m_EnableUIInteraction = value;
                    m_RegisteredUIInteractorCache?.RegisterOrUnregisterXRUIInputModule(m_EnableUIInteraction);
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

        Vector3 m_LastPokeInteractionPoint;

        bool m_FirstFrame = true;
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
        /// Updates the registration with the appropriate UI system based on current settings.
        /// </summary>
        internal virtual void UpdateUIRegistration()
        {
            // First unregister from all UI systems
            m_RegisteredUIInteractorCache?.UnregisterFromXRUIInputModule();
#if UITOOLKIT_WORLDSPACE_ENABLED
            XRUIToolkitHandler.Unregister(this);
#endif

            // Register with the appropriate UI system
            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache?.RegisterWithXRUIInputModule();

#if UITOOLKIT_WORLDSPACE_ENABLED
            if (m_EnableUIInteraction)
            {
                XRUIToolkitHandler.Register(this);

                // Create the UI handler if needed
                if (m_UIToolkitPokeHandler == null)
                    m_UIToolkitPokeHandler = new XRUIToolkitPokeHandler(this);

                // Update visualizer state
                m_UIToolkitPokeHandler?.UpdateVisualizersState();
            }
#endif
        }

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
            m_FirstFrame = true;

            // Register with the appropriate UI system
            if (m_EnableUIInteraction)
                UpdateUIRegistration();

            if (attachPointVelocityTracker is AttachPointVelocityTracker velocityTracker)
                velocityTracker.ResetVelocityTracking();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            SetDebugObjectVisibility(false);

            m_RegisteredUIInteractorCache?.UnregisterFromXRUIInputModule();

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

            // Hover Check
            Vector3 pokeInteractionPoint = GetAttachTransform(null).position;
            Vector3 overlapStart = m_LastPokeInteractionPoint;
            Vector3 interFrameEnd = pokeInteractionPoint;

            BurstPhysicsUtils.GetSphereOverlapParameters(overlapStart, interFrameEnd, out var normalizedOverlapVector, out var overlapSqrMagnitude, out var overlapDistance);

            // If no movement is recorded.
            // Check if spherecast size is sufficient for proper cast, or if first frame since last frame poke position will be invalid.
            if (m_FirstFrame || overlapSqrMagnitude < 0.001f)
            {
                var numberOfOverlaps = m_LocalPhysicsScene.OverlapSphere(interFrameEnd, m_PokeHoverRadius, m_OverlapSphereHits,
                    m_PhysicsLayerMask, m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    if (FindPokeTarget(m_OverlapSphereHits[i], out var newPokeCollision))
                    {
                        m_PokeTargets.Add(newPokeCollision);
                    }
                }
            }
            else
            {
                var numberOfOverlaps = m_LocalPhysicsScene.SphereCast(
                    overlapStart,
                    m_PokeHoverRadius,
                    normalizedOverlapVector,
                    m_SphereCastHits,
                    overlapDistance,
                    m_PhysicsLayerMask,
                    m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    if (FindPokeTarget(m_SphereCastHits[i].collider, out var newPokeCollision))
                    {
                        m_PokeTargets.Add(newPokeCollision);
                    }
                }
            }

            m_LastPokeInteractionPoint = pokeInteractionPoint;
            m_FirstFrame = false;

            return m_PokeTargets.Count;
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

        /// <inheritdoc />
        void IUIHoverInteractor.OnUIHoverEntered(UIHoverEventArgs args) => OnUIHoverEntered(args);

        /// <inheritdoc />
        void IUIHoverInteractor.OnUIHoverExited(UIHoverEventArgs args) => OnUIHoverExited(args);

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
