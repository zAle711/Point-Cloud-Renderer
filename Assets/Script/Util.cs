using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

namespace ale711
{
    public static class Util
    {
        public static void ReadFile(out Vector3[] points, out int[] colors)
        {
            string path = string.Format("Assets/Point Clouds txt/{0}.txt", "global_point_cloud_unreal");
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

                if (!coords[0].Contains(".") || !coords[1].Contains(".") || !coords[2].Contains(".")) continue;

                Vector3 point_position = new Vector3(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetY], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetZ], CultureInfo.InvariantCulture));
                all_points.Add(point_position);
                all_colors.Add(encodeColor(255, 255, 255));

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



    }
}

