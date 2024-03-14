using System;
using UnityEngine;
using Unity.Mathematics;


namespace PointCloudVR
{
    public class Chunk
    {
        public GameObject gObj { get; private set; }
        public Vector3 position { get; private set; }
        public float priority { get; set; }
        public Bounds bounds { get; private set; }

        public Chunk(GameObject gObj, Vector3 position,Bounds bounds)
        {
            this.gObj = gObj;
            this.position = position;
            this.bounds = bounds;
        }

        public Chunk(GameObject gObj, Bounds bounds, float priority)
        {
            this.gObj = gObj;
            position = gObj.transform.position;
            this.bounds = bounds;
            this.priority = priority;
        }
    }

    public struct Point
    {
        public Vector3 position { get; private set; }
        public Color color { get; private set; }

        public Point(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
        }
    }

    public static class Util
    {
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

        public static int encodeColor(int r, int g, int b)
        {
            int encoded = r << 16;
            encoded |= g << 8;
            encoded |= b;
            return encoded;
        }


        public static Color getColor(int c)
        {
            float r = ((c >> 16) & 0xff) / 255.0f;
            float g = ((c >> 8) & 0xff) / 255.0f;

            float b = (c & 0xff) / 255.0f;

            return new Color(r, g, b, 1);
        }




    }
}

