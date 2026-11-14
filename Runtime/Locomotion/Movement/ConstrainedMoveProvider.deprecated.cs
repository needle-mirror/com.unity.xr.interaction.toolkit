using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    public abstract partial class ConstrainedMoveProvider
    {
        /// <summary>
        /// Defines when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="gravityMode"/>
        [Obsolete("GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version.", true)]
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
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="GravityApplicationMode"/>
        [Obsolete("gravityMode has been deprecated in XRI 3.0.0 and will be removed in a future version.", true)]
        public GravityApplicationMode gravityMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether gravity applies to constrained axes when a <see cref="CharacterController"/> is used.
        /// Ignored when a <see cref="GravityProvider"/> component is found in the scene.
        /// </summary>
        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.", true)]
        public bool useGravity
        {
            get => default;
            set => _ = value;
        }
    }
}
