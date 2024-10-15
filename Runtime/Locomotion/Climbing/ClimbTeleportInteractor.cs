using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
    /// <summary>
    /// Interactor that drives climb locomotion assistance via teleportation. This interactor responds to the user grabbing a
    /// <see cref="ClimbInteractable"/> that references the same <see cref="ClimbProvider"/> as this interactor.
    /// </summary>
    /// <remarks>
    /// If the climb interactable's <see cref="ClimbInteractable.climbAssistanceTeleportVolume"/> is not <see langword="null"/>,
    /// the interactor will hover the teleport volume while the user is grabbing the climb interactable, otherwise the
    /// interactor will do nothing. When climb locomotion ends, the interactor will trigger teleportation to the
    /// evaluated destination by starting and immediately ending either a select or activate interaction, depending on
    /// the teleport volume's <see cref="BaseTeleportationInteractable.teleportTrigger"/>. The interactor will also
    /// stop hovering the teleport volume the next frame after teleporting.
    /// </remarks>
    [AddComponentMenu("XR/Locomotion/Climb Teleport Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_ClimbTeleportInteractor)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class ClimbTeleportInteractor : XRBaseInteractor, IXRActivateInteractor
    {
        [SerializeField]
        [Tooltip("The climb locomotion provider to query for active locomotion and climbed interactable.")]
        ClimbProvider m_ClimbProvider;

        /// <summary>
        /// The climb locomotion provider to query for active locomotion and climbed interactable.
        /// </summary>
        public ClimbProvider climbProvider
        {
            get => m_ClimbProvider;
            set => m_ClimbProvider = value;
        }

        [SerializeField]
        [Tooltip("Optional settings for how the hovered teleport volume evaluates a destination anchor. Applies as an " +
            "override to the teleport volume's settings if set to Use Value or if the asset reference is set.")]
        TeleportVolumeDestinationSettingsDatumProperty m_DestinationEvaluationSettings =
            new TeleportVolumeDestinationSettingsDatumProperty(new TeleportVolumeDestinationSettings
            {
                enableDestinationEvaluationDelay = false,
                pollForDestinationChange = true
            });

        /// <summary>
        /// Optional settings for how the hovered teleport volume evaluates a destination anchor. Applies as an override to
        /// the <see cref="TeleportationMultiAnchorVolume.destinationEvaluationSettings"/> of the climbed interactable's
        /// <see cref="ClimbInteractable.climbAssistanceTeleportVolume"/> if
        /// <see cref="Unity.XR.CoreUtils.Datums.DatumProperty{TValue, TDatum}.Value"/> is not <see langword="null"/>.
        /// </summary>
        public TeleportVolumeDestinationSettingsDatumProperty destinationEvaluationSettings
        {
            get => m_DestinationEvaluationSettings;
            set => m_DestinationEvaluationSettings = value;
        }

        readonly LinkedPool<ActivateEventArgs> m_ActivateEventArgs = new LinkedPool<ActivateEventArgs>(() => new ActivateEventArgs(), collectionCheck: false);
        readonly LinkedPool<DeactivateEventArgs> m_DeactivateEventArgs = new LinkedPool<DeactivateEventArgs>(() => new DeactivateEventArgs(), collectionCheck: false);

        TeleportationMultiAnchorVolume m_TargetTeleportVolume;
        TeleportVolumeDestinationSettingsDatumProperty m_PreservedTeleportVolumeSettings;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_ClimbProvider == null && !ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out m_ClimbProvider))
                return;

            m_ClimbProvider.locomotionStarted += OnClimbBegin;
            m_ClimbProvider.locomotionEnded += OnClimbEnd;
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            ReleaseTargetTeleportVolume();
            if (m_ClimbProvider == null)
                return;

            m_ClimbProvider.locomotionStarted -= OnClimbBegin;
            m_ClimbProvider.locomotionEnded -= OnClimbEnd;
            m_ClimbProvider.climbAnchorUpdated -= OnClimbAnchorUpdated;
        }

        void OnClimbBegin(LocomotionProvider provider)
        {
            SetTargetTeleportVolume(m_ClimbProvider.climbAnchorInteractable);
            m_ClimbProvider.climbAnchorUpdated += OnClimbAnchorUpdated;
        }

        void OnClimbEnd(LocomotionProvider provider)
        {
            m_ClimbProvider.climbAnchorUpdated -= OnClimbAnchorUpdated;
            if (m_TargetTeleportVolume == null)
                return;

            // Force teleport by performing the right action
            switch (m_TargetTeleportVolume.teleportTrigger)
            {
                case BaseTeleportationInteractable.TeleportTrigger.OnSelectExited:
                case BaseTeleportationInteractable.TeleportTrigger.OnSelectEntered:
                    StartManualInteraction((IXRSelectInteractable)m_TargetTeleportVolume);
                    EndManualInteraction();
                    break;
                case BaseTeleportationInteractable.TeleportTrigger.OnActivated:
                case BaseTeleportationInteractable.TeleportTrigger.OnDeactivated:
                    using (m_ActivateEventArgs.Get(out var args))
                    {
                        args.interactorObject = this;
                        args.interactableObject = m_TargetTeleportVolume;
                        (m_TargetTeleportVolume as IXRActivateInteractable).OnActivated(args);
                    }
                    using (m_DeactivateEventArgs.Get(out var args))
                    {
                        args.interactorObject = this;
                        args.interactableObject = m_TargetTeleportVolume;
                        (m_TargetTeleportVolume as IXRActivateInteractable).OnDeactivated(args);
                    }
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(BaseTeleportationInteractable.TeleportTrigger)}={m_TargetTeleportVolume.teleportTrigger}.");
                    break;
            }

            ReleaseTargetTeleportVolume();
        }

        void OnClimbAnchorUpdated(ClimbProvider provider)
        {
            SetTargetTeleportVolume(provider.climbAnchorInteractable);
        }

        void SetTargetTeleportVolume(ClimbInteractable activeClimbInteractable)
        {
            var activeTeleportVolume = activeClimbInteractable.climbAssistanceTeleportVolume;
            if (m_TargetTeleportVolume == activeTeleportVolume)
                return;

            ReleaseTargetTeleportVolume();
            m_TargetTeleportVolume = activeTeleportVolume;
            if (m_TargetTeleportVolume == null)
                return;

            m_PreservedTeleportVolumeSettings = m_TargetTeleportVolume.destinationEvaluationSettings;
            if (destinationEvaluationSettings.Value != null)
                m_TargetTeleportVolume.destinationEvaluationSettings = destinationEvaluationSettings;
        }

        void ReleaseTargetTeleportVolume()
        {
            if (m_TargetTeleportVolume != null)
                m_TargetTeleportVolume.destinationEvaluationSettings = m_PreservedTeleportVolumeSettings;

            m_PreservedTeleportVolumeSettings = null;
            m_TargetTeleportVolume = null;
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            if (m_TargetTeleportVolume != null)
                targets.Add(m_TargetTeleportVolume);
        }

        /// <inheritdoc />
        public override bool isSelectActive => base.isSelectActive && isPerformingManualInteraction;

        /// <inheritdoc />
        public bool shouldActivate => false;

        /// <inheritdoc />
        public bool shouldDeactivate => false;

        /// <inheritdoc />
        public void GetActivateTargets(List<IXRActivateInteractable> targets)
        {
            targets.Clear();
            if (m_TargetTeleportVolume != null)
                targets.Add(m_TargetTeleportVolume);
        }
    }
}
