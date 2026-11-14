using System.Collections;
using NUnit.Framework;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InputReaderTests : InputTestFixture
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            TestUtilities.DisableAllInputSystemActions();
        }

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

        const float k_ScrollAmount = 300f;
        const float k_ScrollDeltaMultiplier = 5f;
        const float k_ScrollTolerance = 0.0001f;

        [TestCase(0f, 1f, k_ScrollDeltaMultiplier)]
        [TestCase(0f, -1f, k_ScrollDeltaMultiplier)]
        [TestCase(1f, 0f, 2f)] // A different input module multiplier still lands on scrollAmount pixels.
        [TestCase(-1f, 0f, k_ScrollDeltaMultiplier)]
        [TestCase(0.2f, 0f, k_ScrollDeltaMultiplier)] // A partial input is normalized, so it still scrolls the full amount.
        [TestCase(0.5f, 0.5f, k_ScrollDeltaMultiplier)] // Diagonal.
        [TestCase(0f, 1f, -k_ScrollDeltaMultiplier)] // A negative multiplier inverts scrolling in the UI layer, so the output keeps the pressed direction.
        public void PressScrollInputScrollsAmountInPressedDirection(float x, float y, float multiplier)
        {
            var scrollInput = CreatePressScrollInput();
            var direction = new Vector2(x, y);
            scrollInput.scrollInput.manualValue = direction;

            scrollInput.ProcessScroll(multiplier);

            // The UI layer multiplies the output by the multiplier's magnitude, landing on scrollAmount pixels in the
            // pressed direction whatever the input's magnitude or the multiplier's sign.
            var expected = direction.normalized * (k_ScrollAmount / Mathf.Abs(multiplier));
            Assert.That(scrollInput.ReadValue(), Is.EqualTo(expected).Using(Vector2EqualityComparer.Instance));
        }

        [Test]
        public void PressScrollInputScrollsOncePerPress()
        {
            var gamepad = InputSystem.InputSystem.AddDevice<Gamepad>();
            var action = new InputAction(type: InputActionType.Value, expectedControlType: "Vector2");
            action.AddCompositeBinding("2DVector")
                .With("Up", "<Gamepad>/dpad/up")
                .With("Down", "<Gamepad>/dpad/down")
                .With("Left", "<Gamepad>/dpad/left")
                .With("Right", "<Gamepad>/dpad/right");

            var scrollInput = CreatePressScrollInput();
            scrollInput.scrollInput = new XRInputValueReader<Vector2>("Scroll", XRInputValueReader.InputSourceMode.InputAction)
            {
                inputAction = action,
            };

            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.TryReadValue(out _), Is.False, "Nothing scrolls before a press.");

            Press(gamepad.dpad.up);
            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.TryReadValue(out var pressValue), Is.True);
            Assert.That(pressValue.y * k_ScrollDeltaMultiplier, Is.EqualTo(k_ScrollAmount).Within(k_ScrollTolerance));

            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.ReadValue(), Is.EqualTo(Vector2.zero), "Holding a press does not repeat the scroll.");

            Release(gamepad.dpad.up);
            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.ReadValue(), Is.EqualTo(Vector2.zero), "Releasing a press does not scroll.");

            Press(gamepad.dpad.right);
            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.ReadValue().x * k_ScrollDeltaMultiplier, Is.EqualTo(k_ScrollAmount).Within(k_ScrollTolerance), "The next press scrolls again.");
        }

        [Test]
        public void PressScrollInputZeroMultiplierScrollsNothing()
        {
            var scrollInput = CreatePressScrollInput();
            scrollInput.scrollInput.manualValue = Vector2.up;

            // The output is divided by the multiplier, so a multiplier of 0 must not divide by zero.
            scrollInput.ProcessScroll(0f);

            Assert.That(scrollInput.TryReadValue(out var value), Is.False);
            Assert.That(value, Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void PressScrollInputScrollsAgainAfterReenabling()
        {
            var scrollInput = CreatePressScrollInput();
            scrollInput.scrollInput.manualValue = Vector2.up;
            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);

            scrollInput.enabled = false;
            Assert.That(scrollInput.ReadValue(), Is.EqualTo(Vector2.zero));

            // Still held after re-enabling counts as a new press.
            scrollInput.enabled = true;
            scrollInput.ProcessScroll(k_ScrollDeltaMultiplier);
            Assert.That(scrollInput.ReadValue(), Is.Not.EqualTo(Vector2.zero));
        }

        [UnityTest]
        public IEnumerator PressScrollInputUsesInputModuleMultiplier([Values] bool withInputModule)
        {
            var multiplier = UIInputModule.k_DefaultTrackedScrollDeltaMultiplier;
            if (withInputModule)
            {
                var inputModule = new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule)).GetComponent<XRUIInputModule>();
                inputModule.trackedScrollDeltaMultiplier = 10f;
                multiplier = inputModule.trackedScrollDeltaMultiplier;
                yield return null;
            }

            var scrollInput = CreatePressScrollInput();
            scrollInput.scrollInput.manualValue = Vector2.up;
            yield return null;

            Assert.That(scrollInput.ReadValue().y * multiplier, Is.EqualTo(k_ScrollAmount).Within(k_ScrollTolerance));
        }

        static PressScrollInput CreatePressScrollInput()
        {
            var scrollInput = new GameObject("Press Scroll Input").AddComponent<PressScrollInput>();
            scrollInput.scrollInput = new XRInputValueReader<Vector2>("Scroll", XRInputValueReader.InputSourceMode.ManualValue);
            scrollInput.scrollAmount = k_ScrollAmount;
            return scrollInput;
        }
    }
}
