using UnityEngine;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Settings class for XR Interaction Toolkit editor values.
    /// </summary>
    [ScriptableSettingsPath(ProjectPath.k_XRInteractionSettingsFolder)]
    class XRInteractionEditorSettings : EditorScriptableSettings<XRInteractionEditorSettings>
    {
        /// <summary>
        /// Determines how the Inspector window displays <see cref="XRInputValueReader{TValue}"/> fields.
        /// </summary>
        /// <seealso cref="inputReaderPropertyDrawerMode"/>
        /// <seealso cref="XRInputValueReader{TValue}"/>
        public enum InputReaderPropertyDrawerMode
        {
            /// <summary>
            /// Display the property in a compact format, using a minimal number of lines.
            /// </summary>
            Compact,

            /// <summary>
            /// Display the effective input source underlying the property, using multiple lines.
            /// </summary>
            MultilineEffective,

            /// <summary>
            /// Display all the input sources underlying the property.
            /// </summary>
            MultilineAll,
        }

        [SerializeField]
        InputReaderPropertyDrawerMode m_InputReaderPropertyDrawerMode = InputReaderPropertyDrawerMode.Compact;

        /// <summary>
        /// Gets the setting for how the Inspector window displays <see cref="XRInputValueReader{TValue}"/> fields.
        /// </summary>
        internal InputReaderPropertyDrawerMode inputReaderPropertyDrawerMode
        {
            get => m_InputReaderPropertyDrawerMode;
            set => m_InputReaderPropertyDrawerMode = value;
        }
    }
}
