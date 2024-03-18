using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Represents which hand or controller the interactor is associated with.
    /// </summary>
    /// <seealso cref="IXRInteractor.handedness"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public enum InteractorHandedness
    {
        /// <summary>
        /// The interactor is not associated with a hand or controller.
        /// The gaze interactor is an example of this.
        /// </summary>
        None,

        /// <summary>
        /// The interactor is associated with the left hand or controller.
        /// </summary>
        Left,

        /// <summary>
        /// The interactor is associated with the right hand or controller.
        /// </summary>
        Right,
    }
}
