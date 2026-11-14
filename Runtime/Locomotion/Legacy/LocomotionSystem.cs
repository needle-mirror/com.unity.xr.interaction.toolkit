using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// (Deprecated) The result of a locomotion request.
    /// </summary>
    /// <remarks>This enum is deprecated. Exclusive access behavior is no longer supported.</remarks>
    [Obsolete("RequestResult is deprecated in XRI 3.0.0 and will be removed in a future release. Exclusive access behavior is no longer supported.", true)]
    public enum RequestResult
    {
        /// <summary>
        /// (Deprecated) The locomotion request was successful.
        /// </summary>
        Success,

        /// <summary>
        /// (Deprecated) The locomotion request failed due to the system being currently busy.
        /// </summary>
        Busy,

        /// <summary>
        /// (Deprecated) The locomotion request failed due to an unknown error.
        /// </summary>
        Error,
    }

    /// <summary>
    /// (Deprecated)The <see cref="LocomotionSystem"/> object is used to control access to the XR Origin. This system enforces that only one
    /// Locomotion Provider can move the XR Origin at one time. This is the only place that access to an XR Origin is controlled,
    /// having multiple instances of a <see cref="LocomotionSystem"/> drive a single XR Origin is not recommended.
    /// </summary>
    /// <remarks><see cref="LocomotionSystem"/> is deprecated. Use <see cref="LocomotionMediator"/> instead.</remarks>
    [AddComponentMenu("XR/Locomotion/Legacy/Locomotion System", 11)]
    [HelpURL(XRHelpURLConstants.k_LocomotionSystem)]
    [Obsolete(k_LocomotionSystemDeprecated, true)]
    public class LocomotionSystem : MonoBehaviour
    {
        const string k_LocomotionSystemDeprecated = "LocomotionSystem is deprecated and will be removed in a future release. Use LocomotionMediator instead.";

        /// <summary>
        /// (Deprecated) The timeout (in seconds) for exclusive access to the XR Origin.
        /// </summary>
        public float timeout
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated)The XR Origin object to provide access control to.
        /// </summary>
        public XROrigin xrOrigin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) (Read Only) If this value is true, the XR Origin's position should not be modified until this false.
        /// </summary>
        public bool busy => default;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            throw new NotSupportedException(k_LocomotionSystemDeprecated);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
        }

        /// <summary>
        /// Attempt to "lock" access to the XR Origin for the <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">The locomotion provider that is requesting access.</param>
        /// <returns>Returns a <see cref="RequestResult"/> that reflects the status of the request.</returns>
        public RequestResult RequestExclusiveOperation(LocomotionProvider provider)
        {
            throw new NotSupportedException(k_LocomotionSystemDeprecated);
        }

        /// <summary>
        /// Informs the <see cref="LocomotionSystem"/> that exclusive access to the XR Origin is no longer required.
        /// </summary>
        /// <param name="provider">The locomotion provider that is relinquishing access.</param>
        /// <returns>Returns a <see cref="RequestResult"/> that reflects the status of the request.</returns>
        public RequestResult FinishExclusiveOperation(LocomotionProvider provider)
        {
            throw new NotSupportedException(k_LocomotionSystemDeprecated);
        }
    }
}
