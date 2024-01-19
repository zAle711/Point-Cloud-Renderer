using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

[BurstCompile]
public struct FrustumCullingJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Plane> frustumPlanes;

    [ReadOnly]
    public NativeArray<Bounds> objectBounds;

    [WriteOnly]
    public NativeArray<bool> results;

    public void Execute(int index)
    {
        // Controlla se i punti della bounding box dell'oggetto sono tutti fuori da almeno un piano del frustum
        results[index] = TestPlanesAABB(frustumPlanes, objectBounds[index]);
    }

    public bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)

    {
        for (int i = 0; i < planes.Length; i++)
        {
            Plane plane = planes[i];
            float3 normal_sign = math.sign(plane.normal);
            float3 test_point = (float3)(bounds.center) + (bounds.extents * normal_sign);

            float dot = math.dot(test_point, plane.normal);
            if (dot + plane.distance < 0)
                return false;
        }

        return true;

    }
}

public class FrustumCulling : MonoBehaviour
{
    private Camera mainCamera;
    private NativeArray<Plane> frustumPlanes;
    private NativeArray<Bounds>[] objectBoundsBuffers;
    private NativeArray<bool>[] cullingResultsBuffers;
    private int currentBufferIndex = 0;

    private Vector3[] pointCloud;
    private int[] pointCloudColors;

    public List<int> indexs = new List<int>();
    void Start()
    {
        // Ottieni la telecamera principale
        mainCamera = Camera.main;
        ReadFile();
        // Inizializza gli array NativeArray e i buffer
        frustumPlanes = new NativeArray<Plane>(GeometryUtility.CalculateFrustumPlanes(mainCamera), Allocator.Persistent);
        objectBoundsBuffers = new NativeArray<Bounds>[2];
        cullingResultsBuffers = new NativeArray<bool>[2];

        objectBoundsBuffers[0] = new NativeArray<Bounds>(CreateBounds(), Allocator.Persistent);
        objectBoundsBuffers[1] = new NativeArray<Bounds>(CreateBounds(), Allocator.Persistent);

        cullingResultsBuffers[0] = new NativeArray<bool>(objectBoundsBuffers[0].Length, Allocator.Persistent);
        cullingResultsBuffers[1] = new NativeArray<bool>(objectBoundsBuffers[1].Length, Allocator.Persistent);
    }

    private Bounds[] CreateBounds()
    {
        Vector3 boundSize = new Vector3(1, 1, 1);
        Bounds[] bounds = new Bounds[pointCloud.Length];
        for(int i = 0; i < pointCloud.Length; i++)
        {
            bounds[i] = new Bounds(pointCloud[i], boundSize);
        }

        return bounds;
    }

    void OnDestroy()
    {
        // Rilascia le risorse NativeArray e i buffer
        frustumPlanes.Dispose();
        objectBoundsBuffers[0].Dispose();
        objectBoundsBuffers[1].Dispose();
        cullingResultsBuffers[0].Dispose();
        cullingResultsBuffers[1].Dispose();
    }

    void Update()
    {
        // Aggiorna i piani del frustum
        frustumPlanes.CopyFrom(GeometryUtility.CalculateFrustumPlanes(mainCamera));

        // Scegli i buffer correnti
        NativeArray<Bounds> currentObjectBounds = objectBoundsBuffers[currentBufferIndex];
        NativeArray<bool> currentCullingResults = cullingResultsBuffers[currentBufferIndex];

        // Crea un job per la verifica del Frustum Culling
        FrustumCullingJob cullingJob = new FrustumCullingJob
        {
            frustumPlanes = frustumPlanes,
            objectBounds = currentObjectBounds,
            results = currentCullingResults
        };

        // Esegui il job in parallelo su tutti gli oggetti
        JobHandle cullingJobHandle = cullingJob.Schedule(currentObjectBounds.Length, 64);

        // Attendere la fine del job prima di continuare
        cullingJobHandle.Complete();
        indexs.Clear();
        // Ora puoi utilizzare i risultati per decidere quali oggetti renderizzare
        for (int i = 0; i < currentCullingResults.Length; i++)
        {
            if (currentCullingResults[i])
            {
                indexs.Add(i);
            }
            else
            {
                // L'oggetto è fuori dal frustum, puoi omettere il rendering
            }
        }

        Debug.Log(indexs.Count);

        // Passa al buffer successivo per il prossimo frame
        currentBufferIndex = 1 - currentBufferIndex;
    }

    private void ReadFile()
    {
        string path = string.Format("Assets/Point Clouds txt/{0}.txt", "global_point_cloud_unreal");
        StreamReader inp_stm = new StreamReader(path);
        bool InvertYZ = true;
        int offsetY = InvertYZ ? 2 : 1;
        int offsetZ = InvertYZ ? 1 : 2;

        List<Vector3> all_points = new List<Vector3>();
        List<int> all_colors = new List<int>();

        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            string[] coords = inp_ln.Split();

            if (!coords[0].Contains(".") || !coords[1].Contains(".") || !coords[2].Contains(".")) continue;

            Vector3 point_position = new Vector3(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetY], CultureInfo.InvariantCulture), float.Parse(coords[0 + offsetZ], CultureInfo.InvariantCulture));
            all_points.Add(point_position);
            all_colors.Add(encodeColor(255, 255, 255));

        }

        pointCloud = all_points.ToArray();
        pointCloudColors = all_colors.ToArray();
        //textInfo.text = string.Format("Punti renderizzati: {0}", points.Length);
    }

    private int encodeColor(int r, int g, int b)
    {
        int encoded = r << 16;
        encoded |= g << 8;
        encoded |= b;
        return encoded;
    }
   }
