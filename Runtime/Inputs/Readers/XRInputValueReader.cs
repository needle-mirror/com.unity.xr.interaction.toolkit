using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// Base interface for all input value readers.
    /// </summary>
    /// <remarks>
    /// This empty interface is needed to allow the <c>RequireInterface</c> attribute to be used.
    /// Generic attributes aren't supported until C# 11, so we can't use a typed interface with the <c>RequireInterface</c> attribute yet.
    /// </remarks>
    public interface IXRInputValueReader
    {
    }

    /// <summary>
    /// Interface which allows for callers to read the current value from an input source.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to read, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
    /// <seealso cref="XRInputValueReader"/>
    public interface IXRInputValueReader<TValue> : IXRInputValueReader where TValue : struct
    {
        /// <summary>
        /// Read the current value from the input source.
        /// </summary>
        /// <returns>Returns the current value from the input source. May return <c>default(TValue)</c> if unused or no source is set.</returns>
        TValue ReadValue();

        /// <summary>
        /// Try to read the current value from the input source.
        /// </summary>
        /// <param name="value">When this method returns, contains the current value from the input source. May return <c>default(TValue)</c> if unused or no source is set.</param>
        /// <returns>Returns <see langword="true"/> if the current value was able to be read (and for actions, also if in progress).</returns>
        /// <remarks>
        /// You can use the return value of this method instead of only using <see cref="ReadValue"/> in order to avoid doing
        /// any work when the input action is not in progress, such as when the control is not actuated.
        /// This can be useful for performance reasons.
        /// <br />
        /// If an input processor on an input action returns a different value from the default <typeparamref name="TValue"/>
        /// when the input action is not in progress, the <see langword="out"/> <paramref name="value"/> returned by
        /// this method may not be <c>default(TValue)</c> as is typically the case for <c>Try</c>- methods. If you need
        /// to support processors that return a different value from the default when the control is not actuated,
        /// you should use <see cref="ReadValue()"/> instead of using the return value of this method to skip input handling.
        /// </remarks>
        bool TryReadValue(out TValue value);
    }

    /// <summary>
    /// Base abstract class for a serializable input value reader without typed code.
    /// </summary>
    /// <seealso cref="XRInputValueReader{TValue}"/>
    public abstract class XRInputValueReader
    {
        /// <summary>
        /// The mode that determines from which input source the value is read from.
        /// </summary>
        /// <seealso cref="inputSourceMode"/>
        public enum InputSourceMode
        {
            /// <summary>
            /// The input is explicitly not used.
            /// Set to this mode to avoid any performance cost when the input should be ignored.
            /// </summary>
            Unused,

            /// <summary>
            /// The input is read from an input action defined and serialized with this behavior.
            /// </summary>
            /// <seealso cref="inputAction"/>
            /// <seealso cref="EnableDirectActionIfModeUsed"/>
            /// <seealso cref="DisableDirectActionIfModeUsed"/>
            InputAction,

            /// <summary>
            /// The input is read from an input action defined in the project.
            /// This is the default mode.
            /// </summary>
            InputActionReference,

            /// <summary>
            /// The input is read from an object reference that implements <see cref="IXRInputValueReader{TValue}"/>.
            /// </summary>
            ObjectReference,

            /// <summary>
            /// The input is returned from manually set values, either in the Inspector window or at runtime through scripting.
            /// </summary>
            /// <seealso cref="XRInputValueReader{TValue}.manualValue"/>
            ManualValue,
        }

        [SerializeField]
        // ReSharper disable once InconsistentNaming -- treat like a private field
        private protected InputSourceMode m_InputSourceMode = InputSourceMode.InputActionReference;

        /// <summary>
        /// The mode that determines from which input source the value is read from.
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
        // ReSharper disable once InconsistentNaming -- treat like a private field
        private protected InputAction m_InputAction;

        /// <summary>
        /// The directly serialized embedded input action that is read
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
        /// The reference to an input action that is read
        /// when the mode is set to <see cref="InputSourceMode.InputActionReference"/>.
        /// </summary>
        public InputActionReference inputActionReference
        {
            get => m_InputActionReference;
            set => m_InputActionReference = value;
        }

        readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferenceCache = new UnityObjectReferenceCache<InputActionReference>();

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputValueReader"/>.
        /// </summary>
        protected XRInputValueReader()
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputValueReader"/>.
        /// </summary>
        /// <param name="inputAction">The directly serialized embedded input action.</param>
        /// <param name="inputSourceMode">The initial input source mode.</param>
        protected XRInputValueReader(InputAction inputAction, InputSourceMode inputSourceMode)
        {
            m_InputAction = inputAction;
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
        /// A fast but unsafe method for getting the <see cref="inputActionReference"/> without doing a Unity Object alive check after the first time.
        /// The purpose is to avoid the overhead of the Unity Object alive check when it is known that the reference is alive,
        /// but does not have the rigor of detecting when the asset is deleted after the first check, which should be very rare.
        /// This will handle the user modifying the field in the Inspector window by invalidating the cached version.
        /// </summary>
        /// <param name="reference">The input action reference or actual <see langword="null"/>.</param>
        /// <returns>Returns <see langword="true"/> if the reference is not null. Otherwise, returns <see langword="false"/>.</returns>
        private protected bool TryGetInputActionReference(out InputActionReference reference) =>
            m_InputActionReferenceCache.TryGet(m_InputActionReference, out reference);
    }

    /// <summary>
    /// Serializable typed input value reader that can read the current value from an input source.
    /// Behaviors can declare a field of this type to allow them to read input from an input action or any other source.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to read, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
    [Serializable]
    public class XRInputValueReader<TValue> : XRInputValueReader, IXRInputValueReader<TValue> where TValue : struct
    {
        readonly struct BypassScope : IDisposable
        {
            readonly XRInputValueReader<TValue> m_Reader;

            public BypassScope(XRInputValueReader<TValue> reader)
            {
                m_Reader = reader;
                m_Reader.m_CallingBypass = true;
            }

            public void Dispose()
            {
                m_Reader.m_CallingBypass = false;
            }
        }

        [SerializeField]
        [RequireInterface(typeof(IXRInputValueReader))]
        Object m_ObjectReferenceObject;

        [SerializeField]
        TValue m_ManualValue;

        /// <summary>
        /// The manually set value that is returned
        /// when the mode is set to <see cref="XRInputValueReader.InputSourceMode.ManualValue"/>.
        /// </summary>
        public TValue manualValue
        {
            get => m_ManualValue;
            set => m_ManualValue = value;
        }

        /// <summary>
        /// A runtime bypass that can be used to override the input value returned by this class.
        /// </summary>
        public IXRInputValueReader<TValue> bypass { get; set; }

        /// <summary>
        /// Whether this class is calling into the bypass.
        /// The purpose of this is to allow the bypass to read the raw value of this class using the public methods
        /// without recursively calling into the bypass again.
        /// </summary>
        bool m_CallingBypass;

        readonly UnityObjectReferenceCache<IXRInputValueReader<TValue>, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRInputValueReader<TValue>, Object>();

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputValueReader{TValue}"/>.
        /// </summary>
        public XRInputValueReader()
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputValueReader{TValue}"/>.
        /// </summary>
        /// <param name="name">The name of the directly serialized embedded input action.</param>
        /// <param name="inputSourceMode">The initial input source mode.</param>
        public XRInputValueReader(string name = null, InputSourceMode inputSourceMode = InputSourceMode.InputActionReference)
            : base(InputActionUtility.CreateValueAction(typeof(TValue), name), inputSourceMode)
        {
        }

        /// <summary>
        /// Gets the object reference that is used as the input source
        /// when the mode is set to <see cref="XRInputValueReader.InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <returns>Returns the typed object reference, which may be <see langword="null"/>.</returns>
        public IXRInputValueReader<TValue> GetObjectReference()
        {
            return m_ObjectReference.Get(m_ObjectReferenceObject);
        }

        /// <summary>
        /// Sets the object reference that is used as the input source
        /// when the mode is set to <see cref="XRInputValueReader.InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <param name="value">The typed object reference.</param>
        /// <remarks>
        /// If the argument is to be serialized, it must be a Unity <see cref="Object"/> type.
        /// </remarks>
        public void SetObjectReference(IXRInputValueReader<TValue> value)
        {
            m_ObjectReference.Set(ref m_ObjectReferenceObject, value);
        }

        /// <inheritdoc />
        public TValue ReadValue()
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.ReadValue();
                }
            }

            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    return default;

                case InputSourceMode.InputAction:
                    return ReadValue(m_InputAction);

                case InputSourceMode.InputActionReference:
                    return TryGetInputActionReference(out var reference) ? ReadValue(reference.action) : default;

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        return objectReference?.ReadValue() ?? default;
                    }

                case InputSourceMode.ManualValue:
                    return m_ManualValue;
            }
        }

        /// <inheritdoc />
        public bool TryReadValue(out TValue value)
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.TryReadValue(out value);
                }
            }

            switch (inputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    value = default;
                    return false;

                case InputSourceMode.InputAction:
                    return TryReadValue(m_InputAction, out value);

                case InputSourceMode.InputActionReference:
                    if (TryGetInputActionReference(out var reference))
                        return TryReadValue(reference.action, out value);

                    value = default;
                    return false;

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        if (objectReference != null)
                            return objectReference.TryReadValue(out value);

                        value = default;
                        return false;
                    }

                case InputSourceMode.ManualValue:
                    value = m_ManualValue;
                    return true;
            }
        }

        static TValue ReadValue(InputAction action)
        {
            return action?.ReadValue<TValue>() ?? default;
        }

        static bool TryReadValue(InputAction action, out TValue value)
        {
            if (action == null)
            {
                value = default;
                return false;
            }

            value = action.ReadValue<TValue>();
            return action.IsInProgress();
        }
    }
}
