#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
#if UIELEMENTS_MODULE_PRESENT
using UnityEngine.UIElements;
#endif
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Helper struct to contain relevant pointer hit data from UI Toolkit
    /// </summary>
    internal struct PointerHitData
    {
        public Vector3 worldPosition;
        public Quaternion worldOrientation;
        public float hitDistance;
        public Collider hitCollider;
#if UIELEMENTS_MODULE_PRESENT
        public UIDocument hitDocument;
        public VisualElement hitElement;

        public readonly bool TryGetUIDocument(out UIDocument document)
        {
            document = hitDocument;
            if (document != null)
                return true;

            if (hitCollider != null && hitCollider.TryGetComponent(out document))
                return true;

            return false;
        }
#endif
    }

    /// <summary>
    /// Helper struct to contain physical collision data with UI Documents
    /// </summary>
    internal struct InteractorHitData
    {
        public Vector3 closestPoint; // On document collider surface
        public Vector3 interactorOrigin;
        public Vector3 interactorDirection;
#if UIELEMENTS_MODULE_PRESENT
        public UIDocument hitDocument;
#endif
    }

    /// <summary>
    /// Static utility class to assist with processing UI Toolkit events and registration.
    /// </summary>
    internal static class XRUIToolkitHandler
    {
        const int k_MaxInteractors = 8;
        const int k_InvalidIndex = -1;

        // In the future we will likely want to replace this with values passed in from the
        // specific interactor instead of always using a distance of 10, especially in the case
        // of poke, where we are only checking small distances.
        const float k_MaxRaycastDistance = 10f;

        static readonly Vector3 k_ResetPos = new Vector3(0f, -1000f, 0f);

        class InteractorInfo
        {
            public IXRInteractor interactor;
            public int index;
        }

        /// <summary>
        /// Used to determine whether or not to process UI Toolkit events from interactors.
        /// </summary>
        public static bool uiToolkitSupportEnabled { get; set; }

        /// <summary>
        /// Gets the number of registered interactors.
        /// </summary>
        public static int count => s_RegisteredInteractors.Count;

        static readonly Dictionary<IXRInteractor, InteractorInfo> s_RegisteredInteractors = new();
        static readonly Dictionary<IXRInteractor, InteractorHitData> s_InteractorHitData = new();
        static readonly bool[] s_UsedIndices = new bool[k_MaxInteractors];

        static readonly Dictionary<int, bool> s_LastWasDown = new();
        static readonly Dictionary<int, bool> s_WasReset = new();

#if UITOOLKIT_WORLDSPACE_ENABLED
        static PanelInputConfiguration s_PanelInputConfigurationRef;
        static bool s_EventSystemValidated;
        static bool s_PanelInputConfigurationValidated;
        static bool s_DidCheckPanelInputConfiguration;

        /// <summary>
        /// Dictionary tracking which VisualElements are being manipulated by which interactors
        /// </summary>
        static readonly Dictionary<IXRInteractor, VisualElement> s_InteractorElements = new();

        /// <summary>
        /// Dictionary to store the initial z-depth values of manipulated VisualElements
        /// </summary>
        static readonly Dictionary<uint, float> s_InitialZDepth = new Dictionary<uint, float>();

        /// <summary>
        /// Per-interactor hover tracking state for UI Toolkit hover enter/exit events.
        /// </summary>
        struct HoverState
        {
            public VisualElement element;
            public UIDocument document;
            public GameObject uiObject;
        }

        static readonly Dictionary<IXRInteractor, HoverState> s_PreviousHover = new();

        static readonly LinkedPool<UIHoverEventArgs> s_UIHoverEventArgs =
            new LinkedPool<UIHoverEventArgs>(() => new UIHoverEventArgs(), collectionCheck: false);

        static void UpdateHover(IXRInteractor interactor, bool shouldReset)
        {
            if (interactor is not IUIHoverInteractor hoverInteractor)
                return;

            s_PreviousHover.TryGetValue(interactor, out var previousState);

            // If the previously-hovered host is now invalid, force a reset/exit (parity with uGUI behavior).
            // Handles both disabled targets and Unity's "fake null" destroyed-object state.
            if (!shouldReset &&
                ((previousState.uiObject != null && !previousState.uiObject.activeInHierarchy) ||
                 (previousState.uiObject is not null && previousState.uiObject == null)))
            {
                shouldReset = true;
            }

            var currentState = default(HoverState);

            if (!shouldReset && TryGetPointerHitData(interactor, out var hitData))
            {
                var hitElement = hitData.hitElement;
                if (hitElement != null && hitData.TryGetUIDocument(out var hitDocument) && hitDocument.isActiveAndEnabled)
                {
                    currentState.element = hitElement;
                    currentState.document = hitDocument;
                    currentState.uiObject = hitDocument.gameObject;
                }
            }

            if (ReferenceEquals(previousState.element, currentState.element) &&
                ReferenceEquals(previousState.document, currentState.document) &&
                previousState.uiObject == currentState.uiObject)
            {
                return; // No state change
            }

            using (s_UIHoverEventArgs.Get(out var args))
            {
                // We just need the model to send arg data, we don't care if it's invalid
                var uiInteractor = (IUIInteractor)interactor;
                uiInteractor.TryGetUIModel(out var deviceModel);

                if (shouldReset || previousState.element != null)
                {
                    args.interactorObject = hoverInteractor;
                    args.uiSystem = UIHoverEventArgs.UISystem.UIToolkit;
                    args.uiObject = previousState.uiObject;
                    args.visualElement = previousState.element;
                    args.uiDocument = previousState.document;

                    deviceModel.selectableObject = previousState.uiObject;
                    args.deviceModel = deviceModel;

                    hoverInteractor.OnUIHoverExited(args);
                    s_PreviousHover.Remove(interactor);
                }

                if (currentState.element != null)
                {
                    args.interactorObject = hoverInteractor;
                    args.uiSystem = UIHoverEventArgs.UISystem.UIToolkit;
                    args.uiObject = currentState.uiObject;
                    args.visualElement = currentState.element;
                    args.uiDocument = currentState.document;

                    deviceModel.selectableObject = currentState.uiObject;
                    deviceModel.isScrollable = currentState.element is ScrollView || currentState.element.GetFirstAncestorOfType<ScrollView>() != null;
                    args.deviceModel = deviceModel;

                    hoverInteractor.OnUIHoverEntered(args);
                    s_PreviousHover[interactor] = currentState;
                }
            }
        }
#endif

        /// <summary>
        /// Registers an interactor with the XRUIToolkitHandler.
        /// </summary>
        /// <param name="interactor">The interactor to register.</param>
        /// <returns>The index assigned to the interactor.</returns>
        public static int Register(IXRInteractor interactor)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"There was an attempt to register an interactor {interactor} in edit mode. Registration is only allowed during play mode.");
                return k_InvalidIndex;
            }

            // Check if already registered
            if (s_RegisteredInteractors.TryGetValue(interactor, out var registeredInteractor))
                return registeredInteractor.index;

            // Find first available index from within the k_MaxInteractors
            var availableIndex = k_InvalidIndex;
            for (var i = 0; i < k_MaxInteractors; i++)
            {
                if (!s_UsedIndices[i])
                {
                    availableIndex = i;
                    s_UsedIndices[i] = true;
                    break;
                }
            }

            if (availableIndex == k_InvalidIndex)
            {
                Debug.LogError($"No available indices for interactor registration. {count}/{k_MaxInteractors} slots used. " +
                               "This may indicate a registration leak. Check for missing Unregister calls or edit-mode registrations.");

                // Log currently registered interactors
                foreach (var kvp in s_RegisteredInteractors)
                {
                    Debug.LogError($"  - Slot {kvp.Value.index}: {kvp.Key}");
                }

                return k_InvalidIndex;
            }

            var info = new InteractorInfo
            {
                interactor = interactor,
                index = availableIndex,
            };

            s_RegisteredInteractors.Add(interactor, info);
            return availableIndex;
#else
            return k_InvalidIndex;
#endif
        }

        /// <summary>
        /// Unregisters an interactor from the XRUIToolkitHandler and cleans up its resources.
        /// </summary>
        /// <param name="interactor">The interactor to unregister.</param>
        public static void Unregister(IXRInteractor interactor)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            // Allow unregister to be performed in both playmode and edit mode in order to clean up spurious registrations
            if (!s_RegisteredInteractors.TryGetValue(interactor, out var info))
                return;

            // Ensure one hover exit is emitted if we were hovering
            UpdateHover(interactor, true);

            // Clean up all associated state
            s_LastWasDown.Remove(info.index);
            s_WasReset.Remove(info.index);
            s_UsedIndices[info.index] = false;
            s_RegisteredInteractors.Remove(interactor);
            s_InteractorHitData.Remove(interactor);

            // Clean up any depth changes made by this interactor
            ClearZDepthForInteractor(interactor);
#endif
        }

        /// <summary>
        /// Attempts to get the pointer index for a registered interactor.
        /// </summary>
        /// <param name="interactor">The interactor to get the index for.</param>
        /// <param name="index">The output index if found.</param>
        /// <returns>True if the interactor was found, false otherwise.</returns>
        public static bool TryGetPointerIndex(IXRInteractor interactor, out int index)
        {
            if (s_RegisteredInteractors.TryGetValue(interactor, out var info))
            {
                index = info.index;
                return true;
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Updates the hit data for an interactor.
        /// </summary>
        /// <param name="interactor">The interactor to update hit data for.</param>
        /// <param name="hitData">The hit data to update with.</param>
        public static void UpdateInteractorHitData(IXRInteractor interactor, InteractorHitData hitData)
        {
            s_InteractorHitData[interactor] = hitData;
        }

        /// <summary>
        /// Attempts to get the hit data for an interactor.
        /// </summary>
        /// <param name="interactor">The interactor to get hit data for.</param>
        /// <param name="hitData">The output hit data if found.</param>
        /// <returns>True if hit data was found, false otherwise.</returns>
        public static bool TryGetInteractorHitData(IXRInteractor interactor, out InteractorHitData hitData)
        {
            return s_InteractorHitData.TryGetValue(interactor, out hitData);
        }

        /// <summary>
        /// Clears the hit data for an interactor.
        /// </summary>
        /// <param name="interactor">The interactor to clear hit data for.</param>
        public static void ClearInteractorHitData(IXRInteractor interactor)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            ClearZDepthForInteractor(interactor);
#endif
            s_InteractorHitData.Remove(interactor);
        }

        /// <summary>
        /// Clears all registered interactors and associated data.
        /// </summary>
        public static void Clear()
        {
            s_RegisteredInteractors.Clear();
            s_LastWasDown.Clear();
            s_WasReset.Clear();
            s_InteractorHitData.Clear();
#if UITOOLKIT_WORLDSPACE_ENABLED
            s_InteractorElements.Clear(); // Clear interactor-element associations
            s_PreviousHover.Clear();
#endif
            for (int i = 0; i < k_MaxInteractors; i++)
            {
                s_UsedIndices[i] = false;
            }
        }

        /// <summary>
        /// Checks if an interactor is registered.
        /// </summary>
        /// <param name="interactor">The interactor to check.</param>
        /// <returns>True if the interactor is registered, false otherwise.</returns>
        public static bool IsRegistered(IXRInteractor interactor)
        {
            return s_RegisteredInteractors.ContainsKey(interactor);
        }

        /// <summary>
        /// Handles pointer updates for an interactor.
        /// </summary>
        /// <param name="interactor">The interactor to update.</param>
        /// <param name="pos">The position of the interactor.</param>
        /// <param name="rot">The rotation of the interactor.</param>
        /// <param name="scrollDelta">The amount to scroll based on input.</param>
        /// <param name="isUiSelectInputActive">Whether the UI select input is active.</param>
        /// <param name="shouldReset">Whether the pointer should be reset.</param>
        public static void HandlePointerUpdate(
            IXRInteractor interactor,
            Vector3 pos,
            Quaternion rot,
            Vector2 scrollDelta,
            bool isUiSelectInputActive,
            bool shouldReset)
        {
            if (!TryGetPointerIndex(interactor, out var pointerIndex))
                return;

            // Ensure dictionary entries exist
            s_LastWasDown.TryAdd(pointerIndex, false);
            // Use shouldReset as initial TryAdd entry to skip reset call if it is the first call.
            s_WasReset.TryAdd(pointerIndex, shouldReset);

            // If we're in a reset state and we already reset last time, return early
            if (shouldReset && s_WasReset[pointerIndex])
                return;

            // Validate the panel input configuration.
            // Check for UI Toolkit EventSystem interoperability upon first process.
            // Wait to do this until as late as possible (i.e., not upon startup or Register) since
            // the PanelInputConfiguration.current may not be assigned yet.
            if (ShouldCheckPanelInputConfigurationValidation())
                ValidatePanelInputConfiguration();

            var eventPos = shouldReset ? k_ResetPos : pos;
            var eventRot = shouldReset ? Quaternion.identity : rot;

#if UITOOLKIT_WORLDSPACE_ENABLED
            // Handle pointer movement
            var moveEvent = new InputForUI.PointerEvent
            {
                pointerIndex = pointerIndex,
                type = InputForUI.PointerEvent.Type.PointerMoved,
                worldPosition = eventPos,
                worldOrientation = eventRot,
                eventSource = InputForUI.EventSource.TrackedDevice,
                maxDistance = k_MaxRaycastDistance,
            };
            InputForUI.EventProvider.Dispatch(InputForUI.Event.From(moveEvent));

            // Handle button states
            bool currentPressed = !shouldReset && isUiSelectInputActive;
            if (currentPressed != s_LastWasDown[pointerIndex])
            {
                s_LastWasDown[pointerIndex] = currentPressed;
                var eventType = currentPressed ? InputForUI.PointerEvent.Type.ButtonPressed : InputForUI.PointerEvent.Type.ButtonReleased;

                var buttonEvent = new InputForUI.PointerEvent
                {
                    pointerIndex = pointerIndex,
                    type = eventType,
                    button = InputForUI.PointerEvent.Button.Primary,
                    clickCount = 1,
                    worldPosition = eventPos,
                    worldOrientation = eventRot,
                    eventSource = InputForUI.EventSource.TrackedDevice,
                    maxDistance = k_MaxRaycastDistance,
                };
                InputForUI.EventProvider.Dispatch(InputForUI.Event.From(buttonEvent));
            }

            // Handle scroll
            if (scrollDelta != Vector2.zero)
            {
                var scrollEvent = new InputForUI.PointerEvent
                {
                    pointerIndex = pointerIndex,
                    type = InputForUI.PointerEvent.Type.Scroll,
                    worldPosition = eventPos,
                    worldOrientation = eventRot,
                    eventSource = InputForUI.EventSource.TrackedDevice,
                    maxDistance = k_MaxRaycastDistance,
                    scroll = -scrollDelta
                };
                InputForUI.EventProvider.Dispatch(InputForUI.Event.From(scrollEvent));
            }
#endif

            // Update reset state
            s_WasReset[pointerIndex] = shouldReset;

#if UITOOLKIT_WORLDSPACE_ENABLED
            // Raise XRI hover enter/exit events for UI Toolkit
            UpdateHover(interactor, shouldReset);
#endif

            if (shouldReset)
                ClearInteractorHitData(interactor);
        }

        /// <summary>
        /// Tries to get the current pointer hit data for the specified interactor
        /// </summary>
        /// <param name="interactor">The interactor to get hit data for</param>
        /// <param name="hitData">The hit data if successful</param>
        /// <returns>True if hit data was successfully retrieved</returns>
        public static bool TryGetPointerHitData(IXRInteractor interactor, out PointerHitData hitData)
        {
            hitData = default;

#if UITOOLKIT_WORLDSPACE_ENABLED
            if (!TryGetPointerIndex(interactor, out var pointerIndex))
                return false;

            PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(PointerId.trackedPointerIdBase + pointerIndex);
            if (trackedState == null)
                return false;

            hitData = new PointerHitData
            {
                worldPosition = trackedState.worldPosition,
                worldOrientation = trackedState.worldOrientation,
                hitDistance = trackedState.hit.distance,
                hitCollider = trackedState.hit.collider,
                hitDocument = trackedState.hit.document,
                hitElement = trackedState.hit.element,
            };

            return true;
#else
            return false;
#endif
        }

#if UITOOLKIT_WORLDSPACE_ENABLED || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// Sets the z-depth for a VisualElement associated with a specific interactor.
        /// </summary>
        /// <param name="ve">The VisualElement to set the z-depth for.</param>
        /// <param name="interactor">The interactor that's manipulating this element.</param>
        /// <param name="remainingPokePercentage">The remaining percentages of poke interaction on a scale of 1 (not started) to 0 (complete).</param>
        /// <returns>The new Z value after setting the depth.</returns>
        public static float SetZDepthForInteractor(VisualElement ve, IXRInteractor interactor, float remainingPokePercentage)
        {
            // If new element is being tracked, reset the z-depth before updating reference
            if (s_InteractorElements.TryGetValue(interactor, out var element) && element != null && element.controlid != ve.controlid)
                ClearZDepthForInteractor(interactor);

            // Track which interactor is working with this element
            s_InteractorElements[interactor] = ve;

            // Use resolvedStyle to capture the true rendered initial z-depth value. VisualElement.style translate is 0 on initial call.
            var resolvedStyleTranslateValue = ve.resolvedStyle.translate;
            s_InitialZDepth.TryAdd(ve.controlid, resolvedStyleTranslateValue.z);

            // Calculate new Z position based on poke strength and initial depth
            var newZPosition = s_InitialZDepth[ve.controlid] * remainingPokePercentage; // Calculates new z position from the initial depth (no poke) to 0 (poke complete)

            // Apply the new Z position
            ve.style.translate = new Translate(resolvedStyleTranslateValue.x, resolvedStyleTranslateValue.y, newZPosition);
            return newZPosition;
        }
#endif

#if UITOOLKIT_WORLDSPACE_ENABLED || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// Resets the z-depth for a VisualElement to its original value.
        /// </summary>
        /// <param name="ve">The VisualElement to reset.</param>
        /// <returns>The original z-depth value.</returns>
        static float ResetDepth(VisualElement ve)
        {
            var translateVal = ve.style.translate.value;

            // Use the stored initial value if available
            if (s_InitialZDepth.TryGetValue(ve.controlid, out var originalZ))
            {
                ve.style.translate = new Translate(translateVal.x.value, translateVal.y.value, originalZ);
            }
            else
            {
                // Otherwise reset to 0
                ve.style.translate = new Translate(translateVal.x.value, translateVal.y.value, 0f);
            }

            return originalZ;
        }
#endif

#if UITOOLKIT_WORLDSPACE_ENABLED || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// Clears z-depth for all VisualElements associated with the specified interactor,
        /// resetting them to their original depths.
        /// </summary>
        /// <param name="interactor">The interactor whose UI element depths should be reset.</param>
        public static void ClearZDepthForInteractor(IXRInteractor interactor)
        {
            if (s_InteractorElements.TryGetValue(interactor, out var element) && element != null)
            {
                ResetDepth(element);
                s_InteractorElements.Remove(interactor);
            }
        }
#endif

        /// <summary>
        /// Trigger updates to the Event System
        /// </summary>
        public static void UpdateEventSystem()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (count > 0)
                UIElementsRuntimeUtility.UpdateEventSystem();
#endif
        }

        /// <summary>
        /// Takes a set of colliders and checks if the first one in the list has a
        /// UI Document component.
        /// </summary>
        /// <param name="colliders">List of type <see cref="Collider"/>.</param>
        /// <returns>Will return <see langword="true"/> if a UI Document is found on the first collider in the list.</returns>
        public static bool IsValidUIToolkitInteraction(List<Collider> colliders)
        {
            return colliders.Count > 0 && HasUIDocument(colliders[0]);
        }

        /// <summary>
        /// Takes a single collider and checks to see if that collider has a UI Document component.
        /// </summary>
        /// <param name="collider">The <see cref="Collider"/> to check.</param>
        /// <returns>Will return <see langword="true"/> if a UI Document is found on the <paramref name="collider"/>.</returns>
        public static bool HasUIDocument(Collider collider)
        {
#if UIELEMENTS_MODULE_PRESENT
            return collider.TryGetComponent(out UIDocument _);
#else
            return false;
#endif
        }

        /// <summary>
        /// Checks the need for <see cref="EventSystem"/> and <see cref="PanelInputConfiguration"/> validation.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the <see cref="EventSystem"/> or the <see cref="PanelInputConfiguration"/> have not been validated or checked, or
        /// if the <see cref="PanelInputConfiguration.current"/> has changed. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// This does not check or indicate need for revalidation if the existing <see cref="PanelInputConfiguration.panelInputRedirection"/>
        /// has changed and is now invalid.
        /// </remarks>
        static bool ShouldCheckPanelInputConfigurationValidation()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            // Re-validate panel input configuration if it has changed
            if (s_PanelInputConfigurationRef != PanelInputConfiguration.current)
            {
                s_EventSystemValidated = false;
                s_PanelInputConfigurationValidated = false;
                s_DidCheckPanelInputConfiguration = false;
                return true;
            }

            var didCheckConfiguration = s_DidCheckPanelInputConfiguration || (s_EventSystemValidated && s_PanelInputConfigurationValidated);
            return !didCheckConfiguration;
#else
            return false;
#endif
        }

        /// <summary>
        /// Validates the <see cref="EventSystem"/> and <see cref="PanelInputConfiguration"/> for UI Toolkit support.
        /// </summary>
        static void ValidatePanelInputConfiguration()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            s_DidCheckPanelInputConfiguration = true;
            s_PanelInputConfigurationValidated = false;

            // Validate event system
            if (!s_EventSystemValidated && EventSystem.current == null)
                return;

            s_EventSystemValidated = true;

            // Validate Panel Input Configuration
            var panelInputConfiguration = PanelInputConfiguration.current;

            // Keeps a reference to current panel input configuration to re-validate if panel input configuration changes
            s_PanelInputConfigurationRef = panelInputConfiguration;

            if (panelInputConfiguration == null)
            {
                // Warn user if a Panel Input Configuration was null and could not be found
                Debug.LogWarning("Detected an Event System component that could interfere with UI Toolkit input." +
                    " Create a Panel Input Configuration component and configured it by setting Panel Input Redirection to No input redirection to prevent interactions with the Event System.");
            }
            else if (panelInputConfiguration.panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Never)
            {
                // Warn user if the detected Panel Input Configuration is found but is improperly configured
                Debug.LogWarning("Detected an Event System component that could interfere with UI Toolkit input." +
                    " Configure your Panel Input Configuration component to set Panel Input Redirection to No input redirection to prevent interactions with the Event System.");
            }
            else
            {
                s_PanelInputConfigurationValidated = true;
            }
#endif
        }
    }
}
