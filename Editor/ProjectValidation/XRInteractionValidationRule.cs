using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// Validation rule for the XR Interaction Toolkit projects. Pulled from OpenXRFeature.ValidationRule for future customization and extensibility. 
    /// </summary>
    internal class XRInteractionValidationRule
    {
        /// <summary>
        /// Message describing the rule that will be showed to the developer if it fails.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Lambda function that returns true if validation passes, false if validation fails.
        /// </summary>
        public Func<bool> checkPredicate { get; set; }

        /// <summary>
        /// Lambda function that fixes the issue, if possible.
        /// </summary>
        public Action fixIt { get; set; }

        /// <summary>
        /// Text describing how the issue is fixed, shown in a tooltip.
        /// </summary>
        public string fixItMessage { get; set; }

        /// <summary>
        /// True if the fixIt Lambda function performs a function that is automatic and does not require user input. If your fixIt
        /// function requires user input, set fixItAutomatic to false to prevent the fixIt method from being executed during fixAll.
        /// </summary>
        public bool fixItAutomatic { get; set; }

        /// <summary>
        /// If true, failing the rule is treated as an error and stops the build.
        /// If false, failing the rule is treated as a warning and it doesn't stop the build. The developer has the option to correct the problem, but is not required to.
        /// </summary>
        public bool error { get; set; }

        /// <summary>
        /// If true, will deny the project from entering playmode in editor.
        /// If false, can still enter playmode in editor if this issue isn't fixed.
        /// </summary>
        public bool errorEnteringPlaymode { get; set; }

        /// <summary>
        /// Optional text to display in a help icon with the issue in the validator.
        /// </summary>
        public string helpText { get; set; }

        /// <summary>
        /// Optional link that will be opened if the help icon is clicked.
        /// </summary>
        public string helpLink { get; set; }
        
        internal BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
    }
}
