using UnityEngine;
using UnityEngine.Rendering;

namespace PointCloudVR
{
    public class PointCloudRenderer : MonoBehaviour
    {
        public bool render;

        private Material material;
        private MeshTopology topology;

        private Shader point;
        private Shader quad;

        private ComputeBuffer pointsVertices;
        private ComputeBuffer pointsColors;

        private ComputeBuffer quadVertices;
        private ComputeBuffer quadColors;
        private ComputeBuffer quadNormals;

        private int totalPoints = 0;

        private CommandBuffer commandBuffer;

        private void Start()
        {
            topology = MeshTopology.Points;
            point = Shader.Find("Unlit/PointShader");
            quad = Shader.Find("Unlit/QuadShader");

            material = new Material(point);
            commandBuffer = new CommandBuffer();
        }

        public void SetData(PointCloud pc, PointCloud pcQ)
        {
            pointsVertices = new ComputeBuffer(pc.points.Length, 3 * sizeof(float));
            pointsColors = new ComputeBuffer(pc.colors.Length, sizeof(int));
            
            pointsVertices.SetData(pc.points);
            pointsColors.SetData(pc.colors);

            if (pcQ.points.Length == 0) return;
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
                material.SetBuffer("_Positions", pointsVertices);
                material.SetBuffer("_Colors", pointsColors);
                totalPoints = pointsVertices.count;
            }
            else
            {
                material.SetBuffer("_Positions", quadVertices);
                material.SetBuffer("_Colors", quadColors);
                material.SetBuffer("_Normals", quadNormals);
                totalPoints = quadVertices.count;
            }

            commandBuffer.Clear();
            commandBuffer.DrawProcedural(Matrix4x4.identity, material, 0, topology, totalPoints, 1);

        }

        public void Render(bool render)
        {
            if (render)
            {
                Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer); 
            } 
            else
            {
                Camera.main.RemoveCommandBuffers(CameraEvent.BeforeForwardOpaque);
            }

        }

        public void SetTopology(int topology)
        {
            switch (topology)
            {
                case 0:
                    this.topology = MeshTopology.Points;
                    material = new Material(point);
                    PrepareMaterial();
                    break;
                case 1:
                    this.topology = MeshTopology.Quads;
                    material = new Material(quad);
                    PrepareMaterial();
                    break;
                default:
                    break;
            }
        }


        private void OnDestroy()
        {
            pointsColors.Release();
            pointsVertices.Release();
            quadVertices.Release();
            quadColors.Release();
            quadNormals.Release();
        }

    }

}
