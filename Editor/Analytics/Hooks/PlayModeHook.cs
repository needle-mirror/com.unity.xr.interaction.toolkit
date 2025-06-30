#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Entry point for static callbacks that captures XR Interaction Toolkit analytics data during a play mode session.
    /// </summary>
    /// <seealso cref="XRIPlayModeEvent"/>
    [InitializeOnLoad]
    static class PlayModeHook
    {
        static double s_EnteredPlayModeRealtime;
        static long s_EnteredPlayModeTicks;
        static XRIAnalyticsUtility.XRManagementData s_ManagementData;

        static SimulatorSessionTracker s_DeviceSimulatorSessionTracker = new SimulatorSessionTracker();
        static SimulatorSessionTracker s_InteractionSimulatorSessionTracker = new SimulatorSessionTracker();
        static InteractionManagerTracker s_InteractionManagerTracker = new InteractionManagerTracker();
        static InputModalityManagerTracker s_InputModalityManagerTracker = new InputModalityManagerTracker();
        static readonly LocomotionProviderTracker s_LocomotionProviderTracker = new LocomotionProviderTracker();

        static readonly ProfilerMarker s_AnalyticsMarker = new ProfilerMarker("XRI.Analytics");

        static PlayModeHook()
        {
            if (!EditorAnalytics.enabled)
                return;

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnDeviceSimulatorInstanceChanged(bool active)
        {
            var now = Time.realtimeSinceStartupAsDouble;

            using (s_AnalyticsMarker.Auto())
            {
                if (active)
                    s_DeviceSimulatorSessionTracker.StartSession(now);
                else
                    s_DeviceSimulatorSessionTracker.EndSession(now);
            }
        }

        static void OnInteractionSimulatorInstanceChanged(bool active)
        {
            var now = Time.realtimeSinceStartupAsDouble;

            using (s_AnalyticsMarker.Auto())
            {
                if (active)
                    s_InteractionSimulatorSessionTracker.StartSession(now);
                else
                    s_InteractionSimulatorSessionTracker.EndSession(now);
            }
        }

        static void OnActiveInteractionManagersChanged(XRInteractionManager manager, bool enabled)
        {
            var now = Time.realtimeSinceStartupAsDouble;

            using (s_AnalyticsMarker.Auto())
            {
                if (enabled)
                    s_InteractionManagerTracker.StartSession(manager, now);
                else
                    s_InteractionManagerTracker.EndSession(manager, now);
            }
        }

        static void OnActiveInputModalityManagersChanged(XRInputModalityManager manager, bool enabled)
        {
            var now = Time.realtimeSinceStartupAsDouble;

            using (s_AnalyticsMarker.Auto())
            {
                if (enabled)
                    s_InputModalityManagerTracker.StartSession(manager, now);
                else
                    s_InputModalityManagerTracker.EndSession(manager, now);
            }
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            var now = Time.realtimeSinceStartupAsDouble;
            var nowTicks = DateTime.Now.Ticks;

            // Skip analytics when running tests.
            // The test runner will create a scene named "InitTestScene{DateTime.Now.Ticks}.unity".
            var scene = SceneManager.GetActiveScene();
            var isUnityTest = scene.IsValid() && scene.name.StartsWith("InitTestScene");
            if (isUnityTest)
                return;

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                using (s_AnalyticsMarker.Auto())
                {
                    s_EnteredPlayModeRealtime = now;
                    s_EnteredPlayModeTicks = nowTicks;

                    if (XRDeviceSimulator.instance != null)
                        s_DeviceSimulatorSessionTracker.StartSession(now);

                    if (XRInteractionSimulator.instance != null)
                        s_InteractionSimulatorSessionTracker.StartSession(now);

                    foreach (var manager in XRInteractionManager.activeInteractionManagers)
                    {
                        s_InteractionManagerTracker.StartSession(manager, now);
                    }

                    foreach (var manager in XRInputModalityManager.activeModalityManagers)
                    {
                        s_InputModalityManagerTracker.StartSession(manager, now);
                    }

                    s_LocomotionProviderTracker.StartSession();

                    XRDeviceSimulator.instanceChanged += OnDeviceSimulatorInstanceChanged;
                    XRInteractionSimulator.instanceChanged += OnInteractionSimulatorInstanceChanged;
                    XRInteractionManager.activeInteractionManagersChanged += OnActiveInteractionManagersChanged;
                    XRInputModalityManager.activeModalityManagersChanged += OnActiveInputModalityManagersChanged;

                    // Query the XR Plug-in Management project settings.
                    // Do so before exiting play mode to ensure the active loader is captured
                    // since it's unloaded and cleared during that event.
                    s_ManagementData = XRIAnalyticsUtility.GetXRManagementDataPlayMode();
                    return;
                }
            }

            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                using (s_AnalyticsMarker.Auto())
                {
                    if (XRDeviceSimulator.instance != null)
                        s_DeviceSimulatorSessionTracker.EndSession(now);

                    if (XRInteractionSimulator.instance != null)
                        s_InteractionSimulatorSessionTracker.EndSession(now);

                    foreach (var manager in XRInteractionManager.activeInteractionManagers)
                    {
                        s_InteractionManagerTracker.EndSession(manager, now);
                    }

                    foreach (var manager in XRInputModalityManager.activeModalityManagers)
                    {
                        s_InputModalityManagerTracker.EndSession(manager, now);
                    }

                    s_LocomotionProviderTracker.EndSession();

                    XRDeviceSimulator.instanceChanged -= OnDeviceSimulatorInstanceChanged;
                    XRInteractionSimulator.instanceChanged -= OnInteractionSimulatorInstanceChanged;
                    XRInteractionManager.activeInteractionManagersChanged -= OnActiveInteractionManagersChanged;
                    XRInputModalityManager.activeModalityManagersChanged -= OnActiveInputModalityManagersChanged;

                    XRIAnalytics.Send(GetEventPayload(now, nowTicks));

                    s_DeviceSimulatorSessionTracker.Reset();
                    s_InteractionSimulatorSessionTracker.Reset();
                    s_InteractionManagerTracker.Cleanup();
                    s_InputModalityManagerTracker.Cleanup();
                    s_LocomotionProviderTracker.Reset();
                }
            }
        }

        static XRIPlayModeEvent.Payload GetEventPayload(double now, long nowTicks)
        {
            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var activeBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(activeBuildTarget);

            var payload = new XRIPlayModeEvent.Payload
            {
                activeBuildTarget = activeBuildTarget.ToString(),
                activeBuildTargetGroup = activeBuildTargetGroup.ToString(),
                initManagerOnStart = s_ManagementData.initManagerOnStart,
                activeLoader = s_ManagementData.activeLoader,
                activeLoaders = s_ManagementData.activeLoaders,
                packages = XRIAnalyticsUtility.GetPackageVersionData(),
                generalProjectSettings = XRIAnalyticsUtility.GetGeneralProjectSettingsData(),
                xriProjectSettings = XRIAnalyticsUtility.GetXRIProjectSettingsData(),
                oculusProjectSettings = s_ManagementData.isOculusEnabled ? XRIAnalyticsUtility.GetOculusSettingsData() : default,
                // Editor Play mode uses Desktop Platform Settings regardless of Active Build Target.
                openXRProjectSettings = s_ManagementData.isOpenXREnabled ? XRIAnalyticsUtility.GetOpenXRSettingsData(BuildTargetGroup.Standalone) : default,
                enterPlayModeSettings = EditorSettings.enterPlayModeOptionsEnabled ? EditorSettings.enterPlayModeOptions : EnterPlayModeOptions.None,
                playModeStartTimeTicks = s_EnteredPlayModeTicks,
                playModeEndTimeTicks = nowTicks,
                enteredPlayModeDurationSeconds = (float)s_EnteredPlayModeRealtime,
                playModeDurationSeconds = (float)(now - s_EnteredPlayModeRealtime),
                deviceSimulatorDurationSeconds = (float)s_DeviceSimulatorSessionTracker.sessionDuration,
                deviceSimulatorSessionCount = s_DeviceSimulatorSessionTracker.sessionCount,
                interactionSimulatorDurationSeconds = (float)s_InteractionSimulatorSessionTracker.sessionDuration,
                interactionSimulatorSessionCount = s_InteractionSimulatorSessionTracker.sessionCount,
            };

            s_InteractionManagerTracker.UpdateEventPayload(ref payload);
            s_InputModalityManagerTracker.UpdateEventPayload(ref payload);
            s_LocomotionProviderTracker.UpdateEventPayload(ref payload);

            return payload;
        }
    }
}

#endif
