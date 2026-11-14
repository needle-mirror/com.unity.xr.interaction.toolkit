using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly move their rig continuously over time.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [Obsolete(k_ContinuousMoveProviderBaseDeprecated, true)]
    public abstract class ContinuousMoveProviderBase : LocomotionProvider
    {
        const string k_ContinuousMoveProviderBaseDeprecated = "The ContinuousMoveProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use ContinuousMoveProvider instead.";

        /// <summary>
        /// Defines when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="gravityApplicationMode"/>
        [Obsolete("GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use LocomotionMediator with a GravityProvider.", true)]
        public enum GravityApplicationMode
        {
            /// <summary>
            /// Only begin to apply gravity and apply locomotion when a move input occurs.
            /// When using gravity, continues applying each frame, even if input is stopped, until touching ground.
            /// </summary>
            /// <remarks>
            /// Use this style when you don't want gravity to apply when the player physically walks away and off a ground surface.
            /// Gravity will only begin to move the player back down to the ground when they try to use input to move.
            /// </remarks>
            AttemptingMove,

            /// <summary>
            /// Apply gravity and apply locomotion every frame, even without move input.
            /// </summary>
            /// <remarks>
            /// Use this style when you want gravity to apply when the player physically walks away and off a ground surface,
            /// even when there is no input to move.
            /// </remarks>
            Immediately,
        }

        /// <summary>
        /// The speed, in units per second, to move forward.
        /// </summary>
        public float moveSpeed
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether to enable strafing (sideways movement).
        /// </summary>
        public bool enableStrafe
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether to enable flying (unconstrained movement). This overrides <see cref="useGravity"/>.
        /// </summary>
        public bool enableFly
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether gravity affects this provider when a <see cref="CharacterController"/> is used.
        /// This only applies when <see cref="enableFly"/> is <see langword="false"/>.
        /// </summary>
        public bool useGravity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="GravityApplicationMode"/>
        public GravityApplicationMode gravityApplicationMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The source <see cref="Transform"/> that defines the forward direction.
        /// </summary>
        public Transform forwardSource
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
        }

        /// <summary>
        /// Reads the current value of the move input.
        /// </summary>
        /// <returns>Returns the input vector, such as from a thumbstick.</returns>
        protected abstract Vector2 ReadInput();

        /// <summary>
        /// Determines how much to slide the rig due to <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns the translation amount in world space to move the rig.</returns>
        protected virtual Vector3 ComputeDesiredMove(Vector2 input)
        {
            throw new NotSupportedException(k_ContinuousMoveProviderBaseDeprecated);
        }

        /// <summary>
        /// Creates a locomotion event to move the rig by <paramref name="translationInWorldSpace"/>,
        /// and optionally applies gravity.
        /// </summary>
        /// <param name="translationInWorldSpace">The translation amount in world space to move the rig (pre-gravity).</param>
        protected virtual void MoveRig(Vector3 translationInWorldSpace)
        {
            throw new NotSupportedException(k_ContinuousMoveProviderBaseDeprecated);
        }
    }
}
