using System;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    /// <summary>
    /// Base class for a locomotion provider that allows for constrained movement with a <see cref="CharacterController"/>.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    public abstract partial class ConstrainedMoveProvider
    {
        /// <summary>
        /// Defines when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="gravityMode"/>
        [Obsolete("GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
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

        [SerializeField]
        [Tooltip("Controls when gravity begins to take effect.")]
        [Obsolete("m_GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
        GravityApplicationMode m_GravityApplicationMode;
        /// <summary>
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="GravityApplicationMode"/>
        [Obsolete("gravityMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
        public GravityApplicationMode gravityMode
        {
            get => m_GravityApplicationMode;
            set => m_GravityApplicationMode = value;
        }
    }
}
