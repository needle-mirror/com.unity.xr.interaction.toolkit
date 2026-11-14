using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

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
    [Obsolete(k_CharacterControllerDriverDeprecated, true)]
    public class CharacterControllerDriver : MonoBehaviour
    {
        const string k_CharacterControllerDriverDeprecated = "CharacterControllerDriver is deprecated in XRI 3.0.0 and will be removed in a future release. Instead set useCharacterControllerIfExists to true on the instance of XRBodyTransformer in the scene, and then, if at runtime, re-enable the Body Transformer to make the locomotion system drive the CharacterController.";

        /// <summary>
        /// (Deprecated) The <see cref="LocomotionProvider"/> object to listen to.
        /// </summary>
        public LocomotionProvider locomotionProvider
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The minimum height of the character's capsule that this behavior sets.
        /// </summary>
        /// <seealso cref="maxHeight"/>
        /// <seealso cref="CharacterController.height"/>
        public float minHeight
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The maximum height of the character's capsule that this behavior sets.
        /// </summary>
        /// <seealso cref="minHeight"/>
        /// <seealso cref="CharacterController.height"/>
        public float maxHeight
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) (Read Only) The <see cref="XROrigin"/> used for driving the <see cref="CharacterController"/>.
        /// </summary>
        protected XROrigin xrOrigin => default;

        /// <summary>
        /// (Deprecated) (Read Only) The <see cref="CharacterController"/> that this class drives.
        /// </summary>
        protected CharacterController characterController => default;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            throw new NotSupportedException(k_CharacterControllerDriverDeprecated);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
        }

        /// <summary>
        /// (Deprecated) Updates the <see cref="CharacterController.height"/> and <see cref="CharacterController.center"/>
        /// based on the camera's position.
        /// </summary>
        protected virtual void UpdateCharacterController()
        {
            throw new NotSupportedException(k_CharacterControllerDriverDeprecated);
        }
    }
}
