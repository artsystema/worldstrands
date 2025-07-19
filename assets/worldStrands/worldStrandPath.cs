using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;

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

        [System.Serializable]
        public class WorldPoint
        {
            public Vector3 position;
            public Quaternion rotation = Quaternion.identity;
        }

        [SerializeField]
        private List<WorldPoint> points = new List<WorldPoint>
        {
            new WorldPoint { position = new Vector3(-1f, 0f, 0f) },
            new WorldPoint { position = new Vector3(1f, 0f, 0f) }
        };

        public float profileStep = 0.1f;

        public float sagging = 0f;

        public bool displayCoordinates = true;

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
                points = new List<WorldPoint>
                {
                    new WorldPoint { position = new Vector3(-1f, 0f, 0f) },
                    new WorldPoint { position = new Vector3(1f, 0f, 0f) }
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
            float sumX = 0f, sumY = 0f, minX = float.MaxValue, maxX = float.MinValue;
            for (int i = 0; i < profile.Count; i++)
            {
                sumX += profile[i].x;
                sumY += profile[i].y;
                if (profile[i].x < minX) minX = profile[i].x;
                if (profile[i].x > maxX) maxX = profile[i].x;
            }
            float centerX = sumX / profile.Count;
            float centerY = sumY / profile.Count;

            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "WorldStrandPathMesh";
            }
            // Always assign mesh if MeshFilter is empty or null
            var mf = GetComponent<MeshFilter>();
            if (mf.sharedMesh == null || mf.sharedMesh.vertexCount == 0)
            {
                mf.sharedMesh = _mesh;
            }

            int profileCount = profile.Count;
            List<Vector3> verts = new List<Vector3>();
            List<Color> cols = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> uv2s = new List<Vector2>();
            List<int> tris = new List<int>();
            float totalPathLength = 0f;
            List<float> sectionDistances = new List<float>();
            Vector3 prevPos = points[0].position;
            sectionDistances.Add(0f);
            for (int seg = 1; seg < points.Count; seg++)
            {
                float d = Vector3.Distance(prevPos, points[seg].position);
                totalPathLength += d;
                sectionDistances.Add(totalPathLength);
                prevPos = points[seg].position;
            }
            int crossSectionIdx = 0;
            // Find min/max Y for profile V mapping
            float minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < profile.Count; i++)
            {
                if (profile[i].y < minY) minY = profile[i].y;
                if (profile[i].y > maxY) maxY = profile[i].y;
            }
            float maxDist = 0f;
            for (int seg = 1; seg < points.Count; seg++)
            {
                float d = Vector3.Distance(points[seg - 1].position, points[seg].position);
                if (d > maxDist) maxDist = d;
            }
            for (int seg = 0; seg < points.Count - 1; seg++)
            {
                WorldPoint p0 = points[seg];
                WorldPoint p1 = points[seg + 1];
                float segStartDist = sectionDistances[seg];
                float segEndDist = sectionDistances[seg + 1];
                float dist = segEndDist - segStartDist;
                int steps = Mathf.Max(2, Mathf.CeilToInt(dist / profileStep));
                for (int s = 0; s < steps; s++)
                {
                    float t = (float)s / (steps - 1);
                    Vector3 pos = Vector3.Lerp(p0.position, p1.position, t);
                    Quaternion rot = Quaternion.Slerp(p0.rotation, p1.rotation, t);
                    float pathU = (segStartDist + dist * t) / totalPathLength;
                    float sway = (maxDist > 0f) ? (dist / maxDist) * 4f * t * (1f - t) : 0f;
                    float sag = sagging * dist * 4f * t * (1f - t);
                    pos.y -= sag;
                    for (int j = 0; j < profileCount; j++)
                    {
                        var pp = profile[j];
                        Vector3 localProfile = new Vector3((pp.x - centerX) * profileScale, (pp.y - centerY) * profileScale, 0f);
                        verts.Add(pos + rot * localProfile);
                        cols.Add(pp.color);
                        float v = (maxY - minY) > 0f ? (pp.y - minY) / (maxY - minY) : 0f;
                        uvs.Add(new Vector2(pathU, v));
                        uv2s.Add(new Vector2(v, sway));
                    }
                    crossSectionIdx++;
                }
            }
            int crossSections = verts.Count / profileCount;
            for (int i = 0; i < crossSections - 1; i++)
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
            _mesh.vertices = verts.ToArray();
            _mesh.colors = cols.ToArray();
            _mesh.uv = uvs.ToArray();
            _mesh.uv2 = uv2s.ToArray();
            _mesh.triangles = tris.ToArray();
            _mesh.RecalculateNormals();
        }

        public void BakeMeshToObj(string path)
        {
            if (_mesh == null) return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("o StrandMesh");
            for (int i = 0; i < _mesh.vertexCount; i++)
            {
                var v = _mesh.vertices[i];
                sb.AppendLine($"v {v.x} {v.y} {v.z}");
                var c = (i < _mesh.colors.Length) ? _mesh.colors[i] : Color.white;
                sb.AppendLine($"vc {c.r} {c.g} {c.b} {c.a}");
            }
            foreach (var uv in _mesh.uv)
                sb.AppendLine($"vt {uv.x} {uv.y}");
            for (int i = 0; i < _mesh.triangles.Length; i += 3)
            {
                int a = _mesh.triangles[i] + 1;
                int b = _mesh.triangles[i + 1] + 1;
                int c = _mesh.triangles[i + 2] + 1;
                sb.AppendLine($"f {a}/{a} {b}/{b} {c}/{c}");
            }
            System.IO.File.WriteAllText(path, sb.ToString());
#if UNITY_EDITOR
    UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public void BakeMeshToFbx(string path)
        {
#if UNITY_EDITOR
            var go = new GameObject("StrandMeshExport");
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mf.sharedMesh = _mesh;
            ModelExporter.ExportObject(path, go);
            GameObject.DestroyImmediate(go);
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
