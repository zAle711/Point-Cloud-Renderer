using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMenuGUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ConfirmButton()
    {
        //Invia tramite ROS
        gameObject.SetActive(false);
    }

    public void DiscardButton()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
