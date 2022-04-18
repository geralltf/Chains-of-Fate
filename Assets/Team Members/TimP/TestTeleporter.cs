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
    private bool loadCombatScene = false; // Old way to load combat UI was to actually load the scene.
    public bool quickShowCombatScene = true;
    
    private PlayerController playerController;
    private bool collisionsDisabled = false;

    private void OnCollisionEnter(Collision other)
    {
        if (collisionsDisabled) return;
        
        PlayerController _playerController = other.gameObject.GetComponent<PlayerController>();

        if (_playerController != null)
        {
            playerController = _playerController;

            if (quickShowCombatScene || loadCombatScene)
            {
                collisionsDisabled = true;
            
                // Disable movement of enemy and player.
                GetComponent<EnemyMove>().enabled = false;
                GetComponent<TestTeleporter>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                playerController.GetComponent<Rigidbody>().isKinematic = true;
            }
            
            if (loadCombatScene) // Old approach to loading combat scene.
            {
                SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
            else if (quickShowCombatScene) // New approach to loading combat scene without using Scene Manager.
            {
                ChainsOfFate.Gerallt.GameManager.Instance.ShowCombatUI();

                CombatUI combatUI = ChainsOfFate.Gerallt.GameManager.Instance.combatUI;
                
                List<GameObject> enemies = new List<GameObject>();
                enemies.Add(this.gameObject);

                List<GameObject> partyMembers = new List<GameObject>();
                // TODO: add current party members from current player to this list
                
                playerController.controls.Player.Disable();
                
                combatUI.isTestMode = false;
                combatUI.onCloseCombatUI += CombatUI_OnCloseCombatUI;
                //combatUI.onSceneDestroyed += CombatUI_OnSceneDestroyed;
                combatUI.SetCurrentParty(enemies, partyMembers, playerController.gameObject);
            }
        }

        if (!quickShowCombatScene && !loadCombatScene)
        {
            // General teleporter function.
            // TODO: Need to test general teleporting
            SceneManager.sceneLoaded += SceneManager_GeneralTeleport_OnSceneLoaded;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    private void CombatUI_OnCloseCombatUI(CombatUI combatUI, bool hasWon)
    {
        combatUI.onCloseCombatUI -= CombatUI_OnCloseCombatUI;
        
        ChainsOfFate.Gerallt.GameManager.Instance.HideCombatUI();
        
        // Enable movement of enemy and player.
        playerController.controls.Player.Enable();

        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
        enemyRigidbody.isKinematic = false;

        Rigidbody playerRigidbody = playerController.GetComponent<Rigidbody>();
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.isKinematic = false;

        if (hasWon)
        {
            // Have won the combat with this enemy. So despawn the enemy.
            Destroy(gameObject);
        }

        StartCoroutine(ResumeFunctionAndMovement());
    }

    private void SceneManager_GeneralTeleport_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        SceneManager.sceneLoaded -= SceneManager_GeneralTeleport_OnSceneLoaded;
        
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(newScene);
    }
    
    private void SceneManagerOnsceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.Add(this.gameObject);

        List<GameObject> partyMembers = new List<GameObject>();
        // TODO: add current party members from current player to this list
        
        GameObject[] rootObjects = newScene.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                SceneManager.SetActiveScene(newScene);
                
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
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
