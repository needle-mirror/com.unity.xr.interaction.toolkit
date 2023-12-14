using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Serializable haptic impulse provider that allows for specifying the output channel or device for haptic impulses.
    /// Behaviors can declare a field of this type to allow them to output haptic impulses to a controller specified by
    /// an input action or any other source capable of returning a group of haptic channels.
    /// </summary>
    [Serializable]
    public class XRInputHapticImpulseProvider : IXRHapticImpulseProvider
    {
        /// <summary>
        /// The mode that determines from which input source to get the haptic channels.
        /// </summary>
        /// <seealso cref="inputSourceMode"/>
        public enum InputSourceMode
        {
            /// <summary>
            /// Haptics are explicitly not used.
            /// Set to this mode to avoid any performance cost when haptics are not needed.
            /// </summary>
            Unused,

            /// <summary>
            /// The output channel or device is identified by an input action defined and serialized with this behavior.
            /// </summary>
            /// <seealso cref="inputAction"/>
            /// <seealso cref="EnableDirectActionIfModeUsed"/>
            /// <seealso cref="DisableDirectActionIfModeUsed"/>
            InputAction,

            /// <summary>
            /// The output channel or device is identified by an input action defined in the project.
            /// This is the default mode.
            /// </summary>
            /// <seealso cref="InputActionReference"/>
            InputActionReference,

            /// <summary>
            /// The haptic channels are sourced from an object reference that implements <see cref="IXRHapticImpulseProvider"/>.
            /// </summary>
            ObjectReference,
        }

        [SerializeField]
        InputSourceMode m_InputSourceMode = InputSourceMode.InputActionReference;

        /// <summary>
        /// The mode that determines from which input source to get the haptic channels.
        /// By default this is set to <see cref="InputSourceMode.InputActionReference"/> to read from an input action
        /// defined in the project.
        /// </summary>
        /// <seealso cref="InputSourceMode"/>
        public InputSourceMode inputSourceMode
        {
            get => m_InputSourceMode;
            set => m_InputSourceMode = value;
        }

        [SerializeField]
        InputAction m_InputAction;

        /// <summary>
        /// The directly serialized embedded input action that is read to identify the output channels of a device
        /// when the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        public InputAction inputAction
        {
            get => m_InputAction;
            set => m_InputAction = value;
        }

        [SerializeField]
        InputActionReference m_InputActionReference;

        /// <summary>
        /// The reference to an input action that is read to identify the output channels of a device
        /// when the mode is set to <see cref="InputSourceMode.InputActionReference"/>.
        /// </summary>
        public InputActionReference inputActionReference
        {
            get => m_InputActionReference;
            set => m_InputActionReference = value;
        }

        [SerializeField]
        [RequireInterface(typeof(IXRHapticImpulseProvider))]
        Object m_ObjectReferenceObject;

        readonly UnityObjectReferenceCache<IXRHapticImpulseProvider, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRHapticImpulseProvider, Object>();

        HapticControlActionManager m_HapticControlActionManager;

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputHapticImpulseProvider"/>.
        /// </summary>
        public XRInputHapticImpulseProvider()
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputHapticImpulseProvider"/>.
        /// </summary>
        /// <param name="name">The name of the directly serialized embedded input action.</param>
        /// <param name="wantsInitialStateCheck">Whether the action should start performing if already actuated when the action is enabled.</param>
        /// <param name="inputSourceMode">The initial input source mode.</param>
        public XRInputHapticImpulseProvider(string name = null, bool wantsInitialStateCheck = false, InputSourceMode inputSourceMode = InputSourceMode.InputActionReference)
        {
            m_InputAction = InputActionUtility.CreatePassThroughAction(name: name, wantsInitialStateCheck: wantsInitialStateCheck);
            m_InputSourceMode = inputSourceMode;
        }

        /// <summary>
        /// Enable the directly serialized embedded input action if the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        /// <seealso cref="DisableDirectActionIfModeUsed"/>
        public void EnableDirectActionIfModeUsed()
        {
            if (m_InputSourceMode == InputSourceMode.InputAction)
                m_InputAction.Enable();
        }

        /// <summary>
        /// Disable the directly serialized embedded input action if the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        /// <seealso cref="EnableDirectActionIfModeUsed"/>
        public void DisableDirectActionIfModeUsed()
        {
            if (m_InputSourceMode == InputSourceMode.InputAction)
                m_InputAction.Disable();
        }

        /// <summary>
        /// Gets the object reference that is used as the input source to identify the output channels of a device
        /// when the mode is set to <see cref="InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <returns>Returns the object reference, which may be <see langword="null"/>.</returns>
        public IXRHapticImpulseProvider GetObjectReference()
        {
            return m_ObjectReference.Get(m_ObjectReferenceObject);
        }

        /// <summary>
        /// Sets the object reference that is used as the input source to identify the output channels of a device
        /// when the mode is set to <see cref="InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <param name="value">The object reference.</param>
        /// <remarks>
        /// If the argument is to be serialized, it must be a Unity <see cref="Object"/> type.
        /// </remarks>
        public void SetObjectReference(IXRHapticImpulseProvider value)
        {
            m_ObjectReference.Set(ref m_ObjectReferenceObject, value);
        }

        /// <inheritdoc />
        public IXRHapticImpulseChannelGroup GetChannelGroup()
        {
            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    return null;

                case InputSourceMode.InputAction:
                    m_HapticControlActionManager ??= new HapticControlActionManager();
                    return m_HapticControlActionManager.GetChannelGroup(m_InputAction);

                case InputSourceMode.InputActionReference:
                    if (m_InputActionReference != null)
                    {
                        m_HapticControlActionManager ??= new HapticControlActionManager();
                        return m_HapticControlActionManager.GetChannelGroup(m_InputActionReference.action);
                    }

                    return null;

                case InputSourceMode.ObjectReference:
                    return GetObjectReference()?.GetChannelGroup();
            }
        }
    }
}
