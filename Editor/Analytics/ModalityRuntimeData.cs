#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Information about the modality for either the left or right hand/controller in the XR Input Modality Manager component.
    /// Contains modality durations for the play mode analytics payload.
    /// </summary>
    [Serializable]
    struct ModalityRuntimeData
    {
        /// <summary>
        /// Whether the hand GameObject is assigned in the XR Input Modality Manager component.
        /// </summary>
        [SerializeField]
        public bool handAssigned;

        /// <summary>
        /// Whether the controller GameObject is assigned in the XR Input Modality Manager component.
        /// </summary>
        [SerializeField]
        public bool controllerAssigned;

        /// <summary>
        /// How long neither modality was active during play mode.
        /// </summary>
        [SerializeField]
        public float noneDurationSeconds;

        /// <summary>
        /// How long the tracked hand modality was active during play mode.
        /// </summary>
        [SerializeField]
        public float trackedHandDurationSeconds;

        /// <summary>
        /// How long the motion controller modality was active during play mode.
        /// </summary>
        [SerializeField]
        public float motionControllerDurationSeconds;
    }
}

#endif
