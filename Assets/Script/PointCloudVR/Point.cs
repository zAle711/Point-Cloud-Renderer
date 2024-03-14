using UnityEngine;

namespace PointCloudVR
{
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