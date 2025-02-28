#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// The analytics event for a build.
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [AnalyticInfo(k_EventName, XRIAnalytics.VendorKey, k_EventVersion, k_MaxEventsPerHour, k_MaxItems)]
#endif
    class XRIBuildEvent : XRIBaseAnalyticsEvent<XRIBuildEvent.Payload>
    {
        const string k_EventName = "xrinteractiontoolkit_build";
        const int k_EventVersion = 2;
        const int k_MaxEventsPerHour = XRIAnalytics.DefaultMaxEventsPerHour;
        const int k_MaxItems = XRIAnalytics.DefaultMaxItems;

        /// <summary>
        /// The analytics payload for a build.
        /// </summary>
        [Serializable]
        public struct Payload
#if UNITY_2023_2_OR_NEWER
            : IAnalytic.IData
#endif
        {
            // Do not rename any field, the field names are used to identify the table/event column of this event payload.

            /// <summary>
            /// The GUID of the build.
            /// </summary>
            [SerializeField]
            public string buildGuid;

            /// <summary>
            /// The type of the build when this information is available, either "Player" or "AssetBundle" (or an empty string if unavailable).
            /// <c>UnityEditor.Build.Reporting.BuildType</c> enum as a string.
            /// Enum type only available in Unity 6 or newer, so this field is left blank in earlier versions.
            /// </summary>
            [SerializeField]
            public string buildType;

            /// <summary>
            /// Whether Unity was launched with the -batchmode command line argument.
            /// </summary>
            [SerializeField]
            public bool batchMode;

            /// <summary>
            /// The currently active build target.
            /// <c>UnityEditor.BuildTarget</c> enum as a string.
            /// </summary>
            [SerializeField]
            public string activeBuildTarget;

            /// <summary>
            /// The currently active build target group.
            /// </summary>
            [SerializeField]
            public string activeBuildTargetGroup;

            /// <summary>
            /// Whether project settings has the automatic startup of XR at runtime enabled.
            /// This is the checkbox for Initialize XR on Startup in Edit > Project Settings > XR Plug-in Management.
            /// </summary>
            [SerializeField]
            public bool initManagerOnStart;

            /// <summary>
            /// The list of enabled XR Plug-in Management plug-in providers (<c>UnityEngine.XR.Management.XRLoader</c>).
            /// This is the list of checkboxes in Edit > Project Settings > XR Plug-in Management.
            /// </summary>
            [SerializeField]
            public string[] activeLoaders;

            /// <summary>
            /// The list of relevant XR packages that are installed in the project.
            /// </summary>
            [SerializeField]
            public PackageVersionData[] packages;

            /// <summary>
            /// The list of imported XRI package samples and their originating package versions.
            /// </summary>
            [SerializeField]
            public SampleVersionData[] xriImportedSamples;

            /// <summary>
            /// General project settings that does not relate to XR specifically.
            /// </summary>
            [SerializeField]
            public GeneralProjectSettingsData generalProjectSettings;

            /// <summary>
            /// XRI package project settings data.
            /// </summary>
            [SerializeField]
            public XRIProjectSettingsData xriProjectSettings;

            /// <summary>
            /// Oculus package project settings data. Only valid if Oculus is enabled in the XR Plug-in Management project settings.
            /// </summary>
            [SerializeField]
            public OculusProjectSettingsData oculusProjectSettings;

            /// <summary>
            /// OpenXR package project settings data. Only valid if OpenXR is enabled in the XR Plug-in Management project settings.
            /// </summary>
            [SerializeField]
            public OpenXRProjectSettingsData openXRProjectSettings;

            /// <summary>
            /// Summary data for the interactor components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData interactors;

            /// <summary>
            /// Summary data for the interactable components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData interactables;

            /// <summary>
            /// Summary data for the Locomotion Provider components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData locomotionProviders;

            /// <summary>
            /// Summary data for the UI input module components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData uiInputModules;

            /// <summary>
            /// Summary data for the UI raycaster components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData uiRaycasters;

            /// <summary>
            /// Summary data for the XR Input Modality Manager components across all scenes in the build.
            /// </summary>
            [SerializeField]
            public ComponentSummaryData modalityManagers;

            /// <summary>
            /// Information about the XR Input Modality Manager component.
            /// </summary>
            [SerializeField]
            public ModalityComponentData modalityInfo;

            /// <summary>
            /// The number of scenes processed in the build.
            /// This may be 0 if the build has already completed previously and the assets have not changed.
            /// </summary>
            [SerializeField]
            public int scenesCount;

            /// <summary>
            /// Information about all the processed scenes in the build.
            /// </summary>
            [SerializeField]
            public StaticSceneData[] scenes;

            /// <summary>
            /// Whether the scenes were actually processed during the build.
            /// This is <see langword="false"/> if <c>OnProcessScene</c> was skipped due to the build already
            /// being completed previously and the assets have not changed.
            /// </summary>
            [SerializeField]
            public bool scenesProcessed;

            /// <summary>
            /// The time the build was started.
            /// This is <c>DateTime.Ticks</c> as reported in the <c>UnityEditor.Build.Reporting.BuildSummary</c>.
            /// </summary>
            [SerializeField]
            public long buildStartTimeTicks;

            /// <summary>
            /// The time the build ended.
            /// This is <c>DateTime.Ticks</c> as reported in the <c>UnityEditor.Build.Reporting.BuildSummary</c>.
            /// </summary>
            [SerializeField]
            public long buildEndTimeTicks;

            /// <summary>
            /// The total time taken by the build process in seconds.
            /// This is the duration as reported in the <c>UnityEditor.Build.Reporting.BuildSummary</c>.
            /// </summary>
            [SerializeField]
            public float buildDurationSeconds;

#if UNITY_2023_2_OR_NEWER
            /// <summary>
            /// The package name of the XR Interaction Toolkit, i.e. com.unity.xr.interaction.toolkit.
            /// </summary>
            [SerializeField]
            public string package;

            /// <summary>
            /// The version of the XR Interaction Toolkit package installed.
            /// </summary>
            [SerializeField]
            public string package_ver;
#endif
        }

#if !UNITY_2023_2_OR_NEWER
        /// <inheritdoc />
        public XRIBuildEvent() : base(k_EventName, k_EventVersion, k_MaxEventsPerHour, k_MaxItems)
        {
        }
#endif
    }
}

#endif
