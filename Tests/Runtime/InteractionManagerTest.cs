using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    public class InteractionManagerTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllInteractionObjects();
        }

        [Test]
        public void InteractorRegisteredOnEnable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            XRBaseInteractor registeredInteractor = null;
            manager.interactorRegistered += args => registeredInteractor = args.interactor;
            var interactor = TestUtilities.CreateDirectInteractor();

            var interactors = new List<XRBaseInteractor>();
            manager.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EquivalentTo(new[] { interactor }));
            Assert.That(manager.interactors, Is.EquivalentTo(new[] { interactor }));
            Assert.That(registeredInteractor, Is.SameAs(interactor));
        }

        [Test]
        public void InteractorUnregisteredOnDisable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            XRBaseInteractor unregisteredInteractor = null;
            manager.interactorUnregistered += args => unregisteredInteractor = args.interactor;
            var interactor = TestUtilities.CreateDirectInteractor();
            interactor.enabled = false;

            var interactors = new List<XRBaseInteractor>();
            manager.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.Empty);
            Assert.That(manager.interactors, Is.Empty);
            Assert.That(unregisteredInteractor, Is.SameAs(interactor));
        }

        [Test]
        public void InteractorRegistrationEventsInvoked()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateDirectInteractor();
            XRBaseInteractor registeredInteractor = null;
            XRBaseInteractor unregisteredInteractor = null;
            interactor.registered += args => registeredInteractor = args.interactor;
            interactor.unregistered += args => unregisteredInteractor = args.interactor;
            interactor.enabled = false;

            Assert.That(manager.interactors, Is.Empty);
            Assert.That(unregisteredInteractor, Is.SameAs(interactor));

            interactor.enabled = true;

            Assert.That(manager.interactors, Is.EquivalentTo(new[] { interactor }));
            Assert.That(registeredInteractor, Is.SameAs(interactor));
        }

        [Test]
        public void InteractableRegisteredOnEnable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            XRBaseInteractable registeredInteractable = null;
            manager.interactableRegistered += args => registeredInteractable = args.interactable;
            var interactable = TestUtilities.CreateGrabInteractable();

            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.EquivalentTo(new[] { interactable }));
            Assert.That(manager.interactables, Is.EquivalentTo(new[] { interactable }));
            Assert.That(registeredInteractable, Is.SameAs(interactable));
        }

        [Test]
        public void InteractableUnregisteredOnDisable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            XRBaseInteractable unregisteredInteractable = null;
            manager.interactableUnregistered += args => unregisteredInteractable = args.interactable;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.enabled = false;

            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.Empty);
            Assert.That(manager.interactables, Is.Empty);
            Assert.That(unregisteredInteractable, Is.SameAs(interactable));
        }

        [Test]
        public void InteractableRegistrationEventsInvoked()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            XRBaseInteractable registeredInteractable = null;
            XRBaseInteractable unregisteredInteractable = null;
            interactable.registered += args => registeredInteractable = args.interactable;
            interactable.unregistered += args => unregisteredInteractable = args.interactable;
            interactable.enabled = false;

            Assert.That(manager.interactables, Is.Empty);
            Assert.That(unregisteredInteractable, Is.SameAs(interactable));

            interactable.enabled = true;

            Assert.That(manager.interactables, Is.EquivalentTo(new[] { interactable }));
            Assert.That(registeredInteractable, Is.SameAs(interactable));
        }

        [Test]
        public void InteractableRegisteredOnEnableWithColliders()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(manager.interactables, Has.Count.EqualTo(1));
            Assert.That(manager.interactables[0], Is.EqualTo(interactable));
            Assert.That(interactable.colliders, Has.Count.EqualTo(1));
            Assert.That(manager.GetInteractableForCollider(interactable.colliders.First()), Is.EqualTo(interactable));
        }

        [UnityTest]
        public IEnumerator InteractorCanDestroy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateDirectInteractor();

            Object.Destroy(interactor);

            yield return null;

            Assert.That(manager.interactors, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InteractableCanDestroy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();

            Object.Destroy(interactable);

            yield return null;

            Assert.That(manager.interactables, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InteractionManagersInteractWithCorrectObjects()
        {
            var managerA = TestUtilities.CreateInteractionManager();
            var interactorA = TestUtilities.CreateDirectInteractor();
            interactorA.interactionManager = managerA;
            var interactableA = TestUtilities.CreateGrabInteractable();
            interactableA.interactionManager = managerA;

            var managerB = TestUtilities.CreateInteractionManager();
            var interactorB = TestUtilities.CreateDirectInteractor();
            interactorB.interactionManager = managerB;
            var interactableB = TestUtilities.CreateGrabInteractable();
            interactableB.interactionManager = managerB;

            yield return new WaitForSeconds(0.1f);

            var validTargets = new List<XRBaseInteractable>();
            managerA.GetValidTargets(interactorA, validTargets);
            Assert.That(validTargets, Has.Exactly(1).EqualTo(interactableA));
            managerB.GetValidTargets(interactorA, validTargets);
            Assert.That(validTargets, Is.Empty);

            var hoverTargetList = new List<XRBaseInteractable>();
            interactorA.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactableA));
            interactorB.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactableB));
        }

    }
}
