namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity
{
    /// <summary>
    /// Interface for external control over the <see cref="GravityProvider"/>.
    /// This interface should be implemented from a <see cref="LocomotionProvider"/> and be parented or attached to a <see cref="LocomotionMediator"/>.
    /// </summary>
    public interface IGravityController
    {
        /// <summary>
        /// Whether the gravity can be processed.
        /// Gravity controllers that can process receive queries to <see cref="gravityPaused"/>, controllers that cannot process do not.
        /// </summary>
        /// <remarks>
        /// It's recommended to return <see cref="Behaviour.isActiveAndEnabled"/> when implementing this interface
        /// in a <see cref="MonoBehaviour"/>.
        /// </remarks>
        bool canProcess { get; }

        /// <summary>
        /// Whether gravity is paused.
        /// </summary>
        /// <seealso cref="GravityProvider.IsGravityBlocked"/>
        bool gravityPaused { get; }

        /// <summary>
        /// Attempts to lock gravity.
        /// </summary>
        /// <param name="gravityOverride">The <see cref="GravityOverride"/> to apply.</param>
        /// <returns>Whether the gravity was successfully locked.</returns>
        bool TryLockGravity(GravityOverride gravityOverride);

        /// <summary>
        /// Removes this provider from the <see cref="GravityProvider"/> list's of locked providers.
        /// </summary>
        void RemoveGravityLock();

        /// <summary>
        /// Called from <see cref="GravityProvider.TryLockGravity"/> when gravity lock is changed.
        /// </summary>
        /// <param name="gravityOverride">The <see cref="GravityOverride"/> to apply.</param>
        /// <seealso cref="GravityProvider.onGravityLockChanged"/>
        void OnGravityLockChanged(GravityOverride gravityOverride);

        /// <summary>
        /// Called from <see cref="GravityProvider"/> when the grounded state changes.
        /// </summary>
        /// <param name="isGrounded">Whether the player is on the ground.</param>
        /// <seealso cref="GravityProvider.onGroundedChanged"/>
        void OnGroundedChanged(bool isGrounded);
    }
}
