using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
    public partial class GrabMoveProvider
    {
        [SerializeField]
        [Obsolete("m_GrabMoveAction has been deprecated. Please configure input action using m_GrabMoveInput instead.")]
        InputActionProperty m_GrabMoveAction = new InputActionProperty(new InputAction("Grab Move", type: InputActionType.Button));
        /// <summary>
        /// (Deprecated) The Input System Action that Unity uses to perform grab movement while held. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        /// <seealso cref="grabMoveInput"/>
        [Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.")]
        public InputActionProperty grabMoveAction
        {
            get => m_GrabMoveAction;
            set => SetInputActionProperty(ref m_GrabMoveAction, value);
        }

        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }
    }
}