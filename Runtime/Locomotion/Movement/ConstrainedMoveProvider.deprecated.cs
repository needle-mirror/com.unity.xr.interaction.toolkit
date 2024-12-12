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

        [SerializeField]
        [Tooltip("Controls whether gravity applies to constrained axes when a Character Controller is used. Ignored when a Gravity Provider component is found in the scene.")]
        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
        bool m_UseGravity = true;
        /// <summary>
        /// Controls whether gravity applies to constrained axes when a <see cref="CharacterController"/> is used.
        /// Ignored when a <see cref="GravityProvider"/> component is found in the scene.
        /// </summary>
        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
        public bool useGravity
        {
            get => m_UseGravity;
            set
            {
                m_UseGravity = value;

                if (Application.isPlaying && m_GravityProvider != null)
                    MigrateUseGravityToGravityProvider();
            }
        }

        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
        Vector3 m_GravityDrivenVelocity;

        [Obsolete("Private migration helper.")]
        void MigrateUseGravityToGravityProvider()
        {
            if (m_GravityProvider.useGravity != m_UseGravity)
            {
                Debug.LogWarning("Use Gravity is deprecated on this locomotion component while Gravity Provider component is in scene." +
                    $" Automatically setting Use Gravity to {m_UseGravity} on Gravity Provider." +
                    " Gravity should be controlled on the Gravity Provider instead.", this);
                m_GravityProvider.useGravity = m_UseGravity;
            }
        }
    }
}
