using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
// ReSharper disable Unity.InefficientPropertyAccess -- Test code is easier to read with inefficient access

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class PokeInteractorTests
    {
        static readonly (PokeAxis axis, Vector3 startingPoint, bool valid)[] s_PokeDirections =
        {
            // Note, since test interactables are 1x1x1 in size, we need a starting pos
            // greater than 1 in any direction to start outside the bounds of the object.

            // Valid directions:
            (PokeAxis.X, Vector3.left * 2f, true),
            (PokeAxis.Y, Vector3.down * 2f, true),
            (PokeAxis.Z, Vector3.back * 2f, true),
            (PokeAxis.NegativeX, Vector3.right * 2f, true),
            (PokeAxis.NegativeY, Vector3.up * 2f, true),
            (PokeAxis.NegativeZ, Vector3.forward * 2f, true),
            // Within angle threshold
            (PokeAxis.Z, Quaternion.AngleAxis(44f, Vector3.up) * (Vector3.back * 2f), true),

            // Invalid directions:
            (PokeAxis.X, Vector3.right * 2f, false),
            (PokeAxis.Y, Vector3.up * 2f, false),
            (PokeAxis.Z, Vector3.forward * 2f, false),
            (PokeAxis.NegativeX, Vector3.left * 2f, false),
            (PokeAxis.NegativeY, Vector3.down * 2f, false),
            (PokeAxis.NegativeZ, Vector3.back * 2f, false),
            // Outside angle threshold
            (PokeAxis.Z, Quaternion.AngleAxis(46f, Vector3.up) * (Vector3.back * 2f), false),
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
            filter.pokeConfiguration.Value.enablePokeAngleThreshold = true;
            filter.pokeConfiguration.Value.pokeDirection = args.axis;

            yield return null;

            // Need minimum of 7 frames for an accurate velocity check to occur
            for (int i = 0, displacementFrames = 7; i <= displacementFrames; i++)
            {
                interactor.transform.position = Vector3.Lerp(args.startingPoint, Vector3.zero, (float)i / displacementFrames);
                yield return new WaitForFixedUpdate();
                yield return null;
            }

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
            // This test differs from other target filter tests because the poke interactor processes the target filter on PreProcess, rather than on GetValidTargets.

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

            // Wait 1 frame for next process.
            yield return null;

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

            // Wait one frame for next interactor PreProcess to occur
            yield return null;

            // Validate that process has not been called
            interactor.GetValidTargets(validTargets);
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
            }));

            yield return null;

            // Unlink the filter
            interactor.targetFilter = null;

            yield return null;

            Assert.That(interactor.targetFilter, Is.EqualTo(null));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
                TargetFilterCallback.Unlink,
            }));
        }

        [UnityTest]
        public IEnumerator PokeInteractorTargetFilterModifiesValidTargets()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;

            var interactable = TestUtilities.CreateSimpleInteractable();

            var interactable2 = TestUtilities.CreateSimpleInteractable();
            // Put interactable2 slightly further away than interactable
            interactable2.transform.position = new Vector3(0f, 0f, 0.00001f);

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();

            // Test that normal valid target sorting works
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Create the filter
            var filter = new MockInversionTargetFilter();

            // Link the filter
            interactor.targetFilter = filter;

            yield return new WaitForFixedUpdate();
            yield return null;

            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.EqualTo(new[] { interactable2 }));
        }


        [UnityTest]
        public IEnumerator PokeInteractorValidTargetWithMultipleColliders()
        {
            TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;

            // Setup Colliders first
            GameObject interactableGO = new GameObject("Simple Interactable");
            TestUtilities.CreateGOSphereCollider(interactableGO, false);
            var colliderGO = new GameObject("Secondary Collider");
            TestUtilities.CreateGOSphereCollider(colliderGO, false);
            colliderGO.transform.SetParent(interactableGO.transform, true);

            // Then Rigid Body
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            // Finally interactable (so it will find existing components on Awake)
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();

            // Test that valid targets are correct and there is only 1 in the list
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(validTargets.Count, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator PokeInteractorThinColliderDetectedWithModerateSpeedPoke()
        {
            // Validates the lowered sweep threshold (5mm vs baseline 31.6mm).
            // A moderate-speed poke that displaces 24mm in one frame sits above the new 5mm
            // threshold (triggers SphereCast) but below the old 31.6mm threshold (which would
            // have used OverlapSphere). The interactor passes fully through a 2mm-thick collider,
            // ending 11mm past it, beyond the 2mm hover radius. SphereCast catches the collider
            // along the swept path; OverlapSphere at the endpoint would miss it.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;

            // Use a small hover radius so OverlapSphere at the endpoint can't reach back
            // to the thin collider. Default 15mm would bridge the gap and mask the bug.
            interactor.pokeHoverRadius = 0.002f;

            // Thin collider: 2mm thick in Z, centered at origin.
            // Surfaces at z = -0.001 and z = +0.001.
            var thinGO = new GameObject("Thin Surface");
            var boxCollider = thinGO.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2f, 0.2f, 0.002f);
            var rigidBody = thinGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            var interactable = thinGO.AddComponent<XRSimpleInteractable>();

            // Sync collider into physics world.
            yield return new WaitForFixedUpdate();
            yield return null;

            // Position behind the thin collider (11mm from nearest surface at z = -0.001).
            interactor.transform.position = new Vector3(0f, 0f, -0.012f);

            // Establish motion continuity over consecutive frames.
            // yield return null guarantees exactly one frame, avoiding frame-gap detection
            // that WaitForFixedUpdate can trigger when FixedUpdate cadence differs from Update.
            yield return null;
            yield return null;
            yield return null;

            // Move through the thin collider in one frame.
            // Displacement: 24mm. sqrMagnitude: 0.000576.
            // New threshold (0.000025): canSweep = true → SphereCast from z=-0.012 to z=+0.012 → HIT.
            // Old threshold (0.001): 0.000576 < 0.001 → OverlapSphere at z=+0.012 → MISS.
            interactor.transform.position = new Vector3(0f, 0f, 0.012f);

            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }),
                "SphereCast along the inter-frame path should detect the thin collider");

            Assert.That(interactor.hasHover, Is.True);
        }

        [UnityTest]
        public IEnumerator PokeInteractorTeleportDoesNotCauseGhostHover()
        {
            // Validates the teleport guard (k_MaxReasonableDeltaSqr = 4.0, i.e. 2m).
            // When the interactor jumps more than 2m in a single frame, physically impossible
            // for a human hand at typical XR frame rates, the sweep is suppressed and
            // OverlapSphere at the endpoint is used instead. Without the guard, SphereCast
            // would trace the full teleport path and produce phantom hovers on anything
            // between the old and new positions.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;

            // Place an interactable at the midpoint of the teleport path.
            // If sweep fires, SphereCast from z=0 to z=3 would catch it.
            // If sweep is suppressed, OverlapSphere at z=3 with 15mm radius can't reach z=1.5.
            var midpointGO = new GameObject("Midpoint Interactable");
            var boxCollider = midpointGO.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
            var rigidBody = midpointGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            midpointGO.AddComponent<XRSimpleInteractable>();
            midpointGO.transform.position = new Vector3(0f, 0f, 1.5f);

            // Sync collider into physics world.
            yield return new WaitForFixedUpdate();
            yield return null;

            // Start at origin, establish continuity.
            interactor.transform.position = Vector3.zero;

            yield return null;
            yield return null;
            yield return null;

            // Teleport: 3m displacement in one frame. sqrMagnitude = 9.0 > 4.0.
            // Continuity guard detects this as a teleport, sets m_HasValidPrev = false.
            // canSweep = false → OverlapSphere at z=3 → no detection of midpoint interactable.
            interactor.transform.position = new Vector3(0f, 0f, 3.0f);

            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty,
                "Teleport-like displacement should suppress sweep; interactable along the path should not be detected");
            Assert.That(interactor.hasHover, Is.False);
        }

        [UnityTest]
        public IEnumerator PokeInteractorSweepResumesAfterTeleportOnNextValidFrame()
        {
            // Validates that after a teleport suppresses sweep for one frame,
            // normal sweep behavior resumes on the next consecutive frame with
            // valid (sub-teleport) motion. The continuity state machine should
            // recover gracefully rather than permanently disabling sweep.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreatePokeInteractor();
            interactor.requirePokeFilter = false;
            interactor.pokeHoverRadius = 0.002f;

            // Thin collider at z = 3.0 (near the post-teleport position).
            var thinGO = new GameObject("Post-Teleport Thin Surface");
            var boxCollider = thinGO.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2f, 0.2f, 0.002f);
            var rigidBody = thinGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            var interactable = thinGO.AddComponent<XRSimpleInteractable>();
            thinGO.transform.position = new Vector3(0f, 0f, 3.0f);

            // Sync collider into physics world.
            yield return new WaitForFixedUpdate();
            yield return null;

            // Establish continuity at origin.
            interactor.transform.position = Vector3.zero;

            yield return null;
            yield return null;
            yield return null;

            // Teleport to just behind the thin collider.
            // This frame's sweep is suppressed (delta > 2m), but continuity resets cleanly.
            interactor.transform.position = new Vector3(0f, 0f, 2.988f);

            yield return null;

            // One more frame at the same position to re-establish continuity.
            yield return null;
            yield return null;

            // Moderate-speed poke through the thin collider (24mm displacement).
            // Sweep should fire normally, the teleport suppression was one frame only.
            interactor.transform.position = new Vector3(0f, 0f, 3.012f);

            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }),
                "Sweep should resume after teleport recovery; thin collider should be detected");

            Assert.That(interactor.hasHover, Is.True);
        }
    }
}
