using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    public partial class ContinuousMoveProvider
    {
        /// <summary>
        /// Controls whether gravity affects this provider when a <see cref="CharacterController"/> is used.
        /// This only applies when <see cref="enableFly"/> is <see langword="false"/>.
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
