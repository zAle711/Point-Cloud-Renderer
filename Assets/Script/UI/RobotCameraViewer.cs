using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotCameraViewer : MonoBehaviour
{
    ImageSubscriber imgSub;

    RawImage img;

    // Start is called before the first frame update
    void Start()
    {
        imgSub = GameObject.FindGameObjectWithTag("ImageSubscriber").GetComponent<ImageSubscriber>();
        img = GetComponent<RawImage>();
        img.texture = Texture2D.blackTexture;
    }

    // Update is called once per frame
    void Update()
    {
        if (imgSub.newMessage)
        {
            img.texture = imgSub.GetImage();
        }
    }
}
