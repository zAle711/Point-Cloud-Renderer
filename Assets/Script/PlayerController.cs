using System;
using TMPro;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;

    public float pointRadius = 5.0f;
    public int renderCircles = 1;
    public GameObject MeshTest;
    
    private LineRenderer line;

    private Ray ray;

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        InputHandler();
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MeshTest.SetActive(!MeshTest.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (MeshRenderer mr in MeshTest.transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.SetFloat("_PointSize", pointRadius);
            }
            
        }
       
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (MeshRenderer mr in MeshTest.transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.SetInt("_Circles", renderCircles);
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

// progettazione e sviluppo di un ambiente in realtà virtuale per applicazioni di robotica immersiva
// Application virutal in robotica
//-> tecniche computer graphics per realtà virtuale (ricostruzione mesh partendo da punti, filtraggio e rendering punti )
