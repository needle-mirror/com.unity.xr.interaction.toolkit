using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Editor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// XR Interaction Toolkit project validation details.
    /// </summary>
    static class XRInteractionProjectValidation
    {
        const string k_TeleportLayerName = "Teleport";

        static readonly List<BuildValidationRule> s_BuiltinValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                Message = "XR Interaction Toolkit: Interaction Layer 31 is not set to 'Teleport'.",
                CheckPredicate = () => string.Equals(InteractionLayerSettings.instance.GetLayerNameAt(InteractionLayerSettings.layerSize - 1), k_TeleportLayerName, StringComparison.OrdinalIgnoreCase),
                FixIt = FixTeleportLayer,
                FixItMessage = "XR Interaction Toolkit samples reserve Interaction Layer 31 for teleportation locomotion. Set Interaction Layer 31 to 'Teleport' to prevent conflicts.",
                FixItAutomatic =  true,
                Error = false,
                HelpText = "Please note Interaction Layers are unique to the XR Interaction Toolkit and can be found in Edit > Project Settings > XR Interaction Toolkit",
            }
        };

        /// <summary>
        /// Try to automatically fix the interaction layer 31 to 'Teleport' if possible.
        /// Otherwise, display a Dialog window to offer the user the possibility to fix it directly.
        /// Finally, if the problem is still present, open the XR Interaction Toolkit project settings.
        /// </summary>
        static void FixTeleportLayer()
        {
            if (InteractionLayerSettings.instance.IsLayerEmpty(InteractionLayerSettings.layerSize - 1) || DisplayTeleportDialog())
                InteractionLayerSettings.instance.SetLayerNameAt(InteractionLayerSettings.layerSize - 1, k_TeleportLayerName);
            else
                SettingsService.OpenProjectSettings("Project/XR Interaction Toolkit");
        }

        static bool DisplayTeleportDialog()
        {
            return EditorUtility.DisplayDialog(
                "Fixing Teleport Interaction Layer",
                $"Interaction Layer 31 for teleportation locomotion is currently set to '{InteractionLayerSettings.instance.GetLayerNameAt(InteractionLayerSettings.layerSize - 1)}' instead of 'Teleport'",
                "Automatically Replace",
                "Cancel");
        }

        /// <summary>
        /// Gathers and evaluates validation issues and adds them to a list.
        /// </summary>
        /// <param name="issues">List of validation issues to populate. List is cleared before populating.</param>
        public static void GetCurrentValidationIssues(List<BuildValidationRule> issues)
        {
            issues.Clear();
            foreach (var validation in s_BuiltinValidationRules)
            {
                if (!validation.CheckPredicate?.Invoke() ?? false)
                {
                    issues.Add(validation);
                }
            }
        }

        /// <summary>
        /// Logs validation issues to console.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if there were any errors that should stop the build.</returns>
        /// <remarks>Only errors are logged to the console. Warnings are not.</remarks>
        internal static bool LogBuildValidationErrors()
        {
            var issues = new List<BuildValidationRule>();
            GetCurrentValidationIssues(issues);

            if (issues.Count == 0)
                return false;

            var anyErrors = false;
            foreach (var result in issues)
            {
                if (result.Error)
                    Debug.LogError(result.Message);

                anyErrors |= result.Error;
            }

            if (anyErrors)
            {
                Debug.LogError("Double click to fix XR Interaction project validation issues.");
            }

            return anyErrors;
        }
    }
}