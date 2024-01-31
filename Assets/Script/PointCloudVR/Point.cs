using UnityEngine;

namespace PointCloudVR
{
    public struct Point
    {
        public Point(Vector3 position, int color, Vector3 normal)
        {
            this.position = position;
            this.color = color;
            this.normal = normal;
        }
        
        public Vector3 position { get;  }
        public int color { get;  }
        public Vector3 normal { get; }
    }
}


