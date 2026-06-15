using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class SocketInteractorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanSelectInteractable()
        {
            TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(socketInteractor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(socketInteractor.hasSelection, Is.True);
        }

        [UnityTest]
        public IEnumerator SocketInteractorStartingSelectedInteractableMovesToInteractorPosition()
        {
            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = Vector3.one;
            TestUtilities.DisableDelayProperties(interactable);

            var socketInteractor = TestUtilities.CreateSocketInteractor();
            socketInteractor.startingSelectedInteractable = interactable;

            Assert.That(socketInteractor.interactablesSelected, Is.Empty);
            Assert.That(socketInteractor.hasSelection, Is.False);
            Assert.That(interactable.transform.position, Is.Not.EqualTo(socketInteractor.transform.position).Using(Vector3ComparerWithEqualsOperator.Instance));

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(socketInteractor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(socketInteractor.hasSelection, Is.True);
            Assert.That(interactable.transform.position, Is.EqualTo(socketInteractor.transform.position).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator SocketInteractorHandlesUnregisteredInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var selectedInteractable = TestUtilities.CreateGrabInteractable();
            var hoveredInteractable = TestUtilities.CreateGrabInteractable();
            // Move to a position so it won't be the closest to ensure selectedInteractable is the one selected
            hoveredInteractable.transform.localPosition = new Vector3(0.001f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(socketInteractor.interactablesSelected, Is.EqualTo(new[] { selectedInteractable }));

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));
            socketInteractor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));

            Assert.That(socketInteractor.interactablesHovered, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));

            Object.Destroy(hoveredInteractable);

            yield return null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- Object operator==
            Assert.That(hoveredInteractable == null, Is.True);

            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable }));
            socketInteractor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable }));

            Assert.That(socketInteractor.interactablesHovered, Is.EquivalentTo(new[] { selectedInteractable }));

            Object.Destroy(selectedInteractable);

            yield return null;
            Assert.That(selectedInteractable == null, Is.True);
            Assert.That(socketInteractor.interactablesSelected, Is.Empty);
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanDirectInteractorStealSingleModeInteractableFromSocket()
        {
            TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            Assert.That(interactable.selectMode, Is.EqualTo(InteractableSelectMode.Single));

            var directInteractor = TestUtilities.CreateDirectInteractor();
            var controllerRecorder = TestUtilities.CreateControllerRecorder(directInteractor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;

            // Only need to wait one frame since the single select mode should cause the socket selection to first exit
            // before being selected by the direct interactor.
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(socketInteractor.interactablesSelected, Is.Empty);
            Assert.That(directInteractor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanDirectInteractorStealMultipleModeInteractableFromSocket()
        {
            TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.selectMode = InteractableSelectMode.Multiple;

            var directInteractor = TestUtilities.CreateDirectInteractor();
            var controllerRecorder = TestUtilities.CreateControllerRecorder(directInteractor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;

            // May need to wait two frames.
            // One for XRInteractionManager to enter the selection of the socket and direct interactors.
            // Two for XRInteractionManager.ClearInteractorSelection to exit the selection of the socket
            // since the CanSelect should now want to clear the selection due to no longer being the exclusive
            // interactor selecting.
            yield return new WaitForFixedUpdate();
            yield return null;
            yield return null;

            Assert.That(socketInteractor.interactablesSelected, Is.Empty);
            Assert.That(directInteractor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator SocketInteractorReportsValidTargetWhenInteractableRegisteredAfterContact()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor }));

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { interactable }));

            // Disable the Interactable so it will be removed as a valid target
            interactable.enabled = false;

            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactable.interactorsSelecting, Is.Empty);

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            // Re-enable the Interactable. It should not be required to leave and enter the collider to be selected again.
            interactable.enabled = true;

            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanUseTargetFilter()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForFixedUpdate();
            yield return null;

            var filter = new MockTargetFilter();
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>()));

            // Link the filter
            interactor.targetFilter = filter;
            Assert.That(interactor.targetFilter, Is.EqualTo(filter));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link
            }));

            // Process the filter
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.targetFilter, Is.EqualTo(filter));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process
            }));

            // Disable the filter and check if it will no longer be processed
            filter.canProcess = false;
            interactor.GetValidTargets(validTargets);
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process
            }));

            // Unlink the filter
            interactor.targetFilter = null;
            Assert.That(interactor.targetFilter, Is.EqualTo(null));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
                TargetFilterCallback.Unlink
            }));
        }
    }
}
