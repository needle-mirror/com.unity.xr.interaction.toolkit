using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility class for registering a method to get called automatically to reset statics.
    /// This class can be used by generic static classes which are unable to define their own
    /// <see cref="RuntimeInitializeOnLoadMethodAttribute"/> <c>ResetStaticsOnLoad</c> method.
    /// </summary>
    /// <seealso cref="ComponentLocatorUtility{T}"/>
    static class ResetStaticsUtility
    {
        static readonly List<Action> s_Callbacks = new List<Action>();

        /// <summary>
        /// Register a delegate method to get called when the runtime is starting up.
        /// </summary>
        /// <param name="action">The delegate method that you want to get called at each of the following startups.</param>
        public static void AddResetCallback(Action action)
        {
            if (!s_Callbacks.Contains(action))
                s_Callbacks.Add(action);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticsOnLoad()
        {
            foreach (var action in s_Callbacks)
            {
                action?.Invoke();
            }
        }
    }
}
