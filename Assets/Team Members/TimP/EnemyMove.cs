using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour
{
    public GameObject enemy;
    
    //public float detectionRadius = 10.0f;
    public float speed = 4.0f;
    public float circularTurnSpeed = 0.9f;
    public float circularMoveDirection = 1.0f;
    public float patrollingSpeed = 1.0f;
    public float patrolNearDistance = 0.8f;
    public float patrolChangeTime = 10.0f;
    public bool randomPatrolRoutes = true;
    public bool randomTurning = true;
    public MovementType movementBehaviourFixedType = MovementType.Different;
    public MovementType currMovementBehaviour = MovementType.Circular;
    public float changeBehaviourTime = 15.0f;
    
    private bool waitTurning = false;
    private bool waitNewRoute = false;
    private bool waitNewBehaviour = false;
    
    [SerializeField] private int currentPatrolPointIndex = 0;

    [SerializeField] private List<Vector3> patrolPoints;
    [SerializeField] private float patrolPointMinDistance = 1.0f;
    [SerializeField] private float patrolPointRadius = 10.0f;
    [SerializeField] private int numPatrolPoints = 3;
    
    private PlayerSensor playerSensor;
    private WorldInfo worldInfo;

    public enum MovementType
    {
        Circular = 0,
        Patrolling = 1,
        Different = 2,
        Count = 2 // The count of all MovementTypes
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSensor = GetComponentInChildren<PlayerSensor>();
        worldInfo = FindObjectOfType<WorldInfo>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ChainsOfFate.Gerallt.GameManager.Instance.levelLoadingLock) return;
        
        //playerSensor.detectionRadius = detectionRadius;
        
        if (playerSensor.DetectedPlayer != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            if (movementBehaviourFixedType == MovementType.Different && !waitNewBehaviour)
            {
                StartCoroutine(ChangeBehaviour());
            }

            switch (currMovementBehaviour)
            {
                case MovementType.Circular:
                    CircularMove();
                    break;
                case MovementType.Patrolling:
                    PatrolMove();
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (worldInfo == null)
        {
            worldInfo = FindObjectOfType<WorldInfo>();
        }

        int i = 0;
        Vector3 sceneCenter = worldInfo.sceneBounds.center;
        foreach (Vector3 patrol in patrolPoints)
        {
            Gizmos.DrawLine(sceneCenter + patrol, sceneCenter + patrolPoints[ (i + 1) % patrolPoints.Count]);
            i++;
        }
    }

    public void MoveTowardsPlayer()
    {
        Vector3 pos = transform.position;

        Vector3 playerPos = playerSensor.DetectedPlayer.transform.position;
        Vector3 directionToPlayer = -(pos - playerPos).normalized;
        Vector3 posDelta = directionToPlayer * speed * Time.deltaTime;
            
        pos.x += posDelta.x;
        pos.z += posDelta.z;

        //pos = Vector3.MoveTowards(pos, playerPos, speed * Time.deltaTime); // Old way

        transform.position = pos;
    }

    public void PatrolMove()
    {
        if (!waitNewRoute && (randomPatrolRoutes || patrolPoints.Count == 0))
        {
            StartCoroutine(ChangePatrolRoute());
        }

        Vector3 pos = transform.position;
        Vector3 patrolPoint = worldInfo.sceneBounds.center + patrolPoints[currentPatrolPointIndex];

        float dist = Vector3.Distance(pos, patrolPoint);
        if (dist <= patrolNearDistance)
        {
            // Change to next patrol point.
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Count;
        }
        else
        {
            Vector3 posDelta = Vector3.Lerp(pos, patrolPoint, patrollingSpeed * Time.deltaTime);
            pos.x = posDelta.x;
            pos.z = posDelta.z;
            
            transform.position = pos;
        }
    }

    public void SelectPatrolRoute()
    {
        patrolPoints = new List<Vector3>();

        for (int i = 0; i < numPatrolPoints; i++)
        {
            
            Vector3 patrolPoint = new Vector3(
                patrolPointMinDistance + (Random.insideUnitSphere.x * patrolPointRadius),
                0.0f, 
                patrolPointMinDistance + (Random.insideUnitSphere.z * patrolPointRadius)
            );
            patrolPoints.Add(patrolPoint);
        }
    }
    
    public void CircularMove()
    {
        // Determine movement direction.

        if (randomTurning && !waitTurning)
        {
            StartCoroutine(ChangeDirection());
        }
            
        Vector3 pos = transform.position;
        float oldY = pos.y;

        transform.RotateAround(enemy.transform.position, Vector3.up, 360 * circularMoveDirection * circularTurnSpeed * Time.deltaTime);
            
        pos = transform.position;
        pos.y = oldY;
        transform.position = pos;
    }
        
    IEnumerator ChangeDirection()
    {
        waitTurning = true;
        //circularMoveDirection = (Random.value * 2.0f) - 0.5f;
        circularMoveDirection = Random.insideUnitSphere.x;
        if (circularMoveDirection > 0) circularMoveDirection = 1.0f;
        if (circularMoveDirection < 0) circularMoveDirection = -1.0f;
        yield return new WaitForSeconds(2.0f);
        //yield return new WaitForSeconds(Random.Range(5, 10));
        waitTurning = false;
    }
    
    IEnumerator ChangePatrolRoute()
    {
        waitNewRoute = true;
        SelectPatrolRoute();
        yield return new WaitForSeconds(patrolChangeTime);
        //yield return new WaitForSeconds(Random.Range(5, 10));
        waitNewRoute = false;
    }
    
    IEnumerator ChangeBehaviour()
    {
        waitNewBehaviour = true;
        currMovementBehaviour = RandomMovementType(); // Pick a random behaviour.
        yield return new WaitForSeconds(changeBehaviourTime);
        //yield return new WaitForSeconds(Random.Range(5, 10));
        waitNewBehaviour = false;
    }
    
    public MovementType RandomMovementType()
    {
        return (MovementType)Random.Range(0, (int)MovementType.Count);
    }
}
