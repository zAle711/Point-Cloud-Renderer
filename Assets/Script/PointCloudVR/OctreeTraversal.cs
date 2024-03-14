using PointCloudVR;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TraversalParams
{
    public int maxObjectToRender;

    public Plane[] frustumPlanes;
    public Vector3 cameraPosition;
    
    public PriorityQueue<Chunk> toRender = new PriorityQueue<Chunk>();
    public PriorityQueue<Chunk> toDelete = new PriorityQueue<Chunk>();
    public List<Chunk> currentRendering = new List<Chunk>();

    public bool running = true;
}

public class OctreeTraversal
{
    Thread thread;
    TraversalParams traversalParams;

    PointOctree<Point> octree;

    public OctreeTraversal(PointOctree<Point> octree, int maxPointsToRender)
    {
        this.octree = octree;
        traversalParams = new TraversalParams();

        traversalParams.maxObjectToRender = maxPointsToRender;
        traversalParams.running = true;
        thread = new Thread(new ParameterizedThreadStart(ComputeVisiblePoints));

    }

    public void setData(Plane[] frustumPlanes, Vector3 cameraPosition)
    {
        lock(traversalParams)
        {
           traversalParams.frustumPlanes = frustumPlanes;
           traversalParams.cameraPosition = cameraPosition;
        }
    }

    public void setQueues(PriorityQueue<Chunk> toRender, PriorityQueue<Chunk> toDelete, List<Chunk> currentRendering)
    {
        lock(traversalParams)
        {
            traversalParams.toRender = toRender;
            traversalParams.toDelete = toDelete;
            traversalParams.currentRendering = currentRendering;
        }
    }

    public (PriorityQueue<Chunk>, PriorityQueue<Chunk>) getQueues()
    {
        lock (traversalParams) 
        {
            return (traversalParams.toRender, traversalParams.toDelete);
        }
    }

    private void ComputeVisiblePoints(object obj)
    {
        TraversalParams p = (TraversalParams)obj;

        while(p.running)
        {
            Plane[] frustumPlanes;
            Vector3 cameraPosition;
            int maxObjToRender;
            List<Chunk> currentRendering;
            lock (p)
            {
                frustumPlanes = p.frustumPlanes;
                cameraPosition = p.cameraPosition;
                maxObjToRender  = p.maxObjectToRender;
                currentRendering = new List<Chunk>(p.currentRendering);

            }

            PriorityQueue<Chunk> toRender = new PriorityQueue<Chunk>();
            PriorityQueue<Chunk> toDelete = new PriorityQueue<Chunk> ();


            List<Chunk> visibleChunks = octree.CalculatePointsInsideFrustum(frustumPlanes, cameraPosition, maxObjToRender, ref toRender, ref toDelete, ref currentRendering);


            lock (p)
            {
                p.toRender = toRender;
                p.toDelete = toDelete;
            }


            //Debug.Log($"GameObject visibili: {toRender.Count} -- Camera position:{cameraPosition}");

        }
    }

    public void Start()
    {
        thread.Start(traversalParams);
    }

    public void Stop()
    {
        lock(traversalParams)
        {
            traversalParams.running = false;
        }
    }

}
