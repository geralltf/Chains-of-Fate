using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtons : MonoBehaviour
    {
        public PlayableCharacter playableCharacter;

        public PlayerButtonsAttackSet AttackButtonSet;
        public PlayerButtonsResolveSet ResolveButtonsSet;
        
        public void AttackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            AttackButtonSet.gameObject.SetActive(true);
        }

        public void InventoryButton_OnClick()
        {
            Debug.Log("Inventory");
        }
        
        public void DefendButton_OnClick()
        {
            Debug.Log("Defend");
        }
        
        public void ResolveButton_OnClick()
        {
            this.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(true);
        }
        
        public void FleeButton_OnClick()
        {
            Debug.Log("Flee");
        }
        
        public void OnEnable()
        {
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
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