using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
    /// <summary>
    /// Serializable class that wraps a string usage name for use with <see cref="InputFeatureUsage{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value the usage represents, such as <see cref="Vector2"/> or <see langword="float"/>.</typeparam>
    /// <seealso cref="CommonUsages"/>
    [Serializable]
    public sealed class InputFeatureUsageString<T> where T : struct
    {
        [SerializeField]
        string m_Name;

        /// <summary>
        /// The string name of the usage feature, which maps to an input feature on a device.
        /// </summary>
        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="InputFeatureUsageString{T}"/>.
        /// </summary>
        public InputFeatureUsageString()
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="InputFeatureUsageString{T}"/>.
        /// </summary>
        /// <param name="usageName">The string name of the usage feature, which maps to an input feature on a device.</param>
        public InputFeatureUsageString(string usageName)
        {
            m_Name = usageName;
        }
    }
}
