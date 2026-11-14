using System;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    public partial class XRRayInteractor
    {
        const string k_RotateAnchorDeprecated = "RotateAnchor has been renamed in version 3.0.0. Use RotateAttachTransform instead.";
        const string k_TranslateAnchorDeprecated = "TranslateAnchor has been renamed in version 3.0.0. Use TranslateAttachTransform instead.";

        /// <summary>
        /// (Deprecated) Sets how Attach Transform rotation is controlled.
        /// </summary>
        /// <seealso cref="anchorRotationMode"/>
        [Obsolete("AnchorRotationMode has been deprecated in version 3.0.0. Use RotateMode instead.", true)]
        public enum AnchorRotationMode
        {
            /// <summary>
            /// The Attach Transform rotates over time while rotation input is active.
            /// </summary>
            RotateOverTime,

            /// <summary>
            /// The Attach Transform rotates to match the direction of the 2-dimensional rotation input.
            /// </summary>
            MatchDirection,
        }

        /// <summary>
        /// (Deprecated) Allows the user to move the Attach Transform using the thumbstick.
        /// </summary>
        /// <seealso cref="rotateSpeed"/>
        /// <seealso cref="translateSpeed"/>
        /// <seealso cref="anchorRotateReferenceFrame"/>
        /// <seealso cref="anchorRotationMode"/>
        [Obsolete("allowAnchorControl has been renamed in version 3.0.0. Use manipulateAttachTransform instead. (UnityUpgradable) -> manipulateAttachTransform", true)]
        public bool allowAnchorControl
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The optional reference frame to define the up axis when rotating the attach anchor point.
        /// When not set, rotates about the local up axis of the attach transform.
        /// </summary>
        /// <seealso cref="manipulateAttachTransform"/>
        /// <seealso cref="RotateAnchor(Transform, float)"/>
        /// <seealso cref="RotateAnchor(Transform, Vector2, Quaternion)"/>
        [Obsolete("anchorRotateReferenceFrame has been renamed in version 3.0.0. Use rotateReferenceFrame instead. (UnityUpgradable) -> rotateReferenceFrame", true)]
        public Transform anchorRotateReferenceFrame
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Gets or sets how the attach rotation manipulation is controlled.
        /// </summary>
        /// <seealso cref="manipulateAttachTransform"/>
        /// <seealso cref="AnchorRotationMode"/>
        [Obsolete("anchorRotationMode has been deprecated in version 3.0.0. Use rotateMode instead.", true)]
        public AnchorRotationMode anchorRotationMode
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        [Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use uiPressInput to read button input instead.", true)]
        protected override bool isUISelectActive => default;

        /// <summary>
        /// Rotates the attach anchor for this interactor. This can be useful to rotate a held object.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The rotation amount.</param>
        [Obsolete(k_RotateAnchorDeprecated, true)]
        protected virtual void RotateAnchor(Transform anchor, float directionAmount)
        {
            throw new NotSupportedException(k_RotateAnchorDeprecated);
        }

        /// <summary>
        /// Rotates the attach anchor for this interactor to match a given direction. This can be useful to compute a direction angle for teleportation.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="direction">The directional input.</param>
        /// <param name="referenceRotation">The reference rotation to define the up axis for rotation.</param>
        [Obsolete(k_RotateAnchorDeprecated, true)]
        protected virtual void RotateAnchor(Transform anchor, Vector2 direction, Quaternion referenceRotation)
        {
            throw new NotSupportedException(k_RotateAnchorDeprecated);
        }

        /// <summary>
        /// Translates the attach anchor for this interactor. This can be useful to move a held object closer or further away from the interactor.
        /// </summary>
        /// <param name="rayOrigin">The starting position and direction of any ray casts.</param>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The translation amount.</param>
        [Obsolete(k_TranslateAnchorDeprecated, true)]
        protected virtual void TranslateAnchor(Transform rayOrigin, Transform anchor, float directionAmount)
        {
            throw new NotSupportedException(k_TranslateAnchorDeprecated);
        }
    }
}
