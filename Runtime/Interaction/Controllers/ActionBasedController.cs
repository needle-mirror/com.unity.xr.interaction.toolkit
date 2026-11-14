using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets feature values on a tracked input controller device using actions from the Input System
    /// into XR Interaction states, such as Select. Additionally, it applies the current Pose value
    /// of a tracked device to the transform of the GameObject.
    /// </summary>
    /// <remarks>
    /// This behavior requires that the Input System is enabled in the <b>Active Input Handling</b>
    /// setting in <b>Edit &gt; Project Settings &gt; Player</b> for input values to be read.
    /// Each input action must also be enabled to read the current value of the action. Referenced
    /// input actions in an Input Action Asset are not enabled by default.
    /// </remarks>
    /// <seealso cref="XRBaseController"/>
    [AddComponentMenu("/XR Controller (Action-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_ActionBasedController)]
    [Obsolete(k_ActionBasedControllerDeprecated, true)]
    public class ActionBasedController : XRBaseController
    {
        const string k_ActionBasedControllerDeprecated = "ActionBasedController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.";

        /// <summary>
        /// The Input System action to use for Position Tracking for this GameObject. Must be a <see cref="Vector3Control"/> Control.
        /// </summary>
        public InputActionProperty positionAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for Rotation Tracking for this GameObject. Must be a <see cref="QuaternionControl"/> Control.
        /// </summary>
        public InputActionProperty rotationAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read the Is Tracked state when updating this GameObject position and rotation;
        /// falls back to the tracked device's is tracked state that drives the position or rotation action when not set.
        /// Must be an action with a button-like interaction where phase equals performed when is tracked.
        /// Typically a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionProperty isTrackedAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read the Tracking State when updating this GameObject position and rotation;
        /// falls back to the tracked device's tracking state that drives the position or rotation action when not set.
        /// Must be an <see cref="IntegerControl"/> Control.
        /// </summary>
        /// <seealso cref="InputTrackingState"/>
        public InputActionProperty trackingStateAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for selecting an Interactable.
        /// Must be an action with a button-like interaction where phase equals performed when pressed.
        /// Typically a <see cref="ButtonControl"/> Control or a Value type action with a Press or Sector interaction.
        /// </summary>
        /// <seealso cref="selectActionValue"/>
        public InputActionProperty selectAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read values for selecting an Interactable.
        /// Must be an <see cref="AxisControl"/> Control or <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <remarks>
        /// Optional, Unity uses <see cref="selectAction"/> when not set.
        /// </remarks>
        /// <seealso cref="selectAction"/>
        public InputActionProperty selectActionValue
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for activating a selected Interactable.
        /// Must be an action with a button-like interaction where phase equals performed when pressed.
        /// Typically a <see cref="ButtonControl"/> Control or a Value type action with a Press or Sector interaction.
        /// </summary>
        /// <seealso cref="activateActionValue"/>
        public InputActionProperty activateAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read values for activating a selected Interactable.
        /// Must be an <see cref="AxisControl"/> Control or <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <remarks>
        /// Optional, Unity uses <see cref="activateAction"/> when not set.
        /// </remarks>
        /// <seealso cref="activateAction"/>
        public InputActionProperty activateActionValue
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for Canvas UI interaction.
        /// Must be an action with a button-like interaction where phase equals performed when pressed.
        /// Typically a <see cref="ButtonControl"/> Control or a Value type action with a Press interaction.
        /// </summary>
        /// <seealso cref="uiPressActionValue"/>
        public InputActionProperty uiPressAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read values for Canvas UI interaction.
        /// Must be an <see cref="AxisControl"/> Control or <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <remarks>
        /// Optional, Unity uses <see cref="uiPressAction"/> when not set.
        /// </remarks>
        /// <seealso cref="uiPressAction"/>
        public InputActionProperty uiPressActionValue
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to read values for Canvas UI scrolling.
        /// Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        /// <seealso cref="uiPressAction"/>
        public InputActionProperty uiScrollAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for identifying the device to send haptic impulses to.
        /// Can be any control type that will have an active control driving the action.
        /// </summary>
        public InputActionProperty hapticDeviceAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for rotating the interactor's attach point over time.
        /// Must be a <see cref="Vector2Control"/> Control. Uses the x-axis as the rotation input.
        /// </summary>
        public InputActionProperty rotateAnchorAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for computing a direction angle to rotate the interactor's attach point to match it.
        /// Must be a <see cref="Vector2Control"/> Control. The direction angle should be computed as the arctangent function of x/y.
        /// </summary>
        public InputActionProperty directionalAnchorRotationAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for translating the interactor's attach point closer or further away from the interactor.
        /// Must be a <see cref="Vector2Control"/> Control. Uses the y-axis as the translation input.
        /// </summary>
        public InputActionProperty translateAnchorAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use to enable or disable reading from the Scale Delta Action.
        /// Must be a <see cref="ButtonControl"/> Control. The pressed state of the button will toggle the scale state.
        /// </summary>
        /// <seealso cref="scaleDeltaAction"/>
        public InputActionProperty scaleToggleAction
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The Input System action to use for providing a scale delta value to transformers.
        /// Must be a <see cref="Vector2Control"/> Control. Uses the y-axis as the scale input.
        /// </summary>
        /// <seealso cref="UnityEngine.XR.Interaction.Toolkit.Interactors.IXRScaleValueProvider"/>
        public InputActionProperty scaleDeltaAction
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
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_ActionBasedControllerDeprecated);
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_ActionBasedControllerDeprecated);
        }

        /// <summary>
        /// Evaluates whether the given input action is considered performed.
        /// Unity automatically calls this method during <see cref="UpdateInput"/> to determine
        /// if the interaction state is active this frame.
        /// </summary>
        /// <param name="action">The input action to check.</param>
        /// <returns>Returns <see langword="true"/> when the input action is considered performed. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// More accurately, this evaluates whether the action with a button-like interaction is performed.
        /// Depending on the interaction of the input action, the control driving the value of the input action
        /// may technically be pressed and though the interaction may be in progress, it may not yet be performed,
        /// such as for a Hold interaction. In that example, this method returns <see langword="false"/>.
        /// </remarks>
        /// <seealso cref="InteractionState.active"/>
        protected virtual bool IsPressed(InputAction action)
        {
            throw new NotSupportedException(k_ActionBasedControllerDeprecated);
        }

        /// <summary>
        /// Reads and returns the given action value.
        /// Unity automatically calls this method during <see cref="UpdateInput"/> to determine
        /// the amount or strength of the interaction state this frame.
        /// </summary>
        /// <param name="action">The action to read the value from.</param>
        /// <returns>Returns the action value. If the action is <see langword="null"/> returns the default <see langword="float"/> value (<c>0f</c>).</returns>
        /// <seealso cref="InteractionState.value"/>
        protected virtual float ReadValue(InputAction action)
        {
            throw new NotSupportedException(k_ActionBasedControllerDeprecated);
        }

        /// <inheritdoc />
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            throw new NotSupportedException(k_ActionBasedControllerDeprecated);
        }
    }
}
