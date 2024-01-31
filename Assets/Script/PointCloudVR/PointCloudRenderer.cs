using UnityEngine;

namespace PointCloudVR
{
    class PointCloudRenderer
    {
        private int totalPoints = 0;

        private Material pointMaterial;

        private MeshTopology topology;

        private ComputeBuffer pointsBuffer;
        private ComputeBuffer colorsBuffer;
        private ComputeBuffer normalsBuffer;
        
        public PointCloudRenderer()
        {
            topology = MeshTopology.Quads;
            //pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
            pointMaterial = new Material(Shader.Find("Unlit/CubeShader"));
        }

        private void PrepareMaterial()
        {
            pointMaterial.SetBuffer("_Positions", pointsBuffer);
            pointMaterial.SetBuffer("_Colors", colorsBuffer);
            pointMaterial.SetBuffer("_Normals", normalsBuffer);  
        }

        public void Render()
        {
            if (totalPoints == 0)
            { 
                return;
            }

            pointMaterial.SetPass(0);
            Graphics.DrawProceduralNow(topology, totalPoints, 1);
        }

        public void SetData(Vector3[] points, int[] colors, Vector3[] normals)
        {
            SetPoints(points);
            SetColors(colors);
            SetNormals(normals);

            PrepareMaterial();
        }


        public void SetData(Vector3[] points, int[] colors)
        {

            SetPoints(points);
            SetColors(colors);

            PrepareMaterial();
        }

        private void SetPoints(Vector3[] points)
        {
            if (points.Length == 0) return;

            if (pointsBuffer != null) pointsBuffer.Release();

            pointsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            pointsBuffer.SetData(points);
            totalPoints = points.Length;
        }

        private void SetColors(int[] colors)
        {
            if (colors.Length == 0) return;

            if (colorsBuffer != null) colorsBuffer.Release();

            colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
            colorsBuffer.SetData(colors);
        }

        private void SetNormals(Vector3[] normals)
        {
            if (normals.Length == 0) return;

            if (normalsBuffer != null) normalsBuffer.Release();

            normalsBuffer = new ComputeBuffer(normals.Length, 3 * sizeof(float));
            normalsBuffer.SetData(normals);
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
        


        
    }

}
