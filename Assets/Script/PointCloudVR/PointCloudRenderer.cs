using UnityEngine;

namespace PointCloudVR
{
    class PointCloudRenderer
    {
        public bool render = true;

        private Material pointMaterial;
        private Material quadMaterial;

        private MeshTopology topology;

        private Shader point;
        private Shader quad;

        private ComputeBuffer pointsVertices;
        private ComputeBuffer pointsColors;

        private ComputeBuffer quadVertices;
        private ComputeBuffer quadColors;
        private ComputeBuffer quadNormals;

        private int totalPoints = 0;
        
        public PointCloudRenderer()
        {
            topology = MeshTopology.Points;

            point = Shader.Find("Unlit/PointShader");
            quad = Shader.Find("Unlit/QuadShader");
            
            pointMaterial = new Material(point);
        }

        public void SetData(PointCloud pc, PointCloud pcQ)
        {
            pointsVertices = new ComputeBuffer(pc.points.Length, 3 * sizeof(float));
            pointsColors = new ComputeBuffer(pc.colors.Length, sizeof(int));
            
            pointsVertices.SetData(pc.points);
            pointsColors.SetData(pc.colors);

            quadVertices = new ComputeBuffer(pcQ.points.Length, 3 * sizeof(float));
            quadColors = new ComputeBuffer(pcQ.colors.Length, sizeof(int));
            quadNormals = new ComputeBuffer(pcQ.normals.Length, 3 * sizeof(float));
            
            quadVertices.SetData(pcQ.points);
            quadColors.SetData(pcQ.colors);
            quadNormals.SetData(pcQ.normals);

            PrepareMaterial();
        }

        private void PrepareMaterial()
        {
            if (topology == MeshTopology.Points)
            {
                pointMaterial.SetBuffer("_Positions", pointsVertices);
                pointMaterial.SetBuffer("_Colors", pointsColors);
                totalPoints = pointsVertices.count;
            }
            else
            {
                pointMaterial.SetBuffer("_Positions", quadVertices);
                pointMaterial.SetBuffer("_Colors", quadColors);
                pointMaterial.SetBuffer("_Normals", quadNormals);
                totalPoints = quadVertices.count;
            }
            
        }

        public void Render()
        {
            if (!render && totalPoints == 0) return;

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

        public void SetData(Vector3[] threadPoints, int[] threadColors, Vector3[] threadNormals, int totalPoints)
        {
            if (totalPoints == 0) return;

            Vector3[] points = new Vector3[totalPoints];
            int[] colors = new int[totalPoints];
            Vector3[] normals = topology == MeshTopology.Quads ? new Vector3[totalPoints] : null;

            for(int i = 0; i < totalPoints; i++)
            {
                points[i] = threadPoints[i];
                colors[i] = threadColors[i];
                if (topology == MeshTopology.Quads) normals[i] = threadNormals[i];
            }

            pointsVertices = new ComputeBuffer(totalPoints, 3 * sizeof(float));
            pointsColors = new ComputeBuffer(totalPoints, sizeof(int));

            pointsVertices.SetData(points);
            pointsColors.SetData(colors);

            if (topology == MeshTopology.Quads)
            {
                quadNormals = new ComputeBuffer(totalPoints, 3 * sizeof(float));
                quadNormals.SetData(normals);
            }

            //SetPoints(points);
            //SetColors(colors);
            //SetNormals(normals);

            PrepareMaterial();
        }

        //public void SetData(Vector3[] points, int[] colors, Vector3[] normals)
        //{

        //    SetPoints(points);
        //    SetColors(colors);
        //    SetNormals(normals);

        //    PrepareMaterial();
        //}

        //public void SetData(Vector3[] points, int[] colors)
        //{

        //    SetPoints(points);
        //    SetColors(colors);

        //    PrepareMaterial();
        //}

        //private void SetPoints(Vector3[] points)
        //{
        //    if (points == null || points.Length == 0) return;

        //    if (pointsVertices != null) pointsVertices.Release();

        //    pointsVertices = new ComputeBuffer(points.Length, 3 * sizeof(float));
        //    pointsVertices.SetData(points);
        //    totalPoints = points.Length;
        //}

        //private void SetColors(int[] colors)
        //{
        //    if (colors == null || colors.Length == 0) return;

        //    if (pointsColors != null) pointsColors.Release();

        //    pointsColors = new ComputeBuffer(colors.Length, sizeof(int));
        //    pointsColors.SetData(colors);
        //}

        //private void SetNormals(Vector3[] normals)
        //{
        //    if (normals == null || normals.Length == 0) return;

        //    if (normalsBuffer != null) normalsBuffer.Release();

        //    normalsBuffer = new ComputeBuffer(normals.Length, 3 * sizeof(float));
        //    normalsBuffer.SetData(normals);
        //}

        public void SetTopology(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.POINT:
                    topology = MeshTopology.Points;
                    pointMaterial = new Material(point);
                    PrepareMaterial();
                    break;
                case RenderMode.CUBE:
                    topology = MeshTopology.Quads;
                    pointMaterial = new Material(quad);
                    PrepareMaterial();
                    break;
                default:
                    break;
            }
        }
        


        
    }

}
