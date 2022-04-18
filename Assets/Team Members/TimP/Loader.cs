using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public string mainScene = "Main";
    public string managersScene = "Managers Scene";
    public bool loadMainScene = false;

    private Scene thisScene;
    
    private void Awake()
    {
        thisScene = SceneManager.GetActiveScene();
        
        DontDestroyOnLoad(FindObjectOfType<PlayerController>().gameObject);
        DontDestroyOnLoad(FindObjectOfType<CameraFollow>().gameObject);

        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

        if (gameManager == null)
        {
            SceneManager.LoadScene(managersScene, LoadSceneMode.Additive);
        }

        if (loadMainScene)
        {
            SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
            SceneManager.LoadScene(mainScene, LoadSceneMode.Additive);
        }
    }

    private void SceneManager_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        if (newScene.name == mainScene)
        {
            SceneManager.UnloadSceneAsync(thisScene);
            SceneManager.SetActiveScene(newScene);
            
            SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        }
    }
}
