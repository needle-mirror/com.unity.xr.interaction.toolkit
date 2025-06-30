#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using Assembly = System.Reflection.Assembly;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Analyzes the scenes during the build process to collect static data about the XRI components in the scenes.
    /// This is for getting components in the scenes grouped by type, and distinguishing between built-in XRI components,
    /// components defined in other Unity packages, and custom components.
    /// </summary>
    /// <remarks>
    /// There is no requirement that it has to be done during a build as this class could be used
    /// to analyze a scene passed in from the Editor when not playing, but we currently only use it during the build.
    /// </remarks>
    /// <seealso cref="UpdateEventPayload"/>
    class XRISceneAnalyzer
    {
        /// <summary>
        /// The interactors in the most recent scene processed.
        /// </summary>
        List<IXRInteractor> sceneInteractors { get; } = new List<IXRInteractor>();

        /// <summary>
        /// The interactables in the most recent scene processed.
        /// </summary>
        List<IXRInteractable> sceneInteractables { get; } = new List<IXRInteractable>();

        /// <summary>
        /// The locomotion providers in the most recent scene processed.
        /// </summary>
        List<LocomotionProvider> sceneLocomotionProviders { get; } = new List<LocomotionProvider>();

        /// <summary>
        /// The UI input modules in the most recent scene processed.
        /// </summary>
        List<BaseInputModule> sceneInputModules { get; } = new List<BaseInputModule>();

        /// <summary>
        /// The UI raycasters in the most recent scene processed.
        /// </summary>
        List<BaseRaycaster> sceneRaycasters { get; } = new List<BaseRaycaster>();

        /// <summary>
        /// The XR Input Modality Manager components in the most recent scene processed.
        /// </summary>
        List<XRInputModalityManager> modalityManagers { get; } = new List<XRInputModalityManager>();

        /// <summary>
        /// All interactors across all scenes in the build.
        /// </summary>
        List<IXRInteractor> allInteractors { get; } = new List<IXRInteractor>();

        /// <summary>
        /// All interactables across all scenes in the build.
        /// </summary>
        List<IXRInteractable> allInteractables { get; } = new List<IXRInteractable>();

        /// <summary>
        /// All interactables across all scenes in the build.
        /// </summary>
        List<LocomotionProvider> allLocomotionProviders { get; } = new List<LocomotionProvider>();

        /// <summary>
        /// All UI input modules across all scenes in the build.
        /// </summary>
        List<BaseInputModule> allInputModules { get; } = new List<BaseInputModule>();

        /// <summary>
        /// The UI raycasters across all scenes in the build.
        /// </summary>
        List<BaseRaycaster> allRaycasters { get; } = new List<BaseRaycaster>();

        /// <summary>
        /// The XR Input Modality Manager components across all scenes in the build.
        /// </summary>
        List<XRInputModalityManager> allModalityManagers { get; } = new List<XRInputModalityManager>();

        ModalityComponentData m_ModalityData = new ModalityComponentData();

        static List<GameObject> s_RootGameObjects = new List<GameObject>();

        static List<Assembly> s_XRIAssemblies;

        /// <summary>
        /// Reset the analyzer to its initial state.
        /// Call this after generating the analytics payload to avoid accumulating data across
        /// multiple builds.
        /// </summary>
        public void Reset()
        {
            sceneInteractors.Clear();
            sceneInteractables.Clear();
            sceneLocomotionProviders.Clear();
            sceneInputModules.Clear();
            sceneRaycasters.Clear();
            modalityManagers.Clear();
            allInteractors.Clear();
            allInteractables.Clear();
            allLocomotionProviders.Clear();
            allInputModules.Clear();
            allRaycasters.Clear();
            allModalityManagers.Clear();
            m_ModalityData = default;
        }

        /// <summary>
        /// Get all the relevant components in the scene.
        /// </summary>
        /// <param name="scene">The scene to process.</param>
        /// <returns>Returns a struct containing overview info about the scene.</returns>
        public StaticSceneData CaptureComponentsInScene(Scene scene)
        {
            sceneInteractors.Clear();
            sceneInteractables.Clear();
            sceneLocomotionProviders.Clear();
            sceneInputModules.Clear();
            sceneRaycasters.Clear();
            allInteractors.Clear();

            using (ListPool<IXRInteractor>.Get(out var scratchInteractors))
            using (ListPool<IXRInteractable>.Get(out var scratchInteractables))
            using (ListPool<LocomotionProvider>.Get(out var scratchLocomotionProviders))
            using (ListPool<BaseInputModule>.Get(out var scratchInputModules))
            using (ListPool<BaseRaycaster>.Get(out var scratchRaycasters))
            using (ListPool<XRInputModalityManager>.Get(out var scratchModalityManagers))
            {
                s_RootGameObjects.EnsureCapacity(scene.rootCount);
                scene.GetRootGameObjects(s_RootGameObjects);
                foreach (var go in s_RootGameObjects)
                {
                    go.GetComponentsInChildren(true, scratchInteractors);
                    go.GetComponentsInChildren(true, scratchInteractables);
                    go.GetComponentsInChildren(true, scratchLocomotionProviders);
                    go.GetComponentsInChildren(true, scratchInputModules);
                    go.GetComponentsInChildren(true, scratchRaycasters);
                    go.GetComponentsInChildren(true, scratchModalityManagers);
                    sceneInteractors.AddRange(scratchInteractors);
                    sceneInteractables.AddRange(scratchInteractables);
                    sceneLocomotionProviders.AddRange(scratchLocomotionProviders);
                    sceneInputModules.AddRange(scratchInputModules);
                    sceneRaycasters.AddRange(scratchRaycasters);
                    modalityManagers.AddRange(scratchModalityManagers);
                }

                allInteractors.AddRange(sceneInteractors);
                allInteractables.AddRange(sceneInteractables);
                allLocomotionProviders.AddRange(sceneLocomotionProviders);
                allInputModules.AddRange(sceneInputModules);
                allRaycasters.AddRange(sceneRaycasters);
                allModalityManagers.AddRange(modalityManagers);

                s_RootGameObjects.Clear();

                // Process scene data here that needs the scene to be loaded.
                m_ModalityData.componentExists |= modalityManagers.Count > 0;
                m_ModalityData.leftHandAssigned |= modalityManagers.Any(manager => manager.leftHand != null);
                m_ModalityData.rightHandAssigned |= modalityManagers.Any(manager => manager.rightHand != null);
                m_ModalityData.leftControllerAssigned |= modalityManagers.Any(manager => manager.leftController != null);
                m_ModalityData.rightControllerAssigned |= modalityManagers.Any(manager => manager.rightController != null);
            }

            return new StaticSceneData
            {
                buildIndex = scene.buildIndex,
                sceneGuid = AssetDatabase.GUIDFromAssetPath(scene.path).ToString(),
                interactorsCount = sceneInteractors.Count,
                interactablesCount = sceneInteractables.Count,
                locomotionProvidersCount = sceneLocomotionProviders.Count,
                uiInputModulesCount = sceneInputModules.Count,
                uiRaycastersCount = sceneRaycasters.Count,
                modalityManagersCount = modalityManagers.Count,
            };
        }

        /// <summary>
        /// Get the component summary data for a list of components, grouping the components by type and the count of each.
        /// </summary>
        /// <typeparam name="T">The base type of component, for example <see cref="IXRInteractable"/>.</typeparam>
        /// <param name="components">The list of components to analyze.</param>
        /// <returns>Returns a new component summary struct.</returns>
        public static ComponentSummaryData GetComponentSummaryData<T>(List<T> components)
        {
            CalculateTypeCounts(components,
                out var builtInCount, out var unityCount, out var customCount,
                out var builtInTypes, out var unityTypes, out var customTypes);

            return new ComponentSummaryData
            {
                totalCount = components.Count,
                builtInCount = builtInCount,
                unityCount = unityCount,
                customCount = customCount,
                builtInTypes = builtInTypes,
                unityTypes = unityTypes,
                customTypes = customTypes,
            };
        }

        static void CalculateTypeCounts<T>(List<T> components,
            out int builtInCount, out int unityCount, out int customCount,
            out NameCountData[] builtInTypes, out NameCountData[] unityTypes, out NameCountData[] customTypes)
        {
            builtInCount = 0;
            unityCount = 0;
            customCount = 0;

            using (ListPool<NameCountData>.Get(out var builtInTypesUnsorted))
            using (ListPool<NameCountData>.Get(out var unityTypesUnsorted))
            using (ListPool<NameCountData>.Get(out var customTypesUnsorted))
            using (ListPool<T>.Get(out var customDerivedComponents))
            {
                foreach (var group in components.GroupBy(i => i.GetType()))
                {
                    var typeCategory = XRIAnalyticsUtility.DetermineTypeCategory(group.Key);
                    var count = group.Count();

                    switch (typeCategory)
                    {
                        case XRIAnalyticsUtility.TypeCategory.BuiltIn:
                            builtInCount += count;
                            builtInTypesUnsorted.Add(new NameCountData { typeName = group.Key.Name, count = count, });
                            break;

                        case XRIAnalyticsUtility.TypeCategory.Unity:
                            unityCount += count;
                            unityTypesUnsorted.Add(new NameCountData { typeName = group.Key.FullName, count = count, });
                            break;

                        case XRIAnalyticsUtility.TypeCategory.Custom:
                            customDerivedComponents.AddRange(group);
                            break;
                    }
                }

                if (customDerivedComponents.Count > 0)
                {
                    // Find the Unity type these custom components are derived from.
                    // For those component types that just implement interfaces rather than derive from one of our built-in classes,
                    // fallback to the interface type.
                    foreach (var group in customDerivedComponents.GroupBy(i => XRIAnalyticsUtility.GetClosestUnityType(i.GetType()) ?? typeof(T)))
                    {
                        var count = group.Count();
                        customCount += count;
                        var typeName = XRIAnalyticsUtility.IsXRIRuntimeAssembly(group.Key) ? group.Key.Name : group.Key.FullName;
                        customTypesUnsorted.Add(new NameCountData { typeName = typeName, count = count, });
                    }
                }

                builtInTypes = builtInTypesUnsorted.OrderByDescending(data => data.count).ThenBy(data => data.typeName).ToArray();
                unityTypes = unityTypesUnsorted.OrderByDescending(data => data.count).ThenBy(data => data.typeName).ToArray();
                customTypes = customTypesUnsorted.OrderByDescending(data => data.count).ThenBy(data => data.typeName).ToArray();
            }
        }

        /// <summary>
        /// Update the analytics payload struct with the data from this analyzer.
        /// </summary>
        /// <param name="payload">The analytics payload to write into.</param>
        public void UpdateEventPayload(ref XRIBuildEvent.Payload payload)
        {
            payload.interactors = GetComponentSummaryData(allInteractors);
            payload.interactables = GetComponentSummaryData(allInteractables);
            payload.locomotionProviders = GetComponentSummaryData(allLocomotionProviders);
            payload.uiInputModules = GetComponentSummaryData(allInputModules);
            payload.uiRaycasters = GetComponentSummaryData(allRaycasters);
            payload.modalityManagers = GetComponentSummaryData(allModalityManagers);

            payload.modalityInfo = m_ModalityData;
        }
    }
}

#endif
