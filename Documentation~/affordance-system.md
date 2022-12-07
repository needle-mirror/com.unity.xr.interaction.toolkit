# Affordance system

The XR Interaction Toolkit package provides an affordance system which enables users to create visual and auditory responses to [interaction states](architecture.md#states).

The [XR Interactable Affordance State Provider](xr-interactable-affordance-state-provider.md) connects to an interactable component to determine new affordance states, which then power affordance receivers to animate tweens using affordance themes.

## Affordance receivers

Affordance receivers are components that animate tweens using a referenced theme, which can be a ScriptableObject so that it can be reused across multiple interactable components.

There are various affordance receiver components for different primitive data types, such as `Color` or `Vector3`.

The theme contains a list of affordance states and the values that should be blended during interactions. The Audio Affordance Theme contains Audio Clip references for when the state is either entered (**State Entered**) or exited (**State Exited**), and Unity will play them using `AudioSource.PlayOneShot`. The other Affordance Theme types (Color, Float, Vector2, Vector3, and Vector4) contains that typed value that is interpolated between two values: the start value (**Animation State Start Value**) and the target value (**Animation State End Value**).

The theme assets are created by using the **Assets** &gt; **Create** &gt; **Affordance Theme** menu.
