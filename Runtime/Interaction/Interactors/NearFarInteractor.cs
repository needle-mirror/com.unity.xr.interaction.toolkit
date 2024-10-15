using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Interactor that uses two <see cref="IInteractionCaster"/> references to find valid targets.
    /// The near caster is simpler and not assumed to have any line based casting, while the far caster implements <see cref="ICurveInteractionCaster"/>
    /// to find valid targets, and optionally <see cref="IUIModelUpdater"/> which is used to support UI interaction and sort interaction points.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Interactors/Near-Far Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_NearFarInteractor)]
    public class NearFarInteractor : XRBaseInputInteractor, IXRRayProvider, IUIHoverInteractor, ICurveInteractionDataProvider
    {
        /// <summary>
        /// Enum used to keep track of whether the selection is currently occurring in the near-field or far-field region.
        /// </summary>
        /// <seealso cref="selectionRegion"/>
        public enum Region
        {
            /// <summary>
            /// None means no selection is occurring.
            /// </summary>
            None,

            /// <summary>
            /// Selection is occurring with near-field interaction.
            /// The interaction attach controller does not have an offset.
            /// </summary>
            Near,

            /// <summary>
            /// Selection is occurring with far-field interaction.
            /// The interaction attach controller has an offset.
            /// </summary>
            Far,
        }

        /// <summary>
        /// Enum used to determine the strategy used to compute the the distance used to sort valid targets discovered by the near interaction caster.
        /// </summary>
        /// <seealso cref="nearCasterSortingStrategy"/>
        public enum NearCasterSortingStrategy
        {
            /// <summary>
            /// No sorting should be performed.
            /// </summary>
            None,

            /// <summary>
            /// Sorting is performed based on the square distance between the interactor and interactable attach transform world positions.
            /// This is the most efficient sorting strategy.
            /// </summary>
            SquareDistance,

            /// <summary>
            /// Sorting is performed based on the interactable's defined sorting strategy.
            /// The callstack used here is the most extensive one, and performance will depend on the implementation of the interactable's sorting strategy, which defaults to being collider based.
            /// </summary>
            InteractableBased,

            /// <summary>
            /// Sorting is performed based on the square distance between the interactor's attach transform world position and the closest point of that position on the interactable's collider.
            /// This is an expensive sorting strategy and should only be used if there is a need for high fidelity disambiguation between interactables that are very close to each other.
            /// </summary>
            ClosestPointOnCollider,
        }

        [SerializeField]
        [RequireInterface(typeof(IInteractionAttachController))]
        Object m_InteractionAttachController;

        /// <summary>
        /// Reference to the attach controller used to control the attach transform.
        /// </summary>
        public IInteractionAttachController interactionAttachController
        {
            get => m_InteractionAttachControllerObjectRef.Get(m_InteractionAttachController);
            set => m_InteractionAttachControllerObjectRef.Set(ref m_InteractionAttachController, value);
        }

        readonly UnityObjectReferenceCache<IInteractionAttachController, Object> m_InteractionAttachControllerObjectRef = new UnityObjectReferenceCache<IInteractionAttachController, Object>();

        [SerializeField]
        bool m_EnableNearCasting = true;

        /// <summary>
        /// Determines if the near caster will be used to find valid targets for this interactor.
        /// </summary>
        public bool enableNearCasting
        {
            get => m_EnableNearCasting;
            set => m_EnableNearCasting = value;
        }

        [SerializeField]
        [RequireInterface(typeof(IInteractionCaster))]
        Object m_NearInteractionCaster;

        /// <summary>
        /// Reference to the near interaction caster used to find valid targets for this interactor.
        /// </summary>
        public IInteractionCaster nearInteractionCaster
        {
            get => m_NearCasterObjectRef.Get(m_NearInteractionCaster);
            set => m_NearCasterObjectRef.Set(ref m_NearInteractionCaster, value);
        }

        readonly UnityObjectReferenceCache<IInteractionCaster, Object> m_NearCasterObjectRef = new UnityObjectReferenceCache<IInteractionCaster, Object>();

        [SerializeField]
        NearCasterSortingStrategy m_NearCasterSortingStrategy = NearCasterSortingStrategy.SquareDistance;

        /// <summary>
        /// Strategy used to compute the the distance used to sort valid targets discovered by the near interaction caster.
        /// </summary>
        public NearCasterSortingStrategy nearCasterSortingStrategy
        {
            get => m_NearCasterSortingStrategy;
            set => m_NearCasterSortingStrategy = value;
        }

        [SerializeField]
        bool m_SortNearTargetsAfterTargetFilter;

        /// <summary>
        /// If true, the interactor will sort the near caster's valid targets after the <see cref="XRBaseInteractor.targetFilter"/> has been applied.
        /// Generally, the target filter also takes care of sorting, so this option should only be used if the target filter does not sort.
        /// Not used if no target filter is present.
        /// </summary>
        public bool sortNearTargetsAfterTargetFilter
        {
            get => m_SortNearTargetsAfterTargetFilter;
            set => m_SortNearTargetsAfterTargetFilter = value;
        }

        [Space]
        [SerializeField]
        bool m_EnableFarCasting = true;

        /// <summary>
        /// Determines if the far caster will be used to find valid targets for this interactor.
        /// </summary>
        public bool enableFarCasting
        {
            get => m_EnableFarCasting;
            set => m_EnableFarCasting = value;
        }

        [SerializeField]
        [RequireInterface(typeof(ICurveInteractionCaster))]
        Object m_FarInteractionCaster;

        /// <summary>
        /// Reference to the far interaction caster used to find valid targets for this interactor.
        /// </summary>
        public ICurveInteractionCaster farInteractionCaster
        {
            get => m_FarCasterObjectRef.Get(m_FarInteractionCaster);
            set => m_FarCasterObjectRef.Set(ref m_FarInteractionCaster, value);
        }

        readonly UnityObjectReferenceCache<ICurveInteractionCaster, Object> m_FarCasterObjectRef = new UnityObjectReferenceCache<ICurveInteractionCaster, Object>();

        [SerializeField]
        InteractorFarAttachMode m_FarAttachMode = InteractorFarAttachMode.Far;

        /// <summary>
        /// Determines how the attach transform is adjusted on far select. This typically results in whether
        /// the interactable stays distant at the far hit point or moves to the near hand.
        /// </summary>
        public InteractorFarAttachMode farAttachMode
        {
            get => m_FarAttachMode;
            set => m_FarAttachMode = value;
        }

        [SerializeField]
        bool m_EnableUIInteraction = true;

        /// <summary>
        /// Enable to affect Unity UI GameObjects in a way that is similar to a mouse pointer.
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
                    m_RegisteredUIInteractorCache?.RegisterOrUnregisterXRUIInputModule(m_EnableUIInteraction);
                }
            }
        }

        [SerializeField]
        bool m_BlockUIOnInteractableSelection = true;

        /// <summary>
        /// Enabling this option will block UI interaction when selecting interactables.
        /// </summary>
        public bool blockUIOnInteractableSelection
        {
            get => m_BlockUIOnInteractableSelection;
            set => m_BlockUIOnInteractableSelection = value;
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

        [SerializeField]
        XRInputButtonReader m_UIPressInput = new XRInputButtonReader("UI Press");

        /// <summary>
        /// Input to use for pressing UI elements.
        /// Functions like a mouse button when pointing over UI.
        /// </summary>
        public XRInputButtonReader uiPressInput
        {
            get => m_UIPressInput;
            set => SetInputProperty(ref m_UIPressInput, value);
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_UIScrollInput = new XRInputValueReader<Vector2>("UI Scroll");

        /// <summary>
        /// Input to use for scrolling UI elements.
        /// Functions like a mouse scroll wheel when pointing over UI.
        /// </summary>
        public XRInputValueReader<Vector2> uiScrollInput
        {
            get => m_UIScrollInput;
            set => SetInputProperty(ref m_UIScrollInput, value);
        }

        /// <inheritdoc />
        Transform IXRRayProvider.rayEndTransform => m_RayEndTransform;

        /// <inheritdoc />
        Vector3 IXRRayProvider.rayEndPoint
        {
            get
            {
                if (TryGetCurveEndPoint(out var point) != EndPointType.None)
                    return point;
                return farInteractionCaster.lastSamplePoint;
            }
        }

        /// <summary>
        /// The current region, Near or Far, of the interactable when this interactor has a selection.
        /// When not selecting, the value is <see cref="Region.None"/>.
        /// Exposed as a bindable variable to allow users to subscribe to changes.
        /// </summary>
        /// <remarks>
        /// More technically, this is based on whether the attach transform has an offset or not.
        /// This value can start at <see cref="Region.Far"/> when an interactable hit by the far interaction caster
        /// is selected but change to <see cref="Region.Near"/> as the object is pulled all the way closer.
        /// </remarks>
        /// <seealso cref="Region"/>
        /// <seealso cref="IInteractionAttachController.hasOffset"/>
        public IReadOnlyBindableVariable<Region> selectionRegion => m_SelectionRegion;

        readonly BindableEnum<Region> m_SelectionRegion = new BindableEnum<Region>();

        // Variable use to keep track of which caster last found a valid target.
        Region m_ValidTargetCastSource = Region.None;

        // Variable used to keep track of which caster found the currently selected valid target.
        Region m_SelectedTargetCastSource = Region.None;

        readonly List<Collider> m_TargetColliders = new List<Collider>();
        readonly List<RaycastHit> m_FarRayCastHits = new List<RaycastHit>();
        readonly List<IXRInteractable> m_InternalValidTargets = new List<IXRInteractable>();
        readonly List<XRInteractableSnapVolume> m_InteractableSnapVolumes = new List<XRInteractableSnapVolume>();
        readonly List<IXRInteractable> m_PreFilteredTargets = new List<IXRInteractable>();
        readonly Dictionary<IXRInteractable, int> m_FarTargetToIndexMap = new Dictionary<IXRInteractable, int>();

        // We keep track of whether we released the near interaction this frame to avoid flickering the ray before we get the chance to determine valid targets.
        bool m_ReleasedNearInteractionThisFrame;

        RegisteredUIInteractorCache m_RegisteredUIInteractorCache;

        // Cached reference to the UI model updater
        IUIModelUpdater uiModelUpdater => m_UIModelUpdaterReferenceCache.Get(m_FarInteractionCaster);
        readonly UnityObjectReferenceCache<IUIModelUpdater, Object> m_UIModelUpdaterReferenceCache = new UnityObjectReferenceCache<IUIModelUpdater, Object>();

        bool m_HasValidRayHit;
        Vector3 m_RayEndPoint;
        bool m_ValidHitIsUI;
        Vector3 m_ValidHitNormal;
        Vector3 m_NormalRelativeToInteractable;
        Transform m_RayEndTransform;

        bool isUiSelectInputActive => m_UIPressInput.ReadIsPerformed();
        Vector2 uiScrollInputValue => m_UIScrollInput.ReadValue();

        /// <summary>
        /// Currently this interactor only supports one valid target at a time.
        /// We will investigate expanding support for multiple valid targets in the future.
        /// </summary>
        readonly bool m_AllowMultipleValidTargets = false;

        /// <inheritdoc />
        protected override void Awake()
        {
            // Initialize attach transform in InitializeReferences before letting the base Awake method create it.
            InitializeReferences();
            base.Awake();
            m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);

            buttonReaders.Add(m_UIPressInput);
            valueReaders.Add(m_UIScrollInput);
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache?.RegisterWithXRUIInputModule();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            m_RegisteredUIInteractorCache?.UnregisterFromXRUIInputModule();
            InitializeInteractor();
        }

        /// <summary>
        /// Initializes all required references for this interactor to work.
        /// If components are missing, we try to find local components first, but if none are found, default implementations are added.
        /// </summary>
        protected virtual void InitializeReferences()
        {
            if (farInteractionCaster == null)
            {
                if (TryGetComponent(out ICurveInteractionCaster foundCurveCaster))
                    farInteractionCaster = foundCurveCaster;
                else
                    farInteractionCaster = gameObject.AddComponent<CurveInteractionCaster>();
            }

            if (nearInteractionCaster == null)
            {
                // Since the curve caster implements the same base interface, we have to look in a smarter way.
                // Find the first non-curve caster.
                var casterComponents = GetComponents<IInteractionCaster>();
                IInteractionCaster foundNearCaster = null;
                if (casterComponents.Length > 0)
                {
                    for (var i = 0; i < casterComponents.Length; ++i)
                    {
                        var caster = casterComponents[i];
                        if (caster is ICurveInteractionCaster)
                            continue;

                        foundNearCaster = caster;
                        break;
                    }
                }

                nearInteractionCaster = foundNearCaster ?? gameObject.AddComponent<SphereInteractionCaster>();
            }

            if (interactionAttachController == null)
            {
                if (TryGetComponent(out IInteractionAttachController foundAttachController))
                    interactionAttachController = foundAttachController;
                else
                    interactionAttachController = gameObject.AddComponent<InteractionAttachController>();
            }

            // Create attach transform
            attachTransform = interactionAttachController.GetOrCreateAnchorTransform();
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);

            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                return;

            InitializeInteractor();
            UpdateAnchor();

            var newSelectionRegion = DetermineSelectionRegion();
            EvaluateNearInteraction();
            EvaluateFarInteraction(newSelectionRegion);
            UpdateSelectionRegion(newSelectionRegion);
        }

        void InitializeInteractor()
        {
            m_InternalValidTargets.Clear();
            m_InteractableSnapVolumes.Clear();
            m_TargetColliders.Clear();
            m_FarRayCastHits.Clear();
            m_HasValidRayHit = false;
            m_ValidTargetCastSource = Region.None;
        }

        void UpdateAnchor() => interactionAttachController.DoUpdate(Time.unscaledDeltaTime);

        Region DetermineSelectionRegion()
        {
            if (!hasSelection)
                return Region.None;

            return interactionAttachController.hasOffset ? Region.Far : Region.Near;
        }

        void UpdateSelectionRegion(Region newSelectionRegion)
        {
            m_ReleasedNearInteractionThisFrame = false;
            m_SelectionRegion.Value = newSelectionRegion;
        }

        void EvaluateNearInteraction()
        {
            if (!m_EnableNearCasting || hasSelection || !nearInteractionCaster.TryGetColliderTargets(interactionManager, m_TargetColliders))
                return;

            int registerValidTargets = RegisterNearValidTargets(m_TargetColliders, m_InternalValidTargets);

            if (registerValidTargets > 0)
                m_ValidTargetCastSource = Region.Near;

            // If a target filter is present, sorting is handled by the filter.
            if (targetFilter != null || m_SortNearTargetsAfterTargetFilter)
                return;

            if (registerValidTargets > 1)
            {
                var distanceEvaluator = GetEvaluatorForSortingStrategy(m_NearCasterSortingStrategy);
                if (distanceEvaluator != null)
                    SortingHelpers.SortByDistanceToInteractor(this, m_InternalValidTargets, distanceEvaluator);
            }
        }

        void EvaluateFarInteraction(Region newSelectionRegion)
        {
            bool shouldUpdateFarCast = m_EnableFarCasting || newSelectionRegion == Region.Far;
            if (!shouldUpdateFarCast || m_ValidTargetCastSource == Region.Near)
                return;

            var farCaster = farInteractionCaster;
            bool has3dHit = farCaster.TryGetColliderTargets(interactionManager, m_TargetColliders, m_FarRayCastHits);
            bool has2dHit = TryGetCurrentUIRaycastResult(out var uiHit);
            m_HasValidRayHit = has2dHit || has3dHit;
            m_ValidHitIsUI = false;

            if (!m_HasValidRayHit)
                return;

            m_ValidTargetCastSource = Region.Far;
            var farCasterOrigin = farCaster.samplePoints[0];
            var same2d3dHit = false;

            // Check if 3D hit and 2D hit are the same GameObject
            if (has3dHit && has2dHit)
            {
                var raycast3dHit = m_FarRayCastHits[0];
                same2d3dHit = raycast3dHit.collider != null && uiHit.gameObject == raycast3dHit.collider.gameObject;
            }

            // Do some smarter priority checks here if it has 3D and 2D
            float uiHitSqDistance = has2dHit ? (uiHit.worldPosition - farCasterOrigin).sqrMagnitude : float.MaxValue;
            float collisionHitSqDistance = has3dHit ? (m_FarRayCastHits[0].point - farCasterOrigin).sqrMagnitude : float.MaxValue;

            bool shouldProcess2dHit = has2dHit && (same2d3dHit || !has3dHit || uiHitSqDistance < collisionHitSqDistance);
            bool shouldProcess3dHit = has3dHit && (same2d3dHit || !has2dHit || collisionHitSqDistance < uiHitSqDistance);

            if (shouldProcess3dHit)
                Process3dHit(farCasterOrigin, has2dHit, uiHitSqDistance, ref shouldProcess2dHit);

            if (shouldProcess2dHit)
                Process2dHit(uiHit);
        }

        void Process3dHit(in Vector3 farCasterOrigin, bool has2dHit, float uiHitSqDistance, ref bool shouldProcess2dHit)
        {
            if (!hasSelection)
            {
                if (RegisterFarValidTargets(m_TargetColliders, m_InternalValidTargets, m_InteractableSnapVolumes, out int registeredIndex) > 0)
                {
                    var effectiveHit = m_FarRayCastHits[registeredIndex];
                    m_RayEndPoint = effectiveHit.point;
                    m_ValidHitNormal = effectiveHit.normal;
                    m_RayEndTransform = effectiveHit.transform;

                    if (has2dHit && registeredIndex > 0)
                        shouldProcess2dHit = uiHitSqDistance < (m_RayEndPoint - farCasterOrigin).sqrMagnitude;
                }
                else
                {
                    // If no valid targets were found, and there is a ui hit, we should process the ui hit.
                    if (has2dHit)
                        shouldProcess2dHit = true;
                    // Without any ui hit, this is actually an invalid ray hit.
                    else
                        m_HasValidRayHit = false;
                }
            }
            else
            {
                var closestFar3dHit = m_FarRayCastHits[0];
                m_RayEndPoint = closestFar3dHit.point;
                m_ValidHitNormal = closestFar3dHit.normal;
                m_RayEndTransform = closestFar3dHit.transform;
            }
        }

        void Process2dHit(in RaycastResult uiHit)
        {
            var hitWorldPoint = uiHit.worldPosition;
            m_RayEndPoint = hitWorldPoint;
            m_RayEndTransform = uiHit.gameObject.transform;
            m_ValidHitNormal = uiHit.worldNormal;
            m_ValidHitIsUI = true;
        }

        /// <summary>
        /// Gets the selected interactor distance evaluator strategy used to sort valid interactable targets discovered by the near interaction caster.
        /// The selected sorting strategy can have non-trivial performance implications as the computation will scale according to the quantity of valid targets and the strategy used.
        /// </summary>
        /// <param name="strategy">Strategy to apply.</param>
        /// <returns><see cref="IInteractorDistanceEvaluator"/> instance associated with <see cref="NearCasterSortingStrategy"/> parameter.</returns>
        protected virtual IInteractorDistanceEvaluator GetEvaluatorForSortingStrategy(NearCasterSortingStrategy strategy)
        {
            switch (strategy)
            {
                case NearCasterSortingStrategy.SquareDistance:
                    return SortingHelpers.squareDistanceAttachPointEvaluator;
                case NearCasterSortingStrategy.InteractableBased:
                    return SortingHelpers.interactableBasedEvaluator;
                case NearCasterSortingStrategy.ClosestPointOnCollider:
                    return SortingHelpers.closestPointOnColliderEvaluator;
            }

            return null;
        }

        int RegisterNearValidTargets(List<Collider> targets, List<IXRInteractable> interactables)
        {
            foreach (var target in targets)
            {
                // Add check to prevent multiple valid targets if option is set
                if (interactionManager.TryGetInteractableForCollider(target, out var interactable) &&
                    // Only add interactables that can be hovered
                    interactionManager.IsHoverPossible(this, interactable as IXRHoverInteractable))
                {
                    interactables.Add(interactable);
                }
            }

            if (targetFilter != null)
            {
                m_PreFilteredTargets.Clear();
                m_PreFilteredTargets.AddRange(interactables);
                targetFilter?.Process(this, m_PreFilteredTargets, interactables);
            }

            return interactables.Count;
        }

        int RegisterFarValidTargets(List<Collider> targets, List<IXRInteractable> interactables, List<XRInteractableSnapVolume> snapVolumes, out int firstRegisteredIndex)
        {
            firstRegisteredIndex = -1;
            int numTargets = targets.Count;
            bool foundTarget = false;
            bool hasTargetFilter = targetFilter != null;

            if (hasTargetFilter)
                m_FarTargetToIndexMap.Clear();

            for (int i = 0; i < numTargets; i++)
            {
                bool hasInteractable = interactionManager.TryGetInteractableForCollider(targets[i], out var interactable, out var snapVolume);
                bool isSnapVolume = snapVolume != null;

                // Only add interactables that can be hovered
                bool isHoverPossible = hasInteractable && interactionManager.IsHoverPossible(this, interactable as IXRHoverInteractable);

                if (isHoverPossible)
                {
                    // Mark the first found index
                    if (!foundTarget)
                    {
                        firstRegisteredIndex = i;
                        foundTarget = true;
                    }

                    interactables.Add(interactable);

                    if (isSnapVolume && snapVolume.interactableObject != null)
                        snapVolumes.Add(snapVolume);

                    if (hasTargetFilter)
                        m_FarTargetToIndexMap.TryAdd(interactable, i);
                }

                // If there isn't a target filter, we want to early out if the first target is not a valid interactable.
                // If the target we found is a snap volume for something we don't support, we should ignore it and try for the next target.
                if (!hasTargetFilter && (isHoverPossible || !isSnapVolume))
                    break;
            }

            if (hasTargetFilter)
            {
                m_PreFilteredTargets.Clear();
                m_PreFilteredTargets.AddRange(interactables);
                targetFilter?.Process(this, m_PreFilteredTargets, interactables);
                if (interactables.Count > 0)
                    firstRegisteredIndex = m_FarTargetToIndexMap[interactables[0]];
                else
                    firstRegisteredIndex = -1;
            }

            return interactables.Count;
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            if (m_InternalValidTargets.Count == 0)
                return;

            if (m_AllowMultipleValidTargets)
                targets.AddRange(m_InternalValidTargets);
            else
                targets.Add(m_InternalValidTargets[0]);
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            if (interactablesSelected.Count == 1)
            {
                m_SelectedTargetCastSource = m_ValidTargetCastSource;

                // On a far select, the attach point should either move to the far hit point or stay near.
                // On a near select, the attach point should always stay near.
                var moveToEndPoint = false;
                var endPoint = Vector3.zero;
                if (m_SelectedTargetCastSource == Region.Far && TryGetCurveEndPoint(out endPoint) != EndPointType.None)
                {
                    // Initialize to this interactor's default.
                    moveToEndPoint = m_FarAttachMode == InteractorFarAttachMode.Far;

                    // Interactable can override
                    if (args.interactableObject is IFarAttachProvider farAttachProvider &&
                        farAttachProvider.farAttachMode != InteractableFarAttachMode.DeferToInteractor)
                    {
                        moveToEndPoint = farAttachProvider.farAttachMode == InteractableFarAttachMode.Far;
                    }
                }

                if (moveToEndPoint)
                    interactionAttachController.MoveTo(endPoint);
                else
                    interactionAttachController.ResetOffset();
            }
        }

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (m_SelectedTargetCastSource == Region.Far)
                m_NormalRelativeToInteractable = firstInteractableSelected.GetAttachTransform(this).InverseTransformDirection(m_ValidHitNormal);
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            if (!hasSelection)
            {
                m_ValidTargetCastSource = Region.None;
                m_SelectedTargetCastSource = Region.None;
                m_ReleasedNearInteractionThisFrame = !interactionAttachController.hasOffset;
                interactionAttachController.ResetOffset();
                // Wait to notify until OnSelectExited since that's when we also invoke selectExited,
                // and this gives the interactable a chance to handle deselect before we notify.
                // Unity provided scripts that subscribe don't currently have any need to postpone until later,
                // but this seems better for consistency with event raising.
                m_SelectionRegion.SetValueWithoutNotify(Region.None);
            }
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (!hasSelection)
                m_SelectionRegion.BroadcastValue();
        }

        /// <inheritdoc />
        Transform IXRRayProvider.GetOrCreateAttachTransform() => interactionAttachController.transformToFollow;

        /// <inheritdoc />
        void IXRRayProvider.SetAttachTransform(Transform newAttach) => interactionAttachController.transformToFollow = newAttach;

        /// <inheritdoc />
        Transform IXRRayProvider.GetOrCreateRayOrigin() => farInteractionCaster.castOrigin;

        /// <inheritdoc />
        void IXRRayProvider.SetRayOrigin(Transform newOrigin) => farInteractionCaster.castOrigin = newOrigin;

        /// <inheritdoc />
        public void UpdateUIModel(ref TrackedDeviceModel model)
        {
            if (!isActiveAndEnabled ||
                !m_EnableFarCasting ||
                !m_EnableUIInteraction ||
                uiModelUpdater == null ||
                m_ValidTargetCastSource == Region.Near ||
                // If selecting interactables, don't update UI model.
                (m_BlockUIOnInteractableSelection && hasSelection) ||
                this.IsBlockedByInteractionWithinGroup())
            {
                model.Reset(false);
                return;
            }

            if (!uiModelUpdater.UpdateUIModel(ref model, isUiSelectInputActive, uiScrollInputValue))
                model.Reset(false);
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
        /// Gets the first UI ray cast result, if any ray cast results are available.
        /// </summary>
        /// <param name="raycastResult">When this method returns, contains the UI ray cast result if available; otherwise, the default value.
        /// Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <returns>Returns <see langword="true"/> if a hit occurred, implying the ray cast hit information is valid.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult)
        {
            if (TryGetUIModel(out var model) && model.currentRaycast.isValid)
            {
                raycastResult = model.currentRaycast;
                var raycastEndpointIndex = model.currentRaycastEndpointIndex;
                return raycastEndpointIndex > 0;
            }

            raycastResult = default;
            return false;
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
        bool ICurveInteractionDataProvider.isActive
        {
            get
            {
                bool farSelectMode = selectionRegion.Value == Region.Far;
                bool farCastActive = m_EnableFarCasting || farSelectMode;
                if (!isActiveAndEnabled || !farCastActive || !farInteractionCaster.isInitialized || m_ReleasedNearInteractionThisFrame)
                    return false;

                if (hasSelection)
                    return farSelectMode;

                return m_ValidTargetCastSource != Region.Near && !this.IsBlockedByInteractionWithinGroup();
            }
        }

        /// <inheritdoc />
        bool ICurveInteractionDataProvider.hasValidSelect => m_ValidHitIsUI ? isUiSelectInputActive : hasSelection;

        /// <inheritdoc />
        NativeArray<Vector3> ICurveInteractionDataProvider.samplePoints => farInteractionCaster.samplePoints;

        /// <inheritdoc />
        Vector3 ICurveInteractionDataProvider.lastSamplePoint => farInteractionCaster.lastSamplePoint;

        /// <inheritdoc />
        public Transform curveOrigin => farInteractionCaster.effectiveCastOrigin;

        /// <inheritdoc />
        public EndPointType TryGetCurveEndPoint(out Vector3 endPoint, bool snapToSelectedAttachIfAvailable = false, bool snapToSnapVolumeIfAvailable = false)
        {
            var isFarSelected = interactionAttachController.hasOffset && hasSelection;
            if (snapToSelectedAttachIfAvailable && isFarSelected)
            {
                var targetAttachTransform = firstInteractableSelected.GetAttachTransform(this);
                endPoint = targetAttachTransform.position;
                return EndPointType.AttachPoint;
            }

            if (snapToSnapVolumeIfAvailable && m_InteractableSnapVolumes.Count > 0 && m_InteractableSnapVolumes[0].interactable != null)
            {
                var targetAttachTransform = m_InteractableSnapVolumes[0].interactable.GetAttachTransform(this);
                endPoint = targetAttachTransform.position;
                return EndPointType.AttachPoint;
            }

            endPoint = m_RayEndPoint;
            if (!m_HasValidRayHit)
                return EndPointType.None;
            if (m_ValidHitIsUI)
                return EndPointType.UI;

            return m_InternalValidTargets.Count > 0 || isFarSelected ? EndPointType.ValidCastHit : EndPointType.EmptyCastHit;
        }

        /// <inheritdoc />
        public EndPointType TryGetCurveEndNormal(out Vector3 endNormal, bool snapToSelectedAttachIfAvailable = false)
        {
            var isFarSelected = interactionAttachController.hasOffset && hasSelection;
            if (snapToSelectedAttachIfAvailable && isFarSelected)
            {
                var targetAttachTransform = firstInteractableSelected.GetAttachTransform(this);
                endNormal = targetAttachTransform.TransformDirection(m_NormalRelativeToInteractable);
                return EndPointType.AttachPoint;
            }

            endNormal = m_ValidHitNormal;
            if (!m_HasValidRayHit)
                return EndPointType.None;
            if (m_ValidHitIsUI)
                return EndPointType.UI;

            return m_InternalValidTargets.Count > 0 || isFarSelected ? EndPointType.ValidCastHit : EndPointType.EmptyCastHit;
        }
    }
}
