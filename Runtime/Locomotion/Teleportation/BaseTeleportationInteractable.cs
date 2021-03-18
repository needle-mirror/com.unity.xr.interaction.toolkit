using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The option of which object's orientation in the rig will be matched with the destination after teleporting.
    /// </summary>
    public enum MatchOrientation
    {
        /// <summary>
        /// After teleporting the XR Rig will be positioned such that its up vector matches world space up.
        /// </summary>
        WorldSpaceUp,

        /// <summary>
        /// After teleporting the XR Rig will be positioned such that its up vector matches target up.
        /// </summary>
        TargetUp,

        /// <summary>
        /// After teleporting the XR Rig will be positioned such that its up vector matches target up and forward.
        /// </summary>
        TargetUpAndForward,

        /// <summary>
        /// After teleporting the XR Rig will not attempt to match any orientation.
        /// </summary>
        None,
    }

    /// <summary>
    /// The Teleport Request that describes the result of the teleportation action. Each Teleportation Interactable must fill out a Teleport Request
    /// for each teleport action.
    /// </summary>
    public struct TeleportRequest
    {
        /// <summary>
        /// The position in world space of the Teleportation Destination
        /// </summary>
        public Vector3 destinationPosition;
        /// <summary>
        /// The rotation in world space of the Teleportation Destination, This is used primarily for matching world rotations directly
        /// </summary>
        public Quaternion destinationRotation;
        /// <summary>
        ///  The Time (in unix epoch) of the request
        /// </summary>
        public float requestTime;
        /// <summary>
        /// The option of how to orient the rig after teleportation.
        /// </summary>
        public MatchOrientation matchOrientation;
    }

    /// <summary>
    /// This is intended to be the base class for all Teleportation Interactables. This abstracts the teleport request process for specializations of this class.
    /// </summary>
    public abstract class BaseTeleportationInteractable : XRBaseInteractable
    {
        /// <summary>
        /// Indicates when the teleportation action happens.
        /// </summary>
        public enum TeleportTrigger
        {
            /// <summary>
            /// Teleportation occurs once selection is released without being canceled.
            /// </summary>
            OnSelectExited,

            /// <summary>
            /// Teleportation occurs right when area is selected.
            /// </summary>
            OnSelectEntered,

            /// <summary>
            /// Teleportation occurs on activate.
            /// </summary>
            OnActivated,

            /// <summary>
            /// Teleportation occurs on deactivate.
            /// </summary>
            OnDeactivated,

            /// <summary>
            /// Teleportation occurs once selection is released without being canceled.
            /// </summary>
            [Obsolete("OnSelectExit has been deprecated. Use OnSelectExited instead. (UnityUpgradable) -> OnSelectExited")]
            OnSelectExit = OnSelectExited,

            /// <summary>
            /// Teleportation occurs right when area is selected.
            /// </summary>
            [Obsolete("OnSelectEnter has been deprecated. Use OnSelectEntered instead. (UnityUpgradable) -> OnSelectEntered")]
            OnSelectEnter = OnSelectEntered,

            /// <summary>
            /// Teleportation occurs on activate.
            /// </summary>
            [Obsolete("OnActivate has been deprecated. Use OnActivated instead. (UnityUpgradable) -> OnActivated")]
            OnActivate = OnActivated,

            /// <summary>
            /// Teleportation occurs on deactivate.
            /// </summary>
            [Obsolete("OnDeactivate has been deprecated. Use OnDeactivated instead. (UnityUpgradable) -> OnDeactivated")]
            OnDeactivate = OnDeactivated,
        }

        [SerializeField]
        [Tooltip("The teleportation provider that this teleportation interactable will communicate teleport requests to." +
            " If no teleportation provider is configured, will attempt to find a teleportation provider during Awake.")]
        TeleportationProvider m_TeleportationProvider;

        /// <summary>
        /// The teleportation provider that this teleportation interactable will communicate teleport requests to.
        /// If no teleportation provider is configured, will attempt to find a teleportation provider during Awake.
        /// </summary>
        public TeleportationProvider teleportationProvider
        {
            get => m_TeleportationProvider;
            set => m_TeleportationProvider = value;
        }

        [SerializeField]
        [Tooltip("How to orient the rig after teleportation." +
            "\nSet to:" +
            "\n\nWorld Space Up to stay oriented according to the world space up vector." +
            "\n\nSet to Target Up to orient according to the target BaseTeleportationInteractable Transform's up vector." +
            "\n\nSet to Target Up And Forward to orient according to the target BaseTeleportationInteractable Transform's rotation." +
            "\n\nSet to None to maintain the same orientation before and after teleporting.")]
        MatchOrientation m_MatchOrientation = MatchOrientation.WorldSpaceUp;

        /// <summary>
        /// How to orient the rig after teleportation.
        /// </summary>
        /// <remarks>
        /// Set to:
        /// <list type="bullet">
        /// <item><see cref="MatchOrientation.WorldSpaceUp"/> to stay oriented according to the world space up vector.</item>
        /// <item><see cref="MatchOrientation.TargetUp"/> to orient according to the target <see cref="BaseTeleportationInteractable"/> Transform's up vector.</item>
        /// <item><see cref="MatchOrientation.TargetUpAndForward"/> to orient according to the target <see cref="BaseTeleportationInteractable"/> Transform's rotation.</item>
        /// <item><see cref="MatchOrientation.None"/> to maintain the same orientation before and after teleporting.</item>
        /// </list>
        /// </remarks>
        public MatchOrientation matchOrientation
        {
            get => m_MatchOrientation;
            set => m_MatchOrientation = value;
        }

        [SerializeField]
        [Tooltip("Specify when the teleportation will be triggered. Options map to when the trigger is pressed or when it is released.")]
        TeleportTrigger m_TeleportTrigger = TeleportTrigger.OnSelectExited;

        /// <summary>
        /// Specifies when the teleportation will be triggered.
        /// </summary>
        public TeleportTrigger teleportTrigger
        {
            get => m_TeleportTrigger;
            set => m_TeleportTrigger = value;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            if (m_TeleportationProvider == null)
            {
                m_TeleportationProvider = FindObjectOfType<TeleportationProvider>();
            }
        }

        /// <summary>
        /// Generates a teleport request.
        /// </summary>
        /// <param name="interactor">The interactor generating the teleport request.</param>
        /// <param name="raycastHit">The raycast hit where user will be teleported to.</param>
        /// <param name="teleportRequest">The teleport request.</param>
        /// <returns>Returns <see langword="true"/> if button teleportation was successful. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
            => false;

        void SendTeleportRequest(XRBaseInteractor interactor)
        {
            if (!interactor || m_TeleportationProvider == null)
                return;

            var rayInt = interactor as XRRayInteractor;
            if (rayInt != null)
            {
                if (rayInt.TryGetCurrent3DRaycastHit(out var raycastHit))
                {
                    // Are we still selecting this object?
                    var found = false;
                    foreach (var interactionCollider in colliders)
                    {
                        if (interactionCollider == raycastHit.collider)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        var tr = new TeleportRequest
                        {
                            matchOrientation = m_MatchOrientation,
                            requestTime = Time.time,
                        };
                        if (GenerateTeleportRequest(interactor, raycastHit, ref tr))
                        {
                            m_TeleportationProvider.QueueTeleportRequest(tr);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected internal override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (m_TeleportTrigger == TeleportTrigger.OnSelectEntered)
                SendTeleportRequest(args.interactor);

            base.OnSelectEntered(args);
        }

        /// <inheritdoc />
        protected internal override void OnSelectExited(SelectExitEventArgs args)
        {
            if (m_TeleportTrigger == TeleportTrigger.OnSelectExited && !args.isCanceled)
                SendTeleportRequest(args.interactor);

            base.OnSelectExited(args);
        }

        /// <inheritdoc />
        protected internal override void OnActivated(ActivateEventArgs args)
        {
            if (m_TeleportTrigger == TeleportTrigger.OnActivated)
                SendTeleportRequest(args.interactor);

            base.OnActivated(args);
        }

        /// <inheritdoc />
        protected internal override void OnDeactivated(DeactivateEventArgs args)
        {
            if (m_TeleportTrigger == TeleportTrigger.OnDeactivated)
                SendTeleportRequest(args.interactor);

            base.OnDeactivated(args);
        }
    }
}
