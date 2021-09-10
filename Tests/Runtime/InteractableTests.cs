using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InteractableTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator InteractableIsHoveredWhileAnyInteractorHovering()
        {
            TestUtilities.CreateInteractionManager();
            var interactor1 = TestUtilities.CreateMockInteractor();
            var interactor2 = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactable.hoveringInteractors, Is.Empty);

            interactor1.validTargets.Add(interactable);
            interactor2.validTargets.Add(interactable);

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.hoveringInteractors, Is.EquivalentTo(new[] { interactor1, interactor2 }));

            interactor2.validTargets.Clear();

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.hoveringInteractors, Is.EquivalentTo(new[] { interactor1 }));

            interactor1.validTargets.Clear();

            yield return null;

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactable.hoveringInteractors, Is.Empty);
        }
    }
}
