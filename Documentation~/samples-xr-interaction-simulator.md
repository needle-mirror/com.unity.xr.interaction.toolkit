---
uid: xri-samples-xr-interaction-simulator
---
# XR Interaction Simulator

This sample is installed into the default location for package samples, in the `Assets\Samples\XR Interaction Toolkit\[version]\XR Interaction Simulator` folder. You can move these Assets to a different location.

The XR Interaction Toolkit package provides an example implementation of an XR Interaction Simulator to allow for manipulating an XR headset and a pair of controllers using mouse and keyboard input. This sample contains example bindings for use with that simulator, and a Prefab which you can add to your scene to quickly start using the simulator. Please see the [XR Interaction Simulator Overview](xr-interaction-simulator-overview.md) for more information on how to use the interaction simulator.

![xr-interaction-simulator-overall](images/xr-interaction-simulator/xr-interaction-simulator-overall.gif)

|**Asset**|**Description**|
|---|---|
|**`Editor`**|Asset folder containing some editor-specific utility classes.|
|**`Hand Expression Captures\`**|Asset folder containing captured hand poses for the simulator when simulating tracked hands.|
|**`Scripts\`**|Asset folder containing scripts for the runtime UI for the interaction simulator.|
|**`UI\`**|Asset folder containing prefabs and textures for the runtime UI for the interaction simulator.|
|**`XR Interaction Controller Controls.inputactions`**|Asset that contains actions with default bindings for use with the XR Interaction Simulator focused on controls for the simulated controllers.|
|**`XR Interaction Hand Controls.inputactions`**|Asset that contains actions with default bindings for use with the XR Interaction Simulator focused on controls for the simulated hands.|
|**`XR Interaction Simulator.prefab`**|Prefab with the XR Interaction Simulator component with references to actions configured.|
|**`XR Interaction Simulator Controls.inputactions`**|Asset that contains actions with default bindings for use with the XR Interaction Simulator focused on controls for the simulator itself.|

## Known issues
- When checking `Use XR Interaction Simulator in scenes` in the `Project Settings > XR Plug-in Management > XR Interaction Toolkit` window, there is a race-condition on some devices where an error about being unable to find the simulator prefab is erroneously displayed in the console.
