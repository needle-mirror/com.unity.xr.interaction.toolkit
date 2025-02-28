using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Matches the UI Model to the state of the Interactor.
    /// </summary>
    public interface IUIInteractor
    {
        /// <summary>
        /// Updates the current UI Model to match the state of the Interactor.
        /// </summary>
        /// <param name="model">The returned model that will match this Interactor.</param>
        void UpdateUIModel(ref TrackedDeviceModel model);

        /// <summary>
        /// Attempts to retrieve the current UI Model.
        /// </summary>
        /// <param name="model">The returned model that reflects the UI state of this Interactor.</param>
        /// <returns>Returns <see langword="true"/> if the model was retrieved. Otherwise, returns <see langword="false"/>.</returns>
        bool TryGetUIModel(out TrackedDeviceModel model);
    }

    /// <summary>
    /// Matches the UI Model to the state of the Interactor with support for hover events.
    /// </summary>
    public interface IUIHoverInteractor : IUIInteractor
    {
        /// <summary>
        /// The event that is called when the Interactor begins hovering over a UI element.
        /// </summary>
        /// <remarks>
        /// The <see cref="UIHoverEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        UIHoverEnterEvent uiHoverEntered { get; }

        /// <summary>
        /// The event that is called when this Interactor ends hovering over a UI element.
        /// </summary>
        /// <remarks>
        /// The <see cref="UIHoverEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        UIHoverExitEvent uiHoverExited { get; }

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method when the Interactor begins hovering over a UI element.
        /// </summary>
        /// <param name="args">Event data containing the UI element that is being hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnUIHoverExited(UIHoverEventArgs)"/>
        void OnUIHoverEntered(UIHoverEventArgs args);

        /// <summary>
        /// The <see cref="XRUIInputModule"/> calls this method when the Interactor ends hovering over a UI element.
        /// </summary>
        /// <param name="args">Event data containing the UI element that is no longer hovered over.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="OnUIHoverEntered(UIHoverEventArgs)"/>
        void OnUIHoverExited(UIHoverEventArgs args);
    }

    /// <summary>
    /// Custom class for input modules that send UI input in XR.
    /// </summary>
    [AddComponentMenu("Event/XR UI Input Module", 11)]
    [HelpURL(XRHelpURLConstants.k_XRUIInputModule)]
    public partial class XRUIInputModule : UIInputModule
    {
        struct RegisteredInteractor
        {
            public IUIInteractor interactor;
            public TrackedDeviceModel model;
            internal bool deactivating;
            internal bool active;

            public RegisteredInteractor(IUIInteractor interactor, int deviceIndex)
            {
                this.interactor = interactor;
                model = new TrackedDeviceModel(deviceIndex)
                {
                    interactor = interactor,
                };
                active = true;
                deactivating = false;
            }
        }

        struct RegisteredTouch
        {
            public bool isValid;
            public int touchId;
            public TouchModel model;

            public RegisteredTouch(Touch touch, int deviceIndex)
            {
                touchId = touch.fingerId;
                model = new TouchModel(deviceIndex);
                isValid = true;
            }
        }

        /// <summary>
        /// Represents which Active Input Mode will be used in the situation where the Active Input Handling project setting is set to Both.
        /// </summary>
        /// <seealso cref="activeInputMode"/>
        public enum ActiveInputMode
        {
            /// <summary>
            /// Only use input polled through the built-in Unity Input Manager (Old).
            /// </summary>
            InputManagerBindings,

            /// <summary>
            /// Only use input polled from <see cref="InputActionReference"/> through the newer Input System package.
            /// </summary>
            InputSystemActions,

            /// <summary>
            /// Scan through input from both Unity Input Manager and Input System action references.
            /// Note: This may cause undesired effects or may impact performance if input configuration is duplicated.
            /// </summary>
            Both,
        }

        [HideInInspector]
        [SerializeField]
        ActiveInputMode m_ActiveInputMode;

        /// <summary>
        /// Configures which Active Input Mode will be used in the situation where the Active Input Handling project setting is set to Both.
        /// </summary>
        /// <seealso cref="ActiveInputMode"/>
        [Obsolete("activeInputMode has been deprecated in version 3.1.0. Input System Package (New) will be the default input handling mode used when active input handling is set to Both.")]
        public ActiveInputMode activeInputMode
        {
            get => m_ActiveInputMode;
            set => m_ActiveInputMode = value;
        }

        [Header("Input Devices")]
        [SerializeField]
        [Tooltip("If true, will forward 3D tracked device data to UI elements.")]
        bool m_EnableXRInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 3D tracked device data to UI elements.
        /// </summary>
        public bool enableXRInput
        {
            get => m_EnableXRInput;
            set => m_EnableXRInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward 2D mouse data to UI elements. Ignored when any Input System UI Actions are used.")]
        bool m_EnableMouseInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 2D mouse data to UI elements. Ignored when any Input System UI Actions are used.
        /// </summary>
        public bool enableMouseInput
        {
            get => m_EnableMouseInput;
            set => m_EnableMouseInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward 2D touch data to UI elements. Ignored when any Input System UI Actions are used.")]
        bool m_EnableTouchInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 2D touch data to UI elements. Ignored when any Input System UI Actions are used.
        /// </summary>
        public bool enableTouchInput
        {
            get => m_EnableTouchInput;
            set => m_EnableTouchInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward gamepad data to UI elements. Ignored when any Input System UI Actions are used.")]
        bool m_EnableGamepadInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward gamepad data to UI elements. Ignored when any Input System UI Actions are used.
        /// </summary>
        public bool enableGamepadInput
        {
            get => m_EnableGamepadInput;
            set => m_EnableGamepadInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward joystick data to UI elements. Ignored when any Input System UI Actions are used.")]
        bool m_EnableJoystickInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward joystick data to UI elements. Ignored when any Input System UI Actions are used.
        /// </summary>
        public bool enableJoystickInput
        {
            get => m_EnableJoystickInput;
            set => m_EnableJoystickInput = value;
        }

#if ENABLE_INPUT_SYSTEM
        [Header("Input System UI Actions")]
#else
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Pointer input action reference, such as a mouse or single-finger touch device.")]
        InputActionReference m_PointAction;
        /// <summary>
        /// The Input System action to use to move the pointer on the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference pointAction
        {
            get => m_PointAction;
            set => SetInputAction(ref m_PointAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Left-click input action reference, typically the left button on a mouse.")]
        InputActionReference m_LeftClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the left button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference leftClickAction
        {
            get => m_LeftClickAction;
            set => SetInputAction(ref m_LeftClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Middle-click input action reference, typically the middle button on a mouse.")]
        InputActionReference m_MiddleClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the middle button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference middleClickAction
        {
            get => m_MiddleClickAction;
            set => SetInputAction(ref m_MiddleClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Right-click input action reference, typically the right button on a mouse.")]
        InputActionReference m_RightClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the right button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference rightClickAction
        {
            get => m_RightClickAction;
            set => SetInputAction(ref m_RightClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Scroll wheel input action reference, typically the scroll wheel on a mouse.")]
        InputActionReference m_ScrollWheelAction;
        /// <summary>
        /// The Input System action to use to move the pointer on the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference scrollWheelAction
        {
            get => m_ScrollWheelAction;
            set => SetInputAction(ref m_ScrollWheelAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Navigation input action reference will change which UI element is currently selected to the one up, down, left of or right of the currently selected one.")]
        InputActionReference m_NavigateAction;
        /// <summary>
        /// The Input System action to use to navigate the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference navigateAction
        {
            get => m_NavigateAction;
            set => SetInputAction(ref m_NavigateAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Submit input action reference will trigger a submission of the currently selected UI in the Event System.")]
        InputActionReference m_SubmitAction;
        /// <summary>
        /// The Input System action to use for submitting or activating a UI element. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference submitAction
        {
            get => m_SubmitAction;
            set => SetInputAction(ref m_SubmitAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Cancel input action reference will trigger canceling out of the currently selected UI in the Event System.")]
        InputActionReference m_CancelAction;
        /// <summary>
        /// The Input System action to use for cancelling or backing out of a UI element. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference cancelAction
        {
            get => m_CancelAction;
            set => SetInputAction(ref m_CancelAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("When enabled, built-in Input System actions will be used if no Input System UI Actions are assigned.")]
        bool m_EnableBuiltinActionsAsFallback = true;
        /// <summary>
        /// When enabled, built-in Input System actions will be used if no Input System UI Actions are assigned. This uses the
        /// currently enabled Input System devices: <see cref="Mouse.current"/>, <see cref="Touchscreen.current"/>, <see cref="Gamepad.current"/>, and <see cref="Joystick.current"/>.
        /// </summary>
        public bool enableBuiltinActionsAsFallback
        {
            get => m_EnableBuiltinActionsAsFallback;
            set
            {
                m_EnableBuiltinActionsAsFallback = value;
                m_UseBuiltInInputSystemActions = m_EnableBuiltinActionsAsFallback && !InputActionReferencesAreSet();
            }
        }

#if !ENABLE_LEGACY_INPUT_MANAGER || ENABLE_INPUT_SYSTEM
        [HideInInspector]
#else
        [Header("Input Manager (Old) Gamepad/Joystick Bindings")]
#endif
        [SerializeField]
        [Tooltip("Name of the horizontal axis for gamepad/joystick UI navigation when using the old Input Manager.")]
        string m_HorizontalAxis = "Horizontal";

        /// <summary>
        /// Name of the horizontal axis for UI navigation when using the old Input Manager.
        /// </summary>
        public string horizontalAxis
        {
            get => m_HorizontalAxis;
            set => m_HorizontalAxis = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER || ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the vertical axis for gamepad/joystick UI navigation when using the old Input Manager.")]
        string m_VerticalAxis = "Vertical";

        /// <summary>
        /// Name of the vertical axis for UI navigation when using the old Input Manager.
        /// </summary>
        public string verticalAxis
        {
            get => m_VerticalAxis;
            set => m_VerticalAxis = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER || ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the gamepad/joystick button to use for UI selection or submission when using the old Input Manager.")]
        string m_SubmitButton = "Submit";

        /// <summary>
        /// Name of the gamepad/joystick button to use for UI selection or submission when using the old Input Manager.
        /// </summary>
        public string submitButton
        {
            get => m_SubmitButton;
            set => m_SubmitButton = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER || ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the gamepad/joystick button to use for UI cancel or back commands when using the old Input Manager.")]
        string m_CancelButton = "Cancel";

        /// <summary>
        /// Name of the gamepad/joystick button to use for UI cancel or back commands when using the old Input Manager.
        /// </summary>
        public string cancelButton
        {
            get => m_CancelButton;
            set => m_CancelButton = value;
        }

        // Initialize to 1 so mouse always uses pointer ID of 0
        int m_RollingPointerId = 1;
        Stack<int> m_DeletedPointerIds = new Stack<int>();
        bool m_UseBuiltInInputSystemActions;

        PointerModel m_PointerState;
        NavigationModel m_NavigationState;

        internal const float kPixelPerLine = 20f;

        readonly List<RegisteredTouch> m_RegisteredTouches = new List<RegisteredTouch>();
        readonly List<RegisteredInteractor> m_RegisteredInteractors = new List<RegisteredInteractor>();

        // Reusable event args
        readonly LinkedPool<UIHoverEventArgs> m_UIHoverEventArgs = new LinkedPool<UIHoverEventArgs>(() => new UIHoverEventArgs(), collectionCheck: false);

        /// <summary>
        /// See <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html">MonoBehavior.OnEnable</a>.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // Check active input mode is correct
#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            m_ActiveInputMode = ActiveInputMode.InputManagerBindings;
#else
            m_ActiveInputMode = ActiveInputMode.InputSystemActions;
#endif
            m_PointerState = new PointerModel(0);

            m_NavigationState = new NavigationModel();

            m_UseBuiltInInputSystemActions = m_EnableBuiltinActionsAsFallback && !InputActionReferencesAreSet();

            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
                EnableAllActions();
        }

        /// <summary>
        /// See <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html">MonoBehavior.OnDisable</a>.
        /// </summary>
        protected override void OnDisable()
        {
            RemovePointerEventData(m_PointerState.pointerId);

            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
                DisableAllActions();

            base.OnDisable();
        }

        /// <summary>
        /// Register an <see cref="IUIInteractor"/> with the UI system.
        /// Calling this will enable it to start interacting with UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to use.</param>
        public void RegisterInteractor(IUIInteractor interactor)
        {
            if (interactor == null)
                return;

            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                var registeredInteractor = m_RegisteredInteractors[i];
                if (registeredInteractor.interactor == interactor)
                {
                    // If it was previously disabled by deactivation, reactivate
                    if (!registeredInteractor.active)
                    {
                        registeredInteractor.active = true;
                        registeredInteractor.deactivating = false;
                        registeredInteractor.model.Reset(true);
                        m_RegisteredInteractors[i] = registeredInteractor;
                    }
                    return;
                }
            }

            if (!m_DeletedPointerIds.TryPop(out var newId))
                newId = m_RollingPointerId++;

            m_RegisteredInteractors.Add(new RegisteredInteractor(interactor, newId));
        }

        /// <summary>
        /// Unregisters an <see cref="IUIInteractor"/> with the UI system.
        /// This cancels all UI Interaction and makes the <see cref="IUIInteractor"/> no longer able to affect UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to stop using.</param>
        public void UnregisterInteractor(IUIInteractor interactor)
        {
            if (interactor == null)
                return;

            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                var registeredInteractor = m_RegisteredInteractors[i];
                if (registeredInteractor.interactor == interactor)
                {
                    if (registeredInteractor.active)
                    {
                        registeredInteractor.deactivating = true;
                        registeredInteractor.active = false;
                        m_RegisteredInteractors[i] = registeredInteractor;
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="IUIInteractor"/> from its corresponding Unity UI Pointer Id.
        /// This can be used to identify individual Interactors from the underlying UI Events.
        /// </summary>
        /// <param name="pointerId">A unique integer representing an object that can point at UI.</param>
        /// <returns>Returns the interactor associated with <paramref name="pointerId"/>.
        /// Returns <see langword="null"/> if no Interactor is associated (e.g. if it's a mouse event).</returns>
        public IUIInteractor GetInteractor(int pointerId)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].model.pointerId == pointerId && m_RegisteredInteractors[i].active)
                {
                    return m_RegisteredInteractors[i].interactor;
                }
            }

            return null;
        }

        /// <summary>Retrieves the UI Model for a selected <see cref="IUIInteractor"/>.</summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> you want the model for.</param>
        /// <param name="model">The returned model that reflects the UI state of the <paramref name="interactor"/>.</param>
        /// <returns>Returns <see langword="true"/> if the model was retrieved. Otherwise, returns <see langword="false"/>.</returns>
        public bool GetTrackedDeviceModel(IUIInteractor interactor, out TrackedDeviceModel model)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                {
                    model = m_RegisteredInteractors[i].model;
                    return true;
                }
            }

            model = new TrackedDeviceModel(-1);
            return false;
        }

        /// <inheritdoc />
        protected override void DoProcess()
        {
            if (m_EnableXRInput)
            {
                for (var i = 0; i < m_RegisteredInteractors.Count; i++)
                {
                    var registeredInteractor = m_RegisteredInteractors[i];

                    var oldTarget = registeredInteractor.model.implementationData.pointerTarget;

                    // If device is removed, we send a default state to unclick any UI
                    var isDestroyedUnityObject = registeredInteractor.interactor is Object unityObject && unityObject == null;
                    if (isDestroyedUnityObject || registeredInteractor.deactivating)
                    {
                        registeredInteractor.model.Reset(false);
                        ProcessTrackedDevice(ref registeredInteractor.model, true);
                        RemovePointerEventData(registeredInteractor.model.pointerId);
                        if (isDestroyedUnityObject)
                        {
                            m_DeletedPointerIds.Push(registeredInteractor.model.pointerId);
                            m_RegisteredInteractors.RemoveAt(i--);
                            continue;
                        }
                        else
                        {
                            registeredInteractor.deactivating = false;
                            registeredInteractor.model.Reset(true);
                            m_RegisteredInteractors[i] = registeredInteractor;
                        }
                    }
                    else if (!registeredInteractor.active)
                    {
                        continue;
                    }
                    else
                    {
                        registeredInteractor.interactor.UpdateUIModel(ref registeredInteractor.model);
                        ProcessTrackedDevice(ref registeredInteractor.model);

                        // Some poke logic happens during the Raycast call in ProcessTrackedDevice
                        // and requires an additional update to the select field of the UI Model
                        registeredInteractor.model.UpdatePokeSelectState();
                        m_RegisteredInteractors[i] = registeredInteractor;
                    }

                    // If hover target changed, send event
                    var newTarget = registeredInteractor.model.implementationData.pointerTarget;
                    if (oldTarget != newTarget)
                    {
                        using (m_UIHoverEventArgs.Get(out var args))
                        {
                            args.interactorObject = registeredInteractor.interactor;
                            args.deviceModel = registeredInteractor.model;
                            if (args.interactorObject != null && args.interactorObject is IUIHoverInteractor hoverInteractor)
                            {
                                if (oldTarget != null)
                                {
                                    args.uiObject = oldTarget;
                                    hoverInteractor.OnUIHoverExited(args);
                                }

                                if (newTarget != null && newTarget.activeInHierarchy)
                                {
                                    args.uiObject = newTarget;
                                    hoverInteractor.OnUIHoverEntered(args);
                                }
                            }
                        }
                    }

                    // In the case where the target is disabled while being hovered, this
                    // will fire the appropriate event to any listeners as well as reset the
                    // current model to prevent stale data from causing unwanted behavior
                    if ((oldTarget != null && !oldTarget.activeInHierarchy) || (oldTarget is not null && oldTarget == null))
                    {
                        using (m_UIHoverEventArgs.Get(out var args))
                        {
                            if (oldTarget == newTarget)
                            {
                                registeredInteractor.model.Reset(true);
                                m_RegisteredInteractors[i] = registeredInteractor;
                            }

                            var interactor = registeredInteractor.interactor;
                            if (interactor != null && interactor is IUIHoverInteractor hoverInteractor)
                            {
                                args.interactorObject = interactor;
                                args.uiObject = oldTarget;
                                args.deviceModel = registeredInteractor.model;
                                hoverInteractor.OnUIHoverExited(args);
                            }
                        }
                    }
                }
            }

#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            // Touch needs to take precedence because of the mouse emulation layer
            if (m_ActiveInputMode != ActiveInputMode.InputSystemActions)
            {
                var hasTouches = false;

                if (m_EnableTouchInput)
                    hasTouches = ProcessLegacyTouches();

                if (m_EnableMouseInput && !hasTouches)
                    GetLegacyMouseState();

                GetLegacyNavigationState();
            }
#endif

            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
                GetPointerStates();

            ProcessPointerState(ref m_PointerState);
            ProcessNavigationState(ref m_NavigationState);
        }

        void GetPointerStates()
        {
            if (m_UseBuiltInInputSystemActions)
            {
                if (m_EnableTouchInput && Touchscreen.current != null)
                {
                    m_PointerState.position = Touchscreen.current.position.ReadValue();
#if UNITY_2022_3_OR_NEWER
                    m_PointerState.displayIndex = Touchscreen.current.displayIndex.ReadValue();
#endif
                }

                if (m_EnableMouseInput && Mouse.current != null)
                {
                    m_PointerState.position = Mouse.current.position.ReadValue();
#if UNITY_2022_3_OR_NEWER
                    m_PointerState.displayIndex = Mouse.current.displayIndex.ReadValue();
#endif
                    m_PointerState.scrollDelta = Mouse.current.scroll.ReadValue() * (1 / kPixelPerLine);
                    m_PointerState.leftButtonPressed = Mouse.current.leftButton.isPressed;
                    m_PointerState.rightButtonPressed = Mouse.current.rightButton.isPressed;
                    m_PointerState.middleButtonPressed = Mouse.current.middleButton.isPressed;
                }

                if (m_EnableGamepadInput && Gamepad.current != null)
                {
                    // Combine left stick and dpad for navigation movement
                    m_NavigationState.move = Gamepad.current.leftStick.ReadValue() + Gamepad.current.dpad.ReadValue();
                    m_NavigationState.submitButtonDown = Gamepad.current.buttonSouth.isPressed;
                    m_NavigationState.cancelButtonDown = Gamepad.current.buttonEast.isPressed;
                }

                if (m_EnableJoystickInput && Joystick.current != null)
                {
                    // Combine main joystick and hatswitch for navigation movement
                    m_NavigationState.move = Joystick.current.stick.ReadValue() +
                        (Joystick.current.hatswitch != null ? Joystick.current.hatswitch.ReadValue() : Vector2.zero);
                    m_NavigationState.submitButtonDown = Joystick.current.trigger.isPressed;
                    // This will always be false until we can rely on a secondary button from the joystick
                    m_NavigationState.cancelButtonDown = false;
                }
            }
            else
            {
                if (IsActionEnabled(m_PointAction))
                {
                    m_PointerState.position = m_PointAction.action.ReadValue<Vector2>();
#if UNITY_2022_3_OR_NEWER
                    m_PointerState.displayIndex = GetDisplayIndexFor(m_PointAction.action.activeControl);
#endif
                }
                if (IsActionEnabled(m_ScrollWheelAction))
                    m_PointerState.scrollDelta = m_ScrollWheelAction.action.ReadValue<Vector2>() * (1 / kPixelPerLine);
                if (IsActionEnabled(m_LeftClickAction))
                    m_PointerState.leftButtonPressed = m_LeftClickAction.action.IsPressed();
                if (IsActionEnabled(m_RightClickAction))
                    m_PointerState.rightButtonPressed = m_RightClickAction.action.IsPressed();
                if (IsActionEnabled(m_MiddleClickAction))
                    m_PointerState.middleButtonPressed = m_MiddleClickAction.action.IsPressed();

                if (IsActionEnabled(m_NavigateAction))
                    m_NavigationState.move = m_NavigateAction.action.ReadValue<Vector2>();
                if (IsActionEnabled(m_SubmitAction))
                    m_NavigationState.submitButtonDown = m_SubmitAction.action.WasPerformedThisFrame();
                if (IsActionEnabled(m_CancelAction))
                    m_NavigationState.cancelButtonDown = m_CancelAction.action.WasPerformedThisFrame();
            }
        }

#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
        void GetLegacyMouseState()
        {
            if (Input.mousePresent)
            {
                m_PointerState.position = Input.mousePosition;
                m_PointerState.scrollDelta = Input.mouseScrollDelta;
                m_PointerState.leftButtonPressed = Input.GetMouseButton(0);
                m_PointerState.rightButtonPressed = Input.GetMouseButton(1);
                m_PointerState.middleButtonPressed = Input.GetMouseButton(2);
            }
        }

        bool ProcessLegacyTouches()
        {
            var hasTouches = Input.touchCount > 0;
            if (!hasTouches)
                return false;

            var touchCount = Input.touchCount;
            for (var touchIndex = 0; touchIndex < touchCount; ++touchIndex)
            {
                var touch = Input.GetTouch(touchIndex);
                var registeredTouchIndex = -1;

                // Find if touch already exists
                for (var j = 0; j < m_RegisteredTouches.Count; j++)
                {
                    if (touch.fingerId == m_RegisteredTouches[j].touchId)
                    {
                        registeredTouchIndex = j;
                        break;
                    }
                }

                if (registeredTouchIndex < 0)
                {
                    // Not found, search empty pool
                    for (var j = 0; j < m_RegisteredTouches.Count; j++)
                    {
                        if (!m_RegisteredTouches[j].isValid)
                        {
                            // Re-use the Id
                            var pointerId = m_RegisteredTouches[j].model.pointerId;
                            m_RegisteredTouches[j] = new RegisteredTouch(touch, pointerId);
                            registeredTouchIndex = j;
                            break;
                        }
                    }

                    if (registeredTouchIndex < 0)
                    {
                        // No Empty slots, add one
                        registeredTouchIndex = m_RegisteredTouches.Count;

                        if (!m_DeletedPointerIds.TryPop(out var newId))
                            newId = m_RollingPointerId++;

                        m_RegisteredTouches.Add(new RegisteredTouch(touch, newId));
                    }
                }

                var registeredTouch = m_RegisteredTouches[registeredTouchIndex];
                registeredTouch.model.selectPhase = touch.phase;
                registeredTouch.model.position = touch.position;
                m_RegisteredTouches[registeredTouchIndex] = registeredTouch;
            }

            for (var i = 0; i < m_RegisteredTouches.Count; i++)
            {
                var registeredTouch = m_RegisteredTouches[i];
                ProcessTouch(ref registeredTouch.model);
                if (registeredTouch.model.selectPhase == TouchPhase.Ended || registeredTouch.model.selectPhase == TouchPhase.Canceled)
                    registeredTouch.isValid = false;
                m_RegisteredTouches[i] = registeredTouch;
            }

            return true;
        }

        void GetLegacyNavigationState()
        {
            if ((m_EnableGamepadInput || m_EnableJoystickInput) && Input.GetJoystickNames().Length > 0)
            {
                m_NavigationState.move = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
                m_NavigationState.submitButtonDown = Input.GetButton(m_SubmitButton);
                m_NavigationState.cancelButtonDown = Input.GetButton(m_CancelButton);
            }
        }
#endif

        bool InputActionReferencesAreSet()
        {
            return (m_PointAction != null ||
                m_LeftClickAction != null ||
                m_RightClickAction != null ||
                m_MiddleClickAction != null ||
                m_NavigateAction != null ||
                m_SubmitAction != null ||
                m_CancelAction != null ||
                m_ScrollWheelAction != null);
        }

        void EnableAllActions()
        {
            EnableInputAction(m_PointAction);
            EnableInputAction(m_LeftClickAction);
            EnableInputAction(m_RightClickAction);
            EnableInputAction(m_MiddleClickAction);
            EnableInputAction(m_NavigateAction);
            EnableInputAction(m_SubmitAction);
            EnableInputAction(m_CancelAction);
            EnableInputAction(m_ScrollWheelAction);
        }

        void DisableAllActions()
        {
            DisableInputAction(m_PointAction);
            DisableInputAction(m_LeftClickAction);
            DisableInputAction(m_RightClickAction);
            DisableInputAction(m_MiddleClickAction);
            DisableInputAction(m_NavigateAction);
            DisableInputAction(m_SubmitAction);
            DisableInputAction(m_CancelAction);
            DisableInputAction(m_ScrollWheelAction);
        }

        static bool IsActionEnabled(InputActionReference inputAction)
        {
            return inputAction != null && inputAction.action != null && inputAction.action.enabled;
        }

        static void EnableInputAction(InputActionReference inputAction)
        {
            if (inputAction == null || inputAction.action == null)
                return;
            inputAction.action.Enable();
        }

        static void DisableInputAction(InputActionReference inputAction)
        {
            if (inputAction == null || inputAction.action == null)
                return;
            inputAction.action.Disable();
        }

        void SetInputAction(ref InputActionReference inputAction, InputActionReference value)
        {
            if (Application.isPlaying && inputAction != null)
                inputAction.action?.Disable();

            inputAction = value;

            if (Application.isPlaying && isActiveAndEnabled && inputAction != null)
                inputAction.action?.Enable();
        }

#if UNITY_2022_3_OR_NEWER
        int GetDisplayIndexFor(InputControl control)
        {
            var displayIndex = 0;
            if (control != null && control.device is Pointer pointerCast && pointerCast != null)
            {
                displayIndex = pointerCast.displayIndex.ReadValue();
                Debug.Assert(displayIndex <= byte.MaxValue, "Display index was larger than expected", this);
            }
            return displayIndex;
        }
#endif
    }
}
