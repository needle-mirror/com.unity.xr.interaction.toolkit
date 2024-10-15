using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Settings for evaluating the destination anchor for a <see cref="TeleportationMultiAnchorVolume"/>.
    /// </summary>
    [Serializable]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportVolumeDestinationSettings
    {
        [SerializeField]
        [Tooltip("Whether to delay evaluation of the destination anchor until the user has hovered over the volume " +
            "for a certain amount of time.")]
        bool m_EnableDestinationEvaluationDelay;

        /// <summary>
        /// Whether to delay evaluation of the destination anchor until the user has hovered over the volume
        /// for a certain amount of time.
        /// </summary>
        /// <seealso cref="destinationEvaluationDelayTime"/>
        public bool enableDestinationEvaluationDelay
        {
            get => m_EnableDestinationEvaluationDelay;
            set => m_EnableDestinationEvaluationDelay = value;
        }

        [SerializeField]
        [Tooltip("The amount of time, in seconds, for which the user must hover over the volume before it " +
            "designates a destination anchor.")]
        float m_DestinationEvaluationDelayTime = 1f;

        /// <summary>
        /// The amount of time, in seconds, for which the user must hover over the volume before it designates
        /// a destination anchor. Only used if <see cref="enableDestinationEvaluationDelay"/> is <see langword="true"/>.
        /// </summary>
        /// <seealso cref="enableDestinationEvaluationDelay"/>
        public float destinationEvaluationDelayTime
        {
            get => m_DestinationEvaluationDelayTime;
            set => m_DestinationEvaluationDelayTime = value;
        }

        [SerializeField]
        [Tooltip("Whether to periodically query the filter for its calculated destination. If the determined anchor is " +
            "not the current destination, the volume will initiate re-evaluation of the destination anchor.")]
        bool m_PollForDestinationChange;

        /// <summary>
        /// Whether to periodically query the <see cref="TeleportationMultiAnchorVolume.destinationEvaluationFilter"/> for its calculated destination anchor.
        /// If the determined anchor is different from the current <see cref="TeleportationMultiAnchorVolume.destinationAnchor"/>, the volume will initiate
        /// re-evaluation of the destination anchor.
        /// </summary>
        /// <seealso cref="destinationPollFrequency"/>
        public bool pollForDestinationChange
        {
            get => m_PollForDestinationChange;
            set => m_PollForDestinationChange = value;
        }

        [SerializeField]
        [Tooltip("The amount of time, in seconds, between queries to the filter for its calculated destination anchor.")]
        float m_DestinationPollFrequency = 1f;

        /// <summary>
        /// The amount of time, in seconds, between queries to the <see cref="TeleportationMultiAnchorVolume.destinationEvaluationFilter"/>
        /// for its calculated destination anchor. Only used if <see cref="pollForDestinationChange"/> is <see langword="true"/>.
        /// </summary>
        /// <seealso cref="pollForDestinationChange"/>
        public float destinationPollFrequency
        {
            get => m_DestinationPollFrequency;
            set => m_DestinationPollFrequency = value;
        }

        [SerializeField]
        [RequireInterface(typeof(ITeleportationVolumeAnchorFilter))]
        [Tooltip("The anchor filter used to evaluate a teleportation destination. If set to None, the volume will use " +
            "the anchor furthest from the user as the destination.")]
        Object m_DestinationFilterObject;

        /// <summary>
        /// The object reference to the filter used to evaluate a teleportation destination. This object should implement
        /// the <see cref="ITeleportationVolumeAnchorFilter"/> interface. If this object is <see langword="null"/>, the
        /// volume will use a <see cref="FurthestTeleportationAnchorFilter"/> to evaluate the destination.
        /// </summary>
        /// <remarks>
        /// To access and modify the destination filter at runtime, the <see cref="destinationEvaluationFilter"/> property
        /// should be used instead.
        /// </remarks>
        public Object destinationFilterObject
        {
            get => m_DestinationFilterObject;
            set
            {
                m_DestinationFilterObject = value;
                m_DestinationEvaluationFilter = value as ITeleportationVolumeAnchorFilter;
                m_AssignedFilter = true;
            }
        }

        ITeleportationVolumeAnchorFilter m_DestinationEvaluationFilter;

        [NonSerialized]
        bool m_AssignedFilter;

        /// <summary>
        /// The filter used to evaluate a teleportation destination from the list of anchors. If this is <see langword="null"/>,
        /// the volume will use a <see cref="FurthestTeleportationAnchorFilter"/> to evaluate the destination.
        /// </summary>
        public ITeleportationVolumeAnchorFilter destinationEvaluationFilter
        {
            get
            {
                if (!m_AssignedFilter)
                {
                    m_DestinationEvaluationFilter = m_DestinationFilterObject as ITeleportationVolumeAnchorFilter;
                    m_AssignedFilter = true;
                }
                return m_DestinationEvaluationFilter;
            }
            set
            {
                m_DestinationEvaluationFilter = value;
                m_AssignedFilter = true;
            }
        }
    }
}
