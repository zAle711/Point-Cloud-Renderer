using ale711;
using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;


class PointCloudRenderer
{
    private Vector3[] points;
    private int[] colors;

    private Matrix4x4 localToWorldMatrix;
    private Material pointMaterial;

    public PointCloudRenderer()
    {
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
    }

    public void SetWorldMatrix(Matrix4x4 localToWorldMatrix)
    {
        this.localToWorldMatrix = localToWorldMatrix;
    }

    public void SetData(Vector3[] points, int[] colors)
    {
        this.points = points;
        this.colors = colors;
    }

    public void Render()
    {
        if (points == null || points.Length == 0) return;
        //Debug.Log(pointCloud.Length);
        ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetMatrix("_Transform", localToWorldMatrix);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);

        positionsBuffer.Release();
        colorsBuffer.Release();

    }
}

public class PointCloud : MonoBehaviour
{
    public float boundSize = 0.5f;
    public Camera cam;
    public int maxPointsToRender = 100000;

    private Vector3[] pointCloud;
    private int[] pointCloudColors;

    private Material pointMaterial;

    private FrustumCulling frustumCulling;
    private PointCloudRenderer pointCloudRenderer;

    private int maxPointsPerLeafNode = 32;
    private KDTree tree;

    private Vector3[] shaderPoints;
    private int[] shaderColors;

    private List<(Vector3, Vector3)> gizmoData;

    public TextMeshProUGUI textLabel;

    private void Awake()
    {
        pointCloudRenderer = new PointCloudRenderer();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        Util.ReadFile(out pointCloud, out pointCloudColors);
        tree = new KDTree(pointCloud, 32);
        gizmoData = new List<(Vector3, Vector3)>();
        //GetGizmoCubes();
        //pointCloudRenderer.SetData(pointCloud, pointCloudColors);
        frustumCulling = new FrustumCulling(pointCloud, pointCloudColors, tree , boundSize, maxPointsToRender);
        frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
        frustumCulling.Start();

    }
    private void RetrieveData()
    {
        frustumCulling.SetCamera(cam, transform.localToWorldMatrix);
        var (points, colors, gizmoBounds) = frustumCulling.GetData();

        if (points != null && colors != null)
        {
            pointCloudRenderer.SetData(points, colors);
            textLabel.text = $"Punti Renderizzati: {points.Length}";
        }
        gizmoData.Clear();
        if (gizmoBounds.Count != 0)
        {
            gizmoData = gizmoBounds;
        }

        
    }
    // Update is called once per frame
    void Update()
    {
       
        RetrieveData();

        //Debug.Log(indexs.Count + " " + pointCloud.Length);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            frustumCulling.Stop();
        }

        pointCloudRenderer.SetWorldMatrix(transform.localToWorldMatrix);
    }

    public Color coloreTrasparente = new Color(0f, 1f, 0f, 0.5f);
    public Vector3 dimensioniCubo = new Vector3(1f, 1f, 1f);

    private void OnDrawGizmos()
    {
        
        if (gizmoData == null || gizmoData.Count == 0) return;
        // Imposta il colore del gizmo
        Handles.color = coloreTrasparente;

        //// Calcola la matrice di trasformazione per posizione e rotazione del cubo
        //Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        foreach( (Vector3 pos, Vector3 size) cube in gizmoData)
        {
            Handles.DrawWireCube(cube.pos + cube.size / 2, cube.size);
        }
        // Disegna il wireframe del cubo trasparente

        // Ripristina il colore a quello predefinito di Unity
        Handles.color = Color.white;
    }

    private void OnRenderObject()
    {
        pointCloudRenderer.Render();
    }

    private void OnValidate()
    {
        if (frustumCulling != null)
        {
            Debug.Log("Valore cambiato nell'editor");
            frustumCulling.SetMaxPoints(maxPointsToRender);
        }
    }
}
