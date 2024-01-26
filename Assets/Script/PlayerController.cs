using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;

    private WayPointPublisher waypointPub;

    private void Start()
    {
        waypointPub = new WayPointPublisher();
    }

    void FireRay()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            Debug.Log(hitData.point);
            waypointPub.SendWaypoint(hitData.point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireRay();
        }
    }
    private void HandleMovement()
    {
        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
    }
}
