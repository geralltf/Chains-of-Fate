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
        //PlayerController = FindObjectOfType<PlayerController>();
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
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        PlayerController = playerController;
        
        if (playerController != null)
        {
            //this.gameObject.SetActive(false);
            GetComponent<EnemyMove>().enabled = false;
            
            SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    private void SceneManagerOnsceneLoaded(Scene sceneInstance, LoadSceneMode sceneMode)
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.Add(this.gameObject);

        List<GameObject> partyMembers = new List<GameObject>();

        SceneManager.SetActiveScene(sceneInstance);
        //SceneManager.MoveGameObjectToScene(other.gameObject,scene);
        GameObject[] rootObjects = sceneInstance.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                PlayerController.controls.Player.Disable();
                
                combatUI.isTestMode = false;
                combatUI.onSceneDestroyed += CombatUI_OnSceneDestroyed;
                combatUI.SetCurrentParty(enemies, partyMembers, PlayerController.gameObject);
                break;
            }
        }
        
        SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void CombatUI_OnSceneDestroyed(CombatUI combatUI)
    {
        combatUI.onSceneDestroyed -= CombatUI_OnSceneDestroyed;
        
        PlayerController.controls.Player.Enable();

        StartCoroutine(ResumeMovement());
    }

    IEnumerator ResumeMovement()
    {
        yield return new WaitForSeconds(GetComponent<EnemyNPC>().timeUntilMovementResumes);

        EnemyMove enemyMove = GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
