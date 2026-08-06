namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Filter used by the <see cref="XRInteractionSimulator"/> to decide whether a
    /// <see cref="RaycastHit"/> should be ignored when aiming a simulated device
    /// (controller or hand) at a world point during point-and-click.
    /// </summary>
    internal interface ISimulatorDeviceAimRaycastFilter
    {
        /// <summary>
        /// Determines whether the given <paramref name="raycastHit"/> should be discarded
        /// (skipped) rather than used as the aim target.
        /// </summary>
        /// <param name="raycastHit">The raycast hit to evaluate.</param>
        /// <returns>Returns <see langword="true"/> if the hit should be discarded; otherwise, <see langword="false"/>.</returns>
        bool DiscardRaycastHit(RaycastHit raycastHit);
    }
}
