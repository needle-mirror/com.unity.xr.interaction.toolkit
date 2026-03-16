#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
#endif

#if UITOOLKIT_WORLDSPACE_ENABLED
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility for filtering trigger colliders from physics query results based on
    /// snap volume and UI document settings.
    /// </summary>
    internal static class TriggerColliderFilterUtility
    {
        /// <summary>
        /// Determines whether a trigger collider should be removed from query results.
        /// </summary>
        /// <remarks>
        /// This method is only called for non-null trigger colliders. UI Document detection
        /// is only available when UI Toolkit is installed on Unity 6.2 or higher.
        /// </remarks>
        static bool ShouldRemoveTriggerCollider(
            XRInteractionManager interactionManager,
            Collider triggerCollider,
            bool baseQueryIncludesTriggers,
            bool testSnapVolumes,
            bool testUIDocuments)
        {
            var matchesSnapVolume = testSnapVolumes &&
                interactionManager != null &&
                interactionManager.IsColliderRegisteredSnapVolume(triggerCollider);

#if UITOOLKIT_WORLDSPACE_ENABLED
            var matchesUIDocument = testUIDocuments &&
                XRUIToolkitHandler.HasUIDocument(triggerCollider);
#else
            var matchesUIDocument = false;
#endif

            var matchesRelevantType = matchesSnapVolume || matchesUIDocument;

            // If the base query includes triggers, we only test for *disallowed* types, and remove on match.
            // If the base query ignores triggers, we only test for *allowed* types, and remove when not matched.
            return baseQueryIncludesTriggers ? matchesRelevantType : !matchesRelevantType;
        }

        /// <summary>
        /// Filters trigger colliders from physics query results based on whether the base query includes triggers
        /// and which trigger types are allowed. Null colliders are always removed.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager used to help identify snap volume colliders.</param>
        /// <param name="colliders">Array containing collider results.</param>
        /// <param name="count">Number of valid entries in <paramref name="colliders"/>.</param>
        /// <param name="baseQueryIncludesTriggers">Whether the base physics query is configured to include trigger colliders.</param>
        /// <param name="allowSnapVolumeTriggers">Whether snap volume trigger colliders should be allowed through the filter.</param>
        /// <param name="allowUIDocumentTriggers">Whether UI Document trigger colliders should be allowed through the filter.</param>
        /// <returns>The number of colliders remaining after filtering.</returns>
        /// <remarks>
        /// When <paramref name="baseQueryIncludesTriggers"/> is <see langword="true"/>, triggers are already present in the results,
        /// so this filter removes only trigger colliders of explicitly disallowed special types (snap volumes / UI documents).
        /// <para>
        /// When <paramref name="baseQueryIncludesTriggers"/> is <see langword="false"/>, triggers are not expected in the results,
        /// so if any triggers appear, this filter keeps only trigger colliders of explicitly allowed special types and removes the rest.
        /// </para>
        /// </remarks>
        public static int FilterTriggerColliders(
            XRInteractionManager interactionManager,
            Collider[] colliders,
            int count,
            bool baseQueryIncludesTriggers,
            bool allowSnapVolumeTriggers,
            bool allowUIDocumentTriggers)
        {
            var remainingCount = count;

            // If base query includes triggers, we test for disallowed types.
            // If base query ignores triggers, we test for allowed types.
            var testSnapVolumes = baseQueryIncludesTriggers ? !allowSnapVolumeTriggers : allowSnapVolumeTriggers;
            var testUIDocuments = baseQueryIncludesTriggers ? !allowUIDocumentTriggers : allowUIDocumentTriggers;

            for (var index = 0; index < remainingCount; ++index)
            {
                var hitCollider = colliders[index];
                if (hitCollider == null)
                {
                    colliders[index--] = colliders[--remainingCount];
                    continue;
                }

                if (!hitCollider.isTrigger)
                    continue;

                if (ShouldRemoveTriggerCollider(
                    interactionManager,
                    hitCollider,
                    baseQueryIncludesTriggers,
                    testSnapVolumes,
                    testUIDocuments))
                {
                    colliders[index--] = colliders[--remainingCount];
                }
            }

            return remainingCount;
        }

        /// <summary>
        /// Filters trigger colliders from physics query results based on whether the base query includes triggers
        /// and which trigger types are allowed. Null colliders are always removed.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager used to help identify snap volume colliders.</param>
        /// <param name="raycastHits">Array containing raycast hit results.</param>
        /// <param name="count">Number of valid entries in <paramref name="raycastHits"/>.</param>
        /// <param name="baseQueryIncludesTriggers">Whether the base physics query is configured to include trigger colliders.</param>
        /// <param name="allowSnapVolumeTriggers">Whether snap volume trigger colliders should be allowed through the filter.</param>
        /// <param name="allowUIDocumentTriggers">Whether UI Document trigger colliders should be allowed through the filter.</param>
        /// <returns>The number of hits remaining after filtering.</returns>
        /// <remarks>
        /// When <paramref name="baseQueryIncludesTriggers"/> is <see langword="true"/>, triggers are already present in the results,
        /// so this filter removes only trigger colliders of explicitly disallowed special types (snap volumes / UI documents).
        /// <para>
        /// When <paramref name="baseQueryIncludesTriggers"/> is <see langword="false"/>, triggers are not expected in the results,
        /// so if any triggers appear, this filter keeps only trigger colliders of explicitly allowed special types and removes the rest.
        /// </para>
        /// </remarks>
        public static int FilterTriggerColliders(
            XRInteractionManager interactionManager,
            RaycastHit[] raycastHits,
            int count,
            bool baseQueryIncludesTriggers,
            bool allowSnapVolumeTriggers,
            bool allowUIDocumentTriggers)
        {
            var remainingCount = count;

            // If base query includes triggers, we test for disallowed types.
            // If base query ignores triggers, we test for allowed types.
            var testSnapVolumes = baseQueryIncludesTriggers ? !allowSnapVolumeTriggers : allowSnapVolumeTriggers;
            var testUIDocuments = baseQueryIncludesTriggers ? !allowUIDocumentTriggers : allowUIDocumentTriggers;

            for (var index = 0; index < remainingCount; ++index)
            {
                var hitCollider = raycastHits[index].collider;
                if (hitCollider == null)
                {
                    raycastHits[index--] = raycastHits[--remainingCount];
                    continue;
                }

                if (!hitCollider.isTrigger)
                    continue;

                if (ShouldRemoveTriggerCollider(
                    interactionManager,
                    hitCollider,
                    baseQueryIncludesTriggers,
                    testSnapVolumes,
                    testUIDocuments))
                {
                    raycastHits[index--] = raycastHits[--remainingCount];
                }
            }

            return remainingCount;
        }
    }
}
