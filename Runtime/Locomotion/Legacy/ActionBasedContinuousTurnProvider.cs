using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly rotate their rig continuously over time
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="ActionBasedSnapTurnProvider"/>
    [AddComponentMenu("XR/Locomotion/Legacy/Continuous Turn Provider (Action-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_ActionBasedContinuousTurnProvider)]
    [Obsolete(k_ActionBasedContinuousTurnProviderDeprecated, true)]
    public class ActionBasedContinuousTurnProvider : ContinuousTurnProviderBase
    {
        const string k_ActionBasedContinuousTurnProviderDeprecated = "ActionBasedContinuousTurnProvider has been deprecated in version 3.0.0. Use ContinuousTurnProvider instead.";

        /// <summary>
        /// The Input System Action that Unity uses to read Turn data from the left hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty leftHandTurnAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action that Unity uses to read Turn data from the right hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty rightHandTurnAction
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
            throw new NotSupportedException(k_ActionBasedContinuousTurnProviderDeprecated);
        }
    }
}
