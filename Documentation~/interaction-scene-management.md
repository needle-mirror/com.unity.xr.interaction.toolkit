---
uid: xri-scene-management
---

## Scene management considerations

This section covers how interaction components find other components they depend on when you load and manage scenes.

### Finding a manager component

Interactors and interactables must reference the same [XR Interaction Manager](xref:xri-xr-interaction-manager) to interact with each other. You can either assign a manager to each interaction component with the **Interaction Manager** property, or rely on the components to automatically find a manager (and create one if necessary). When you don't set their **Interaction Manager** property, interactors and interactables look for a manager during their [`OnEnable`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html) method.

You can change this auto-find and auto-create behavior in the **XR Interaction Toolkit's** [Runtime Settings](xri-settings.md#runtime-settings) in your **Project Settings**.

With the default Project Settings, interactors and interactables try to find an XR Interaction Manager and create one if needed. If an asynchronous scene load is in progress (for example, another script calls [`SceneManager.LoadSceneAsync`](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html) before [`XRBaseInteractor`](xref:UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor) or [`XRBaseInteractable`](xref:UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable) run their `OnEnable`), the components wait for that load to finish before searching the new scene for an [`XRInteractionManager`](xref:UnityEngine.XR.Interaction.Toolkit.XRInteractionManager). A default manager is only created after all scene loads complete and no existing manager is found.

> [!TIP]
> If you need interaction to work before or during an asynchronous scene load, add an XR Interaction Manager component to your first scene instead of relying on auto-creation.

You might need to customize the XR Interaction Manager, for example by setting global filters in the Inspector or creating a derived script. The simplest approach is to place the manager on an active GameObject in your main scene and load other scenes additively. The first active and enabled XR Interaction Manager becomes the cached default that unassigned interactors and interactables use.

If you have multiple XR Interaction Manager components at runtime, you should manually assign the correct manager to the interaction components that you want to work together. You can use the [XR Interaction Debugger](xref:xri-debugger-window) window to find out which manager component manages which interactor and interactables. The default manager that will be used for interaction components without an assigned manager has "&lt;Default&gt;" appended to its name (only shown when more than one manager exists).

<a id="dont-destroy-on-load"></a>
### Using `DontDestroyOnLoad` to keep components through scene loading

When you load a scene non-additively (for example, with `SceneManager.LoadSceneAsync("MySceneName", LoadSceneMode.Single)`), Unity destroys all GameObjects in the current scene before loading the new one. To prevent a GameObject and its components from being destroyed, pass it to [`Object.DontDestroyOnLoad`](https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html) before the scene loads.

This approach lets you keep the same XR Interaction Manager across scene loads, so interaction continues to work without creating a new manager each time. A simple way to set this up is to add the following component to a root GameObject:

[!code-cs [dont-destroy-on-load-sample](../DocCodeSamples.Tests/DontDestroyOnLoadSample.cs)]

Interactors that enable UI Interaction search for the [XR UI Input Module](xref:xri-ui-input-module) component on the [Event System](ui-setup.md#event-system). You should typically also mark that Event System GameObject as `DontDestroyOnLoad`.

A common practice is to place the XR Interaction Manager, the XR UI Input Module, and the [XR Origin](xref:xr-core-utils-xr-origin) in your main scene and mark all of them as `DontDestroyOnLoad`. This way, other components can always find them without needing to create new instances after each scene load.
