using System;
using System.Collections.Generic;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// A locomotion provider that allows the user to rotate their rig using a specified 2D axis input.
    /// The provider can take input from multiple different devices (such as Left and Right hands).
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [AddComponentMenu("XR/Locomotion/Legacy/Snap Turn Provider (Device-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_DeviceBasedSnapTurnProvider)]
    [Obsolete(k_DeviceBasedSnapTurnProviderDeprecated, true)]
    public class DeviceBasedSnapTurnProvider : SnapTurnProviderBase
    {
        const string k_DeviceBasedSnapTurnProviderDeprecated = "DeviceBasedSnapTurnProvider has been deprecated in version 3.0.0. Use SnapTurnProvider instead.";

        /// <summary>
        /// Sets which input axis to use when reading from controller input.
        /// </summary>
        /// <seealso cref="turnUsage"/>
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
        /// The 2D Input Axis on the controller devices that will be used to trigger a snap turn.
        /// </summary>
        public InputAxes turnUsage
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The XRControllers that allow SnapTurn.  An XRController must be enabled in order to Snap Turn.
        /// </summary>
        public List<XRBaseController> controllers
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The deadzone that the controller movement will have to be above to trigger a snap turn.
        /// </summary>
        public float deadZone
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override Vector2 ReadInput()
        {
            throw new NotSupportedException(k_DeviceBasedSnapTurnProviderDeprecated);
        }
    }
}
