using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVari : MonoBehaviour
{
    public float size = 3f;
    //private Vector3 c1 = new(-6.685f, -0.237717f, 6.62915f);
    //private Vector3 c2 = new(6.685f, 0.237717f, 6.54505f);
    //private Vector3 c3 = new(-6.685f, -2.56805f, -6.54505f);
    //private Vector3 c4 = new(6.685f, -2.09262f, -6.62915f);

    public Color start = Color.blue;
    public float value = 0.0f;
    public Color end = Color.red;

    public Color res;

    private void Start()
    {
        
    }

    
    //public Vector3 c1 = new(-0.486343f, 0.00995071f, 0.469029f);
    //public Vector3 c2 = new(0.486343f, -0.00995071f, 0.463076f);
    //public Vector3 c3 = new(-0.486343f, 0.288752f, -0.463076f);
    //public Vector3 c4 = new(0.486343f, 0.26885f, -0.469029f);

    //public int[] triangles = new int[6] { 0, 1, 3, 0, 3, 2 };
    //// Start is called before the first frame update
    //void Start()
    //{
    //    transform.position = c1;
    //    GameObject sph1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    sph1.name = "1";
    //    sph1.transform.position = c1;
    //    GameObject sph2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    sph2.transform.position = c2;
    //    sph2.name = "2";
    //    GameObject sph3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    sph3.transform.position = c3;
    //    sph3.name = "3";
    //    GameObject sph4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    sph4.transform.position = c4;
    //    sph4.name = "4";
    //}

    //private void CalculateMesh()
    //{
    //    MeshFilter mf = GetComponent<MeshFilter>();

    //    Mesh mesh = new Mesh();
    //    mesh.vertices = new Vector3[4] {c1 * size, c2 * size, c3 * size, c4 * size };
    //    mesh.triangles = triangles;

    //    mesh.normals = new Vector3[4] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };

    //    mesh.RecalculateBounds();
    //    mesh.RecalculateNormals();

    //    mf.mesh = mesh;
    //}
    private void OnValidate()
    {

        res = Color.Lerp(start, end, Mathf.InverseLerp(0f, 5f, value));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
