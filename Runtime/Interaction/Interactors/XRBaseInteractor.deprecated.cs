using System;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public abstract partial class XRBaseInteractor
    {
#pragma warning disable 618
        /// <summary>
        /// (Deprecated) Defines whether interactions are enabled or not.
        /// </summary>
        /// <remarks>
        /// <example>
        /// <c>enableInteractions = value;</c> is a convenience property for:
        /// <code>
        /// allowHover = value;
        /// allowSelect = value;
        /// </code>
        /// </example>
        /// </remarks>
        [Obsolete("enableInteractions has been deprecated. Use allowHover and allowSelect instead.")]
        public bool enableInteractions
        {
            get => m_AllowHover && m_AllowSelect;
            set
            {
                m_AllowHover = value;
                m_AllowSelect = value;
            }
        }

        [SerializeField, FormerlySerializedAs("m_OnHoverEnter")]
        XRInteractorEvent m_OnHoverEntered = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor begins hovering over an Interactable.
        /// </summary>
        [Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.")]
        public XRInteractorEvent onHoverEntered
        {
            get => m_OnHoverEntered;
            set => m_OnHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnHoverExit")]
        XRInteractorEvent m_OnHoverExited = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor ends hovering over an Interactable.
        /// </summary>
        [Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.")]
        public XRInteractorEvent onHoverExited
        {
            get => m_OnHoverExited;
            set => m_OnHoverExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectEnter")]
        XRInteractorEvent m_OnSelectEntered = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor begins selecting an Interactable.
        /// </summary>
        [Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.")]
        public XRInteractorEvent onSelectEntered
        {
            get => m_OnSelectEntered;
            set => m_OnSelectEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectExit")]
        XRInteractorEvent m_OnSelectExited = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor ends selecting an Interactable.
        /// </summary>
        [Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature instead.")]
        public XRInteractorEvent onSelectExited
        {
            get => m_OnSelectExited;
            set => m_OnSelectExited = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor begins hovering over an Interactable.
        /// </summary>
        [Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered")]
        public XRInteractorEvent onHoverEnter => onHoverEntered;

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor ends hovering over an Interactable.
        /// </summary>
        [Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited")]
        public XRInteractorEvent onHoverExit => onHoverExited;

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor begins selecting an Interactable.
        /// </summary>
        [Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered")]
        public XRInteractorEvent onSelectEnter => onSelectEntered;

        /// <summary>
        /// (Deprecated) Gets or sets the event that is called when this Interactor ends selecting an Interactable.
        /// </summary>
        [Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited")]
        public XRInteractorEvent onSelectExit => onSelectExited;

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        [Obsolete("OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.")]
        protected internal virtual void OnHoverEntering(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        [Obsolete("OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.")]
        protected internal virtual void OnHoverEntered(XRBaseInteractable interactable)
        {
            m_OnHoverEntered?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        [Obsolete("OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.")]
        protected internal virtual void OnHoverExiting(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        [Obsolete("OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.")]
        protected internal virtual void OnHoverExited(XRBaseInteractable interactable)
        {
            m_OnHoverExited?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        [Obsolete("OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.")]
        protected internal virtual void OnSelectEntering(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        [Obsolete("OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.")]
        protected internal virtual void OnSelectEntered(XRBaseInteractable interactable)
        {
            m_OnSelectEntered?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        [Obsolete("OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.")]
        protected internal virtual void OnSelectExiting(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) This method is called by the Interaction Manager
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        [Obsolete("OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.")]
        protected internal virtual void OnSelectExited(XRBaseInteractable interactable)
        {
            m_OnSelectExited?.Invoke(interactable);
        }
#pragma warning restore 618
    }
}
