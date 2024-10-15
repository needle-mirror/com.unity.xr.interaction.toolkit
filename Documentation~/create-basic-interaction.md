---
uid: xri-create-basic-interaction
---

# Create a basic interaction

Make a GameObject interactable by adding the necessary components to it.

<a id="create-grab-interactable"></a>
## Create an Interactable for the player to grab

Interactable components define how the user can interact with objects in a scene. To create a basic 3D cube that can be grabbed, use **GameObject &gt; XR &gt; Grab Interactable**.

For this example, create a plane for the cube to rest on so it does not fall out of reach. Use **GameObject &gt; 3D Object &gt; Plane**, then click on the Cube and click and drag the Transform gizmo to position it above the Plane GameObject.

In the screenshot below, the GameObject with the XR Grab Interactable supports grabbing, moving, dropping, and throwing with smoothed tracking.

![interactable-setup](images/interactable-setup.png)

> [!TIP]
> Interactables added through the **GameObject &gt; XR** menu use a Box Collider to detect interaction, but other types of Collider components such as a convex Mesh Collider can provide better hit detection at the cost of performance.

To configure an existing GameObject to make it an interactable object to allow the user to grab it, select it in your scene and add these components:
- Add **Component &gt; XR &gt; XR Grab Interactable**
- Add **Component &gt; Physics &gt; Box Collider**
