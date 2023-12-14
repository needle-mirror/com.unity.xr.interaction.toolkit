using System;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver
{
    /// <summary>
    /// An interface that represents an affordance receiver that processes a tween
    /// then updates the affordance state according to the tween output.
    /// Does not schedule jobs with the job system.
    /// </summary>
    /// <seealso cref="IAsyncAffordanceStateReceiver"/>
    /// <seealso cref="BaseSynchronousAffordanceStateReceiver{T}"/>
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public interface ISynchronousAffordanceStateReceiver : IAffordanceStateReceiver
    {
        /// <summary>
        /// Compute new tween target using theme data and consume it inline to update listeners with new affordance value.
        /// </summary>
        /// <param name="tweenTarget">Tween interpolation target based on delta time parameter.</param>
        void HandleTween(float tweenTarget);
    }
}
