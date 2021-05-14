using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Utility functions related to sorting.
    /// </summary>
    static class SortingHelpers
    {
        /// <summary>
        /// Reusable mapping of Interactables to their distance squared from an Interactor (used for sort).
        /// </summary>
        static readonly Dictionary<XRBaseInteractable, float> s_InteractableDistanceSqrMap = new Dictionary<XRBaseInteractable, float>();

        /// <summary>
        /// Used to avoid GC Alloc that would happen if using <see cref="InteractableDistanceComparison"/> directly
        /// as argument to <see cref="List{T}.Sort(Comparison{T})"/>.
        /// </summary>
        static readonly Comparison<XRBaseInteractable> s_InteractableDistanceComparison = InteractableDistanceComparison;

        public static void Sort<T>(IList<T> hits, IComparer<T> comparer) where T : struct
        {
            bool fullPass;
            do
            {
                fullPass = true;
                for (var i = 1; i < hits.Count; ++i)
                {
                    var result = comparer.Compare(hits[i - 1], hits[i]);
                    if (result > 0)
                    {
                        var temp = hits[i - 1];
                        hits[i - 1] = hits[i];
                        hits[i] = temp;
                        fullPass = false;
                    }
                }
            } while (fullPass == false);
        }

        /// <summary>
        /// Sorts the Interactables in <paramref name="unsortedTargets"/> by distance to the <paramref name="interactor"/>,
        /// storing the ordered result in <paramref name="results"/>.
        /// </summary>
        /// <param name="interactor">The Interactor to calculate distance against.</param>
        /// <param name="unsortedTargets">The read only list of Interactables to sort. This list is not modified.</param>
        /// <param name="results">The results list to populate with the sorted results.</param>
        /// <remarks>
        /// Clears <paramref name="results"/> before adding to it.
        /// This method is not thread safe.
        /// </remarks>
        public static void SortByDistanceToInteractor(XRBaseInteractor interactor, List<XRBaseInteractable> unsortedTargets, List<XRBaseInteractable> results)
        {
            results.Clear();
            results.AddRange(unsortedTargets);

            s_InteractableDistanceSqrMap.Clear();
            foreach (var interactable in unsortedTargets)
            {
                s_InteractableDistanceSqrMap[interactable] = interactable.GetDistanceSqrToInteractor(interactor);
            }

            results.Sort(s_InteractableDistanceComparison);
        }

        static int InteractableDistanceComparison(XRBaseInteractable x, XRBaseInteractable y)
        {
            var xDistance = s_InteractableDistanceSqrMap[x];
            var yDistance = s_InteractableDistanceSqrMap[y];
            if (xDistance > yDistance)
                return 1;
            if (xDistance < yDistance)
                return -1;

            return 0;
        }
    }
}
