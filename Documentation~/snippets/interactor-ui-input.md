#### UI input settings {#ui-input}

The UI input settings specify which controller inputs trigger press and scroll actions. These options are [Input Readers](xref:xri-input-readers) that you can configure to use a reference to a binding declared in an Input Action asset or a direct binding of an Input Action. You can also customize or synthesize the inputs by implementing the [`IXRInputValueReader`](xref:UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.IXRInputValueReader`1) interface or by setting the input property value manually at runtime.

* **UI Press Input**: The input for UI presses. This input has two components: a Boolean value, and a range value that triggers the press when the input exceeds a threshold. (The threshold can be set in the Input Action asset or the direct binding.)

* **UI Scroll Input**: The input for panning a scroll view. This input is a 2D axis, such as a joystick. The standard input for UI scrolling is the primary 2D axis of the controller, which is a thumbstick. Because these inputs are also used for other interactions, such as locomotion and object manipulation, you should carefully test your scenes to make sure to avoid conflicts or unintentional interactions. You can set the **Block Interactions With Screen Space UI** option to true to avoid some conflicts.
