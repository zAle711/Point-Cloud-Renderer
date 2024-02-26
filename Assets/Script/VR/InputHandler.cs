using Oculus.Interaction;
using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject waypointMenu;
    public GameObject cameraUI;
    public float menuOffset;

    public WayPointPublisher waypointPub;
    public ImageSubscriber imgSub;
    public RayInteractor rayInteractor;


    // Update is called once per frame
    void Update()
    {

        //Vector3 cursorPosition = rayInteractor.CollisionInfo.Value.Point;

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            bool isActive = mainMenu.activeSelf;
            if (!isActive)
            {
                mainMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuOffset;
                Vector3 rotationEuler = Camera.main.transform.rotation.eulerAngles;
                rotationEuler.z = 0;
                mainMenu.transform.rotation = Quaternion.Euler(rotationEuler);
            }

            mainMenu.SetActive(!mainMenu.activeSelf);
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {

            if ( !waypointMenu.activeSelf && rayInteractor.CollisionInfo.HasValue)
            {
                
                waypointMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuOffset;
                Vector3 rotationEuler = Camera.main.transform.rotation.eulerAngles;
                rotationEuler.z = 0;
                waypointMenu.transform.rotation = Quaternion.Euler(rotationEuler);

                Vector3 waypoint = rayInteractor.CollisionInfo.Value.Point;
                waypointPub.setDestination(waypoint);
                string text = $"Hai selezionato il punto: \n{waypoint}\nVuoi inviare la coordianta al robot?";

                waypointMenu.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                waypointMenu.SetActive(true);
            }
            
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            if (!cameraUI.activeSelf)
            {
                cameraUI.transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuOffset;
                Vector3 rotationEuler = Camera.main.transform.rotation.eulerAngles;
                rotationEuler.z = 0;
                cameraUI.transform.rotation = Quaternion.Euler(rotationEuler);
            }
               

            cameraUI.SetActive(!cameraUI.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!cameraUI.activeSelf)
            {
                cameraUI.transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuOffset;
                Vector3 rotationEuler = Camera.main.transform.rotation.eulerAngles;
                rotationEuler.z = 0;
                cameraUI.transform.rotation = Quaternion.Euler(rotationEuler);
                imgSub.Subscribe();
            } else
            {
                imgSub.Unsubscribe();
            }

            cameraUI.SetActive(!cameraUI.activeSelf);
        }
    }


    
}
