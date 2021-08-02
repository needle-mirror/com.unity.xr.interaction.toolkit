using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InteractionManagerTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
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
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(registeredInteractor, Is.SameAs(interactor));
            Assert.That(manager.IsRegistered(interactor), Is.True);
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
            Assert.That(unregisteredInteractor, Is.SameAs(interactor));
            Assert.That(manager.IsRegistered(interactor), Is.False);
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

            var interactors = new List<XRBaseInteractor>();
            manager.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.Empty);
            Assert.That(unregisteredInteractor, Is.SameAs(interactor));

            interactor.enabled = true;

            manager.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
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
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));
            Assert.That(registeredInteractable, Is.SameAs(interactable));
            Assert.That(manager.IsRegistered(interactable), Is.True);
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
            Assert.That(unregisteredInteractable, Is.SameAs(interactable));
            Assert.That(manager.IsRegistered(interactable), Is.False);
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

            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.Empty);
            Assert.That(unregisteredInteractable, Is.SameAs(interactable));

            interactable.enabled = true;

            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));
            Assert.That(registeredInteractable, Is.SameAs(interactable));
        }

        [Test]
        public void InteractableRegisteredOnEnableWithColliders()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();

            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));
            Assert.That(interactable.colliders, Has.Count.EqualTo(1));
            Assert.That(manager.GetInteractableForCollider(interactable.colliders.First()), Is.EqualTo(interactable));
        }

        [Test]
        public void InteractableUnregistersAssociatedColliders()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var firstInteractable = TestUtilities.CreateGrabInteractable();
            var secondInteractable = TestUtilities.CreateGrabInteractable();

            manager.UnregisterInteractable(firstInteractable);
            manager.UnregisterInteractable(secondInteractable);

            // Setup so the first Interactable has both colliders, and the second Interactable has a conflicting reference
            secondInteractable.transform.SetParent(firstInteractable.transform);
            firstInteractable.colliders.Clear();
            firstInteractable.colliders.AddRange(firstInteractable.GetComponentsInChildren<Collider>());
            secondInteractable.colliders.Clear();
            secondInteractable.colliders.AddRange(secondInteractable.GetComponentsInChildren<Collider>());

            Assert.That(firstInteractable.colliders, Has.Count.EqualTo(2));
            Assert.That(secondInteractable.colliders, Has.Count.EqualTo(1));

            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[0]), Is.Null);
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[1]), Is.Null);

            manager.RegisterInteractable(firstInteractable);

            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[0]), Is.SameAs(firstInteractable));
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[1]), Is.SameAs(firstInteractable));

            LogAssert.Expect(LogType.Warning, new Regex("A Collider used by an Interactable object is already registered with another Interactable object*"));
            manager.RegisterInteractable(secondInteractable);

            // Interactables registered afterward do not replace the existing Collider association
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[0]), Is.SameAs(firstInteractable));
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[1]), Is.SameAs(firstInteractable));

            manager.UnregisterInteractable(secondInteractable);

            // Interactables registered afterward should not cause the registered Collider association to be removed
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[0]), Is.SameAs(firstInteractable));
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[1]), Is.SameAs(firstInteractable));

            manager.UnregisterInteractable(firstInteractable);

            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[0]), Is.Null);
            Assert.That(manager.GetInteractableForCollider(firstInteractable.colliders[1]), Is.Null);
        }

        // Tests that Interactors and Interactables can register or unregister
        // while the Interaction Manager is iterating over the list of Interactors to process events in Update.
        [UnityTest]
        public IEnumerator ObjectsCanRegisterDuringEvents()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5f;

            var otherInteractor = TestUtilities.CreateDirectInteractor();
            var otherInteractable = TestUtilities.CreateSimpleInteractable();
            otherInteractor.enabled = false;
            otherInteractable.enabled = false;
            // Don't let them get in the way, both are only used to test registration
            otherInteractor.interactionLayerMask = 0;
            otherInteractable.interactionLayerMask = 0;

            // Upon Select, enable the other Interactor to have it register with the Interaction Manager during the update loop.
            // Upon Deselect, disable the other Interactor to have it unregister from the Interaction Manager during the update loop.
            interactor.selectEntered.AddListener(args =>
            {
                otherInteractor.enabled = true;
                otherInteractable.enabled = true;
            });
            interactor.selectExited.AddListener(args =>
            {
                otherInteractor.enabled = false;
                otherInteractable.enabled = false;
            });

            // Prepare controller state which will be used to cause a Select during the Interaction Manager update loop
            var controller = interactor.GetComponent<XRBaseController>();
            var controllerState = new XRControllerState(0f, Vector3.zero, Quaternion.identity, false, false, false);
            controller.SetControllerState(controllerState);

            var interactors = new List<XRBaseInteractor>();
            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));

            // Wait for Physics update to ensure the Interactable can be detected by the Interactor
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);

            // Press Grip
            controllerState.selectInteractionState = new InteractionState { active = true, activatedThisFrame = true };

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.True);

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new XRBaseInteractor[] { interactor, otherInteractor }));
            Assert.That(interactables, Is.EqualTo(new XRBaseInteractable[] { interactable, otherInteractable }));

            // Release Grip
            controllerState.selectInteractionState = new InteractionState { active = false, deactivatedThisFrame = true };

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));
        }

        // Tests that Interactors and Interactables can register or unregister
        // while the Interaction Manager is iterating over the list of Interactors to process in ProcessInteractors.
        [UnityTest]
        public IEnumerator ObjectsCanRegisterDuringProcessInteractors()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = EnablerInteractor.CreateInteractor();

            var otherInteractor = TestUtilities.CreateDirectInteractor();
            var otherInteractable = TestUtilities.CreateSimpleInteractable();
            otherInteractor.enabled = false;
            otherInteractable.enabled = false;
            // Don't let them get in the way, both are only used to test registration
            otherInteractor.interactionLayerMask = 0;
            otherInteractable.interactionLayerMask = 0;

            interactor.interactor = otherInteractor;
            interactor.interactable = otherInteractable;

            yield return null;

            var interactors = new List<XRBaseInteractor>();
            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactables, Is.Empty);

            interactor.enableBehaviors = true;

            yield return null;

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new XRBaseInteractor[] { interactor, otherInteractor }));
            Assert.That(interactables, Is.EqualTo(new XRBaseInteractable[] { otherInteractable }));

            interactor.enableBehaviors = false;

            yield return null;

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactables, Is.Empty);
        }

        // Tests that Interactors and Interactables can register or unregister
        // while the Interaction Manager is iterating over the list of Interactables to process in ProcessInteractables.
        [UnityTest]
        public IEnumerator ObjectsCanRegisterDuringProcessInteractables()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = EnablerInteractable.CreateInteractable();

            var otherInteractor = TestUtilities.CreateDirectInteractor();
            var otherInteractable = TestUtilities.CreateSimpleInteractable();
            otherInteractor.enabled = false;
            otherInteractable.enabled = false;
            // Don't let them get in the way, both are only used to test registration
            otherInteractor.interactionLayerMask = 0;
            otherInteractable.interactionLayerMask = 0;

            interactable.interactor = otherInteractor;
            interactable.interactable = otherInteractable;

            yield return null;

            var interactors = new List<XRBaseInteractor>();
            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.Empty);
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));

            interactable.enableBehaviors = true;

            yield return null;

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.EqualTo(new XRBaseInteractor[] { otherInteractor }));
            Assert.That(interactables, Is.EqualTo(new XRBaseInteractable[] { interactable, otherInteractable }));

            interactable.enableBehaviors = false;

            yield return null;

            manager.GetRegisteredInteractors(interactors);
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactors, Is.Empty);
            Assert.That(interactables, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator InteractorRegistrationEventsOccurSameFrame()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateDirectInteractor();
            interactor.enabled = false;

            var numRegistered = 0;
            var numUnregistered = 0;

            interactor.registered += args =>
            {
                ++numRegistered;
            };
            interactor.unregistered += args =>
            {
                ++numUnregistered;
            };

            interactor.enabled = true;

            Assert.That(numRegistered, Is.EqualTo(1));
            Assert.That(numUnregistered, Is.EqualTo(0));

            interactor.enabled = false;

            Assert.That(numRegistered, Is.EqualTo(1));
            Assert.That(numUnregistered, Is.EqualTo(1));

            interactor.enabled = true;

            Assert.That(numRegistered, Is.EqualTo(2));
            Assert.That(numUnregistered, Is.EqualTo(1));

            yield return null;

            Assert.That(numRegistered, Is.EqualTo(2));
            Assert.That(numUnregistered, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator InteractableRegistrationEventsOccurSameFrame()
        {
            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.enabled = false;

            var numRegistered = 0;
            var numUnregistered = 0;

            interactable.registered += args =>
            {
                ++numRegistered;
            };
            interactable.unregistered += args =>
            {
                ++numUnregistered;
            };

            interactable.enabled = true;

            Assert.That(numRegistered, Is.EqualTo(1));
            Assert.That(numUnregistered, Is.EqualTo(0));

            interactable.enabled = false;

            Assert.That(numRegistered, Is.EqualTo(1));
            Assert.That(numUnregistered, Is.EqualTo(1));

            interactable.enabled = true;

            Assert.That(numRegistered, Is.EqualTo(2));
            Assert.That(numUnregistered, Is.EqualTo(1));

            yield return null;

            Assert.That(numRegistered, Is.EqualTo(2));
            Assert.That(numUnregistered, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator InteractorCanDestroy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateDirectInteractor();

            Object.Destroy(interactor);

            yield return null;

            var interactors = new List<XRBaseInteractor>();
            manager.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.Empty);
            Assert.That(manager.IsRegistered(interactor), Is.False);
        }

        [UnityTest]
        public IEnumerator InteractableCanDestroy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();

            Object.Destroy(interactable);

            yield return null;

            var interactables = new List<XRBaseInteractable>();
            manager.GetRegisteredInteractables(interactables);
            Assert.That(interactables, Is.Empty);
            Assert.That(manager.IsRegistered(interactable), Is.False);
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

        [Test]
        public void RegistrationListRegisterReturnsStatusChange()
        {
            var registrationList = new XRInteractionManager.RegistrationList<string>();
            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            Assert.That(registrationList.Register("A"), Is.False);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);

            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            registrationList.Flush();

            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);

            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            registrationList.Flush();

            Assert.That(registrationList.Register("A"), Is.False);
            Assert.That(registrationList.IsRegistered("A"), Is.True);
        }

        [Test]
        public void RegistrationListUnregisterReturnsStatusChange()
        {
            var registrationList = new XRInteractionManager.RegistrationList<string>();
            Assert.That(registrationList.Unregister("A"), Is.False);
            Assert.That(registrationList.IsRegistered("A"), Is.False);

            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.False);
            Assert.That(registrationList.Unregister("A"), Is.False);

            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.True);
            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);
            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);
            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.False);
        }

        [Test]
        public void RegistrationListSnapshotUnaffectedUntilFlush()
        {
            var registrationList = new XRInteractionManager.RegistrationList<string>();
            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);
            Assert.That(registrationList.registeredSnapshot, Is.Empty);

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.True);
            Assert.That(registrationList.registeredSnapshot, Is.EqualTo(new[] { "A" }));

            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);
            Assert.That(registrationList.Register("B"), Is.True);
            Assert.That(registrationList.IsRegistered("B"), Is.True);
            Assert.That(registrationList.registeredSnapshot, Is.EqualTo(new[] { "A" }));

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.False);
            Assert.That(registrationList.IsRegistered("B"), Is.True);
            Assert.That(registrationList.registeredSnapshot, Is.EqualTo(new[] { "B" }));
        }

        [Test]
        public void RegistrationListGetRegisteredItemsIncludesAll()
        {
            var registrationList = new XRInteractionManager.RegistrationList<string>();
            var registeredItems = new List<string>();
            registrationList.GetRegisteredItems(registeredItems);
            Assert.That(registeredItems, Is.Empty);

            // Should include pending adds
            Assert.That(registrationList.Register("A"), Is.True);
            registrationList.GetRegisteredItems(registeredItems);
            Assert.That(registeredItems, Is.EqualTo(new[] { "A" }));

            registrationList.Flush();

            // Should still be equal after flush
            registrationList.GetRegisteredItems(registeredItems);
            Assert.That(registeredItems, Is.EqualTo(new[] { "A" }));

            // Should include all in the order they were registered
            Assert.That(registrationList.Register("B"), Is.True);
            registrationList.GetRegisteredItems(registeredItems);
            Assert.That(registeredItems, Is.EqualTo(new[] { "A", "B" }));

            // Should filter out pending removes from the snapshot
            Assert.That(registrationList.Unregister("A"), Is.True);
            registrationList.GetRegisteredItems(registeredItems);
            Assert.That(registeredItems, Is.EqualTo(new[] { "B" }));
        }

        [Test]
        public void RegistrationListFastPathMatches()
        {
            var registrationList = new XRInteractionManager.RegistrationList<string>();
            Assert.That(registrationList.Register("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.True);

            registrationList.Flush();

            Assert.That(registrationList.IsRegistered("A"), Is.True);
            Assert.That(registrationList.IsStillRegistered("A"), Is.True);

            Assert.That(registrationList.Unregister("A"), Is.True);
            Assert.That(registrationList.IsRegistered("A"), Is.False);
            Assert.That(registrationList.IsStillRegistered("A"), Is.False);
        }

        /// <summary>
        /// Interactor that enables another Interactor and Interactable during <see cref="ProcessInteractor"/>.
        /// </summary>
        class EnablerInteractor : XRBaseInteractor
        {
            public XRBaseInteractor interactor { get; set; }

            public XRBaseInteractable interactable { get; set; }

            public bool enableBehaviors { get; set; }

            /// <inheritdoc />
            public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
            {
                base.ProcessInteractor(updatePhase);

                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    if (interactor != null)
                        interactor.enabled = enableBehaviors;

                    if (interactable != null)
                        interactable.enabled = enableBehaviors;
                }
            }

            /// <inheritdoc />
            public override void GetValidTargets(List<XRBaseInteractable> targets)
            {
                targets.Clear();
            }

            public static EnablerInteractor CreateInteractor()
            {
                var interactorGO = new GameObject { name = "Enabler Interactor" };
                var interactor = interactorGO.AddComponent<EnablerInteractor>();
                return interactor;
            }
        }

        /// <summary>
        /// Interactable that enables another Interactor and Interactable during <see cref="ProcessInteractable"/>.
        /// </summary>
        class EnablerInteractable : XRBaseInteractable
        {
            public XRBaseInteractor interactor { get; set; }

            public XRBaseInteractable interactable { get; set; }

            public bool enableBehaviors { get; set; }

            /// <inheritdoc />
            public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
            {
                base.ProcessInteractable(updatePhase);

                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    if (interactor != null)
                        interactor.enabled = enableBehaviors;

                    if (interactable != null)
                        interactable.enabled = enableBehaviors;
                }
            }

            public static EnablerInteractable CreateInteractable()
            {
                var interactableGO = new GameObject { name = "Enabler Interactable" };
                var interactable = interactableGO.AddComponent<EnablerInteractable>();
                return interactable;
            }
        }
    }
}
