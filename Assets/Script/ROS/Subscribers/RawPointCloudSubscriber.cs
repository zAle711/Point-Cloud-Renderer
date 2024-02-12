using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using PointCloud2Msg = RosMessageTypes.Sensor.PointCloud2Msg;
using System;
using TMPro;
using System.IO;
using System.Globalization;

public class RawPointCloudSubscriber : MonoBehaviour
{
    public string topicName = "/camera/rgb/points";
    public bool showPointCloud = true;
    public bool isColored = false;
    public bool invertYZ = true;

    private ROSConnection rosConnection;
    private PointCloud2Msg lastMessage;
    private bool newMessage = false;

    private Vector3[] points;
    private int[] colors;
    private Material pointMaterial;

    private TextMeshProUGUI textInfo;

    public bool readFromFile = false;

    // Start is called before the first frame update
    void Start()
    {
        pointMaterial = new Material(Shader.Find("Unlit/PointShader"));
        //textInfo = GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>();

        rosConnection = ROSConnection.GetOrCreateInstance();
        if (!readFromFile) 
            rosConnection.Subscribe<PointCloud2Msg>(topicName, OnMessageReceived);
        else 
            ReadFile();
        
    }

    private void ReadFile()
    {
        string path = string.Format("Assets/Point Clouds txt/{0}.txt", "point_cloud_unreal_alg");
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

            Vector3 point_position = new Vector3(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetY], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetZ], CultureInfo.InvariantCulture));
            all_points.Add(point_position);
            all_colors.Add(encodeColor(255, 255, 255));
            
        }

        points = all_points.ToArray();
        colors = all_colors.ToArray();
        textInfo.text = string.Format("Punti renderizzati: {0}", points.Length);
    }

    private void OnMessageReceived(PointCloud2Msg msg)
    {
        lastMessage = msg;
        newMessage = true;
    }

    private void GetPointsAndColors()
    {
        int size = lastMessage.data.Length;

        byte[] byteArray = new byte[size];
        byteArray = lastMessage.data;

        int point_step = (int)lastMessage.point_step;

        size = size / point_step;

        points = new Vector3[size];
        colors = new int[size];

        int x_posi;
        int y_posi;
        int z_posi;

        float x;
        float y;
        float z;

        int rgb_posi;

        int r;
        int g;
        int b;

        for (int n = 0; n < size; n++)
        {
            x_posi = n * point_step + 0;
            y_posi = n * point_step + 4;
            z_posi = n * point_step + 8;

            x = BitConverter.ToSingle(byteArray, x_posi);
            y = BitConverter.ToSingle(byteArray, y_posi);
            z = BitConverter.ToSingle(byteArray, z_posi);

            points[n] = invertYZ ? new Vector3(x, z, y) : new Vector3(x, y, z);

            if (isColored)
            {
                rgb_posi = n * point_step + 16;

                b = byteArray[rgb_posi + 0];
                g = byteArray[rgb_posi + 1];
                r = byteArray[rgb_posi + 2];       
            } else
            {
                r = 255;
                g = 255;
                b = 255;
            }

            colors[n] = encodeColor(r, g, b);
            

        }

    }

    private int encodeColor(int r, int g, int b)
    {
        int encoded = r << 16;
        encoded |= g << 8;
        encoded |= b;
        return encoded;
    }

    // Update is called once per frame
    void Update()
    {
        if (newMessage)
        {
            GetPointsAndColors();
            //textInfo.text = string.Format("Punti renderizzati: {0}", points.Length);
        }
    }

    private void DrawPoints()
    {
        float pointSize = 1.0f;

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
        if (showPointCloud) 
            DrawPoints();
    }
}
