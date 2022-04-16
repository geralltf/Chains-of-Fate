using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    public GameObject player;

    private void Start()
    {
        //offset = transform.position - player.transform.position; // Wasn't very adaptive to different screen sizes, nor easy for others to use.

        // NEW offset based on positioning player on screen center.
        Camera camera = GetComponent<Camera>();
        
        Vector3 screenCenter = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, camera.nearClipPlane);
        
        offset = camera.ScreenToWorldPoint(screenCenter);
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
