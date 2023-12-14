using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

#if AR_FOUNDATION_PRESENT
using UnityEngine.XR.ARFoundation;
#endif

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// A helper class for <see cref="CreateUtils"/>, isolating the creation logic for Main Cameras into
    /// a separate file.
    /// </summary>
    static class XRMainCameraFactory
    {
        internal static Camera CreateXRMainCamera(CreateUtils.HardwareTarget target)
        {
            switch (target)
            {
                case CreateUtils.HardwareTarget.VR:
                    return CreateVRMainCamera();
                case CreateUtils.HardwareTarget.MobileAR:
                    return CreateARMainCamera();
                default:
                    throw new InvalidEnumArgumentException($"Invalid {nameof(CreateUtils.HardwareTarget)}: {target}");
            }
        }

        static Camera CreateVRMainCamera()
        {
            var camera = CreateMainCamera();
            camera.nearClipPlane = 0.01f;

            SetupTrackedPoseDriverInput(camera);
            return camera;
        }

        static Camera CreateARMainCamera()
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                Debug.LogWarningFormat(
                    mainCam.gameObject,
                    "XR Origin Main Camera requires the \"MainCamera\" tag, but the current scene contains another enabled Camera tagged \"MainCamera\". For AR to function properly, remove the \"MainCamera\" tag from \'{0}\' or disable it.",
                    mainCam.name);
            }

            var camera = CreateMainCamera();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 20f;

#if AR_FOUNDATION_PRESENT
            var cameraGo = camera.gameObject;
            cameraGo.AddComponent<ARCameraManager>();
            cameraGo.AddComponent<ARCameraBackground>();
#endif

            SetupTrackedPoseDriverInput(camera);
            return camera;
        }

        static Camera CreateMainCamera()
        {
            var cameraGo = ObjectFactory.CreateGameObject(
                "Main Camera",
                typeof(Camera),
                typeof(AudioListener));

            var camera = cameraGo.GetComponent<Camera>();
            camera.tag = "MainCamera";

            return camera;
        }

        static void SetupTrackedPoseDriverInput(Camera camera)
        {
            var trackedPoseDriver = camera.gameObject.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();

            var positionAction = new InputAction("Position", binding: "<XRHMD>/centerEyePosition", expectedControlType: "Vector3");
            positionAction.AddBinding("<HandheldARInputDevice>/devicePosition");
            var rotationAction = new InputAction("Rotation", binding: "<XRHMD>/centerEyeRotation", expectedControlType: "Quaternion");
            rotationAction.AddBinding("<HandheldARInputDevice>/deviceRotation");
            var trackingStateAction = new InputAction("Tracking State", binding: "<XRHMD>/trackingState", expectedControlType: "Integer");

            trackedPoseDriver.positionInput = new InputActionProperty(positionAction);
            trackedPoseDriver.rotationInput = new InputActionProperty(rotationAction);
            trackedPoseDriver.trackingStateInput = new InputActionProperty(trackingStateAction);
            trackedPoseDriver.ignoreTrackingState = false;
        }
    }
}
