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

    public void setData(Plane[] frustumPlanes, Vector3 cameraPosition)
    {
        lock(traversalParams)
        {
           traversalParams.frustumPlanes = frustumPlanes;
           traversalParams.cameraPosition = cameraPosition;
        }
    }

    public void SetQueue(SimplePriorityQueue<Chunk> nowRendering)
    {
        lock (traversalParams)
        {
            traversalParams.nowRendering = nowRendering;
        }
    }

    public void setQueues(SimplePriorityQueue<Chunk> toRender, SimplePriorityQueue<Chunk> toDelete, SimplePriorityQueue<Chunk> nowRendering)
    {
        lock(traversalParams)
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
        //SimplePriorityQueue<Chunk> toRender = new SimplePriorityQueue<Chunk>();
        //SimplePriorityQueue<Chunk> toDelete = new SimplePriorityQueue<Chunk>();

        Plane[] frustumPlanes;
        Vector3 cameraPosition;
        int maxObjToRender;
        SimplePriorityQueue<Chunk> toRender;
        SimplePriorityQueue<Chunk> toDelete;
        SimplePriorityQueue<Chunk> nowRendering;
        List<Chunk> currentRendering = new List<Chunk>();

        while (p.running)
        {
           
            lock (p)
            {
                frustumPlanes = p.frustumPlanes;
                cameraPosition = p.cameraPosition;
                maxObjToRender  = p.maxObjectToRender;
                ////currentRendering = p.currentRendering.Count != 0 ? new List<Chunk>(p.currentRendering) : new List<Chunk>();
                //toRender = p.toRender;
                //toDelete = p.toDelete;
                nowRendering = p.nowRendering;
            }
            toRender = new SimplePriorityQueue<Chunk>();
            toDelete = new SimplePriorityQueue<Chunk>();
            //toRender.Clear();
            //toDelete.Clear();

            Bounds[] visibleNodeBounds = new Bounds[maxObjToRender];


            octree.CalculatePointsInsideFrustum(frustumPlanes, cameraPosition, maxObjToRender, ref toRender, ref toDelete, ref nowRendering, ref visibleNodeBounds);


            lock (p)
            {
                p.toRender = toRender;
                p.toDelete = toDelete;
                //p.visibleNodesBounds = visibleNodeBounds;
                p.lastUpdate = DateTime.Now;

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
