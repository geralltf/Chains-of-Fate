using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //public PlayerInput playerInput;
    public CoFPlayerControls controls;
    private Vector2 move;
    public float speed = 10f;
    private CombatUI CUI;
    private void Awake()
    {
        controls = new CoFPlayerControls();
        controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>(); //on button press gets the movement value and starts the movement
        controls.Player.Movement.canceled += ctx => move = Vector2.zero; //on button release stops movement
        //DontDestroyOnLoad(this);
        SceneManager.sceneLoaded+= SceneManagerOnsceneLoaded;
    }
 
    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    private void SceneManagerOnsceneLoaded(Scene sceneInstance, LoadSceneMode sceneMode)
    {
        
        GameObject[] rootObjects = sceneInstance.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                CUI = combatUI;
                if (CUI.isLoaded == false)
                {
                    CUI.onSceneDestroyed += CUIOnonSceneDestroyed;
                    CUI.isLoaded = true;
                }
                break;
            }
        }
    }

    private void CUIOnonSceneDestroyed()
    {
        controls.Player.Enable();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(move.x, 0.0f, move.y)*speed*Time.deltaTime; //gets a value based on actual seconds not pc specs that is used to calculate movement
        transform.Translate(movement,Space.World);
        
    }
}
