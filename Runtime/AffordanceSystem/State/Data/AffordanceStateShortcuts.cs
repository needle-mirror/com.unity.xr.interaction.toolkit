namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State
{
    /// <summary>
    /// Affordance state shortcuts to facilitate the use of affordance state indices in an enum-like way.
    /// </summary>
    public static class AffordanceStateShortcuts
    {
        /// <summary>
        /// Disabled state index.
        /// </summary>
        public const byte disabled = 0;

        /// <summary>
        /// Default disabled affordance state data.
        /// </summary>
        public static AffordanceStateData disabledState { get; } = new AffordanceStateData(disabled, 1f);

        /// <summary>
        /// Idle State index.
        /// </summary>
        public const byte idle = 1;

        /// <summary>
        /// Default idle affordance state data.
        /// </summary>
        public static AffordanceStateData idleState { get; } = new AffordanceStateData(idle, 1f);

        /// <summary>
        /// Hovered state index.
        /// </summary>
        public const byte hovered = 2;

        /// <summary>
        /// Default hovered state data.
        /// </summary>
        public static AffordanceStateData hoveredState { get; } = new AffordanceStateData(hovered, 0f);

        /// <summary>
        /// Hovered Priority state index.
        /// </summary>
        public const byte hoveredPriority = 3;

        /// <summary>
        /// Default hovered priority state data.
        /// </summary>
        public static AffordanceStateData hoveredPriorityState { get; } = new AffordanceStateData(hoveredPriority, 0f);

        /// <summary>
        /// Selected state index.
        /// </summary>
        public const byte selected = 4;

        /// <summary>
        /// Default selected state data.
        /// </summary>
        public static AffordanceStateData selectedState { get; } = new AffordanceStateData(selected, 1f);

        /// <summary>
        /// Activated state index.
        /// </summary>
        public const byte activated = 5;

        /// <summary>
        /// Default activated state data.
        /// </summary>
        public static AffordanceStateData activatedState { get; } = new AffordanceStateData(activated, 1f);
    }
}
