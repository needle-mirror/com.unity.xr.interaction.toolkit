using System.Collections;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class FallbackCompositeInputTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            InputSystem.InputSystem.RegisterBindingComposite<Vector3FallbackComposite>();
            InputSystem.InputSystem.RegisterBindingComposite<QuaternionFallbackComposite>();
            InputSystem.InputSystem.RegisterBindingComposite<IntegerFallbackComposite>();
            InputSystem.InputSystem.RegisterBindingComposite<ButtonFallbackComposite>();
        }

        public override void TearDown()
        {
            base.TearDown();
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator FallbackCompositeIsSupportedByTrackedPoseDriver()
        {
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            var actionMap = asset.AddActionMap("Main");
            var positionAction = actionMap.AddAction("Position");
            positionAction.AddCompositeBinding("Vector3Fallback")
                .With("first", "<TrackedDevice>{LeftHand}/devicePosition")
                .With("second", "<TrackedDevice>{RightHand}/devicePosition");
            var rotationAction = actionMap.AddAction("Rotation");
            rotationAction.AddCompositeBinding("QuaternionFallback")
                .With("first", "<TrackedDevice>{LeftHand}/deviceRotation")
                .With("second", "<TrackedDevice>{RightHand}/deviceRotation");
            var isTrackedAction = actionMap.AddAction("Is Tracked", InputActionType.Button);
            isTrackedAction.wantsInitialStateCheck = true;
            isTrackedAction.AddCompositeBinding("ButtonFallback")
                .With("first", "<TrackedDevice>{LeftHand}/isTracked")
                .With("second", "<TrackedDevice>{RightHand}/isTracked");
            var trackingStateAction = actionMap.AddAction("Tracking State");
            trackingStateAction.AddCompositeBinding("IntegerFallback")
                .With("first", "<TrackedDevice>{LeftHand}/trackingState")
                .With("second", "<TrackedDevice>{RightHand}/trackingState");

            actionMap.Enable();

            var controllerGameObject = new GameObject("Action Based Controller");
            var transform = controllerGameObject.transform;
            var trackedPoseDriver = controllerGameObject.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.positionInput = new InputActionProperty(positionAction);
            trackedPoseDriver.rotationInput = new InputActionProperty(rotationAction);
            trackedPoseDriver.trackingStateInput = new InputActionProperty(trackingStateAction);

            // Add a device that will be resolved in the second part of the fallback composite binding
            var secondDevice = InputSystem.InputSystem.AddDevice<TrackedDevice>();
            InputSystem.InputSystem.SetDeviceUsage(secondDevice, InputSystem.CommonUsages.RightHand);

            var firstPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(0f, 45f, 0f));
            var secondPose = new Pose(new Vector3(4f, 5f, 6f), Quaternion.Euler(30f, 60f, 90f));
            const InputTrackingState firstTrackingState = InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.Velocity;
            const InputTrackingState secondTrackingState = InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.AngularVelocity;

            Set(secondDevice.devicePosition, secondPose.position);
            Set(secondDevice.deviceRotation, secondPose.rotation);
            Set(secondDevice.isTracked, 0f);
            Set(secondDevice.trackingState, (int)secondTrackingState);

            yield return null;

            Assert.That(positionAction.ReadValue<Vector3>(), Is.EqualTo(secondPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(rotationAction.ReadValue<Quaternion>(), Is.EqualTo(secondPose.rotation).Using(QuaternionEqualityComparer.Instance));
            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(trackingStateAction.ReadValue<int>(), Is.EqualTo((int)secondTrackingState));
            Assert.That(positionAction.activeControl, Is.EqualTo(secondDevice.devicePosition));
            Assert.That(rotationAction.activeControl, Is.EqualTo(secondDevice.deviceRotation));
            Assert.That(isTrackedAction.activeControl, Is.Null);
            Assert.That(trackingStateAction.activeControl, Is.EqualTo(secondDevice.trackingState));

            Assert.That(transform.localPosition, Is.EqualTo(secondPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(transform.localRotation, Is.EqualTo(secondPose.rotation).Using(QuaternionEqualityComparer.Instance));

            // Extra verification that Is Tracked works like a button and has an active control when true
            Set(secondDevice.isTracked, 1f);

            yield return null;

            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(1f));
            Assert.That(isTrackedAction.activeControl, Is.EqualTo(secondDevice.isTracked));

            // Add a device that will be resolved in the first part of the fallback composite binding
            var firstDevice = InputSystem.InputSystem.AddDevice<TrackedDevice>();
            InputSystem.InputSystem.SetDeviceUsage(firstDevice, InputSystem.CommonUsages.LeftHand);

            Set(firstDevice.devicePosition, firstPose.position);
            Set(firstDevice.deviceRotation, firstPose.rotation);
            Set(firstDevice.isTracked, 0f);
            Set(firstDevice.trackingState, (int)firstTrackingState);

            yield return null;

            Assert.That(positionAction.ReadValue<Vector3>(), Is.EqualTo(firstPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(rotationAction.ReadValue<Quaternion>(), Is.EqualTo(firstPose.rotation).Using(QuaternionEqualityComparer.Instance));
            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(trackingStateAction.ReadValue<int>(), Is.EqualTo((int)firstTrackingState));
            Assert.That(positionAction.activeControl, Is.EqualTo(firstDevice.devicePosition));
            Assert.That(rotationAction.activeControl, Is.EqualTo(firstDevice.deviceRotation));
            Assert.That(isTrackedAction.activeControl, Is.Null);
            Assert.That(trackingStateAction.activeControl, Is.EqualTo(firstDevice.trackingState));

            Assert.That(transform.localPosition, Is.EqualTo(firstPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(transform.localRotation, Is.EqualTo(firstPose.rotation).Using(QuaternionEqualityComparer.Instance));

            // Extra verification that Is Tracked works like a button and has an active control when true
            Set(firstDevice.isTracked, 1f);

            yield return null;

            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(1f));
            Assert.That(isTrackedAction.activeControl, Is.EqualTo(firstDevice.isTracked));

            // Remove the first device, the actions should start falling back again
            InputSystem.InputSystem.RemoveDevice(firstDevice);

            yield return null;

            Assert.That(positionAction.ReadValue<Vector3>(), Is.EqualTo(secondPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(rotationAction.ReadValue<Quaternion>(), Is.EqualTo(secondPose.rotation).Using(QuaternionEqualityComparer.Instance));
            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(1f));
            Assert.That(trackingStateAction.ReadValue<int>(), Is.EqualTo((int)secondTrackingState));
            Assert.That(positionAction.activeControl, Is.EqualTo(secondDevice.devicePosition));
            Assert.That(rotationAction.activeControl, Is.EqualTo(secondDevice.deviceRotation));
            Assert.That(isTrackedAction.activeControl, Is.EqualTo(secondDevice.isTracked));
            Assert.That(trackingStateAction.activeControl, Is.EqualTo(secondDevice.trackingState));

            Assert.That(transform.localPosition, Is.EqualTo(secondPose.position).Using(Vector3ComparerWithEqualsOperator.Instance));
            Assert.That(transform.localRotation, Is.EqualTo(secondPose.rotation).Using(QuaternionEqualityComparer.Instance));

            // Extra verification that Is Tracked works like a button and has no active control when false
            Set(secondDevice.isTracked, 0f);

            yield return null;

            Assert.That(isTrackedAction.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(isTrackedAction.activeControl, Is.Null);
        }
    }
}
