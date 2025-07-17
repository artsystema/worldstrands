using System.Collections.Generic;
using UnityEngine;

namespace worldStrands
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class worldStrandPath : MonoBehaviour
    {
        [System.Serializable]
        public class ProfilePoint
        {
            public float x;
            public float y;
            public Color color;
        }

        public float profileScale = 1f;

        [SerializeField]
        private List<ProfilePoint> profile = new List<ProfilePoint>
        {
            new ProfilePoint { x = 0f, y = 0f, color = Color.white },
            new ProfilePoint { x = 1f, y = -0.2f, color = Color.white }
        };

        [SerializeField]
        private List<Vector3> points = new List<Vector3>
        {
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f)
        };

        Mesh _mesh;

        void Awake()
        {
            if (profile == null || profile.Count == 0)
            {
                profile = new List<ProfilePoint>
                {
                    new ProfilePoint { x = 0f, y = 0f, color = Color.white },
                    new ProfilePoint { x = 1f, y = -0.2f, color = Color.white }
                };
            }
            if (points == null || points.Count == 0)
            {
                points = new List<Vector3>
                {
                    new Vector3(-1f, 0f, 0f),
                    new Vector3(1f, 0f, 0f)
                };
            }
        }

        void OnValidate()
        {
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            if (points.Count < 2 || profile.Count < 2)
            {
                return;
            }

            // Calculate profile centroid
            float sumX = 0f, sumY = 0f;
            for (int i = 0; i < profile.Count; i++)
            {
                sumX += profile[i].x;
                sumY += profile[i].y;
            }
            float centerX = sumX / profile.Count;
            float centerY = sumY / profile.Count;

            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "WorldStrandPathMesh";
                GetComponent<MeshFilter>().sharedMesh = _mesh;
            }

            int segments = points.Count;
            int profileCount = profile.Count;

            Vector3[] verts = new Vector3[segments * profileCount];
            Color[] cols = new Color[verts.Length];
            List<int> tris = new List<int>();

            for (int i = 0; i < segments; i++)
            {
                for (int j = 0; j < profileCount; j++)
                {
                    var pp = profile[j];
                    // Center the profile
                    verts[i * profileCount + j] = points[i] + new Vector3((pp.x - centerX) * profileScale, (pp.y - centerY) * profileScale, 0f);
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
