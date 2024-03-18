using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InputReaderTests : InputTestFixture
    {
        [TearDown]
        public override void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
            base.TearDown();
        }

        [TestCase(InputActionType.Value)]
        [TestCase(InputActionType.Value, "Scale(factor=2)")]
        [TestCase(InputActionType.Value, "Scale(factor=2)", true)]
        [TestCase(InputActionType.Value, "AxisDeadzone(min=0.125,max=0.925)")]
        [TestCase(InputActionType.Value, "AxisDeadzone(min=0.125,max=0.925)", true)]
        [TestCase(InputActionType.Button)]
        [TestCase(InputActionType.Button, "Scale(factor=2)")]
        [TestCase(InputActionType.Button, "Scale(factor=2)", true)]
        [TestCase(InputActionType.Button, "AxisDeadzone(min=0.125,max=0.925)")]
        [TestCase(InputActionType.Button, "AxisDeadzone(min=0.125,max=0.925)", true)]
        public void ButtonReaderReadValueFromAxisControl(InputActionType actionType, string processors = null, bool processorOnBinding = false)
        {
            var gamepad = InputSystem.InputSystem.AddDevice<Gamepad>();

            InputAction action;
            const string bindingPath = "<Gamepad>/leftTrigger";
            if (processorOnBinding)
            {
                action = new InputAction(type: actionType);
                action.AddBinding(bindingPath, processors: processors);
            }
            else
            {
                action = new InputAction(type: actionType, binding: bindingPath, processors: processors);
            }

            var reader = new XRInputButtonReader("Select", inputSourceMode: XRInputButtonReader.InputSourceMode.InputAction)
            {
                inputActionValue = action,
            };

            DriveAxisControl(gamepad, reader, action, processors);
        }

        [TestCase(InputActionType.Value)]
        [TestCase(InputActionType.Value, "ScaleVector2(x=2,y=2)")]
        [TestCase(InputActionType.Value, "ScaleVector2(x=2,y=2)", true)]
        [TestCase(InputActionType.Button)]
        [TestCase(InputActionType.Button, "ScaleVector2(x=2,y=2)")]
        [TestCase(InputActionType.Button, "ScaleVector2(x=2,y=2)", true)]
        public void ButtonReaderReadValueFromStickControl(InputActionType actionType, string processors = null, bool processorOnBinding = false)
        {
            var gamepad = InputSystem.InputSystem.AddDevice<Gamepad>();

            InputAction action;
            const string bindingPath = "<Gamepad>/leftStick";
            if (processorOnBinding)
            {
                action = new InputAction(type: actionType);
                action.AddBinding(bindingPath, processors: processors);
            }
            else
            {
                action = new InputAction(type: actionType, binding: bindingPath, processors: processors);
            }

            var reader = new XRInputButtonReader("Select", inputSourceMode: XRInputButtonReader.InputSourceMode.InputAction)
            {
                inputActionValue = action,
            };

            DriveStickControl(gamepad, reader, action, processors);
        }

        [TestCase(InputActionType.Value)]
        [TestCase(InputActionType.Value, "Scale(factor=2)", "ScaleVector2(x=2,y=2)")]
        [TestCase(InputActionType.Value, "AxisDeadzone(min=0.125,max=0.925)", "ScaleVector2(x=2,y=2)")]
        [TestCase(InputActionType.Button)]
        [TestCase(InputActionType.Button, "Scale(factor=2)", "ScaleVector2(x=2,y=2)")]
        [TestCase(InputActionType.Button, "AxisDeadzone(min=0.125,max=0.925)", "ScaleVector2(x=2,y=2)")]
        public void ButtonReaderReadValueFromMixedControl(InputActionType actionType, string axisProcessors = null, string stickProcessors = null)
        {
            var gamepad = InputSystem.InputSystem.AddDevice<Gamepad>();

            var action = new InputAction(type: actionType);
            action.AddBinding("<Gamepad>/leftTrigger", processors: axisProcessors);
            action.AddBinding("<Gamepad>/leftStick", processors: stickProcessors);

            var reader = new XRInputButtonReader("Select", inputSourceMode: XRInputButtonReader.InputSourceMode.InputAction)
            {
                inputActionValue = action,
            };

            // Test that the reader can switch between reading as either a float or Vector2, in both directions.
            DriveAxisControl(gamepad, reader, action, axisProcessors);
            DriveStickControl(gamepad, reader, action, stickProcessors);
            DriveAxisControl(gamepad, reader, action, axisProcessors);
        }

        void DriveAxisControl(Gamepad gamepad, XRInputButtonReader reader, InputAction action, string processors)
        {
            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Disabled));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            action.Enable();

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Waiting));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            Set(gamepad.leftTrigger, 0.234f);

            var controlValue = 0.234f;
            var expectedValue = controlValue;
            if (processors != null && processors.StartsWith("Scale"))
                expectedValue *= 2f;
            else if (processors != null && processors.StartsWith("AxisDeadzone"))
                expectedValue = new AxisDeadzoneProcessor().Process(expectedValue);

            Assert.That(action.activeControl, Is.SameAs(gamepad.leftTrigger));
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Started));
            Assert.That(action.activeValueType, Is.EqualTo(typeof(float)));
            Assert.That(action.ReadValue<float>(), Is.EqualTo(expectedValue).Within(0.00001));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(controlValue).Within(0.00001));
            Assert.That(reader.ReadValue(), Is.EqualTo(expectedValue).Within(0.00001));

            Set(gamepad.leftTrigger, 0f);

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Waiting));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            action.Disable();

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Disabled));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<float>(), Is.EqualTo(0f));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));
        }

        void DriveStickControl(Gamepad gamepad, XRInputButtonReader reader, InputAction action, string processors)
        {
            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Disabled));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            action.Enable();

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Waiting));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            Set(gamepad.leftStick, new Vector2(0.123f, 0.234f));

            // The leftStick type is StickControl, so it needs to be filtered
            var controlValue = new StickDeadzoneProcessor().Process(new Vector2(0.123f, 0.234f));
            var expectedValue = controlValue;
            if (processors != null && processors.StartsWith("Scale"))
                expectedValue *= 2f;

            Assert.That(action.activeControl, Is.SameAs(gamepad.leftStick));
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Started));
            Assert.That(action.activeValueType, Is.EqualTo(typeof(Vector2)));
            Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(expectedValue).Using(Vector2EqualityComparer.Instance));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(controlValue.magnitude).Within(0.00001));
            Assert.That(reader.ReadValue(), Is.EqualTo(expectedValue.magnitude).Within(0.00001));

            Set(gamepad.leftStick, Vector2.zero);

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Waiting));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));

            action.Disable();

            Assert.That(action.activeControl, Is.Null);
            Assert.That(action.phase, Is.EqualTo(InputActionPhase.Disabled));
            Assert.That(action.activeValueType, Is.Null);
            Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
            Assert.That(action.GetControlMagnitude(), Is.EqualTo(0f));
            Assert.That(reader.ReadValue(), Is.EqualTo(0f));
        }
    }
}
