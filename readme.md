# worldstrands

This repository contains a prototype Unity tool for generating stylized ropes with prayer flags. The cross‑section profile data is stored directly on the `worldStrandPath` component and edited through a custom curve editor in its inspector.

Scripts are located under `assets/worldStrands/`.

## Strand Path Component

`worldStrandPath` extrudes its internal profile list along a set of path points. Path points can be adjusted directly in the Scene view and the generated mesh is stored in a `MeshFilter` on the same GameObject. The inspector provides a small grid where you can place or remove profile points and adjust their colors.

The runtime component exposes a **Profile Scale** value which controls the overall size of the cross‑section. The custom editor (`worldStrandPathEditor`) automatically ensures at least two profile vertices and draws interactive handles for editing. Double‑click in the profile preview to add a new vertex or right‑click an existing one to remove it. Dragging the handles updates the `x` and `y` values while preserving the chosen color. A color field appears for the currently selected profile point so you can paint the strand as you edit it.

Moving path points works directly in the Scene view via Unity handles, and the **Update Mesh** button can be used to force regeneration if needed. All related scripts live under `assets/worldStrands/`.
