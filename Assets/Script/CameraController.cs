using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;

    private Vector3[] points;
    private int[] colors;

    public int totalPoints = 0;
    
    private Dictionary<string, List<Vector3>> nearPoints;
    private List<MeshRenderer> chunksInsideArea;
    private Material pointMaterial;
    private float pointSize = 5.0f;

    private bool test = false;

    private bool pointLoaded = false;
    private Task<(Vector3[], int[])> computePointsTask;

    public World world;

    private float distance = 10.0f;
    private List<Chunk> nearChunks;

    private bool showPoints = false;

    private void Start()
    {
        nearPoints = new Dictionary<string, List<Vector3>>();
        chunksInsideArea = new List<MeshRenderer>();
        pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        nearChunks = new List<Chunk>();
    }
   
    private void EditRenderChunks(bool show)
    {
        if (nearChunks.Count != 0)
        {
            foreach(Chunk c in nearChunks)
            {
                c.meshRenderer.enabled = show;
            }
            if (show) nearChunks.Clear();
        }
    }

    private void UpdateNearPoints()
    {
        if (computePointsTask == null) return;

        if (computePointsTask.IsCompleted)
        {
            (Vector3[] pointsArray, int[] colorsArray) = computePointsTask.Result;
            points = pointsArray;
            colors = colorsArray;
            totalPoints = points.Length;
            pointLoaded = true;
            EditRenderChunks(false);

        }
        //if (computePointsTask == null)
        //{
        //    computePointsTask = ComputePointsAsync(nearPoints);
        //}

        //if (computePointsTask.IsCompleted)
        //{
        //    (Vector3[] pointsArray, int[] colorsArray) = computePointsTask.Result;
        //    points = pointsArray;
        //    colors = colorsArray;
        //    pointLoaded = true;
        //    computePointsTask = null;
        //}
    }

    private void HandleMovement()
    {
        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
    }

    void Update()
    {

        UpdateNearPoints();
        HandleMovement();


        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (!showPoints || computePointsTask == null)
            {
                computePointsTask = ComputePointsWithDistanceAsync(transform.position, world.chunks);
                showPoints = true;
                return;
            }

            if (showPoints)
            {
                points = null;
                colors = null;
                pointLoaded = false;
                EditRenderChunks(true);
                showPoints = false;
                totalPoints = 0;
            }
            //if (!test)
            //{
            //    MeshRenderer[] chunksToHide = new MeshRenderer[chunksInsideArea.Count];
            //    chunksInsideArea.CopyTo(chunksToHide);
            //    for (int i = 0; i < chunksToHide.Length; i++)
            //    {
            //        chunksToHide[i].GetComponent<MeshRenderer>().enabled = false;
            //    }
            //}
            //test = !test;
            ////Debug.Log(points.Length);
        }

    }

    private Task<(Vector3[], int[])> ComputePointsWithDistanceAsync(Vector3 cameraPosition, Dictionary<string, Chunk> chunks)
    {
        return Task.Run( () => {

            List<Vector3> allPoints = new List<Vector3>();

            foreach (KeyValuePair<string, Chunk> entry in chunks)
            {
                //Vector3 chunkPosition = GetPositionFromName(entry.Key);
                //Debug.Log(cameraPosition + " " + entry.Value.chunkPosition + " " + Vector3.Distance(cameraPosition, entry.Value.chunkPosition));
                if (Vector3.Distance(cameraPosition, entry.Value.chunkPosition) <= distance)
                {
                    nearChunks.Add(entry.Value);
                    allPoints.AddRange(entry.Value.GetPoints());
                    //Debug.Log("Aggiunti: " + entry.Value.GetPoints().Count + " punti.");
                }
            }
            Vector3[] pointsArray = allPoints.ToArray();
            int[] colorsArray = new int[pointsArray.Length];
            for (int i = 0; i < pointsArray.Length; i++)
            {
                colorsArray[i] = 16777215;
            }

            return (pointsArray, colorsArray);
        });
    }

    //private async Task<(Vector3[], int[])> ComputePointsWithDistanceAsync(Vector3 cameraPosition, Dictionary<string, Chunk> chunks)
    //{
    //    TaskCompletionSource<(Vector3[], int[])> tcs = new TaskCompletionSource<(Vector3[], int[])>();

    //    await Task.Run( () => {

    //        List<Vector3> allPoints = new List<Vector3>();

    //        foreach (KeyValuePair<string, Chunk> entry in chunks)
    //        {
    //            //Vector3 chunkPosition = GetPositionFromName(entry.Key);
    //            //Debug.Log(cameraPosition + " " + entry.Value.chunkPosition + " " + Vector3.Distance(cameraPosition, entry.Value.chunkPosition));
    //            if (Vector3.Distance(cameraPosition, entry.Value.chunkPosition) <= distance)
    //            {
    //                nearChunks.Add(entry.Value);
    //                allPoints.AddRange(entry.Value.GetPoints());
    //                //Debug.Log("Aggiunti: " + entry.Value.GetPoints().Count + " punti.");
    //            }
    //        }
    //        Vector3[] pointsArray = allPoints.ToArray();
    //        int[] colorsArray = new int[pointsArray.Length];
    //        for (int i = 0; i < pointsArray.Length; i++)
    //        {
    //            colorsArray[i] = 16777215;
    //        }

    //        tcs.SetResult((pointsArray, colorsArray));

    //    });

    //    return await tcs.Task;
    //}

    private async Task<(Vector3[], int[])> ComputePointsAsync(Dictionary<string, List<Vector3>> input_points)
    {
        // Crea un oggetto TaskCompletionSource con il tipo di risultato desiderato
        TaskCompletionSource<(Vector3[], int[])> tcs = new TaskCompletionSource<(Vector3[], int[])>();

        await Task.Run(() =>
        {
            List<Vector3> allPoints = new List<Vector3>();

            foreach (List<Vector3> p in input_points.Values.ToList())
            {
                allPoints.AddRange(p);
            }

            Vector3[] pointsArray = allPoints.ToArray();
            int[] colorsArray = new int[pointsArray.Length];
            for (int i = 0; i < pointsArray.Length; i++)
            {
                colorsArray[i] = 16777215;
            }

            tcs.SetResult((pointsArray, colorsArray));

        });

        // Restituisci il Task associato all'oggetto TaskCompletionSource
        return await tcs.Task;
    }


    private void DrawPoints()
    {
        if (points == null || points.Length == 0)
            return;

        ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
        positionsBuffer.SetData(points);
        colorsBuffer.SetData(colors);
        pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
        pointMaterial.SetBuffer("_Positions", positionsBuffer);
        pointMaterial.SetBuffer("_Colors", colorsBuffer);
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Length, 1);

        positionsBuffer.Release();
        colorsBuffer.Release();

    }

    private void OnRenderObject()
    {
        if (pointLoaded){
            DrawPoints();
        }
        
    }













    //private void ComputePoints()
    //{
    //    if (nearPoints.Keys.Count == 0) return;

    //    List<Vector3> allPoints = new List<Vector3>();

    //    foreach (List<Vector3> p in nearPoints.Values)
    //    {
    //        allPoints.AddRange(p);
    //    }

    //    points = allPoints.ToArray();

    //    colors = new int[points.Length];
    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        colors[i] = 16777215;
    //    }

    //    pointLoaded = true;
    //    totalPoints = points.Length;

    //}



}
