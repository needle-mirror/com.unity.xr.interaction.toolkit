using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;
#if OPEN_XR_1_17_OR_NEWER
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Samples.XRMouse.Editor
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the XR Mouse Interaction Demo sample,
    /// checking that other required samples and packages are installed.
    /// </summary>
    static class XRMouseSampleProjectValidation
    {
        const string k_SampleDisplayName = "XR Mouse Interaction Demo";
        const string k_Category = "XR Interaction Toolkit";
        const string k_HandsInteractionDemoSampleName = "Hands Interaction Demo";
        const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
        const string k_OpenXRPackageName = "com.unity.xr.openxr";
        const string k_AndroidXRPackageName = "com.unity.xr.androidxr-openxr";
        const string k_AndroidXRPackageDisplayName = "Android XR (OpenXR)";
        const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        static readonly PackageVersion s_MinimumOpenXRPackageVersion = new PackageVersion("1.17.0-pre.2");

        static AddRequest s_OpenXRPackageAddRequest;
        static AddRequest s_AndroidXRPackageAddRequest;

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] {k_HandsInteractionDemoSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_HandsInteractionDemoSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion)}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category, k_HandsInteractionDemoSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion),
                FixIt = () =>
                {
                    if (TryFindSample(k_XRIPackageName, string.Empty, k_HandsInteractionDemoSampleName, out var sample))
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_Category, k_HandsInteractionDemoSampleName),
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => true,
                Message = $"[{k_SampleDisplayName}] Unity 6 (6000.0) or newer is required to use the XRMousePointer.",
                HelpText = "The XRMousePointer requires Unity 6 or later. To use this component, please upgrade to Unity 6 or newer.",
                Category = k_Category,
                CheckPredicate = () => IsUnity6OrNewer(),
                FixItAutomatic = false,
                Error = true,
            },
#if UNITY_6000_0_OR_NEWER
            new BuildValidationRule
            {
                IsRuleEnabled = () => (s_OpenXRPackageAddRequest == null || s_OpenXRPackageAddRequest.IsCompleted),
                Message = $"[{k_SampleDisplayName}] OpenXR Plugin ({k_OpenXRPackageName}) package must be at version {s_MinimumOpenXRPackageVersion} or higher to use the XRMousePointer.",
                HelpText = "The XRMousePointer requires OpenXR 1.17.0 or later for Android Mouse Interaction Profile support.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_OpenXRPackageName) >= s_MinimumOpenXRPackageVersion,
                FixIt = () =>
                {
                    if (s_OpenXRPackageAddRequest == null || s_OpenXRPackageAddRequest.IsCompleted)
                        ProjectValidationUtility.InstallOrUpdatePackage(k_OpenXRPackageName, s_MinimumOpenXRPackageVersion, ref s_OpenXRPackageAddRequest);
                },
                FixItAutomatic = true,
                Error = true,
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => (s_AndroidXRPackageAddRequest == null || s_AndroidXRPackageAddRequest.IsCompleted),
                Message = $"[{k_SampleDisplayName}] {k_AndroidXRPackageDisplayName} ({k_AndroidXRPackageName}) package must be installed to use the XRMousePointer.",
                HelpText = "The XRMousePointer requires the Android XR (OpenXR) package for Android XR device support.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_AndroidXRPackageName),
                FixIt = () =>
                {
                    if (s_AndroidXRPackageAddRequest == null || s_AndroidXRPackageAddRequest.IsCompleted)
                    {
                        s_AndroidXRPackageAddRequest = Client.Add(k_AndroidXRPackageName);
                        if (s_AndroidXRPackageAddRequest.Error != null)
                        {
                            Debug.LogError($"Package installation error: {s_AndroidXRPackageAddRequest.Error}: {s_AndroidXRPackageAddRequest.Error.message}");
                        }
                    }
                },
                FixItAutomatic = true,
                Error = true,
            },
#endif
        };

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
            {
                if (validation.CheckPredicate == null || (!validation.CheckPredicate.Invoke() && validation.Error))
                {
                    ShowWindow();
                    return;
                }
            }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () =>
            {
                SettingsService.OpenProjectSettings(k_ProjectValidationSettingsPath);
            };
        }

        static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName, out Sample sample)
        {
            sample = default;

            if (!PackageVersionUtility.IsPackageInstalled(packageName))
                return false;

            IEnumerable<Sample> packageSamples;
            try
            {
                packageSamples = Sample.FindByPackage(packageName, packageVersion);
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule. Exception: {e}");
                return false;
            }

            if (packageSamples == null)
            {
                Debug.LogWarning($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
                return false;
            }

            foreach (var packageSample in packageSamples)
            {
                if (packageSample.displayName == sampleDisplayName)
                {
                    sample = packageSample;
                    return true;
                }
            }

            Debug.LogWarning($"Couldn't find {sampleDisplayName} sample in the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
            return false;
        }

        static string ToString(string packageName, string packageVersion)
        {
            return string.IsNullOrEmpty(packageVersion) ? packageName : $"{packageName}@{packageVersion}";
        }

        static string GetImportSampleVersionMessage(string packageFolderName, string sampleDisplayName, PackageVersion version)
        {
            if (ProjectValidationUtility.SampleImportMeetsMinimumVersion(packageFolderName, sampleDisplayName, version) || !ProjectValidationUtility.HasSampleImported(packageFolderName, sampleDisplayName))
                return string.Empty;

            return $"An older version of {sampleDisplayName} has been found. This may cause errors.";
        }

        /// <summary>
        /// Checks if the current Unity Editor version is Unity 6 (6000.0) or newer.
        /// </summary>
        /// <returns>True if Unity 6 or newer is being used.</returns>
        static bool IsUnity6OrNewer()
        {
#if UNITY_6000_0_OR_NEWER
            return true;
#else
            return false;
#endif
        }
    }
}
