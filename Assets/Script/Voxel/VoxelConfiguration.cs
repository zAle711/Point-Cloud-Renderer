using UnityEngine;


[CreateAssetMenu(menuName = "VoxelsSetting")]
public class VoxelConfiguration : ScriptableObject
{
    private static VoxelConfiguration instance;
    public int ChunkSize = 100;
    public bool InvertYZ = true;
    public bool Centimeters = true;
    public int BlocksAtTime = 2500;
    public string TopicName = "points_array"; //points_array
    public string FileName = ""; //global_point_cloud_rgb  global_point_cloud_unreal
    public static VoxelConfiguration Configuration()
    {
        if (instance == null)
        {
            instance = Resources.Load<VoxelConfiguration>("Config/VoxelConfig");
        }
        return instance;
    }


}