using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Helper class that provides access to a shared <see cref="ScriptableObject"/> instance created at runtime
    /// and destroys the instance when it is no longer in use.
    /// </summary>
    /// <seealso cref="ScriptableSingletonCache{T}"/>
    static class ScriptableSingletonCache
    {
        /// <summary>
        /// Data container specific to each <see cref="ScriptableObject"/> type.
        /// </summary>
        internal class Container
        {
            ScriptableObject m_Instance;
            readonly HashSet<object> m_Users = new HashSet<object>();

            public ScriptableObject GetInstance<T>(object user) where T : ScriptableObject
            {
                m_Users.Add(user);
                if (m_Instance == null)
                    m_Instance = ScriptableObject.CreateInstance<T>();

                return m_Instance;
            }

            public void ReleaseInstance(object user)
            {
                if (m_Users.Remove(user) && m_Users.Count == 0)
                    Destroy();
            }

            public void Destroy()
            {
                if (m_Instance != null)
                {
                    Object.Destroy(m_Instance);
                    m_Instance = null;
                }

                m_Users.Clear();
            }
        }

        /// <summary>
        /// Map between the <see cref="ScriptableObject"/> type and the container class which stores the instance
        /// of that type and references to all the components using it.
        /// </summary>
        static readonly Dictionary<Type, Container> s_Containers = new Dictionary<Type, Container>();

        /// <summary>
        /// Get the container for the <see cref="ScriptableObject"/> instance and its users.
        /// </summary>
        /// <param name="type">Type of <see cref="ScriptableObject"/> instance.</param>
        /// <returns>Returns the container for the <see cref="ScriptableObject"/> instance and its users.</returns>
        public static Container GetContainer(Type type)
        {
            if (!s_Containers.TryGetValue(type, out var container))
            {
                container = new Container();
                s_Containers[type] = container;
            }

            return container;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticsOnLoad()
        {
            if (s_Containers.Count == 0)
                return;

            foreach (var container in s_Containers.Values)
            {
                container.Destroy();
            }

            s_Containers.Clear();
        }
    }

    /// <summary>
    /// Helper class that provides access to a shared <see cref="ScriptableObject"/> instance created at runtime
    /// and destroys the instance when it is no longer in use.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="ScriptableObject"/> instance to provide.</typeparam>
    static class ScriptableSingletonCache<T> where T : ScriptableObject
    {
        /// <summary>
        /// Gets a singleton instance of <typeparamref name="T"/>. This creates an instance if one does not already exist.
        /// </summary>
        /// <param name="user">Object using the singleton instance. This is typically a component.</param>
        /// <returns>Returns a singleton <see cref="ScriptableObject"/> instance of type <typeparamref name="T"/>.</returns>
        public static T GetInstance(object user)
        {
            return (T)ScriptableSingletonCache.GetContainer(typeof(T)).GetInstance<T>(user);
        }

        /// <summary>
        /// Removes the given user object from access to the singleton instance of <typeparamref name="T"/>. This
        /// destroys the instance if no other objects are using it.
        /// </summary>
        /// <param name="user">Object no longer using the singleton instance. This is typically a component.</param>
        public static void ReleaseInstance(object user)
        {
            ScriptableSingletonCache.GetContainer(typeof(T)).ReleaseInstance(user);
        }
    }
}
