using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    public partial class ContinuousMoveProvider
    {
        [SerializeField]
        [Tooltip("Controls whether gravity affects this provider when a Character Controller is used and flying is disabled. Ignored when a Gravity Provider component is found in the scene. Deprecated in XRI 3.1.0, use Gravity Provider instead.")]
        [Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
        bool m_UseGravity = true;
        /// <summary>
        /// Controls whether gravity affects this provider when a <see cref="CharacterController"/> is used.
        /// This only applies when <see cref="enableFly"/> is <see langword="false"/>.
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
