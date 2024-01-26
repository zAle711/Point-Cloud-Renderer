using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DataStructures.ViliWonka.KDTree;
using PointCloudVR;

public class TestOctree : MonoBehaviour
{
    // Start is called before the first frame update
    PointOctree<Vector3> octree;
    KDTree kdTree;
    private Vector3[] points;
    private int[] colors;

    private Plane[] planes;
    private Vector3 cameraPosition;
    private Vector3 cameraDirection;

    PointCloudVR.PointCloudRenderer pcRender;
    void Start()
    {
        Vector3 center = new Vector3(0, 5, 0);
        PointCloudVR.PointCloudReader.ReadFileBuild(out points, out colors, "global_point_cloud_unreal.txt");

        octree = new PointOctree<Vector3>(100, center, 1f);
        InsertAllPoints();
        kdTree = new KDTree(points, colors, 32);
        pcRender = new PointCloudVR.PointCloudRenderer();
       
    }

    private void InsertAllPoints()
    {
        for (int i = 0; i < points.Length; i++)
        {
            octree.Add(points[i], points[i]);
        }
    }

    private void OnDrawGizmos()
    {
        if (octree == null) return;
        octree.DrawAllBounds();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            InsertAllPoints();
            Debug.Log(octree.Count);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            cameraPosition = Camera.main.transform.position;
            cameraDirection = Camera.main.transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            KDQuery query = new KDQuery();
            float start = Time.realtimeSinceStartup;

            Vector3[] visiblePoints = octree.GetVisiblePoints(planes);

            float OcTreeElapsedTime = Time.realtimeSinceStartup - start;

            start = Time.realtimeSinceStartup;
            FrustumQueryResult queryRes = query.Check(kdTree, cameraPosition, cameraDirection, planes, 1000000000);
            
            float KDTreeElapsedTime = Time.realtimeSinceStartup - start;

            Debug.Log($"OCTree -> {OcTreeElapsedTime} - {visiblePoints.Length}");
            Debug.Log($"KDTree -> {KDTreeElapsedTime} - {queryRes.pointsToRender.Length}");
            
            //int[] c = new int[list.Length];


            //for (int i = 0; i < list.Length; i++)
            //{
            //    c[i] = PointCloudVR.Util.encodeColor(255, 255, 255);
            //}

            //pcRender.SetData(list, c);


        }

        if( Input.GetKeyDown(KeyCode.Mouse1))
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            
            float start = Time.realtimeSinceStartup;
            

            

        }
    }

    private void OnRenderObject()
    {
        if (pcRender == null) return;
        pcRender.Render();
    }
}
