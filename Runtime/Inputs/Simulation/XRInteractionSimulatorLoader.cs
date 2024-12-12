#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// This class instantiates the interaction simulator in the scene depending on
    /// project settings.
    /// </summary>
    /// <seealso cref="XRInteractionSimulator"/>
    [Preserve]
    public static class XRInteractionSimulatorLoader
    {
        /// <summary>
        /// See <see cref="RuntimeInitializeLoadType.AfterSceneLoad"/>.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad), Preserve]
        public static void Initialize()
        {
            // Will execute the static constructor as a side effect.
        }

        [Preserve]
        static XRInteractionSimulatorLoader()
        {
            if (!XRDeviceSimulatorSettings.Instance.automaticallyInstantiateSimulatorPrefab ||
                (XRDeviceSimulatorSettings.Instance.automaticallyInstantiateInEditorOnly && !Application.isEditor))
                return;

#if UNITY_INCLUDE_TESTS
            // For a consistent test environment, do not instantiate the simulator when running tests.
            // The simulator component will need to be explicitly added during a test if it is used for testing.
            // Additionally, as of Input System 1.4.4, the InputState.Change call in the component's Update causes
            // a NullReferenceException deep in the stack trace if running during tests.
            // The test runner will create a scene named "InitTestScene{DateTime.Now.Ticks}.unity".
            var scene = SceneManager.GetActiveScene();
            var isUnityTest = scene.IsValid() && scene.name.StartsWith("InitTestScene");
            if (isUnityTest)
            {
                Debug.Log("Skipping automatic instantiation of XR Interaction Simulator prefab since tests are running.");
                return;
            }
#endif

            if (XRInteractionSimulator.instance != null)
            {
                Object.DontDestroyOnLoad(XRInteractionSimulator.instance);
                return;
            }

            if (XRDeviceSimulator.instance != null)
            {
                Object.DontDestroyOnLoad(XRDeviceSimulator.instance);
                return;
            }

            var simulatorPrefab = XRDeviceSimulatorSettings.Instance.simulatorPrefab;
            if (simulatorPrefab == null)
            {
                Debug.LogWarning("XR Interaction Simulator prefab was missing, cannot automatically instantiate." +
                    " Open Window > Package Manager, select XR Interaction Toolkit, and Reimport the XR Device Simulator sample," +
                    " and then toggle the setting in Edit > Project Settings > XR Plug-in Management > XR Interaction Toolkit to try to resolve this issue.");
                return;
            }

            var simulatorInstance = Object.Instantiate(simulatorPrefab);

            // Strip off (Clone) from the name
            simulatorInstance.name = simulatorPrefab.name;

            Object.DontDestroyOnLoad(simulatorInstance);
        }
    }
}
#endif
