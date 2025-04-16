#if AR_FOUNDATION_5_2_OR_NEWER && (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX)
#define XR_SIMULATION_AVAILABLE
#endif

using System;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

#if XR_SIMULATION_AVAILABLE
#if XR_MANAGEMENT_4_0_OR_NEWER
using UnityEngine.XR.Management;
#endif
using UnityEngine.XR.Simulation;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// The target device or devices to update from input.
    /// </summary>
    /// <remarks>
    /// <see cref="FlagsAttribute"/> to support updating multiple controls from one input
    /// (e.g. to drive a controller and the head from the same input).
    /// </remarks>
    [Flags]
    public enum TargetedDevices
    {
        /// <summary>
        /// No target device to update.
        /// </summary>
        None = 0,

        /// <summary>
        /// No target device, behaving as an FPS controller.
        /// </summary>
        FPS = 1 << 0,

        /// <summary>
        /// Update left controller or hand position and rotation.
        /// </summary>
        LeftDevice = 1 << 1,

        /// <summary>
        /// Update right controller or hand position and rotation.
        /// </summary>
        RightDevice = 1 << 2,

        /// <summary>
        /// Update HMD position and rotation.
        /// </summary>
        HMD = 1 << 3,
    }

    /// <summary>
    /// The target device control(s) to update from input.
    /// </summary>
    /// <remarks>
    /// <see cref="FlagsAttribute"/> to support updating multiple controls from input
    /// (e.g. to drive the primary and secondary 2D axis on a controller from the same input).
    /// </remarks>
    /// <seealso cref="XRDeviceSimulator.axis2DTargets"/>
    [Flags]
    public enum Axis2DTargets
    {
        /// <summary>
        /// Do not update device state from input.
        /// </summary>
        None = 0,

        /// <summary>
        /// Update device position from input.
        /// </summary>
        Position = 1 << 0,

        /// <summary>
        /// Update the primary touchpad or joystick on a controller device from input.
        /// </summary>
        Primary2DAxis = 1 << 1,

        /// <summary>
        /// Update the secondary touchpad or joystick on a controller device from input.
        /// </summary>
        Secondary2DAxis = 1 << 2,
    }

    /// <summary>
    /// The device input mode of the left and right controller.
    /// </summary>
    public enum ControllerInputMode
    {
        /// <summary>
        /// No current active mode.
        /// </summary>
        None,

        /// <summary>
        /// Mode for simulating controller trigger input.
        /// </summary>
        Trigger,

        /// <summary>
        /// Mode for simulating controller grip input.
        /// </summary>
        Grip,

        /// <summary>
        /// Mode for simulating controller primary button input.
        /// </summary>
        PrimaryButton,

        /// <summary>
        /// Mode for simulating controller secondary button input.
        /// </summary>
        SecondaryButton,

        /// <summary>
        /// Mode for simulating controller menu button input.
        /// </summary>
        Menu,

        /// <summary>
        /// Mode for simulating controller primary axis 2D click input.
        /// </summary>
        Primary2DAxisClick,

        /// <summary>
        /// Mode for simulating controller secondary axis 2D click input.
        /// </summary>
        Secondary2DAxisClick,

        /// <summary>
        /// Mode for simulating controller primary axis 2D touch input.
        /// </summary>
        Primary2DAxisTouch,

        /// <summary>
        /// Mode for simulating controller secondary axis 2D touch input.
        /// </summary>
        Secondary2DAxisTouch,

        /// <summary>
        /// Mode for simulating controller primary touch input.
        /// </summary>
        PrimaryTouch,

        /// <summary>
        /// Mode for simulating controller secondary touch input.
        /// </summary>
        SecondaryTouch,
    }

    /// <summary>
    /// The coordinate space in which to operate.
    /// </summary>
    public enum Space
    {
        /// <summary>
        /// Applies translations of a controller or HMD relative to its own coordinate space, considering its own rotations.
        /// Will translate a controller relative to itself, independent of the camera.
        /// </summary>
        Local,

        /// <summary>
        /// Applies translations of a controller or HMD relative to its parent. If the object does not have a parent, meaning
        /// it is a root object, the parent coordinate space is the same as the world coordinate space. This is the same
        /// as <see cref="Local"/> but without considering its own rotations.
        /// </summary>
        Parent,

        /// <summary>
        /// Applies translations of a controller or HMD relative to the screen.
        /// Will translate a controller relative to the camera, independent of the controller's orientation.
        /// </summary>
        Screen,
    }

    /// <summary>
    /// Utility functions for simulation classes.
    /// </summary>
    static class XRSimulatorUtility
    {
        /// <summary>
        /// The maximum angle the XR Camera can have around the x-axis.
        /// </summary>
        internal static readonly float cameraMaxXAngle = 80f;

        internal static readonly Vector3 leftDeviceDefaultInitialPosition = new Vector3(-0.1f, -0.05f, 0.3f);
        internal static readonly Vector3 rightDeviceDefaultInitialPosition = new Vector3(0.1f, -0.05f, 0.3f);

        internal static SimulatedDeviceLifecycleManager FindCreateSimulatedDeviceLifecycleManager(GameObject simulator)
        {
            if (ComponentLocatorUtility<SimulatedDeviceLifecycleManager>.TryFindComponent(out var simulatedDeviceLifecycleManager))
            {
                return simulatedDeviceLifecycleManager;
            }
            else
            {
                simulatedDeviceLifecycleManager = simulator.AddComponent<SimulatedDeviceLifecycleManager>();
                return simulatedDeviceLifecycleManager;
            }
        }

        internal static SimulatedHandExpressionManager FindCreateSimulatedHandExpressionManager(GameObject simulator)
        {
            if (ComponentLocatorUtility<SimulatedHandExpressionManager>.TryFindComponent(out var simulatedHandExpressionManager))
            {
                return simulatedHandExpressionManager;
            }
            else
            {
                simulatedHandExpressionManager = simulator.AddComponent<SimulatedHandExpressionManager>();
                return simulatedHandExpressionManager;
            }
        }

        internal static void Subscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetInputAction(reference);
            if (action != null)
            {
                if (performed != null)
                    action.performed += performed;
                if (canceled != null)
                    action.canceled += canceled;
            }
        }

        internal static void Unsubscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetInputAction(reference);
            if (action != null)
            {
                if (performed != null)
                    action.performed -= performed;
                if (canceled != null)
                    action.canceled -= canceled;
            }
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }

#if ENABLE_VR || UNITY_GAMECORE
        internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedControllerState state, in Quaternion inverseCameraParentRotation)
            => GetDeltaRotation(translateSpace, state.deviceRotation, inverseCameraParentRotation);

        internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedHandState state, in Quaternion inverseCameraParentRotation)
            => GetDeltaRotation(translateSpace, state.rotation, inverseCameraParentRotation);

        internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedHMDState state, in Quaternion inverseCameraParentRotation)
            => GetDeltaRotation(translateSpace, state.centerEyeRotation, inverseCameraParentRotation);

        internal static void GetAxes(Space translateSpace, Transform cameraTransform, out Vector3 right, out Vector3 up, out Vector3 forward)
        {
            if (cameraTransform == null)
                throw new ArgumentNullException(nameof(cameraTransform));

            switch (translateSpace)
            {
                case Space.Local:
                    // Makes the assumption that the Camera and the Controllers are siblings
                    // (meaning they share a parent GameObject).
                    var cameraParent = cameraTransform.parent;
                    if (cameraParent != null)
                    {
                        right = cameraParent.TransformDirection(Vector3.right);
                        up = cameraParent.TransformDirection(Vector3.up);
                        forward = cameraParent.TransformDirection(Vector3.forward);
                    }
                    else
                    {
                        right = Vector3.right;
                        up = Vector3.up;
                        forward = Vector3.forward;
                    }

                    break;
                case Space.Parent:
                    right = Vector3.right;
                    up = Vector3.up;
                    forward = Vector3.forward;
                    break;
                case Space.Screen:
                    right = cameraTransform.TransformDirection(Vector3.right);
                    up = cameraTransform.TransformDirection(Vector3.up);
                    forward = cameraTransform.TransformDirection(Vector3.forward);
                    break;
                default:
                    right = Vector3.right;
                    up = Vector3.up;
                    forward = Vector3.forward;
                    Assert.IsTrue(false, $"Unhandled {nameof(translateSpace)}={translateSpace}.");
                    return;
            }
        }

        internal static Quaternion GetDeltaRotation(Space translateSpace, Quaternion rotation, in Quaternion inverseCameraParentRotation)
        {
            switch (translateSpace)
            {
                case Space.Local:
                    return rotation * inverseCameraParentRotation;
                case Space.Parent:
                    return Quaternion.identity;
                case Space.Screen:
                    return inverseCameraParentRotation;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(translateSpace)}={translateSpace}.");
                    return Quaternion.identity;
            }
        }
#endif

#if XR_SIMULATION_AVAILABLE && XR_MANAGEMENT_4_0_OR_NEWER
        internal static bool XRSimulationLoaderEnabledForEditorPlayMode()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
                return XRGeneralSettings.Instance.Manager.activeLoader is SimulationLoader simulationLoader && simulationLoader != null;

            return false;
        }
#endif

        /// <summary>
        /// Finds and sets camera transform if necessary.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the camera reference is valid. Otherwise, returns <see langword="false"/>.</returns>
        internal static bool FindCameraTransform(ref (Transform transform, Camera camera) cachedCamera, ref Transform cameraTransform)
        {
            // Sync the cache tuple if necessary
            if (cachedCamera.transform != cameraTransform)
                cachedCamera = (cameraTransform, cameraTransform != null ? cameraTransform.GetComponent<Camera>() : null);

            // Camera.main returns the first active and enabled main camera, so if the cached one
            // is no longer enabled, find the new main camera. This is to support, for example,
            // toggling between different XROrigin rigs each with their own main camera.
            if (cachedCamera.transform == null ||
                (cachedCamera.camera != null && !cachedCamera.camera.isActiveAndEnabled))
            {
                var mainCamera = Camera.main;
                if (mainCamera == null)
                    return false;

                cameraTransform = mainCamera.transform;
                cachedCamera = (cameraTransform, cameraTransform.GetComponent<Camera>());
            }

            return true;
        }

        internal static unsafe bool TryExecuteCommand(InputDeviceCommand* commandPtr, out long result)
        {
            // This is a utility method called by XRSimulatedHMD and XRSimulatedController
            // since both devices share the same command handling.
            // This replicates the logic in XRToISXDevice::IOCTL (XRInputToISX.cpp).
            var type = commandPtr->type;
            if (type == RequestSyncCommand.Type)
            {
                // The state is maintained by XRDeviceSimulator, so no need for any change
                // when focus is regained. Returning success instructs Input System to not
                // reset the device.
                result = InputDeviceCommand.GenericSuccess;
                return true;
            }

            if (type == QueryCanRunInBackground.Type)
            {
                ((QueryCanRunInBackground*)commandPtr)->canRunInBackground = true;
                result = InputDeviceCommand.GenericSuccess;
                return true;
            }

            result = default;
            return false;
        }

        internal static Vector3 GetTranslationInDeviceSpace(float xTranslateInput, float yTranslateInput, float zTranslateInput, Transform cameraTransform, Quaternion cameraParentRotation, Quaternion inverseCameraParentRotation)
        {
            var translationInWorldSpace = GetTranslationInWorldSpace(xTranslateInput, yTranslateInput, zTranslateInput, cameraTransform, cameraParentRotation);
            var translationInDeviceSpace = inverseCameraParentRotation * translationInWorldSpace;

            return translationInDeviceSpace;
        }

        internal static Vector3 GetTranslationInWorldSpace(float xTranslateInput, float yTranslateInput, float zTranslateInput, Transform cameraTransform, Quaternion cameraParentRotation)
        {
            var scaledKeyboardTranslateInput = new Vector3(
                xTranslateInput,
                yTranslateInput,
                zTranslateInput);

            var forward = cameraTransform.forward;
            var cameraParentUp = cameraParentRotation * Vector3.up;
            if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(forward, cameraParentUp)), 1f))
            {
                forward = -cameraTransform.up;
            }

            var forwardProjected = Vector3.ProjectOnPlane(forward, cameraParentUp);
            var forwardRotation = Quaternion.LookRotation(forwardProjected, cameraParentUp);
            var translationInWorldSpace = forwardRotation * scaledKeyboardTranslateInput;

            return translationInWorldSpace;
        }
    }

    /// <summary>
    /// Extension methods for <see cref="TargetedDevices"/>.
    /// </summary>
    static class TargetedDeviceExtensions
    {
        /// <summary>
        /// Returns the flags enum with the given flag set.
        /// </summary>
        /// <param name="devices">The flags enum instance.</param>
        /// <param name="device">The flag to also set in the returned instance.</param>
        /// <returns>Returns the flags enum with the given flag set.</returns>
        public static TargetedDevices WithDevice(this TargetedDevices devices, TargetedDevices device)
        {
            return devices | device;
        }

        /// <summary>
        /// Returns the flags enum with the given flag not set.
        /// </summary>
        /// <param name="devices">The flags enum instance.</param>
        /// <param name="device">The flag to clear in the returned instance.</param>
        /// <returns>Returns the flags enum with the given flag not set.</returns>
        public static TargetedDevices WithoutDevice(this TargetedDevices devices, TargetedDevices device)
        {
            return devices & ~device;
        }

        /// <summary>
        /// Determines whether one or more bit fields are set in the flags
        /// Non-boxing version of <c>HasFlag</c> for <see cref="TargetedDevices"/>.
        /// </summary>
        /// <param name="devices">The flags enum instance.</param>
        /// <param name="device">The flag to check if set.</param>
        /// <returns>Returns <see langword="true"/> if the bit field or bit fields are set, otherwise returns <see langword="false"/>.</returns>
        public static bool HasDevice(this TargetedDevices devices, TargetedDevices device)
        {
            return (devices & device) == device;
        }
    }
}
