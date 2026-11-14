#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// (Deprecated) This class instantiates the <see cref="XRDeviceSimulator"/> in the scene depending on
    /// project settings.
    /// </summary>
    [Obsolete(k_XRDeviceSimulatorLoaderDeprecated, true)]
    public static class XRDeviceSimulatorLoader
    {
        const string k_XRDeviceSimulatorLoaderDeprecated = "XRDeviceSimulatorLoader has been replaced by the XRInteractionSimulatorLoader. ";

        /// <summary>
        /// (Deprecated) This method has been replaced with <see cref="XRInteractionSimulatorLoader.Initialize"/>.
        /// </summary>
        public static void Initialize()
        {
            throw new NotSupportedException(k_XRDeviceSimulatorLoaderDeprecated);
        }
    }
}
#endif
