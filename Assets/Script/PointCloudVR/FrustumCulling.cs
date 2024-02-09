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
        public int totalPoints = 0;
    }

    public class FrustumCulling
    {
        Thread thread;
        FrustumParams frustumParams;

        public FrustumCulling(PointOctree OcTree, int maxPointsToRender)
        {
            frustumParams = new FrustumParams();
            frustumParams.OcTree = OcTree;
            frustumParams.maxPointsToRender = maxPointsToRender;
            frustumParams.pointsToRender = new Vector3[maxPointsToRender * 4];
            frustumParams.pointsColorsToRender = new int[maxPointsToRender * 4];
            frustumParams.normalsToRender = new Vector3[maxPointsToRender * 4];
            thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
            thread.Priority = System.Threading.ThreadPriority.Lowest;
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
                case RenderMode.QUAD:
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
                frustumParams.pointsToRender = new Vector3[maxPointsToRender * 4];
                frustumParams.pointsColorsToRender = new int[maxPointsToRender * 4];
                frustumParams.normalsToRender = new Vector3[maxPointsToRender * 4];
            }
        }

        public void Start()
        {
            thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
            thread.Priority = System.Threading.ThreadPriority.Lowest;
            frustumParams.running = true;
            thread.Start(frustumParams);
        }

        public void Stop()
        {
            lock (frustumParams)
            {
                frustumParams.running = false;
                frustumParams.pointsToRender = null;
                frustumParams.pointsColorsToRender = null;
                frustumParams.normalsToRender = null;
            }
        }

        public (Vector3[], int[], Vector3[], int) GetData()
        {
            lock (frustumParams)
            {
                return (frustumParams.pointsToRender, frustumParams.pointsColorsToRender, frustumParams.normalsToRender, frustumParams.totalPoints);
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

            //Point[] points = new Point[frustumParams.maxPointsToRender];
            Point[] quads = new Point[frustumParams.maxPointsToRender * 4];


            while (p.running)
            {
                Plane[] frustumPlanes;
                int totalPoints = 0;

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
                    octree.GetVisibleQuads(frustumPlanes, quads, cubeSize, maxPoints, ref totalPoints);
                else
                    octree.GetVisiblePoints(frustumPlanes, quads, maxPoints, ref totalPoints);



                Vector3[] visiblePoints = new Vector3[totalPoints];
                int[] visibleColors = new int[totalPoints];
                Vector3[] visibleNormals = quad ? new Vector3[totalPoints] : null;

                //for (int i = 0; i < totalPoints; i++)
                //{
                //    frustumParams.pointsToRender[i] = quads[i].position;
                //    frustumParams.pointsColorsToRender[i] = quads[i].color;
                //    frustumParams.normalsToRender[i] = quads[i].normal;
                //}

                Debug.Log($"Punti trovati: {totalPoints}");

                for (int i = 0; i < totalPoints; i++)
                {
                    visiblePoints[i] = quads[i].position;
                    visibleColors[i] = quads[i].color;
                    if (quad) visibleNormals[i] = quads[i].normal;
                }

                lock (p)
                {

                    //for (int i = 0; i < totalPoints; i++)
                    //{
                    //    frustumParams.pointsToRender[i] = quads[i].position;
                    //    frustumParams.pointsColorsToRender[i] = quads[i].color;
                    //    frustumParams.normalsToRender[i] = quads[i].normal;
                    //}
                    frustumParams.pointsToRender = visiblePoints;
                    frustumParams.pointsColorsToRender = visibleColors;
                    frustumParams.normalsToRender = visibleNormals;
                    p.totalPoints = totalPoints;
                    p.lastUpdate = DateTime.Now;
                }


            }
        }
    }

}
