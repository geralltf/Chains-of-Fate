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
    public PlayerController PlayerController;
    private void Awake()
    {
        PlayerController = FindObjectOfType<PlayerController>();
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.GetComponent<PlayerController>()!=null)
        {
            SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
        */
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerController>()!= null)
        {
            this.gameObject.SetActive(false);
            SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
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
                PlayerController.controls.Player.Disable();
                break;
            }
        }
        
        SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void OnTriggerExit(Collider other)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
