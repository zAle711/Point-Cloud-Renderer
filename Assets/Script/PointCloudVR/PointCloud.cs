using PointCloudVR;
using DataStructures.ViliWonka.KDTree;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace PointCloudVR
{
    public enum RenderMode
    {
        POINT,
        CUBE
    }

    public class PointCloud : MonoBehaviour
    {
        public string fileName = "test.txt";
        public RenderMode renderMode;
        public DataStructure dataStructure;
        public float cubeSize = 0.05f;

        private Camera cam;

        public int maxPointsToRender = 100000;
        public float boundSize = 0.5f;
        
        //KDTree Partitions
        private GizmoBox[] gizmoBoxes;

        //Objects to Render only visible points
        private FrustumCulling frustumCulling;
        private PointCloudRenderer pointCloudRenderer;
        //Data structure to store Points
        private KDTree KdTree;
        private PointOctree<int> Octree;

        private GameObject floor;
        private TextMeshProUGUI textLabel;

        private void Awake()
        {
            pointCloudRenderer = new PointCloudRenderer();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Read PointCloud from file and create KDTree to store points.
            Debug.Log(System.IO.Path.Combine(Application.streamingAssetsPath, fileName));
            PointCloudReader.ReadFileBuild(out Vector3[] pointCloud, out int[] pointCloudColors, fileName);
            
            KdTree = new KDTree(pointCloud, pointCloudColors, 32);
            Octree = new PointOctree<int>(10, new Vector3(0, 5, 0), 1f);

            for (int i = 0; i < pointCloud.Length; i++)
            {
                Octree.Add(pointCloudColors[i], pointCloud[i]);
            }

            Debug.Log($"Punti caricati: {pointCloud.Length}");

            cam = Camera.main;
            floor = GameObject.FindGameObjectWithTag("Floor");
            textLabel = GameObject.FindGameObjectWithTag("Info").GetComponent<TextMeshProUGUI>();

            textLabel.text = $"Punti dal file: {pointCloud.Length}";

            InizializeFloor();
            frustumCulling = new FrustumCulling(Octree, KdTree, boundSize, maxPointsToRender);
            frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
            frustumCulling.Start();

            //pointCloudRenderer.SetData(pointCloud, pointCloudColors);
        }

        private void InizializeFloor()
        {
            Vector3[] meshVertex = PointCloudReader.GetFloorVertex();
            if (meshVertex == null) return;

            MeshFilter meshFilter = floor.GetComponent<MeshFilter>();
            MeshCollider meshCollider = floor.GetComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            mesh.vertices = meshVertex;
            mesh.triangles = new int[6] { 0, 1, 3, 1, 2, 3 };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }


        private void RetrieveData()
        {
            if (frustumCulling == null) return;

            var (points, colors, gizmoBoxes) = frustumCulling.GetData();

            if (points != null && colors != null)
            {
                pointCloudRenderer.SetData(points, colors);
                //textLabel.text = $"Punti Renderizzati: {points.Length}";
            }

            this.gizmoBoxes = gizmoBoxes;

        }
        // Update is called once per frame
        void Update()
        {

            RetrieveData();
            frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
            //textLabel.text = $"Punti Renderizzati: {pointCloudRenderer.GetNPoints()}";
            pointCloudRenderer.SetWorldMatrix(transform.localToWorldMatrix);
        }


        private void OnDrawGizmos()
        {
            if (gizmoBoxes == null) return;

            Handles.color = new Color(0f, 1f, 0f, 0.5f);

            for (int i = 0; i < gizmoBoxes.Length; i++)
            {
                Handles.DrawWireCube(gizmoBoxes[i].position + gizmoBoxes[i].size / 2, gizmoBoxes[i].size);
            }

            Handles.color = Color.white;
        }

        private void OnRenderObject()
        {
            if (pointCloudRenderer == null) return;
                pointCloudRenderer.Render();
        }

        private void OnValidate()
        {
            if (frustumCulling != null)
            {
                frustumCulling.SetMaxPoints(maxPointsToRender);
                pointCloudRenderer.SetTopology(renderMode);
                frustumCulling.SetRenderMode(renderMode);
                frustumCulling.SetCubeSize(cubeSize);
                frustumCulling.SetDataStructure(dataStructure);
            }
        }
    }

}
