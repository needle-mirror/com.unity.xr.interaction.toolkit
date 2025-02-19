#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Single entry of an installed sample name and version.
    /// </summary>
    [Serializable]
    struct SampleVersionData
    {
        /// <summary>
        /// The name of the package, such as "Starter Assets".
        /// </summary>
        [SerializeField]
        public string sample;

        /// <summary>
        /// The package version of the sample that it originated from, such as "3.2.0".
        /// This may be older than the currently installed package version.
        /// </summary>
        [SerializeField]
        public string version;
    }
}

#endif
