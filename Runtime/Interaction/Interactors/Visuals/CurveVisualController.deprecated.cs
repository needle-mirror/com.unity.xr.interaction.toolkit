using System;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
    public partial class LineProperties
    {
        /// <summary>
        /// (Deprecated) Width of the line at the start.
        /// </summary>
        [Obsolete("starWidth has been renamed to startWidth. (UnityUpgradable) -> startWidth", true)]
        public float starWidth
        {
            get => default;
            set => _ = value;
        }
    }
}
