#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using Unity.XR.CoreUtils.Editor.Analytics;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <inheritdoc />
    abstract class XRIBaseAnalyticsEvent<T> : EditorAnalyticsEvent<T> where T : struct
#if UNITY_2023_2_OR_NEWER
        , IAnalytic.IData
#endif
    {
#if !UNITY_2023_2_OR_NEWER
        /// <summary>
        /// Hourly limit for this event name.
        /// </summary>
        int maxEventsPerHour { get; }

        /// <summary>
        /// Maximum number of items in this event.
        /// </summary>
        int maxItems { get; }

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="eventName">This event name.</param>
        /// <param name="eventVersion">This event version.</param>
        /// <param name="maxEventsPerHour">Hourly limit for this event name.</param>
        /// <param name="maxItems">Maximum number of items in this event.</param>
        protected XRIBaseAnalyticsEvent(string eventName, int eventVersion, int maxEventsPerHour, int maxItems)
            : base(eventName, eventVersion)
        {
            this.maxEventsPerHour = maxEventsPerHour;
            this.maxItems = maxItems;
        }
#endif

        /// <inheritdoc />
        protected override AnalyticsResult SendToAnalyticsServer(T parameter)
        {
#if UNITY_2023_2_OR_NEWER
            var result = EditorAnalytics.SendAnalytic(this);
#else
            var result = EditorAnalytics.SendEventWithLimit(EventName, parameter, EventVersion);
#endif

            return result;
        }

        /// <inheritdoc />
        protected override AnalyticsResult RegisterWithAnalyticsServer()
        {
#if UNITY_2023_2_OR_NEWER
            return AnalyticsResult.Ok;
#else
            return EditorAnalytics.RegisterEventWithLimit(EventName, maxEventsPerHour, maxItems, XRIAnalytics.VendorKey, EventVersion);
#endif
        }
    }
}

#endif
