#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// (Deprecated) This class instantiates the <see cref="XRDeviceSimulator"/> in the scene depending on
    /// project settings.
    /// </summary>
    [Obsolete("XRDeviceSimulatorLoader has been replaced by the XRInteractionSimulatorLoader. ", false)]
    public static class XRDeviceSimulatorLoader
    {
        /// <summary>
        /// (Deprecated) This method has been replaced with <see cref="XRInteractionSimulatorLoader.Initialize"/>.
        /// </summary>
        public static void Initialize()
        {
        }
    }
}
#endif
