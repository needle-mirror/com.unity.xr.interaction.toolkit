using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// Utility class that provides useful methods for behaviors that use input readers.
    /// </summary>
    /// <seealso cref="XRInputButtonReader"/>
    /// <seealso cref="XRInputValueReader{TValue}"/>
    /// <seealso cref="XRInputHapticImpulseProvider"/>
    public static class XRInputReaderUtility
    {
        /// <summary>
        /// Helper method for setting an input property which automatically enables or disables
        /// directly serialized embedded input actions during Play mode.
        /// </summary>
        /// <param name="property">The <see langword="ref"/> to the field.</param>
        /// <param name="value">The new value being set.</param>
        /// <param name="behavior">The behavior with the property being set.</param>
        /// <remarks>
        /// <example>
        /// This example demonstrates code of a MonoBehaviour that uses an input property:
        /// <code>
        /// [SerializeField]
        /// XRInputHapticImpulseProvider m_HapticOutput = new XRInputHapticImpulseProvider("Haptic");
        ///
        /// public XRInputHapticImpulseProvider hapticOutput
        /// {
        ///     get => m_HapticOutput;
        ///     set => XRInputReaderUtility.SetInputProperty(ref m_HapticOutput, value, this);
        /// }
        ///
        /// void OnEnable()
        /// {
        ///     m_HapticOutput.EnableDirectActionIfModeUsed();
        /// }
        ///
        /// void OnDisable()
        /// {
        ///     m_HapticOutput.DisableDirectActionIfModeUsed();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public static void SetInputProperty(ref XRInputHapticImpulseProvider property, XRInputHapticImpulseProvider value, Behaviour behavior)
        {
            if (value == null)
            {
                Debug.LogError("Setting XRInputHapticImpulseProvider property to null is disallowed and has therefore been ignored.", behavior);
                return;
            }

            if (Application.isPlaying)
                property?.DisableDirectActionIfModeUsed();

            property = value;

            if (Application.isPlaying && behavior.isActiveAndEnabled)
                property.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// Helper method for setting an input property which automatically enables or disables
        /// directly serialized embedded input actions during Play mode.
        /// </summary>
        /// <param name="property">The <see langword="ref"/> to the field.</param>
        /// <param name="value">The new value being set.</param>
        /// <param name="behavior">The behavior with the property being set.</param>
        /// <remarks>
        /// <example>
        /// This example demonstrates code of a MonoBehaviour that uses an input property:
        /// <code>
        /// [SerializeField]
        /// XRInputButtonReader m_GrabMoveInput = new XRInputButtonReader("Grab Move");
        ///
        /// public XRInputButtonReader grabMoveInput
        /// {
        ///     get => m_GrabMoveInput;
        ///     set => XRInputReaderUtility.SetInputProperty(ref m_GrabMoveInput, value, this);
        /// }
        ///
        /// void OnEnable()
        /// {
        ///     m_GrabMoveInput.EnableDirectActionIfModeUsed();
        /// }
        ///
        /// void OnDisable()
        /// {
        ///     m_GrabMoveInput.DisableDirectActionIfModeUsed();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public static void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value, Behaviour behavior)
        {
            if (value == null)
            {
                Debug.LogError("Setting XRInputButtonReader property to null is disallowed and has therefore been ignored.", behavior);
                return;
            }

            if (Application.isPlaying)
                property?.DisableDirectActionIfModeUsed();

            property = value;

            if (Application.isPlaying && behavior.isActiveAndEnabled)
                property.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// Helper method for setting an input property which automatically enables or disables
        /// directly serialized embedded input actions during Play mode.
        /// </summary>
        /// <param name="property">The <see langword="ref"/> to the field.</param>
        /// <param name="value">The new value being set.</param>
        /// <param name="behavior">The behavior with the property being set.</param>
        /// <typeparam name="TValue">Type of the value read by the property, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
        /// <remarks>
        /// <example>
        /// This example demonstrates code of a MonoBehaviour that uses an input property:
        /// <code>
        /// [SerializeField]
        /// XRInputValueReader&lt;Vector2&gt; m_LeftHandMoveInput = new XRInputValueReader&lt;Vector2&gt;("Left Hand Move");
        ///
        /// public XRInputValueReader&lt;Vector2&gt; leftHandMoveInput
        /// {
        ///     get => m_LeftHandMoveInput;
        ///     set => XRInputReaderUtility.SetInputProperty(ref m_LeftHandMoveInput, value, this);
        /// }
        ///
        /// void OnEnable()
        /// {
        ///     m_LeftHandMoveInput.EnableDirectActionIfModeUsed();
        /// }
        ///
        /// void OnDisable()
        /// {
        ///     m_LeftHandMoveInput.DisableDirectActionIfModeUsed();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public static void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value, Behaviour behavior) where TValue : struct
        {
            if (value == null)
            {
                Debug.LogError("Setting XRInputValueReader property to null is disallowed and has therefore been ignored.", behavior);
                return;
            }

            if (Application.isPlaying)
                property?.DisableDirectActionIfModeUsed();

            property = value;

            if (Application.isPlaying && behavior.isActiveAndEnabled)
                property.EnableDirectActionIfModeUsed();
        }

        internal static void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value, Behaviour behavior, List<XRInputButtonReader> buttonReaders)
        {
            if (value == null)
            {
                Debug.LogError("Setting XRInputButtonReader property to null is disallowed and has therefore been ignored.", behavior);
                return;
            }

            if (Application.isPlaying && property != null)
            {
                buttonReaders?.Remove(property);
                property.DisableDirectActionIfModeUsed();
            }

            property = value;

            if (Application.isPlaying)
            {
                buttonReaders?.Add(property);
                if (behavior.isActiveAndEnabled)
                    property.EnableDirectActionIfModeUsed();
            }
        }

        internal static void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value, Behaviour behavior, List<XRInputValueReader> valueReaders) where TValue : struct
        {
            if (value == null)
            {
                Debug.LogError("Setting XRInputValueReader property to null is disallowed and has therefore been ignored.", behavior);
                return;
            }

            if (Application.isPlaying && property != null)
            {
                valueReaders?.Remove(property);
                property.DisableDirectActionIfModeUsed();
            }

            property = value;

            if (Application.isPlaying)
            {
                valueReaders?.Add(property);
                if (behavior.isActiveAndEnabled)
                    property.EnableDirectActionIfModeUsed();
            }
        }
    }
}
