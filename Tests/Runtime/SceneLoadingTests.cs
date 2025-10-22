using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class SceneLoadingTests : InputTestFixture
    {
        // This scene is empty.
        const string k_Scene0Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene0.unity";

        // This scene contains a single activated GameObject "Scene1 Manager" with an XRInteractionManager component added to it.
        // - "Scene1 Manager" [XRInteractionManager]
        const string k_Scene1Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene1.unity";

        // This scene contains a single deactivated GameObject "Scene2 Manager" with an XRInteractionManager component added to it.
        // - "Scene2 Manager" (deactivated) [XRInteractionManager]
        const string k_Scene2Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene2.unity";

        // This scene contains a single activated root GameObject "Activated Root" with a single deactivated
        // child GameObject "Scene3 Manager" with an XRInteractionManager component added to it.
        // - "Activated Root"
        //   - "Scene2 Manager" (deactivated) [XRInteractionManager]
        const string k_Scene3Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene3.unity";

        // This scene contains a single activated GameObject "Scene4 EventSystem" with an XRUIInputModule component added to it.
        // - "Scene4 EventSystem" [EventSystem, XRUIInputModule]
        const string k_Scene4Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene4.unity";

        // This scene contains an activated XRInteractionManager and interactable component.
        // - "Scene5 Manager" [XRInteractionManager]
        // - "Scene5 Interactable" [XRSimpleInteractable]
        const string k_Scene5Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene5.unity";

        // This scene contains a single activated GameObject "Scene6 Interactable" with an XRSimpleInteractable component added to it.
        // - "Scene6 Interactable" [XRSimpleInteractable]
        const string k_Scene6Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene6.unity";

        // This scene contains two activated GameObjects, with an XRInteractionManager component added to each, one enabled and one disabled.
        // - "Scene7 Disabled Manager" [XRInteractionManager] (disabled)
        // - "Scene7 Enabled Manager" [XRInteractionManager]
        const string k_Scene7Path = "Packages/com.unity.xr.interaction.toolkit/Tests/Scenes/SceneLoadingTests.Scene7.unity";

        public enum LoadMethod
        {
            Sync,
            Async,
        }

        static readonly LoadMethod[] s_LoadMethods =
        {
            LoadMethod.Sync,
            LoadMethod.Async,
        };

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            TestUtilities.DisableAllInputSystemActions();
            Assume.That(SceneManager.sceneCount, Is.EqualTo(1));
            Assume.That(ComponentLocatorUtility<XRInteractionManager>.FindComponent(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(true), Is.Null);
        }

        [TearDown]
        public override void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
            base.TearDown();
        }

        [UnityTest]
        [Description("Start loading a scene and find a component in that scene from the existing scene using deferred locator method.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentInLoadingScene([ValueSource(nameof(s_LoadMethods))] LoadMethod loadMethod)
        {
#if UNITY_EDITOR
            // Begin loading scene
            var scene = BeginLoadSceneAdditive(k_Scene1Path, loadMethod, out var sceneLoad);

            // These methods can't find the component until the scene has finished loading
            Assume.That(SceneManager.sceneCount, Is.EqualTo(2));
            Assume.That(SceneManager.GetSceneAt(1).isLoaded, Is.False);
            Assume.That(ComponentLocatorUtility<XRInteractionManager>.FindComponent(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(true), Is.Null);

            // Attempt to find the component using the deferred method, which handles detecting an in-progress scene loading.
            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);
            Assert.That(callbackResult, Is.Null);

            // Finish loading scene
            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);
            Assert.That(scene.rootCount, Is.Not.EqualTo(0));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));
            Assert.That(FindFirstObjectByType<XRInteractionManager>(), Is.SameAs(manager));
            Assert.That(FindFirstObjectByType<XRInteractionManager>(true), Is.SameAs(manager));

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            // When no scenes are currently being loaded, the callback should be called immediately.
            callbackCalled = false;
            callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading a scene and find the enabled component in that scene from the existing scene using deferred locator method.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentInLoadingScenePrioritizesEnabled([ValueSource(nameof(s_LoadMethods))] LoadMethod loadMethod)
        {
#if UNITY_EDITOR
            // Begin loading scene
            var scene = BeginLoadSceneAdditive(k_Scene7Path, loadMethod, out var sceneLoad);

            // These methods can't find the component until the scene has finished loading
            Assume.That(SceneManager.sceneCount, Is.EqualTo(2));
            Assume.That(SceneManager.GetSceneAt(1).isLoaded, Is.False);
            Assume.That(ComponentLocatorUtility<XRInteractionManager>.FindComponent(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(), Is.Null);
            Assume.That(FindFirstObjectByType<XRInteractionManager>(true), Is.Null);

            // Attempt to find the component using the deferred method, which handles detecting an in-progress scene loading.
            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);
            Assert.That(callbackResult, Is.Null);

            // Finish loading scene
            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);
            Assert.That(scene.rootCount, Is.Not.EqualTo(0));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));
            Assert.That(manager.enabled, Is.True);
            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(2));
            Assert.That(FindAllObjectByType<XRInteractionManager>(), Contains.Item(manager));

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            // When no scenes are currently being loaded, the callback should be called immediately.
            callbackCalled = false;
            callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading a scene and attempt to find a component in that scene from the existing scene using deferred locator method" +
            " when it is on a deactivated root GameObject.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentInLoadingSceneExcludesDeactivatedRootGameObject([ValueSource(nameof(s_LoadMethods))] LoadMethod loadMethod)
        {
#if UNITY_EDITOR
            var scene = BeginLoadSceneAdditive(k_Scene2Path, loadMethod, out var sceneLoad);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);
            Assert.That(callbackResult, Is.Null);

            // Finish loading scene
            yield return sceneLoad;

            Assert.That(scene.IsValid(), Is.True);
            Assert.That(scene.isLoaded, Is.True);
            Assert.That(scene.rootCount, Is.Not.EqualTo(0));

            Assert.That(ComponentLocatorUtility<XRInteractionManager>.FindComponent(), Is.Null);
            Assert.That(FindFirstObjectByType<XRInteractionManager>(), Is.Null);
            var manager = FindFirstObjectByType<XRInteractionManager>(true);
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading a scene and attempt to find a component in that scene from the existing scene using deferred locator method" +
            " when it is on a deactivated child GameObject.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentInLoadingSceneExcludesDeactivatedChildGameObject([ValueSource(nameof(s_LoadMethods))] LoadMethod loadMethod)
        {
#if UNITY_EDITOR
            var scene = BeginLoadSceneAdditive(k_Scene3Path, loadMethod, out var sceneLoad);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);
            Assert.That(callbackResult, Is.Null);

            // Finish loading scene
            yield return sceneLoad;

            Assert.That(scene.IsValid(), Is.True);
            Assert.That(scene.isLoaded, Is.True);
            Assert.That(scene.rootCount, Is.Not.EqualTo(0));

            Assert.That(ComponentLocatorUtility<XRInteractionManager>.FindComponent(), Is.Null);
            Assert.That(FindFirstObjectByType<XRInteractionManager>(), Is.Null);
            var manager = FindFirstObjectByType<XRInteractionManager>(true);
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading two scenes and block the second scene from finishing and use the deferred locator method to find the component in that second scene.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentWithMultipleLoadingWhereFirstLoadFinishesFirst()
        {
#if UNITY_EDITOR
            var firstLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));
            firstLoad.allowSceneActivation = true;
            var secondLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));
            secondLoad.allowSceneActivation = false;

            var firstScene = SceneManager.GetSceneByPath(k_Scene2Path);
            var secondScene = SceneManager.GetSceneByPath(k_Scene1Path);

            yield return firstLoad;

            Assert.That(firstScene.isLoaded, Is.True);
            Assert.That(secondScene.isLoaded, Is.False);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);

            secondLoad.allowSceneActivation = true;
            yield return secondLoad;

            Assert.That(firstScene.isLoaded, Is.True);
            Assert.That(secondScene.isLoaded, Is.True);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(secondScene);
            yield return SceneManager.UnloadSceneAsync(firstScene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading two scenes and block the second scene from finishing and use the deferred locator method to find the component in that second scene" +
            " when it is forced to finish loading due to a synchronous scene load.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentWithMultipleLoadingWhereFirstLoadFinishesFirstFollowedBySyncLoad()
        {
#if UNITY_EDITOR
            var firstLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));
            firstLoad.allowSceneActivation = true;
            var secondLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));
            secondLoad.allowSceneActivation = false;

            var firstScene = SceneManager.GetSceneByPath(k_Scene2Path);
            var secondScene = SceneManager.GetSceneByPath(k_Scene1Path);

            yield return firstLoad;

            Assert.That(firstScene.isLoaded, Is.True);
            Assert.That(secondScene.isLoaded, Is.False);

            var scene3 = EditorSceneManager.LoadSceneInPlayMode(k_Scene3Path, new LoadSceneParameters(LoadSceneMode.Additive));

            Assert.That(firstScene.isLoaded, Is.True);
            Assert.That(secondScene.isLoaded, Is.False);
            Assert.That(scene3.isLoaded, Is.False);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            Assert.That(callbackCalled, Is.False);

            yield return null;

            // The sync load forces all previous async loads to finish
            Assert.That(firstScene.isLoaded, Is.True);
            Assert.That(secondScene.isLoaded, Is.True);
            Assert.That(scene3.isLoaded, Is.True);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(secondScene);
            yield return SceneManager.UnloadSceneAsync(firstScene);
            yield return SceneManager.UnloadSceneAsync(scene3);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Start loading a scene without the component, attempt to find using deferred locator method, and then begin loading an additional scene with the component.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentWithSecondSceneLoadingStartedAfterBegin()
        {
#if UNITY_EDITOR
            var firstLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));
            firstLoad.allowSceneActivation = true;

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            var secondLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));
            secondLoad.allowSceneActivation = false;

            var firstScene = SceneManager.GetSceneByPath(k_Scene2Path);
            var secondScene = SceneManager.GetSceneByPath(k_Scene1Path);

            yield return firstLoad;

            Assert.That(callbackCalled, Is.False);

            secondLoad.allowSceneActivation = true;
            yield return secondLoad;

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(secondScene);
            yield return SceneManager.UnloadSceneAsync(firstScene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Use deferred component locator method to create the component when it isn't found in the loaded scene.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentCreatesComponentIfNotFoundInLoadedScene()
        {
#if UNITY_EDITOR
            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent, createComponent: true);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.Not.Null);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return null;

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Use deferred component locator method to create the component when it isn't found in the scene that is loading.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentCreatesComponentIfNotFoundInLoadingScene()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var scene = SceneManager.GetSceneByPath(k_Scene2Path);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent, createComponent: true);

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Use deferred component locator method to create the component while another callback is waiting on a scene to be loaded.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentCanUpdateExistingSubscriptionToCreateComponent()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var scene = SceneManager.GetSceneByPath(k_Scene2Path);

            var callbackCalled1 = false;
            XRInteractionManager callbackResult1 = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent1);

            var callbackCalled2 = false;
            XRInteractionManager callbackResult2 = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent2, createComponent: true);

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled1, Is.True);
            Assert.That(callbackResult1, Is.SameAs(manager));

            Assert.That(callbackCalled2, Is.True);
            Assert.That(callbackResult2, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent1(XRInteractionManager component)
            {
                callbackCalled1 = true;
                callbackResult1 = component;
            }

            void OnFindComponent2(XRInteractionManager component)
            {
                callbackCalled2 = true;
                callbackResult2 = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Use deferred component locator method to create the component when the second find doesn't request to create.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentCanRetainExistingSubscriptionToCreateComponent()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var scene = SceneManager.GetSceneByPath(k_Scene2Path);

            var callbackCalled1 = false;
            XRInteractionManager callbackResult1 = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent1, createComponent: true);

            var callbackCalled2 = false;
            XRInteractionManager callbackResult2 = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent2);

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled1, Is.True);
            Assert.That(callbackResult1, Is.SameAs(manager));

            Assert.That(callbackCalled2, Is.True);
            Assert.That(callbackResult2, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent1(XRInteractionManager component)
            {
                callbackCalled1 = true;
                callbackResult1 = component;
            }

            void OnFindComponent2(XRInteractionManager component)
            {
                callbackCalled2 = true;
                callbackResult2 = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Use deferred component locator method to create the component while another callback is waiting on a scene to be loaded.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator LocateComponentNotifiesWaitingCallbackIfComponentCreated()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene2Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var scene = SceneManager.GetSceneByPath(k_Scene2Path);

            var callbackCalled = false;
            XRInteractionManager callbackResult = null;
            ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(OnFindComponent);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindOrCreateComponent();

            Assert.That(scene.isLoaded, Is.False);

            Assert.That(manager, Is.Not.Null);

            Assert.That(callbackCalled, Is.True);
            Assert.That(callbackResult, Is.SameAs(manager));

            callbackCalled = false;
            callbackResult = null;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            // Should not invoke the callback again when the scene it was previously waiting on to load finishes loading.
            Assert.That(callbackCalled, Is.False);
            Assert.That(callbackResult, Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);

            void OnFindComponent(XRInteractionManager component)
            {
                callbackCalled = true;
                callbackResult = component;
            }
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Interactor should allow the Starting Selected Interactable to be selected even after Start with manager still pending.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator InteractorSelectsStartingSelectedInteractableAfterManagerSceneLoaded()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));
            sceneLoad.allowSceneActivation = false;
            var scene = SceneManager.GetSceneByPath(k_Scene1Path);

            var interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();

            interactor.keepSelectedTargetValid = true;
            interactor.startingSelectedInteractable = interactable;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(interactor.IsSelecting(interactable), Is.False);
            Assert.That(interactor.interactionManager, Is.Null);

            yield return null;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(interactor.IsSelecting(interactable), Is.False);
            Assert.That(interactor.interactionManager, Is.Null);

            sceneLoad.allowSceneActivation = true;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);

            Assert.That(interactor.IsSelecting(interactable), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractor)interactor), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)interactable), Is.True);
            Assert.That(interactor.interactionManager, Is.SameAs(manager));
            Assert.That(interactable.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Components should be registered automatically after OnEnable with manager still pending.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ComponentRegisteredAfterManagerSceneLoaded()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));
            sceneLoad.allowSceneActivation = false;
            var scene = SceneManager.GetSceneByPath(k_Scene1Path);

            var group = TestUtilities.CreateInteractionGroup();
            var interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractable();
            var snapVolume = TestUtilities.CreateSnapVolume();

            group.AddGroupMember(interactor);
            snapVolume.interactable = interactable;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(group.interactionManager, Is.Null);
            Assert.That(interactor.interactionManager, Is.Null);
            Assert.That(interactable.interactionManager, Is.Null);
            Assert.That(snapVolume.interactionManager, Is.Null);

            yield return null;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(group.interactionManager, Is.Null);
            Assert.That(interactor.interactionManager, Is.Null);
            Assert.That(interactable.interactionManager, Is.Null);
            Assert.That(snapVolume.interactionManager, Is.Null);

            sceneLoad.allowSceneActivation = true;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            Assert.That(manager.IsRegistered(group), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractor)interactor), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)interactable), Is.True);
            Assert.That(manager.TryGetInteractableForCollider(snapVolume.snapCollider, out var snapInteractable), Is.True);
            Assert.That(snapInteractable, Is.SameAs(interactable));

            Assert.That(group.interactionManager, Is.SameAs(manager));
            Assert.That(interactor.interactionManager, Is.SameAs(manager));
            Assert.That(interactable.interactionManager, Is.SameAs(manager));
            Assert.That(snapVolume.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Components should be registered automatically after OnEnable with EventSystem still pending.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ComponentRegisteredAfterEventSystemSceneLoaded()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene4Path, new LoadSceneParameters(LoadSceneMode.Additive));
            sceneLoad.allowSceneActivation = false;
            var scene = SceneManager.GetSceneByPath(k_Scene4Path);

            var manager = TestUtilities.CreateInteractionManager();
            var pokeInteractor = TestUtilities.CreatePokeInteractor();
            var rayInteractor = TestUtilities.CreateRayInteractor();
            var nearFarInteractor = TestUtilities.CreateNearFarInteractor();

            pokeInteractor.enableUIInteraction = true;
            rayInteractor.enableUIInteraction = true;
            nearFarInteractor.enableUIInteraction = true;

            Assert.That(FindAllObjectByType<EventSystem>(), Has.Length.EqualTo(0));
            Assert.That(FindAllObjectByType<XRUIInputModule>(), Has.Length.EqualTo(0));

            yield return null;

            Assert.That(FindAllObjectByType<EventSystem>(), Has.Length.EqualTo(0));
            Assert.That(FindAllObjectByType<XRUIInputModule>(), Has.Length.EqualTo(0));

            sceneLoad.allowSceneActivation = true;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<EventSystem>(), Has.Length.EqualTo(1));
            Assert.That(FindAllObjectByType<XRUIInputModule>(), Has.Length.EqualTo(1));

            Assert.That(EventSystem.current, Is.Not.Null);
            var inputModule = EventSystem.current.GetComponent<XRUIInputModule>();
            Assert.That(inputModule, Is.Not.Null);
            Assert.That(inputModule.gameObject.scene, Is.EqualTo(scene));

            Assert.That(inputModule.GetTrackedDeviceModel(pokeInteractor, out var pokeModel), Is.True);
            Assert.That(inputModule.GetTrackedDeviceModel(rayInteractor, out var rayModel), Is.True);
            Assert.That(inputModule.GetTrackedDeviceModel(nearFarInteractor, out var nearFarModel), Is.True);
            Assert.That(pokeModel.interactor, Is.SameAs(pokeInteractor));
            Assert.That(rayModel.interactor, Is.SameAs(rayInteractor));
            Assert.That(nearFarModel.interactor, Is.SameAs(nearFarInteractor));

            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Manager should not be created when a scene with a manager and interactable is finished loading after main scene interactable tries to find the manager.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ComponentsRegisteredAfterManagerSceneLoaded()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene5Path, new LoadSceneParameters(LoadSceneMode.Additive));
            sceneLoad.allowSceneActivation = false;
            var scene = SceneManager.GetSceneByPath(k_Scene5Path);

            var mainInteractable = TestUtilities.CreateSimpleInteractable();

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            yield return null;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            sceneLoad.allowSceneActivation = true;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            XRSimpleInteractable sceneInteractable = null;
            foreach (var go in rootGameObjects)
            {
                if (go.TryGetComponent(out sceneInteractable))
                    break;
            }

            Assert.That(sceneInteractable, Is.Not.Null);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            Assert.That(manager.IsRegistered((IXRInteractable)mainInteractable), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)sceneInteractable), Is.True);

            Assert.That(mainInteractable.interactionManager, Is.SameAs(manager));
            Assert.That(sceneInteractable.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Manager should not be created when a scene with a manager is finished loading after main scene interactable tries to find the manager" +
            " and multiple scenes are being waited on for loading.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ComponentsRegisteredAfterSoloManagerSceneLoadedLastInSet()
        {
#if UNITY_EDITOR
            var emptyScene = EditorSceneManager.LoadSceneInPlayMode(k_Scene0Path, new LoadSceneParameters(LoadSceneMode.Additive));
            var scene = EditorSceneManager.LoadSceneInPlayMode(k_Scene1Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var mainInteractable = TestUtilities.CreateSimpleInteractable();

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            yield return null;

            Assert.That(emptyScene.isLoaded, Is.True);
            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            Assert.That(manager.IsRegistered((IXRInteractable)mainInteractable), Is.True);

            Assert.That(mainInteractable.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(emptyScene);
            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Manager should not be created when a scene with a manager is finished loading after main scene interactable tries to find the manager" +
            " and multiple scenes are being waited on for loading and that final scene has an interactable that searches for the manager too.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ComponentsRegisteredAfterMixedManagerSceneLoadedLastInSet()
        {
#if UNITY_EDITOR
            var emptyScene = EditorSceneManager.LoadSceneInPlayMode(k_Scene0Path, new LoadSceneParameters(LoadSceneMode.Additive));
            var scene = EditorSceneManager.LoadSceneInPlayMode(k_Scene5Path, new LoadSceneParameters(LoadSceneMode.Additive));

            var mainInteractable = TestUtilities.CreateSimpleInteractable();

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            yield return null;

            Assert.That(emptyScene.isLoaded, Is.True);
            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.EqualTo(scene));

            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            XRSimpleInteractable sceneInteractable = null;
            foreach (var go in rootGameObjects)
            {
                if (go.TryGetComponent(out sceneInteractable))
                    break;
            }

            Assert.That(sceneInteractable, Is.Not.Null);

            Assert.That(manager.IsRegistered((IXRInteractable)mainInteractable), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)sceneInteractable), Is.True);

            Assert.That(mainInteractable.interactionManager, Is.SameAs(manager));
            Assert.That(sceneInteractable.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(emptyScene);
            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

        [UnityTest]
        [Description("Manager should be created when a scene without a manager is finished loading after main scene interactable tries to find the manager.")]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator ManagerCreatedAfterInteractableSceneLoaded()
        {
#if UNITY_EDITOR
            var sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(k_Scene6Path, new LoadSceneParameters(LoadSceneMode.Additive));
            sceneLoad.allowSceneActivation = false;
            var scene = SceneManager.GetSceneByPath(k_Scene6Path);

            var mainInteractable = TestUtilities.CreateSimpleInteractable();

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            yield return null;

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(0));

            Assert.That(mainInteractable.interactionManager, Is.Null);

            sceneLoad.allowSceneActivation = true;

            yield return sceneLoad;

            Assert.That(scene.isLoaded, Is.True);

            Assert.That(FindAllObjectByType<XRInteractionManager>(), Has.Length.EqualTo(1));

            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            XRSimpleInteractable sceneInteractable = null;
            foreach (var go in rootGameObjects)
            {
                if (go.TryGetComponent(out sceneInteractable))
                    break;
            }

            Assert.That(sceneInteractable, Is.Not.Null);

            var manager = ComponentLocatorUtility<XRInteractionManager>.FindComponent();
            Assert.That(manager, Is.Not.Null);
            Assert.That(manager.gameObject.scene, Is.Not.EqualTo(scene));

            Assert.That(manager.IsRegistered((IXRInteractable)mainInteractable), Is.True);
            Assert.That(manager.IsRegistered((IXRInteractable)sceneInteractable), Is.True);

            Assert.That(mainInteractable.interactionManager, Is.SameAs(manager));
            Assert.That(sceneInteractable.interactionManager, Is.SameAs(manager));

            yield return SceneManager.UnloadSceneAsync(scene);
#else
            yield return null;
#endif
        }

#if UNITY_EDITOR
        static Scene BeginLoadSceneAdditive(string scenePath, LoadMethod loadMethod, out AsyncOperation sceneLoad)
        {
            // Begin loading scene
            Scene scene;
            sceneLoad = null;
            switch (loadMethod)
            {
                case LoadMethod.Sync:
                    scene = EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                    break;
                case LoadMethod.Async:
                    sceneLoad = EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                    Assume.That(sceneLoad.isDone, Is.False);
                    scene = SceneManager.GetSceneByPath(scenePath);
                    break;
                default:
                    Assert.Fail("Invalid Load Method test case.");
                    throw new InvalidOperationException();
            }

            Assume.That(scene.IsValid(), Is.True);
            Assume.That(scene.isLoaded, Is.False);
            Assume.That(scene.rootCount, Is.EqualTo(0));

            return scene;
        }
#endif

        static T FindFirstObjectByType<T>(bool includeInactive = false) where T : Object
        {
#if HAS_FIND_FIRST_OBJECT_BY_TYPE
            return Object.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#else
            return Object.FindObjectOfType<T>(includeInactive);
#endif
        }

        static T[] FindAllObjectByType<T>(bool includeInactive = false) where T : Object
        {
#if HAS_FIND_FIRST_OBJECT_BY_TYPE
            return Object.FindObjectsByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>(includeInactive);
#endif
        }
    }
}
