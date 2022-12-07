using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class AffordanceSystemTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator AffordanceStateTransitionWorks()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();

            var affordanceStateProvider = interactable.gameObject.AddComponent<XRInteractableAffordanceStateProvider>();
            affordanceStateProvider.interactableSource = interactable;
            var floatAffordanceReceiver = interactable.gameObject.AddComponent<FloatAffordanceReceiver>();

            var testFloatTheme = new FloatAffordanceTheme();
            testFloatTheme.SetAffordanceThemeDataList(new List<AffordanceThemeData<float>>
            {
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.disabled), animationStateStartValue = 0f, animationStateEndValue = 0f },
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.idle), animationStateStartValue = 0f, animationStateEndValue = 0f },
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.hovered), animationStateStartValue = 0.5f, animationStateEndValue = 0.5f },
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.hoveredPriority), animationStateStartValue = 0.5f, animationStateEndValue = 0.5f },
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.selected), animationStateStartValue = 1f, animationStateEndValue = 1f },
                new AffordanceThemeData<float> { stateName = nameof(AffordanceStateShortcuts.activated), animationStateStartValue = 0f, animationStateEndValue = 0f }
            });

            floatAffordanceReceiver.affordanceStateProvider = affordanceStateProvider;
            floatAffordanceReceiver.affordanceTheme = testFloatTheme;

            yield return null;

            // Test to ensure we're in the idle state
            Assert.That(floatAffordanceReceiver.currentAffordanceValue.Value, Is.EqualTo(0f));

            var directInteractor = TestUtilities.CreateDirectInteractor();

            yield return null;
            yield return new WaitForFixedUpdate();

            var validTargets = new List<IXRInteractable>();
            manager.GetValidTargets(directInteractor, validTargets);
            Assert.That(validTargets, Is.EqualTo(new[] { interactable }));

            Assert.That(directInteractor.interactablesHovered, Is.EqualTo(new[] { interactable }));
            Assert.That(directInteractor.hasHover, Is.True);

            // Wait for affordance transitions to complete
            yield return new WaitWhile(() => affordanceStateProvider.isCurrentlyTransitioning);

            // Test that Hover state is active
            Assert.That(floatAffordanceReceiver.currentAffordanceValue.Value, Is.EqualTo(0.5f));
        }
    }
}
