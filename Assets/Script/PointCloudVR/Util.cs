using System;
using UnityEngine;

namespace PointCloudVR
{
    public static class Util
    {
        public static int encodeColor(int r, int g, int b)
        {
            int encoded = r << 16;
            encoded |= g << 8;
            encoded |= b;
            return encoded;
        }

        public static void ZFace(Vector3 point, int color, Vector3 normal, float size, int currentIndex, ref Point[] quads)
        {

            Vector3 bottomLeft = new Vector3(point.x + size, point.y - size, point.z);
            Vector3 topLeft = new Vector3(point.x + size, point.y + size, point.z);
            Vector3 topRight = new Vector3(point.x - size, point.y + size, point.z);
            Vector3 bottomRight = new Vector3(point.x - size, point.y - size, point.z);

            quads[currentIndex] = new Point(bottomLeft, color, normal);
            quads[currentIndex + 1] = new Point(topLeft, color, normal);
            quads[currentIndex + 2] = new Point(topRight, color, normal);
            quads[currentIndex + 3] = new Point(bottomRight, color, normal);

        }

        public static void YFace(Vector3 point, int color, Vector3 normal, float size, int currentIndex, ref Point[] quads)
        {

            Vector3 bottomLeft = new Vector3(point.x + size, point.y , point.z - size);
            Vector3 topLeft = new Vector3(point.x - size, point.y, point.z - size);
            Vector3 topRight = new Vector3(point.x - size, point.y, point.z + size);
            Vector3 bottomRight = new Vector3(point.x + size, point.y, point.z + size);

            quads[currentIndex] = new Point(bottomLeft, color, normal);
            quads[currentIndex + 1] = new Point(topLeft, color, normal);
            quads[currentIndex + 2] = new Point(topRight, color, normal);
            quads[currentIndex + 3] = new Point(bottomRight, color, normal);
        }


        public static void XFace(Vector3 point, int color, Vector3 normal, float size, int currentIndex, ref Point[] quads)
        {
            //Point[] p = new Point[4];

            Vector3 bottomLeft = new Vector3(point.x, point.y - size, point.z - size);
            Vector3 topLeft = new Vector3(point.x, point.y + size, point.z - size);
            Vector3 topRight = new Vector3(point.x, point.y + size, point.z + size);
            Vector3 bottomRight = new Vector3(point.x, point.y - size, point.z + size);

            quads[currentIndex] = new Point(bottomLeft, color, normal);
            quads[currentIndex + 1] = new Point(topLeft, color, normal);
            quads[currentIndex + 2] = new Point(topRight, color, normal);
            quads[currentIndex + 3] = new Point(bottomRight, color, normal);
            
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

