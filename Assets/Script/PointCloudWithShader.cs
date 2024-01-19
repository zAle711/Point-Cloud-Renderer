using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using PointCloud2Msg = RosMessageTypes.Sensor.PointCloud2Msg;
using PoseArrayMsg = RosMessageTypes.Geometry.PoseArrayMsg;
using System;
using System.IO;
using System.Globalization;

public class PointCloudWithShader : MonoBehaviour
{
    

    public float pointSize = 1.0f;

    private string topicName = "/player0/scan_cloud"; //"/global_point_cloud"; //"/camera/depth/points"; //"/player0/scan_cloud";
    //private string topicName = "/global_point_cloud";
    private ROSConnection rosConnection;

    public MeshRenderer meshRenderer;
    private Mesh mesh;

    private Vector3[] points;
    private int[] colors;

    private Color[] meshColors;

    private bool newMessage = false;
    private Material pointMaterial;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer.material = new Material(Shader.Find("Custom/MyDefaultPoint"));
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));

        rosConnection = ROSConnection.GetOrCreateInstance();

        //rosConnection.Subscribe<PoseArrayMsg>("/points_array", OnMessageReceivedPoseArray);

        //ReadFile();
        rosConnection.Subscribe<PointCloud2Msg>(topicName, OnMessageReceived);
    }

    private void ReadFile()
    {
        //string path = "Assets/PointsClouds/global_point_cloud_unreal.txt";
        //string path = "Assets/PointsClouds/global_point_cloud_rgb.txt";
        string path = "Assets/Point Clouds txt/global_point_cloud_rgb.txt";
        StreamReader inp_stm = new StreamReader(path);

        List<Vector3> temp_points = new List<Vector3>();
        List<int> temp_colors = new List<int>();
        List<Color> temp_mesh_colors = new List<Color>();
        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            string[] coords = inp_ln.Split();

            int offsetY = 2;
            int offsetZ = 1;

            if (!coords[0].Contains(".") || !coords[1].Contains(".") || !coords[2].Contains(".")) continue;

            Vector3 point_position = new Vector3(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[offsetY], CultureInfo.InvariantCulture), float.Parse(coords[offsetZ], CultureInfo.InvariantCulture));
            int point_color = 16777215; // RED = 16711680 int x = r << 16 | g << 8 | b;



            if (coords.Length == 4)
            {
                point_color = int.Parse(coords[3]);
            }

            temp_points.Add(point_position);
            temp_colors.Add(point_color);
            temp_mesh_colors.Add(new Color(255,255,255));

        }
        Debug.Log("Punti totali: " + temp_points.Count);
        points = temp_points.ToArray();
        colors = temp_colors.ToArray();
        meshColors = temp_mesh_colors.ToArray();

        newMessage = true;
        //return points;
    }
    void OnMessageReceived(PointCloud2Msg msg)
    {
        //string debugTxt = string.Format("Width: {0} --- Height: {1} --- Row Step: {2}", msg.width, msg.height, msg.row_step);

        int size = msg.data.Length;

        byte[] byteArray = msg.data;

        int point_step = (int)msg.point_step;
        size /= point_step;

        int x_posi;
        int y_posi;
        int z_posi;

        float x;
        float y;
        float z;

        points = new Vector3[size];
        colors = new int[size];

        for (int n = 0; n < size; n++)
        {
            x_posi = n * point_step + 0;
            y_posi = n * point_step + 4;
            z_posi = n * point_step + 8;

            x = BitConverter.ToSingle(byteArray, x_posi);
            y = BitConverter.ToSingle(byteArray, y_posi);
            z = BitConverter.ToSingle(byteArray, z_posi);

            points[n] = new Vector3(x, z, y);
            colors[n] = 16777215;


        }

        //points = temp_points.ToArray();
        //meshColors = temp_colors.ToArray();

        newMessage = true;
        //Debug.Log(debugTxt);



    }

    // Update is called once per frame
    void Update()
    {


        if (newMessage)
        {
            //Debug.Log("Disegno!");
            DrawMesh();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //WritePointsToFile();
        }
    }

    private void DrawMesh()
    {
        mesh.Clear();
        mesh.vertices = points;
        mesh.colors = meshColors;

        int[] indices = new int[points.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            indices[i] = i;
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetIndices(indices, MeshTopology.Points, 0);
        //mesh.RecalculateBounds();
        //AssetDatabase.CreateAsset(mesh, "Assets/testMeshColorRTABMap4.asset");
        //AssetDatabase.SaveAssets();
        newMessage = false; //Resets and waits for new message
    }

    private void DrawPoints()
    {

        if (points == null || colors == null)
            return;

        ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);
        pointMaterial.SetFloat("_PointSize", pointSize);
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);

        positionsBuffer.Release();
        colorsBuffer.Release();

        newMessage = false;
    }

    private void OnRenderObject()
    {
        DrawPoints();
    }
}
