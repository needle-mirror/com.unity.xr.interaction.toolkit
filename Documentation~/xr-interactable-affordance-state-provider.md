# XR Interactable Affordance State Provider

State Machine component that derives an interaction affordance state from an associated interactable.

| **Property** | **Description** |
|---|---|
| **Transition Duration** | Duration of transition in seconds. 0 means no smoothing. |
| **Interactable Source** | Interactable component for which to provide affordance states. If null, will try and find an interactable component attached. |
| **Select Click Animation Mode** | Condition to trigger click animation for Selected interaction events. |
| **Activate Click Animation Mode** | Condition to trigger click animation for activated interaction events. |
| **Click Animation Duration** | Duration of click animations for selected and activated events. |
| **Click Animation Curve** | Animation curve reference for click animation events. Select the More menu (&#8942;) to choose between a direct reference and a reusable scriptable object animation curve datum. |