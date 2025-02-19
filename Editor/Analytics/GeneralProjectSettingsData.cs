#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// General project settings that does not relate to XR specifically.
    /// </summary>
    [Serializable]
    struct GeneralProjectSettingsData
    {
        /// <summary>
        /// The Active Input Handling in Edit > Project Settings > Player.
        /// <para>0 = Input Manager (Old)</para>
        /// <para>1 = Input System Package (New)</para>
        /// <para>2 = Both</para>
        /// <para>-1 = unable to obtain value with reflection</para>
        /// </summary>
        [SerializeField]
        public int activeInputHandling;

        /// <summary>
        /// Whether the project is using a scriptable render pipeline, for example URP.
        /// A value of <see langword="false"/> indicates that the project is using the built-in render pipeline.
        /// </summary>
        [SerializeField]
        public bool hasRenderPipeline;
    }
}

#endif
