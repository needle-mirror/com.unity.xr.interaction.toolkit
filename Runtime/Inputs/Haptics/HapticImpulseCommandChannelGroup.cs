// ENABLE_VR is not defined on Game Core but the assembly is available with limited features when the XR module is enabled.
// These are the guards that Input System uses to define the GetHapticCapabilitiesCommand class.
#if ENABLE_VR || UNITY_GAMECORE
#define INPUT_HAPTICS_AVAILABLE
#endif

using System.Collections.Generic;
#if INPUT_HAPTICS_AVAILABLE
using UnityEngine.InputSystem.XR.Haptics;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// A haptic impulse channel group that uses input system commands to query the haptic capabilities of a device.
    /// </summary>
    public class HapticImpulseCommandChannelGroup : IXRHapticImpulseChannelGroup
    {
        /// <inheritdoc />
        public int channelCount => m_Channels.Count;

        readonly List<IXRHapticImpulseChannel> m_Channels = new List<IXRHapticImpulseChannel>();

        InputSystem.InputDevice m_Device;

        /// <inheritdoc />
        public IXRHapticImpulseChannel GetChannel(int channel = 0)
        {
            if (channel < 0)
            {
                Debug.LogError("Haptic channel can't be negative.");
                return null;
            }

            return channel < m_Channels.Count ? m_Channels[channel] : null;
        }

        /// <summary>
        /// Initialize the channel group with the given device.
        /// Does nothing if already initialized with the same device.
        /// </summary>
        /// <param name="device">The input device that haptic impulses should be sent to.</param>
        public void Initialize(InputSystem.InputDevice device)
        {
            if (m_Device == device)
                return;

            m_Device = device;
            m_Channels.Clear();

            if (device == null)
                return;

#if INPUT_HAPTICS_AVAILABLE
            var command = GetHapticCapabilitiesCommand.Create();
            var result = device.ExecuteCommand(ref command);
            int numChannels;
            if (result < 0L)
            {
                Debug.LogWarning($"Failed to get haptic capabilities of {device}, error code {result}. Continuing assuming a single haptic channel.");
                numChannels = 1;
            }
            else
            {
                numChannels = (int)command.numChannels;
            }
#else
            Debug.LogWarning($"Unable to get haptic capabilities of {device} on platform {Application.platform}. Continuing assuming a single haptic channel.");
            const int numChannels = 1;
#endif

            for (var index = 0; index < numChannels; ++index)
            {
                m_Channels.Add(new HapticImpulseCommandChannel
                {
                    motorChannel = index,
                    device = device,
                });
            }
        }
    }
}
