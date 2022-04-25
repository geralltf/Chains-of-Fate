using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WorldInfo : MonoBehaviour
{
    //public float loadRange = 50; // Dont need this anymore. Found dynamically from ground plane collider bounds extents.
    public Object leftScene, rightScene, upScene, downScene;
    public bool checkBoundaryCollisions = true;
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

    public enum SceneDirection
    {
        Undefined,
        Left,
        Right,
        Top,
        Bottom
    }
    
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
        centrePoint = transform.position;

        Tilemap tilemap = GetComponent<Tilemap>();
        
        // Fetch the Collider from the 2D tilemap ground plane
        Collider2D thisCollider = GetComponent<Collider2D>();
        Vector3 mapSize = thisCollider.bounds.size;
        //mapSize = tilemap.size;
        //mapSize = tilemap.CellToWorld(tilemap.size);
        
        centrePoint = thisCollider.bounds.center;
        
        //Vector3 mapSize = thisCollider.bounds.extents;
        
        // Get the map bounds for this scene.
        sceneBounds = new Bounds(centrePoint, mapSize);

        if (enableDebugColour)
        {
            GetComponent<MeshRenderer>().material.color = debugColour;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(sceneBounds.center, sceneBounds.size);
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
    

    public static string GetSceneName(Object sceneObj)
    {
        if (sceneObj == null)
        {
            return string.Empty;
        }
        
        SceneAsset sceneAsset = sceneObj as SceneAsset;
        ;
        if (sceneAsset == null)
        {
            throw new ArgumentException(
                "Object is meant to be of type 'SceneAsset'. Drag an actual Scene file using the inspector.");
        }
        
        return sceneAsset.name;
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
                sceneName = GetSceneName(leftScene);
                break;
            case SceneDirection.Right:
                sceneLoaded = rightSceneLoaded;
                sceneName = GetSceneName(rightScene);
                break;
            case SceneDirection.Top:
                sceneLoaded = upSceneLoaded;
                sceneName = GetSceneName(upScene);
                break;
            case SceneDirection.Bottom:
                sceneLoaded = downSceneLoaded;
                sceneName = GetSceneName(downScene);
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

                StartCoroutine(LoadSceneAsync(sceneName));
            }
        }
    }

    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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
                sceneName = GetSceneName(leftScene);
                dirBounds.x += -sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x < centrePoint.x - (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Right:
                sceneName = GetSceneName(rightScene);
                dirBounds.x += sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x > centrePoint.x + (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Top:
                sceneName = GetSceneName(upScene);
                dirBounds.y += sceneBounds.extents.y;
                boundaryLocation.y = (sceneBounds.center.y + dirBounds.y);
                dist = (pos.y - boundaryLocation.y);
                posDelta.y = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.y > centrePoint.y + (sceneBounds.extents.y - gameManager.boundaryRange));
                break;
            case SceneDirection.Bottom:
                sceneName = GetSceneName(downScene);
                dirBounds.y += -sceneBounds.extents.y;
                boundaryLocation.y = (sceneBounds.center.y + dirBounds.y);
                dist = (pos.y - boundaryLocation.y);
                posDelta.y = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.y < centrePoint.y - (sceneBounds.extents.y - gameManager.boundaryRange));
                break;
        }

        if (string.IsNullOrEmpty(sceneName) && dist < gameManager.boundaryMinDistance && boundsTest && checkBoundaryCollisions)
        {
            float oldZ = pos.z;
            pos.x += posDelta.x;
            pos.y += posDelta.y;
            pos.z = oldZ;
                
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
                loadOffset.y += sceneBounds.extents.y;
                approaching = SceneDirection.Bottom;
                break;
            case SceneDirection.Bottom:
                loadOffset.y += -sceneBounds.extents.y;
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
        if ((pos.y < centrePoint.y + sceneBounds.extents.y - gameManager.unloadRange) && approachingDirection == SceneDirection.Top)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Top scene
        if (pos.y > centrePoint.y + (sceneBounds.extents.y - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Top))
            {
                TryLoadScene(SceneDirection.Top);
            }
        }
        if (pos.y > centrePoint.y + sceneBounds.extents.y)
        {
            FinishLoading();
        }
    }
    
    void CheckBottom(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Bottom
        if ((pos.y > centrePoint.y - sceneBounds.extents.y + gameManager.unloadRange) && approachingDirection == SceneDirection.Bottom)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Bottom scene
        if (pos.y < centrePoint.y - (sceneBounds.extents.y - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Bottom))
            {
                TryLoadScene(SceneDirection.Bottom);
            }
        }
        if (pos.y < centrePoint.y - sceneBounds.extents.y)
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
    }
}
