using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Chunk : MonoBehaviour
{
	private int chunkSize;
	public int[] chunkData;
	private GameObject LOD0;
	private GameObject LOD1;

	public string chunkName;
	public int newBlocks = 0;

	public bool done = false;
	public bool started = false;

	public int totalPoints = 0;

	private List<Vector3> points;

	private Vector3[] vertices;
	private int[] triangles;

	void Awake()
	{
		chunkSize = VoxelConfiguration.Configuration().ChunkSize;
		points = new List<Vector3>();

		LOD0 = transform.Find("LOD0").gameObject;
		LOD1 = transform.Find("LOD1").gameObject;
		LOD1.GetComponent<MeshFilter>().mesh = ChunkHelper.CreateCubeMesh(chunkSize);
	}

	public List<Vector3> GetPoints()
    {
		return points;
    }

	public void SetPosition(Vector3Int position)
    {
		float positionOffset = 100.0f / chunkSize;
		Vector3 chunkPosition = new Vector3( position.x / positionOffset,position.y / positionOffset, position.z / positionOffset);	
		transform.position = chunkPosition;
    }
	
	public void SetChunkName(string chunkName)
    {
		this.chunkName = chunkName;
    }

	public void SetChunkData(List<int> data)
    {
		chunkData = new int[chunkSize * chunkSize * chunkSize];
		foreach(int point in data)
        {
			chunkData[point] = 1;
			AddPoints(point);
			totalPoints += 1;
		}
    }

	private void AddPoints(int idx)
    {

		int z = idx / (chunkSize * chunkSize);
		idx -= (z * chunkSize * chunkSize);
		int y = idx / chunkSize;
		int x = idx % chunkSize;

		points.Add(new Vector3(transform.position.x + x / 100.0f, transform.position.y + y / 100.0f, transform.position.z + z / 100.0f));
	}

    private void GreedyChunk()
    {
		// Sweep over each axis (X, Y, Z)
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		for (int Axis = 0; Axis < 3; ++Axis)
		{
			// 2 Perpendicular axis
			int Axis1 = (Axis + 1) % 3;
			int Axis2 = (Axis + 2) % 3;

			int MainAxisLimit = chunkSize;
			int Axis1Limit = chunkSize;
			int Axis2Limit = chunkSize;

			// auto DeltaAxis1 = FIntVector::ZeroValue;
			// auto DeltaAxis2 = FIntVector::ZeroValue;
			int[] DeltaAxis1 = new int[3];
			int[] DeltaAxis2 = new int[3];

			int[] ChunkItr = new int[3];
			int[] AxisMask = new int[3];

			AxisMask[Axis] = 1;

			// TArray<FMask> Mask;
			// Mask.SetNum(Axis1Limit * Axis2Limit);
			int[] Mask = new int[Axis1Limit * Axis2Limit * Axis2Limit];
			// Check each slice of the chunk
			for (ChunkItr[Axis] = -1; ChunkItr[Axis] < MainAxisLimit;)
			{
				int N = 0;

				// Compute Mask
				for (ChunkItr[Axis2] = 0; ChunkItr[Axis2] < Axis2Limit; ++ChunkItr[Axis2])
				{
					for (ChunkItr[Axis1] = 0; ChunkItr[Axis1] < Axis1Limit; ++ChunkItr[Axis1], ++N)
					{
						int index1 = ChunkHelper.GetIndex(ChunkItr[0], ChunkItr[1], ChunkItr[2], chunkSize);
						int CurrentBlock = ChunkItr[Axis] == -1 ? 0 : chunkData[index1];
						int index2 = ChunkHelper.GetIndex(ChunkItr[0] + AxisMask[0], ChunkItr[1] + AxisMask[1], ChunkItr[2] + AxisMask[2], chunkSize);
						int CompareBlock = ChunkItr[Axis] >= chunkSize - 1 ? 0 : chunkData[index2];

						bool CurrentBlockOpaque = CurrentBlock != 0;
						bool CompareBlockOpaque = CompareBlock != 0;

						if (CurrentBlockOpaque == CompareBlockOpaque)
						{
							Mask[N] = 0;
						}
						else if (CurrentBlockOpaque)
						{
							Mask[N] = 1;
						}
						else
						{
							Mask[N] = -1;
						}
					}
				}

				++ChunkItr[Axis];
				N = 0;

				// Generate Mesh From Mask
				for (int j = 0; j < Axis2Limit; ++j)
				{
					for (int i = 0; i < Axis1Limit;)
					{
						if (Mask[N] != 0)
						{
							int CurrentMask = Mask[N];
							ChunkItr[Axis1] = i;
							ChunkItr[Axis2] = j;

							int Width;

							for (Width = 1; i + Width < Axis1Limit && Mask[N + Width] == CurrentMask && CurrentMask != 0; ++Width)
							{
							}

							int Height;
							bool Done = false;

							for (Height = 1; j + Height < Axis2Limit; ++Height)
							{
								for (int k = 0; k < Width; ++k)
								{
									if (Mask[N + k + Height * Axis1Limit] == CurrentMask && CurrentMask != 0) continue;

									Done = true;
									break;
								}

								if (Done) break;
							}

							DeltaAxis1[Axis1] = Width;
							DeltaAxis2[Axis2] = Height;

							vertices.Add(new Vector3(ChunkItr[0], ChunkItr[1], ChunkItr[2]) / 100);
							vertices.Add(new Vector3(ChunkItr[0] + DeltaAxis1[0], ChunkItr[1] + DeltaAxis1[1], ChunkItr[2] + DeltaAxis1[2]) / 100);
							vertices.Add(new Vector3(ChunkItr[0] + DeltaAxis2[0], ChunkItr[1] + DeltaAxis2[1], ChunkItr[2] + DeltaAxis2[2]) / 100);
							vertices.Add(new Vector3(ChunkItr[0] + DeltaAxis2[0] + DeltaAxis1[0], ChunkItr[1] + DeltaAxis2[1] + DeltaAxis1[1], ChunkItr[2] + DeltaAxis2[2] + DeltaAxis1[2]) / 100);

							if (CurrentMask == -1)
							{
								triangles.Add(vertices.Count - 2);
								triangles.Add(vertices.Count - 3);
								triangles.Add(vertices.Count - 4);

								triangles.Add(vertices.Count - 1);
								triangles.Add(vertices.Count - 3);
								triangles.Add(vertices.Count - 2);
							}
							else
							{
								triangles.Add(vertices.Count - 4);
								triangles.Add(vertices.Count - 3);
								triangles.Add(vertices.Count - 2);

								triangles.Add(vertices.Count - 2);
								triangles.Add(vertices.Count - 3);
								triangles.Add(vertices.Count - 1);
							}

							for (int l = 0; l < Height; ++l)
							{
								for (int k = 0; k < Width; ++k)
								{
									Mask[N + k + l * Axis1Limit] = 0;
								}
							}

							i += Width;
							N += Width;

						}
						else
						{
							i++;
							N++;
						}
					}
				}
			}
		}

		this.vertices = vertices.ToArray();
		this.triangles = triangles.ToArray();
		done = true;
	}

	public async void ComputeMesh()
    {
		await Task.Run( () => GreedyChunk() );
    }

    public void Render()
    {
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		LOD0.GetComponent<MeshFilter>().mesh = mesh;

	}
	

}