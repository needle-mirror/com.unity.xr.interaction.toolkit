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

        bool m_EnableUIInteraction;
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
            m_BaseInteractor = uiInteractor as XRBaseInteractor;
        }

        /// <summary>
        /// Register with or unregister from the Input Module (if necessary).
        /// </summary>
        /// <param name="enableUIInteraction">Whether UI interaction should be enabled. Will register/unregister with the XR UI Input Module.</param>
        /// <remarks>
        /// If this behavior is not active and enabled, this function does nothing since it is assumed <c>OnEnable</c>/<c>OnDisable</c> will call into the other methods.
        /// </remarks>
        public void RegisterOrUnregisterXRUIInputModule(bool enableUIInteraction)
        {
            if (!Application.isPlaying || (m_BaseInteractor != null && !m_BaseInteractor.isActiveAndEnabled))
                return;

            if (enableUIInteraction)
                RegisterWithXRUIInputModule();
            else
                UnregisterFromXRUIInputModule();
        }

        /// <summary>
        /// Register with the <see cref="XRUIInputModule"/> (if necessary).
        /// </summary>
        /// <seealso cref="UnregisterFromXRUIInputModule"/>
        public void RegisterWithXRUIInputModule()
        {
            m_EnableUIInteraction = true;
            if (m_FindingEventSystem)
                return;

            if (m_InputModule == null)
                FindOrCreateXRUIInputModule();
            else
                Register();
        }

        /// <summary>
        /// Unregister from the <see cref="XRUIInputModule"/> (if necessary).
        /// </summary>
        /// <seealso cref="RegisterWithXRUIInputModule"/>
        public void UnregisterFromXRUIInputModule()
        {
            m_EnableUIInteraction = false;
            if (m_FindingEventSystem)
                return;

            Unregister();
        }

        void Register()
        {
            if (m_RegisteredInputModule == m_InputModule)
                return;

            if (!m_EnableUIInteraction)
                return;

            UnregisterFromXRUIInputModule();

            if (m_InputModule != null)
            {
                m_InputModule.RegisterInteractor(m_UiInteractor);
                m_RegisteredInputModule = m_InputModule;
            }
        }

        void Unregister()
        {
            if (m_RegisteredInputModule != null)
            {
                m_RegisteredInputModule.UnregisterInteractor(m_UiInteractor);
                m_RegisteredInputModule = null;
            }
        }

        void FindOrCreateXRUIInputModule()
        {
            if (m_FindingEventSystem)
                return;

            m_FindingEventSystem = true;

            var eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                // Set the cached reference to help avoid unnecessary find calls
                if (ComponentLocatorUtility<EventSystem>.componentCache == null)
                    ComponentLocatorUtility<EventSystem>.SetComponentCache(eventSystem);

                OnFindEventSystem(eventSystem);
            }
            else
                ComponentLocatorUtility<EventSystem>.FindComponentDeferred(
                    m_FindEventSystem ??= OnFindEventSystem,
                    createComponent: XRInteractionRuntimeSettings.Instance.managerCreationMode == XRInteractionRuntimeSettings.ManagerCreationMode.CreateAutomatically);
        }

        void OnFindEventSystem(EventSystem eventSystem)
        {
            m_FindingEventSystem = false;

            if (eventSystem == null)
                return;

            // Remove the Standalone Input Module if already implemented, since it will block the XRUIInputModule
            if (eventSystem.TryGetComponent<StandaloneInputModule>(out var standaloneInputModule))
                Object.Destroy(standaloneInputModule);

            // Get or add our XR UI Input Module component
            if (!eventSystem.TryGetComponent(out m_InputModule))
                m_InputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();

            Register();
        }

        /// <summary>
        /// Attempts to retrieve the current UI Model.
        /// </summary>
        /// <param name="model">The returned model that reflects the UI state of this Interactor.</param>
        /// <returns>Returns <see langword="true"/> if the model was retrieved. Otherwise, returns <see langword="false"/>.</returns>
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
