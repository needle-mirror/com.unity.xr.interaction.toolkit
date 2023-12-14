using UnityEngine.InputSystem;
#if OPENXR_1_6_OR_NEWER
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.OpenXR;
using UnityEngine.XR.OpenXR.Input;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// Class to assist with getting a haptic impulse channel group from an input action,
    /// handling either an OpenXR <see cref="HapticControl"/> binding
    /// or an Any (<c>/*</c>) binding to identify an input device.
    /// </summary>
    public class HapticControlActionManager
    {
        readonly HapticImpulseCommandChannelGroup m_DeviceChannelGroup;

#if OPENXR_1_6_OR_NEWER
        readonly OpenXRHapticImpulseChannel m_OpenXRChannel;
        readonly HapticImpulseSingleChannelGroup m_OpenXRChannelGroup;
#endif

        /// <summary>
        /// Initializes and returns an instance of <see cref="HapticControlActionManager"/>.
        /// </summary>
        public HapticControlActionManager()
        {
            m_DeviceChannelGroup = new HapticImpulseCommandChannelGroup();
#if OPENXR_1_6_OR_NEWER
            m_OpenXRChannel = new OpenXRHapticImpulseChannel();
            m_OpenXRChannelGroup = new HapticImpulseSingleChannelGroup(m_OpenXRChannel);
#endif
        }

        /// <summary>
        /// Gets the haptic impulse channel group appropriate for the given input action.
        /// </summary>
        /// <param name="action">The input action that is either bound to an OpenXR haptic binding or identifies the device.</param>
        /// <returns>Returns the haptic impulse channel group appropriate for the given action.</returns>
        public IXRHapticImpulseChannelGroup GetChannelGroup(InputAction action)
        {
            if (action == null)
                return null;

            // If the action is bound to a HapticControl, it will never perform and the activeControl stays null.
            // The action is instead just used to identify the output path in OpenXR, so we need to iterate the controls.
            // HapticControl is only defined in OpenXR.
            var activeControl = action.activeControl;
            if (activeControl == null)
            {
#if OPENXR_1_6_OR_NEWER
                // Note: By only using the first control, this prevents using an input action
                // to play to both controllers when {LeftHand}/{RightHand} usage is omitted.
                // For playing to both controllers, you would need to use a separate action for each controller,
                // and use two separate components for playing.
                var controls = action.controls;
                if (controls.Count > 0 && controls[0] is HapticControl hapticControl)
                {
                    m_OpenXRChannel.hapticAction = action;
                    m_OpenXRChannel.device = hapticControl.device;
                    return m_OpenXRChannelGroup;
                }
#endif

                return null;
            }

            m_DeviceChannelGroup.Initialize(activeControl.device);
            return m_DeviceChannelGroup;
        }
    }
}
