using Unity.XR.CoreUtils.Editor.Analytics;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics
{
    /// <inheritdoc />
    abstract class XRIBaseAnalyticsEvent<T> : EditorAnalyticsEvent<T> where T : struct, IAnalytic.IData
    {
        /// <inheritdoc />
        protected override AnalyticsResult SendToAnalyticsServer(T parameter)
        {
            var result = EditorAnalytics.SendAnalytic(this);

            return result;
        }

        /// <inheritdoc />
        protected override AnalyticsResult RegisterWithAnalyticsServer()
        {
            return AnalyticsResult.Ok;
        }
    }
}
