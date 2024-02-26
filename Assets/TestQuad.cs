using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PointCloudVR;
public class TestQuad : MonoBehaviour
{
    // Start is called before the first frame update
    public MeshFilter mf;
    public MeshRenderer mr;
    public MeshCollider mc;
    private Material material;

    float pointRadius = 2.5f;
    bool renderCircles = false;

    private string fileName = "corridoio.pcd";
    private PointCloud pc;
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        PointCloud pcq;

        PointCloudReader.ReadPCDFile(out pc, out pcq, fileName, 0.5f, true);

        material = new Material(Shader.Find("Custom/Quad4PointScreenSizeShader"));
        material.enableInstancing = true;
        material.SetFloat("_PointSize", pointRadius);
        material.SetInt("_Circles", renderCircles ? 1 : 0);
        Rect screen = Camera.main.pixelRect;
        material.SetInt("_ScreenWidth", (int)screen.width);
        material.SetInt("_ScreenHeight", (int)screen.height);

        CreateMesh();

        //material = new Material(Shader.Find("Custom/QuadGeoScreenSizeShader"));
        //Rect screen = Camera.main.pixelRect;
        //material.enableInstancing = true;
        //material.SetFloat("_PointSize", pointRadius);
        //material.SetInt("_Circles", renderCircles ? 1 : 0);
        //material.SetInt("_ScreenWidth", (int)screen.width);
        //material.SetInt("_ScreenHeight", (int)screen.height);
        //Debug.Log(material);
        //CreateMeshGeometry();
        //Debug.Log($"Lunghezza point Cloud: {pc.points.Length}");
    }

    void CreateMeshGeometry()
    {
        Mesh mesh = new Mesh();

        //mf = gameObject.AddComponent<MeshFilter>();
        //mr = gameObject.AddComponent<MeshRenderer>();
        
        

        int[] indecies = new int[pc.points.Length];
        Color[] colors = new Color[pc.colors.Length];
        for (int i = 0; i < pc.points.Length; ++i)
        {
            indecies[i] = i;
            colors[i] = getColor(pc.colors[i]);
        }
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = pc.points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

        mf.mesh = mesh;
        mr.material = material;

        mc.sharedMesh = mesh;
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] newVertexBuffer = new Vector3[pc.points.Length * 4];
        Color[] newColorBuffer = new Color[pc.colors.Length * 4];
        Vector2[] offsetBuffer = new Vector2[pc.points.Length * 4];
        int[] indecies = new int[pc.points.Length * 4];

        for (int i = 0; i < pc.points.Length; i++)
        {
            int startindex = i * 4;
            newVertexBuffer[startindex] = newVertexBuffer[startindex + 1] = newVertexBuffer[startindex + 2] = newVertexBuffer[startindex + 3] = pc.points[i];
            offsetBuffer[startindex + 0] = new Vector2(-1.0f, +1.0f);
            offsetBuffer[startindex + 1] = new Vector2(+1.0f, +1.0f);
            offsetBuffer[startindex + 2] = new Vector2(+1.0f, -1.0f);
            offsetBuffer[startindex + 3] = new Vector2(-1.0f, -1.0f);

            newColorBuffer[startindex] = newColorBuffer[startindex + 1] = newColorBuffer[startindex + 2] = newColorBuffer[startindex + 3] = getColor(pc.colors[i]);
            indecies[startindex] = startindex;
            indecies[startindex + 1] = startindex + 1;
            indecies[startindex + 2] = startindex + 2;
            indecies[startindex + 3] = startindex + 3;
        }
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = newVertexBuffer;
        mesh.colors = newColorBuffer;
        mesh.uv = offsetBuffer;
        mesh.SetIndices(indecies, MeshTopology.Quads, 0);

        mf.mesh = mesh;
        mr.material = material;

        mc.sharedMesh = mesh;
        mc.sharedMesh.SetTriangles(mc.sharedMesh.GetTriangles(0), 0);

    }

    Color getColor(int c)
    {
        float r = ((c >> 16) & 0xff) / 255.0f;
        float g = ((c >> 8) & 0xff) / 255.0f;

        float b = (c & 0xff) / 255.0f;

        return new Color(r, g, b, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
