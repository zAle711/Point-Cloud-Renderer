using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using PoseArrayMsg = RosMessageTypes.Geometry.PoseArrayMsg;

public class PointCloudSubscriber : MonoBehaviour
{
    private ROSConnection rosConnection;
    private string topicName;

    private PoseArrayMsg lastMessage;
    public bool newMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        topicName = VoxelConfiguration.Configuration().TopicName;
        if (topicName != "") rosConnection.Subscribe<PoseArrayMsg>(topicName, OnMessageReceived);
    }

    private void OnMessageReceived(PoseArrayMsg msg)
    {
        lastMessage = msg;
        newMessage = true;
    }

    public Vector3[] GetPointsFromMessage()
    {
        Vector3[] newBlocks = new Vector3[lastMessage.poses.Length];

       for(int i = 0; i < newBlocks.Length; i++)
        {
            newBlocks[i] = new Vector3((float)lastMessage.poses[i].position.x, (float)lastMessage.poses[i].position.z, (float)lastMessage.poses[i].position.y);
        }

        newMessage = false;
        lastMessage = null;
        return newBlocks;
    }

}
