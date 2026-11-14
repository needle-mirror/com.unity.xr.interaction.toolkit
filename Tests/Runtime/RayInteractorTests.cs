using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class RayInteractorTests
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static readonly Type[] s_RayInteractorTypes =
        {
            typeof(XRRayInteractor),
            typeof(XRGazeInteractor),
        };

        static readonly XRRayInteractor.LineType[] s_LineTypes =
        {
            XRRayInteractor.LineType.StraightLine,
            XRRayInteractor.LineType.ProjectileCurve,
            XRRayInteractor.LineType.BezierCurve,
        };

        static readonly XRBaseInputInteractor.InputTriggerType[] s_SelectActionTriggers =
        {
            XRBaseInputInteractor.InputTriggerType.State,
            XRBaseInputInteractor.InputTriggerType.StateChange,
            XRBaseInputInteractor.InputTriggerType.Toggle,
            XRBaseInputInteractor.InputTriggerType.Sticky,
        };

#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
        UnityEngine.UIElements.PanelSettings m_PanelSettings;
#endif

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
            if (m_PanelSettings != null)
                Object.DestroyImmediate(m_PanelSettings);

            TestUtilities.RestoreUIToolkitTrackedInput();
#endif
        }

        const float k_MomentumScrollDistance = 300f;
        const float k_MomentumTolerance = 0.5f;
        const int k_MaxHoverFrames = 600;

        // Slow deceleration so a momentum scroll is still scrolling when the test presses.
        const float k_SlowDecelerationRate = 0.5f;

        // Interactor scroll input that scrolls k_MomentumScrollDistance pixels down at the default tracked scroll delta multiplier.
        static readonly Vector2 k_MomentumScrollInput = new Vector2(0f, -k_MomentumScrollDistance / UIInputModule.k_DefaultTrackedScrollDeltaMultiplier);

        [UnityTest]
        public IEnumerator ScrollRectMomentumContinuesUntilPressed()
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXRUIInputModule();
            var camera = new GameObject("Camera").AddComponent<Camera>();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            var recorder = TestUtilities.CreateScrollRectCanvas(camera, interactor.transform, k_SlowDecelerationRate);
            EnableUIScrollMomentum(interactor);

            // Wait for the ray to hover the canvas.
            yield return new WaitForFixedUpdate();
            yield return null;
            yield return null;

            interactor.uiScrollInput.manualValue = k_MomentumScrollInput;
            yield return null;
            interactor.uiScrollInput.manualValue = Vector2.zero;
            yield return null;

            Assert.That(recorder.total.y, Is.LessThan(0f).And.GreaterThan(-k_MomentumScrollDistance + k_MomentumTolerance), "The scroll should still be scrolling.");

            // Press the UI mid-scroll, the same way a trigger press does.
            interactor.uiPressInput.QueueManualState(true, 1f);
            yield return null;
            var deliveredAtPress = recorder.deltas.Count;
            yield return null;

            Assert.That(recorder.deltas.Count, Is.EqualTo(deliveredAtPress), "A press stops the momentum.");
            Assert.That(recorder.total.y, Is.GreaterThan(-k_MomentumScrollDistance + k_MomentumTolerance));
        }

#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_3_OR_NEWER
        [UnityTest]
        public IEnumerator UIToolkitMomentumContinuesUntilPressed()
        {
            TestUtilities.ForceUIToolkitTrackedInput();

            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateUIToolkitInputSetup();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            EnableUIScrollMomentum(interactor);
            var scrollView = TestUtilities.CreateWorldSpaceScrollView(out m_PanelSettings);
            scrollView.scrollDecelerationRate = k_SlowDecelerationRate;

            // Wait for UI Toolkit to report the hover, which the handler uses to find the ScrollView.
            var hovering = false;
            interactor.uiHoverEntered.AddListener(_ => hovering = true);
            for (var i = 0; i < k_MaxHoverFrames && !hovering; ++i)
                yield return null;

            Assert.That(hovering, Is.True, "The interactor should hover the panel.");

            var startOffset = scrollView.scrollOffset.y + k_MomentumScrollDistance;
            scrollView.scrollOffset = new Vector2(0f, startOffset);

            interactor.uiScrollInput.manualValue = k_MomentumScrollInput;
            yield return null;
            interactor.uiScrollInput.manualValue = Vector2.zero;
            yield return null;

            var moved = scrollView.scrollOffset.y - startOffset;
            Assert.That(moved, Is.GreaterThan(0f).And.LessThan(k_MomentumScrollDistance - k_MomentumTolerance), "The scroll should still be scrolling.");

            // Press the UI mid-scroll, the same way a trigger press does. UI Toolkit receives the press through its own event processing.
            interactor.uiPressInput.QueueManualState(true, 1f);
            yield return null;
            var offsetAtPress = scrollView.scrollOffset.y;
            yield return null;

            Assert.That(scrollView.scrollOffset.y, Is.EqualTo(offsetAtPress), "A press stops the momentum.");
            Assert.That(scrollView.scrollOffset.y - startOffset, Is.LessThan(k_MomentumScrollDistance - k_MomentumTolerance));
        }
#endif

        static void EnableUIScrollMomentum(XRRayInteractor interactor)
        {
            interactor.enableUIInteraction = true;
            interactor.uiScrollMomentum = true;
            interactor.uiScrollInput = new XRInputValueReader<Vector2>("UI Scroll", XRInputValueReader.InputSourceMode.ManualValue);
            interactor.uiPressInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
        }

        [UnityTest]
        public IEnumerator RayInteractorCanHoverInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator RayInteractorValidTargetsListEmptyWhenInteractorDisabled()
        {
            // This tests that the ray interactor will return an empty list of valid
            // targets when the interactor component or the GameObject is disabled and
            // will correctly add the target back into the list upon enabling the interactor
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Disable interactor GameObject
            interactor.gameObject.SetActive(false);

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Enable interactor GameObject
            interactor.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Disable interactor component
            interactor.enabled = false;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Enable interactor component
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator RayInteractorValidTargetsRemainEmptyWhenInteractorEnabledWithNoRayHit()
        {
            // This tests that the ray interactor will return an empty list of valid
            // targets when the interactor component or the GameObject is disabled
            // while it has a valid target and the valid targets will remain empty
            // when the interactor is enabled again facing away from the interactable.
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Disable interactor GameObject
            interactor.gameObject.SetActive(false);

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Face interactor away from valid target and enable interactor GameObject
            interactor.transform.forward = Vector3.back;
            interactor.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Face interactor towards the valid target
            interactor.transform.forward = Vector3.forward;
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));

            // Disable interactor component
            interactor.enabled = false;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Face interactor away from valid target and enable interactor
            interactor.transform.forward = Vector3.back;
            interactor.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.Empty);
            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Face interactor towards the valid target
            interactor.transform.forward = Vector3.forward;
            yield return new WaitForFixedUpdate();
            yield return null;

            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator RayInteractorCanAutoDeselect([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = CreateRayInteractor(rayInteractorType);
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            interactor.hoverToSelect = true;
            interactor.hoverTimeToSelect = 0.1f;
            interactor.autoDeselect = true;
            interactor.timeToAutoDeselect = 0.1f;
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.allowGazeInteraction = true;
            interactable.allowGazeSelect = true;
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(0.1f);

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            // Disable hover to select to ensure it doesn't reselect after auto deselect
            interactor.hoverToSelect = false;

            yield return new WaitForSeconds(0.15f);

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);
        }

        [UnityTest]
        public IEnumerator RayInteractorCanSelectInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.interactablesSelected, Is.Empty);

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;

            yield return new WaitForSeconds(0.1f);
            // The recorder applies the press on its second Update; a single slow frame can satisfy the wait before that.
            yield return null;

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        [UnityTest]
        public IEnumerator RayInteractorCanSelectAndReleaseInteractable([ValueSource(nameof(s_SelectActionTriggers))] XRBaseInputInteractor.InputTriggerType selectActionTrigger)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            interactor.selectActionTrigger = selectActionTrigger;
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            // Press Grip
            interactor.selectInput.QueueManualState(true, 1f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.True);
            Assert.That(interactor.selectInput.ReadWasPerformedThisFrame(), Is.True);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            // Release Grip
            interactor.selectInput.QueueManualState(false, 0f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            switch (selectActionTrigger)
            {
                case XRBaseInputInteractor.InputTriggerType.State:
                case XRBaseInputInteractor.InputTriggerType.StateChange:
                    Assert.That(interactable.isSelected, Is.False);
                    Assert.That(interactor.isSelectActive, Is.False);
                    break;
                case XRBaseInputInteractor.InputTriggerType.Toggle:
                case XRBaseInputInteractor.InputTriggerType.Sticky:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
            }

            // Press Grip again
            interactor.selectInput.QueueManualState(true, 1f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.True);
            Assert.That(interactor.selectInput.ReadWasPerformedThisFrame(), Is.True);
            Assert.That(interactable.isHovered, Is.True);
            switch (selectActionTrigger)
            {
                case XRBaseInputInteractor.InputTriggerType.State:
                case XRBaseInputInteractor.InputTriggerType.StateChange:
                case XRBaseInputInteractor.InputTriggerType.Sticky:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
                case XRBaseInputInteractor.InputTriggerType.Toggle:
                    Assert.That(interactable.isSelected, Is.False);
                    Assert.That(interactor.isSelectActive, Is.False);
                    break;
            }

            // Release Grip again
            interactor.selectInput.QueueManualState(false, 0f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
        }

        [UnityTest]
        public IEnumerator RayInteractorCanReleaseInteractableAfterHoverToSelectWhenNotGripping([ValueSource(nameof(s_SelectActionTriggers))] XRBaseInputInteractor.InputTriggerType selectActionTrigger)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            interactor.hoverToSelect = true;
            interactor.hoverTimeToSelect = 0.2f;
            interactor.selectActionTrigger = selectActionTrigger;
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;

            yield return new WaitForSeconds(0.1f);

            // Hasn't met duration threshold yet
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            yield return new WaitForSeconds(0.2f);

            // Hovered for long enough
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            interactor.hoverToSelect = false;

            yield return null;

            // This table summarizes what must be done by a user to drop the Interactable
            // when hoverToSelect is disabled. It depends on whether the Select (i.e. Grip)
            // is pressed after the Interactable is automatically selected from hoverToSelect.
            // |Type       |To drop when Grip is false|To drop when Grip is true after auto Select|
            // |-----------|--------------------------|-------------------------------------------|
            // |State      |Nothing (will drop)       |Release Grip                               |
            // |StateChange|Press then Release Grip   |Release Grip                               |
            // |Toggle     |Press Grip                |Nothing (will drop since already toggled)  |
            // |Sticky     |Press then Release Grip   |Release Grip                               |
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            switch (selectActionTrigger)
            {
                case XRBaseInputInteractor.InputTriggerType.State:
                    Assert.That(interactable.isSelected, Is.False);
                    Assert.That(interactor.isSelectActive, Is.False);
                    break;
                case XRBaseInputInteractor.InputTriggerType.StateChange:
                case XRBaseInputInteractor.InputTriggerType.Toggle:
                case XRBaseInputInteractor.InputTriggerType.Sticky:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
            }

            // Press Grip
            interactor.selectInput.QueueManualState(true, 1f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.True);
            Assert.That(interactor.selectInput.ReadWasPerformedThisFrame(), Is.True);
            Assert.That(interactable.isHovered, Is.True);
            switch (selectActionTrigger)
            {
                case XRBaseInputInteractor.InputTriggerType.State:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
                case XRBaseInputInteractor.InputTriggerType.StateChange:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
                case XRBaseInputInteractor.InputTriggerType.Toggle:
                    Assert.That(interactable.isSelected, Is.False);
                    Assert.That(interactor.isSelectActive, Is.False);
                    break;
                case XRBaseInputInteractor.InputTriggerType.Sticky:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
            }

            // Release Grip
            interactor.selectInput.QueueManualState(false, 0f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
        }

        [UnityTest]
        public IEnumerator RayInteractorCanReleaseInteractableAfterHoverToSelectWhenGripping([ValueSource(nameof(s_SelectActionTriggers))] XRBaseInputInteractor.InputTriggerType selectActionTrigger)
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            interactor.hoverToSelect = true;
            interactor.hoverTimeToSelect = 0.2f;
            interactor.selectActionTrigger = selectActionTrigger;
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;

            yield return new WaitForSeconds(0.1f);

            // Hasn't met duration threshold yet
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);

            yield return new WaitForSeconds(0.2f);

            // Hovered for long enough
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            // Press Grip
            interactor.selectInput.QueueManualState(true, 1f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.True);
            Assert.That(interactor.selectInput.ReadWasPerformedThisFrame(), Is.True);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactor.isSelectActive, Is.True);

            interactor.hoverToSelect = false;

            yield return null;

            // This table summarizes what must be done by a user to drop the Interactable
            // when hoverToSelect is disabled. It depends on whether the Select (i.e. Grip)
            // is pressed after the Interactable is automatically selected from hoverToSelect.
            // |Type       |To drop when Grip is false|To drop when Grip is true after auto Select|
            // |-----------|--------------------------|-------------------------------------------|
            // |State      |Nothing (will drop)       |Release Grip                               |
            // |StateChange|Press then Release Grip   |Release Grip                               |
            // |Toggle     |Press Grip                |Nothing (will drop since already toggled)  |
            // |Sticky     |Press then Release Grip   |Release Grip                               |
            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.True);
            Assert.That(interactable.isHovered, Is.True);
            switch (selectActionTrigger)
            {
                case XRBaseInputInteractor.InputTriggerType.State:
                case XRBaseInputInteractor.InputTriggerType.StateChange:
                case XRBaseInputInteractor.InputTriggerType.Sticky:
                    Assert.That(interactable.isSelected, Is.True);
                    Assert.That(interactor.isSelectActive, Is.True);
                    break;
                case XRBaseInputInteractor.InputTriggerType.Toggle:
                    Assert.That(interactable.isSelected, Is.False);
                    Assert.That(interactor.isSelectActive, Is.False);
                    break;
            }

            // Release Grip
            interactor.selectInput.QueueManualState(false, 0f);

            yield return null;

            Assert.That(interactor.selectInput.ReadIsPerformed(), Is.False);
            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.isSelected, Is.False);
            Assert.That(interactor.isSelectActive, Is.False);
        }

        [UnityTest]
        public IEnumerator ManualInteractorSelection()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.right * 5.0f;

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);

            interactor.StartManualInteraction((IXRSelectInteractable)interactable);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            interactor.EndManualInteraction();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
        }

        [UnityTest]
        public IEnumerator RayInteractorCanResetOnInteractableDestroy()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            var attachPosition = interactor.attachTransform.position;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;

            yield return new WaitForSeconds(0.1f);
            // The recorder applies the press on its second Update; a single slow frame can satisfy the wait before that.
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));

            Object.Destroy(interactable.gameObject);

            yield return null;

            Assert.That(interactor.interactablesSelected, Is.Empty);
            Assert.That(interactor.attachTransform.position, Is.EqualTo(attachPosition));
        }

        [UnityTest]
        public IEnumerator RayInteractorCanLimitClosestOnly([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType)
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXROrigin();
            var interactor = CreateRayInteractor(rayInteractorType);
            interactor.hitClosestOnly = false;
            interactor.lineType = XRRayInteractor.LineType.StraightLine;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            // Verify that valid targets and hover targets is empty
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.Empty);

            Assert.That(interactor.interactablesHovered, Is.Empty);

            var nearInteractable = TestUtilities.CreateSimpleInteractable();
            nearInteractable.allowGazeInteraction = rayInteractorType == typeof(XRGazeInteractor);
            nearInteractable.transform.localPosition = new Vector3(0f, 0f, 10f);

            var farInteractable = TestUtilities.CreateSimpleInteractable();
            farInteractable.allowGazeInteraction = rayInteractorType == typeof(XRGazeInteractor);
            farInteractable.transform.localPosition = new Vector3(0f, 0f, 20f);

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            // Verify that both Interactables are hit
            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.EqualTo(new[] { nearInteractable, farInteractable }));

            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { nearInteractable, farInteractable }));
            Assert.That(nearInteractable.interactorsHovering, Is.EqualTo(new[] { interactor }));
            Assert.That(farInteractable.interactorsHovering, Is.EqualTo(new[] { interactor }));

            interactor.hitClosestOnly = true;

            // Wait for Valid Targets list to be updated
            yield return null;

            // Verify that only the closest Interactable is considered valid
            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.EqualTo(new[] { nearInteractable }));

            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { nearInteractable }));
            Assert.That(nearInteractable.interactorsHovering, Is.EqualTo(new[] { interactor }));
            Assert.That(farInteractable.interactorsHovering, Is.Empty);
        }

        [UnityTest]
        public IEnumerator RayInteractorCanLimitHitsOnSnapVolumes([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType)
        {
            TestUtilities.CreateInteractionManager();
            var interactor = CreateRayInteractor(rayInteractorType);
            interactor.lineType = XRRayInteractor.LineType.StraightLine;
            interactor.maxRaycastDistance = 50f;
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;

            // Create 3 objects, from furthest to closest to the ray interactor:
            // Interactable, "Far" Snap Volume, Trigger collider, "Near" Snap Volume <--- Ray Interactor
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 40f;
            Assert.That(interactable.colliders, Has.Count.EqualTo(1));

            var farSnapVolume = TestUtilities.CreateSnapVolume();
            farSnapVolume.transform.position = interactor.transform.position + interactor.transform.forward * 30f;
            farSnapVolume.interactable = interactable;

            var triggerCollider = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = Vector3.one;
            triggerCollider.transform.position = interactor.transform.position + interactor.transform.forward * 20f;

            var nearSnapVolume = TestUtilities.CreateSnapVolume();
            nearSnapVolume.transform.position = interactor.transform.position + interactor.transform.forward * 10f;
            nearSnapVolume.interactable = interactable;

            // Ignore trigger, ignore snap volume --> Hit Interactable
            interactor.raycastTriggerInteraction = QueryTriggerInteraction.Ignore;
            interactor.raycastSnapVolumeInteraction = XRRayInteractor.QuerySnapVolumeInteraction.Ignore;

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            var hasHit = interactor.TryGetCurrent3DRaycastHit(out var hit);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.collider, Is.SameAs(interactable.colliders[0]));

            // Ignore trigger, collide snap volume --> Hit "Near" Snap Volume
            interactor.raycastTriggerInteraction = QueryTriggerInteraction.Ignore;
            interactor.raycastSnapVolumeInteraction = XRRayInteractor.QuerySnapVolumeInteraction.Collide;

            yield return null;

            hasHit = interactor.TryGetCurrent3DRaycastHit(out hit);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.collider, Is.SameAs(nearSnapVolume.snapCollider));

            // Collide trigger, ignore snap volume --> Hit Trigger collider
            interactor.raycastTriggerInteraction = QueryTriggerInteraction.Collide;
            interactor.raycastSnapVolumeInteraction = XRRayInteractor.QuerySnapVolumeInteraction.Ignore;

            yield return null;

            hasHit = interactor.TryGetCurrent3DRaycastHit(out hit);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.collider, Is.SameAs(triggerCollider));

            // Collide trigger, collide snap volume --> Hit "Near" Snap Volume
            interactor.raycastTriggerInteraction = QueryTriggerInteraction.Collide;
            interactor.raycastSnapVolumeInteraction = XRRayInteractor.QuerySnapVolumeInteraction.Collide;

            yield return null;

            hasHit = interactor.TryGetCurrent3DRaycastHit(out hit);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.collider, Is.SameAs(nearSnapVolume.snapCollider));
        }

        [UnityTest]
        [Ignore("Ignored for known issue where only the ray casts of the first segment where a hit occurred are captured.")]
        public IEnumerator RayInteractorHitsAllAlongCurve([ValueSource(nameof(s_LineTypes))] XRRayInteractor.LineType lineType)
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXROrigin();
            var interactor = TestUtilities.CreateRayInteractor();
            const int sampleFrequency = 5;
            interactor.sampleFrequency = sampleFrequency;
            interactor.hitClosestOnly = false;
            interactor.lineType = lineType;

            yield return null;

            // Get the Sample Points to determine where to place the Interactable Planes in different segments
            Vector3[] samplePoints = null;
            var isValid = interactor.GetLinePoints(ref samplePoints, out var samplePointsCount);

            Assert.That(isValid, Is.True);
            Assert.That(samplePoints, Is.Not.Null);
            Assert.That(samplePointsCount, Is.EqualTo(lineType == XRRayInteractor.LineType.StraightLine ? 2 : sampleFrequency));
            Assert.That(samplePoints.Length, Is.GreaterThanOrEqualTo(samplePointsCount));

            // Verify that valid targets and hover targets is empty
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.Empty);

            Assert.That(interactor.interactablesHovered, Is.Empty);

            // Create two Interactable Planes such that they are within two separate line segments of the curve
            // (although for Straight Line, there is only the one line segment).
            // Add Interactable Plane near the end of the curve
            var from = samplePoints[samplePointsCount - 2];
            var to = samplePoints[samplePointsCount - 1];
            var farInteractable = CreatePlaneInteractable(Vector3.Lerp(from, to, 0.75f));

            // Add Interactable Plane near the start of the curve
            from = samplePoints[0];
            to = samplePoints[1];
            var nearInteractable = CreatePlaneInteractable(Vector3.Lerp(from, to, 0.25f));

            // Wait for Physics update for hit
            yield return new WaitForFixedUpdate();
            yield return null;

            // Verify that both Interactable Planes are hit
            interactor.GetValidTargets(validTargets);

            Assert.That(validTargets, Is.EqualTo(new[] { nearInteractable, farInteractable }));

            Assert.That(interactor.interactablesHovered, Is.EqualTo(new[] { nearInteractable, farInteractable }));

            IXRInteractable CreatePlaneInteractable(Vector3 position)
            {
                var planeGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
                var interactable = planeGO.AddComponent<XRSimpleInteractable>();
                planeGO.transform.localPosition = position;
                planeGO.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                planeGO.transform.localScale = new Vector3(100f, 1f, 100f);
                return interactable;
            }
        }

        [UnityTest]
        public IEnumerator RayInteractorSamplePointsContinuesThroughGeometry([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType, [ValueSource(nameof(s_LineTypes))] XRRayInteractor.LineType lineType)
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXROrigin();
            var interactor = CreateRayInteractor(rayInteractorType);
            if (rayInteractorType == typeof(XRGazeInteractor))
                interactor.gameObject.AddComponent<XRInteractorLineVisual>();
            interactor.transform.position = Vector3.zero;
            interactor.transform.rotation = Quaternion.Euler(-45f, 0f, 0f);
            const int sampleFrequency = 5;
            interactor.sampleFrequency = sampleFrequency;
            interactor.maxRaycastDistance = 20f;
            // These produce a curve that is the rough equivalent of the default projectile curve
            // in order to keep the assertions the same between the test cases.
            interactor.controlPointHeight = 0f;
            interactor.controlPointDistance = 22.6f;
            interactor.endPointHeight = -38.9f;
            interactor.endPointDistance = 45.2f;
            interactor.lineType = lineType;

            var lineVisual = interactor.GetComponent<XRInteractorLineVisual>();
            Assert.That(lineVisual, Is.Not.Null);
            Assert.That(lineVisual.enabled, Is.True);
            lineVisual.overrideInteractorLineLength = false;
            lineVisual.stopLineAtFirstRaycastHit = false;
            // Ensure we don't bend
            lineVisual.lineOriginOffset = 0f;
            // Disable line bending
            lineVisual.lineBendRatio = 1f;
            // Disable any automatic changes to the line length
            lineVisual.autoAdjustLineLength = false;

            var lineRenderer = interactor.GetComponent<LineRenderer>();
            Assert.That(lineRenderer, Is.Not.Null);
            Assert.That(lineRenderer.enabled, Is.True);

            yield return null;

            Vector3[] samplePoints = null;
            var isValid = interactor.GetLinePoints(ref samplePoints, out var samplePointsCount);

            Assert.That(isValid, Is.True);
            Assert.That(samplePoints, Is.Not.Null);
            Assert.That(samplePointsCount, Is.EqualTo(lineType == XRRayInteractor.LineType.StraightLine ? 2 : sampleFrequency));
            Assert.That(samplePoints.Length, Is.GreaterThanOrEqualTo(samplePointsCount));

            // Verify that the Ray Interactor is not hitting anything
            var isHitInfoValid = interactor.TryGetHitInfo(out _, out _, out var hitPositionInLine, out var isValidTarget);

            Assert.That(isHitInfoValid, Is.False);
            Assert.That(isValidTarget, Is.False);
            Assert.That(hitPositionInLine, Is.EqualTo(0));

            yield return null;

            // The Line Renderer should match
            Assert.That(lineVisual.enabled, Is.True);
            Assert.That(lineRenderer.enabled, Is.True);
            var lineRendererPositions = new Vector3[samplePointsCount];
            // LineRenderer.GetPositions always returns 0 when running with the -batchmode flag
            if (!Application.isBatchMode)
            {
                var lineRendererPositionsCount = lineRenderer.GetPositions(lineRendererPositions);
                Assert.That(lineRendererPositionsCount, Is.EqualTo(samplePointsCount));
                Assert.That(lineRendererPositions, Is.EquivalentTo(samplePoints).Using(Vector3ComparerWithEqualsOperator.Instance));
                Assert.That(lineRenderer.positionCount, Is.EqualTo(samplePointsCount));
            }

            // Create and place a plane between sample index 1 and 2
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localPosition = new Vector3(0f, 0f, 13f);
            plane.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            plane.transform.localScale = new Vector3(10f, 1f, 10f);

            // Wait for Physics update for hit and onBeforeRender callback to be invoked in XRInteractorLineVisual
            yield return new WaitForFixedUpdate();
            yield return null;
            yield return null;

            // Verify that the Ray Interactor is hitting the plane
            isHitInfoValid = interactor.TryGetHitInfo(out var hitPosition, out var hitNormal, out hitPositionInLine, out isValidTarget);

            Assert.That(isHitInfoValid, Is.True);
            Assert.That(isValidTarget, Is.False);
            Assert.That(hitPositionInLine, Is.EqualTo(lineType == XRRayInteractor.LineType.StraightLine ? 1 : 2));
            Assert.That(hitPosition.z, Is.EqualTo(plane.transform.position.z).Using(FloatEqualityComparer.Instance));
            Assert.That(hitNormal, Is.EqualTo(plane.transform.up).Using(Vector3ComparerWithEqualsOperator.Instance));

            // The sample points should continue beyond the hit to allow the
            // Line Visual behavior to render them
            Vector3[] samplePointsWithPlane = null;
            isValid = interactor.GetLinePoints(ref samplePointsWithPlane, out var samplePointsCountWithPlane);

            Assert.That(isValid, Is.True);
            Assert.That(samplePointsWithPlane, Is.Not.Null);
            Assert.That(samplePointsCountWithPlane, Is.EqualTo(lineType == XRRayInteractor.LineType.StraightLine ? 2 : sampleFrequency));
            Assert.That(samplePointsWithPlane.Length, Is.GreaterThanOrEqualTo(samplePointsCountWithPlane));
            Assert.That(samplePointsWithPlane, Is.EquivalentTo(samplePoints).Using(Vector3ComparerWithEqualsOperator.Instance));

            // The Line Renderer should still match
            Assert.That(lineVisual.enabled, Is.True);
            Assert.That(lineRenderer.enabled, Is.True);
            if (!Application.isBatchMode)
            {
                var lineRendererPositionsCount = lineRenderer.GetPositions(lineRendererPositions);
                Assert.That(lineRendererPositionsCount, Is.EqualTo(samplePointsCount));
                Assert.That(lineRendererPositions, Is.EquivalentTo(samplePoints).Using(Vector3ComparerWithEqualsOperator.Instance));
                Assert.That(lineRenderer.positionCount, Is.EqualTo(samplePointsCount));
            }

            lineVisual.stopLineAtFirstRaycastHit = true;

            // Wait for onBeforeRender callback to be invoked in XRInteractorLineVisual
            yield return null;
            yield return null;

            // The Line Renderer should now stop at the first hit
            Assert.That(lineVisual.enabled, Is.True);
            Assert.That(lineRenderer.enabled, Is.True);
            if (!Application.isBatchMode)
            {
                var lineRendererPositionsCount = lineRenderer.GetPositions(lineRendererPositions);
                Assert.That(lineRendererPositionsCount, Is.EqualTo(hitPositionInLine + 1));
                var expectedLineRendererPositions = samplePoints.Take(hitPositionInLine).ToList();
                expectedLineRendererPositions.Add(hitPosition);
                Assert.That(lineRendererPositions.Take(hitPositionInLine + 1), Is.EquivalentTo(expectedLineRendererPositions).Using(Vector3ComparerWithEqualsOperator.Instance));
                Assert.That(lineRenderer.positionCount, Is.EqualTo(hitPositionInLine + 1));
            }
        }

        [UnityTest]
        public IEnumerator RayInteractorCanUseTargetFilter([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType)
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXROrigin();
            var interactor = CreateRayInteractor(rayInteractorType);
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateSimpleInteractable();
            interactable.allowGazeInteraction = rayInteractorType == typeof(XRGazeInteractor);
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

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
                TargetFilterCallback.Link
            }));

            // Process the filter
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(interactor.targetFilter, Is.EqualTo(filter));
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process
            }));

            // Disable the filter and check if it will no longer be processed
            filter.canProcess = false;
            interactor.GetValidTargets(validTargets);
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process
            }));

            // Unlink the filter
            interactor.targetFilter = null;
            Assert.That(interactor.targetFilter, Is.EqualTo(null));
            Assert.That(filter.callbackExecution, Is.EqualTo(new List<TargetFilterCallback>
            {
                TargetFilterCallback.Link,
                TargetFilterCallback.Process,
                TargetFilterCallback.Unlink
            }));
        }

        [UnityTest]
        public IEnumerator RayInteractorTryGetHitInfoReturnsHighestScoredInteractable([ValueSource(nameof(s_RayInteractorTypes))] Type rayInteractorType)
        {
            TestUtilities.CreateInteractionManager();
            TestUtilities.CreateXROrigin();
            var interactor = CreateRayInteractor(rayInteractorType);
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var frontInteractable = TestUtilities.CreateSimpleInteractable();
            frontInteractable.allowGazeInteraction = rayInteractorType == typeof(XRGazeInteractor);
            frontInteractable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;
            var backInteractable = TestUtilities.CreateSimpleInteractable();
            backInteractable.allowGazeInteraction = rayInteractorType == typeof(XRGazeInteractor);
            backInteractable.transform.position = interactor.transform.position + interactor.transform.forward * 8.0f;

            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(frontInteractable.isHovered, Is.True);
            Assert.That(backInteractable.isHovered, Is.True);

            // Check valid targets
            var validTargets = new List<IXRInteractable>();
            interactor.GetValidTargets(validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { frontInteractable, backInteractable }));

            // Create the filter
            var targetFilter = TestUtilities.CreateTargetFilter();
            var distEvaluator = targetFilter.AddEvaluator<XRDistanceEvaluator>();
            distEvaluator.maxDistance = 10.0f;

            // Configure target filter to invert distance weight
            foreach (var key in distEvaluator.weight.keys)
                distEvaluator.weight.RemoveKey(0);

            distEvaluator.weight.AddKey(0.0f, 1.0f);
            distEvaluator.weight.AddKey(1.0f, 0.0f);

            // Link the filter
            interactor.targetFilter = targetFilter;
            Assert.That(interactor.targetFilter, Is.EqualTo(targetFilter));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Process the filter
            interactor.GetValidTargets(validTargets);
            Assert.That(interactor.targetFilter, Is.EqualTo(targetFilter));
            Assert.That(validTargets, Is.EqualTo(new[] { backInteractable, frontInteractable }));

            // Compare target filter processed results
            interactor.TryGetHitInfo(out var pos, out var norm, out var posInLine, out var isValid); // Returns info for back interactable
            interactor.TryGetCurrent3DRaycastHit(out var raycastHit); // Returns info for front interactable
            Assert.That(validTargets[0], Is.EqualTo(backInteractable));
            Assert.That(raycastHit.point, Is.Not.EqualTo(pos));
            Assert.That(pos, Is.EqualTo(backInteractable.transform.position - Vector3.forward));
        }


        [UnityTest]
        public IEnumerator RayInteractorCanSelectInteractorWithSnapVolume()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;
            var snapVolume = TestUtilities.CreateSnapVolume();
            snapVolume.interactable = interactable;
            snapVolume.snapToCollider = interactable.colliders[0];
            snapVolume.transform.position = interactable.transform.position;

            var controllerRecorder = TestUtilities.CreateControllerRecorder(interactor, (recording) =>
            {
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.0f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    false, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(0.1f, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
                recording.AddRecordingFrameNonAlloc(new XRControllerState(float.MaxValue, Vector3.zero, Quaternion.identity, InputTrackingState.All, true,
                    true, false, false));
            });
            controllerRecorder.isPlaying = true;

            yield return new WaitForSeconds(0.1f);
            // The recorder applies the press on its second Update; a single slow frame can satisfy the wait before that.
            yield return null;

            Assert.That(interactor.interactablesSelected, Is.EqualTo(new[] { interactable }));
        }

        static XRRayInteractor CreateRayInteractor(Type type)
        {
            if (type == typeof(XRGazeInteractor))
                return TestUtilities.CreateGazeInteractor();
            if (type == typeof(XRRayInteractor))
                return TestUtilities.CreateRayInteractor();

            Assert.Fail("Unhandled Ray Interactor type.");
            return null;
        }
    }
}
