using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestMap : MonoBehaviour { 

    public Texture2D map;
    public float precision;
    public Vector3 offSet = new Vector3(-6.12f, 0, -4.68f);

    public string filePath = "Assets/excasi.pgm";
    public Vector2[] uvs;
    // Start is called before the first frame update
    void Start()
    {
        ReadMap();

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshCollider mc = GetComponent<MeshCollider>();

        Vector3[] vertices = new Vector3[4];

        float size_w = map.width * precision;
        float size_h = map.height * precision;

        //vertices[0] = new Vector3(-size_w / 2, 0, -size_h / 2);
        //vertices[1] = new Vector3(size_w / 2, 0, -size_h / 2);
        //vertices[2] = new Vector3(-size_w / 2, 0, size_h / 2);
        //vertices[3] = new Vector3(size_w / 2, 0, size_h / 2);

        vertices[0] = offSet;
        vertices[1] = offSet + new Vector3(0, 0, size_h);
        vertices[2] = offSet + new Vector3(size_w, 0, size_h);
        vertices[3] = offSet + new Vector3(size_w, 0, 0);

        mf.mesh.triangles = new int[6] { 1,2,3, 1,3,0};

        uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(0, 0); //
        uvs[2] = new Vector2(1, 0); // 
        uvs[3] = new Vector2(1, 1);
        mf.mesh.uv = uvs;

        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();

        mc.sharedMesh = mf.mesh;

        Material material = new Material(Shader.Find("Standard"));
        
        material.mainTexture = map;

        mr.material = material;

        if (true)
        {
            foreach(Vector3 v in mf.mesh.vertices)
            {
                Debug.Log(v);
            }
        }
        

        //CreateMap();



    }

    private void ReadMap()
    {
        // Leggi il file .pgm
        byte[] bytes = File.ReadAllBytes(filePath);
        string header = System.Text.Encoding.ASCII.GetString(bytes);
        string[] headerLines = header.Split('\n');

        int width = int.Parse(headerLines[1]);
        int height = int.Parse(headerLines[2]);
        int maxValue = int.Parse(headerLines[3]);


        Debug.Log($"Info: width: {width} height: {height} maxValue: {maxValue}");

        Color[] pixels = new Color[width * height];
        int dataStartIndex = header.IndexOf('\n', header.IndexOf('\n', header.IndexOf('\n') + 1) + 1) + 1;

        for (int i = 0; i < width * height; i++)
        {
            float normalizedValue = (float)bytes[dataStartIndex + i] / maxValue;
            pixels[i] = new Color(normalizedValue, normalizedValue, normalizedValue);
        }

        // Create texture and apply to material
        map = new Texture2D(width, height);
        map.SetPixels(pixels);
        map.Apply();

        GameObject.FindWithTag("CameraDebug").GetComponent<RawImage>().texture = map;

        //MeshRenderer mr = GetComponent<MeshRenderer>();
        //mr.material.mainTexture = map;
        //targetMaterial.mainTexture = texture;

        //// Crea una nuova Texture2D
        //Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);


        //uvs[0] = Vector2.zero;
        //uvs[1] = new Vector2(0, 1);
        //uvs[2] = new Vector2(1, 1);
        //uvs[3] = new Vector2(1, 0);

        //// Leggi i dati del file .pgm
        //int index = 4; // Indice per i dati del file .pgm
        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        int value;
        //        if (int.TryParse(lines[index++], out value))
        //        {
        //            Color color = new Color(value / (float)maxValue, value / (float)maxValue, value / (float)maxValue, 1.0f);
        //            texture.SetPixel(x, y, color);
        //        }
        //        else
        //        {
        //            // Gestisci il caso in cui la stringa non può essere convertita in un intero
        //            // Ad esempio, potresti impostare un valore predefinito o saltare la riga
        //            Debug.LogError("Impossibile convertire la stringa in un intero.");
        //        }
        //    }
        //}

        // Applica le modifiche alla texture
        ////texture.Apply();
        //Debug.Log(texture.width);
        //Debug.Log(texture.height);
    }

    Texture2D FlipTexture(Texture2D original)
    {
        Texture2D editedTexture = new Texture2D(original.width, original.height);

        // Copia i pixel dalla texture originale alla texture invertita
        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                // Inverti l'asse X durante la copia dei pixel
                editedTexture.SetPixel(original.width - x - 1, y, original.GetPixel(x, y));
            }
        }

        // Applica la texture modificata
        editedTexture.Apply();

        // Assign the edited texture to the material
        return editedTexture;
    }

    private void CreateMap()
    {
        //float size_w = map.width * precision;
        //float size_h = map.height * precision;

        //Mesh mesh = new Mesh();

        //Vector3[] vertices = new Vector3[4];

        //vertices[0] = new Vector3(size_w / 2, 0, size_h / 2);
        //vertices[1] = new Vector3(-size_w / 2, 0, size_h / 2);
        //vertices[2] = new Vector3(-size_w / 2, 0, -size_h / 2);
        //vertices[3] = new Vector3(size_w / 2, 0, -size_h / 2);

        //mesh.vertices = vertices;
        //mesh.triangles = new int[6] { 2,1,0,3,2,0 };


        //uvs[0] = new Vector2(vertices[0].x, vertices[0].z);
        //uvs[1] = new Vector2(vertices[1].x, vertices[1].z);
        //uvs[2] = new Vector2(vertices[2].x, vertices[2].z);
        //uvs[3] = new Vector2(vertices[3].x, vertices[3].z);

        //mesh.uv = uvs;

        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        //mf.mesh = mesh;

        //float planeWidth = transform.localScale.x;
        //float planeHeight = transform.localScale.z;

        //mr.material.mainTextureScale = new Vector2(planeWidth / size_w, planeHeight / size_h);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
