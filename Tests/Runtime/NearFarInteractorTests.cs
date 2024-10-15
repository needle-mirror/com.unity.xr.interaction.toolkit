using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class NearFarInteractorTests
    {
        static readonly bool[] s_BooleanValues = { false, true };

        static readonly int[] s_NumSegmentValues = { 1, 2, 5, 25 };

        static readonly CurveInteractionCaster.HitDetectionType[] s_HitDetectionTypes =
        {
            CurveInteractionCaster.HitDetectionType.Raycast,
            CurveInteractionCaster.HitDetectionType.ConeCast,
            CurveInteractionCaster.HitDetectionType.SphereCast
        };

        static readonly (InteractorFarAttachMode interactorFarAttachMode, InteractableFarAttachMode interactableFarAttachMode)[] s_FarAttachModeArgs =
        {
            (InteractorFarAttachMode.Near, InteractableFarAttachMode.DeferToInteractor),
            (InteractorFarAttachMode.Near, InteractableFarAttachMode.Near),
            (InteractorFarAttachMode.Near, InteractableFarAttachMode.Far),
            (InteractorFarAttachMode.Far, InteractableFarAttachMode.DeferToInteractor),
            (InteractorFarAttachMode.Far, InteractableFarAttachMode.Near),
            (InteractorFarAttachMode.Far, InteractableFarAttachMode.Far),
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator TestEnableNearCasting([ValueSource(nameof(s_BooleanValues))] bool enableNearCasting)
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.enableNearCasting = enableNearCasting;
            interactor.enableFarCasting = false;

            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.enableNearCasting, Is.EqualTo(enableNearCasting));

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });

            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.EqualTo(enableNearCasting));
            Assert.That(interactor.interactablesSelected, enableNearCasting ? Is.EqualTo(new[] { interactable }) : Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestEnableFarCasting([ValueSource(nameof(s_BooleanValues))] bool enableFarCasting)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = false;
            interactor.enableFarCasting = enableFarCasting;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, enableFarCasting ? Is.EqualTo(new[] { interactable }) : Is.Empty);

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.EqualTo(enableFarCasting));
            Assert.That(interactor.interactablesSelected, enableFarCasting ? Is.EqualTo(new[] { interactable }) : Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestObstructedFarCasting()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = false;
            interactor.enableFarCasting = true;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            // Create cube to obstruct far interaction
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = interactor.transform.position + interactor.transform.forward * 3.0f;
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            validTargets.Clear();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestFarCastingTargetFilterWithMultiColliderInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = false;
            interactor.enableFarCasting = true;

            // Setup target filter
            var targetFilter = interactor.gameObject.AddComponent<XRTargetFilter>();
            interactor.targetFilter = targetFilter;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            // Setup interactable with multiple colliders
            GameObject interactableGO = new GameObject("Grab Interactable");
            TestUtilities.CreateGOSphereCollider(interactableGO, false);
            TestUtilities.CreateGOBoxCollider(interactableGO, false);
            XRGrabInteractable interactable = interactableGO.AddComponent<XRGrabInteractable>();
            var rigidBody = interactableGO.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Setup interactable with multiple colliders
            GameObject secondaryGameObject = new GameObject("Secondary Grab Interactable");
            TestUtilities.CreateGOSphereCollider(secondaryGameObject, false);
            TestUtilities.CreateGOBoxCollider(secondaryGameObject, false);
            XRGrabInteractable secondaryInteractable = secondaryGameObject.AddComponent<XRGrabInteractable>();
            rigidBody = secondaryGameObject.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            secondaryInteractable.transform.position = interactor.transform.position + interactor.transform.forward * 6.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }


        [UnityTest]
        public IEnumerator TestEnableUIInteraction([ValueSource(nameof(s_BooleanValues))] bool enableUIInteraction)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractorWithXROrigin(out var camera);

            interactor.enableUIInteraction = enableUIInteraction;
            interactor.enableNearCasting = false;
            interactor.enableFarCasting = true;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Place UI closer than interactable
            var uiCanvas = TestUtilities.CreateUICanvas(camera);
            uiCanvas.transform.position = interactor.transform.position + interactor.transform.forward * 2.5f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, enableUIInteraction ? Is.Empty : Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            yield return new WaitForSeconds(0.1f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var endPointType = interactor.TryGetCurveEndPoint(out var endPoint);

            Assert.That(endPointType, Is.EqualTo(enableUIInteraction ? EndPointType.UI : EndPointType.ValidCastHit));
        }

        [UnityTest]
        public IEnumerator TestBlockUIOnInteractableSelection([ValueSource(nameof(s_BooleanValues))] bool blockUIOnInteractableSelection)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractorWithXROrigin(out var camera);

            interactor.enableNearCasting = false;
            interactor.enableFarCasting = true;
            interactor.enableUIInteraction = true;
            interactor.blockUIOnInteractableSelection = blockUIOnInteractableSelection;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            // Place interactable closer than UI
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 2.5f;

            var uiCanvas = TestUtilities.CreateUICanvas(camera);
            uiCanvas.transform.position = interactor.transform.position + interactor.transform.forward * 5f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            // Move interactable further out than UI
            interactor.interactionAttachController.MoveTo(interactor.transform.position + interactor.transform.forward * 10f);

            yield return new WaitForSeconds(0.1f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            var endPointType = interactor.TryGetCurveEndPoint(out var endPoint);

            Assert.That(endPointType, Is.EqualTo(blockUIOnInteractableSelection ? EndPointType.ValidCastHit : EndPointType.UI));
        }

        [UnityTest]
        public IEnumerator TestNearCasterSortingStrategy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.nearCasterSortingStrategy = NearFarInteractor.NearCasterSortingStrategy.SquareDistance;

            interactor.enableNearCasting = true;
            interactor.enableFarCasting = true;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = new Vector3(0f, 0.05f, 0.0f);

            // Place second interactable above first, within range of near interaction
            var interactable2 = TestUtilities.CreateGrabInteractable();
            interactable2.transform.position = new Vector3(0f, 0.055f, 0.0f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator TestFarInteractionAttachMode([ValueSource(nameof(s_FarAttachModeArgs))] (InteractorFarAttachMode interactorFarAttachMode, InteractableFarAttachMode interactableFarAttachMode) args)
        {
            var effectiveNear = args.interactableFarAttachMode == InteractableFarAttachMode.DeferToInteractor && args.interactorFarAttachMode == InteractorFarAttachMode.Near ||
                args.interactableFarAttachMode == InteractableFarAttachMode.Near;

            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = true;
            interactor.enableFarCasting = true;
            interactor.farAttachMode = args.interactorFarAttachMode;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var interactable = TestUtilities.CreateGrabInteractable();

            interactable.farAttachMode = args.interactableFarAttachMode;
            interactable.useDynamicAttach = !effectiveNear;
            interactable.snapToColliderVolume = false;
            interactable.reinitializeDynamicAttachEverySingleGrab = true;

            var initialInteractablePosition = interactor.transform.position + interactor.transform.forward * 5.0f;
            interactable.transform.position = initialInteractablePosition;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            if (effectiveNear)
                Assert.That(interactable.transform.position, Is.EqualTo(interactor.transform.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            else
                Assert.That(interactable.transform.position, Is.EqualTo(initialInteractablePosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator TestDetermineSelectionRegion()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = false;
            interactor.enableFarCasting = true;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            NearFarInteractor.Region? invokedSelectionRegion = null;
            interactor.selectionRegion.Subscribe(OnNearFarSelectionRegionChanged);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.selectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.Far));
            Assert.That(invokedSelectionRegion.HasValue, Is.True);
            Assert.That(invokedSelectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.Far));

            invokedSelectionRegion = null;
            interactor.interactionAttachController.MoveTo(interactor.transform.position);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.selectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.Near));
            Assert.That(invokedSelectionRegion.HasValue, Is.True);
            Assert.That(invokedSelectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.Near));

            invokedSelectionRegion = null;

            manager.SelectExit((IXRSelectInteractor)interactor, interactable);

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.selectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.None));
            Assert.That(invokedSelectionRegion.HasValue, Is.True);
            Assert.That(invokedSelectionRegion.Value, Is.EqualTo(NearFarInteractor.Region.None));

            void OnNearFarSelectionRegionChanged(NearFarInteractor.Region selectionRegion)
            {
                invokedSelectionRegion = selectionRegion;
            }
        }

        [UnityTest]
        public IEnumerator TestEvaluateNearInteractionPriority()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();

            interactor.enableNearCasting = true;
            interactor.enableFarCasting = true;

            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = Vector3.zero;

            var interactable2 = TestUtilities.CreateGrabInteractable();
            interactable2.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator TestNearFarInteractorManualSelection()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.right * 5.0f;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);

            interactor.StartManualInteraction((IXRSelectInteractable)interactable);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            interactor.EndManualInteraction();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestNearFarInteractorCanSelectInteractorWithSnapVolume()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            var snapVolume = TestUtilities.CreateSnapVolume();
            snapVolume.interactable = interactable;
            snapVolume.snapToCollider = interactable.colliders[0];
            snapVolume.transform.position = interactable.transform.position;

            yield return new WaitForFixedUpdate();
            yield return null;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;
            controllerRecorder.visitEachFrame = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator TestNearFarInteractorCurveInteractionCaster([ValueSource(nameof(s_HitDetectionTypes))] CurveInteractionCaster.HitDetectionType hitDetectionType, [ValueSource(nameof(s_NumSegmentValues))] int numSegments)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var curveInteractionCaster = interactor.farInteractionCaster as CurveInteractionCaster;

            Assert.That(curveInteractionCaster, Is.Not.Null);

            curveInteractionCaster.hitDetectionType = hitDetectionType;
            curveInteractionCaster.targetNumCurveSegments = numSegments;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            yield return new WaitForSeconds(0.1f);

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            interactor.StartManualInteraction((IXRSelectInteractable)interactable);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            interactor.EndManualInteraction();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestNearFarInteractorCurveInteractionCasterBlocked([ValueSource(nameof(s_HitDetectionTypes))] CurveInteractionCaster.HitDetectionType hitDetectionType, [ValueSource(nameof(s_NumSegmentValues))] int numSegments)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            var curveInteractionCaster = interactor.farInteractionCaster as CurveInteractionCaster;

            Assert.That(curveInteractionCaster, Is.Not.Null);

            curveInteractionCaster.hitDetectionType = hitDetectionType;
            curveInteractionCaster.targetNumCurveSegments = numSegments;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5f;

            yield return new WaitForSeconds(0.1f);

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            var blocker = TestUtilities.CreateCubeGameObject();
            blocker.transform.position = interactor.transform.position + interactor.transform.forward * 2f;

            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
        }

        [UnityTest]
        public IEnumerator TestNearFarInteractorCurveInteractionCasterBlockedUI([ValueSource(nameof(s_HitDetectionTypes))] CurveInteractionCaster.HitDetectionType hitDetectionType, [ValueSource(nameof(s_NumSegmentValues))] int numSegments)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateNearFarInteractorWithXROrigin(out var camera);
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            interactor.blockUIOnInteractableSelection = false;

            var curveInteractionCaster = interactor.farInteractionCaster as CurveInteractionCaster;

            Assert.That(curveInteractionCaster, Is.Not.Null);

            curveInteractionCaster.hitDetectionType = hitDetectionType;
            curveInteractionCaster.targetNumCurveSegments = numSegments;

            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 2f;

            var blocker = TestUtilities.CreateUICanvas(camera);
            blocker.transform.position = interactor.transform.position + interactor.transform.forward * 5f;

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            yield return new WaitForFixedUpdate();
            yield return null;

            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 8f;

            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
        }

    }
}
