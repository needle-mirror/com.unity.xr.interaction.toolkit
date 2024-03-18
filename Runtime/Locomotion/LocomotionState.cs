namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// The state of locomotion for any given <see cref="LocomotionProvider"/>.
    /// </summary>
    public enum LocomotionState
    {
        /// <summary>
        /// Locomotion state where the <see cref="LocomotionProvider"/> is idle, before locomotion input occurs.
        /// </summary>
        Idle,

        /// <summary>
        /// Locomotion state where the <see cref="LocomotionProvider"/> is getting ready to move, when locomotion start
        /// input occurs.
        /// </summary>
        /// <remarks>
        /// The provider enters this state after calling <see cref="LocomotionProvider.TryPrepareLocomotion"/>. It remains
        /// in this state until it enters the <see cref="Moving"/> state, which happens either when the
        /// provider calls <see cref="LocomotionProvider.TryStartLocomotionImmediately"/> or during the
        /// <see cref="LocomotionProvider.mediator"/>'s next <see cref="LocomotionMediator.Update"/> in which
        /// <see cref="LocomotionProvider.canStartMoving"/> is <see langword="true"/>, whichever happens first.
        /// </remarks>
        Preparing,

        /// <summary>
        /// Locomotion state where the <see cref="LocomotionProvider"/> is queuing <see cref="IXRBodyTransformation"/>s
        /// with the <see cref="XRBodyTransformer"/>.
        /// </summary>
        /// <remarks>
        /// The provider can enter this state in one of two ways:
        /// 1. From the <see cref="Preparing"/> state, the first time
        /// <see cref="LocomotionProvider.canStartMoving"/> is <see langword="true"/> during any of the
        /// <see cref="LocomotionProvider.mediator"/>'s <see cref="LocomotionMediator.Update"/> calls after the
        /// provider called <see cref="LocomotionProvider.TryPrepareLocomotion"/>, or
        /// 2. From any state other than <see cref="Moving"/>, immediately after the provider called
        /// <see cref="LocomotionProvider.TryStartLocomotionImmediately"/>.
        /// </remarks>
        Moving,

        /// <summary>
        /// Locomotion state where the <see cref="LocomotionProvider"/> is no longer moving, after locomotion end input
        /// has completed.
        /// </summary>
        /// <remarks>
        /// The provider enters this state after calling <see cref="LocomotionProvider.TryEndLocomotion"/>, and it
        /// remains in this state until it returns to the <see cref="Idle"/> state during the
        /// <see cref="LocomotionProvider.mediator"/>'s <see cref="LocomotionMediator.Update"/> in the next frame.
        /// </remarks>
        Ended,
    }

    /// <summary>
    /// Extension methods for <see cref="LocomotionState"/>.
    /// </summary>
    public static class LocomotionStateExtensions
    {
        /// <summary>
        /// Whether this is the state of actively preparing or performing locomotion. This returns <see langword="true"/>
        /// if <paramref name="state"/> is <see cref="LocomotionState.Preparing"/> or <see cref="LocomotionState.Moving"/>,
        /// <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="state">The locomotion state to check.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="state"/> is <see cref="LocomotionState.Preparing"/> or
        /// <see cref="LocomotionState.Moving"/>, <see langword="false"/> otherwise.</returns>
        public static bool IsActive(this LocomotionState state)
        {
            return state == LocomotionState.Preparing || state == LocomotionState.Moving;
        }
    }
}
