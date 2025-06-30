using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

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
        public IEnumerator LocomotionStateChanges()
        {
            var mediator = TestUtilities.CreateLocomotionMediatorWithXROrigin();

            var provider1 = TestUtilities.CreateMockLocomotionProvider(mediator);
            var provider2 = TestUtilities.CreateMockLocomotionProvider(mediator);
            var provider3 = TestUtilities.CreateMockLocomotionProvider(mediator);

            LocomotionState? provider1ChangedState = null;
            LocomotionState? provider2ChangedState = null;
            LocomotionState? provider3ChangedState = null;
            provider1.locomotionStateChanged += (_, state) => provider1ChangedState = state;
            provider2.locomotionStateChanged += (_, state) => provider2ChangedState = state;
            provider3.locomotionStateChanged += (_, state) => provider3ChangedState = state;

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
            Assert.That(provider1ChangedState, Is.EqualTo(null));
            Assert.That(provider2ChangedState, Is.EqualTo(null));
            Assert.That(provider3ChangedState, Is.EqualTo(null));

            var provider1StartedLocomotion = false;
            var provider2StartedLocomotion = false;
            var provider3StartedLocomotion = false;
            provider1.locomotionStarted += _ => provider1StartedLocomotion = true;
            provider2.locomotionStarted += _ => provider2StartedLocomotion = true;
            provider3.locomotionStarted += _ => provider3StartedLocomotion = true;

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
            Assert.That(provider1ChangedState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider2ChangedState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider3ChangedState, Is.EqualTo(LocomotionState.Preparing));

            provider1ChangedState = null;
            provider2ChangedState = null;
            provider3ChangedState = null;

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
            Assert.That(provider1ChangedState, Is.EqualTo(null));
            Assert.That(provider2ChangedState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider3ChangedState, Is.EqualTo(null));

            provider2ChangedState = null;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(provider1.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider2.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(provider3.locomotionState, Is.EqualTo(LocomotionState.Preparing));

            Assert.That(provider1ChangedState, Is.EqualTo(null));
            Assert.That(provider2ChangedState, Is.EqualTo(null));
            Assert.That(provider3ChangedState, Is.EqualTo(null));

            // Frame 3:
            // Provider 1: Moving    -> Ended
            // Provider 2: Moving    -> Ended
            // Provider 3: Preparing -> Ended

            var provider1EndedLocomotion = false;
            var provider2EndedLocomotion = false;
            var provider3EndedLocomotion = false;
            provider1.locomotionEnded += _ => provider1EndedLocomotion = true;
            provider2.locomotionEnded += _ => provider2EndedLocomotion = true;
            provider3.locomotionEnded += _ => provider3EndedLocomotion = true;

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
            Assert.That(provider1ChangedState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(provider2ChangedState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(provider3ChangedState, Is.EqualTo(LocomotionState.Ended));

            provider1ChangedState = null;
            provider2ChangedState = null;
            provider3ChangedState = null;

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
            Assert.That(provider1ChangedState, Is.EqualTo(LocomotionState.Preparing));
            Assert.That(provider2ChangedState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(provider3ChangedState, Is.EqualTo(LocomotionState.Idle));
        }

        [UnityTest]
        public IEnumerator LocomotionEndedEventDeferred()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            TestUtilities.CreateXRBodyTransformer(xrOrigin);
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            var provider = TestUtilities.CreateMockLocomotionProvider(mediator);

            LocomotionState? providerChangedState = null;
            provider.locomotionStateChanged += (_, state) => providerChangedState = state;

            var providerStartedLocomotion = false;
            provider.locomotionStarted += _ => providerStartedLocomotion = true;

            var providerEndedLocomotion = false;
            provider.locomotionEnded += _ => providerEndedLocomotion = true;

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Idle));

            yield return null;

            // Provider: Idle -> Moving

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(providerChangedState, Is.EqualTo(null));
            Assert.That(providerStartedLocomotion, Is.False);
            Assert.That(providerEndedLocomotion, Is.False);

            provider.InvokeTryStartLocomotionImmediately();

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(providerChangedState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(providerStartedLocomotion, Is.True);
            Assert.That(providerEndedLocomotion, Is.False);

            providerChangedState = null;

            // Provider: Moving -> Ended
            // No queued transformation, so locomotionEnded should fire immediately

            provider.InvokeTryEndLocomotion();

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(providerChangedState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(providerEndedLocomotion, Is.True);

            providerChangedState = null;
            providerStartedLocomotion = false;
            providerEndedLocomotion = false;

            yield return null;

            // Provider: Ended -> Moving

            provider.InvokeTryStartLocomotionImmediately();

            yield return null;

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(providerChangedState, Is.EqualTo(LocomotionState.Moving));
            Assert.That(providerStartedLocomotion, Is.True);
            Assert.That(providerEndedLocomotion, Is.False);

            // Provider: Moving -> Ended
            // Queue a transformation, and then end locomotion.
            // Should need to wait until the XRBodyTransformer has been updated to apply the transformation
            // for the locomotionEnded event to fire.

            var transformationApplied = false;
            provider.delegateTransformation.transformation += _ => transformationApplied = true;
            provider.InvokeTryQueueTransformation();
            provider.InvokeTryEndLocomotion();

            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(providerChangedState, Is.EqualTo(LocomotionState.Ended));
            Assert.That(providerEndedLocomotion, Is.False);
            Assert.That(transformationApplied, Is.False);

            yield return null;

            // Should now have applied the transformation and thus the locomotionEnded event should have fired.
            // Provider: Ended -> Idle

            Assert.That(transformationApplied, Is.True);
            Assert.That(provider.locomotionState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(providerChangedState, Is.EqualTo(LocomotionState.Idle));
            Assert.That(providerEndedLocomotion, Is.True);
            Assert.That(transformationApplied, Is.True);
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
            var afterStepLocomotionInvoked = false;
            var transformationApplied = false;
            provider.beforeStepLocomotion += _ => beforeStepLocomotionInvoked = true;
            provider.afterStepLocomotion += _ => afterStepLocomotionInvoked = true;
            provider.delegateTransformation.transformation += _ => transformationApplied = true;

            // Cannot queue if Idle
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            provider.InvokeTryPrepareLocomotion();

            // Cannot queue if Preparing
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.False);
            Assert.That(afterStepLocomotionInvoked, Is.False);
            Assert.That(transformationApplied, Is.False);

            provider.InvokeTryStartLocomotionImmediately();

            // Can queue if Moving
            Assert.That(provider.InvokeTryQueueTransformation(), Is.True);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.True);
            Assert.That(afterStepLocomotionInvoked, Is.True);
            Assert.That(transformationApplied, Is.True);

            beforeStepLocomotionInvoked = false;
            afterStepLocomotionInvoked = false;
            transformationApplied = false;
            provider.InvokeTryEndLocomotion();

            // Cannot queue if Ending
            Assert.That(provider.InvokeTryQueueTransformation(), Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(beforeStepLocomotionInvoked, Is.False);
            Assert.That(afterStepLocomotionInvoked, Is.False);
            Assert.That(transformationApplied, Is.False);
        }

        [UnityTest]
        public IEnumerator TeleportToAreaWithSphereCastOverlapIsBlocked([ValueSource(nameof(s_HitDetectionTypes))] XRRayInteractor.HitDetectionType hitDetectionType)
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
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            interactor.selectInput.QueueManualState(true, 1f);
            yield return null;

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

            interactor.selectInput.QueueManualState(false, 0f);
            yield return null;

            Assert.That(interactor.isSelectActive, Is.False);

            Assert.That(teleArea.IsSelected(interactor), Is.False);
            Assert.That(teleportingInvoked, hitDetectionType != XRRayInteractor.HitDetectionType.SphereCast ? Is.True : Is.False);
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
        public IEnumerator TeleportationMonitorDetectsTeleport()
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

            // Create interactor under the XR Origin
            var interactor = TestUtilities.CreateMockInteractor();
            interactor.transform.SetParent(xrOrigin.Origin.transform);

            var teleportedInvoked = false;
            var monitor = new TeleportationMonitor();
            monitor.teleported += (_, _, _) => teleportedInvoked = true;
            monitor.AddInteractor(interactor);

            // Manually trigger teleport to the anchor
            teleAnchor.RequestTeleport();

            // Wait a frame for the queued teleport request to be executed
            yield return null;

            Assert.That(teleportingInvoked, Is.True);
            Assert.That(teleportedInvoked, Is.True);
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

        [UnityTest]
        public IEnumerator Jump()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            mediator.xrOrigin = xrOrigin;

            var gravityProvider = xrOrigin.gameObject.AddComponent<GravityProvider>();
            gravityProvider.mediator = mediator;

            var jumpProvider = xrOrigin.gameObject.AddComponent<JumpProvider>();
            jumpProvider.mediator = mediator;
            jumpProvider.unlimitedInAirJumps = true;

            var initialHeight = xrOrigin.Origin.transform.position.y;

            jumpProvider.Jump();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(xrOrigin.Origin.transform.position.y, Is.GreaterThan(initialHeight));
        }

        [UnityTest]
        public IEnumerator Gravity()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();

            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            mediator.xrOrigin = xrOrigin;

            var gravityProvider = xrOrigin.gameObject.AddComponent<GravityProvider>();
            gravityProvider.mediator = mediator;

            var initialHeight = xrOrigin.Origin.transform.position.y;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(xrOrigin.Origin.transform.position.y, Is.LessThan(initialHeight));
        }
    }
}
