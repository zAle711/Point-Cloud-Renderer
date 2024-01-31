using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using ImageMsg = RosMessageTypes.Sensor.ImageMsg;

using Unity.Robotics.ROSTCPConnector.MessageGeneration;

public class ImageSubscriber : MonoBehaviour
{
    public string topicName = "/camera/rgb/image_color";

    private ROSConnection rosConnection;
    private ImageMsg lastMessage;

    private Texture2D texture;

    public bool newMessage = false;
    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<ImageMsg>(topicName, OnMessageReceived);
    }

    private void OnMessageReceived(ImageMsg msg)
    {
        lastMessage = msg;
        newMessage = true;
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
