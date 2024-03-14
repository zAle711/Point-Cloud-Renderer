using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PointCloudVR
{

    public class PointCloudVisualizer : MonoBehaviour
    {
        // Start is called before the first frame update
        public string Point_Cloud = "";
        public string Point_Cloud_Mesh = "";
        public float Quad_Size = 0.15f;
        public bool invertYZ = false;
        public int maxGameObject = 100;
        
        public TextMeshProUGUI text;
        public Camera myCamera;

        //Point Cloud
        private Vector3[] points;
        private Color[] colors;
        
        private PointOctree<Point> pointCloudOctree;
        private Material material;

        private OctreeTraversal octreeTraversal;


        private SimplePriorityQueue<Chunk> toRender = new SimplePriorityQueue<Chunk>();
        private SimplePriorityQueue<Chunk> toDelete = new SimplePriorityQueue<Chunk>();
        private DateTime lastUpdate;
        
        private SimplePriorityQueue<Chunk> nowRendering = new SimplePriorityQueue<Chunk>();

        private List<Chunk> currentRendering = new List<Chunk>();
        private Bounds[] visibleNodeBounds;
        //Mesh
        public GameObject pcMesh;

        void Start()
        {
            visibleNodeBounds = new Bounds[maxGameObject];
            lastUpdate = DateTime.Now;
            if (Point_Cloud != "")
            {
                PrepareMaterial();
                (points, colors) = PointCloudReader.ReadPCDFile(Point_Cloud, invertYZ);

                CreateOctree();
                CreateGameObjects();

                octreeTraversal = new OctreeTraversal(pointCloudOctree, maxGameObject);
                octreeTraversal.setData(GeometryUtility.CalculateFrustumPlanes(myCamera), myCamera.transform.position);
                octreeTraversal.Start();

                StartCoroutine(RenderChunks());

            }
            if (Point_Cloud_Mesh != "")
            {
                PointCloudReader.ReadPLYFile(out Vector3[] v, out int[] c, out Vector3[] n, out int[] t, Point_Cloud_Mesh, invertYZ);
                CreateMesh(v, c, n, t);
            }
     
        }

        IEnumerator RenderChunks()
        {
            for (; ;)
            {

                if (currentRendering.Count != maxGameObject)
                {
                    if (toRender.Count != 0)
                    {
                        Chunk c = toRender.Dequeue();
                        c.gObj.SetActive(true);
                        currentRendering.Add(c);
                    }
                }


                if (toDelete.Count != 0)
                {
                    Chunk c = toDelete.Dequeue();
                    c.gObj.SetActive(false);
                    currentRendering.Remove(c);
                }
                                       
                yield return new WaitForSeconds(.1f);

            }

        }

        private void CreateGameObjects()
        {
            GameObject go = new GameObject("PC-Container");
            go.transform.SetParent(transform.parent);
            go.transform.position = Vector3.zero;

            pointCloudOctree.CreateMesh(go.transform, material);
        }

        private void CreateOctree()
        {
            Point[] pointCloud = new Point[points.Length];

            for (int i = 0; i < pointCloud.Length; i++)
            {
                pointCloud[i] = new Point(points[i], colors[i]);
            }

            pointCloudOctree = new PointOctree<Point>(25f, Vector3.zero, 5);

            foreach (Point point in pointCloud)
            {
                pointCloudOctree.Add(point, point.position);
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
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            mf.mesh = mesh;

            Material mat = new Material(Shader.Find("Unlit/TestPCD"));
            ComputeBuffer colors = new ComputeBuffer(c.Length, sizeof(int));
            colors.SetData(c);
            mat.SetBuffer("_Colors", colors);

            mr.material = mat;

            mc.sharedMesh = mesh;
        }

        // Update is called once per frame
        void Update()
        {
            octreeTraversal.setData(GeometryUtility.CalculateFrustumPlanes(myCamera), myCamera.transform.position);
            octreeTraversal.setQueues(toRender, toDelete, currentRendering);

            if (lastUpdate != octreeTraversal.GetLastUpdate())
            {
                (toRender, toDelete, visibleNodeBounds) = octreeTraversal.getData();
            }

            text.text = $"toRender: {toRender.Count} -- toDelete: {toDelete.Count} -- currentRendering: {currentRendering.Count}";
        }

        private void PrepareMaterial()
        {
            float pointRadius = 2.5f;
            int renderCircles = 0;

            material = new Material(Shader.Find("Custom/QuadGeoScreenSizeShader"));
            material.enableInstancing = true;
            material.SetFloat("_PointSize", pointRadius);
            material.SetInt("_Circles", renderCircles);
            Rect screen = Camera.main.pixelRect;
            material.SetInt("_ScreenWidth", (int)screen.width);
            material.SetInt("_ScreenHeight", (int)screen.height);
        }

        private void OnApplicationQuit()
        {
            octreeTraversal.Stop();
        }

        private void OnDrawGizmos()
        {
            if (visibleNodeBounds == null) return;

            Gizmos.color = Color.red;
            for(int i = 0; i < visibleNodeBounds.Length; i++)
            {
                if (visibleNodeBounds[i] == null) break;
                Gizmos.DrawWireCube(visibleNodeBounds[i].center, visibleNodeBounds[i].size);
            }
        }
    }

}
