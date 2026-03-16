using System;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// This class is a convenience class to handle interactor link between an <see cref="IUIInteractor"/>
    /// and an <see cref="XRUIInputModule"/>.
    /// </summary>
    class RegisteredUIInteractorCache
    {
        XRUIInputModule m_InputModule;
        XRUIInputModule m_RegisteredInputModule;
        readonly IUIInteractor m_UiInteractor;
        readonly XRBaseInteractor m_BaseInteractor;
        readonly bool m_IsBaseInteractor;

        bool m_EnableUIInteraction = true;
        bool m_FindingEventSystem;

        // Used to avoid GC Alloc from delegate object creation if passing the method directly
        Action<EventSystem> m_FindEventSystem;

        /// <summary>
        /// Initializes and returns an instance of <see cref="RegisteredUIInteractorCache"/>.
        /// </summary>
        /// <param name="uiInteractor">This is the interactor that will be registered with the UI Input Module.</param>
        public RegisteredUIInteractorCache(IUIInteractor uiInteractor)
        {
            // This constructor only requires the IUIInteractor reference
            // as only one XRUIInputModule may be present at one time.
            m_UiInteractor = uiInteractor;
            m_IsBaseInteractor = uiInteractor is XRBaseInteractor;
            m_BaseInteractor = m_IsBaseInteractor ? (XRBaseInteractor)uiInteractor : null;
        }

        /// <summary>
        /// Register or unregister with the <see cref="XRUIInputModule"/> (if necessary).
        /// Assumed to be called from the <c>enableUIInteraction</c> property setter of the interactor.
        /// </summary>
        /// <param name="enableUIInteraction">Whether UI interaction should be enabled. Will register/unregister with the XR UI Input Module.</param>
        public void UpdateEnableUIInteraction(bool enableUIInteraction)
        {
            m_EnableUIInteraction = enableUIInteraction;
            if (m_FindingEventSystem)
                return;

            if (enableUIInteraction)
                RegisterWithInputModule(m_InputModule);
            else
                HandleOnDisable();
        }

        /// <summary>
        /// Locate and register with the <see cref="XRUIInputModule"/>.
        /// Assumed to be called from <c>OnEnable</c> of the interactor.
        /// </summary>
        public void HandleOnEnable()
        {
            // This guards against an interactor being disabled and re-enabled before the deferred find finishes.
            if (m_FindingEventSystem)
                return;

            if (m_InputModule == null)
            {
                if (ComponentLocatorUtility<XRUIInputModule>.componentCache != null)
                    m_InputModule = ComponentLocatorUtility<XRUIInputModule>.componentCache;
                else
                {
                    var eventSystem = EventSystem.current ?? ComponentLocatorUtility<EventSystem>.componentCache;
                    if (eventSystem != null)
                        m_InputModule = eventSystem.GetComponent<XRUIInputModule>();
                }
            }

            if (m_InputModule != null)
                RegisterWithInputModule(m_InputModule);
            else
            {
                // This is an optimization to not search for the input module if both manager modes are Manual
                // since it won't create one and will just add to the waitlist during the callback anyway.
                var runtimeSettings = XRInteractionRuntimeSettings.Instance;
                if (runtimeSettings.uiModuleRegistrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual &&
                    runtimeSettings.managerCreationMode == XRInteractionRuntimeSettings.ManagerCreationMode.Manual)
                {
                    XRUIInputModule.RegisterWithWaitlist(m_UiInteractor);
                }
                else
                {
                    m_FindingEventSystem = true;
                    ComponentLocatorUtility<EventSystem>.FindComponentDeferred(
                        m_FindEventSystem ??= OnFindEventSystem,
                        createComponent: runtimeSettings.managerCreationMode == XRInteractionRuntimeSettings.ManagerCreationMode.CreateAutomatically,
                        dontDestroyOnLoad: true);
                }
            }
        }

        /// <summary>
        /// Unregister from the <see cref="XRUIInputModule"/>.
        /// Assumed to be called from <c>OnDisable</c> of the interactor.
        /// </summary>
        public void HandleOnDisable()
        {
            XRUIInputModule.UnregisterFromWaitList(m_UiInteractor);
            UnregisterWithInputModule();
        }

        /// <summary>
        /// Handle being registered with the <see cref="XRUIInputModule"/>.
        /// Assumed to be called from <see cref="IUIInteractorRegistrationHandler.OnRegistered"/> of the interactor.
        /// </summary>
        /// <param name="args">Event data containing the XR UI Input Module that registered this UI interactor.</param>
        public void HandleOnRegistered(UIInteractorRegisteredEventArgs args)
        {
            if (args.inputModule != m_InputModule && m_InputModule != null)
                Debug.LogWarning($"An Interactor was registered with an unexpected {nameof(XRUIInputModule)}." +
                    $" {this} was expecting to communicate with \"{m_InputModule}\" but was registered with \"{args.inputModule}\".", m_BaseInteractor);

            if (m_RegisteredInputModule != null && args.inputModule != m_RegisteredInputModule)
                Debug.LogWarning($"An Interactor was registered with another {nameof(XRUIInputModule)} while already registered with an input module component." +
                    $" {this} was expecting to only communicate with \"{m_RegisteredInputModule}\" but was registered with \"{args.inputModule}\" also." +
                    $" This Interactor will not automatically unregister with the original {nameof(XRUIInputModule)} when this component is disabled.", m_BaseInteractor);

            m_RegisteredInputModule = args.inputModule;
            m_InputModule = args.inputModule;
        }

        /// <summary>
        /// Handle being registered with the <see cref="XRUIInputModule"/>.
        /// Assumed to be called from <see cref="IUIInteractorRegistrationHandler.OnUnregistered"/> of the interactor.
        /// </summary>
        /// <param name="args">Event data containing the XR UI Input Module that unregistered this UI interactor.</param>
        public void HandleOnUnregistered(UIInteractorUnregisteredEventArgs args)
        {
            if (args.inputModule != m_RegisteredInputModule)
                Debug.LogWarning($"An Interactor was unregistered from an unexpected {nameof(XRUIInputModule)}." +
                    $" {this} was expecting to communicate with \"{m_RegisteredInputModule}\" but was unregistered from \"{args.inputModule}\".", m_BaseInteractor);

            m_RegisteredInputModule = null;

            // Clear the reference immediately so it can evaluate to null for the rest of this frame.
            // The input module component reference can still be obtained through the event args.
            if (args.inputModuleDestroyed && ReferenceEquals(m_InputModule, args.inputModule))
                m_InputModule = null;

            if (m_InputModule == null && m_EnableUIInteraction && (!m_IsBaseInteractor || m_BaseInteractor.isActiveAndEnabled))
                XRUIInputModule.RegisterWithWaitlist(m_UiInteractor);
        }

        void OnFindEventSystem(EventSystem eventSystem)
        {
            m_FindingEventSystem = false;

            if (eventSystem != null)
            {
                // Remove the Standalone Input Module if already implemented, since it will block the XRUIInputModule
                if (eventSystem.TryGetComponent<StandaloneInputModule>(out var standaloneInputModule))
                    Object.Destroy(standaloneInputModule);

                // Get or add our XR UI Input Module component
                if (m_InputModule == null && !eventSystem.TryGetComponent(out m_InputModule))
                    m_InputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
            }

            RegisterWithInputModule(m_InputModule);
        }

        void RegisterWithInputModule(XRUIInputModule module)
        {
            if (!m_EnableUIInteraction)
                return;

            if (m_IsBaseInteractor && !m_BaseInteractor.isActiveAndEnabled)
                return;

            // Skip if already registered with this module to avoid a needless unregister/re-register.
            if (m_RegisteredInputModule == module && m_RegisteredInputModule != null)
                return;

            // Note that this block of code intentionally differs from the very similar XR Interaction Manager registration logic.
            // There's no property to set the XRUIInputModule reference directly, so we don't need to early return if
            // the module reference differs. Thus, we should also not check that field when the registration mode is Manual.

            if (XRInteractionRuntimeSettings.Instance.uiModuleRegistrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual)
            {
                XRUIInputModule.RegisterWithWaitlist(m_UiInteractor);
                return;
            }

            UnregisterWithInputModule();

            if (module != null)
                module.RegisterInteractor(m_UiInteractor);
            else
                XRUIInputModule.RegisterWithWaitlist(m_UiInteractor);
        }

        void UnregisterWithInputModule()
        {
            if (m_RegisteredInputModule != null)
                m_RegisteredInputModule.UnregisterInteractor(m_UiInteractor);
        }

        /// <summary>
        /// Attempts to retrieve the current UI Model.
        /// </summary>
        /// <param name="model">The returned model that reflects the UI state of this Interactor.</param>
        /// <returns>Returns <see langword="true"/> if the model was retrieved. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="IUIInteractor.TryGetUIModel"/>
        public bool TryGetUIModel(out TrackedDeviceModel model)
        {
            if (m_InputModule != null)
                return m_InputModule.GetTrackedDeviceModel(m_UiInteractor, out model);

            model = TrackedDeviceModel.invalid;
            return false;
        }

        /// <summary>
        /// Use this to determine if the ray is currently hovering over a UI GameObject.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if hovering over a UI element. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="UIInputModule.IsPointerOverGameObject(int)"/>
        /// <seealso cref="EventSystem.IsPointerOverGameObject(int)"/>
        public bool IsOverUIGameObject()
        {
            return (m_InputModule != null && TryGetUIModel(out var uiModel) && m_InputModule.IsPointerOverGameObject(uiModel.pointerId));
        }

        /// <summary>
        /// Use this to get the current UI GameObject that a pointer is hovering over.
        /// </summary>
        /// <param name="useAnyPointerId">If <see langword="true"/>, the current GameObject for any pointer will be returned.</param>
        /// <param name="currentGameObject">Returns the UI element a pointer is hovering over, or <see langword="null"/>.</param>
        /// <returns>Returns <see langword="true"/> if hovering over a UI element. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="UIInputModule.GetCurrentGameObject(int)"/>
        public bool TryGetCurrentUIGameObject(bool useAnyPointerId, out GameObject currentGameObject)
        {
            if (m_InputModule != null)
            {
                if (useAnyPointerId)
                    currentGameObject = m_InputModule.GetCurrentGameObject(-1);
                else if (TryGetUIModel(out var uiModel))
                    currentGameObject = m_InputModule.GetCurrentGameObject(uiModel.pointerId);
                else
                    currentGameObject = null;

                return currentGameObject != null;
            }

            currentGameObject = null;
            return false;
        }
    }
}
