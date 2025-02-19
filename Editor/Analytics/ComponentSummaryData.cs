#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Summary data for a list of components, with a particular base type (for example, <c>IXRInteractable</c>), across all scenes.
    /// The components are grouped by type and the count of each type.
    /// </summary>
    [Serializable]
    struct ComponentSummaryData
    {
        /// <summary>
        /// The total count of components of this base type across all scenes.
        /// This is the sum of <see cref="builtInCount"/>, <see cref="unityCount"/>, and <see cref="customCount"/>.
        /// </summary>
        [SerializeField]
        public int totalCount;

        /// <summary>
        /// The number of built-in XRI components of this base type across all scenes.
        /// </summary>
        [SerializeField]
        public int builtInCount;

        /// <summary>
        /// The number of components of this base type defined in other Unity packages across all scenes.
        /// </summary>
        [SerializeField]
        public int unityCount;

        /// <summary>
        /// The number of components of this base type defined in non-Unity packages or project scripts across all scenes.
        /// </summary>
        [SerializeField]
        public int customCount;

        /// <summary>
        /// The list of unique component types across all scenes for those that are a built-in XRI component.
        /// </summary>
        [SerializeField]
        public NameCountData[] builtInTypes;

        /// <summary>
        /// The list of unique component types across all scenes for those that are defined in other Unity packages.
        /// </summary>
        [SerializeField]
        public NameCountData[] unityTypes;

        /// <summary>
        /// The list of unique component types across all scenes for those that are defined in non-Unity packages or project scripts.
        /// </summary>
        [SerializeField]
        public NameCountData[] customTypes;
    }

    /// <summary>
    /// Single entry of a type name and count of that type.
    /// </summary>
    [Serializable]
    struct NameCountData
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
    }
}

#endif
