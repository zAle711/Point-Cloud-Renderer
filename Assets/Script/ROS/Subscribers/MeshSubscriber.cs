using RosMessageTypes.Geometry;
using RosMessageTypes.Shape;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class MeshSubscriber : MonoBehaviour
{
    // Start is called before the first frame update
    public string topicName = "";

    private ROSConnection rosConnection;

    private MeshFilter mf;
    private MeshRenderer mr;
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mr.material = new Material(Shader.Find("Unlit/TestPCD"));

        rosConnection = ROSConnection.GetOrCreateInstance();

        rosConnection.Subscribe<MeshMsg>(topicName, OnMessageReceived);
    }

    private void OnMessageReceived(MeshMsg msg)
    {
        int[] t = new int[msg.triangles.Length * 3];
        Vector3[] v = new Vector3[msg.vertices.Length];

        int index = 0;
        foreach(MeshTriangleMsg face in msg.triangles)
        {
            t[index] = (int) face.vertex_indices[0];
            t[index + 1] = (int) face.vertex_indices[2];
            t[index + 2] = (int) face.vertex_indices[1];

            index += 3;
        }
        Debug.Log($"{t.Length} -> {index}");
        index = 0;
        
        foreach(PointMsg p in msg.vertices)
        {
            v[index] = new Vector3((float) p.x, (float) p.y, (float) p.z);
            index += 1;
        }

        mf.mesh.Clear();
        mf.mesh.vertices = v;
        mf.mesh.triangles = t;

        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
