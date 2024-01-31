using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PointCloudVR
{
    class FrustumParams
    {
        
        public PointOctree OcTree;

        //Frustum Culling Params
        public Plane[] frustumPlanes;
        
        public int maxPointsToRender;

        //Params To Render Only visible Cube Faces
        public Vector3 cameraPosition;
        //public Vector3 cameraDirection;

        public bool running = true;

        public float fov;
        public bool quad = false;
        public float quadSize = 0.05f;

        public DateTime lastUpdate;

        public Vector3[] pointsToRender;
        public int[] pointsColorsToRender;
        public Vector3[] normalsToRender;

    }

    public class FrustumCulling
    {
        Thread thread;
        FrustumParams frustumParams;

        public FrustumCulling(PointOctree OcTree , float size, int maxPointsToRender)
        {
            frustumParams = new FrustumParams();
            frustumParams.OcTree = OcTree;
            frustumParams.maxPointsToRender = maxPointsToRender;
            thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
        }

        public void SetCamera(Camera camera, Matrix4x4 localWorldMatrix)
        {
            lock (frustumParams)
            {
                frustumParams.cameraPosition = camera.transform.position;
                frustumParams.frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix * localWorldMatrix);
                //FORSE ALTEZZA SCHERMO
            }
        }

        public void SetRenderMode(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.POINT:
                    lock (frustumParams)
                    {
                        frustumParams.quad = false;
                    }
                    break;
                case RenderMode.CUBE:
                    lock (frustumParams)
                    {
                        frustumParams.quad = true;
                    }
                    break;

                default:
                    break;

            }
        }

        public void SetCubeSize(float size)
        {
            lock (frustumParams)
            {
                frustumParams.quadSize = size;
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

        public (Vector3[], int[], Vector3[]) GetData()
        {
            lock (frustumParams)
            {
                return (frustumParams.pointsToRender, frustumParams.pointsColorsToRender, frustumParams.normalsToRender);
            }
        }

        public DateTime GetLastUpdateTime()
        {
            lock(frustumParams)
            {
                return frustumParams.lastUpdate;
            }
        }

        private void ComputeInsidePoints(object obj)
        {
            FrustumParams p = (FrustumParams)obj;
            PointOctree octree = p.OcTree;

            //void GetDataFromResult(Point[] points, bool quad, out ComputeBuffer cbP, out ComputeBuffer cbC, out ComputeBuffer cbN)
            //{
            //    cbP = new ComputeBuffer(points.Length, 3 * sizeof(float));
            //    cbC = new ComputeBuffer(points.Length, sizeof(int));
            //    cbN = quad ? new ComputeBuffer(points.Length, 3 * sizeof(float)) : null;

            //    Vector3[] p = new Vector3[points.Length];
            //    int[] c = new int[points.Length];
            //    Vector3[] n = quad ? new Vector3[points.Length] : null;

            //    for(int i = 0; i < points.Length; i++)
            //    {
            //        p[i] = points[i].position;
            //        c[i] = points[i].color;
            //        if (quad) n[i] = points[i].normal;
            //    }

            //    cbP.SetData(p);
            //    cbC.SetData(c);

            //    if (quad) cbN.SetData(n);

            //}

            while (p.running)
            {
                Plane[] frustumPlanes;
                Point[] points;
                
                bool quad;
                float cubeSize;
                int maxPoints;

                lock (p)
                {
                    frustumPlanes = p.frustumPlanes;
                    quad = p.quad;
                    cubeSize = p.quadSize;
                    maxPoints = p.maxPointsToRender;
                }

                if (quad)
                    points = octree.GetVisibleQuads(frustumPlanes, cubeSize, maxPoints);
                else
                    points = octree.GetVisiblePoints(frustumPlanes, maxPoints);

                Vector3[] visiblePoints = new Vector3[points.Length];
                int[] visibleColors = new int[points.Length];
                Vector3[] visibleNormals = quad ? new Vector3[points.Length] : null;

                for(int i = 0; i < points.Length; i++)
                {
                    visiblePoints[i] = points[i].position;
                    visibleColors[i] = points[i].color;
                    if (quad) visibleNormals[i] = points[i].normal;
                }

                lock (p)
                {
                    p.pointsToRender = visiblePoints;
                    p.pointsColorsToRender = visibleColors;
                    p.normalsToRender = visibleNormals;
                    p.lastUpdate = DateTime.Now;
                }
                //ComputeBuffer visibleP;
                //ComputeBuffer visibleC;
                //ComputeBuffer visibleN;

                //GetDataFromResult(points, quad, out visibleP, out visibleC, out visibleN);

                //lock (p)
                //{
                //    if (p.p != null) p.p.Release();
                //    if (p.c != null) p.c.Release();
                //    if (p.n != null) p.n.Release();

                //    p.p = visibleP;
                //    p.c = visibleC;
                //    p.n = visibleN;
                //    p.lastUpdate = DateTime.Now;
                //}

            }
        }
    }

}
