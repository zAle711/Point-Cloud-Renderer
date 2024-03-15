using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Linq;

namespace PointCloudVR
{
    public static class PointCloudReader
    {
        private static float minX = float.MaxValue;
        private static float maxX = float.MinValue;
        private static float minY = float.MaxValue;
        private static float minZ = float.MaxValue;
        private static float maxZ = float.MinValue;

        public static (Vector3[], Color[]) ReadPCDFile(string fileName, bool invertXY = true)
        {
            int HEADER_SIZE = 11;
           
            List<Vector3> p = new List<Vector3>();
            List<Color> c = new List<Color>();

            string filePath = Path.Combine("D:/VR_Training/PointCloud", fileName);

            StreamReader inp_stm = new StreamReader(filePath);
           
            int offsetY = invertXY ? 2 : 1;
            int offsetZ = invertXY ? 1 : 2;
            int i = 0;
            int pointCloudSize = 0;
            while (i < HEADER_SIZE)
            {
                string inp_ln = inp_stm.ReadLine();
                string[] coords = inp_ln.Split();

                if (coords[0] == "POINTS")
                {
                    pointCloudSize = int.Parse(coords[1]);
                }

                i += 1;
            }

            Vector3[] points = new Vector3[pointCloudSize];
            Color[] colors = new Color[pointCloudSize];

            i = 0;
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                string[] coords = inp_ln.Split(); 

                try
                {

                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                    float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);
                    Vector3 point_position = new Vector3(x, y, z);
                    int color = int.Parse(coords[3]);

                    points[i] = point_position;
                    colors[i] = Util.getColor(color);
                    UpdateMinMaxValues(point_position);
                    i += 1;
                }
                catch { }
            }
            Debug.Log($"Point Cloud Size: {pointCloudSize} points Length: {points.Length} colors Length: {colors.Length}");
            CreateFloor();
            return (points, colors);
            
        }

        public static void ReadPLYFile(out Vector3[] vertices, out int[] colors, out Vector3[] normals, out int[] triangles, string fileName, bool invertXY = true)
        {
            int headerLength = 0;
            int totalVertices = 0;

            string filePath = Path.Combine("D:/VR_Training/Mesh", fileName);
            StreamReader inp_stm = new StreamReader(filePath);

            List<Vector3> p = new List<Vector3>();
            List<int> c = new List<int>();
            List<Vector3> n = new List<Vector3>();
            List<int> t = new List<int>();

            bool header = true;

            int offsetY = invertXY ? 2 : 1;
            int offsetZ = invertXY ? 1 : 2;
            int j = 0;
            while (!inp_stm.EndOfStream)
            {
                j += 1;
                string inp_ln = inp_stm.ReadLine();
                if (header)
                {
                    string[] args = inp_ln.Split();

                    if (args[0] == "element" && args[1] == "vertex")
                    {
                        totalVertices = int.Parse(args[2]);
                        Debug.Log("Vertici totali nella Mesh: " + totalVertices);
                    }

                    if (inp_ln == "end_header")
                    {
                        header = false;
                    }

                    headerLength += 1;
                    continue;
                }

                string[] coords = inp_ln.Split();
                if (j <= totalVertices + headerLength)
                {
                    try
                    {
                        float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                        float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                        float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);

                        c.Add(Util.encodeColor(int.Parse(coords[6]), int.Parse(coords[7]), int.Parse(coords[8])));

                        float xn = float.Parse(coords[3], CultureInfo.InvariantCulture);
                        float yn = float.Parse(coords[3 + offsetY], CultureInfo.InvariantCulture);
                        float zn = float.Parse(coords[3 + offsetZ], CultureInfo.InvariantCulture);

                        p.Add(new Vector3(x, y, z));

                        n.Add(new Vector3(xn, yn, zn));
                        //all_points.AddRange(AddFaceWithNormal(point_position, color, point_normal, quadSize));

                    }
                    catch { }
                    
                }
                else
                {
                    int i1 = int.Parse(coords[1]);
                    int i2 = int.Parse(coords[2]);
                    int i3 = int.Parse(coords[3]);

                    t.Add(i1);
                    t.Add(i2);
                    t.Add(i3);
                }
            }
            Debug.Log($"Vertici: {p.Count} Colori: {c.Count} Normali: {n.Count} Triangoli: {t.Count}");
            vertices = p.ToArray();
            colors = c.ToArray();
            normals = n.ToArray();

            triangles = t.ToArray();
        }

        private static void UpdateMinMaxValues(Vector3 point)
        {
            maxX = maxX > point.x ? maxX : point.x;
            minX = minX < point.x ? minX : point.x;
            minY = minY < point.y ? minY : point.y;
            maxZ = maxZ > point.z ? maxZ : point.z;
            minZ = minZ < point.z ? minZ : point.z;
        }

        public static void CreateFloor ()
        {
            GameObject prefab = Resources.Load("Floor", typeof(GameObject)) as GameObject;
            GameObject floor = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, GameObject.FindWithTag("Enviroment").transform);
            Vector3[] v = new Vector3[4];
            float offset = 1.5f;
            if (maxX == float.MinValue || minX == float.MaxValue || minY == float.MaxValue || maxZ == float.MinValue || minZ == float.MaxValue)
            {
                return;
            }

            v[0] = new Vector3(minX - offset, minY - 0.25f, minZ - offset);
            v[1] = new Vector3(minX - offset, minY - 0.25f, maxZ + offset);
            v[2] = new Vector3(maxX + offset, minY - 0.25f, maxZ + offset);
            v[3] = new Vector3(maxX + offset, minY - 0.25f, minZ - offset);
            
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

    }

}
