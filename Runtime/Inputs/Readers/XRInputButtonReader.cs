using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// Interface which allows for callers to read the button's state from an input source.
    /// </summary>
    /// <seealso cref="XRInputButtonReader"/>
    public interface IXRInputButtonReader : IXRInputValueReader<float>
    {
        /// <summary>
        /// Read whether the button is currently performed, which typically means whether the button is being pressed.
        /// This is typically true for multiple frames.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the button is performed. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// For input actions, this depends directly on the interaction(s) driving the action (including the
        /// default interaction if no specific interaction has been added to the action or binding).
        /// </remarks>
        bool ReadIsPerformed();

        /// <summary>
        /// Read whether the button performed this frame, which typically means whether the button started being pressed during this frame.
        /// This is typically only true for one single frame.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the button performed this frame. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// For input actions, this depends directly on the interaction(s) driving the action (including the
        /// default interaction if no specific interaction has been added to the action or binding).
        /// </remarks>
        bool ReadWasPerformedThisFrame();

        /// <summary>
        /// Read whether the button completed this frame, which typically means whether the button stopped being pressed during this frame.
        /// This is typically only true for one single frame.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the button completed this frame. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// For input actions, this depends directly on the interaction(s) driving the action (including the
        /// default interaction if no specific interaction has been added to the action or binding).
        /// </remarks>
        bool ReadWasCompletedThisFrame();
    }

    /// <summary>
    /// Serializable typed input button reader that can read the current button's state from an input source.
    /// Behaviors can declare a field of this type to allow them to read input from an input action or any other source.
    /// </summary>
    [Serializable]
    public class XRInputButtonReader : IXRInputButtonReader
    {
        struct BypassScope : IDisposable
        {
            readonly XRInputButtonReader m_Reader;

            public BypassScope(XRInputButtonReader reader)
            {
                m_Reader = reader;
                m_Reader.m_CallingBypass = true;
            }

            public void Dispose()
            {
                m_Reader.m_CallingBypass = false;
            }
        }

        /// <summary>
        /// The mode that determines from which input source the button's state is read from.
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
            /// <seealso cref="inputActionPerformed"/>
            /// <seealso cref="inputActionValue"/>
            /// <seealso cref="EnableDirectActionIfModeUsed"/>
            /// <seealso cref="DisableDirectActionIfModeUsed"/>
            InputAction,

            /// <summary>
            /// The input is read from an input action defined in the project.
            /// This is the default mode.
            /// </summary>
            /// <seealso cref="inputActionReferencePerformed"/>
            /// <seealso cref="inputActionReferenceValue"/>
            InputActionReference,

            /// <summary>
            /// The input is read from an object reference that implements <see cref="IXRInputButtonReader"/>.
            /// </summary>
            /// <seealso cref="GetObjectReference"/>
            /// <seealso cref="SetObjectReference"/>
            ObjectReference,

            /// <summary>
            /// The input is returned from manually set values, either in the Inspector window or at runtime through scripting.
            /// </summary>
            /// <seealso cref="manualPerformed"/>
            /// <seealso cref="manualValue"/>
            /// <seealso cref="manualFramePerformed"/>
            /// <seealso cref="manualFrameCompleted"/>
            ManualValue,
        }

        [SerializeField]
        InputSourceMode m_InputSourceMode = InputSourceMode.InputActionReference;

        /// <summary>
        /// The mode that determines from which input source the button's state is read from.
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
        InputAction m_InputActionPerformed;

        /// <summary>
        /// The directly serialized embedded input action that is read to determine whether the button is down
        /// when the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        /// <remarks>
        /// Must have a button-like interaction where phase equals performed when down.
        /// This is the case with an input action whose Action Type is Button
        /// or whose Action Type is Value with an interaction like Press or Sector.
        /// </remarks>
        public InputAction inputActionPerformed
        {
            get => m_InputActionPerformed;
            set => m_InputActionPerformed = value;
        }

        [SerializeField]
        InputAction m_InputActionValue;

        /// <summary>
        /// The directly serialized embedded input action that is read to determine the scalar value that varies from 0 to 1
        /// when the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        public InputAction inputActionValue
        {
            get => m_InputActionValue;
            set => m_InputActionValue = value;
        }

        [SerializeField]
        InputActionReference m_InputActionReferencePerformed;

        /// <summary>
        /// The reference to an input action that is read to determine whether the button is down
        /// when the mode is set to <see cref="InputSourceMode.InputActionReference"/>.
        /// </summary>
        /// <remarks>
        /// Must have a button-like interaction where phase equals performed when down.
        /// This is the case with an input action whose Action Type is Button
        /// or whose Action Type is Value with an interaction like Press or Sector.
        /// </remarks>
        public InputActionReference inputActionReferencePerformed
        {
            get => m_InputActionReferencePerformed;
            set => m_InputActionReferencePerformed = value;
        }

        [SerializeField]
        InputActionReference m_InputActionReferenceValue;

        /// <summary>
        /// The reference to an input action that is read to determine the scalar value that varies from 0 to 1
        /// when the mode is set to <see cref="InputSourceMode.InputActionReference"/>.
        /// </summary>
        public InputActionReference inputActionReferenceValue
        {
            get => m_InputActionReferenceValue;
            set => m_InputActionReferenceValue = value;
        }

        [SerializeField]
        [RequireInterface(typeof(IXRInputButtonReader))]
        Object m_ObjectReferenceObject;

        [SerializeField]
        bool m_ManualPerformed;

        /// <summary>
        /// The manually set performed state that is returned
        /// when the mode is set to <see cref="InputSourceMode.ManualValue"/>.
        /// </summary>
        /// <seealso cref="manualFramePerformed"/>
        /// <seealso cref="manualFrameCompleted"/>
        /// <seealso cref="IXRInputButtonReader.ReadIsPerformed"/>
        public bool manualPerformed
        {
            get => m_ManualPerformed;
            set => m_ManualPerformed = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_ManualValue;

        /// <summary>
        /// The manually set scalar value that varies from 0 to 1 that is returned
        /// when the mode is set to <see cref="InputSourceMode.ManualValue"/>.
        /// </summary>
        public float manualValue
        {
            get => m_ManualValue;
            set => m_ManualValue = value;
        }

        // Serialized to allow the property drawer to queue for the next frame, repeating logic in QueueManualState
        [SerializeField]
        bool m_ManualQueuePerformed;
        [SerializeField]
        bool m_ManualQueueWasPerformedThisFrame;
        [SerializeField]
        bool m_ManualQueueWasCompletedThisFrame;
        [SerializeField]
        float m_ManualQueueValue;
        [SerializeField]
        int m_ManualQueueTargetFrame;

        // Not serialized
        int m_ManualFramePerformed;

        /// <summary>
        /// The frame that the manual performed state was set to <see langword="true"/>
        /// when the mode is set to <see cref="InputSourceMode.ManualValue"/>.
        /// </summary>
        /// <seealso cref="manualPerformed"/>
        /// <seealso cref="IXRInputButtonReader.ReadWasPerformedThisFrame"/>
        public int manualFramePerformed
        {
            get => m_ManualFramePerformed;
            set => m_ManualFramePerformed = value;
        }

        // Not serialized
        int m_ManualFrameCompleted;

        /// <summary>
        /// The frame that the manual performed state was set to <see langword="false"/>
        /// when the mode is set to <see cref="InputSourceMode.ManualValue"/>.
        /// </summary>
        /// <seealso cref="manualPerformed"/>
        /// <seealso cref="IXRInputButtonReader.ReadWasCompletedThisFrame"/>
        public int manualFrameCompleted
        {
            get => m_ManualFrameCompleted;
            set => m_ManualFrameCompleted = value;
        }

        /// <summary>
        /// A runtime bypass that can be used to override the button input state returned by this class.
        /// </summary>
        public IXRInputButtonReader bypass { get; set; }

        /// <summary>
        /// Whether this class is calling into the bypass.
        /// The purpose of this is to allow the bypass to read the raw value of this class using the public methods
        /// without recursively calling into the bypass again.
        /// </summary>
        bool m_CallingBypass;

        readonly UnityObjectReferenceCache<IXRInputButtonReader, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRInputButtonReader, Object>();

        readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferencePerformedCache = new UnityObjectReferenceCache<InputActionReference>();
        readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferenceValueCache = new UnityObjectReferenceCache<InputActionReference>();

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputButtonReader"/>.
        /// </summary>
        public XRInputButtonReader()
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRInputButtonReader"/>.
        /// </summary>
        /// <param name="name">The name of the directly serialized embedded input action for performed.</param>
        /// <param name="valueName">The name of the directly serialized embedded input action for the scalar value. Based off of <paramref name="name"/> if left <see langword="null"/>.</param>
        /// <param name="wantsInitialStateCheck">Whether the action should start performing if already actuated when the action is enabled.</param>
        /// <param name="inputSourceMode">The initial input source mode.</param>
        public XRInputButtonReader(string name = null, string valueName = null, bool wantsInitialStateCheck = false, InputSourceMode inputSourceMode = InputSourceMode.InputActionReference)
        {
            m_InputActionPerformed = InputActionUtility.CreateButtonAction(name, wantsInitialStateCheck);
            m_InputActionValue = InputActionUtility.CreateValueAction(typeof(float), valueName ?? (name != null ? (name + " Value") : null));
            m_InputSourceMode = inputSourceMode;
        }

        /// <summary>
        /// Enable the directly serialized embedded input action if the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        /// <seealso cref="DisableDirectActionIfModeUsed"/>
        public void EnableDirectActionIfModeUsed()
        {
            if (m_InputSourceMode == InputSourceMode.InputAction)
            {
                m_InputActionPerformed.Enable();
                m_InputActionValue.Enable();
            }
        }

        /// <summary>
        /// Disable the directly serialized embedded input action if the mode is set to <see cref="InputSourceMode.InputAction"/>.
        /// </summary>
        /// <seealso cref="EnableDirectActionIfModeUsed"/>
        public void DisableDirectActionIfModeUsed()
        {
            if (m_InputSourceMode == InputSourceMode.InputAction)
            {
                m_InputActionPerformed.Disable();
                m_InputActionValue.Disable();
            }
        }

        /// <summary>
        /// Gets the object reference that is used as the input source
        /// when the mode is set to <see cref="InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <returns>Returns the object reference, which may be <see langword="null"/>.</returns>
        public IXRInputButtonReader GetObjectReference()
        {
            return m_ObjectReference.Get(m_ObjectReferenceObject);
        }

        /// <summary>
        /// Sets the object reference that is used as the input source
        /// when the mode is set to <see cref="InputSourceMode.ObjectReference"/>.
        /// </summary>
        /// <param name="value">The object reference.</param>
        /// <remarks>
        /// If the argument is to be serialized, it must be a Unity <see cref="Object"/> type.
        /// </remarks>
        public void SetObjectReference(IXRInputButtonReader value)
        {
            m_ObjectReference.Set(ref m_ObjectReferenceObject, value);
        }

        /// <summary>
        /// Queue a manual state to be effective on the next frame.
        /// This method automatically determines whether the button should be considered as newly performed that frame based on the current state.
        /// </summary>
        /// <param name="performed">The manual button performed state that should be returned next frame.</param>
        /// <param name="value">The manual scalar value that varies from 0 to 1 that should be returned next frame.</param>
        public void QueueManualState(bool performed, float value) =>
            QueueManualState(performed, value, !m_ManualPerformed && performed, m_ManualPerformed && !performed);

        /// <summary>
        /// Queue a manual state to be effective on the next frame.
        /// </summary>
        /// <param name="performed">The manual button performed state that should be returned next frame.</param>
        /// <param name="value">The manual scalar value that varies from 0 to 1 that should be returned next frame.</param>
        /// <param name="performedThisFrame">Whether the manual button should be considered as performed that frame on the next frame.</param>
        /// <param name="completedThisFrame">Whether the manual button should be considered as completed that frame on the next frame.</param>
        public void QueueManualState(bool performed, float value, bool performedThisFrame, bool completedThisFrame)
        {
            if (m_InputSourceMode != InputSourceMode.ManualValue)
                Debug.LogWarning($"QueueManualState was called but the input source mode is set to {m_InputSourceMode}." +
                    $"You may want to set {nameof(inputSourceMode)} to {nameof(InputSourceMode.ManualValue)} for the manual state to be effective next frame.");

            m_ManualQueuePerformed = performed;
            m_ManualQueueWasPerformedThisFrame = performedThisFrame;
            m_ManualQueueWasCompletedThisFrame = completedThisFrame;
            m_ManualQueueValue = value;
            m_ManualQueueTargetFrame = Time.frameCount + 1;
        }

        void RefreshManualIfNeeded()
        {
            if (m_ManualQueueTargetFrame > 0 && Time.frameCount >= m_ManualQueueTargetFrame)
            {
                m_ManualPerformed = m_ManualQueuePerformed;
                if (m_ManualQueueWasPerformedThisFrame)
                    m_ManualFramePerformed = Time.frameCount;
                if (m_ManualQueueWasCompletedThisFrame)
                    m_ManualFrameCompleted = Time.frameCount;
                m_ManualValue = m_ManualQueueValue;

                m_ManualQueueTargetFrame = 0;
            }
        }

        /// <inheritdoc />
        public bool ReadIsPerformed()
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.ReadIsPerformed();
                }
            }

            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    return false;

                case InputSourceMode.InputAction:
                    return IsPerformed(m_InputActionPerformed);

                case InputSourceMode.InputActionReference:
                    return TryGetInputActionReferencePerformed(out var reference) && IsPerformed(reference.action);

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        return objectReference?.ReadIsPerformed() ?? false;
                    }

                case InputSourceMode.ManualValue:
                    RefreshManualIfNeeded();
                    return m_ManualPerformed;
            }
        }

        /// <inheritdoc />
        public bool ReadWasPerformedThisFrame()
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.ReadWasPerformedThisFrame();
                }
            }

            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    return false;

                case InputSourceMode.InputAction:
                    return WasPerformedThisFrame(m_InputActionPerformed);

                case InputSourceMode.InputActionReference:
                    return TryGetInputActionReferencePerformed(out var reference) && WasPerformedThisFrame(reference.action);

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        return objectReference?.ReadWasPerformedThisFrame() ?? false;
                    }

                case InputSourceMode.ManualValue:
                    RefreshManualIfNeeded();
                    return m_ManualPerformed && m_ManualFramePerformed == Time.frameCount;
            }
        }

        /// <inheritdoc />
        public bool ReadWasCompletedThisFrame()
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.ReadWasCompletedThisFrame();
                }
            }

            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    return false;

                case InputSourceMode.InputAction:
                    return WasCompletedThisFrame(m_InputActionPerformed);

                case InputSourceMode.InputActionReference:
                    return TryGetInputActionReferencePerformed(out var reference) && WasCompletedThisFrame(reference.action);

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        return objectReference?.ReadWasCompletedThisFrame() ?? false;
                    }

                case InputSourceMode.ManualValue:
                    RefreshManualIfNeeded();
                    return !m_ManualPerformed && m_ManualFrameCompleted == Time.frameCount;
            }
        }

        /// <inheritdoc />
        public float ReadValue()
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
                    return ReadValueToFloat(m_InputActionValue);

                case InputSourceMode.InputActionReference:
                    return TryGetInputActionReferenceValue(out var reference) ? ReadValueToFloat(reference.action) : default;

                case InputSourceMode.ObjectReference:
                    {
                        var objectReference = GetObjectReference();
                        return objectReference?.ReadValue() ?? default;
                    }

                case InputSourceMode.ManualValue:
                    RefreshManualIfNeeded();
                    return m_ManualValue;
            }
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            if (bypass != null && !m_CallingBypass)
            {
                using (new BypassScope(this))
                {
                    return bypass.TryReadValue(out value);
                }
            }

            switch (m_InputSourceMode)
            {
                case InputSourceMode.Unused:
                default:
                    value = default;
                    return false;

                case InputSourceMode.InputAction:
                    return TryReadValue(m_InputActionValue, out value);

                case InputSourceMode.InputActionReference:
                    if (TryGetInputActionReferenceValue(out var reference))
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
                    RefreshManualIfNeeded();
                    value = m_ManualValue;
                    return true;
            }
        }

        static bool IsPerformed(InputAction action)
        {
            if (action == null)
                return false;

            var phase = action.phase;
            return phase == InputActionPhase.Performed || (phase != InputActionPhase.Disabled && action.WasPerformedThisFrame());
        }

        static bool WasPerformedThisFrame(InputAction action)
        {
            return action != null && action.WasPerformedThisFrame();
        }

        static bool WasCompletedThisFrame(InputAction action)
        {
            return action != null && action.WasCompletedThisFrame();
        }

        float ReadValueToFloat(InputAction action)
        {
            if (action == null)
                return default;

            // Processors such as AxisDeadzone or ScaleVector2 can cause the action's value (obtained with ReadValue)
            // to be different from the control's magnitude (obtained with GetControlMagnitude).
            // Evaluating whether the root binding, composite binding, or the action has any processors is not trivial.
            // We evaluate the magnitude of the read Vector2 value instead of using GetControlMagnitude to be safe.

            var activeValueType = action.activeValueType;
            if (activeValueType == null || activeValueType == typeof(float))
                return action.ReadValue<float>();

            if (activeValueType == typeof(Vector2))
                return action.ReadValue<Vector2>().magnitude;

            return Mathf.Max(action.GetControlMagnitude(), 0f);
        }

        bool TryReadValue(InputAction action, out float value)
        {
            if (action == null)
            {
                value = default;
                return false;
            }

            value = ReadValueToFloat(action);
            return action.IsInProgress();
        }

        bool TryGetInputActionReferencePerformed(out InputActionReference reference) =>
            m_InputActionReferencePerformedCache.TryGet(m_InputActionReferencePerformed, out reference);

        bool TryGetInputActionReferenceValue(out InputActionReference reference) =>
            m_InputActionReferenceValueCache.TryGet(m_InputActionReferenceValue, out reference);
    }
}
