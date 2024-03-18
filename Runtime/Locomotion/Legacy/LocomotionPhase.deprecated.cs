using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// (Deprecated) Options for describing different phases of a locomotion.
    /// </summary>
    /// <remarks><see cref="LocomotionPhase"/> is deprecated. Use <see cref="LocomotionState"/> instead.</remarks>
    [Obsolete("LocomotionPhase is deprecated in XRI 3.0.0 and will be removed in a future release. Use LocomotionState instead.", false)]
    public enum LocomotionPhase
    {
        /// <summary>
        /// (Deprecated) Describes the idle state of a locomotion, for example, when the user is standing still with no locomotion inputs.
        /// </summary>
        /// <remarks><see cref="LocomotionPhase.Idle"/> is deprecated. Use <see cref="LocomotionState.Idle"/> instead.</remarks>
        [Obsolete("LocomotionPhase.Idle is deprecated and will be removed in a future release. Use LocomotionState.Idle instead.", false)]
        Idle,
        /// <summary>
        /// (Deprecated) Describes the started state of a locomotion, for example, when the locomotion input action is started.
        /// </summary>
        /// <remarks><see cref="LocomotionPhase.Started"/> is deprecated. Use <see cref="LocomotionState.Preparing"/> instead.</remarks>
        [Obsolete("LocomotionPhase.Started is deprecated and will be removed in a future release. Use LocomotionState.Preparing instead.", false)]
        Started,
        /// <summary>
        /// (Deprecated) Describes the moving state of a locomotion, for example, when the user is continuously moving by pushing the joystick.
        /// </summary>
        /// <remarks><see cref="LocomotionPhase.Moving"/> is deprecated. Use <see cref="LocomotionState.Moving"/> instead.</remarks>
        [Obsolete("LocomotionPhase.Moving is deprecated and will be removed in a future release. Use LocomotionState.Moving instead.", false)]
        Moving,
        /// <summary>
        /// (Deprecated) Describes the done state of a locomotion, for example, when the user has ended moving.
        /// </summary>
        /// <remarks><see cref="LocomotionPhase.Done"/> is deprecated. Use <see cref="LocomotionState.Ended"/> instead.</remarks>
        [Obsolete("LocomotionPhase.Done is deprecated and will be removed in a future release. Use LocomotionState.Ended instead.", false)]
        Done,
    }
}
