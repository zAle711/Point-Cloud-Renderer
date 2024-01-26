using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 0.05f;
    public float speedY = 0.03f;
    public float rotationSpeed = 1.0f;
    public GameObject head;

    // Update is called once per frame
    void Update()
    {

        if (OVRInput.Get(OVRInput.Button.One))
        {
            transform.position += new Vector3(0f, -speedY, 0f);
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            transform.position += new Vector3(0f, speedY, 0f);
        }

        //MOVEMENT
        Vector2 axis_1 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        axis_1 = Vector2.ClampMagnitude(axis_1, 1f);

        Vector3 cameraForward = head.transform.forward;
        Vector3 cameraRight = head.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraRight.Normalize();
        cameraRight.Normalize();

        Vector3 direction = axis_1.x * cameraRight + axis_1.y * cameraForward;
        Vector3 velocity = direction * speed;

        transform.position += velocity;

        //ROTATION
        Vector2 axis_2 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        float rotationAmount = axis_2.x * rotationSpeed;

        // Applica la rotazione alla camera o all'oggetto desiderato
        transform.Rotate(Vector3.up, rotationAmount);


    }
}
