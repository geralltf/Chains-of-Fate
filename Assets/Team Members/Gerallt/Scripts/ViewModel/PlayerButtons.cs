using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtons : MonoBehaviour
    {
        public PlayableCharacter playableCharacter;

        public PlayerButtonsAttackSet AttackButtonSet;
        public PlayerButtonsResolveSet ResolveButtonsSet;
        public PlayerButtonsInventorySet InventoryButtonsSet;
        public PlayerButtonsDefensiveSet DefensiveButtonsSet;
        
        public void AttackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            AttackButtonSet.gameObject.SetActive(true);
        }

        public void InventoryButton_OnClick()
        {
            this.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(true);
        }
        
        public void DefendButton_OnClick()
        {
            this.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(true);
        }
        
        public void ResolveButton_OnClick()
        {
            this.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(true);
        }
        
        public void FleeButton_OnClick()
        {
            Debug.Log("Flee");

            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
        
        public void OnEnable()
        {
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }

        public void OnDisable()
        {

        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}