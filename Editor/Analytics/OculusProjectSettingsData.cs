#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Oculus package project settings data for the analytics payload.
    /// </summary>
    [Serializable]
    struct OculusProjectSettingsData
    {
        /// <summary>
        /// Whether Oculus is enabled in the XR Plug-in Management and thus whether this data is valid to use.
        /// </summary>
        [SerializeField]
        public bool valid;

        /// <summary>
        /// The stereo rendering mode selected for desktop-based Oculus platforms.
        /// <c>Unity.XR.Oculus.OculusSettings.StereoRenderingModeDesktop</c> enum as an int.
        /// <para>0 = MultiPass</para>
        /// <para>1 = SinglePassInstanced</para>
        /// </summary>
        [SerializeField]
        public int renderModeDesktop;

        /// <summary>
        /// The stereo rendering mode selected for Android-based Oculus platforms.
        /// <c>Unity.XR.Oculus.OculusSettings.StereoRenderingModeAndroid</c> enum as an int.
        /// <para>0 = MultiPass</para>
        /// <para>2 = Multiview</para>
        /// </summary>
        [SerializeField]
        public int renderModeAndroid;

        /// <summary>
        /// The selected foveated rendering method used when foveation is enabled.
        /// <c>Unity.XR.Oculus.OculusSettings.FoveationMethod</c> enum as an int.
        /// <para>0 = FixedFoveatedRendering</para>
        /// <para>1 = EyeTrackedFoveatedRendering</para>
        /// <para>2 = FixedFoveatedRenderingUsingUnityAPIForURP</para>
        /// <para>3 = EyeTrackedFoveatedRenderingUsingUnityAPIForURP</para>
        /// <para>-1 = unable to obtain value since Oculus package version does not meet minimum version requirement for field</para>
        /// </summary>
        [SerializeField]
        public int foveationMethod;
    }
}

#endif
