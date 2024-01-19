using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
public class KdTest : MonoBehaviour
{
    private Vector3[] pointCloud;
    private int[] pointsColor;

    public int maxPointsPerLeafNode = 32;
    public GameObject myCamera;
    public float radius = 10;

    private Material pointMaterial;
    private Vector3[] points;
    private int[] colors;
    

    private KDTree tree;
    private KDQuery query;
    // Start is called before the first frame update
    void Start()
    {
        ReadFile();
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        tree = new KDTree(pointCloud, maxPointsPerLeafNode);
        query = new KDQuery();
    }

    private int encodeColor(int r, int g, int b)
    {
        int encoded = r << 16;
        encoded |= g << 8;
        encoded |= b;
        return encoded;
    }

    private void ReadFile()
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

        pointCloud = all_points.ToArray();
        pointsColor = all_colors.ToArray();
        //textInfo.text = string.Format("Punti renderizzati: {0}", points.Length);
    }

    private void GetPointsInRadius()
    {
        float start = Time.realtimeSinceStartup;
        List<int> results = new List<int>();
        query.Radius(tree, myCamera.transform.position, radius, results);
        points = new Vector3[results.Count];
        colors = new int[results.Count];

        for(int i = 0; i < results.Count; i++)
        {
            points[i] = pointCloud[results[i]];
            colors[i] = pointsColor[results[i]];
        }

        Debug.Log("Trovati " + points.Length + " punti in " + (Time.realtimeSinceStartup - start));
    }

    private void DrawPoints()
    {
        if (points == null || points.Length == 0)
            return;

        ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);
        pointMaterial.SetFloat("_PointSize", 1.0f);
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);

        positionsBuffer.Release();
        colorsBuffer.Release();

    }

    private void OnRenderObject()
    {
         DrawPoints();

    }

    // Update is called once per frame
    void Update()
    {
        GetPointsInRadius();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    List<int> results = new List<int>();
        //    query.Radius(tree, myCamera.transform.position, 10, results);
        //    Debug.Log(results.Count);
        //}
    }
}
