using System;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to smoothly rotate their rig continuously over time.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    /// <seealso cref="SnapTurnProviderBase"/>
    [Obsolete(k_ContinuousTurnProviderBaseDeprecated, true)]
    public abstract class ContinuousTurnProviderBase : LocomotionProvider
    {
        const string k_ContinuousTurnProviderBaseDeprecated = "The ContinuousMoveProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use ContinuousTurnProvider instead.";

        /// <summary>
        /// The number of degrees/second clockwise to rotate when turning clockwise.
        /// </summary>
        public float turnSpeed
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
        }

        /// <summary>
        /// Reads the current value of the turn input.
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
            throw new NotSupportedException(k_ContinuousTurnProviderBaseDeprecated);
        }

        /// <summary>
        /// Rotates the rig by <paramref name="turnAmount"/> degrees.
        /// </summary>
        /// <param name="turnAmount">The amount of rotation in degrees.</param>
        protected void TurnRig(float turnAmount)
        {
            throw new NotSupportedException(k_ContinuousTurnProviderBaseDeprecated);
        }
    }
}
