using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using TFMessageMsg = RosMessageTypes.Tf2.TFMessageMsg;
using TransformStampedMsg = RosMessageTypes.Geometry.TransformStampedMsg;
using PointMsg = RosMessageTypes.Geometry.PointMsg;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class TfSubscriber : MonoBehaviour
{

    private ROSConnection rosConnection;
    public string topicName = "/base_link_position";

    public Material robotMat;
    public Material odomMat;
    public Material robotTrailMat;

    private Vector3 odomPosition = Vector3.zero; //map -> odom
    private Quaternion odomRotation = Quaternion.identity;
    private Vector3 robotPosition = Vector3.zero; //odom -> base_link
    private Quaternion robotRotation = Quaternion.identity;

    private Vector3 odomPositionUnity = Vector3.zero; //map -> odom
    private Quaternion odomRotationUnity = Quaternion.identity;
    private Vector3 robotPositionUnity = Vector3.zero; //odom -> base_link
    private Quaternion robotRotationUnity = Quaternion.identity;

    private GameObject robot;
    private GameObject odom;

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<PointMsg>(topicName, NewNewOnMessageReceived);
        InitializeGameObject();
    }

    private void NewNewOnMessageReceived(PointMsg msg)
    {
        
        Vector3 new_robot_positon = new Vector3((float)msg.x, (float)msg.z, (float)msg.y);
        Debug.Log(new_robot_positon);
        robot.transform.position = new_robot_positon;
    }

    private void NewOnMessageReceived(TransformStampedMsg msg)
    {
        Vector3 new_robotPostion = new Vector3((float) msg.transform.translation.x, (float) msg.transform.translation.z, (float) msg.transform.translation.y);
        Quaternion new_robotRotation = new Quaternion((float)msg.transform.rotation.x, (float)msg.transform.rotation.z, (float)msg.transform.rotation.y, (float)msg.transform.rotation.w);

        Debug.Log(new_robotPostion);

        robot.transform.position = new_robotPostion;
        robot.transform.rotation = new_robotRotation;

        // robot.transform.SetPositionAndRotation(new_robotPostion, new_robotRotation);
    }

    private void InitializeGameObject()
    {
        GameObject parent = GameObject.FindWithTag("VisualObjects");
        robot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        robot.name = "Robot";
        robot.transform.parent = parent.transform;
        robot.transform.position = new Vector3(0, 0.15f, 0);
        robot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        robot.GetComponent<MeshRenderer>().material = robotMat;
        //TrailRenderer robotTrail = robot.AddComponent<TrailRenderer>();
        //robotTrail.time = 10f;
        //robotTrail.material = robotTrailMat;

        odom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        odom.name = "Odom";
        odom.transform.parent = parent.transform;
        odom.transform.position = new Vector3(0, 0.15f, 0);
        odom.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        odom.GetComponent<MeshRenderer>().material = odomMat;
    }

    public (Vector3, Quaternion) GetPosition()
    {
        Debug.Log(odomPosition);
        return (odomPosition + robotPosition, odomRotation * robotRotation);
    }

    private void OnMessageReceived(TFMessageMsg msg)
    {
        foreach (TransformStampedMsg transformStamped in msg.transforms)
        {
            robotPosition = new Vector3(
                        (float)transformStamped.transform.translation.x,
                        (float)transformStamped.transform.translation.z,
                        (float)transformStamped.transform.translation.y
             );

            Debug.Log(robotPosition);
        }

        robot.transform.position = robotPosition;
        //string mapFrameId = "map";
        //string odomFrameId = "odom";
        //string robotFrameId = "base_link";

        //foreach (TransformStampedMsg transformStamped in msg.transforms)
        //{
        //    if (transformStamped.header.frame_id == mapFrameId && transformStamped.child_frame_id == odomFrameId)
        //    {
        //        //odomPositionUnity = transformStamped.transform.translation.From<FLU>();
        //        //odomRotationUnity = transformStamped.transform.rotation.From<FLU>();
        //        odomRotation = new Quaternion(
        //            (float)transformStamped.transform.rotation.x,
        //            (float)transformStamped.transform.rotation.z,
        //            (float)transformStamped.transform.rotation.y,
        //            (float)transformStamped.transform.rotation.w
        //        );

        //        odomPosition = new Vector3(
        //            (float)transformStamped.transform.translation.x,
        //            (float)transformStamped.transform.translation.z,
        //            (float)transformStamped.transform.translation.y
        //        );
        //    }
        //    else if (transformStamped.header.frame_id == odomFrameId && transformStamped.child_frame_id == robotFrameId)
        //    {
        //        // Extract position and rotation of robot frame

        //        //robotPositionUnity = transformStamped.transform.translation.From<FLU>();
        //        //robotRotationUnity = transformStamped.transform.rotation.From<FLU>();

        //        robotRotation = new Quaternion(
        //            (float)transformStamped.transform.rotation.x,
        //            (float)transformStamped.transform.rotation.z,
        //            (float)transformStamped.transform.rotation.y,
        //            (float)transformStamped.transform.rotation.w
        //        );



        //        Vector3 rot = robotRotation.eulerAngles;

        //        robotRotation = Quaternion.Euler(rot.x, rot.z, rot.y);
        //        Quaternion temp =  Quaternion.identity *
        //                Quaternion.AngleAxis(robotRotation.eulerAngles.x, Vector3.right) *
        //                Quaternion.AngleAxis(robotRotation.eulerAngles.y, Vector3.up) *
        //                Quaternion.AngleAxis(robotRotation.eulerAngles.z, Vector3.forward);

        //        robotPosition = new Vector3(
        //            (float)transformStamped.transform.translation.x,
        //            (float)transformStamped.transform.translation.z,
        //            (float)transformStamped.transform.translation.y
        //        );

        //        Debug.Log($"{temp} {robotRotation}");
        //    }
        //}

        ////Debug.Log($"{robotPosition} {robotPositionUnity} {robotRotation} {robotRotationUnity}");

        ////Debug.Log($"ODOM: {odomPosition} - BASE LINK {robotPosition} - {odomPosition + robotPosition}");
        //robot.transform.position = odomPosition + robotPosition;
        //odom.transform.position = odomPosition;

    }

}

