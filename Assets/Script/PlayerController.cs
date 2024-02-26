using TMPro;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;
    public GameObject waypointMenu;
    
    private LineRenderer line;

    private Ray ray;

    private void Start()
    {

        //waypointMenu = GameObject.FindGameObjectWithTag("WaypointMenu");


        line = GetComponent<LineRenderer>();
        line.sortingOrder = 1;
        line.startColor = Color.green;
        line.endColor = Color.red;

        Debug.Log(waypointMenu);

        //line.useWorldSpace = true;
        //waypointPub = new WayPointPublisher();
    }

    public void ConfirmButton()
    {
        //waypointPub.SendWaypoint(selectedPosition);
        waypointMenu.SetActive(false);
    }

    public void DeclineButton()
    {
        waypointMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

       
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

// progettazione e sviluppo di un ambiente in realtà virtuale per applicazioni di robotica immersiva
// Application virutal in robotica
//-> tecniche computer graphics per realtà virtuale (ricostruzione mesh partendo da punti, filtraggio e rendering punti )
