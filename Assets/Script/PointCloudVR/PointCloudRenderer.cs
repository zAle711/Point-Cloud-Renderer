using UnityEngine;

namespace PointCloudVR
{
    public class PointCloudRenderer : MonoBehaviour
    {
        public bool render;

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

        private void Start()
        {
            topology = MeshTopology.Quads;
            render = true;
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
            if (!render || totalPoints == 0) return;

            pointMaterial.SetPass(0);
            
            Graphics.DrawProceduralNow(topology, totalPoints, 1);
        }
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
        public void SetTopology(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.POINT:
                    topology = MeshTopology.Points;
                    pointMaterial = new Material(point);
                    PrepareMaterial();
                    break;
                case RenderMode.QUAD:
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
