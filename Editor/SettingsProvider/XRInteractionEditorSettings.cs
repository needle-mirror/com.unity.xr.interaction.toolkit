using UnityEngine;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
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
        /// Returns the singleton settings instance or loads the settings asset if it exists.
        /// Unlike <see cref="EditorScriptableSettings{T}.Instance"/>, this method will not create the asset if it does not exist.
        /// </summary>
        /// <returns>A settings class derived from <see cref="ScriptableObject"/>, or <see langword="null"/>.</returns>
        internal static XRInteractionEditorSettings GetInstanceOrLoadOnly()
        {
            if (BaseInstance != null)
                return BaseInstance;

            // See CreateAndLoad() in base class.
            const string filter = "t:{0}";
            var settingsType = typeof(XRInteractionEditorSettings);
            foreach (var guid in AssetDatabase.FindAssets(string.Format(filter, settingsType.Name)))
            {
                BaseInstance = AssetDatabase.LoadAssetAtPath<XRInteractionEditorSettings>(AssetDatabase.GUIDToAssetPath(guid));
                if (BaseInstance != null)
                    break;
            }

            return BaseInstance;
        }

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
