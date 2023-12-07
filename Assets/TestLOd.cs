using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLOd : MonoBehaviour
{

    private LODGroup lodGroup;
	public int chunkSize = 50;
	private int[] chunkData;
	public Material chunkMaterial;

	private GameObject LOD0;
	private GameObject LOD1;

	// Start is called before the first frame update
	void Start()
    {

		//chunkData = new int[chunkSize * chunkSize * chunkSize];
		//for (int i = 0; i < chunkSize * chunkSize * chunkSize; i++)
  //      {
		//	chunkData[i] = Random.Range(0, 2);
  //      }

		//Chunk c = new Chunk(Vector3Int.zero, "0_0_0", chunkMaterial);
		//c.chunkData = chunkData;
		//c.GreedyChunk();

		//Mesh mesh = new Mesh();
		//mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		//mesh.vertices = c.vertices;
		//mesh.triangles = c.triangles;
		
		//mesh.RecalculateBounds();
		//mesh.RecalculateNormals();


		//LOD0 = transform.Find("LOD0").gameObject;
		//LOD0.GetComponent<MeshFilter>().mesh = mesh;

		//LOD1 = transform.Find("LOD1").gameObject;
		//LOD1.GetComponent<MeshFilter>().mesh = CreateCubeMesh();


		////lodMeshes[0] = new Mesh();
		////lodMeshes[1] = CreateCubeMesh();

		////for (int i = 0; i < lodMeshes.Length; i++)
		////{
		////	Renderer[] renderers = { CreateMeshRenderer(lodMeshes[i]) };
		////	lods[i] = new LOD(GetLODPercentage(i), renderers);
		////}

		////lodGroup.SetLODs(lods);

	}

	private Renderer[] CreateCube()
    {
		GameObject cube = new GameObject("cube");
		MeshFilter filter = cube.AddComponent<MeshFilter>();
		MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
		cube.transform.parent = transform;
		
		filter.mesh = CreateCubeMesh();

		Renderer[] renderers = new Renderer[] { renderer };
		return renderers;




	}

	Renderer[] CreateMeshRenderer(Mesh mesh)
	{
		// Create a new GameObject with a MeshFilter and MeshRenderer
		GameObject go = new GameObject("Cube");
		go.transform.parent = this.transform;
		MeshFilter meshFilter = go.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		Renderer renderer = go.AddComponent<MeshRenderer>();
		renderer.enabled = false; // Set to false so that the renderer doesn't render the mesh directly
		Renderer[] renderers = { renderer};
		return renderers;
	}

	float GetLODPercentage(int lodIndex)
	{
		// Calculate the LOD percentage based on your preferences
		// For example, you can evenly distribute the LOD percentages
		return lodIndex / (float)(2 - 1);
	}

	private Mesh CreateCubeMesh()
	{
		Vector3[] cube_vertices = new Vector3[]
		{
			new Vector3(0.00f, 0.00f, 0.00f),
			new Vector3(0.00f, 1.00f, 0.00f),
			new Vector3(0.00f, 0.00f, 1.00f),
			new Vector3(0.00f, 1.00f, 1.00f),
			new Vector3(1.00f, 0.00f, 0.00f),
			new Vector3(1.00f, 1.00f, 0.00f),
			new Vector3(1.00f, 0.00f, 1.00f),
			new Vector3(1.00f, 1.00f, 1.00f),
			new Vector3(0.00f, 0.00f, 0.00f),
			new Vector3(0.00f, 0.00f, 1.00f),
			new Vector3(1.00f, 0.00f, 0.00f),
			new Vector3(1.00f, 0.00f, 1.00f),
			new Vector3(0.00f, 1.00f, 0.00f),
			new Vector3(0.00f, 1.00f, 1.00f),
			new Vector3(1.00f, 1.00f, 0.00f),
			new Vector3(1.00f, 1.00f, 1.00f),
			new Vector3(0.00f, 0.00f, 0.00f),
			new Vector3(1.00f, 0.00f, 0.00f),
			new Vector3(0.00f, 1.00f, 0.00f),
			new Vector3(1.00f, 1.00f, 0.00f),
			new Vector3(0.00f, 0.00f, 1.00f),
			new Vector3(1.00f, 0.00f, 1.00f),
			new Vector3(0.00f, 1.00f, 1.00f),
			new Vector3(1.00f, 1.00f, 1.00f),
		};

		int[] cube_triangles = new int[]
		{
			2, 1, 0, 3, 1, 2, 4, 5, 6, 6, 5, 7, 10, 9, 8, 11, 9, 10, 12, 13, 14, 14, 13, 15, 18, 17, 16, 19, 17, 18, 20, 21, 22, 22, 21, 23
		};

		for (int i = 0; i < cube_vertices.Length; i++)
		{
			cube_vertices[i] *= (chunkSize / 100.0f);
		}

		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = cube_vertices;
		mesh.triangles = cube_triangles;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		return mesh;
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
