using UnityEngine;

namespace PointCloudVR
{
    class PointCloudRenderer
    {
        private int totalPoints = 0;

        private Material pointMaterial;

        private MeshTopology topology;

        private Shader point;
        private Shader quad;

        private ComputeBuffer pointsBuffer;
        private ComputeBuffer colorsBuffer;
        private ComputeBuffer normalsBuffer;
        
        public PointCloudRenderer()
        {
            topology = MeshTopology.Points;

            point = Shader.Find("Unlit/PointShader");
            quad = Shader.Find("Unlit/QuadShader");
            
            pointMaterial = new Material(point);
        }

        private void PrepareMaterial()
        {
            pointMaterial.SetBuffer("_Positions", pointsBuffer);
            pointMaterial.SetBuffer("_Colors", colorsBuffer);
            
            if (topology == MeshTopology.Quads) pointMaterial.SetBuffer("_Normals", normalsBuffer);  
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
        //public void SetData(ComputeBuffer p, ComputeBuffer c, ComputeBuffer n)
        //{
        //    pointsBuffer.Release();
        //    pointsBuffer = p;
        //    colorsBuffer.Release();
        //    colorsBuffer = c;
        //    normalsBuffer.Release();
        //    normalsBuffer = n;

        //    PrepareMaterial();
        //}

        //public void SetData(ComputeBuffer p, ComputeBuffer c)
        //{
        //    pointsBuffer.Release();
        //    pointsBuffer = p;
        //    colorsBuffer.Release();
        //    colorsBuffer = c;

        //    PrepareMaterial();
        //}

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
            if (points == null || points.Length == 0) return;

            if (pointsBuffer != null) pointsBuffer.Release();

            pointsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            pointsBuffer.SetData(points);
            totalPoints = points.Length;
        }

        private void SetColors(int[] colors)
        {
            if (colors == null || colors.Length == 0) return;

            if (colorsBuffer != null) colorsBuffer.Release();

            colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
            colorsBuffer.SetData(colors);
        }

        private void SetNormals(Vector3[] normals)
        {
            if (normals == null || normals.Length == 0) return;

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
                    pointMaterial = new Material(point);
                    break;
                case RenderMode.CUBE:
                    topology = MeshTopology.Quads;
                    pointMaterial = new Material(quad);
                    break;
                default:
                    break;
            }
        }
        


        
    }

}
