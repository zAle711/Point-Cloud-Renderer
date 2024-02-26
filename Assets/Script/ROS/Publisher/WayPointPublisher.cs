using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using PoseWithCovarianceMsg = RosMessageTypes.Geometry.PoseWithCovarianceMsg;
using PoseMsg = RosMessageTypes.Geometry.PoseMsg;
using RosMessageTypes.Geometry;

public class WayPointPublisher : MonoBehaviour
{
    private ROSConnection rosConnection;
    private string topicName = "/waypoint/test";

    private Vector3 destination;
    private bool isDestinationSelected;

    public void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.RegisterPublisher<PoseWithCovarianceMsg>(topicName);
        isDestinationSelected = false;
    }

    public void setDestination(Vector3 destination)
    {
        this.destination = destination;
        isDestinationSelected = true;
    }

    public void removeSelection()
    {
        isDestinationSelected = false;
    }
    public void SendWaypoint()
    {
        if (isDestinationSelected)
        {
            Debug.Log("Invio il punto selezionato!");
            PoseWithCovarianceMsg msg = new PoseWithCovarianceMsg();

            Quaternion q = Quaternion.identity;
            PointMsg pointMsg = new PointMsg(destination.x, destination.z, destination.y);
            QuaternionMsg quaternionMsg = new QuaternionMsg(q.x, q.y, q.z, q.w);
            msg.pose = new PoseMsg(pointMsg, quaternionMsg);

            rosConnection.Publish(topicName, msg);
        }
       
    }
}
