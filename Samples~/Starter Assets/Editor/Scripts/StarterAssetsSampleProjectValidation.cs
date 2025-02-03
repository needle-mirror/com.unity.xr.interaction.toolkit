using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Samples
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the Starter Assets sample package.
    /// </summary>
    class StarterAssetsSampleProjectValidation
    {
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_TeleportLayerName = "Teleport";
        const int k_TeleportLayerIndex = 31;
        const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        const string k_ShaderGraphPackageName = "com.unity.shadergraph";
        const string k_InputSystemPackageName = "com.unity.inputsystem";
        static readonly PackageVersion s_RecommendedPackageVersion = new PackageVersion("1.11.0");
        const string k_InputActionAssetName = "XRI Default Input Actions";
        const string k_InputActionAssetGuid = "c348712bda248c246b8c49b3db54643f";

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>();

        static AddRequest s_ShaderGraphPackageAddRequest;
        static AddRequest s_InputSystemPackageAddRequest;
        static AddRequest s_UIPackageAddRequest;

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

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            // In the Player Settings UI we have to delay the call one frame to let the settings provider get initialized
            // since we need to access the settings asset to set the rule's non-delegate properties (FixItAutomatic).
            EditorApplication.delayCall += AddRulesAndRunCheck;
        }

        static void AddRulesAndRunCheck()
        {
            if (s_BuildValidationRules.Count == 0)
            {
                s_BuildValidationRules.Add(
                    new BuildValidationRule
                    {
                        Category = k_Category,
                        Message = $"[{k_StarterAssetsSampleName}] Interaction Layer {k_TeleportLayerIndex} should be set to '{k_TeleportLayerName}' for teleportation locomotion.",
                        FixItMessage = $"XR Interaction Toolkit samples reserve Interaction Layer {k_TeleportLayerIndex} for teleportation locomotion. Set Interaction Layer {k_TeleportLayerIndex} to '{k_TeleportLayerName}' to prevent conflicts.",
                        HelpText = "Please note Interaction Layers are unique to the XR Interaction Toolkit and can be found in Edit > Project Settings > XR Plug-in Management > XR Interaction Toolkit",
                        FixItAutomatic = InteractionLayerSettings.Instance.IsLayerEmpty(k_TeleportLayerIndex) || IsInteractionLayerTeleport(),
                        Error = false,
                        CheckPredicate = IsInteractionLayerTeleport,
                        FixIt = () =>
                        {
                            if (InteractionLayerSettings.Instance.IsLayerEmpty(k_TeleportLayerIndex) || DisplayTeleportDialog())
                                InteractionLayerSettings.Instance.SetLayerNameAt(k_TeleportLayerIndex, k_TeleportLayerName);
                            else
                                SettingsService.OpenProjectSettings(XRInteractionToolkitSettingsProvider.k_SettingsPath);
                        },
                    });

                s_BuildValidationRules.Add(
                    new BuildValidationRule
                    {
                        IsRuleEnabled = () => s_ShaderGraphPackageAddRequest == null || s_ShaderGraphPackageAddRequest.IsCompleted,
                        Message = $"[{k_StarterAssetsSampleName}] Shader Graph ({k_ShaderGraphPackageName}) package must be installed for materials used in this sample.",
                        Category = k_Category,
                        CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_ShaderGraphPackageName),
                        FixIt = () =>
                        {
                            s_ShaderGraphPackageAddRequest = Client.Add(k_ShaderGraphPackageName);
                            if (s_ShaderGraphPackageAddRequest.Error != null)
                            {
                                Debug.LogError($"Package installation error: {s_ShaderGraphPackageAddRequest.Error}: {s_ShaderGraphPackageAddRequest.Error.message}");
                            }
                        },
                        FixItAutomatic = true,
                        Error = false,
                    });

                s_BuildValidationRules.Add(
                    new BuildValidationRule
                    {
                        IsRuleEnabled = () => s_InputSystemPackageAddRequest == null || s_InputSystemPackageAddRequest.IsCompleted,
                        Message = $"[{k_StarterAssetsSampleName}] Input System ({k_InputSystemPackageName}) package must be at version {s_RecommendedPackageVersion} or higher to use Project-wide Actions with {k_InputActionAssetName}.",
                        Category = k_Category,
                        CheckPredicate = () => InputSystem.actions == null || PackageVersionUtility.GetPackageVersion(k_InputSystemPackageName) >= s_RecommendedPackageVersion,
                        FixIt = () =>
                        {
                            if (s_InputSystemPackageAddRequest == null || s_InputSystemPackageAddRequest.IsCompleted)
                                InstallOrUpdateInputSystem();
                        },
                        HelpText = "This version added support for automatic loading of custom extensions of InputProcessor, InputInteraction, and InputBindingComposite defined by this package.",
                        FixItAutomatic = true,
                        Error = InputSystem.actions != null && (InputSystem.actions.name == k_InputActionAssetName || AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(InputSystem.actions)) == k_InputActionAssetGuid),
                    });

                s_BuildValidationRules.Add(
                // Is appropriate UI package installed
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
                });

#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
                s_BuildValidationRules.Add(
                    new BuildValidationRule
                    {
                        IsRuleEnabled = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName),
                        Message = $"[{k_StarterAssetsSampleName}] TextMesh Pro - TMP Essentials must be installed for this sample.",
                        HelpText = "Can be installed using Window > TextMeshPro > Import TMP Essential Resources or by clicking this Edit button and then Import TMP Essentials in the window that appears.",
                        Category = k_Category,
                        CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName) && TextMeshProEssentialsInstalled(),
                        FixIt = () =>
                        {
                            TMP_PackageResourceImporterWindow.ShowPackageImporterWindow();
                        },
                        FixItAutomatic = false,
                        Error = true,
                });
#endif
            }

            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }

            ShowWindowIfIssuesExist();
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
            {
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
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

#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
        static bool TextMeshProEssentialsInstalled()
        {
            // Matches logic in Project Settings window, see TMP_PackageResourceImporter.cs.
            // For simplicity, we don't also copy the check if the asset needs to be updated.
            return File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
        }
#endif

        static bool IsInteractionLayerTeleport()
        {
            return string.Equals(InteractionLayerSettings.Instance.GetLayerNameAt(k_TeleportLayerIndex), k_TeleportLayerName, StringComparison.OrdinalIgnoreCase);
        }

        static bool DisplayTeleportDialog()
        {
            return EditorUtility.DisplayDialog(
                "Fixing Teleport Interaction Layer",
                $"Interaction Layer {k_TeleportLayerIndex} for teleportation locomotion is currently set to '{InteractionLayerSettings.Instance.GetLayerNameAt(k_TeleportLayerIndex)}' instead of '{k_TeleportLayerName}'",
                "Automatically Replace",
                "Cancel");
        }

        static void InstallOrUpdateInputSystem()
        {
            // Set a 3-second timeout for request to avoid editor lockup
            var currentTime = DateTime.Now;
            var endTime = currentTime + TimeSpan.FromSeconds(3);

            var request = Client.Search(k_InputSystemPackageName);
            if (request.Status == StatusCode.InProgress)
            {
                Debug.Log($"Searching for ({k_InputSystemPackageName}) in Unity Package Registry.");
                while (request.Status == StatusCode.InProgress && currentTime < endTime)
                    currentTime = DateTime.Now;
            }

            var addRequest = k_InputSystemPackageName;
            if (request.Status == StatusCode.Success && request.Result.Length > 0)
            {
                var versions = request.Result[0].versions;
                var recommendedVersion = new PackageVersion(versions.recommended);
                var latestCompatible = new PackageVersion(versions.latestCompatible);
                if (recommendedVersion < s_RecommendedPackageVersion && s_RecommendedPackageVersion <= latestCompatible)
                    addRequest = $"{k_InputSystemPackageName}@{s_RecommendedPackageVersion}";
            }

            s_InputSystemPackageAddRequest = Client.Add(addRequest);
            if (s_InputSystemPackageAddRequest.Error != null)
            {
                Debug.LogError($"Package installation error: {s_InputSystemPackageAddRequest.Error}: {s_InputSystemPackageAddRequest.Error.message}");
            }
        }
    }
}
