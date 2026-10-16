using System;
using UnityEngine;
#if XR_HANDS_1_7_OR_NEWER
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Configuration;
#endif
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// A component that automatically sets the hand subsystem configuration
    /// to use the Common Gestures mode instead of Legacy.
    /// </summary>
    public class XRHandsConfigurationManager : MonoBehaviour
    {
#if XR_HANDS_1_7_OR_NEWER
        XRHandSubsystem m_HandSubsystem;
        bool m_LoggedMissingHandSubsystem;
#endif

        void Start()
        {
#if XR_HANDS_1_7_OR_NEWER
            // We don't log here if the hand subsystem is missing because the subsystem may not yet be added
            // if manually done by other behaviors during the first frame's Awake/OnEnable/Start.
            // Logging will occur in Update.
            if (XRInputTrackingAggregator.TryGetHandSubsystem(out m_HandSubsystem))
                UpdateHandsConfiguration(m_HandSubsystem, XRHandDevicePoseSource.CommonGestures);
#else
            Debug.LogWarning("Script requires XR Hands (com.unity.xr.hands) package 1.7.0 or newer to set hand subsystem configuration." +
                " Install using Window > Package Manager or click Fix on the related issue in Edit > Project Settings > XR Plug-in Management > Project Validation.", this);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
#if XR_HANDS_1_7_OR_NEWER
            // Retry finding the running hand subsystem if necessary.
            if (m_HandSubsystem == null || !m_HandSubsystem.running)
            {
                if (XRInputTrackingAggregator.TryGetHandSubsystem(out var runningHandSubsystem))
                {
                    if (m_HandSubsystem != runningHandSubsystem)
                    {
                        m_HandSubsystem = runningHandSubsystem;
                        UpdateHandsConfiguration(m_HandSubsystem, XRHandDevicePoseSource.CommonGestures);
                    }
                }
                // Don't warn if there was some hand subsystem obtained at one time.
                // Without this check, the warning would be logged when exiting play mode.
                else if (m_HandSubsystem == null)
                {
                    LogMissingHandSubsystem();
                }
            }
#endif
        }

#if XR_HANDS_1_7_OR_NEWER
        void LogMissingHandSubsystem()
        {
            if (m_LoggedMissingHandSubsystem)
                return;

            Debug.LogWarning("Hand Tracking Subsystem not found or not running, can't update its config.", this);
            m_LoggedMissingHandSubsystem = true;
        }

        static void UpdateHandsConfiguration(XRHandSubsystem handSubsystem, XRHandDevicePoseSource poseSource)
        {
#if XR_HANDS_1_9_OR_NEWER
            var config = handSubsystem.handSubsystemConfiguration;
#else
            var config = new XRHandSubsystemConfiguration();
#endif
            config.xrHandDevicePoseSource = poseSource;

            handSubsystem.UpdateHandsConfiguration(config);
        }
#endif
    }
}
