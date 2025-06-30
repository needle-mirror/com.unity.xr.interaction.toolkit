using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Contains locomotion-related data for play mode analytics payload.
    /// </summary>
    [Serializable]
    struct NameCountUsageData
    {
        /// <summary>
        /// Single entry of a type name, count of that type, and whether the type was used.
        /// </summary>
        [Serializable]
        public struct NameCountUsageEntry
        {
            /// <summary>
            /// The type name.
            /// </summary>
            [SerializeField]
            public string typeName;

            /// <summary>
            /// The number of components that are of this type.
            /// </summary>
            [SerializeField]
            public int count;

            /// <summary>
            /// Whether the component was used, such as if the locomotion provider started.
            /// </summary>
            [SerializeField]
            public bool wasUsed;
        }

        /// <summary>
        /// The list of unique component types for those that are a built-in XRI component.
        /// </summary>
        [SerializeField]
        public NameCountUsageEntry[] builtInTypes;

        /// <summary>
        /// The list of unique component types for those that are defined in other Unity packages.
        /// </summary>
        [SerializeField]
        public NameCountUsageEntry[] unityTypes;

        /// <summary>
        /// The list of unique component types for those that are defined in non-Unity packages or project scripts.
        /// </summary>
        [SerializeField]
        public NameCountUsageEntry[] customTypes;
    }
}
