#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
using System.Collections;
using NUnit.Framework;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
#if UITOOLKIT_WORLDSPACE_ENABLED
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class ScrollMomentumTests
    {
        const float k_FrameTime = 1f / 72f;
        const float k_Tolerance = 0.01f;
        const int k_MaxFrames = 500;
        const float k_ScrollDistance = 300f;

        // A uGUI scroll delta already includes the tracked scroll delta multiplier, so this scrolls k_ScrollDistance pixels.
        static readonly Vector2 k_ScrollDelta = new Vector2(0f, k_ScrollDistance);

        // Handles scroll events without scrolling, like an input field inside a scroll view.
        class ScrollEventConsumer : MonoBehaviour, IScrollHandler
        {
            public void OnScroll(PointerEventData eventData)
            {
            }
        }

        ScrollMomentum m_ScrollMomentum;

        [SetUp]
        public void SetUp()
        {
            m_ScrollMomentum = new ScrollMomentum();
        }

        [TearDown]
        public void TearDown()
        {
            m_ScrollMomentum.Clear();
            TestUtilities.DestroyAllSceneObjects();
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (m_PanelSettings != null)
                Object.DestroyImmediate(m_PanelSettings);
#endif
        }

        // The default rate at two frame rates, and no deceleration (0).
        [TestCase(ScrollMomentum.k_DefaultDecelerationRate, k_FrameTime)]
        [TestCase(ScrollMomentum.k_DefaultDecelerationRate, 1f / 120f)]
        [TestCase(0f, k_FrameTime)]
        public void MomentumScrollsExactDistanceAtAnyFrameRate(float decelerationRate, float deltaTime)
        {
            var remaining = k_ScrollDelta;
            var total = Vector2.zero;
            for (var i = 0; i < k_MaxFrames && remaining != Vector2.zero; ++i)
            {
                var step = ScrollMomentum.GetStep(remaining, decelerationRate, deltaTime);
                total += step;
                remaining -= step;
            }

            Assert.That(remaining, Is.EqualTo(Vector2.zero));
            Assert.That(total.y, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [Test]
        public void DecelerationRateIsCappedSoMomentumEnds()
        {
            var capped = ScrollMomentum.GetStep(k_ScrollDelta, ScrollMomentum.k_MaxDecelerationRate, k_FrameTime);

            Assert.That(ScrollMomentum.GetStep(k_ScrollDelta, 1f, k_FrameTime), Is.EqualTo(capped));
            Assert.That(ScrollMomentum.GetStep(k_ScrollDelta, 2f, k_FrameTime), Is.EqualTo(capped));
            Assert.That(capped.y, Is.GreaterThan(0f));
        }

        [Test]
        public void NoMomentumWithoutScrollHandlerOrScroll()
        {
            var plainObject = new GameObject("No Scroll Handler");
            var recorder = CreateScrollTarget("Scroll Target");

            Assert.That(m_ScrollMomentum.TryAdd(CreateEventData(plainObject, k_ScrollDelta)), Is.False);
            Assert.That(m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, Vector2.zero)), Is.False);
        }

        [Test]
        public void MomentumScrollsFullDistanceOverSeveralFrames()
        {
            var recorder = CreateScrollTarget("Scroll Target");
            var child = CreateChild("Child", recorder.transform);

            // Pointing at a child finds the scroll handler on its parent.
            Assert.That(m_ScrollMomentum.TryAdd(CreateEventData(child, k_ScrollDelta)), Is.True);
            RunMomentumToCompletion();

            Assert.That(recorder.deltas.Count, Is.GreaterThan(1));
            Assert.That(recorder.total.y, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [Test]
        public void ScrollDuringMomentumExtendsIt()
        {
            var recorder = CreateScrollTarget("Scroll Target");

            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, k_ScrollDelta));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, k_ScrollDelta));
            RunMomentumToCompletion();

            Assert.That(recorder.total.y, Is.EqualTo(k_ScrollDistance * 2f).Within(k_Tolerance));
        }

        [Test]
        public void OppositeScrollDuringMomentumReplacesItOnThatAxis()
        {
            var recorder = CreateScrollTarget("Scroll Target");

            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, new Vector2(k_ScrollDistance, k_ScrollDistance)));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            var movedBeforeOpposite = recorder.total;
            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, -k_ScrollDelta));
            RunMomentumToCompletion();

            // The vertical scroll moves its full distance back from where the content was, and the horizontal scroll still finishes.
            Assert.That(recorder.total.y, Is.EqualTo(movedBeforeOpposite.y - k_ScrollDistance).Within(k_Tolerance));
            Assert.That(recorder.total.x, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [Test]
        public void MomentumUsesScrollRectDecelerationRate()
        {
            var fast = CreateScrollRect("Fast", 0.01f, inertia: true);
            var slow = CreateScrollRect("Slow", 0.5f, inertia: true);
            var fastRecorder = fast.gameObject.AddComponent<TestUtilities.ScrollRecorder>();
            var slowRecorder = slow.gameObject.AddComponent<TestUtilities.ScrollRecorder>();

            m_ScrollMomentum.TryAdd(CreateEventData(fast.gameObject, k_ScrollDelta));
            m_ScrollMomentum.TryAdd(CreateEventData(slow.gameObject, k_ScrollDelta));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(fastRecorder.total.y, Is.EqualTo(ScrollMomentum.GetStep(k_ScrollDelta, 0.01f, k_FrameTime).y).Within(k_Tolerance));
            Assert.That(slowRecorder.total.y, Is.EqualTo(ScrollMomentum.GetStep(k_ScrollDelta, 0.5f, k_FrameTime).y).Within(k_Tolerance));
        }

        [Test]
        public void ScrollRectWithoutInertiaScrollsInstantly()
        {
            // Momentum follows the ScrollRect's own inertia setting: without inertia there is nothing to continue.
            var scrollRect = CreateScrollRect("No Inertia", ScrollMomentum.k_DefaultDecelerationRate, inertia: false);
            var recorder = scrollRect.gameObject.AddComponent<TestUtilities.ScrollRecorder>();

            m_ScrollMomentum.TryAdd(CreateEventData(scrollRect.gameObject, k_ScrollDelta));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(recorder.deltas.Count, Is.EqualTo(1));
            Assert.That(recorder.total.y, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [Test]
        public void MomentumStopsWhenTargetIsDeactivatedDisabledOrDestroyed()
        {
            var deactivated = CreateScrollTarget("Deactivated");
            var disabled = CreateScrollTarget("Disabled");
            var destroyed = CreateScrollTarget("Destroyed");
            m_ScrollMomentum.TryAdd(CreateEventData(deactivated.gameObject, k_ScrollDelta));
            m_ScrollMomentum.TryAdd(CreateEventData(disabled.gameObject, k_ScrollDelta));
            m_ScrollMomentum.TryAdd(CreateEventData(destroyed.gameObject, k_ScrollDelta));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            deactivated.gameObject.SetActive(false);
            disabled.enabled = false;
            Object.DestroyImmediate(destroyed.gameObject);
            Assert.DoesNotThrow(() => m_ScrollMomentum.UpdateMomentum(k_FrameTime));

            deactivated.gameObject.SetActive(true);
            disabled.enabled = true;
            var deliveredBeforeReactivating = deactivated.deltas.Count;
            var deliveredBeforeReenabling = disabled.deltas.Count;
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(deactivated.deltas.Count, Is.EqualTo(deliveredBeforeReactivating));
            Assert.That(disabled.deltas.Count, Is.EqualTo(deliveredBeforeReenabling), "A disabled scroll handler stops its momentum, so it does not resume when enabled again.");
        }

        [TestCase("Scroll Handler")]
        [TestCase("Child")]
        // A child that handles scroll events itself, like an input field, is the nearest scroll handler to the press,
        // but the press is still inside the scroll view, so it still stops the scroll view's momentum.
        [TestCase("Child Handling Scroll Events")]
        public void PressInsideTargetStopsMomentum(string pressedObjectName)
        {
            var recorder = CreateScrollTarget("Scroll Handler");
            CreateChild("Child", recorder.transform);
            CreateChild("Child Handling Scroll Events", recorder.transform).AddComponent<ScrollEventConsumer>();
            var pressedObject = pressedObjectName == recorder.name ? recorder.gameObject : recorder.transform.Find(pressedObjectName).gameObject;

            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, k_ScrollDelta));
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            m_ScrollMomentum.Stop(pressedObject);
            var deliveredBeforePress = recorder.deltas.Count;
            RunMomentumToCompletion();

            Assert.That(recorder.deltas.Count, Is.EqualTo(deliveredBeforePress));
        }

        [Test]
        public void PressStopsOnlyPressedTarget()
        {
            var pressed = CreateScrollTarget("Pressed");
            var other = CreateScrollTarget("Other");
            var unrelated = new GameObject("Unrelated");

            m_ScrollMomentum.TryAdd(CreateEventData(pressed.gameObject, k_ScrollDelta));
            m_ScrollMomentum.TryAdd(CreateEventData(other.gameObject, k_ScrollDelta));
            m_ScrollMomentum.Stop(unrelated);
            m_ScrollMomentum.Stop(null);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(pressed.deltas.Count, Is.EqualTo(1), "A press outside every scroll handler stops nothing.");

            m_ScrollMomentum.Stop(pressed.gameObject);
            RunMomentumToCompletion();

            Assert.That(pressed.deltas.Count, Is.EqualTo(1));
            Assert.That(other.total.y, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [UnityTest]
        public IEnumerator InputModuleAdvancesItsMomentumUntilDisabled()
        {
            var inputModule = TestUtilities.CreateXRUIInputModule();
            var recorder = CreateScrollTarget("Scroll Target");
            yield return null;

            inputModule.scrollMomentum.TryAdd(CreateEventData(recorder.gameObject, k_ScrollDelta));
            yield return null;
            yield return null;

            Assert.That(recorder.deltas.Count, Is.EqualTo(2), "The input module advances its momentum once each frame.");

            // Disabling the input module stops its momentum.
            inputModule.enabled = false;
            inputModule.enabled = true;
            yield return null;

            Assert.That(recorder.deltas.Count, Is.EqualTo(2));
        }

        [Test]
        public void ClearStopsAllMomentum()
        {
            var recorder = CreateScrollTarget("Scroll Target");
            m_ScrollMomentum.TryAdd(CreateEventData(recorder.gameObject, k_ScrollDelta));

            m_ScrollMomentum.Clear();
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(recorder.deltas, Is.Empty);
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        const float k_ViewSize = 200f;
        const float k_ContentHeight = 4000f;
        const float k_StartOffset = 1000f;

        // An interactor scroll delta before the default tracked scroll delta multiplier (5), so this scrolls k_ScrollDistance pixels down.
        static readonly Vector2 k_UIToolkitScrollDelta = new Vector2(0f, -k_ScrollDistance / UIInputModule.k_DefaultTrackedScrollDeltaMultiplier);

        PanelSettings m_PanelSettings;
        UIDocument m_Document;

        [UnityTest]
        public IEnumerator UIToolkitMomentumScrollsExactDistance()
        {
            var (scrollView, item) = AddScrollView();
            yield return null;
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);

            Assert.That(m_ScrollMomentum.TryAdd(item, k_UIToolkitScrollDelta), Is.True);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            var afterOneFrame = scrollView.scrollOffset.y - k_StartOffset;
            RunMomentumToCompletion();

            Assert.That(afterOneFrame, Is.GreaterThan(0f).And.LessThan(k_ScrollDistance));
            Assert.That(scrollView.scrollOffset.y - k_StartOffset, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        [UnityTest]
        public IEnumerator UIToolkitHorizontalScrollViewScrollsSidewaysWithVerticalScroll()
        {
            // The same as a horizontal-only ScrollView does with the mouse wheel.
            var (scrollView, item) = AddScrollView(ScrollViewMode.Horizontal);
            yield return null;
            scrollView.scrollOffset = new Vector2(k_StartOffset, 0f);

            m_ScrollMomentum.TryAdd(item, k_UIToolkitScrollDelta);
            RunMomentumToCompletion();

            Assert.That(scrollView.scrollOffset.x - k_StartOffset, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
            Assert.That(scrollView.scrollOffset.y, Is.EqualTo(0f));
        }

        [UnityTest]
        public IEnumerator UIToolkitMouseWheelScrollSizeScalesDistance()
        {
            // Mouse Wheel Scroll Size relative to the default plays the role of ScrollRect.scrollSensitivity for UI Toolkit.
            var (scrollView, item) = AddScrollView();
            scrollView.mouseWheelScrollSize = new ScrollView().mouseWheelScrollSize * 0.5f;
            yield return null;
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);

            m_ScrollMomentum.TryAdd(item, k_UIToolkitScrollDelta);
            RunMomentumToCompletion();

            Assert.That(scrollView.scrollOffset.y - k_StartOffset, Is.EqualTo(k_ScrollDistance * 0.5f).Within(k_Tolerance));
        }

        [UnityTest]
        public IEnumerator UIToolkitMomentumUsesScrollViewDecelerationRate()
        {
            var (fast, fastItem) = AddScrollView();
            var (slow, slowItem) = AddScrollView();
            fast.scrollDecelerationRate = 0.01f;
            slow.scrollDecelerationRate = 0.5f;
            yield return null;
            fast.scrollOffset = new Vector2(0f, k_StartOffset);
            slow.scrollOffset = new Vector2(0f, k_StartOffset);

            m_ScrollMomentum.TryAdd(fastItem, k_UIToolkitScrollDelta);
            m_ScrollMomentum.TryAdd(slowItem, k_UIToolkitScrollDelta);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            var distance = new Vector2(0f, k_ScrollDistance);
            Assert.That(fast.scrollOffset.y - k_StartOffset, Is.EqualTo(ScrollMomentum.GetStep(distance, 0.01f, k_FrameTime).y).Within(k_Tolerance));
            Assert.That(slow.scrollOffset.y - k_StartOffset, Is.EqualTo(ScrollMomentum.GetStep(distance, 0.5f, k_FrameTime).y).Within(k_Tolerance));
        }

        [UnityTest]
        public IEnumerator UIToolkitMomentumStopsAtEndOfContent()
        {
            var (scrollView, item) = AddScrollView();
            yield return null;
            scrollView.scrollOffset = Vector2.zero;

            m_ScrollMomentum.TryAdd(item, -k_UIToolkitScrollDelta);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            // Scrolling up from the top cannot move, so the momentum scroll ends instead of pushing against the edge.
            Assert.That(scrollView.scrollOffset.y, Is.EqualTo(0f));
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            Assert.That(scrollView.scrollOffset.y, Is.EqualTo(k_StartOffset));
        }

        [UnityTest]
        public IEnumerator UIToolkitMomentumStopsWhenScrollViewIsRemoved()
        {
            var (scrollView, item) = AddScrollView();
            yield return null;
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);

            m_ScrollMomentum.TryAdd(item, k_UIToolkitScrollDelta);
            scrollView.RemoveFromHierarchy();

            Assert.DoesNotThrow(() => m_ScrollMomentum.UpdateMomentum(k_FrameTime));
        }

        [UnityTest]
        public IEnumerator UIToolkitMomentumStopsWhenScrollViewIsDisabled()
        {
            var (scrollView, item) = AddScrollView();
            yield return null;
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);

            m_ScrollMomentum.TryAdd(item, k_UIToolkitScrollDelta);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            scrollView.SetEnabled(false);
            var offsetWhenDisabled = scrollView.scrollOffset;
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(scrollView.scrollOffset, Is.EqualTo(offsetWhenDisabled), "A disabled ScrollView does not scroll.");

            scrollView.SetEnabled(true);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);

            Assert.That(scrollView.scrollOffset, Is.EqualTo(offsetWhenDisabled), "The momentum stopped, so it does not resume when enabled again.");
        }

        [UnityTest]
        public IEnumerator UIToolkitNoMomentumOutsideScrollView()
        {
            CreateDocument();
            var element = new VisualElement();
            m_Document.rootVisualElement.Add(element);
            yield return null;

            Assert.That(m_ScrollMomentum.TryAdd(element, k_UIToolkitScrollDelta), Is.False);
            Assert.That(m_ScrollMomentum.TryAdd((VisualElement)null, k_UIToolkitScrollDelta), Is.False);
        }

        [UnityTest]
        public IEnumerator UIToolkitManagerAdvancesMomentumEachFrame()
        {
            // Scenes with only UI Toolkit panels have no UIInputModule, so XRUIToolkitManager drives momentum there.
            var manager = new GameObject("XR UI Toolkit Manager").AddComponent<XRUIToolkitManager>();
            var (scrollView, item) = AddScrollView();
            yield return null;
            scrollView.scrollOffset = new Vector2(0f, k_StartOffset);

            // The manager gives the handler its momentum while enabled, so interactor scrolls reach it.
            var managerMomentum = XRUIToolkitHandler.activeScrollMomentum;
            Assert.That(managerMomentum, Is.Not.Null);

            managerMomentum.TryAdd(item, k_UIToolkitScrollDelta);
            yield return null;
            var movedAfterOneFrame = scrollView.scrollOffset.y - k_StartOffset;
            yield return null;
            var movedAfterTwoFrames = scrollView.scrollOffset.y - k_StartOffset;

            Assert.That(movedAfterOneFrame, Is.GreaterThan(0f));
            Assert.That(movedAfterTwoFrames, Is.GreaterThan(movedAfterOneFrame));

            // Disabling the manager stops its momentum and takes it back from the handler.
            manager.enabled = false;
            Assert.That(XRUIToolkitHandler.activeScrollMomentum, Is.Null);
            var offsetWhenDisabled = scrollView.scrollOffset.y;
            yield return null;
            Assert.That(scrollView.scrollOffset.y, Is.EqualTo(offsetWhenDisabled));
        }

        [UnityTest]
        public IEnumerator UIToolkitPressStopsOnlyPressedScrollView()
        {
            var (pressed, pressedItem) = AddScrollView();
            var (other, otherItem) = AddScrollView();
            yield return null;
            pressed.scrollOffset = new Vector2(0f, k_StartOffset);
            other.scrollOffset = new Vector2(0f, k_StartOffset);

            m_ScrollMomentum.TryAdd(pressedItem, k_UIToolkitScrollDelta);
            m_ScrollMomentum.TryAdd(otherItem, k_UIToolkitScrollDelta);
            m_ScrollMomentum.UpdateMomentum(k_FrameTime);
            SendPointerDown(pressedItem);
            yield return null;

            var offsetAtPress = pressed.scrollOffset.y;
            RunMomentumToCompletion();

            Assert.That(pressed.scrollOffset.y, Is.EqualTo(offsetAtPress), "A press inside the ScrollView stops its momentum.");
            Assert.That(other.scrollOffset.y - k_StartOffset, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance), "Other ScrollViews keep scrolling.");

            // The press listener is removed with the momentum scroll, and a new scroll starts momentum again.
            m_ScrollMomentum.TryAdd(pressedItem, k_UIToolkitScrollDelta);
            RunMomentumToCompletion();
            Assert.That(pressed.scrollOffset.y - offsetAtPress, Is.EqualTo(k_ScrollDistance).Within(k_Tolerance));
        }

        void CreateDocument()
        {
            m_PanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            m_Document = new GameObject("UI Document").AddComponent<UIDocument>();
            m_Document.panelSettings = m_PanelSettings;
        }

        (ScrollView scrollView, VisualElement item) AddScrollView(ScrollViewMode mode = ScrollViewMode.Vertical)
        {
            if (m_Document == null)
                CreateDocument();

            var scrollView = new ScrollView(mode);
            scrollView.style.width = k_ViewSize;
            scrollView.style.height = k_ViewSize;
            var item = new VisualElement();
            if (mode == ScrollViewMode.Horizontal)
                item.style.width = k_ContentHeight;
            else
                item.style.height = k_ContentHeight;

            item.style.flexShrink = 0f;
            scrollView.Add(item);
            m_Document.rootVisualElement.Add(scrollView);
            return (scrollView, item);
        }

        static void SendPointerDown(VisualElement target)
        {
            using var pointerDown = PointerDownEvent.GetPooled();
            pointerDown.target = target;
            target.SendEvent(pointerDown);
        }
#endif

        static TestUtilities.ScrollRecorder CreateScrollTarget(string name)
        {
            return new GameObject(name).AddComponent<TestUtilities.ScrollRecorder>();
        }

        static GameObject CreateChild(string name, Transform parent)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent, false);
            return child;
        }

        static ScrollRect CreateScrollRect(string name, float decelerationRate, bool inertia)
        {
            var scrollRect = new GameObject(name, typeof(RectTransform)).AddComponent<ScrollRect>();
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(scrollRect.transform, false);
            scrollRect.content = (RectTransform)content.transform;
            scrollRect.decelerationRate = decelerationRate;
            scrollRect.inertia = inertia;
            return scrollRect;
        }

        static PointerEventData CreateEventData(GameObject pointerEnter, Vector2 scrollDelta)
        {
            return new PointerEventData(null) { pointerEnter = pointerEnter, scrollDelta = scrollDelta };
        }

        void RunMomentumToCompletion()
        {
            for (var i = 0; i < k_MaxFrames; ++i)
                m_ScrollMomentum.UpdateMomentum(k_FrameTime);
        }
    }
}
