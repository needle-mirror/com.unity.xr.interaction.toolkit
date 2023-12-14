# Glossary

| Term | Meaning |
|---|---|
| **Interactor** | An object in a scene that can select or move another object in that scene. |
| **Interactable** | An object in a scene that the user can interact with (for example, grab it, press it, or throw it). |
| **Hover** | The state where an Interactor is in a valid state to interact with an object. This differs between Ray and Direct interaction.|
| **Select** | The state where an Interactor is currently interacting with an object. |
| **Interaction Manager** | A manager component that handles interaction between a set of Interactors and Interactables. |
| **Input Reader** | A serialized property that abstracts a source of input and allows components to read input from an XR device in a generic way. It supports different modes to select the method used to read input, including from a component or ScriptableObject reference that generates logical values. These are used by interactors to read input, such as a button press, to allow interaction events like select. |
| **Gesture** | Sequences of movements that translate into an action that manipulates an interactable. |
| **Haptic** | Sensory or visual stimuli that is sent to the user to give feedback for interaction. |
| **XR Origin** | Formerly XR Rig, this serves as the center of tracking space in an XR scene. |