using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTeleporter : MonoBehaviour
{
    public string scene;

    private void Awake()
    {
        //SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void SceneManagerOnsceneLoaded(Scene sceneInstance, LoadSceneMode sceneMode)
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.Add(this.gameObject);
        SceneManager.SetActiveScene(sceneInstance);
        //SceneManager.MoveGameObjectToScene(other.gameObject,scene);
        GameObject[] rootObjects = sceneInstance.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                combatUI.SetCurrentEnemies(enemies);
            }
            
            break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
