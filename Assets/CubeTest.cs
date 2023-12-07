using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;
    // Start is called before the first frame update
    void Start()
    {
        vertices = GetComponent<MeshFilter>().mesh.vertices;
        triangles = GetComponent<MeshFilter>().mesh.triangles;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
