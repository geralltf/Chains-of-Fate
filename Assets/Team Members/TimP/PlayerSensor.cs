using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public float detectionRadius;
    public PlayerController DetectedPlayer;

    private WorldInfo worldInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        worldInfo = FindObjectOfType<WorldInfo>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // private void FixedUpdate()
    // {
    //     if (ChainsOfFate.Gerallt.GameManager.Instance.levelLoadingLock) return;
    //
    //     RaycastHit[] hits = Physics.SphereCastAll(transform.parent.position, detectionRadius, transform.parent.forward);
    //     
    //     DetectedPlayer = null;
    //     
    //     foreach (RaycastHit hit in hits)
    //     {
    //         PlayerController playerController = hit.transform.GetComponent<PlayerController>();
    //         
    //         if (playerController != null)
    //         {
    //             DetectedPlayer = playerController;
    //
    //             break;
    //         }
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            DetectedPlayer = playerController;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            DetectedPlayer = null;
        }
    }
}
