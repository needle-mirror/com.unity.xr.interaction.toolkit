using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// XR Interaction Toolkit project validation details.
    /// </summary>
    internal static class XRInteractionProjectValidation
    {
        static readonly XRInteractionValidationRule[] BuiltinValidationRules =
        {
            new XRInteractionValidationRule
            {
                message = "Interaction Layer 31 is not set to 'Teleport'.",
                checkPredicate = () => string.Equals(InteractionLayerSettings.instance.GetLayerNameAt(InteractionLayerSettings.k_LayerSize - 1), "Teleport", StringComparison.OrdinalIgnoreCase),
                fixIt = OpenProjectSettings,
                fixItMessage = "XR Interaction Toolkit samples reserve Interaction Layer 31 for teleportation locomotion. Set Interaction Layer 31 to 'Teleport' to prevent conflicts.",
                fixItAutomatic =  false,
                error = false,
                errorEnteringPlaymode = false,
                helpText = "Please note Interaction Layers are unique to the XR Interaction Toolkit and can be found in Edit > Project Settings > XR Interaction Toolkit",
            }
        };

        static readonly List<XRInteractionValidationRule> CachedValidationList = new List<XRInteractionValidationRule>(BuiltinValidationRules.Length);

        /// <summary>
        /// Open the XR Interaction Toolkit project settings
        /// </summary>
        internal static void OpenProjectSettings() => SettingsService.OpenProjectSettings("Project/XR Interaction Toolkit");

        /// <summary>
        /// Gathers all validation issues regardless if they are violated.
        /// </summary>
        /// <param name="issues">List of validation issues to populate. List is cleared before populating.</param>
        /// <param name="buildTargetGroup">Build target group to check for validation issues</param>
        internal static void GetAllValidationIssues(List<XRInteractionValidationRule> issues, BuildTargetGroup buildTargetGroup)
        {
            issues.Clear();
            issues.AddRange(BuiltinValidationRules.Where(s => s.buildTargetGroup == buildTargetGroup || s.buildTargetGroup == BuildTargetGroup.Unknown));
        }

        /// <summary>
        /// Gathers and evaluates validation issues and adds them to a list.
        /// </summary>
        /// <param name="issues">List of validation issues to populate. List is cleared before populating.</param>
        /// <param name="buildTargetGroup">Build target group to check for validation issues</param>
        public static void GetCurrentValidationIssues(List<XRInteractionValidationRule> issues, BuildTargetGroup buildTargetGroup)
        {
            CachedValidationList.Clear();
            CachedValidationList.AddRange(BuiltinValidationRules.Where(s => s.buildTargetGroup == buildTargetGroup || s.buildTargetGroup == BuildTargetGroup.Unknown));
            
            issues.Clear();
            foreach (var validation in CachedValidationList)
            {
                if (!validation.checkPredicate?.Invoke() ?? false)
                {
                    issues.Add(validation);
                }
            }
        }

        /// <summary>
        /// Logs validation issues to console.
        /// </summary>
        /// <returns>True if there were any errors that should stop the build.</returns>
        internal static bool LogBuildValidationIssues(BuildTargetGroup targetGroup)
        {
            var issues = new List<XRInteractionValidationRule>();
            GetCurrentValidationIssues(issues, targetGroup);

            if (issues.Count <= 0) return false;

            var anyErrors = false;
            foreach (var result in issues)
            {
                if (result.error)
                    Debug.LogError(result.message);
                else
                    Debug.LogWarning(result.message);
                anyErrors |= result.error;
            }

            if (anyErrors)
            {
                Debug.LogError("Double click to fix XR Interaction project validation issues.");
            }

            return anyErrors;
        }

        /// <summary>
        /// Logs playmode validation issues (any rule that fails with errorEnteringPlaymode set to true).
        /// </summary>
        /// <returns>True if there were any errors that should prevent the project from starting in editor playmode.</returns>
        internal static bool LogPlaymodeValidationIssues()
        {
            var issues = new List<XRInteractionValidationRule>();
            GetCurrentValidationIssues(issues, BuildTargetGroup.Standalone);

            if (issues.Count <= 0) return false;

            var playmodeErrors = false;
            foreach (var result in issues)
            {
                if (result.errorEnteringPlaymode || result.error)
                    Debug.LogError(result.message);
                else
                    Debug.LogWarning(result.message);

                playmodeErrors |= result.errorEnteringPlaymode;
            }

            return playmodeErrors;
        }
    }
}
