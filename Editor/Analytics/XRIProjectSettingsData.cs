#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// XRI package project settings data for the analytics payload.
    /// </summary>
    [Serializable]
    struct XRIProjectSettingsData
    {
        /// <summary>
        /// Determines how the Inspector window displays input reader properties.
        /// <c>UnityEditor.XR.Interaction.Toolkit.XRInteractionEditorSettings.InputReaderPropertyDrawerMode</c> enum as an int.
        /// <para>0 = Compact</para>
        /// <para>1 = MultilineEffective</para>
        /// <para>2 = MultilineAll</para>
        /// </summary>
        [SerializeField]
        public int inputReaderPropertyDrawerMode;

        /// <summary>
        /// Whether the simulator prefab is automatically instantiated.
        /// </summary>
        [SerializeField]
        public bool automaticallyInstantiateSimulatorPrefab;

        /// <summary>
        /// Whether the simulator prefab is only automatically instantiated in the Unity Editor
        /// or whether it can also be instantiated in standalone builds.
        /// </summary>
        [SerializeField]
        public bool automaticallyInstantiateInEditorOnly;

        /// <summary>
        /// Whether to use the classic/legacy XRDeviceSimulator prefab or the new XRInteractionSimulator prefab introduced with XRI 3.1.
        /// </summary>
        [SerializeField]
        public bool useClassic;

        /// <summary>
        /// Whether the simulator prefab is assigned.
        /// </summary>
        [SerializeField]
        public bool hasSimulatorPrefab;

        /// <summary>
        /// The number of custom interaction layers defined in the project.
        /// </summary>
        [SerializeField]
        public int userLayersCount;

        /// <summary>
        /// Whether the interaction layer User Layer 31 is Teleport, which is recommended by the Starter Assets sample.
        /// </summary>
        [SerializeField]
        public bool hasTeleportLayer31;
    }
}

#endif
