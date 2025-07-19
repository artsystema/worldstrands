# WorldStrands

WorldStrands is a small Unity prototype for crafting stylized ropes or cables. It ships with a `worldStrandPath` component that extrudes a custom cross‑section along editable path points to create a mesh.

Scripts live under `assets/worldStrands/`.

## Features
- Interactive profile editor with per‑vertex color. Double‑click to add points and right‑click to remove.
- Scene view handles for moving and rotating path points.
- `Profile Scale`, `Path Detail` and `Sagging` settings to tweak the mesh.
- Optional coordinate overlay when editing profile vertices.
- Export buttons to bake the generated mesh to OBJ or FBX.

## Quick Start
1. Add `worldStrandPath` to a GameObject that has a `MeshFilter` and `MeshRenderer`.
2. Shape the profile and path using the inspector and scene view.
3. Press **Update Mesh** to rebuild, or use **Bake Mesh** / **Bake Mesh (FBX)** to save the result.
