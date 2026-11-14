using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly move their rig continuously over time
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [AddComponentMenu("XR/Locomotion/Legacy/Continuous Move Provider (Action-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_ActionBasedContinuousMoveProvider)]
    [Obsolete(k_ActionBasedContinuousMoveProviderDeprecated, true)]
    public class ActionBasedContinuousMoveProvider : ContinuousMoveProviderBase
    {
        const string k_ActionBasedContinuousMoveProviderDeprecated = "ActionBasedContinuousMoveProvider has been deprecated in version 3.0.0. Use ContinuousMoveProvider instead.";

        /// <summary>
        /// The Input System Action that Unity uses to read Move data from the left hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty leftHandMoveAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System Action that Unity uses to read Move data from the right hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty rightHandMoveAction
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
            throw new NotSupportedException(k_ActionBasedContinuousMoveProviderDeprecated);
        }
    }
}
