using UnityEngine;

namespace PointCloudVR
{
    class PointCloudRenderer
    {
        private Vector3[] points;
        private int[] colors;

        private Matrix4x4 localToWorldMatrix;
        private Material pointMaterial;

        private MeshTopology topology;

        public PointCloudRenderer()
        {
            topology = MeshTopology.Points;
            pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        }

        public void SetWorldMatrix(Matrix4x4 localToWorldMatrix)
        {
            this.localToWorldMatrix = localToWorldMatrix;
        }
        public void SetData(Vector3[] points, int[] colors)
        {
            this.points = points;
            this.colors = colors;
        }
        public void SetTopology(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.POINT:
                    topology = MeshTopology.Points;
                    break;
                case RenderMode.CUBE:
                    topology = MeshTopology.Quads;
                    break;
                default:
                    break;
            }
        }
        public int GetNPoints()
        {
            return (points != null) ? points.Length : 0;
        }

        public void Render()
        {
            if (points == null || points.Length == 0) return;

            ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
            positionsBuffer.SetData(points);
            colorsBuffer.SetData(colors);
            pointMaterial.SetMatrix("_Transform", localToWorldMatrix);
            pointMaterial.SetBuffer("_Positions", positionsBuffer);
            pointMaterial.SetBuffer("_Colors", colorsBuffer);
            pointMaterial.SetPass(0);
            Graphics.DrawProceduralNow(topology, points.Length, 1);

            positionsBuffer.Release();
            colorsBuffer.Release();

        }
    }

}
