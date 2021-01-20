using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    public class SocketInteractorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllInteractionObjects();
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanSelectInteractable()
        {
            TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForSeconds(0.1f);

            Assert.That(socketInteractor.selectTarget, Is.EqualTo(interactable));
        }

        [UnityTest]
        public IEnumerator SocketInteractorHandlesUnregisteredInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var selectedInteractable = TestUtilities.CreateGrabInteractable();
            var hoveredInteractable = TestUtilities.CreateGrabInteractable();
            hoveredInteractable.transform.localPosition = new Vector3(0.001f, 0f, 0f);

            yield return new WaitForSeconds(0.1f);

            Assert.That(socketInteractor.selectTarget, Is.EqualTo(selectedInteractable));

            var validTargets = new List<XRBaseInteractable>();
            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Has.Exactly(1).EqualTo(hoveredInteractable));

            var hoverTargetList = new List<XRBaseInteractable>();
            socketInteractor.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(hoveredInteractable));

            Object.Destroy(hoveredInteractable);

            yield return null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- Object operator==
            Assert.That(hoveredInteractable == null, Is.True);

            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Is.Empty);

            socketInteractor.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Is.Empty);

            Object.Destroy(selectedInteractable);

            yield return null;
            Assert.That(selectedInteractable == null, Is.True);
            Assert.That(socketInteractor.selectTarget == null, Is.True);
        }

        [UnityTest]
        public IEnumerator SocketInteractorCanDirectInteractorStealFrom()
        {
            TestUtilities.CreateInteractionManager();
            var socketInteractor = TestUtilities.CreateSocketInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            var directInteractor = TestUtilities.CreateDirectInteractor();
            var controller = directInteractor.GetComponent<XRController>();
            var controllerRecorder = TestUtilities.CreateControllerRecorder(controller, (recording) =>
            {
                recording.AddRecordingFrame(0.0f, Vector3.zero, Quaternion.identity,
                    true, false, false);
                recording.AddRecordingFrame(float.MaxValue, Vector3.zero, Quaternion.identity,
                    true, false, false);
            });
            controllerRecorder.isPlaying = true;

            yield return new WaitForSeconds(0.1f);

            Assert.That(socketInteractor.selectTarget, Is.EqualTo(null));
            Assert.That(directInteractor.selectTarget, Is.EqualTo(interactable));
        }
    }
}
