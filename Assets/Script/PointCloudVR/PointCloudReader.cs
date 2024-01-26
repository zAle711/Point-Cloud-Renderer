using System;
using System.Collections;
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

        public static void ReadFileBuild(out Vector3[] points, out int[] colors, string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            //string text = System.IO.File.ReadAllText(filePath);
            StreamReader inp_stm = new StreamReader(filePath);

            //var textFile = Resources.Load<TextAsset>("point_cloud_unreal_alg");
            //List<string> lines = new List<string>(text.Split('\n'));

            bool InvertYZ = true;
            int offsetY = InvertYZ ? 2 : 1;
            int offsetZ = InvertYZ ? 1 : 2;

            List<Vector3> all_points = new List<Vector3>();
            List<int> all_colors = new List<int>();

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
                    UpdateMinMaxValues(point_position);

                    //all_points.Add(point_position);
                    int color;

                    if (coords.Length == 40)
                    {
                        color = int.Parse(coords[3]);
                        //all_colors.Add(color);
                    }
                    else
                    {
                        float normalizedY = Mathf.InverseLerp(0f, 4f, point_position.y);
                        Color c = Color.Lerp(Color.blue, Color.green, normalizedY);
                        color = Util.encodeColor((int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255));
                    }

                    all_points.Add(point_position);
                    all_colors.Add(color);

                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }

            points = all_points.ToArray();
            colors = all_colors.ToArray();
        }
        public static void ReadFile(out Vector3[] points, out int[] colors, string fileName)
        {
            string path = string.Format("Assets/Point Clouds txt/{0}.txt", fileName);
            StreamReader inp_stm = new StreamReader(path);
            bool InvertYZ = true;
            int offsetY = InvertYZ ? 2 : 1;
            int offsetZ = InvertYZ ? 1 : 2;

            List<Vector3> all_points = new List<Vector3>();
            List<int> all_colors = new List<int>();

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
                    UpdateMinMaxValues(point_position);

                    //all_points.Add(point_position);
                    int color;

                    if (coords.Length == 40)
                    {
                        color = int.Parse(coords[3]);
                        //all_colors.Add(color);
                    }
                    else
                    {
                        float normalizedY = Mathf.InverseLerp(0f, 4f, point_position.y);
                        Color c = Color.Lerp(Color.blue, Color.green, normalizedY);
                        color = Util.encodeColor((int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255));
                    }

                    all_points.Add(point_position);
                    all_colors.Add(color);

                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }

            points = all_points.ToArray();
            colors = all_colors.ToArray();
        }

        private static void UpdateMinMaxValues(Vector3 point)
        {
            maxX = maxX > point.x ? maxX : point.x;
            minX = minX < point.x ? minX : point.x;
            minY = minY < point.y ? minY : point.y;
            maxZ = maxZ > point.z ? maxZ : point.z;
            minZ = minZ < point.z ? minZ : point.z;
        }

        public static Vector3[] GetFloorVertex()
        {
            Vector3[] v = new Vector3[4];

            if (maxX == float.MinValue || minX == float.MaxValue || minY == float.MaxValue || maxZ == float.MinValue || minZ == float.MaxValue)
            {
                return null;
            }

            v[0] = new Vector3(minX, minY - 0.05f, minZ);
            v[1] = new Vector3(minX, minY - 0.05f, maxZ);
            v[2] = new Vector3(maxX, minY - 0.05f, maxZ);
            v[3] = new Vector3(maxX, minY - 0.05f, minZ);

            return v;
        }

    }

}
