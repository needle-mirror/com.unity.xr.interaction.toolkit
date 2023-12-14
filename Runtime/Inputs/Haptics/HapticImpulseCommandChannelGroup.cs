using System.Collections.Generic;
using UnityEngine.InputSystem.XR.Haptics;

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
