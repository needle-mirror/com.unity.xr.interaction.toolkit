using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class ConstrainedXRBodyManipulatorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator MoveBody_CharacterController()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var characterController = xrOrigin.Origin.AddComponent<CharacterController>();
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.skinWidth = 0.1f;
            var cameraTransform = xrOrigin.Camera.transform;
            var bodyPositionEvaluator = ScriptableObject.CreateInstance<UnderCameraBodyPositionEvaluator>();

            // Move camera anywhere, so long as the calculated body position and height are different from the initial
            // values for the character controller.
            cameraTransform.localPosition = new Vector3(1f, 1.5f, 0.5f);
            cameraTransform.localRotation = Quaternion.Euler(30f, 60f, 0f);
            var bodyGroundPosition = bodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin);
            var cameraHeight = xrOrigin.CameraInOriginSpaceHeight;
            var expectedCapsuleHeight = cameraHeight - bodyGroundPosition.y;
            var expectedCapsuleCenter = new Vector3(bodyGroundPosition.x, bodyGroundPosition.y + cameraHeight * 0.5f + characterController.skinWidth, bodyGroundPosition.z);
            Assert.That(expectedCapsuleHeight, Is.Not.EqualTo(characterController.height).Using(FloatEqualityComparer.Instance));
            Assert.That(expectedCapsuleCenter, Is.Not.EqualTo(characterController.center).Using(Vector3ComparerWithEqualsOperator.Instance));

            // Link manipulator to body
            var xrBody = new XRMovableBody(xrOrigin, bodyPositionEvaluator);
            var characterControllerManipulator = ScriptableObject.CreateInstance<CharacterControllerBodyManipulator>();
            xrBody.LinkConstrainedManipulator(characterControllerManipulator);
            Assert.That(characterControllerManipulator.linkedBody.xrOrigin, Is.EqualTo(xrOrigin));
            Assert.That(characterControllerManipulator.linkedBody.bodyPositionEvaluator, Is.EqualTo(bodyPositionEvaluator));
            Assert.That(characterControllerManipulator.characterController, Is.EqualTo(characterController));

            // Capsule should match the body position and height and relative to the origin, and the origin should move
            var motion = Vector3.forward;
            var origin = xrOrigin.Origin.transform;
            var expectedOriginPosition = origin.position + motion;
            characterControllerManipulator.MoveBody(motion);
            Assert.That(characterController.height, Is.EqualTo(expectedCapsuleHeight).Using(FloatEqualityComparer.Instance));
            Assert.That(characterController.center, Is.EqualTo(expectedCapsuleCenter).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(characterControllerManipulator.lastCollisionFlags, Is.EqualTo(CollisionFlags.None));
            Assert.That(origin.position, Is.EqualTo(expectedOriginPosition).Using(Vector3ComparerWithEqualsOperator.Instance));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Move camera back to origin at height of 2 meters
            cameraTransform.localPosition = new Vector3(0f, 2f, 0f);
            expectedCapsuleHeight = 2f;
            expectedCapsuleCenter = new Vector3(0f, 1f + characterController.skinWidth, 0f);

            // Now test with collision
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            cube.gameObject.AddComponent<Rigidbody>();
            cube.position = origin.position + motion + Vector3.up;
            cube.rotation = origin.rotation;
            cube.localScale = new Vector3(0.5f, 2f, 0.5f);

            // Wait for physics to update after adding rigidbody
            yield return new WaitForFixedUpdate();
            yield return null;

            characterControllerManipulator.MoveBody(motion);
            Assert.That(characterController.height, Is.EqualTo(expectedCapsuleHeight).Using(FloatEqualityComparer.Instance));
            Assert.That(characterController.center, Is.EqualTo(expectedCapsuleCenter).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(characterController.collisionFlags.HasFlag(CollisionFlags.Sides));

            yield return new WaitForFixedUpdate();
            yield return null;

            Object.Destroy(characterControllerManipulator);
            Object.Destroy(bodyPositionEvaluator);
        }
    }
}
