using System;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    public abstract partial class LocomotionProvider
    {
        /// <summary>
        /// (Deprecated) The <see cref="startLocomotion"/> action will be called when a <see cref="LocomotionProvider"/> successfully begins a locomotion event.
        /// </summary>
        /// <seealso cref="beginLocomotion"/>
        /// <remarks>
        /// <c>startLocomotion</c> has been deprecated. Use <see cref="beginLocomotion"/> instead.
        /// </remarks>
        [Obsolete("startLocomotion has been deprecated in XRI 3.0.0. Use beginLocomotion instead. (UnityUpgradable) -> beginLocomotion", true)]
#pragma warning disable 67 // Never invoked, kept for API Updater
        public event Action<LocomotionSystem> startLocomotion;
#pragma warning restore 67

        [Tooltip("(Deprecated) The Locomotion System that this locomotion provider communicates with for exclusive access to an XR Origin." +
            " If one is not provided, the behavior will attempt to locate one during its Awake call.")]
        [Obsolete("LocomotionSystem is deprecated in XRI 3.0.0 and will be removed in a future release. Use mediator instead.", false)]
        LocomotionSystem m_System;

        /// <summary>
        /// (Deprecated) The <see cref="LocomotionSystem"/> that this <see cref="LocomotionProvider"/> communicates with
        /// for exclusive access to an XR Origin.
        /// </summary>
        /// <remarks><see cref="system"/> is deprecated. Use <see cref="mediator"/> instead.</remarks>
        [Obsolete("LocomotionSystem is deprecated in XRI 3.0.0 and will be removed in a future release. Use mediator instead.", false)]
        public LocomotionSystem system
        {
            get => m_System;
            set => m_System = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="LocomotionPhase"/> of this <see cref="LocomotionProvider"/>.
        /// </summary>
        /// <remarks><see cref="locomotionPhase"/> is deprecated. Use <see cref="locomotionState"/> instead.
        /// Note that <see cref="locomotionState"/> cannot be set because it is determined by the <see cref="mediator"/>.</remarks>
        [Obsolete("locomotionPhase is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionState instead.", false)]
        public LocomotionPhase locomotionPhase { get; protected set; }

#pragma warning disable 0067 // Suppress event never used warning
        /// <summary>
        /// (Deprecated) Unity calls the <see cref="beginLocomotion"/> action when a <see cref="LocomotionProvider"/> successfully begins a locomotion event.
        /// </summary>
        /// <remarks><see cref="beginLocomotion"/> is deprecated. Use <see cref="locomotionStarted"/> instead.</remarks>
        [Obsolete("beginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionStarted instead.", false)]
        public event Action<LocomotionSystem> beginLocomotion;

        /// <summary>
        /// (Deprecated) Unity calls the <see cref="endLocomotion"/> action when a <see cref="LocomotionProvider"/> successfully ends a locomotion event.
        /// </summary>
        /// <remarks><see cref="endLocomotion"/> is deprecated. Use <see cref="locomotionEnded"/> instead.</remarks>
        [Obsolete("endLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionEnded instead.", false)]
        public event Action<LocomotionSystem> endLocomotion;
#pragma warning restore 0067

        /// <summary>
        /// (Deprecated) Checks if locomotion can begin.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if locomotion can start. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="CanBeginLocomotion"/> is deprecated. Instead, query <see cref="isLocomotionActive"/> to check
        /// if locomotion can start.</remarks>
        [Obsolete("CanBeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, query isLocomotionActive to check if locomotion can start.", false)]
        protected bool CanBeginLocomotion()
        {
            if (m_System == null)
                return false;

            return !m_System.busy;
        }

        /// <summary>
        /// (Deprecated) Invokes begin locomotion events.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="BeginLocomotion"/> is deprecated. Instead, call <see cref="TryPrepareLocomotion"/> when
        /// locomotion start input occurs.</remarks>
        [Obsolete("BeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryPrepareLocomotion when locomotion start input occurs.", false)]
        protected bool BeginLocomotion()
        {
            if (m_System == null)
                return false;

            var success = m_System.RequestExclusiveOperation(this) == RequestResult.Success;
            if (success)
                beginLocomotion?.Invoke(m_System);

            return success;
        }

        /// <summary>
        /// (Deprecated) Invokes end locomotion events.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks><see cref="EndLocomotion"/> is deprecated. Instead, call <see cref="TryEndLocomotion"/> when
        /// locomotion end input has completed.</remarks>
        [Obsolete("EndLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryEndLocomotion when locomotion end input has completed.", false)]
        protected bool EndLocomotion()
        {
            if (m_System == null)
                return false;

            var success = m_System.FinishExclusiveOperation(this) == RequestResult.Success;
            if (success)
                endLocomotion?.Invoke(m_System);

            return success;
        }
    }
}
