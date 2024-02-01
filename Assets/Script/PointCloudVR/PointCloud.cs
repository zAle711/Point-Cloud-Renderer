using System;
using TMPro;
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
        public string fileName = "corridoio_with_normals.pcd";
        public RenderMode renderMode;
        public float cubeSize = 0.025f;

        private Camera cam;

        public int maxPointsToRender = 100000;
        public float boundSize = 0.5f;

        //Objects to Render only visible points
        private FrustumCulling frustumCulling;
        private PointCloudRenderer pointCloudRenderer;
        
        //Data structure to store Points
        private PointOctree ocTree;

        private GameObject floor;
        private TextMeshProUGUI textLabel;

        private Point[] pointCloud;
        private Vector3[] points;
        private int[] colors;
        private Vector3[] normals;

        private DateTime lastUpdate;

        private void Awake()
        {
            pointCloudRenderer = new PointCloudRenderer();
        }

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            floor = GameObject.FindGameObjectWithTag("Floor");
            textLabel = GameObject.FindGameObjectWithTag("Info").GetComponent<TextMeshProUGUI>();
           
            //Read PointCloud from file and create Octree to store points.
            pointCloud = PointCloudReader.ReadFileWithNormals(out points, out colors, out normals, fileName, cubeSize);
            
            CreateOctree();

            textLabel.text = $"Punti caricati nell' Octree: {ocTree.Count}";

            PointCloudReader.CreateFloor(floor);
            
            frustumCulling = new FrustumCulling(ocTree, maxPointsToRender); 
            
            frustumCulling.SetCamera(cam, transform.localToWorldMatrix);

            frustumCulling.Start();

            //pointCloudRenderer.SetData(points, colors, normals);
        }

        private void CreateOctree()
        {
            ocTree = new PointOctree(25f, Vector3.zero, 2.5f);

            foreach(Point p in pointCloud)
            {
                ocTree.Add(p, p.position);
            }
        }

        private void RetrieveData()
        {
            if (frustumCulling == null) return;

            if (frustumCulling.GetLastUpdateTime() == lastUpdate)
            {
                //Debug.Log("Punti già aggiornati");
                return;
            }

            var (p, c, n) = frustumCulling.GetData();

            lastUpdate = frustumCulling.GetLastUpdateTime();
            textLabel.text = $"Vertici totali: {p.Length}";
            if (renderMode == RenderMode.POINT)
                pointCloudRenderer.SetData(p, c);
            else
                if (n != null) pointCloudRenderer.SetData(p, c, n);


        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RetrieveData();
                textLabel.text = $"adsa + {Time.realtimeSinceStartup}";
            }
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                frustumCulling.Stop();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                frustumCulling.Start();
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
            }

            //frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
            ////textLabel.text = $"Punti Renderizzati: {pointCloudRenderer.GetNPoints()}";
            //pointCloudRenderer.SetWorldMatrix(transform.localToWorldMatrix);
        }


        private void OnDrawGizmos()
        {
            //ocTree.DrawAllBounds();
            //if (gizmoBoxes == null) return;

            //Handles.color = new Color(0f, 1f, 0f, 0.5f);

            //for (int i = 0; i < gizmoBoxes.Length; i++)
            //{
            //    Handles.DrawWireCube(gizmoBoxes[i].position + gizmoBoxes[i].size / 2, gizmoBoxes[i].size);
            //}

            //Handles.color = Color.white;
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
            }
        }
    }

}

