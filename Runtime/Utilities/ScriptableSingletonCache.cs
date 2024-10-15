using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Helper class that provides access to a shared <see cref="ScriptableObject"/> instance created at runtime
    /// and destroys the instance when it is no longer in use.
    /// </summary>
    /// <typeparam name="T">Type of ScriptableObject instance to provide.</typeparam>
    static class ScriptableSingletonCache<T> where T : ScriptableObject
    {
        static T s_Instance;
        static readonly Dictionary<ScriptableObject, HashSet<object>> s_UsersPerInstance = new Dictionary<ScriptableObject, HashSet<object>>();

        /// <summary>
        /// Gets a singleton instance of <typeparamref name="T"/>. This creates an instance if one does not already exist.
        /// </summary>
        /// <param name="user">Object using the singleton instance.</param>
        /// <returns>Returns a singleton ScriptableObject instance of type <typeparamref name="T"/>.</returns>
        public static T GetInstance(object user)
        {
            if (s_Instance == null)
                s_Instance = ScriptableObject.CreateInstance<T>();

            if (!s_UsersPerInstance.TryGetValue(s_Instance, out var users))
            {
                users = new HashSet<object>();
                s_UsersPerInstance.Add(s_Instance, users);
            }

            users.Add(user);
            return s_Instance;
        }

        /// <summary>
        /// Removes the given user object from access to the singleton instance of <typeparamref name="T"/>. This
        /// destroys the instance if no other objects are using it.
        /// </summary>
        /// <param name="user">Object no longer using the singleton instance.</param>
        public static void ReleaseInstance(object user)
        {
            if (s_Instance == null)
                return;

            if (!s_UsersPerInstance.TryGetValue(s_Instance, out var users))
            {
                Object.Destroy(s_Instance);
                return;
            }

            users.Remove(user);
            if (users.Count == 0)
            {
                s_UsersPerInstance.Remove(s_Instance);
                Object.Destroy(s_Instance);
            }
        }
    }
}
