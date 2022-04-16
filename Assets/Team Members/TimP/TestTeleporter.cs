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
    
    private PlayerController playerController;
    private bool collisionsDisabled = false;

    private void OnCollisionEnter(Collision other)
    {
        if (collisionsDisabled) return;
        
        PlayerController _playerController = other.gameObject.GetComponent<PlayerController>();

        if (_playerController != null)
        {
            playerController = _playerController;
            collisionsDisabled = true;
            
            // Disable movement of enemy and player.
            GetComponent<EnemyMove>().enabled = false;
            GetComponent<TestTeleporter>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            playerController.GetComponent<Rigidbody>().isKinematic = true;

            SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    private void SceneManagerOnsceneLoaded(Scene sceneInstance, LoadSceneMode sceneMode)
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.Add(this.gameObject);

        List<GameObject> partyMembers = new List<GameObject>();
        // TODO: add current party members from current player to this list
        
        GameObject[] rootObjects = sceneInstance.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                SceneManager.SetActiveScene(sceneInstance);
                
                playerController.controls.Player.Disable();
                
                combatUI.isTestMode = false;
                combatUI.onSceneDestroyed += CombatUI_OnSceneDestroyed;
                combatUI.SetCurrentParty(enemies, partyMembers, playerController.gameObject);
                break;
            }
        }
        
        SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void CombatUI_OnSceneDestroyed(CombatUI combatUI)
    {
        combatUI.onSceneDestroyed -= CombatUI_OnSceneDestroyed;
        
        // Enable movement of enemy and player.
        playerController.controls.Player.Enable();
        
        GetComponent<Rigidbody>().isKinematic = false;
        playerController.GetComponent<Rigidbody>().isKinematic = false;
        
        StartCoroutine(ResumeFunctionAndMovement());
    }

    IEnumerator ResumeFunctionAndMovement()
    {
        yield return new WaitForSeconds(GetComponent<EnemyNPC>().timeUntilMovementResumes);
        
        TestTeleporter testTeleporter = GetComponent<TestTeleporter>();
        if (testTeleporter != null)
        {
            testTeleporter.enabled = true;
        }
        
        EnemyMove enemyMove = GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }

        collisionsDisabled = false;
    }
}
