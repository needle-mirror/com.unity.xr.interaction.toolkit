using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial class XRRig
    {
#pragma warning disable 618 // Disable Obsolete warnings, kept around to read old data and migrate to new format.
        const TrackingSpaceType k_MigratedTrackingSpace = (TrackingSpaceType)3;
        const TrackingOriginModeFlags k_MigratedTrackingOriginMode = (TrackingOriginModeFlags)(1 << 31);

        [SerializeField]
        TrackingSpaceType m_TrackingSpace = k_MigratedTrackingSpace;

        /// <summary>
        /// Whether the experience is Room Scale or Stationary. Not all devices support all tracking spaces; if the
        /// selected tracking space is not set it will fall back to Stationary.
        /// </summary>
        [Obsolete("trackingSpace has been deprecated. Use requestedTrackingOriginMode instead.", true)]
        public TrackingSpaceType trackingSpace
        {
            get => throw new NotSupportedException("trackingSpace has been deprecated. Use requestedTrackingOriginMode instead.");
            set => throw new NotSupportedException("trackingSpace has been deprecated. Use requestedTrackingOriginMode instead.");
        }

        [SerializeField]
        TrackingOriginModeFlags m_TrackingOriginMode = k_MigratedTrackingOriginMode;

        /// <summary>
        /// The type of tracking origin to use for this Rig. Tracking origins identify where (0, 0, 0) is in the world
        /// of tracking. Not all devices support all tracking spaces.
        /// </summary>
        [Obsolete("trackingOriginMode has been deprecated. Use requestedTrackingOriginMode and/or currentTrackingOriginMode instead.")]
        public TrackingOriginModeFlags trackingOriginMode
        {
            get
            {
                if (m_TrackingOriginMode != k_MigratedTrackingOriginMode)
                    return m_TrackingOriginMode;

                if (Application.isPlaying)
                    return currentTrackingOriginMode;

                switch (m_RequestedTrackingOriginMode)
                {
                    case TrackingOriginMode.Device:
                        return TrackingOriginModeFlags.Device;
                    case TrackingOriginMode.Floor:
                        return TrackingOriginModeFlags.Floor;
                    default:
                        return TrackingOriginModeFlags.Unknown;
                }
            }
            set
            {
                m_TrackingOriginMode = value;
                UpgradeTrackingOriginModeToRequest();
                TryInitializeCamera();
            }
        }

        /// <summary>
        /// Utility helper to migrate from <see cref="TrackingSpaceType"/> to <see cref="TrackingOriginModeFlags"/> seamlessly.
        /// </summary>
        void UpgradeTrackingSpaceToTrackingOriginMode()
        {
            if (m_TrackingSpace <= TrackingSpaceType.RoomScale)
            {
                switch (m_TrackingSpace)
                {
                    case TrackingSpaceType.RoomScale:
                        m_TrackingOriginMode = TrackingOriginModeFlags.Floor;
                        break;
                    case TrackingSpaceType.Stationary:
                        m_TrackingOriginMode = TrackingOriginModeFlags.Device;
                        break;
                }

                // Set to an invalid value to indicate the value has been migrated.
                m_TrackingSpace = k_MigratedTrackingSpace;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif // UNITY_EDITOR
            }
        }

        void UpgradeTrackingOriginModeToRequest()
        {
            if (m_TrackingOriginMode != k_MigratedTrackingOriginMode)
            {
                switch (m_TrackingOriginMode)
                {
                    case TrackingOriginModeFlags.Unknown:
                        m_RequestedTrackingOriginMode = TrackingOriginMode.NotSpecified;
                        break;
                    case TrackingOriginModeFlags.Device:
                        m_RequestedTrackingOriginMode = TrackingOriginMode.Device;
                        break;
                    case TrackingOriginModeFlags.Floor:
                        m_RequestedTrackingOriginMode = TrackingOriginMode.Floor;
                        break;
                    default:
                        // Cannot migrate value
                        Debug.LogWarning($"Cannot migrate {m_TrackingOriginMode:F} to {nameof(TrackingOriginMode)} type for {nameof(requestedTrackingOriginMode)}.", this);
                        return;
                }

                // Set to an invalid value to indicate the value has been migrated.
                m_TrackingOriginMode = k_MigratedTrackingOriginMode;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif // UNITY_EDITOR
            }
        }

        bool SetupCameraLegacy()
        {
            // Execution path for when Input Subsystems are not present.
            switch (m_RequestedTrackingOriginMode)
            {
                case TrackingOriginMode.Floor:
                    return SetupCameraLegacy(TrackingSpaceType.RoomScale);
                case TrackingOriginMode.Device:
                    return SetupCameraLegacy(TrackingSpaceType.Stationary);
            }

            return true;
        }

        bool SetupCameraLegacy(TrackingSpaceType trackingSpaceType)
        {
            var trackingSettingsSet = XRDevice.SetTrackingSpaceType(trackingSpaceType);
            if (trackingSpaceType == TrackingSpaceType.Stationary)
                InputTracking.Recenter();

            if (trackingSettingsSet)
                MoveOffsetHeight();

            return trackingSettingsSet;
        }

        bool IsModeStaleLegacy()
        {
            // Execution path for when Input Subsystems are not present.
            switch (m_RequestedTrackingOriginMode)
            {
                case TrackingOriginMode.Floor:
                    return XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale;
                case TrackingOriginMode.Device:
                    return XRDevice.GetTrackingSpaceType() != TrackingSpaceType.Stationary;
            }

            return false;
        }
#pragma warning restore 618
    }
}