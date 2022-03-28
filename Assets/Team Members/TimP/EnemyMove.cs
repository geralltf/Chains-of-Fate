using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Transform target;

    public float withinRange;

    public float speed;

    private PlayerSensor playerSensor;
    // Start is called before the first frame update
    void Start()
    {
        playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSensor.DetectedPlayer != null)
        {
            //float dist = Vector3.Distance(transform.position, playerSensor.DetectedPlayer.transform.position);
            //if (dist <= withinRange)
            //{
            //}
            transform.position = Vector3.MoveTowards(transform.position, playerSensor.DetectedPlayer.transform.position, speed);
        }
        
    }
}
