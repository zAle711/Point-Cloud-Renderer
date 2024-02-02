using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace PointCloudVR
{
    public static class PointCloudReader
    {
        private static float minX = float.MaxValue;
        private static float maxX = float.MinValue;
        private static float minY = float.MaxValue;
        private static float minZ = float.MaxValue;
        private static float maxZ = float.MinValue;


        public static Point[] ReadFileWithNormals(out Vector3[] points, out int[] colors, out Vector3[] normals, string fileName, float quadSize, bool invertXY = true)
        {
            List<Point> all_points = new List<Point>();

            string filePath = Path.Combine("D:/VR_Training/PointCloud", fileName);
            Debug.Log($"FIle da caricare: {filePath}");
            StreamReader inp_stm = new StreamReader(filePath);

            int offsetY = invertXY ? 2 : 1;
            int offsetZ = invertXY ? 1 : 2;

            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                string[] coords = inp_ln.Split();

                try
                {
                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                    float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);

                    int color = int.Parse(coords[3]);

                    float xn = float.Parse(coords[4], CultureInfo.InvariantCulture);
                    float yn = float.Parse(coords[4 + offsetY], CultureInfo.InvariantCulture);
                    float zn = float.Parse(coords[4 + offsetZ], CultureInfo.InvariantCulture);

                    Vector3 point_position = new Vector3(x, y, z);
                    UpdateMinMaxValues(point_position);
                    Vector3 point_normal = Util.GetNormalVector(xn, yn, zn);

                    //all_points.AddRange(AddFaceWithNormal(point_position, color, point_normal, quadSize));
                    all_points.Add(new Point(point_position, color, point_normal));

                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }

            points = new Vector3[all_points.Count];
            colors = new int[all_points.Count];
            normals = new Vector3[all_points.Count];

            for(int i = 0; i < all_points.Count; i++)
            {
                points[i] = all_points[i].position;
                colors[i] = all_points[i].color;
                normals[i] = all_points[i].normal;
            }

            return all_points.ToArray();
        }



        private static void UpdateMinMaxValues(Vector3 point)
        {
            maxX = maxX > point.x ? maxX : point.x;
            minX = minX < point.x ? minX : point.x;
            minY = minY < point.y ? minY : point.y;
            maxZ = maxZ > point.z ? maxZ : point.z;
            minZ = minZ < point.z ? minZ : point.z;
        }

        public static void CreateFloor (GameObject floor)
        {
            Vector3[] v = new Vector3[4];

            if (maxX == float.MinValue || minX == float.MaxValue || minY == float.MaxValue || maxZ == float.MinValue || minZ == float.MaxValue)
            {
                return;
            }

            v[0] = new Vector3(minX, minY - 0.025f, minZ);
            v[1] = new Vector3(minX, minY - 0.025f, maxZ);
            v[2] = new Vector3(maxX, minY - 0.025f, maxZ);
            v[3] = new Vector3(maxX, minY - 0.025f, minZ);
            
            
            MeshFilter meshFilter = floor.GetComponent<MeshFilter>();
            MeshCollider meshCollider = floor.GetComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            mesh.vertices = v;
            mesh.triangles = new int[6] { 0, 1, 3, 1, 2, 3 };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public static Point[] AddFaceWithNormal(Vector3 position, int color, Vector3 normal, float faceSize)
        {
            Point[] quadVertices = new Point[4];

            if (normal.x == 1 || normal.x == -1)
            {
                return Util.XFace(position, color, normal, faceSize);
            } 
            else if (normal.y == 1 || normal.y == -1)
            {
                return Util.YFace(position, color, normal, faceSize);
            } 
            else if (normal.z == 1 || normal.z == -1)
            {
                return Util.ZFace(position, color, normal, faceSize);
            }

            return quadVertices;
        }

    }

}
