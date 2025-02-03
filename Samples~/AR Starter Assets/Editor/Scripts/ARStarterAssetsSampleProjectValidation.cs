using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;

#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Samples.ARStarterAssets.Editor
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the AR Starter Assets sample,
    /// checking that other required samples are installed.
    /// </summary>
    static class ARStarterAssetsSampleProjectValidation
    {
        const string k_SampleDisplayName = "AR Starter Assets";
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
        const string k_ARFPackageName = "com.unity.xr.arfoundation";
        const string k_ARFPackageMinVersionString = "4.2.8";
        const float k_TimeOutInSeconds = 3f;

#if UNITY_6000_0_OR_NEWER
        // The s_MinimumUIPackageVersion should match the UGUI_2_0_PRESENT version in the
        // Unity.XR.Interaction.Toolkit.Samples.StarterAssets.Editor.asmdef
        // and the Unity.XR.Interaction.Toolkit.Samples.StarterAssets.asmdef
        static readonly PackageVersion s_MinimumUIPackageVersion = new PackageVersion("2.0.0");
        const string k_UIPackageName = "com.unity.ugui";
        const string k_UIPackageDisplayName = "Unity UI";
#else
        // The s_MinimumUIPackageVersion should match the TEXT_MESH_PRO_PRESENT version in the
        // Unity.XR.Interaction.Toolkit.Samples.StarterAssets.Editor.asmdef
        // and the Unity.XR.Interaction.Toolkit.Samples.StarterAssets.asmdef
        static readonly PackageVersion s_MinimumUIPackageVersion = new PackageVersion("3.0.8");
        const string k_UIPackageName = "com.unity.textmeshpro";
        const string k_UIPackageDisplayName = "TextMeshPro";
#endif
        static AddRequest s_UIPackageAddRequest;

        static readonly PackageVersion s_ARFPackageMinVersion = new PackageVersion(k_ARFPackageMinVersionString);

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_ARFPackageAddRequest == null || s_ARFPackageAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] AR Foundation ({k_ARFPackageName}) package must be installed or updated to use this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_ARFPackageName) >= s_ARFPackageMinVersion,
                FixIt = () =>
                {
                    var packString = k_ARFPackageName;
                    var searchResult = Client.Search(k_ARFPackageName, true);
                    var timeout = Time.realtimeSinceStartup + k_TimeOutInSeconds;
                    while (!searchResult.IsCompleted && timeout > Time.realtimeSinceStartup)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    if (searchResult.IsCompleted)
                    {
                        var version = searchResult.Result
                            .Where((info) => string.Compare(k_ARFPackageName, info.name) == 0)
                            .Select(info => info.versions.recommended)
                            .FirstOrDefault();

                        if (!string.IsNullOrEmpty(version))
                        {
                            var verifiedVersion = new PackageVersion(version);
                            if (verifiedVersion >= s_ARFPackageMinVersion)
                            {
                                packString = k_ARFPackageName + "@" + version;
                            }
                            else
                            {
                                Debug.LogError($"Package installation error: {k_ARFPackageMinVersionString}@{version} is below the minimum version of {k_ARFPackageMinVersionString}. Please install manually from Package Manager or update to a newer version of the Unity Editor.");
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Timeout trying to get package list after {k_TimeOutInSeconds} seconds.");
                    }

                    s_ARFPackageAddRequest = Client.Add(packString);
                    if (s_ARFPackageAddRequest.Error != null)
                    {
                        Debug.LogError($"Package installation error: {s_ARFPackageAddRequest.Error}: {s_ARFPackageAddRequest.Error.message}");
                    }
                },
                FixItAutomatic = true,
                Error = true,
            },
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] {k_StarterAssetsSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion)}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion),
                FixIt = () =>
                {
                    if (TryFindSample(k_XRIPackageName, string.Empty, k_StarterAssetsSampleName, out var sample))
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_Category, k_StarterAssetsSampleName),
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_UIPackageAddRequest == null || s_UIPackageAddRequest.IsCompleted,
                Message = $"[{k_StarterAssetsSampleName}] {k_UIPackageDisplayName} ({k_UIPackageName}) package must be installed and at minimum version {s_MinimumUIPackageVersion}.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_UIPackageName) >= s_MinimumUIPackageVersion,
                FixIt = () =>
                {
                    if (s_UIPackageAddRequest == null || s_UIPackageAddRequest.IsCompleted)
                        ProjectValidationUtility.InstallOrUpdatePackage(k_UIPackageName, s_MinimumUIPackageVersion, ref s_UIPackageAddRequest);
                },
                FixItAutomatic = true,
                Error = true,
            },
#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
            new BuildValidationRule
            {
                IsRuleEnabled = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName),
                Message = $"[{k_SampleDisplayName}] TextMesh Pro - TMP Essentials must be installed for this sample.",
                HelpText = "Can be installed using Window > TextMeshPro > Import TMP Essential Resources or by clicking this Edit button and then Import TMP Essentials in the window that appears.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName) && TextMeshProEssentialsInstalled(),
                FixIt = () =>
                {
                    TMP_PackageResourceImporterWindow.ShowPackageImporterWindow();
                },
                FixItAutomatic = false,
                Error = true,
            },
#endif
        };

        static AddRequest s_ARFPackageAddRequest;

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }
        }

        static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName, out Sample sample)
        {
            sample = default;

            var packageSamples = Sample.FindByPackage(packageName, packageVersion);
            if (packageSamples == null)
            {
                Debug.LogError($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
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

            Debug.LogError($"Couldn't find {sampleDisplayName} sample in the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
            return false;
        }

#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
        static bool TextMeshProEssentialsInstalled()
        {
            // Matches logic in Project Settings window, see TMP_PackageResourceImporter.cs.
            // For simplicity, we don't also copy the check if the asset needs to be updated.
            return File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
        }
#endif

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
    }
}
