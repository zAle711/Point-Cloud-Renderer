using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVari : MonoBehaviour
{
    public Color c = Color.red;
    public float y = 0f;
    public Color start = Color.blue;
    public Color end = Color.red;
    public float min = 0f;
    public float max = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float interpolated = Mathf.InverseLerp(min, max, y);
        Debug.Log(interpolated);
        c = Color.Lerp(start, end, interpolated);

        int r, g, b;

        r = (int)c.r * 255;
        g = (int)c.g * 255;
        b = (int)c.b * 255;

        Debug.Log(r + " " + g + " " + b);
    }
}
