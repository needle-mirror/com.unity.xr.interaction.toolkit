using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// An interactable that teleports the user to a specific position and/or rotation defined by one of several anchors.
    /// The volume designates a destination anchor upon first hover based on a <see cref="ITeleportationVolumeAnchorFilter"/>.
    /// </summary>
    /// <seealso cref="ITeleportationVolumeAnchorFilter"/>
    /// <seealso cref="TeleportationAnchor"/>
    [AddComponentMenu("XR/Teleportation Multi-Anchor Volume", 11)]
    [HelpURL(XRHelpURLConstants.k_TeleportationMultiAnchorVolume)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportationMultiAnchorVolume : BaseTeleportationInteractable
    {
        [SerializeField]
        [Tooltip("The transforms that represent the possible teleportation destinations.")]
        List<Transform> m_AnchorTransforms = new List<Transform>();

        /// <summary>
        /// The transforms that represent the possible teleportation destinations.
        /// </summary>
        public List<Transform> anchorTransforms => m_AnchorTransforms;

        [SerializeField]
        [Tooltip("Settings for how this volume evaluates a destination anchor.")]
        TeleportVolumeDestinationSettingsDatumProperty m_DestinationEvaluationSettings =
            new TeleportVolumeDestinationSettingsDatumProperty(new TeleportVolumeDestinationSettings());

        /// <summary>
        /// Settings for how this volume evaluates a destination anchor.
        /// </summary>
        public TeleportVolumeDestinationSettingsDatumProperty destinationEvaluationSettings
        {
            get => m_DestinationEvaluationSettings;
            set => m_DestinationEvaluationSettings = value;
        }

        /// <summary>
        /// The filter used to evaluate a teleportation destination from the list of anchors. This is the same as
        /// the <see cref="TeleportVolumeDestinationSettings.destinationEvaluationFilter"/> in <see cref="destinationEvaluationSettings"/>
        /// unless that value is <see langword="null"/>, in which case this will return an instance of <see cref="FurthestTeleportationAnchorFilter"/>.
        /// </summary>
        public ITeleportationVolumeAnchorFilter destinationEvaluationFilter
        {
            get
            {
                var filterInSettings = m_DestinationEvaluationSettings.Value.destinationEvaluationFilter;
                if (filterInSettings != null)
                    return filterInSettings;

                return m_DefaultAnchorFilterCache;
            }
        }

        /// <summary>
        /// A normalized representation of the current progress towards evaluating a destination anchor, depending on
        /// <see cref="destinationEvaluationSettings"/>. This value is 0 while not hovered. If <see cref="TeleportVolumeDestinationSettings.enableDestinationEvaluationDelay"/>
        /// is <see langword="true"/>, this value will start at 0 when initiating destination evaluation (either upon first hover
        /// or when re-evaluation occurs due to <see cref="TeleportVolumeDestinationSettings.pollForDestinationChange"/>)
        /// and will increase to 1 over the course of <see cref="TeleportVolumeDestinationSettings.destinationEvaluationDelayTime"/>
        /// seconds while hovered. Otherwise, this value will be 1 while hovered. This resets to 0 after the volume creates a teleport request.
        /// </summary>
        /// <seealso cref="TeleportVolumeDestinationSettings.enableDestinationEvaluationDelay"/>
        /// <seealso cref="TeleportVolumeDestinationSettings.destinationEvaluationDelayTime"/>
        public float destinationEvaluationProgress { get; private set; }

        /// <summary>
        /// The transform representing the current teleportation destination. When <see cref="destinationEvaluationProgress"/>
        /// is 1, this will be one of the transforms in <see cref="anchorTransforms"/>. Otherwise, this will be <see langword="null"/>.
        /// </summary>
        public Transform destinationAnchor { get; private set; }

        /// <summary>
        /// Calls the methods in its invocation list when the <see cref="destinationAnchor"/> changes.
        /// </summary>
        public event Action<TeleportationMultiAnchorVolume> destinationAnchorChanged;

        bool shouldDelayDestinationEvaluation
        {
            get
            {
                var settingsValue = m_DestinationEvaluationSettings.Value;
                return settingsValue.enableDestinationEvaluationDelay && settingsValue.destinationEvaluationDelayTime > 0f;
            }
        }

        ITeleportationVolumeAnchorFilter m_DefaultAnchorFilterCache;

        bool m_WaitingToEvaluateDestination;
        float m_WaitStartTime;

        float m_LastDestinationQueryTime;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            foreach (var anchorTransform in m_AnchorTransforms)
            {
                Gizmos.color = Color.blue;
                GizmoHelpers.DrawWireCubeOriented(anchorTransform.position, anchorTransform.rotation, 1f);
                GizmoHelpers.DrawAxisArrows(anchorTransform, 1f);
            }
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            m_DefaultAnchorFilterCache = DefaultDestinationFilterCache.SubscribeAndGetInstance(this);
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            DefaultDestinationFilterCache.Unsubscribe(this);
        }

        /// <inheritdoc />
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            // Only evaluate destination upon first hover
            if (interactorsHovering.Count != 1)
                return;

            ClearDestinationAnchor();
            if (shouldDelayDestinationEvaluation)
            {
                m_WaitingToEvaluateDestination = true;
                m_WaitStartTime = Time.time;
            }
            else
            {
                EvaluateDestinationAnchor();
            }
        }

        /// <inheritdoc />
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (!isHovered)
            {
                m_WaitingToEvaluateDestination = false;
                ClearDestinationAnchor();
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || !isHovered)
                return;

            var settings = m_DestinationEvaluationSettings.Value;
            if (m_WaitingToEvaluateDestination)
            {
                destinationEvaluationProgress = (Time.time - m_WaitStartTime) / settings.destinationEvaluationDelayTime;
                if (destinationEvaluationProgress >= 1f)
                {
                    m_WaitingToEvaluateDestination = false;
                    EvaluateDestinationAnchor();
                }

                return;
            }

            if (settings.pollForDestinationChange && Time.time - m_LastDestinationQueryTime > settings.destinationPollFrequency)
            {
                m_LastDestinationQueryTime = Time.time;
                var anchorIndex = destinationEvaluationFilter.GetDestinationAnchorIndex(this);
                if (anchorIndex >= 0 && anchorIndex < m_AnchorTransforms.Count &&
                    m_AnchorTransforms[anchorIndex] == destinationAnchor)
                {
                    return;
                }

                // Re-evaluate destination if the filter chooses an anchor that is different from the current destination
                ClearDestinationAnchor();
                if (shouldDelayDestinationEvaluation)
                {
                    m_WaitingToEvaluateDestination = true;
                    m_WaitStartTime = Time.time;
                }
                else
                {
                    destinationEvaluationProgress = 1f;
                    if (anchorIndex >= 0 && anchorIndex < m_AnchorTransforms.Count)
                        SetDestinationAtValidIndex(anchorIndex);
                }
            }
        }

        void EvaluateDestinationAnchor()
        {
            destinationEvaluationProgress = 1f;
            m_LastDestinationQueryTime = Time.time;
            var anchorIndex = destinationEvaluationFilter.GetDestinationAnchorIndex(this);
            if (anchorIndex >= 0 && anchorIndex < m_AnchorTransforms.Count)
                SetDestinationAtValidIndex(anchorIndex);
        }

        void SetDestinationAtValidIndex(int anchorIndex)
        {
            destinationAnchor = m_AnchorTransforms[anchorIndex];
            destinationAnchorChanged?.Invoke(this);
        }

        void ClearDestinationAnchor()
        {
            destinationAnchor = null;
            destinationEvaluationProgress = 0f;
            destinationAnchorChanged?.Invoke(this);
        }

        /// <inheritdoc />
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            return destinationAnchor != null ? destinationAnchor : base.GetAttachTransform(interactor);
        }

        /// <inheritdoc />
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (destinationAnchor == null)
                return false;

            teleportRequest.destinationPosition = destinationAnchor.position;
            teleportRequest.destinationRotation = destinationAnchor.rotation;
            ClearDestinationAnchor();
            return true;
        }

        static class DefaultDestinationFilterCache
        {
            static FurthestTeleportationAnchorFilter s_FilterInstance;
            static readonly HashSet<TeleportationMultiAnchorVolume> s_Users = new HashSet<TeleportationMultiAnchorVolume>();

            public static ITeleportationVolumeAnchorFilter SubscribeAndGetInstance(TeleportationMultiAnchorVolume user)
            {
                s_Users.Add(user);
                if (s_FilterInstance == null)
                    s_FilterInstance = ScriptableObject.CreateInstance<FurthestTeleportationAnchorFilter>();

                return s_FilterInstance;
            }

            public static void Unsubscribe(TeleportationMultiAnchorVolume user)
            {
                s_Users.Remove(user);
                if (s_Users.Count == 0)
                    Destroy(s_FilterInstance);
            }
        }
    }
}