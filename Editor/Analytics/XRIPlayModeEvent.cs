#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// The analytics event for play mode.
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [AnalyticInfo(k_EventName, XRIAnalytics.VendorKey, k_EventVersion, k_MaxEventsPerHour, k_MaxItems)]
#endif
    class XRIPlayModeEvent : XRIBaseAnalyticsEvent<XRIPlayModeEvent.Payload>
    {
        const string k_EventName = "xrinteractiontoolkit_playmode";
        const int k_EventVersion = 1;
        const int k_MaxEventsPerHour = XRIAnalytics.DefaultMaxEventsPerHour;
        const int k_MaxItems = XRIAnalytics.DefaultMaxItems;

        /// <summary>
        /// The analytics payload for play mode.
        /// </summary>
        [Serializable]
        public struct Payload
#if UNITY_2023_2_OR_NEWER
            : IAnalytic.IData
#endif
        {
            // Do not rename any field, the field names are used to identify the table/event column of this event payload.

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
            /// The currently active XR Plug-in Management plug-in provider instance (<c>UnityEngine.XR.Management.XRLoader</c>).
            /// This may be empty if no loader is active or no headset is connected to the computer.
            /// </summary>
            [SerializeField]
            public string activeLoader;

            /// <summary>
            /// The list of enabled XR Plug-in Management plug-in providers (<c>UnityEngine.XR.Management.XRLoader</c>).
            /// This is the list of checkboxes in Edit > Project Settings > XR Plug-in Management.
            /// </summary>
            /// <remarks>
            /// Note that Editor Play mode uses Desktop Platform Settings regardless of Active Build Target.
            /// </remarks>
            [SerializeField]
            public string[] activeLoaders;

            /// <summary>
            /// The list of relevant XR packages that are installed in the project.
            /// </summary>
            [SerializeField]
            public PackageVersionData[] packages;

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
            /// The effective current setting related to scene and domain reload, which can greatly affect the duration spent to enter play mode.
            /// This corresponds with Enter Play Mode Settings inside Edit > Project Settings > Editor.
            /// <para>0 = Reload Domain and Scene</para>
            /// <para>1 = Reload Scene only</para>
            /// <para>2 = Reload Domain only</para>
            /// <para>3 = Do not reload Domain or Scene</para>
            /// </summary>
            [SerializeField]
            public EnterPlayModeOptions enterPlayModeSettings;

            /// <summary>
            /// Peak number of enabled input modality managers during a play mode session.
            /// </summary>
            [SerializeField]
            public int modalityManagersPeakCount;

            /// <summary>
            /// Total count of different input modality managers that were enabled during a play mode session.
            /// </summary>
            [SerializeField]
            public int modalityManagersObjectCount;

            /// <summary>
            /// Peak number of enabled interaction managers during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactionManagersPeakCount;

            /// <summary>
            /// Total count of different interaction managers that were enabled during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactionManagersObjectCount;

            /// <summary>
            /// Peak number of registered interactors across all interaction managers during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactorsPeakRegisteredCount;

            /// <summary>
            /// Total count of different interactors that were registered during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactorsObjectRegisteredCount;

            /// <summary>
            /// Peak number of registered interactables across all interaction managers during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactablesPeakRegisteredCount;

            /// <summary>
            /// Total count of different interactables that were registered during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactablesObjectRegisteredCount;

            /// <summary>
            /// The timestamp when the Unity Editor has entered play mode.
            /// This is <c>DateTime.Now.Ticks</c> at <c>PlayModeStateChange.EnteredPlayMode</c>.
            /// </summary>
            [SerializeField]
            public long playModeStartTimeTicks;

            /// <summary>
            /// The timestamp when the Unity Editor is exiting play mode.
            /// This is <c>DateTime.Now.Ticks</c> at <c>PlayModeStateChange.ExitingPlayMode</c>
            /// </summary>
            [SerializeField]
            public long playModeEndTimeTicks;

            /// <summary>
            /// Duration in seconds it took to enter play mode from clicking the play button.
            /// This is <c>Time.realtimeSinceStartupAsDouble</c> at <c>PlayModeStateChange.EnteredPlayMode</c>.
            /// </summary>
            [SerializeField]
            public float enteredPlayModeDurationSeconds;

            /// <summary>
            /// Duration in seconds of the play mode session.
            /// This is <c>Time.realtimeSinceStartupAsDouble</c> between <c>PlayModeStateChange.EnteredPlayMode</c> and <c>PlayModeStateChange.ExitingPlayMode</c>.
            /// </summary>
            [SerializeField]
            public float playModeDurationSeconds;

            /// <summary>
            /// Duration in seconds of at least one enabled input modality manager during a play mode session.
            /// </summary>
            [SerializeField]
            public float modalityManagerDurationSeconds;

            /// <summary>
            /// Information about the modality of the left hand/controller during a play mode session.
            /// </summary>
            [SerializeField]
            public ModalityRuntimeData leftModalityInfo;

            /// <summary>
            /// Information about the modality of the right hand/controller during a play mode session.
            /// </summary>
            [SerializeField]
            public ModalityRuntimeData rightModalityInfo;

            /// <summary>
            /// Duration in seconds of at least one enabled interaction manager during a play mode session.
            /// </summary>
            [SerializeField]
            public float interactionManagerDurationSeconds;

            /// <summary>
            /// Duration in seconds of the classic simulator component singleton being active during a play mode session.
            /// </summary>
            [SerializeField]
            public float deviceSimulatorDurationSeconds;

            /// <summary>
            /// The number of different classic simulator components that were active during a play mode session.
            /// </summary>
            [SerializeField]
            public int deviceSimulatorSessionCount;

            /// <summary>
            /// Duration in seconds of the newer interaction simulator component singleton being active during a play mode session.
            /// </summary>
            [SerializeField]
            public float interactionSimulatorDurationSeconds;

            /// <summary>
            /// The number of different newer interaction simulator components that were active during a play mode session.
            /// </summary>
            [SerializeField]
            public int interactionSimulatorSessionCount;

            /// <summary>
            /// Locomotion-related data captured during play mode.
            /// </summary>
            [SerializeField]
            public NameCountUsageData locomotionPlayModeData;

            /// <summary>
            /// Whether a comfort provider was used during play mode.
            /// </summary>
            [SerializeField]
            public bool locomotionComfortWasUsed;

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
        public XRIPlayModeEvent() : base(k_EventName, k_EventVersion, k_MaxEventsPerHour, k_MaxItems)
        {
        }
#endif
    }
}

#endif
