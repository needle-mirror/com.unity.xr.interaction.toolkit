using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public abstract partial class XRBaseControllerInteractor
    {
        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Select Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectEntered"/>
        [Obsolete("playAudioClipOnSelectEnter has been deprecated. Use playAudioClipOnSelectEntered instead. (UnityUpgradable) -> playAudioClipOnSelectEntered")]
        public bool playAudioClipOnSelectEnter => playAudioClipOnSelectEntered;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
        [Obsolete("audioClipForOnSelectEnter has been deprecated. Use audioClipForOnSelectEntered instead. (UnityUpgradable) -> audioClipForOnSelectEntered")]
        public AudioClip audioClipForOnSelectEnter => audioClipForOnSelectEntered;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectEntered"/>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("AudioClipForOnSelectEnter has been deprecated. Use audioClipForOnSelectEntered instead. (UnityUpgradable) -> audioClipForOnSelectEntered")]
        public AudioClip AudioClipForOnSelectEnter
        {
            get => audioClipForOnSelectEntered;
            set => audioClipForOnSelectEntered = value;
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Select Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnSelectExited"/>
        [Obsolete("playAudioClipOnSelectExit has been deprecated. Use playAudioClipOnSelectExited instead. (UnityUpgradable) -> playAudioClipOnSelectExited")]
        public bool playAudioClipOnSelectExit => playAudioClipOnSelectExited;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
        [Obsolete("audioClipForOnSelectExit has been deprecated. Use audioClipForOnSelectExited instead. (UnityUpgradable) -> audioClipForOnSelectExited")]
        public AudioClip audioClipForOnSelectExit => audioClipForOnSelectExited;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Select Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnSelectExited"/>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("AudioClipForOnSelectExit has been deprecated. Use audioClipForOnSelectExited instead. (UnityUpgradable) -> audioClipForOnSelectExited")]
        public AudioClip AudioClipForOnSelectExit
        {
            get => audioClipForOnSelectExited;
            set => audioClipForOnSelectExited = value;
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Hover Entered.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverEntered"/>
        [Obsolete("playAudioClipOnHoverEnter has been deprecated. Use playAudioClipOnHoverEntered instead. (UnityUpgradable) -> playAudioClipOnHoverEntered")]
        public bool playAudioClipOnHoverEnter => playAudioClipOnHoverEntered;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
        [Obsolete("audioClipForOnHoverEnter has been deprecated. Use audioClipForOnHoverEntered instead. (UnityUpgradable) -> audioClipForOnHoverEntered")]
        public AudioClip audioClipForOnHoverEnter => audioClipForOnHoverEntered;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Entered.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverEntered"/>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("AudioClipForOnHoverEnter has been deprecated. Use audioClipForOnHoverEntered instead. (UnityUpgradable) -> audioClipForOnHoverEntered")]
        public AudioClip AudioClipForOnHoverEnter
        {
            get => audioClipForOnHoverEntered;
            set => audioClipForOnHoverEntered = value;
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Controls whether to play an <see cref="AudioClip"/> on Hover Exited.
        /// </summary>
        /// <seealso cref="audioClipForOnHoverExited"/>
        [Obsolete("playAudioClipOnHoverExit has been deprecated. Use playAudioClipOnHoverExited instead. (UnityUpgradable) -> playAudioClipOnHoverExited")]
        public bool playAudioClipOnHoverExit => playAudioClipOnHoverExited;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
        [Obsolete("audioClipForOnHoverExit has been deprecated. Use audioClipForOnHoverExited instead. (UnityUpgradable) -> audioClipForOnHoverExited")]
        public AudioClip audioClipForOnHoverExit => audioClipForOnHoverExited;

        /// <summary>
        /// The <see cref="AudioClip"/> to play on Hover Exited.
        /// </summary>
        /// <seealso cref="playAudioClipOnHoverExited"/>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("AudioClipForOnHoverExit has been deprecated. Use audioClipForOnHoverExited instead. (UnityUpgradable) -> audioClipForOnHoverExited")]
        public AudioClip AudioClipForOnHoverExit
        {
            get => audioClipForOnHoverExited;
            set => audioClipForOnHoverExited = value;
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Controls whether to play haptics on Select Entered.
        /// </summary>
        /// <seealso cref="hapticSelectEnterIntensity"/>
        /// <seealso cref="hapticSelectEnterDuration"/>
        [Obsolete("playHapticsOnSelectEnter has been deprecated. Use playHapticsOnSelectEntered instead. (UnityUpgradable) -> playHapticsOnSelectEntered")]
        public bool playHapticsOnSelectEnter => playHapticsOnSelectEntered;

        /// <summary>
        /// Controls whether to play haptics on Select Exited.
        /// </summary>
        /// <seealso cref="hapticSelectExitIntensity"/>
        /// <seealso cref="hapticSelectExitDuration"/>
        [Obsolete("playHapticsOnSelectExit has been deprecated. Use playHapticsOnSelectExited instead. (UnityUpgradable) -> playHapticsOnSelectExited")]
        public bool playHapticsOnSelectExit => playHapticsOnSelectExited;

        /// <summary>
        /// Controls whether to play haptics on Hover Entered.
        /// </summary>
        /// <seealso cref="hapticHoverEnterIntensity"/>
        /// <seealso cref="hapticHoverEnterDuration"/>
        [Obsolete("playHapticsOnHoverEnter has been deprecated. Use playHapticsOnHoverEntered instead. (UnityUpgradable) -> playHapticsOnHoverEntered")]
        public bool playHapticsOnHoverEnter => playHapticsOnHoverEntered;
    }
}