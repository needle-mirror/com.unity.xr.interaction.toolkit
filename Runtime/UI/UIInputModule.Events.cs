using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UIELEMENTS_MODULE_PRESENT || PACKAGE_DOCS_GENERATION
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    public abstract partial class UIInputModule
    {
        /// <summary>
        /// Calls the methods in its invocation list after the input module collects a list of type <see cref="RaycastResult"/>, but before the results are used.
        /// Note that not all fields of the event data are still valid or up to date at this point in the UI event processing.
        /// This event can be used to read, modify, or reorder results.
        /// After the event, the first result in the list with a non-null GameObject will be used.
        /// </summary>
        public event Action<PointerEventData, List<RaycastResult>> finalizeRaycastResults;

        /// <summary>
        /// This occurs when a UI pointer enters an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerEnter;

        /// <summary>
        /// This occurs when a UI pointer exits an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerExit;

        /// <summary>
        /// This occurs when a select button down occurs while a UI pointer is hovering an element.
        /// This event is executed using ExecuteEvents.ExecuteHierarchy when sent to the target element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerDown;

        /// <summary>
        /// This occurs when a select button up occurs while a UI pointer is hovering an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerUp;

        /// <summary>
        /// This occurs when a select button click occurs while a UI pointer is hovering an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerClick;

        /// <summary>
        /// This occurs while a UI pointer is moving over elements.
        /// </summary>
        /// <remarks>
        /// This may induce performance penalties due to the frequency in which this event gets called
        /// and should be used with that consideration in mind.
        /// </remarks>
        public event Action<GameObject, PointerEventData> pointerMove;

        /// <summary>
        /// This occurs when a potential drag occurs on an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> initializePotentialDrag;

        /// <summary>
        /// This occurs when a drag first occurs on an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> beginDrag;

        /// <summary>
        /// This occurs every frame while dragging an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> drag;

        /// <summary>
        /// This occurs on the last frame an element is dragged.
        /// </summary>
        public event Action<GameObject, PointerEventData> endDrag;

        /// <summary>
        /// This occurs when a dragged element is dropped on a drop handler.
        /// </summary>
        public event Action<GameObject, PointerEventData> drop;

        /// <summary>
        /// This occurs when an element is scrolled
        /// This event is executed using ExecuteEvents.ExecuteHierarchy when sent to the target element.
        /// </summary>
        public event Action<GameObject, PointerEventData> scroll;

        /// <summary>
        /// This occurs on update for the currently selected object.
        /// </summary>
        public event Action<GameObject, BaseEventData> updateSelected;

        /// <summary>
        /// This occurs when the move axis is activated.
        /// </summary>
        public event Action<GameObject, AxisEventData> move;

        /// <summary>
        /// This occurs when the submit button is pressed.
        /// </summary>
        public event Action<GameObject, BaseEventData> submit;

        /// <summary>
        /// This occurs when the cancel button is pressed.
        /// </summary>
        public event Action<GameObject, BaseEventData> cancel;
    }

    #region Hover
    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor initiates hovering over a new UI element.
    /// </summary>
    [Serializable]
    public sealed class UIHoverEnterEvent : UnityEvent<UIHoverEventArgs>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor ends hovering over a UI element.
    /// </summary>
    [Serializable]
    public sealed class UIHoverExitEvent : UnityEvent<UIHoverEventArgs>
    {
    }

    /// <summary>
    /// Arguments passed to the <see cref="UnityEvent"/> that Unity invokes when an Interactor is hovering over a UI element.
    /// </summary>
    public class UIHoverEventArgs
    {
        /// <summary>
        /// Indicates which UI system produced a UI hover event.
        /// </summary>
        /// <seealso cref="uiSystem"/>
        public enum UISystem
        {
            /// <summary>
            /// uGUI (Unity UI).
            /// </summary>
            UnityUI,

            /// <summary>
            /// UI Toolkit (UITK).
            /// </summary>
            UIToolkit,

            /// <summary>
            /// Unknown or not specified.
            /// </summary>
            Unknown = -1,
        }

        /// <summary>
        /// The <see cref="IUIInteractor"/> that is hovering.
        /// </summary>
        public IUIInteractor interactorObject { get; set; }

        /// <summary>
        /// The <see cref="TrackedDeviceModel"/> corresponding to the controller or hand
        /// interacting with the UI element that is being hovered over.
        /// </summary>
        public TrackedDeviceModel deviceModel { get; set; }

        /// <summary>
        /// The object representing the hovered UI target.
        /// For uGUI (Unity UI), this is the Graphic's GameObject.
        /// For UI Toolkit, this is the panel host GameObject (e.g. the UIDocument GameObject), or <see langword="null"/> if not available.
        /// </summary>
        public GameObject uiObject { get; set; }

        /// <summary>
        /// Indicates which UI system produced this hover event.
        /// </summary>
        public UISystem uiSystem { get; set; }

#if UIELEMENTS_MODULE_PRESENT || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// For UI Toolkit interactions, the VisualElement being hovered.
        /// Will be <see langword="null"/> for uGUI (Unity UI) interactions.
        /// </summary>
        public VisualElement visualElement { get; set; }

        /// <summary>
        /// Optional reference to the UIDocument associated with the hovered VisualElement, if available.
        /// Will be <see langword="null"/> for uGUI (Unity UI) interactions or when no document can be determined.
        /// </summary>
        public UIDocument uiDocument { get; set; }
#endif
    }
    #endregion

    #region Registration

    /// <summary>
    /// Event data associated with the event when a UI interactor is registered with an <see cref="XRUIInputModule"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IUIInteractor"/> is registered with the <see cref="XRUIInputModule"/>, the
    /// <see cref="IUIInteractorRegistrationHandler.OnRegistered"/> method and <see cref="XRUIInputModule.interactorRegistered"/> event are invoked
    /// with <see cref="UIInteractorRegisteredEventArgs"/> as parameters.
    /// </remarks>
    /// <seealso cref="IUIInteractorRegistrationHandler"/>
    /// <seealso cref="XRUIInputModule"/>
    public class UIInteractorRegisteredEventArgs
    {
        /// <summary>
        /// The XR UI Input Module associated with the registration event.
        /// </summary>
        public XRUIInputModule inputModule { get; set; }

        /// <summary>
        /// The UI interactor that was registered.
        /// </summary>
        public IUIInteractor interactor { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when a UI interactor is unregistered from an <see cref="XRUIInputModule"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IUIInteractor"/> is unregistered with the <see cref="XRUIInputModule"/>, the
    /// <see cref="IUIInteractorRegistrationHandler.OnUnregistered"/> method and <see cref="XRUIInputModule.interactorUnregistered"/> event are invoked
    /// with <see cref="UIInteractorUnregisteredEventArgs"/> as parameters.
    /// </remarks>
    /// <seealso cref="IUIInteractorRegistrationHandler"/>
    /// <seealso cref="XRUIInputModule"/>
    public class UIInteractorUnregisteredEventArgs
    {
        /// <summary>
        /// The XR UI Input Module associated with the registration event.
        /// </summary>
        public XRUIInputModule inputModule { get; set; }

        /// <summary>
        /// The UI interactor that was unregistered.
        /// </summary>
        public IUIInteractor interactor { get; set; }

        /// <summary>
        /// Whether the unregistration event was due to the input module being destroyed.
        /// </summary>
        /// <seealso cref="inputModule"/>
        public bool inputModuleDestroyed { get; set; }
    }

    #endregion
}
