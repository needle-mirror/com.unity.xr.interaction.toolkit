using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
// ReSharper disable Unity.InefficientPropertyAccess -- Test code is easier to read with inefficient access

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class PokeInteractorTests
    {
        static readonly (PokeAxis axis, Vector3 startingPoint, bool valid)[] s_PokeDirections =
        {
            // Valid directions:
            (PokeAxis.X, Vector3.left, true),
            (PokeAxis.Y, Vector3.down, true),
            (PokeAxis.Z, Vector3.back, true),
            (PokeAxis.NegativeX, Vector3.right, true),
            (PokeAxis.NegativeY, Vector3.up, true),
            (PokeAxis.NegativeZ, Vector3.forward, true),
            // Within angle threshold
            (PokeAxis.Z, Quaternion.AngleAxis(44f, Vector3.up) * Vector3.back, true),

            // Invalid directions:
            (PokeAxis.X, Vector3.right, false),
            (PokeAxis.Y, Vector3.up, false),
            (PokeAxis.Z, Vector3.forward, false),
            (PokeAxis.NegativeX, Vector3.left, false),
            (PokeAxis.NegativeY, Vector3.down, false),
            (PokeAxis.NegativeZ, Vector3.back, false),
            // Outside angle threshold
            (PokeAxis.Z, Quaternion.AngleAxis(45f, Vector3.up) * Vector3.back, false),
        };

        static readonly (Vector3 interactorPosition, bool valid)[] s_PokeHovers =
        {
            (Vector3.zero, true),
            // This radius is calculated by the axisDepth in poke logic and the pokeHoverRadius in the interactor.
            (new Vector3(0f, 0f, 1.015f), true),
            (new Vector3(0f, 0f, 1.016f), false),
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator PokeInteractorCanConstrainPokeDirection([ValueSource(nameof(s_PokeDirections))] (PokeAxis axis, Vector3 startingPoint, bool valid) args)
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            var filter = interactable.gameObject.AddComponent<XRPokeFilter>();
            filter.pokeConfiguration.Value.pokeDirection = args.axis;

            interactor.transform.position = args.startingPoint;

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.transform.position = Vector3.zero;

            yield return new WaitForFixedUpdate();
            yield return null;

            if (args.valid)
            {
                Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
                Assert.That(interactor.hasSelection, Is.True);
            }
            else
            {
                Assert.That(interactor.interactablesSelected, Is.Empty);
                Assert.That(interactor.hasSelection, Is.False);
            }
        }

        [UnityTest]
        public IEnumerator PokeInteractorCanHoverInteractable([ValueSource(nameof(s_PokeHovers))] (Vector3 interactorPosition, bool valid) args)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.transform.position = args.interactorPosition;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.gameObject.AddComponent<XRPokeFilter>();

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);

            if (args.valid)
            {
                Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
                Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
                Assert.That(interactor.hasHover, Is.True);
            }
            else
            {
                Assert.That(validTargets, Is.Empty);
                Assert.That(interactor.interactablesHovered, Is.Empty);
                Assert.That(interactor.hasHover, Is.False);
            }
        }

        [UnityTest]
        public IEnumerator PokeInteractorValidTargetsListEmptyWhenInteractorDisabled()
        {
            // This tests that the poke interactor will return an empty list of valid 
            // targets when the interactor component or the GameObject is disabled and
            // will correctly add the target back into the list upon enabling the interactor.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.gameObject.AddComponent<XRPokeFilter>();

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Disable interactor GameObject
            interactor.gameObject.SetActive(false);

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
            
            // Enable interactor GameObject
            interactor.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
            
            // Disable interactor
            interactor.enabled = false;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
            
            // Enable interactor
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }
        
        [UnityTest]
        public IEnumerator PokeInteractorValidTargetsRemainEmptyWhenInteractorEnabledWithNoValidPoke()
        {
            // This tests that the poke interactor will return an empty list of valid 
            // targets when the interactor component or the GameObject is disabled
            // while it has a valid target and the valid targets will remain empty
            // when the interactor is enabled again without a valid poke contact.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.gameObject.AddComponent<XRPokeFilter>();

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);

            // Disable interactor GameObject
            interactor.gameObject.SetActive(false);

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
            
            // Position interactor to an invalid poke position and enable interactor GameObject
            interactor.transform.position = Vector3.one * 2;
            interactor.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return null;
            
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Position interactor to a valid poke position
            interactor.transform.position = Vector3.zero;
            yield return new WaitForFixedUpdate();
            yield return null;
            
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
            
            // Disable interactor component
            interactor.enabled = false;
            
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);
            
            // Position interactor to an invalid poke position and enable interactor
            interactor.transform.position = Vector3.one * 2;
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return null;
            
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);
            Assert.That(interactor.hasHover, Is.False);

            // Position interactor to a valid poke position
            interactor.transform.position = Vector3.zero;
            yield return new WaitForFixedUpdate();
            yield return null;
            
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator PokeInteractorCanUseTargetFilter()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;
            var interactable = TestUtilities.CreateSimpleInteractable();
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(interactable.isHovered, Is.True);

            // Create the filter
            var filter = new MockTargetFilter();
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>()));

            // Link the filter
            interactor.targetFilter = filter;
            Assert.That(interactor.targetFilter, Is.EqualTo(filter));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
            }));

            // Process the filter
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(interactor.targetFilter, Is.EqualTo(filter));
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
            }));

            // Disable the filter and check if it will no longer be processed
            filter.canProcess = false;
            interactor.GetValidTargets(validTargets);
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
            }));

            // Unlink the filter
            interactor.targetFilter = null;
            Assert.That(interactor.targetFilter, Is.EqualTo(null));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
                TargetFilterCallback.Unlink,
            }));
        }
    }
}