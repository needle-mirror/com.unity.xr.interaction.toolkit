#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UITOOLKIT_WORLDSPACE_ENABLED
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Scroll momentum for UI scroll input, where each scroll target slows to a stop on its own.
    /// </summary>
    /// <remarks>
    /// Scroll added to a target is applied over the following frames and slows down at that target's deceleration rate,
    /// so several targets can scroll at once, and each keeps scrolling after the pointer aims elsewhere.
    /// Targets are uGUI scroll handlers, such as <see cref="ScrollRect"/>, and UI Toolkit <c>ScrollView</c> elements.
    /// Remaining distances use the uGUI tracked scroll delta convention: interactor scroll input multiplied by
    /// <see cref="UIInputModule.trackedScrollDeltaMultiplier"/>, before any per-target scroll sensitivity.
    /// Each owner keeps its own instance: <see cref="UIInputModule"/> for uGUI targets, and <see cref="XRUIToolkitManager"/>
    /// for UI Toolkit targets.
    /// </remarks>
    class ScrollMomentum
    {
        /// <summary>
        /// The deceleration rate for uGUI scroll handlers that are not a <see cref="ScrollRect"/>,
        /// matching the default <see cref="ScrollRect.decelerationRate"/>.
        /// </summary>
        internal const float k_DefaultDecelerationRate = 0.135f;

        /// <summary>
        /// The highest deceleration rate used. A rate of <c>1</c> would never slow down, so the scroll would never finish.
        /// </summary>
        internal const float k_MaxDecelerationRate = 0.99f;

        // Remaining distance below this is applied in a single step instead of scrolling indefinitely.
        const float k_MinimumRemainingDistance = 0.5f;

        struct MomentumTarget
        {
            public GameObject scrollHandler;

            // Looked up once when the momentum scroll starts so the per-frame update does no component lookups.
            public ScrollRect scrollRect;
#if UITOOLKIT_WORLDSPACE_ENABLED
            public ScrollView scrollView;
#endif
            public Vector2 remainingDistance;
        }

        readonly List<MomentumTarget> m_MomentumTargets = new List<MomentumTarget>();
        PointerEventData m_EventData;
        EventSystem m_EventDataEventSystem;
#if UITOOLKIT_WORLDSPACE_ENABLED
        float m_DefaultMouseWheelScrollSize;
        readonly EventCallback<PointerDownEvent> m_OnScrollViewPointerDown;

        /// <summary>
        /// Initializes and returns an instance of <see cref="ScrollMomentum"/>.
        /// </summary>
        internal ScrollMomentum()
        {
            m_OnScrollViewPointerDown = OnScrollViewPointerDown;
        }
#endif

        /// <summary>
        /// Adds the scroll delta of a uGUI pointer event to the momentum scroll of the scroll handler under the pointer.
        /// </summary>
        /// <param name="eventData">The pointer event, with a scroll delta that already includes the tracked scroll delta multiplier.</param>
        /// <returns>Returns <see langword="true"/> if a momentum scroll was started or extended, or <see langword="false"/>
        /// if there is no scroll handler under the pointer.</returns>
        internal bool TryAdd(PointerEventData eventData)
        {
            if (eventData.scrollDelta == Vector2.zero)
                return false;

            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
            if (scrollHandler == null)
                return false;

            scrollHandler.TryGetComponent<ScrollRect>(out var scrollRect);
            Add(new MomentumTarget { scrollHandler = scrollHandler, scrollRect = scrollRect }, eventData.scrollDelta);
            return true;
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        /// <summary>
        /// Adds an interactor scroll delta to the momentum scroll of the UI Toolkit <see cref="ScrollView"/> containing the hovered element.
        /// </summary>
        /// <param name="hoveredElement">The element under the pointer.</param>
        /// <param name="scrollDelta">The interactor scroll delta, before the tracked scroll delta multiplier.</param>
        /// <returns>Returns <see langword="true"/> if a momentum scroll was started or extended, or <see langword="false"/>
        /// if the hovered element is not inside a <see cref="ScrollView"/>.</returns>
        internal bool TryAdd(VisualElement hoveredElement, Vector2 scrollDelta)
        {
            if (scrollDelta == Vector2.zero || hoveredElement == null)
                return false;

            var scrollView = hoveredElement as ScrollView ?? hoveredElement.GetFirstAncestorOfType<ScrollView>();
            if (scrollView == null)
                return false;

            Add(new MomentumTarget { scrollView = scrollView }, scrollDelta * UIInputModule.GetActiveTrackedScrollDeltaMultiplier());
            return true;
        }
#endif

        /// <summary>
        /// Advances every momentum scroll by the given time.
        /// </summary>
        /// <param name="deltaTime">The time in seconds to advance.</param>
        internal void UpdateMomentum(float deltaTime)
        {
            for (var i = m_MomentumTargets.Count - 1; i >= 0; --i)
            {
                var target = m_MomentumTargets[i];
                if (!IsValid(target))
                {
                    RemoveAt(i);
                    continue;
                }

                var step = GetStep(target.remainingDistance, GetDecelerationRate(target), deltaTime);
                if (step == Vector2.zero)
                    continue;

                target.remainingDistance -= step;
                var moved = Apply(target, step);

                if (!moved || target.remainingDistance == Vector2.zero)
                    RemoveAt(i);
                else
                    m_MomentumTargets[i] = target;
            }
        }

        /// <summary>
        /// Stops the momentum scroll of every uGUI scroll handler that contains the pressed object.
        /// </summary>
        /// <param name="pressedObject">The object a pointer pressed.</param>
        /// <remarks>
        /// A press stops the momentum the same way it stops a <see cref="ScrollRect"/>'s own inertia, and still reaches
        /// the pressed object the same way. Any press inside the scroll handler counts, including presses on child objects
        /// that handle scroll events themselves.
        /// </remarks>
        internal void Stop(GameObject pressedObject)
        {
            if (m_MomentumTargets.Count == 0 || pressedObject == null)
                return;

            var pressedTransform = pressedObject.transform;
            for (var i = m_MomentumTargets.Count - 1; i >= 0; --i)
            {
                var scrollHandler = m_MomentumTargets[i].scrollHandler;
                if (scrollHandler != null && pressedTransform.IsChildOf(scrollHandler.transform))
                    RemoveAt(i);
            }
        }

        /// <summary>
        /// Stops every momentum scroll.
        /// </summary>
        internal void Clear()
        {
            for (var i = m_MomentumTargets.Count - 1; i >= 0; --i)
                RemoveAt(i);
        }

        /// <summary>
        /// Gets how much of the remaining scroll distance to apply this frame.
        /// </summary>
        /// <param name="remainingDistance">The scroll distance still to apply.</param>
        /// <param name="decelerationRate">The fraction of the scroll speed that remains after one second,
        /// using the same convention as <see cref="ScrollRect.decelerationRate"/>. Values above
        /// <see cref="k_MaxDecelerationRate"/> are treated as that rate.</param>
        /// <param name="deltaTime">The time in seconds since the previous frame.</param>
        /// <returns>Returns the scroll distance to apply this frame.</returns>
        /// <remarks>
        /// Applying a fraction of the remaining distance each frame makes the total distance exact at any frame rate,
        /// with the same exponential slowdown as <see cref="ScrollRect"/> inertia.
        /// </remarks>
        internal static Vector2 GetStep(Vector2 remainingDistance, float decelerationRate, float deltaTime)
        {
            var rate = Mathf.Clamp(decelerationRate, 0f, k_MaxDecelerationRate);
            var step = remainingDistance * (1f - Mathf.Pow(rate, deltaTime));
            var minimumSquared = k_MinimumRemainingDistance * k_MinimumRemainingDistance;
            return (remainingDistance - step).sqrMagnitude < minimumSquared ? remainingDistance : step;
        }

        void Add(MomentumTarget newTarget, Vector2 distance)
        {
            for (var i = 0; i < m_MomentumTargets.Count; ++i)
            {
                var target = m_MomentumTargets[i];
                if (!IsSameTarget(target, newTarget))
                    continue;

                target.remainingDistance = AddDistance(target.remainingDistance, distance);
                m_MomentumTargets[i] = target;
                return;
            }

            newTarget.remainingDistance = distance;
            m_MomentumTargets.Add(newTarget);
#if UITOOLKIT_WORLDSPACE_ENABLED
            // A press anywhere in the ScrollView content stops its momentum. Trickle down so presses on child elements count.
            // Registered on the content container, the same as the ScrollView's own press handling, so that handling runs
            // first and the ScrollView can still be dragged after the press.
            newTarget.scrollView?.contentContainer.RegisterCallback(m_OnScrollViewPointerDown, TrickleDown.TrickleDown);
#endif
        }

        static Vector2 AddDistance(Vector2 remaining, Vector2 added)
        {
            if (remaining.x * added.x < 0f)
                remaining.x = 0f;

            if (remaining.y * added.y < 0f)
                remaining.y = 0f;

            return remaining + added;
        }

        void RemoveAt(int index)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            m_MomentumTargets[index].scrollView?.contentContainer.UnregisterCallback(m_OnScrollViewPointerDown, TrickleDown.TrickleDown);
#endif
            m_MomentumTargets.RemoveAt(index);
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        void OnScrollViewPointerDown(PointerDownEvent evt)
        {
            var stoppedMomentum = false;
            for (var i = m_MomentumTargets.Count - 1; i >= 0; --i)
            {
                if (m_MomentumTargets[i].scrollView?.contentContainer != evt.currentTarget)
                    continue;

                stoppedMomentum = true;
                RemoveAt(i);
            }

            // A press that stops a momentum scroll only stops it: it does not reach the elements inside the ScrollView, but can
            // still drag the ScrollView. This matches how a ScrollView handles a press that stops its own inertia.
            if (!stoppedMomentum)
                return;

            ((VisualElement)evt.currentTarget).CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }
#endif

        static bool IsSameTarget(in MomentumTarget first, in MomentumTarget second)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (first.scrollView != second.scrollView)
                return false;
#endif
            return first.scrollHandler == second.scrollHandler;
        }

        static bool IsValid(in MomentumTarget target)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            // A disabled ScrollView stops, the same way UI Toolkit stops sending it scroll and pointer events.
            if (target.scrollView != null)
                return target.scrollView.panel != null && target.scrollView.enabledInHierarchy;
#endif
            return target.scrollHandler != null && target.scrollHandler.activeInHierarchy;
        }

        static float GetDecelerationRate(in MomentumTarget target)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (target.scrollView != null)
                return target.scrollView.scrollDecelerationRate;
#endif
            if (target.scrollRect != null)
                return target.scrollRect.inertia ? target.scrollRect.decelerationRate : 0f;

            return k_DefaultDecelerationRate;
        }

        // Returns whether the target can still move, so a momentum scroll stops once a ScrollView reaches the end of its content,
        // or once a uGUI scroll handler is disabled and no longer receives scroll events.
        bool Apply(in MomentumTarget target, Vector2 step)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (target.scrollView != null)
            {
                if (m_DefaultMouseWheelScrollSize <= 0f)
                    m_DefaultMouseWheelScrollSize = new ScrollView().mouseWheelScrollSize;

                // UI Toolkit scrolls opposite to the uGUI scroll delta. A custom mouseWheelScrollSize scales the
                // distance the same way ScrollRect.scrollSensitivity does for uGUI.
                var sensitivity = target.scrollView.mouseWheelScrollSize / m_DefaultMouseWheelScrollSize;

                // A horizontal-only ScrollView scrolls sideways with vertical scroll, the same as it does with the mouse wheel.
                var scrollStep = step;
                if (target.scrollView.mode == ScrollViewMode.Horizontal && Mathf.Abs(step.y) > Mathf.Abs(step.x))
                    scrollStep = new Vector2(step.y, 0f);

                var previousOffset = target.scrollView.scrollOffset;
                target.scrollView.scrollOffset -= scrollStep * sensitivity;
                return target.scrollView.scrollOffset != previousOffset;
            }
#endif
            if (m_EventData == null || m_EventDataEventSystem != EventSystem.current)
            {
                m_EventDataEventSystem = EventSystem.current;
                m_EventData = new PointerEventData(m_EventDataEventSystem);
            }

            m_EventData.scrollDelta = step;
            return ExecuteEvents.Execute(target.scrollHandler, m_EventData, ExecuteEvents.scrollHandler);
        }
    }
}
