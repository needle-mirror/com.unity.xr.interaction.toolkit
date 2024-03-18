using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class LocomotionTests
    {
        static readonly MatchOrientation[] s_MatchOrientations =
        {
            MatchOrientation.WorldSpaceUp,
            MatchOrientation.TargetUp,
            MatchOrientation.TargetUpAndForward,
        };

        static readonly MatchOrientation[] s_DirectionalMatchOrientations =
        {
            MatchOrientation.WorldSpaceUp,
            MatchOrientation.TargetUp,
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator LocomotionStateChanges()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();

            var provider1 = TestUtilities.CreateMockLocomotionProvider(mediator);
            var provider2 = TestUtilities.CreateMockLocomotionProvider(mediator);
            var provider3 = TestUtilities.CreateMockLocomotionProvider(mediator);

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Idle));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Frame 1:
            // Provider 1: Idle -> Moving
            // Provider 2: Idle -> Preparing
            // Provider 3: Idle -> Preparing

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider1.isLocomotionActive, Is.False);
            Assert.That(provider2.isLocomotionActive, Is.False);
            Assert.That(provider3.isLocomotionActive, Is.False);

            var provider1StartedLocomotion = false;
            var provider2StartedLocomotion = false;
            var provider3StartedLocomotion = false;
            provider1.locomotionStarted += provider => provider1StartedLocomotion = true;
            provider2.locomotionStarted += provider => provider2StartedLocomotion = true;
            provider3.locomotionStarted += provider => provider3StartedLocomotion = true;

            provider1.InvokeTryStartLocomotionImmediately();
            provider2.InvokeTryPrepareLocomotion();
            provider3.InvokeTryPrepareLocomotion();

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider1.isLocomotionActive, Is.True);
            Assert.That(provider2.isLocomotionActive, Is.True);
            Assert.That(provider3.isLocomotionActive, Is.True);
            Assert.That(provider1StartedLocomotion, Is.True);
            Assert.That(provider2StartedLocomotion, Is.False);
            Assert.That(provider3StartedLocomotion, Is.False);

            // Frame 2:
            // Provider 1: Moving    -> Moving
            // Provider 2: Preparing -> Moving
            // Provider 3: Preparing -> Preparing

            provider2.FinishPreparation();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider2.isLocomotionActive, Is.True);
            Assert.That(provider2StartedLocomotion, Is.True);
            Assert.That(provider3StartedLocomotion, Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Preparing));

            // Frame 3:
            // Provider 1: Moving    -> Ended
            // Provider 2: Moving    -> Ended
            // Provider 3: Preparing -> Ended

            var provider1EndedLocomotion = false;
            var provider2EndedLocomotion = false;
            var provider3EndedLocomotion = false;
            provider1.locomotionEnded += provider => provider1EndedLocomotion = true;
            provider2.locomotionEnded += provider => provider2EndedLocomotion = true;
            provider3.locomotionEnded += provider => provider3EndedLocomotion = true;

            provider1.InvokeTryEndLocomotion();
            provider2.InvokeTryEndLocomotion();
            provider3.InvokeTryEndLocomotion();

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(provider1.isLocomotionActive, Is.False);
            Assert.That(provider2.isLocomotionActive, Is.False);
            Assert.That(provider3.isLocomotionActive, Is.False);
            Assert.That(provider1EndedLocomotion, Is.True);
            Assert.That(provider2EndedLocomotion, Is.True);
            Assert.That(provider3EndedLocomotion, Is.True);

            // Frame 4:
            // Provider 1: Ended -> Preparing
            // Provider 2: Ended -> Idle
            // Provider 3: Ended -> Idle

            provider1.InvokeTryPrepareLocomotion();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider1.isLocomotionActive, Is.True);
            Assert.That(provider2.isLocomotionActive, Is.False);
            Assert.That(provider3.isLocomotionActive, Is.False);
        }

        [UnityTest]
        public IEnumerator ProviderCanOnlyPrepareLocomotionWhenLocomotionInactive()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();
            var provider = TestUtilities.CreateMockLocomotionProvider(mediator);

            // Can prepare if Idle
            Assert.That(provider.InvokeTryPrepareLocomotion(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Preparing));

            // Cannot prepare if Preparing
            Assert.That(provider.InvokeTryPrepareLocomotion(), Is.False);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Preparing));

            provider.FinishPreparation();

            yield return new WaitForFixedUpdate();
            yield return null;

            // Cannot prepare if Moving
            Assert.That(provider.InvokeTryPrepareLocomotion(), Is.False);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));

            provider.InvokeTryEndLocomotion();

            // Can prepare if Ended
            Assert.That(provider.InvokeTryPrepareLocomotion(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Preparing));
        }

        [UnityTest]
        public IEnumerator ProviderCanOnlyStartLocomotionWhenNotMoving()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();
            var provider = TestUtilities.CreateMockLocomotionProvider(mediator);

            // Can start if Idle
            Assert.That(provider.InvokeTryStartLocomotionImmediately(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));

            // Cannot start if Moving
            Assert.That(provider.InvokeTryStartLocomotionImmediately(), Is.False);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));

            provider.InvokeTryEndLocomotion();

            // Can start if Ended
            Assert.That(provider.InvokeTryStartLocomotionImmediately(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));

            provider.InvokeTryEndLocomotion();

            yield return new WaitForFixedUpdate();
            yield return null;

            provider.InvokeTryPrepareLocomotion();

            // Can start if Preparing
            Assert.That(provider.InvokeTryStartLocomotionImmediately(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));
        }

        [UnityTest]
        public IEnumerator ProviderCanOnlyEndLocomotionWhenLocomotionActive()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();
            var provider = TestUtilities.CreateMockLocomotionProvider(mediator);

            provider.InvokeTryPrepareLocomotion();

            // Can end if Preparing
            Assert.That(provider.InvokeTryEndLocomotion(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Ended));

            provider.InvokeTryStartLocomotionImmediately();

            // Can end if Moving
            Assert.That(provider.InvokeTryEndLocomotion(), Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Ended));

            // Cannot end if Ended
            Assert.That(provider.InvokeTryEndLocomotion(), Is.False);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Ended));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Cannot end if Idle
            Assert.That(provider.InvokeTryEndLocomotion(), Is.False);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Idle));
        }

        [UnityTest]
        public IEnumerator ProviderCanOnlyQueueTransformationWhenMoving()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();
            var provider = TestUtilities.CreateMockLocomotionProvider(mediator);

            var beforeStepLocomotionInvoked = false;
            var transformationApplied = false;
            provider.beforeStepLocomotion += locomotionProvider => beforeStepLocomotionInvoked = true;
            provider.delegateTransformation.transformation += body => transformationApplied = true;

            // Cannot queue if Idle
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            provider.InvokeTryPrepareLocomotion();

            // Cannot queue if Preparing
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.False);
            Assert.That(transformationApplied, Is.False);

            provider.InvokeTryStartLocomotionImmediately();

            // Can queue if Moving
            Assert.That(provider.InvokeTryQueueTransformation(), Is.True);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.True);
            Assert.That(transformationApplied, Is.True);

            beforeStepLocomotionInvoked = false;
            transformationApplied = false;
            provider.InvokeTryEndLocomotion();

            // Cannot queue if Ending
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.False);
            Assert.That(transformationApplied, Is.False);
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLine([ValueSource(nameof(s_MatchOrientations))] MatchOrientation matchOrientation)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // Create teleportation anchor
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = matchOrientation;

            // Set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45f, 0f, 0f, Space.World);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleAnchor }));

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input so teleport is triggered
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.True);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.True);

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            if (matchOrientation == MatchOrientation.TargetUp ||
                matchOrientation == MatchOrientation.TargetUpAndForward)
            {
                Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
                var projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, teleAnchor.transform.up);
                Assert.That(projectedCameraForward.normalized, Is.EqualTo(teleAnchor.transform.forward).Using(Vector3ComparerWithEqualsOperator.Instance));
            }
            else if (matchOrientation == MatchOrientation.WorldSpaceUp)
            {
                Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(Vector3.up).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin up vector");
                Assert.That(xrOrigin.Camera.transform.forward, Is.EqualTo(Vector3.forward).Using(Vector3ComparerWithEqualsOperator.Instance), "Projected forward");
            }
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndFilterByHitNormal()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // Create teleportation anchor with plane as child so the hit normal can be misaligned with the anchor's up
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

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleAnchor }));

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input, but teleport should not occur
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.False);

            // Wait a frame for any queued teleport request to be executed (which shouldn't happen)
            yield return null;

            Assert.That(cameraTrans.position, Is.EqualTo(originalCameraPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(originTrans.up, Is.EqualTo(originalOriginUp).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(cameraTrans.forward, Is.EqualTo(originalCameraForward).Using(Vector3ComparerWithEqualsOperator.Instance));

            // Now increase the normal tolerance and try teleporting again
            teleAnchor.upNormalToleranceDegrees = 50f;

            // Manually drive the select input so teleport is triggered
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.True);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.True);

            // Wait a frame for the queued teleport request to be executed
            yield return null;

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

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.ProjectileCurve;

            // create teleportation anchor
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;

            // Set teleportation anchor plane
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-90f, 0f, 0f, Space.World);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleAnchor }));

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input so teleport is triggered
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.True);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.True);

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithBezierCurve()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.BezierCurve;

            // Create teleportation anchor
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = MatchOrientation.TargetUp;

            // Set teleportation anchor plane
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-90f, 0f, 0f, Space.World);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleAnchor }));

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input so teleport is triggered
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.True);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.True);

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(teleAnchor.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorWithStraightLineAndDirectionalInput([ValueSource(nameof(s_DirectionalMatchOrientations))] MatchOrientation matchOrientation)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

            // Interactor
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform);
            interactor.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // Fake directional input by manually rotating the attach transform
            var attachTransform = interactor.attachTransform;
            attachTransform.Rotate(Vector3.up, 30f);

            // Create teleportation anchor
            var teleAnchor = TestUtilities.CreateTeleportAnchorPlane();
            teleAnchor.interactionManager = manager;
            teleAnchor.teleportationProvider = teleProvider;
            teleAnchor.matchOrientation = matchOrientation;
            teleAnchor.matchDirectionalInput = true;

            // Set teleportation anchor plane in the forward direction of controller
            teleAnchor.transform.position = interactor.transform.forward + Vector3.down;
            teleAnchor.transform.Rotate(-45f, 0f, 0f, Space.World);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { teleAnchor }));

            // Calculate expected forward direction AFTER rotating attach transform but BEFORE the controller performs the fake teleportation
            var planeNormal = matchOrientation == MatchOrientation.WorldSpaceUp ? Vector3.up : teleAnchor.transform.up;
            var expectedForward = Vector3.ProjectOnPlane(attachTransform.forward, planeNormal).normalized;

            var teleportingInvoked = false;
            teleAnchor.teleporting.AddListener(_ => teleportingInvoked = true);

            // Manually drive the select input so teleport is triggered
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;
            Assert.That(interactor.selectInput.ReadValue(), Is.EqualTo(1f));
            Assert.That(interactor.logicalSelectState.active, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);
            Assert.That(teleAnchor.IsSelected(interactor), Is.True);
            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;
            Assert.That(interactor.selectInput.ReadValue(), Is.EqualTo(0f));
            Assert.That(interactor.logicalSelectState.active, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
            Assert.That(teleAnchor.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, Is.True);

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            var cameraPosAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            Assert.That(xrOrigin.Camera.transform.position, Is.EqualTo(teleAnchor.transform.position + cameraPosAdjustment).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(planeNormal).Using(Vector3ComparerWithEqualsOperator.Instance), "XR Origin up vector");
            if (matchOrientation == MatchOrientation.WorldSpaceUp)
            {
                Assert.That(xrOrigin.Camera.transform.forward, Is.EqualTo(expectedForward).Using(Vector3ComparerWithEqualsOperator.Instance));
            }
            else if (matchOrientation == MatchOrientation.TargetUp)
            {
                var projectedCameraForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, planeNormal);
                Assert.That(projectedCameraForward.normalized, Is.EqualTo(expectedForward).Using(Vector3ComparerWithEqualsOperator.Instance));
            }
        }

        [UnityTest]
        public IEnumerator TeleportToAnchorManually()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config teleportation on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var teleProvider = xrOrigin.gameObject.AddComponent<TeleportationProvider>();
            teleProvider.mediator = mediator;

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
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            mediator.xrOrigin = xrOrigin;
            var snapProvider = xrOrigin.gameObject.AddComponent<SnapTurnProvider>();
            snapProvider.mediator = mediator;
            var turnAmount = snapProvider.turnAmount;

            snapProvider.rightHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.ManualValue;
            snapProvider.rightHandTurnInput.manualValue = Vector2.right;

            yield return null;

            Assert.That(xrOrigin.transform.rotation.eulerAngles, Is.EqualTo(new Vector3(0f, turnAmount, 0f)).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator SnapTurnAround()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();

            // Config snap turn on XR Origin
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            mediator.xrOrigin = xrOrigin;
            var snapProvider = xrOrigin.gameObject.AddComponent<SnapTurnProvider>();
            snapProvider.mediator = mediator;

            snapProvider.rightHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.ManualValue;
            snapProvider.rightHandTurnInput.manualValue = Vector2.down;

            yield return null;

            Assert.That(xrOrigin.transform.rotation.eulerAngles, Is.EqualTo(new Vector3(0f, 180f, 0f)).Using(Vector3ComparerWithEqualsOperator.Instance));
        }
    }
}
