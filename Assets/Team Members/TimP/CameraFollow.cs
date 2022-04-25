using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    private Camera _camera;
    
    public GameObject player;
    public Vector3 orthoOffset;
    public Vector3 perspectiveOffset;
    public float trackingSpeed = 2.0f;
    
    public Vector3 GetCenterWorldPosition()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, _camera.nearClipPlane);

        Vector3 worldCenter = _camera.ScreenToWorldPoint(screenCenter);

        return worldCenter;
    }
    
    private void Awake()
    {
        //offset = transform.position - player.transform.position;

        // NEW offset based on positioning player on screen center.
        _camera = GetComponent<Camera>();
        
        offset = GetCenterWorldPosition();

        if (player == null)
        {
            player = ChainsOfFate.Gerallt.GameManager.Instance.GetPlayer().gameObject;
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position = player.transform.position + offset;
        transform.position = Vector3.Lerp(player.transform.position + offset, player.transform.position + offset, trackingSpeed * Time.fixedDeltaTime);
        
        // Vector3 position = transform.position;
        // position.x = Mathf.Lerp(transform.position.x, player.transform.position.x + offset.x, trackingSpeed * Time.fixedDeltaTime);
        // position.y = Mathf.Lerp(transform.position.y, player.transform.position.y + offset.y, trackingSpeed * Time.fixedDeltaTime);
        // transform.position = position;
        
        if (_camera.orthographic)
        {
            transform.position += orthoOffset;
        }
        else
        {
            transform.position += perspectiveOffset;
        }
    }
}
