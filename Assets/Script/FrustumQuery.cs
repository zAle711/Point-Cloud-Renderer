/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataStructures.ViliWonka.KDTree
{

    public partial class KDQuery
    {
        //public void Check(KDTree tree, Vector3 forseCamera, List<(Vector3, Vector3)> result, Plane[] frustumPlanes, out Vector3[] shaderPoints, out int[] shaderColors)
        //{

        //    Reset();

        //    Vector3[] points = tree.Points;
        //    int[] permutation = tree.Permutation;

        //    //float squaredRadius = queryRadius * queryRadius;

        //    var rootNode = tree.RootNode;

        //    PushToQueue(rootNode, rootNode.bounds.ClosestPoint(forseCamera));

        //    KDQueryNode queryNode = null;
        //    KDNode node = null;

        //    List<Vector3> p = new List<Vector3>();
        //    List<int> c = new List<int>();

        //    // KD search with pruning (don't visit areas which distance is more away than range)
        //    // Recursion done on Stack
        //    while (LeftToProcess > 0)
        //    {

        //        queryNode = PopFromQueue();
        //        node = queryNode.node;
                

        //        if (!node.Leaf)
        //        {

        //            int partitionAxis = node.partitionAxis;
        //            float partitionCoord = node.partitionCoordinate;

        //            Vector3 tempClosestPoint = queryNode.tempClosestPoint;

        //            if (ale711.Util.TestPlanesAABB(frustumPlanes, node.negativeChild.bounds.Bounds))
        //            {
        //                PushToQueue(node.negativeChild, tempClosestPoint);
        //                result.Add((node.negativeChild.bounds.min, node.negativeChild.bounds.size));
        //            }
                        

        //            if (ale711.Util.TestPlanesAABB(frustumPlanes, node.positiveChild.bounds.Bounds))
        //            {
        //                PushToQueue(node.positiveChild, tempClosestPoint);
        //                result.Add((node.positiveChild.bounds.min, node.positiveChild.bounds.size));
        //            }



        //        }
        //        else
        //        {
                    



        //            for (int i = node.start; i < node.end; i++)
        //            {

        //                int index = permutation[i];
        //                p.Add(points[index]);
        //                c.Add(ale711.Util.encodeColor(255,255,255));

        //            }

        //        }
        //    }

        //    shaderPoints = p.ToArray();
        //    shaderColors = c.ToArray();
        //}

        public void Check(KDTree tree, Vector3 forseCamera, List<(Vector3, Vector3)> result, Plane[] frustumPlanes, List<Vector3> p, List<int> c, int maxPointsToRender)
        {

            Reset();

            p.Clear();
            c.Clear();

            Vector3[] points = tree.Points;
            int[] permutation = tree.Permutation;

            //float squaredRadius = queryRadius * queryRadius;

            var rootNode = tree.RootNode;

            PushToQueue(rootNode, rootNode.bounds.ClosestPoint(forseCamera));
            //PushToHeap(rootNode, rootNode.bounds.ClosestPoint(forseCamera), forseCamera);

            KDQueryNode queryNode = null;
            KDNode node = null;
            int depth = 0;
            //List<Vector3> p = new List<Vector3>();
            //List<int> c = new List<int>();

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while (LeftToProcess > 0)
            {
                if (p.Count >= maxPointsToRender) break;

                queryNode = PopFromQueue();
                //queryNode = PopFromHeap();
                node = queryNode.node;

                //if (depth == 250) break;

                if (!node.Leaf)
                {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;
                    bool deeper = false;
                    if (ale711.Util.TestPlanesAABB(frustumPlanes, node.negativeChild.bounds.Bounds))
                    {
                        deeper = true;
                        PushToQueue(node.negativeChild, tempClosestPoint);
                        //PushToHeap(node.negativeChild, queryNode.tempClosestPoint, forseCamera);
                        result.Add((node.negativeChild.bounds.min, node.negativeChild.bounds.size));
                    }


                    if (ale711.Util.TestPlanesAABB(frustumPlanes, node.positiveChild.bounds.Bounds))
                    {
                        deeper = true;
                        PushToQueue(node.positiveChild, tempClosestPoint);
                        //PushToHeap(node.positiveChild, queryNode.tempClosestPoint, forseCamera);
                        result.Add((node.positiveChild.bounds.min, node.positiveChild.bounds.size));
                    }

                    if (deeper) depth++;
                }
                else
                {
                    for (int i = node.start; i < node.end; i++)
                    {

                        int index = permutation[i];
                        p.Add(points[index]);
                        c.Add(ale711.Util.encodeColor(255, 255, 255));

                    }

                

                }
            }

            //shaderPoints = p.ToArray();
            //shaderColors = c.ToArray();
        }

    }
}