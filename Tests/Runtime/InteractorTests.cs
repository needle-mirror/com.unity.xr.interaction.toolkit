using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
#if UNITY_EDITOR && UNITY_6000_0_50_OR_NEWER
using UnityEditor;
#endif
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InteractorTests
    {
#if UNITY_EDITOR && UNITY_6000_0_50_OR_NEWER
        const string k_DynamicsPath = "ProjectSettings/DynamicsManager.asset";
        const bool k_DefaultGenerateOnTriggerStayEvents = true;

        // Used for setting the Generate On Trigger Stay Events property in the Physics settings
        // since there is no public API for it.
        SerializedObject m_PhysicsManager;
        SerializedProperty m_GenerateOnTriggerStayEvents;
        bool m_OriginalGenerateOnTriggerStayEvents;
#endif

        static readonly Type[] s_ContactInteractors =
        {
            typeof(XRDirectInteractor),
            typeof(XRSocketInteractor),
        };

        static readonly bool[] s_BooleanValues = { false, true };

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
#if UNITY_EDITOR && UNITY_6000_0_50_OR_NEWER
            var found = AssetDatabase.LoadAllAssetsAtPath(k_DynamicsPath);
            if (found == null)
            {
                Assert.Fail($"Failed to load asset {k_DynamicsPath} containing data for the PhysicsManager.");
                return;
            }

            m_PhysicsManager = new SerializedObject(found[0]);
            m_GenerateOnTriggerStayEvents = m_PhysicsManager.FindProperty("m_GenerateOnTriggerStayEvents");
            if (m_GenerateOnTriggerStayEvents == null)
            {
                Assert.Fail("Failed to find property m_GenerateOnTriggerStayEvents in the PhysicsManager. Has it been renamed or removed in this version of Unity?");
                return;
            }

            // Capture Physics Project Setting value and set to known default
            m_OriginalGenerateOnTriggerStayEvents = m_GenerateOnTriggerStayEvents.boolValue;
            m_GenerateOnTriggerStayEvents.boolValue = k_DefaultGenerateOnTriggerStayEvents;
            m_PhysicsManager.ApplyModifiedProperties();
#endif
        }

        [SetUp]
        public void SetUp()
        {
#if UNITY_EDITOR && UNITY_6000_0_50_OR_NEWER
            if (m_GenerateOnTriggerStayEvents != null)
            {
                m_GenerateOnTriggerStayEvents.boolValue = k_DefaultGenerateOnTriggerStayEvents;
                m_PhysicsManager.ApplyModifiedProperties();
            }
#endif
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
#if UNITY_EDITOR && UNITY_6000_0_50_OR_NEWER
            if (m_PhysicsManager != null)
            {
                if (m_GenerateOnTriggerStayEvents != null)
                {
                    // Restore Physics Project Setting value back to the original
                    m_GenerateOnTriggerStayEvents.boolValue = m_OriginalGenerateOnTriggerStayEvents;
                    m_PhysicsManager.ApplyModifiedProperties();
                }

                m_PhysicsManager.Dispose();
                m_PhysicsManager = null;
            }
#endif
        }

        [UnityTest]
        public IEnumerator ContactInteractorTargetStaysValidWhenTouchingAnyCollider(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This tests that an Interactable will stay as a valid target as long as
            // the Direct and Socket Interactor is touching any Collider associated with the Interactable,
            // and remains so if only some (but not all) of the Interactable colliders leaves.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            var manager = TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            var triggerCollider = interactor.GetComponent<SphereCollider>();
            Assert.That(triggerCollider, Is.Not.Null);
            Assert.That(triggerCollider.isTrigger, Is.True);

            var interactable = TestUtilities.CreateGrabInteractable();
            // Prevent the Interactable from being selected to allow the object to be moved freely
            interactable.interactionLayers = 0;
            var sphereCollider = interactable.GetComponent<SphereCollider>();
            sphereCollider.center = Vector3.zero;
            sphereCollider.radius = 0.5f;
            Assert.That(sphereCollider, Is.Not.Null);
            interactable.transform.position = Vector3.forward * 10f;

            // Create another Collider to have as part of the Interactable
            var boxColliderTransform = new GameObject("Box Collider", typeof(BoxCollider)).transform;
            boxColliderTransform.SetParent(interactable.transform);
            boxColliderTransform.localPosition = Vector3.right;
            boxColliderTransform.localRotation = Quaternion.identity;
            var boxCollider = boxColliderTransform.GetComponent<BoxCollider>();
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;

            interactable.colliders.Clear();
            interactable.colliders.Add(sphereCollider);
            interactable.colliders.Add(boxCollider);

            interactable.enabled = false;
            interactable.enabled = true;

            Assert.That(manager.TryGetInteractableForCollider(sphereCollider, out var sphereColliderInteractable), Is.True);
            Assert.That(sphereColliderInteractable, Is.EqualTo(interactable));
            Assert.That(manager.TryGetInteractableForCollider(boxCollider, out var boxColliderInteractable), Is.True);
            Assert.That(boxColliderInteractable, Is.EqualTo(interactable));

            yield return new WaitForFixedUpdate();
            yield return null;

            var directOverlaps = Physics.OverlapSphere(triggerCollider.transform.position, triggerCollider.radius, -1, QueryTriggerInteraction.Ignore);
            Assert.That(directOverlaps, Is.Empty);

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            // Move the Interactable to the Direct Interactor so that it overlaps both colliders
            interactable.transform.position = Vector3.left * 0.5f;

            yield return new WaitForFixedUpdate();
            yield return null;

            directOverlaps = Physics.OverlapSphere(triggerCollider.transform.position, triggerCollider.radius, -1, QueryTriggerInteraction.Ignore);
            Assert.That(directOverlaps, Is.EquivalentTo(new Collider[] { sphereCollider, boxCollider }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            // Move the Interactable some so one of the colliders leaves
            interactable.transform.position = Vector3.left * 2f;

            yield return new WaitForFixedUpdate();
            yield return null;

            directOverlaps = Physics.OverlapSphere(triggerCollider.transform.position, triggerCollider.radius, -1, QueryTriggerInteraction.Ignore);
            Assert.That(directOverlaps, Is.EquivalentTo(new Collider[] { boxCollider }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            // Move the Interactable some so the other collider is the one being hovered
            // to test that colliders can re-enter after previously exiting
            interactable.transform.position = Vector3.right * 1f;

            yield return new WaitForFixedUpdate();
            yield return null;

            directOverlaps = Physics.OverlapSphere(triggerCollider.transform.position, triggerCollider.radius, -1, QueryTriggerInteraction.Ignore);
            Assert.That(directOverlaps, Is.EquivalentTo(new Collider[] { sphereCollider }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            // Move the Interactable so all colliders exits the Direct Interactor
            interactable.transform.position = Vector3.forward * 10f;

            yield return new WaitForFixedUpdate();
            yield return null;

            directOverlaps = Physics.OverlapSphere(triggerCollider.transform.position, triggerCollider.radius, -1, QueryTriggerInteraction.Ignore);
            Assert.That(directOverlaps, Is.Empty);

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ContactInteractorTargetStaysValidWhenTouchingAnyTriggerCollider(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This tests that an Interactable will stay as a valid target as long as
            // the Direct and Socket Interactor with multiple trigger colliders is touching
            // the collider associated with the Interactable, and remains so if only some
            // (but not all) of the Interactor trigger colliders leaves.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            // Create an interactable offset to the right
            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = Vector3.right * 10f;

            // Setup collider on the interactable
            Assert.That(interactable.GetComponent<Rigidbody>(), Is.Not.Null);
            var interactableCollider = interactable.GetComponent<SphereCollider>();
            Assert.That(interactableCollider, Is.Not.Null);
            Assert.That(interactableCollider.isTrigger, Is.False);
            interactableCollider.center = Vector3.zero;
            interactableCollider.radius = 0.5f;

            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            // Setup both trigger colliders on the interactor.
            // Create a box and sphere, and position them such that the box overlaps the left half of the sphere.
            var sphereTrigger = interactor.GetComponent<SphereCollider>();
            Assert.That(sphereTrigger, Is.Not.Null);
            Assert.That(sphereTrigger.isTrigger, Is.True);
            sphereTrigger.center = Vector3.zero;
            sphereTrigger.radius = 0.5f;

            var boxTrigger = interactor.gameObject.AddComponent<BoxCollider>();
            boxTrigger.isTrigger = true;
            boxTrigger.center = new Vector3(-0.5f, 0f, 0f);
            boxTrigger.size = Vector3.one;

            yield return new WaitForFixedUpdate();
            yield return null;

            var overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider }));

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Move the interactable to the interactor so that it overlaps one collider
            interactable.transform.position = new Vector3(0.75f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Move the interactable some more so it overlaps both trigger colliders
            interactable.transform.position = new Vector3(0.25f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger, boxTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Move the interactable back so it exits the box but remain entered on the sphere.
            // This tests that the interactable remains hovered while still touching one of the interactor's colliders.
            interactable.transform.position = new Vector3(0.75f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Move the interactable so it exits all trigger colliders of the interactor
            interactable.transform.position = Vector3.right * 10f;

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ContactInteractorTargetConvertsColliderToInteractableWhileTouchingMultipleTriggers(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This tests that an Interactable will stay as a valid target as long as
            // the Direct and Socket Interactor with multiple trigger colliders is touching
            // the collider associated with the Interactable, and remains so if only some
            // (but not all) of the Interactor trigger colliders leaves. This test verifies
            // the conversion logic from being an unassociated collider to one associated with
            // an interactable keeps the count of trigger enters of that collider.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            // Create an interactable offset to the right
            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = Vector3.right * 10f;

            // Disable so the manager does not have an association mapping the collider
            // to the interactable.
            interactable.enabled = false;

            // Setup collider on the interactable
            Assert.That(interactable.GetComponent<Rigidbody>(), Is.Not.Null);
            var interactableCollider = interactable.GetComponent<SphereCollider>();
            Assert.That(interactableCollider, Is.Not.Null);
            Assert.That(interactableCollider.isTrigger, Is.False);
            interactableCollider.center = Vector3.zero;
            interactableCollider.radius = 0.5f;

            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            // Setup both trigger colliders on the interactor.
            // Create a box and sphere, and position them such that the box overlaps the left half of the sphere.
            var sphereTrigger = interactor.GetComponent<SphereCollider>();
            Assert.That(sphereTrigger, Is.Not.Null);
            Assert.That(sphereTrigger.isTrigger, Is.True);
            sphereTrigger.center = Vector3.zero;
            sphereTrigger.radius = 0.5f;

            var boxTrigger = interactor.gameObject.AddComponent<BoxCollider>();
            boxTrigger.isTrigger = true;
            boxTrigger.center = new Vector3(-0.5f, 0f, 0f);
            boxTrigger.size = Vector3.one;

            yield return new WaitForFixedUpdate();
            yield return null;

            var overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider }));

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Move the interactable to the interactor so that it overlaps one collider
            interactable.transform.position = new Vector3(0.75f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Move the interactable some more so it overlaps both trigger colliders
            interactable.transform.position = new Vector3(0.25f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger, boxTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Enable the interactable so the collider becomes associated with the interactable
            interactable.enabled = true;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Move the interactable back so it exits the box but remain entered on the sphere.
            // This tests that the interactable remains hovered while still touching one of the interactor's colliders
            // after being converted from an unassociated collider.
            interactable.transform.position = new Vector3(0.75f, 0f, 0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider, sphereTrigger }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Move the interactable so it exits all trigger colliders of the interactor
            interactable.transform.position = Vector3.right * 10f;

            yield return new WaitForFixedUpdate();
            yield return null;

            overlaps = Physics.OverlapSphere(
                interactableCollider.transform.position, interactableCollider.radius, -1,
                QueryTriggerInteraction.Collide);
            Assert.That(overlaps, Is.EquivalentTo(new Collider[] { interactableCollider }));

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactor.interactablesHovered, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ContactInteractorCullsValidTargetsWhenInteractableUnregistered(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will remove an unregistered Interactable
            // from its valid targets list.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            var interactable = TestUtilities.CreateGrabInteractable();

            yield return new WaitForFixedUpdate();

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            interactable.enabled = false;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ContactInteractorCullsValidTargetsUponRegistering(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will update the list of valid targets
            // to exclude those that have been unregistered during the time when the Interactor
            // was not subscribed to the unregister event.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            var interactable = TestUtilities.CreateGrabInteractable();

            // Wait both for fixed update and a frame to ensure the Interactor has had a chance to update
            // Direct interactor may update on update or on fixed update
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            interactor.enabled = false;
            interactable.enabled = false;
            interactor.enabled = true;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesValidTargetsForPreviouslyUnassociatedCollidersWhenInteractableRegistered(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will maintain the list of all entered Colliders
            // so that if any of them later become associated with a registered Interactable,
            // that Interactable will become a valid target.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = Vector3.forward * 10f;
            interactable.enabled = false;

            yield return new WaitForFixedUpdate();

            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            interactable.transform.position = Vector3.zero;

            // Wait both for fixed update and a frame to ensure the Interactor has had a chance to update
            // Direct interactor may update on update or on fixed update
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            interactable.enabled = true;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesValidTargetsForPreviouslyUnassociatedCollidersUponRegistering(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will later associate the collider when
            // the Interactable is registered during the time when the Interactor
            // was not subscribed to the register event.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = Vector3.forward * 10f;
            interactable.enabled = false;

            yield return new WaitForFixedUpdate();

            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            interactor.enabled = false;
            interactable.transform.position = Vector3.zero;

            yield return new WaitForFixedUpdate();

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);

            interactable.enabled = true;
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator ContactInteractorIgnoresDisabledCollidersWhenSortingValidTargets(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will ignore disabled colliders
            // when sorting to find the closest interactable to select.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            // Create Interaction Manager
            TestUtilities.CreateInteractionManager();

            // Interactable 1 has a single sphere collider centered on its local origin.
            // The sphere collider has a radius of 1.
            var interactable1 = TestUtilities.CreateGrabInteractable();
            interactable1.transform.position = new Vector3(-1.1f, 0, 0);
            interactable1.enabled = false;
            interactable1.name = "interactable1";

            // Interactable 1 has a single sphere collider centered on its local origin.
            // The sphere collider has a radius of 1. It is also disabled.
            var interactable2 = TestUtilities.CreateGrabInteractable();
            interactable2.GetComponent<SphereCollider>().enabled = false;
            interactable2.transform.position = new Vector3(1, 0, 0);
            interactable2.enabled = false;
            interactable2.name = "interactable2";

            yield return new WaitForFixedUpdate();

            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            interactor.enabled = false;

            yield return new WaitForFixedUpdate();

            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty, $"All interactors and interactables are disabled, so there should be no valid targets.");

            interactor.enabled = true;
            interactable1.enabled = true;
            interactable2.enabled = true;

            yield return new WaitForFixedUpdate();

            // Since interactable2's collider is disabled, it should not show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable1 }));
        }

        [UnityTest]
        public IEnumerator ContactInteractorValidTargetsListEmptyWhenInteractorDisabled(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will clear valid targets
            // and stayed colliders when the interactor or its GameObject is disabled and
            // the targets will be correctly added back in when the interactor is enabled again.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            // Create Interactable
            var interactable = TestUtilities.CreateGrabInteractable();
            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // De-activate the interactor GameObject
            interactor.gameObject.SetActive(false);

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Reactivate the interactor GameObject
            interactor.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // De-activate the interactor component.
            interactor.enabled = false;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Reactivate the interactor component
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactInteractorValidTargetsRemainClearWhenEnabledWithNoContact(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            // This will test that the Direct and Socket Interactor will clear valid targets
            // and colliders when the interactor is disabled during contact and the valid
            // targets and colliders will remain clear when the interactor is enabled again
            // while not touching any colliders.

            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);

            Assert.That(interactor, Is.Not.Null);

            // Create Interactable
            var interactable = TestUtilities.CreateGrabInteractable();
            Vector3 interactorInitPosition = interactor.transform.position;
            Vector3 interactorMovedPosition = Vector3.one * 2f;
            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // De-activate the interactor component.
            interactor.enabled = false;

            // Reposition the interactor away from the interactable and re-enable the interactor via Component
            interactor.transform.position = interactorMovedPosition;
            interactor.enabled = true;

            // Ensure no lingering hovers when interactor is moved away when disabled
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Move the interactor to the initial position
            interactor.transform.position = interactorInitPosition;
            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // De-activate the interactor GameObject.
            interactor.gameObject.SetActive(false);

            // Reposition the interactor away from the interactable and re-enable the interactor via GameObject
            interactor.transform.position = interactorMovedPosition;
            interactor.gameObject.SetActive(true);

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Move the interactor to the initial position
            interactor.transform.position = interactorInitPosition;
            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesStayedCollidersOnDisablingInteractorCollider(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactor, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Disable the interactor's collider component.
            interactor.GetComponent<Collider>().enabled = false;

            yield return new WaitForFixedUpdate();
            yield return null;

            // Since interactor's collider is disabled, it should not allow the interactable to show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            yield return new WaitForFixedUpdate();

            // Re-enable the collider component, the interactor should re-hover the interactable.
            interactor.GetComponent<Collider>().enabled = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesStayedCollidersOnDisablingInteractableCollider(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactor, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Disable the interactable's collider component.
            interactable.GetComponent<Collider>().enabled = false;

            yield return new WaitForFixedUpdate();
            yield return null;

            // Since interactable's collider is disabled, it should not show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            yield return new WaitForFixedUpdate();

            // Re-enable the collider component, the interactor should re-hover it.
            interactable.GetComponent<Collider>().enabled = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesStayedCollidersOnDeactivatingInteractableObject(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactor, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Deactivate the interactable's gameObject.
            interactable.gameObject.SetActive(false);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Since interactable's collider is effectively disabled, it should not show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            yield return new WaitForFixedUpdate();

            // Activating the interactable's gameObject should cause the interactor to re-hover it.
            interactable.gameObject.SetActive(true);

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesStayedCollidersOnDestroyingInteractableCollider(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactor, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Destroy the interactable's collider.
            Object.Destroy(interactable.GetComponent<Collider>());

            yield return new WaitForFixedUpdate();
            yield return null;

            // Since interactable's collider is destroyed, it should not show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
        }

        [UnityTest]
        public IEnumerator ContactInteractorUpdatesStayedCollidersOnDestroyingInteractableObject(
            [ValueSource(nameof(s_ContactInteractors))] Type interactorType,
            [ValueSource(nameof(s_BooleanValues))] bool generateOnTriggerStayEvents)
        {
            ApplyTriggerStaySetting(generateOnTriggerStayEvents);

            TestUtilities.CreateInteractionManager();
            var interactor = CreateContactInteractor(interactorType);
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactor, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Check that the interactable is a valid target of and can be hovered by the interactor.
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Destroy the interactable's gameObject.
            Object.Destroy(interactable.gameObject);

            yield return new WaitForFixedUpdate();
            yield return null;

            // Since interactable's gameObject is destroyed, it should not show up in the list of valid targets.
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
        }

        [UnityTest]
        public IEnumerator InteractorCanProcessHoverFilters()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactor.validTargets.Add(interactable);

            var filter1WasProcessed = false;
            var filter1 = new XRHoverFilterDelegate((x, y) =>
            {
                filter1WasProcessed = true;
                return true;
            });
            interactor.hoverFilters.Add(filter1);

            var filter2WasProcessed = false;
            var filter2 = new XRHoverFilterDelegate((x, y) =>
            {
                filter2WasProcessed = true;
                return true;
            });
            interactor.hoverFilters.Add(filter2);

            yield return null;

            Assert.That(filter1WasProcessed, Is.True);
            Assert.That(filter2WasProcessed, Is.True);
            Assert.That(interactable.interactorsHovering, Is.EquivalentTo(new[] { interactor }));

            // Add filter that returns false
            var filter3WasProcessed = false;
            var filter3 = new XRHoverFilterDelegate((x, y) =>
            {
                filter3WasProcessed = true;
                return false;
            });
            interactor.hoverFilters.Add(filter3);

            yield return null;

            Assert.That(filter3WasProcessed, Is.True);
            Assert.That(interactable.interactorsHovering, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InteractorCanProcessSelectFilters()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactor.validTargets.Add(interactable);

            var filter1WasProcessed = false;
            var filter1 = new XRSelectFilterDelegate((x, y) =>
            {
                filter1WasProcessed = true;
                return true;
            });
            interactor.selectFilters.Add(filter1);

            var filter2WasProcessed = false;
            var filter2 = new XRSelectFilterDelegate((x, y) =>
            {
                filter2WasProcessed = true;
                return true;
            });
            interactor.selectFilters.Add(filter2);

            yield return null;

            Assert.That(filter1WasProcessed, Is.True);
            Assert.That(filter2WasProcessed, Is.True);
            Assert.That(interactable.interactorsSelecting, Is.EquivalentTo(new[] { interactor }));

            // Add filter that returns false
            var filter3WasProcessed = false;
            var filter3 = new XRSelectFilterDelegate((x, y) =>
            {
                filter3WasProcessed = true;
                return false;
            });
            interactor.selectFilters.Add(filter3);

            yield return null;

            Assert.That(filter3WasProcessed, Is.True);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InteractableCanBeManuallySelected()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateMockInputInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();

            interactor.keepSelectedTargetValid = true;

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            interactor.StartManualInteraction((IXRSelectInteractable)interactable);

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasSelection, Is.True);
            Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor }));
            Assert.That(interactable.isSelected, Is.True);

            Assert.That(interactor.isPerformingManualInteraction, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasSelection, Is.True);
            Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor }));
            Assert.That(interactable.isSelected, Is.True);

            Assert.That(interactor.isPerformingManualInteraction, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            interactor.EndManualInteraction();

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
        }

        [UnityTest]
        public IEnumerator UnregisteredInteractableCannotBeManuallySelected()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateMockInputInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();

            interactor.keepSelectedTargetValid = true;
            interactable.enabled = false;

            Assert.That(manager.IsRegistered((IXRInteractor)interactor), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)interactable), Is.False);

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            LogAssert.Expect(LogType.Warning, "Tried to start manual interaction but select was prevented.");
            interactor.StartManualInteraction((IXRSelectInteractable)interactable);

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            LogAssert.Expect(LogType.Warning, "Tried to end manual interaction but was not performing manual interaction. Ignoring request.");
            interactor.EndManualInteraction();

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.hasSelection, Is.False);
            Assert.That(interactable.interactorsSelecting, Is.Empty);
            Assert.That(interactable.isSelected, Is.False);

            Assert.That(interactor.isPerformingManualInteraction, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
        }

        static XRBaseInteractor CreateContactInteractor(Type interactorType)
        {
            if (interactorType == typeof(XRDirectInteractor))
                return TestUtilities.CreateDirectInteractor();

            if (interactorType == typeof(XRSocketInteractor))
            {
                var interactor = TestUtilities.CreateSocketInteractor();

                // Set the socket's recycleDelayTime to 0 instead of the default 1s.
                interactor.recycleDelayTime = 0f;

                return interactor;
            }

            Assert.Fail($"Unhandled interactor type: {interactorType}");
            return null;
        }

        void ApplyTriggerStaySetting(bool generateOnTriggerStayEvents)
        {
#if UNITY_EDITOR
#if UNITY_6000_0_50_OR_NEWER
            Assume.That(m_PhysicsManager, Is.Not.Null);
            Assume.That(m_GenerateOnTriggerStayEvents, Is.Not.Null);
            m_GenerateOnTriggerStayEvents.boolValue = generateOnTriggerStayEvents;
            m_PhysicsManager.ApplyModifiedProperties();
#else
            if (!generateOnTriggerStayEvents)
                Assert.Ignore("Generate On Trigger Stay Events project setting only available with Unity 6 or newer (at patch version 6000.0.50f1).");
#endif
#endif
        }
    }
}
