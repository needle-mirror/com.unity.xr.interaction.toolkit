---
uid: xri-samples-world-space-ui
---
# World Space UI

The World Space UI sample provides a demo scene and prefabs that are configured to provide world space UI support using both UI Toolkit and uGUI in conjunction with XR Interaction Toolkit.

For an overview of the world space UI Toolkit support, please view the [XRI manual page](xref:xri-ui-world-space-ui-toolkit-support).

> [!NOTE]
> World-Space support for UI Toolkit is only available in Unity 6.2 and later. For more information, please see the [Unity manual page](https://docs.unity3d.com/6000.2/Documentation/Manual/ui-systems/world-space-ui.html).

This sample is installed into the default location for package samples, in the `Assets\Samples\XR Interaction Toolkit\[version]\World Space UI` folder. You can move these Assets to a different location.

|**Asset**|**Description**|
|---|---|
|**`Editor\`**|Custom editor scripts and project validation scripts.|
|**`Materials\`**|Materials used for the prefabs.|
|**`Models\`**|Models used for the prefabs.|
|**`Prefabs\`**|uGUI and UI Toolkit configuration prefabs.|
|**`Scripts\`**|Contains scripts for the demo scene.|
|**`Styles\`**|Styles used for UI Toolkit documents.|
|**`UI Toolkit Documents\`**|UI Toolkit documents used in the demo scene and prefabs.|
|**`UI Toolkit Panel Settings\`**|UI Toolkit panel settings used in the demo scene and prefabs.|
|**`DemoScene`**|Scene that demonstrates world space UI using both UI Toolkit and uGUI.<br />See [Demo scene](#demo-scene) below.|

## Prerequisites and setup

In order for this sample to function properly, a few additional packages and samples are required. Install these by clicking **Fix** in **Edit** &gt; **Project Settings** &gt; **XR Plug-in Management** &gt; **Project Validation** or by using the **Window** &gt; **Package Manager** window.
  * [Starter Assets](xref:xri-samples-starter-assets) sample - imported from Package Manager under XR Interaction Toolkit in the Samples area
  * [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample - imported from Package Manager under XR Interaction Toolkit in the Samples area
  * [UIElements](https://docs.unity3d.com/Manual/UIElements.html) module - The UIElements module must be enabled to use the required UI Toolkit features. This can be enabled in **Package Manager** by navigating to the **Built-in** section, click on **UIElements** in the list, and then clicking the **Enable** button in the information pane.
  * TextMesh Pro - TMP Essential Resources must be installed. These can be imported by navigating to **Window** &gt; **TextMeshPro** &gt; **Import TMP Essential Resources**.

## Demo scene

The `DemoScene` contains a UI Toolkit station and a uGUI station which demonstrate world space UI configuration. Each station contains a single modal button, a scrollview, and a set of UI controls that consist of a slider, a dropdown, buttons, and toggles. The scene includes the `XR Origin Hands (XR Rig)` prefab from the [Hands Interaction Demo](xref:xri-samples-hands-interaction-demo) sample, enabling hand tracking support for UI interaction via poke and pinch gestures.

### UI Toolkit Example

The UI Toolkit station contains prefabs for the `UI Toolkit Scrollview`, the `UI Toolkit Grab UI`, which contains the UI controls, and a `UI Toolkit Button With Event`. The UI documents, which layout each of these world space UI objects, can be found in the `UI Toolkit Documents` folder in the sample. There is a single style sheet called `XRIStyleSheet` which defines the XRI style to the UI documents that leverage it.

Additionally, the `UI Toolkit Button With Event` prefab contains the `Button Event Sample` script which demonstrates how to bind to a UI Toolkit button click event.

### uGUI Example

The uGUI station is a duplicate of the `UI Sample` prefab that can be found in the XRI Starter Assets sample. The style and layout for uGUI is primarily done through the Hierarchy and Inspector in the Editor.

## Prefabs

|**Prefab**|**Description**|
|---|---|
|**`UGUI Grab UI`**|uGUI prefab which can be grabbed and repositioned.|
|**`UI Toolkit Button With Event`**|UI Toolkit prefab that contains a single modal button.|
|**`UI Toolkit Grab UI`**|UI Toolkit prefab which can be grabbed and repositioned.|
|**`UI Toolkit Scrollview`**|UI Toolkit prefab that contains a scrollview.|

## Scripts

The following script is included to support the prefabs and `DemoScene`.

|**Script**|**Description**|
|---|---|
|**`ButtonEventSample`**|Sample script that demonstrates how to bind to a UI Toolkit button click event.|

## UI Toolkit Panel Settings

The UI Toolkit Panel Settings contain two panel settings assets which are used in the demo scene. The difference between these two panel settings is the way the UI Documents will handle collider creation. The `WorldSpacePanel` asset will expect a box collider on the UI document GameObject to use for UI Toolkit functionality, while the `WorldSpacePanel_Auto` settings will automatically generate a box collider for use based on the document size.

> [!NOTE]
> When a parent interactable (such as `XRGrabInteractable`) has an empty **Colliders** list, it automatically discovers colliders on its children during `Awake` using `GetComponentsInChildren`. If a child `UIDocument` has a collider, the parent interactable can mistakenly claim that collider, resulting in unwanted behavior. To avoid this, manually assign the parent interactable's colliders in the Inspector so the automatic discovery is skipped.

### Dynamic collider support

The `WorldSpacePanel_Auto` panel settings use **Match 3-D bounding box** collider update mode, which creates a collider dynamically during `LateUpdate`. When the [XR UI Toolkit Manager](xref:xri-xr-ui-toolkit-manager) is present in the scene, it monitors for dynamically created colliders and registers them with the interactable. Poke and ray interaction work with dynamically created panel colliders without additional setup.

The `WorldSpacePanel` panel settings use **Keep existing colliders**, which requires a collider to be manually added to the UI Document GameObject. Since the collider is present before `Awake`, no dynamic registration is needed.

> [!NOTE]
> The `UIDocument` component must be present on the GameObject before the interactable registers with the Interaction Manager. Adding a `UIDocument` after registration will not be detected by the manager.

For more details on dynamic collider registration and collider update modes, refer to the [UI Toolkit support page](xref:xri-ui-world-space-ui-toolkit-support).
