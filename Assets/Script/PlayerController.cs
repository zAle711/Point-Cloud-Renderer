using TMPro;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;
    public GameObject waypointMenu;

    private WayPointPublisher waypointPub;
    
    private LineRenderer line;

    private Ray ray;

    private Vector3 selectedPosition;
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

    void FireRay()
    {
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            line.positionCount = 2;
            Vector3 pos = transform.position;
            pos.y -= 0.10f;
            line.SetPosition(0, pos);
            line.SetPosition(1, hitData.point);
            selectedPosition = hitData.point;
            //waypointPub.SendWaypoint(hitData.point);
        } else
        {
            selectedPosition = Vector3.zero;
        }
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireRay();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            line.positionCount = 0;
            if (selectedPosition != Vector3.zero)
            {
                Debug.Log("Punto selezionato: " + selectedPosition);
                waypointMenu.SetActive(true);
                waypointMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Punto selezionato {selectedPosition} \n Vuoi inviare la coordinata al robot?";
                
            }
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
