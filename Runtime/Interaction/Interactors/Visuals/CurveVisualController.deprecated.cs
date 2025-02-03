using System;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
    public partial class LineProperties
    {
        /// <summary>
        /// (Deprecated) Width of the line at the start.
        /// </summary>
        [Obsolete("starWidth has been renamed to startWidth. (UnityUpgradable) -> startWidth")]
        public float starWidth
        {
            get => m_StartWidth;
            set => m_StartWidth = value;
        }
    }
}
