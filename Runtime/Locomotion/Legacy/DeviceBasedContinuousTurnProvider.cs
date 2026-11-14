using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly rotate their rig continuously over time
    /// using a specified 2D axis input.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="DeviceBasedSnapTurnProvider"/>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [AddComponentMenu("XR/Locomotion/Legacy/Continuous Turn Provider (Device-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_DeviceBasedContinuousTurnProvider)]
    [Obsolete(k_DeviceBasedContinuousTurnProviderDeprecated, true)]
    public class DeviceBasedContinuousTurnProvider : ContinuousTurnProviderBase
    {
        const string k_DeviceBasedContinuousTurnProviderDeprecated = "DeviceBasedContinuousTurnProvider has been deprecated in version 3.0.0. Use ContinuousTurnProvider instead.";

        /// <summary>
        /// Sets which input axis to use when reading from controller input.
        /// </summary>
        /// <seealso cref="inputBinding"/>
        public enum InputAxes
        {
            /// <summary>
            /// Use the primary touchpad or joystick on a device.
            /// </summary>
            Primary2DAxis = 0,
            /// <summary>
            /// Use the secondary touchpad or joystick on a device.
            /// </summary>
            Secondary2DAxis = 1,
        }

        /// <summary>
        /// The 2D Input Axis on the controller devices that will be used to trigger a turn.
        /// </summary>
        public InputAxes inputBinding
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The XRControllers that allow turning. An XRController must be enabled in order to turn.
        /// </summary>
        public List<XRBaseController> controllers
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Value below which input values will be clamped. After clamping, values will be renormalized to [0, 1] between min and max.
        /// </summary>
        public float deadzoneMin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Value above which input values will be clamped. After clamping, values will be renormalized to [0, 1] between min and max.
        /// </summary>
        public float deadzoneMax
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override Vector2 ReadInput()
        {
            throw new NotSupportedException(k_DeviceBasedContinuousTurnProviderDeprecated);
        }

        /// <summary>
        /// Gets value adjusted based on deadzone thresholds.
        /// </summary>
        /// <param name="value">The value to be adjusted.</param>
        /// <returns>Returns adjusted 2D vector.</returns>
        protected Vector2 GetDeadzoneAdjustedValue(Vector2 value)
        {
            throw new NotSupportedException(k_DeviceBasedContinuousTurnProviderDeprecated);
        }

        /// <summary>
        /// Gets value adjusted based on deadzone thresholds.
        /// </summary>
        /// <param name="value">The value to be adjusted.</param>
        /// <returns>Returns adjusted value.</returns>
        protected float GetDeadzoneAdjustedValue(float value)
        {
            throw new NotSupportedException(k_DeviceBasedContinuousTurnProviderDeprecated);
        }
    }
}
