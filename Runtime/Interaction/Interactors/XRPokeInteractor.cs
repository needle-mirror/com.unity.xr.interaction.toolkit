using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for interacting with interactables through poking.
    /// </summary>
    /// <seealso cref="XRPokeFilter"/>
    [AddComponentMenu("XR/XR Poke Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_XRPokeInteractor)]
    public class XRPokeInteractor : XRBaseInteractor, IUIInteractor
    {
        /// <summary>
        /// Reusable list of interactables (used to process the valid targets when this interactor has a filter).
        /// </summary>
        static readonly List<IXRInteractable> s_Results = new List<IXRInteractable>();

        [SerializeField]
        [Tooltip("The depth threshold within which an interaction can begin to be evaluated as a poke.")]
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
        [Tooltip("The width threshold within which an interaction can begin to be evaluated as a poke.")]
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
        [Tooltip("The width threshold within which an interaction can be evaluated as a poke select.")]
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
        [Tooltip("The radius threshold within which an interaction can be evaluated as a poke hover.")]
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
        [Tooltip("Distance along the poke interactable interaction axis that allows for a poke to be triggered sooner/with less precision.")]
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
        [Tooltip("Physics layer mask used for limiting poke sphere overlap.")]
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
        [Tooltip("Determines whether the poke sphere overlap will hit triggers.")]
        QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Determines whether the poke sphere overlap will hit triggers.
        /// </summary>
        public QueryTriggerInteraction physicsTriggerInteraction
        {
            get => m_PhysicsTriggerInteraction;
            set => m_PhysicsTriggerInteraction = value;
        }

        [SerializeField]
        [Tooltip("Denotes whether or not valid targets will only include objects with a poke filter.")]
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
        [Tooltip("When enabled, this allows the poke interactor to hover and select UI elements.")]
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
        [Tooltip("Denotes whether or not debug visuals are enabled for this poke interactor.")]
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

        GameObject m_HoverDebugSphere;
        MeshRenderer m_HoverDebugRenderer;

        Vector3 m_LastPokeInteractionPoint;

        bool m_PokeCanSelect;
        bool m_FirstFrame = true;
        IXRSelectInteractable m_CurrentPokeTarget;

        readonly RaycastHit[] m_SphereCastHits = new RaycastHit[25];
        readonly Collider[] m_OverlapSphereHits = new Collider[25];
        readonly List<PokeCollision> m_PokeTargets = new List<PokeCollision>();
        readonly List<IXRSelectFilter> m_InteractableSelectFilters = new List<IXRSelectFilter>();
                        
        RegisteredUIInteractorCache m_RegisteredUIInteractorCache;
        PhysicsScene m_LocalPhysicsScene;
        // Used to avoid GC Alloc each frame in UpdateUIModel
        Func<Vector3> m_PositionGetter;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            useAttachPointVelocity = true;
            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
            m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);
            m_PositionGetter = GetPokePosition;
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDebugObjectVisibility(true);
            m_FirstFrame = true;

            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache.RegisterWithXRUIInputModule();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            SetDebugObjectVisibility(false);

            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache.UnregisterFromXRUIInputModule();
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                isInteractingWithUI = TrackedDeviceGraphicRaycaster.IsPokeInteractingWithUI(this);
                m_PokeCanSelect = EvaluatePokeInteraction(out m_CurrentPokeTarget);
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                UpdateDebugVisuals();
            }
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();

            if (!isActiveAndEnabled)
                return;

            foreach (var pokeCollision in m_PokeTargets)
            {
                targets.Add(pokeCollision.interactable);
            }

            var filter = targetFilter;
            if (filter != null && filter.canProcess)
            {
                filter.Process(this, targets, s_Results);

                // Copy results elements to targets
                targets.Clear();
                targets.AddRange(s_Results);
            }
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return m_PokeCanSelect && interactable == m_CurrentPokeTarget && base.CanSelect(interactable);
        }

        /// <summary>
        /// Evaluates whether or not an attempted poke interaction is valid.
        /// </summary>
        /// <param name="newHoveredInteractable"> The newly hovered interactable. </param>
        /// <returns>
        /// Returns <see langword="true"/> if poke interaction can be completed.
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        bool EvaluatePokeInteraction(out IXRSelectInteractable newHoveredInteractable)
        {
            newHoveredInteractable = null;
            int sphereOverlapCount = EvaluateSphereOverlap();
            bool hasOverlap = sphereOverlapCount > 0;
            bool canCompletePokeInteraction = false;

            if (hasOverlap)
            {
                var smallestSqrDistance = float.MaxValue;
                int pokeTargetsCount = m_PokeTargets.Count;
                IXRSelectInteractable closestInteractable = null;

                for (var i = 0; i < pokeTargetsCount; ++i)
                {
                    var interactable = m_PokeTargets[i].interactable;
                    if (interactable is IXRSelectInteractable selectable &&
                        interactable is IXRHoverInteractable hoverable && hoverable.IsHoverableBy(this))
                    {
                        var sqrDistance = interactable.GetDistanceSqrToInteractor(this);
                        if (sqrDistance < smallestSqrDistance)
                        {
                            smallestSqrDistance = sqrDistance;
                            closestInteractable  = selectable;
                        }
                    }
                }

                if (closestInteractable != null)
                {
                    canCompletePokeInteraction = true;
                    newHoveredInteractable = closestInteractable;
                }
            }

            return canCompletePokeInteraction;
        }

        int EvaluateSphereOverlap()
        {
            m_PokeTargets.Clear();

            // Hover Check
            Vector3 pokeInteractionPoint = GetAttachTransform(null).position;
            Vector3 interFramePokeStart = m_LastPokeInteractionPoint;
            Vector3 interFrameEnd = pokeInteractionPoint;

            Vector3 interFrameDirectionVector = pokeInteractionPoint - m_LastPokeInteractionPoint;
            float interFrameSqrMagnitude = math.distancesq(pokeInteractionPoint, m_LastPokeInteractionPoint);

            // If no movement is recorded.
            // Check if spherecast size is sufficient for proper cast, or if first frame since last frame poke position will be invalid.
            if (interFrameSqrMagnitude <= 0.001f || m_FirstFrame)
            {
                int numberOfOverlaps = m_LocalPhysicsScene.OverlapSphere(interFrameEnd, m_PokeHoverRadius, m_OverlapSphereHits,
                    interactionLayers, QueryTriggerInteraction.UseGlobal);

                for (var i = 0; i < numberOfOverlaps; ++i)
                {
                    if(FindPokeTarget(m_OverlapSphereHits[i], out var newPokeCollision))
                    {
                        m_PokeTargets.Add(newPokeCollision);
                    }
                }
            }
            else
            {
                int numberOfOverlaps = m_LocalPhysicsScene.SphereCast(
                    interFramePokeStart,
                    m_PokeHoverRadius,
                    interFrameDirectionVector.normalized,
                    m_SphereCastHits,
                    math.sqrt(interFrameSqrMagnitude),
                    interactionLayers);

                if (numberOfOverlaps > 0)
                {
                    for (var i = 0; i < numberOfOverlaps; ++i)
                    {
                        if(FindPokeTarget(m_SphereCastHits[i].collider, out var newPokeCollision))
                        {
                            m_PokeTargets.Add(newPokeCollision);
                        }
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
                if (m_RequirePokeFilter)
                {
                    if (interactable is XRBaseInteractable baseInteractable)
                    {
                        baseInteractable.selectFilters.GetAll(m_InteractableSelectFilters);
                        foreach (var filter in m_InteractableSelectFilters)
                        {
                            if (filter is XRPokeFilter pokeFilter && filter.canProcess)
                            {
                                newPokeCollision = new PokeCollision(hitCollider, interactable, pokeFilter);
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    newPokeCollision = new PokeCollision(hitCollider, interactable, null);
                    return true;
                }
            }
            return false;
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

        T GetOrAddComponent<T>() where T : Component => GetOrAddComponent<T>(gameObject);

        static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            return go.TryGetComponent<T>(out var component) ? component : go.AddComponent<T>();
        }

        readonly struct PokeCollision
        {
            public readonly Collider collider;
            public readonly IXRInteractable interactable;
            public readonly XRPokeFilter filter;
            public readonly bool hasPokeFilter;

            public PokeCollision(Collider collider, IXRInteractable interactable, XRPokeFilter filter)
            {
                this.collider = collider;
                this.interactable = interactable;
                this.filter = filter;
                this.hasPokeFilter = filter != null;
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
            var position = pokeInteractionTransform.position;
            var orientation = pokeInteractionTransform.rotation;
            Vector3 startPoint = position;
            Vector3 penetrationDirection = orientation * Vector3.forward;
            Vector3 endPoint = startPoint + (penetrationDirection * m_PokeDepth);

            model.position = position;
            model.orientation = orientation;
            model.positionGetter = m_PositionGetter;
            model.select = TrackedDeviceGraphicRaycaster.HasPokeSelect(this);
            model.raycastLayerMask = m_PhysicsLayerMask;
            model.pokeDepth = m_PokeDepth;
            model.interactionType = UIInteractionType.Poke;

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
            return m_RegisteredUIInteractorCache.TryGetUIModel(out model);
        }
    }
}
