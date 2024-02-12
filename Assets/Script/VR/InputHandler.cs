using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject waypointMenu;
    public float menuOffset;

    public RayInteractor rayInteractor;
    // Start is called before the first frame update
    void Start()
    {
    }

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
            Debug.Log(rayInteractor.CollisionInfo.HasValue);

            if (rayInteractor.CollisionInfo.HasValue && !waypointMenu.activeSelf)
            {
                
                waypointMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuOffset;
                Vector3 rotationEuler = Camera.main.transform.rotation.eulerAngles;
                rotationEuler.z = 0;
                waypointMenu.transform.rotation = Quaternion.Euler(rotationEuler);

                Vector3 waypoint = rayInteractor.CollisionInfo.Value.Point;

                string text = $"Hai selezionato il punto: \n{waypoint}\n\nVuoi inviare la coordianta al robot?";

                waypointMenu.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                waypointMenu.SetActive(true);
            }
            //Debug.Log($"{rayInteractor.CollisionInfo.Value.Point}");
        }
    }
}
