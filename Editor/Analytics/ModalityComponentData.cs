#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Information about the XR Input Modality Manager component for the build analytics payload.
    /// </summary>
    [Serializable]
    struct ModalityComponentData
    {
        /// <summary>
        /// Whether the component exists in any scene of the build.
        /// </summary>
        [SerializeField]
        public bool componentExists;

        /// <summary>
        /// Whether the component has the Left Hand GameObject assigned.
        /// </summary>
        [SerializeField]
        public bool leftHandAssigned;

        /// <summary>
        /// Whether the component has the Right Hand GameObject assigned.
        /// </summary>
        [SerializeField]
        public bool rightHandAssigned;

        /// <summary>
        /// Whether the component has the Left Controller GameObject assigned.
        /// </summary>
        [SerializeField]
        public bool leftControllerAssigned;

        /// <summary>
        /// Whether the component has the Right Controller GameObject assigned.
        /// </summary>
        [SerializeField]
        public bool rightControllerAssigned;
    }
}

#endif
