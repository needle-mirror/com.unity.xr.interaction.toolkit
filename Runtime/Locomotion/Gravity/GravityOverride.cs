namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity
{
    /// <summary>
    /// Locks the gravity for the given provider.
    /// </summary>
    /// <seealso cref="IGravityController"/>
    public enum GravityOverride
    {
        /// <summary>
        /// Overrides the gravity provider to not use gravity.
        /// </summary>
        ForcedOff,

        /// <summary>
        /// Overrides the gravity provider to use gravity.
        /// </summary>
        ForcedOn,
    }
}
