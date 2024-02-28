using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Defines an interface for updating a UI data model.
    /// </summary>
    /// <remarks>
    /// The primary functionality is to enable casters to receive calls to update the UI data model based on the current state of the caster and
    /// attempting to obtain the current UI raycast result from the caster.
    /// </remarks>
    /// <seealso cref="CurveInteractionCaster"/>
    /// <seealso cref="IUIInteractor.UpdateUIModel"/>
    public interface IUIModelUpdater
    {
        /// <summary>
        /// Updates the UI data model based on the implementer of this interface.
        /// </summary>
        /// <param name="uiModel">UI data model to update.</param>
        /// <param name="isSelectActive">UI select input state to write into the UI data model.</param>
        /// <param name="scrollDelta">UI scroll input state to write into the UI data model.</param>
        /// <returns>Returns <see langword="true"/> if UI data model was updated successfully.</returns>
        bool UpdateUIModel(ref TrackedDeviceModel uiModel, bool isSelectActive, in Vector2 scrollDelta);
    }
}