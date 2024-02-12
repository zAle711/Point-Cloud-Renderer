using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PointCloudVR;
using TMPro;

public class MenuActions : MonoBehaviour
{
    private Toggle point_cloud_toggle;
    private Toggle mesh_toggle;
    private TMP_Dropdown point_cloud_render_dropdown;


    public PointCloudRenderer pcRenderer;
    public GameObject meshObj;
    // Start is called before the first frame update
    void Start()
    {
        point_cloud_toggle = GameObject.Find("Toggle_Point_Cloud").GetComponent<Toggle>();
        point_cloud_toggle.onValueChanged.AddListener(delegate { pcValueChanged(point_cloud_toggle); });
        
        mesh_toggle = GameObject.Find("Toggle_Mesh").GetComponent<Toggle>();
        mesh_toggle.onValueChanged.AddListener(delegate { meshValueChanged(mesh_toggle); });

        point_cloud_render_dropdown = GameObject.Find("Dropdown_Render_Mode").GetComponent<TMP_Dropdown>();
        
        point_cloud_render_dropdown.onValueChanged.AddListener(delegate { dropdownValueChanged(point_cloud_render_dropdown); });

        transform.gameObject.SetActive(false);
    }

    private void meshValueChanged(Toggle change)
    {
        meshObj.SetActive(change.isOn);
    }

    private void pcValueChanged(Toggle change)
    {
        pcRenderer.render = change.isOn;
    }

    private void dropdownValueChanged(TMP_Dropdown change)
    {
        
        switch (change.captionText.text)
        {
            case "Point":
                pcRenderer.SetTopology(PointCloudVR.RenderMode.POINT);
                break;
            case "Quad":
                pcRenderer.SetTopology(PointCloudVR.RenderMode.QUAD);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
