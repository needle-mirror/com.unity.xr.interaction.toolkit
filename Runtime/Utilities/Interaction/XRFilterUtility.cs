using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility methods for hover and select filters.
    /// </summary>
    static class XRFilterUtility
    {
        /// <summary>
        /// Returns the processing result of the given hover filters using the given Interactor and Interactable as
        /// parameters.
        /// </summary>
        /// <param name="hoverFilters">The hover filters to process.</param>
        /// <param name="interactor">The Interactor to be validate by the hover filters.</param>
        /// <param name="interactable">The Interactable to be validate by the hover filters.</param>
        /// <returns>
        /// Returns <see langword="true"/> if all processed filters also return <see langword="true"/>, or if
        /// <see cref="hoverFilters"/> is empty. Otherwise, returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method will ensure that all changes are buffered when processing, the buffered changes are applied
        /// when the processing is finished.
        /// </remarks>
        public static bool Process(SmallRegistrationList<IXRHoverFilter> hoverFilters, IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            if (hoverFilters.registeredSnapshot.Count == 0)
                return true;

            var alreadyBufferingChanges = hoverFilters.bufferChanges;
            hoverFilters.bufferChanges = true;
            var result = true;
            try
            {
                foreach (var filter in hoverFilters.registeredSnapshot)
                {
                    if (!filter.canProcess || filter.Process(interactor, interactable))
                        continue;

                    result = false;
                    break;
                }
            }
            finally
            {
                if (!alreadyBufferingChanges)
                    hoverFilters.bufferChanges = false;
            }

            return result;
        }

        /// <summary>
        /// Returns the processing result of the given select filters using the given Interactor and Interactable as
        /// parameters.
        /// </summary>
        /// <param name="selectFilters">The select filters to process.</param>
        /// <param name="interactor">The Interactor to be validate by the select filters.</param>
        /// <param name="interactable">The Interactable to be validate by the select filters.</param>
        /// <returns>
        /// Returns <see langword="true"/> if all processed filters also return <see langword="true"/>, or if
        /// <see cref="selectFilters"/> is empty. Otherwise, returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method will ensure that all changes are buffered when processing, the buffered changes are applied
        /// when the processing is finished.
        /// </remarks>
        public static bool Process(SmallRegistrationList<IXRSelectFilter> selectFilters, IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            if (selectFilters.registeredSnapshot.Count == 0)
                return true;

            var alreadyBufferingChanges = selectFilters.bufferChanges;
            selectFilters.bufferChanges = true;
            var result = true;
            try
            {
                foreach (var filter in selectFilters.registeredSnapshot)
                {
                    if (!filter.canProcess || filter.Process(interactor, interactable))
                        continue;

                    result = false;
                    break;
                }
            }
            finally
            {
                if (!alreadyBufferingChanges)
                    selectFilters.bufferChanges = false;
            }

            return result;
        }
    }
}
