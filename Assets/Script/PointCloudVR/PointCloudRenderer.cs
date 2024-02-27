using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

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

        private float pointRadius = 5f;
        private int renderCircles = 0;
        private CommandBuffer commandBuffer;

        private void Start()
        {
            topology = MeshTopology.Points;
            point = Shader.Find("Unlit/PointShader");
            quad = Shader.Find("Unlit/QuadShader");

            material = new Material(Shader.Find("Custom/QuadGeoScreenSizeShader"));
            material.enableInstancing = true;
            material.SetFloat("_PointSize", pointRadius);
            material.SetInt("_Circles", renderCircles);
            Rect screen = Camera.main.pixelRect;
            material.SetInt("_ScreenWidth", (int)screen.width);
            material.SetInt("_ScreenHeight", (int)screen.height);

            //commandBuffer = new CommandBuffer();
        }

        public void CreateMeshFromPointCloud(Vector3[] points, Color[] colors)
        {
            GameObject pointCloudContainer = new GameObject("PC-Container");
            pointCloudContainer.transform.SetParent(transform.parent.transform.parent);
            pointCloudContainer.transform.position = Vector3.zero;

            int MAX_VERTICES = 65000;

            int numberOfGameObjects = 1 + (points.Length - 1) / MAX_VERTICES;
            for (int i = 0; i < numberOfGameObjects; i++)
            {
                var points_slice = points.Skip(i * MAX_VERTICES).Take(MAX_VERTICES).ToArray();
                var colors_slice = colors.Skip(i * MAX_VERTICES).Take(MAX_VERTICES).ToArray();
                CreateMeshGameObject(i, points_slice, colors_slice, pointCloudContainer.transform);
            }

        }

        private void CreateMeshGameObject(int i, Vector3[] points, Color[] colors, Transform parent)
        {
            GameObject go = new GameObject($"Mesh-{i}");
            go.transform.parent = parent;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            int[] indecies = new int[points.Length];
            
            for (int j = 0; j < points.Length; ++j)
            {
                indecies[j] = j; 
            }

            mesh.vertices = points;
            mesh.colors = colors;
            mesh.SetIndices(indecies, MeshTopology.Points, 0);

            mf.mesh = mesh;
            mr.material = material;

        }

        public void SetData(PointCloud pc, PointCloud pcQ)
        {
            //pointsVertices = new ComputeBuffer(pc.points.Length, 3 * sizeof(float));
            //pointsColors = new ComputeBuffer(pc.colors.Length, sizeof(int));
            
            //pointsVertices.SetData(pc.points);
            //pointsColors.SetData(pc.colors);

            //if (pcQ.points.Length == 0) return;
            //quadVertices = new ComputeBuffer(pcQ.points.Length, 3 * sizeof(float));
            //quadColors = new ComputeBuffer(pcQ.colors.Length, sizeof(int));
            //quadNormals = new ComputeBuffer(pcQ.normals.Length, 3 * sizeof(float));
            
            //quadVertices.SetData(pcQ.points);
            //quadColors.SetData(pcQ.colors);
            //quadNormals.SetData(pcQ.normals);

            //PrepareMaterial();
        }

        private void PrepareMaterial()
        {
            //if (topology == MeshTopology.Points)
            //{
            //    material.SetBuffer("_Positions", pointsVertices);
            //    material.SetBuffer("_Colors", pointsColors);
            //    totalPoints = pointsVertices.count;
            //}
            //else
            //{
            //    material.SetBuffer("_Positions", quadVertices);
            //    material.SetBuffer("_Colors", quadColors);
            //    material.SetBuffer("_Normals", quadNormals);
            //    totalPoints = quadVertices.count;
            //}

            //commandBuffer.Clear();
            //commandBuffer.DrawProcedural(Matrix4x4.identity, material, 0, topology, totalPoints, 1);

        }

        public void Render(bool render)
        {
            //if (render)
            //{
            //    Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer); 
            //} 
            //else
            //{
            //    Camera.main.RemoveCommandBuffers(CameraEvent.BeforeForwardOpaque);
            //}

        }

        public void SetTopology(int topology)
        {
            //switch (topology)
            //{
            //    case 0:
            //        this.topology = MeshTopology.Points;
            //        material = new Material(point);
            //        PrepareMaterial();
            //        break;
            //    case 1:
            //        this.topology = MeshTopology.Quads;
            //        material = new Material(quad);
            //        PrepareMaterial();
            //        break;
            //    default:
            //        break;
            //}
        }


        private void OnDestroy()
        {
            //pointsColors.Release();
            //pointsVertices.Release();
            //quadVertices.Release();
            //quadColors.Release();
            //quadNormals.Release();
        }

    }

}
