using System.Collections;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class CurveVisualControllerTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator LineRendererPositions_DoNotContainNaN_WhenEndPointEqualsOrigin()
        {
            var go = new GameObject("CurveVisualController Test");
            go.transform.position = Vector3.zero;
            go.SetActive(false);

            var mockProvider = go.AddComponent<MockCurveDataProvider>();
            mockProvider.lastSamplePoint = Vector3.zero;

            var lineRenderer = go.AddComponent<LineRenderer>();
            var curveVisual = go.AddComponent<CurveVisualController>();
            curveVisual.lineRenderer = lineRenderer;
            curveVisual.curveInteractionDataProvider = mockProvider;
            go.SetActive(true);

            yield return null;

            var positionCount = lineRenderer.positionCount;
            Assert.That(positionCount, Is.GreaterThan(0), "LineRenderer should have positions after visual update");

            var positions = new Vector3[positionCount];
            lineRenderer.GetPositions(positions);

            for (var i = 0; i < positionCount; i++)
            {
                Assert.That(float.IsNaN(positions[i].x), Is.False, $"Position[{i}].x is NaN");
                Assert.That(float.IsNaN(positions[i].y), Is.False, $"Position[{i}].y is NaN");
                Assert.That(float.IsNaN(positions[i].z), Is.False, $"Position[{i}].z is NaN");
            }
        }

        class MockCurveDataProvider : MonoBehaviour, ICurveInteractionDataProvider
        {
            public Vector3 lastSamplePoint { get; set; }

            public bool isActive => true;

            public bool hasValidSelect => false;

            public Transform curveOrigin => transform;

            public NativeArray<Vector3> samplePoints => default;

            public EndPointType TryGetCurveEndPoint(out Vector3 endPoint, bool snapToSelectedAttachIfAvailable = false, bool snapToSnapVolumeIfAvailable = false)
            {
                endPoint = lastSamplePoint;
                return EndPointType.None;
            }

            public EndPointType TryGetCurveEndNormal(out Vector3 endNormal, bool snapToSelectedAttachIfAvailable = false)
            {
                endNormal = Vector3.up;
                return EndPointType.None;
            }
        }
    }
}
