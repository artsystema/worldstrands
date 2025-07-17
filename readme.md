# worldstrands

This repository contains a prototype Unity tool for generating stylized ropes with prayer flags. The cross‑section profile data is stored directly on the `worldStrandPath` component and edited through a custom curve editor in its inspector.

Scripts are located under `assets/worldStrands/`.

## Strand Path Component

`worldStrandPath` extrudes its internal profile list along a set of path points. Path points can be adjusted directly in the Scene view and the generated mesh is stored in a `MeshFilter` on the same GameObject. The inspector provides a small grid where you can place or remove profile points and adjust their colors.
