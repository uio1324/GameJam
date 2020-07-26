using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeFitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = GetComponent<Camera>();
        var height = Screen.height;
        var width = Screen.width;
        if(width == 1080)
        {
            camera.orthographicSize *= (float)height / 1920;
        }
        else
        {
            var aspect = ((float)height / width) / (1920f / 1080f);
            camera.orthographicSize *= aspect;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
