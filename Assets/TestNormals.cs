using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System;

public class TestNormals : MonoBehaviour
{
    public string fileName;
    public float cubeSize = 0.025f;
    private Vector3[] points;
    private int[] colors;
    private Vector3[] normals;

    private ComputeBuffer p;
    private ComputeBuffer c;
    private ComputeBuffer n;

    public bool rendering = false;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        ReadFileWithNormals();

        mat = new Material(Shader.Find("Unlit/QuadShader"));

        p = new ComputeBuffer(points.Length, 3 * sizeof(float));
        c = new ComputeBuffer(colors.Length, sizeof(int));
        n = new ComputeBuffer(normals.Length, 3 * sizeof(float));

        p.SetData(points);
        c.SetData(colors);
        n.SetData(normals);

        mat.SetBuffer("_Positions", p);
        mat.SetBuffer("_Colors", c);
        mat.SetBuffer("_Normals", n);
        

    }

    private void OnRenderObject()
    {
        if (!rendering) return;


        mat.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Quads, points.Length, 1);

    }
    private void ReadFileWithNormals()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        //string text = System.IO.File.ReadAllText(filePath);
        StreamReader inp_stm = new StreamReader(filePath);

        //var textFile = Resources.Load<TextAsset>("point_cloud_unreal_alg");
        //List<string> lines = new List<string>(text.Split('\n'));

        bool InvertYZ = true;
        int offsetY = InvertYZ ? 2 : 1;
        int offsetZ = InvertYZ ? 1 : 2;

        List<Vector3> all_points = new List<Vector3>();
        List<int> all_colors = new List<int>();
        List<Vector3> all_normals = new List<Vector3>();
        int i = 0;
        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            string[] coords = inp_ln.Split();

            try
            {
                float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);

                //all_points.Add(point_position);
                int color = int.Parse(coords[3]);

                float xn = float.Parse(coords[4], CultureInfo.InvariantCulture);
                float yn = float.Parse(coords[4 + offsetY], CultureInfo.InvariantCulture);
                float zn = float.Parse(coords[4 + offsetZ], CultureInfo.InvariantCulture);

                Vector3 point_position = new Vector3(x, y, z);
                Vector3 debug = new Vector3(xn, yn, zn);
                Vector3 point_normal = GetNormalVector(xn, yn, zn);

                AddFaceToAllPoints(all_points, all_colors, all_normals, point_position, color, point_normal);
                //all_points.Add(point_position);
                //all_colors.Add(color);
                //all_normals.Add(point_normal);

                if (i % 100 == 0)
                {
                    Debug.Log($"Vettore normale Arrotondato : {debug} -> {point_normal}");
                }


            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        points = all_points.ToArray();
        colors = all_colors.ToArray();
        normals = all_normals.ToArray();

        Debug.Log($"{points.Length == colors.Length && colors.Length == normals.Length}");
    }

    private void AddFaceToAllPoints(List<Vector3> p, List<int> c, List<Vector3> normals, Vector3 positon, int color, Vector3 normal)
    {
        if (normal.x == -1)
            p.AddRange(RightFace(positon, cubeSize));
        else if (normal.x == 1)
            p.AddRange(LeftFace(positon, cubeSize));
        else if (normal.y == -1)
            p.AddRange(UpFace(positon, cubeSize));
        else if (normal.y == 1)
            p.AddRange(DownFace(positon, cubeSize));
        else if (normal.z == -1)
            p.AddRange(ForwardFace(positon, cubeSize));
        else
            p.AddRange(BackFace(positon, cubeSize));


        for (int i = 0; i < 4; i++)
        {
            c.Add(color);
            normals.Add(normal);
        }
    }

    private void OnDestroy()
    {
        p.Release();
        c.Release();
        n.Release();
    }

    private Vector3[] ForwardFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x - size, point.y - size, point.z - size);
        p[1] = new Vector3(point.x - size, point.y + size, point.z - size);
        p[2] = new Vector3(point.x + size, point.y + size, point.z - size);
        p[3] = new Vector3(point.x + size, point.y - size, point.z - size);

        return p;
    }

    private Vector3[] BackFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x + size, point.y - size, point.z + size);
        p[1] = new Vector3(point.x + size, point.y + size, point.z + size);
        p[2] = new Vector3(point.x - size, point.y + size, point.z + size);
        p[3] = new Vector3(point.x - size, point.y - size, point.z + size);

        return p;
    }

    private Vector3[] DownFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x - size, point.y + size, point.z + size);
        p[1] = new Vector3(point.x + size, point.y + size, point.z + size);
        p[2] = new Vector3(point.x + size, point.y + size, point.z - size);
        p[3] = new Vector3(point.x - size, point.y + size, point.z - size);

        return p;
    }

    private Vector3[] UpFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x + size, point.y - size, point.z - size);
        p[1] = new Vector3(point.x + size, point.y - size, point.z + size);
        p[2] = new Vector3(point.x - size, point.y - size, point.z + size);
        p[3] = new Vector3(point.x - size, point.y - size, point.z - size);

        return p;

    }

    private Vector3[] RightFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x - size, point.y - size, point.z + size);
        p[1] = new Vector3(point.x - size, point.y + size, point.z + size);
        p[2] = new Vector3(point.x - size, point.y + size, point.z - size);
        p[3] = new Vector3(point.x - size, point.y - size, point.z - size);

        return p;
    }

    private Vector3[] LeftFace(Vector3 point, float size)
    {
        Vector3[] p = new Vector3[4];

        p[0] = new Vector3(point.x + size, point.y + size, point.z + size);
        p[1] = new Vector3(point.x + size, point.y - size, point.z + size);
        p[2] = new Vector3(point.x + size, point.y - size, point.z - size);
        p[3] = new Vector3(point.x + size, point.y + size, point.z - size);

        return p;
    }


    private Vector3 GetNormalVector(float x, float y, float z)
    {
        float xa = Mathf.Abs(x);
        float ya = Mathf.Abs(y);
        float za = Mathf.Abs(z);

        float max = Mathf.Max(xa, MathF.Max(ya, za));

        float[] v = new float[3];

        

        if (max == xa)
        {
            return new Vector3(Mathf.RoundToInt(x), 0, 0);
        }

        if (max == ya)
        {
            return new Vector3(0, Mathf.RoundToInt(y), 0);
        }

        return new Vector3(0, 0, Mathf.RoundToInt(z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
