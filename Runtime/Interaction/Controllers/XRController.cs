using System;
#if XR_LEGACY_INPUT_HELPERS_2_1_OR_NEWER || PACKAGE_DOCS_GENERATION
using UnityEngine.Experimental.XR.Interaction;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets feature values on a tracked input controller device from the XR input subsystem
    /// into XR Interaction states, such as Select. Additionally, it applies the current Pose value
    /// of a tracked device to the transform of the <see cref="GameObject"/>.
    /// </summary>
    /// <remarks>
    /// It is recommended to use <see cref="ActionBasedController"/> instead of this behavior.
    /// This behavior does not need as much initial setup as compared to <see cref="ActionBasedController"/>,
    /// however input processing is less customizable and the <see cref="Inputs.Simulation.XRDeviceSimulator"/> cannot be used to drive
    /// this behavior.
    /// </remarks>
    /// <seealso cref="XRBaseController"/>
    /// <seealso cref="ActionBasedController"/>
    [AddComponentMenu("/XR Controller (Device-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_XRController)]
    [Obsolete(k_XRControllerDeprecated, true)]
    public class XRController : XRBaseController
    {
        const string k_XRControllerDeprecated = "XRController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.";

        /// <summary>
        /// The <see cref="XRNode"/> for this controller.
        /// </summary>
        public XRNode controllerNode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use for detecting a select.
        /// </summary>
        public InputHelpers.Button selectUsage
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use for detecting activation.
        /// </summary>
        public InputHelpers.Button activateUsage
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use for detecting a UI press.
        /// </summary>
        public InputHelpers.Button uiPressUsage
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The amount that a user needs to press an axis in order to trigger an interaction event.
        /// </summary>
        public float axisToPressThreshold
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use to rotate an anchor to the Left.
        /// </summary>
        public InputHelpers.Button rotateObjectLeft
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use to rotate an anchor to the Right.
        /// </summary>
        public InputHelpers.Button rotateObjectRight
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input that will be used to translate the anchor away from the interactor (into the screen / away from the player).
        /// </summary>
        public InputHelpers.Button moveObjectIn
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input that will be used to translate the anchor towards the interactor (out of the screen / towards the player).
        /// </summary>
        public InputHelpers.Button moveObjectOut
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The input to use to compute a directional angle to rotate the interactor's attach point to match it.
        /// </summary>
        public InputHelpers.Axis2D directionalAnchorRotation
        {
            get => default;
            set => _ = value;
        }

#if XR_LEGACY_INPUT_HELPERS_2_1_OR_NEWER || PACKAGE_DOCS_GENERATION

        /// <summary>
        /// Pose provider used to provide tracking data separate from the <see cref="controllerNode"/>.
        /// </summary>
        public BasePoseProvider poseProvider
        {
            get => default;
            set => _ = value;
        }

#else
#endif
        /// <summary>
        /// (Read Only) The <see cref="InputDevice"/> Unity uses to read data from.
        /// </summary>
        public InputDevice inputDevice => default;

        /// <inheritdoc />
        protected override void Awake()
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }

        /// <summary>
        /// Evaluates whether the button is considered pressed.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns <see langword="true"/> when the button is considered pressed. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool IsPressed(InputHelpers.Button button)
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }

        /// <summary>
        /// Reads and returns the given action value.
        /// </summary>
        /// <param name="button">The button to read the value from.</param>
        /// <returns>Returns the button value.</returns>
        protected virtual float ReadValue(InputHelpers.Button button)
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }

        /// <inheritdoc />
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            throw new NotSupportedException(k_XRControllerDeprecated);
        }
    }
}
