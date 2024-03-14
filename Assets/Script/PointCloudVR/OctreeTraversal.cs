using PointCloudVR;
using System;
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

    public void setQueues(PriorityQueue<Chunk> toRender, PriorityQueue<Chunk> toDelete, List<Chunk> currentRendering)
    {
        lock(traversalParams)
        {
            traversalParams.toRender = toRender;
            traversalParams.toDelete = toDelete;
            traversalParams.currentRendering = currentRendering;
        }
    }

    public DateTime GetLastUpdate()
    {
        lock(traversalParams)
        {
            return traversalParams.lastUpdate;
        }
    }

    public (PriorityQueue<Chunk>, PriorityQueue<Chunk>, Bounds[]) getData()
    {
        lock (traversalParams) 
        {
            return (traversalParams.toRender, traversalParams.toDelete, traversalParams.visibleNodesBounds);
        }
    }

    private void ComputeVisiblePoints(object obj)
    {
        TraversalParams p = (TraversalParams)obj;
        PriorityQueue<Chunk> toRender = new PriorityQueue<Chunk>();
        PriorityQueue<Chunk> toDelete = new PriorityQueue<Chunk>();

        Plane[] frustumPlanes;
        Vector3 cameraPosition;
        int maxObjToRender;
        List<Chunk> currentRendering = new List<Chunk>();

        while (p.running)
        {
           
            lock (p)
            {
                frustumPlanes = p.frustumPlanes;
                cameraPosition = p.cameraPosition;
                maxObjToRender  = p.maxObjectToRender;
                currentRendering = p.currentRendering.Count != 0 ? new List<Chunk>(p.currentRendering) : new List<Chunk>();

            }

            toRender.Clear();
            toDelete.Clear();

            Bounds[] visibleNodeBounds = new Bounds[maxObjToRender];


            octree.CalculatePointsInsideFrustum(frustumPlanes, cameraPosition, 0, ref toRender, ref toDelete, ref currentRendering, ref visibleNodeBounds);


            lock (p)
            {
                p.toRender = toRender;
                p.toDelete = toDelete;
                p.visibleNodesBounds = visibleNodeBounds;
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
