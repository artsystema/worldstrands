# worldstrands

This repository contains a prototype Unity tool for generating stylized ropes with prayer flags. The initial implementation includes a `WorldStrandProfile` ScriptableObject and its custom Inspector UI. This profile defines the vertical cross‑section of a strand with positions and per‑vertex colors.

Scripts are located under `assets/worldStrands/`.

## Strand Path Component

`WorldStrandPath` is a `MonoBehaviour` that extrudes a `WorldStrandProfile` along a list of path points. Points can be adjusted directly in the Scene view and the generated mesh is stored in a `MeshFilter` on the same GameObject. The inspector now embeds the profile editor so you can tweak cross‑section points without leaving the component.
