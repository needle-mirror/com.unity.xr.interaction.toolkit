#if OPENXR_1_6_OR_NEWER || PACKAGE_DOCS_GENERATION
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.OpenXR
{
    /// <summary>
    /// Allows for sending haptic impulses to a channel on a device specified by a haptic control.
    /// Only available when the OpenXR Plugin (com.unity.xr.openxr) package is installed.
    /// </summary>
    /// <seealso cref="IXRHapticImpulseChannel"/>
    /// <seealso cref="OpenXRInput"/>
    public class OpenXRHapticImpulseChannel : IXRHapticImpulseChannel
    {
        /// <summary>
        /// Action to send haptic impulse through.
        /// </summary>
        public InputAction hapticAction { get; set; }

        /// <summary>
        /// The input device to send the impulse to.
        /// </summary>
        public InputSystem.InputDevice device { get; set; }

        /// <inheritdoc />
        public bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
            var actionHandle = OpenXRInput.GetActionHandle(hapticAction);
            if (actionHandle == 0L)
                return false;

            OpenXRInput.SendHapticImpulse(hapticAction, amplitude, frequency, duration, device);
            return true;
        }
    }
}
#endif
