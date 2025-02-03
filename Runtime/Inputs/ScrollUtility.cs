using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
    /// <summary>
    /// Helper class to normalize scroll input data with compensation for bugged versions of Unity which return unexpected
    /// ranges of values.
    /// </summary>
    static class ScrollUtility
    {
        /// <summary>
        /// This <c>120f</c> const is the expected platform specific scroll amount on Windows that you would typically
        /// use as the divisor to convert from platform specific, matching would be reported with uniform scroll values.
        /// </summary>
        /// <remarks>
        /// For example, <c>(0.00, -120.00)</c> dividing by this <c>120</c> to produce <c>(0.00, -1.00)</c> for a single scroll down.
        /// <br />
        /// Note this may not be the actual conversion divisor to convert from platform specific to normalized for this
        /// version of Unity as some versions incorrectly report input values as if it was set to uniform instead.
        /// </remarks>
        public const float windowsPlatformSpecificDivisor = 120f;

        /// <summary>
        /// This <c>20f</c> const is the expected platform specific scroll amount on macOS that you would typically
        /// use as the divisor to convert from platform specific, matching would be reported with uniform scroll values.
        /// </summary>
        /// <remarks>
        /// For example, <c>(0.00, 2.00)</c> dividing by this <c>20</c> to produce <c>(0.00, 0.10)</c> for a single scroll down.
        /// Note that due to scroll being non-linear where faster scrolling produces a larger value on some frames, a single
        /// slow scroll down may not produce the same amount scrolled in a scroll rect.
        /// <br />
        /// Note this may not be the actual conversion divisor to convert from platform specific to normalized for this
        /// version of Unity as some versions incorrectly report input values as if it was set to uniform instead.
        /// </remarks>
        public const float macPlatformSpecificDivisor = 20f;

#if UNITY_INPUT_SYSTEM_PLATFORM_SCROLL_DELTA && INPUT_SYSTEM_1_9_OR_NEWER // Project Setting was added with 6000.0.9 and 1.9.0
        static bool s_Subscribed;
#endif

        static bool s_DivisorComputed;

        /// <summary>
        /// The divisor of the scroll wheel delta to calculate the number of ticks of rotation of the scroll wheel.
        /// For example, if on Windows and Keep Platform Specific Input Range is enabled, this value will usually be <c>120f</c>.
        /// If on macOS and Keep Platform Specific Input Range is enabled, this value will be <c>20f</c>.
        /// Otherwise, this value will be <c>1f</c> meaning the input system scroll wheel delta value is already normalized.
        /// </summary>
        /// <remarks>
        /// This is similar to <c>InputSystem.InputSystem.scrollWheelDeltaPerTick</c> but with proper compensation
        /// for various Unity versions where the implementation caused in incorrect value.
        /// </remarks>
        static float s_Divisor;

        /// <summary>
        /// When on Windows in certain patch versions of Unity 6.0, compute the divisor dynamically based on the input.
        /// </summary>
        static bool s_ComputeDivisorBasedOnInput;

        /// <summary>
        /// Returns a normalized scroll input data. This puts it in terms of the "Uniform Across All Platforms" values.
        /// For example, instead of a Windows value of (0, -120) it will return (0, -1).
        /// </summary>
        /// <param name="input">The input system scroll value to process.</param>
        /// <returns>Returns the normalized scroll data.</returns>
        /// <remarks>
        /// This does not do any other platform processing like reversing scroll direction for Windows and macOS defaults
        /// to match values for the same direction scrolled on the mouse, nor keep the same value for the same number
        /// of ticks scrolled, as macOS reports non-linear values based on the speed of the scroll and thus one tick
        /// scrolled can produce different results between platforms.
        /// </remarks>
        public static Vector2 GetNormalized(in Vector2 input)
        {
            // Compute the divisor if this is the first time called
            if (!s_DivisorComputed)
                RefreshDivisor();

            if (s_ComputeDivisorBasedOnInput)
            {
                if (Mathf.Abs(input.y) >= windowsPlatformSpecificDivisor || Mathf.Abs(input.x) >= windowsPlatformSpecificDivisor)
                    s_Divisor = windowsPlatformSpecificDivisor;
                else
                    s_Divisor = 1f;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (s_Divisor == 1f || s_Divisor == 0f)
                return input;

            return input / s_Divisor;
        }

#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
        static void RefreshDivisor()
        {
            // Mouse scroll delta is assumed to be reported as already normalized when accessed through the old legacy Input Manager
            s_ComputeDivisorBasedOnInput = false;
            s_Divisor = 1f;
            s_DivisorComputed = true;
        }
#elif UNITY_INPUT_SYSTEM_PLATFORM_SCROLL_DELTA && INPUT_SYSTEM_1_9_OR_NEWER // Project Setting was added with 6000.0.9 and 1.9.0
        static void RefreshDivisor()
        {
            s_ComputeDivisorBasedOnInput = false;

            // Subscribe to the Input System project setting changes since the value
            // can change as the Scroll Delta Behavior property changes.
            if (!s_Subscribed)
            {
                InputSystem.InputSystem.onSettingsChange += OnInputSystemSettingsChanged;
                s_Subscribed = true;
            }

            if (Application.platform is RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer)
            {
                var scrollDeltaBehavior = InputSystem.InputSystem.settings.scrollDeltaBehavior;
                if (scrollDeltaBehavior == InputSettings.ScrollDeltaBehavior.UniformAcrossAllPlatforms)
                {
                    s_Divisor = 1f;
                }
                else if (scrollDeltaBehavior == InputSettings.ScrollDeltaBehavior.KeepPlatformSpecificInputRange)
                {
#if UNITY_6000_0_57_OR_NEWER_PATCH // Unknown end of the range until the patch to fix Unity 6.0 is released.
                    // Must dynamically guess the value based on inputs.
                    s_ComputeDivisorBasedOnInput = true;
                    // Placeholder value until computed during GetNormalized
                    s_Divisor = 1f;
#else
                    s_Divisor = windowsPlatformSpecificDivisor;
#endif
                }
            }
            else if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer)
            {
                s_Divisor = macPlatformSpecificDivisor;

                var scrollDeltaBehavior = InputSystem.InputSystem.settings.scrollDeltaBehavior;
                if (scrollDeltaBehavior == InputSettings.ScrollDeltaBehavior.UniformAcrossAllPlatforms)
                {
#if UNITY_6000_0_23_THROUGH_54 // [6000.0.23f1, 6000.0.54f1] inclusive
                    // Compensate for bugged input values where macOS reports platform specific values instead of uniform
                    s_Divisor = macPlatformSpecificDivisor;
#else
                    s_Divisor = 1f;
#endif
                }
            }
            else
            {
                s_Divisor = 1f;
            }

            s_DivisorComputed = true;
        }
#else
        static void RefreshDivisor()
        {
            s_ComputeDivisorBasedOnInput = false;

            // On older versions of Unity and Input System prior to Unity 6.0, the input system scroll values are
            // always platform specific instead of uniform.
            if (Application.platform is RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer)
                s_Divisor = windowsPlatformSpecificDivisor;
            else if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer)
                s_Divisor = macPlatformSpecificDivisor;
            else
                s_Divisor = 1f;

            s_DivisorComputed = true;
        }
#endif

#if UNITY_INPUT_SYSTEM_PLATFORM_SCROLL_DELTA && INPUT_SYSTEM_1_9_OR_NEWER // Project Setting was added with 6000.0.9 and 1.9.0
        static void OnInputSystemSettingsChanged()
        {
            // Re-compute the divisor since the project settings changed
            RefreshDivisor();
        }
#endif
    }
}
