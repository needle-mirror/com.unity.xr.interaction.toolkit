#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Runtime tracker for an XRI simulator component.
    /// This class is used to track whether one of the two XRI simulators is used by tracking the duration in seconds
    /// the simulator is active during a play mode session.
    /// </summary>
    class SimulatorSessionTracker
    {
        /// <summary>
        /// The number of different simulator components that were active during a play mode session.
        /// </summary>
        /// <remarks>
        /// This is likely either 0 or 1, but it could be higher if there is a simulator loaded and unloaded with
        /// each scene rather than using <c>Object.DontDestroyOnLoad</c> that the loader enforces for auto instantiation.
        /// </remarks>
        public int sessionCount { get; private set; }

        /// <summary>
        /// Duration in seconds of the simulator component singleton being active during a play mode session.
        /// </summary>
        /// <remarks>
        /// Continues to accumulate time while the simulator component is disabled since the simulator session
        /// is only ended when the component is destroyed.
        /// </remarks>
        public double sessionDuration { get; private set; }

        double m_StartTimestamp;
        bool m_SessionOpen;

        /// <summary>
        /// Reset the tracker to its initial state.
        /// Call this after generating the analytics payload to avoid accumulating data across
        /// multiple play mode sessions when domain reload is disabled.
        /// </summary>
        public void Reset()
        {
            sessionCount = 0;
            sessionDuration = 0d;
            m_StartTimestamp = 0d;
            m_SessionOpen = false;
        }

        /// <summary>
        /// Start tracking the time the simulator component is active, invoked when its <c>Awake</c> is called.
        /// </summary>
        /// <param name="now">Current timestamp.</param>
        public void StartSession(double now)
        {
            m_StartTimestamp = now;
            if (!m_SessionOpen)
                sessionCount++;

            m_SessionOpen = true;
        }

        /// <summary>
        /// End tracking the time the simulator component is active, invoked when its <c>OnDestroy</c> is called.
        /// </summary>
        /// <param name="now">Current timestamp.</param>
        public void EndSession(double now)
        {
            if (!m_SessionOpen)
                return;

            sessionDuration += now - m_StartTimestamp;
            m_SessionOpen = false;
        }
    }
}

#endif
