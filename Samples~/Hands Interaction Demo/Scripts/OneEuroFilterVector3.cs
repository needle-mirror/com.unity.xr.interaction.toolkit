namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Provides a means to smooth jittery <see cref="Vector3"/> signals.
    /// This filter is particularly effective for small and rapid movements,
    /// making it useful for applications like motion tracking or gesture recognition.
    /// </summary>
    /// <remarks>
    /// The filtering process relies on two main parameters: <c>minCutoff</c> and <c>beta</c>.
    /// <list type="bullet">
    /// <item>
    /// <term><c>minCutoff</c></term>
    /// <description> primarily influences the smoothing at low speeds.</description>
    /// </item>
    /// <item>
    /// <term><c>beta</c></term>
    /// <description> determines the filter's responsiveness to speed changes.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class OneEuroFilterVector3
    {
        const float k_DeltaTimeThreshold = 0.001f;
        const float k_TwoPi = 2f * Mathf.PI;

        Vector3 m_LastRawValue;
        Vector3 m_LastFilteredValue;
        readonly float m_MinCutoff;
        readonly float m_Beta;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneEuroFilterVector3"/> with specified cutoff and beta values.
        /// </summary>
        /// <param name="initialRawValue">The initial raw value for the filter.</param>
        /// <param name="minCutoff">The minimum cutoff frequency in Hz. Default is 50f.</param>
        /// <param name="beta">The speed coefficient for the filter. Default is 10f.</param>
        /// <remarks>
        /// Filter parameters:
        /// <list type="bullet">
        /// <item>
        /// <term><paramref name="minCutoff"/></term>
        /// <description>
        /// Controls the amount of smoothing at low speeds. A smaller value will introduce
        /// more smoothing and potential lag, helping to reduce low-frequency jitter. A larger value
        /// may feel more responsive but can let through more jitter. A starting value of 50 Hz
        /// provides light smoothing suitable for hand tracking.
        /// </description>
        /// </item>
        /// <item>
        /// <term><paramref name="beta"/></term>
        /// <description>
        /// Determines the filter's adjustment to speed changes. A smaller value provides consistent
        /// smoothing, while a larger one reduces smoothing during fast motion for lower latency.
        /// A starting value of 10 is recommended, but fine-tuning
        /// might be necessary based on specific use cases.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Initialize"/>
        public OneEuroFilterVector3(Vector3 initialRawValue, float minCutoff = 50f, float beta = 10f)
        {
            m_LastRawValue = initialRawValue;
            m_LastFilteredValue = initialRawValue;
            m_MinCutoff = minCutoff;
            m_Beta = beta;
        }

        /// <summary>
        /// Resets the initial raw value. Useful to recover from tracking loss.
        /// </summary>
        /// <param name="initialRawValue">Raw value to reset filtering basis to.</param>
        public void Initialize(Vector3 initialRawValue)
        {
            m_LastRawValue = initialRawValue;
            m_LastFilteredValue = initialRawValue;
        }

        /// <summary>
        /// Filters the given <see cref="Vector3"/> rawValue using the internal minCutoff and beta parameters.
        /// </summary>
        /// <param name="rawValue">The raw <see cref="Vector3"/> value to be filtered.</param>
        /// <param name="deltaTime">The time since the last filter update.</param>
        /// <returns>The filtered <see cref="Vector3"/> value.</returns>
        public Vector3 Filter(Vector3 rawValue, float deltaTime)
        {
            return Filter(rawValue, deltaTime, m_MinCutoff, m_Beta);
        }

        /// <summary>
        /// Filters the given <see cref="Vector3"/> rawValue using provided minCutoff and beta parameters.
        /// This method computes the speed of change in the signal and dynamically adjusts the amount of smoothing
        /// based on the speed and the provided minCutoff and beta values.
        /// </summary>
        /// <param name="rawValue">The raw <see cref="Vector3"/> value to be filtered.</param>
        /// <param name="deltaTime">The time since the last filter update.</param>
        /// <param name="minCutoff">The minimum cutoff value for the filter. Influences the amount of smoothing at low speeds.</param>
        /// <param name="beta">Determines the filter's adjustment to speed changes, influencing its responsiveness.</param>
        /// <returns>The filtered <see cref="Vector3"/> value.</returns>
        public Vector3 Filter(Vector3 rawValue, float deltaTime, float minCutoff, float beta)
        {
            // Guard against division by zero or invalid parameters.
            if (deltaTime < k_DeltaTimeThreshold || minCutoff < 0f || beta < 0f)
                return m_LastFilteredValue;

            // Finite-difference derivative: how fast the signal is moving (units/sec).
            // Use Euclidean magnitude so diagonal motion gets uniform smoothing across all axes,
            // preserving straight-line trajectories.
            float speed = ((rawValue - m_LastRawValue) / deltaTime).magnitude;

            // Adaptive cutoff: cutoff = minCutoff + beta * |speed|.
            // At rest → heavy smoothing (removes jitter). Fast motion → light smoothing (reduces lag).
            float alpha = ComputeAlpha(minCutoff + beta * speed, deltaTime);

            // First-order low-pass filter. Same alpha for all axes preserves movement direction.
            Vector3 filteredValue = Vector3.Lerp(m_LastFilteredValue, rawValue, alpha);

            m_LastRawValue = rawValue;
            m_LastFilteredValue = filteredValue;

            return filteredValue;
        }

        static float ComputeAlpha(float cutoffFrequency, float deltaTime)
        {
            // Convert cutoff frequency (Hz) to a smoothing factor in [0,1].
            // Higher cutoff → smaller tau → alpha closer to 1 → less smoothing.
            if (cutoffFrequency <= 0f)
                return 0f;

            float tau = 1f / (k_TwoPi * cutoffFrequency);
            return 1f / (1f + tau / deltaTime);
        }
    }
}
