using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace PointCloudVR
{
    public struct GizmoBox
    {
        public GizmoBox(Vector3 position, Vector3 size)
        {
            this.position = position;
            this.size = size;
        }

        public Vector3 position { get; }
        public Vector3 size { get; }

        public override string ToString() => $"({position}, {size})";
    }

    public static class Util
    {
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

                //if (!coords[0].Contains(".") || !coords[1].Contains(".") || !coords[2].Contains(".")) continue;

                try
                {
                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                    float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);

                    Vector3 point_position = new Vector3(x, y, z);
                    //all_points.Add(point_position);
                    int color;

                    if (coords.Length == 40)
                    {
                        color = int.Parse(coords[3]);
                        //all_colors.Add(color);
                    } else
                    {
                        float normalizedY = Mathf.InverseLerp(0f, 4f, point_position.y);
                        Color c = Color.Lerp(Color.blue, Color.green, normalizedY);
                        color = encodeColor((int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255));
                    }

                    all_points.Add(point_position);
                    all_colors.Add(color);
                    //var (quadPoints, quadColors) = GetVisibleFacesOfCube(point_position, color, 0.05f, new Vector3(0,0,0));

                    //all_points.AddRange(quadPoints);
                    //all_colors.AddRange(quadColors);


                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }

            points = all_points.ToArray();
            colors = all_colors.ToArray();
        }

        public static int encodeColor(int r, int g, int b)
        {
            int encoded = r << 16;
            encoded |= g << 8;
            encoded |= b;
            return encoded;
        }

        public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)

        {
            for (int i = 0; i < 6; i++)
            {
                Plane plane = planes[i];
                float3 normal_sign = math.sign(plane.normal);
                float3 test_point = (float3)(bounds.center) + (bounds.extents * normal_sign);

                float dot = math.dot(test_point, plane.normal);
                if (dot + plane.distance < 0)
                    return false;
            }

            return true;

        }

        public static (Vector3[], int[]) GetVisibleFacesOfCube(Vector3 point, int color, float size, Vector3 cameraDirection)
        {
            float offset = 0.15f;
            List<Vector3> pointsList = new List<Vector3>();

            //Debug.Log($"Camera direction: {cameraDirection}, Direction: {Vector3.forward} = Dot: {Vector3.Dot(cameraDirection, Vector3.forward)}");

            if (Vector3.Dot(cameraDirection, Vector3.forward) + offset >= 0)
            {
                //FRONT
                //points[0] = new Vector3(point.x - size, point.y - size, point.z - size);
                //points[1] = new Vector3(point.x - size, point.y + size, point.z - size);
                //points[2] = new Vector3(point.x + size, point.y + size, point.z - size);
                //points[3] = new Vector3(point.x + size, point.y - size, point.z - size);

                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z - size));
                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z - size));
                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z - size));
                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z - size));
            }

            if (Vector3.Dot(cameraDirection, Vector3.back) + offset >= 0)
            {
                //BACK
                //points[7] = new Vector3(point.x - size, point.y - size, point.z + size);
                //points[6] = new Vector3(point.x - size, point.y + size, point.z + size);
                //points[5] = new Vector3(point.x + size, point.y + size, point.z + size);
                //points[4] = new Vector3(point.x + size, point.y - size, point.z + size);

                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z + size));
                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z + size));
                
            }

            if (Vector3.Dot(cameraDirection, Vector3.down) + offset >= 0)
            {
                //TOP
                //points[11] = new Vector3(point.x - size, point.y + size, point.z - size);
                //points[10] = new Vector3(point.x + size, point.y + size, point.z - size);
                //points[8] = new Vector3(point.x - size, point.y + size, point.z + size);
                //points[9] = new Vector3(point.x + size, point.y + size, point.z + size);

                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z - size));
                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z - size));
            }
            if (Vector3.Dot(cameraDirection, Vector3.up) + offset >= 0)
            {
                //BOTTOM
                //points[15] = new Vector3(point.x - size, point.y - size, point.z - size);
                //points[12] = new Vector3(point.x + size, point.y - size, point.z - size);
                //points[14] = new Vector3(point.x - size, point.y - size, point.z + size);
                //points[13] = new Vector3(point.x + size, point.y - size, point.z + size);

                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z - size));
                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z - size));

            }

            if (Vector3.Dot(cameraDirection, Vector3.right) + offset >= 0)
            {
                //LEFT
                //points[19] = new Vector3(point.x - size, point.y - size, point.z - size);
                //points[18] = new Vector3(point.x - size, point.y + size, point.z - size);
                //points[16] = new Vector3(point.x - size, point.y - size, point.z + size);
                //points[17] = new Vector3(point.x - size, point.y + size, point.z + size);

                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x - size, point.y + size, point.z - size));
                pointsList.Add(new Vector3(point.x - size, point.y - size, point.z - size));
            }
            if (Vector3.Dot(cameraDirection, Vector3.left) + offset >= 0)
            {
                //RIGHT
                //points[23] = new Vector3(point.x + size, point.y + size, point.z - size);
                //points[22] = new Vector3(point.x + size, point.y - size, point.z - size);
                //points[20] = new Vector3(point.x + size, point.y + size, point.z + size);
                //points[21] = new Vector3(point.x + size, point.y - size, point.z + size);

                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z + size));
                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z + size));
                pointsList.Add(new Vector3(point.x + size, point.y - size, point.z - size));
                pointsList.Add(new Vector3(point.x + size, point.y + size, point.z - size));
            }

            Vector3[] points = pointsList.ToArray();
            int[] colors = new int[points.Length]; 
           
            

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }

            return (points, colors);
        }

    }
}

