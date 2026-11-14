using System;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Helper class for different kinds of input.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [Obsolete(k_InputHelpersDeprecated, true)]
    public static class InputHelpers
    {
        const string k_InputHelpersDeprecated = "InputHelpers has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.";
        const string k_IsPressedDeprecated = "IsPressed has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader instead.";
        const string k_TryReadSingleValueDeprecated = "TryReadSingleValue has been deprecated in version 3.0.0. Use XRInputDeviceValueReader instead.";
        const string k_TryReadAxis2DValueDeprecated = "TryReadAxis2DValue has been deprecated in version 3.0.0. Use XRInputDeviceValueReader instead.";

        /// <summary>
        /// A list of buttons that can be bound to.
        /// </summary>
        [Obsolete("Button has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.", true)]
        public enum Button
        {
            /// <summary>
            /// Represents and invalid button.
            /// </summary>
            None = 0,

            /// <summary>
            /// Represents a menu button, used to pause, go back, or otherwise exit gameplay.
            /// </summary>
            MenuButton,

            /// <summary>
            /// A binary measure of whether the index finger is activating the trigger.
            /// </summary>
            Trigger,

            /// <summary>
            /// Represents the user's grip on the controller.
            /// </summary>
            Grip,

            /// <summary>
            /// A binary measure of whether the index finger is activating the trigger.
            /// </summary>
            TriggerButton,

            /// <summary>
            /// A binary measure of whether the device is being gripped.
            /// </summary>
            GripButton,

            /// <summary>
            /// The primary face button being pressed on a device, or sole button if only one is available.
            /// </summary>
            PrimaryButton,

            /// <summary>
            /// The primary face button being touched on a device.
            /// </summary>
            PrimaryTouch,

            /// <summary>
            /// The secondary face button being pressed on a device.
            /// </summary>
            SecondaryButton,

            /// <summary>
            /// The secondary face button being touched on a device.
            /// </summary>
            SecondaryTouch,

            /// <summary>
            /// Represents the primary 2D axis being touched.
            /// </summary>
            Primary2DAxisTouch,

            /// <summary>
            /// Represents the primary 2D axis being clicked or otherwise depressed.
            /// </summary>
            Primary2DAxisClick,

            /// <summary>
            /// Represents the primary 2D axis being touched.
            /// </summary>
            Secondary2DAxisTouch,

            /// <summary>
            /// Represents the secondary 2D axis being clicked or otherwise depressed.
            /// </summary>
            Secondary2DAxisClick,

            /// <summary>
            /// Represents an upwards motion on the primary touchpad or joystick on a device.
            /// </summary>
            PrimaryAxis2DUp,

            /// <summary>
            /// Represents a downwards motion on the primary touchpad or joystick on a device.
            /// </summary>
            PrimaryAxis2DDown,

            /// <summary>
            /// Represents a leftwards motion on the primary touchpad or joystick on a device.
            /// </summary>
            PrimaryAxis2DLeft,

            /// <summary>
            /// Represents a rightwards motion on the primary touchpad or joystick on a device.
            /// </summary>
            PrimaryAxis2DRight,

            /// <summary>
            /// Represents an upwards motion on the secondary touchpad or joystick on a device.
            /// </summary>
            SecondaryAxis2DUp,

            /// <summary>
            /// Represents a downwards motion on the secondary touchpad or joystick on a device.
            /// </summary>
            SecondaryAxis2DDown,

            /// <summary>
            /// Represents a leftwards motion on the secondary touchpad or joystick on a device.
            /// </summary>
            SecondaryAxis2DLeft,

            /// <summary>
            /// Represents a rightwards motion on the secondary touchpad or joystick on a device.
            /// </summary>
            SecondaryAxis2DRight,
        }

        /// <summary>
        /// A list of 2D axis inputs that can be bound to.
        /// </summary>
        [Obsolete("Axis2D has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.", true)]
        public enum Axis2D
        {
            /// <summary>
            /// Represents an invalid 2D axis.
            /// </summary>
            None = 0,

            /// <summary>
            /// Represents the primary touchpad or joystick on a device.
            /// </summary>
            PrimaryAxis2D,

            /// <summary>
            /// Represents the secondary touchpad or joystick on a device.
            /// </summary>
            SecondaryAxis2D,
        }

        const float k_DefaultPressThreshold = 0.1f;

        /// <summary>
        /// Checks whether button is pressed or not.
        /// </summary>
        /// <param name="device"> The input device.</param>
        /// <param name="button"> The button that is being checked.</param>
        /// <param name="isPressed"> A boolean that will be true if button is pressed and false if not.</param>
        /// <param name="pressThreshold"> The threshold of what defines a press.</param>
        /// <returns>Returns <see langword="true"/> if device and button are valid. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete(k_IsPressedDeprecated, true)]
        public static bool IsPressed(this InputDevice device, Button button, out bool isPressed, float pressThreshold = -1.0f)
        {
            throw new NotSupportedException(k_IsPressedDeprecated);
        }

        /// <summary>
        /// Tries to read and return the supplied button value.
        /// </summary>
        /// <param name="device">The input device.</param>
        /// <param name="button">The button to read the value.</param>
        /// <param name="singleValue">The button value; it will have the default <see langword="float"/> value (<c>0f</c>) if the button value can't be read.</param>
        /// <returns>Returns <see langword="true"/> if the button value can be read. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete(k_TryReadSingleValueDeprecated, true)]
        public static bool TryReadSingleValue(this InputDevice device, Button button, out float singleValue)
        {
            throw new NotSupportedException(k_TryReadSingleValueDeprecated);
        }

        /// <summary>
        /// Tries to read and return the supplied 2D axis value.
        /// </summary>
        /// <param name="device">The input device.</param>
        /// <param name="axis2D">The 2D axis to read the value.</param>
        /// <param name="value">The 2D axis value; it will have the default <see cref="Vector2"/> value if the 2D axis value can't be read.</param>
        /// <returns>Returns <see langword="true"/> if the 2D axis value can be read. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete(k_TryReadAxis2DValueDeprecated, true)]
        public static bool TryReadAxis2DValue(this InputDevice device, Axis2D axis2D, out Vector2 value)
        {
            throw new NotSupportedException(k_TryReadAxis2DValueDeprecated);
        }
    }
}
