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

        public Point(Vector3 position, int color)
        {
            this.position = position;
            this.color = color;
            normal = Vector3.zero;
        }

        public Vector3 position { get; }
        public int color { get; }
        public Vector3 normal { get; }
    }

    public struct PointCloud
    {
        public Vector3[] points { get; private set; }
        public int[] colors { get; private set; }
        public Vector3[] normals { get; private set; }

        public PointCloud(Vector3[] points, int[] colors)
        {
            this.points = points;
            this.colors = colors;
            normals = new Vector3[0];
        }

        public PointCloud(Vector3[] points, int[] colors, Vector3[] normals)
        {
            this.points = points;
            this.colors = colors;
            this.normals = normals;
        }
    }

}