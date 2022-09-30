using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InteractorLineVisualTests
    {
        static readonly Gradient k_InvalidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };
        static readonly Gradient k_ValidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };
        static readonly Gradient k_BlockedColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.yellow, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator LineVisualUsesCorrectStateVisualOptions()
        {
            TestUtilities.CreateInteractionManager();

            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var lineVisual = interactor.GetComponent<XRInteractorLineVisual>();
            var lineRenderer = lineVisual.GetComponent<LineRenderer>();
            lineVisual.invalidColorGradient = k_InvalidColorGradient;
            lineVisual.validColorGradient = k_ValidColorGradient;
            lineVisual.blockedColorGradient = k_BlockedColorGradient;
            var validReticle = new GameObject("valid reticle");
            lineVisual.reticle = validReticle;
            var blockedReticle = new GameObject("blocked reticle");
            lineVisual.blockedReticle = blockedReticle;

            // No valid target
            yield return new WaitForSeconds(0.1f);
            lineVisual.UpdateLineVisual();
            Assert.That(lineRenderer.colorGradient.Evaluate(0f), Is.EqualTo(k_InvalidColorGradient.Evaluate(0f)).Using(ColorEqualityComparer.Instance));
            Assert.That(lineRenderer.colorGradient.Evaluate(1f), Is.EqualTo(k_InvalidColorGradient.Evaluate(1f)).Using(ColorEqualityComparer.Instance));
            Assert.That(validReticle.activeSelf, Is.False);
            Assert.That(blockedReticle.activeSelf, Is.False);

            // Valid target exists
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;
            yield return new WaitForSeconds(0.1f);
            lineVisual.UpdateLineVisual();
            Assert.That(lineRenderer.colorGradient.Evaluate(0f), Is.EqualTo(k_ValidColorGradient.Evaluate(0f)).Using(ColorEqualityComparer.Instance));
            Assert.That(lineRenderer.colorGradient.Evaluate(1f), Is.EqualTo(k_ValidColorGradient.Evaluate(1f)).Using(ColorEqualityComparer.Instance));
            Assert.That(validReticle.activeSelf, Is.True);
            Assert.That(blockedReticle.activeSelf, Is.False);

            // Valid target exists but is not selectable
            var blockedFilter = new XRSelectFilterDelegate((x, y) => false);
            interactable.selectFilters.Add(blockedFilter);
            yield return new WaitForSeconds(0.1f);
            lineVisual.UpdateLineVisual();
            Assert.That(lineRenderer.colorGradient.Evaluate(0f), Is.EqualTo(k_BlockedColorGradient.Evaluate(0f)).Using(ColorEqualityComparer.Instance));
            Assert.That(lineRenderer.colorGradient.Evaluate(1f), Is.EqualTo(k_BlockedColorGradient.Evaluate(1f)).Using(ColorEqualityComparer.Instance));
            Assert.That(validReticle.activeSelf, Is.False);
            Assert.That(blockedReticle.activeSelf, Is.True);
        }
    }
}