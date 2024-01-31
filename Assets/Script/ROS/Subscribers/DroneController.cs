using UnityEngine;

public class DroneController : MonoBehaviour
{
    public TfSubscriber tfSubscriber;
    // Update is called once per frame
    void Update()
    {
        if (tfSubscriber.newMessage)
        {
            var (position, rotation) = tfSubscriber.GetUpdatedTransform();
            if (!position.Equals(Vector3.zero))
            {
                transform.SetPositionAndRotation(position, rotation);
            }
            
        }
    }
}
