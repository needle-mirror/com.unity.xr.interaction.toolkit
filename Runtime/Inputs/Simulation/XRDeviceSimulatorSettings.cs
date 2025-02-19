using Unity.XR.CoreUtils;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Configuration class for Interaction Simulator which
    /// stores settings related to automatic instantiation.
    /// </summary>
    [ScriptableSettingsPath(ProjectPath.k_XRInteractionSettingsFolder)]
    class XRDeviceSimulatorSettings : ScriptableSettings<XRDeviceSimulatorSettings>
    {
        /// <summary>
        /// Returns the singleton settings instance or loads the settings asset if it exists.
        /// Unlike <see cref="ScriptableSettings{T}.Instance"/>, this method will not create the asset if it does not exist.
        /// </summary>
        /// <returns>A settings class derived from <see cref="ScriptableObject"/>, or <see langword="null"/>.</returns>
        internal static XRDeviceSimulatorSettings GetInstanceOrLoadOnly()
        {
            if (BaseInstance != null)
                return BaseInstance;

            // See CreateAndLoad() in base class.
            Assert.IsTrue(HasCustomPath);
            BaseInstance = Resources.Load(GetFilePath(), typeof(XRDeviceSimulatorSettings)) as XRDeviceSimulatorSettings;

            return BaseInstance;
        }

        [SerializeField]
        bool m_AutomaticallyInstantiateSimulatorPrefab;

        /// <summary>
        /// Setting this value to <see langword="true"/> will tell the <see cref="XRInteractionSimulatorLoader"/> to look for and automatically
        /// add the <see cref="simulatorPrefab"/> to the current scene if it does not already exist.
        /// </summary>
        internal bool automaticallyInstantiateSimulatorPrefab
        {
            get => m_AutomaticallyInstantiateSimulatorPrefab;
            set => m_AutomaticallyInstantiateSimulatorPrefab = value;
        }

        [SerializeField]
        bool m_AutomaticallyInstantiateInEditorOnly = true;

        /// <summary>
        /// Enable to only automatically instantiate the <see cref="simulatorPrefab"/> if the application is running inside the Unity Editor,
        /// preventing it from automatically appearing in standalone builds. Disable to allow the simulator to be created in standalone builds.
        /// </summary>
        /// <remarks>
        /// Setting this value to <see langword="true"/> will limit the <see cref="XRInteractionSimulatorLoader"/> to
        /// only automatically instantiate the <see cref="simulatorPrefab"/> if the application is running inside the Unity Editor.
        /// This property is only used if <see cref="automaticallyInstantiateSimulatorPrefab"/> is enabled.
        /// </remarks>
        internal bool automaticallyInstantiateInEditorOnly
        {
            get => m_AutomaticallyInstantiateInEditorOnly;
            set => m_AutomaticallyInstantiateInEditorOnly = value;
        }

        [SerializeField]
        bool m_UseClassic;

        /// <summary>
        /// Enable this to automatically use the legacy <see cref="XRDeviceSimulator"/> prefab. Disable to return to the default behavior of automatically using of the <see cref="XRInteractionSimulator"/> prefab instead.
        /// </summary>
        internal bool useClassic
        {
            get => m_UseClassic;
            set => m_UseClassic = value;
        }

        [SerializeField]
        GameObject m_SimulatorPrefab;

        /// <summary>
        /// This is the prefab to instantiate when <see cref="automaticallyInstantiateSimulatorPrefab"/> is set to <see langword="true"/>.
        /// </summary>
        internal GameObject simulatorPrefab
        {
            get => m_SimulatorPrefab;
            set => m_SimulatorPrefab = value;
        }

    }
}
