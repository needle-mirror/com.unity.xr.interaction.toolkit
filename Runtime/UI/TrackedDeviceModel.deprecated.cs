using System;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    public partial struct TrackedDeviceModel
    {
        /// <summary>
        /// The maximum distance to ray cast to check for UI.
        /// </summary>
        /// <remarks>
        /// <c>maxRaycastDistance</c> has been deprecated. Its value was unused, calling this property is unnecessary and should be removed.
        /// </remarks>
        [Obsolete("maxRaycastDistance has been deprecated. Its value was unused, calling this property is unnecessary and should be removed.", true)]
        public float maxRaycastDistance
        {
            get => default;
            set => _ = value;
        }
    }
}
