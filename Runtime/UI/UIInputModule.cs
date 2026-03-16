using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.UI;
#if TEXTMESHPRO_PACKAGE_PRESENT || (UGUI_2_0_OR_NEWER && UNITY_2023_2_OR_NEWER)
using TMPro;
#endif


namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Base class for input modules that send UI input.
    /// </summary>
    /// <remarks>
    /// Multiple input modules may be placed on the same event system. In such a setup,
    /// the modules will synchronize with each other.
    /// </remarks>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_UIInputModule)]
    public abstract partial class UIInputModule : BaseInputModule
    {
        [Header("Configuration")]
        [SerializeField, FormerlySerializedAs("clickSpeed")]
        [Tooltip("The maximum time (in seconds) between two mouse presses for it to be consecutive click.")]
        float m_ClickSpeed = 0.3f;
        /// <summary>
        /// The maximum time (in seconds) between two mouse presses for it to be consecutive click.
        /// </summary>
        public float clickSpeed
        {
            get => m_ClickSpeed;
            set => m_ClickSpeed = value;
        }

        [SerializeField, FormerlySerializedAs("moveDeadzone")]
        [Tooltip("The absolute value required by a move action on either axis required to trigger a move event.")]
        float m_MoveDeadzone = 0.6f;
        /// <summary>
        /// The absolute value required by a move action on either axis required to trigger a move event.
        /// </summary>
        public float moveDeadzone
        {
            get => m_MoveDeadzone;
            set => m_MoveDeadzone = value;
        }

        [SerializeField, FormerlySerializedAs("repeatDelay")]
        [Tooltip("The Initial delay (in seconds) between an initial move action and a repeated move action.")]
        float m_RepeatDelay = 0.5f;
        /// <summary>
        /// The Initial delay (in seconds) between an initial move action and a repeated move action.
        /// </summary>
        public float repeatDelay
        {
            get => m_RepeatDelay;
            set => m_RepeatDelay = value;
        }

        [FormerlySerializedAs("repeatRate")]
        [SerializeField, Tooltip("The speed (in seconds) that the move action repeats itself once repeating.")]
        float m_RepeatRate = 0.1f;
        /// <summary>
        /// The speed (in seconds) that the move action repeats itself once repeating.
        /// </summary>
        public float repeatRate
        {
            get => m_RepeatRate;
            set => m_RepeatRate = value;
        }

        [FormerlySerializedAs("trackedDeviceDragThresholdMultiplier")]
        [SerializeField, Tooltip("Scales the EventSystem.pixelDragThreshold, for tracked devices, to make selection easier.")]
        float m_TrackedDeviceDragThresholdMultiplier = 2f;
        /// <summary>
        /// Scales the <see cref="EventSystem.pixelDragThreshold"/>, for tracked devices, to make selection easier.
        /// </summary>
        public float trackedDeviceDragThresholdMultiplier
        {
            get => m_TrackedDeviceDragThresholdMultiplier;
            set => m_TrackedDeviceDragThresholdMultiplier = value;
        }

        [SerializeField, Tooltip("Scales the scroll delta in pointer event data, for tracked devices only.")]
        float m_TrackedScrollDeltaMultiplier = 5f;
        /// <summary>
        /// Scales the scroll delta in pointer event data, for tracked devices only.
        /// This is a multiplier value that allows you to adjust the scroll speed sent to UI components.
        /// </summary>
        /// <remarks>
        /// This value controls the magnitude of the <see cref="PointerEventData.scrollDelta"/> value. It acts as a multiplier,
        /// so a value of <c>1</c> passes through the original value. A value larger than one increases the scrolling speed,
        /// and a value less than one decreases the speed. A value of zero prevents scrolling from working at all.
        /// You can set this to a negative value to invert the scroll direction.
        /// </remarks>
        /// <seealso cref="nonTrackedScrollDeltaMultiplier"/>
        public float trackedScrollDeltaMultiplier
        {
            get => m_TrackedScrollDeltaMultiplier;
            set => m_TrackedScrollDeltaMultiplier = value;
        }

        [SerializeField, Tooltip("Scales the scroll delta in pointer event data, for non-tracked devices only.")]
        float m_NonTrackedScrollDeltaMultiplier = 6f;
        /// <summary>
        /// Scales the scroll delta in pointer event data, for non-tracked devices only.
        /// This is a multiplier value that allows you to adjust the scroll speed sent to UI components.
        /// </summary>
        /// <remarks>
        /// This value controls the magnitude of the <see cref="PointerEventData.scrollDelta"/> value. It acts as a multiplier,
        /// so a value of <c>1</c> passes through the original value. A value larger than one increases the scrolling speed,
        /// and a value less than one decreases the speed. A value of zero prevents scrolling from working at all.
        /// You can set this to a negative value to invert the scroll direction.
        /// <br />
        /// This value is not used to scale the scroll delta when exclusively using the old Input Manager instead of
        /// the Input System in order to maintain legacy functionality.
        /// </remarks>
        /// <seealso cref="trackedScrollDeltaMultiplier"/>
        public float nonTrackedScrollDeltaMultiplier
        {
            get => m_NonTrackedScrollDeltaMultiplier;
            set => m_NonTrackedScrollDeltaMultiplier = value;
        }

        [SerializeField, Tooltip("Disables sending events from Event System to UI Toolkit on behalf of this Input Module.")]
        bool m_BypassUIToolkitEvents;

        /// <summary>
        /// Disables sending events from Event System to UI Toolkit on behalf of this Input Module.
        /// </summary>
        public bool bypassUIToolkitEvents
        {
            get => m_BypassUIToolkitEvents;
            set => m_BypassUIToolkitEvents = value;
        }

        Camera m_UICamera;

        /// <summary>
        /// The <see cref="Camera"/> that Unity uses to perform ray casts when determining the screen space location of a tracked device cursor.
        /// </summary>
        public Camera uiCamera
        {
            get
            {
                // If set by the user, always use this Camera.
                if (m_UICamera != null)
                    return m_UICamera;

                // Camera.main returns the first active and enabled Main Camera, so if the cached one
                // is no longer enabled, find the new Main Camera. This is to support, for example,
                // toggling between different XROrigin rigs each with their own Main Camera.
                if (m_MainCameraCache == null || !m_MainCameraCache.isActiveAndEnabled)
                    m_MainCameraCache = Camera.main;

                return m_MainCameraCache;
            }
            set => m_UICamera = value;
        }

        Camera m_MainCameraCache;

        [SerializeField]
        [Tooltip("When enabled, additional sorting is run to ensure things closer to the point of interaction are considered first. It is recommended to enable this for XR devices and disable for non-XR devices.")]
        bool m_PrioritizeDistanceSorting;

        /// <summary>
        /// When enabled, the UI Input Module will run additional sorting after <c>RaycastAll</c> is called to ensure
        /// things that are closer to the point of interaction are considered first when analyzing results.
        /// It is recommended to enable this for XR devices and disable for non-XR devices.
        /// </summary>
        /// <remarks>
        /// This is only available for tracked devices. When re-sorting the Raycast Results list, this will sort
        /// first by distance, then follow the same order as the <c>EventSystem.RaycastComparer</c> uses to determine
        /// result ordering. If using non-XR platforms, this should be set to <see langword="false"/> since the
        /// additional sorting will add unnecessary overhead and existing results are already sorted in a way that
        /// makes sense for screen space interactions.
        /// </remarks>
        public bool prioritizeDistanceSorting
        {
            get => m_PrioritizeDistanceSorting;
            set => m_PrioritizeDistanceSorting = value;
        }

        AxisEventData m_CachedAxisEvent;
        readonly Dictionary<int, PointerEventData> m_PointerEventByPointerId = new Dictionary<int, PointerEventData>();
        readonly Dictionary<int, TrackedDeviceEventData> m_TrackedDeviceEventByPointerId = new Dictionary<int, TrackedDeviceEventData>();

        /// <summary>
        /// This is a copy of the RaycastComparer from EventSystem to allow for custom sorting of raycast results
        /// based on distance first.
        /// Original source file located at unity/Packages/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs
        /// </summary>
        sealed class RaycastResultComparer : IComparer<RaycastResult>
        {
            public int Compare(RaycastResult lhs, RaycastResult rhs)
            {
                // Prioritize distance first for XR applications, but only if the delta is large enough to be visually meaningful.
                // When distances are nearly equal, continue with the same ordering logic as the EventSystem.
                // This distance threshold helps with sorting consistency as the distance can fluctuate over time.

                // More details of the specific case this is solving for:
                // This threshold for distance sorting was added because the distance between a Scroll Rect's
                // Viewport child GameObject will sometimes change ordering compared to a parent GameObject of
                // the Scroll Rect since the distances are sub-millimeter even when there aren't any Z-offsets in
                // the Rect Transforms. This could cause the closest element hit to no longer be under the
                // `IScrollHandler` component hierarchy, which can cause unwanted behavior, such as re-enabling
                // of locomotion in the Controller Input Action Manager component, even when the user is still
                // pointing at the same UI. This distance flipping behavior seems to occur consistently when scrolling
                // beyond the beginning or end when the Scroll Rect has Movement Type set to either Unrestricted or Elastic.
                const float nearlyEqualThreshold = 1e-3f;
                if (Mathf.Abs(lhs.distance - rhs.distance) > nearlyEqualThreshold)
                    return lhs.distance.CompareTo(rhs.distance);

                if (lhs.module != rhs.module)
                {
                    var lhsEventCamera = lhs.module.eventCamera;
                    var rhsEventCamera = rhs.module.eventCamera;
                    if (lhsEventCamera != null && rhsEventCamera != null && lhsEventCamera.depth != rhsEventCamera.depth)
                    {
                        // need to reverse the standard compareTo
                        if (lhsEventCamera.depth < rhsEventCamera.depth)
                            return 1;
                        if (lhsEventCamera.depth == rhsEventCamera.depth)
                            return 0;

                        return -1;
                    }

                    if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                        return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);

                    if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                        return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
                }

                // Renderer sorting
                if (lhs.sortingLayer != rhs.sortingLayer)
                {
                    // Uses the layer value to properly compare the relative order of the layers.
                    var rid = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                    var lid = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                    return rid.CompareTo(lid);
                }

                if (lhs.sortingOrder != rhs.sortingOrder)
                    return rhs.sortingOrder.CompareTo(lhs.sortingOrder);

                // comparing depth only makes sense if the two raycast results have the same root canvas (case 912396)
                if (lhs.depth != rhs.depth && lhs.module.rootRaycaster == rhs.module.rootRaycaster)
                    return rhs.depth.CompareTo(lhs.depth);

                if (lhs.distance != rhs.distance)
                    return lhs.distance.CompareTo(rhs.distance);

                return lhs.index.CompareTo(rhs.index);
            }
        }

        static readonly RaycastResultComparer s_RaycastResultComparer = new RaycastResultComparer();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <remarks>
        /// Processing is postponed from earlier in the frame (<see cref="EventSystem"/> has a
        /// script execution order of <c>-1000</c>) until this Update to allow other systems to
        /// update the poses that will be used to generate the ray casts used by this input module.
        /// <br />
        /// For Ray Interactor, it must wait until after the Controller pose updates and Locomotion
        /// moves the Rig in order to generate the current sample points used to create the rays used
        /// for this frame. Those positions will be determined during <see cref="DoProcess"/>.
        /// Ray Interactor needs the UI ray casts to be completed by the time <see cref="XRInteractionManager"/>
        /// calls into <see cref="IXRInteractor.GetValidTargets"/> since that is dependent on
        /// whether a UI hit was closer than a 3D hit. This processing must therefore be done
        /// between Locomotion and <see cref="IXRInteractor.PreprocessInteractor"/> to minimize latency.
        /// </remarks>
        protected virtual void Update()
        {
            // Check to make sure that Process should still be called.
            // It would likely cause unexpected results if processing was done
            // when this module is no longer the current one.
            if (eventSystem.IsActive() && eventSystem.currentInputModule == this && eventSystem == EventSystem.current)
            {
                DoProcess();
            }
        }

        /// <summary>
        /// Process the current tick for the module.
        /// </summary>
        /// <remarks>
        /// Executed once per Update call. Override for custom processing.
        /// </remarks>
        /// <seealso cref="Process"/>
        protected virtual void DoProcess()
        {
            SendUpdateEventToSelectedObject();
        }

        /// <summary>
        /// See <a href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.BaseInputModule.html#UnityEngine_EventSystems_BaseInputModule_Process">BaseInputModule.Process()</a>.
        /// </summary>
        public override void Process()
        {
            // Postpone processing until later in the frame
        }

        /// <summary>
        /// Sends an update event to the currently selected object.
        /// </summary>
        /// <returns>Returns whether the update event was used by the selected object.</returns>
        protected bool SendUpdateEventToSelectedObject()
        {
            var selectedGameObject = eventSystem.currentSelectedGameObject;
            if (selectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            updateSelected?.Invoke(selectedGameObject, data);
            ExecuteEvents.Execute(selectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Called by <c>EventSystem</c> when the input module is made current.
        /// </summary>
        public override void ActivateModule()
        {
            base.ActivateModule();

            // This is required for mouse/pointer events to work with UI Toolkit
            if (bypassUIToolkitEvents)
#pragma warning disable CS0618 // Type or member is obsolete
                EventSystem.SetUITookitEventSystemOverride(eventSystem, false, false);
#pragma warning restore CS0618 // Type or member is obsolete

            // Select firstSelectedGameObject if nothing is selected ATM.
            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        /// <summary>
        /// This will check the existing lists of pointer events and hand back the most current
        /// GameObject entered by the current pointer.
        /// </summary>
        /// <param name="pointerId">ID of the XR device pointer, mouse pointer or touch registered with the UIInputModule.
        /// Meaning this should correspond to either <see cref="PointerEventData"/>.<c>pointerId</c> or <see cref="TrackedDeviceEventData"/>.<c>pointerId</c>.
        /// </param>
        /// <returns>The GameObject that triggered the <see cref="PointerEventData.pointerEnter"/> event.</returns>
        /// <remarks>
        /// Any negative value used for <paramref name="pointerId"/> will be treated as <c>any</c>. The first event
        /// from a tracked device will be used first, then to standard pointer devices such as mice and touchscreens.
        /// </remarks>
        /// <seealso cref="IsPointerOverGameObject" />
        public GameObject GetCurrentGameObject(int pointerId)
        {
            // For negative pointer IDs, find any cached pointer events that have a registered pointerEnter object
            if (pointerId < 0)
            {
                foreach (var trackedEvent in m_TrackedDeviceEventByPointerId.Values)
                {
                    if (trackedEvent != null && trackedEvent.pointerEnter != null)
                        return trackedEvent.pointerEnter;
                }

                foreach (var trackedEvent in m_PointerEventByPointerId.Values)
                {
                    if (trackedEvent != null && trackedEvent.pointerEnter != null)
                        return trackedEvent.pointerEnter;
                }
            }
            else
            {
                if (m_TrackedDeviceEventByPointerId.TryGetValue(pointerId, out var trackedDeviceEvent))
                    return trackedDeviceEvent?.pointerEnter;

                if (m_PointerEventByPointerId.TryGetValue(pointerId, out var pointerEvent))
                    return pointerEvent?.pointerEnter;
            }
            return null;
        }

        /// <summary>
        /// Is the pointer with the given ID over an EventSystem object?
        /// </summary>
        /// <param name="pointerId">ID of the XR device pointer, mouse pointer or touch registered with the UIInputModule.
        /// Meaning this should correspond to either <see cref="PointerEventData"/>.<c>pointerId</c> or <see cref="TrackedDeviceEventData"/>.<c>pointerId</c>.
        /// </param>
        /// <returns>Returns <see langword="true"/> if the given pointer is currently hovering over a <c>GameObject</c>. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// The pointer IDs are generated at runtime by the UIInputModule as devices are registered. Calling this method
        /// without any parameters will attempt to use the Left Mouse Button and will likely result in unexpected behavior.
        /// A negative pointerId value will be interpreted as "any pointer" and will return true if any XR pointer is
        /// currently over a GameObject.
        /// Note: The IDs used to check for interaction are not the same as standard InputDevice device IDs.
        /// </remarks>
        /// <seealso cref="GetCurrentGameObject" />
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using UnityEngine.EventSystems;
        /// using UnityEngine.XR.Interaction.Toolkit.UI;
        ///
        /// public class ClickExample : MonoBehaviour
        /// {
        ///     [SerializeField]
        ///     UIInputModule inputModule;
        ///
        ///     private void OnEnable()
        ///     {
        ///         if (inputModule != null)
        ///         {
        ///             inputModule.pointerClick += OnDeviceButtonClick;
        ///         }
        ///     }
        ///
        ///     private void OnDisable()
        ///     {
        ///         if (inputModule != null)
        ///         {
        ///             inputModule.pointerClick -= OnDeviceButtonClick;
        ///         }
        ///     }
        ///
        ///     // This method will fire after registering with the UIInputModule callbacks. The UIInputModule will
        ///     // pass the PointerEventData for the device responsible for triggering the callback and can be used to
        ///     // find the pointerId registered with the EventSystem for that device-specific event.
        ///     private void OnDeviceButtonClick(GameObject selected, PointerEventData pointerData)
        ///     {
        ///         if (EventSystem.current.IsPointerOverGameObject(pointerData.pointerId))
        ///         {
        ///             Debug.Log($"Clicked on {EventSystem.current.currentSelectedGameObject}", this);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public override bool IsPointerOverGameObject(int pointerId)
        {
            return GetCurrentGameObject(pointerId) != null;
        }

        RaycastResult PerformRaycast(PointerEventData eventData, bool resort)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            // Note: RaycastAll has an internal sorting algorithm that should be considered
            // when analyzing the results. Here is the order that things are sorted:
            // - raycaster differences
            //    - eventCamera depth
            //    - sortOrderPriority
            //    - renderOrderPriority
            // - sortingLayer
            // - sortingOrder
            // - depth
            // - distance
            // - 2D physics
            //    - sortingGroupID
            //    - sortingGroupOrder
            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            if (resort && m_RaycastResultCache.Count > 1)
                SortingHelpers.Sort(m_RaycastResultCache, s_RaycastResultComparer);

            finalizeRaycastResults?.Invoke(eventData, m_RaycastResultCache);
            var result = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            return result;
        }

        //CONSIDER: #XRA-664
        internal RaycastResult PerformRaycast(ref PointerEventData eventData, Vector2 screenPosition)
        {
            eventData ??= new PointerEventData(eventSystem);
            eventData.position = screenPosition;

            // Skip finalizeRaycastResults in this method since the assumption is that this method bypasses the normal
            // process sequence since it is used by the simulator. Skipping it keeps the execution flow unchanged.
            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            var result = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            return result;
        }

        /// <summary>
        /// Takes an existing <see cref="PointerModel"/> and dispatches all relevant changes through the event system.
        /// It also updates the internal data of the <see cref="PointerModel"/>.
        /// </summary>
        /// <param name="pointerState">The pointer state you want to forward into the UI Event System.</param>
        private protected void ProcessPointerState(ref PointerModel pointerState)
        {
            if (!pointerState.changedThisFrame)
                return;

            var eventData = GetOrCreateCachedPointerEvent(pointerState.pointerId);
            eventData.Reset();

            pointerState.CopyTo(eventData);

            eventData.pointerCurrentRaycast = PerformRaycast(eventData, false);

            // Left Mouse Button
            // The left mouse button is 'dominant' and we want to also process hover and scroll events as if the occurred during the left click.
            var buttonState = pointerState.leftButton;
            eventData.button = PointerEventData.InputButton.Left;
            buttonState.CopyTo(eventData);
            ProcessPointerButton(buttonState.lastFrameDelta, eventData);

            ProcessPointerMovement(eventData);
            ProcessScrollWheel(eventData);

            pointerState.CopyFrom(eventData);

            ProcessPointerButtonDrag(eventData, UIPointerType.MouseOrPen);

            buttonState.CopyFrom(eventData);
            pointerState.leftButton = buttonState;

            // Right Mouse Button
            buttonState = pointerState.rightButton;
            eventData.button = PointerEventData.InputButton.Right;
            buttonState.CopyTo(eventData);

            ProcessPointerButton(buttonState.lastFrameDelta, eventData);
            ProcessPointerButtonDrag(eventData, UIPointerType.MouseOrPen);

            buttonState.CopyFrom(eventData);
            pointerState.rightButton = buttonState;

            // Middle Mouse Button
            buttonState = pointerState.middleButton;
            eventData.button = PointerEventData.InputButton.Middle;
            buttonState.CopyTo(eventData);

            ProcessPointerButton(buttonState.lastFrameDelta, eventData);
            ProcessPointerButtonDrag(eventData, UIPointerType.MouseOrPen);

            buttonState.CopyFrom(eventData);
            pointerState.middleButton = buttonState;

            pointerState.OnFrameFinished();
        }

        void ProcessPointerMovement(PointerEventData eventData)
        {
            var currentPointerTarget = eventData.pointerCurrentRaycast.gameObject;

            // If the pointer moved, send move events to all UI elements the pointer is
            // currently over.
            var wasMoved = eventData.IsPointerMoving();
            if (wasMoved)
            {
                for (var i = 0; i < eventData.hovered.Count; ++i)
                {
                    pointerMove?.Invoke(eventData.hovered[i], eventData);
                    ExecuteEvents.Execute(eventData.hovered[i], eventData, ExecuteEvents.pointerMoveHandler);
                }
            }

            // If we have no target or pointerEnter has been deleted,
            // we just send exit events to anything we are tracking
            // and then exit.
            if (currentPointerTarget == null || eventData.pointerEnter == null)
            {
                foreach (var hovered in eventData.hovered)
                {
                    pointerExit?.Invoke(hovered, eventData);
                    ExecuteEvents.Execute(hovered, eventData, ExecuteEvents.pointerExitHandler);
                }

                eventData.hovered.Clear();

                if (currentPointerTarget == null)
                {
                    eventData.pointerEnter = null;
                    return;
                }
            }

            if (eventData.pointerEnter == currentPointerTarget)
                return;

            var commonRoot = FindCommonRoot(eventData.pointerEnter, currentPointerTarget);

            // We walk up the tree until a common root and the last entered and current entered object is found.
            // Then send exit and enter events up to, but not including, the common root.
            if (eventData.pointerEnter != null)
            {
                var target = eventData.pointerEnter.transform;

                while (target != null)
                {
                    if (commonRoot != null && commonRoot.transform == target)
                        break;

                    var targetGameObject = target.gameObject;
                    pointerExit?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerExitHandler);

                    eventData.hovered.Remove(targetGameObject);

                    target = target.parent;
                }
            }

            eventData.pointerEnter = currentPointerTarget;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- Could be null if it was destroyed immediately after executing above
            if (currentPointerTarget != null)
            {
                var target = currentPointerTarget.transform;

                while (target != null && target.gameObject != commonRoot)
                {
                    var targetGameObject = target.gameObject;
                    pointerEnter?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerEnterHandler);

                    if (wasMoved)
                    {
                        pointerMove?.Invoke(targetGameObject, eventData);
                        ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerMoveHandler);
                    }

                    eventData.hovered.Add(targetGameObject);

                    target = target.parent;
                }
            }
        }

        void ProcessPointerButton(ButtonDeltaState mouseButtonChanges, PointerEventData eventData, bool clickOnDown = false)
        {
            var hoverTarget = eventData.pointerCurrentRaycast.gameObject;

            if ((mouseButtonChanges & ButtonDeltaState.Pressed) != 0)
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
                eventData.useDragThreshold = true;

                var selectHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(hoverTarget);

                // If we have clicked something new, deselect the old thing
                // and leave 'selection handling' up to the press event.
                if (eventSystem.currentSelectedGameObject != null && selectHandler != eventSystem.currentSelectedGameObject)
                    eventSystem.SetSelectedGameObject(null, eventData);

                // search for the control that will receive the press.
                // if we can't find a press handler set the press
                // handler to be what would receive a click.

                pointerDown?.Invoke(hoverTarget, eventData);
                var newPressed = ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.pointerDownHandler);

                // We didn't find a press handler, so we search for a click handler.
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);

                var time = Time.unscaledTime;

                if (newPressed == eventData.lastPress && ((time - eventData.clickTime) < m_ClickSpeed))
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;

                eventData.pointerPress = newPressed;
                eventData.rawPointerPress = hoverTarget;

                // Save the drag handler for drag events during this mouse down.
                var dragObject = ExecuteEvents.GetEventHandler<IDragHandler>(hoverTarget);
                eventData.pointerDrag = dragObject;

                if (dragObject != null)
                {
                    initializePotentialDrag?.Invoke(dragObject, eventData);
                    ExecuteEvents.Execute(dragObject, eventData, ExecuteEvents.initializePotentialDrag);
                }

                var clickOnDownTarget = eventData.pointerPress;

                // Update mouse button state to released if click on down is applicable
                if (clickOnDown && CanTargetClickOnDown(clickOnDownTarget))
                {
                    mouseButtonChanges = ButtonDeltaState.Released;
                }
            }

            if ((mouseButtonChanges & ButtonDeltaState.Released) != 0)
            {
                var target = eventData.pointerPress;
                pointerUp?.Invoke(target, eventData);
                ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);
                var pointerDrag = eventData.pointerDrag;

                if (target == pointerUpHandler && eventData.eligibleForClick)
                {
                    pointerClick?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.dragging && pointerDrag != null)
                {
                    drop?.Invoke(hoverTarget, eventData);
                    ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.dropHandler);
                }

                eventData.eligibleForClick = false;
                eventData.pointerPress = null;
                eventData.rawPointerPress = null;

                if (eventData.dragging && pointerDrag != null)
                {
                    endDrag?.Invoke(pointerDrag, eventData);
                    ExecuteEvents.Execute(pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }

                eventData.dragging = false;
                eventData.pointerDrag = null;
            }
        }

        void ProcessPointerButtonDrag(PointerEventData eventData, UIPointerType pointerType, float pixelDragThresholdMultiplier = 1.0f)
        {
            if (!eventData.IsPointerMoving() ||
                (pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked) ||
                eventData.pointerDrag == null)
            {
                return;
            }

            if (!eventData.dragging)
            {
                var threshold = eventSystem.pixelDragThreshold * pixelDragThresholdMultiplier;
                if (!eventData.useDragThreshold || (eventData.pressPosition - eventData.position).sqrMagnitude >= (threshold * threshold))
                {
                    var target = eventData.pointerDrag;
                    beginDrag?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.beginDragHandler);
                    eventData.dragging = true;
                }
            }

            if (eventData.dragging)
            {
                // If we moved from our initial press object, process an up for that object.
                var target = eventData.pointerPress;
                if (target != eventData.pointerDrag)
                {
                    pointerUp?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }

                drag?.Invoke(eventData.pointerDrag, eventData);
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }

        void ProcessScrollWheel(PointerEventData eventData)
        {
            var scrollDelta = eventData.scrollDelta;
            if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
                scroll?.Invoke(scrollHandler, eventData);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, eventData, ExecuteEvents.scrollHandler);
            }
        }

#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
        private protected void ProcessTouch(ref TouchModel touchState)
        {
            if (!touchState.changedThisFrame)
                return;

            var eventData = GetOrCreateCachedPointerEvent(touchState.pointerId);
            eventData.Reset();

            touchState.CopyTo(eventData);

            eventData.pointerCurrentRaycast = (touchState.selectPhase == TouchPhase.Canceled) ? new RaycastResult() : PerformRaycast(eventData, false);
            eventData.button = PointerEventData.InputButton.Left;

            ProcessPointerButton(touchState.selectDelta, eventData);
            ProcessPointerMovement(eventData);
            ProcessPointerButtonDrag(eventData, UIPointerType.Touch);

            touchState.CopyFrom(eventData);

            touchState.OnFrameFinished();
        }
#endif

        private protected void ProcessTrackedDevice(ref TrackedDeviceModel deviceState, bool force = false)
        {
            if (!deviceState.changedThisFrame && !force)
                return;

            var eventData = GetOrCreateCachedTrackedDeviceEvent(deviceState.pointerId);
            eventData.Reset();
            deviceState.CopyTo(eventData);
            eventData.scrollDelta *= m_TrackedScrollDeltaMultiplier;

            eventData.button = PointerEventData.InputButton.Left;

            // Use a screen point outside the camera's viewport to use for the `EventSystem.RaycastAll`.
            // This invalid point will be used for the event data's position property so we don't trigger any hits
            // from a GraphicRaycaster component on a Canvas.
            // The TrackedDeviceGraphicRaycaster does not use position, and instead upcasts the eventData to
            // TrackedDeviceEventData to access the ray points to cast with.
            //
            // Ideally we could just copy the `EventSystem.RaycastAll` method and skip raycaster modules that aren't
            // used for tracked devices (i.e., so only our TrackedDeviceGraphicRaycaster or other whitelisted raycasters runs),
            // but the sorting comparer (`EventSystem.RaycastComparer`) for the RaycastResult list is private to the EventSystem.
            // `BaseRaycaster.eventCamera.pixelRect - new Vector2(-1f, -1f)` would also ensure the `Camera.ScreenToViewportPoint`
            // method returns a negative value, causing an early out of `GraphicRaycaster.Raycast`, but that would need to be
            // done for each raycaster.
            //
            // For typical setups in XR projects, (-1, -1) should likely be outside the Camera viewport.
            var savedPosition = eventData.position;
            var savedDelta = eventData.delta;
            eventData.position = new Vector2(-1f, -1f);
            eventData.delta = Vector2.zero;
            eventData.pointerCurrentRaycast = PerformRaycast(eventData, m_PrioritizeDistanceSorting);
            // Restore the original value after the Raycast is complete.
            eventData.position = savedPosition;
            eventData.delta = savedDelta;

            if (TryGetCamera(eventData, out var screenPointCamera))
            {
                Vector2 screenPosition;
                if (eventData.pointerCurrentRaycast.isValid)
                {
                    screenPosition = screenPointCamera.WorldToScreenPoint(eventData.pointerCurrentRaycast.worldPosition);
                    if ((deviceState.selectDelta & ButtonDeltaState.Pressed) != 0)
                    {
                        eventData.pressWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
                    }
                }
                else
                {
                    var endPosition = eventData.rayPoints.Count > 0 ? eventData.rayPoints[eventData.rayPoints.Count - 1] : Vector3.zero;
                    screenPosition = screenPointCamera.WorldToScreenPoint(endPosition);
                    eventData.position = screenPosition;
                }

                var thisFrameDelta = screenPosition - eventData.position;
                eventData.position = screenPosition;
                eventData.delta = thisFrameDelta;

                ProcessPointerButton(deviceState.selectDelta, eventData, deviceState.clickOnDown);
                ProcessPointerMovement(eventData);
                ProcessScrollWheel(eventData);

                // In a VR headset context, the camera can move while the pointer/controller stays put, but this
                // breaks the standard 2D screen space model. This will ensure that the initial press position used
                // for drag detection is updated as head-movement updates each frame.
                if (eventData.pressPosition != Vector2.zero)
                {
                    eventData.pressPosition = screenPointCamera.WorldToScreenPoint(eventData.pressWorldPosition);
                }

                ProcessPointerButtonDrag(eventData, UIPointerType.Tracked, m_TrackedDeviceDragThresholdMultiplier);

                var oldTarget = deviceState.implementationData.pointerTarget;
                deviceState.CopyFrom(eventData);

                var newTarget = deviceState.implementationData.pointerTarget;
                if (oldTarget != newTarget)
                {
                    if (newTarget != null)
                    {
                        var selectable = newTarget.GetComponentInParent<ISelectHandler>();
                        var scrollable = newTarget.GetComponentInParent<IScrollHandler>();
                        deviceState.selectableObject = (selectable as Component)?.gameObject;
                        deviceState.isScrollable = scrollable != null;
                    }
                    else
                    {
                        deviceState.selectableObject = null;
                        deviceState.isScrollable = false;
                    }
                }
            }

            deviceState.OnFrameFinished();
        }

        bool TryGetCamera(PointerEventData eventData, out Camera screenPointCamera)
        {
            // Get associated Camera, or Main Camera, or Camera from ray cast, and if *nothing* exists, then abort processing this frame.
            screenPointCamera = uiCamera;
            if (screenPointCamera != null)
                return true;

            var module = eventData.pointerCurrentRaycast.module;
            if (module != null)
            {
                screenPointCamera = module.eventCamera;
                return screenPointCamera != null;
            }

            return false;
        }

        /// <summary>
        /// Takes an existing NavigationModel and dispatches all relevant changes through the event system.
        /// It also updates the internal data of the NavigationModel.
        /// </summary>
        /// <param name="navigationState">The navigation state you want to forward into the UI Event System</param>
        private protected void ProcessNavigationState(ref NavigationModel navigationState)
        {
            var usedSelectionChange = SendUpdateEventToSelectedObject();

            // Don't send move events if disabled in the EventSystem.
            if (!eventSystem.sendNavigationEvents)
                return;

            var implementationData = navigationState.implementationData;
            var selectedGameObject = eventSystem.currentSelectedGameObject;

            var movement = navigationState.move;
            if (!usedSelectionChange && (!Mathf.Approximately(movement.x, 0f) || !Mathf.Approximately(movement.y, 0f)))
            {
                var time = Time.unscaledTime;

                var moveDirection = MoveDirection.None;
                if (movement.sqrMagnitude > m_MoveDeadzone * m_MoveDeadzone)
                {
                    if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                        moveDirection = (movement.x > 0f) ? MoveDirection.Right : MoveDirection.Left;
                    else
                        moveDirection = (movement.y > 0f) ? MoveDirection.Up : MoveDirection.Down;
                }

                if (moveDirection != implementationData.lastMoveDirection)
                {
                    implementationData.consecutiveMoveCount = 0;
                }

                if (moveDirection != MoveDirection.None)
                {
                    var allow = true;
                    if (implementationData.consecutiveMoveCount != 0)
                    {
                        if (implementationData.consecutiveMoveCount > 1)
                            allow = (time > (implementationData.lastMoveTime + m_RepeatRate));
                        else
                            allow = (time > (implementationData.lastMoveTime + m_RepeatDelay));
                    }

                    if (allow)
                    {
                        var eventData = GetOrCreateCachedAxisEvent();
                        eventData.Reset();

                        eventData.moveVector = movement;
                        eventData.moveDir = moveDirection;

                        move?.Invoke(selectedGameObject, eventData);
                        ExecuteEvents.Execute(selectedGameObject, eventData, ExecuteEvents.moveHandler);
                        usedSelectionChange = eventData.used;

                        implementationData.consecutiveMoveCount++;
                        implementationData.lastMoveTime = time;
                        implementationData.lastMoveDirection = moveDirection;
                    }
                }
                else
                {
                    implementationData.consecutiveMoveCount = 0;
                }
            }
            else
            {
                implementationData.consecutiveMoveCount = 0;
            }

            if (!usedSelectionChange)
            {
                if (selectedGameObject != null)
                {
                    var data = GetBaseEventData();
                    if ((navigationState.submitButtonDelta & ButtonDeltaState.Pressed) != 0)
                    {
                        submit?.Invoke(selectedGameObject, data);
                        ExecuteEvents.Execute(selectedGameObject, data, ExecuteEvents.submitHandler);
                    }

                    if (!data.used && (navigationState.cancelButtonDelta & ButtonDeltaState.Pressed) != 0)
                    {
                        cancel?.Invoke(selectedGameObject, data);
                        ExecuteEvents.Execute(selectedGameObject, data, ExecuteEvents.cancelHandler);
                    }
                }
            }

            navigationState.implementationData = implementationData;
            navigationState.OnFrameFinished();
        }

        /// <summary>
        /// Remove the <see cref="PointerEventData"/> associated with the given ID.
        /// </summary>
        /// <param name="pointerId">ID of the XR device pointer, mouse pointer or touch registered with the UIInputModule.
        /// Meaning this should correspond to either <see cref="PointerEventData"/>.<c>pointerId</c> or <see cref="TrackedDeviceEventData"/>.<c>pointerId</c>.
        /// </param>
        private protected void RemovePointerEventData(int pointerId)
        {
            if (!m_TrackedDeviceEventByPointerId.Remove(pointerId))
                m_PointerEventByPointerId.Remove(pointerId);
        }

        PointerEventData GetOrCreateCachedPointerEvent(int pointerId)
        {
            if (!m_PointerEventByPointerId.TryGetValue(pointerId, out var result))
            {
                result = new PointerEventData(eventSystem);
                m_PointerEventByPointerId.Add(pointerId, result);
            }

            return result;
        }

        TrackedDeviceEventData GetOrCreateCachedTrackedDeviceEvent(int pointerId)
        {
            if (!m_TrackedDeviceEventByPointerId.TryGetValue(pointerId, out var result))
            {
                result = new TrackedDeviceEventData(eventSystem);
                m_TrackedDeviceEventByPointerId.Add(pointerId, result);
            }

            return result;
        }

        AxisEventData GetOrCreateCachedAxisEvent()
        {
            var result = m_CachedAxisEvent;
            if (result == null)
            {
                result = new AxisEventData(eventSystem);
                m_CachedAxisEvent = result;
            }

            return result;
        }

        static bool CanTargetClickOnDown(GameObject clickOnDownTarget)
        {
            if (clickOnDownTarget == null || !clickOnDownTarget.TryGetComponent<Selectable>(out var selectable))
                return false;

            // Ignore Click UI On Down if there's a IScrollHandler (i.e. Scroll Rect) above the target
            // unless the content is fully visible where scrolling is not necessary.
            // The scroll area would not be draggable if the user poked a button under a Scroll Rect
            // due to the artificial release being immediately triggered, preventing drags if we allowed this.
            //
            // Regarding use of `?.`
            // We can use an explicit null check instead of UnityEngine.Object lifetime check on the parent
            // because we've already ensured the target is not destroyed above, and this is more performant.
            var parent = clickOnDownTarget.transform.parent;
            var scrollHandler = parent?.GetComponentInParent<IScrollHandler>();
            if (scrollHandler != null)
            {
                if (scrollHandler is ScrollRect scrollRect)
                {
                    if (scrollRect.IsActive())
                    {
                        // `IsActive` implies `content` is not null, but just in case do a cheap null check.
                        if (scrollRect.content is null)
                            return false;

                        // Disallow when not fully visible, either vertically or horizontally,
                        // when the Scroll Rect allows scrolling in that axis
                        var contentRect = scrollRect.content.rect;
                        var viewportRect = scrollRect.viewport != null ? scrollRect.viewport.rect : ((RectTransform)scrollRect.transform).rect;

                        if (scrollRect.vertical && contentRect.height > viewportRect.height)
                            return false;

                        if (scrollRect.horizontal && contentRect.width > viewportRect.width)
                            return false;
                    }
                }
                else
                    return false;
            }

            if (selectable is Button || selectable is Toggle || selectable is InputField || selectable is Dropdown)
                return true;

#if TEXTMESHPRO_PACKAGE_PRESENT || (UGUI_2_0_OR_NEWER && UNITY_2023_2_OR_NEWER)
            if (selectable is TMP_InputField || selectable is TMP_Dropdown)
                return true;
#endif

            return false;
        }

#if UNITY_INPUT_SYSTEM_INPUT_MODULE_SCROLL_DELTA // Method was added in 6000.0.11
        /// <summary>
        /// Converts <see cref="PointerEventData.scrollDelta"/> to corresponding number of ticks of the scroll wheel.
        /// </summary>
        public override Vector2 ConvertPointerEventScrollDeltaToTicks(Vector2 scrollDelta)
        {
#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            // A multiplier is not applied to `Input.mouseScrollDelta` when exclusively using the old Input Manager.
            // Since the divisor would be 1, we can just return it.
            return scrollDelta;
#else
            // This is the same const value used by `InputSystemUIInputModule`.
            // When the multiplier is zero, scroll is effectively disabled.
            const float kSmallestScrollDeltaPerTick = 0.00001f;

            // This method is generally invoked by `PanelEventHandler.OnScroll` when processing mouse wheel
            // rather than from an XR tracked device, so the multiplier for non-tracked is used to convert
            // from the scaled value back to the number of ticks of the mouse wheel. This does mean that
            // the conversion may be incorrect, but there is not enough context provided to this method
            // to determine which multiplier should be used, so we assume non-tracked. A potential workaround
            // is to store a class field during `ProcessScrollWheel` when dispatching to the `IScrollHandler`
            // which device source triggered the scroll event, and then use the appropriate multiplier here.
            var multiplier = m_NonTrackedScrollDeltaMultiplier;

            if (Mathf.Abs(multiplier) < kSmallestScrollDeltaPerTick)
                return Vector2.zero;

            return scrollDelta / multiplier;
#endif
        }
#endif
    }
}
