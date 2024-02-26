using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Robotics.ROSTCPConnector;
using ImageMsg = RosMessageTypes.Sensor.ImageMsg;

using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using UnityEngine.UI;

public class ImageSubscriber : MonoBehaviour
{
    public string topicName = "/camera/rgb/image_color";

    private ROSConnection rosConnection;
    private ImageMsg lastMessage;

    private Texture2D texture;

    public bool newMessage = false;
    public bool isPanelActive = false;

    public GameObject imagePanel;
    private RawImage img;
    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        img = imagePanel.GetComponentInChildren<RawImage>();
       
    }

    public void Unsubscribe()
    {
        rosConnection.Unsubscribe(topicName);
        img.texture = null;
        img.color = new Color(0, 0, 0, 0.20f);
    }

    public void Subscribe()
    {
        rosConnection.Subscribe<ImageMsg>(topicName, OnMessageReceived);
        img.color = Color.white;
        SetPanelTitle();
    }

    private void SetPanelTitle()
    {
        TextMeshProUGUI title = imagePanel.GetComponentInChildren<TextMeshProUGUI>();
        title.text = topicName;
    }

    private void OnMessageReceived(ImageMsg msg)
    {
        Debug.Log("immagine arrivata!");
        
        lastMessage = msg;
        newMessage = true;

        img.texture = msg.ToTexture2D();

    }

    public Texture2D GetImage()
    {
        if (lastMessage == null) 
            return Texture2D.blackTexture;

        if (texture == null)
            texture = lastMessage.ToTexture2D();

        byte[] imageBytes = lastMessage.data;
        texture.LoadRawTextureData(imageBytes);
        texture.Apply();

        newMessage = false;

        return texture;
    }
}
