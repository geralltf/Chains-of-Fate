using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Transform target;
    public GameObject enemy;
    public float withinRange;
    public float angle;
    public float speed;

    private PlayerSensor playerSensor;
    // Start is called before the first frame update
    void Start()
    {
        playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerSensor.DetectedPlayer != null)
        {
            //float dist = Vector3.Distance(transform.position, playerSensor.DetectedPlayer.transform.position);
            //if (dist <= withinRange)
            //{
            //}
            transform.position = Vector3.MoveTowards(transform.position, playerSensor.DetectedPlayer.transform.position, speed);
        }
        else
        {
            transform.RotateAround(enemy.transform.position, Vector3.up, angle);
        }
        
    }
}
