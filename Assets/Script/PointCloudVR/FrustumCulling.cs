using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace PointCloudVR
{
    class FrustumParams
    {
        
        public PointOctree<int> OcTree;
        //Frustum Culling Params
        public Plane[] frustumPlanes;
        public Bounds[] bounds;
        public float boundSize;
        public int maxPointsToRender;

        //Params To Render Only visible Cube Faces
        public Vector3 cameraPosition;
        public Vector3 cameraDirection;

        public bool running = true;

        public float fov;
        public bool quad = false;
        public float quadSize = 0.05f;
        //Results of KDQuery
        public Vector3[] pointsToRender;
        public int[] pointsColorsToRender;
        public GizmoBox[] gizmoBoxes;

    }

    public class FrustumCulling
    {
        Thread thread;
        FrustumParams frustumParams;

        public FrustumCulling(PointOctree<int> OcTree , float size, int maxPointsToRender)
        {
            frustumParams = new FrustumParams();
            frustumParams.OcTree = OcTree;
            frustumParams.boundSize = size;
            frustumParams.maxPointsToRender = maxPointsToRender;
            thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
        }

        public void SetCamera(Camera camera, Matrix4x4 localWorldMatrix)
        {
            lock (frustumParams)
            {
                frustumParams.cameraPosition = camera.transform.position;
                frustumParams.frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix * localWorldMatrix);
                frustumParams.fov = camera.fieldOfView;
                frustumParams.cameraDirection = camera.transform.forward;
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

        public (Vector3[], int[]) GetData()
        {
            lock (frustumParams)
            {
                return (frustumParams.pointsToRender, frustumParams.pointsColorsToRender);
            }
        }

        private void ComputeInsidePoints(object obj)
        {
            FrustumParams p = (FrustumParams)obj;
            PointOctree<int> octree = p.OcTree;

            while (p.running)
            {
                Vector3 cameraPosition;
                Vector3 cameraDirection;
                Plane[] frustumPlanes;
                bool quad;
                float cubeSize;

                lock (p)
                {
                    cameraPosition = p.cameraPosition;
                    cameraDirection = p.cameraDirection;
                    frustumPlanes = p.frustumPlanes;
                    quad = p.quad;
                    cubeSize = p.quadSize;
                }

                var (colors, points) = octree.GetVisiblePoints(frustumPlanes);

                lock (p)
                {
                    p.pointsColorsToRender = colors;
                    p.pointsToRender = points;
                }           

            }
        }
    }

}
