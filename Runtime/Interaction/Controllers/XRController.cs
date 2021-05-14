using UnityEngine.SpatialTracking;

#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets feature values on a tracked input controller device from the XR input subsystem
    /// into XR Interaction states, such as Select. Additionally, it applies the current Pose value
    /// of a tracked device to the transform of the GameObject.
    /// </summary>
    /// <remarks>
    /// It is recommended to use <see cref="ActionBasedController"/> instead of this behavior.
    /// This behavior does not need as much initial setup as compared to <see cref="ActionBasedController"/>,
    /// however input processing is less customizable and the XR Device Simulator cannot be used to drive
    /// this behavior.
    /// </remarks>
    /// <seealso cref="XRBaseController"/>
    /// <seealso cref="ActionBasedController"/>
    [AddComponentMenu("XR/XR Controller (Device-based)")]
    [HelpURL(XRHelpURLConstants.k_XRController)]
    public class XRController : XRBaseController
    {
        [SerializeField]
        XRNode m_ControllerNode = XRNode.RightHand;

        /// <summary>
        /// The <see cref="XRNode"/> for this controller.
        /// </summary>
        public XRNode controllerNode
        {
            get => m_ControllerNode;
            set => m_ControllerNode = value;
        }

        [SerializeField]
        InputHelpers.Button m_SelectUsage = InputHelpers.Button.Grip;

        /// <summary>
        /// The input to use for detecting a select.
        /// </summary>
        public InputHelpers.Button selectUsage
        {
            get => m_SelectUsage;
            set => m_SelectUsage = value;
        }

        [SerializeField]
        InputHelpers.Button m_ActivateUsage = InputHelpers.Button.Trigger;

        /// <summary>
        /// The input to use for detecting activation.
        /// </summary>
        public InputHelpers.Button activateUsage
        {
            get => m_ActivateUsage;
            set => m_ActivateUsage = value;
        }

        [SerializeField]
        InputHelpers.Button m_UIPressUsage = InputHelpers.Button.Trigger;

        /// <summary>
        /// The input to use for detecting a UI press.
        /// </summary>
        public InputHelpers.Button uiPressUsage
        {
            get => m_UIPressUsage;
            set => m_UIPressUsage = value;
        }

        [SerializeField]
        float m_AxisToPressThreshold = 0.1f;

        /// <summary>
        /// The amount an axis needs to be pressed to trigger an interaction event.
        /// </summary>
        public float axisToPressThreshold
        {
            get => m_AxisToPressThreshold;
            set => m_AxisToPressThreshold = value;
        }

        [SerializeField]
        InputHelpers.Button m_RotateAnchorLeft = InputHelpers.Button.PrimaryAxis2DLeft;

        /// <summary>
        /// The input to use to rotate an anchor to the Left.
        /// </summary>
        public InputHelpers.Button rotateObjectLeft
        {
            get => m_RotateAnchorLeft;
            set => m_RotateAnchorLeft = value;
        }

        [SerializeField]
        InputHelpers.Button m_RotateAnchorRight = InputHelpers.Button.PrimaryAxis2DRight;

        /// <summary>
        /// The input to use to rotate an anchor to the Right.
        /// </summary>
        public InputHelpers.Button rotateObjectRight
        {
            get => m_RotateAnchorRight;
            set => m_RotateAnchorRight = value;
        }

        [SerializeField]
        InputHelpers.Button m_MoveObjectIn = InputHelpers.Button.PrimaryAxis2DUp;

        /// <summary>
        /// The input that will be used to translate the anchor away from the interactor.
        /// </summary>
        public InputHelpers.Button moveObjectIn
        {
            get => m_MoveObjectIn;
            set => m_MoveObjectIn = value;
        }

        [SerializeField]
        InputHelpers.Button m_MoveObjectOut = InputHelpers.Button.PrimaryAxis2DDown;

        /// <summary>
        /// The input that will be used to translate the anchor towards the interactor.
        /// </summary>
        public InputHelpers.Button moveObjectOut
        {
            get => m_MoveObjectOut;
            set => m_MoveObjectOut = value;
        }

#if LIH_PRESENT
        [SerializeField]
        BasePoseProvider m_PoseProvider;

        /// <summary>
        /// Pose provider used to provide tracking data separate from the <see cref="XRNode"/>.
        /// </summary>
        public BasePoseProvider poseProvider
        {
            get => m_PoseProvider;
            set => m_PoseProvider = value;
        }
#endif

        InputDevice m_InputDevice;
        /// <summary>
        /// (Read Only) The <see cref="InputDevice"/> being used to read data from.
        /// </summary>
        public InputDevice inputDevice => m_InputDevice.isValid ? m_InputDevice : m_InputDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            controllerState.poseDataFlags = PoseDataFlags.NoData;
#if LIH_PRESENT_V1API
            if (m_PoseProvider != null)
            {
                if (m_PoseProvider.TryGetPoseFromProvider(out var poseProviderPose))
                {
                    controllerState.position = poseProviderPose.position;
                    controllerState.rotation = poseProviderPose.rotation;
                    controllerState.poseDataFlags = PoseDataFlags.Position | PoseDataFlags.Rotation;
                }
            }
            else
#elif LIH_PRESENT_V2API
            if (m_PoseProvider != null)
            {
                var retFlags = m_PoseProvider.GetPoseFromProvider(out var poseProviderPose);
                if ((retFlags & PoseDataFlags.Position) != 0)
                {
                    controllerState.position = poseProviderPose.position;
                    controllerState.poseDataFlags |= PoseDataFlags.Position;
                }
                if ((retFlags & PoseDataFlags.Rotation) != 0)
                {
                    controllerState.rotation = poseProviderPose.rotation;
                    controllerState.poseDataFlags |= PoseDataFlags.Rotation;
                }
            }
            else
#endif
            {
                if (inputDevice.TryGetFeatureValue(CommonUsages.trackingState, out var trackingState))
                {
                    if ((trackingState & InputTrackingState.Position) != 0 &&
                        inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition))
                    {
                        controllerState.position = devicePosition;
                        controllerState.poseDataFlags |= PoseDataFlags.Position;
                    }

                    if ((trackingState & InputTrackingState.Rotation) != 0 &&
                        inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out var deviceRotation))
                    {
                        controllerState.rotation = deviceRotation;
                        controllerState.poseDataFlags |= PoseDataFlags.Rotation;
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);
            if (controllerState == null)
                return;

            controllerState.ResetFrameDependentStates();
            controllerState.selectInteractionState.SetFrameState(IsPressed(m_SelectUsage));
            controllerState.activateInteractionState.SetFrameState(IsPressed(m_ActivateUsage));
            controllerState.uiPressInteractionState.SetFrameState(IsPressed(m_UIPressUsage));
        }

        /// <summary>
        /// Evaluates whether the button is considered pressed.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns <see langword="true"/> when the button is considered pressed. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool IsPressed(InputHelpers.Button button)
        {
            inputDevice.IsPressed(button, out var pressed, m_AxisToPressThreshold);
            return pressed;
        }

        /// <inheritdoc />
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            if (inputDevice.TryGetHapticCapabilities(out var capabilities) &&
                capabilities.supportsImpulse)
            {
                return inputDevice.SendHapticImpulse(0u, amplitude, duration);
            }
            return false;
        }
    }
}
