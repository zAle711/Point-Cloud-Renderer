using DataStructures.ViliWonka.KDTree;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace PointCloudVR
{
    public enum DataStructure
    {
        KDTREE,
        OCTREE
    }
    class FrustumParams
    {
        
        public KDTree KDTree;
        public PointOctree<Vector3> OcTree;
        //Frustum Culling Params
        public Plane[] frustumPlanes;
        public Bounds[] bounds;
        public float boundSize;
        public int maxPointsToRender;

        public DataStructure dataStructure;

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

    public class FrustumQueryResult
    {
        public Vector3[] pointsToRender;
        public int[] colorPointsToRender;
        public GizmoBox[] gizmoBoxes;

        public FrustumQueryResult(List<Vector3> pointsToRender, List<int> colorPointsToRender, List<GizmoBox> gizmoBoxes)
        {
            this.pointsToRender = pointsToRender.ToArray();
            this.colorPointsToRender = colorPointsToRender.ToArray();
            this.gizmoBoxes = gizmoBoxes.ToArray();
        }
    }

    public class FrustumCulling
    {
        Thread thread;
        FrustumParams frustumParams;

        public FrustumCulling(PointOctree<Vector3> OcTree ,KDTree KDTree, float size, int maxPointsToRender)
        {
            frustumParams = new FrustumParams();
            frustumParams.OcTree = OcTree;
            frustumParams.KDTree = KDTree;
            frustumParams.boundSize = size;
            frustumParams.bounds = CreateBounds();
            frustumParams.maxPointsToRender = maxPointsToRender;
            thread = new Thread(new ParameterizedThreadStart(ComputeInsidePoints));
        }

        private Bounds[] CreateBounds()
        {
            Bounds[] bounds = new Bounds[frustumParams.KDTree.Points.Length];
            Vector3 size = new Vector3(frustumParams.boundSize, frustumParams.boundSize, frustumParams.boundSize);
            for (int i = 0; i < frustumParams.KDTree.Points.Length; i++)
            {
                bounds[i] = new Bounds(frustumParams.KDTree.Points[i], size);
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

        public (Vector3[], int[], GizmoBox[]) GetData()
        {
            lock (frustumParams)
            {
                return (frustumParams.pointsToRender, frustumParams.pointsColorsToRender, frustumParams.gizmoBoxes);
            }
        }

        public void SetDataStructure(DataStructure structure)
        {
            lock (frustumParams)
            {
                frustumParams.dataStructure = structure;
            }
        }

        private void ComputeInsidePoints(object obj)
        {

            FrustumParams p = (FrustumParams)obj;
            KDTree kDTree = p.KDTree;
            PointOctree<Vector3> octree = p.OcTree;
            KDQuery query = new KDQuery();
            Stopwatch stopwatch = new Stopwatch();


            while (p.running)
            {
                Vector3 cameraPosition;
                Vector3 cameraDirection;
                Plane[] frustumPlanes;
                bool quad;
                float cubeSize;
                DataStructure dataStructure;

                lock (p)
                {
                    cameraPosition = p.cameraPosition;
                    cameraDirection = p.cameraDirection;
                    frustumPlanes = p.frustumPlanes;
                    quad = p.quad;
                    cubeSize = p.quadSize;
                    dataStructure = p.dataStructure;
                }
              
                if (dataStructure == DataStructure.KDTREE)
                {
                    FrustumQueryResult res = query.Check(kDTree, cameraPosition, cameraDirection, frustumPlanes, p.maxPointsToRender, quad, cubeSize);

                    lock (p)
                    {
                        p.pointsToRender = res.pointsToRender;
                        p.pointsColorsToRender = res.colorPointsToRender;
                        p.gizmoBoxes = res.gizmoBoxes;
                    }

                } else
                {
                    Vector3[] visiblePoints = octree.GetVisiblePoints(frustumPlanes);

                    lock (p)
                    {
                        p.pointsToRender = visiblePoints;
                    }
                }

                

            }
        }
    }

}
