<!--
All interactors (Near-Far, Direct, Poke, Ray, Gaze, Socket, AR Gesture)

To include this file (adjust heading level and include file path as needed):

## Interactor Filters {#interactor-filters}

[!INCLUDE [interactor-filters-config](snippets/interactor-filters-config.md)]
-->

Configure the starting set of filters that an interactor uses to determine which interactables are eligible for interaction. All of these filter properties are optional. If you do not assign them, the interactor uses default behavior.

Refer to [Interaction filters](xref:xri-interaction-filters) for more information about implementing filters.

| **Property** | **Description** |
|---|---|
| **Starting Target Filter** | Filters the list of interactables that are eligible for interaction. |
| **Starting Hover Filters** list | Validates which potential hover targets are eligible for hover interaction. |
| **Starting Select Filters** list | Validates which potential select targets are eligible for select interaction. |
