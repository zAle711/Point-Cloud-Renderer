using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using TFMessageMsg = RosMessageTypes.Tf2.TFMessageMsg;
using TransformStampedMsg = RosMessageTypes.Geometry.TransformStampedMsg;
public class TfSubscriber : MonoBehaviour
{

    private ROSConnection rosConnection;
    private string topicName = "/tf";
    private TFMessageMsg lastMessage;
    public bool newMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<TFMessageMsg>(topicName, OnMessageReceived);
    }

    private void OnMessageReceived(TFMessageMsg msg)
    {
        lastMessage = msg;
        newMessage = true;
    }

    public (Vector3 position, Quaternion rotation) GetUpdatedTransform()
    {
        //map -> world
        Vector3 pos1 = Vector3.zero;
        Quaternion rot1 = Quaternion.identity;

        //base_link -> map
        Vector3 pos2 = Vector3.zero;
        Quaternion rot2 = Quaternion.identity;

        foreach (TransformStampedMsg tsMsg in lastMessage.transforms)
        {
            if (tsMsg.header.frame_id == "world" && tsMsg.child_frame_id == "map")
            {
                pos1 = new Vector3((float)tsMsg.transform.translation.x, (float)tsMsg.transform.translation.y, (float)tsMsg.transform.translation.z);
                rot1 = new Quaternion((float)tsMsg.transform.rotation.x, (float)tsMsg.transform.rotation.y, (float)tsMsg.transform.rotation.z, (float)tsMsg.transform.rotation.w);
            }

            if (tsMsg.header.frame_id == "map" && tsMsg.child_frame_id == "base_link")
            {
                pos2 = new Vector3((float)tsMsg.transform.translation.x, (float)tsMsg.transform.translation.y, (float)tsMsg.transform.translation.z);
                rot2 = new Quaternion((float)tsMsg.transform.rotation.x, (float)tsMsg.transform.rotation.y, (float)tsMsg.transform.rotation.z, (float)tsMsg.transform.rotation.w);
            }


        }

        Vector3 newPosition = pos1 + pos2;
        newPosition = new Vector3(newPosition.x, newPosition.z, newPosition.y);
        Quaternion newRotation = rot2 * rot1;
        newMessage = false;
        return (newPosition, newRotation);
    }

   
}
