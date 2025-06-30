#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Tracks the activation status of various locomotion providers during a play mode session.
    /// </summary>
    class LocomotionProviderTracker
    {
        /// <summary>
        /// The list of Locomotion Provider types that were present in the play mode session,
        /// allowing duplicates.
        /// </summary>
        readonly List<Type> m_ProviderTypes = new List<Type>();

        /// <summary>
        /// The set of Locomotion Provider types that were used, meaning they moved into the <see cref="LocomotionState.Moving"/> state.
        /// </summary>
        readonly HashSet<Type> m_ProviderTypesUsed = new HashSet<Type>();

        /// <summary>
        /// Whether the comfort vignette was used.
        /// </summary>
        bool m_LocomotionComfortWasUsed;

        /// <summary>
        /// Resets the tracker to its initial state.
        /// Call this after generating the analytics payload to avoid accumulating data across
        /// multiple play mode sessions when domain reload is disabled.
        /// </summary>
        public void Reset()
        {
            m_ProviderTypes.Clear();
            m_ProviderTypesUsed.Clear();
            m_LocomotionComfortWasUsed = false;
        }

        /// <summary>
        /// Retrieves the current locomotion play mode data.
        /// </summary>
        /// <returns>Returns a <see cref="NameCountUsageData"/> containing the tracked locomotion data.</returns>
        NameCountUsageData GetLocomotionData()
        {
            var builtInTypesUnsorted = new List<NameCountUsageData.NameCountUsageEntry>();
            var unityTypesUnsorted = new List<NameCountUsageData.NameCountUsageEntry>();
            var customTypesUnsorted = new List<NameCountUsageData.NameCountUsageEntry>();
            var customDerivedComponents = new List<Type>();

            foreach (var group in m_ProviderTypes.GroupBy(i => i))
            {
                var typeCategory = XRIAnalyticsUtility.DetermineTypeCategory(group.Key);
                var count = group.Count();
                var wasUsed = m_ProviderTypesUsed.Contains(group.Key);

                switch (typeCategory)
                {
                    case XRIAnalyticsUtility.TypeCategory.BuiltIn:
                        builtInTypesUnsorted.Add(new NameCountUsageData.NameCountUsageEntry { typeName = group.Key.Name, count = count, wasUsed = wasUsed, });
                        break;

                    case XRIAnalyticsUtility.TypeCategory.Unity:
                        unityTypesUnsorted.Add(new NameCountUsageData.NameCountUsageEntry { typeName = group.Key.FullName, count = count, wasUsed = wasUsed, });
                        break;

                    case XRIAnalyticsUtility.TypeCategory.Custom:
                        customDerivedComponents.AddRange(group);
                        break;
                }
            }

            if (customDerivedComponents.Count > 0)
            {
                // Find the Unity type these custom components are derived from.
                foreach (var group in customDerivedComponents.GroupBy(XRIAnalyticsUtility.GetClosestUnityType))
                {
                    var count = group.Count();
                    var wasUsed = group.Any(i => m_ProviderTypesUsed.Contains(i));
                    var typeName = XRIAnalyticsUtility.IsXRIRuntimeAssembly(group.Key) ? group.Key.Name : group.Key.FullName;
                    customTypesUnsorted.Add(new NameCountUsageData.NameCountUsageEntry { typeName = typeName, count = count, wasUsed = wasUsed, });
                }
            }

            return new NameCountUsageData
            {
                builtInTypes = builtInTypesUnsorted.OrderByDescending(data => data.wasUsed).ThenBy(data => data.typeName).ToArray(),
                unityTypes = unityTypesUnsorted.OrderByDescending(data => data.wasUsed).ThenBy(data => data.typeName).ToArray(),
                customTypes = customTypesUnsorted.OrderByDescending(data => data.wasUsed).ThenBy(data => data.typeName).ToArray(),
            };
        }

        /// <summary>
        /// Update the analytics payload struct with the data from this tracker.
        /// </summary>
        /// <param name="payload">The analytics payload to write into.</param>
        public void UpdateEventPayload(ref XRIPlayModeEvent.Payload payload)
        {
            payload.locomotionPlayModeData = GetLocomotionData();
            payload.locomotionComfortWasUsed = m_LocomotionComfortWasUsed;
        }

        /// <summary>
        /// Start tracking the locomotion provider components.
        /// </summary>
        public void StartSession()
        {
            foreach (var provider in LocomotionProvider.locomotionProviders)
            {
                if (provider != null)
                {
                    m_ProviderTypes.Add(provider.GetType());
                    provider.locomotionStarted += OnLocomotionStarted;
                }
            }

            LocomotionProvider.locomotionProvidersChanged += OnLocomotionProvidersChanged;
            TunnelingVignetteController.vignetteProviderQueued += OnVignetteProviderQueued;
        }

        /// <summary>
        /// End tracking the locomotion provider components.
        /// </summary>
        public void EndSession()
        {
            foreach (var provider in LocomotionProvider.locomotionProviders)
            {
                if (provider != null)
                    provider.locomotionStarted -= OnLocomotionStarted;
            }

            LocomotionProvider.locomotionProvidersChanged -= OnLocomotionProvidersChanged;
            TunnelingVignetteController.vignetteProviderQueued -= OnVignetteProviderQueued;
        }

        void OnLocomotionProvidersChanged(LocomotionProvider provider)
        {
            // Handle a locomotion provider component being added after the start session.
            var providerType = provider.GetType();
            m_ProviderTypes.Add(providerType);

            // Subscribe to the event to track when used (if necessary)
            if (!m_ProviderTypesUsed.Contains(providerType))
                provider.locomotionStarted += OnLocomotionStarted;
        }

        void OnLocomotionStarted(LocomotionProvider provider)
        {
            // Once the locomotion provider instance has started once,
            // we no longer need to subscribe to the event since we only need to record
            // the fact that the type has been used.
            m_ProviderTypesUsed.Add(provider.GetType());
            provider.locomotionStarted -= OnLocomotionStarted;
        }

        void OnVignetteProviderQueued(ITunnelingVignetteProvider provider)
        {
            m_LocomotionComfortWasUsed = true;
            TunnelingVignetteController.vignetteProviderQueued -= OnVignetteProviderQueued;
        }
    }
}

#endif
