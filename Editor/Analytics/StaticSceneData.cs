#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// Information about a particular scene included in a build.
    /// </summary>
    [Serializable]
    struct StaticSceneData
    {
        /// <summary>
        /// The index of the scene in the Scene List.
        /// </summary>
        [SerializeField]
        public int buildIndex;

        /// <summary>
        /// The GUID of the scene.
        /// </summary>
        [SerializeField]
        public string sceneGuid;

        /// <summary>
        /// The number of interactor components (<c>IXRInteractor</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int interactorsCount;

        /// <summary>
        /// The number of interactable components (<c>IXRInteractable</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int interactablesCount;

        /// <summary>
        /// The number of Locomotion Provider components (<c>LocomotionProvider</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int locomotionProvidersCount;

        /// <summary>
        /// The number of UI input module components (<c>BaseInputModule</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int uiInputModulesCount;

        /// <summary>
        /// The number of UI raycaster components (<c>BaseRaycaster</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int uiRaycastersCount;

        /// <summary>
        /// The number of XR Input Modality Manager components (<c>XRInputModalityManager</c>) in the scene.
        /// </summary>
        [SerializeField]
        public int modalityManagersCount;
    }
}

#endif
