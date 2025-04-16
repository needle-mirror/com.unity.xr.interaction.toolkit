#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Build processor that captures XR Interaction Toolkit analytics data during the build process.
    /// </summary>
    /// <seealso cref="XRIBuildEvent"/>
    class BuildProcessor : IPreprocessBuildWithReport, IProcessSceneWithReport, IPostprocessBuildWithReport
    {
        XRIBuildEvent.Payload? m_PayloadObject;
        BuildReport m_BuildReport;

        XRISceneAnalyzer m_SceneAnalyzer = new XRISceneAnalyzer();
        List<StaticSceneData> m_Scenes = new List<StaticSceneData>();

        bool m_AssetBundleDelayCallScheduled;

        /// <summary>
        /// Whether the scene(s) have been processed during the build.
        /// When a scene has already been built, the built may skip calling <see cref="OnProcessScene"/>.
        /// This would cause the scenes list and component type breakdown to appear to be empty even if the build list
        /// has scenes and contains XRI components. This flags whether this build processor actually had a chance to
        /// capture data about the scenes.
        /// </summary>
        bool m_ProcessSceneCalled;

        static readonly ProfilerMarker s_AnalyticsMarker = new ProfilerMarker("XRI.Analytics");

        /// <inheritdoc />
        public int callbackOrder => int.MaxValue;

        /// <inheritdoc />
        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorAnalytics.enabled)
                return;

            // This is mainly for ensuring a previous build is cleaned up.
            // We are unable to determine when an AssetBundle build is done in Unity versions before Unity 6
            // since that is not available in the BuildReport, so this ensures everything is reset.
            // Normally this is done at the final step after sending analytics.
            ResetFields();
        }

        /// <inheritdoc />
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (!EditorAnalytics.enabled)
                return;

            // When this callback is invoked for Scene loading during Editor play mode, the BuildReport is null.
            if (report == null)
                return;

            using (s_AnalyticsMarker.Auto())
            {
                m_ProcessSceneCalled = true;

                m_Scenes.Add(m_SceneAnalyzer.CaptureComponentsInScene(scene));

                // When an AssetBundle build is occuring, this OnProcessScene method is called multiple times, one for each scene.
                // Schedule a delayed call to the next Editor frame to ensure all scenes have finished building and the final BuildReport is available.
                // Note OnPostprocessBuild is not invoked during an AssetBundle build.
                // Note that this means that AssetBundle builds are not sent before Unity 6.
#if BUILD_TYPE_AVAILABLE
                if (report.summary.buildType == BuildType.AssetBundle && !m_AssetBundleDelayCallScheduled)
                {
                    m_AssetBundleDelayCallScheduled = true;

                    m_BuildReport = report;

                    // When -quit is specified on the command-line, the quitting event is used instead
                    // because the delayCall will not have a chance to be invoked.
                    if (!HasCommandLineQuit())
                        EditorApplication.delayCall += OnAssetBundleBuildDelayCall;
                    else
                        EditorApplication.quitting += OnAssetBundleBuildDelayCall;
                }
#endif
            }
        }

        /// <inheritdoc />
        public void OnPostprocessBuild(BuildReport report)
        {
            if (!EditorAnalytics.enabled)
                return;

            if (report == null)
                return;

            using (s_AnalyticsMarker.Auto())
            {
                m_PayloadObject = GetEventPayload(report, m_SceneAnalyzer, m_Scenes, m_ProcessSceneCalled);
                m_BuildReport = report;

                // Workaround for the totalTime in build report summary not being available during OnPostprocessBuild,
                // since the value is still 0 at this point. Delaying the call to the next Editor frame to ensure the build duration is available.
                // https://issuetracker.unity3d.com/issues/buildreports-summary-has-an-incorrect-totaltime-value-when-accessed-in-onpostprocessbuild-function
                // When -quit is specified on the command-line, the quitting event is used instead
                // because the delayCall will not have a chance to be invoked.
                if (!HasCommandLineQuit())
                    EditorApplication.delayCall += OnPlayerBuildDelayCall;
                else
                    EditorApplication.quitting += OnPlayerBuildDelayCall;
            }
        }

        void OnAssetBundleBuildDelayCall()
        {
            if (m_BuildReport == null)
                return;

            using (s_AnalyticsMarker.Auto())
            {
                m_PayloadObject = GetEventPayload(m_BuildReport, m_SceneAnalyzer, m_Scenes, m_ProcessSceneCalled);
                FinalizeAndSendPayload();
            }
        }

        void OnPlayerBuildDelayCall()
        {
            if (!m_PayloadObject.HasValue || m_BuildReport == null)
                return;

            using (s_AnalyticsMarker.Auto())
            {
                FinalizeAndSendPayload();
            }
        }

        void FinalizeAndSendPayload()
        {
            if (!m_PayloadObject.HasValue || m_BuildReport == null)
                return;

            var payload = m_PayloadObject.Value;
            var summary = m_BuildReport.summary;
            payload.buildStartTimeTicks = summary.buildStartedAt.Ticks;
            payload.buildEndTimeTicks = summary.buildEndedAt.Ticks;
            payload.buildDurationSeconds = (float)summary.totalTime.TotalSeconds;

            XRIAnalytics.Send(payload);

            ResetFields();
        }

        void ResetFields()
        {
            m_PayloadObject = null;
            m_BuildReport = null;
            m_SceneAnalyzer.Reset();
            m_Scenes.Clear();
            m_ProcessSceneCalled = false;
        }

        static XRIBuildEvent.Payload GetEventPayload(BuildReport report, XRISceneAnalyzer sceneAnalyzer, List<StaticSceneData> scenes, bool scenesProcessed)
        {
            var summary = report.summary;
            var managementData = XRIAnalyticsUtility.GetXRManagementDataBuild(summary.platformGroup);

            var payload = new XRIBuildEvent.Payload
            {
                buildGuid = summary.guid.ToString(),
#if BUILD_TYPE_AVAILABLE
                buildType = summary.buildType.ToString(),
#endif
                batchMode = Application.isBatchMode,
                activeBuildTarget = summary.platform.ToString(),
                activeBuildTargetGroup = summary.platformGroup.ToString(),
                initManagerOnStart = managementData.initManagerOnStart,
                activeLoaders = managementData.activeLoaders,
                packages = XRIAnalyticsUtility.GetPackageVersionData(),
                generalProjectSettings = XRIAnalyticsUtility.GetGeneralProjectSettingsData(),
                xriImportedSamples = XRIAnalyticsUtility.GetImportedXRISamplesData(),
                xriProjectSettings = XRIAnalyticsUtility.GetXRIProjectSettingsData(),
                oculusProjectSettings = managementData.isOculusEnabled ? XRIAnalyticsUtility.GetOculusSettingsData() : default,
                openXRProjectSettings = managementData.isOpenXREnabled ? XRIAnalyticsUtility.GetOpenXRSettingsData(summary.platformGroup) : default,
                scenesCount = scenes.Count,
                scenes = scenes.ToArray(),
                scenesProcessed = scenesProcessed,
            };

            sceneAnalyzer.UpdateEventPayload(ref payload);

            return payload;
        }

        static bool HasCommandLineQuit()
        {
            return Environment.GetCommandLineArgs().Any(arg => string.Equals(arg, "-quit", StringComparison.OrdinalIgnoreCase));
        }
    }
}

#endif
