using System.Collections.Generic;
using UnityEngine;

namespace WorldStrands
{
    [CreateAssetMenu(fileName = "WorldStrandProfile", menuName = "WorldStrands/Profile", order = 0)]
    public class WorldStrandProfile : ScriptableObject
    {
        [System.Serializable]
        public struct ProfilePoint
        {
            public float y;
            public Color color;
        }

        public List<ProfilePoint> points = new List<ProfilePoint>
        {
            new ProfilePoint { y = 0f, color = Color.white },
            new ProfilePoint { y = -0.2f, color = Color.white }
        };
    }
}
