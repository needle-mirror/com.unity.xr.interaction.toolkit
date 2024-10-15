using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// (Deprecated) Controls a <see cref="CharacterController"/> height and center position based on the camera's (HMD)
    /// position upon locomotion events of a <see cref="LocomotionProvider"/> (typically the continuous movement provider).
    /// </summary>
    /// <remarks>
    /// <see cref="CharacterControllerDriver"/> is deprecated. Instead set <see cref="XRBodyTransformer.useCharacterControllerIfExists"/>
    /// to <see langword="true"/> on the instance of <see cref="XRBodyTransformer"/> in the scene, and then, if at runtime,
    /// re-enable the Body Transformer to make the locomotion system drive the <see cref="CharacterController"/>.
    /// </remarks>
    [AddComponentMenu("XR/Locomotion/Legacy/Character Controller Driver", 11)]
    [HelpURL(XRHelpURLConstants.k_CharacterControllerDriver)]
    [Obsolete("CharacterControllerDriver is deprecated in XRI 3.0.0 and will be removed in a future release. Instead set useCharacterControllerIfExists to true on the instance of XRBodyTransformer in the scene, and then, if at runtime, re-enable the Body Transformer to make the locomotion system drive the CharacterController.", false)]
    public class CharacterControllerDriver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Locomotion Provider object to listen to.")]
        LocomotionProvider m_LocomotionProvider;
        /// <summary>
        /// (Deprecated) The <see cref="LocomotionProvider"/> object to listen to.
        /// </summary>
        public LocomotionProvider locomotionProvider
        {
            get => m_LocomotionProvider;
            set
            {
                Unsubscribe(m_LocomotionProvider);
                m_LocomotionProvider = value;
                Subscribe(m_LocomotionProvider);

                SetupCharacterController();
                UpdateCharacterController();
            }
        }

        [SerializeField]
        [Tooltip("The minimum height of the character's capsule that will be set by this behavior.")]
        float m_MinHeight;
        /// <summary>
        /// (Deprecated) The minimum height of the character's capsule that this behavior sets.
        /// </summary>
        /// <seealso cref="maxHeight"/>
        /// <seealso cref="CharacterController.height"/>
        public float minHeight
        {
            get => m_MinHeight;
            set => m_MinHeight = value;
        }

        [SerializeField]
        [Tooltip("The maximum height of the character's capsule that will be set by this behavior.")]
        float m_MaxHeight = float.PositiveInfinity;
        /// <summary>
        /// (Deprecated) The maximum height of the character's capsule that this behavior sets.
        /// </summary>
        /// <seealso cref="minHeight"/>
        /// <seealso cref="CharacterController.height"/>
        public float maxHeight
        {
            get => m_MaxHeight;
            set => m_MaxHeight = value;
        }

        XROrigin m_XROrigin;
        /// <summary>
        /// (Deprecated) (Read Only) The <see cref="XROrigin"/> used for driving the <see cref="CharacterController"/>.
        /// </summary>
        protected XROrigin xrOrigin => m_XROrigin;

        /// <summary>
        /// (Read Only) The <see cref="XRRig"/> used for driving the <see cref="CharacterController"/>.
        /// </summary>
        [Obsolete("xrRig has been deprecated. Use xrOrigin instead.", true)]
        protected XRRig xrRig => default;

        CharacterController m_CharacterController;
        /// <summary>
        /// (Deprecated) (Read Only) The <see cref="CharacterController"/> that this class drives.
        /// </summary>
        protected CharacterController characterController => m_CharacterController;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_LocomotionProvider == null)
            {
                m_LocomotionProvider = GetComponent<ContinuousMoveProviderBase>();
                if (m_LocomotionProvider == null)
                {
                    m_LocomotionProvider = ComponentLocatorUtility<ContinuousMoveProviderBase>.FindComponent();
                    if (m_LocomotionProvider == null)
                        Debug.LogWarning("Unable to drive properties of the Character Controller without the locomotion events of a Locomotion Provider." +
                            " Set Locomotion Provider or ensure a Continuous Move Provider component is in your scene.", this);
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            Subscribe(m_LocomotionProvider);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            Unsubscribe(m_LocomotionProvider);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            SetupCharacterController();
            UpdateCharacterController();
        }

        /// <summary>
        /// (Deprecated) Updates the <see cref="CharacterController.height"/> and <see cref="CharacterController.center"/>
        /// based on the camera's position.
        /// </summary>
        protected virtual void UpdateCharacterController()
        {
            if (m_XROrigin == null || m_CharacterController == null)
                return;

            var height = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);

            Vector3 center = m_XROrigin.CameraInOriginSpacePos;
            center.y = height / 2f + m_CharacterController.skinWidth;

            m_CharacterController.height = height;
            m_CharacterController.center = center;
        }

        void Subscribe(LocomotionProvider provider)
        {
            if (provider != null)
            {
                provider.beginLocomotion += OnBeginLocomotion;
                provider.endLocomotion += OnEndLocomotion;
            }
        }

        void Unsubscribe(LocomotionProvider provider)
        {
            if (provider != null)
            {
                provider.beginLocomotion -= OnBeginLocomotion;
                provider.endLocomotion -= OnEndLocomotion;
            }
        }

        void SetupCharacterController()
        {
            if (m_LocomotionProvider == null || m_LocomotionProvider.system == null)
                return;

            m_XROrigin = m_LocomotionProvider.system.xrOrigin;
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            m_CharacterController = m_XROrigin != null ? m_XROrigin.Origin.GetComponent<CharacterController>() : null;
#pragma warning restore IDE0031

            if (m_CharacterController == null && m_XROrigin != null)
            {
                Debug.LogError($"Could not get CharacterController on {m_XROrigin.Origin}, unable to drive properties." +
                    $" Ensure there is a CharacterController on the \"Rig\" GameObject of {m_XROrigin}.",
                    this);
            }
        }

        void OnBeginLocomotion(LocomotionSystem system)
        {
            UpdateCharacterController();
        }

        void OnEndLocomotion(LocomotionSystem system)
        {
            UpdateCharacterController();
        }
    }
}
