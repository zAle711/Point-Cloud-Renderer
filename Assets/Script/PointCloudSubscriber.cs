using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using PoseArrayMsg = RosMessageTypes.Geometry.PoseArrayMsg;
using PoseMsg = RosMessageTypes.Geometry.PoseMsg;

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

    public List<Vector3> GetPointsFromMessage()
    {
        List<Vector3> newBlocks = new List<Vector3>();
        foreach(PoseMsg point in lastMessage.poses)
        {
            Vector3 block = new Vector3((float) point.position.x, (float) point.position.z, (float) point.position.y);
            newBlocks.Add(block);
        }
        newMessage = false;
        return newBlocks;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
