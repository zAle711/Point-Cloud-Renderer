using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using System.Text;

//using Unity.Robotics.ROSTCPConnector;
//using PoseArrayMsg = RosMessageTypes.Geometry.PoseArrayMsg;
//using PoseMsg = RosMessageTypes.Geometry.PoseMsg;


public class World : MonoBehaviour
{
	public PointCloudSubscriber pointCloudSubscriber;
	public GameObject chunkPrefab;
	public CameraController myCamera;

	private int CHUNKSIZE;
	private int CHUNKDATALENGTH;
	private int BlocksAtTime;
	private bool InvertYZ;
	private bool Centimeters;
	private bool readingFromRos;



	//Datastractures to handle voxels
	private List<Vector3> blocksList;
	//private HashSet<string> 5555555555555555555555555555555555555555555555555;
	private HashSet<string> chunksToUpdate;
	public Dictionary<string, Chunk> chunks;

	//private HashSet<(string chunkName, Vector3Int position, int[] chunkData)> gameObjectsToAdd;
	private Dictionary<string, (Vector3Int, List<int>)> gameObjectsToAdd;

	private bool coroutineStarted = false;
	private bool coroutineStartedGameObjects = false;

	private TextMeshProUGUI textLabel;

	private Dictionary<string, int[]> LookUpTableDM;

	private string currentKey = null;
	private Chunk currentChunk = null;

	
	void Start()
	{
		CHUNKSIZE = VoxelConfiguration.Configuration().ChunkSize;
		CHUNKDATALENGTH = CHUNKSIZE * CHUNKSIZE * CHUNKSIZE;
		InvertYZ = VoxelConfiguration.Configuration().InvertYZ;
		BlocksAtTime = VoxelConfiguration.Configuration().BlocksAtTime;
		Centimeters = VoxelConfiguration.Configuration().Centimeters;
		readingFromRos = VoxelConfiguration.Configuration().TopicName != "";

		blocksList = new List<Vector3>();
		chunksToUpdate = new HashSet<string>();
		chunks = new Dictionary<string, Chunk>();
		gameObjectsToAdd = new Dictionary<string, (Vector3Int position, List<int> indexs)>();

		ChunkHelper.IntToString();

		LookUpTableDM = ChunkHelper.FillTableDM();

		textLabel = GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>();

        if (VoxelConfiguration.Configuration().FileName != "") ReadCloudFromFile();

	}


    private IEnumerator RenderWorldAsync()
    {

        while (chunksToUpdate.Count > 0)
        {
            if (currentKey == null && currentChunk == null)
            {
                currentKey = chunksToUpdate.First();
                currentChunk = chunks[currentKey];
            }


            if (!currentChunk.started)
            {
                currentChunk.ComputeMesh();
            }

            if (currentChunk.done)
            {

                currentChunk.Render();

				chunksToUpdate.Remove(currentKey);

                currentChunk.done = false;
                currentChunk.started = false;

                currentKey = null;
                currentChunk = null;
            }

			yield return null;

		}

		coroutineStarted = false;
		//Debug.Log("Rendering Coroutine Finita!");
	}

    void AddBlocksToList()
    {
		blocksList.AddRange(pointCloudSubscriber.GetPointsFromMessage());

	}

    void AddBlockToWorld()
	{

        if (blocksList.Count >= BlocksAtTime)
        {
            for (int i = 0; i < BlocksAtTime; i++)
            {
				AddBlock(blocksList[i]);

            }

            blocksList.RemoveRange(0, BlocksAtTime);
        }

    }


	private string GetTextInfo()
    {
		return string.Format("Chunks creati {0}\nGameObject da aggiungere {1}\nPunti Renderizzati: {2}", chunks.Count,gameObjectsToAdd.Count, myCamera.totalPoints);
    }

	void Update()
	{
		float start = Time.realtimeSinceStartup;

		if (pointCloudSubscriber.newMessage) AddBlocksToList();

		AddBlockToWorld();

        if (!coroutineStarted && chunksToUpdate.Count != 0 && true)
        {
			//Debug.Log("Coroutine Per Rendering partita");
			coroutineStarted = true;
            //StartCoroutine(RenderWorldAsync());
        }

		if (!coroutineStartedGameObjects && gameObjectsToAdd.Count != 0)
        {
			//Debug.Log("Coroutine Per gameObject partita");
			coroutineStartedGameObjects = true;
			//TestAddGO();
			StartCoroutine(AddGameObjectsChunkToWorld());

		}

		textLabel.text = GetTextInfo();
	}

	void ReadCloudFromFile()
    {
		string path = string.Format("Assets/Point Clouds txt/{0}.txt", VoxelConfiguration.Configuration().FileName);
		StreamReader inp_stm = new StreamReader(path);

		int offsetY = InvertYZ ? 2 : 1;
		int offsetZ = InvertYZ ? 1 : 2;

		while (!inp_stm.EndOfStream)
		{
			string inp_ln = inp_stm.ReadLine();
			string[] coords = inp_ln.Split();

			if (!coords[0].Contains(".") || !coords[1].Contains(".") || !coords[2].Contains(".")) continue;

			Vector3 point_position = new Vector3(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetY], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetZ], CultureInfo.InvariantCulture));

			AddBlock(point_position);
		}
	}

	private IEnumerator AddGameObjectsChunkToWorld()
    {

        //foreach(string chunkName in gameObjectsToAdd.Keys)
        //      {
        //	var (position, indexs) = gameObjectsToAdd[chunkName];

        //	if (indexs.Count < 25)
        //	{
        //		gameObjectsToAdd.Remove(chunkName);
        //		continue;
        //	}	


        //	GameObject newChunk = Instantiate(chunkPrefab, transform);
        //	newChunk.name = chunkName;
        //	Chunk c = newChunk.GetComponent<Chunk>();
        //	c.SetPosition(position);
        //	c.SetChunkName(chunkName);
        //	c.SetChunkData(indexs);

        //	chunks.Add(chunkName, c);
        //	chunksToUpdate.Add(chunkName);
        //	gameObjectsToAdd.Remove(chunkName);
        //	yield return new WaitForSeconds(0.03f);

        //}

        while (gameObjectsToAdd.Count > 0)
        {
            string chunkName = gameObjectsToAdd.Keys.First();
            var (position, indexs) = gameObjectsToAdd[chunkName];

			if (indexs.Count < 50)
            {
				gameObjectsToAdd.Remove(chunkName);
				Debug.Log("Chunk Rimosso");
				continue;
            }

            GameObject newChunk = Instantiate(chunkPrefab, transform);
            newChunk.name = chunkName;
            Chunk c = newChunk.GetComponent<Chunk>();
            c.SetPosition(position);
            c.SetChunkName(chunkName);
            c.SetChunkData(indexs);

            chunks.Add(chunkName, c);
            chunksToUpdate.Add(chunkName);
            gameObjectsToAdd.Remove(chunkName);
            yield return new WaitForSeconds(0.03f);
        }

        coroutineStartedGameObjects = false;
    }

	private void AddBlock(Vector3 point_position)
    {
		int chunk_x, chunk_y, chunk_z;
		int x, y, z;
		string chunkName;
		StringBuilder sb = new StringBuilder();
		if (readingFromRos)
		{
			chunk_x = (int)point_position.x;
			chunk_y = (int)point_position.y;
			chunk_z = (int)point_position.z;

			chunkName = sb.AppendFormat("{0}_{1}_{2}", ChunkHelper.IntToStringTable[chunk_x], ChunkHelper.IntToStringTable[chunk_y], ChunkHelper.IntToStringTable[chunk_z]).ToString();

			x = (int)(Math.Abs(point_position.x) * 100) % 100;
			y = (int)(Math.Abs(point_position.y) * 100) % 100;
			z = (int)(Math.Abs(point_position.z) * 100) % 100;
		} else
        {
			int[] chunk_and_index = ChunkHelper.GetChunkAndIndexFromPosition(point_position, CHUNKSIZE);
			chunk_x = chunk_and_index[0];
			chunk_y = chunk_and_index[1];
			chunk_z = chunk_and_index[2];

			chunkName = sb.AppendFormat("{0}_{1}_{2}", ChunkHelper.IntToStringTable[chunk_x], ChunkHelper.IntToStringTable[chunk_y], ChunkHelper.IntToStringTable[chunk_z]).ToString();

			x = chunk_and_index[3];
			y = chunk_and_index[4];
			z = chunk_and_index[5];
		}

		int[] chunkData = null;
		List<int> chunkDataList = null;
		if (chunks.ContainsKey(chunkName))
        {
			chunkData = chunks[chunkName].chunkData;
		}
        else
        {

            if (gameObjectsToAdd.Keys.Contains(chunkName))
            {

                chunkDataList = gameObjectsToAdd[chunkName].Item2;

            }
            else
            {
				Vector3Int chunkPosition = new Vector3Int(chunk_x, chunk_y, chunk_z);
				gameObjectsToAdd.Add(chunkName, (chunkPosition, new List<int>()));
				chunkDataList = gameObjectsToAdd[chunkName].Item2;
			}
        }

        if (Centimeters)
        {
            int index = x + y * CHUNKSIZE + z * CHUNKSIZE * CHUNKSIZE;
			if (chunkData != null)
				FillChunkData(chunkData, index);
			else
				chunkDataList.Add(index);
        }
        else
        {
			x /= 10;
			y /= 10;
			z /= 10;

			sb.Clear();
			string key = sb.AppendFormat("{0}-{1}-{2}", x, y, z).ToString();
            int[] index = LookUpTableDM[key];

			if (chunkData != null)
				FillChunkData(chunkData, index);
			else
				chunkDataList.AddRange(index);
        }

    }

    private void FillChunkData(int[] chunkData, int index)
    {
		if (chunkData[index] == 0)
		{
			chunkData[index] = 1;
		}
    }

	private void FillChunkData(int[] chunkData, int[] index)
    {
		if (chunkData[index[0]] == 0)
        {
			foreach (int i in index)
            {
				chunkData[i] = 1;
            }

        }
    }

}