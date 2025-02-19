#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Single entry of an installed package name and version.
    /// </summary>
    [Serializable]
    struct PackageVersionData
    {
        /// <summary>
        /// The name of the package, such as "com.unity.xr.openxr".
        /// </summary>
        [SerializeField]
        public string package;

        /// <summary>
        /// The version of the package, such as "1.12.1".
        /// </summary>
        [SerializeField]
        public string version;
    }
}

#endif
