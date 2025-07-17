using System.Collections.Generic;
using UnityEngine;

namespace worldStrands
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class worldStrandPath : MonoBehaviour
    {
        public worldStrandProfile profile;

        public List<Vector3> points = new List<Vector3>
        {
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f)
        };

        Mesh _mesh;

        void OnValidate()
        {
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            if (profile == null || points.Count < 2 || profile.points.Count < 2)
            {
                return;
            }

            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "WorldStrandPathMesh";
                GetComponent<MeshFilter>().sharedMesh = _mesh;
            }

            int segments = points.Count;
            int profileCount = profile.points.Count;

            Vector3[] verts = new Vector3[segments * profileCount];
            Color[] cols = new Color[verts.Length];
            List<int> tris = new List<int>();

            for (int i = 0; i < segments; i++)
            {
                for (int j = 0; j < profileCount; j++)
                {
                    var pp = profile.points[j];
                    verts[i * profileCount + j] = points[i] + Vector3.up * pp.y;
                    cols[i * profileCount + j] = pp.color;
                }
            }

            for (int i = 0; i < segments - 1; i++)
            {
                for (int j = 0; j < profileCount - 1; j++)
                {
                    int v0 = i * profileCount + j;
                    int v1 = i * profileCount + j + 1;
                    int v2 = (i + 1) * profileCount + j;
                    int v3 = (i + 1) * profileCount + j + 1;

                    tris.Add(v0);
                    tris.Add(v2);
                    tris.Add(v1);

                    tris.Add(v1);
                    tris.Add(v2);
                    tris.Add(v3);
                }
            }

            _mesh.Clear();
            _mesh.vertices = verts;
            _mesh.colors = cols;
            _mesh.triangles = tris.ToArray();
            _mesh.RecalculateNormals();
        }
    }
}
