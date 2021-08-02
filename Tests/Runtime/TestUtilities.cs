using System;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    public static class TestUtilities
    {
        internal static void DestroyAllSceneObjects()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name.Contains("tests runner"))
                    continue;
                Object.DestroyImmediate(go);
            }
        }

        internal static void CreateGOSphereCollider(GameObject go, bool isTrigger = true)
        {
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.radius = 1.0f;
            collider.isTrigger = isTrigger;
        }

        internal static XRInteractionManager CreateInteractionManager()
        {
            GameObject managerGO = new GameObject("Interaction Manager");
            XRInteractionManager manager = managerGO.AddComponent<XRInteractionManager>();
            return manager;
        }

        internal static XRDirectInteractor CreateDirectInteractor()
        {
            GameObject interactorGO = new GameObject("Direct Interactor");
            CreateGOSphereCollider(interactorGO);
            XRController controller = interactorGO.AddComponent<XRController>();
            XRDirectInteractor interactor = interactorGO.AddComponent<XRDirectInteractor>();
            interactor.xrController = controller;
            controller.enableInputTracking = false;
            controller.enableInputActions = false;
            return interactor;
        }

        internal static XRRig CreateXRRig()
        {
            var xrRigGO = new GameObject("XR Rig");
            xrRigGO.SetActive(false);
            var xrRig = xrRigGO.AddComponent<XRRig>();
            xrRig.rig = xrRigGO;

            // Add camera offset
            var cameraOffsetGO = new GameObject("CameraOffset");
            cameraOffsetGO.transform.SetParent(xrRig.transform,false);
            xrRig.cameraFloorOffsetObject = cameraOffsetGO;

            xrRig.transform.position = Vector3.zero;
            xrRig.transform.rotation = Quaternion.identity;

            // Add camera
            var cameraGO = new GameObject("Camera");
            var camera = cameraGO.AddComponent<Camera>();

            cameraGO.transform.SetParent(cameraOffsetGO.transform, false);
            xrRig.cameraGameObject = cameraGO;
            xrRigGO.SetActive(true);

            XRDevice.DisableAutoXRCameraTracking(camera, true);

            return xrRig;
        }

        internal static TeleportationAnchor CreateTeleportAnchorPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "plane";
            TeleportationAnchor teleAnchor = plane.AddComponent<TeleportationAnchor>();
            return teleAnchor;
        }

        internal static XRRayInteractor CreateRayInteractor()
        {
            GameObject interactorGO = new GameObject("Ray Interactor");
            XRController controller = interactorGO.AddComponent<XRController>();
            XRRayInteractor interactor = interactorGO.AddComponent<XRRayInteractor>();
            XRInteractorLineVisual ilv = interactorGO.AddComponent<XRInteractorLineVisual>();
            interactor.xrController = controller;
            controller.enableInputTracking = false;
            interactor.enableUIInteraction = false;
            controller.enableInputActions = false;
            return interactor;
        }

        internal static XRSocketInteractor CreateSocketInteractor()
        {
            GameObject interactorGO = new GameObject("Socket Interactor");
            CreateGOSphereCollider(interactorGO);
            XRSocketInteractor interactor = interactorGO.AddComponent<XRSocketInteractor>();
            return interactor;
        }

        internal static XRGrabInteractable CreateGrabInteractable()
        {
            GameObject interactableGO = new GameObject("Grab Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRGrabInteractable interactable = interactableGO.AddComponent<XRGrabInteractable>();
            var rigidBody = interactableGO.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRSimpleInteractable CreateSimpleInteractable()
        {
            GameObject interactableGO = new GameObject("Simple Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRControllerRecorder CreateControllerRecorder(XRController controller, Action<XRControllerRecording> addRecordingFrames)
        {
            var controllerRecorder = controller.gameObject.AddComponent<XRControllerRecorder>();
            controllerRecorder.xrController = controller;
            controllerRecorder.recording = ScriptableObject.CreateInstance<XRControllerRecording>();

            addRecordingFrames(controllerRecorder.recording);
            return controllerRecorder;
        }
    }
}
