using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;



    private Vector3[] points;
    private int[] colors;

    private float last;
    public float interval = 0.5f;
    
    private Dictionary<string, List<Vector3>> nearPoints;
    private Material pointMaterial;
    private float pointSize = 5.0f;

    private bool test = false;

    private bool pointLoaded = false;
    private void Start()
    {
        nearPoints = new Dictionary<string, List<Vector3>>();
        last = Time.realtimeSinceStartup;
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        
    }

    private void OnTriggerEnter(Collider other)
    {
        nearPoints.Add(other.gameObject.name, other.gameObject.GetComponent<Chunk>().GetPoints());
    }

    private void OnTriggerExit(Collider other)
    {
        nearPoints.Remove(other.gameObject.name);
    }


    void Update()
    {
        //GetCurrentChunk();

        if (Time.realtimeSinceStartup - last > interval)
        {
            ComputePoints();
            last = Time.realtimeSinceStartup;
        }

        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            test = !test;
        }

    }

    private void ComputePoints()
    {
        if (nearPoints.Keys.Count == 0) return;

        List<Vector3> allPoints = new List<Vector3>();
       
        foreach(List<Vector3> p in nearPoints.Values)
        {
            allPoints.AddRange(p);
        }

        points = allPoints.ToArray();

        colors = new int[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            colors[i] = 16777215;
        }

        pointLoaded = true;

    }

    private void DrawPoints()
    {
        if (!pointLoaded)
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
        Debug.Log(points.Length);

    }

    private void OnRenderObject()
    {
        if (test){
            DrawPoints();
        }
        
    }
}
