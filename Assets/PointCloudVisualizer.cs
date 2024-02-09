using System;
using System.Collections;
using System.Collections.Generic;
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

    public class PointCloudVisualizer : MonoBehaviour
    {
        // Start is called before the first frame update
        public string Point_Cloud = "";
        public string Point_Cloud_Mesh = "";
        public float Quad_Size = 0.15f;
        public bool invertYZ = false;
    

        private PointCloud pc;
        private PointCloud pcQ;

        private GameObject pc_mesh;

        public PointCloudRenderer pcRenderer;

        void Start()
        {        
            if (Point_Cloud != "") PointCloudReader.ReadPCDFile(out pc, out pcQ, Point_Cloud, Quad_Size, invertYZ);
            PointCloudReader.CreateFloor(GameObject.FindGameObjectWithTag("Floor"));
            PointCloudReader.ReadPLYFile(out Vector3[] v, out int[] c, out Vector3[] n, out int[] t, Point_Cloud_Mesh, invertYZ);

            pc_mesh = GameObject.FindGameObjectWithTag("PCMesh");
            CreateMesh(v, c, n, t);         

            //pcRenderer.SetData(pc, pcQ);
        }

        private void CreateMesh(Vector3[] v, int[] c, Vector3[] n, int[] t)
        {
            MeshFilter mf = pc_mesh.GetComponent<MeshFilter>();
            MeshRenderer mr = pc_mesh.GetComponent<MeshRenderer>();
            MeshCollider mc = pc_mesh.GetComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = v;
            mesh.normals = n;
            mesh.triangles = t;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            mf.mesh = mesh;

            Material mat = new Material(Shader.Find("Unlit/TestPCD"));
            ComputeBuffer colors = new ComputeBuffer(c.Length, sizeof(int));
            colors.SetData(c);
            mat.SetBuffer("_Colors", colors);

            mr.material = mat;

            mc.sharedMesh = mesh;
            //pc_mesh.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                pcRenderer.SetTopology(RenderMode.QUAD);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                pcRenderer.SetTopology(RenderMode.POINT);
            }
        }


        private void OnRenderObject()
        {
            pcRenderer.Render();
        }
    }

}
