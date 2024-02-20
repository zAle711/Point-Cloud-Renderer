using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PointCloudVR
{

    public class PointCloudVisualizer : MonoBehaviour
    {
        // Start is called before the first frame update
        public string Point_Cloud = "";
        public string Point_Cloud_Mesh = "";
        public float Quad_Size = 0.15f;
        public bool invertYZ = false;
    

        private PointCloud pc;
        private PointCloud pcQ;

        public PointCloudRenderer pcRenderer;
        public GameObject pcMesh;

        void Start()
        {
            if (Point_Cloud != "")
            {
                PointCloudReader.ReadPCDFile(out pc, out pcQ, Point_Cloud, Quad_Size, invertYZ);
                //PointCloudReader.CreateFloor(GameObject.FindGameObjectWithTag("Floor"));
                pcRenderer.SetData(pc, pcQ);

            }
            if (Point_Cloud_Mesh != "")
            {
                PointCloudReader.ReadPLYFile(out Vector3[] v, out int[] c, out Vector3[] n, out int[] t, Point_Cloud_Mesh, invertYZ);
                // pc_mesh = GameObject.FindGameObjectWithTag("PCMesh");
                CreateMesh(v, c, n, t);
            }

   

            
        }

        private void CreateMesh(Vector3[] v, int[] c, Vector3[] n, int[] t)
        {
            MeshFilter mf = pcMesh.GetComponent<MeshFilter>();
            MeshRenderer mr = pcMesh.GetComponent<MeshRenderer>();
            MeshCollider mc = pcMesh.GetComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = v;
            mesh.normals = n;
            mesh.triangles = t;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
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
            
        }


        private void OnRenderObject()
        {
            //pcRenderer.Render();
        }

        private void OnPostRender()
        {
           //pcRenderer.Render();
        }
    }

}