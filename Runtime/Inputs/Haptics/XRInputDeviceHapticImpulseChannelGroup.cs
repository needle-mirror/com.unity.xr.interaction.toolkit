using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
    /// <summary>
    /// A haptic impulse channel group that uses an XR input subsystem device to query the haptic capabilities of a device.
    /// </summary>
    public class XRInputDeviceHapticImpulseChannelGroup : IXRHapticImpulseChannelGroup
    {
        /// <inheritdoc />
        public int channelCount => m_Channels.Count;

        InputDevice m_Device;

        readonly List<IXRHapticImpulseChannel> m_Channels = new List<IXRHapticImpulseChannel>();

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
        public void Initialize(InputDevice device)
        {
            if (m_Device == device)
                return;

            m_Device = device;
            m_Channels.Clear();

            if (!device.isValid)
                return;

            if (!device.TryGetHapticCapabilities(out var capabilities))
            {
                Debug.LogWarning($"Failed to get haptic capabilities of {device}");
                return;
            }

            if (!capabilities.supportsImpulse)
            {
                Debug.LogWarning($"{device} does not support haptic impulse.");
                return;
            }

            var numChannels = (int)capabilities.numChannels;
            for (var index = 0; index < numChannels; ++index)
            {
                m_Channels.Add(new XRInputDeviceHapticImpulseChannel
                {
                    motorChannel = index,
                    device = device,
                });
            }
        }
    }
}
