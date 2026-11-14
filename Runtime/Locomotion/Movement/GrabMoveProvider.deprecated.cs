using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    public partial class GrabMoveProvider
    {
        /// <summary>
        /// (Deprecated) The Input System Action that Unity uses to perform grab movement while held. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        /// <seealso cref="grabMoveInput"/>
        [Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.", true)]
        public InputActionProperty grabMoveAction
        {
            get => default;
            set => _ = value;
        }
    }
}
