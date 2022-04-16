using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WorldInfo : MonoBehaviour
{
    //public float loadRange = 50; // Dont need this anymore. Found dynamically from ground plane collider bounds extents.
    public string leftScene, rightScene, upScene, downScene;
    public bool enableDebugColour = false;
    public Color debugColour;
    
    private Transform player;
    private Vector3 centrePoint;
    private SceneDirection newSceneDirection;
    internal Bounds sceneBounds;
    [SerializeField] private SceneDirection approachingDirection = SceneDirection.Undefined;
    
    private bool leftSceneLoaded, rightSceneLoaded, upSceneLoaded, downSceneLoaded;

    private Scene? thisScene;
    private Scene? theNewScene;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
        centrePoint = transform.position;

        // Fetch the Collider from the ground plane 
        Collider thisCollider = GetComponent<Collider>();
        Vector3 mapSize = thisCollider.bounds.size;

        // Get the map bounds for this scene.
        sceneBounds = new Bounds(centrePoint, mapSize);

        if (enableDebugColour)
        {
            GetComponent<MeshRenderer>().material.color = debugColour;
        }
    }
        

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckPosition();
    }

    public enum SceneDirection
    {
        Undefined,
        Left,
        Right,
        Top,
        Bottom
    }
    public void TryLoadScene(SceneDirection sceneDirection)
    {
        bool sceneLoaded = false;
        string sceneName = string.Empty;
        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

        if (gameManager.levelLoadingLock) return; // Can't load new levels because we are in progress of loading another.
        
        switch (sceneDirection)
        {
            case SceneDirection.Left:
                sceneLoaded = leftSceneLoaded;
                sceneName = leftScene;
                break;
            case SceneDirection.Right:
                sceneLoaded = rightSceneLoaded;
                sceneName = rightScene;
                break;
            case SceneDirection.Top:
                sceneLoaded = upSceneLoaded;
                sceneName = upScene;
                break;
            case SceneDirection.Bottom:
                sceneLoaded = downSceneLoaded;
                sceneName = downScene;
                break;
        }
        
        if (!sceneLoaded)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                gameManager.levelLoadingLock = true; // Lock out others from loading levels.
                
                switch (sceneDirection)
                {
                    case SceneDirection.Left:
                        leftSceneLoaded = true;
                        break;
                    case SceneDirection.Right:
                        rightSceneLoaded = true;
                        break;
                    case SceneDirection.Top:
                        upSceneLoaded = true;
                        break;
                    case SceneDirection.Bottom:
                        downSceneLoaded = true;
                        break;
                }
                
                newSceneDirection = sceneDirection;
                SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
                
                gameManager.ShowLevelLoadingIndicator(sceneName);
                
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }

    public bool CheckOutsideBounds(SceneDirection sceneDirection)
    {
        bool outsideBounds = false;
        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;
        
        // Move the player back into the scene away from the out of bounds zone.
        Vector3 dirBounds = Vector3.zero;
        float dist = 0;
        Vector3 pos = player.position;
        Vector3 boundaryLocation = Vector3.zero;
        string sceneName = string.Empty;
        Vector3 posDelta = Vector3.zero;
        bool boundsTest = false;
        float dt = Time.fixedDeltaTime;
        
        switch (sceneDirection)
        {
            case SceneDirection.Left:
                sceneName = leftScene;
                dirBounds.x += -sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x < centrePoint.x - (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Right:
                sceneName = rightScene;
                dirBounds.x += sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x > centrePoint.x + (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Top:
                sceneName = upScene;
                dirBounds.z += sceneBounds.extents.z;
                boundaryLocation.z = (sceneBounds.center.z + dirBounds.z);
                dist = (pos.z - boundaryLocation.z);
                posDelta.z = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.z > centrePoint.z + (sceneBounds.extents.z - gameManager.boundaryRange));
                break;
            case SceneDirection.Bottom:
                sceneName = downScene;
                dirBounds.z += -sceneBounds.extents.z;
                boundaryLocation.z = (sceneBounds.center.z + dirBounds.z);
                dist = (pos.z - boundaryLocation.z);
                posDelta.z = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.z < centrePoint.z - (sceneBounds.extents.z - gameManager.boundaryRange));
                break;
        }

        if (string.IsNullOrEmpty(sceneName) && dist < gameManager.boundaryMinDistance && boundsTest)
        {
            float oldY = pos.y;
            pos.x += posDelta.x;
            pos.z += posDelta.z;
            pos.y = oldY;
                
            player.position = pos;

            outsideBounds = true;
        }

        return outsideBounds;
    }
    
    private void SceneManager_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;

        // Get the offset from the current map to apply to the new map.
        Vector3 loadOffset = Vector3.zero;
        SceneDirection approaching = SceneDirection.Undefined;
        
        switch (newSceneDirection)
        {
            case SceneDirection.Left:
                loadOffset.x += -sceneBounds.extents.x;
                approaching = SceneDirection.Right;
                break;
            case SceneDirection.Right:
                loadOffset.x += sceneBounds.extents.x;
                approaching = SceneDirection.Left;
                break;
            case SceneDirection.Top:
                loadOffset.z += sceneBounds.extents.z;
                approaching = SceneDirection.Bottom;
                break;
            case SceneDirection.Bottom:
                loadOffset.z += -sceneBounds.extents.z;
                approaching = SceneDirection.Top;
                break;
        }
        loadOffset *= 2.0f; // Double the extents is the full map size.
        
        // Translate the new scene's root objects to new position based on current scene's position.
        GameObject[] rootObjects = newScene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            GameObject go = rootObjects[i];

            Transform rootTransform = go.GetComponent<Transform>();
            WorldInfo worldInfo = go.GetComponent<WorldInfo>();
            
            if (worldInfo != null)
            {
                worldInfo.sceneBounds.center = sceneBounds.center + loadOffset;
                worldInfo.transform.position = sceneBounds.center + loadOffset;
                worldInfo.centrePoint = worldInfo.transform.position;
                worldInfo.thisScene = newScene;
                worldInfo.approachingDirection = approaching;

                continue;
            }

            if (rootTransform != null)
            {
                rootTransform.position = sceneBounds.center + loadOffset;
            }
        }
        
        // Unload old scene since the new scene has loaded.
        if (!thisScene.HasValue)
        {
            thisScene = SceneManager.GetActiveScene();
        }

        theNewScene = newScene;
        
        // Hide the level loading indicator.
        ChainsOfFate.Gerallt.GameManager.Instance.HideLevelLoadingIndicator();
    }

    private void FinishLoading()
    {
        if (theNewScene.HasValue)
        {
            SceneManager.UnloadSceneAsync(thisScene.Value);
            SceneManager.SetActiveScene(theNewScene.Value);

            theNewScene = null;

            // Disengage lock since we are finished loading the level.
            ChainsOfFate.Gerallt.GameManager.Instance.levelLoadingLock = false;
        }
    }

    void CheckLeft(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Left
        if ((pos.x > centrePoint.x - sceneBounds.extents.x + gameManager.unloadRange) && approachingDirection == SceneDirection.Left)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Left scene
        if (pos.x < centrePoint.x - (sceneBounds.extents.x - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Left))
            {
                TryLoadScene(SceneDirection.Left);
            }
        }
        if (pos.x < centrePoint.x - sceneBounds.extents.x)
        {
            FinishLoading();
        }
    }
    
    void CheckRight(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Right
        if ((pos.x < centrePoint.x + sceneBounds.extents.x - gameManager.unloadRange) && approachingDirection == SceneDirection.Right)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Right scene
        if (pos.x > centrePoint.x + (sceneBounds.extents.x - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Right))
            {
                TryLoadScene(SceneDirection.Right);
            }
        }
        if (pos.x > centrePoint.x + sceneBounds.extents.x)
        {
            FinishLoading();
        }
    }
    
    void CheckTop(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Top
        if ((pos.z < centrePoint.z + sceneBounds.extents.z - gameManager.unloadRange) && approachingDirection == SceneDirection.Top)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Top scene
        if (pos.z > centrePoint.z + (sceneBounds.extents.z - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Top))
            {
                TryLoadScene(SceneDirection.Top);
            }
        }
        if (pos.z > centrePoint.z + sceneBounds.extents.z)
        {
            FinishLoading();
        }
    }
    
    void CheckBottom(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Bottom
        if ((pos.z > centrePoint.z - sceneBounds.extents.z + gameManager.unloadRange) && approachingDirection == SceneDirection.Bottom)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Bottom scene
        if (pos.z < centrePoint.z - (sceneBounds.extents.z - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Bottom))
            {
                TryLoadScene(SceneDirection.Bottom);
            }
        }
        if (pos.z < centrePoint.z - sceneBounds.extents.z)
        {
            FinishLoading();
        }
    }
    
    void CheckPosition()
    {
        if (player.hasChanged)
        {
            Vector3 pos = player.position;
            ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

            CheckLeft(pos, gameManager);
            CheckRight(pos, gameManager);
            CheckTop(pos, gameManager);
            CheckBottom(pos, gameManager);
        }
        
        // Old scene loading code by Tim: 
        
        // if (player.hasChanged)
        // {
        //     if (player.position.x > centrePoint.x + loadRange)
        //     {
        //         if (player.position.x > centrePoint.x + (loadRange))
        //         {
        //             SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //         }
        //
        //         if (!rightSceneLoaded)
        //         {
        //             if (rightScene != null)
        //             {
        //                 SceneManager.LoadScene(rightScene, LoadSceneMode.Additive);
        //                 rightSceneLoaded = true;
        //             }
        //         }
        //     }
        //
        //     if (player.position.x < centrePoint.x - loadRange)
        //     {
        //         if (player.position.x < centrePoint.x - (loadRange))
        //         {
        //             SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //         }
        //
        //         if (!leftSceneLoaded)
        //         {
        //             if (leftScene != null)
        //             {
        //                 SceneManager.LoadScene(leftScene, LoadSceneMode.Additive);
        //                 leftSceneLoaded = true;
        //             }
        //             
        //         }
        //     }
        //
        //     if (player.position.y > centrePoint.y + loadRange)
        //     {
        //         if (player.position.y > centrePoint.y + (loadRange * 2))
        //         {
        //             SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //         }
        //
        //         if (!downSceneLoaded)
        //         {
        //             if (downScene != null)
        //             {
        //                 SceneManager.LoadScene(downScene, LoadSceneMode.Additive);
        //                 downSceneLoaded = true;
        //             }
        //             
        //         }
        //     }
        //
        //     if (player.position.y < centrePoint.y - loadRange)
        //     {
        //         if (player.position.y < centrePoint.y - (loadRange * 2))
        //         {
        //             SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //         }
        //
        //         if (!upSceneLoaded)
        //         {
        //             if (upScene != null)
        //             {
        //                 SceneManager.LoadScene(upScene, LoadSceneMode.Additive);
        //                 upSceneLoaded = true;
        //             }
        //             
        //         }
        //     }
        //
        // }
    }
}
