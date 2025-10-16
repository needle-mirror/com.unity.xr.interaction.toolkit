// Object.FindFirstObjectByType<T> API added in:

// 2022.3.0f1 or newer
#if UNITY_2022_3_OR_NEWER
#define HAS_FIND_FIRST_OBJECT_BY_TYPE
#endif

// 2022.2.5f1 or newer
#if UNITY_2022_2 && !(UNITY_2022_2_0 || UNITY_2022_2_1 || UNITY_2022_2_2 || UNITY_2022_2_3 || UNITY_2022_2_4)
#define HAS_FIND_FIRST_OBJECT_BY_TYPE
#endif

// 2021.3.18f1 or newer
#if UNITY_2021_3 && !(UNITY_2021_3_0 || UNITY_2021_3_1 || UNITY_2021_3_2 || UNITY_2021_3_3 || UNITY_2021_3_4 || UNITY_2021_3_5 || UNITY_2021_3_6 || UNITY_2021_3_7 || UNITY_2021_3_8 || UNITY_2021_3_9 || UNITY_2021_3_10 || UNITY_2021_3_11 || UNITY_2021_3_12 || UNITY_2021_3_13 || UNITY_2021_3_14 || UNITY_2021_3_15 || UNITY_2021_3_16 || UNITY_2021_3_17)
#define HAS_FIND_FIRST_OBJECT_BY_TYPE
#endif

// 2020.3.45f1 or newer (48 is the final 2020.3 patch version)
#if UNITY_2020_3 && (UNITY_2020_3_45 || UNITY_2020_3_46 || UNITY_2020_3_47 || UNITY_2020_3_48)
#define HAS_FIND_FIRST_OBJECT_BY_TYPE
#endif

using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility methods for locating component instances.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    static class ComponentLocatorUtility<T> where T : Component
    {
        /// <summary>
        /// Cached reference to a found component of type <see cref="T"/>.
        /// </summary>
        static T s_ComponentCache;

        /// <summary>
        /// Cached reference to a found component of type <see cref="T"/>.
        /// </summary>
        internal static T componentCache => s_ComponentCache;

        /// <summary>
        /// Last frame that <see cref="Find"/> was called.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType -- The frame the find was attempted is specific to each T component type
        static int s_LastTryFindFrame = -1;

        static SceneLoadedCallback s_SceneLoadedCallback;

        class SceneLoadedCallback
        {
            public bool createComponent { get; set; }

            public bool dontDestroyOnLoad { get; set; }

            public HashSet<Scene> ignoredScenes { get; } = new HashSet<Scene>();

            public List<Action<T>> callbacks { get; } = new List<Action<T>>();

            public bool subscribed { get; private set; }

            public void SubscribeSceneLoaded()
            {
                if (subscribed)
                    return;

                SceneManager.sceneLoaded += OnSceneLoaded;
                subscribed = true;
            }

            void UnsubscribeSceneLoaded()
            {
                if (!subscribed)
                    return;

                SceneManager.sceneLoaded -= OnSceneLoaded;
                subscribed = false;
            }

            public void Finish(T component)
            {
                ignoredScenes.Clear();
                UnsubscribeSceneLoaded();

                if (createComponent && component is null)
                    component = CreateComponent(dontDestroyOnLoad);

                createComponent = false;
                dontDestroyOnLoad = false;

                InvokeCallbacks(component);
            }

            void InvokeCallbacks(T component)
            {
                if (callbacks.Count > 0)
                {
                    foreach (var callback in callbacks)
                    {
                        callback?.Invoke(component);
                    }

                    callbacks.Clear();
                }
            }

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (s_ComponentCache != null)
                {
                    Finish(s_ComponentCache);
                    return;
                }

                // Skip if we have already searched this scene, where the scene is not yet isLoaded but does have root GameObjects.
                // This can happen during the first frame for the main scene or when loading a scene that has a component that triggers a search.
                if (!ignoredScenes.Contains(scene) && TryFindComponentInScene(scene, out s_ComponentCache))
                {
                    // Found
                    Finish(s_ComponentCache);
                    return;
                }

                // Unity will not set isLoaded true until right before the SceneManager.sceneLoaded event is invoked
                // for that scene. Unity also invokes them in order, so if the last scene is loaded, we know we have
                // no more scenes to wait on.
                var allScenesLoaded = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded;
                if (allScenesLoaded)
                {
                    // Not found and this was the last scene we were waiting on to finish loading
                    Finish(null);
                }
            }
        }

        static bool UnsuccessfulFindWasPerformedThisFrame()
        {
            // If the cached component is a destroyed Object, we should always retry finding the component this frame
            // even if we had previously already searched.
            // In the situation where the cached component was destroyed and replaced with a new component in the scene,
            // we should allow that new component to be found with a new attempt.
            // If the component was not successfully found during a Find, it will be true null.
            return s_ComponentCache is null && s_LastTryFindFrame == Time.frameCount;
        }

        /// <summary>
        /// Set the cached reference to the component returned by <see cref="componentCache"/>.
        /// </summary>
        /// <param name="component">The component to make as the default.</param>
        internal static void SetComponentCache(T component)
        {
            s_ComponentCache = component;

            // Notify if there were callers waiting on a scene load since the component has been found.
            if (s_SceneLoadedCallback != null && s_SceneLoadedCallback.subscribed && s_ComponentCache != null)
                s_SceneLoadedCallback.Finish(s_ComponentCache);
        }

        /// <summary>
        /// Find or create a new GameObject with component <typeparamref name="T"/>.
        /// </summary>
        /// <param name="dontDestroyOnLoad">Whether the component created if not found should be set as <c>Object.DontDestroyOnLoad</c>. Does nothing if the component already exists.</param>
        /// <returns>Returns the found or created component.</returns>
        /// <remarks>
        /// Does not include inactive GameObjects when finding the component, but if a component was previously created
        /// as a direct result of this class, it will return that component even if the GameObject is now inactive.
        /// </remarks>
        public static T FindOrCreateComponent(bool dontDestroyOnLoad = false)
        {
            if (s_ComponentCache == null)
            {
                s_ComponentCache = Find();

                if (s_ComponentCache == null)
                    SetComponentCache(CreateComponent(dontDestroyOnLoad));
            }

            return s_ComponentCache;
        }

        /// <summary>
        /// Find a component <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Returns the found component, or <see langword="null"/> if one could not be found.</returns>
        /// <remarks>
        /// Does not include inactive GameObjects when finding the component, but if a component was previously created
        /// as a direct result of this class, it will return that component even if the GameObject is now inactive.
        /// </remarks>
        public static T FindComponent()
        {
            TryFindComponent(out var component);
            return component;
        }

        /// <summary>
        /// Find a component <typeparamref name="T"/>.
        /// </summary>
        /// <param name="component">When this method returns, contains the found component, or <see langword="null"/> if one could not be found.</param>
        /// <returns>Returns <see langword="true"/> if the component exists, otherwise returns <see langword="false"/>.</returns>
        /// <remarks>
        /// Does not include inactive GameObjects when finding the component, but if a component was previously created
        /// as a direct result of this class, it will return that component even if the GameObject is now inactive.
        /// </remarks>
        /// <see cref="FindOrCreateComponent"/>
        public static bool TryFindComponent(out T component)
        {
            if (s_ComponentCache != null)
            {
                component = s_ComponentCache;
                return true;
            }

            s_ComponentCache = Find();
            component = s_ComponentCache;
            return component != null;
        }

        /// <summary>
        /// Find a component <typeparamref name="T"/>.
        /// </summary>
        /// <param name="component">When this method returns, contains the found component, or <see langword="null"/> if one could not be found.</param>
        /// <param name="limitTryFindPerFrame">If <see langword="true"/>, this method will only perform <see cref="Find"/> if it has not already been unsuccessfully called this frame.</param>
        /// <returns>Returns <see langword="true"/> if the component exists, otherwise returns <see langword="false"/>.</returns>
        /// <remarks>This function will return a cached component from a previous search regardless if <see cref="limitTryFindPerFrame"/> is <see langword="true"/>.</remarks>
        public static bool TryFindComponent(out T component, bool limitTryFindPerFrame)
        {
            // If a search for this component has already been unsuccessfully performed this frame, don't search again.
            if (limitTryFindPerFrame && UnsuccessfulFindWasPerformedThisFrame())
            {
                component = null;
                return false;
            }
            return TryFindComponent(out component);
        }

        /// <summary>
        /// Find a component <typeparamref name="T"/>. This method is aware of scenes currently being loaded and will wait to search
        /// those scenes until they finish loading.
        /// </summary>
        /// <param name="callback">The callback method that you want to invoke to get the result of the find operation.
        /// The argument to the callback can be <see langword="null"/> if not found and a component was not created.</param>
        /// <param name="createComponent">Whether to create the component if not found.</param>
        /// <param name="dontDestroyOnLoad">Whether the component created if not found should be set as <c>Object.DontDestroyOnLoad</c>. Does nothing if the component already exists.</param>
        /// <param name="limitTryFindPerFrame">If <see langword="true"/>, this method will only perform <see cref="Find"/> if it has not already been unsuccessfully called this frame.</param>
        /// <remarks>
        /// Does not include inactive GameObjects when finding the component, but if a component was previously created
        /// as a direct result of this class, it will return that component even if the GameObject is now inactive.
        /// <br />
        /// This function will return a cached component from a previous search regardless if <see cref="limitTryFindPerFrame"/> is <see langword="true"/>.
        /// <br />
        /// The callback to return the result of the find operation may not be invoked immediately
        /// if the component couldn't be found and there is a scene currently being loaded. A <see langword="null"/> result will not be
        /// returned until all scenes have finished loading.
        /// </remarks>
        public static void FindComponentDeferred(Action<T> callback, bool createComponent = false, bool dontDestroyOnLoad = false, bool limitTryFindPerFrame = true)
        {
            if (s_ComponentCache != null)
            {
                // Notify if there were callers waiting on a scene load since the component has been found.
                if (s_SceneLoadedCallback != null && s_SceneLoadedCallback.subscribed)
                    s_SceneLoadedCallback.Finish(s_ComponentCache);

                callback?.Invoke(s_ComponentCache);
                return;
            }

            var findCalled = false;
            if (!limitTryFindPerFrame || !UnsuccessfulFindWasPerformedThisFrame())
            {
                findCalled = true;
                s_ComponentCache = Find();
                if (s_ComponentCache != null)
                {
                    // Notify if there were callers waiting on a scene load since the component has been found.
                    if (s_SceneLoadedCallback != null && s_SceneLoadedCallback.subscribed)
                        s_SceneLoadedCallback.Finish(s_ComponentCache);

                    callback?.Invoke(s_ComponentCache);
                    return;
                }
            }

            // Iterate over the scenes to determine if any are still being loaded. If so, subscribe to the SceneManager.sceneLoaded
            // and then attempt to find the component in any of these scenes once loaded.
            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    continue;

                if (scene.isLoaded)
                    continue;

                s_SceneLoadedCallback ??= new SceneLoadedCallback();

                // The active scene upon entering Play mode is not yet marked as loaded, yet the GameObjects
                // are already available. The Find() call above would have found the component if it existed
                // in this scene, so mark it as ignored to avoid searching it again once the sceneLoaded event
                // is invoked.
                if (findCalled && scene.rootCount > 0)
                    s_SceneLoadedCallback.ignoredScenes.Add(scene);
                else
                    s_SceneLoadedCallback.SubscribeSceneLoaded();
            }

            // This helps to ensure we never create the component when all scenes are unloaded
            // when exiting Play mode and the scene is being unloaded. If a component was created
            // during `OnDestroy` because of a scene being unloaded from exiting Play mode, it would lead to
            // errors logged about extra GameObjects.
            // "Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)"
            // As such, this means that the default component created by this script (e.g., the XR Interaction Manager)
            // will wait to be created until the `SceneManager.sceneLoaded` callback instead of during `OnEnable`
            // when the component is trying to be found.
            if (s_SceneLoadedCallback != null && !s_SceneLoadedCallback.subscribed)
            {
                var allScenesLoaded = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded;
                if (!allScenesLoaded)
                    s_SceneLoadedCallback.SubscribeSceneLoaded();
            }

            if (s_SceneLoadedCallback != null && s_SceneLoadedCallback.subscribed)
            {
                // Still waiting on scene(s) to load
                if (callback != null)
                    s_SceneLoadedCallback.callbacks.Add(callback);

                s_SceneLoadedCallback.createComponent |= createComponent;
                s_SceneLoadedCallback.dontDestroyOnLoad |= dontDestroyOnLoad;
            }
            else if (createComponent)
            {
                // We aren't waiting on any scene to load, create the component now
                var component = CreateComponent(dontDestroyOnLoad);
                callback?.Invoke(component);
            }
            else
            {
                // Not found and no scenes to wait on
                callback?.Invoke(null);
            }
        }

        static T CreateComponent(bool dontDestroyOnLoad)
        {
            s_ComponentCache = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();

            if (dontDestroyOnLoad)
                Object.DontDestroyOnLoad(s_ComponentCache);

            return s_ComponentCache;
        }

        static T Find()
        {
            s_LastTryFindFrame = Time.frameCount;

#if HAS_FIND_FIRST_OBJECT_BY_TYPE
            // Preferred API
            // Sorting by ID since this utility is often used to find a component of which there is a single instance,
            // so the penalty for sorting will be minimal but with the benefit of having consistent results when
            // there are multiple components to choose from.
            var objectsByType = Object.FindObjectsByType(typeof(T), FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
#else
            // Fallback API
            var objectsByType = Object.FindObjectsOfType<T>(includeInactive: false);
#endif

            if (objectsByType.Length == 0)
                return null;

            if (objectsByType.Length > 1)
            {
                // In the case of multiple components, prioritize the first that is enabled
                for (var i = 0; i < objectsByType.Length; ++i)
                {
                    var obj = objectsByType[i];
                    if (obj is Behaviour behavior && behavior.enabled)
                        return (T)obj;
                }
            }

            return (T)objectsByType[0];
        }

        static bool TryFindComponentInScene(Scene scene, out T component, bool includeInactive = false)
        {
            component = null;
            var foundComponent = false;
            if (scene.rootCount > 0)
            {
                using (ListPool<GameObject>.Get(out var scratchGameObjects))
                {
                    scene.GetRootGameObjects(scratchGameObjects);
                    foreach (var rootGameObject in scratchGameObjects)
                    {
                        if (!includeInactive && !rootGameObject.activeSelf)
                            continue;

                        var childComponent = rootGameObject.GetComponentInChildren<T>(includeInactive);
                        if (childComponent != null)
                        {
                            if (includeInactive || childComponent.gameObject.activeInHierarchy)
                            {
                                if (childComponent is Behaviour behavior && behavior.enabled)
                                {
                                    component = childComponent;
                                    return true;
                                }

                                // Continue searching to prioritize the first that is enabled,
                                // otherwise we will return the first instance discovered.
                                if (!foundComponent)
                                {
                                    component = childComponent;
                                    foundComponent = true;
                                }
                            }
                        }
                    }
                }
            }

            return foundComponent;
        }
    }
}
