# WorldStrands

WorldStrands is a lightweight Unity prototype for generating stylized ropes or cables. The package supplies a **worldStrandPath** component that extrudes a custom 2D profile along a user defined path to build a mesh at runtime.

Scripts can be found under `assets/worldStrands/`.

## Features

- Interactive profile editor with per–vertex colors
- Scene view handles for moving and rotating path points
- Adjustable **Profile Scale**, **Path Detail** and **Sagging** options
- Optional coordinate overlay for profile vertices
- One‑click export to OBJ or FBX

## Installation

1. Copy the `assets/worldStrands` folder into your Unity project.
2. Ensure the project has the Unity FBX Exporter package installed if you want to export FBX files.

## Usage

1. In Unity create a GameObject and add a `MeshFilter` and `MeshRenderer` if it does not already have them.
2. Add the **worldStrandPath** component.
3. In the inspector open the *Profile* section to edit the cross‑section. Double‑click inside the preview to add points or right‑click points to delete. Colors can be assigned per vertex.
4. In the *Points* list adjust the path of the cable. Handles appear in the Scene view so you can position and rotate each point.
5. Use **Profile Scale** to scale the cross‑section, **Path Detail** to change how many slices are generated along the path and **Sagging** to add droop between points.
6. Press **Update Mesh** to rebuild the mesh at any time. When you are happy with the result you can use **Bake Mesh** or **Bake Mesh (FBX)** to write the generated mesh to disk.

The generated mesh is assigned to the object's MeshFilter, so the strand can be used like any other mesh in your scene.

## License

This project is provided as a small experimental tool. See the repository for license information.
