using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

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

            Assert.That(socketInteractor.selectTarget, Is.EqualTo(interactable));
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

            Assert.That(socketInteractor.selectTarget, Is.EqualTo(selectedInteractable));

            var validTargets = new List<XRBaseInteractable>();
            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));
            socketInteractor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));

            var hoverTargets = new List<XRBaseInteractable>();
            socketInteractor.GetHoverTargets(hoverTargets);
            Assert.That(hoverTargets, Is.EquivalentTo(new[] { selectedInteractable, hoveredInteractable }));

            Object.Destroy(hoveredInteractable);

            yield return null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- Object operator==
            Assert.That(hoveredInteractable == null, Is.True);

            manager.GetValidTargets(socketInteractor, validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable }));
            socketInteractor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EquivalentTo(new[] { selectedInteractable }));

            socketInteractor.GetHoverTargets(hoverTargets);
            Assert.That(hoverTargets, Is.EquivalentTo(new[] { selectedInteractable }));

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
