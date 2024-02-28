# CurveVisualController

The `CurveVisualController` class is designed to provide a versatile and configurable controller for a Line Renderer component, based on data provided from an interactor implementing `ICurveInteractionDataProvider` to visually represent it.

It uses a configurable origin and direction, along with end point data from the Curve Data Provider, to generate a bezier curve that masks discrepancies between divergent origins, and mask any stabilization applied.

Importantly, the per-state line properties allow for dynamic adjustment of the line's visual properties based on the state of the curve data provider, according to what the `EndPointType` and selection data provide. In practice this allows for animated color, line width and bend properties to be applied to the line renderer, based on the state of the interactor implementing `ICurveInteractionDataProvider`.

This class also has many advanced configuration options, including:
- Support for different line dynamics modes (`Traditional`, `RetractOnHitLoss`, and `ExpandFromHitPoint`).
- Customizable visual settings, including the number of points for curve creation and maximum curve extension distance.
- Control over line dynamics, such as retraction delay and duration, extension rate, and expansion rate.
- Advanced curve adjustments, including mid-point computation for complex curves and snapping options to attach points or snap volumes.
- Comprehensive color settings, including the ability to change and blend the color gradient based on different states (e.g., hit detection).
- End-point type-specific line configuration, allowing for customized line properties depending on the type of interaction detected (e.g., UI hit, non-UI hit).
- Material settings to swap line renderer's material based whether the data provider reports an empty hit, to enable the use of a material that doesn't write to depth and avoid z-fighting. 
- Use of Unity's Burst compiler when possible to accelerate the computation of the complex curve mathematics.

| **Property**                            | **Description** |
|-----------------------------------------|---|
| **Line Renderer**                       | Line renderer to control. |
| **Curve Visual Object**                 | Curve data source used to generate the visual curve. |
| **Override Line Origin**                | Indicates whether to override the line origin with a custom transform. |
| **Line Origin Transform**               | The transform that determines the origin position and direction of the line when overriding. |
| **Customize Line Properties For State** | Indicates whether to customize line properties for different endpoint type states. |
| **Line Property Animation Speed**       | Speed at which the line width changes when transitioning between states. |
| **No Valid Hit Properties**             | Line properties when no hit is detected or when over an object that cannot be interacted with. |
| **UI Hit Properties**                   | Line properties when a valid UI hit is detected. |
| **UI Press Properties**                 | Line properties when a valid UI press hit is detected. |
| **Hover Hit Properties**                | Line properties when a valid non-UI hit is detected. |
| **Select Hit Properties**               | Line properties when a valid selection is detected. |

| **Line Properties**                    | **Description** |
|----------------------------------------|---|
| **Smoothly Curve Line**                | Determine if the line should smoothly curve when this state property is active. If false, a straight line will be drawn. |
| **Line Bend Ratio**                    | Ratio to control the bend of the line by adjusting the mid-point. A value of 1 defaults to a straight line. |
| **Adjust Width**                       | Determine if the line width should be customized from defaults when this state property is active. |
| **Start Width**                        | Width of the line at the start. |
| **End Width**                          | Width of the line at the end. |
| **End Width Scale Distance Factor**    | If greater than 0, the curve end width will be scaled based on the the percentage of the line length to the max visual curve distance, multiplied by the scale factor. |
| **Adjust Gradient**                    | Determine if the line color should change when this state property is active. |
| **Gradient**                           | Color gradient to use when this state property is active. |
| **Customize Expand Line Draw Percent** | Determine if the line mode expansion should be customized from defaults. |
| **Expand Mode Line Draw Percent**      | Percent of the line to draw when using the expand from hit point mode when this state property is active. |

| **Advanced Properties**                   | **Description** |
|-------------------------------------------|---|
| **Visual Point Count**                    | Number of points used to create the visual curve. |
| **Max Visual Curve Distance**             | Maximum distance the visual curve can extend. |
| **Resting Visual Line Length**            | Default length of the line when not extended or retracted. |
| **Compute Mid Point With Complex Curves** | Determines if the mid-point is computed for curves with more than 1 segment. |
| **Snap To Selected Attach If Available**  | Snaps the line to a selected attachment point if available. |
| **Snap To Snap Volume If Available**      | Snaps the line to a snap volume if available. |
| **Curve Start Offset**                    | Offset at the start of the curve to avoid overlap with the origin.  |
| **Curve End Offset**                      | Offset at the end of the curve in meters to avoid overlap with the target. |
| **Render Line In World Space**            | If true the line will be rendered in world space, otherwise it will be rendered in local space. Set this to false in the event that high speed locomotion causes some visual artifacts with the line renderer. |
| **Extend Line To Empty Hit**              | Determines if the line should extend out to empty hits, if not, length will be maintained. |
| **Extension Rate**                        | Rate at which the line extends to meet hit point. Set to 0 for instant extension. |
| **Line Dynamics Mode**                    | Specifies the dynamics mode of the line. |
| **Retract Delay**                         | Delay before the line starts retracting after extending. |
| **Retract Duration**                      | Duration it takes for the line to fully retract. |
| **End Point Expansion Rate**              | Rate at which the end point expands and retracts to and from the end point. |
| **Swap Materials**                        | Indicates whether to swap the line renderer's material for different states. |
| **Normal Line Material**                  | Material to use in all cases other than when over 3D geometry that is not a valid interactable target. |
| **Empty Hit Material**                    | Material to use when over 3D geometry that is not a valid interactable target. |
