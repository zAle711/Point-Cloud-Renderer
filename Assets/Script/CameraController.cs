using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//using Unity.Robotics.ROSTCPConnector;
//using TFMessageMsg = RosMessageTypes.Tf2.TFMessageMsg;
//using TransformStampedMsg = RosMessageTypes.Geometry.TransformStampedMsg;
//using TransformMsg = RosMessageTypes.Geometry.TransformMsg;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;

    public World world;

    private int CHUNK_SIZE;
    private Material selectionMaterial;
    private GameObject selection_cube;

    public List<string> chunksAround = new List<string>();
    private string currentChunk;

    public GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        CHUNK_SIZE = VoxelConfiguration.Configuration().ChunkSize;
        selectionMaterial = Resources.Load("Material/GreenSelection", typeof(Material)) as Material;
        currentChunk = ChunkHelper.GetChunkFromPosition(transform.position);    
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    InteractRaycast();
        //}

        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);

    }

    void CreateSelectionBox(Vector3 chunk_position)
    {
        Debug.Log(chunk_position);
        if (selection_cube != null)
        {
            Destroy(selection_cube);
        }
        int[] chunk = ChunkHelper.GetChunkAndIndexFromPosition(chunk_position);
        string key = string.Format("{0}_{1}_{2}", chunk[0], chunk[1], chunk[2]);

        if (world.chunks.ContainsKey(key))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = world.chunks[key].transform.parent.transform.position + new Vector3((float)CHUNK_SIZE / 200, (float)CHUNK_SIZE / 200, (float)CHUNK_SIZE / 200);
            cube.transform.localScale = new Vector3(CHUNK_SIZE * 0.01f, CHUNK_SIZE * 0.01f, CHUNK_SIZE * 0.01f);
            cube.name = "SelectionCube";
            cube.tag = "SelectionCube";
            cube.GetComponent<Renderer>().material = selectionMaterial;

            selection_cube = cube;
        } else
        {
            Debug.Log("La chiave " + key + " non è presente nel dizionario");
        }

       
    }

    void InteractRaycast()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 forwardDirection = transform.forward;

        Ray ray = new Ray(cameraPosition, forwardDirection);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            CreateSelectionBox(hitData.point);
        }

        //Vector3 interactionRayEndpoint = forwardDirection * interactionRayLength;
        //Debug.DrawLine(cameraPosition, interactionRayEndpoint);

    }
}
