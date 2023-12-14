using System;
using Unity.XR.CoreUtils;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// (Deprecated) The result of a locomotion request.
    /// </summary>
    /// <remarks>This enum is deprecated. Exclusive access behavior is no longer supported.</remarks>
    [Obsolete("RequestResult is deprecated in XRI 3.0.0 and will be removed in a future release. Exclusive access behavior is no longer supported.", false)]
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
    [Obsolete("LocomotionSystem is deprecated and will be removed in a future release. Use LocomotionMediator instead.", false)]
    public class LocomotionSystem : MonoBehaviour
    {
        LocomotionProvider m_CurrentExclusiveProvider;
        float m_TimeMadeExclusive;

        [SerializeField]
        [Tooltip("The timeout (in seconds) for exclusive access to the XR Origin.")]
        float m_Timeout = 10f;

        /// <summary>
        /// (Deprecated) The timeout (in seconds) for exclusive access to the XR Origin.
        /// </summary>
        public float timeout
        {
            get => m_Timeout;
            set => m_Timeout = value;
        }

        [SerializeField, FormerlySerializedAs("m_XRRig")]
        [Tooltip("The XR Origin object to provide access control to.")]
        XROrigin m_XROrigin;

        /// <summary>
        /// (Deprecated)The XR Origin object to provide access control to.
        /// </summary>
        public XROrigin xrOrigin
        {
            get => m_XROrigin;
            set => m_XROrigin = value;
        }

        /// <summary>
        /// (Deprecated) (Read Only) If this value is true, the XR Origin's position should not be modified until this false.
        /// </summary>
        public bool busy => m_CurrentExclusiveProvider != null;

        /// <summary>
        /// (Deprecated) The XR Rig object to provide access control to.
        /// </summary>
        [Obsolete("xrRig is marked for deprecation and will be removed in a future version. Please use xrOrigin instead.", true)]
        public XRRig xrRig
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Read Only) If this value is true, the XR Origin's position should not be modified until this false.
        /// </summary>
        /// <remarks>
        /// <c>Busy</c> has been deprecated. Use <see cref="busy"/> instead.
        /// </remarks>
#pragma warning disable IDE1006 // Naming Styles
        [Obsolete("Busy has been deprecated. Use busy instead. (UnityUpgradable) -> busy", true)]
        public bool Busy => default;
#pragma warning restore IDE1006

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_XROrigin == null)
            {
                m_XROrigin = GetComponentInParent<XROrigin>();
                if (m_XROrigin == null)
                    ComponentLocatorUtility<XROrigin>.TryFindComponent(out m_XROrigin);
            }

            if (ComponentLocatorUtility<LocomotionMediator>.TryFindComponent(out _))
                Debug.LogWarning("This scene contains both a Locomotion System and a Locomotion Mediator, which may result in unexpected locomotion behavior. It is recommended to use the Locomotion Mediator.", this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            if (m_CurrentExclusiveProvider != null && Time.time > m_TimeMadeExclusive + m_Timeout)
            {
                ResetExclusivity();
            }
        }

        /// <summary>
        /// Attempt to "lock" access to the XR Origin for the <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">The locomotion provider that is requesting access.</param>
        /// <returns>Returns a <see cref="RequestResult"/> that reflects the status of the request.</returns>
        public RequestResult RequestExclusiveOperation(LocomotionProvider provider)
        {
            if (provider == null)
                return RequestResult.Error;

            if (m_CurrentExclusiveProvider == null)
            {
                m_CurrentExclusiveProvider = provider;
                m_TimeMadeExclusive = Time.time;
                return RequestResult.Success;
            }

            return m_CurrentExclusiveProvider != provider ? RequestResult.Busy : RequestResult.Error;
        }

        void ResetExclusivity()
        {
            m_CurrentExclusiveProvider = null;
            m_TimeMadeExclusive = 0f;
        }

        /// <summary>
        /// Informs the <see cref="LocomotionSystem"/> that exclusive access to the XR Origin is no longer required.
        /// </summary>
        /// <param name="provider">The locomotion provider that is relinquishing access.</param>
        /// <returns>Returns a <see cref="RequestResult"/> that reflects the status of the request.</returns>
        public RequestResult FinishExclusiveOperation(LocomotionProvider provider)
        {
            if (provider == null || m_CurrentExclusiveProvider == null)
                return RequestResult.Error;

            if (m_CurrentExclusiveProvider == provider)
            {
                ResetExclusivity();
                return RequestResult.Success;
            }

            return RequestResult.Error;
        }
    }
}
