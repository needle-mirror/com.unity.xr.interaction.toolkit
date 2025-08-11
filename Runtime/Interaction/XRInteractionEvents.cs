using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Event data associated with an interaction event between an Interactor and Interactable.
    /// </summary>
    /// <remarks>
    /// This class is currently used to define the event args for the interaction events supported by the <see cref="XRInteractionManager"/>.
    ///
    /// This abstract class can be used to define custom events that are based on an interaction between an <see cref="IXRInteractor"/>
    /// and an <see cref="IXRInteractable"/>. If the custom event needs to be executed through the XR Interaction Toolkit interaction system
    /// using <see cref="XRInteractionManager"/>, a custom extension of <see cref="XRInteractionManager"/> is be required.
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates the implementation of the <see cref="HoverEnterEventArgs"/>.</para>
    /// <code source="XRInteractionEvents.cs" region="HoverEnterEventArgs" title="HoverEnterEventArgs"/>
    /// </example>
    /// <seealso cref="HoverEnterEventArgs"/>
    /// <seealso cref="HoverExitEventArgs"/>
    /// <seealso cref="SelectEnterEventArgs"/>
    /// <seealso cref="SelectExitEventArgs"/>
    /// <seealso cref="FocusEnterEventArgs"/>
    /// <seealso cref="FocusExitEventArgs"/>
    /// <seealso cref="ActivateEventArgs"/>
    /// <seealso cref="DeactivateEventArgs"/>
    public abstract partial class BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    #region Hover

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor initiates hovering over an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRHoverInteractor"/> begins hovering an <see cref="IXRHoverInteractable"/>, this <see cref="HoverEnterEvent"/> is invoked
    /// by the <see cref="XRInteractionManager"/>. Use <see cref="UnityEvent.AddListener"/> to implement a callback when <see cref="HoverEnterEvent"/>
    /// is invoked. Use <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="HoverEnterEvent"/> of an <see cref="XRBaseInteractor"/> and implements custom callback functions
    /// that will be called when the <see cref="HoverEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="HoverEnterEventArgs"/>
    /// <seealso cref="HoverExitEvent"/>
    /// <seealso cref="IXRHoverInteractor"/>
    /// <seealso cref="IXRHoverInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class HoverEnterEvent : UnityEvent<HoverEnterEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor initiates hovering over an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRHoverInteractor"/> begins hovering an <see cref="IXRHoverInteractable"/>, a <see cref="HoverEnterEvent"/> is invoked
    /// with <see cref="HoverEnterEventArgs"/> as parameters. The <see cref="HoverEnterEventArgs"/> provides the <see cref="IXRHoverInteractor"/> object,
    /// the <see cref="IXRHoverInteractable"/> object, and the <see cref="XRInteractionManager"/> object associated with the hover event.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="HoverEnterEvent"/> of an <see cref="XRBaseInteractor"/> and implements customs callback functions
    /// that will be called when the <see cref="HoverEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="HoverEnterEvent"/>
    /// <seealso cref="IXRHoverInteractor"/>
    /// <seealso cref="IXRHoverInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    #region HoverEnterEventArgs
    public class HoverEnterEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractor interactorObject
        {
            get => (IXRHoverInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractable interactableObject
        {
            get => (IXRHoverInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    #endregion

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor ends hovering over an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRHoverInteractor"/> stops hovering an <see cref="IXRHoverInteractable"/>, this <see cref="HoverExitEvent"/> is invoked
    /// by the <see cref="XRInteractionManager"/>. Use <see cref="UnityEvent.AddListener"/> to implement a callback when <see cref="HoverExitEvent"/>
    /// is invoked. Use <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="HoverExitEvent"/> of an <see cref="XRBaseInteractor"/> and implements custom callback functions
    /// that will be called when the <see cref="HoverExitEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="HoverExitEventArgs"/>
    /// <seealso cref="HoverEnterEvent"/>
    /// <seealso cref="IXRHoverInteractor"/>
    /// <seealso cref="IXRHoverInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class HoverExitEvent : UnityEvent<HoverExitEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor ends hovering over an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRHoverInteractor"/> ends hovering an <see cref="IXRHoverInteractable"/>, a <see cref="HoverExitEvent"/> is invoked
    /// with <see cref="HoverExitEventArgs"/> as parameters. The <see cref="HoverExitEventArgs"/> provides the <see cref="IXRHoverInteractor"/> object,
    /// the <see cref="IXRHoverInteractable"/> object, and the <see cref="XRInteractionManager"/> object associated with the hover event. It also provides
    /// the <see cref="isCanceled"/> boolean which will be <see langword="true"/> if the <see cref="interactorObject"/> or the <see cref="interactableObject"/>
    /// is unregistered with the <see cref="XRInteractionManager"/>, disabled, or destroyed during the hover. Additionally, <see cref="isCanceled"/> will be
    /// <see langword="true"/> if either <see cref="XRInteractionManager.CancelInteractableHover(IXRHoverInteractable)"/> or <see cref="XRInteractionManager.HoverCancel(IXRHoverInteractor,IXRHoverInteractable)"/> is called manually.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="HoverExitEvent"/> of an <see cref="XRBaseInteractor"/> and implements customs callback functions
    /// that will be called when the <see cref="HoverExitEvent"/> is invoked. The <see cref="HoverExitEventArgs"/> are utilized in the
    /// `XRInteractionEventsSample.OnInteractorHoverExit` callback to return early if the hover event was canceled.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="HoverExitEvent"/>
    /// <seealso cref="IXRHoverInteractor"/>
    /// <seealso cref="IXRHoverInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class HoverExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractor interactorObject
        {
            get => (IXRHoverInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRHoverInteractable interactableObject
        {
            get => (IXRHoverInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }

        /// <summary>
        /// Whether the hover was ended due to being canceled, such as from
        /// either the Interactor or Interactable being unregistered due to being
        /// disabled or destroyed.
        /// </summary>
        public bool isCanceled { get; set; }
    }

    #endregion

    #region Select

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor initiates selecting an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRSelectInteractor"/> begins selecting an <see cref="IXRSelectInteractable"/>, this <see cref="SelectEnterEvent"/> is invoked
    /// by the <see cref="XRInteractionManager"/>. Use <see cref="UnityEvent.AddListener"/> to implement a callback when <see cref="SelectEnterEvent"/>
    /// is invoked. Use <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="SelectEnterEvent"/> of an <see cref="XRBaseInteractor"/> and implements custom callback functions
    /// that will be called when the <see cref="SelectEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="SelectEnterEventArgs"/>
    /// <seealso cref="SelectExitEvent"/>
    /// <seealso cref="IXRSelectInteractor"/>
    /// <seealso cref="IXRSelectInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class SelectEnterEvent : UnityEvent<SelectEnterEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor initiates selecting an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRSelectInteractor"/> begins selecting an <see cref="IXRSelectInteractable"/>, a <see cref="SelectEnterEvent"/> is invoked
    /// with <see cref="SelectEnterEventArgs"/> as parameters. The <see cref="SelectEnterEventArgs"/> parameter provides the <see cref="IXRSelectInteractor"/> object,
    /// the <see cref="IXRSelectInteractable"/> object, and the <see cref="XRInteractionManager"/> object associated with the select event.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="SelectEnterEvent"/> of an <see cref="XRBaseInteractor"/> and implements customs callback functions
    /// that will be called when the <see cref="SelectEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="SelectEnterEvent"/>
    /// <seealso cref="IXRSelectInteractor"/>
    /// <seealso cref="IXRSelectInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class SelectEnterEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractor interactorObject
        {
            get => (IXRSelectInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractable interactableObject
        {
            get => (IXRSelectInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interactor ends selecting an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRSelectInteractor"/> stops selecting an <see cref="IXRSelectInteractable"/>, this <see cref="SelectExitEvent"/> is invoked
    /// by the <see cref="XRInteractionManager"/>. Use <see cref="UnityEvent.AddListener"/> to implement a callback when <see cref="SelectExitEvent"/>
    /// is invoked. Use <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="SelectExitEvent"/> of an <see cref="XRBaseInteractor"/> and implements custom callback functions
    /// that will be called when the <see cref="SelectExitEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="SelectExitEventArgs"/>
    /// <seealso cref="SelectEnterEvent"/>
    /// <seealso cref="IXRSelectInteractor"/>
    /// <seealso cref="IXRSelectInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class SelectExitEvent : UnityEvent<SelectExitEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interactor ends selecting an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRSelectInteractor"/> ends selecting an <see cref="IXRSelectInteractable"/>, a <see cref="SelectExitEvent"/> is invoked
    /// with <see cref="SelectExitEventArgs"/> as parameters. The <see cref="SelectExitEventArgs"/> provides the <see cref="IXRSelectInteractor"/> object,
    /// the <see cref="IXRSelectInteractable"/> object, and the <see cref="XRInteractionManager"/> object associated with the selection. It also provides
    /// the <see cref="isCanceled"/> boolean which will be <see langword="true"/> if the <see cref="interactorObject"/> or the <see cref="interactableObject"/>
    /// is unregistered with the <see cref="XRInteractionManager"/>, disabled, or destroyed during selection. Additionally, <see cref="isCanceled"/> will be
    /// <see langword="true"/> if either <see cref="XRInteractionManager.CancelInteractableSelection(IXRSelectInteractable)"/> or <see cref="XRInteractionManager.SelectCancel(IXRSelectInteractor,IXRSelectInteractable)"/>
    /// are called manually.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="SelectExitEvent"/> of an <see cref="XRBaseInteractor"/> and implements customs callback functions
    /// that will be called when the <see cref="SelectExitEvent"/> is invoked. The <see cref="SelectExitEventArgs"/> are utilized in the
    /// `XRInteractionEventsSample.OnInteractorSelectExit` callback to return early if the selection event was canceled.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="SelectExitEvent"/>
    /// <seealso cref="IXRSelectInteractor"/>
    /// <seealso cref="IXRSelectInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class SelectExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractor interactorObject
        {
            get => (IXRSelectInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRSelectInteractable interactableObject
        {
            get => (IXRSelectInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }

        /// <summary>
        /// Whether the selection was ended due to being canceled, such as from
        /// either the Interactor or Interactable being unregistered due to being
        /// disabled or destroyed.
        /// </summary>
        public bool isCanceled { get; set; }
    }

    #endregion

    #region Focus

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interaction group initiates focusing an Interactable.
    /// </summary>
    /// <remarks>
    /// The <see cref="FocusEnterEvent"/> is a <see cref="UnityEvent"/> that is invoked when an <see cref="IXRInteractor"/> that is a member of an <see cref="XRInteractionGroup"/>
    /// selects an <see cref="IXRFocusInteractable"/>.
    ///
    /// When an interactable is focused, the <see cref="XRInteractionGroup"/> maintains a reference to the focusing <see cref="IXRInteractor"/> and the focused
    /// <see cref="IXRFocusInteractable"/>. This focus state is maintained until the focusing <see cref="IXRInteractor"/> selects a different <see cref="IXRInteractable"/>, the
    /// focusing <see cref="IXRInteractor"/> selects 'nothing', or until the focused <see cref="IXRFocusInteractable"/> is selected by a different <see cref="IXRInteractor"/> when the
    /// <see cref="IXRFocusInteractable.focusMode"/> is set to <see cref="InteractableFocusMode.Single"/>. If <see cref="IXRFocusInteractable.focusMode"/> is set to
    /// <see cref="InteractableFocusMode.Multiple"/>, the focus state will be maintained when another <see cref="IXRInteractor"/> selects the <see cref="IXRFocusInteractable"/>.
    ///
    /// The <see cref="FocusEnterEvent"/>s are primarily called from the <see cref="XRInteractionManager"/>. If the selecting <see cref="IXRInteractor"/> is not a member of a
    /// <see cref="XRInteractionGroup"/>, no <see cref="FocusEnterEvent"/> will be invoked.
    ///
    /// The <see cref="FocusEnterEvent"/> can be used to implement custom callbacks when an <see cref="IXRFocusInteractable"/> is focused. Use <see cref="UnityEvent.AddListener"/>
    /// to bind a custom callback to the <see cref="FocusEnterEvent"/> invocation and <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="FocusEnterEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="FocusEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="FocusExitEvent"/>
    /// <seealso cref="FocusEnterEventArgs"/>
    /// <seealso cref="InteractableFocusMode"/>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="IXRFocusInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class FocusEnterEvent : UnityEvent<FocusEnterEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interaction group gains focus of an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractor"/> that is a member of an <see cref="XRInteractionGroup"/> selects an <see cref="IXRFocusInteractable"/>, a <see cref="FocusEnterEvent"/>
    /// is invoked with <see cref="FocusEnterEventArgs"/> as parameters. The <see cref="FocusEnterEventArgs"/> provides the <see cref="IXRInteractor"/> object, the
    /// <see cref="IXRFocusInteractable"/> object, the <see cref="XRInteractionGroup"/>, and the <see cref="XRInteractionManager"/> object associated with the focus event.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="FocusEnterEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="FocusEnterEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="FocusEnterEvent"/>
    /// <seealso cref="InteractableFocusMode"/>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="IXRFocusInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class FocusEnterEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The interaction group associated with the interaction event.
        /// </summary>
        public IXRInteractionGroup interactionGroup { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRFocusInteractable interactableObject
        {
            get => (IXRFocusInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when an Interaction group ends focusing an Interactable.
    /// </summary>
    /// <remarks>
    /// The <see cref="FocusExitEvent"/> is a <see cref="UnityEvent"/> that is invoked when an <see cref="XRInteractionGroup"/> ends focusing an <see cref="IXRFocusInteractable"/>.
    ///
    /// Focusing can be exited in a few different ways. If the focusing <see cref="IXRInteractor"/> selects a different <see cref="IXRInteractable"/> or the focusing
    /// <see cref="IXRInteractor"/> selects 'nothing', focusing will be ended. Alternatively, if the focused <see cref="IXRFocusInteractable"/> is selected by a different
    /// <see cref="IXRInteractor"/> when the <see cref="IXRFocusInteractable.focusMode"/> is set to <see cref="InteractableFocusMode.Single"/>, focusing will be exited as well.
    /// If <see cref="IXRFocusInteractable.focusMode"/> is set to <see cref="InteractableFocusMode.Multiple"/> and another <see cref="IXRInteractor"/> selects the
    /// <see cref="IXRFocusInteractable"/>, focus will not be exited.
    ///
    /// The <see cref="FocusExitEvent"/>s are primarily called from the <see cref="XRInteractionManager"/>.
    ///
    /// The <see cref="FocusExitEvent"/> can be used to implement custom callbacks when focus is ended on an <see cref="IXRFocusInteractable"/>. Use <see cref="UnityEvent.AddListener"/>
    /// to bind a custom callback to the <see cref="FocusExitEvent"/> invocation and <see cref="UnityEvent.RemoveListener"/> to stop listening for the event invocation.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="FocusExitEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="FocusExitEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="FocusEnterEvent"/>
    /// <seealso cref="FocusExitEventArgs"/>
    /// <seealso cref="InteractableFocusMode"/>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="IXRFocusInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class FocusExitEvent : UnityEvent<FocusExitEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an Interaction group ends focusing an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="XRInteractionGroup"/> ends focusing an <see cref="IXRFocusInteractable"/>, a <see cref="FocusExitEvent"/>
    /// is invoked with <see cref="FocusExitEventArgs"/> as parameters. The <see cref="FocusExitEventArgs"/> provides the <see cref="IXRInteractor"/> object, the
    /// <see cref="IXRFocusInteractable"/> object, the <see cref="XRInteractionGroup"/>, and the <see cref="XRInteractionManager"/> object associated with the focus event.
    /// It also provides the <see cref="isCanceled"/> boolean which will be <see langword="true"/> if the interactor is destroyed or the <see cref="interactableObject"/>
    /// is unregistered with the <see cref="XRInteractionManager"/>, disabled, or destroyed during focus.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="FocusExitEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="FocusExitEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="FocusEnterEvent"/>
    /// <seealso cref="InteractableFocusMode"/>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="IXRFocusInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class FocusExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The interaction group associated with the interaction event.
        /// </summary>
        public IXRInteractionGroup interactionGroup { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRFocusInteractable interactableObject
        {
            get => (IXRFocusInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }

        /// <summary>
        /// The Interaction Manager associated with the interaction event.
        /// </summary>
        public XRInteractionManager manager { get; set; }

        /// <summary>
        /// Whether the focus was lost due to being canceled, such as from
        /// either the Interaction group or Interactable being unregistered due to being
        /// disabled or destroyed.
        /// </summary>
        public bool isCanceled { get; set; }
    }

    #endregion

    #region Activate

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when the selecting Interactor activates an Interactable.
    /// </summary>
    /// <remarks>
    /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
    /// This is a generic event when an <see cref="IXRActivateInteractor"/> wants to activate its selected <see cref="IXRActivateInteractable"/>,
    /// such as from a trigger pull on a controller.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="ActivateEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="ActivateEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="DeactivateEvent"/>
    /// <seealso cref="ActivateEventArgs"/>
    /// <seealso cref="IXRActivateInteractor"/>
    /// <seealso cref="IXRActivateInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class ActivateEvent : UnityEvent<ActivateEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when the selecting Interactor activates an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRActivateInteractor"/> activates a selected <see cref="IXRActivateInteractable"/>, an <see cref="ActivateEvent"/>
    /// is invoked with <see cref="ActivateEventArgs"/> as parameters. The <see cref="ActivateEventArgs"/> provides the <see cref="IXRActivateInteractor"/> object and the
    /// <see cref="IXRActivateInteractable"/> object associated with the activate event.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="ActivateEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="ActivateEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="DeactivateEvent"/>
    /// <seealso cref="IXRActivateInteractor"/>
    /// <seealso cref="IXRActivateInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class ActivateEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractor interactorObject
        {
            get => (IXRActivateInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractable interactableObject
        {
            get => (IXRActivateInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that Unity invokes when the selecting Interactor deactivates an Interactable.
    /// </summary>
    /// <remarks>
    /// Not to be confused with activating or deactivating a <see cref="GameObject"/> with <see cref="GameObject.SetActive"/>.
    /// This is a generic event when an <see cref="IXRActivateInteractor"/> wants to deactivate its selected <see cref="IXRActivateInteractable"/>,
    /// such as from a trigger pull on a controller.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="DeactivateEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="DeactivateEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="ActivateEvent"/>
    /// <seealso cref="DeactivateEventArgs"/>
    /// <seealso cref="IXRActivateInteractor"/>
    /// <seealso cref="IXRActivateInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    [Serializable]
    public sealed class DeactivateEvent : UnityEvent<DeactivateEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when the selecting Interactor deactivates an Interactable.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRActivateInteractor"/> deactivates a selected and activated <see cref="IXRActivateInteractable"/>, a <see cref="DeactivateEvent"/>
    /// is invoked with <see cref="DeactivateEventArgs"/> as parameters. The <see cref="DeactivateEventArgs"/> provides the <see cref="IXRActivateInteractor"/> object and the
    /// <see cref="IXRActivateInteractable"/> object associated with the deactivate event.
    /// </remarks>
    /// <example>
    /// <para>The following example adds a listener to the <see cref="DeactivateEvent"/> of an <see cref="XRBaseInteractable"/> and implements custom callback functions
    /// that will be called when the <see cref="DeactivateEvent"/> is invoked.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionEventsSample.cs"/>
    /// </example>
    /// <seealso cref="DeactivateEvent"/>
    /// <seealso cref="IXRActivateInteractor"/>
    /// <seealso cref="IXRActivateInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRBaseInteractable"/>
    public class DeactivateEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractor interactorObject
        {
            get => (IXRActivateInteractor)base.interactorObject;
            set => base.interactorObject = value;
        }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public new IXRActivateInteractable interactableObject
        {
            get => (IXRActivateInteractable)base.interactableObject;
            set => base.interactableObject = value;
        }
    }

    #endregion

    #region Registration

    /// <summary>
    /// Event data associated with a registration event with an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// This class is currently used to define the event args for the registration events supported by the <see cref="XRInteractionManager"/>.
    ///
    /// This abstract class can be used to define custom events that register some type of object with a custom extension of the <see cref="XRInteractionManager"/>.
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates the implementation of the <see cref="InteractorRegisteredEventArgs"/>.</para>
    /// <code source="XRInteractionEvents.cs" region="InteractorRegistrationEvent" title="InteractorRegistrationEventArgs"/>
    /// </example>
    /// <seealso cref="InteractionGroupRegisteredEventArgs"/>
    /// <seealso cref="InteractorRegisteredEventArgs"/>
    /// <seealso cref="InteractableRegisteredEventArgs"/>
    /// <seealso cref="InteractionGroupUnregisteredEventArgs"/>
    /// <seealso cref="InteractorUnregisteredEventArgs"/>
    /// <seealso cref="InteractableUnregisteredEventArgs"/>
    public abstract class BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interaction Manager associated with the registration event.
        /// </summary>
        public XRInteractionManager manager { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an <see cref="IXRInteractionGroup"/> is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractionGroup"/> is registered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractionGroup.registered"/> event and <see cref="XRInteractionManager.interactionGroupRegistered"/> events are invoked
    /// with <see cref="InteractionGroupRegisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractionGroupRegisteredEventArgs"/> provides the registering <see cref="interactionGroupObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the registration event. Additionally, <see cref="InteractionGroupRegisteredEventArgs"/> provides an optional <see cref="containingGroupObject"/>
    /// in the case that there is a containing or nested interaction group. If not set, <see cref="containingGroupObject"/> will be <see langword="null"/>.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactionGroupRegistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactionGroupRegistered"/> is invoked. Enabling the "Example Interaction Group"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractionGroup"/>
    /// <seealso cref="XRInteractionGroup"/>
    /// <seealso cref="XRInteractionManager"/>
    public class InteractionGroupRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interaction Group that was registered.
        /// </summary>
        public IXRInteractionGroup interactionGroupObject { get; set; }

        /// <summary>
        /// The Interaction Group that contains the registered Group. Will be <see langword="null"/> if there is no containing Group.
        /// </summary>
        public IXRInteractionGroup containingGroupObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactor is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractor"/> is registered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractor.registered"/> event and <see cref="XRInteractionManager.interactorRegistered"/> events are invoked
    /// with <see cref="InteractorRegisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractorRegisteredEventArgs"/> provides the registering <see cref="interactorObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the registration event. Additionally, <see cref="InteractorRegisteredEventArgs"/> provides an optional <see cref="containingGroupObject"/>
    /// if the <see cref="IXRInteractor"/> is part of an <see cref="IXRInteractionGroup"/>. If not set, <see cref="containingGroupObject"/> will be <see langword="null"/>.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactorRegistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactorRegistered"/> is invoked. Enabling the "Example Interactor"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRInteractionManager"/>
    #region InteractorRegistrationEvent
    public partial class InteractorRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactor that was registered.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }

        /// <summary>
        /// The Interaction Group that contains the registered Interactor. Will be <see langword="null"/> if there is no containing Group.
        /// </summary>
        public IXRInteractionGroup containingGroupObject { get; set; }
    }

    #endregion

    /// <summary>
    /// Event data associated with the event when an Interactable is registered with an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractable"/> is registered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractable.registered"/> event and <see cref="XRInteractionManager.interactableRegistered"/> events are invoked
    /// with <see cref="InteractableRegisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractableRegisteredEventArgs"/> provides the registering <see cref="interactableObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the registration event.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactableRegistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactableRegistered"/> is invoked. Enabling the "Example Interactable"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractable"/>
    /// <seealso cref="XRBaseInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    public partial class InteractableRegisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactable that was registered.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interaction Group is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractionGroup"/> is unregistered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractionGroup.unregistered"/> event and <see cref="XRInteractionManager.interactableUnregistered"/> events are invoked
    /// with <see cref="InteractionGroupUnregisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractionGroupUnregisteredEventArgs"/> provides the unregistering <see cref="interactionGroupObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the unregistration event.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactionGroupUnregistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactionGroupUnregistered"/> is invoked. Enabling the "Example Interaction Group"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractionGroup"/>
    /// <seealso cref="XRInteractionGroup"/>
    /// <seealso cref="XRInteractionManager"/>
    public class InteractionGroupUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interaction Group that was unregistered.
        /// </summary>
        public IXRInteractionGroup interactionGroupObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactor is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractor"/> is unregistered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractor.unregistered"/> event and <see cref="XRInteractionManager.interactorUnregistered"/> events are invoked
    /// with <see cref="InteractorUnregisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractorUnregisteredEventArgs"/> provides the unregistering <see cref="interactorObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the unregistration event.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactorUnregistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactorUnregistered"/> is invoked. Enabling the "Example Interactor"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractor"/>
    /// <seealso cref="XRBaseInteractor"/>
    /// <seealso cref="XRInteractionManager"/>
    public partial class InteractorUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactor that was unregistered.
        /// </summary>
        public IXRInteractor interactorObject { get; set; }
    }

    /// <summary>
    /// Event data associated with the event when an Interactable is unregistered from an <see cref="XRInteractionManager"/>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IXRInteractable"/> is unregistered with the <see cref="XRInteractionManager"/>, the
    /// <see cref="IXRInteractable.unregistered"/> event and <see cref="XRInteractionManager.interactableUnregistered"/> events are invoked
    /// with <see cref="InteractableRegisteredEventArgs"/> as parameters.
    ///
    /// <see cref="InteractableRegisteredEventArgs"/> provides the unregistering <see cref="interactableObject"/> and the <see cref="XRInteractionManager"/>
    /// associated with the unregistration event.
    /// </remarks>
    /// <example>
    /// <para>The following example subscribes to the <see cref="XRInteractionManager.interactableUnregistered"/> event and implements custom callback functions
    /// that will be called when the <see cref="XRInteractionManager.interactableUnregistered"/> is invoked. Enabling the "Example Interactable"
    /// game object will trigger the registered callback, while disabling it will trigger the unregistered callback.</para>
    /// <code source="../../DocCodeSamples.Tests/XRInteractionManagerRegistrationEventsSample.cs"/>
    /// </example>
    /// <seealso cref="IXRInteractable"/>
    /// <seealso cref="XRBaseInteractable"/>
    /// <seealso cref="XRInteractionManager"/>
    public partial class InteractableUnregisteredEventArgs : BaseRegistrationEventArgs
    {
        /// <summary>
        /// The Interactable that was unregistered.
        /// </summary>
        public IXRInteractable interactableObject { get; set; }
    }

    #endregion
}
