using UnityEngine;

public class DroneController : MonoBehaviour
{
    public TfSubscriber tfSubscriber;
    // Update is called once per frame
    void Update()
    {

        var (position, rotation) = tfSubscriber.GetPosition();

        if (!position.Equals(Vector3.zero))
        {
            transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            //Debug.Log($"Posizione Scartata: {position} e {position.Equals(Vector3.zero)}");
        }
    }
}
