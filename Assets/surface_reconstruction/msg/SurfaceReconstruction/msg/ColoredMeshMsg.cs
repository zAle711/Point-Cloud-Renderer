//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.SurfaceReconstruction
{
    [Serializable]
    public class ColoredMeshMsg : Message
    {
        public const string k_RosMessageName = "surface_reconstruction/ColoredMesh";
        public override string RosMessageName => k_RosMessageName;

        public Shape.MeshTriangleMsg[] triangles;
        public Geometry.PointMsg[] vertices;
        public int[] colors;

        public ColoredMeshMsg()
        {
            this.triangles = new Shape.MeshTriangleMsg[0];
            this.vertices = new Geometry.PointMsg[0];
            this.colors = new int[0];
        }

        public ColoredMeshMsg(Shape.MeshTriangleMsg[] triangles, Geometry.PointMsg[] vertices, int[] colors)
        {
            this.triangles = triangles;
            this.vertices = vertices;
            this.colors = colors;
        }

        public static ColoredMeshMsg Deserialize(MessageDeserializer deserializer) => new ColoredMeshMsg(deserializer);

        private ColoredMeshMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.triangles, Shape.MeshTriangleMsg.Deserialize, deserializer.ReadLength());
            deserializer.Read(out this.vertices, Geometry.PointMsg.Deserialize, deserializer.ReadLength());
            deserializer.Read(out this.colors, sizeof(int), deserializer.ReadLength());
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.WriteLength(this.triangles);
            serializer.Write(this.triangles);
            serializer.WriteLength(this.vertices);
            serializer.Write(this.vertices);
            serializer.WriteLength(this.colors);
            serializer.Write(this.colors);
        }

        public override string ToString()
        {
            return "ColoredMeshMsg: " +
            "\ntriangles: " + System.String.Join(", ", triangles.ToList()) +
            "\nvertices: " + System.String.Join(", ", vertices.ToList()) +
            "\ncolors: " + System.String.Join(", ", colors.ToList());
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
