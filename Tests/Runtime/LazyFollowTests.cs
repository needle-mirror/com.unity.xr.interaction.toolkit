using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class LazyFollowTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        // Helper to create an inactive GameObject with LazyFollow already awoken,
        // so callers can configure the component before OnEnable fires.
        static LazyFollow CreateInactiveLazyFollow(string name = "Lazy Follow Object")
        {
            var go = new GameObject(name);
            go.SetActive(false);
            return go.AddComponent<LazyFollow>();
        }

        // Helper to build a standard LookAt snap scenario.
        // Target at (5,0,0), offset (0,0,5) → snap position = (5,0,5).
        // Correct look direction from (5,0,5) toward (5,0,0): forward = (0,0,1) = Vector3.forward.
        // If position is NOT applied before rotation is computed, SubscribeAndUpdate will have
        // already zeroed transform.position to (0,0,0), giving forward = (-1,0,0) = Vector3.left.
        static (LazyFollow lazyFollow, GameObject targetGO) CreateLookAtSnapScenario(
            LazyFollow.RotationFollowMode rotationFollowMode)
        {
            var lazyFollow = CreateInactiveLazyFollow();
            lazyFollow.snapOnEnable = true;
            lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.Follow;
            lazyFollow.rotationFollowMode = rotationFollowMode;
            lazyFollow.targetOffset = new Vector3(0f, 0f, 5f);

            var targetGO = new GameObject("Target");
            targetGO.transform.position = new Vector3(5f, 0f, 0f);
            lazyFollow.target = targetGO.transform;

            return (lazyFollow, targetGO);
        }

        [UnityTest]
        public IEnumerator OnEnable_WithSnapOnEnableDisabled_DoesNotZeroOutPosition()
        {
            // Regression test: in the old OnEnable implementation, SubscribeAndUpdate fired
            // immediately with Value = float3(0,0,0) (the initial tweenable value). When
            // snapOnEnable was false, HandleTween was never called, so the transform position
            // was left at the world origin instead of the object's actual position.
            var lazyFollow = CreateInactiveLazyFollow();
            lazyFollow.snapOnEnable = false;
            lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.Follow;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.None;

            var targetGO = new GameObject("Target");
            targetGO.transform.position = new Vector3(5f, 5f, 5f);
            lazyFollow.target = targetGO.transform;

            var expectedPosition = new Vector3(1f, 2f, 3f);
            lazyFollow.transform.position = expectedPosition;

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should not be zeroed out when snapOnEnable is false.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_WithPositionFollowModeNone_DoesNotZeroOutPosition()
        {
            // Regression test: in the old OnEnable implementation, HandleTween for position was
            // guarded by `if (positionFollowMode != None)`. With positionFollowMode=None, HandleTween
            // was never called even when snapOnEnable was true, leaving the position at the world origin.
            var lazyFollow = CreateInactiveLazyFollow();
            lazyFollow.snapOnEnable = true;
            lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.None;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.None;

            var expectedPosition = new Vector3(1f, 2f, 3f);
            lazyFollow.transform.position = expectedPosition;

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should not be zeroed out when positionFollowMode is None.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_WithSnapOnEnableEnabled_SnapsToTargetPosition()
        {
            // Tests that when snapOnEnable is true and a target is available, the object
            // immediately snaps to the target's position on enable.
            var lazyFollow = CreateInactiveLazyFollow();
            lazyFollow.snapOnEnable = true;
            lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.Follow;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.None;
            lazyFollow.targetOffset = Vector3.zero;

            var targetPosition = new Vector3(5f, 5f, 5f);
            var targetGO = new GameObject("Target");
            targetGO.transform.position = targetPosition;
            lazyFollow.target = targetGO.transform;

            lazyFollow.transform.position = new Vector3(1f, 2f, 3f);

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(targetPosition).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should snap to the target position when snapOnEnable is true.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_AfterDisableAndReenable_DoesNotZeroOutPosition()
        {
            // Tests that re-enabling the component after a disable cycle does not zero out the
            // transform position. This validates that the fix works across multiple enable/disable cycles.
            var lazyFollow = CreateInactiveLazyFollow();
            lazyFollow.snapOnEnable = false;
            lazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.Follow;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.None;

            var targetGO = new GameObject("Target");
            targetGO.transform.position = new Vector3(5f, 5f, 5f);
            lazyFollow.target = targetGO.transform;

            var expectedPosition = new Vector3(1f, 2f, 3f);
            lazyFollow.transform.position = expectedPosition;

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should be maintained on first enable.");

            lazyFollow.gameObject.SetActive(false);
            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should be maintained after re-enable.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_WithSnapOnEnable_LookAt_RotationComputedFromSnappedPosition()
        {
            // Regression test: the position snap must be fully applied to transform.position before
            // TryGetThresholdTargetRotation runs, because LookAt reads transform.position directly:
            //   forward = (transform.position - m_Target.position).normalized
            // SubscribeAndUpdate fires immediately with the tweenable's initial Value of float3(0,0,0),
            // zeroing transform.position. If HandleTween(1f) for position is not called before the
            // rotation target is computed, the look direction is calculated from (0,0,0) instead of
            // the snap position, producing an incorrect rotation.
            //
            // Setup: target at (5,0,0), offset (0,0,5) → snap position = (5,0,5).
            //   Correct: forward = (5,0,5)-(5,0,0) = (0,0,1) → transform.forward ≈ Vector3.forward
            //   Wrong:   forward = (0,0,0)-(5,0,0) = (-1,0,0) → transform.forward ≈ Vector3.left
            var (lazyFollow, _) = CreateLookAtSnapScenario(LazyFollow.RotationFollowMode.LookAt);

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(new Vector3(5f, 0f, 5f)).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should snap to the target offset position.");

            // The angle between actual and expected rotation must be well under 1 degree.
            // In the bug case this angle exceeds 90 degrees (facing left vs. facing forward).
            var expectedRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            Assert.That(Quaternion.Angle(lazyFollow.transform.rotation, expectedRotation),
                Is.LessThan(1f),
                "LookAt rotation must be computed from the snapped position, not the zeroed-out intermediate position.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_WithSnapOnEnable_LookAtWithWorldUp_RotationComputedFromSnappedPosition()
        {
            // Same ordering regression as the LookAt test above, but for LookAtWithWorldUp mode.
            // LookAtWithWorldUp also reads transform.position to compute its forward vector before
            // projecting it onto the world-up plane:
            //   forward = (transform.position - m_Target.position).normalized
            //   → projected onto XZ plane → rotation
            // The position snap must be applied before this computation runs.
            //
            // Setup: target at (5,0,0), offset (0,0,5) → snap position = (5,0,5).
            //   Correct: forward = (0,0,1) → transform.forward ≈ Vector3.forward
            //   Wrong:   forward = (-1,0,0) → transform.forward ≈ Vector3.left
            var (lazyFollow, _) = CreateLookAtSnapScenario(LazyFollow.RotationFollowMode.LookAtWithWorldUp);

            lazyFollow.gameObject.SetActive(true);

            Assert.That(lazyFollow.transform.position,
                Is.EqualTo(new Vector3(5f, 0f, 5f)).Using(Vector3ComparerWithEqualsOperator.Instance),
                "Position should snap to the target offset position.");

            var expectedRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            Assert.That(Quaternion.Angle(lazyFollow.transform.rotation, expectedRotation),
                Is.LessThan(1f),
                "LookAtWithWorldUp rotation must be computed from the snapped position, not the zeroed-out intermediate position.");

            yield return null;
        }
    }
}
