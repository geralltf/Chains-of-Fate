using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class WorldInfo : MonoBehaviour
{
    //public float loadRange = 50; // Dont need this anymore. Found dynamically from ground plane collider bounds extents.
    public string leftScene, rightScene, upScene, downScene;
    
    private Transform player;
    private Vector3 centrePoint;
    private SceneDirection newSceneDirection;
    private Bounds sceneBounds;
    
    private bool leftSceneLoaded, rightSceneLoaded, upSceneLoaded, downSceneLoaded;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
        centrePoint = transform.position;

        // Fetch the Collider from the ground plane 
        Collider thisCollider = GetComponent<Collider>();
        Vector3 mapSize = thisCollider.bounds.size;

        // Get the map bounds for this scene.
        sceneBounds = new Bounds(centrePoint, mapSize);
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
        Left,
        Right,
        Top,
        Bottom
    }
    public void TryLoadScene(SceneDirection sceneDirection)
    {
        bool sceneLoaded = false;
        string sceneName = string.Empty;
        
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
                newSceneDirection = sceneDirection;
                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                
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
            }
            else
            {
                // Move the player back into the scene away from the out of bounds zone.
                Vector3 dirBounds = Vector3.zero;
                switch (newSceneDirection)
                {
                    case SceneDirection.Left:
                        dirBounds.x += -sceneBounds.extents.x;
                        break;
                    case SceneDirection.Right:
                        dirBounds.x += sceneBounds.extents.x;
                        break;
                    case SceneDirection.Top:
                        dirBounds.z += -sceneBounds.extents.z;
                        break;
                    case SceneDirection.Bottom:
                        dirBounds.z += sceneBounds.extents.z;
                        break;
                }
                
                Vector3 pos = player.position;

                Vector3 direction = -(player.position - (centrePoint + dirBounds)).normalized;

                pos += direction * 2.0f; // * Time.deltaTime;
                
                player.position = pos;
            }
        }
    }

    private void SceneManagerOnsceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;

        // Get the offset from the current map to apply to the new map.
        Vector3 loadOffset = Vector3.zero;
        switch (newSceneDirection)
        {
            case SceneDirection.Left:
                loadOffset.x += -sceneBounds.extents.x;
                break;
            case SceneDirection.Right:
                loadOffset.x += sceneBounds.extents.x;
                break;
            case SceneDirection.Top:
                loadOffset.z += -sceneBounds.extents.z;
                break;
            case SceneDirection.Bottom:
                loadOffset.z += sceneBounds.extents.z;
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
                break;
            }

            if (rootTransform != null)
            {
                rootTransform.position = sceneBounds.center + loadOffset;
            }
        }
        
        // Unload old scene since the new scene has loaded.
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    void CheckPosition()
    {
        if (player.hasChanged)
        {
            Vector3 pos = player.position;

            // Left scene
            if (pos.x < centrePoint.x - sceneBounds.extents.x)
            {
                TryLoadScene(SceneDirection.Left);
            }
            
            // Right scene
            if (pos.x > centrePoint.x + sceneBounds.extents.x)
            {
                TryLoadScene(SceneDirection.Right);
            }
            
            // Top scene
            if (pos.z < centrePoint.z - sceneBounds.extents.z)
            {
                TryLoadScene(SceneDirection.Top);
            }
            
            // Bottom scene
            if (pos.z > centrePoint.z + sceneBounds.extents.z)
            {
                TryLoadScene(SceneDirection.Bottom);
            }
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
