using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

class FrustumParams
{
    public Plane[] frustumPlanes;
    public Vector3 cameraPosition;

    public Bounds[] bounds;

    public Vector3[] pointCloud;
    public int[] pointCloudColors;
    public KDTree tree;
    public float boundSize;
    public float fov;
    public bool running = true;

    public Vector3[] pointsToRender;
    public int[] pointsColorsToRender;

    public int maxPointsToRender;

    public List<(Vector3, Vector3)> gizmoBounds;
}
public class FrustumCulling
{
    Thread thread;
    FrustumParams frustumParams;

    public FrustumCulling(Vector3[] pointCloud, int[] colors, KDTree tree, float size, int maxPointsToRender)
    {
        frustumParams = new FrustumParams();
        frustumParams.pointCloud = pointCloud;
        frustumParams.pointCloudColors = colors;
        frustumParams.tree = tree;
        frustumParams.boundSize = size;
        frustumParams.bounds = CreateBounds();
        frustumParams.gizmoBounds = new List<(Vector3, Vector3)>();
        frustumParams.maxPointsToRender = maxPointsToRender;
        thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
    }

    private Bounds[] CreateBounds()
    {
        Bounds[] bounds = new Bounds[frustumParams.pointCloud.Length];
        Vector3 size = new Vector3(frustumParams.boundSize, frustumParams.boundSize, frustumParams.boundSize);
        for(int i = 0; i < frustumParams.pointCloud.Length; i++)
        {
            bounds[i] = new Bounds(frustumParams.pointCloud[i], size);
        }
        return bounds;
    }

    public void SetCamera(Camera camera, Matrix4x4 localWorldMatrix)
    {
        lock (frustumParams)
        {
            frustumParams.cameraPosition = camera.transform.position;
            frustumParams.frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix * localWorldMatrix);
            frustumParams.fov = camera.fieldOfView;
            //FORSE ALTEZZA SCHERMO
        }
    }

    public void SetMaxPoints(int maxPointsToRender)
    {
        lock (frustumParams)
        {
            frustumParams.maxPointsToRender = maxPointsToRender;
        }
    }

    public void Start()
    {
        thread.Start(frustumParams);
    }

    public void Stop()
    {
        lock (frustumParams)
        {
            frustumParams.running = false;
        }
    }

    public (Vector3[], int[], List<(Vector3, Vector3)>) GetData()
    {
        lock (frustumParams)
        {
            return (frustumParams.pointsToRender, frustumParams.pointsColorsToRender, frustumParams.gizmoBounds);
        }
    }

    private void ComputeInsidePoints(object obj)
    {
        
        FrustumParams p = (FrustumParams)obj;
        Debug.Log(p.frustumPlanes.Length);
        KDQuery query = new KDQuery(); 
        while (p.running)
        {
            List<(Vector3, Vector3)> bounds = new List<(Vector3, Vector3)>();
            List<Vector3> points = new List<Vector3>();
            List<int> colors = new List<int>();

            query.Check(frustumParams.tree, frustumParams.cameraPosition, bounds, frustumParams.frustumPlanes, points, colors, frustumParams.maxPointsToRender);

            //for(int i = 0; i < p.pointCloud.Length;i++)
            //{
            //    if (ale711.Util.TestPlanesAABB(frustumParams.frustumPlanes, frustumParams.bounds[i]))
            //    {
            //        points.Add(p.pointCloud[i]);
            //        colors.Add(p.pointCloudColors[i]);
            //    }
            //}

            lock (p)
            {
                p.pointsToRender = points.ToArray();
                p.pointsColorsToRender = colors.ToArray();
                p.gizmoBounds = bounds;
            }

        }
    }
}
