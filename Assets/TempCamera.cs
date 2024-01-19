using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;

    private Vector3[] pointCloud;
    private int[] pointCloudColors;

    private Vector3[] points;
    private int[] colors;

    private Material pointMaterial;
    public Frustum frustum;

    Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        //ReadFile();
        frustum = new Frustum(pointCloud, pointCloudColors);
    }
    private void SetPointsToRender()
    {
        frustum.UpdatePoints();
        if (!frustum.IsDone()) return;

        frustum.SetPointsAndColors( ref points, ref colors);

        Debug.Log(points.Length + " " + colors.Length);

        //if (frustum.points == null || frustum.colors == null)
        //{
        //    points = frustum.points;
        //    colors = frustum.colors;
        //}
        //List<int> idx = frustum.GetPointsInsideFrustum(pointCloud);
        //points = new Vector3[idx.Count];
        //colors = new int[idx.Count];

        //for (int i = 0; i < idx.Count; i++)
        //{
        //    points[i] = pointCloud[idx[i]];
        //    colors[i] = pointCloudColors[idx[i]];
        //}
    }
    // Update is called once per frame

    private void GetCoordinate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData))
            {
                Debug.Log(hitData.point);
            }


        }

    }
    void Update()
    {
        HandleMovement();
        GetCoordinate();
        //SetPointsToRender();
       
    }

    private void HandleMovement()
    {
        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
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
        pointCloudColors = all_colors.ToArray();
        //textInfo.text = string.Format("Punti renderizzati: {0}", points.Length);
    }

    private void DrawPoints()
    {
        
        if (points == null || colors == null)
            return;
        Debug.Log(points.Length);
        ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);

        positionsBuffer.Release();
        colorsBuffer.Release();

    }
    private int encodeColor(int r, int g, int b)
    {
        int encoded = r << 16;
        encoded |= g << 8;
        encoded |= b;
        return encoded;
    }

    private void OnRenderObject()
    {
        DrawPoints();
    }
}
