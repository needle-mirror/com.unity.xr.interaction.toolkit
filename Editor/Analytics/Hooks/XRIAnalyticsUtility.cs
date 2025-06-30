#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.Compilation;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Assembly = System.Reflection.Assembly;

#if XR_MANAGEMENT_4_0_OR_NEWER
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
#endif

#if OCULUS_3_0_OR_NEWER
using Unity.XR.Oculus;
#endif

#if OPENXR_1_6_OR_NEWER
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Utility class for collecting data for XRI analytics, such as Project Settings for XR Plug-in Management and OpenXR.
    /// Some data is included in both the build and play mode analytics payloads, so this class contains common code
    /// for getting data for either.
    /// </summary>
    static class XRIAnalyticsUtility
    {
        const string k_PackageDisplayName = "XR Interaction Toolkit";
        const string k_TeleportLayerName = "Teleport";
        const int k_TeleportLayerIndex = 31;

        /// <summary>
        /// Category a type belongs to based on the assembly: XRI built-in, Unity, or Custom.
        /// </summary>
        /// <seealso cref="DetermineTypeCategory"/>
        public enum TypeCategory
        {
            /// <summary>
            /// XRI built-in types.
            /// </summary>
            BuiltIn,

            /// <summary>
            /// Types from other Unity assemblies.
            /// </summary>
            Unity,

            /// <summary>
            /// Custom/third-party types.
            /// </summary>
            Custom,
        }

        static List<Assembly> s_XRIAssemblies;

        public struct XRManagementData
        {
            public bool initManagerOnStart;
            public string activeLoader;
            public string[] activeLoaders;
            public bool isOculusEnabled;
            public bool isOpenXREnabled;
        }

        static PackageVersionData[] s_PackageVersions;

        /// <summary>
        /// These are the list of relevant XR packages we want to report in the analytics payload.
        /// We gather the installed version of these packages to report in the payload.
        /// </summary>
        static readonly string[] s_PackagesToCheck =
        {
            // Unity XR packages
            "com.unity.xr.androidxr-openxr",
            "com.unity.xr.arfoundation",
            "com.unity.xr.hands",
            "com.unity.xr.management",
            "com.unity.xr.meta-openxr",
            "com.unity.xr.oculus",
            "com.unity.xr.openxr",
            "com.unity.xr.visionos",

            // Google packages
            "com.google.xr.extensions",

            // Meta packages
            "com.meta.xr.sdk.all",
            "com.meta.xr.mrutilitykit",
            "com.meta.xr.sdk.audio",
            "com.meta.xr.sdk.avatars",
            "com.meta.xr.sdk.core",
            "com.meta.xr.sdk.haptics",
            "com.meta.xr.sdk.interaction",
            "com.meta.xr.sdk.interaction.ovr",
            "com.meta.xr.sdk.platform",
            "com.meta.xr.sdk.simulator",
            "com.meta.xr.simulator",
            "com.meta.xr.sdk.voice",

            // Microsoft MRTK packages
            "com.microsoft.mixedreality.toolkit.foundation",
            "com.microsoft.mixedreality.toolkit.standardassets",
            "com.microsoft.mixedreality.toolkit.extensions",
            "com.microsoft.mixedreality.toolkit.tools",
            "com.microsoft.mixedreality.toolkit.handphysicsservice",
            "org.mixedrealitytoolkit.core",
            "org.mixedrealitytoolkit.accessibility",
            "org.mixedrealitytoolkit.audio",
            "org.mixedrealitytoolkit.environment",
            "org.mixedrealitytoolkit.input",
            "org.mixedrealitytoolkit.spatialmanipulation",
            "org.mixedrealitytoolkit.tools",
            "org.mixedrealitytoolkit.standardassets",
            "org.mixedrealitytoolkit.uxcore",
            "org.mixedrealitytoolkit.windowsspeech",
            "org.mixedrealitytoolkit.uxcomponents",
            "org.mixedrealitytoolkit.uxcomponents.noncanvas",
        };

        public static XRManagementData GetXRManagementDataPlayMode()
        {
#if XR_MANAGEMENT_4_0_OR_NEWER
            // Editor Play mode uses Desktop Platform Settings regardless of Active Build Target.
            var generalSettings = XRGeneralSettings.Instance;
            return GetXRManagementData(generalSettings);
#else
            return default;
#endif
        }

        public static XRManagementData GetXRManagementDataBuild()
        {
            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var activeBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(activeBuildTarget);
            return GetXRManagementDataBuild(activeBuildTargetGroup);
        }

        public static XRManagementData GetXRManagementDataBuild(BuildTargetGroup buildTargetGroup)
        {
#if XR_MANAGEMENT_4_0_OR_NEWER
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            return GetXRManagementData(generalSettings);
#else
            return default;
#endif
        }

#if XR_MANAGEMENT_4_0_OR_NEWER
        static XRManagementData GetXRManagementData(XRGeneralSettings generalSettings)
        {
            var data = new XRManagementData();

            if (generalSettings != null)
            {
                data.initManagerOnStart = generalSettings.InitManagerOnStart;

                var managerSettings = generalSettings.Manager;
                if (managerSettings != null)
                {
                    data.activeLoader = managerSettings.activeLoader != null ? managerSettings.activeLoader.GetType().Name : null;
                    data.activeLoaders = managerSettings.activeLoaders.Where(loader => loader != null).Select(loader => loader.GetType().Name).ToArray();

#if OCULUS_3_0_OR_NEWER
                    data.isOculusEnabled = managerSettings.activeLoaders.Where(loader => loader != null).Any(loader => loader is OculusLoader);
#endif

#if OPENXR_1_6_OR_NEWER
                    data.isOpenXREnabled = managerSettings.activeLoaders.Where(loader => loader != null).Any(loader => loader is OpenXRLoader);
#endif
                }
            }

            return data;
        }
#endif

        public static OculusProjectSettingsData GetOculusSettingsData()
        {
            var data = new OculusProjectSettingsData();

#if OCULUS_3_0_OR_NEWER
            var attr = typeof(OculusSettings).GetCustomAttribute<XRConfigurationDataAttribute>();
            var settingsKey = attr != null ? attr.buildSettingsKey : "Unity.XR.Oculus.Settings";

            if (EditorBuildSettings.TryGetConfigObject<OculusSettings>(settingsKey, out var oculusSettings))
            {
                data.valid = true;
                data.renderModeDesktop = (int)oculusSettings.m_StereoRenderingModeDesktop;
                data.renderModeAndroid = (int)oculusSettings.m_StereoRenderingModeAndroid;
#if OCULUS_3_2_OR_NEWER
                data.foveationMethod = (int)oculusSettings.FoveatedRenderingMethod;
#else
                data.foveationMethod = -1;
#endif
            }
#endif

            return data;
        }

        public static OpenXRProjectSettingsData GetOpenXRSettingsData(BuildTargetGroup buildTargetGroup)
        {
            var data = new OpenXRProjectSettingsData();

#if OPENXR_1_6_OR_NEWER
            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
            if (openXRSettings != null)
            {
                var unityInteractionFeatures = new List<string>();
                var unityFeatures = new List<string>();
                var customInteractionFeaturesCount = 0;
                var customFeaturesCount = 0;

                foreach (var feature in openXRSettings.GetFeatures())
                {
                    if (feature == null || !feature.enabled)
                        continue;

                    var type = feature.GetType();
                    var attr = type.GetCustomAttribute<OpenXRFeatureAttribute>();
                    if (attr == null)
                        continue;

                    var unityType = attr.Company == "Unity" || IsUnityAssembly(type);

                    if (feature is OpenXRInteractionFeature)
                    {
                        if (unityType)
                            unityInteractionFeatures.Add(attr.FeatureId);
                        else
                            customInteractionFeaturesCount++;
                    }
                    else
                    {
                        if (unityType)
                            unityFeatures.Add(attr.FeatureId);
                        else
                            customFeaturesCount++;
                    }
                }

                data.valid = true;
                data.renderMode = (int)openXRSettings.renderMode;
                data.unityInteractionFeaturesCount = unityInteractionFeatures.Count;
                data.unityFeaturesCount = unityFeatures.Count;
                data.customInteractionFeaturesCount = customInteractionFeaturesCount;
                data.customFeaturesCount = customFeaturesCount;
                data.unityInteractionFeatures = unityInteractionFeatures.OrderBy(id => id).ToArray();
                data.unityFeatures = unityFeatures.OrderBy(id => id).ToArray();
            }
#endif

            return data;
        }

        public static PackageVersionData[] GetPackageVersionData()
        {
            if (s_PackageVersions == null)
            {
                var foundPackages = new List<PackageVersionData>(s_PackagesToCheck.Length);

                foreach (var package in PackageManager.PackageInfo.GetAllRegisteredPackages())
                {
                    if (s_PackagesToCheck.Contains(package.name))
                        foundPackages.Add(new PackageVersionData { package = package.name, version = package.version });
                }

                s_PackageVersions = foundPackages.ToArray();
            }

            return s_PackageVersions;
        }

        public static SampleVersionData[] GetImportedXRISamplesData()
        {
            var samples = new List<(string sampleName, PackageVersion packageVersion)>();
            ProjectValidationUtility.GetImportedSamples(k_PackageDisplayName, samples);

            return samples.OrderBy(sample => sample.sampleName).Select(sample => new SampleVersionData { sample = sample.sampleName, version = sample.packageVersion.ToString(), }).ToArray();
        }

        public static GeneralProjectSettingsData GetGeneralProjectSettingsData()
        {
            return new GeneralProjectSettingsData
            {
                activeInputHandling = GetActiveInputHandlingMode() ?? -1,
                hasRenderPipeline = GraphicsSettings.currentRenderPipeline != null,
            };
        }

        public static XRIProjectSettingsData GetXRIProjectSettingsData()
        {
            var xriProjectSettings = new XRIProjectSettingsData();

            // The static Instance getter in these settings classes can cause assets to be created on disk,
            // so use GetInstanceOrLoadOnly to avoid creating the settings assets if they don't already exist.
            var simulatorSettings = XRDeviceSimulatorSettings.GetInstanceOrLoadOnly();
            if (simulatorSettings != null)
            {
                xriProjectSettings.automaticallyInstantiateSimulatorPrefab = simulatorSettings.automaticallyInstantiateSimulatorPrefab;
                xriProjectSettings.automaticallyInstantiateInEditorOnly = simulatorSettings.automaticallyInstantiateInEditorOnly;
                xriProjectSettings.hasSimulatorPrefab = simulatorSettings.simulatorPrefab != null;
            }

            var interactionEditorSettings = XRInteractionEditorSettings.GetInstanceOrLoadOnly();
            if (interactionEditorSettings != null)
            {
                xriProjectSettings.inputReaderPropertyDrawerMode = (int)interactionEditorSettings.inputReaderPropertyDrawerMode;
            }

            var interactionLayerSettings = InteractionLayerSettings.GetInstanceOrLoadOnly();
            if (interactionLayerSettings != null)
            {
                var userLayersCount = 0;
                for (var i = InteractionLayerSettings.builtInLayerSize; i < InteractionLayerSettings.layerSize; ++i)
                {
                    if (!interactionLayerSettings.IsLayerEmpty(i))
                        userLayersCount++;
                }

                xriProjectSettings.userLayersCount = userLayersCount;
                xriProjectSettings.hasTeleportLayer31 = string.Equals(interactionLayerSettings.GetLayerNameAt(k_TeleportLayerIndex), k_TeleportLayerName, StringComparison.OrdinalIgnoreCase);
            }

            return xriProjectSettings;
        }

        static int? GetActiveInputHandlingMode()
        {
            var playerSettings = (SerializedObject)typeof(PlayerSettings).GetMethod("GetSerializedObject", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
            var activeInputHandlerProp = playerSettings?.FindProperty("activeInputHandler");
            return activeInputHandlerProp?.intValue;
        }

        /// <summary>
        /// Determines which category a type belongs to based on the assembly: XRI built-in, Unity, or Custom.
        /// </summary>
        /// <param name="type">The type to categorize.</param>
        /// <returns>Returns the type category.</returns>
        public static TypeCategory DetermineTypeCategory(Type type)
        {
            if (IsXRIRuntimeAssembly(type))
                return TypeCategory.BuiltIn;

            if (IsUnityAssembly(type))
                return TypeCategory.Unity;

            return TypeCategory.Custom;
        }

        /// <summary>
        /// Determine if the type is from one of the XR Interaction Toolkit runtime assemblies, including samples.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns <see langword="true"/> if the type is defined in a XRI runtime assembly, including samples.</returns>
        public static bool IsXRIRuntimeAssembly(Type type)
        {
            if (s_XRIAssemblies == null || s_XRIAssemblies.Count == 0)
            {
                s_XRIAssemblies ??= new List<Assembly>();
                foreach (var assembly in CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies))
                {
                    if (assembly.name.StartsWith("Unity.XR.Interaction.Toolkit"))
                    {
                        try
                        {
                            var reflectionAssembly = Assembly.LoadFrom(assembly.outputPath);
                            s_XRIAssemblies.Add(reflectionAssembly);
                        }
                        catch
                        {
                            // Ignore any exceptions loading the assembly
                        }
                    }
                }

                // Fallback in case the assembly loading code above fails
                // to at least ensure the core Unity.XR.Interaction.Toolkit assembly is included
                if (s_XRIAssemblies.Count == 0)
                    s_XRIAssemblies.Add(typeof(IXRInteractable).Assembly);
            }

            return s_XRIAssemblies.Any(a => type.Assembly == a);
        }

        /// <summary>
        /// Finds and returns the closest Unity-derived type for a given type,
        /// or the type itself it's in a Unity assembly.
        /// </summary>
        /// <param name="type">The type to find the Unity type for.</param>
        /// <returns>Returns either the type if it's a Unity assembly type or the closest Unity assembly base type.
        /// May return <see langword="null"/> if there isn't a Unity type in the class hierarchy.</returns>
        public static Type GetClosestUnityType(Type type)
        {
            while (type != null && !IsUnityAssembly(type))
            {
                type = type.BaseType;
            }

            return type;
        }

        public static bool IsUnityAssembly(Type type)
        {
            var assemblyName = type.Assembly.GetName().Name;
            return assemblyName.StartsWith("Unity.") || assemblyName.StartsWith("UnityEngine.");
        }
    }
}

#endif
