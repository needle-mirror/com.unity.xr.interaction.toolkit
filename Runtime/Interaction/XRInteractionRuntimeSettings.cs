using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Configuration class for XR Interaction Toolkit runtime settings.
    /// </summary>
    [ScriptableSettingsPath(ProjectPath.k_XRInteractionSettingsFolder)]
    class XRInteractionRuntimeSettings : ScriptableSettings<XRInteractionRuntimeSettings>
    {
        /// <summary>
        /// Determines whether the manager component is automatically created.
        /// </summary>
        /// <seealso cref="managerCreationMode"/>
        public enum ManagerCreationMode
        {
            /// <summary>
            /// Create the manager component automatically as needed.
            /// This is the default mode.
            /// </summary>
            CreateAutomatically,

            /// <summary>
            /// Do not automatically create the manager component.
            /// The manager component must be manually added to the scene or manually instantiated at runtime for interaction
            /// to function.
            /// </summary>
            Manual,
        }

        /// <summary>
        /// Determines whether interaction components are automatically registered with a manager component
        /// when the manager reference is not set or the manager is destroyed.
        /// </summary>
        /// <seealso cref="managerRegistrationMode"/>
        public enum ManagerRegistrationMode
        {
            /// <summary>
            /// Will find the manager component and assign the manager component reference automatically at runtime and register with the manager component.
            /// Any registered interaction components to a manager being destroyed will automatically transfer to another manager,
            /// either while the original is destroyed if another manager is already active and enabled, or later when another manager is enabled.
            /// This is the default mode.
            /// </summary>
            FindAutomatically,

            /// <summary>
            /// Do not automatically find and register with the manager component when the manager reference is not set.
            /// The interaction manager reference must be set in the appropriate component or registered through scripting.
            /// Any registered interaction components to a manager being destroyed will automatically be added to the waitlist,
            /// but the other manager will not automatically register components from the waitlist and must be invoked through scripting
            /// to finish the transfer to another manager.
            /// </summary>
            Manual,
        }

        /// <summary>
        /// Internal access to the <see cref="ScriptableObject"/> instance of this configuration.
        /// This is intended to be used by unit tests to bypass the Unity project's settings.
        /// </summary>
        /// <seealso cref="ScriptableSettings{T}.Instance"/>
        internal static XRInteractionRuntimeSettings InstanceInternal
        {
            get => BaseInstance;
            set => BaseInstance = value;
        }

        /// <summary>
        /// Returns the singleton settings instance or loads the settings asset if it exists.
        /// Unlike <see cref="ScriptableSettings{T}.Instance"/>, this method will not create the asset if it does not exist.
        /// </summary>
        /// <returns>A settings class derived from <see cref="ScriptableObject"/>, or <see langword="null"/>.</returns>
        internal static XRInteractionRuntimeSettings GetInstanceOrLoadOnly()
        {
            if (BaseInstance != null)
                return BaseInstance;

            // See CreateAndLoad() in base class.
            Assert.IsTrue(HasCustomPath);
            BaseInstance = Resources.Load(GetFilePath(), typeof(XRInteractionRuntimeSettings)) as XRInteractionRuntimeSettings;

            return BaseInstance;
        }

        [SerializeField]
        ManagerCreationMode m_ManagerCreationMode;

        /// <summary>
        /// Gets the setting for controlling whether the manager component is automatically created.
        /// </summary>
        /// <seealso cref="ManagerCreationMode"/>
        public ManagerCreationMode managerCreationMode
        {
            get => m_ManagerCreationMode;
            set => m_ManagerCreationMode = value;
        }

        [SerializeField]
        ManagerRegistrationMode m_ManagerRegistrationMode;

        /// <summary>
        /// Gets the setting for controlling whether the manager component is automatically found.
        /// </summary>
        /// <seealso cref="ManagerRegistrationMode"/>
        public ManagerRegistrationMode managerRegistrationMode
        {
            get => m_ManagerRegistrationMode;
            set => m_ManagerRegistrationMode = value;
        }
    }
}
