using DataStructures.ViliWonka.KDTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public string fileName = "mesh.ply";
    public string fileNameP = "corridoio_with_normals.pcd";

    private Vector3[] vertices;
    private Vector3[] normals;
    private int[] triangles;

    private Vector3[] p;
    private int[] c;

    private KDTree kDTree;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        ReadFileWithNormals(fileName);
        PointCloudVR.PointCloudReader.ReadFileWithNormals(out p, out c, out Vector3[] n, fileNameP, 0.01f);
        Debug.Log($"v: {vertices.Length} n: {normals.Length} t: {triangles.Length}");

        kDTree = new KDTree(p);
        mat = new Material(Shader.Find("Unlit/TestPCD"));
        CreateMesh();
    }

    private void CreateMesh()
    {
        int[] colors = new int[vertices.Length];

        KDQuery query = new KDQuery();
        List<int> res = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            res.Clear();
            query.ClosestPoint(kDTree, vertices[i], res);
            colors[i] = c[res[0]];
        }
        Debug.Log($"{colors.Length} -- {vertices.Length}");

        ComputeBuffer shaderColors = new ComputeBuffer(colors.Length, sizeof(int));
        shaderColors.SetData(colors);
        mat.SetBuffer("_Colors", shaderColors);
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshCollider mc = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        mr.material = mat;
        mf.mesh = mesh;
        mc.sharedMesh = mesh;

        //shaderColors.Release();

    }

    public void ReadFileWithNormals(string fileName, bool invertXY = true)
    {
        int totalVertices = 261217;
        int totalHeader = 14;
        //1.6292 -1.1831 0.81108 -0.79958 0.27016 0.53636 0.0043096 

        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log($"FIle da caricare: {filePath}");
        StreamReader inp_stm = new StreamReader(filePath);

        List<Vector3> p = new List<Vector3>();
        List<Vector3> n = new List<Vector3>();
        List<int> t = new List<int>();

        int offsetY = invertXY ? 2 : 1;
        int offsetZ = invertXY ? 1 : 2;
        int i = 0;
        int j = 0;
        int max = int.MinValue;
        while (!inp_stm.EndOfStream)
        {
            j += 1;
            string inp_ln = inp_stm.ReadLine();

            string[] coords = inp_ln.Split();
            if (j <= totalVertices + totalHeader)
            {
                try
                {
                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[offsetY], CultureInfo.InvariantCulture);
                    float z = float.Parse(coords[offsetZ], CultureInfo.InvariantCulture);

                    //int color = int.Parse(coords[3]);

                    float xn = float.Parse(coords[3], CultureInfo.InvariantCulture);
                    float yn = float.Parse(coords[3 + offsetY], CultureInfo.InvariantCulture);
                    float zn = float.Parse(coords[3 + offsetZ], CultureInfo.InvariantCulture);

                    p.Add(new Vector3(x, y, z));

                    n.Add(new Vector3(xn, yn, zn));
                    //all_points.AddRange(AddFaceWithNormal(point_position, color, point_normal, quadSize));

                }
                catch (Exception e)
                {
                    i += 1;
                    Debug.Log(j);
                }
            }
            else
            {
                int i1 = int.Parse(coords[1]);
                int i2 = int.Parse(coords[2]);
                int i3 = int.Parse(coords[3]);

                max = max > i1 ? max : i1;
                max = max > i2 ? max : i2;
                max = max > i3 ? max : i3;

                t.Add(i1);
                t.Add(i2);
                t.Add(i3);
            }
           

        }

        Debug.Log($"Linee saltate: {i}");
        Debug.Log($"Index max: {max}");
        triangles = t.ToArray();
        vertices = p.ToArray();
        normals = n.ToArray();
    }

}
