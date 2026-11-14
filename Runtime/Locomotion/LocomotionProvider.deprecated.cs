using System;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    public abstract partial class LocomotionProvider
    {
        const string k_CanBeginLocomotionDeprecated = "CanBeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, query isLocomotionActive to check if locomotion can start.";
        const string k_BeginLocomotionDeprecated = "BeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryPrepareLocomotion when locomotion start input occurs.";
        const string k_EndLocomotionDeprecated = "EndLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryEndLocomotion when locomotion end input has completed.";

        /// <summary>
        /// (Deprecated) The <see cref="LocomotionSystem"/> that this <see cref="LocomotionProvider"/> communicates with
        /// for exclusive access to an XR Origin.
        /// </summary>
        /// <remarks><see cref="system"/> is deprecated. Use <see cref="mediator"/> instead.</remarks>
        [Obsolete("LocomotionSystem is deprecated in XRI 3.0.0 and will be removed in a future release. Use mediator instead.", true)]
        public LocomotionSystem system
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="LocomotionPhase"/> of this <see cref="LocomotionProvider"/>.
        /// </summary>
        /// <remarks><see cref="locomotionPhase"/> is deprecated. Use <see cref="locomotionState"/> instead.
        /// Note that <see cref="locomotionState"/> cannot be set because it is determined by the <see cref="mediator"/>.</remarks>
        [Obsolete("locomotionPhase is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionState instead.", true)]
        public LocomotionPhase locomotionPhase { get; protected set; }

#pragma warning disable 0067 // Suppress event never used warning
        /// <summary>
        /// (Deprecated) Unity calls the <see cref="beginLocomotion"/> action when a <see cref="LocomotionProvider"/> successfully begins a locomotion event.
        /// </summary>
        /// <remarks><see cref="beginLocomotion"/> is deprecated. Use <see cref="locomotionStarted"/> instead.</remarks>
        [Obsolete("beginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionStarted instead.", true)]
        public event Action<LocomotionSystem> beginLocomotion;

        /// <summary>
        /// (Deprecated) Unity calls the <see cref="endLocomotion"/> action when a <see cref="LocomotionProvider"/> successfully ends a locomotion event.
        /// </summary>
        /// <remarks><see cref="endLocomotion"/> is deprecated. Use <see cref="locomotionEnded"/> instead.</remarks>
        [Obsolete("endLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionEnded instead.", true)]
        public event Action<LocomotionSystem> endLocomotion;
#pragma warning restore 0067

        /// <summary>
        /// (Deprecated) Checks if locomotion can begin.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if locomotion can start. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="CanBeginLocomotion"/> is deprecated. Instead, query <see cref="isLocomotionActive"/> to check
        /// if locomotion can start.</remarks>
        [Obsolete(k_CanBeginLocomotionDeprecated, true)]
        protected bool CanBeginLocomotion()
        {
            throw new NotSupportedException(k_CanBeginLocomotionDeprecated);
        }

        /// <summary>
        /// (Deprecated) Invokes begin locomotion events.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="BeginLocomotion"/> is deprecated. Instead, call <see cref="TryPrepareLocomotion"/> when
        /// locomotion start input occurs.</remarks>
        [Obsolete(k_BeginLocomotionDeprecated, true)]
        protected bool BeginLocomotion()
        {
            throw new NotSupportedException(k_BeginLocomotionDeprecated);
        }

        /// <summary>
        /// (Deprecated) Invokes end locomotion events.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="EndLocomotion"/> is deprecated. Instead, call <see cref="TryEndLocomotion"/> when
        /// locomotion end input has completed.</remarks>
        [Obsolete(k_EndLocomotionDeprecated, true)]
        protected bool EndLocomotion()
        {
            throw new NotSupportedException(k_EndLocomotionDeprecated);
        }
    }
}
