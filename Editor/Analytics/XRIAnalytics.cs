#if XRI_ANALYTICS_DEBUGGING_ENABLED
using UnityEngine;
#endif

#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <summary>
    /// The entry point class to send XR Interaction Toolkit analytics data.
    /// </summary>
    [InitializeOnLoad]
    static class XRIAnalytics
    {
        /// <summary>
        /// Vendor key name.
        /// </summary>
        public const string VendorKey = "unity.xrinteractiontoolkit";

        /// <summary>
        /// Default hourly limit for an event.
        /// </summary>
        public const int DefaultMaxEventsPerHour = 1000; // Same default value as AnalyticInfoAttribute

        /// <summary>
        /// Default maximum number of items in an event.
        /// </summary>
        public const int DefaultMaxItems = 1000; // Same default value as AnalyticInfoAttribute

#if UNITY_2023_2_OR_NEWER
        const string k_PackageName = "com.unity.xr.interaction.toolkit";
#endif

        static XRIPlayModeEvent playModeEvent { get; } = new XRIPlayModeEvent();

        static XRIBuildEvent buildEvent { get; } = new XRIBuildEvent();

        static XRIAnalytics()
        {
#if !UNITY_2023_2_OR_NEWER
            playModeEvent.Register();
            buildEvent.Register();
#endif
        }

        /// <summary>
        /// Send the given parameter as payload to the analytics server.
        /// </summary>
        /// <param name="payload">The parameter object within the event.</param>
        /// <returns>Returns whenever the event was successfully sent. Returns <see langword="false"/> if this event was not registered yet.</returns>
        public static bool Send(XRIPlayModeEvent.Payload payload)
        {
            if (!EditorAnalytics.enabled)
                return false;

#if UNITY_2023_2_OR_NEWER
            if (!TryGetPackageVersion(k_PackageName, out var packageVersion))
                return false;

            // Ensure package name and version are included in the payload
            payload.package = k_PackageName;
            payload.package_ver = packageVersion;
#endif

#if XRI_ANALYTICS_DEBUGGING_ENABLED
            Debug.Log($"{payload.GetType().FullName}\n{JsonUtility.ToJson(payload, true)}");
            return true;
#else
            return playModeEvent.Send(payload);
#endif
        }

        /// <summary>
        /// Send the given parameter as payload to the analytics server.
        /// </summary>
        /// <param name="payload">The parameter object within the event.</param>
        /// <returns>Returns whenever the event was successfully sent. Returns <see langword="false"/> if this event was not registered yet.</returns>
        public static bool Send(XRIBuildEvent.Payload payload)
        {
            if (!EditorAnalytics.enabled)
                return false;

#if UNITY_2023_2_OR_NEWER
            if (!TryGetPackageVersion(k_PackageName, out var packageVersion))
                return false;

            // Ensure package name and version are included in the payload
            payload.package = k_PackageName;
            payload.package_ver = packageVersion;
#endif

#if XRI_ANALYTICS_DEBUGGING_ENABLED
            Debug.Log($"{payload.GetType().FullName}\n{JsonUtility.ToJson(payload, true)}");
            return true;
#else
            return buildEvent.Send(payload);
#endif
        }

#if UNITY_2023_2_OR_NEWER
        static bool TryGetPackageVersion(string packageName, out string version)
        {
            // FindForPackageName can fail and return null, even if the package is installed, if attempted too early.
            // This would happen if this was evaluated during the static constructor as a static readonly field.
            // So only attempt to get the version while finalizing the payload.

            var info = PackageManager.PackageInfo.FindForPackageName(packageName);
            if (info == null)
            {
                version = null;
                return false;
            }

            version = info.version;
            return true;
        }
#endif
    }
}

#endif
