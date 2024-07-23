using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class LocomotionTests
    {
        static readonly XRRayInteractor.HitDetectionType[] s_HitDetectionTypes =
        {
            XRRayInteractor.HitDetectionType.Raycast,
            XRRayInteractor.HitDetectionType.SphereCast,
            XRRayInteractor.HitDetectionType.ConeCast,
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator TeleportToAreaWithSphereCastOverlapIsBlocked([ValueSource(nameof(s_HitDetectionTypes))] XRRayInteractor.HitDetectionType hitDetectionType)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;
            interactor.hitDetectionType = hitDetectionType;
            interactor.sphereCastRadius = 1.5f;

            // Create teleportation area
            var teleArea = TestUtilities.CreateTeleportAreaPlane();
            teleArea.interactionManager = manager;
            teleArea.teleportationProvider = teleProvider;
            teleArea.matchOrientation = MatchOrientation.TargetUp;

            // Pitch the interactor down so that the cast will hit the teleport area
            interactor.transform.position = Vector3.up;
            interactor.transform.Rotate(Vector3.right, 45f, Space.Self);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleArea }));

            var teleportingInvoked = false;
            teleArea.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input so teleport is triggered
            var controller = interactor.GetComponent<XRController>();
            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.None, false,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.5f, Vector3.zero, Quaternion.identity, InputTrackingState.None, false,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.None, false,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactor.isSelectActive, Is.True);

            Assert.That(interactor.TryGetCurrent3DRaycastHit(out var hit, out var hitIndex), Is.True);
            Assert.That(hitIndex, Is.EqualTo(1));

            if (hitDetectionType == XRRayInteractor.HitDetectionType.SphereCast)
            {
                // The interactor's sphere cast overlaps with the teleport area, so the cast does not return a point.
                Assert.That(hit.distance, Is.EqualTo(0f));
                Assert.That(hit.point, Is.EqualTo(Vector3.zero));
            }
            else
            {
                // The interactor is 1 unit above the teleport area aiming down at a 45-degree angle,
                // so the distance to the teleport area should be sqrt(2) units since each side of the
                // triangle is 1 unit.
                Assert.That(hit.distance, Is.EqualTo(Mathf.Sqrt(2f)).Within(1e-5f));
                Assert.That(hit.point, Is.EqualTo(Vector3.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
            }

            Assert.That(teleArea.IsSelected(interactor), hitDetectionType != XRRayInteractor.HitDetectionType.SphereCast ? Is.True : Is.False);
            Assert.That(teleportingInvoked, Is.False);

            yield return new WaitForSeconds(1f);

            Assert.That(interactor.isSelectActive, Is.False);

            Assert.That(teleArea.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, hitDetectionType != XRRayInteractor.HitDetectionType.SphereCast ? Is.True : Is.False);
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndMatchTargetUp()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            LocomotionSystem locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            TeleportationProvider teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;

            // set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45, 0, 0, Space.World);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);
            Vector3 cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
            Assert.That(projectedCameraForward.normalized, Is.EqualTo(teleAnchor.transform.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndMatchWorldSpace()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            LocomotionSystem locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            TeleportationProvider teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.WorldSpaceUp;

            // set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45, 0, 0, Space.World);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);
            Vector3 cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin position");
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(Vector3.up).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin up vector");
            Assert.That(xrOrigin.Camera.transform.forward, Is.EqualTo(Vector3.forward).Using(Vector3ComparerWithEqualsOperator.Instance), "Projected forward");
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndMatchTargetUpAndForward()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            LocomotionSystem locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            TeleportationProvider teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUpAndForward;

            // set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45, 0, 0, Space.World);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);
            Vector3 cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
            Assert.That(projectedCameraForward.normalized, Is.EqualTo(teleAnchor.transform.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndFilterByHitNormal()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchor with plane as child so the hit normal can be misaligned with the anchor's up
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "plane";
            var planeTrans = plane.transform;
            var teleAnchorTrans = new GameObject("teleportation anchor").transform;
            planeTrans.SetParent(teleAnchorTrans);
            planeTrans.Rotate(-45, 0, 0, Space.World);
            teleAnchorTrans.position = interactor.transform.forward + Vector3.down;
            var teleAnchor = teleAnchorTrans.gameObject.AddComponent<TeleportationAnchor>();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUpAndForward;
            teleAnchor.filterSelectionByHitNormal = true;
            teleAnchor.upNormalToleranceDegrees = 30f;

            var cameraTrans = xrOrigin.Camera.transform;
            var originalCameraPosition = cameraTrans.position;
            var originalCameraForward = cameraTrans.forward;
            var originTrans = xrOrigin.transform;
            var originalOriginUp = originTrans.up;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action. first teleportation should fail
            yield return new WaitForSeconds(1f);
            Assert.That(cameraTrans.position, Is.EqualTo(originalCameraPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(originTrans.up, Is.EqualTo(originalOriginUp).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(cameraTrans.forward, Is.EqualTo(originalCameraForward).Using(Vector3ComparerWithEqualsOperator.Instance));

            // now increase the normal tolerance and try teleporting again
            controllerRecorder.isPlaying = false;
            controllerRecorder.ResetPlayback();
            teleAnchor.upNormalToleranceDegrees = 50f;
            controllerRecorder.isPlaying = true;
            yield return new WaitForSeconds(1f);
            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
            var projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
            Assert.That(projectedCameraForward.normalized, Is.EqualTo(teleAnchor.transform.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithProjectile()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            LocomotionSystem locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            TeleportationProvider teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.ProjectileCurve;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;

            // set teleportation anchor plane
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-90, 0, 0, Space.World);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);

            Vector3 cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithBezierCurve()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            LocomotionSystem locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            TeleportationProvider teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.BezierCurve;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;

            // set teleportation anchor plane
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-90, 0, 0, Space.World);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.2f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);

            Vector3 cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndMatchWorldUpAndDirectionalInput()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // fake directional input by manually rotating the attach transform
            var attachTransform = interactor.attachTransform;
            attachTransform.Rotate(Vector3.up, 30f);

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.WorldSpaceUp;
            teleAnchor.matchDirectionalInput = true;

            // set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45, 0, 0, Space.World);

            // calculate expected forward direction AFTER rotating attach transform but BEFORE the controller performs the fake teleportation
            var expectedForward = Vector3.ProjectOnPlane(attachTransform.forward, Vector3.up).normalized;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);
            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(Vector3.up).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin up vector");
            Assert.That(xrOrigin.Camera.transform.forward, Is.EqualTo(expectedForward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndMatchTargetUpAndDirectionalInput()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // config teleportation on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // interactor
            var interactor = TestUtilities.CreateRayInteractor();

            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // controller
            var controller = interactor.GetComponent<XRController>();

            // fake directional input by manually rotating the attach transform
            var attachTransform = interactor.attachTransform;
            attachTransform.Rotate(Vector3.up, 30f);

            // create teleportation anchors
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;
            teleAnchor.matchDirectionalInput = true;

            // set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45, 0, 0, Space.World);

            // calculate expected forward direction AFTER rotating attach transform but BEFORE the controller performs the fake teleportation
            var expectedForward = Vector3.ProjectOnPlane(attachTransform.forward, teleAnchor.transform.up).normalized;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
            });
            controllerRecorder.isPlaying = true;

            // wait for 1s to make sure the recorder simulates the action
            yield return new WaitForSeconds(1f);
            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin up vector");
            var projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
            Assert.That(projectedCameraForward.normalized, Is.EqualTo(expectedForward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorManually()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.system = locoSys;

            // Create teleportation anchor
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUpAndForward;

            teleAnchor.transform.position = new Vector3(0f, -1f, 1f);
            teleAnchor.transform.Rotate(-45f, 0f, 0f, Space.World);

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually trigger teleport to the anchor
            teleAnchor.RequestTeleport();

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            Assert.That(teleportingInvoked, Is.True);

            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
            var projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
            Assert.That(projectedCameraForward.normalized, Is.EqualTo(teleAnchor.transform.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator SnapTurn()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config snap turn on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            locoSys.xrOrigin = xrOrigin;
            var snapProvider = xrOrigin.gameObject.AddComponent<DeviceBasedSnapTurnProvider>();
            snapProvider.system = locoSys;
            float turnAmount = snapProvider.turnAmount;

            snapProvider.FakeStartTurn(false);

            yield return new WaitForSeconds(0.1f);

            Assert.That(xrOrigin.transform.rotation.eulerAngles, Is.EqualTo(new Vector3(0f, turnAmount, 0f)).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator SnapTurnAround()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config snap turn on XR Origin
            var locoSys = xrOrigin.gameObject.AddComponent<LocomotionSystem>();
            locoSys.xrOrigin = xrOrigin;
            var snapProvider = xrOrigin.gameObject.AddComponent<DeviceBasedSnapTurnProvider>();
            snapProvider.system = locoSys;

            snapProvider.FakeStartTurnAround();

            yield return new WaitForSeconds(0.1f);

            Assert.That(xrOrigin.transform.rotation.eulerAngles, Is.EqualTo(new Vector3(0f, 180f, 0f)).Using(Vector3ComparerWithEqualsOperator.Instance));
        }
    }
}
