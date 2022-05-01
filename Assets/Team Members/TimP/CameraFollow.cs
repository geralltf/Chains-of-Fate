using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public Vector3 orthoOffset;
    public Vector3 perspectiveOffset;
    public float trackingSpeed = 2.0f;

    private Vector3 offset;
    private Camera _camera;
    private PlayerController playerController;
    
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

        playerController = player.GetComponent<PlayerController>();
        playerController.OnReady += PlayerController_OnReady;
    }

    private Vector3 GetOffset()
    {
        if (_camera.orthographic)
        {
            offset = orthoOffset;
        }
        else
        {
            offset = perspectiveOffset;
        }

        return offset;
    }
    
    private void PlayerController_OnReady()
    {
        transform.position = playerController.defaultSpawnLocation + GetOffset();
    }

    public void UpdateZ()
    {
        Vector3 pos = transform.position;

        pos.x = playerController.transform.position.x + GetOffset().x;
        pos.y = playerController.transform.position.y + GetOffset().y;
        pos.z = playerController.transform.position.z + GetOffset().z;
        
        transform.position = pos;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateZ();
        
        //transform.position = player.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, player.transform.position, trackingSpeed * Time.fixedDeltaTime);
    }
}
