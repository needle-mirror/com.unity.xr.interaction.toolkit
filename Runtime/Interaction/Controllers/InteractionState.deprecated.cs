using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial struct InteractionState
    {
        /// <summary>
        /// (Deprecated) Whether the interaction state was deactivated this frame.
        /// </summary>
        /// <remarks>
        /// <c>deActivatedThisFrame</c> has been deprecated. Use <see cref="deactivatedThisFrame"/> instead.
        /// </remarks>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("deActivatedThisFrame has been deprecated. Use deactivatedThisFrame instead. (UnityUpgradable) -> deactivatedThisFrame", true)]
        public bool deActivatedThisFrame
        {
            get => default;
            set => _ = value;
        }
#pragma warning restore IDE1006

        /// <summary>
        /// (Deprecated) Resets the interaction states that are based on whether they occurred "this frame".
        /// </summary>
        /// <remarks>
        /// <c>Reset</c> has been deprecated. Use <see cref="ResetFrameDependent"/> instead.
        /// </remarks>
        [Obsolete("Reset has been renamed. Use ResetFrameDependent instead. (UnityUpgradable) -> ResetFrameDependent()", true)]
        public void Reset()
        {
        }
    }
}
