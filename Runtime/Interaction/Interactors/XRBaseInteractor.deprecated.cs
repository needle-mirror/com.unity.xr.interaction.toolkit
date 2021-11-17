using System;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public abstract partial class XRBaseInteractor
    {
#pragma warning disable 618
        /// <summary>
        /// (Deprecated) Allows interaction with Interactables whose Interaction Layer Mask overlaps with any Layer in this Interaction Layer Mask.
        /// </summary>
        /// <seealso cref="XRBaseInteractable.interactionLayerMask"/>
        /// <remarks><c>interactionLayerMask</c> has been deprecated. Use <see cref="interactionLayers"/> instead.</remarks>
        [Obsolete("interactionLayerMask has been deprecated. Use interactionLayers instead.")]
        public LayerMask interactionLayerMask
        {
            get => m_InteractionLayerMask;
            set => m_InteractionLayerMask = value;
        }

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
        /// <c>enableInteractions</c> has been deprecated. Use <see cref="allowHover"/> and <see cref="allowSelect"/> instead.
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
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverEntered</c> has been deprecated. Use <see cref="hoverEntered"/> with updated signature instead.</remarks>
        [Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.")]
        public XRInteractorEvent onHoverEntered
        {
            get => m_OnHoverEntered;
            set => m_OnHoverEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnHoverExit")]
        XRInteractorEvent m_OnHoverExited = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverExited</c> has been deprecated. Use <see cref="hoverExited"/> with updated signature instead.</remarks>
        [Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.")]
        public XRInteractorEvent onHoverExited
        {
            get => m_OnHoverExited;
            set => m_OnHoverExited = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectEnter")]
        XRInteractorEvent m_OnSelectEntered = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectEntered</c> has been deprecated. Use <see cref="selectEntered"/> with updated signature instead.</remarks>
        [Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.")]
        public XRInteractorEvent onSelectEntered
        {
            get => m_OnSelectEntered;
            set => m_OnSelectEntered = value;
        }

        [SerializeField, FormerlySerializedAs("m_OnSelectExit")]
        XRInteractorEvent m_OnSelectExited = new XRInteractorEvent();
        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectExited</c> has been deprecated. Use <see cref="selectExited"/> with updated signature instead.</remarks>
        [Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature instead.")]
        public XRInteractorEvent onSelectExited
        {
            get => m_OnSelectExited;
            set => m_OnSelectExited = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverEnter</c> has been deprecated. Use <see cref="onHoverEntered"/> instead.</remarks>
        [Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered")]
        public XRInteractorEvent onHoverEnter => onHoverEntered;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends hovering over an Interactable.
        /// </summary>
        /// <remarks><c>onHoverExit</c> has been deprecated. Use <see cref="onHoverExited"/> instead.</remarks>
        [Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited")]
        public XRInteractorEvent onHoverExit => onHoverExited;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor begins selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectEnter</c> has been deprecated. Use <see cref="onSelectEntered"/> instead.</remarks>
        [Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered")]
        public XRInteractorEvent onSelectEnter => onSelectEntered;

        /// <summary>
        /// (Deprecated) Gets or sets the event that Unity calls when this Interactor ends selecting an Interactable.
        /// </summary>
        /// <remarks><c>onSelectExit></c> has been deprecated. Use <see cref="onSelectExited"/> instead.</remarks>
        [Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited")]
        public XRInteractorEvent onSelectExit => onSelectExited;

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverEntering(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverEntering(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete("OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.")]
        protected virtual void OnHoverEntering(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverEntered(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverEntered(HoverEnterEventArgs)"/> instead.</remarks>
        [Obsolete("OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.")]
        protected virtual void OnHoverEntered(XRBaseInteractable interactable)
        {
            m_OnHoverEntered?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends hovering over an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverExited(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverExiting(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverExiting(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete("OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.")]
        protected virtual void OnHoverExiting(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends hovering over an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer hovered over.</param>
        /// <seealso cref="OnHoverEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnHoverExited(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnHoverExited(HoverExitEventArgs)"/> instead.</remarks>
        [Obsolete("OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.")]
        protected virtual void OnHoverExited(XRBaseInteractable interactable)
        {
            m_OnHoverExited?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor first initiates selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectEntering(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectEntering(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete("OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.")]
        protected virtual void OnSelectEntering(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor first initiates selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is being selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectEntered(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectEntered(SelectEnterEventArgs)"/> instead.</remarks>
        [Obsolete("OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.")]
        protected virtual void OnSelectEntered(XRBaseInteractable interactable)
        {
            m_OnSelectEntered?.Invoke(interactable);
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// right before the Interactor ends selection of an Interactable
        /// in a first pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectExited(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectExiting(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectExiting(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete("OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.")]
        protected virtual void OnSelectExiting(XRBaseInteractable interactable)
        {
        }

        /// <summary>
        /// (Deprecated) The <see cref="XRInteractionManager"/> calls this method
        /// when the Interactor ends selection of an Interactable
        /// in a second pass.
        /// </summary>
        /// <param name="interactable">Interactable that is no longer selected.</param>
        /// <seealso cref="OnSelectEntered(XRBaseInteractable)"/>
        /// <remarks><c>OnSelectExited(XRBaseInteractable)</c> has been deprecated. Use <see cref="OnSelectExited(SelectExitEventArgs)"/> instead.</remarks>
        [Obsolete("OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.")]
        protected virtual void OnSelectExited(XRBaseInteractable interactable)
        {
            m_OnSelectExited?.Invoke(interactable);
        }
#pragma warning restore 618
    }
}
