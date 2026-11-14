using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// A locomotion provider that allows the user to rotate their rig using a 2D axis input
    /// from an input system action.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Legacy/Snap Turn Provider (Action-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_ActionBasedSnapTurnProvider)]
    [Obsolete(k_ActionBasedSnapTurnProviderDeprecated, true)]
    public class ActionBasedSnapTurnProvider : SnapTurnProviderBase
    {
        const string k_ActionBasedSnapTurnProviderDeprecated = "ActionBasedSnapTurnProvider has been deprecated in version 3.0.0. Use SnapTurnProvider instead.";

        /// <summary>
        /// The Input System Action that Unity uses to read Snap Turn data sent from the left hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty leftHandSnapTurnAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action that Unity uses to read Snap Turn data sent from the right hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty rightHandSnapTurnAction
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
        }

        /// <inheritdoc />
        protected override Vector2 ReadInput()
        {
            throw new NotSupportedException(k_ActionBasedSnapTurnProviderDeprecated);
        }
    }
}
