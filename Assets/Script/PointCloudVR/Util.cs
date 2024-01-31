using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;

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

        public static Point[] ZFace(Vector3 point, int color, Vector3 normal, float size)
        {
            Point[] p = new Point[4];

            Vector3 bottomLeft = new Vector3(point.x + size, point.y - size, point.z);
            Vector3 topLeft = new Vector3(point.x + size, point.y + size, point.z);
            Vector3 topRight = new Vector3(point.x - size, point.y + size, point.z);
            Vector3 bottomRight = new Vector3(point.x - size, point.y - size, point.z);

            p[0] = new Point(bottomLeft, color, normal);
            p[1] = new Point(topLeft, color, normal);
            p[2] = new Point(topRight, color, normal);
            p[3] = new Point(bottomRight, color, normal);

            return p;
        }

        public static Point[] YFace(Vector3 point, int color, Vector3 normal, float size)
        {
            Point[] p = new Point[4];

            Vector3 bottomLeft = new Vector3(point.x + size, point.y , point.z - size);
            Vector3 topLeft = new Vector3(point.x - size, point.y, point.z - size);
            Vector3 topRight = new Vector3(point.x - size, point.y, point.z + size);
            Vector3 bottomRight = new Vector3(point.x + size, point.y, point.z + size);

            p[0] = new Point(bottomLeft, color, normal);
            p[1] = new Point(topLeft, color, normal);
            p[2] = new Point(topRight, color, normal);
            p[3] = new Point(bottomRight, color, normal);

            return p;
        }


        public static Point[] XFace(Vector3 point, int color, Vector3 normal, float size)
        {
            Point[] p = new Point[4];

            Vector3 bottomLeft = new Vector3(point.x, point.y - size, point.z - size);
            Vector3 topLeft = new Vector3(point.x, point.y + size, point.z - size);
            Vector3 topRight = new Vector3(point.x, point.y + size, point.z + size);
            Vector3 bottomRight = new Vector3(point.x, point.y - size, point.z + size);

            p[0] = new Point(bottomLeft, color, normal);
            p[1] = new Point(topLeft, color, normal);
            p[2] = new Point(topRight, color, normal);
            p[3] = new Point(bottomRight, color, normal);
            
            return p;
        }

        public static Vector3 GetNormalVector(float x, float y, float z)
        {
            float xa = Mathf.Abs(x);
            float ya = Mathf.Abs(y);
            float za = Mathf.Abs(z);

            float max = Mathf.Max(xa, MathF.Max(ya, za));

            Vector3 normal = Vector3.zero;

            if (max == xa)
                normal.x = Mathf.RoundToInt(x);
            else if (max == ya)
                normal.y = Mathf.RoundToInt(y);
            else if (max == za)
                normal.z = Mathf.RoundToInt(z);
            
            return normal;
        }

    }
}

