using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// A locomotion provider that allows the user to rotate their rig using a 2D axis input.
    /// </summary>
    [Obsolete(k_SnapTurnProviderBaseDeprecated, true)]
    public abstract class SnapTurnProviderBase : LocomotionProvider
    {
        const string k_SnapTurnProviderBaseDeprecated = "SnapTurnProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use SnapTurnProvider instead.";

        /// <summary>
        /// The number of degrees clockwise Unity rotates the rig when snap turning clockwise.
        /// </summary>
        public float turnAmount
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The amount of time that Unity waits before starting another snap turn.
        /// </summary>
        public float debounceTime
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether to enable left and right snap turns.
        /// </summary>
        /// <seealso cref="enableTurnAround"/>
        public bool enableTurnLeftRight
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Controls whether to enable 180° snap turns.
        /// </summary>
        /// <seealso cref="enableTurnLeftRight"/>
        public bool enableTurnAround
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The time (in seconds) to delay the first turn after receiving initial input for the turn.
        /// Subsequent turns while holding down input are delayed by the <see cref="debounceTime"/>, not the delay time.
        /// This delay can be used, for example, as time to set a tunneling vignette effect as a VR comfort option.
        /// </summary>
        public float delayTime
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            throw new NotSupportedException(k_SnapTurnProviderBaseDeprecated);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
        }

        /// <summary>
        /// Reads the current value of the snap turn input.
        /// </summary>
        /// <returns>Returns the input vector, such as from a thumbstick.</returns>
        protected abstract Vector2 ReadInput();

        /// <summary>
        /// Determines the turn amount in degrees for the given <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns the turn amount in degrees for the given <paramref name="input"/> vector.</returns>
        protected virtual float GetTurnAmount(Vector2 input)
        {
            throw new NotSupportedException(k_SnapTurnProviderBaseDeprecated);
        }

        /// <summary>
        /// Begins turning locomotion.
        /// </summary>
        /// <param name="amount">Amount to turn.</param>
        protected void StartTurn(float amount)
        {
            throw new NotSupportedException(k_SnapTurnProviderBaseDeprecated);
        }
    }
}
