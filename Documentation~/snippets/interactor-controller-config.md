<!-- Ray, Direct, Gaze,

## XR Controller Configuration (deprecated) {#legacy-configuration}

[!INCLUDE [interactor-controller-config](snippets/interactor-controller-config.md)]
-->

Use the **XR Controller Configuration** options when you are upgrading a Unity project that used an earlier version of the XR Interaction Toolkit package, but don't have time to immediately redo all of your input configuration. Version 3 of the toolkit introduced [Input Readers](xref:xri-input-readers) as the primary way to configure interactor input and deprecated the [XR Controller](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.6/manual/xr-controller-action-based.html) components. You can set the **Input Compatibility Mode** property to one of the following options to control how an interactor receives input:

* **Automatic**: Chooses the **Force Deprecated Input** mode if it finds an action- or device-based [XR Controller](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.6/manual/xr-controller-action-based.html) component in the GameObject hierarchy. Otherwise, it chooses the **Force Input Readers** mode.
* **Force Deprecated Input**: Always use the older input method. An action- or device-based [XR Controller](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.6/manual/xr-controller-action-based.html) component must exist in the GameObject hierarchy.
* **Force Input Readers**: Always use the input readers configured for the interactor instance. The interactor does not receive input for unconfigured input reader properties.

> [!TIP]
> You can choose different options for different interactor components as you update them to use the newer input reader system.

![Controller compatibility options](../images/xr-ray-controller-configuration.png)

| **Property** | **Description** |
| :--- | :--- |
| **Input Compatibility Mode** | Choose how an interactor instance receives input.|
| **Hide Controller On Select** | Controls whether this Interactor should hide the controller model on selection. This behavior was supported by earlier versions of the toolkit, but is deprecated and might be removed in a future version. |
