using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class XRBodyTransformerTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator TransformationsAreAppliedInQueuePriorityOrder()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            // Order of transformations:
            // 1. (priority -1) Move origin forward 1 meter
            // 2. (priority 0) Rotate origin yaw 90 degrees
            // 3. (priority 0) Move origin forward 1 meter
            // 4. (priority int.MaxValue) Rotate origin pitch 90 degrees
            var transformation1 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Translate(Vector3.forward); });
            var transformation2 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Rotate(Vector3.up, 90f); });
            var transformation3 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Translate(Vector3.forward); });
            var transformation4 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Rotate(Vector3.right, 90f); });
            var expectedPosition = new Vector3(1f, 0f, 1f);
            var expectedRotation = Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(90f, Vector3.right);

            // Queue in decreasing order to test that priority value is respected
            xrBodyTransformer.QueueTransformation(transformation4, int.MaxValue);
            xrBodyTransformer.QueueTransformation(transformation2);
            xrBodyTransformer.QueueTransformation(transformation3);
            xrBodyTransformer.QueueTransformation(transformation1, -1);

            yield return new WaitForFixedUpdate();
            yield return null;

            var originTransform = xrOrigin.transform;
            Assert.That(originTransform.position, Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(originTransform.rotation, Is.EqualTo(expectedRotation).Using(QuaternionEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator TransformationsWithSamePriorityAreAppliedInQueueOrder()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            // Order of transformations:
            // 1. (priority -1) Move origin forward 1 meter
            // 2. (priority 0) Rotate origin yaw 90 degrees
            // 3. (priority 0) Move origin forward 1 meter
            // 4. (priority 0) Rotate origin pitch 90 degrees
            var transformation1 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Translate(Vector3.forward); });
            var transformation2 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Rotate(Vector3.up, 90f); });
            var transformation3 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Translate(Vector3.forward); });
            var transformation4 = new DelegateXRBodyTransformation(body => { body.xrOrigin.transform.Rotate(Vector3.right, 90f); });
            var expectedPosition = new Vector3(1f, 0f, 1f);
            var expectedRotation = Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(90f, Vector3.right);

            xrBodyTransformer.QueueTransformation(transformation2);
            xrBodyTransformer.QueueTransformation(transformation3);
            xrBodyTransformer.QueueTransformation(transformation1, -1);
            xrBodyTransformer.QueueTransformation(transformation4);

            yield return new WaitForFixedUpdate();
            yield return null;

            var originTransform = xrOrigin.transform;
            Assert.That(originTransform.position, Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(originTransform.rotation, Is.EqualTo(expectedRotation).Using(QuaternionEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator QueuedTransformationsAreOnlyAppliedForOneFrame()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            var m_Transformation1Applied = false;
            var m_Transformation2Applied = false;
            var m_Transformation3Applied = false;
            var transformation1 = new DelegateXRBodyTransformation(body => { m_Transformation1Applied = true; });
            var transformation2 = new DelegateXRBodyTransformation(body => { m_Transformation2Applied = true; });
            var transformation3 = new DelegateXRBodyTransformation(body => { m_Transformation3Applied = true; });

            xrBodyTransformer.QueueTransformation(transformation1);
            xrBodyTransformer.QueueTransformation(transformation2);
            xrBodyTransformer.QueueTransformation(transformation3);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsTrue(m_Transformation1Applied);
            Assert.IsTrue(m_Transformation2Applied);
            Assert.IsTrue(m_Transformation3Applied);

            m_Transformation1Applied = false;
            m_Transformation2Applied = false;
            m_Transformation3Applied = false;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsFalse(m_Transformation1Applied);
            Assert.IsFalse(m_Transformation2Applied);
            Assert.IsFalse(m_Transformation3Applied);
        }

        [UnityTest]
        public IEnumerator ConstrainedBodyManipulatorLinksToBody()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var characterController = xrOrigin.Origin.AddComponent<CharacterController>();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            // Manipulator should link to body when it is assigned to body transformer
            var characterControllerManipulator = ScriptableObject.CreateInstance<CharacterControllerBodyManipulator>();
            xrBodyTransformer.constrainedBodyManipulator = characterControllerManipulator;
            Assert.That(characterControllerManipulator.linkedBody.xrOrigin, Is.EqualTo(xrOrigin));
            Assert.That(characterControllerManipulator.linkedBody.bodyPositionEvaluator, Is.EqualTo(xrBodyTransformer.bodyPositionEvaluator));
            Assert.That(characterControllerManipulator.characterController, Is.EqualTo(characterController));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Should unlink when body transformer is disabled
            xrBodyTransformer.enabled = false;
            Assert.That(characterControllerManipulator.linkedBody, Is.Null);

            Object.Destroy(characterControllerManipulator);
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_DelegateXRBodyTransformation()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            var transformationApplied = false;
            var transformation = new DelegateXRBodyTransformation(body => { transformationApplied = true; });
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsTrue(transformationApplied);
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_OriginMovement()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            var originTransform = xrOrigin.Origin.transform;
            var translation = new Vector3(1.5f, 2.5f, 3.5f);
            var transformation = new XROriginMovement { motion = translation, forceUnconstrained = true, };
            var expectedPosition = originTransform.position + translation;
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(originTransform.position, Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_ConstrainedOriginMovement()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            // Test without constrained manipulator
            var motion = Vector3.forward;
            var transformation = new XROriginMovement { motion = motion, forceUnconstrained = false, };
            xrBodyTransformer.QueueTransformation(transformation);
            var origin = xrOrigin.Origin.transform;
            var expectedPosition = origin.position + motion;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(origin.position, Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance));

            // Test with character controller without collision
            var characterController = xrOrigin.Origin.AddComponent<CharacterController>();
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.height = 2f;
            characterController.radius = 0.5f;
            var characterControllerManipulator = ScriptableObject.CreateInstance<CharacterControllerBodyManipulator>();
            xrBodyTransformer.constrainedBodyManipulator = characterControllerManipulator;
            xrBodyTransformer.QueueTransformation(transformation);
            expectedPosition = origin.position + motion;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(origin.position, Is.EqualTo(expectedPosition).Using(Vector3ComparerWithEqualsOperator.Instance));

            // Test with character controller with collision
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            cube.gameObject.AddComponent<Rigidbody>();
            cube.position = origin.position + motion + Vector3.up;
            cube.rotation = origin.rotation;
            cube.localScale = new Vector3(0.5f, 2f, 0.5f);

            // Wait for physics to update after adding rigidbody
            yield return new WaitForFixedUpdate();
            yield return null;

            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(characterController.collisionFlags.HasFlag(CollisionFlags.Sides));
            Assert.That(characterControllerManipulator.lastCollisionFlags.HasFlag(CollisionFlags.Sides));

            Object.Destroy(characterControllerManipulator);
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_BodyGroundPositioning()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);
            var bodyPositionEvaluator = xrBodyTransformer.bodyPositionEvaluator;

            // Make sure body starts out offset from origin
            xrOrigin.Camera.transform.localPosition = Vector3.forward;
            Assert.That(bodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin), Is.Not.EqualTo(Vector3.zero).Using(Vector3ComparerWithEqualsOperator.Instance));

            var targetPosition = new Vector3(1.5f, 2.5f, 3.5f);
            var transformation = new XRBodyGroundPosition { targetPosition = targetPosition };
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin), Is.EqualTo(targetPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_OriginUpAlignment()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            var targetUp = new Vector3(1.5f, 2.5f, 3.5f);
            var transformation = new XROriginUpAlignment { targetUp = targetUp };
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(xrOrigin.Origin.transform.up, Is.EqualTo(targetUp.normalized).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_BodyYawRotation()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);
            var bodyPositionEvaluator = xrBodyTransformer.bodyPositionEvaluator;

            // Make sure body starts out with some non-identity rotation and is offset from the origin
            xrOrigin.Camera.transform.localPosition = new Vector3(1.5f, 2.5f, 3.5f);
            Assert.That(bodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin), Is.Not.EqualTo(Vector3.zero).Using(Vector3ComparerWithEqualsOperator.Instance));
            var initialRotation = Quaternion.Euler(30f, 60f, 90f);
            var origin = xrOrigin.Origin.transform;
            origin.localRotation = initialRotation;
            var initialBodyGroundPosition = bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin);

            var angleDelta = 123f;
            var expectedRotation = initialRotation * Quaternion.AngleAxis(angleDelta, Vector3.up);
            var transformation = new XRBodyYawRotation { angleDelta = angleDelta };
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(origin.localRotation, Is.EqualTo(expectedRotation).Using(QuaternionEqualityComparer.Instance));
            Assert.That(bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin), Is.EqualTo(initialBodyGroundPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_CameraForwardXZAlignment()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);

            // Make sure origin and camera start out with non-identity rotations and camera is offset from the origin
            var cameraTransform = xrOrigin.Camera.transform;
            cameraTransform.localPosition = new Vector3(1.5f, 2.5f, 3.5f);
            cameraTransform.localRotation = Quaternion.Euler(30f, 60f, 90f);
            var origin = xrOrigin.Origin.transform;
            origin.localRotation = Quaternion.Euler(30f, 30f, 0f);
            var initialCameraPosition = cameraTransform.position;

            var targetDirection = Vector3.forward;
            var transformation = new XRCameraForwardXZAlignment { targetDirection = targetDirection };
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            var projectedCamForward = Vector3.ProjectOnPlane(cameraTransform.forward, origin.up).normalized;
            var projectedTargetForward = Vector3.ProjectOnPlane(targetDirection, origin.up).normalized;
            Assert.That(projectedCamForward, Is.EqualTo(projectedTargetForward).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(cameraTransform.position, Is.EqualTo(initialCameraPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }

        [UnityTest]
        public IEnumerator ApplyTransformation_BodyScaling()
        {
            var xrOrigin = TestUtilities.CreateXROrigin();
            var xrBodyTransformer = TestUtilities.CreateXRBodyTransformer(xrOrigin);
            var bodyPositionEvaluator = xrBodyTransformer.bodyPositionEvaluator;

            // Make sure body starts out offset from origin
            xrOrigin.Camera.transform.localPosition = new Vector3(1.5f, 2.5f, 3.5f);
            Assert.That(bodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin), Is.Not.EqualTo(Vector3.zero).Using(Vector3ComparerWithEqualsOperator.Instance));

            var initialBodyGroundPosition = bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin);
            var scale = 123f;
            var transformation = new XRBodyScale { uniformScale = scale };
            xrBodyTransformer.QueueTransformation(transformation);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(xrOrigin.Origin.transform.localScale, Is.EqualTo(Vector3.one * scale).Using(Vector3ComparerWithEqualsOperator.Instance));
            var pos = bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin);
            Assert.That(bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin), Is.EqualTo(initialBodyGroundPosition).Using(Vector3ComparerWithEqualsOperator.Instance));
        }
    }
}