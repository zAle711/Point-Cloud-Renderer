using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using PointCloud2Msg = RosMessageTypes.Sensor.PointCloud2Msg;
using System;


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

    ComputeBuffer positionsBuffer;
    ComputeBuffer colorsBuffer;

    public bool readFromFile = false;

    // Start is called before the first frame update
    void Start()
    {
        pointMaterial = new Material(Shader.Find("Unlit/PointShader"));
        //textInfo = GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>();

        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<PointCloud2Msg>(topicName, OnMessageReceived);
        
    }

    private void OnMessageReceived(PointCloud2Msg msg)
    {
        lastMessage = msg;
        newMessage = true;
        GetPointsAndColors();

        if (points.Length != 0 && colors.Length != 0)
            InitializeMaterial();
            

        
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

    private void InitializeMaterial()
    {

        if (positionsBuffer != null)
            positionsBuffer.Release();
        if (colorsBuffer != null)
            colorsBuffer.Release();

        positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);

    }

    private void OnRenderObject()
    {
        if (points != null && colors != null)
        {
            Debug.Log("REnderizzo!");
            pointMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);
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
    }

}
