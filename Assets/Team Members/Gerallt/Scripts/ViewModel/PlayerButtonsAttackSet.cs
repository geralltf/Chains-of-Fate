using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsAttackSet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void SwordButton_OnClick()
        {
            Debug.Log("Sword attack"); //TODO: differentiate between different attacks, maybe pass in the equipped WeaponBase
            
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IAttackAction attackAction = (IAttackAction)currentCharacter;

            if (attackAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                attackAction.Attack(target); 
                
                combatGameManager.FinishedTurn(currentCharacter);
                combatGameManager.RaiseAttackEvent(currentCharacter, target);
            }
        }
        
        public void DaggerButton_OnClick()
        {
            Debug.Log("Dagger attack"); //TODO: differentiate between different attacks, maybe pass in the equipped WeaponBase
            
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IAttackAction attackAction = (IAttackAction)currentCharacter;

            if (attackAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                attackAction.Attack(target); 
                
                combatGameManager.FinishedTurn(currentCharacter);
                combatGameManager.RaiseAttackEvent(currentCharacter, target);
            }
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
        
        public void OnEnable()
        {

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