using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using TFMessageMsg = RosMessageTypes.Tf2.TFMessageMsg;
using TransformStampedMsg = RosMessageTypes.Geometry.TransformStampedMsg;
using TransformMsg = RosMessageTypes.Geometry.TransformMsg;


public class TfDroneSubscriber : MonoBehaviour
{
    private ROSConnection rosConnection;
    private string topicName = "/tf";

    private Vector3 newPosition;
    private bool newMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<TFMessageMsg>(topicName, OnMessageReceived);
    }

    private void OnMessageReceived(TFMessageMsg message)
    {
        Debug.Log("MEsasggio ricevuto!");
        //map -> world
        Vector3 pos1 = Vector3.zero;
        Quaternion rot1 = Quaternion.identity;

        //base_link -> map
        Vector3 pos2 = Vector3.zero;
        Quaternion rot2 = Quaternion.identity;


        // TFMessageMsg -> TransformStampedMsg[] -> TransformMsg -> Vector3Msg -> Vector3/Quaternion
        foreach (TransformStampedMsg tsMsg in message.transforms)
        {
            if (tsMsg.header.frame_id == "world" && tsMsg.child_frame_id == "map")
            {
                pos1 = new Vector3((float)tsMsg.transform.translation.x, (float)tsMsg.transform.translation.y, (float)tsMsg.transform.translation.z);
                rot1 = new Quaternion((float)tsMsg.transform.rotation.x, (float)tsMsg.transform.rotation.y, (float)tsMsg.transform.rotation.z, (float)tsMsg.transform.rotation.w);
                Debug.Log(string.Format("frame_id: world child_frame_id: map postion: {0} rotation: {1}", pos1, rot1));
            }

            if (tsMsg.header.frame_id == "map" && tsMsg.child_frame_id == "base_link")
            {
                pos2 = new Vector3((float)tsMsg.transform.translation.x, (float)tsMsg.transform.translation.y, (float)tsMsg.transform.translation.z);
                rot2 = new Quaternion((float)tsMsg.transform.rotation.x, (float)tsMsg.transform.rotation.y, (float)tsMsg.transform.rotation.z, (float)tsMsg.transform.rotation.w);
                Debug.Log(string.Format("frame_id: map child_frame_id: base_link postion: {0} rotation: {1}", pos2, rot2));
            }


        }

        if (pos1.Equals(Vector3.zero) && pos2.Equals(Vector3.zero) && rot1.Equals(Quaternion.identity) && rot2.Equals(Quaternion.identity)) return;

        newPosition = pos1 + pos2;

        newMessage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (newMessage)
        {
            transform.position = newPosition;
            newMessage = false;
        }
    }
}
