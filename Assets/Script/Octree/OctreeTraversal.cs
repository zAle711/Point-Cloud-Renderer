using PointCloudVR;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Priority_Queue;

public class TraversalParams
{
    public int maxObjectToRender;

    public Plane[] frustumPlanes;
    public Vector3 cameraPosition;
    public Vector3 cameraForward;
    
    public SimplePriorityQueue<Chunk> toRender = new SimplePriorityQueue<Chunk>();
    public SimplePriorityQueue<Chunk> toDelete = new SimplePriorityQueue<Chunk>();
    public List<Chunk> currentRendering = new List<Chunk>();

    public SimplePriorityQueue<Chunk> nowRendering = new SimplePriorityQueue<Chunk>();

    public DateTime lastUpdate;
    public Bounds[] visibleNodesBounds;
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
        traversalParams.lastUpdate = DateTime.Now;
        traversalParams.maxObjectToRender = maxPointsToRender;
        traversalParams.running = true;
        thread = new Thread(new ParameterizedThreadStart(ComputeVisiblePoints));

    }

    public void SetData(Plane[] frustumPlanes, Vector3 cameraPosition, Vector3 cameraForward)
    {
        lock(traversalParams)
        {
           traversalParams.frustumPlanes = frustumPlanes;
           traversalParams.cameraPosition = cameraPosition;
           traversalParams.cameraForward = cameraForward;
        }
    }

    public void SetQueues(SimplePriorityQueue<Chunk> toRender, SimplePriorityQueue<Chunk> toDelete, SimplePriorityQueue<Chunk> nowRendering)
    {
        lock (traversalParams)
        {
            traversalParams.toRender = toRender;
            traversalParams.toDelete = toDelete;
            traversalParams.nowRendering = nowRendering;
        }
    }

    public DateTime GetLastUpdate()
    {
        lock(traversalParams)
        {
            return traversalParams.lastUpdate;
        }
    }

    public (SimplePriorityQueue<Chunk>, SimplePriorityQueue<Chunk>, Bounds[]) getData()
    {
        lock (traversalParams) 
        {
            return (traversalParams.toRender, traversalParams.toDelete, traversalParams.visibleNodesBounds);
        }
    }

    private void ComputeVisiblePoints(object obj)
    {
        TraversalParams p = (TraversalParams)obj;

        Plane[] frustumPlanes;
        Vector3 cameraPosition;
        Vector3 cameraForward;
        int maxObjToRender;
        SimplePriorityQueue<Chunk> toRender;
        SimplePriorityQueue<Chunk> toDelete;
        SimplePriorityQueue<Chunk> nowRendering;

        while (p.running)
        {
           
            lock (p)
            {
                frustumPlanes = p.frustumPlanes;
                cameraPosition = p.cameraPosition;
                cameraForward = p.cameraForward;
                maxObjToRender  = p.maxObjectToRender;
                toRender = p.toRender;
                toDelete = p.toDelete;
                nowRendering = p.nowRendering;
            }

            octree.CalculatePointsInsideFrustum(frustumPlanes, cameraPosition, cameraForward, maxObjToRender, ref toRender, ref nowRendering, ref toDelete);

            lock (p)
            {
                p.toRender = toRender;
            }

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
