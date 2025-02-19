#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// OpenXR package project settings data for the analytics payload.
    /// </summary>
    [Serializable]
    struct OpenXRProjectSettingsData
    {
        /// <summary>
        /// Whether OpenXR is enabled in the XR Plug-in Management and thus whether this data is valid to use.
        /// </summary>
        [SerializeField]
        public bool valid;

        /// <summary>
        /// The selected Render Mode.
        /// <c>UnityEngine.XR.OpenXR.OpenXRSettings.RenderMode</c> enum as an int.
        /// <para>0 = MultiPass</para>
        /// <para>1 = SinglePassInstanced</para>
        /// </summary>
        [SerializeField]
        public int renderMode;

        /// <summary>
        /// The number of Enabled Interaction Profiles that are defined by Unity.
        /// These are types derived from <c>OpenXRInteractionFeature</c>.
        /// </summary>
        [SerializeField]
        public int unityInteractionFeaturesCount;

        /// <summary>
        /// The number of enabled OpenXR features, excluding Interaction Profiles, that are defined by Unity.
        /// These are types derived from <c>OpenXRFeature</c>.
        /// </summary>
        [SerializeField]
        public int unityFeaturesCount;

        /// <summary>
        /// The number of Enabled Interaction Profiles that are custom, i.e. not defined by Unity.
        /// These are types derived from <c>OpenXRInteractionFeature</c>.
        /// </summary>
        [SerializeField]
        public int customInteractionFeaturesCount;

        /// <summary>
        /// The number of enabled OpenXR features, excluding Interaction Profiles, that are custom, i.e. not defined by Unity.
        /// These are types derived from <c>OpenXRFeature</c>.
        /// </summary>
        [SerializeField]
        public int customFeaturesCount;

        /// <summary>
        /// The string identifiers of Enabled Interaction Profiles that are defined by Unity.
        /// </summary>
        [SerializeField]
        public string[] unityInteractionFeatures;

        /// <summary>
        /// The string identifiers of enabled OpenXR features, excluding Interaction Profiles, that are defined by Unity.
        /// </summary>
        [SerializeField]
        public string[] unityFeatures;
    }
}

#endif
