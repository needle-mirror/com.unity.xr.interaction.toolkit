using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit.Utilities;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Behaviour implementation of <see cref="IXRInteractionGroup"/>. An Interaction Group hooks into the interaction system
    /// (via <see cref="XRInteractionManager"/>) and enforces that only one <see cref="IXRGroupMember"/> within the Group
    /// can interact at a time. Each Group member must be either an <see cref="IXRInteractor"/> or an <see cref="IXRInteractionGroup"/>.
    /// </summary>
    /// <remarks>
    /// The member prioritized for interaction in any given frame is whichever member was interacting the previous frame
    /// if it can select in the current frame. If there is no such member, then the interacting member is whichever one
    /// in the ordered list of members interacts first.
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/XR Interaction Group", 11)]
    [HelpURL(XRHelpURLConstants.k_XRInteractionGroup)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_InteractionGroups)]
    public class XRInteractionGroup : MonoBehaviour, IXRInteractionGroup, IXRGroupMember
    {
        /// <inheritdoc />
        public event Action<InteractionGroupRegisteredEventArgs> registered;

        /// <inheritdoc />
        public event Action<InteractionGroupUnregisteredEventArgs> unregistered;

        [SerializeField]
        [Tooltip("The XR Interaction Manager that this Interaction Group will communicate with (will find one if not set manually).")]
        XRInteractionManager m_InteractionManager;

        XRInteractionManager m_RegisteredInteractionManager;

        /// <summary>
        /// The <see cref="XRInteractionManager"/> that this Interaction Group will communicate with (will find one if <see langword="null"/>).
        /// </summary>
        public XRInteractionManager interactionManager
        {
            get => m_InteractionManager;
            set
            {
                m_InteractionManager = value;
                if (Application.isPlaying && isActiveAndEnabled)
                    RegisterWithInteractionManager();
            }
        }

        /// <inheritdoc />
        public IXRInteractionGroup containingGroup { get; private set; }

        [SerializeField]
        [Tooltip("Ordered list of Interactors or Interaction Groups that are registered with the Group on Awake.")]
        [RequireInterface(typeof(IXRGroupMember))]
        List<Object> m_StartingGroupMembers = new List<Object>();

        /// <summary>
        /// Ordered list of Interactors or Interaction Groups that are registered with the Group on Awake.
        /// All objects in this list should implement the <see cref="IXRGroupMember"/> interface and either the
        /// <see cref="IXRInteractor"/> interface or the <see cref="IXRInteractionGroup"/> interface.
        /// </summary>
        /// <remarks>
        /// There are separate methods to access and modify the Group members used after Awake.
        /// </remarks>
        /// <seealso cref="AddGroupMember"/>
        /// <seealso cref="MoveGroupMemberTo"/>
        /// <seealso cref="RemoveGroupMember"/>
        /// <seealso cref="ClearGroupMembers"/>
        /// <seealso cref="ContainsGroupMember"/>
        /// <seealso cref="GetGroupMembers"/>
        public List<Object> startingGroupMembers
        {
            get => m_StartingGroupMembers;
            set => m_StartingGroupMembers = value;
        }

        /// <inheritdoc />
        public IXRInteractor activeInteractor { get; private set; }

        readonly RegistrationList<IXRGroupMember> m_GroupMembers = new RegistrationList<IXRGroupMember>();
        readonly List<IXRGroupMember> m_TempGroupMembers = new List<IXRGroupMember>();
        bool m_IsProcessingGroupMembers;

        readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();
        readonly List<XRBaseInteractable> m_DeprecatedValidTargets = new List<XRBaseInteractable>();

        /// <summary>
        /// Cached reference to an <see cref="XRInteractionManager"/> found with <see cref="Object.FindObjectOfType{Type}()"/>.
        /// </summary>
        static XRInteractionManager s_InteractionManagerCache;

        static readonly List<IXRSelectInteractable> s_InteractablesSelected = new List<IXRSelectInteractable>();
        static readonly List<IXRHoverInteractable> s_InteractablesHovered = new List<IXRHoverInteractable>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            m_InteractionManager = EditorComponentLocatorUtility.FindSceneComponentOfType<XRInteractionManager>(gameObject);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            // Setup Interaction Manager
            FindCreateInteractionManager();

            // Starting member interactors will be re-registered with the manager below when they are added to the group.
            // Make sure the group is registered first.
            RegisterWithInteractionManager();

            // It is more efficient to add than move, but if there are existing items
            // use move to ensure the correct order dictated by the starting lists.
            if (m_GroupMembers.flushedCount > 0)
            {
                var index = 0;
                foreach (var obj in m_StartingGroupMembers)
                {
                    if (obj != null && obj is IXRGroupMember groupMember)
                        MoveGroupMemberTo(groupMember, index++);
                }
            }
            else
            {
                foreach (var obj in m_StartingGroupMembers)
                {
                    if (obj != null && obj is IXRGroupMember groupMember)
                        AddGroupMember(groupMember);
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            FindCreateInteractionManager();
            RegisterWithInteractionManager();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            UnregisterWithInteractionManager();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            ClearGroupMembers();
        }

        /// <inheritdoc />
        void IXRInteractionGroup.OnRegistered(InteractionGroupRegisteredEventArgs args)
        {
            if (args.manager != m_InteractionManager)
                Debug.LogWarning($"An Interaction Group was registered with an unexpected {nameof(XRInteractionManager)}." +
                                 $" {this} was expecting to communicate with \"{m_InteractionManager}\" but was registered with \"{args.manager}\".", this);

            m_RegisteredInteractionManager = args.manager;

            m_GroupMembers.Flush();
            m_IsProcessingGroupMembers = true;
            foreach (var groupMember in m_GroupMembers.registeredSnapshot)
            {
                if (!m_GroupMembers.IsStillRegistered(groupMember))
                    continue;

                if (groupMember.containingGroup == null)
                    RegisterAsGroupMember(groupMember);
            }

            m_IsProcessingGroupMembers = false;

            registered?.Invoke(args);
        }

        /// <inheritdoc />
        void IXRInteractionGroup.OnBeforeUnregistered()
        {
            m_GroupMembers.Flush();
            m_IsProcessingGroupMembers = true;
            foreach (var groupMember in m_GroupMembers.registeredSnapshot)
            {
                if (!m_GroupMembers.IsStillRegistered(groupMember))
                    continue;

                RegisterAsNonGroupMember(groupMember);
            }

            m_IsProcessingGroupMembers = false;
        }

        /// <inheritdoc />
        void IXRInteractionGroup.OnUnregistered(InteractionGroupUnregisteredEventArgs args)
        {
            if (args.manager != m_RegisteredInteractionManager)
                Debug.LogWarning($"An Interaction Group was unregistered from an unexpected {nameof(XRInteractionManager)}." +
                                 $" {this} was expecting to communicate with \"{m_RegisteredInteractionManager}\" but was unregistered from \"{args.manager}\".", this);

            m_RegisteredInteractionManager = null;
            unregistered?.Invoke(args);
        }

        /// <inheritdoc />
        public void AddGroupMember(IXRGroupMember groupMember)
        {
            if (groupMember == null)
                throw new ArgumentNullException(nameof(groupMember));

            if (!ValidateAddGroupMember(groupMember))
                return;

            if (m_IsProcessingGroupMembers)
                Debug.LogWarning($"{groupMember} added while {name} is processing Group members. It won't be processed until the next process.", this);

            if (m_GroupMembers.Register(groupMember))
                RegisterAsGroupMember(groupMember);
        }

        /// <inheritdoc />
        public void MoveGroupMemberTo(IXRGroupMember groupMember, int newIndex)
        {
            if (groupMember == null)
                throw new ArgumentNullException(nameof(groupMember));

            if (!ValidateAddGroupMember(groupMember))
                return;

            // BaseRegistrationList<T> does not yet support reordering with pending registration changes.
            if (m_IsProcessingGroupMembers)
            {
                Debug.LogError($"Cannot move {groupMember} while {name} is processing Group members.", this);
                return;
            }

            m_GroupMembers.Flush();
            if (m_GroupMembers.MoveItemImmediately(groupMember, newIndex) && groupMember.containingGroup == null)
                RegisterAsGroupMember(groupMember);
        }

        bool ValidateAddGroupMember(IXRGroupMember groupMember)
        {
            if (!(groupMember is IXRInteractor || groupMember is IXRInteractionGroup))
            {
                Debug.LogError($"Group member {groupMember} must be either an Interactor or an Interaction Group.", this);
                return false;
            }

            if (groupMember.containingGroup != null && !ReferenceEquals(groupMember.containingGroup, this))
            {
                Debug.LogError($"Cannot add/move {groupMember} because it is already part of a Group. Remove the member from the Group first.", this);
                return false;
            }

            if (groupMember is IXRInteractionGroup subGroup && subGroup.HasDependencyOnGroup(this))
            {
                Debug.LogError($"Cannot add/move {groupMember} because this would create a circular dependency of groups.", this);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool RemoveGroupMember(IXRGroupMember groupMember)
        {
            if (m_GroupMembers.Unregister(groupMember))
            {
                RegisterAsNonGroupMember(groupMember);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void ClearGroupMembers()
        {
            m_GroupMembers.Flush();
            for (var index = m_GroupMembers.flushedCount - 1; index >= 0; --index)
            {
                var groupMember = m_GroupMembers.GetRegisteredItemAt(index);
                RemoveGroupMember(groupMember);
            }
        }

        /// <inheritdoc />
        public bool ContainsGroupMember(IXRGroupMember groupMember)
        {
            return m_GroupMembers.IsRegistered(groupMember);
        }

        /// <inheritdoc />
        public void GetGroupMembers(List<IXRGroupMember> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            m_GroupMembers.GetRegisteredItems(results);
        }

        /// <inheritdoc />
        public bool HasDependencyOnGroup(IXRInteractionGroup group)
        {
            if (ReferenceEquals(group, this))
                return true;

            GetGroupMembers(m_TempGroupMembers);
            foreach (var groupMember in m_TempGroupMembers)
            {
                if (groupMember is IXRInteractionGroup subGroup && subGroup.HasDependencyOnGroup(group))
                    return true;
            }

            return false;
        }

        void FindCreateInteractionManager()
        {
            if (m_InteractionManager != null)
                return;

            if (s_InteractionManagerCache == null)
                s_InteractionManagerCache = FindObjectOfType<XRInteractionManager>();

            if (s_InteractionManagerCache == null)
            {
                var interactionManagerGO = new GameObject("XR Interaction Manager", typeof(XRInteractionManager));
                s_InteractionManagerCache = interactionManagerGO.GetComponent<XRInteractionManager>();
            }

            m_InteractionManager = s_InteractionManagerCache;
        }

        void RegisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager == m_InteractionManager)
                return;

            UnregisterWithInteractionManager();

            if (m_InteractionManager != null)
            {
                m_InteractionManager.RegisterInteractionGroup(this);
            }
        }

        void UnregisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager == null)
                return;

            m_RegisteredInteractionManager.UnregisterInteractionGroup(this);
        }

        void RegisterAsGroupMember(IXRGroupMember groupMember)
        {
            if (m_RegisteredInteractionManager == null)
                return;

            groupMember.OnRegisteringAsGroupMember(this);
            ReRegisterGroupMemberWithInteractionManager(groupMember);
        }

        void RegisterAsNonGroupMember(IXRGroupMember groupMember)
        {
            if (m_RegisteredInteractionManager == null)
                return;

            groupMember.OnRegisteringAsNonGroupMember();
            ReRegisterGroupMemberWithInteractionManager(groupMember);
        }

        void ReRegisterGroupMemberWithInteractionManager(IXRGroupMember groupMember)
        {
            if (m_RegisteredInteractionManager == null)
                return;

            // Re-register the interactor or group so the manager can update its status as part of a group
            switch (groupMember)
            {
                case IXRInteractor interactor:
                    if (m_RegisteredInteractionManager.IsRegistered(interactor))
                    {
                        m_RegisteredInteractionManager.UnregisterInteractor(interactor);
                        m_RegisteredInteractionManager.RegisterInteractor(interactor);
                    }
                    break;
                case IXRInteractionGroup group:
                    if (m_RegisteredInteractionManager.IsRegistered(group))
                    {
                        m_RegisteredInteractionManager.UnregisterInteractionGroup(group);
                        m_RegisteredInteractionManager.RegisterInteractionGroup(group);
                    }
                    break;
                default:
                    Debug.LogError($"Group member {groupMember} must be either an Interactor or an Interaction Group.", this);
                    break;
            }
        }

        /// <inheritdoc />
        void IXRInteractionGroup.PreprocessGroupMembers(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Flush once at the start of the update phase, and this is the first method invoked by the manager
            m_GroupMembers.Flush();

            m_IsProcessingGroupMembers = true;
            foreach (var groupMember in m_GroupMembers.registeredSnapshot)
            {
                if (!m_GroupMembers.IsStillRegistered(groupMember))
                    continue;

                switch (groupMember)
                {
                    case IXRInteractor interactor:
                        if (!m_RegisteredInteractionManager.IsRegistered(interactor))
                            continue;

                        interactor.PreprocessInteractor(updatePhase);
                        break;
                    case IXRInteractionGroup group:
                        if (!m_RegisteredInteractionManager.IsRegistered(group))
                            continue;

                        group.PreprocessGroupMembers(updatePhase);
                        break;
                }
            }

            m_IsProcessingGroupMembers = false;
        }

        /// <inheritdoc />
        void IXRInteractionGroup.ProcessGroupMembers(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            m_IsProcessingGroupMembers = true;
            foreach (var groupMember in m_GroupMembers.registeredSnapshot)
            {
                if (!m_GroupMembers.IsStillRegistered(groupMember))
                    continue;

                switch (groupMember)
                {
                    case IXRInteractor interactor:
                        if (!m_RegisteredInteractionManager.IsRegistered(interactor))
                            continue;

                        interactor.ProcessInteractor(updatePhase);
                        break;
                    case IXRInteractionGroup group:
                        if (!m_RegisteredInteractionManager.IsRegistered(group))
                            continue;

                        group.ProcessGroupMembers(updatePhase);
                        break;
                }
            }

            m_IsProcessingGroupMembers = false;
        }

        /// <inheritdoc />
        void IXRInteractionGroup.UpdateGroupMemberInteractions()
        {
            // Prioritize previous active interactor if it can select this frame
            IXRInteractor prePrioritizedInteractor = null;
            if (activeInteractor != null && m_RegisteredInteractionManager.IsRegistered(activeInteractor) &&
                activeInteractor is IXRSelectInteractor activeSelectInteractor &&
                CanStartOrContinueAnySelect(activeSelectInteractor))
            {
                prePrioritizedInteractor = activeInteractor;
            }

            ((IXRInteractionGroup)this).UpdateGroupMemberInteractions(prePrioritizedInteractor, out var interactorThatPerformedInteraction);
            activeInteractor = interactorThatPerformedInteraction;
        }

        bool CanStartOrContinueAnySelect(IXRSelectInteractor selectInteractor)
        {
            if (selectInteractor.keepSelectedTargetValid)
            {
                foreach (var interactable in selectInteractor.interactablesSelected)
                {
                    if (m_RegisteredInteractionManager.CanSelect(selectInteractor, interactable))
                        return true;
                }
            }

            m_RegisteredInteractionManager.GetValidTargets(selectInteractor, m_ValidTargets);
            foreach (var target in m_ValidTargets)
            {
                if (!(target is IXRSelectInteractable selectInteractable))
                    continue;

                if (m_RegisteredInteractionManager.CanSelect(selectInteractor, selectInteractable))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        void IXRInteractionGroup.UpdateGroupMemberInteractions(IXRInteractor prePrioritizedInteractor, out IXRInteractor interactorThatPerformedInteraction)
        {
            interactorThatPerformedInteraction = null;
            m_IsProcessingGroupMembers = true;
            foreach (var groupMember in m_GroupMembers.registeredSnapshot)
            {
                if (!m_GroupMembers.IsStillRegistered(groupMember))
                    continue;

                switch (groupMember)
                {
                    case IXRInteractor interactor:
                        if (!m_RegisteredInteractionManager.IsRegistered(interactor))
                            continue;

                        var preventInteraction = prePrioritizedInteractor != null && interactor != prePrioritizedInteractor;
                        UpdateInteractorInteractions(interactor, preventInteraction, out var performedInteraction);
                        if (performedInteraction)
                        {
                            interactorThatPerformedInteraction = interactor;
                            prePrioritizedInteractor = interactor;
                        }

                        break;
                    case IXRInteractionGroup group:
                        if (!m_RegisteredInteractionManager.IsRegistered(group))
                            continue;

                        group.UpdateGroupMemberInteractions(prePrioritizedInteractor, out var interactorInSubGroupThatPerformedInteraction);
                        if (interactorInSubGroupThatPerformedInteraction != null)
                        {
                            interactorThatPerformedInteraction = interactorInSubGroupThatPerformedInteraction;
                            prePrioritizedInteractor = interactorInSubGroupThatPerformedInteraction;
                        }

                        break;
                }
            }

            m_IsProcessingGroupMembers = false;
            activeInteractor = interactorThatPerformedInteraction;
        }

        void UpdateInteractorInteractions(IXRInteractor interactor, bool preventInteraction, out bool performedInteraction)
        {
            performedInteraction = false;

            using (XRInteractionManager.s_GetValidTargetsMarker.Auto())
                m_RegisteredInteractionManager.GetValidTargets(interactor, m_ValidTargets);

            // Cast to the abstract base classes to assist with backwards compatibility with existing user code.
            XRInteractionManager.GetOfType(m_ValidTargets, m_DeprecatedValidTargets);

            var selectInteractor = interactor as IXRSelectInteractor;
            var hoverInteractor = interactor as IXRHoverInteractor;

            if (selectInteractor != null)
            {
                using (XRInteractionManager.s_EvaluateInvalidSelectionsMarker.Auto())
                {
                    if (preventInteraction)
                        ClearAllInteractorSelections(selectInteractor);
                    else
                        m_RegisteredInteractionManager.ClearInteractorSelectionInternal(selectInteractor, m_ValidTargets);
                }
            }

            if (hoverInteractor != null)
            {
                using (XRInteractionManager.s_EvaluateInvalidHoversMarker.Auto())
                {
                    if (preventInteraction)
                        ClearAllInteractorHovers(hoverInteractor);
                    else
                        m_RegisteredInteractionManager.ClearInteractorHoverInternal(hoverInteractor, m_ValidTargets, m_DeprecatedValidTargets);
                }
            }

            if (preventInteraction)
                return;

            if (selectInteractor != null)
            {
                using (XRInteractionManager.s_EvaluateValidSelectionsMarker.Auto())
                    m_RegisteredInteractionManager.InteractorSelectValidTargetsInternal(selectInteractor, m_ValidTargets, m_DeprecatedValidTargets);

                // Alternatively check if the base interactor is interacting with UGUI
                // TODO move this api call to IUIInteractor for XRI 3.0
                if (selectInteractor.hasSelection || (interactor is XRBaseInteractor baseInteractor && baseInteractor.isInteractingWithUI))
                    performedInteraction = true;
            }

            if (hoverInteractor != null)
            {
                using (XRInteractionManager.s_EvaluateValidHoversMarker.Auto())
                    m_RegisteredInteractionManager.InteractorHoverValidTargetsInternal(hoverInteractor, m_ValidTargets, m_DeprecatedValidTargets);

                if (hoverInteractor.hasHover)
                    performedInteraction = true;
            }
        }

        void ClearAllInteractorSelections(IXRSelectInteractor selectInteractor)
        {
            if (selectInteractor.interactablesSelected.Count == 0)
                return;

            s_InteractablesSelected.Clear();
            s_InteractablesSelected.AddRange(selectInteractor.interactablesSelected);
            for (var i = s_InteractablesSelected.Count - 1; i >= 0; --i)
            {
                var interactable = s_InteractablesSelected[i];
                m_RegisteredInteractionManager.SelectExitInternal(selectInteractor, interactable);
            }
        }

        void ClearAllInteractorHovers(IXRHoverInteractor hoverInteractor)
        {
            if (hoverInteractor.interactablesHovered.Count == 0)
                return;

            s_InteractablesHovered.Clear();
            s_InteractablesHovered.AddRange(hoverInteractor.interactablesHovered);
            for (var i = s_InteractablesHovered.Count - 1; i >= 0; --i)
            {
                var interactable = s_InteractablesHovered[i];
                m_RegisteredInteractionManager.HoverExitInternal(hoverInteractor, interactable);
            }
        }

        /// <inheritdoc />
        void IXRGroupMember.OnRegisteringAsGroupMember(IXRInteractionGroup group)
        {
            if (containingGroup != null)
            {
                Debug.LogError($"{name} is already part of a Group. Remove the member from the Group first.", this);
                return;
            }

            if (!group.ContainsGroupMember(this))
            {
                Debug.LogError($"{nameof(IXRGroupMember.OnRegisteringAsGroupMember)} was called but the Group does not contain {name}. " +
                               "Add the member to the Group rather than calling this method directly.", this);
                return;
            }

            containingGroup = group;
        }

        /// <inheritdoc />
        void IXRGroupMember.OnRegisteringAsNonGroupMember()
        {
            containingGroup = null;
        }
    }
}
