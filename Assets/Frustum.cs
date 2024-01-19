using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Mathematics;
using System;

public class Frustum
{
    private Plane[] planes;
    
    public Vector3 boundsSize = new Vector3(1,1,1);
    
    private Vector3[] pointCloud;
    private int[] pointCloudColors;

    private Task computeFrustumPoints;

    private Vector3[] points;
    private int[] colors;

    private float start;
    public Frustum(Vector3[] pointCloud, int[] pointCloudColors)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        this.pointCloud = pointCloud;
        this.pointCloudColors = pointCloudColors;
    }


    //public void SetPointsAndColors(Vector3[] pointCloud, int[] pointCloudColors)
    //{
    //    this.pointCloud = pointCloud;
    //    this.pointCloudColors = pointCloudColors;
    //}

    public bool TestPlanesAABB(Plane[] planes, Bounds bounds)

    {
        for (int i = 0; i < planes.Length; i++)
        {
            Plane plane = planes[i];
            float3 normal_sign = math.sign(plane.normal);
            float3 test_point = (float3)(bounds.center) + (bounds.extents * normal_sign);

            float dot = math.dot(test_point, plane.normal);
            if (dot + plane.distance < 0)
                return false;
        }

        return true;
    }

    public void SetPointsAndColors(ref Vector3[] visiblePoints, ref int[] visiblePointsColors)
    {
        visiblePoints = points;
        visiblePointsColors = colors;

        computeFrustumPoints = null;
        Debug.Log(Time.realtimeSinceStartup - start);
    }

    private Task GetPointsAsync(Plane[] planes)
    {
        return Task.Run( () => {

            List<int> indexs = new List<int>();
            for (int i = 0; i < pointCloud.Length; i++)
            {
                Bounds pointsBounds = new Bounds(pointCloud[i], boundsSize);
                if (TestPlanesAABB(planes, pointsBounds))
                    indexs.Add(i);
            }

            Vector3[] visiblePoins = new Vector3[indexs.Count];
            int[] visiblePointsColors = new int[indexs.Count];
            
            for (int j = 0; j < indexs.Count; j++)
            {
                visiblePoins[j] = pointCloud[indexs[j]];
                visiblePointsColors[j] = pointCloudColors[indexs[j]];
            }

            points = visiblePoins;
            colors = visiblePointsColors;
            //return (visiblePoins, visiblePointsColors);
        
        });
    }

    public void UpdatePoints()
    {
        if (computeFrustumPoints == null)
        {
            start = Time.realtimeSinceStartup;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            computeFrustumPoints = GetPointsAsync(planes);
        }
    }

    public bool IsDone()
    {
        return computeFrustumPoints.IsCompleted && points.Length != 0;
    }

    //private void Update()
    //{
    //    if (pointCloud == null) return;

    //    if (computeFrustumPoints == null)
    //    {
    //        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    //        computeFrustumPoints = GetPointsAsync(planes);
    //    } else
    //    {
    //        if (computeFrustumPoints.IsCompleted)
    //        {
    //            (Vector3[] pointsArray, int[] colorsArray) = computeFrustumPoints.Result;
    //            points = pointsArray;
    //            colors = colorsArray; 
    //        }
    //    }

        
    //}

    public List<int> GetPointsInsideFrustum(Vector3[] pointCloud)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        List<int> indexs = new List<int>();

        for(int i = 0; i < pointCloud.Length; i++)
        {
            if (IsPointInFrustum(pointCloud[i]))
            {
                indexs.Add(i);
            }
        }

        return indexs;
    }

    private bool IsPointInFrustum(Vector3 pointPosition)
    {
        Bounds pointsBounds = new Bounds(pointPosition, boundsSize);

        return GeometryUtility.TestPlanesAABB(planes, pointsBounds);
    }

}
