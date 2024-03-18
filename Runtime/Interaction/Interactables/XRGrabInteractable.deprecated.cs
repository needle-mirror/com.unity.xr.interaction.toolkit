using System;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    public partial class XRGrabInteractable
    {
        const string k_AttachPointCompatibilityModeDeprecated = "attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.";
        const string k_GravityOnDetachDeprecated = "gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach";

        /// <summary>
        /// Controls the method used when calculating the target position of the object.
        /// </summary>
        /// <seealso cref="attachPointCompatibilityMode"/>
        [Obsolete("AttachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.", true)]
        public enum AttachPointCompatibilityMode
        {
            /// <summary>
            /// Use the default, correct method for calculating the target position of the object.
            /// </summary>
            [Obsolete("Default has been deprecated and will be removed in a future version of XRI. It is the only mode now.", true)]
            Default,

            /// <summary>
            /// Use an additional offset from the center of mass when calculating the target position of the object.
            /// Also incorporate the scale of the Interactor's Attach Transform.
            /// Marked for deprecation.
            /// This is the backwards compatible support mode for projects that accounted for the
            /// unintended difference when using XR Interaction Toolkit prior to version <c>1.0.0-pre.4</c>.
            /// To have the effective attach position be the same between all <see cref="XRBaseInteractable.MovementType"/> values, use <see cref="Default"/>.
            /// </summary>
            [Obsolete("Legacy has been deprecated and will be removed in a future version of XRI.", true)]
            Legacy,
        }

        /// <summary>
        /// Controls the method used when calculating the target position of the object.
        /// Use <see cref="AttachPointCompatibilityMode.Default"/> for consistent attach points
        /// between all <see cref="XRBaseInteractable.MovementType"/> values.
        /// Marked for deprecation, this property will be removed in a future version.
        /// </summary>
        /// <remarks>
        /// This is a backwards compatibility option in order to keep the old, incorrect method
        /// of calculating the attach point. Projects that already accounted for the difference
        /// can use the Legacy option to maintain the same attach positioning from older versions
        /// without needing to modify the Attach Transform position.
        /// </remarks>
        /// <seealso cref="AttachPointCompatibilityMode"/>
        [Obsolete(k_AttachPointCompatibilityModeDeprecated, true)]
        public AttachPointCompatibilityMode attachPointCompatibilityMode
        {
            get
            {
                Debug.LogError(k_AttachPointCompatibilityModeDeprecated, this);
                throw new NotSupportedException(k_AttachPointCompatibilityModeDeprecated);
            }
            set
            {
                _ = value;
                Debug.LogError(k_AttachPointCompatibilityModeDeprecated, this);
                throw new NotSupportedException(k_AttachPointCompatibilityModeDeprecated);
            }
        }

        /// <summary>
        /// (Deprecated) Forces this object to have gravity when released
        /// (will still use pre-grab value if this is <see langword="false"/>).
        /// </summary>
        /// <remarks>
        /// <c>gravityOnDetach</c> has been deprecated. Use <see cref="forceGravityOnDetach"/> instead.
        /// </remarks>
        [Obsolete(k_GravityOnDetachDeprecated, true)]
        public bool gravityOnDetach
        {
            get
            {
                Debug.LogError(k_GravityOnDetachDeprecated, this);
                throw new NotSupportedException(k_GravityOnDetachDeprecated);
            }
            set
            {
                _ = value;
                Debug.LogError(k_GravityOnDetachDeprecated, this);
                throw new NotSupportedException(k_GravityOnDetachDeprecated);
            }
        }
    }
}
