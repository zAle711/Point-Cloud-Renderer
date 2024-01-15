using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public static class ChunkHelper
{
	public static Dictionary<int, string> IntToStringTable;
	public static string GetChunkFromPosition(Vector3 position)
    {
		return "" + IntToStringTable[(int)position.x] + "_" + IntToStringTable[(int)position.y] + "_" + IntToStringTable[(int)position.z];
    }

	public static int[] GetChunkAndIndexFromRosPoint(Vector3 position)
    {
		int chunk_x = (int)position.x;
		int chunk_y = (int)position.y;
		int chunk_z = (int)position.z;

		int x = (int)(Math.Abs(position.x) * 100) % 100;
		int y = (int)(Math.Abs(position.y) * 100) % 100;
		int z = (int)(Math.Abs(position.z) * 100) % 100;

		int[] chunk_index = { chunk_x, chunk_y, chunk_z, x, y, z };
		return chunk_index;
	}

	public static int[] GetChunkAndIndexFromPosition(Vector3 position, int CHUNKSIZE)
    {
		int chunk_x = (int)position.x;
		int chunk_y = (int)position.y;
		int chunk_z = (int)position.z;

		int pos_x = (int)(Mathf.Abs(position.x) * 100 - Mathf.Floor(Mathf.Abs(position.x)) * 100);
		int pos_y = (int)(Mathf.Abs(position.y) * 100 - Mathf.Floor(Mathf.Abs(position.y)) * 100);
		int pos_z = (int)(Mathf.Abs(position.z) * 100 - Mathf.Floor(Mathf.Abs(position.z)) * 100);
		int chunkMultiplier = 100 / CHUNKSIZE;

		chunk_x = position.x >= 0 ? chunk_x * chunkMultiplier + pos_x / CHUNKSIZE : chunk_x * chunkMultiplier - (pos_x / CHUNKSIZE) - 1;
		chunk_y = position.y >= 0 ? chunk_y * chunkMultiplier + pos_y / CHUNKSIZE : chunk_y * chunkMultiplier - (pos_y / CHUNKSIZE) - 1;
		chunk_z = position.z >= 0 ? chunk_z * chunkMultiplier + pos_z / CHUNKSIZE : chunk_z * chunkMultiplier - (pos_z / CHUNKSIZE) - 1;

		pos_x = pos_x < CHUNKSIZE ? pos_x : pos_x - (pos_x / CHUNKSIZE) * CHUNKSIZE;
		pos_y = pos_y < CHUNKSIZE ? pos_y : pos_y - (pos_y / CHUNKSIZE) * CHUNKSIZE;
		pos_z = pos_z < CHUNKSIZE ? pos_z : pos_z - (pos_z / CHUNKSIZE) * CHUNKSIZE;

		pos_x = position.x >= 0 ? pos_x : CHUNKSIZE - 1 - pos_x;
		pos_y = position.y >= 0 ? pos_y : CHUNKSIZE - 1 - pos_y;
		pos_z = position.z >= 0 ? pos_z : CHUNKSIZE - 1 - pos_z;

		int[] chunk_index = { chunk_x, chunk_y, chunk_z, pos_x, pos_y, pos_z };
		return chunk_index;

    }

	public static Vector3Int GetBlockIndex(int x, int y, int z, int ChunkSize)
	{
		int ix, iy, iz;

		ix = (x < 0) ? x & ChunkSize - 1: x % ChunkSize;
		iy = (y < 0) ? y & ChunkSize - 1: y % ChunkSize;
		iz = (z < 0) ? z & ChunkSize - 1: z % ChunkSize;

		return new Vector3Int(ix, iy, iz);
	}

	public static int GetIndex(int x, int y, int z, int ChunkSize)
	{
		if ( x < 0 || y < 0 || z < 0)
        {
			x = (x < 0) ? x & ChunkSize - 1 : x % ChunkSize;
			y = (y < 0) ? y & ChunkSize - 1 : y % ChunkSize;
			z = (z < 0) ? z & ChunkSize - 1 : z % ChunkSize;

		}
		
		return x + ChunkSize * y + ChunkSize * ChunkSize * z;
	}

	public static Dictionary<string, int[]> FillTableDM()
	{
		Dictionary<string, int[]> tempDict = new Dictionary<string, int[]>();
		int CHUNKSIZE = VoxelConfiguration.Configuration().ChunkSize;
		int cm = 10;
		int max = CHUNKSIZE / cm;
		for (int i = 0; i < max; i++)
		{
			for (int j = 0; j < max; j++)
			{
				for (int l = 0; l < max; l++)
				{

					string key = string.Format("{0}-{1}-{2}", i, j, l);
					List<int> indexs = new List<int>();

					for (int x = 0; x < cm; x++)
					{
						for (int y = 0; y < cm; y++)
						{
							for (int z = 0; z < cm; z++)
							{
								indexs.Add((i * 10 + x) + (j * 10 + y) * CHUNKSIZE + (l * 10 + z) * CHUNKSIZE * CHUNKSIZE);
							}
						}
					}
					tempDict.Add(key, indexs.ToArray());
				}
			}
		}

		return tempDict;
	}


	public static Mesh CreateCubeMesh(int chunkSize)
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

		for(int i = 0; i < cube_vertices.Length; i++)
        {
			cube_vertices[i] *= chunkSize / 100.0f;
		}

		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = cube_vertices;
		mesh.triangles = cube_triangles;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		return mesh;
    }


	public static void IntToString()
    {
		Dictionary<int, string> IntToStringTemp = new Dictionary<int, string>();

		for (int i = -1000;  i <= 1000; i++)
        {
			IntToStringTemp.Add(i, i.ToString());
        }

		IntToStringTable = IntToStringTemp;
    }
}
