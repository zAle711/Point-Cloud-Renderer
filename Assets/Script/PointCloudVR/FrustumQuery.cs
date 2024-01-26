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
using PointCloudVR;

namespace DataStructures.ViliWonka.KDTree
{

    public partial class KDQuery
    {
        public FrustumQueryResult Check(KDTree tree, Vector3 cameraPosition, Vector3 cameraDirection, Plane[] frustumPlanes, int maxPointsToRender, bool quad = false, float cubeSize = 0.025f)
        {

            Reset();

            List<Vector3> p = new List<Vector3>();
            List<int> c = new List<int>();

            List<GizmoBox> gizmoBoxes = new List<GizmoBox>();

            Vector3[] points = tree.Points;
            int[] colors = tree.Colors;
            int[] permutation = tree.Permutation;

            var rootNode = tree.RootNode;

            PushToQueue(rootNode, rootNode.bounds.ClosestPoint(cameraPosition));
            //PushToHeap(rootNode, rootNode.bounds.ClosestPoint(forseCamera), forseCamera);

            KDQueryNode queryNode = null;
            KDNode node = null;

            while (LeftToProcess > 0)
            {
                if (p.Count >= maxPointsToRender) break;

                queryNode = PopFromQueue();
                //queryNode = PopFromHeap();
                node = queryNode.node;

                if (!node.Leaf)
                {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;
                    if (Util.TestPlanesAABB(frustumPlanes, node.negativeChild.bounds.Bounds))
                    {

                        PushToQueue(node.negativeChild, tempClosestPoint);
                        //PushToHeap(node.negativeChild, queryNode.tempClosestPoint, forseCamera);
                        //result.Add((node.negativeChild.bounds.min, node.negativeChild.bounds.size));

                        gizmoBoxes.Add( new GizmoBox(node.negativeChild.bounds.min, node.negativeChild.bounds.size));
                    }


                    if (Util.TestPlanesAABB(frustumPlanes, node.positiveChild.bounds.Bounds))
                    {

                        PushToQueue(node.positiveChild, tempClosestPoint);
                        //PushToHeap(node.positiveChild, queryNode.tempClosestPoint, forseCamera);
                        gizmoBoxes.Add(new GizmoBox(node.positiveChild.bounds.min, node.positiveChild.bounds.size));
                    }

                }
                else
                {
                    for (int i = node.start; i < node.end; i++)
                    {

                        int index = permutation[i];

                        if (quad)
                        {
                            var (quadPoints, quadColors) = Util.GetVisibleFacesOfCube(points[index], colors[index], cubeSize, cameraDirection);
                            p.AddRange(quadPoints);
                            c.AddRange(quadColors);
                        }
                        else
                        {
                            p.Add(points[index]);
                            c.Add(colors[index]);
                        }
                    }
                }
            }

            FrustumQueryResult queryResult = new FrustumQueryResult(p, c, gizmoBoxes);
            return queryResult;
        }

    }
}