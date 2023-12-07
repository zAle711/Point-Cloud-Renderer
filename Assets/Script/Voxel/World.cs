using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;
using TMPro;

//using Unity.Robotics.ROSTCPConnector;
//using PoseArrayMsg = RosMessageTypes.Geometry.PoseArrayMsg;
//using PoseMsg = RosMessageTypes.Geometry.PoseMsg;


public class World : MonoBehaviour
{
	public PointCloudSubscriber pointCloudSubscriber;
	public GameObject chunkPrefab;
	public Material chunkMaterial;

	private int CHUNKSIZE;
	private int BlocksAtTime;
	private bool InvertYZ;
	private bool Centimeters;

	//Datastractures to handle voxels
	private List<Vector3> blocksList;
	//private HashSet<string> chunksToUpdate;
	private HashSet<string> chunksToUpdate;
	public Dictionary<string, Chunk> chunks;



	private bool coroutineStarted = false;

	private TextMeshProUGUI textLabel;

	private Dictionary<string, int[]> LookUpTableDM;
	//private Dictionary<string, int[]> LookUpTable5CM;

	private string currentKey = null;
	private Chunk currentChunk = null;

	private List<string> chunksAroundPlayer;
	private List<string> lastChunks;
	
	void Start()
	{
		CHUNKSIZE = VoxelConfiguration.Configuration().ChunkSize;
		InvertYZ = VoxelConfiguration.Configuration().InvertYZ;
		BlocksAtTime = VoxelConfiguration.Configuration().BlocksAtTime;
		Centimeters = VoxelConfiguration.Configuration().Centimeters;

		blocksList = new List<Vector3>();
		chunksToUpdate = new HashSet<string>();
		chunks = new Dictionary<string, Chunk>();
		chunksAroundPlayer = new List<string>();
		lastChunks = new List<string>();

		LookUpTableDM = ChunkHelper.FillTableDM();
		//LookUpTable5CM = ChunkHelper.FillTable5CM();

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

			Debug.Log(currentKey);

            if (!currentChunk.started)
            {
                currentChunk.ComputeMesh();
            }

            if (currentChunk.done)
            {
                float start = Time.realtimeSinceStartup;
                currentChunk.Render();
                float end = Time.realtimeSinceStartup;
				//Debug.Log(currentKey + " " + (end - start));
				chunksToUpdate.Remove(currentKey);

                currentChunk.done = false;
                currentChunk.started = false;

                currentKey = null;
                currentChunk = null;
            }

			yield return null;

		}
	}

    void AddBlocksToList()
    {
		List<Vector3> newPoints = pointCloudSubscriber.GetPointsFromMessage();
		blocksList.AddRange(newPoints);

	}

    void AddBlockToWorld()
	{
		float start = Time.realtimeSinceStartup;
        if (blocksList.Count >= BlocksAtTime)
        {
            for (int i = 0; i < BlocksAtTime; i++)
            {
                Vector3 point_position = blocksList[i];
                AddBlock(point_position);
            }

            blocksList.RemoveRange(0, BlocksAtTime);
        }

		//Debug.Log(string.Format("Tempo impiegato ad inserire {0} punti uguale a {1}", BlocksAtTime, Time.realtimeSinceStartup - start));
    }


	private string GetTextInfo()
    {
		return string.Format("Chunks creati {0},HashSet {1}, blocchi da aggiungere {2}", chunks.Count, chunksToUpdate.Count ,blocksList.Count);
    }

	void Update()
	{

		if (pointCloudSubscriber.newMessage) AddBlocksToList();

		AddBlockToWorld();

        if (!coroutineStarted && chunksToUpdate.Count != 0)
        {
            coroutineStarted = true;
            StartCoroutine(RenderWorldAsync());
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
		BlocksAtTime = blocksList.Count;
		//Debug.Log(blocksList.Count);
	}

	private void AddBlock(Vector3 point_position)
    {
		//int[] chunk_and_index = ChunkHelper.GetChunkAndIndexFromPosition(point_position);
		bool reading_from_ros = VoxelConfiguration.Configuration().TopicName != "";
		int[] chunk_and_index = reading_from_ros ? ChunkHelper.GetChunkAndIndexFromRosPoint(point_position) : ChunkHelper.GetChunkAndIndexFromPosition(point_position);

		string chunkName = string.Format("{0}_{1}_{2}", chunk_and_index[0], chunk_and_index[1], chunk_and_index[2]);

		Chunk c;
		if (chunks.ContainsKey(chunkName))
        {
			c = chunks[chunkName];
		}
        else
        {
			Vector3Int chunkPosition = new Vector3Int(chunk_and_index[0], chunk_and_index[1], chunk_and_index[2]);
			GameObject newChunk = Instantiate(chunkPrefab, transform);
			newChunk.name = chunkName;
			c = newChunk.GetComponent<Chunk>();
			c.SetPosition(chunkPosition);
			c.SetChunkName(chunkName);

			chunks.Add(chunkName, c);

			//newBlockPerChunk.Add(chunkName, 0);
		}

        if (Centimeters)
        {
            int index = chunk_and_index[3] + chunk_and_index[4] * CHUNKSIZE + chunk_and_index[5] * CHUNKSIZE * CHUNKSIZE;
            FillChunkData(c, index);
        }
        else
        {
            int x = chunk_and_index[3] / 10;
            int y = chunk_and_index[4] / 10;
            int z = chunk_and_index[5] / 10;

            string key = string.Format("{0}-{1}-{2}", x, y, z);
            int[] index = LookUpTableDM[key];
            FillChunkData(c, index);
        }

		chunksToUpdate.Add(c.chunkName);

        //     if (newBlockPerChunk.ContainsKey(c.chunkName) && newBlockPerChunk[c.chunkName] >= CHUNKSIZE * CHUNKSIZE * CHUNKSIZE * 0.02f)
        //     {
        //         chunksToUpdate.Add(c.chunkName);
        //         newBlockPerChunk.Remove(c.chunkName);
        //     }

    }

    private void FillChunkData(Chunk c, int index)
    {
		if (c.chunkData[index] == 0)
		{
			c.chunkData[index] = 1;

            //newBlockPerChunk[c.chunkName] = newBlockPerChunk.ContainsKey(c.chunkName) ? newBlockPerChunk[c.chunkName] + 1 : 1;
		}
    }

	private void FillChunkData(Chunk c, int[] index)
    {
		if (c.chunkData[index[0]] == 0)
        {
			foreach (int i in index)
            {
				c.chunkData[i] = 1;
            }

            //newBlockPerChunk[c.chunkName] = newBlockPerChunk.ContainsKey(c.chunkName) ? newBlockPerChunk[c.chunkName] + index.Length : index.Length;
        }
    }

}