using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using PoseWithCovarianceMsg = RosMessageTypes.Geometry.PoseWithCovarianceMsg;
using PoseMsg = RosMessageTypes.Geometry.PoseMsg;
using RosMessageTypes.Geometry;

public class WayPointPublisher : MonoBehaviour
{
    private ROSConnection rosConnection;
    private string topicName = "/waypoint/test";

    public void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.RegisterPublisher<PoseWithCovarianceMsg>(topicName);
    }

    public void SendWaypoint(Vector3 position)
    {
        PoseWithCovarianceMsg msg = new PoseWithCovarianceMsg();

        Quaternion q = Quaternion.identity;
        PointMsg pointMsg = new PointMsg(position.x, position.z, position.y);
        QuaternionMsg quaternionMsg = new QuaternionMsg(q.x, q.y, q.z, q.w);
        msg.pose = new PoseMsg(pointMsg, quaternionMsg);

        rosConnection.Publish(topicName, msg);
    }
}
